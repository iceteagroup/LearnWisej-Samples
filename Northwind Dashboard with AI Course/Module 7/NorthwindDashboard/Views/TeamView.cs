using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Module 6 team view: the sales organization chart, the profile of the selected employee with their top
	/// customers and a radar against the team average, and the period leaderboard.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each employee is one <see cref="OrgNode"/>: <c>Id</c> is the employee number as a string, <c>ParentId</c> the
	/// manager's number (empty for the one root), <c>Text</c> and <c>SubText</c> the name and title, and <c>Tag</c> the
	/// <see cref="Employee"/>. A node click, a leaderboard row and a navigation target all end in the same selection
	/// code.
	/// </para>
	/// <para>
	/// The radar compares a person with the team average on five measures. Each measure is scaled so that the best
	/// team member scores 100, which keeps revenue, counts and percentages on one comparable axis range. A score of 50
	/// means half of the period's best value for that measure, not 50 dollars or 50 percent; the amounts are beside
	/// the chart and in the leaderboard.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// context.Navigate(DashboardArea.Team, employee);
	/// </code>
	/// </example>
	public partial class TeamView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public TeamView()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows the profile of the employee a navigation request carries.
		/// </summary>
		/// <param name="target">An <see cref="Employee"/>; other targets are ignored.</param>
		public override void ShowTarget(object target)
		{
			if (target is Employee employee && performance.ContainsKey(employee))
				Select(employee, true);
		}

		/// <summary>
		/// Reloads the organization chart, the leaderboard and the selected profile for the current period.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			performance = SalesAnalytics.EmployeeSales(Dashboard).ToDictionary(p => p.Employee);
			CreateNodes();

			var rows = performance.Values.OrderByDescending(p => p.Revenue).Select(p => new EmployeeRow(p)).ToList();
			colShare.Maximum = Math.Max(1, Math.Ceiling(rows.Max(r => r.Share) / 5) * 5);
			leaderboardGrid.DataSource = rows;
			leaderboardGrid.CurrentCell = null;
			colChange.Visible = period.HasComparison;
			leaderboardCard.Text = "Leaderboard · " + period.Name.ToLowerInvariant();

			// The first time, the period's top seller; afterwards the selection survives the period change.
			Select(selected ?? rows[0].Employee, false);
		}

		#endregion

		#region Implementation

		// AvatarGroup: up to twelve top customers, six of them drawn; the rest are behind the "+N" overflow avatar.
		private const int MaxCustomers = 12;
		private const int VisibleCustomers = 6;

		private Dictionary<Employee, EmployeeSales> performance = new Dictionary<Employee, EmployeeSales>();
		private Employee selected;

		// The nodes are created once: Id and ParentId are the Northwind keys, so the chart draws the reporting lines.
		private void CreateNodes()
		{
			if (orgChart.Nodes.Count > 0)
				return;

			foreach (var employee in Dashboard.Database.Employees)
			{
				orgChart.Nodes.Add(new OrgNode
				{
					Id = employee.Id.ToString(CultureInfo.InvariantCulture),
					ParentId = employee.ReportsTo?.ToString(CultureInfo.InvariantCulture) ?? "",
					Text = employee.FullName,
					SubText = employee.Title,
					Tag = employee
				});
			}
		}

		// reveal: expand the managers above the employee, for a selection that did not come from the chart itself.
		private void Select(Employee employee, bool reveal)
		{
			selected = employee;
			var period = Dashboard.Period;
			var person = performance[employee];

			// The selected node carries the primary tone; the others stay neutral.
			foreach (var node in orgChart.Nodes)
			{
				node.Tone = ReferenceEquals(node.Tag, employee) ? MeterTone.Primary : MeterTone.Neutral;
				if (reveal && node.Tag is Employee manager && Manages(manager, employee) && !orgChart.IsExpanded(node))
					orgChart.SetExpanded(node, true);
			}

			profileCard.Text = employee.FullName;
			profileAvatar.Text = employee.FullName;
			profileAvatar.Label = employee.TitleOfCourtesy + " " + employee.FullName;
			profileAvatar.SubLabel = employee.Title + " · " + employee.City + ", " + employee.Country;

			revenueValue.Text = DashboardStyle.CompactMoney(person.Revenue);
			ordersValue.Text = DashboardStyle.Count(person.Orders);
			averageValue.Text = DashboardStyle.Money(person.AverageOrder);
			onTimeValue.Text = person.Orders == 0 ? "—" : DashboardStyle.Percent(person.OnTimeRate);
			territoriesLabel.Text = "Territories: " + string.Join(", ", employee.Territories.Select(t => t.Name)) +
				" · " + string.Join(", ", employee.Territories.Select(t => t.Region.Name).Distinct()) + " region";

			UpdateRadar(employee);

			var top = employee.Orders
				.Where(o => period.Contains(o.OrderDate))
				.GroupBy(o => o.Customer)
				.Select(g => (Customer: g.Key, Revenue: g.Sum(o => o.Subtotal)))
				.OrderByDescending(x => x.Revenue)
				.ToArray();

			// Clear before adding, and give each item its own customer: ItemClick reads it back from e.Item.Tag.
			customersGroup.Items.Clear();
			foreach (var (customer, revenue) in top.Take(MaxCustomers))
				customersGroup.Items.Add(new AvatarItem { Text = customer.Company, Label = customer.Company + " · " + DashboardStyle.Money(revenue), Tag = customer });

			customersGroup.MaxVisible = VisibleCustomers;
			customersLabel.Text = top.Length == 0 ? "NO CUSTOMERS IN THIS PERIOD" : "TOP CUSTOMERS · " + top.Length + " IN THIS PERIOD";

			leaderboardGrid.ClearSelection();
			for (int i = 0; i < leaderboardGrid.RowCount; i++)
			{
				if (leaderboardGrid.Rows[i].DataBoundItem is EmployeeRow row && row.Employee == employee)
				{
					leaderboardGrid.Rows[i].Selected = true;
					break;
				}
			}
		}

		// Follows ReportsTo upward; Northwind has one root and no cycles.
		private bool Manages(Employee manager, Employee employee)
		{
			for (int? id = employee.ReportsTo; id != null; id = Dashboard.Database.Employees.First(e => e.Id == id).ReportsTo)
			{
				if (id == manager.Id)
					return true;
			}

			return false;
		}

		// Scales every measure to the team's best value (= 100) so the radar axes are comparable.
		private void UpdateRadar(Employee employee)
		{
			var team = performance.Values.ToArray();
			var measures = new (string Axis, Func<EmployeeSales, double> Value)[]
			{
				("Revenue", p => (double)p.Revenue),
				("Orders", p => p.Orders),
				("Avg. order", p => (double)p.AverageOrder),
				("Customers", p => p.Customers),
				("On time", p => p.OnTimeRate)
			};

			radarChart.Series[0].Name = employee.FirstName;
			radarChart.DataSource = measures.Select(m =>
			{
				// A measure nobody scored on stays at zero instead of dividing by zero.
				double best = team.Max(m.Value);
				double Scale(double value) => best <= 0 ? 0 : Math.Round(value / best * 100, 1);
				return new RadarAxis
				{
					Axis = m.Axis,
					Person = Scale(m.Value(performance[employee])),
					Team = Scale(team.Average(m.Value))
				};
			}).ToArray();
		}

		private void orgChart_NodeClick(object sender, OrgNodeEventArgs e)
		{
			if (e.Node.Tag is Employee employee)
			{
				Select(employee, false);
				SetStatus("OrgChart.NodeClick → Tag → employee · AvatarGroup items");
			}
		}

		private void leaderboardGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && leaderboardGrid.Rows[e.RowIndex].DataBoundItem is EmployeeRow row)
			{
				Select(row.Employee, true);
				SetStatus("DataGridView.CellClick → DataBoundItem → employee · AvatarGroup items");
			}
		}

		private void profileCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (selected != null)
			{
				Navigate(DashboardArea.Orders, selected);
				SetStatus("Panel.ToolClick → Navigate(Orders, employee)");
			}
		}

		private void customersGroup_ItemClick(object sender, AvatarItemEventArgs e)
		{
			if (e.Item.Tag is Customer customer)
			{
				Navigate(DashboardArea.Customers, customer);
				SetStatus("AvatarGroup.ItemClick → e.Item.Tag → Customers");
			}
		}

		// The avatars behind "+N" have no item to click: the overflow opens the employee's orders, which list them all.
		private void customersGroup_OverflowClick(object sender, EventArgs e)
		{
			if (selected != null)
			{
				Navigate(DashboardArea.Orders, selected);
				SetStatus("AvatarGroup.OverflowClick → Navigate(Orders, employee)");
			}
		}

		private sealed class RadarAxis
		{
			public string Axis { get; init; }

			public double Person { get; init; }

			public double Team { get; init; }
		}

		#endregion

	}
}
