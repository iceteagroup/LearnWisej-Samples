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
using Wisej.NorthwindDashboard.Data;
using Wisej.NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Order list with status filters, search, an advanced query builder and drill-down from other views.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Orders are limited to the reporting period, except when another view opens the list for a specific day.
	/// Drill-down targets (a day, customer, product or sales rep) appear in the card header with a clear button.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Orders, new DateTime(1998, 5, 4));
	/// </code>
	/// </example>
	public partial class OrdersView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public OrdersView()
		{
			InitializeComponent();

			clearTool = new ComponentTool { Name = "clear", ImageSource = DashboardStyle.Icon("close"), ToolTipText = "Show all orders again", Visible = false };
			gridCard.Tools.Add(clearTool);

			// The placeholder overlays the grid whenever the filters leave no rows.
			emptyState = new EmptyState
			{
				Kind = EmptyStateKind.NoResults,
				Title = "No matching orders",
				Description = "Try another status, search text or filter rule.",
				ActionText = "Clear filters",
				TargetControl = ordersGrid
			};
			emptyState.ActionClick += emptyState_ActionClick;

			statusFilter.SelectedIndex = 0;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows the orders of a day, customer, product or sales rep.
		/// </summary>
		/// <param name="target">A <see cref="DateTime"/>, <see cref="Customer"/>, <see cref="Product"/> or <see cref="Employee"/>.</param>
		public override void ShowTarget(object target)
		{
			this.target = target is DateTime || target is Customer || target is Product || target is Employee ? target : null;
			statusFilter.SelectedIndex = 0;
			ApplyFilters();
		}

		/// <summary>
		/// Prepares the filter fields on first use and reloads the list.
		/// </summary>
		protected override void RefreshView()
		{
			if (queryBuilder.Fields.Count == 0)
				CreateQueryFields();

			ApplyFilters();
		}

		/// <summary>
		/// Gives the toolbar a second row on a tablet or a phone.
		/// </summary>
		/// <param name="e">Event data.</param>
		/// <remarks>
		/// The Tablet and Phone profiles (OrdersView.resx) dock the status chips above the search box and the filter
		/// button; the chips wrap to two lines on a phone. The height of the toolbar's row is a row style, which
		/// responsive properties do not cover.
		/// </remarks>
		protected override void OnResponsiveProfileChanged(ResponsiveProfileChangedEventArgs e)
		{
			base.OnResponsiveProfileChanged(e);

			layout.RowStyles[0].Height = e.CurrentProfile.Name switch
			{
				"Phone" => 124,
				"Tablet" => 92,
				_ => 60
			};
		}

		#endregion

		#region Implementation

		private readonly EmptyState emptyState;
		private readonly ComponentTool clearTool;
		private object target;

		private void CreateQueryFields()
		{
			var db = Dashboard.Database;
			queryBuilder.Fields.Add(new FilterField("Customer", "Customer"));
			queryBuilder.Fields.Add(new FilterField("Country", "Country", FilterFieldType.Choice, db.Customers.Select(c => c.Country).Distinct().OrderBy(c => c).ToArray()));
			queryBuilder.Fields.Add(new FilterField("SalesRep", "Sales rep", FilterFieldType.Choice, db.Employees.Select(e => e.FullName).ToArray()));
			queryBuilder.Fields.Add(new FilterField("Shipper", "Shipper", FilterFieldType.Choice, db.Shippers.Select(s => s.Company).ToArray()));
			queryBuilder.Fields.Add(new FilterField("Status", "Status", FilterFieldType.Choice, Enum.GetValues<OrderStatus>().Select(DashboardStyle.StatusText).ToArray()));
			queryBuilder.Fields.Add(new FilterField("Amount", "Amount ($)", FilterFieldType.Number));
			queryBuilder.Fields.Add(new FilterField("Freight", "Freight ($)", FilterFieldType.Number));
			queryBuilder.Fields.Add(new FilterField("Lines", "Order lines", FilterFieldType.Number));
			queryBuilder.Fields.Add(new FilterField("OrderDate", "Order date", FilterFieldType.Date));
		}

		private void ApplyFilters()
		{
			if (Dashboard == null)
				return;

			var period = Dashboard.Period;
			string search = searchBox.Text.Trim();
			var rows = Dashboard.Database.Orders
				.Where(o => target is DateTime day ? o.OrderDate.Date == day.Date : period.Contains(o.OrderDate))
				.Where(MatchesTarget)
				.Where(o => MatchesSearch(o, search))
				.Select(o => new OrderRow(o, Dashboard))
				.Where(r => Evaluate(queryBuilder.Root, r))
				.ToList();

			// Counts reflect every filter except the status chips themselves.
			statusFilter.Items[0].Count = rows.Count;
			for (int i = 1; i < statusFilter.Items.Count; i++)
			{
				var status = Enum.Parse<OrderStatus>((string)statusFilter.Items[i].Value);
				statusFilter.Items[i].Count = rows.Count(r => r.Status == status);
			}

			var selected = statusFilter.SelectedItem?.Value as string;
			var visible = rows
				.Where(r => selected == null || selected == "All" || r.Status.ToString() == selected)
				.OrderByDescending(r => r.OrderDate)
				.ThenByDescending(r => r.Id)
				.ToList();

			ordersGrid.DataSource = visible;
			ordersGrid.CurrentCell = null;

			summaryLabel.Text = "Showing " + DashboardStyle.Count(visible.Count) + (visible.Count == 1 ? " order" : " orders") +
				" · " + DashboardStyle.Money(visible.Sum(r => r.Amount)) + " net · " + TargetScope(period);

			gridCard.Text = TargetTitle();
			clearTool.Visible = target != null;

			int rules = CountRules(queryBuilder.Root);
			advancedButton.Text = rules == 0 ? "Advanced filter" : "Advanced filter (" + rules + ")";
		}

		private bool MatchesTarget(Order order) => target switch
		{
			Customer customer => order.Customer == customer,
			Product product => order.Lines.Any(l => l.Product == product),
			Employee employee => order.Employee == employee,
			_ => true
		};

		private static bool MatchesSearch(Order order, string search)
		{
			if (search.Length == 0)
				return true;

			var comparison = StringComparison.CurrentCultureIgnoreCase;
			return search.Split(' ', StringSplitOptions.RemoveEmptyEntries).All(word =>
				order.Id.ToString(CultureInfo.InvariantCulture).Contains(word.TrimStart('#'), StringComparison.Ordinal) ||
				order.Customer.Company.Contains(word, comparison) ||
				order.Customer.Country.Contains(word, comparison) ||
				order.Customer.City.Contains(word, comparison) ||
				order.Employee.FullName.Contains(word, comparison) ||
				order.Shipper.Company.Contains(word, comparison) ||
				order.Lines.Any(l => l.Product.Name.Contains(word, comparison)));
		}

		private string TargetTitle() => target switch
		{
			DateTime day => "Orders placed on " + DashboardStyle.Date(day),
			Customer customer => "Orders from " + customer.Company,
			Product product => "Orders containing " + product.Name,
			Employee employee => "Orders booked by " + employee.FullName,
			_ => "Orders"
		};

		private string TargetScope(DashboardPeriod period) =>
			target is DateTime ? "all periods" : period.Name.ToLowerInvariant() + " (" + period.RangeText + ")";

		// Evaluates the query builder tree; incomplete rules are ignored rather than excluding every order.
		private static bool Evaluate(QueryGroup group, OrderRow row)
		{
			var results = group.Rules.Select(rule => Evaluate(rule, row)).Where(result => result.HasValue).Select(result => result.Value)
				.Concat(group.Groups.Where(g => CountRules(g) > 0).Select(g => Evaluate(g, row)))
				.ToArray();

			if (results.Length == 0)
				return true;

			return group.Logic == QueryLogic.And ? results.All(r => r) : results.Any(r => r);
		}

		private static bool? Evaluate(QueryRule rule, OrderRow row)
		{
			object value = rule.Field switch
			{
				"Customer" => row.Customer,
				"Country" => row.Country,
				"SalesRep" => row.SalesRep,
				"Shipper" => row.Shipper,
				"Status" => row.StatusText,
				"Amount" => row.Amount,
				"Freight" => row.Freight,
				"Lines" => (decimal)row.Items,
				"OrderDate" => row.OrderDate,
				_ => null
			};

			if (rule.Operator == QueryOperator.IsNull)
				return value == null || value as string == "";

			if (rule.Operator == QueryOperator.IsNotNull)
				return !(value == null || value as string == "");

			if (string.IsNullOrWhiteSpace(rule.Value))
				return null;

			int? order = value switch
			{
				decimal number when decimal.TryParse(rule.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var limit) => number.CompareTo(limit),
				DateTime date when DateTime.TryParseExact(rule.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day) => date.Date.CompareTo(day),
				string text => string.Compare(text, rule.Value, StringComparison.CurrentCultureIgnoreCase),
				_ => null
			};

			string textValue = Convert.ToString(value, CultureInfo.CurrentCulture) ?? "";
			return rule.Operator switch
			{
				QueryOperator.Equal => order == 0,
				QueryOperator.NotEqual => order != 0,
				QueryOperator.GreaterThan => order > 0,
				QueryOperator.GreaterThanOrEqual => order >= 0,
				QueryOperator.LessThan => order < 0,
				QueryOperator.LessThanOrEqual => order <= 0,
				QueryOperator.Contains => textValue.Contains(rule.Value, StringComparison.CurrentCultureIgnoreCase),
				QueryOperator.StartsWith => textValue.StartsWith(rule.Value, StringComparison.CurrentCultureIgnoreCase),
				_ => null
			};
		}

		private static int CountRules(QueryGroup group) => group.Rules.Count + group.Groups.Sum(CountRules);

		private void ShowQuery(bool show)
		{
			queryCard.Visible = show;
			layout.RowStyles[1].Height = show ? 300 : 0;
		}

		private void ClearFilters()
		{
			target = null;
			searchBox.Text = "";
			queryBuilder.Root.Rules.Clear();
			queryBuilder.Root.Groups.Clear();
			statusFilter.SelectedIndex = 0;
			ApplyFilters();
		}

		private OrderRow RowAt(int index) => index >= 0 && index < ordersGrid.RowCount ? ordersGrid.Rows[index].DataBoundItem as OrderRow : null;

		private void statusFilter_SelectionChanged(object sender, EventArgs e) => ApplyFilters();

		private void searchBox_TextChanged(object sender, EventArgs e) => ApplyFilters();

		private void queryBuilder_QueryChanged(object sender, EventArgs e) => ApplyFilters();

		private void advancedButton_Click(object sender, EventArgs e) => ShowQuery(!queryCard.Visible);

		private void queryCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (e.Tool.Name == "reset")
			{
				queryBuilder.Root.Rules.Clear();
				queryBuilder.Root.Groups.Clear();
				ApplyFilters();
			}
			else
			{
				ShowQuery(false);
			}
		}

		private void gridCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (e.Tool.Name == "clear")
			{
				target = null;
				ApplyFilters();
			}
		}

		private void emptyState_ActionClick(object sender, EventArgs e) => ClearFilters();

		private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (RowAt(e.RowIndex) is OrderRow row)
				OrderDialog.ShowOrder(Dashboard, row.Order);
		}

		private void colCustomer_AvatarClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (e.DataBoundItem is OrderRow row)
				Dashboard.Navigate(DashboardArea.Customers, row.Order.Customer);
		}

		private void colSalesRep_AvatarClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (e.DataBoundItem is OrderRow row)
				Dashboard.Navigate(DashboardArea.Team, row.Order.Employee);
		}

		private void colActions_ActionClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (!(e.DataBoundItem is OrderRow row))
				return;

			if (e.Action.Name == "view")
			{
				OrderDialog.ShowOrder(Dashboard, row.Order);
			}
			else if (row.ShippedDate != null)
			{
				AlertBox.Show("Order " + row.Number + " already shipped on " + DashboardStyle.Date(row.ShippedDate.Value) + ".", MessageBoxIcon.Information);
			}
			else
			{
				Dashboard.SetStage(row.Order, FulfillmentStage.Shipped);
				AlertBox.Show("Order " + row.Number + " marked as shipped via " + row.Shipper + ".", MessageBoxIcon.Information);
			}
		}

		#endregion

	}
}
