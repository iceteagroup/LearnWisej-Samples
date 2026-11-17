using NorthwindDashboard.Data;

namespace NorthwindDashboard.Views
{

	/// <summary>
	/// Stands in for an area that a later module builds.
	/// </summary>
	/// <remarks>
	/// <see cref="MainPage"/> creates it for every area whose table entry names a module instead of a view type, so
	/// the seven navigation destinations open from the first module on.
	/// </remarks>
	/// <example>
	/// <code>
	/// var view = new PlaceholderView(DashboardArea.Sales, 3);
	/// </code>
	/// </example>
	public partial class PlaceholderView : DashboardView
	{

		#region Constructors

		/// <summary>
		/// Creates the view for the designer.
		/// </summary>
		public PlaceholderView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Creates the placeholder of an area.
		/// </summary>
		/// <param name="area">Area the placeholder stands in for; its name becomes the heading.</param>
		/// <param name="module">Number of the course module that builds the area.</param>
		public PlaceholderView(DashboardArea area, int module)
			: this()
		{
			Text = area.ToString();
			messageLabel.Text = "This view is built in Module " + module + ".";
		}

		#endregion

	}
}
