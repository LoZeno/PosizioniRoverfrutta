using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PosizioniRoverfrutta.Services
{
    public class ResourceHelpers
    {
        public static string LoadBase64Logo()
        {
            var imageBytes = LoadResourceImageAsByteArray();
            var base64Image = Convert.ToBase64String(imageBytes);
            return base64Image;
        }

        private static byte[] LoadResourceImageAsByteArray()
        {
            var resourceUri = new Uri("pack://application:,,,/PosizioniRoverfrutta;component/Content/Pictures/Logo/Shield.png", UriKind.Absolute);
            var image = new BitmapImage(resourceUri);
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
    }
}