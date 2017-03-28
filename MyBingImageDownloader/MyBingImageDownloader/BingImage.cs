using System;
using Windows.UI.Xaml.Media.Imaging;

namespace MyBingImageDownloader
{
    // A class to store the downloaded bing image and information about it
    public class BingImage
    {
        public string CopyrightText { get; set; }
        public Uri CopyrightLink { get; set; }
        public string StartDate { get; set; }
        public BitmapImage Image { get; set; }
    }
}
