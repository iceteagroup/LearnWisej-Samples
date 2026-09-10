using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// One line of the activity feed ("S. Patel commented on #1003"). The feed is the same data on every
    /// profile — docked on desktop, behind a tab on tablet and phone — which is the point of Module 3:
    /// the profile moves the panel that shows these, it never rebuilds or reloads them.
    /// </summary>
    public sealed class TicketEvent
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Text { get; set; }
        public DateTime At { get; set; } = DateTime.Now;

        /// <summary>"2m", "18m", "1h", "3d" — the compact age the feed shows next to each line.</summary>
        public string Age(DateTime now)
        {
            var span = now - At;
            if (span.TotalMinutes < 1) return "now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h";
            return $"{(int)span.TotalDays}d";
        }
    }
}
