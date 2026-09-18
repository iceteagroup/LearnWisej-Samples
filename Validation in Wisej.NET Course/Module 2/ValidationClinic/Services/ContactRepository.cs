using System;
using System.Collections.Generic;
using System.Linq;
using ValidationClinic.Models;

namespace ValidationClinic.Services
{
    public sealed class DuplicateNameException : Exception { }

    // Session-owned stand-in for a database. All checks happen before replacement.
    public sealed class ContactRepository
    {
        private List<ContactEditModel> contacts = new List<ContactEditModel>
        {
            new ContactEditModel { Id = 1, Name = "Ada Lovelace", Email = "ada@example.com", Age = 36 },
            new ContactEditModel { Id = 2, Name = "Grace Hopper", Email = "grace@example.com", Age = 45 }
        };
        public bool FailNextSave { get; set; }
        public int WriteCount { get; private set; }
        public List<ContactEditModel> Snapshot() => contacts.Select(x => x.Copy()).ToList();

        public void Save(ContactEditModel model) => SaveAll(Snapshot(), model);

        public void SaveAll(IEnumerable<ContactEditModel> rows, ContactEditModel edited = null)
        {
            var next = rows.Select(x => x.Copy()).ToList();
            if (edited != null)
            {
                next.RemoveAll(x => edited.Id != 0 && x.Id == edited.Id);
                next.Add(edited.Copy());
            }
            if (next.GroupBy(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
                throw new DuplicateNameException();
            if (FailNextSave)
            {
                FailNextSave = false;
                throw new InvalidOperationException("Simulated repository outage; nothing was written.");
            }
            int nextId = Math.Max(contacts.Select(x => x.Id).DefaultIfEmpty().Max(), next.Select(x => x.Id).DefaultIfEmpty().Max()) + 1;
            foreach (var item in next.Where(x => x.Id == 0)) item.Id = nextId++;
            if (edited != null) edited.Id = next.Last().Id;
            contacts = next;
            WriteCount++;
        }
    }
}
