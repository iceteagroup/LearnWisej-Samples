using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The one door every layer logs through. The page hands it a sink (its <c>Trace(string)</c> helper,
    /// which adds the timestamp and selects the last item); services, stores and security rules only say
    /// which layer decided and what. The reviewer reads the right-hand card to prove the handler was thin.
    /// </summary>
    public sealed class ActivityTrace
    {
        private readonly Action<string> _sink;

        public ActivityTrace(Action<string> sink)
        {
            _sink = sink ?? (_ => { });
        }

        public void Ui(string message) => Write("UI →", message);
        public void Service(string message) => Write("Service:", message);
        public void Data(string message) => Write("Data:", message);
        public void Security(string message) => Write("Security:", message);
        public void Job(string message) => Write("Job:", message);

        private void Write(string layer, string message) => _sink($"{layer,-9} {message}");
    }
}
