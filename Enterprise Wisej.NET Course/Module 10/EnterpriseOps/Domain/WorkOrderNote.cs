using System;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// A free-text note attached to a work order. The text arrives from **outside** the application — a customer
    /// portal, an e-mail gateway, a field technician's phone — so it is untrusted by definition and never rendered
    /// as HTML without going through <see cref="EnterpriseOps.Security.HtmlText"/> first.
    ///
    /// The seed data deliberately contains one note with an injected payload so the lab can show both renderings.
    /// </summary>
    public sealed class WorkOrderNote
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public string TenantId { get; set; }

        /// <summary>Where the text came from — the reviewer's first question about any HTML-capable surface.</summary>
        public string Source { get; set; }

        public string Author { get; set; }
        public DateTime CreatedUtc { get; set; }

        /// <summary>Raw, exactly as received. Never bind this to a control whose AllowHtml is true.</summary>
        public string Text { get; set; }

        public override string ToString() => $"note #{Id} on WO-{WorkOrderId:0000} from {Source}";
    }
}
