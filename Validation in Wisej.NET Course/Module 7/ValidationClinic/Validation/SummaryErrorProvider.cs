using System;
using System.Collections.Generic;
using System.Linq;
using Wisej.Web;

namespace ValidationClinic.Validation
{
    public sealed class SummaryErrorProvider : IErrorProvider
    {
        private readonly Label label;
        private readonly Action<Control, string> writeIcon;
        private readonly Dictionary<Control, string> errors = new();
        public SummaryErrorProvider(Label label, Action<Control, string> writeIcon)
        { this.label = label; this.writeIcon = writeIcon; }
        public string GetError(Control control) => errors.TryGetValue(control, out var message) ? message : "";
        public void SetError(Control control, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) errors.Remove(control);
            else errors[control] = message;
            // The extender calls this provider directly; mirror those errors to icons too.
            writeIcon(control, message ?? "");
            label.Text = string.Join(Environment.NewLine, errors.Values.Distinct());
            label.Visible = errors.Count != 0;
        }
        public void Clear()
        {
            foreach (var control in errors.Keys.ToArray()) SetError(control, "");
        }
    }
}
