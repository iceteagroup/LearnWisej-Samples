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

using System.Collections.Generic;
using System.Linq;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Reflows the cards of a view's 12-column <see cref="TableLayoutPanel"/> for the Tablet and Phone client profiles.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A card's cell and column span are properties the table extends its children with, and Wisej's responsive
	/// properties hold a control's own properties, so the designer keeps one layout, the Default profile, and this class
	/// derives the narrower ones from it when <see cref="Control.ResponsiveProfileChanged"/> fires:
	/// </para>
	/// <list type="bullet">
	/// <item><description>Tablet: a row of KPI tiles (spans of 3) becomes two rows of two; every other card spans the width.</description></item>
	/// <item><description>Phone: one card per row, KPI tiles included; their values need the width.</description></item>
	/// </list>
	/// <para>
	/// A card keeps the height of its designed row, or its <see cref="Control.MinimumSize"/> height when that is larger,
	/// so a card can ask for more room on a profile in the designer. A row sized in percent, which fills a view that does
	/// not scroll, gets <see cref="FillRowHeight"/>, and the view scrolls instead.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// cards = new CardLayout(layout);
	/// cards.Apply(Application.ActiveProfile.Name);
	/// </code>
	/// </example>
	internal sealed class CardLayout
	{

		#region Constructors

		/// <summary>
		/// Records the designed layout of <paramref name="table"/>.
		/// </summary>
		/// <param name="table">A view's card grid, docked in the view.</param>
		public CardLayout(TableLayoutPanel table)
		{
			this.table = table;
			designDock = table.Dock;
			designHeight = table.Height;
			designPadding = table.Padding;
			designAutoScroll = ((ScrollableControl)table.Parent).AutoScroll;
			designRows = table.RowStyles.Cast<RowStyle>().Select(r => new RowStyle(r.SizeType, r.Height)).ToArray();

			cards = table.Controls.Cast<Control>()
				.Select(c => new Card(c, table.GetCellPosition(c), table.GetColumnSpan(c), table.GetRowSpan(c)))
				.OrderBy(c => c.Cell.Row)
				.ThenBy(c => c.Cell.Column)
				.ToArray();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Height of a card whose designed row is sized in percent, on Tablet and Phone.
		/// </summary>
		public int FillRowHeight { get; set; } = 620;

		#endregion

		#region Methods

		/// <summary>
		/// Arranges the cards for a client profile: the designed layout for Default or any profile other than Tablet and
		/// Phone.
		/// </summary>
		/// <param name="profile">Name of the active client profile.</param>
		public void Apply(string profile)
		{
			bool narrow = profile == "Phone" || profile == "Tablet";
			if (narrow == reflowed && profile == applied)
				return;

			table.SuspendLayout();
			try
			{
				if (narrow)
					Reflow(profile == "Phone");
				else
					Restore();
			}
			finally
			{
				table.ResumeLayout(true);
			}

			reflowed = narrow;
			applied = profile;
		}

		#endregion

		#region Implementation

		private sealed record Card(Control Control, TableLayoutPanelCellPosition Cell, int ColumnSpan, int RowSpan);

		private const int Columns = 12;

		private readonly TableLayoutPanel table;
		private readonly Card[] cards;
		private readonly RowStyle[] designRows;
		private readonly DockStyle designDock;
		private readonly int designHeight;
		private readonly Padding designPadding;
		private readonly bool designAutoScroll;
		private bool reflowed;
		private string applied;

		private void Reflow(bool phone)
		{
			var rows = new List<float>();

			foreach (var designRow in cards.GroupBy(c => c.Cell.Row))
			{
				// A card can ask for more height on a profile through its responsive MinimumSize.
				float height = designRow.Aggregate(RowHeight(designRow.Key),
					(h, c) => System.Math.Max(h, c.Control.MinimumSize.Height + c.Control.Margin.Vertical));

				// KPI tiles, two to a row on a tablet; anything wider takes a row of its own.
				bool tiles = !phone && designRow.All(c => c.ColumnSpan <= 3);
				int perRow = tiles ? 2 : 1;
				int span = Columns / perRow;

				int index = 0;
				foreach (var card in designRow)
				{
					if (index % perRow == 0)
						rows.Add(height);

					Place(card.Control, (index % perRow) * span, rows.Count - 1, span, 1);
					index++;
				}
			}

			SetRows(rows.Select(h => new RowStyle(SizeType.Absolute, h)));

			table.Padding = new Padding(phone ? 8 : 12);
			table.Dock = DockStyle.Top;
			table.Height = (int)rows.Sum() + table.Padding.Vertical;
			((ScrollableControl)table.Parent).AutoScroll = true;
		}

		private void Restore()
		{
			foreach (var card in cards)
				Place(card.Control, card.Cell.Column, card.Cell.Row, card.ColumnSpan, card.RowSpan);

			SetRows(designRows.Select(r => new RowStyle(r.SizeType, r.Height)));

			table.Padding = designPadding;
			table.Dock = designDock;
			table.Height = designHeight;
			((ScrollableControl)table.Parent).AutoScroll = designAutoScroll;
		}

		private float RowHeight(int row)
		{
			var style = row < designRows.Length ? designRows[row] : null;
			return style?.SizeType == SizeType.Absolute ? style.Height : FillRowHeight;
		}

		private void Place(Control control, int column, int row, int columnSpan, int rowSpan)
		{
			table.SetCellPosition(control, new TableLayoutPanelCellPosition(column, row));
			table.SetColumnSpan(control, columnSpan);
			table.SetRowSpan(control, rowSpan);
		}

		private void SetRows(IEnumerable<RowStyle> styles)
		{
			table.RowStyles.Clear();
			foreach (var style in styles)
				table.RowStyles.Add(style);

			table.RowCount = table.RowStyles.Count;
		}

		#endregion

	}
}
