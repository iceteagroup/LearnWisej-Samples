using System;
using System.ComponentModel;
using NorthwindDashboard.Data;
using Wisej.Web;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Base class of the dashboard areas hosted by <see cref="MainPage"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The designer creates views without a context, so derived classes load data only from <see cref="RefreshView"/>,
	/// which runs after <see cref="Dashboard"/> is assigned. A period change refreshes a visible view immediately; a
	/// hidden view is marked stale and refreshed the next time it is shown.
	/// </para>
	/// <para>
	/// A view never touches the shell's controls. It reports through <see cref="SetStatus"/> and asks for another area
	/// through <see cref="Navigate"/>.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// public partial class SalesView : DashboardView
	/// {
	///     public SalesView() { InitializeComponent(); }
	///     protected override void RefreshView() { /* read Dashboard.Period and fill the controls */ }
	/// }
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
		/// Gets or sets the page that hosts the view; <see cref="MainPage"/> assigns it before <see cref="Dashboard"/>.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MainPage Shell { get; set; }

		/// <summary>
		/// Gets the heading the shell shows above the view; the default is the view's <c>Text</c>.
		/// </summary>
		[Browsable(false)]
		public virtual string Title => Text;

		/// <summary>
		/// Gets whether the shell's period selector applies to this view; the shell hides the selector when it does not.
		/// </summary>
		[Browsable(false)]
		public virtual bool UsesPeriod => true;

		#endregion

		#region Methods

		/// <summary>
		/// Brings a record into focus, such as a customer on the map; the default implementation does nothing.
		/// </summary>
		/// <param name="target">Record passed with the navigation request.</param>
		/// <remarks>
		/// The shell calls it after the view is visible and refreshed, each time a navigation carries a target.
		/// </remarks>
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
		/// Writes the event path of the last interaction to the shell's status strip.
		/// </summary>
		/// <param name="text">One line, such as "MapView.MarkerClick → e.Marker.Tag → customer".</param>
		protected void SetStatus(string text) => Shell?.SetStatus(text);

		/// <summary>
		/// Asks the shell to show another area, optionally with a record to bring into focus there.
		/// </summary>
		/// <param name="area">Area to show.</param>
		/// <param name="target">Record handed to the area's <see cref="ShowTarget"/>, or null.</param>
		protected void Navigate(DashboardArea area, object target = null) => Dashboard?.Navigate(area, target);

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
