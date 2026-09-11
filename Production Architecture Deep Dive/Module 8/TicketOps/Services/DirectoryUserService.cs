using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Production profile: the operator roster would come from the company directory (LDAP / Entra ID)
    /// and the current operator from the authenticated principal. This stand-in logs the directory
    /// queries it would issue and answers from the seed roster, so the screen behaves the same.
    /// </summary>
    public sealed class DirectoryUserService : IUserService
    {
        private readonly List<Operator> _roster = SeedData.Operators().ToList();
        private readonly ILog _log;
        private Operator _current;

        public DirectoryUserService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _current = _roster.First(o => o.Id == SeedData.DefaultOperatorId);
            _log.Info(LogLayer.Session, "DirectoryUserService", $"would resolve the authenticated principal → {_current} (stand-in)");
        }

        public Operator Current => _current;

        public IReadOnlyList<Operator> AllOperators()
        {
            _log.Info(LogLayer.Data, "DirectoryUserService.AllOperators", "would query: (&(objectClass=user)(memberOf=CN=TicketOps,OU=Groups))");
            return _roster.ToList();
        }

        public IReadOnlyList<Operator> FindTechnicians()
        {
            _log.Info(LogLayer.Data, "DirectoryUserService.FindTechnicians", "would query: (&(objectClass=user)(memberOf=CN=TicketOps-Technicians,OU=Groups))");
            return _roster.Where(o => o.Role == OperatorRole.Technician).ToList();
        }

        public Operator SignInAs(int operatorId)
        {
            var next = _roster.FirstOrDefault(o => o.Id == operatorId)
                       ?? throw new ArgumentException($"Unknown operator {operatorId}", nameof(operatorId));
            _current = next;
            return next;
        }
    }
}
