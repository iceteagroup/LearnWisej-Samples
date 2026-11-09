using System;
using System.Linq;
using System.Security.Claims;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Wisej.Ext.WebAuthn;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// Module 6: the five lab outcomes and the store races, with assertions signed by a software
    /// authenticator and checked by the extension's real <c>WebAuthn.Validate</c>.
    /// </summary>
    public class PasskeyTests
    {
        private const string Origin = "https://localhost:6406";

        /// <summary>Stores a credential for <paramref name="accountId"/> exactly as enrollment would.</summary>
        private static PasskeyCredential Enroll(TestWorld w, string accountId, SyntheticAuthenticator key, string label = "Work laptop · Windows Hello")
        {
            var credential = new PasskeyCredential
            {
                RowId = Guid.NewGuid().ToString("N"),
                RpId = "localhost",
                CredentialId = key.CredentialId,
                AccountId = accountId,
                UserHandle = w.Passkeys.UserHandleFor(accountId),
                PublicKeyJson = JSON.Stringify(key.PublicKey),
                Algorithm = (int)key.Algorithm,
                SignCount = 0,
                Label = label
            };
            Assert.True(w.Passkeys.Add(credential));
            return w.Passkeys.Find("localhost", key.CredentialId);
        }

        private static byte[] Bytes(PasskeyChallenge c) => Convert.FromBase64String(c.Base64);

        // ---- the five outcomes ---------------------------------------------------------------

        [Theory]
        [InlineData(COSEAlgorithmIdentifier.ES256)]
        [InlineData(COSEAlgorithmIdentifier.RS256)]
        public void Outcome1_a_valid_passkey_signs_account_A_in_with_one_grant(COSEAlgorithmIdentifier algorithm)
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator(algorithm);
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);

            var ticket = w.PasskeyService.BeginSignIn();
            var response = key.Get(Bytes(ticket), Origin, stored.UserHandle, counter: 1);
            var result = w.PasskeyService.CompleteSignIn(ticket.Id, response);

            Assert.Equal(PasskeyOutcome.Accepted, result.Outcome);
            Assert.Equal(AccessOpsDatabase.DanaAccountId, result.SignIn.AccountId);
            Assert.Equal("passkey", result.SignIn.Method);
            Assert.Equal(1, w.Grants.CountAll());
            var after = w.Passkeys.Find("localhost", key.CredentialId);
            Assert.Equal(1, after.SignCount);
            Assert.Equal(stored.Version + 1, after.Version);
            Assert.NotNull(after.LastUsedAt);
        }

        [Fact]
        public void Outcome2_account_As_credential_presented_for_account_B_finds_no_key()
        {
            using var w = new TestWorld();
            using var keyA = new SyntheticAuthenticator();
            using var keyB = new SyntheticAuthenticator();
            Enroll(w, AccessOpsDatabase.DanaAccountId, keyA);
            var b = Enroll(w, AccessOpsDatabase.JoAccountId, keyB, "Account B laptop");

            // A's authenticator signs, but the response names account B's user handle (fixture F09)
            var ticket = w.PasskeyService.BeginSignIn();
            var result = w.PasskeyService.CompleteSignIn(ticket.Id, keyA.Get(Bytes(ticket), Origin, b.UserHandle, counter: 1));

            Assert.Equal(PasskeyOutcome.NoMatchingKey, result.Outcome);
            Assert.Equal(0, w.Grants.CountAll());
            Assert.Equal(0, w.Passkeys.Find("localhost", keyB.CredentialId).SignCount);   // no commit
        }

        [Fact]
        public void Outcome3_an_unapproved_origin_is_refused_even_with_a_valid_signature()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);

            var ticket = w.PasskeyService.BeginSignIn();
            var result = w.PasskeyService.CompleteSignIn(ticket.Id, key.Get(Bytes(ticket), "https://localhost.evil.example", stored.UserHandle, counter: 1));

            Assert.Equal(PasskeyOutcome.OriginRejected, result.Outcome);
            Assert.Equal(0, w.Grants.CountAll());
        }

        [Fact]
        public void Outcome4_a_replayed_response_is_refused_because_the_challenge_was_consumed()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);

            var ticket = w.PasskeyService.BeginSignIn();
            var response = key.Get(Bytes(ticket), Origin, stored.UserHandle, counter: 5);
            Assert.True(w.PasskeyService.CompleteSignIn(ticket.Id, response).Succeeded);

            // the same signed response again: WebAuthn.Validate would still return true
            Assert.True(WebAuthn.Validate(stored.Key, response.AuthenticatorData.Base64, response.ClientData.Base64, response.Signature));
            var replay = w.PasskeyService.CompleteSignIn(ticket.Id, response);
            Assert.Equal(PasskeyOutcome.ChallengeRejected, replay.Outcome);
            Assert.Equal(1, w.Grants.CountAll());
        }

        [Fact]
        public void Outcome5_a_cancelled_prompt_burns_the_challenge_and_creates_no_grant()
        {
            using var w = new TestWorld();
            var ticket = w.PasskeyService.BeginSignIn();
            var result = w.PasskeyService.BrowserFailed(ticket, PasskeyService.SignInType);

            Assert.Equal(PasskeyOutcome.BrowserFailed, result.Outcome);
            Assert.False(w.Challenges.TryConsume(ticket.Id, PasskeyService.SignInType, Bytes(ticket)));
            Assert.Equal(0, w.Grants.CountAll());
            Assert.Equal("That passkey could not sign you in. Try again or use another sign-in method.", PasskeyResult.SignInRefused);
        }

        // ---- the other assertion checks ------------------------------------------------------

        [Theory]
        [InlineData("rpid")]
        [InlineData("type")]
        [InlineData("presence")]
        [InlineData("verification")]
        [InlineData("tampered")]
        [InlineData("challenge")]
        public void Every_other_check_refuses_without_a_grant(string broken)
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);
            var ticket = w.PasskeyService.BeginSignIn();
            var challenge = broken == "challenge" ? new byte[32] : Bytes(ticket);

            var response = key.Get(challenge, Origin, stored.UserHandle, counter: 1,
                rpId: broken == "rpid" ? "accessops.example" : "localhost",
                userPresent: broken != "presence",
                userVerified: broken != "verification",
                type: broken == "type" ? "webauthn.create" : "webauthn.get");
            if (broken == "tampered")
                response.Signature[^1] ^= 0x01;

            var result = w.PasskeyService.CompleteSignIn(ticket.Id, response);
            var expected = broken switch
            {
                "rpid" => PasskeyOutcome.RpIdMismatch,
                "type" => PasskeyOutcome.WrongType,
                "presence" => PasskeyOutcome.UserNotPresent,
                "verification" => PasskeyOutcome.UserNotVerified,
                "tampered" => PasskeyOutcome.NoMatchingKey,
                _ => PasskeyOutcome.ChallengeRejected
            };
            Assert.Equal(expected, result.Outcome);
            Assert.Equal(0, w.Grants.CountAll());
        }

        [Fact]
        public void Short_ES256_components_validate_after_re_encoding()
        {
            // 2,000 genuine signatures: WebAuthn.Validate alone rejects the ones whose r or s is shorter
            // than 32 bytes; re-encoded by Es256Signature every one of them verifies
            using var key = new SyntheticAuthenticator();
            var auth = new byte[37];
            var clientData = System.Text.Encoding.UTF8.GetBytes("{\"type\":\"webauthn.get\"}");
            var signed = auth.Concat(System.Security.Cryptography.SHA256.HashData(clientData)).ToArray();
            int rawFailures = 0, fixedFailures = 0;
            for (int i = 0; i < 2000; i++)
            {
                var der = key.SignRaw(signed);
                if (!WebAuthn.Validate(key.PublicKey, Convert.ToBase64String(auth), Convert.ToBase64String(clientData), der))
                    rawFailures++;
                if (!WebAuthn.Validate(key.PublicKey, Convert.ToBase64String(auth), Convert.ToBase64String(clientData), Es256Signature.ForValidate(der)))
                    fixedFailures++;
            }
            Assert.Equal(0, fixedFailures);
            Assert.True(rawFailures > 0, "expected some short components in 2,000 signatures");
        }

        [Fact]
        public void User_verification_is_read_from_the_signed_flags_byte_not_the_extension_property()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);

            // what the 4.1.7 client hands over for a verified user: UV bit (0x04) set in the signed
            // authenticator data, but UserVerified = false because it reads bit 1
            var ticket = w.PasskeyService.BeginSignIn();
            var response = key.Get(Bytes(ticket), Origin, stored.UserHandle, counter: 1, userVerified: true);
            response.AuthenticatorData.UserVerified = false;
            Assert.True(w.PasskeyService.CompleteSignIn(ticket.Id, response).Succeeded);

            // and the reverse: the property says true, the signed byte says no verification
            var second = w.PasskeyService.BeginSignIn();
            var unverified = key.Get(Bytes(second), Origin, stored.UserHandle, counter: 2, userVerified: false);
            unverified.AuthenticatorData.UserVerified = true;
            Assert.Equal(PasskeyOutcome.UserNotVerified, w.PasskeyService.CompleteSignIn(second.Id, unverified).Outcome);
        }

        [Theory]
        [InlineData(0, 0, true)]      // many authenticators always report 0
        [InlineData(0, 1, true)]
        [InlineData(7, 8, true)]
        [InlineData(7, 7, false)]     // non-zero and not increasing: anomaly
        [InlineData(7, 3, false)]
        [InlineData(7, 0, false)]
        public void Counter_policy(long stored, long received, bool accepted)
            => Assert.Equal(accepted, CounterPolicy.Accepts(stored, received));

        [Fact]
        public void A_counter_that_goes_backwards_is_refused_as_an_anomaly()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);
            var first = w.PasskeyService.BeginSignIn();
            Assert.True(w.PasskeyService.CompleteSignIn(first.Id, key.Get(Bytes(first), Origin, stored.UserHandle, counter: 9)).Succeeded);

            var second = w.PasskeyService.BeginSignIn();
            var cloned = w.PasskeyService.CompleteSignIn(second.Id, key.Get(Bytes(second), Origin, stored.UserHandle, counter: 4));
            Assert.Equal(PasskeyOutcome.CounterAnomaly, cloned.Outcome);
            Assert.Equal(1, w.Grants.CountAll());
        }

        // ---- the store races -----------------------------------------------------------------

        [Fact]
        public void Store_race_read_twice_commit_first_Updated_then_stale_Conflict()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            Enroll(w, AccessOpsDatabase.DanaAccountId, key);

            var read1 = w.Passkeys.Find("localhost", key.CredentialId);
            var read2 = w.Passkeys.Find("localhost", key.CredentialId);
            Assert.Equal(CommitOutcome.Updated, w.Passkeys.CommitUse(read1, 1, false));
            Assert.Equal(CommitOutcome.Conflict, w.Passkeys.CommitUse(read2, 1, false));
            Assert.Equal(1, w.Passkeys.Find("localhost", key.CredentialId).SignCount);
        }

        [Fact]
        public void Store_race_delete_before_commit_NotFound_and_the_row_stays_gone()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            Enroll(w, AccessOpsDatabase.DanaAccountId, key);

            var read = w.Passkeys.Find("localhost", key.CredentialId);
            Assert.True(w.Passkeys.Delete(AccessOpsDatabase.DanaAccountId, read.RowId));
            Assert.Equal(CommitOutcome.NotFound, w.Passkeys.CommitUse(read, 1, false));
            Assert.Null(w.Passkeys.Find("localhost", key.CredentialId));            // no resurrection

            // re-enrolled with the same credential ID: a new row identity, so the stale read still misses
            Enroll(w, AccessOpsDatabase.DanaAccountId, key);
            Assert.Equal(CommitOutcome.NotFound, w.Passkeys.CommitUse(read, 1, false));
        }

        [Fact]
        public void A_sign_in_that_loses_the_race_to_a_delete_creates_no_grant()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);
            var ticket = w.PasskeyService.BeginSignIn();
            var response = key.Get(Bytes(ticket), Origin, stored.UserHandle, counter: 1);

            // the owner deletes the lost device's passkey between the read and the commit (F10)
            var read = w.Passkeys.Find("localhost", key.CredentialId);
            w.Passkeys.Delete(AccessOpsDatabase.DanaAccountId, read.RowId);
            Assert.Equal(CommitOutcome.NotFound, w.Passkeys.CommitUse(read, 1, false));
            var result = w.PasskeyService.CompleteSignIn(ticket.Id, response);
            Assert.False(result.Succeeded);
            Assert.Equal(0, w.Grants.CountAll());
        }

        [Fact]
        public void The_unique_key_refuses_the_same_credential_under_a_second_account()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            Enroll(w, AccessOpsDatabase.DanaAccountId, key);
            var again = new PasskeyCredential
            {
                RowId = Guid.NewGuid().ToString("N"), RpId = "localhost", CredentialId = key.CredentialId,
                AccountId = AccessOpsDatabase.JoAccountId, UserHandle = w.Passkeys.UserHandleFor(AccessOpsDatabase.JoAccountId),
                PublicKeyJson = key.PublicKeyJson, Algorithm = -7, Label = "copy"
            };
            Assert.False(w.Passkeys.Add(again));
        }

        // ---- enrollment and review -----------------------------------------------------------

        private static ClaimsPrincipal Principal(TestWorld w, out VerifiedSignIn signIn) => w.SignInPartner(out signIn);

        [Fact]
        public void Enrollment_needs_a_current_grant_and_a_recent_sign_in()
        {
            using var w = new TestWorld();
            var principal = Principal(w, out var signIn);
            Assert.True(w.PasskeyService.MayEnroll(principal));

            w.Clock.Now += TimeSpan.FromMinutes(11);
            Assert.False(w.PasskeyService.MayEnroll(principal));                     // not recent
            w.Clock.Now -= TimeSpan.FromMinutes(11);

            w.Grants.Revoke(signIn.GrantId);
            Assert.False(w.PasskeyService.MayEnroll(principal));                     // grant revoked
            Assert.Null(w.PasskeyService.BeginEnrollment(principal));
        }

        [Fact]
        public void Enrollment_stores_a_key_that_later_validates_and_an_ascii_user_handle()
        {
            using var w = new TestWorld();
            var principal = Principal(w, out _);
            using var key = new SyntheticAuthenticator();

            var enrollment = w.PasskeyService.BeginEnrollment(principal);
            Assert.Matches("^[0-9a-f]{48}$", enrollment.User.Id);                    // opaque ASCII, not an e-mail
            Assert.Equal("localhost", enrollment.RelyingParty.ID);

            var created = w.PasskeyService.CompleteEnrollment(principal, enrollment.Challenge.Id,
                key.Create(Convert.FromBase64String(enrollment.Challenge.Base64), Origin), "Work laptop · Windows Hello");
            Assert.Equal(PasskeyOutcome.Accepted, created.Outcome);

            // the JSON.Stringify -> new PublicKey(json) round trip keeps a key Validate accepts
            var ticket = w.PasskeyService.BeginSignIn();
            var signedIn = w.PasskeyService.CompleteSignIn(ticket.Id, key.Get(Bytes(ticket), Origin, enrollment.User.Id, counter: 1));
            Assert.Equal(PasskeyOutcome.Accepted, signedIn.Outcome);
        }

        [Theory]
        [InlineData("origin")]
        [InlineData("type")]
        [InlineData("challenge")]
        [InlineData("account")]
        public void Enrollment_refusals(string broken)
        {
            using var w = new TestWorld();
            var principal = Principal(w, out _);
            using var key = new SyntheticAuthenticator();
            var before = w.Passkeys.ForAccount(AccessOpsDatabase.DanaAccountId).Count;   // Debug builds seed display fixtures
            var enrollment = w.PasskeyService.BeginEnrollment(principal);
            var challenge = broken == "challenge" ? new byte[32] : Convert.FromBase64String(enrollment.Challenge.Base64);
            var response = key.Create(challenge, broken == "origin" ? "http://localhost:6406" : Origin, broken == "type" ? "webauthn.get" : "webauthn.create");

            // "account": another signed-in account tries to finish Dana's ceremony
            var who = principal;
            if (broken == "account")
            {
                var jo = w.Accounts.FindById(AccessOpsDatabase.JoAccountId);
                var grant = w.Grants.Issue(jo.Id, jo.Tenant, jo.Role);
                who = ApplicationPrincipal.Build(new VerifiedSignIn { AccountId = jo.Id, TenantId = jo.Tenant, GrantId = grant.Id, Roles = new[] { jo.Role }, VerifiedAt = w.Clock.Now });
            }

            var result = w.PasskeyService.CompleteEnrollment(who, enrollment.Challenge.Id, response, "x");
            Assert.False(result.Succeeded);
            Assert.Equal(before, w.Passkeys.ForAccount(AccessOpsDatabase.DanaAccountId).Count);
        }

        [Fact]
        public void Rename_and_delete_touch_only_the_signed_in_owners_credentials()
        {
            using var w = new TestWorld();
            using var keyA = new SyntheticAuthenticator();
            var a = Enroll(w, AccessOpsDatabase.DanaAccountId, keyA, "Phone · synced passkey");
            var jo = w.Accounts.FindById(AccessOpsDatabase.JoAccountId);
            var joGrant = w.Grants.Issue(jo.Id, jo.Tenant, jo.Role);
            var joPrincipal = ApplicationPrincipal.Build(new VerifiedSignIn { AccountId = jo.Id, TenantId = jo.Tenant, GrantId = joGrant.Id, Roles = new[] { "Reader" }, VerifiedAt = w.Clock.Now });

            // a row ID of account A typed into account B's request deletes nothing
            Assert.False(w.PasskeyService.Delete(joPrincipal, a.RowId));
            Assert.False(w.PasskeyService.Rename(joPrincipal, a.RowId, "mine now"));
            Assert.NotNull(w.Passkeys.Find("localhost", keyA.CredentialId));

            var dana = Principal(w, out _);
            Assert.True(w.PasskeyService.Rename(dana, a.RowId, "Old phone"));
            Assert.Equal("Old phone", w.Passkeys.Find("localhost", keyA.CredentialId).Label);
            Assert.True(w.PasskeyService.Delete(dana, a.RowId));
            Assert.DoesNotContain(w.Passkeys.ForAccount(AccessOpsDatabase.DanaAccountId), c => c.RowId == a.RowId);
            Assert.Contains(w.Dump("audit"), row => row.Contains("passkey.delete") && row.Contains("allowed"));
        }

        [Fact]
        public void The_log_carries_no_full_credential_id_user_handle_or_signature()
        {
            using var w = new TestWorld();
            using var key = new SyntheticAuthenticator();
            var stored = Enroll(w, AccessOpsDatabase.DanaAccountId, key);
            var ticket = w.PasskeyService.BeginSignIn();
            var response = key.Get(Bytes(ticket), Origin, stored.UserHandle, counter: 1);
            w.PasskeyService.CompleteSignIn(ticket.Id, response);
            w.PasskeyService.CompleteSignIn(ticket.Id, response);   // and a refusal

            var lines = w.Logs.Lines.Concat(w.Dump("audit")).ToList();
            Assert.Contains(lines, l => l.Contains("commit=Updated"));
            Assert.DoesNotContain(lines, l => l.Contains(key.CredentialId) || l.Contains(stored.UserHandle) || l.Contains(Convert.ToBase64String(response.Signature)) || l.Contains(ticket.Id));
        }
    }
}
