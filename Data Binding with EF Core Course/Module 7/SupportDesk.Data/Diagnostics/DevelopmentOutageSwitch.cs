using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SupportDesk.Data.Diagnostics;

/// <summary>
/// Lab prop: while <see cref="IsDown"/> is true every connection open fails, which is what the
/// application sees when the database server is stopped or unreachable. Development only; it lets
/// the failure path of every module be shown on demand without touching the real connection string.
/// </summary>
public sealed class DevelopmentOutageSwitch
{
    public bool IsDown { get; set; }
}

/// <summary>Thrown by the outage interceptor: the database is unreachable.</summary>
public sealed class DatabaseUnavailableException : Exception
{
    public DatabaseUnavailableException()
        : base("Simulated outage: the Support Desk database is unreachable (DevelopmentOutageSwitch.IsDown = true).")
    {
    }
}

public sealed class OutageInterceptor : DbConnectionInterceptor
{
    private readonly DevelopmentOutageSwitch _switch;

    public OutageInterceptor(DevelopmentOutageSwitch outageSwitch) => _switch = outageSwitch;

    public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        if (_switch.IsDown) throw new DatabaseUnavailableException();
        return result;
    }

    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
    {
        if (_switch.IsDown) throw new DatabaseUnavailableException();
        return new ValueTask<InterceptionResult>(result);
    }
}
