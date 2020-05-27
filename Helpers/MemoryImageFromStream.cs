using System;
using System.Windows.Media.Imaging;

namespace RetroGameHandler.Helpers
{
    public static class MemoryImageFromStream
    {
        public static BitmapImage LoadImageToMemory(System.IO.Stream stream)
        {
            if (stream.CanRead)
            {
                BitmapImage image = new BitmapImage();

                try
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = new System.IO.MemoryStream();
                    stream.CopyTo(image.StreamSource);
                    image.EndInit();
                    stream.Close();
                    stream.Dispose();
                    image.StreamSource.Close();
                    image.StreamSource.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return default;
                }
                return image;
            }

            return default;
        }
    }
}