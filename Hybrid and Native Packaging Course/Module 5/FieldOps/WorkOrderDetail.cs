using System;
using System.Drawing;
using System.Linq;
using System.Net;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// One work order and its device features (Module 5): Take Photo, Signature, Navigate and the
    /// reminder - each from an ordinary server-side handler through Wisej.Hybrid.Device, each shown
    /// only where it works (absent, not disabled), each with a browser answer. The Module 1 status
    /// flow stays at the bottom.
    /// </summary>
    public partial class WorkOrderDetail : Page
    {
        private const string CameraReason = "FieldOps uses the camera to attach a photo of the fault to this work order.";
        private const string LocationReason = "FieldOps uses your location to show the distance to the site and stamp your arrival time.";
        private const string SignaturePadId = "fo-sig";

        private readonly WorkOrderService _service = WorkOrderService.Current;
        private readonly PhotoStore _photoStore = PhotoStore.Current;
        private readonly SignatureService _signatures = SignatureService.Current;
        private readonly PermissionGate _permissions;
        private readonly string _number;
        private WorkOrder _workOrder;
        private bool _signed;

        public WorkOrderDetail(string number)
        {
            InitializeComponent();
            _number = number;
            _permissions = new PermissionGate((text, tone) => { ShowBanner(text, tone); Application.Update(this); });
        }

        private void WorkOrderDetail_Load(object sender, EventArgs e)
        {
            // The capability matrix, read once per session (DeviceCapabilities), drives the actions.
            var caps = DeviceCapabilities.Current;
            btnPhoto.Visible = caps.Camera;
            uploadPhoto.Visible = !caps.Camera;           // browser, PWA, or a client without a camera
            btnNavigate.Text = caps.NativeNavigate
                ? "Navigate <span style=\"font-size:10.5px;opacity:.85;margin-left:6px\">DEVICE</span>"
                : "Open route";
            btnNavigate.BackColor = caps.NativeNavigate ? Ops.Accent : Color.White;
            btnNavigate.ForeColor = caps.NativeNavigate ? Color.White : Ops.Accent;
            lblFootNote.Text = "Device.Valid = " + (caps.Valid ? "true" : "false");

            // The drawing surface: plain canvas + a few lines of client script. Same in both clients.
            Application.Eval(SignatureScript);
            lblSignaturePad.Text =
                $"<canvas id=\"{SignaturePadId}\" width=\"262\" height=\"58\" style=\"display:block;border:1px dashed #cfd6de;border-radius:7px;background:#fff;touch-action:none;cursor:crosshair\" " +
                "onpointerdown=\"fieldOpsSig.down(event,this)\" onpointermove=\"fieldOpsSig.move(event,this)\" onpointerup=\"fieldOpsSig.up(event,this)\" onpointerleave=\"fieldOpsSig.up(event,this)\"></canvas>";

            Reload();
            ReminderService.HandleResponses(n => { if (n != _number) new WorkOrderDetail(n).Show(); });
            var reminder = ReminderService.ScheduleFor(_workOrder);
            if (reminder != null) SetStatus(reminder, Ops.Good);
        }

        private void Reload()
        {
            try
            {
                _workOrder = _service.Get(_number);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"opening {_number}", ex);
                ShowBanner("This work order could not be opened. Go back to the list and try again.", "bad");
                return;
            }

            lblAppTitle.Text = $"FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:13px\">· {_workOrder.Number}</span>";
            lblJob.Text =
                $"<div style=\"padding-top:12px\"><div style=\"font-size:14.5px;font-weight:800;color:#0d1b2a;white-space:nowrap\">{Enc(_workOrder.Fault)}</div>" +
                $"<div style=\"font-size:12.5px;color:#5a6b7d;margin-top:3px;white-space:nowrap\">{Enc(_workOrder.Customer)} · {Enc(_workOrder.SiteAddress)}</div></div>";
            var left = _workOrder.DueAt - DateTime.Now;
            lblSla.Text = left > TimeSpan.Zero
                ? $"<span style=\"font-size:11px;font-weight:800;color:#b3741a;background:#fdf5e6;border:1px solid #eed6a6;border-radius:999px;padding:3px 11px;white-space:nowrap\">SLA {(int)left.TotalHours}h {left.Minutes:00}m</span>"
                : "<span style=\"font-size:11px;font-weight:800;color:#c0392b;background:#fdecea;border:1px solid #f0b4ad;border-radius:999px;padding:3px 11px\">SLA passed</span>";

            rbAccepted.Checked = _workOrder.Status == WorkOrderStatus.Assigned;
            rbOnSite.Checked = _workOrder.Status == WorkOrderStatus.Accepted;
            rbCompleted.Checked = _workOrder.Status >= WorkOrderStatus.OnSite;
            RenderPhotos();
        }

        private void RenderPhotos()
        {
            var photos = _photoStore.List(_number);
            var slots = Enumerable.Range(0, 3).Select(i =>
            {
                if (i < photos.Count)
                {
                    var p = photos[photos.Count - 1 - i];
                    return $"<div style=\"width:74px;height:60px;border-radius:7px;border:1px solid #34506c;overflow:hidden;position:relative\">" +
                           $"<img src=\"{_photoStore.ThumbnailDataUri(_number, p.Name)}\" style=\"width:74px;height:60px;display:block\"/>" +
                           $"<span style=\"position:absolute;left:0;right:0;bottom:0;background:rgba(15,30,50,.6);color:#dbe7f3;font-size:10px;font-weight:700;text-align:center\">{p.Bytes / 1024} KB</span></div>";
                }
                return "<div style=\"width:74px;height:60px;border-radius:7px;background:#f4f7fa;border:1px dashed #cfd6de;display:flex;align-items:center;justify-content:center;color:#a9b4c0;font-size:10.5px;font-weight:700\">empty</div>";
            });
            lblPhotoStrip.Text = "<div style=\"display:flex;gap:9px;margin-top:6px\">" + string.Concat(slots) + "</div>";
            lblCount.Text = $"{photos.Count} photo{(photos.Count == 1 ? "" : "s")} · {(_signed ? "signed" : "unsigned")}";
        }

        #region Take Photo: Device.Media in the client, Upload in the browser

        private async void btnPhoto_Click(object sender, EventArgs e)
        {
            if (!Device.Valid || !Device.Media.CaptureSupported) { uploadPhoto.Visible = true; return; }

            var permission = await _permissions.AskAsync(PermissionType.Camera, CameraReason);
            if (!ReportRefusal(permission, "camera", "Use Upload Photo to attach a picture instead."))
            {
                uploadPhoto.Visible = true;     // the manual path stays available
                Application.Update(this);
                return;
            }

            ShowBanner("Camera open on the device. btnPhoto_Click is awaiting CapturePhotoAsync on the server.", "info");
            SetStatus("Camera open — the handler is awaiting the photo", Ops.Warn);
            Application.Update(this);
            try
            {
                using var photo = await Device.Media.CapturePhotoAsync(1600, 1600, 0.7f, 0);
                if (photo == null)
                {
                    HideBanner();
                    SetStatus("Capture cancelled — no photo was added", Ops.Muted);
                    OpsLog.Info($"photo: {_number} capture cancelled");
                }
                else
                {
                    var stored = _photoStore.Save(_workOrder.Number, photo);
                    HideBanner();
                    RenderPhotos();
                    SetStatus($"1 photo stored · {stored.Bytes / 1024} KB · thumbnail pushed", Ops.Good);
                    OpsLog.Info($"photo: {_number} captured, {stored.Bytes} bytes stored outside the web root");
                }
            }
            catch (Exception ex)
            {
                OpsLog.Error($"photo capture {_number}", ex);
                ShowBanner("The camera did not return a photo. Try again, or use Upload Photo.", "bad");
                uploadPhoto.Visible = true;
            }
            Application.Update(this);
        }

        private void uploadPhoto_Uploaded(object sender, UploadedEventArgs e)
        {
            try
            {
                long original = 0, stored = 0;
                for (int i = 0; i < e.Files.Count; i++)
                {
                    var file = e.Files[i];
                    if (file.ContentType == null || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowBanner("That file is not an image, so it was not attached.", "warn");
                        continue;
                    }
                    original += file.ContentLength;
                    stored += _photoStore.SaveDownscaled(_number, file.InputStream).Bytes;
                }
                RenderPhotos();
                if (stored > 0)
                {
                    SetStatus($"Photo uploaded · {original / 1024} KB → {stored / 1024} KB on the server", Ops.Good);
                    OpsLog.Info($"photo: {_number} uploaded through the browser fallback ({original} → {stored} bytes)");
                }
            }
            catch (Exception ex)
            {
                OpsLog.Error($"photo upload {_number}", ex);
                ShowBanner("The photo could not be stored. Your work order is unchanged.", "bad");
            }
        }

        #endregion

        #region Signature: a canvas in the page, a PNG on the server

        private void btnSignature_Click(object sender, EventArgs e)
        {
            Application.Eval($"fieldOpsSig.png('{SignaturePadId}')", result =>
            {
                var dataUrl = result as string;
                if (string.IsNullOrEmpty(dataUrl))
                {
                    ShowBanner("Sign in the box first — the signature is empty.", "warn");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSigner.Text))
                {
                    ShowBanner("Type the signer's name under the signature.", "warn");
                    return;
                }
                try
                {
                    var png = Convert.FromBase64String(dataUrl.Substring(dataUrl.IndexOf(',') + 1));
                    _signatures.Save(_number, png, txtSigner.Text);
                    _signed = true;
                    HideBanner();
                    RenderPhotos();
                    SetStatus($"Signed by {txtSigner.Text.Trim()} · {png.Length / 1024 + 1} KB PNG · {DateTime.Now:HH:mm} server time", Ops.Good);
                    OpsLog.Info($"signature: {_number} signed by {txtSigner.Text.Trim()} ({png.Length} bytes)");
                }
                catch (Exception ex)
                {
                    OpsLog.Error($"signature {_number}", ex);
                    ShowBanner("The signature could not be saved. Please sign again.", "bad");
                }
            });
        }

        private const string SignatureScript =
            "window.fieldOpsSig=window.fieldOpsSig||{" +
            "down:function(e,c){c.setPointerCapture&&c.setPointerCapture(e.pointerId);var x=c.getContext('2d'),r=c.getBoundingClientRect();x.lineWidth=2.6;x.lineCap='round';x.strokeStyle='#1f2d3a';x.beginPath();x.moveTo(e.clientX-r.left,e.clientY-r.top);c._d=true;c.dataset.inked='1';}," +
            "move:function(e,c){if(!c._d)return;var x=c.getContext('2d'),r=c.getBoundingClientRect();x.lineTo(e.clientX-r.left,e.clientY-r.top);x.stroke();}," +
            "up:function(e,c){c._d=false;}," +
            "png:function(id){var c=document.getElementById(id);return c&&c.dataset.inked==='1'?c.toDataURL('image/png'):'';}};";

        #endregion

        #region Navigate: location + the maps app in the client, a maps link in the browser

        private async void btnNavigate_Click(object sender, EventArgs e)
        {
            var site = _workOrder;
            if (!DeviceCapabilities.Current.NativeNavigate)
            {
                ShowBanner($"Confirm the site address — {site.SiteAddress} — FieldOps opens the route in your maps site.", "info");
                Application.Navigate("https://www.google.com/maps/dir/?api=1&destination=" + Uri.EscapeDataString(site.Customer + ", " + site.SiteAddress), "_blank");
                SetStatus("Route opened in a new tab from the site address", Ops.Good);
                return;
            }

            var permission = await _permissions.AskAsync(PermissionType.LocationWhenInUse, LocationReason);
            if (!ReportRefusal(permission, "location", "The route still opens from the site address."))
            {
                await OpenMapAsync(site, null);
                Application.Update(this);
                return;
            }

            Wisej.Hybrid.Location here = null;
            try
            {
                here = await Device.Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10)));
            }
            catch (Exception ex)
            {
                OpsLog.Error($"location {_number}", ex);
            }

            if (here == null)
            {
                ShowBanner("No location fix (indoors, or the GPS is off). The route opens from the site address.", "warn");
            }
            else
            {
                // Server time, not the device's Timestamp: a phone clock is not evidence of arrival.
                LocationLog.Record(_number, Session.Technician.Id, here.Latitude, here.Longitude, here.Accuracy);
                var km = Wisej.Hybrid.Location.CalculateDistance(here, new Wisej.Hybrid.Location(site.SiteLatitude, site.SiteLongitude), DistanceUnits.Kilometers);
                SetStatus($"Arrival stamped {DateTime.Now:HH:mm} · ±{here.Accuracy ?? 0:F0} m · {km:F1} km to the site", Ops.Good);
            }
            await OpenMapAsync(site, here);
            Application.Update(this);
        }

        private async System.Threading.Tasks.Task OpenMapAsync(WorkOrder site, Wisej.Hybrid.Location here)
        {
            try
            {
                bool opened = await Device.Map.TryOpenAsync(new Wisej.Hybrid.Location(site.SiteLatitude, site.SiteLongitude),
                    new MapLaunchOptions { Name = site.Customer, NavigationMode = NavigationMode.Driving });
                OpsLog.Info($"map: Device.Map.TryOpenAsync {site.Number} -> {opened}");
                if (!opened) ShowBanner("No maps app could open the route on this device.", "warn");
                else if (here != null) HideBanner();
            }
            catch (Exception ex)
            {
                OpsLog.Error($"map {site.Number}", ex);
                ShowBanner("The maps app could not be opened.", "bad");
            }
        }

        #endregion

        #region Permission outcomes as states of the screen

        /// <summary>True when granted. Otherwise shows the right sentence and, for good, the Settings button.</summary>
        private bool ReportRefusal(PermissionGate.Outcome outcome, string feature, string fallback)
        {
            btnSettings.Visible = false;
            switch (outcome)
            {
                case PermissionGate.Outcome.Granted:
                    return true;
                case PermissionGate.Outcome.Denied:
                    ShowBanner($"FieldOps may not use the {feature} this time. {fallback}", "warn");
                    SetStatus($"{Cap(feature)} not allowed — ask again next time", Ops.Warn);
                    return false;
                default:
                    ShowBanner($"The {feature} is switched off for FieldOps in the phone's settings. {fallback} To allow it, open Settings.", "bad");
                    SetStatus($"{Cap(feature)} refused for good — Settings needed", Ops.Bad);
                    btnSettings.Visible = true;     // never re-prompt in a loop
                    return false;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e) => PermissionGate.OpenSettings();

        #endregion

        #region Module 1: the status flow

        private WorkOrderStatus ChosenStatus =>
            rbCompleted.Checked ? WorkOrderStatus.Completed :
            rbOnSite.Checked ? WorkOrderStatus.OnSite : WorkOrderStatus.Accepted;

        private void btnSaveStatus_Click(object sender, EventArgs e)
        {
            var next = ChosenStatus;
            try
            {
                var before = _workOrder;
                var saved = _service.UpdateStatus(_workOrder.Number, next, _workOrder.Version, Session.Technician.Id);
                if (AppMode.IsLocal)
                {
                    // Local app (Module 3): saved on this device only, recorded for the next Remote sign-in.
                    DeviceSync.RecordPending(before, next);
                    Reload();
                    ShowBanner($"Saved on this device: {saved.Number} is now {WorkOrder.StatusText(saved.Status)}. It reaches the FieldOps server when you are back online.", "warn");
                    return;
                }
                Reload();
                SetStatus($"Saved: {saved.Number} is now {WorkOrder.StatusText(saved.Status)}", Ops.Good);
            }
            catch (StatusTransitionException ex)
            {
                ShowBanner(ex.Message, "warn");
            }
            catch (WorkOrderConflictException ex)
            {
                OpsLog.Error($"status update {_workOrder.Number} -> {next}", ex);
                Reload();
                ShowBanner($"{_workOrder.Number} was changed somewhere else. It is now {WorkOrder.StatusText(_workOrder.Status)}.", "bad");
            }
            catch (Exception ex)
            {
                OpsLog.Error($"status update {_workOrder.Number} -> {next}", ex);
                ShowBanner("The status could not be saved. Nothing was changed.", "bad");
            }
        }

        #endregion

        private void btnBack_Click(object sender, EventArgs e)
        {
            var list = Session.ListPage;
            if (list == null || list.IsDisposed)
                Session.ListPage = list = new WorkOrdersPage();
            list.Show();
            list.LoadOrders(_number);
            Dispose();
        }

        private void SetStatus(string text, Color tone)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = tone;
        }

        private void ShowBanner(string text, string tone)
        {
            var (bg, bd, bar, ink) = tone switch
            {
                "bad" => ("#fdecea", "#f0b4ad", "#c0392b", "#7d2018"),
                "warn" => ("#fdf5e6", "#eed6a6", "#c98a12", "#6d4a06"),
                _ => ("#eaf3ff", "#bcd8ff", "#1565d8", "#123f7d"),
            };
            lblBanner.Text =
                $"<div style=\"display:flex;align-items:center;gap:12px;padding:10px 15px;background:{bg};border:1px solid {bd};border-left:4px solid {bar};border-radius:9px\">" +
                $"<span style=\"flex:none;width:21px;height:21px;border-radius:999px;background:{bar};color:#fff;display:flex;align-items:center;justify-content:center;font-size:12.5px;font-weight:800\">!</span>" +
                $"<span style=\"font-size:12.5px;color:{ink};line-height:1.4\">{Enc(text)}</span></div>";
            lblBanner.Visible = true;
        }

        private void HideBanner() => lblBanner.Visible = false;

        private static string Cap(string s) => char.ToUpperInvariant(s[0]) + s.Substring(1);

        private static string Enc(string s) => WebUtility.HtmlEncode(s ?? "");
    }
}
