using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// Four commands, four mechanisms:
    /// 1. <c>btnOpen</c> gets an <see cref="System.Drawing.Image"/> the server decoded, which
    ///    Wisej.NET sends to the browser as PNG data,
    /// 2. <c>btnSave</c> gets a named theme image as an <c>ImageSource</c> string the client resolves,
    /// 3. <c>btnPrint</c> and <c>btnDelete</c> share one <see cref="ImageList"/> through <c>ImageKey</c>.
    /// Clicking a button adds its line to <c>lblDiagnostics</c>, which reads the control back
    /// through <see cref="IImage"/> and names the property that is actually populated. Clicking
    /// Delete again replaces the picture stored under its key, and once more sets a second image
    /// property on it so the key stops deciding.
    /// </summary>
    public partial class CommandPage : Page
    {
        /// <summary>The theme defines this one, so the client takes it from a cache it already holds.</summary>
        private const string SaveThemeImage = "icon-save";

        /// <summary>
        /// The image source the lab assigns on top of btnDelete's ImageKey. The lesson writes
        /// "icon-delete"; Bootstrap-4 has no image of that name, so this sample names a theme
        /// image that really exists - what matters is that a second image property takes the slot.
        /// </summary>
        private const string DeleteThemeImage = "icon-close";

        private static readonly Color Muted = Color.FromArgb(138, 152, 168);
        private static readonly Color Good = Color.FromArgb(23, 128, 79);
        private static readonly Color Bad = Color.FromArgb(180, 47, 47);
        private static readonly Color IdleBack = Color.FromArgb(244, 247, 250);
        private static readonly Color GoodBack = Color.FromArgb(230, 246, 238);
        private static readonly Color BadBack = Color.FromArgb(253, 236, 236);

        /// <summary>The buttons whose line is currently printed, in the order they were clicked.</summary>
        private readonly List<Control> reported = new List<Control>();

        /// <summary>Controls that had an ImageKey until a second image property took the slot.</summary>
        private readonly HashSet<Control> displacedKeys = new HashSet<Control>();

        /// <summary>0 = Delete not reported yet, 1 = reported, 2 = key repointed, 3 = slot taken.</summary>
        private int deleteStage;

        public CommandPage()
        {
            InitializeComponent();

            LoadCommandIcons();
            WriteDiagnostics();
        }

        // ── the four mechanisms ─────────────────────────────────────────────────

        private void LoadCommandIcons()
        {
            // 1. An image object. The file is decoded into server memory here and now, and the
            //    bytes the browser receives are produced by Wisej.NET, not by the file.
            this.btnOpen.Image = LoadServerImage("open.png");

            // 2. A string. Nothing is decoded on the server: the client resolves "icon-save"
            //    against the theme it has already downloaded.
            this.btnSave.ImageSource = SaveThemeImage;

            // 3. One collection, addressed by key. ImageKey and not ImageIndex, because "delete"
            //    still means delete after somebody inserts an image above it.
            this.imagesCommands.Images.Add("print", LoadServerImage("print.png"));
            this.imagesCommands.Images.Add("delete", LoadServerImage("delete.png"));

            this.btnPrint.ImageList = this.imagesCommands;
            this.btnPrint.ImageKey = "print";

            this.btnDelete.ImageList = this.imagesCommands;
            this.btnDelete.ImageKey = "delete";
        }

        /// <summary>
        /// Reads a file from the Images folder beside the application into a
        /// <see cref="System.Drawing.Image"/>. Application.MapPath resolves it whether the
        /// application runs from bin/ or from a published folder.
        /// </summary>
        private static Image LoadServerImage(string fileName)
        {
            var path = Application.MapPath("Images/" + fileName);

            // Image.FromFile keeps the file locked for the lifetime of the Image, which is the
            // last thing a server application wants. Read the bytes and let the file go.
            var bytes = File.ReadAllBytes(path);
            return Image.FromStream(new MemoryStream(bytes));
        }

        // ── one click, one diagnostics line ─────────────────────────────────────

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Report(this.btnOpen);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Report(this.btnSave);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Report(this.btnPrint);
        }

        /// <summary>
        /// Delete carries the rest of the lab. The first click reports it like the others; the
        /// second replaces the picture stored under its key; the third sets a second image
        /// property on the same control and lets the diagnostics line report which one won.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            switch (this.deleteStage)
            {
                case 0:
                    Report(this.btnDelete);
                    this.deleteStage = 1;
                    break;

                case 1:
                    ReplaceDeleteEntry();
                    this.deleteStage = 2;
                    break;

                default:
                    TakeTheSlotFromTheKey();
                    this.deleteStage = 3;
                    break;
            }
        }

        private void Report(Control control)
        {
            if (!this.reported.Contains(control))
                this.reported.Add(control);

            WriteDiagnostics();

            if (this.reported.Count == 4)
                SetStatus("Diagnostics complete — every icon accounted for.", Good, GoodBack);
            else
                SetStatus(control.Name + " read through IImage.", Muted, IdleBack);
        }

        /// <summary>
        /// Replaces the picture stored under one key. No image property on any control is
        /// assigned: the button points at the key, not at the picture, so it follows.
        /// </summary>
        private void ReplaceDeleteEntry()
        {
            // The collection indexer hands back the entry, not a copy, so assigning its Image is
            // how a key is repointed at a different picture.
            var entry = this.imagesCommands.Images["delete"];
            var previous = entry.Image;
            entry.Image = LoadServerImage("delete-alt.png");
            previous?.Dispose();

            RepaintControlsOn(this.imagesCommands, "delete");

            WriteDiagnostics();
            SetStatus("One entry replaced — every control on that key followed.", Good, GoodBack);
        }

        /// <summary>
        /// The second half of the lab note. An image source assigned somewhere else - a theme
        /// helper, a style pass, a later refactor - takes the one visual slot the control has.
        /// </summary>
        /// <remarks>
        /// Read back afterwards, <c>ImageKey</c> is not merely ignored: Wisej.NET 4.1.4 clears it.
        /// The properties are mutually exclusive, so the later assignment does not win a fight,
        /// it ends one - and the key that used to decide is gone from the control.
        /// </remarks>
        private void TakeTheSlotFromTheKey()
        {
            var keyBefore = this.btnDelete.ImageKey;

            this.btnDelete.ImageSource = DeleteThemeImage;

            if (!string.IsNullOrEmpty(keyBefore) && string.IsNullOrEmpty(this.btnDelete.ImageKey))
                this.displacedKeys.Add(this.btnDelete);

            WriteDiagnostics();
            SetStatus("btnDelete now answers ImageSource. Assigning it cleared the key.", Bad, BadBack);
        }

        /// <summary>
        /// Finds every image-capable control on the page that draws <paramref name="key"/> from
        /// <paramref name="list"/> and repaints it.
        /// </summary>
        /// <remarks>
        /// Wisej.NET serves a control's picture from a URL stamped with that control's own
        /// version, and the version only moves when one of the control's image properties is
        /// written. A picture swapped inside the list therefore reaches the server but not a
        /// browser still holding the old URL, so each control is told to resolve its key again -
        /// the same key, written back unchanged. That is a repaint, not a different picture, and
        /// the search above is the demonstration: this code does not know which controls use the
        /// key, it asks.
        /// </remarks>
        private void RepaintControlsOn(ImageList list, string key)
        {
            foreach (var control in AllControls(this))
            {
                if (control is IImage image && image.ImageList == list && image.ImageKey == key)
                {
                    image.ImageKey = null;
                    image.ImageKey = key;
                }
            }
        }

        private static IEnumerable<Control> AllControls(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;

                foreach (var grandChild in AllControls(child))
                    yield return grandChild;
            }
        }

        // ── reading the mechanism back through IImage ───────────────────────────

        private void WriteDiagnostics()
        {
            var html = new StringBuilder();

            foreach (var control in this.reported)
                html.Append(DescribeIcon(control));

            this.lblDiagnostics.Text = html.ToString();
        }

        /// <summary>
        /// The cast is the point: a Button, a Label and anything else image-capable expose the same
        /// six properties through <see cref="IImage"/>, so one method can report on all of them.
        /// </summary>
        /// <remarks>
        /// The order of the tests matters. ImageSource is asked about first because assigning it
        /// takes the slot from everything else, and ImageList/ImageKey before Image because a
        /// control fed from a list also returns that entry's picture from Image - testing Image
        /// first would report every keyed control as if it owned an image object.
        /// </remarks>
        private string DescribeIcon(Control control)
        {
            if (!(control is IImage image))
                return Line(control.Name, "no image model", "the control does not implement IImage", "#8a98a8");

            if (!string.IsNullOrEmpty(image.ImageSource))
            {
                // A control this page watched lose its key gets the interesting line, because
                // "theme image name ..." would not explain why the keyed picture went away.
                return this.displacedKeys.Contains(control)
                    ? Line(control.Name, "ImageSource", "the key no longer decides", "#b42f2f")
                    : Line(control.Name, "ImageSource", "theme image name \"" + image.ImageSource + "\"", "#1565d8");
            }

            if (image.ImageList != null && !string.IsNullOrEmpty(image.ImageKey))
                return Line(control.Name, "ImageList + Key", "key \"" + image.ImageKey + "\"", "#17804f");

            if (image.Image != null)
                return Line(control.Name, "Image", "System.Drawing.Image → sent as PNG", "#b06a00");

            return Line(control.Name, "no icon", "no image property is populated", "#8a98a8");
        }

        /// <summary>One diagnostics row: the control name, the mechanism, and what it points at.</summary>
        private static string Line(string name, string mechanism, string detail, string color)
        {
            return "<div style='display:flex;align-items:center;gap:12px;height:28px;" +
                   "font-family:Consolas,\"Courier New\",monospace;font-size:13.5px;'>" +
                   "<span style='flex:none;width:92px;color:#5a6b7d;font-weight:700;'>" + name + "</span>" +
                   "<span style='flex:none;width:132px;color:" + color + ";font-weight:800;'>" + mechanism + "</span>" +
                   "<span style='color:#34465a;'>" + detail + "</span></div>";
        }

        private void SetStatus(string text, Color foreColor, Color backColor)
        {
            this.lblStatus.Text = text;
            this.lblStatus.ForeColor = foreColor;
            this.lblStatus.BackColor = backColor;
        }
    }
}
