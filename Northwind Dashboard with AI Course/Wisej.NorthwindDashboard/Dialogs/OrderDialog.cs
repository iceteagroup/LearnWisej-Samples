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
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.NorthwindDashboard.Views;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Dialogs
{

	/// <summary>
	/// Order details: progress, customer, sales rep, lines and totals, with a shipping action for open orders.
	/// </summary>
	/// <example>
	/// <code>
	/// OrderDialog.ShowOrder(context, context.Database.FindOrder(10248));
	/// </code>
	/// </example>
	public partial class OrderDialog : Form
	{

		#region Constructors

		/// <summary>
		/// Creates an empty dialog for the designer.
		/// </summary>
		public OrderDialog()
		{
			InitializeComponent();

			// The Phone profile (OrderDialog.resx) stacks the parties and the totals; the dialog takes the whole screen.
			if (Application.ActiveProfile.Name == "Phone")
				WindowState = FormWindowState.Maximized;
		}

		/// <summary>
		/// Creates the dialog for an order.
		/// </summary>
		/// <param name="context">Session context.</param>
		/// <param name="order">Order to show.</param>
		public OrderDialog(DashboardContext context, Order order) : this()
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
			this.order = order ?? throw new ArgumentNullException(nameof(order));

			titleLabel.Text = "Order " + order.Number;
			subtitleLabel.Text = "Placed " + DashboardStyle.Date(order.OrderDate) + " · " + order.Lines.Count + " lines · " +
				order.Units + " units · via " + order.Shipper.Company;

			customerAvatar.Text = order.Customer.Company;
			customerAvatar.Label = order.Customer.Company;
			customerAvatar.SubLabel = order.Customer.Contact + " · " + order.Customer.ContactTitle;
			shipToLabel.Text = "Ship to " + order.ShipName + ", " + order.ShipAddress + Environment.NewLine +
				order.ShipPostalCode + " " + order.ShipCity + (string.IsNullOrEmpty(order.ShipRegion) ? "" : ", " + order.ShipRegion) + ", " + order.ShipCountry;

			salesRepAvatar.Text = order.Employee.FullName;
			salesRepAvatar.Label = order.Employee.FullName;
			salesRepAvatar.SubLabel = order.Employee.Title;

			linesGrid.DataSource = order.Lines.Select(l => new LineRow(l)).ToList();
			linesGrid.CurrentCell = null;
			totalsLabel.Text = "Subtotal " + DashboardStyle.MoneyCents(order.Subtotal) + "   ·   Freight " + DashboardStyle.MoneyCents(order.Freight) +
				"   ·   Total " + DashboardStyle.MoneyCents(order.Subtotal + order.Freight);

			UpdateStatus();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows the details of an order as a modal dialog.
		/// </summary>
		/// <param name="context">Session context.</param>
		/// <param name="order">Order to show.</param>
		public static void ShowOrder(DashboardContext context, Order order)
		{
			if (context == null || order == null)
				return;

			// Non-blocking modal: the callback disposes the dialog when it closes.
			new OrderDialog(context, order).ShowDialog((dialog, result) => dialog.Dispose());
		}

		#endregion

		#region Implementation

		private readonly DashboardContext context;
		private readonly Order order;

		private void UpdateStatus()
		{
			var status = context.StatusOf(order);
			var stage = context.StageOf(order);
			var shipped = context.ShippedDate(order);

			statusChip.Text = DashboardStyle.StatusText(status);
			statusChip.Tone = DashboardStyle.StatusTone(status);

			stepper.Steps[0].State = StepState.Completed;
			stepper.Steps[0].Description = DashboardStyle.Date(order.OrderDate);

			stepper.Steps[1].State = shipped != null || stage == FulfillmentStage.Packed ? StepState.Completed : StepState.Active;
			stepper.Steps[1].Description = shipped != null ? "Done" : stage == FulfillmentStage.Packed ? "Packed" : stage == FulfillmentStage.Picking ? "Picking" : "Waiting";

			if (shipped is DateTime date)
			{
				int late = (date - order.RequiredDate).Days;
				stepper.Steps[2].State = late > 0 ? StepState.Error : StepState.Completed;
				stepper.Steps[2].Description = DashboardStyle.Date(date) + (late > 0 ? " · " + late + (late == 1 ? " day late" : " days late") : "");
				stepper.Steps[3].State = late > 0 ? StepState.Error : StepState.Completed;
			}
			else
			{
				stepper.Steps[2].State = stage == FulfillmentStage.Packed ? StepState.Active : StepState.Pending;
				stepper.Steps[2].Description = "Not shipped";
				stepper.Steps[3].State = status == OrderStatus.Overdue ? StepState.Error : StepState.Pending;
			}

			stepper.Steps[3].Description = DashboardStyle.Date(order.RequiredDate);
			shipButton.Visible = shipped == null;
		}

		private void shipButton_Click(object sender, EventArgs e)
		{
			context.SetStage(order, FulfillmentStage.Shipped);
			UpdateStatus();
			AlertBox.Show("Order " + order.Number + " shipped via " + order.Shipper.Company + ".", MessageBoxIcon.Information, allowHtml: false);
		}

		private void customerAvatar_Click(object sender, EventArgs e)
		{
			Close();
			context.Navigate(DashboardArea.Customers, order.Customer);
		}

		private void salesRepAvatar_Click(object sender, EventArgs e)
		{
			Close();
			context.Navigate(DashboardArea.Team, order.Employee);
		}

		private sealed class LineRow
		{
			public LineRow(OrderLine line)
			{
				Product = line.Product.Name;
				Category = line.Product.Category.Name + " · " + line.Product.QuantityPerUnit;
				Quantity = line.Quantity;
				UnitPrice = line.UnitPrice;
				Discount = line.Discount;
				Total = line.Total;
			}

			public string Product { get; }

			public string Category { get; }

			public int Quantity { get; }

			public decimal UnitPrice { get; }

			public decimal Discount { get; }

			public decimal Total { get; }
		}

		#endregion

	}
}
