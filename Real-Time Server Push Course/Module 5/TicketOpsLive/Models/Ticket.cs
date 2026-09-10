using System;
using System.ComponentModel;

namespace TicketOpsLive.Models
{
    /// <summary>
    /// The binding-friendly ticket of the lesson. INotifyPropertyChanged makes property changes visible to the
    /// binding infrastructure; the sample still calls BindingSource.ResetBindings(false) once per event for clarity.
    /// UpdatedAt stays a real DateTime (the grid formats it as HH:mm:ss) — never a preformatted string.
    /// RecentlyUpdated is the temporary "updated" marker of the row-cue pattern; markerTimer clears it after 3 s.
    /// </summary>
    public class Ticket : INotifyPropertyChanged
    {
        private TicketStatus _status;
        private string _owner;
        private DateTime _updatedAt;
        private bool _recentlyUpdated;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }

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

        /// <summary>True for ~3 seconds after the row arrived or changed (the "updated" row cue).</summary>
        public bool RecentlyUpdated
        {
            get => _recentlyUpdated;
            set
            {
                _recentlyUpdated = value;
                OnPropertyChanged(nameof(RecentlyUpdated));
                OnPropertyChanged(nameof(Marker));
            }
        }

        /// <summary>What the marker column shows: "●" while RecentlyUpdated, otherwise nothing. Derived, never stored.</summary>
        public string Marker => _recentlyUpdated ? "●" : "";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
