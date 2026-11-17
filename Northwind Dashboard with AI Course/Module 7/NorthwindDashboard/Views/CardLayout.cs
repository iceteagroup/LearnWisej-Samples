using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Reflows the cards of a view for the Tablet and Phone client profiles, and restores the designed layout for
	/// Default.
	/// </summary>
	/// <remarks>
	/// <para>
	/// CardLayout is an application helper, not a Wisej control. A card's cell and column span are properties the
	/// <see cref="TableLayoutPanel"/> extends its children with, and Wisej's responsive properties hold a control's own
	/// properties, so the designer keeps one layout, the Default profile, and this class derives the narrower ones
	/// from it when the client profile changes. Everything it changes it recorded first, and <c>Apply("Default")</c>
	/// puts every value back.
	/// </para>
	/// <para>
	/// A view built on a 12-column <see cref="TableLayoutPanel"/> gets these arrangements:
	/// </para>
	/// <list type="bullet">
	/// <item><description>Tablet: a row of KPI tiles (spans of 3) becomes two rows of two; every other card spans the width.</description></item>
	/// <item><description>Phone: one card per row, KPI tiles included; their values need the width.</description></item>
	/// </list>
	/// <para>
	/// A card keeps the height of the designed rows it spans, or its <see cref="Control.MinimumSize"/> height when that
	/// is larger, so a card can ask for more room on a profile in the designer. A row sized in percent, which fills a
	/// view that does not scroll, gets <see cref="FillRowHeight"/>, and the view scrolls instead. The card that ends a
	/// row takes the right margin of the cards that end the designed rows, so every row ends at the same edge.
	/// </para>
	/// <para>
	/// A view without such a table whose controls are placed by location (not docked, anchored top-left) is handled
	/// too: on a phone its controls are stacked at full width in reading order, and on a tablet the view keeps its
	/// layout and scrolls. A view whose controls are docked or anchored to its edges needs nothing: they already
	/// follow the width.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// cards = CardLayout.For(view);
	/// cards?.Apply(Application.ActiveProfile.Name);
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
			view = (ScrollableControl)table.Parent;
			designDock = table.Dock;
			designHeight = table.Height;
			designPadding = table.Padding;
			designAutoScroll = view.AutoScroll;
			designRows = table.RowStyles.Cast<RowStyle>().Select(r => new RowStyle(r.SizeType, r.Height)).ToArray();

			cards = table.Controls.Cast<Control>()
				.Select(c => new Card(c, table.GetCellPosition(c), table.GetColumnSpan(c), table.GetRowSpan(c), c.Margin))
				.OrderBy(c => c.Cell.Row)
				.ThenBy(c => c.Cell.Column)
				.ToArray();
		}

		// A view whose controls are placed by location.
		private CardLayout(ScrollableControl view)
		{
			this.view = view;
			designAutoScroll = view.AutoScroll;
			placed = view.Controls.Cast<Control>()
				.Select(c => new Placed(c, c.Bounds, c.Anchor))
				.OrderBy(p => p.Bounds.Top)
				.ThenBy(p => p.Bounds.Left)
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
		/// Returns the card layout of a view, or null when the view needs none.
		/// </summary>
		/// <param name="view">A dashboard view.</param>
		/// <returns>
		/// The layout of the view's 12-column table when it has one; a stacking layout when its controls are placed by
		/// location; null when the controls are docked or anchored to follow the view, or the view is empty.
		/// </returns>
		public static CardLayout For(ScrollableControl view)
		{
			var table = view.Controls.OfType<TableLayoutPanel>().FirstOrDefault(t => t.ColumnCount == Columns);
			if (table != null)
				return new CardLayout(table);

			// Placed by location: not docked, and anchored top-left only, so the designed bounds hold at any view size.
			bool byLocation = view.Controls.Count > 0 && view.Controls.Cast<Control>()
				.All(c => c.Dock == DockStyle.None && c.Anchor == (AnchorStyles.Top | AnchorStyles.Left));
			return byLocation ? new CardLayout(view) : null;
		}

		/// <summary>
		/// Arranges the cards for a client profile: the designed layout for Default or any profile other than Tablet and
		/// Phone.
		/// </summary>
		/// <param name="profile">Name of the active client profile.</param>
		public void Apply(string profile)
		{
			bool phone = profile == "Phone";
			bool narrow = phone || profile == "Tablet";

			// The designed layout is applied once. A narrow one is computed again each time it is asked for: for a view
			// created under a narrow profile, a card's responsive MinimumSize arrives after the first call.
			if (profile == applied && !narrow)
				return;

			var container = (Control)table ?? view;
			container.SuspendLayout();
			try
			{
				if (table == null)
					Stack(phone, narrow);
				else if (narrow)
					Reflow(phone);
				else
					Restore();
			}
			finally
			{
				container.ResumeLayout(true);
			}

			applied = profile;
		}

		#endregion

		#region Implementation

		private sealed record Card(Control Control, TableLayoutPanelCellPosition Cell, int ColumnSpan, int RowSpan, Padding Margin);

		private sealed record Placed(Control Control, Rectangle Bounds, AnchorStyles Anchor);

		private const int Columns = 12;

		// Gap between stacked controls of a view placed by location.
		private const int StackGap = 12;

		private readonly ScrollableControl view;
		private readonly TableLayoutPanel table;
		private readonly Card[] cards;
		private readonly Placed[] placed;
		private readonly RowStyle[] designRows;
		private readonly DockStyle designDock;
		private readonly int designHeight;
		private readonly Padding designPadding;
		private readonly bool designAutoScroll;
		private string applied = "Default";

		private void Reflow(bool phone)
		{
			var rows = new List<float>();

			// The right margin of the cards that end a designed row; the cards that end a reflowed row take it.
			var edgeCards = cards.Where(c => c.Cell.Column + c.ColumnSpan == Columns).ToArray();
			int edgeMargin = edgeCards.Length > 0 ? edgeCards.Min(c => c.Margin.Right) : -1;

			foreach (var designRow in cards.GroupBy(c => c.Cell.Row))
			{
				// A card can ask for more height on a profile through its responsive MinimumSize.
				float minimum = designRow.Max(c => c.Control.MinimumSize.Height + c.Margin.Vertical);

				// KPI tiles, two to a row on a tablet; anything wider takes a row of its own.
				bool tiles = !phone && designRow.All(c => c.ColumnSpan <= 3);
				int perRow = tiles ? 2 : 1;
				int span = Columns / perRow;

				int index = 0;
				int count = designRow.Count();
				foreach (var card in designRow)
				{
					// A card that spans several designed rows keeps their combined height.
					float height = System.Math.Max(minimum, Enumerable.Range(card.Cell.Row, card.RowSpan).Sum(RowHeight));
					if (index % perRow == 0)
						rows.Add(height);
					else
						rows[rows.Count - 1] = System.Math.Max(rows[rows.Count - 1], height);

					Place(card.Control, (index % perRow) * span, rows.Count - 1, span, 1);

					bool endsRow = index % perRow == perRow - 1 || index == count - 1;
					if (endsRow && edgeMargin >= 0)
						card.Control.Margin = new Padding(card.Margin.Left, card.Margin.Top, edgeMargin, card.Margin.Bottom);
					else
						card.Control.Margin = card.Margin;

					index++;
				}
			}

			SetRows(rows.Select(h => new RowStyle(SizeType.Absolute, h)));

			// Narrow screens get a narrower frame around the cards, never a wider one than designed.
			int frame = phone ? 8 : 12;
			table.Padding = new Padding(
				System.Math.Min(designPadding.Left, frame), System.Math.Min(designPadding.Top, frame),
				System.Math.Min(designPadding.Right, frame), System.Math.Min(designPadding.Bottom, frame));
			table.Dock = DockStyle.Top;
			table.Height = (int)rows.Sum() + table.Padding.Vertical;
			view.AutoScroll = true;
		}

		private void Restore()
		{
			foreach (var card in cards)
			{
				Place(card.Control, card.Cell.Column, card.Cell.Row, card.ColumnSpan, card.RowSpan);
				card.Control.Margin = card.Margin;
			}

			SetRows(designRows.Select(r => new RowStyle(r.SizeType, r.Height)));

			table.Padding = designPadding;
			table.Dock = designDock;
			table.Height = designHeight;
			view.AutoScroll = designAutoScroll;
		}

		// Phone: one control per row at the view's width, in reading order. Tablet and Default: the designed bounds;
		// a tablet may be narrower than the design, so there the view scrolls.
		private void Stack(bool phone, bool narrow)
		{
			int top = 0;
			foreach (var item in placed)
			{
				if (phone)
				{
					// Anchored left and right, the control follows the view's width from here on.
					item.Control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					item.Control.Bounds = new Rectangle(0, top, view.ClientSize.Width, item.Bounds.Height);
					top += item.Bounds.Height + StackGap;
				}
				else
				{
					item.Control.Anchor = item.Anchor;
					item.Control.Bounds = item.Bounds;
				}
			}

			view.AutoScroll = narrow || designAutoScroll;
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
