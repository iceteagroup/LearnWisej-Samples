using System;
using System.Collections.Generic;
using System.Linq;

namespace VisualOperationsStudio.Diagnostics
{
    /// <summary>One render, as three numbers. Without them, tuning is guesswork.</summary>
    public class RenderSample
    {
        public RenderSample(string surface, double milliseconds, int width, int height, int objects)
        {
            Surface = surface;
            Milliseconds = milliseconds;
            Width = width;
            Height = height;
            Objects = objects;
            At = DateTime.Now;
        }

        public string Surface { get; }

        public double Milliseconds { get; }

        public int Width { get; }

        public int Height { get; }

        public int Objects { get; }

        public DateTime At { get; }

        public override string ToString() =>
            $"{At:HH:mm:ss}  {Surface,-22} {Milliseconds,7:0.0} ms   {Width}x{Height}   {Objects} objects";
    }

    /// <summary>
    /// Records how long a render took, how large its output was and how many scene objects it actually
    /// drew. One instance per session, held on the model, written to by every surface that draws.
    /// </summary>
    public class RenderMetrics
    {
        private const int Capacity = 200;

        private readonly List<RenderSample> samples = new List<RenderSample>();

        public IReadOnlyList<RenderSample> Samples => this.samples;

        public void Record(string surface, TimeSpan elapsed, int width, int height, int objects)
        {
            this.samples.Add(new RenderSample(surface, elapsed.TotalMilliseconds, width, height, objects));

            while (this.samples.Count > Capacity)
                this.samples.RemoveAt(0);
        }

        public void Clear() => this.samples.Clear();

        /// <summary>The median of the recorded times for one surface, or null when it has not drawn yet.</summary>
        public double? Median(string surface)
        {
            var times = this.samples
                .Where(sample => sample.Surface == surface)
                .Select(sample => sample.Milliseconds)
                .OrderBy(value => value)
                .ToList();

            if (times.Count == 0)
                return null;

            return times.Count % 2 == 1
                ? times[times.Count / 2]
                : (times[times.Count / 2 - 1] + times[times.Count / 2]) / 2.0;
        }

        public int Count(string surface) => this.samples.Count(sample => sample.Surface == surface);
    }

    /// <summary>Carries one render's figures from a painted control to whoever is recording them.</summary>
    public class RenderedEventArgs : EventArgs
    {
        public RenderedEventArgs(string surface, TimeSpan elapsed, int width, int height, int objects)
        {
            Surface = surface;
            Elapsed = elapsed;
            Width = width;
            Height = height;
            Objects = objects;
        }

        public string Surface { get; }

        public TimeSpan Elapsed { get; }

        public int Width { get; }

        public int Height { get; }

        public int Objects { get; }
    }
}
