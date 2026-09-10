namespace TicketOps.Domain
{
    /// <summary>
    /// Who may see what. The diagnostics page is for operators: it maps the system (versions, session counts,
    /// dependency states), so even without secrets it is Supervisor-only. Technicians see "Access denied".
    /// </summary>
    public enum OperatorRole
    {
        Technician,
        Supervisor
    }
}
