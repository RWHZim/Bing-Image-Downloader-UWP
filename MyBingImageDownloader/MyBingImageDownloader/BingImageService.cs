using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyBingImageDownloader
{
    public static class BingImageService
    {
        private static async Task<dynamic> DownloadJson(int numberOfImages, string location)
        {
            using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                string jsonString = 
                    await httpClient.GetStringAsync($"https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n={numberOfImages}&mkt={location}");
                return JsonConvert.DeserializeObject<dynamic>(jsonString);
            }
        }

        public static async Task<List<BingImage>> GetBingImages(int numberOfImages, string location)
        {
            List<BingImage> bingImages = new List<BingImage>();
            var imageData = await DownloadJson(numberOfImages, location);
            // Will throw an exception if i > 8 
            // (only the last 8 Bing images are available)
            for (int i = 0; i < numberOfImages; i++)
            {
                try
                {
                    // Get data attached to the image
                    string copyrightText = imageData.images[i].copyright;
                    string startDate = imageData.images[i].startdate;
                    Uri copyrightLink = imageData.images[i].copyrightlink;
                    string url = imageData.images[i].url;
                    string imageUrl = $"https://www.bing.com{url}";
                    var uri = new Uri(imageUrl);
                    var image = new BitmapImage(uri);
                    var bingImage = new BingImage()
                    {
                        CopyrightText = copyrightText,
                        CopyrightLink = copyrightLink,
                        Image = image,
                        StartDate = startDate
                    };
                    bingImages.Add(bingImage);
                }
                catch (Exception)
                {
                    return bingImages;
                }
            }
            return bingImages;
        }
    }
}
