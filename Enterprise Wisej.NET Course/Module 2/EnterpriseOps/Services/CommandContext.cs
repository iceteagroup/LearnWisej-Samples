using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Tenant + user + correlation id, captured when a command starts. Services receive it as a parameter
    /// and never reach for session state themselves — that is what makes them reviewable without the designer.
    /// </summary>
    public class CommandContext
    {
        public string TenantId { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }
        public string CorrelationId { get; set; }

        /// <summary>A fresh context (new correlation id) for the next command of this session.</summary>
        public static CommandContext From(SessionContext session)
        {
            return new CommandContext
            {
                TenantId = session.TenantId,
                UserName = session.UserName,
                Role = session.Role,
                CorrelationId = SessionContext.NewCorrelationId()
            };
        }

        public override string ToString() => $"{TenantId}/{UserName} corr={CorrelationId}";
    }
}
