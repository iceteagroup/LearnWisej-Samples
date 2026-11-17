using System;
using System.Collections.Generic;
using System.Linq;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 5 products view: the catalog with stock positions, category and status filters, and the reorder
	/// suggestions as an <see cref="ActionCardList"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The list creates one <see cref="ActionCard"/> per suggestion. <c>CardCreated</c> is where the application adds
	/// the card's <see cref="CardAction"/> and subscribes to the card: <c>ActionClick</c> places a purchase order,
	/// <c>Click</c> on the card itself opens the product's orders. They are two intents with two handlers.
	/// </para>
	/// <para>
	/// A purchase order adds to the session's units on order through <see cref="DashboardContext.PlacePurchaseOrder"/>.
	/// The stock on hand and the shared Northwind data do not change: ordered is not received.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Products, category);
	/// </code>
	/// </example>
	public partial class ProductsView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public ProductsView()
		{
			InitializeComponent();

			// Stock coverage is units in stock divided by the reorder level: at or below 1 needs a reorder.
			colStock.Thresholds.Add(new MeterThreshold(0, MeterTone.Danger));
			colStock.Thresholds.Add(new MeterThreshold(0.01, MeterTone.Warning));
			colStock.Thresholds.Add(new MeterThreshold(1.01, MeterTone.Success));

			statusChips = new[] { inStockChip, incomingChip, reorderChip, outOfStockChip, discontinuedChip };
			inventoryCard.Resize += (s, e) => PlaceStockChips();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Filters the catalog to a category, or selects a product.
		/// </summary>
		/// <param name="target">A <see cref="Category"/> or <see cref="Product"/>.</param>
		public override void ShowTarget(object target)
		{
			if (Dashboard == null)
				return;

			updating = true;
			try
			{
				foreach (var chip in statusChips)
					chip.Selected = false;

				for (int i = 0; i < categoryFilter.Items.Count; i++)
					categoryFilter.SetSelected(i, target is Category category && (int)categoryFilter.Items[i].Value == category.Id);
			}
			finally
			{
				updating = false;
			}

			ApplyFilters();

			if (target is Product product)
			{
				for (int i = 0; i < productsGrid.RowCount; i++)
				{
					if (productsGrid.Rows[i].DataBoundItem is ProductRow row && row.Product == product)
					{
						productsGrid.CurrentCell = productsGrid.Rows[i].Cells[0];
						productsGrid.Rows[i].Selected = true;
						productsGrid.ScrollRowIntoView(i);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Reloads the catalog, inventory summary and reorder suggestions.
		/// </summary>
		protected override void RefreshView()
		{
			if (categoryFilter.Items.Count == 0)
			{
				foreach (var category in Dashboard.Database.Categories)
					categoryFilter.Items.Add(new Segment { Text = category.Name, Value = category.Id, ToolTipText = category.Description });
			}

			rows = SalesAnalytics.ProductSales(Dashboard).Select(s => new ProductRow(s, Dashboard)).ToList();
			UpdateInventory();
			UpdateSuggestions();
			ApplyFilters();

			if (Visible && !purchasing)
				PostStatus(LoadStatus);
		}

		/// <summary>
		/// Names the reorder list's event path in the status strip when the view is shown.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible && Dashboard != null)
				PostStatus(LoadStatus);
		}

		#endregion

		#region Implementation

		private const string LoadStatus = "ActionCardList.CardCreated → CardAction";

		private readonly ChipLabel[] statusChips;
		private List<ProductRow> rows = new List<ProductRow>();
		private bool updating;
		private bool purchasing;

		private void UpdateInventory()
		{
			int Count(StockStatus status) => rows.Count(r => r.StockStatus == status);
			int inStock = Count(StockStatus.InStock), incoming = Count(StockStatus.Incoming), reorder = Count(StockStatus.Reorder);
			int outOfStock = Count(StockStatus.OutOfStock), discontinued = Count(StockStatus.Discontinued);

			// Segments are rebuilt on every refresh, so the collection is cleared first.
			inventoryMeter.Maximum = Math.Max(1, rows.Count);
			inventoryMeter.Value = rows.Count;
			inventoryMeter.Segments.Clear();
			inventoryMeter.Segments.Add(new MeterSegment("In stock", inStock, MeterTone.Success));
			inventoryMeter.Segments.Add(new MeterSegment("Restock incoming", incoming, MeterTone.Primary));
			inventoryMeter.Segments.Add(new MeterSegment("Reorder", reorder, MeterTone.Warning));
			inventoryMeter.Segments.Add(new MeterSegment("Out of stock", outOfStock, MeterTone.Danger));
			inventoryMeter.Segments.Add(new MeterSegment("Discontinued", discontinued, MeterTone.Neutral));
			inventoryMeter.ValueText = rows.Count + " products · " + DashboardStyle.Money(rows.Where(r => !r.Product.Discontinued).Sum(r => r.UnitPrice * r.UnitsInStock)) + " stock at list price";

			inStockChip.Text = inStock + " in stock";
			incomingChip.Text = incoming + " incoming";
			reorderChip.Text = reorder + " to reorder";
			outOfStockChip.Text = outOfStock + " out of stock";
			discontinuedChip.Text = discontinued + " discontinued";

			int attention = reorder + outOfStock;
			attentionKpi.Value = attention == 1 ? "1 product" : attention + " products";
			attentionKpi.Caption = reorder + " to reorder · " + outOfStock + " out of stock" + Environment.NewLine + incoming + " covered by open purchase orders";
			attentionKpi.Tone = outOfStock > 0 ? KpiTone.Danger : attention > 0 ? KpiTone.Warning : KpiTone.Success;
		}

		// Products at or below their reorder level: out of stock first, then the ones nothing is on order for.
		private void UpdateSuggestions()
		{
			var items = rows
				.Where(r => r.StockStatus == StockStatus.OutOfStock || r.StockStatus == StockStatus.Reorder || r.StockStatus == StockStatus.Incoming)
				.OrderBy(r => r.StockStatus == StockStatus.OutOfStock ? 0 : r.StockStatus == StockStatus.Reorder ? 1 : 2)
				.ThenBy(r => r.StockCoverage)
				.Select(r => new Suggestion(r, Dashboard))
				.ToList();

			reorderList.DataSource = items;
			reorderCard.Text = "Reorder suggestions · " + items.Count;
		}

		private void ApplyFilters()
		{
			if (updating)
				return;

			var statuses = statusChips.Where(c => c.Selected).Select(c => Enum.Parse<StockStatus>((string)c.Tag)).ToHashSet();
			var categories = categoryFilter.SelectedItems.Select(s => (int)s.Value).ToHashSet();
			var byStatus = rows.Where(r => statuses.Count == 0 || statuses.Contains(r.StockStatus)).ToList();

			foreach (var item in categoryFilter.Items)
				item.Count = byStatus.Count(r => r.Product.CategoryId == (int)item.Value);

			var visible = byStatus.Where(r => categories.Count == 0 || categories.Contains(r.Product.CategoryId)).ToList();
			productsGrid.DataSource = visible;
			productsGrid.CurrentCell = null;
			catalogCard.Text = "Product catalog · " + (visible.Count == rows.Count ? rows.Count + " products" : visible.Count + " of " + rows.Count + " products") +
				" · sales " + Dashboard.Period.Name.ToLowerInvariant();
		}

		// The context raises ProductsChanged, which refreshes this view: the new units on order come back from the
		// context, not from the card that was clicked.
		private void PlacePurchaseOrder(Product product, string status)
		{
			if (product.Discontinued)
			{
				AlertBox.Show(product.Name + " is discontinued and can no longer be ordered.", MessageBoxIcon.Warning, allowHtml: false);
				return;
			}

			int units = Dashboard.SuggestedOrderQuantity(product);
			purchasing = true;
			try
			{
				Dashboard.PlacePurchaseOrder(product, units);
			}
			finally
			{
				purchasing = false;
			}

			SetStatus(status);
			AlertBox.Show("Purchase order for " + units + " × " + product.Name + " sent to " + product.Supplier.Company + ".", MessageBoxIcon.Information, allowHtml: false);
		}

		// The five stock chips are placed by location, in one row at the designed width. In a narrower card they wrap;
		// the Tablet and Phone profiles give the card the height of the extra rows (MinimumSize in ProductsView.resx).
		private void PlaceStockChips()
		{
			const int Left = 16, Top = 56, Gap = 8, RowPitch = 34;

			int right = inventoryCard.ClientSize.Width - Left;
			int x = Left, y = Top;
			foreach (var chip in statusChips)
			{
				if (x > Left && x + chip.Width > right)
				{
					x = Left;
					y += RowPitch;
				}

				chip.Location = new System.Drawing.Point(x, y);
				x += chip.Width + Gap;
			}
		}

		// The shell writes its own line after a navigation or a period change returns; posting ours makes the
		// view's line the one that stays.
		private void PostStatus(string text) => Application.Post(() => SetStatus(text));

		private void statusChip_SelectedChanged(object sender, EventArgs e) => ApplyFilters();

		private void categoryFilter_SelectionChanged(object sender, EventArgs e) => ApplyFilters();

		private void colActions_ActionClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (!(e.DataBoundItem is ProductRow row))
				return;

			if (e.Action.Name == "reorder")
				PlacePurchaseOrder(row.Product, "DataGridViewActionsColumn.ActionClick → context.PlacePurchaseOrder");
			else
			{
				SetStatus("DataGridViewActionsColumn.ActionClick → Navigate(Orders, product)");
				Navigate(DashboardArea.Orders, row.Product);
			}
		}

		private void reorderList_CardCreated(object sender, ActionCardListItemEventArgs e)
		{
			// Each card captures its own suggestion. One command keeps room for the text in a narrow card; the card
			// itself opens the product's orders.
			var suggestion = (Suggestion)e.DataItem;
			e.Card.Actions.Add(new CardAction { Name = "order", Text = "Order " + suggestion.Quantity, Primary = true });
			e.Card.ActionClick += (s, args) => PlacePurchaseOrder(suggestion.Product, "ActionCard.ActionClick → context.PlacePurchaseOrder");
			e.Card.Click += (s, args) =>
			{
				SetStatus("ActionCard.Click → Navigate(Orders, product)");
				Navigate(DashboardArea.Orders, suggestion.Product);
			};
		}

		// Card record for the reorder list; property names follow ActionCardList's conventional display members.
		private sealed class Suggestion
		{
			public Suggestion(ProductRow row, DashboardContext context)
			{
				Product = row.Product;
				Quantity = context.SuggestedOrderQuantity(Product);
				Title = Product.Name;
				Description = Product.UnitsInStock + " in stock · reorder at " + Product.ReorderLevel +
					(row.UnitsOnOrder > 0 ? " · " + row.UnitsOnOrder + " on order" : "");
				Meta = Product.Supplier.Company + " · " + Product.Supplier.Phone;
				Badge = row.StatusText;
				BadgeTone = row.StockStatus switch
				{
					StockStatus.OutOfStock => ActionCardTone.Danger,
					StockStatus.Reorder => ActionCardTone.Warning,
					_ => ActionCardTone.Primary
				};
			}

			public Product Product { get; }

			public int Quantity { get; }

			public string Title { get; }

			public string Description { get; }

			public string Meta { get; }

			public string Badge { get; }

			public ActionCardTone BadgeTone { get; }
		}

		#endregion

	}
}
