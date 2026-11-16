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
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Sales organization chart, individual profiles and the period leaderboard.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The radar compares a person with the team average on five measures. Each measure is scaled so that the
	/// best team member scores 100, which keeps revenue, counts and percentages on one comparable axis range.
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
		/// Shows the profile of an employee.
		/// </summary>
		/// <param name="target">An <see cref="Employee"/>.</param>
		public override void ShowTarget(object target)
		{
			if (target is Employee employee && performance.ContainsKey(employee))
				Select(employee);
		}

		/// <summary>
		/// Reloads the organization chart, leaderboard and selected profile for the current period.
		/// </summary>
		protected override void RefreshView()
		{
			var period = Dashboard.Period;
			performance = SalesAnalytics.EmployeeSales(Dashboard).ToDictionary(p => p.Employee);
			UpdateOrganization();

			var rows = performance.Values.OrderByDescending(p => p.Revenue).Select(p => new EmployeeRow(p)).ToList();
			colShare.Maximum = Math.Max(1, Math.Ceiling(rows.Max(r => r.Share) / 5) * 5);
			leaderboardGrid.DataSource = rows;
			leaderboardGrid.CurrentCell = null;
			colChange.Visible = period.HasComparison;
			leaderboardCard.Text = "Leaderboard · " + period.Name.ToLowerInvariant();

			Select(selected ?? rows[0].Employee);
		}

		#endregion

		#region Implementation

		private Dictionary<Employee, EmployeeSales> performance = new Dictionary<Employee, EmployeeSales>();
		private Employee selected;

		private void UpdateOrganization()
		{
			if (orgChart.Nodes.Count == 0)
			{
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

			double average = performance.Values.Average(p => (double)p.Revenue);
			foreach (var node in orgChart.Nodes)
				node.Tone = (double)performance[(Employee)node.Tag].Revenue >= average ? MeterTone.Success : MeterTone.Warning;
		}

		private void Select(Employee employee)
		{
			selected = employee;
			var period = Dashboard.Period;
			var person = performance[employee];

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
			customersGroup.Items.Clear();
			foreach (var (customer, revenue) in top.Take(12))
				customersGroup.Items.Add(new AvatarItem { Text = customer.Company, Label = customer.Company + " · " + DashboardStyle.Money(revenue), Tag = customer });
			customersGroup.MaxVisible = 6;
			customersLabel.Text = top.Length == 0 ? "NO CUSTOMERS IN THIS PERIOD" : "TOP CUSTOMERS · " + top.Length + " IN THIS PERIOD";

			for (int i = 0; i < leaderboardGrid.RowCount; i++)
			{
				if (leaderboardGrid.Rows[i].DataBoundItem is EmployeeRow row && row.Employee == employee)
				{
					leaderboardGrid.Rows[i].Selected = true;
					break;
				}
			}
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
				Select(employee);
		}

		private void leaderboardGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && leaderboardGrid.Rows[e.RowIndex].DataBoundItem is EmployeeRow row)
				Select(row.Employee);
		}

		private void profileCard_ToolClick(object sender, ToolClickEventArgs e)
		{
			if (selected != null)
				Dashboard.Navigate(DashboardArea.Orders, selected);
		}

		private void customersGroup_ItemClick(object sender, AvatarItemEventArgs e)
		{
			if (e.Item.Tag is Customer customer)
				Dashboard.Navigate(DashboardArea.Customers, customer);
		}

		private void customersGroup_OverflowClick(object sender, EventArgs e)
		{
			if (selected != null)
				Dashboard.Navigate(DashboardArea.Orders, selected);
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
