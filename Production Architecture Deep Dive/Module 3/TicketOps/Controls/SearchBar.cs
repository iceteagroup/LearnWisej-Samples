using System;
using Wisej.Web;

namespace TicketOps.Controls
{
    /// <summary>Carries the query text of a <see cref="SearchBar.SearchRequested"/> event.</summary>
    public sealed class SearchEventArgs : EventArgs
    {
        public SearchEventArgs(string query)
        {
            Query = query ?? "";
        }

        public string Query { get; }
    }

    /// <summary>
    /// Reusable, designer-friendly search control: one TextBox, one Button, one TableLayoutPanel.
    /// Drop it on any screen — the ticket toolbar, the activity feed, a lookup dialog — and talk to it
    /// through its small public surface only: <see cref="Query"/>, <see cref="Placeholder"/>,
    /// <see cref="ButtonText"/> and the <see cref="SearchRequested"/> event (raised on the button and on Enter).
    /// The inner TextBox and Button are private on purpose: publish intent, not implementation.
    /// The Ticket Workspace uses two instances (tickets, activity) instead of duplicating the wiring.
    /// </summary>
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            InitializeComponent();
        }

        /// <summary>Host screens subscribe to this instead of wiring their own textbox events.</summary>
        public event EventHandler<SearchEventArgs> SearchRequested;

        /// <summary>The current query, trimmed. Set it to pre-fill the box (e.g. when a screen restores state).</summary>
        public string Query
        {
            get => this.txtSearch.Text?.Trim() ?? "";
            set => this.txtSearch.Text = value ?? "";
        }

        /// <summary>The watermark shown while the box is empty ("Search tickets…").</summary>
        public string Placeholder
        {
            get => this.txtSearch.Watermark;
            set => this.txtSearch.Watermark = value;    // expose intent, hide the textbox
        }

        /// <summary>The caption of the button ("Search", "Filter", "Go").</summary>
        public string ButtonText
        {
            get => this.btnSearch.Text;
            set => this.btnSearch.Text = value;
        }

        /// <summary>Empties the box without raising <see cref="SearchRequested"/>.</summary>
        public void Clear()
        {
            this.txtSearch.Text = "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RaiseSearchRequested();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                RaiseSearchRequested();
        }

        private void RaiseSearchRequested()
        {
            SearchRequested?.Invoke(this, new SearchEventArgs(Query));   // raise a clean event
        }
    }
}
