namespace EnterpriseOps.Services.Workflow
{
    /// <summary>The six steps of the Escalation Wizard, in order. Owned by the workflow, shown by the wizard.</summary>
    public enum WizardStep
    {
        Reason = 0,
        Attachments = 1,
        Approver = 2,
        DueDate = 3,
        Notifications = 4,
        Review = 5,
    }

    public static class WizardSteps
    {
        public const int Count = 6;

        public static string Title(WizardStep step) => step switch
        {
            WizardStep.Reason => "Reason",
            WizardStep.Attachments => "Attachments",
            WizardStep.Approver => "Approver",
            WizardStep.DueDate => "Due date",
            WizardStep.Notifications => "Notifications",
            _ => "Review",
        };
    }
}
