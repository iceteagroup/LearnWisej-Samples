using System;
using System.Linq;
using System.Threading;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Dialogs
{
    /// <summary>
    /// The LegacyOrderDesk EditOrderDialog ported to a Wisej.Web.Form: same fields (Customer /
    /// Owner / Status), same DialogResult contract, same OrderValidator rule. What changed:
    /// System.Windows.Forms → Wisej.Web, the layout moved into EditOrderDialog.Designer.cs, and
    /// the CALLER owns the lifetime — a closed dialog is not disposed (dialogs are reusable), so
    /// the caller wraps it in a using block. <see cref="LiveInstances"/> makes that visible.
    /// </summary>
    public partial class EditOrderDialog : Form
    {
        // Lab instrumentation: how many EditOrderDialog instances exist in this PROCESS right now
        // (constructed and not yet disposed). A static is fine for a counter; it is process-wide,
        // so two browser tabs share it (Module 4 is about exactly that).
        private static int _liveInstances;
        private static int _created;

        /// <summary>Instances constructed and not yet disposed (process-wide).</summary>
        public static int LiveInstances => Volatile.Read(ref _liveInstances);

        /// <summary>Instances constructed since the process started.</summary>
        public static int TotalCreated => Volatile.Read(ref _created);

        private readonly OrderService _service;
        private readonly TracePanel _trace;
        private bool _counted;

        /// <summary>The edited copy; the caller saves it when ShowDialog returns DialogResult.OK.</summary>
        public Order Order { get; private set; }

        public EditOrderDialog(Order order, OrderService service, TracePanel trace = null)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _trace = trace;
            Order = (order ?? throw new ArgumentNullException(nameof(order))).Clone();

            Interlocked.Increment(ref _liveInstances);
            Interlocked.Increment(ref _created);
            _counted = true;

            InitializeComponent();

            this.Text = order.Id == 0 ? "New Order" : "Edit Order " + order.Id;
            comboCustomer.Items.AddRange(_service.Customers.Select(c => (object)c.Name).ToArray());
            comboOwner.Items.AddRange(_service.Owners.Select(o => (object)o).ToArray());
            comboStatus.Items.AddRange(Enum.GetNames(typeof(OrderStatus)).Select(s => (object)s).ToArray());
            ResetState();

            Log(TraceKind.Server, "new EditOrderDialog(" + order.Id + ")", "Wisej.Web.Form · LiveInstances=" + LiveInstances);
        }

        /// <summary>
        /// Reloads the fields from <see cref="Order"/>. Only needed when a dialog instance is kept
        /// and reused on purpose (the video's <c>_editDialog?.ResetState()</c>); the samples use a
        /// fresh instance per edit inside a using block instead.
        /// </summary>
        public void ResetState()
        {
            comboCustomer.SelectedItem = Order.CustomerName;
            comboOwner.SelectedItem = Order.Owner;               // null for order 1040 → nothing selected
            comboStatus.SelectedItem = Order.Status.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Log(TraceKind.ClientToServer, "btnSave.Click", "Edit Order " + Order.Id);

            Order.Customer = _service.Customers.FirstOrDefault(c => c.Name == (comboCustomer.SelectedItem as string));
            Order.Owner = comboOwner.SelectedItem as string;
            if (comboStatus.SelectedItem is string status)
                Order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status);

            var result = new OrderValidator().Validate(Order);          // ✓ business rule reused as-is
            if (result.HasErrors)
            {
                Log(TraceKind.Server, "OrderValidator.Validate", result.ToString());
                Log(TraceKind.Finding, "validation MessageBox stays modal", "a decision is required — the user must fix the data first");
                // A blocking MessageBox for a validation failure is still right (the user must act);
                // the informational "Saved." one is what Module 3 replaces with a Toast.
                MessageBox.Show(string.Join(Environment.NewLine, result.Errors.Values), "Cannot save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log(TraceKind.Server, "OrderValidator.Validate", "valid");
            this.DialogResult = DialogResult.OK;
            Log(TraceKind.Server, "EditOrderDialog.Close()", "DialogResult.OK → ShowDialog() returns in the caller");
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Log(TraceKind.ClientToServer, "btnCancel.Click", "Edit Order " + Order.Id);
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>Called from Dispose(bool) in the designer file: the instance is released now.</summary>
        private void OnDisposing()
        {
            if (!_counted) return;
            _counted = false;
            Interlocked.Decrement(ref _liveInstances);
            Log(TraceKind.Server, "EditOrderDialog.Dispose()", "Edit Order " + Order.Id + " released · LiveInstances=" + LiveInstances);
        }

        private void Log(TraceKind kind, string name, string payload)
        {
            if (_trace == null || _trace.IsDisposed) return;
            _trace.Add(kind, name, payload);
        }
    }
}
