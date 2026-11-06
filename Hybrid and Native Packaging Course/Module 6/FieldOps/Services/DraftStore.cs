using System;
using System.Collections.Concurrent;

namespace FieldOps.Services
{
    /// <summary>
    /// The half-typed work: which order was open and what the technician had typed. Kept on the
    /// server, keyed by technician - so it survives a web-view reload, a new session, even a
    /// different device. (The Hybrid client also keeps the id and the reading in
    /// Device.Preferences; a long note belongs here, not there.)
    /// </summary>
    public static class DraftStore
    {
        public sealed class Draft
        {
            public string Number { get; init; }
            public string Reading { get; init; }
            public string Notes { get; init; }
            public DateTime SavedAt { get; init; }
        }

        private static readonly ConcurrentDictionary<string, Draft> _drafts = new ConcurrentDictionary<string, Draft>();

        public static void Save(string technicianId, string number, string reading, string notes) =>
            _drafts[technicianId] = new Draft { Number = number, Reading = reading, Notes = notes, SavedAt = DateTime.Now };

        public static Draft Get(string technicianId) =>
            _drafts.TryGetValue(technicianId, out var d) ? d : null;

        public static void Clear(string technicianId) => _drafts.TryRemove(technicianId, out _);
    }
}
