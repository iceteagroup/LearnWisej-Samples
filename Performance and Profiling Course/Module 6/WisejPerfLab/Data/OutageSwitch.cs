using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WisejPerfLab.Data
{
    /// <summary>
    /// The lab switch behind the "Break the database" button. Every module has to show what its
    /// scenario looks like when it fails, and a failing scenario has to stay measurable: the probe
    /// still writes its PERF end record, and the screen says what went wrong instead of going blank.
    /// </summary>
    public sealed class OutageSwitch
    {
        private int _isDown;

        public bool IsDown => Volatile.Read(ref _isDown) == 1;

        public void Break() => Volatile.Write(ref _isDown, 1);

        public void Restore() => Volatile.Write(ref _isDown, 0);
    }

    /// <summary>Thrown instead of opening a connection while the outage switch is on.</summary>
    public sealed class DatabaseUnavailableException : Exception
    {
        public DatabaseUnavailableException()
            : base("The ticket database is not reachable.")
        {
        }
    }

    /// <summary>
    /// Fails the connection instead of the query, which is what a real outage does: the trace shows the
    /// scenario ending early, with the time spent before the failure still recorded.
    /// </summary>
    public sealed class OutageInterceptor : DbConnectionInterceptor
    {
        private readonly OutageSwitch _outage;

        public OutageInterceptor(OutageSwitch outage)
        {
            _outage = outage;
        }

        public override InterceptionResult ConnectionOpening(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            if (_outage.IsDown)
                throw new DatabaseUnavailableException();

            return base.ConnectionOpening(connection, eventData, result);
        }

        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            if (_outage.IsDown)
                throw new DatabaseUnavailableException();

            return base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }
    }
}
