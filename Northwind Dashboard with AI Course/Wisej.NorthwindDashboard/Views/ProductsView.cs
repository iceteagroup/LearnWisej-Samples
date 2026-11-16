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
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Product catalog with stock positions, category and status filters, and reorder suggestions.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Purchase orders placed here add to the session's units on order; the shared Northwind data is not changed.
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
		}

		#endregion

		#region Implementation

		private readonly ChipLabel[] statusChips;
		private List<ProductRow> rows = new List<ProductRow>();
		private bool updating;

		private void UpdateInventory()
		{
			int Count(StockStatus status) => rows.Count(r => r.StockStatus == status);
			int inStock = Count(StockStatus.InStock), incoming = Count(StockStatus.Incoming), reorder = Count(StockStatus.Reorder);
			int outOfStock = Count(StockStatus.OutOfStock), discontinued = Count(StockStatus.Discontinued);

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

		private void PlacePurchaseOrder(Product product)
		{
			if (product.Discontinued)
			{
				AlertBox.Show(product.Name + " is discontinued and can no longer be ordered.", MessageBoxIcon.Warning, allowHtml: false);
				return;
			}

			int units = Dashboard.SuggestedOrderQuantity(product);
			Dashboard.PlacePurchaseOrder(product, units);
			AlertBox.Show("Purchase order for " + units + " × " + product.Name + " sent to " + product.Supplier.Company + ".", MessageBoxIcon.Information, allowHtml: false);
		}

		private void statusChip_SelectedChanged(object sender, EventArgs e) => ApplyFilters();

		private void categoryFilter_SelectionChanged(object sender, EventArgs e) => ApplyFilters();

		private void colActions_ActionClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (!(e.DataBoundItem is ProductRow row))
				return;

			if (e.Action.Name == "reorder")
				PlacePurchaseOrder(row.Product);
			else
				Dashboard.Navigate(DashboardArea.Orders, row.Product);
		}

		private void reorderList_CardCreated(object sender, ActionCardListItemEventArgs e)
		{
			// One command keeps room for the text in a narrow card; the card itself opens the product's orders.
			var suggestion = (Suggestion)e.DataItem;
			e.Card.Actions.Add(new CardAction { Name = "order", Text = "Order " + suggestion.Quantity, Primary = true });
			e.Card.ActionClick += (s, args) => PlacePurchaseOrder(suggestion.Product);
			e.Card.Click += (s, args) => Dashboard.Navigate(DashboardArea.Orders, suggestion.Product);
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
