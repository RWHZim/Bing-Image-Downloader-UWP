using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace MyBingImageDownloader
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            // various countries to get Bing images from. You can change these to suit your needs
            string[] countries = { "en-US", "en-CN", "en-JP", "en-AU", "en-UK", "en-DE", "en-NZ", "en-CA" };
            CountryCombobox.ItemsSource = countries;
            CountryCombobox.SelectedItem = "en-US";
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (bingImageGridView.SelectionMode == ListViewSelectionMode.Single)
                bingImageGridView.SelectionMode = ListViewSelectionMode.Multiple;
            else
                bingImageGridView.SelectionMode = ListViewSelectionMode.Single;
        }

        private async void GetBingImages()
        {
            var bingImages = await BingImageService.GetBingImages(8, CountryCombobox.SelectedItem.ToString());
            bingImageGridView.ItemsSource = bingImages;
        }

        private void CountryCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetBingImages();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // If there are any selected pictures, save them
            if (bingImageGridView.SelectedItems.Count > 0)
            {
                // Get the folder to save images to
                var myPictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                foreach (var bingImage in bingImageGridView.SelectedItems)
                {
                    try
                    {
                        // Get the current BingImage
                        var img = ((BingImage)bingImage);
                        // Get the stream from the image
                        IRandomAccessStream stream = await RandomAccessStreamReference.CreateFromUri((Uri)img.Image.UriSource).OpenReadAsync();
                        // Create the BitmapDecoder
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                        // Create the SoftwareBitmap
                        SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                        // Create the StorageFile to save the image in
                        StorageFile file_Save = await myPictures.SaveFolder.CreateFileAsync($"{img.StartDate}{CountryCombobox.SelectedValue}.jpg", CreationCollisionOption.ReplaceExisting);
                        // Create a BitmapEncoder to encode the file
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, await file_Save.OpenAsync(FileAccessMode.ReadWrite));
                        // Set the SoftwareBitmap to the one created earlier
                        encoder.SetSoftwareBitmap(softwareBitmap);
                        // Save changes
                        await encoder.FlushAsync();
                    }
                    catch (Exception)
                    {
                        // exception handling here
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GetBingImages();
        }
    }
}
