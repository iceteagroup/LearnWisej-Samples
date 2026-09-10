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
    /// The <b>Layouts</b> section (Module 3 · Containers, Layouts, Navigation, and Reuse).
    /// <para>
    /// Three demos inside a <see cref="TableLayoutPanel"/> frame: a <see cref="TableLayoutPanel"/> form with
    /// aligned labels and editors, a <see cref="FlowLayoutPanel"/> of cards that wraps instead of being clipped,
    /// and an anchored panel where each control says what its <c>Anchor</c> does. On top sit the two reusable
    /// UserControls the lab asks for — <see cref="RecordHeader"/> and <see cref="StatusStrip"/> — which this page
    /// drives through three properties and one event and nothing else.
    /// </para>
    /// <para>
    /// The page also owns a narrow layout of its own: a section reflows its content, the shell reflows the shell.
    /// </para>
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

            // profile-aware logic that belongs to the section, not to the shell
            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
        }

        private void LayoutsPage_Load(object sender, EventArgs e)
        {
            ConsoleLog.Add("LayoutsPage · tblDemo (Fill) added first, then pnlCommands / statusStripLayouts / recordHeader (Top)");
            SyncNarrowWithProfile("LayoutsPage.Load");
            LoadCards("LayoutsPage.Load");
        }

        // ------------------------------------------------------------------------------------------------------------
        // ISection — what the shell's Refresh command calls
        // ------------------------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public string Title => "Layouts";

        /// <inheritdoc/>
        public void RefreshSection() => LoadCards("RefreshSection");

        // ------------------------------------------------------------------------------------------------------------
        // ISectionRecords — what the shell's StatusBar, New and Save commands need
        // ------------------------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public int RecordCount => this._service.Count;

        /// <inheritdoc/>
        public string NewRecord()
        {
            var card = this._service.AddNext();
            this.flowChips.Controls.Add(CreateChip(card));
            this._dirty = true;

            ConsoleLog.Add("LayoutCardService.AddNext() → " + card.Id + " (" + this._service.Count + " cards, the FlowLayoutPanel rewraps)");
            SelectCard(card, "flowChips");
            UpdateReusableControls();
            return card.Id;
        }

        /// <inheritdoc/>
        public SectionSaveResult Save()
        {
            if (!this._dirty)
            {
                ConsoleLog.Add("LayoutsPage.Save() → nothing has changed, the service was not called");
                return SectionSaveResult.Rejected("Nothing has changed on Layouts since the last save.");
            }

            try
            {
                this._service.Save();
            }
            catch (Exception ex)
            {
                // the exception text goes to the Event log only; the user gets a sentence they can act on
                ConsoleLog.Add("✗ LayoutCardService.Save() — " + ex.GetType().Name);
                ConsoleLog.Add("   " + ex.Message);
                return SectionSaveResult.Failed("Layouts could not be saved — the service refused the write. Try again in a moment.");
            }

            this._dirty = false;
            ConsoleLog.Add("✓ LayoutCardService.Save() — " + this._service.Count + " cards");
            UpdateReusableControls();
            return SectionSaveResult.Ok("Saved " + this._service.Count + " layout cards.");
        }

        // ------------------------------------------------------------------------------------------------------------
        // Command row and the two UserControls — thin handlers calling named methods
        // ------------------------------------------------------------------------------------------------------------

        private void btnAddCard_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(this.btnAddCard.Name);
            var id = NewRecord();
            ConsoleLog.Status("Added " + id + " to Layouts.", StatusLevel.Ok);
        }

        private void recordHeader_RefreshRequested(object sender, EventArgs e) => RefreshFromReusableControl("recordHeader");
        private void statusStripLayouts_RefreshRequested(object sender, EventArgs e) => RefreshFromReusableControl("statusStripLayouts");

        /// <summary>
        /// Both UserControls raise the same event and this page answers it the same way — it never asks which
        /// button was clicked, because neither control exposes a button.
        /// </summary>
        private void RefreshFromReusableControl(string source)
        {
            ConsoleLog.Control(source);
            ConsoleLog.Add(source + ".RefreshRequested → LayoutsPage.RefreshSection()");
            RefreshSection();
            ConsoleLog.Status("Layouts refreshed at " + DateTime.Now.ToString("HH:mm:ss") + ".", StatusLevel.Ok);
        }

        private void chkNarrowLayout_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleLog.Control(this.chkNarrowLayout.Name);
            ApplyNarrowLayout(this.chkNarrowLayout.Checked);
        }

        private void chkSimulateServiceFailure_CheckedChanged(object sender, EventArgs e)
        {
            this._service.SimulateFailure = this.chkSimulateServiceFailure.Checked;
            ConsoleLog.Control(this.chkSimulateServiceFailure.Name);
            ConsoleLog.Add(this._service.SimulateFailure
                ? "LayoutCardService.SimulateFailure = true — add a card, then Save, to see the error path"
                : "LayoutCardService.SimulateFailure = false — saving works again (recovery)");
            ConsoleLog.Status(this._service.SimulateFailure
                ? "Save failures are being simulated on Layouts."
                : "Save failures are no longer simulated on Layouts.",
                this._service.SimulateFailure ? StatusLevel.Warning : StatusLevel.Ok);
        }

        private void btnAnchorRight_Click(object sender, EventArgs e)
            => ExplainAnchor(this.btnAnchorRight.Name, "Anchor = Top | Right — the width never changes, the gap to the right edge never changes.");

        private void btnAnchorBottom_Click(object sender, EventArgs e)
            => ExplainAnchor(this.btnAnchorBottom.Name, "Anchor = Bottom | Right — it stays in the corner however the panel is resized.");

        private void ExplainAnchor(string controlName, string explanation)
        {
            ConsoleLog.Control(controlName);
            ConsoleLog.Add(controlName + " → " + explanation);
            ConsoleLog.Status(explanation, StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Data
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>Rebuilds the card chips from the service and stamps both reusable controls.</summary>
        private void LoadCards(string source)
        {
            this.flowChips.SuspendLayout();
            ClearChips();

            foreach (var card in this._service.Cards)
                this.flowChips.Controls.Add(CreateChip(card));

            this.flowChips.ResumeLayout(true);

            this._lastRefresh = DateTime.Now;
            UpdateReusableControls();

            // keep the selected card across a refresh; the chips are new objects, the model is not
            var fallback = this._service.Count == 0 ? null : this._service.Cards[0];
            SelectCard(this._selected == null ? fallback : this._selected, null);

            ConsoleLog.Add(source + " → LayoutCardService.Cards = " + this._service.Count + " cards rebuilt into flowChips");
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

        /// <summary>One chip per card: a fixed-size Button the FlowLayoutPanel is free to wrap.</summary>
        private Button CreateChip(LayoutCard card)
        {
            var chip = new Button
            {
                Name = "chip" + card.Id.Replace("-", ""),
                AccessibleName = "Layout card " + card.Id,
                Margin = new Padding(4),
                Size = new Size(228, 34),
                Tag = card,
                Text = card.Id + " · " + card.Region,
                ToolTipText = card.Container + " — " + card.Rule
            };
            chip.Click += chip_Click;
            return chip;
        }

        private void chip_Click(object sender, EventArgs e)
        {
            var chip = (Control)sender;
            SelectCard((LayoutCard)chip.Tag, chip.Name);
        }

        /// <summary>
        /// Fills the TableLayoutPanel form and tells the shell which record is selected.
        /// <paramref name="source"/> is the control the user actually used, or <c>null</c> when the selection is
        /// only being restored (a refresh must not claim the user clicked a chip).
        /// </summary>
        private void SelectCard(LayoutCard card, string source)
        {
            this._selected = card;

            this.txtRegion.Text = card == null ? "" : card.Region;
            this.txtContainer.Text = card == null ? "" : card.Container;
            this.txtRule.Text = card == null ? "" : card.Rule;

            if (!string.IsNullOrEmpty(source))
                ConsoleLog.Control(source);

            ConsoleLog.Record(card == null ? null : card.Id);
        }

        /// <summary>
        /// The whole conversation with the two reusable controls: three properties each. No child control of
        /// <see cref="RecordHeader"/> or <see cref="StatusStrip"/> is ever touched from here.
        /// </summary>
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
        // The section's own narrow layout
        // ------------------------------------------------------------------------------------------------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
            => SyncNarrowWithProfile("Application.ResponsiveProfileChanged");

        private void SyncNarrowWithProfile(string source)
        {
            var profile = Application.ActiveProfile == null ? "Desktop" : Application.ActiveProfile.Name;
            var narrow = profile == "Phone" || profile == "Tablet";

            if (this.chkNarrowLayout.Checked != narrow)
            {
                ConsoleLog.Add(source + " → LayoutsPage narrow = " + (narrow ? "true" : "false") + " (" + profile + ")");
                this.chkNarrowLayout.Checked = narrow;   // raises CheckedChanged → ApplyNarrowLayout
            }
            else
            {
                ApplyNarrowLayout(narrow);
            }
        }

        /// <summary>
        /// Two columns become one and the cells are re-assigned — the controls themselves are never re-created.
        /// This is the shell's <c>ApplyNarrowProfile</c> idea, one level down.
        /// </summary>
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
                    this.tblDemo.RowCount = 3;
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

                    this.tblDemo.SetColumnSpan(this.grpAnchor, 1);
                    PlaceInCell(this.grpTable, 0, 0);
                    PlaceInCell(this.grpFlow, 0, 1);
                    PlaceInCell(this.grpAnchor, 0, 2);
                }
                else
                {
                    this.tblDemo.ColumnCount = 2;
                    this.tblDemo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    this.tblDemo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    this.tblDemo.RowCount = 2;
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 52F));
                    this.tblDemo.RowStyles.Add(new RowStyle(SizeType.Percent, 48F));

                    PlaceInCell(this.grpTable, 0, 0);
                    PlaceInCell(this.grpFlow, 1, 0);
                    PlaceInCell(this.grpAnchor, 0, 1);
                    this.tblDemo.SetColumnSpan(this.grpAnchor, 2);
                }
            }
            finally
            {
                this.tblDemo.ResumeLayout(true);
            }

            ConsoleLog.Add("LayoutsPage.ApplyNarrowLayout(" + (narrow ? "true" : "false") + ") → tblDemo.ColumnCount = " + this.tblDemo.ColumnCount);
        }

        private void PlaceInCell(Control control, int column, int row)
        {
            this.tblDemo.SetColumn(control, column);
            this.tblDemo.SetRow(control, row);
        }
    }
}
