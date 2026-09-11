using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The EF Core-backed write side. Every operation has the same shape:
    ///
    ///   authorize → short-lived DbContext → BEGIN → load by tenant + id → check concurrency →
    ///   validate → persist → audit → COMMIT → map failures to safe messages.
    ///
    /// The transaction boundary lives here — not in the form, not in the repository — because this is
    /// the only place where the whole operation is visible. Nothing in this file references Wisej.
    /// </summary>
    public sealed class WorkOrderCommandService : IWorkOrderCommandService
    {
        private readonly SessionDatabase _database;
        private readonly IActivityTrace _trace;
        private readonly Func<TimeSpan> _commandTimeout;

        public WorkOrderCommandService(SessionDatabase database, IActivityTrace trace, Func<TimeSpan> commandTimeout)
        {
            _database = database;
            _trace = trace;
            _commandTimeout = commandTimeout;
        }

        #region Approve — the walkthrough's operation

        public Task<CommandResult> ApproveAsync(ApproveWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken)
        {
            return RunAsync("Approve", command.WorkOrderId, context, cancellationToken, async (repository, dbContext, ct) =>
            {
                if (string.IsNullOrWhiteSpace(command.Comment))
                    return ErrorMap.Rejected(ErrorMap.ValidationFailed, "An approval comment is required.", context.CorrelationId, _trace);

                var workOrder = await repository.LoadAsync(command.TenantId, command.WorkOrderId, ct);
                if (workOrder == null)
                    return ErrorMap.Rejected(ErrorMap.NotFound, $"Work order {command.WorkOrderId} was not found in tenant {command.TenantId}.", context.CorrelationId, _trace);

                // 1. Concurrency: the version the user saw becomes the UPDATE's WHERE clause (checked by the database at SaveChanges).
                _trace.Trace(TraceLayer.Service, $"check concurrency: user saw v{command.ExpectedVersion}, database has v{workOrder.Version}" +
                                                 (command.ExpectedVersion == workOrder.Version ? " → match" : " → MISMATCH (the database will reject the UPDATE)"));

                // 2. Transition: pure domain rule.
                if (!WorkOrderTransitions.CanApprove(workOrder.Status, out string reason))
                {
                    _trace.Trace(TraceLayer.Service, $"validate transition {workOrder.Status} → Completed ✗ ({reason})");
                    return ErrorMap.Rejected(ErrorMap.StateInvalid, reason, context.CorrelationId, _trace);
                }
                _trace.Trace(TraceLayer.Service, $"validate transition {workOrder.Status} → Completed ✓");

                // 3. Persist + audit inside the same transaction.
                repository.ExpectVersion(workOrder, command.ExpectedVersion);
                workOrder.Status = WorkOrderStatus.Completed;
                workOrder.ApprovedBy = command.UserId;
                workOrder.ApprovedUtc = DateTime.UtcNow;
                workOrder.ApprovalComment = command.Comment.Trim();
                repository.AddAudit(Audit(context, "Approve", workOrder.Id, "Committed", null, $"v{command.ExpectedVersion} → v{workOrder.Version}: {workOrder.ApprovalComment}"));

                await SaveAsync(dbContext, $"UPDATE WorkOrders SET Status='Completed', Version={workOrder.Version} WHERE Id={workOrder.Id} AND Version={command.ExpectedVersion} · INSERT AuditEntries", ct);

                return CommandResult.Ok("Work order approved.", context.CorrelationId, workOrder.Id, workOrder.Version);
            });
        }

        #endregion

        #region Create / Update

        public Task<CommandResult> CreateAsync(CreateWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken)
        {
            return RunAsync("Create", null, context, cancellationToken, async (repository, dbContext, ct) =>
            {
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(command.Number) || !command.Number.StartsWith("WO-", StringComparison.OrdinalIgnoreCase))
                    errors.Add("Number must look like WO-1234.");
                if (string.IsNullOrWhiteSpace(command.Title))
                    errors.Add("Title is required.");
                if (errors.Count > 0)
                {
                    _trace.Trace(TraceLayer.Service, $"validate CreateWorkOrderCommand ✗ ({string.Join(" ", errors)})");
                    return CommandResult.Fail("Please correct the highlighted fields.", ErrorMap.ValidationFailed, context.CorrelationId, errors);
                }
                _trace.Trace(TraceLayer.Service, "validate CreateWorkOrderCommand ✓");

                var workOrder = new WorkOrder
                {
                    TenantId = command.TenantId,
                    Number = command.Number.Trim().ToUpperInvariant(),
                    Title = command.Title.Trim(),
                    Customer = command.Customer?.Trim() ?? "",
                    Site = command.Site?.Trim() ?? "",
                    Priority = command.Priority,
                    Status = WorkOrderStatus.New,
                    AssignedTo = "",
                    CreatedUtc = DateTime.UtcNow,
                    DueUtc = command.DueUtc,
                    Version = 1,
                };
                repository.Add(workOrder);

                // The audit row needs the generated id, so it is added after the first SaveChanges — still inside the transaction.
                await SaveAsync(dbContext, $"INSERT WorkOrders ({workOrder.Number}) — the UNIQUE index decides", ct);
                repository.AddAudit(Audit(context, "Create", workOrder.Id, "Committed", null, $"{workOrder.Number} '{workOrder.Title}'"));
                await SaveAsync(dbContext, "INSERT AuditEntries", ct);

                return CommandResult.Ok($"Work order {workOrder.Number} created (id {workOrder.Id}).", context.CorrelationId, workOrder.Id, workOrder.Version);
            });
        }

        public Task<CommandResult> UpdateAsync(UpdateWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken)
        {
            return RunAsync("Update", command.WorkOrderId, context, cancellationToken, async (repository, dbContext, ct) =>
            {
                if (string.IsNullOrWhiteSpace(command.Title))
                    return CommandResult.Fail("Please correct the highlighted fields.", ErrorMap.ValidationFailed, context.CorrelationId, new[] { "Title is required." });

                var workOrder = await repository.LoadAsync(command.TenantId, command.WorkOrderId, ct);
                if (workOrder == null)
                    return ErrorMap.Rejected(ErrorMap.NotFound, $"Work order {command.WorkOrderId} was not found in tenant {command.TenantId}.", context.CorrelationId, _trace);

                _trace.Trace(TraceLayer.Service, $"check concurrency: user saw v{command.ExpectedVersion}, database has v{workOrder.Version}" +
                                                 (command.ExpectedVersion == workOrder.Version ? " → match" : " → MISMATCH (the database will reject the UPDATE)"));

                var newStatus = command.Status ?? workOrder.Status;
                if (!WorkOrderTransitions.CanChangeStatus(workOrder.Status, newStatus, out string reason))
                {
                    _trace.Trace(TraceLayer.Service, $"validate status change {workOrder.Status} → {newStatus} ✗ ({reason})");
                    return ErrorMap.Rejected(ErrorMap.StateInvalid, reason, context.CorrelationId, _trace);
                }
                _trace.Trace(TraceLayer.Service, $"validate UpdateWorkOrderCommand ✓ (status {workOrder.Status} → {newStatus})");

                string change = $"{workOrder.Status} → {newStatus}, '{command.Title.Trim()}'";
                repository.ExpectVersion(workOrder, command.ExpectedVersion);
                workOrder.Title = command.Title.Trim();
                workOrder.Customer = command.Customer?.Trim() ?? "";
                workOrder.Site = command.Site?.Trim() ?? "";
                workOrder.Priority = command.Priority;
                workOrder.Status = newStatus;
                repository.AddAudit(Audit(context, "Update", workOrder.Id, "Committed", null, $"v{command.ExpectedVersion} → v{workOrder.Version}: {change}"));

                await SaveAsync(dbContext, $"UPDATE WorkOrders … WHERE Id={workOrder.Id} AND Version={command.ExpectedVersion} · INSERT AuditEntries", ct);

                return CommandResult.Ok($"Work order {workOrder.Number} updated.", context.CorrelationId, workOrder.Id, workOrder.Version);
            });
        }

        #endregion

        #region The template every command follows (authorize → context → transaction → map → audit)

        private delegate Task<CommandResult> CommandBody(IWorkOrderRepository repository, EnterpriseOpsDbContext dbContext, CancellationToken ct);

        private async Task<CommandResult> RunAsync(string operation, int? workOrderId, CommandContext context, CancellationToken cancellationToken, CommandBody body)
        {
            // A bounded wait: a command that cannot finish becomes DB_TIMEOUT, not a hung session.
            var timeout = _commandTimeout();
            using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(timeout);
            var ct = timeoutSource.Token;

            // One session, one SQLite connection: commands and queries take turns instead of overlapping.
            await _database.Gate.WaitAsync(cancellationToken);
            try
            {
                // Security first: no transaction is opened for a caller who may not perform the operation.
                var op = (Operation)Enum.Parse(typeof(Operation), operation);
                if (!Permissions.IsAllowed(context.Role, op, out string denied))
                {
                    _trace.Trace(TraceLayer.Security, $"authorize {context.UserId} ({context.Role}) → {operation} DENIED");
                    await WriteRejectionAuditAsync(context, operation, workOrderId, ErrorMap.PermissionDenied, denied);
                    return ErrorMap.Rejected(ErrorMap.PermissionDenied, denied, context.CorrelationId, _trace);
                }
                _trace.Trace(TraceLayer.Security, $"authorize {context.UserId} ({context.Role}) → {operation} allowed");

                CommandResult result = null;
                string auditDetail = null;

                // The unit of work: created here, disposed in the finally below. Nothing outside this
                // method ever sees the DbContext — that is the whole point of the boundary.
                var dbContext = _database.CreateContext($"{operation} command");
                try
                {
                    var repository = new WorkOrderRepository(dbContext, _trace);
                    var transaction = await dbContext.Database.BeginTransactionAsync(ct);
                    _trace.Trace(TraceLayer.Data, $"BEGIN TRANSACTION (timeout {timeout.TotalMilliseconds:0} ms)");
                    try
                    {
                        result = await body(repository, dbContext, ct);

                        if (result.Success)
                        {
                            await transaction.CommitAsync(ct);
                            _trace.Trace(TraceLayer.Data, "COMMIT");
                            _trace.Trace(TraceLayer.Audit, $"{operation} Committed · corr {context.CorrelationId} (written inside the transaction)");
                            _trace.Trace(TraceLayer.Service, $"CommandResult.Ok → \"{result.UserMessage}\"");
                        }
                        else
                        {
                            // A domain rejection: nothing was persisted, roll back explicitly.
                            await transaction.RollbackAsync(CancellationToken.None);
                            _trace.Trace(TraceLayer.Data, $"ROLLBACK ({result.ErrorCode})");
                        }
                    }
                    catch (Exception exception)
                    {
                        // The database's vocabulary stops here. The user gets a message, the audit gets a code.
                        var mapped = ErrorMap.Map(exception, operation, context.CorrelationId);
                        _trace.Trace(TraceLayer.Data, $"ROLLBACK ← {exception.GetType().Name}{(exception.InnerException != null ? " / " + exception.InnerException.GetType().Name : "")}");
                        _trace.Trace(TraceLayer.Service, $"ErrorMap: {exception.GetType().Name} → {mapped.Code}");

                        TryRollback(transaction);
                        auditDetail = mapped.AuditDetail;
                        result = CommandResult.Fail(mapped.UserMessage, mapped.Code, context.CorrelationId);
                        _trace.Trace(TraceLayer.Service, $"CommandResult.Fail {mapped.Code} → \"{mapped.UserMessage}\"");
                    }
                    finally
                    {
                        await transaction.DisposeAsync();
                    }
                }
                finally
                {
                    // Short-lived means short-lived: the context dies with the operation, committed or not.
                    _database.Release(dbContext, result != null && result.Success ? null : "rolled back");
                }

                // Written after the rollback, in its own context and its own transaction — an audit row that
                // rolls back with the failure it records is no audit at all.
                if (!result.Success)
                {
                    await WriteRejectionAuditAsync(context, operation, workOrderId ?? result.WorkOrderId,
                        result.ErrorCode, auditDetail ?? result.UserMessage);
                }

                return result;
            }
            finally
            {
                _database.Gate.Release();
            }
        }

        /// <summary>Best-effort rollback: the transaction may already be gone (timeout, connection loss).</summary>
        private void TryRollback(IDbContextTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception exception)
            {
                _trace.Trace(TraceLayer.Data, $"rollback was already done by the provider ({exception.GetType().Name})");
            }
        }

        private async Task SaveAsync(EnterpriseOpsDbContext dbContext, string statement, CancellationToken ct)
        {
            _trace.Trace(TraceLayer.Data, $"SaveChanges → {statement}");
            int rows = await dbContext.SaveChangesAsync(ct);
            _trace.Trace(TraceLayer.Data, $"SaveChanges affected {rows} row(s)");
        }

        /// <summary>
        /// A rejection is audited in its own short-lived context and its own transaction, after the
        /// command's transaction rolled back — otherwise the audit row would roll back with it.
        /// </summary>
        private async Task WriteRejectionAuditAsync(CommandContext context, string operation, int? workOrderId, string code, string detail)
        {
            EnterpriseOpsDbContext auditContext = null;
            try
            {
                auditContext = _database.CreateContext("rejection audit");
                auditContext.AuditEntries.Add(Audit(context, operation, workOrderId, "Rejected", code, detail));
                await auditContext.SaveChangesAsync(CancellationToken.None);
                _trace.Trace(TraceLayer.Audit, $"{operation} Rejected {code} · corr {context.CorrelationId} (own transaction, survives the rollback)");
            }
            catch (Exception exception)
            {
                // Auditing must never turn a mapped failure into an unmapped one.
                _trace.Trace(TraceLayer.Audit, $"could not write the rejection audit: {exception.GetType().Name}");
            }
            finally
            {
                if (auditContext != null)
                    _database.Release(auditContext);
            }
        }

        private static AuditEntry Audit(CommandContext context, string action, int? workOrderId, string outcome, string code, string detail)
            => new AuditEntry
            {
                TenantId = context.TenantId,
                WorkOrderId = workOrderId,
                Action = action,
                Outcome = outcome,
                ErrorCode = code,
                UserId = context.UserId,
                CorrelationId = context.CorrelationId,
                Detail = detail != null && detail.Length > 400 ? detail.Substring(0, 400) : detail,
                TimestampUtc = DateTime.UtcNow,
            };

        #endregion
    }
}
