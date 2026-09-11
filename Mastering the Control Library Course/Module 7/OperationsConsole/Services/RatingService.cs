using System;
using System.Globalization;
using OperationsConsole.Models;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The server-side half of the rating widget: what a rating is (a whole number from 1 to 5), whether a
    /// payload from the browser is acceptable, and storage. In memory; <see cref="SimulateFailure"/> makes
    /// <see cref="Save"/> throw.
    /// </summary>
    public class RatingService
    {
        /// <summary>Lowest acceptable rating. The one place this number exists.</summary>
        public const int MinRating = 1;

        /// <summary>Highest acceptable rating. The one place this number exists.</summary>
        public const int MaxRating = 5;

        private readonly RatingModel _current = new RatingModel { Max = MaxRating };

        /// <summary>When true, <see cref="Save"/> throws, the way an unreachable store would.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>The rating this session is editing.</summary>
        public RatingModel Current => this._current;

        /// <summary>
        /// Validates and normalises a payload that arrived from the browser.
        /// <para>
        /// This is the line that stops a hand-edited <c>fireWidgetEvent("ratingChanged", …)</c>:
        /// it accepts only something that converts to a whole number inside the scale, and it
        /// explains, in words a user can read, why it said no.
        /// </para>
        /// </summary>
        /// <param name="raw">The <c>value</c> field of <c>WidgetEventArgs.Data</c> (any JSON type, or null).</param>
        /// <param name="value">The normalised rating when the method returns true.</param>
        /// <param name="error">A message for the user when the method returns false.</param>
        public bool TryNormalize(object raw, out int value, out string error)
        {
            value = 0;
            error = null;

            if (raw == null)
            {
                error = "The rating message carried no value.";
                return false;
            }

            // The payload is JSON: a number arrives as long/double, a hand-edited one as a string.
            double number;
            if (raw is string text)
            {
                if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                {
                    error = "\"" + text + "\" is not a rating — expected a whole number between "
                            + MinRating + " and " + MaxRating + ".";
                    return false;
                }
            }
            else
            {
                try { number = Convert.ToDouble(raw, CultureInfo.InvariantCulture); }
                catch (Exception)
                {
                    error = "The rating message carried " + raw.GetType().Name + ", which is not a number.";
                    return false;
                }
            }

            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                error = "The rating message carried a value that is not a real number.";
                return false;
            }

            if (Math.Abs(number - Math.Round(number)) > 0.0001)
            {
                error = number.ToString("0.##", CultureInfo.InvariantCulture)
                        + " is not a whole number — ratings are " + MinRating + " to " + MaxRating + ".";
                return false;
            }

            int candidate = (int)Math.Round(number);
            if (candidate < MinRating || candidate > MaxRating)
            {
                error = candidate + " is outside the " + MinRating + "–" + MaxRating + " scale.";
                return false;
            }

            value = candidate;
            return true;
        }

        /// <summary>
        /// Stores an already validated rating. Throws when <see cref="SimulateFailure"/> is set —
        /// the caller must catch it, tell the user, and leave the widget editable.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value never went through <see cref="TryNormalize"/>.</exception>
        /// <exception cref="InvalidOperationException">The simulated store failure.</exception>
        public RatingModel Save(int value)
        {
            // Defence in depth: the handler validates, and the service refuses to store rubbish
            // even if a future caller forgets to.
            if (value < MinRating || value > MaxRating)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "A rating must be between " + MinRating + " and " + MaxRating + ".");

            if (this.SimulateFailure)
                throw new InvalidOperationException("The ratings store did not accept the write.");

            this._current.Value = value;
            this._current.SavedValue = value;
            this._current.SavedAt = DateTime.Now;
            this._current.SaveCount++;
            return this._current;
        }

        /// <summary>
        /// Records the value the user is looking at without storing it (a rejected or failed save).
        /// Keeps <c>Current.IsSaved</c> honest so the card never claims a value was stored.
        /// </summary>
        public void MarkUnsaved(int shownValue)
        {
            this._current.Value = shownValue;
        }

        /// <summary>Re-reads the rating — what <c>RefreshSection()</c> calls. In-memory, so it just returns it.</summary>
        public RatingModel Reload() => this._current;
    }
}
