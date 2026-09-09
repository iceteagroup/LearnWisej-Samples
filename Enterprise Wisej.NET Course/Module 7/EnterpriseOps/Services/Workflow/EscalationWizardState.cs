using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// The wizard state object: everything collected so far plus which steps are complete, in ONE typed
    /// object. Pages read from and write to it; WorkflowStateStore persists it on the server, which is
    /// what survives a browser refresh (review question 2). No control references, no business rules.
    /// </summary>
    public class EscalationWizardState
    {
        public string DraftId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
        public int WorkOrderId { get; set; }
        public int WorkOrderVersion { get; set; }
        public string WorkOrderTitle { get; set; }
        public string TenantId { get; set; }

        // Step 1 · Reason
        public string Reason { get; set; } = "";

        // Step 2 · Attachments (staged, not yet attached)
        public List<Attachment> Attachments { get; } = new List<Attachment>();

        // Step 3 · Approver
        public string ApproverId { get; set; }

        // Step 4 · Due date
        public DateTime DueAtLocal { get; set; } = DateTime.Now.Date.AddDays(2).AddHours(17);

        // Step 5 · Notifications
        public bool NotifyEmail { get; set; } = true;
        public bool NotifyInApp { get; set; } = true;
        public bool NotifySms { get; set; }

        // Progress
        public WizardStep CurrentStep { get; set; } = WizardStep.Reason;
        public HashSet<WizardStep> CompletedSteps { get; } = new HashSet<WizardStep>();
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        public NotificationChannels Channels =>
            (NotifyEmail ? NotificationChannels.Email : 0) |
            (NotifyInApp ? NotificationChannels.InApp : 0) |
            (NotifySms ? NotificationChannels.Sms : 0);

        public string Progress => $"step {(int)CurrentStep + 1} of {WizardSteps.Count} · {CompletedSteps.Count} complete";

        public string AttachmentSummary => Attachments.Count == 0 ? "none" : string.Join(" · ", Attachments.Select(a => a.FileName));

        public string ChannelSummary => Channels == NotificationChannels.None ? "none" : Channels.ToString().Replace(", ", " + ").ToLowerInvariant();

        public void MarkComplete(WizardStep step)
        {
            CompletedSteps.Add(step);
            UpdatedUtc = DateTime.UtcNow;
        }
    }
}
