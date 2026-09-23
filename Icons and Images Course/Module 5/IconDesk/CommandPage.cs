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
    /// 3. <c>btnPrint</c> and <c>btnDelete</c> share one <see cref="ImageList"/> through <c>ImageKey</c>,
    /// 4. <c>btnConflict</c> is left empty until the lab sets two image properties on it at once.
    /// The diagnostics label reads every one of them back through <see cref="IImage"/>.
    /// </summary>
    public partial class CommandPage : Page
    {
        /// <summary>The theme defines this one, so the client takes it from a cache it already holds.</summary>
        private const string SaveThemeImage = "icon-check";

        private bool deleteSwapped;

        public CommandPage()
        {
            InitializeComponent();

            LoadCommandIcons();
            ReportMechanisms();
        }

        // ── the four mechanisms ─────────────────────────────────────────────────

        private void LoadCommandIcons()
        {
            // 1. An image object. Image.FromFile decodes the file into server memory here and now,
            //    and the bytes the browser receives are produced by Wisej.NET, not by the file.
            this.btnOpen.Image = LoadServerImage("open.png");

            // 2. A string. Nothing is decoded on the server: the client resolves "icon-check"
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

            // A Label is an image-capable control too, and it is on the same key as the button.
            this.lblDeleteEcho.ImageList = this.imagesCommands;
            this.lblDeleteEcho.ImageKey = "delete";
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

        // ── reading the mechanism back through IImage ───────────────────────────

        private void btnInspect_Click(object sender, EventArgs e)
        {
            ReportMechanisms();
            this.lblStatus.Text = "Inspected every control through IImage - the report reads the properties, not the picture.";
        }

        private void ReportMechanisms()
        {
            var report = new StringBuilder();
            report.Append("<b>What is actually carrying each icon</b><br><br>");

            foreach (var control in new Control[] { this.btnOpen, this.btnSave, this.btnPrint, this.btnDelete, this.lblDeleteEcho, this.btnConflict })
                report.Append("<b>").Append(control.Name).Append("</b> &mdash; ").Append(Describe(control)).Append("<br>");

            this.lblDiagnostics.Text = report.ToString();
        }

        /// <summary>
        /// The cast is the point: a Button, a Label and anything else image-capable expose the same
        /// six properties through <see cref="IImage"/>, so one method can report on all of them.
        /// More than one answer means more than one property is populated, which is the usual cause
        /// of "the wrong icon".
        /// </summary>
        private static string Describe(Control control)
        {
            if (!(control is IImage image))
                return "not an image-capable control - it does not implement IImage";

            // Ask about the list first. A control that takes its picture from an ImageList also
            // returns that picture from Image, so testing Image first would report every keyed
            // control as if it owned an image object.
            if (image.ImageList != null && !string.IsNullOrEmpty(image.ImageKey))
                return $"<b>ImageList + ImageKey</b> - entry \"{image.ImageKey}\" of a shared collection " +
                       "(reading Image back returns that entry's picture, which is why this test comes first)";

            if (image.ImageList != null && image.ImageIndex >= 0)
                return $"<b>ImageList + ImageIndex</b> - entry {image.ImageIndex} of a shared collection, " +
                       "which the next inserted image will silently change";

            var found = new List<string>();

            if (image.Image != null)
                found.Add($"<b>Image</b> - a {image.Image.Width}x{image.Image.Height} System.Drawing.Image in server memory, sent to the browser as PNG");

            if (!string.IsNullOrEmpty(image.ImageSource))
                found.Add($"<b>ImageSource</b> - the string \"{image.ImageSource}\", resolved by the client");

            if (found.Count == 0)
                return "no image property is set";

            // In practice this never reports two: Wisej.NET clears Image when ImageSource is
            // assigned. The branch stays because it is the honest test - if a future version kept
            // both, this is the line that would tell you.
            return found.Count == 1
                ? found[0]
                : string.Join(", and ", found) + " &mdash; <b>two properties feed one slot</b>";
        }

        // ── navigation ──────────────────────────────────────────────────────────

        /// <summary>
        /// The command page instance is handed to the lab page and handed back, so one session
        /// keeps one set of controls and one ImageList rather than rebuilding them on every hop.
        /// </summary>
        private void btnImageLab_Click(object sender, EventArgs e)
        {
            Application.MainPage = new ImageLabPage(this);
        }

        private void btnGallery_Click(object sender, EventArgs e)
        {
            Application.MainPage = new IconGalleryPage(this);
        }

        private void btnPacks_Click(object sender, EventArgs e)
        {
            Application.MainPage = new IconComparePage(this);
        }

        private void btnResources_Click(object sender, EventArgs e)
        {
            Application.MainPage = new ResourceLabPage(this);
        }

        // ── what the lab note is about ──────────────────────────────────────────

        /// <summary>
        /// Replaces the picture stored under one key. No control is touched: both the button and the
        /// label point at the key, not at the image, so both follow.
        /// </summary>
        private void btnSwapDelete_Click(object sender, EventArgs e)
        {
            this.deleteSwapped = !this.deleteSwapped;

            var replacement = LoadServerImage(this.deleteSwapped ? "delete-alt.png" : "delete.png");

            // The collection indexer hands back the entry, not a copy, so assigning its Image is
            // how a key is repointed at a different picture.
            var entry = this.imagesCommands.Images["delete"];
            var previous = entry.Image;
            entry.Image = replacement;
            previous?.Dispose();

            this.lblStatus.Text = this.deleteSwapped
                ? "Key \"delete\" now holds delete-alt.png. btnDelete and lblDeleteEcho both changed, and neither was assigned to."
                : "Key \"delete\" is back to delete.png. Again, both controls followed the key.";
        }

        /// <summary>
        /// Sets an image object and then an image source on the same button. They feed one visual
        /// slot, and Wisej.NET does not keep both: assigning <c>ImageSource</c> <b>clears</b>
        /// <c>Image</c>. Verified here - read the button back afterwards and only ImageSource is
        /// populated, which is why "the icon disappeared" is nearly always a second assignment
        /// somewhere else in the code rather than a broken file.
        /// </summary>
        private void btnSetBoth_Click(object sender, EventArgs e)
        {
            this.btnConflict.Image = LoadServerImage("print.png");
            var imageWasSet = this.btnConflict.Image != null;

            this.btnConflict.ImageSource = "icon-search";
            var imageSurvived = this.btnConflict.Image != null;

            ReportMechanisms();

            this.lblStatus.Text =
                $"btnConflict: Image assigned ({imageWasSet}), then ImageSource assigned - Image still set afterwards: {imageSurvived}. " +
                "One slot, so the later assignment does not win a fight, it ends one.";
        }
    }
}
