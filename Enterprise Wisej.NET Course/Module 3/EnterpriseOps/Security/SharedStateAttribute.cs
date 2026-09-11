using System;

namespace EnterpriseOps.Security
{
    /// <summary>The eight owners a piece of state can have (lesson: "every piece of state needs an owner").</summary>
    public enum StateScope { Application, Tenant, User, Session, Tab, Workflow, BackgroundJob, Request }

    /// <summary>
    /// Documents a static field or singleton that is deliberately shared by every session on the server:
    /// what it holds, which scope owns it and how concurrent access is synchronized. In the static-state audit
    /// an undocumented mutable static is a finding, and a documented one with a per-user scope is a bug.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SharedStateAttribute : Attribute
    {
        public StateScope Scope { get; }
        public string Holds { get; }
        public string Synchronization { get; }

        public SharedStateAttribute(StateScope scope, string holds, string synchronization)
        {
            Scope = scope;
            Holds = holds;
            Synchronization = synchronization;
        }
    }
}
