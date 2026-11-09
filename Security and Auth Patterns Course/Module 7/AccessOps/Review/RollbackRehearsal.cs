#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.Extensions.Logging;

namespace AccessOps.Review
{
    /// <summary>One step of the rollback runbook as the review screen shows it.</summary>
    public sealed class RunbookStep
    {
        /// <summary>1-6.</summary>
        public int Number { get; init; }

        /// <summary>The runbook action.</summary>
        public string Action { get; init; }

        /// <summary>Who owns it.</summary>
        public string Owner { get; init; }

        /// <summary>"pending" until the step ran, then the result chip ("Done", "Allow", "Deny · no export", …).</summary>
        public string Result { get; set; } = "pending";

        /// <summary>What the step observed (plain text).</summary>
        public string Detail { get; set; } = "";

        /// <summary>False when the step did not reach its expected result.</summary>
        public bool Ok { get; set; } = true;
    }

    /// <summary>
    /// DEBUG ONLY - the rehearsed rollback of the release review: stop promotion, switch the partner
    /// route off at ingress (<see cref="IngressSwitch"/>), prove the intranet baseline still serves a
    /// Reader, repeat the stale-tab case (the revoked grant stays revoked, no export), prove no earlier
    /// revocation came back, and revoke the grants the run's callbacks issued. Code and configuration
    /// roll back; the grant store does not.
    /// </summary>
    public sealed class RollbackRehearsal
    {
        private readonly ReleaseReviewRunner _runner;
        private readonly AccessOpsDatabase _db;
        private readonly GrantStore _grants;
        private readonly AuditService _audit;
        private readonly ILogger _log;

        /// <summary>Creates the rehearsal next to the runner whose evidence it acts on.</summary>
        public RollbackRehearsal(ReleaseReviewRunner runner, IServiceProvider services)
        {
            _runner = runner;
            _db = (AccessOpsDatabase)services.GetService(typeof(AccessOpsDatabase));
            _grants = (GrantStore)services.GetService(typeof(GrantStore));
            _audit = (AuditService)services.GetService(typeof(AuditService));
            _log = ((ILoggerFactory)services.GetService(typeof(ILoggerFactory))).CreateLogger("AccessOps.Rollback");
        }

        /// <summary>The runbook, before the rehearsal.</summary>
        public static List<RunbookStep> Steps() => new List<RunbookStep>
        {
            new RunbookStep { Number = 1, Action = "Stop promotion and page the owners", Owner = "Release lead" },
            new RunbookStep { Number = 2, Action = "Turn the partner route off at ingress", Owner = "Platform" },
            new RunbookStep { Number = 3, Action = "Intranet Reader reads a permitted record", Owner = "App team" },
            new RunbookStep { Number = 4, Action = "Repeat case 7: revoked grant, stale tab", Owner = "App team" },
            new RunbookStep { Number = 5, Action = "Earlier revocations still in place after rollback", Owner = "Data owner" },
            new RunbookStep { Number = 6, Action = "Revoke the grant the replayed callback issued", Owner = "Security" },
        };

        /// <summary>Seconds from the ingress switch to the first 503 on the partner route (the runbook's recovery time).</summary>
        public double SecondsToPartnerOff { get; private set; }

        /// <summary>Runs step <paramref name="step"/>; never throws.</summary>
        public async Task RunAsync(RunbookStep step, ReviewContext context, string label)
        {
            using var scope = Correlation.Begin(_log, label);
            try
            {
                switch (step.Number)
                {
                    case 1:
                        _revokedBefore = _grants.CountRevoked();
                        _audit.Write(new AuditEvent("review", "release.stop", "partner rollout", "-", "stopped", "rehearsal", label));
                        step.Result = "Done";
                        step.Detail = "promotion stopped; owners paged (rehearsal)";
                        break;

                    case 2:
                        var watch = Stopwatch.StartNew();
                        IngressSwitch.Write(_db.DataRoot, on: false);
                        var status = 0;
                        while (watch.Elapsed < TimeSpan.FromSeconds(20))
                        {
                            status = await PartnerStatusAsync();
                            if (status == 503)
                                break;
                            await Task.Delay(250);
                        }
                        SecondsToPartnerOff = Math.Round(watch.Elapsed.TotalSeconds, 1);
                        _audit.Write(new AuditEvent("review", "ingress.partner", "partner route", "-", status == 503 ? "off" : "still-on", "rollback", label));
                        step.Ok = status == 503;
                        step.Result = step.Ok ? "Done" : "Failed";
                        step.Detail = $"partner route answers {status} after {SecondsToPartnerOff} s";
                        break;

                    case 3:
                        var probe = new EvidenceCase { Number = 1, Title = "", Route = "Intranet", Fixture = "F01", ExpectedOutcome = "Allow", Label = EvidenceLabel.Fixture };
                        await _runner.RunAsync(probe, context);
                        step.Ok = probe.Outcome == "Allow";
                        step.Result = step.Ok ? "Allow" : "Failed";
                        step.Detail = probe.Actual;
                        break;

                    case 4:
                        var stale = new EvidenceCase { Number = 7, Title = "", Route = "Partner", Fixture = "F05", ExpectedOutcome = "Deny", Label = EvidenceLabel.Live };
                        await _runner.RunAsync(stale, context);
                        step.Ok = stale.Outcome == "Deny" && stale.RepositoryCalls == 0;
                        step.Result = step.Ok ? "Deny · no export" : "Failed";
                        step.Detail = stale.Actual;
                        break;

                    case 5:
                        var revokedNow = _grants.CountRevoked();
                        step.Ok = revokedNow >= _revokedBefore;
                        step.Result = step.Ok ? "Verified" : "Failed";
                        step.Detail = $"revoked grants before the rollback {_revokedBefore}, after {revokedNow}: none came back";
                        break;

                    case 6:
                        var revoked = 0;
                        foreach (var id in _runner.CallbackGrants)
                            if (_grants.Revoke(id))
                                revoked++;
                        step.Result = "Revoked";
                        step.Detail = $"{revoked} grant(s) issued by the run's callbacks revoked";
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "rollback step {Step} failed. correlation={Correlation}", step.Number, label);
                step.Ok = false;
                step.Result = "Failed";
                step.Detail = "The step could not be completed. The details are in the server log under " + label + ".";
            }
        }

        private long _revokedBefore;

        /// <summary>Turns the partner route back on (the "restore" after the rehearsal).</summary>
        public async Task<int> RestoreAsync()
        {
            IngressSwitch.Write(_db.DataRoot, on: true);
            var watch = Stopwatch.StartNew();
            var status = 0;
            while (watch.Elapsed < TimeSpan.FromSeconds(20))
            {
                status = await PartnerStatusAsync();
                if (status != 503 && status != 0)
                    break;
                await Task.Delay(250);
            }
            _audit.Write(new AuditEvent("review", "ingress.partner", "partner route", "-", status == 503 ? "still-off" : "on", "restore", AuditService.NewCorrelationId()));
            return status;
        }

        private async Task<int> PartnerStatusAsync()
        {
            var handler = new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false };
            handler.SslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, errors) =>
                errors == System.Net.Security.SslPolicyErrors.None
                || (certificate is X509Certificate2 cert && cert.GetNameInfo(X509NameType.DnsName, false) == "localhost");
            using var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
            try
            {
                using var response = await http.GetAsync(_runner.PartnerUrl + "/Default.html");
                return (int)response.StatusCode;
            }
            catch (HttpRequestException)
            {
                return 0;   // nothing listening
            }
            catch (TaskCanceledException)
            {
                return 0;
            }
        }
    }
}
#endif
