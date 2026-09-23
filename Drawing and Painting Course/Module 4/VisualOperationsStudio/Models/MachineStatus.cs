using System;
using System.Collections.Generic;
using System.Linq;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// One row of the operations grid. The painted cells read their values from this object in one
    /// step, rather than searching a collection while the grid is scrolling.
    /// </summary>
    public class MachineStatus
    {
        /// <summary>Below this health score the row is a warning.</summary>
        public const int WarningBelow = 60;

        /// <summary>Below this health score the row is critical.</summary>
        public const int CriticalBelow = 35;

        public MachineStatus(string id, string machine, string site, int health, IReadOnlyList<double> recentReadings)
        {
            Id = id;
            Machine = machine;
            Site = site;
            Health = health;
            RecentReadings = recentReadings ?? new double[0];
        }

        public string Id { get; }

        public string Machine { get; }

        public string Site { get; }

        /// <summary>Health score, 0 to 100. Higher is better.</summary>
        public int Health { get; }

        /// <summary>The readings the Trend cell draws. Fewer than two means there is no line to draw.</summary>
        public IReadOnlyList<double> RecentReadings { get; }

        public double LastReading => RecentReadings.Count == 0 ? 0 : RecentReadings[RecentReadings.Count - 1];

        public string LastReadingText => RecentReadings.Count == 0 ? "-" : $"{LastReading:0} %";

        public string Severity =>
            Health < CriticalBelow ? "Critical" :
            Health < WarningBelow ? "Warning" : "Normal";

        /// <summary>
        /// The same state as markup, for the AllowHtml comparison column. Markup wins for text, icons
        /// and accessibility; painting wins for geometry markup cannot express.
        /// </summary>
        public string SeverityHtml
        {
            get
            {
                var colour = Health < CriticalBelow ? "#d93a3a" : Health < WarningBelow ? "#b26a00" : "#1f7a4d";
                var background = Health < CriticalBelow ? "#fdecec" : Health < WarningBelow ? "#fdf3e2" : "#e8f6ee";
                return $"<span style=\"display:inline-block;padding:2px 10px;border-radius:10px;background:{background};color:{colour};font-weight:600\">{Severity} {Health}%</span>";
            }
        }
    }

    public static class MachineStatusGenerator
    {
        private static readonly string[] Sites = { "Turin", "Dresden", "Lyon", "Bilbao", "Gdansk" };
        private static readonly string[] Kinds = { "Press", "Lathe", "Mill", "Robot", "Conveyor", "Oven" };

        /// <summary>
        /// Builds <paramref name="count"/> rows from a fixed seed, so every run of the sample shows the
        /// same grid. Roughly one row in twelve has a single reading, which leaves its Trend cell with
        /// nothing to draw.
        /// </summary>
        public static List<MachineStatus> Generate(int count)
        {
            var random = new Random(20260923);
            var rows = new List<MachineStatus>(count);

            for (var i = 0; i < count; i++)
            {
                var site = Sites[i % Sites.Length];
                var kind = Kinds[(i / Sites.Length) % Kinds.Length];
                var health = random.Next(12, 101);
                var readings = i % 12 == 5
                    ? new[] { (double)random.Next(20, 90) }
                    : Trend(random, health);

                rows.Add(new MachineStatus($"M{i + 1:0000}", $"{kind} {i + 1:000}", site, health, readings));
            }

            return rows;
        }

        private static double[] Trend(Random random, int health)
        {
            // A healthy machine drifts a little; an unhealthy one climbs.
            var drift = health >= MachineStatus.WarningBelow ? 4 : 16;
            var value = (double)random.Next(25, 60);

            return Enumerable.Range(0, 12)
                .Select(_ =>
                {
                    value = Math.Min(100, Math.Max(0, value + random.Next(-drift, drift + 1)));
                    return value;
                })
                .ToArray();
        }
    }
}
