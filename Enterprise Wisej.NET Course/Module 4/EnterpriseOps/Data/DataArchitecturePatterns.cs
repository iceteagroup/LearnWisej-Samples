// DataArchitecturePatterns.cs — the pattern card from the lesson resource (the file the walkthrough
// video opens). It is kept as a comment so the real types do not collide with it; each line points
// at where the pattern is implemented in this project.
//
//   public sealed record CommandResult(bool Success, string UserMessage, string? ErrorCode = null)
//   {
//       public static CommandResult Ok(string message) => new(true, message);
//       public static CommandResult Fail(string message, string code) => new(false, message, code);
//   }
//   → Services/Commands/CommandResult.cs (plus CorrelationId, Detail, Errors, NewVersion)
//
//   public interface IWorkOrderCommandService
//   {
//       Task<CommandResult> ApproveAsync(ApproveWorkOrderCommand command, CancellationToken cancellationToken);
//   }
//   → Services/IWorkOrderCommandService.cs (Create, Update, Approve; the CommandContext is a parameter)
//
//   public sealed record ApproveWorkOrderCommand(Guid WorkOrderId, string TenantId, string UserId, string Comment, byte[] ExpectedRowVersion);
//   → Services/Commands/ApproveWorkOrderCommand.cs (int WorkOrderId / int ExpectedVersion: the course domain's token)
//
//   public sealed class WorkOrderCommandService : IWorkOrderCommandService
//   {
//       public async Task<CommandResult> ApproveAsync(ApproveWorkOrderCommand command, CancellationToken cancellationToken)
//       {
//           // Pattern: create a short-lived DbContext here, start transaction,
//           // load by tenant + ID, check concurrency, validate transition,
//           // persist, audit, commit, map failures to safe messages.
//       }
//   }
//   → Data/WorkOrderCommandService.cs (RunAsync is the template; ApproveAsync is the body)
//
// Other pieces of the boundary:
//   Data/EnterpriseOpsDbContext.cs          the unit of work (per command / per query)
//   Data/SessionDatabase.cs                 the session-long SQLite connection + the short-lived context factory
//   Data/WorkOrderRepository.cs             load/save the aggregate by tenant + id
//   Data/WorkOrderQueryService.cs           read-only projections for screens
//   Data/ErrorMap.cs                        database exception → result code + user message + audit line
//   Data/SessionLongContextAntiPattern.cs   the wrong lifetime, kept so the failure can be shown
