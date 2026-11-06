using System;
using System.IO;
using System.Linq;

namespace FieldOps.Services
{
    /// <summary>
    /// Keeps work-order photos on the server's disk, outside the web root (the web root of this
    /// project is the project folder itself, so anything under it could be served as a URL).
    /// Only the order number and a random file name identify a photo; the bytes never sit in
    /// session state.
    /// </summary>
    public sealed class PhotoStore
    {
        private readonly string _root;

        public PhotoStore(string root)
        {
            _root = root;
        }

        public static PhotoStore Current { get; } = new PhotoStore(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FieldOps", "photos"));

        public string Save(string orderNumber, Stream content, string extension)
        {
            var folder = Path.Combine(_root, orderNumber);
            Directory.CreateDirectory(folder);
            var name = Guid.NewGuid().ToString("N") + extension;
            using (var file = File.Create(Path.Combine(folder, name)))
                content.CopyTo(file);
            return name;
        }

        public int Count(string orderNumber)
        {
            var folder = Path.Combine(_root, orderNumber);
            return Directory.Exists(folder) ? Directory.EnumerateFiles(folder).Count() : 0;
        }
    }
}
