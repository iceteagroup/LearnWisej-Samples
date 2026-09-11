using System;
using System.Collections.Generic;
using System.Drawing;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Layouts</b> section: the layout cards in a <see cref="FlowLayoutPanel"/>, the selected card in a
    /// <see cref="TableLayoutPanel"/> form, and the two reusable UserControls — <see cref="RecordHeader"/> and
    /// <see cref="StatusStrip"/> — driven through their properties and one event.
    /// </summary>
    public partial class LayoutsPage : UserControl, ISection, ISectionRecords
    {
        private readonly LayoutCardService _service = new LayoutCardService();
        private LayoutCard _selected;
        private DateTime? _lastRefresh;
        private bool _dirty;

        public LayoutsPage()
        {
            InitializeComponent();

            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
        }

        private void LayoutsPage_Load(object sender, EventArgs e)
        {
            ApplyNarrowLayout(IsNarrowProfile());
            LoadCards();
        }

        // ------------------------------------------------------------------------------------------------------------
        // ISection / ISectionRecords
        // ------------------------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public string Title => "Layouts";

        /// <inheritdoc/>
        public void RefreshSection() => LoadCards();

        /// <inheritdoc/>
        public int RecordCount => this._service.Count;

        /// <inheritdoc/>
        public string NewRecord()
        {
            var card = this._service.AddNext();
            this.flowChips.Controls.Add(CreateChip(card));
            this._dirty = true;

            SelectCard(card);
            UpdateReusableControls();
            return card.Id;
        }

        /// <inheritdoc/>
        public SectionSaveResult Save()
        {
            if (!this._dirty)
                return SectionSaveResult.Rejected("Nothing has changed on Layouts since the last save.");

            try
            {
                this._service.Save();
            }
            catch (Exception)
            {
                return SectionSaveResult.Failed("Layouts could not be saved — the service refused the write. Try again in a moment.");
            }

            this._dirty = false;
            UpdateReusableControls();
            return SectionSaveResult.Ok("Saved " + this._service.Count + " layout cards.");
        }

        // ------------------------------------------------------------------------------------------------------------
        // The two UserControls and the failure switch
        // ------------------------------------------------------------------------------------------------------------

        private void recordHeader_RefreshRequested(object sender, EventArgs e) => RefreshFromReusableControl(this.recordHeader.Name);
        private void statusStripLayouts_RefreshRequested(object sender, EventArgs e) => RefreshFromReusableControl(this.statusStripLayouts.Name);

        private void RefreshFromReusableControl(string source)
        {
            ShellStatus.Control(source);
            RefreshSection();
            ShellStatus.Show("Layouts refreshed at " + DateTime.Now.ToString("HH:mm:ss") + ".", StatusLevel.Ok);
        }

        private void chkSimulateServiceFailure_CheckedChanged(object sender, EventArgs e)
        {
            this._service.SimulateFailure = this.chkSimulateServiceFailure.Checked;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Data
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>Rebuilds the card chips from the service and stamps both reusable controls.</summary>
        private void LoadCards()
        {
            this.flowChips.SuspendLayout();
            ClearChips();

            foreach (var card in this._service.Cards)
                this.flowChips.Controls.Add(CreateChip(card));

            this.flowChips.ResumeLayout(true);

            this._lastRefresh = DateTime.Now;
            UpdateReusableControls();

            var fallback = this._service.Count == 0 ? null : this._service.Cards[0];
            SelectCard(this._selected ?? fallback);
        }

        private void ClearChips()
        {
            var previous = new List<Control>();
            foreach (Control chip in this.flowChips.Controls)
                previous.Add(chip);

            this.flowChips.Controls.Clear();

            foreach (var chip in previous)
                chip.Dispose();
        }

        /// <summary>One chip per card: a fixed-size Button the FlowLayoutPanel wraps.</summary>
        private Button CreateChip(LayoutCard card)
        {
            var chip = new Button
            {
                Name = "chip" + card.Id.Replace("-", ""),
                AccessibleName = "Layout card " + card.Id,
                Margin = new Padding(4),
                Size = new Size(228, 34),
                Tag = card,
                Text = card.Id + " · " + card.Region
            };
            chip.Click += chip_Click;
            return chip;
        }

        private void chip_Click(object sender, EventArgs e)
        {
            var chip = (Control)sender;
            ShellStatus.Control(chip.Name);
            SelectCard((LayoutCard)chip.Tag);
        }

        /// <summary>Fills the form and tells the shell which record is selected.</summary>
        private void SelectCard(LayoutCard card)
        {
            this._selected = card;

            this.txtRegion.Text = card == null ? "" : card.Region;
            this.txtContainer.Text = card == null ? "" : card.Container;
            this.txtRule.Text = card == null ? "" : card.Rule;

            ShellStatus.Record(card == null ? null : card.Id);
        }

        private void UpdateReusableControls()
        {
            this.recordHeader.Title = Title;
            this.recordHeader.RecordCount = this._service.Count;
            this.recordHeader.LastRefresh = this._lastRefresh;

            this.statusStripLayouts.Title = Title;
            this.statusStripLayouts.RecordCount = this._service.Count;
            this.statusStripLayouts.LastRefresh = this._lastRefresh;
        }

        // ------------------------------------------------------------------------------------------------------------
        // The section's own narrow layout: two columns become one, the controls are never re-created
        // ------------------------------------------------------------------------------------------------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
            => ApplyNarrowLayout(IsNarrowProfile());

        private static bool IsNarrowProfile()
        {
            var profile = Application.ActiveProfile == null ? "Desktop" : Application.ActiveProfile.Name;
            return profile == "Phone" || profile == "Tablet";
        }

        private void ApplyNarrowLayout(bool narrow)
        {
            this.tblDemo.SuspendLayout();
            try
            {
                this.tblDemo.ColumnStyles.Clear();
                this.tblDemo.RowStyles.Clear();

                if (narrow)
                {
                    this.tblDemo.ColumnCount = 1;
                    this.tblDemo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    this.tblDemo.RowCount = 2;
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

                    PlaceInCell(this.grpTable, 0, 0);
                    PlaceInCell(this.grpFlow, 0, 1);
                }
                else
                {
                    this.tblDemo.ColumnCount = 2;
                    this.tblDemo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    this.tblDemo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    this.tblDemo.RowCount = 1;
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                    PlaceInCell(this.grpTable, 0, 0);
                    PlaceInCell(this.grpFlow, 1, 0);
                }
            }
            finally
            {
                this.tblDemo.ResumeLayout(true);
            }
        }

        private void PlaceInCell(Control control, int column, int row)
        {
            this.tblDemo.SetColumn(control, column);
            this.tblDemo.SetRow(control, row);
        }
    }
}
