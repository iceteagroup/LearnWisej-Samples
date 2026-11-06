// ============================================================================================
//  TEST DOUBLE - NEVER DEPLOY.
//  "Contoso Identity": a minimal OpenID Connect provider for the AccessOps lab. It implements
//  just enough of the authorization code flow with PKCE for the lab to run on localhost
//  without an external account: discovery, JWKS, authorize (sign-in page + a simulated
//  second factor), token (authorization_code and refresh_token), userinfo, revocation,
//  end-session, and one protected "HR directory" API that AccessOps calls with the access
//  token. Signing keys are generated at start-up and live in memory only.
// ============================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (!app.Environment.IsDevelopment())
    throw new InvalidOperationException("The AccessOps test identity provider is a test double and only runs in the Development environment.");

var issuer = app.Configuration["Issuer"].TrimEnd('/');
var clients = app.Configuration.GetSection("Clients").GetChildren().ToDictionary(
    c => c.Key,
    c => new Client(
        c.Key,
        c["Secret"] ?? throw new InvalidOperationException($"No secret configured for client '{c.Key}'. Set Clients__{c.Key}__Secret in the environment."),
        c.GetSection("RedirectUris").Get<string[]>() ?? Array.Empty<string>(),
        c.GetSection("PostLogoutRedirectUris").Get<string[]>() ?? Array.Empty<string>()));
var users = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "users.json")))
    .RootElement.GetProperty("users").EnumerateArray()
    .Select(u => new User(
        u.GetProperty("subject").GetString(), u.GetProperty("email").GetString(), u.GetProperty("password").GetString(),
        u.GetProperty("givenName").GetString(), u.GetProperty("familyName").GetString(),
        u.GetProperty("jobTitle").GetString(), u.GetProperty("department").GetString()))
    .ToList();

var rsa = RSA.Create(2048);
var keyId = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(8));
var pending = new ConcurrentDictionary<string, AuthorizeRequest>();      // sign-in pages in progress
var codes = new ConcurrentDictionary<string, IssuedCode>();             // one-time authorization codes
var accessTokens = new ConcurrentDictionary<string, IssuedToken>();
var refreshTokens = new ConcurrentDictionary<string, IssuedToken>();
var idpSessions = new ConcurrentDictionary<string, string>();          // IdP session cookie -> subject
const string SessionCookie = "contoso.idp.session";
var log = app.Logger;
var accessTokenLifetime = TimeSpan.FromSeconds(app.Configuration.GetValue("AccessTokenLifetimeSeconds", 300));

log.LogWarning("TEST IDENTITY PROVIDER - NEVER DEPLOY. Issuer {Issuer}, {Users} demo users.", issuer, users.Count);

// ---- discovery and keys ----------------------------------------------------------------------

app.MapGet("/.well-known/openid-configuration", () => Results.Json(new Dictionary<string, object>
{
    ["issuer"] = issuer,
    ["authorization_endpoint"] = issuer + "/connect/authorize",
    ["token_endpoint"] = issuer + "/connect/token",
    ["userinfo_endpoint"] = issuer + "/connect/userinfo",
    ["jwks_uri"] = issuer + "/.well-known/jwks.json",
    ["revocation_endpoint"] = issuer + "/connect/revoke",
    ["end_session_endpoint"] = issuer + "/connect/endsession",
    ["response_types_supported"] = new[] { "code" },
    ["response_modes_supported"] = new[] { "query" },
    ["grant_types_supported"] = new[] { "authorization_code", "refresh_token" },
    ["subject_types_supported"] = new[] { "public" },
    ["id_token_signing_alg_values_supported"] = new[] { "RS256" },
    ["code_challenge_methods_supported"] = new[] { "S256" },
    ["token_endpoint_auth_methods_supported"] = new[] { "client_secret_post", "client_secret_basic" },
    ["scopes_supported"] = new[] { "openid", "profile", "email", "offline_access", "hr.read" },
    ["claims_supported"] = new[] { "sub", "iss", "aud", "exp", "iat", "nonce", "auth_time", "email", "email_verified", "name", "given_name", "family_name" }
}));

app.MapGet("/.well-known/jwks.json", () =>
{
    var p = rsa.ExportParameters(false);
    return Results.Json(new
    {
        keys = new[]
        {
            new { kty = "RSA", use = "sig", alg = "RS256", kid = keyId, n = WebEncoders.Base64UrlEncode(p.Modulus), e = WebEncoders.Base64UrlEncode(p.Exponent) }
        }
    });
});

// ---- authorize: validate, sign in (or reuse the IdP session), second factor, code ------------

app.MapGet("/connect/authorize", (HttpContext ctx) =>
{
    var q = ctx.Request.Query;
    if (!clients.TryGetValue(q["client_id"].ToString(), out var client))
        return Page("Unknown application", "<p>The application that sent you here is not registered.</p>", 400);

    var redirectUri = q["redirect_uri"].ToString();
    if (!client.RedirectUris.Contains(redirectUri, StringComparer.Ordinal))
        return Page("Redirect URI not registered", "<p>The redirect URI does not match a registered value exactly.</p>", 400);

    // from here on errors go back to the client
    var state = q["state"].ToString();
    if (q["response_type"] != "code")
        return Results.Redirect(Error(redirectUri, "unsupported_response_type", state));
    if (q["code_challenge_method"] != "S256" || string.IsNullOrEmpty(q["code_challenge"]))
        return Results.Redirect(Error(redirectUri, "invalid_request", state));   // PKCE S256 is mandatory here

    var request = new AuthorizeRequest(
        Id: Random(24), Client: client, RedirectUri: redirectUri, State: state, Nonce: q["nonce"].ToString(),
        Scopes: q["scope"].ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries),
        CodeChallenge: q["code_challenge"].ToString(), Prompt: q["prompt"].ToString(), CreatedUtc: DateTime.UtcNow);

    // single sign-on: an open IdP session in this browser completes silently
    if (request.Prompt != "login"
        && ctx.Request.Cookies.TryGetValue(SessionCookie, out var sid)
        && idpSessions.TryGetValue(sid, out var subject))
    {
        log.LogInformation("authorize: IdP session reused for {Subject} - no credentials asked", subject);
        return Results.Redirect(IssueCode(request, users.Single(u => u.Subject == subject)));
    }

    pending[request.Id] = request;
    return SignInPage(request, null);
});

app.MapPost("/connect/login", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    if (!pending.TryGetValue(form["request"].ToString(), out var request) || DateTime.UtcNow - request.CreatedUtc > TimeSpan.FromMinutes(10))
        return Page("Sign-in expired", "<p>This sign-in page has expired. Go back to the application and start again.</p>", 400);

    var user = users.FirstOrDefault(u => string.Equals(u.Email, form["email"].ToString().Trim(), StringComparison.OrdinalIgnoreCase));
    var ok = user != null && CryptographicOperations.FixedTimeEquals(
        Encoding.UTF8.GetBytes(user.Password), Encoding.UTF8.GetBytes(form["password"].ToString()));
    if (!ok)
        return SignInPage(request, "That email and password combination is not valid.");

    // simulated second factor: the test double asks the user to approve on "their phone"
    return Page("Approve the sign-in", $@"
<div class=""mfa""><span class=""n"">2</span><div><b>Approve the sign-in on your phone</b>
<div class=""sub"">Multi-factor policy is administered here, not in AccessOps.</div></div></div>
<form method=""post"" action=""/connect/approve"">
<input type=""hidden"" name=""request"" value=""{Enc(request.Id)}"" />
<input type=""hidden"" name=""subject"" value=""{Enc(user.Subject)}"" />
<button type=""submit"">Approve</button></form>");
});

app.MapPost("/connect/approve", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    if (!pending.TryRemove(form["request"].ToString(), out var request))
        return Page("Sign-in expired", "<p>This sign-in page has expired. Go back to the application and start again.</p>", 400);

    var user = users.Single(u => u.Subject == form["subject"].ToString());
    var sid = Random(24);
    idpSessions[sid] = user.Subject;
    ctx.Response.Cookies.Append(SessionCookie, sid, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Lax, Path = "/" });
    log.LogInformation("authorize: {Subject} signed in at the IdP", user.Subject);
    return Results.Redirect(IssueCode(request, user));
});

// ---- token -----------------------------------------------------------------------------------

app.MapPost("/connect/token", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var client = AuthenticateClient(ctx, form);
    if (client == null)
        return TokenError("invalid_client", 401);

    switch (form["grant_type"].ToString())
    {
        case "authorization_code":
        {
            if (!codes.TryRemove(form["code"].ToString(), out var code))                       // one time only
                return TokenError("invalid_grant");
            if (code.ExpiresUtc < DateTime.UtcNow || code.Request.Client.Id != client.Id
                || code.Request.RedirectUri != form["redirect_uri"].ToString())
                return TokenError("invalid_grant");
            var verifier = form["code_verifier"].ToString();
            var challenge = WebEncoders.Base64UrlEncode(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
            if (!CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(challenge), Encoding.ASCII.GetBytes(code.Request.CodeChallenge)))
            {
                log.LogWarning("token: PKCE verifier does not match the challenge");
                return TokenError("invalid_grant");
            }
            log.LogInformation("token: code redeemed for {Subject} (PKCE S256 verified)", code.User.Subject);
            return Tokens(client, code.User, code.Request.Scopes, code.Request.Nonce, includeIdToken: true);
        }
        case "refresh_token":
        {
            if (!refreshTokens.TryRemove(form["refresh_token"].ToString(), out var rt) || rt.ExpiresUtc < DateTime.UtcNow || rt.ClientId != client.Id)
                return TokenError("invalid_grant");
            log.LogInformation("token: refresh for {Subject} (refresh token rotated)", rt.User.Subject);
            return Tokens(client, rt.User, rt.Scopes, null, includeIdToken: false);
        }
        default:
            return TokenError("unsupported_grant_type");
    }
});

app.MapGet("/connect/userinfo", (HttpContext ctx) =>
{
    var token = Bearer(ctx);
    if (token == null)
        return Results.Unauthorized();
    return Results.Json(UserClaims(token.User));
});

app.MapPost("/connect/revoke", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    if (AuthenticateClient(ctx, form) == null)
        return Results.Unauthorized();
    var value = form["token"].ToString();
    var removed = accessTokens.TryRemove(value, out _) | refreshTokens.TryRemove(value, out _);
    log.LogInformation("revoke: token {Outcome}", removed ? "revoked" : "unknown");
    return Results.Ok();
});

app.MapGet("/connect/endsession", (HttpContext ctx) =>
{
    if (ctx.Request.Cookies.TryGetValue(SessionCookie, out var sid) && idpSessions.TryRemove(sid, out var subject))
    {
        log.LogInformation("endsession: IdP session of {Subject} ended", subject);
        foreach (var t in refreshTokens.Where(t => t.Value.User.Subject == subject).Select(t => t.Key).ToList())
            refreshTokens.TryRemove(t, out _);
    }
    ctx.Response.Cookies.Delete(SessionCookie);

    var target = ctx.Request.Query["post_logout_redirect_uri"].ToString();
    if (clients.Values.Any(c => c.PostLogoutRedirectUris.Contains(target, StringComparer.Ordinal)))
    {
        var state = ctx.Request.Query["state"].ToString();
        return Results.Redirect(string.IsNullOrEmpty(state) ? target : QueryHelpers.AddQueryString(target, "state", state));
    }
    return Page("Signed out", "<p>You have been signed out of Contoso Identity.</p>");
});

// ---- the downstream API AccessOps calls with the access token ----------------------------------

app.MapGet("/api/hr/profile", (HttpContext ctx) =>
{
    var token = Bearer(ctx);
    if (token == null)
        return Results.Unauthorized();
    return Results.Json(new { jobTitle = token.User.JobTitle, department = token.User.Department });
});

app.Run();

// ---- helpers -----------------------------------------------------------------------------------

string IssueCode(AuthorizeRequest request, User user)
{
    var code = Random(32);
    codes[code] = new IssuedCode(request, user, DateTime.UtcNow.AddSeconds(60));
    var target = QueryHelpers.AddQueryString(request.RedirectUri, new Dictionary<string, string>
    {
        ["code"] = code,
        ["state"] = request.State,
        ["iss"] = issuer
    });
    return target;
}

IResult Tokens(Client client, User user, string[] scopes, string nonce, bool includeIdToken)
{
    var now = DateTimeOffset.UtcNow;
    var access = Random(32);
    accessTokens[access] = new IssuedToken(user, client.Id, scopes, now.Add(accessTokenLifetime).UtcDateTime);
    var body = new Dictionary<string, object>
    {
        ["access_token"] = access,
        ["token_type"] = "Bearer",
        ["expires_in"] = (int)accessTokenLifetime.TotalSeconds,
        ["scope"] = string.Join(' ', scopes)
    };
    if (scopes.Contains("offline_access"))
    {
        var refresh = Random(32);
        refreshTokens[refresh] = new IssuedToken(user, client.Id, scopes, now.AddHours(8).UtcDateTime);
        body["refresh_token"] = refresh;
    }
    if (includeIdToken)
    {
        var claims = UserClaims(user);
        claims["iss"] = issuer;
        claims["aud"] = client.Id;
        claims["iat"] = now.ToUnixTimeSeconds();
        claims["exp"] = now.AddMinutes(5).ToUnixTimeSeconds();
        claims["auth_time"] = now.ToUnixTimeSeconds();
        if (!string.IsNullOrEmpty(nonce))
            claims["nonce"] = nonce;
        body["id_token"] = Jwt(claims);
    }
    return Results.Json(body, statusCode: 200);
}

Dictionary<string, object> UserClaims(User user) => new Dictionary<string, object>
{
    ["sub"] = user.Subject,
    ["email"] = user.Email,
    ["email_verified"] = true,
    ["name"] = user.GivenName + " " + user.FamilyName,
    ["given_name"] = user.GivenName,
    ["family_name"] = user.FamilyName
};

string Jwt(Dictionary<string, object> payload)
{
    var header = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new { alg = "RS256", typ = "JWT", kid = keyId }));
    var body = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
    var signature = rsa.SignData(Encoding.ASCII.GetBytes(header + "." + body), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    return header + "." + body + "." + WebEncoders.Base64UrlEncode(signature);
}

Client AuthenticateClient(HttpContext ctx, IFormCollection form)
{
    string id = form["client_id"], secret = form["client_secret"];
    var auth = ctx.Request.Headers.Authorization.ToString();
    if (auth.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        var pair = Encoding.UTF8.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':', 2);
        id = Uri.UnescapeDataString(pair[0]);
        secret = Uri.UnescapeDataString(pair.Length > 1 ? pair[1] : "");
    }
    return id != null && clients.TryGetValue(id, out var client)
        && CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(client.Secret), Encoding.UTF8.GetBytes(secret ?? ""))
        ? client : null;
}

IssuedToken Bearer(HttpContext ctx)
{
    var auth = ctx.Request.Headers.Authorization.ToString();
    if (!auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        return null;
    return accessTokens.TryGetValue(auth.Substring(7), out var t) && t.ExpiresUtc > DateTime.UtcNow ? t : null;
}

IResult TokenError(string error, int status = 400) => Results.Json(new { error }, statusCode: status);

string Error(string redirectUri, string error, string state)
    => QueryHelpers.AddQueryString(redirectUri, new Dictionary<string, string> { ["error"] = error, ["state"] = state });

IResult SignInPage(AuthorizeRequest request, string error)
{
    var chips = string.Concat(request.Scopes.Select(s => $"<span class=\"chip\">{Enc(s)}</span>"));
    var message = error == null ? "" : $"<div class=\"err\">{Enc(error)}</div>";
    return Page("Sign in", $@"
<div class=""sub"">AccessOps is requesting</div><div class=""chips"">{chips}</div>{message}
<form method=""post"" action=""/connect/login"">
<input type=""hidden"" name=""request"" value=""{Enc(request.Id)}"" />
<input name=""email"" type=""email"" placeholder=""name@contoso.example"" autocomplete=""username"" required />
<input name=""password"" type=""password"" placeholder=""Password"" autocomplete=""current-password"" required />
<button type=""submit"">Continue</button></form>
<div class=""foot"">The password is typed on this page. AccessOps receives a code, never a credential.</div>");
}

IResult Page(string title, string body, int status = 200) => Results.Content($@"<!DOCTYPE html>
<html><head><meta charset=""utf-8"" /><title>Contoso Identity · {Enc(title)}</title>
<style>
body{{margin:0;font-family:Segoe UI,Arial,sans-serif;background:#f3f1fa}}
.bar{{height:40px;background:#4c2fa8;color:#fff;font-weight:700;font-size:14.5px;display:flex;align-items:center;padding:0 18px}}
.bar span{{font-weight:500;opacity:.85;font-size:13px;margin-left:9px}}
.card{{max-width:520px;margin:40px auto;background:#fff;border:1px solid #6d4bd0;border-radius:9px;overflow:hidden}}
.in{{padding:24px 28px}} .sub{{font-size:13px;color:#5a6b7d}}
.chips{{display:flex;gap:8px;margin-top:9px}} .chip{{padding:5px 12px;border-radius:999px;font-size:12px;font-weight:700;background:#f2edff;border:1px solid #d6c8f5;color:#4c2fa8;font-family:Consolas,monospace}}
input{{display:block;width:100%;box-sizing:border-box;margin-top:12px;padding:10px 14px;border:1px solid #cdd9e6;border-radius:7px;font-size:13px}}
button{{margin-top:14px;width:100%;padding:11px 18px;border:0;border-radius:8px;background:#4c2fa8;color:#fff;font-size:14px;font-weight:700;cursor:pointer}}
.err{{margin-top:14px;padding:10px 14px;background:#fdecea;border-left:4px solid #c0392b;color:#7d2018;font-size:13px;border-radius:6px}}
.mfa{{display:flex;gap:14px;align-items:center;padding:18px 20px;border-radius:10px;background:#f7f4ff;border:1px solid #d6c8f5;color:#2c1a63}}
.mfa .n{{width:34px;height:34px;border-radius:9px;background:#4c2fa8;color:#fff;display:flex;align-items:center;justify-content:center;font-weight:800}}
.foot{{margin-top:18px;font-size:12px;color:#8a9aab}}
.test{{text-align:center;font-size:11px;color:#7a5410;margin-top:-28px}}
</style></head><body>
<div class=""bar"">Contoso Identity<span>· secure sign-in</span></div>
<div class=""card""><div class=""in"">{body}</div></div>
<div class=""test"">Test identity provider for the AccessOps lab · never deploy</div>
</body></html>", "text/html; charset=utf-8", Encoding.UTF8, status);

static string Random(int bytes) => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(bytes));

static string Enc(string value) => HtmlEncoder.Default.Encode(value ?? "");

record Client(string Id, string Secret, string[] RedirectUris, string[] PostLogoutRedirectUris);
record User(string Subject, string Email, string Password, string GivenName, string FamilyName, string JobTitle, string Department);
record AuthorizeRequest(string Id, Client Client, string RedirectUri, string State, string Nonce, string[] Scopes, string CodeChallenge, string Prompt, DateTime CreatedUtc);
record IssuedCode(AuthorizeRequest Request, User User, DateTime ExpiresUtc);
record IssuedToken(User User, string ClientId, string[] Scopes, DateTime ExpiresUtc);
