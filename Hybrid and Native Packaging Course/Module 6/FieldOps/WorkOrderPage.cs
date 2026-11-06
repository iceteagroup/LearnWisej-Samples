using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// FieldOps on a phone or tablet (Module 4): the list, then the detail full screen on a phone,
    /// side by side on a tablet. It survives the phone's habits - a call sends it to the
    /// background, ReloadOnForeground reloads it, Android's back gesture asks to go back - because
    /// the open order and the draft are saved outside the controls and every way back goes through
    /// one NavigateBack.
    /// </summary>
    public partial class WorkOrderPage : Page
    {
        private const string PrefOrder = "fieldops.wo";
        private const string PrefDraft = "fieldops.draft";

        private readonly WorkOrderService _service = WorkOrderService.Current;
        private IReadOnlyList<WorkOrder> _orders = Array.Empty<WorkOrder>();
        private WorkOrder _open;            // the detail on screen, null = the list
        private bool _dirty;                // an unsent reading or note
        private bool _sideBySide;           // tablet: list and detail together
        private bool _loading;              // filling the detail must not count as typing
        private Form _dialog;               // an open dialog: the first thing Back closes
        private bool _subscribed;

        public WorkOrderPage()
        {
            InitializeComponent();
        }

        private void WorkOrderPage_Load(object sender, EventArgs e)
        {
            _sideBySide = Device.Valid ? Device.System.IsTablet : Width >= 900;
            if (_sideBySide)
            {
                gridList.Dock = DockStyle.Left;
                gridList.Width = 360;
                pnlDetail.Visible = true;
            }

            SetStatus(Device.Valid ? "Remote · FieldOps server" : "Browser · FieldOps server", Ops.Good);
            lblPlatform.Text = Device.Valid ? Device.System.Platform.ToString() : "Browser";

            ShowList();

            if (Device.Valid)
            {
                SignIn.ApplyTitle(Session.Technician);
                // The system strip in the app-bar blue (phones; ignored where there is none).
                Device.StatusBar.BackColor = Ops.Accent;

                Device.Lifecycle.Backgrounding += Lifecycle_Backgrounding;
                Device.Lifecycle.Stopped += Lifecycle_Stopped;
                Device.Lifecycle.Resumed += Lifecycle_Resumed;
                Device.Lifecycle.Activated += Lifecycle_Activated;
                Device.Lifecycle.Deactivated += Lifecycle_Deactivated;
                Device.Application.BackRequested += Application_BackRequested;
                _subscribed = true;
            }

            // Also after a ReloadOnForeground reload: the page is new, the draft is not.
            RestoreDraft();
        }

        private void UnsubscribeDevice()
        {
            if (!_subscribed) return;
            _subscribed = false;
            try
            {
                Device.Lifecycle.Backgrounding -= Lifecycle_Backgrounding;
                Device.Lifecycle.Stopped -= Lifecycle_Stopped;
                Device.Lifecycle.Resumed -= Lifecycle_Resumed;
                Device.Lifecycle.Activated -= Lifecycle_Activated;
                Device.Lifecycle.Deactivated -= Lifecycle_Deactivated;
                Device.Application.BackRequested -= Application_BackRequested;
            }
            catch (Exception ex)
            {
                OpsLog.Error("unsubscribing device events", ex);
            }
        }

        #region Lifecycle: save on the way out, restore on the way back

        private void Lifecycle_Backgrounding(object sender, EventArgs e)
        {
            OpsLog.Info($"lifecycle: Backgrounding (session {Application.SessionId[..8]}, open={_open?.Number ?? "list"})");
            SaveDraft();
            SetStatus("Background · draft saved", Color.FromArgb(0x8A, 0x93, 0xA0));
        }

        private void Lifecycle_Stopped(object sender, EventArgs e)
        {
            OpsLog.Info($"lifecycle: Stopped (session {Application.SessionId[..8]})");
            SaveDraft();
        }

        private void Lifecycle_Resumed(object sender, EventArgs e)
        {
            OpsLog.Info($"lifecycle: Resumed (session {Application.SessionId[..8]})");
            SetStatus("Remote · FieldOps server", Ops.Good);
            // With ReloadOnForeground = false this page is still here: put the draft back anyway.
            RestoreDraft();
            Application.Update(this);
        }

        private void Lifecycle_Activated(object sender, EventArgs e) =>
            OpsLog.Info($"lifecycle: Activated (session {Application.SessionId[..8]})");

        private void Lifecycle_Deactivated(object sender, EventArgs e) =>
            OpsLog.Info($"lifecycle: Deactivated (session {Application.SessionId[..8]})");

        /// <summary>The open order id and the reading. On every field change, and on Backgrounding.</summary>
        private void SaveDraft()
        {
            if (_open == null) return;
            DraftStore.Save(Session.Technician.Id, _open.Number, txtReading.Text, txtNotes.Text);
            if (Device.Valid)
            {
                // Simple values only: an id and a short reading. The note lives on the server.
                Device.Preferences.Set(PrefOrder, _open.Number);
                Device.Preferences.Set(PrefDraft, txtReading.Text ?? "");
            }
        }

        private void ClearDraft()
        {
            DraftStore.Clear(Session.Technician.Id);
            if (Device.Valid)
            {
                Device.Preferences.Remove(PrefOrder);
                Device.Preferences.Remove(PrefDraft);
            }
        }

        private void RestoreDraft()
        {
            string number = null, reading = null, notes = null;
            var draft = DraftStore.Get(Session.Technician.Id);
            if (draft != null)
            {
                (number, reading, notes) = (draft.Number, draft.Reading, draft.Notes);
            }
            else if (Device.Valid)
            {
                number = Device.Preferences.Get(PrefOrder, "") as string;
                reading = Device.Preferences.Get(PrefDraft, "") as string;
            }
            if (string.IsNullOrEmpty(number)) return;

            var order = _orders.FirstOrDefault(o => o.Number == number);
            if (order == null) return;

            ShowDetail(order, reading, notes);
            _dirty = !string.IsNullOrEmpty(reading) || !string.IsNullOrEmpty(notes);
            ShowBanner($"Draft restored — {order.Number}" + (string.IsNullOrEmpty(reading) ? "" : $" · reading {reading}"), "good");
            OpsLog.Info($"draft: restored {order.Number} reading={reading} (session {Application.SessionId[..8]})");
        }

        private void Draft_Changed(object sender, EventArgs e)
        {
            if (_loading || _open == null) return;
            _dirty = true;
            SaveDraft();
        }

        #endregion

        #region One way back: Android's gesture, the app-bar arrow, the browser

        private void Application_BackRequested(object sender, EventArgs e)
        {
            OpsLog.Info($"back: Device.Application.BackRequested (session {Application.SessionId[..8]})");
            NavigateBack();
        }

        private void btnBack_Click(object sender, EventArgs e) => NavigateBack();

        private async void NavigateBack()
        {
            if (_dialog != null) { _dialog.Close(); return; }   // a dialog first
            if (_open == null) return;                            // the list: nothing to pop
            if (_dirty && !await ConfirmLeave()) return;          // unsent reading: ask first
            ShowList();
            Application.Update(this);
        }

        private async Task<bool> ConfirmLeave()
        {
            _dialog = new LeaveDialog(_open.Number, string.IsNullOrEmpty(txtReading.Text) ? "(none)" : txtReading.Text);
            try
            {
                var answer = await _dialog.ShowDialogAsync();
                if (answer == DialogResult.Yes)
                {
                    ClearDraft();
                    return true;
                }
                return false;
            }
            finally
            {
                _dialog = null;
            }
        }

        #endregion

        #region List and detail

        private void LoadList()
        {
            try
            {
                _orders = _service.GetTodaysOrders(Session.Technician.Id);
                gridList.DataSource = _orders.Select(o => new { Card = Card(o) }).ToList();
                colCard.DataPropertyName = "Card";
            }
            catch (Exception ex)
            {
                OpsLog.Error("loading the phone list", ex);
                ShowBanner("Your work orders could not be loaded. Pull down to try again.", "bad");
            }
        }

        private static string Card(WorkOrder o) =>
            $"<div style=\"padding:4px 2px\"><div style=\"display:flex;align-items:center;gap:8px\">" +
            $"<span style=\"font-family:Consolas,monospace;font-size:12px;font-weight:800;color:#1565d8\">{o.Number}</span>" +
            $"<span style=\"margin-left:auto\">{Ops.StatusChip(o.Status, 10)}</span></div>" +
            $"<div style=\"font-size:12.5px;color:#1f2d3a;font-weight:600;margin-top:3px\">{WebUtility.HtmlEncode(o.Customer)}</div>" +
            $"<div style=\"font-size:11px;color:#7d8ea0;margin-top:2px\">{o.Priority} priority</div></div>";

        private void gridList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _orders.Count) return;
            var next = _orders[e.RowIndex];
            if (_open != null && _open.Number == next.Number) return;
            ShowDetail(next, null, null);
        }

        private void ShowList()
        {
            _open = null;
            _dirty = false;
            lblDetailMessage.Text = "";
            lblBanner.Visible = false;
            lblAppTitle.Text = "FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:12px\">· Work orders</span>";
            if (!_sideBySide)
            {
                pnlDetail.Visible = false;
                gridList.Visible = true;
            }
            LoadList();
        }

        private void ShowDetail(WorkOrder order, string reading, string notes)
        {
            _open = _service.Get(order.Number);
            _loading = true;
            try
            {
                lblAppTitle.Text = $"FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:12px\">· {_open.Number}</span>";
                lblDetailHeader.Text =
                    $"<div style=\"display:flex;align-items:center;gap:8px\"><span style=\"font-family:Consolas,monospace;font-size:12.5px;font-weight:800;color:#1565d8\">{_open.Number}</span>" +
                    $"<span style=\"margin-left:auto\">{Ops.StatusChip(_open.Status, 10)}</span></div>" +
                    $"<div style=\"font-size:14px;color:#1f2d3a;font-weight:700;margin-top:5px\">{WebUtility.HtmlEncode(_open.Customer)}</div>" +
                    $"<div style=\"font-size:11px;color:#7d8ea0;margin-top:2px\">{_open.Priority} priority · {WebUtility.HtmlEncode(_open.Fault)}</div>";
                txtReading.Text = reading ?? _open.MeterReading?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "";
                txtNotes.Text = notes ?? _open.Notes ?? "";
                lblDetailMessage.Text = "";
                _dirty = false;
            }
            finally
            {
                _loading = false;
            }

            if (!_sideBySide) gridList.Visible = false;
            pnlDetail.Visible = true;
        }

        private void btnSubmitReading_Click(object sender, EventArgs e)
        {
            if (_open == null) return;
            try
            {
                var saved = _service.SubmitReading(_open.Number, txtReading.Text, txtNotes.Text, _open.Version, Session.Technician.Id);
                _open = saved;
                _dirty = false;
                ClearDraft();
                ShowMessage($"Reading {saved.MeterReading} submitted for {saved.Number}.", "good");
                OpsLog.Info($"{saved.Number}: reading {saved.MeterReading} submitted by {Session.Technician.Id}");
            }
            catch (StatusTransitionException ex)
            {
                ShowMessage(ex.Message, "warn");
            }
            catch (WorkOrderConflictException ex)
            {
                OpsLog.Error($"submit reading {_open.Number}", ex);
                ShowMessage($"{_open.Number} was changed somewhere else. Your reading is kept — check the order and submit again.", "bad");
                _open = _service.Get(_open.Number);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"submit reading {_open.Number}", ex);
                ShowMessage("The reading could not be sent. It is kept on this screen — try again in a moment.", "bad");
            }
        }

        #endregion

        private void SetStatus(string text, Color dot)
        {
            lblStatusText.Text = $"<span style=\"display:inline-block;width:7px;height:7px;border-radius:999px;background:{ColorTranslator.ToHtml(dot)};margin-right:8px\"></span>{WebUtility.HtmlEncode(text)}";
        }

        private void ShowBanner(string text, string tone)
        {
            var (back, line, ink) = Tone(tone);
            lblBanner.Text =
                $"<div style=\"display:flex;align-items:center;gap:9px;height:100%;padding:0 13px;background:{back};border-left:4px solid {line};border-bottom:1px solid {line}55\">" +
                $"<span style=\"flex:none;width:17px;height:17px;border-radius:999px;background:{line};color:#fff;display:flex;align-items:center;justify-content:center;font-size:10.5px;font-weight:800\">i</span>" +
                $"<span style=\"font-size:11.5px;color:{ink};line-height:1.35\">{WebUtility.HtmlEncode(text)}</span></div>";
            lblBanner.Visible = true;
        }

        private void ShowMessage(string text, string tone)
        {
            var (back, line, ink) = Tone(tone);
            lblDetailMessage.Text = $"<div style=\"margin-top:10px;padding:8px 11px;background:{back};border-left:4px solid {line};border-radius:6px;font-size:11.5px;color:{ink}\">{WebUtility.HtmlEncode(text)}</div>";
        }

        private static (string back, string line, string ink) Tone(string tone) => tone switch
        {
            "warn" => ("#fff4e0", "#e8a13c", "#8a5a10"),
            "bad" => ("#fdecea", "#c0392b", "#7d2018"),
            "good" => ("#e6f6ee", "#1f9d6b", "#14603f"),
            _ => ("#eaf3ff", "#1565d8", "#144a99"),
        };
    }
}
