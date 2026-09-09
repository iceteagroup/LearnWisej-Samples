using System;
using System.Collections.Generic;

namespace IntegrationLab.Data
{
    /// <summary>
    /// Deterministic synthetic "line load" data: the same inputs always produce the same cells,
    /// so the demo script and the troubleshooting notes can quote exact values.
    ///
    /// Three consumers:
    ///   - the postback endpoint (<see cref="Generate"/>): the first render loads one page of data;
    ///   - the background task (<see cref="NextLiveBatch"/>): a bounded stream of new readings;
    ///   - the Wisej.NET Designer (<see cref="DesignSample"/>): sample data without any service.
    /// </summary>
    public static class LoadSampleService
    {
        public const double MaxValue = 100;

        /// <summary>Hourly production profile (0..1) for a day shift: ramps up at 06:00, peaks mid-morning and mid-afternoon, tails off after 18:00.</summary>
        private static readonly double[] HourProfile =
        {
            0.08, 0.06, 0.05, 0.05, 0.06, 0.10, 0.28, 0.55, 0.78, 0.90, 0.96, 0.84,
            0.62, 0.74, 0.88, 0.97, 0.90, 0.72, 0.48, 0.30, 0.22, 0.16, 0.12, 0.10
        };

        /// <summary>Weekday weights: Mon..Sun. The weekend runs a reduced crew.</summary>
        private static readonly double[] DayProfile = { 0.92, 1.00, 0.97, 1.03, 0.95, 0.55, 0.35 };

        /// <summary>
        /// The "page of data" the postback endpoint answers with: <paramref name="days"/> × <paramref name="hours"/> cells.
        /// </summary>
        public static IReadOnlyList<HeatmapCell> Generate(int days, int hours, int seed = 7)
        {
            ValidateGrid(days, hours);
            var cells = new List<HeatmapCell>(days * hours);
            var random = new Lcg(seed);
            for (int day = 0; day < days; day++)
            {
                for (int hour = 0; hour < hours; hour++)
                {
                    double baseline = Profile(day, hour) * 88;
                    double jitter = (random.NextDouble() - 0.5) * 14;
                    cells.Add(new HeatmapCell(day, hour, Clamp(Math.Round(baseline + jitter, 1))));
                }
            }
            return cells;
        }

        /// <summary>
        /// One live batch: the same profile with a slow travelling surge, so consecutive pushes visibly differ
        /// while staying deterministic per <paramref name="iteration"/>.
        /// </summary>
        public static LiveReading NextLiveBatch(int iteration, int days, int hours)
        {
            ValidateGrid(days, hours);
            var cells = new List<HeatmapCell>(days * hours);
            var random = new Lcg(1000 + iteration);
            int surgeHour = (9 + iteration) % hours;
            int surgeDay = (iteration / 3) % days;
            for (int day = 0; day < days; day++)
            {
                for (int hour = 0; hour < hours; hour++)
                {
                    double baseline = Profile(day, hour) * 88;
                    double distance = Math.Abs(hour - surgeHour) + Math.Abs(day - surgeDay) * 2;
                    double surge = distance < 4 ? (4 - distance) * 6 : 0;
                    double jitter = (random.NextDouble() - 0.5) * 10;
                    cells.Add(new HeatmapCell(day, hour, Clamp(Math.Round(baseline + surge + jitter, 1))));
                }
            }

            // Boiler temperature follows the surge: 72 °F at rest, up to ~106 °F when the surge lands mid-shift.
            double temperature = Math.Round(72 + 22 * Profile(surgeDay, surgeHour) + 12 * Math.Sin(iteration * 0.7), 1);
            return new LiveReading(iteration, cells, Clamp(temperature, 40, 120));
        }

        /// <summary>Design-time sample: a smooth profile with no jitter, so the Designer shows a recognisable picture.</summary>
        public static IReadOnlyList<HeatmapCell> DesignSample(int days, int hours)
        {
            ValidateGrid(days, hours);
            var cells = new List<HeatmapCell>(days * hours);
            for (int day = 0; day < days; day++)
                for (int hour = 0; hour < hours; hour++)
                    cells.Add(new HeatmapCell(day, hour, Clamp(Math.Round(Profile(day, hour) * 92, 1))));
            return cells;
        }

        public static HeatmapCell FindPeak(IReadOnlyList<HeatmapCell> cells)
        {
            if (cells == null || cells.Count == 0)
                throw new InvalidOperationException("No data has been loaded yet.");
            var peak = cells[0];
            for (int i = 1; i < cells.Count; i++)
                if (cells[i].Value > peak.Value) peak = cells[i];
            return peak;
        }

        private static double Profile(int day, int hour)
            => DayProfile[day % DayProfile.Length] * HourProfile[hour % HourProfile.Length];

        private static double Clamp(double value, double min = 0, double max = MaxValue)
            => Math.Max(min, Math.Min(max, value));

        private static void ValidateGrid(int days, int hours)
        {
            if (days < 1 || days > 31) throw new ArgumentOutOfRangeException(nameof(days), days, "days must be between 1 and 31.");
            if (hours < 1 || hours > 24) throw new ArgumentOutOfRangeException(nameof(hours), hours, "hours must be between 1 and 24.");
        }

        /// <summary>Tiny linear congruential generator: deterministic across runs and platforms (System.Random is not guaranteed to be).</summary>
        private sealed class Lcg
        {
            private uint _state;
            public Lcg(int seed) { _state = (uint)seed * 2654435761u + 12345u; }
            public double NextDouble()
            {
                _state = _state * 1664525u + 1013904223u;
                return (_state >> 8) / (double)(1u << 24);
            }
        }
    }

    /// <summary>One push of the background task: a full cell batch plus the gauge reading that goes with it.</summary>
    public sealed class LiveReading
    {
        public LiveReading(int iteration, IReadOnlyList<HeatmapCell> cells, double temperature)
        {
            this.Iteration = iteration;
            this.Cells = cells;
            this.Temperature = temperature;
        }

        public int Iteration { get; }
        public IReadOnlyList<HeatmapCell> Cells { get; }
        public double Temperature { get; }
    }
}
