using System;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Commands
{
    /// <summary>Input for Create. <c>Number</c> must be unique per tenant — the database enforces it.</summary>
    public sealed record CreateWorkOrderCommand(
        string TenantId,
        string UserId,
        string Number,
        string Title,
        string Customer,
        string Site,
        Priority Priority,
        DateTime? DueUtc);
}
