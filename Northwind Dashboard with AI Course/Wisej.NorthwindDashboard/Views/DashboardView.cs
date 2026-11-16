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
using System.ComponentModel;
using System.Linq;
using Wisej.NorthwindDashboard.Data;
using Wisej.Web;

namespace Wisej.NorthwindDashboard.Views
{

	/// <summary>
	/// Base class of the dashboard areas hosted by <see cref="MainPage"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The designer creates views without a context, so derived classes load data only from <see cref="RefreshView"/>,
	/// which runs after <see cref="Dashboard"/> is assigned. Period and workflow changes refresh a visible view
	/// immediately; a hidden view is marked stale and refreshed the next time it is shown.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// var view = new OverviewView { Dock = DockStyle.Fill, Dashboard = context };
	/// contentPanel.Controls.Add(view);
	/// </code>
	/// </example>
	[ToolboxItem(false)]
	public class DashboardView : UserControl
	{

		#region Constructors

		/// <summary>
		/// Creates the view.
		/// </summary>
		public DashboardView()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the session context that supplies data, the period and navigation.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DashboardContext Dashboard
		{
			get => dashboard;
			set
			{
				if (dashboard == value)
					return;

				Detach();
				dashboard = value;
				Attach();
				RequestRefresh(true);
			}
		}

		/// <summary>
		/// Gets the page title shown by the shell.
		/// </summary>
		[Browsable(false)]
		public virtual string Title => Text;

		/// <summary>
		/// Gets whether the shell's period selector applies to this view.
		/// </summary>
		[Browsable(false)]
		public virtual bool UsesPeriod => true;

		#endregion

		#region Methods

		/// <summary>
		/// Brings a record into focus, such as a customer on the map; the default implementation does nothing.
		/// </summary>
		/// <param name="target">Record passed with the navigation request.</param>
		public virtual void ShowTarget(object target)
		{
		}

		/// <summary>
		/// Reloads the view from <see cref="Dashboard"/>; called only when a context is assigned.
		/// </summary>
		protected virtual void RefreshView()
		{
		}

		/// <summary>
		/// Called when the session ships an order or changes its fulfillment step; refreshes by default.
		/// </summary>
		protected virtual void OnOrdersChanged() => RequestRefresh(false);

		/// <summary>
		/// Called when the session places a purchase order; refreshes by default.
		/// </summary>
		protected virtual void OnProductsChanged() => RequestRefresh(false);

		/// <summary>
		/// Refreshes a stale view when it becomes visible.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Visible && stale)
				RequestRefresh(true);
		}

		/// <summary>
		/// Reflows the view's cards for the new client profile.
		/// </summary>
		/// <param name="e">Event data.</param>
		/// <remarks>
		/// Wisej raises the event after it has applied the responsive properties set in the designer, when the view is
		/// created under a profile other than Default and whenever the browser crosses into another profile. See
		/// <see cref="CardLayout"/> for the arrangements.
		/// </remarks>
		protected override void OnResponsiveProfileChanged(ResponsiveProfileChangedEventArgs e)
		{
			base.OnResponsiveProfileChanged(e);

			cards ??= Controls.OfType<TableLayoutPanel>().Where(t => t.ColumnCount == 12).Select(t => new CardLayout(t)).FirstOrDefault();
			cards?.Apply(e.CurrentProfile.Name);
		}

		/// <summary>
		/// Detaches from the context before disposing.
		/// </summary>
		/// <param name="disposing">Whether managed resources are being released.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Detach();

			base.Dispose(disposing);
		}

		#endregion

		#region Implementation

		private DashboardContext dashboard;
		private CardLayout cards;
		private bool stale;

		// Refreshes now when visible (or when forced after a new context); otherwise defers until shown.
		private void RequestRefresh(bool force)
		{
			if (dashboard == null)
				return;

			if (!Visible && !force)
			{
				stale = true;
				return;
			}

			stale = false;
			RefreshView();
		}

		private void Attach()
		{
			if (dashboard == null)
				return;

			dashboard.PeriodChanged += Dashboard_PeriodChanged;
			dashboard.OrdersChanged += Dashboard_OrdersChanged;
			dashboard.ProductsChanged += Dashboard_ProductsChanged;
		}

		private void Detach()
		{
			if (dashboard == null)
				return;

			dashboard.PeriodChanged -= Dashboard_PeriodChanged;
			dashboard.OrdersChanged -= Dashboard_OrdersChanged;
			dashboard.ProductsChanged -= Dashboard_ProductsChanged;
		}

		private void Dashboard_PeriodChanged(object sender, EventArgs e)
		{
			if (UsesPeriod)
				RequestRefresh(false);
		}

		private void Dashboard_OrdersChanged(object sender, EventArgs e) => OnOrdersChanged();

		private void Dashboard_ProductsChanged(object sender, EventArgs e) => OnProductsChanged();

		#endregion

	}
}
