using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Services.Commands;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The write side of the data boundary. Commands go in, a <see cref="CommandResult"/> comes out —
    /// nothing else crosses. The UI depends on this interface, never on the EF Core implementation.
    /// </summary>
    public interface IWorkOrderCommandService
    {
        Task<CommandResult> CreateAsync(CreateWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken);
        Task<CommandResult> UpdateAsync(UpdateWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken);
        Task<CommandResult> ApproveAsync(ApproveWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken);
    }
}
