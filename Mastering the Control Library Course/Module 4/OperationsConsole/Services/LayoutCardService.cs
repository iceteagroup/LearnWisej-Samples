using System;
using System.Collections.Generic;
using OperationsConsole.Models;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The in-memory data behind the Layouts section: one card per region of the console.
    /// <see cref="SimulateFailure"/> makes <see cref="Save"/> throw, so the error outcome of Save can be shown.
    /// </summary>
    public sealed class LayoutCardService
    {
        private static readonly string[][] Extra =
        {
            new[] { "Filters",       "FlowLayoutPanel",           "a collection of controls that must wrap, not clip" },
            new[] { "Detail form",   "TableLayoutPanel",          "aligned labels and editors in rows and columns" },
            new[] { "Preview pane",  "SplitContainer.Panel2",     "the user decides how much room the preview gets" },
            new[] { "Notes",         "TabPage (peer section)",    "a peer view, never a required step of a workflow" },
            new[] { "Toolbar extras","ToolBar (AutoOverflow)",    "frequent commands stay reachable when the width shrinks" }
        };

        private readonly List<LayoutCard> _cards = new List<LayoutCard>();
        private int _nextId;

        public LayoutCardService()
        {
            Add("Command surface", "ToolBar · Dock = Top", "frequently used commands live on a ToolBar");
            Add("Status surface", "StatusBar · Dock = Bottom", "real status: profile, record count, last refresh");
            Add("Work surface", "SplitContainer · Dock = Fill", "two adjacent regions the user can resize");
            Add("Navigation", "SplitContainer.Panel1", "the navigation is a region, so it is docked, not positioned");
            Add("Detail surface", "TabControl · Dock = Fill", "six peer sections, no hidden required fields");
            Add("Record header", "RecordHeader UserControl", "repeated UI belongs in a UserControl, not in six copies");
        }

        /// <summary>When true, <see cref="Save"/> throws.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>The cards, in the order they were created.</summary>
        public IReadOnlyList<LayoutCard> Cards => _cards;

        /// <summary>How many cards the section holds.</summary>
        public int Count => _cards.Count;

        /// <summary>Adds the next card from the sample set and returns it.</summary>
        public LayoutCard AddNext()
        {
            var row = Extra[(_nextId - 6 + Extra.Length * 4) % Extra.Length];
            return Add(row[0], row[1], row[2]);
        }

        /// <summary>Saves the cards.</summary>
        public void Save()
        {
            if (SimulateFailure)
                throw new InvalidOperationException("The layout cards could not be saved.");
        }

        private LayoutCard Add(string region, string container, string rule)
        {
            var card = new LayoutCard("LAY-" + (++_nextId).ToString("000"), region, container, rule);
            _cards.Add(card);
            return card;
        }
    }
}
