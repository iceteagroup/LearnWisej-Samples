using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>Fake profile: a fixed roster, signed in as Dana Reyes (Technician) until told otherwise.</summary>
    public sealed class FakeUserService : IUserService
    {
        private readonly List<Operator> _roster = SeedData.Operators().ToList();
        private readonly ILog _log;
        private Operator _current;

        public FakeUserService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _current = _roster.First(o => o.Id == SeedData.DefaultOperatorId);
            _log.Info(LogLayer.Session, "FakeUserService", $"signed in as {_current} (session-scoped)");
        }

        public Operator Current => _current;

        public IReadOnlyList<Operator> AllOperators() => _roster.ToList();

        public IReadOnlyList<Operator> FindTechnicians() => _roster.Where(o => o.Role == OperatorRole.Technician).ToList();

        public Operator SignInAs(int operatorId)
        {
            var next = _roster.FirstOrDefault(o => o.Id == operatorId)
                       ?? throw new ArgumentException($"Unknown operator {operatorId}", nameof(operatorId));
            _current = next;
            _log.Info(LogLayer.Session, "FakeUserService.SignInAs", $"current operator → {next} (this session only)");
            return next;
        }
    }
}
