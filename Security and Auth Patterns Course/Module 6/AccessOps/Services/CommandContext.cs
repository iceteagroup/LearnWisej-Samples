using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AccessOps.Services
{
    /// <summary>
    /// One user command: a correlation ID minted where the command starts (the top of the event
    /// handler), plus the actor and the client details taken from the verified SessionContext -
    /// never from a control. Services receive it as a parameter instead of reaching back into the
    /// session, so work that outlives the click still knows who started it and under which ID.
    /// </summary>
    public sealed class CommandContext
    {
        private CommandContext(string correlationId, int actorId, string actor, string clientAddress, string userAgent)
        {
            this.CorrelationId = correlationId;
            this.ActorId = actorId;
            this.Actor = actor;
            this.ClientAddress = clientAddress;
            this.UserAgent = userAgent;
        }

        /// <summary>"7f3c-9ad2": short enough to read over the phone, quoted to the user as the reference.</summary>
        public string CorrelationId { get; }

        public int ActorId { get; }

        /// <summary>A stable identifier ("u-2081"), not a name: logs carry references, not people.</summary>
        public string Actor { get; }

        public string ClientAddress { get; }
        public string UserAgent { get; }

        public static CommandContext For(IAccessOpsSession session) => For(session.Current);

        public static CommandContext For(SessionContext me)
            => new CommandContext(NewCorrelationId(), me.EmployeeId, "u-" + me.EmployeeId, me.ClientAddress, me.UserAgent);

        /// <summary>A command with no signed-in actor: a sign-in attempt, a share-link request.</summary>
        public static CommandContext Anonymous(string actor, string clientAddress, string userAgent)
            => new CommandContext(NewCorrelationId(), 0, actor, clientAddress, userAgent);

        /// <summary>The logging scope every line written during the command inherits.</summary>
        public Dictionary<string, object> Scope() => new Dictionary<string, object>
        {
            ["CorrelationId"] = this.CorrelationId,
            ["UserId"] = this.Actor
        };

        // 32 random bits, formatted xxxx-xxxx. A reference for one command, not a key for anything;
        // a deployment that already speaks W3C trace context would use Activity.Current's trace id.
        private static string NewCorrelationId()
        {
            var hex = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
            return hex.Substring(0, 4) + "-" + hex.Substring(4, 4);
        }
    }
}
