using System;
using System.Globalization;
using System.Linq;
using NorthwindDashboard.Data;
using NorthwindDashboard.Dialogs;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 4 orders workbench: status chips with counts, search, a typed query builder, a grid that uses the six new
	/// column types, an empty-state placeholder, and the route into the order dialog.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="ApplyFilters"/> is the one place that decides which orders are listed. Its stages run in a fixed
	/// order: scope, target, search, rules, chip counts, selected status, sort.
	/// </para>
	/// <para>
	/// Orders are limited to the reporting period, except when another view opens the list for one day: a day is an
	/// explicit scope across all periods, and the scope chip and the summary line say so. A customer, product or sales
	/// rep target refines the current period instead.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Orders, new DateTime(1998, 5, 6));
	/// context.Navigate(DashboardArea.Orders, context.Database.FindCustomer("AROUT"));
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

			// The placeholder covers the grid whenever the filters leave no rows.
			emptyState = new EmptyState
			{
				Kind = EmptyStateKind.NoResults,
				Title = "No orders match",
				Description = "Try another status, search text or filter rule.",
				ActionText = "Clear filters",
				TargetControl = ordersGrid
			};
			emptyState.ActionClick += emptyState_ActionClick;

			Quietly(() => statusFilter.SelectedIndex = 0);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows the orders of a day, customer, product or sales rep.
		/// </summary>
		/// <param name="target">A <see cref="DateTime"/>, <see cref="Customer"/>, <see cref="Product"/> or <see cref="Employee"/>.</param>
		/// <remarks>
		/// The status strip is left alone: the view that sent the target has already written the event path.
		/// </remarks>
		public override void ShowTarget(object target)
		{
			this.target = target is DateTime || target is Customer || target is Product || target is Employee ? target : null;
			targetArrived = true;
			Quietly(() => statusFilter.SelectedIndex = 0);
			ApplyFilters();
		}

		/// <summary>
		/// Prepares the filter fields on first use and reloads the list.
		/// </summary>
		protected override void RefreshView()
		{
			if (queryBuilder.Fields.Count == 0)
				Quietly(CreateQueryFields);

			ApplyFilters();
		}

		/// <summary>
		/// Names the filter pipeline in the status strip when the view is shown without a target.
		/// </summary>
		/// <param name="e">Event data.</param>
		/// <remarks>
		/// The shell writes its own navigation text after it shows a view, and a target arrives after the view is
		/// visible, so the text is posted to the end of the request and dropped when a target came with it.
		/// </remarks>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (!Visible || Dashboard == null)
				return;

			targetArrived = false;
			Application.Post(() =>
			{
				if (!targetArrived)
					SetStatus(PipelineStatus);
			});
		}

		#endregion

		#region Implementation

		private const string PipelineStatus = "Candidates → counts → selected status";

		private readonly EmptyState emptyState;
		private object target;
		private bool quiet;
		private bool targetArrived;

		// Runs a change to the filter controls made by code, so their events neither filter twice nor claim the status strip.
		private void Quietly(Action change)
		{
			quiet = true;
			try
			{
				change();
			}
			finally
			{
				quiet = false;
			}
		}

		// The field type decides the editor and the operators a rule offers; Evaluate compares by the same type.
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

			// Stages 1-4: scope (the period, or the explicit day), target, search, rules. These are the candidates.
			var period = Dashboard.Period;
			string search = searchBox.Text.Trim();
			var rows = Dashboard.Database.Orders
				.Where(o => target is DateTime day ? o.OrderDate.Date == day.Date : period.Contains(o.OrderDate))
				.Where(MatchesTarget)
				.Where(o => MatchesSearch(o, search))
				.Select(o => new OrderRow(o, Dashboard))
				.Where(r => Evaluate(queryBuilder.Root, r))
				.ToList();

			// Stage 5: the counts come from the candidates, before the selected chip narrows the list, so every other
			// chip keeps showing what it would list.
			statusFilter.Items[0].Count = rows.Count;
			for (int i = 1; i < statusFilter.Items.Count; i++)
			{
				var status = Enum.Parse<OrderStatus>((string)statusFilter.Items[i].Value);
				statusFilter.Items[i].Count = rows.Count(r => r.Status == status);
			}

			// Stages 6-7: the selected status, then newest first; the order number settles equal dates.
			var selected = statusFilter.SelectedItem?.Value as string;
			var visible = rows
				.Where(r => selected == null || selected == "All" || r.Status.ToString() == selected)
				.OrderByDescending(r => r.OrderDate)
				.ThenByDescending(r => r.Id)
				.ToList();

			ordersGrid.DataSource = visible;
			ordersGrid.CurrentCell = null;
			ordersGrid.ClearSelection();

			summaryLabel.Text = "Showing " + DashboardStyle.Count(visible.Count) + (visible.Count == 1 ? " order" : " orders") +
				" · " + DashboardStyle.Money(visible.Sum(r => r.Amount)) + " net · " + TargetScope(period);

			gridCard.Text = TargetTitle();
			ShowScope(period);

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

		// The first chip names the scope: the period, or the day that replaces it. A day chip can be closed. The second
		// chip names the customer, product or sales rep that refines the period.
		private void ShowScope(DashboardPeriod period)
		{
			scopeChip.Text = target is DateTime day
				? "Day: " + day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
				: "Period: " + period.Name;
			scopeChip.Closable = target is DateTime;
			scopeChip.Width = ChipWidth(scopeChip);

			string text = target switch
			{
				Customer customer => "Customer: " + customer.Company,
				Product product => "Product: " + product.Name,
				Employee employee => "Sales rep: " + employee.FullName,
				_ => null
			};
			targetChip.Visible = text != null;
			if (text != null)
			{
				targetChip.Text = text;
				targetChip.Width = ChipWidth(targetChip);
			}
		}

		// A ChipLabel fills its bounds, so the bounds follow the caption: about 6 pixels per character of the
		// 13-pixel font, the pill's padding, and the close button when there is one.
		private static int ChipWidth(ChipLabel chip) => (int)(chip.Text.Length * 5.8) + 26 + (chip.Closable ? 22 : 0);

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

		// One rule. The row property decides the comparison: a decimal compares as a number, a DateTime as a day, a
		// string as text. Null means "this rule cannot be evaluated yet" (no value, or a value of the wrong type).
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
			layout.RowStyles[1].Height = show ? 270 : 0;
		}

		// Resets the filter state and the controls that show it, so what the user sees and what is applied agree.
		private void ClearFilters()
		{
			target = null;
			Quietly(() =>
			{
				searchBox.Text = "";
				queryBuilder.Root.Rules.Clear();
				queryBuilder.Root.Groups.Clear();
				statusFilter.SelectedIndex = 0;
			});
			ApplyFilters();
		}

		// The bound object of a grid row. A row index is a position on screen: sorting and filtering change it.
		private OrderRow RowAt(int index) => index >= 0 && index < ordersGrid.RowCount ? ordersGrid.Rows[index].DataBoundItem as OrderRow : null;

		private void Open(OrderRow row, string source)
		{
			SetStatus(source + " → bound row → OrderDialog");
			OrderDialog.ShowOrder(Dashboard, row.Order);
		}

		private void statusFilter_SelectionChanged(object sender, EventArgs e)
		{
			if (quiet)
				return;

			ApplyFilters();
			SetStatus("ChipGroup.SelectionChanged → ApplyFilters");
		}

		private void searchBox_TextChanged(object sender, EventArgs e)
		{
			if (quiet)
				return;

			ApplyFilters();
			SetStatus(PipelineStatus);
		}

		private void queryBuilder_QueryChanged(object sender, EventArgs e)
		{
			if (quiet)
				return;

			ApplyFilters();
			SetStatus("QueryBuilder rules → typed comparison");
		}

		private void advancedButton_Click(object sender, EventArgs e) => ShowQuery(!queryCard.Visible);

		private void queryCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (e.Tool.Name == "reset")
			{
				Quietly(() =>
				{
					queryBuilder.Root.Rules.Clear();
					queryBuilder.Root.Groups.Clear();
				});
				ApplyFilters();
				SetStatus("QueryBuilder rules → typed comparison");
			}
			else
			{
				ShowQuery(false);
			}
		}

		// Either chip's close button removes the navigation target; the status, search and rules stay.
		private void targetChip_CloseClick(object sender, EventArgs e)
		{
			target = null;
			ApplyFilters();
			SetStatus("ChipLabel.CloseClick → target cleared → ApplyFilters");
		}

		private void emptyState_ActionClick(object sender, EventArgs e)
		{
			ClearFilters();
			SetStatus("EmptyState.ActionClick → ClearFilters → ApplyFilters");
		}

		private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (RowAt(e.RowIndex) is OrderRow row)
				Open(row, "CellDoubleClick");
		}

		private void colCustomer_AvatarClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (e.DataBoundItem is OrderRow row)
			{
				SetStatus("AvatarClick → bound row → Customers");
				Navigate(DashboardArea.Customers, row.Order.Customer);
			}
		}

		// e.DataBoundItem is the object behind the clicked row, whatever the sort order or the filter.
		private void colActions_ActionClick(object sender, DataGridViewVisualCellEventArgs e)
		{
			if (e.DataBoundItem is OrderRow row && e.Action.Name == "open")
				Open(row, "ActionClick");
		}

		#endregion

	}
}
