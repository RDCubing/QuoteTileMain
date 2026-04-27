using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


namespace QuoteTile
{
    public static class AppBackgroundHelper
    {
        private const string DefaultBackgroundPath = "ms-appx:///Images/tnm0ZI.jpg";

        public static ImageBrush GetCurrentBackground()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string path = settings.Values["AppBackground"] as string;

            if (string.IsNullOrEmpty(path))
            {
                // First install: save default background
                settings.Values["AppBackground"] = DefaultBackgroundPath;
                path = DefaultBackgroundPath;
            }

            return new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(path)),
                Stretch = Stretch.UniformToFill
            };
        }

        public static void SetBackground(string path)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["AppBackground"] = path;
        }
    }
}
