using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TicketOps.Domain
{
    public enum WorkOrderStatus
    {
        Open,
        Scheduled,
        InProgress,
        Closed
    }

    public enum WorkOrderPriority
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// The observable work order: the row the grid shows and the record the detail editor edits.
    ///
    /// Plain C# with no UI dependency (it would compile in a class library that never references
    /// Wisej.NET). What makes it "observable" is <see cref="INotifyPropertyChanged"/>: every editable
    /// setter goes through <see cref="SetField{T}"/>, which raises <see cref="PropertyChanged"/> only when
    /// the value really changed. The BindingList that holds the rows hears that event and tells the grid
    /// to repaint the cell — without it, writing <c>order.Cost = 2150m</c> in code would leave the
    /// screen stale.
    ///
    /// Dirty tracking lives here too, as data: <see cref="AcceptChanges"/> takes a snapshot of the last
    /// saved values, <see cref="IsDirty"/> compares against it, <see cref="RejectChanges"/> restores it.
    /// The service decides WHEN those are called (after a successful save, on discard); the screen only
    /// displays the flag.
    ///
    /// Display strings ("$2,150.00", "Jun 14", "Unassigned") are deliberately absent: formatting is a UI
    /// concern and lives in the grid column styles and the CellFormatting handler.
    /// </summary>
    public sealed class WorkOrder : INotifyPropertyChanged
    {
        private string _title;
        private WorkOrderStatus _status;
        private WorkOrderPriority _priority;
        private string _assignedTo;
        private DateTime _dueDate;
        private decimal _cost;

        private Snapshot _saved;
        private bool _hasSnapshot;

        /// <summary>Assigned by the repository; never edited on screen, so it does not notify.</summary>
        public int Id { get; set; }

        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        public WorkOrderStatus Status
        {
            get => _status;
            set
            {
                // IsOverdue depends on Status: announce the derived value too, or the overdue tint never repaints.
                if (SetField(ref _status, value))
                    OnPropertyChanged(nameof(IsOverdue));
            }
        }

        public WorkOrderPriority Priority
        {
            get => _priority;
            set => SetField(ref _priority, value);
        }

        public string AssignedTo
        {
            get => _assignedTo;
            set => SetField(ref _assignedTo, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                if (SetField(ref _dueDate, value))
                    OnPropertyChanged(nameof(IsOverdue));
            }
        }

        public decimal Cost
        {
            get => _cost;
            set => SetField(ref _cost, value);
        }

        /// <summary>Derived: recomputed from DueDate and Status, announced from their setters.</summary>
        public bool IsOverdue => DueDate.Date < DateTime.Today && Status != WorkOrderStatus.Closed;

        /// <summary>True when any editable value differs from the last accepted (saved) snapshot.</summary>
        public bool IsDirty => _hasSnapshot && !_saved.Equals(Snapshot.Of(this));

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Marks the current values as the saved state: IsDirty becomes false.
        /// Called by the service after the repository accepted the record (and once when a row is loaded).
        /// </summary>
        public void AcceptChanges()
        {
            _saved = Snapshot.Of(this);
            _hasSnapshot = true;
            OnPropertyChanged(nameof(IsDirty));
        }

        /// <summary>
        /// Restores the last accepted values. Each setter raises PropertyChanged, so the grid row and the
        /// bound detail fields revert on screen without any manual copying.
        /// </summary>
        public void RejectChanges()
        {
            if (!_hasSnapshot)
                return;

            Title = _saved.Title;
            Status = _saved.Status;
            Priority = _saved.Priority;
            AssignedTo = _saved.AssignedTo;
            DueDate = _saved.DueDate;
            Cost = _saved.Cost;
            OnPropertyChanged(nameof(IsDirty));
        }

        /// <summary>A detached copy (the repository stores copies, never the live bound object).</summary>
        public WorkOrder Clone()
        {
            var copy = new WorkOrder
            {
                Id = Id,
                Title = Title,
                Status = Status,
                Priority = Priority,
                AssignedTo = AssignedTo,
                DueDate = DueDate,
                Cost = Cost
            };
            copy.AcceptChanges();
            return copy;
        }

        public override string ToString()
            => $"#{Id} \"{Title}\" {Status}/{Priority} → {AssignedTo ?? "(unassigned)"} due {DueDate:yyyy-MM-dd} cost {Cost}";

        /// <summary>
        /// One helper for every setter: no notification (and no false dirty flag) when the value is
        /// unchanged; PropertyChanged for the property AND for IsDirty when it is.
        /// </summary>
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(name);
            OnPropertyChanged(nameof(IsDirty));
            return true;
        }

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>The editable values, captured for dirty tracking and rollback.</summary>
        private readonly struct Snapshot : IEquatable<Snapshot>
        {
            public readonly string Title;
            public readonly WorkOrderStatus Status;
            public readonly WorkOrderPriority Priority;
            public readonly string AssignedTo;
            public readonly DateTime DueDate;
            public readonly decimal Cost;

            private Snapshot(WorkOrder o)
            {
                Title = o._title;
                Status = o._status;
                Priority = o._priority;
                AssignedTo = o._assignedTo;
                DueDate = o._dueDate;
                Cost = o._cost;
            }

            public static Snapshot Of(WorkOrder o) => new Snapshot(o);

            public bool Equals(Snapshot other)
                => string.Equals(Title, other.Title, StringComparison.Ordinal)
                && Status == other.Status
                && Priority == other.Priority
                && string.Equals(AssignedTo, other.AssignedTo, StringComparison.Ordinal)
                && DueDate == other.DueDate
                && Cost == other.Cost;

            public override bool Equals(object obj) => obj is Snapshot s && Equals(s);

            public override int GetHashCode() => HashCode.Combine(Title, Status, Priority, AssignedTo, DueDate, Cost);
        }
    }
}
