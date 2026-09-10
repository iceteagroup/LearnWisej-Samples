using System;
using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>Where the approver notification stands. ManualReview = a compensation is outstanding.</summary>
    public enum NotificationStatus { Pending, Sent, ManualReview }

    /// <summary>
    /// The record the workflow persists. It is created by EscalationWorkflow only — never by a wizard page.
    /// </summary>
    public class Escalation
    {
        public int Id;
        public int WorkOrderId;
        public string TenantId;
        public string Reason;
        public string ApproverId;
        public DateTimeOffset DueAt;
        public NotificationChannels Channels;
        public List<Attachment> Attachments = new List<Attachment>();
        public NotificationStatus NotificationStatus = NotificationStatus.Pending;
        public string RequestedBy;
        public DateTime CreatedUtc;
        public string CorrelationId;

        public string Number => $"ESC-{Id}";
    }

    /// <summary>A staged upload. The wizard stages files; the workflow attaches them; cancel discards them.</summary>
    public class Attachment
    {
        public string StagedId;
        public string FileName;
        public long SizeBytes;

        public override string ToString() => $"{FileName} ({SizeBytes / 1024} KB)";
    }

    /// <summary>Who may approve an escalation (from the approver directory).</summary>
    public class Approver
    {
        // Properties, not fields: ComboBox.DisplayMember binds through the property descriptor and
        // throws "Cannot bind to the new display member" for a public field (verified 2026-09-10).
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }

        public override string ToString() => $"{Id} — {Role}";
    }

    [Flags]
    public enum NotificationChannels
    {
        None = 0,
        Email = 1,
        InApp = 2,
        Sms = 4,
    }
}
