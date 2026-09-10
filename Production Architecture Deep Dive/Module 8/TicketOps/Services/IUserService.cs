using System.Collections.Generic;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Who is signed in, and roster lookups. Lifetime: Session — <see cref="Current"/> is per-user state,
    /// the textbook example of what a Shared service must never hold.
    /// </summary>
    public interface IUserService
    {
        Operator Current { get; }

        IReadOnlyList<Operator> AllOperators();

        IReadOnlyList<Operator> FindTechnicians();

        /// <summary>Lab shortcut for "log in as": changes this session's current operator only.</summary>
        Operator SignInAs(int operatorId);
    }
}
