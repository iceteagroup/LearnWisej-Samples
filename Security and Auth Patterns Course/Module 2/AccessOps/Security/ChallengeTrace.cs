using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace AccessOps.Security
{
    /// <summary>
    /// The redacted challenge trace (lab deliverable): one log line per request that takes part in
    /// the Windows handshake - a request carrying an <c>Authorization</c> header, or a response
    /// carrying <c>WWW-Authenticate</c>. It records the method, path, the request scheme, the
    /// status and the offered schemes. It never records a header value: an <c>Authorization</c>
    /// value is a credential and a <c>WWW-Authenticate</c> continuation carries a token, so both
    /// are written as "<c>Negotiate [redacted]</c>".
    /// </summary>
    public sealed class ChallengeTrace
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ChallengeTrace> _logger;

        /// <summary>Creates the middleware.</summary>
        public ChallengeTrace(RequestDelegate next, ILogger<ChallengeTrace> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>Runs the rest of the pipeline, then writes the trace line.</summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var authorization = SchemeOnly(context.Request.Headers.Authorization);

            await _next(context);

            var offered = context.Response.Headers.WWWAuthenticate;
            if (authorization == null && StringValues.IsNullOrEmpty(offered))
                return;

            var identity = context.User?.Identity;
            _logger.LogInformation(
                "challenge {Method} {Path} request={Authorization} -> {Status} offered={Offered} identity={Identity}",
                context.Request.Method,
                context.Request.Path.Value,
                authorization ?? "(no Authorization header)",
                context.Response.StatusCode,
                StringValues.IsNullOrEmpty(offered) ? "-" : string.Join(", ", offered.Select(SchemeOnly)),
                identity?.IsAuthenticated == true ? $"{identity.AuthenticationType} · {WindowsName.Redact(identity.Name)}" : "anonymous");
        }

        /// <summary>"Negotiate", or "Negotiate [redacted]" when the header carries a token.</summary>
        public static string SchemeOnly(string header)
        {
            if (string.IsNullOrWhiteSpace(header))
                return null;
            header = header.Trim();
            var space = header.IndexOf(' ');
            return space < 0 ? header : header[..space] + " [redacted]";
        }
    }

    /// <summary>Masks a Windows account name the way the walkthrough does ("CONTOSO\re•••1").</summary>
    public static class WindowsName
    {
        /// <summary>
        /// Keeps the domain (or machine) name, the first two characters and the last character of
        /// the user name: <c>CONTOSO\reader1</c> becomes <c>CONTOSO\re•••1</c>.
        /// </summary>
        public static string Redact(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "(none)";
            var slash = name.IndexOf('\\');
            var domain = slash < 0 ? "" : name[..(slash + 1)];
            var user = slash < 0 ? name : name[(slash + 1)..];
            if (user.Length <= 3)
                return domain + new string('•', user.Length);
            return domain + user[..2] + "•••" + user[^1];
        }
    }
}
