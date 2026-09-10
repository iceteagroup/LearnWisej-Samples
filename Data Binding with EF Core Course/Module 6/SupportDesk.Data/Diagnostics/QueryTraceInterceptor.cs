using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SupportDesk.Data.Diagnostics;

/// <summary>
/// Reports every executed command (text + duration) to the current <see cref="QueryTrace"/> scope.
/// Registered once, on the factory options; it costs nothing when no scope is active.
/// </summary>
public sealed class QueryTraceInterceptor : DbCommandInterceptor
{
    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Report(command, eventData);
        return result;
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        Report(command, eventData);
        return new ValueTask<DbDataReader>(result);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        Report(command, eventData);
        return result;
    }

    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        Report(command, eventData);
        return new ValueTask<object?>(result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        Report(command, eventData);
        return result;
    }

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        Report(command, eventData);
        return new ValueTask<int>(result);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        => QueryTrace.Report(TraceKind.Failure, $"{eventData.Exception.GetType().Name}: {Compact(eventData.Exception.Message, 160)} — {Compact(command.CommandText, 120)}", eventData.Duration.TotalMilliseconds);

    public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        CommandFailed(command, eventData);
        return Task.CompletedTask;
    }

    private static void Report(DbCommand command, CommandExecutedEventData eventData)
        => QueryTrace.Report(TraceKind.Command, Compact(command.CommandText, 400), eventData.Duration.TotalMilliseconds);

    /// <summary>One line, collapsed whitespace, cut with an ellipsis when longer than <paramref name="max"/>.</summary>
    public static string Compact(string text, int max)
    {
        var one = Whitespace.Replace(text, " ").Trim();
        return one.Length <= max ? one : one[..max] + "…";
    }
}
