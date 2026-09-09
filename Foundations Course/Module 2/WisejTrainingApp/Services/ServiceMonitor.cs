using System;
using System.Collections.Generic;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// A tiny stand-in for "the thing that knows whether the services are up". The lesson says the
    /// dashboard buttons don't need a real database or API — the goal is event-driven behaviour — so
    /// this monitor keeps a running flag and answers each check with a fake latency in milliseconds.
    ///
    /// It is an instance field on DashboardWindow (one per user session), never static.
    /// </summary>
    public class ServiceMonitor
    {
        public const string ServerName = "Server";
        public const string DatabaseName = "Database";
        public const string ApiName = "API Service";

        private readonly Random random = new Random();

        /// <summary>True between Start and Stop/Reset. Offline services answer no latency.</summary>
        public bool IsRunning { get; private set; }

        /// <summary>The failure switch: when true the API check times out instead of answering.</summary>
        public bool SimulateApiOutage { get; set; }

        public void Start() => IsRunning = true;

        public void Stop() => IsRunning = false;

        public void Reset()
        {
            IsRunning = false;
            SimulateApiOutage = false;
        }

        /// <summary>Checks the three services the dashboard shows, in display order.</summary>
        public List<ServiceCheck> CheckAll()
        {
            return new List<ServiceCheck>
            {
                Check(ServerName),
                Check(DatabaseName),
                Check(ApiName),
            };
        }

        /// <summary>One check: Degraded when the outage is simulated, Offline while stopped, otherwise Online with a latency.</summary>
        public ServiceCheck Check(string name)
        {
            if (name == ApiName && SimulateApiOutage)
            {
                return new ServiceCheck
                {
                    Name = name,
                    State = ServiceState.Degraded,
                    LatencyMs = 2000,
                    Error = "timeout after 2000 ms",
                };
            }

            if (!IsRunning)
            {
                return new ServiceCheck { Name = name, State = ServiceState.Offline };
            }

            return new ServiceCheck
            {
                Name = name,
                State = ServiceState.Online,
                LatencyMs = FakeLatency(name),
            };
        }

        /// <summary>A plausible number per service so the "Status refreshed" line changes on every Refresh.</summary>
        private int FakeLatency(string name)
        {
            return name switch
            {
                DatabaseName => random.Next(24, 40),   // the database is the slowest
                ApiName => random.Next(14, 24),
                _ => random.Next(8, 16),               // the web server is the fastest
            };
        }
    }
}
