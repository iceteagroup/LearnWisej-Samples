using System;
using System.ComponentModel;

namespace TicketOpsLive.Models
{
    /// <summary>
    /// The binding-friendly ticket of the lesson. INotifyPropertyChanged makes property changes visible to the
    /// binding infrastructure; the sample still calls BindingSource.ResetBindings(false) once per event for clarity.
    /// UpdatedAt stays a real DateTime (the grid formats it as HH:mm:ss) — never a preformatted string.
    ///
    /// Module 6 adds <see cref="TenantId"/>: the metadata the lab asks for ("add tenant or role metadata and filter
    /// at least one event type"). The hub carries it on every event and each session decides whether the event is
    /// for it — the hub itself never keeps a list of who wants what.
    ///
    /// <see cref="Clone"/> is what makes the ownership boundary real. The hub hands out clones, never the instances
    /// it stores, so a session can bind, sort and even mutate its copy without touching global state.
    /// </summary>
    public class Ticket : INotifyPropertyChanged
    {
        private TicketStatus _status;
        private string _owner;
        private DateTime _updatedAt;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }

        /// <summary>The tenant this ticket belongs to ("Contoso" / "Northwind"). The filtering metadata of the lab.</summary>
        public string TenantId { get; set; }

        public string Owner
        {
            get => _owner;
            set { _owner = value; OnPropertyChanged(nameof(Owner)); }
        }

        public TicketStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { _updatedAt = value; OnPropertyChanged(nameof(UpdatedAt)); }
        }

        /// <summary>
        /// A detached copy. The hub clones on the way in (nobody keeps a reference to what it stores) and on the
        /// way out (GetSnapshot, the event args), so global state and session state never share an instance.
        /// </summary>
        public Ticket Clone()
        {
            return new Ticket
            {
                Id = this.Id,
                Title = this.Title,
                Customer = this.Customer,
                TenantId = this.TenantId,
                Owner = this.Owner,
                Status = this.Status,
                UpdatedAt = this.UpdatedAt
            };
        }

        /// <summary>Copies the mutable fields of <paramref name="source"/> into this instance (used by the hub's update path).</summary>
        public void CopyFrom(Ticket source)
        {
            if (source == null)
                return;

            this.Title = source.Title;
            this.Customer = source.Customer;
            this.TenantId = source.TenantId;
            this.Owner = source.Owner;
            this.Status = source.Status;
            this.UpdatedAt = source.UpdatedAt;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
