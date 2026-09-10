using System;

namespace OperationsConsole.Models
{
    /// <summary>
    /// The satisfaction rating of one customer — the only piece of state the Widgets section owns.
    /// It is a plain server-side model: the browser only ever sends a single number, and
    /// everything else (who it belongs to, the scale, when it was stored) is decided on the server.
    /// </summary>
    public class RatingModel
    {
        /// <summary>Stable ID of the rated record — what <c>ConsoleLog.Record(...)</c> shows in the diagnostic panel.</summary>
        public string CustomerId { get; set; } = "CUST-1042";

        /// <summary>Display name, also used as the widget's caption (<c>Options.label</c>).</summary>
        public string CustomerName { get; set; } = "Northwind Traders";

        /// <summary>Top of the scale (<c>Options.max</c>). The service owns the rule; the model carries it.</summary>
        public int Max { get; set; } = 5;

        /// <summary>The value currently shown in the widget (0 = never rated).</summary>
        public int Value { get; set; }

        /// <summary>The value the service actually stored (0 = nothing stored yet).</summary>
        public int SavedValue { get; set; }

        /// <summary>When the stored value was written.</summary>
        public DateTime? SavedAt { get; set; }

        /// <summary>How many times this rating has been stored in this session.</summary>
        public int SaveCount { get; set; }

        /// <summary>True when the value on screen is the value the service holds.</summary>
        public bool IsSaved => this.SavedValue > 0 && this.SavedValue == this.Value;

        /// <summary>Human-readable state for the status area and the rating card.</summary>
        public string Describe()
        {
            if (this.SavedValue <= 0)
                return this.CustomerName + " has no saved rating yet — click a star.";

            if (!this.IsSaved)
                return this.CustomerName + " · showing " + this.Value + "/" + this.Max +
                       ", last saved value is " + this.SavedValue + "/" + this.Max + ".";

            return this.CustomerName + " · saved rating " + this.SavedValue + "/" + this.Max +
                   " at " + (this.SavedAt.HasValue ? this.SavedAt.Value.ToString("HH:mm:ss") : "—") +
                   " (" + this.SaveCount + " save" + (this.SaveCount == 1 ? "" : "s") + " this session)";
        }
    }
}
