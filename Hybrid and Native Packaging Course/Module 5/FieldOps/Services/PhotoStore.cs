using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace FieldOps.Services
{
    /// <summary>
    /// Keeps work-order photos on the server's disk, OUTSIDE the web root (this project's web root
    /// is the project folder, so nothing under it is safe), under a random name. No URL maps to
    /// them: the page receives small thumbnails rendered by the server for the work order the
    /// session has open. Session state holds identifiers, never pictures.
    /// </summary>
    public sealed class PhotoStore
    {
        public const int MaxSide = 1600;
        private readonly string _root;

        public PhotoStore(string root)
        {
            _root = root;
        }

        public static PhotoStore Current { get; } = new PhotoStore(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FieldOps", "photos"));

        public sealed record StoredPhoto(string Name, long Bytes);

        /// <summary>Streams the bytes to disk; returns the stored size.</summary>
        public StoredPhoto Save(string orderNumber, Stream content, string extension)
        {
            var folder = Path.Combine(_root, orderNumber);
            Directory.CreateDirectory(folder);
            var name = Guid.NewGuid().ToString("N") + extension;
            var path = Path.Combine(folder, name);
            using (var file = File.Create(path))
                content.CopyTo(file);
            return new StoredPhoto(name, new FileInfo(path).Length);
        }

        /// <summary>The Hybrid client's photo: already resized and compressed on the device.</summary>
        public StoredPhoto Save(string orderNumber, Image photo)
        {
            using var jpeg = new MemoryStream();
            photo.Save(jpeg, ImageFormat.Jpeg);
            jpeg.Position = 0;
            return Save(orderNumber, jpeg, ".jpg");
        }

        /// <summary>
        /// The browser's Upload posts the ORIGINAL file (often 4-8 MB). Downscale it once, on
        /// arrival, to the same 1600 px the client asks for, and store that.
        /// </summary>
        public StoredPhoto SaveDownscaled(string orderNumber, Stream original)
        {
            using var image = Image.FromStream(original);
            double scale = Math.Min(1.0, (double)MaxSide / Math.Max(image.Width, image.Height));
            if (scale >= 1.0) return Save(orderNumber, image);
            using var small = new Bitmap(image, new Size((int)(image.Width * scale), (int)(image.Height * scale)));
            return Save(orderNumber, small);
        }

        public IReadOnlyList<StoredPhoto> List(string orderNumber)
        {
            var folder = Path.Combine(_root, orderNumber);
            if (!Directory.Exists(folder)) return Array.Empty<StoredPhoto>();
            return new DirectoryInfo(folder).GetFiles()
                .OrderBy(f => f.CreationTimeUtc)
                .Select(f => new StoredPhoto(f.Name, f.Length))
                .ToList();
        }

        public int Count(string orderNumber) => List(orderNumber).Count;

        /// <summary>A small JPEG thumbnail as a data URI - rendered on demand, never a public URL.</summary>
        public string ThumbnailDataUri(string orderNumber, string name, int width = 74, int height = 60)
        {
            var path = Path.Combine(_root, orderNumber, Path.GetFileName(name));
            using var image = Image.FromFile(path);
            using var thumb = new Bitmap(image, new Size(width, height));
            using var ms = new MemoryStream();
            thumb.Save(ms, ImageFormat.Jpeg);
            return "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
        }
    }
}
