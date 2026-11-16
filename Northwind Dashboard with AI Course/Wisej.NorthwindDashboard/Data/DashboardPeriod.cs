///////////////////////////////////////////////////////////////////////////////
//
// (C) 2026 ICE TEA GROUP LLC - ALL RIGHTS RESERVED
//
//
//
// ALL INFORMATION CONTAINED HEREIN IS, AND REMAINS
// THE PROPERTY OF ICE TEA GROUP LLC AND ITS SUPPLIERS, IF ANY.
// THE INTELLECTUAL PROPERTY AND TECHNICAL CONCEPTS CONTAINED
// HEREIN ARE PROPRIETARY TO ICE TEA GROUP LLC AND ITS SUPPLIERS
// AND MAY BE COVERED BY U.S. AND FOREIGN PATENTS, PATENT IN PROCESS, AND
// ARE PROTECTED BY TRADE SECRET OR COPYRIGHT LAW.
//
// DISSEMINATION OF THIS INFORMATION OR REPRODUCTION OF THIS MATERIAL
// IS STRICTLY FORBIDDEN UNLESS PRIOR WRITTEN PERMISSION IS OBTAINED
// FROM ICE TEA GROUP LLC.
//

///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Wisej.NorthwindDashboard.Data
{

	/// <summary>
	/// Reporting ranges offered by the period selector.
	/// </summary>
	public enum PeriodKind
	{

		/// <summary>
		/// The 30 days ending on the as-of date, in daily buckets.
		/// </summary>
		Last30Days,

		/// <summary>
		/// The 90 days ending on the as-of date, in weekly buckets.
		/// </summary>
		Last90Days,

		/// <summary>
		/// January 1 through the as-of date, in weekly buckets, compared with the same dates a year earlier.
		/// </summary>
		YearToDate,

		/// <summary>
		/// The 12 months ending on the as-of date, in calendar-month buckets.
		/// </summary>
		Last12Months,

		/// <summary>
		/// The complete order history, in calendar-month buckets.
		/// </summary>
		AllTime
	}

	/// <summary>
	/// Granularity of the buckets used by trend charts and sparklines.
	/// </summary>
	public enum BucketSize
	{

		/// <summary>
		/// One bucket per day.
		/// </summary>
		Day,

		/// <summary>
		/// Seven-day buckets aligned backwards from the end of the period.
		/// </summary>
		Week,

		/// <summary>
		/// Calendar months intersecting the period.
		/// </summary>
		Month
	}

	/// <summary>
	/// One time slice of a <see cref="DashboardPeriod"/>.
	/// </summary>
	public sealed class PeriodBucket
	{

		#region Constructors

		internal PeriodBucket(int index, DateTime start, DateTime end, string label, bool isPartial)
		{
			Index = index;
			Start = start;
			End = end;
			Label = label;
			IsPartial = isPartial;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the zero-based position of the bucket.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Gets the first day of the bucket.
		/// </summary>
		public DateTime Start { get; }

		/// <summary>
		/// Gets the last day of the bucket (inclusive).
		/// </summary>
		public DateTime End { get; }

		/// <summary>
		/// Gets the axis label, such as "Apr 7" or "Jun '97". Partial months end with an asterisk.
		/// </summary>
		public string Label { get; }

		/// <summary>
		/// Gets whether the bucket covers less than its nominal length.
		/// </summary>
		public bool IsPartial { get; }

		#endregion

	}

	/// <summary>
	/// Reporting period with its buckets and, when the data covers it, an equivalent comparison period.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Periods end on the database's as-of date (the last recorded order). Weekly buckets are aligned backwards
	/// from that date, so the most recent bucket is always complete; only the earliest bucket can be shorter.
	/// A comparison period is available only when it lies entirely within the recorded history; otherwise
	/// <see cref="HasComparison"/> is false and trends should be hidden rather than computed from partial data.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var period = DashboardPeriod.Create(PeriodKind.YearToDate, db.AsOfDate, db.FirstOrderDate);
	/// var revenue = db.Orders.Where(o => period.Contains(o.OrderDate)).Sum(o => o.Subtotal);
	/// </code>
	/// </example>
	public sealed class DashboardPeriod
	{

		#region Constructors

		private DashboardPeriod()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the selected range.
		/// </summary>
		public PeriodKind Kind { get; private set; }

		/// <summary>
		/// Gets the first day of the period.
		/// </summary>
		public DateTime Start { get; private set; }

		/// <summary>
		/// Gets the last day of the period (the as-of date).
		/// </summary>
		public DateTime End { get; private set; }

		/// <summary>
		/// Gets whether a complete comparison period exists.
		/// </summary>
		public bool HasComparison { get; private set; }

		/// <summary>
		/// Gets the first day of the comparison period; only meaningful when <see cref="HasComparison"/> is true.
		/// </summary>
		public DateTime PreviousStart { get; private set; }

		/// <summary>
		/// Gets the last day of the comparison period; only meaningful when <see cref="HasComparison"/> is true.
		/// </summary>
		public DateTime PreviousEnd { get; private set; }

		/// <summary>
		/// Gets the bucket granularity.
		/// </summary>
		public BucketSize BucketSize { get; private set; }

		/// <summary>
		/// Gets the buckets of the period, oldest first.
		/// </summary>
		public IReadOnlyList<PeriodBucket> Buckets { get; private set; }

		/// <summary>
		/// Gets the comparison buckets aligned by index with <see cref="Buckets"/>; empty without a comparison.
		/// </summary>
		public IReadOnlyList<PeriodBucket> PreviousBuckets { get; private set; }

		/// <summary>
		/// Gets the display name, such as "Year to date".
		/// </summary>
		public string Name => Kind switch
		{
			PeriodKind.Last30Days => "Last 30 days",
			PeriodKind.Last90Days => "Last 90 days",
			PeriodKind.YearToDate => "Year to date",
			PeriodKind.Last12Months => "Last 12 months",
			_ => "All time"
		};

		/// <summary>
		/// Gets the covered dates, such as "Jan 1 – May 6, 1998".
		/// </summary>
		public string RangeText => FormatRange(Start, End);

		/// <summary>
		/// Gets the comparison caption, such as "vs. Jan 1 – May 6, 1997", or a note explaining its absence.
		/// </summary>
		public string ComparisonText => HasComparison
			? "vs. " + FormatRange(PreviousStart, PreviousEnd)
			: Kind == PeriodKind.AllTime ? "Complete order history" : "No earlier data to compare";

		/// <summary>
		/// Gets whether any bucket is partial, which trend captions should mention.
		/// </summary>
		public bool HasPartialBuckets => Buckets.Any(b => b.IsPartial);

		#endregion

		#region Methods

		/// <summary>
		/// Creates the period of the specified kind ending on the as-of date.
		/// </summary>
		/// <param name="kind">Range to create.</param>
		/// <param name="asOf">Last day of the period.</param>
		/// <param name="firstDate">First day with recorded data; comparisons must not start earlier.</param>
		public static DashboardPeriod Create(PeriodKind kind, DateTime asOf, DateTime firstDate)
		{
			asOf = asOf.Date;
			firstDate = firstDate.Date;

			DateTime start;
			BucketSize size;
			Func<DateTime, DateTime> previous = null;
			switch (kind)
			{
				case PeriodKind.Last30Days:
					start = asOf.AddDays(-29);
					size = BucketSize.Day;
					previous = d => d.AddDays(-30);
					break;

				case PeriodKind.Last90Days:
					start = asOf.AddDays(-89);
					size = BucketSize.Week;
					previous = d => d.AddDays(-90);
					break;

				case PeriodKind.YearToDate:
					start = new DateTime(asOf.Year, 1, 1);
					size = BucketSize.Week;
					previous = d => d.AddYears(-1);
					break;

				case PeriodKind.Last12Months:
					start = asOf.AddYears(-1).AddDays(1);
					size = BucketSize.Month;
					previous = d => d.AddYears(-1);
					break;

				default:
					start = firstDate;
					size = BucketSize.Month;
					break;
			}

			if (start < firstDate)
				start = firstDate;

			var buckets = CreateBuckets(start, asOf, size);
			var period = new DashboardPeriod
			{
				Kind = kind,
				Start = start,
				End = asOf,
				BucketSize = size,
				Buckets = buckets,
				PreviousBuckets = Array.Empty<PeriodBucket>(),
				HasComparison = previous != null && previous(start) >= firstDate
			};

			if (period.HasComparison)
			{
				period.PreviousStart = previous(start);
				period.PreviousEnd = previous(asOf);
				period.PreviousBuckets = buckets
					.Select(b => new PeriodBucket(b.Index, previous(b.Start), previous(b.End), Label(size, previous(b.Start), b.IsPartial), b.IsPartial))
					.ToArray();
			}

			return period;
		}

		/// <summary>
		/// Returns whether the date falls within the period.
		/// </summary>
		/// <param name="date">Date to test.</param>
		public bool Contains(DateTime date) => date.Date >= Start && date.Date <= End;

		/// <summary>
		/// Returns whether the date falls within the comparison period.
		/// </summary>
		/// <param name="date">Date to test.</param>
		public bool InComparison(DateTime date) => HasComparison && date.Date >= PreviousStart && date.Date <= PreviousEnd;

		/// <summary>
		/// Returns the index of the bucket containing the date, or -1.
		/// </summary>
		/// <param name="date">Date to locate.</param>
		public int BucketOf(DateTime date) => Find(Buckets, date.Date);

		/// <summary>
		/// Returns the index of the comparison bucket containing the date, or -1.
		/// </summary>
		/// <param name="date">Date to locate.</param>
		public int PreviousBucketOf(DateTime date) => Find(PreviousBuckets, date.Date);

		/// <summary>
		/// Returns the percentage change from the comparison value, or null without a usable comparison.
		/// </summary>
		/// <param name="current">Value for the period.</param>
		/// <param name="previous">Value for the comparison period.</param>
		public decimal? Change(decimal current, decimal previous) =>
			HasComparison && previous != 0 ? Math.Round((current - previous) / previous * 100, 1) : null;

		#endregion

		#region Implementation

		private static IReadOnlyList<PeriodBucket> CreateBuckets(DateTime start, DateTime end, BucketSize size)
		{
			var ranges = new List<(DateTime Start, DateTime End, bool Partial)>();
			switch (size)
			{
				case BucketSize.Day:
					for (var day = start; day <= end; day = day.AddDays(1))
						ranges.Add((day, day, false));
					break;

				case BucketSize.Week:
					for (var last = end; last >= start; last = last.AddDays(-7))
					{
						var first = last.AddDays(-6);
						ranges.Insert(0, (first < start ? start : first, last, first < start));
					}
					break;

				default:
					for (var month = new DateTime(start.Year, start.Month, 1); month <= end; month = month.AddMonths(1))
					{
						var monthEnd = month.AddMonths(1).AddDays(-1);
						var first = month < start ? start : month;
						var last = monthEnd > end ? end : monthEnd;
						ranges.Add((first, last, first != month || last != monthEnd));
					}
					break;
			}

			return ranges.Select((r, i) => new PeriodBucket(i, r.Start, r.End, Label(size, r.Start, r.Partial), r.Partial)).ToArray();
		}

		private static string Label(BucketSize size, DateTime start, bool partial)
		{
			var culture = CultureInfo.CurrentCulture;
			return size == BucketSize.Month
				? start.ToString("MMM ''yy", culture) + (partial ? "*" : "")
				: start.ToString("MMM d", culture);
		}

		private static int Find(IReadOnlyList<PeriodBucket> buckets, DateTime date)
		{
			for (int i = 0; i < buckets.Count; i++)
			{
				if (date >= buckets[i].Start && date <= buckets[i].End)
					return i;
			}

			return -1;
		}

		private static string FormatRange(DateTime start, DateTime end)
		{
			var culture = CultureInfo.CurrentCulture;
			return start.Year == end.Year
				? start.ToString("MMM d", culture) + " – " + end.ToString("MMM d, yyyy", culture)
				: start.ToString("MMM d, yyyy", culture) + " – " + end.ToString("MMM d, yyyy", culture);
		}

		#endregion

	}
}
