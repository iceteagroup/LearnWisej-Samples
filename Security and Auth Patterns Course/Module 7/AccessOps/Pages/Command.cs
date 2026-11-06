using System;
using AccessOps.Services;
using Microsoft.Extensions.Logging;

namespace AccessOps.Pages
{
    /// <summary>
    /// The top of every command handler: mint the correlation ID, open the logging scope, and log
    /// the start and the end. Everything the services log in between inherits CorrelationId and
    /// UserId from the scope.
    /// </summary>
    internal sealed class Command : IDisposable
    {
        private static readonly ILogger Log = AppHost.Get<ILoggerFactory>().CreateLogger("AccessOps.Commands");

        private readonly IDisposable _scope;
        private readonly string _name;
        private readonly DateTime _started = DateTime.UtcNow;

        private Command(string name, CommandContext context)
        {
            _name = name;
            this.Context = context;
            _scope = Log.BeginScope(context.Scope());
            Log.LogInformation("Command started · {Command}", name);
        }

        public CommandContext Context { get; }

        public static Command Begin(string name) => new Command(name, CommandContext.For(AppHost.Get<IAccessOpsSession>()));

        public void Dispose()
        {
            Log.LogInformation("Command completed · {Command} · {Elapsed} ms", _name, (int)(DateTime.UtcNow - _started).TotalMilliseconds);
            _scope.Dispose();
        }
    }
}
