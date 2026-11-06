using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// The two ways a photo reaches a work order, behind one gate. Both pages call these, so the
    /// list's action strip and the detail page cannot drift apart.
    /// </summary>
    internal static class PhotoActions
    {
        /// <summary>
        /// Hybrid client: the platform camera, resized and compressed on the device. Returns a
        /// sentence for the screen. A null photo means the technician backed out - an outcome,
        /// not an error.
        /// </summary>
        public static async Task<string> CaptureAsync(string orderNumber)
        {
            using var photo = await Device.Media.CapturePhotoAsync(1600, 1600, 0.7f, 0);
            if (photo == null)
                return "Capture cancelled - no photo was added.";

            using var jpeg = new MemoryStream();
            photo.Save(jpeg, ImageFormat.Jpeg);
            jpeg.Position = 0;
            PhotoStore.Current.Save(orderNumber, jpeg, ".jpg");
            return $"Photo added to {orderNumber} ({PhotoStore.Current.Count(orderNumber)} attached).";
        }

        /// <summary>Browser and PWA: the file the Upload control posted.</summary>
        public static string SaveUploaded(string orderNumber, UploadedEventArgs e)
        {
            int saved = 0;
            for (int i = 0; i < e.Files.Count; i++)
            {
                var file = e.Files[i];
                if (file.ContentType == null || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    continue;
                PhotoStore.Current.Save(orderNumber, file.InputStream, Path.GetExtension(file.FileName));
                saved++;
            }
            return saved == 0
                ? "That file is not an image, so it was not attached."
                : $"Photo added to {orderNumber} ({PhotoStore.Current.Count(orderNumber)} attached).";
        }
    }
}
