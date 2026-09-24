using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// One row of the operations grid. The painted cells read their values from this object in one
    /// step, rather than searching a collection while the grid is scrolling.
    /// </summary>
    public class MachineStatus
    {
        /// <summary>At or above this health score the machine is OK.</summary>
        public const int OkAtLeast = 80;

        /// <summary>At or above this health score the machine is a warning; below it, critical.</summary>
        public const int WarningAtLeast = 60;

        public static readonly Color OkColor = Color.FromArgb(31, 157, 107);
        public static readonly Color WarningColor = Color.FromArgb(232, 161, 60);
        public static readonly Color CriticalColor = Color.FromArgb(224, 86, 59);

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

        public string LastReadingText => RecentReadings.Count == 0 ? "-" : $"{LastReading:0.0} °C";

        public string Severity =>
            Health >= OkAtLeast ? "OK" :
            Health >= WarningAtLeast ? "Warning" : "Critical";

        /// <summary>The colour the painted Health bar and the Trend line use for this row.</summary>
        public Color Tone => ToneFor(Health);

        public static Color ToneFor(int health) =>
            health >= OkAtLeast ? OkColor :
            health >= WarningAtLeast ? WarningColor : CriticalColor;

        /// <summary>
        /// The same state as markup, for the AllowHtml comparison column. Markup wins for text, icons
        /// and accessibility; painting wins for geometry markup cannot express.
        /// </summary>
        public string SeverityHtml
        {
            get
            {
                var tone = ColorTranslator.ToHtml(Tone);
                return "<span style=\"display:inline-flex;align-items:center;gap:6px;font-size:11px;font-weight:800;" +
                       $"color:{tone};background:{tone}1a;border:1px solid {tone}55;border-radius:999px;padding:2px 9px\">" +
                       $"<span style=\"width:7px;height:7px;border-radius:999px;background:{tone}\"></span>{Severity}</span>";
            }
        }
    }

    public static class MachineStatusGenerator
    {
        private static readonly string[] Sites = { "Turin", "Porto", "Lyon", "Dresden", "Bilbao", "Gdansk" };
        private static readonly string[] Kinds = { "PRESS", "LATHE", "OVEN", "MILL", "PUMP", "CNC" };

        /// <summary>
        /// The six machines the walkthrough shows, in the order it shows them. They are the first rows
        /// of the bound list so the grid on screen is the grid in the video.
        /// </summary>
        private static readonly object[][] Featured =
        {
            new object[] { "PRESS-014", "Turin", 92, new double[] { 58, 61, 60, 64, 67, 66, 70, 72 } },
            new object[] { "LATHE-207", "Turin", 76, new double[] { 70, 68, 64, 61, 59, 55, 52, 49 } },
            new object[] { "OVEN-003", "Porto", 41, new double[] { 44, 46, 43, 39, 35, 30, 27, 22 } },
            new object[] { "MILL-118", "Porto", 88, new double[] { 51, 54, 53, 57, 58, 61, 63, 62 } },
            new object[] { "PUMP-052", "Lyon", 63, new double[] { 66, 63, 65, 60, 58, 59, 55, 57 } },
            new object[] { "CNC-441", "Lyon", 97, new double[] { 72, 74, 73, 76, 79, 81, 80, 84 } },
        };

        /// <summary>
        /// Builds <paramref name="count"/> rows from a fixed seed, so every run of the sample shows the
        /// same grid. Roughly one row in twelve has a single reading, which leaves its Trend cell with
        /// nothing to draw.
        /// </summary>
        public static List<MachineStatus> Generate(int count)
        {
            var random = new Random(20260923);
            var rows = new List<MachineStatus>(count);

            for (var i = 0; i < count && i < Featured.Length; i++)
            {
                var f = Featured[i];
                rows.Add(new MachineStatus($"M{i + 1:0000}", (string)f[0], (string)f[1], (int)f[2], (double[])f[3]));
            }

            for (var i = rows.Count; i < count; i++)
            {
                var site = Sites[i % Sites.Length];
                var kind = Kinds[(i / Sites.Length) % Kinds.Length];
                var health = random.Next(12, 101);
                var readings = i % 12 == 5
                    ? new[] { (double)random.Next(20, 90) }
                    : Trend(random, health);

                rows.Add(new MachineStatus($"M{i + 1:0000}", $"{kind}-{i + 1:000}", site, health, readings));
            }

            return rows;
        }

        private static double[] Trend(Random random, int health)
        {
            // A healthy machine drifts a little; an unhealthy one climbs.
            var drift = health >= MachineStatus.WarningAtLeast ? 4 : 16;
            var value = (double)random.Next(25, 60);

            return Enumerable.Range(0, 8)
                .Select(_ =>
                {
                    value = Math.Min(100, Math.Max(0, value + random.Next(-drift, drift + 1)));
                    return value;
                })
                .ToArray();
        }
    }
}
