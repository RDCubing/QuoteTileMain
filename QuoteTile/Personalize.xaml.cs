using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace QuoteTile
{
    public sealed partial class Personalize : SettingsFlyout
    {
        public Personalize()
        {
            this.InitializeComponent();

            var settings = ApplicationData.Current.LocalSettings;

            // Restore dim checkbox
            var value = settings.Values["DimBackground"];
            if (value != null && value is bool)
                DimCheckBox.IsChecked = (bool)value;
            else
                DimCheckBox.IsChecked = false;

            // Restore background selection (prevents blank ComboBox)
            string savedBackground = settings.Values["AppBackground"] as string;

            if (string.IsNullOrEmpty(savedBackground))
            {
                comboBox.SelectedIndex = 0; // first option on first install
            }
            else
            {
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    string path = "";

                    switch (item.Content.ToString())
                    {
                        case "Beach (default)": path = "ms-appx:///Images/tnm0ZI.jpg"; break;
                        case "Gradient 1": path = "ms-appx:///Images/milestone.png"; break;
                        case "Gradient 2": path = "ms-appx:///Images/remove.png"; break;
                        case "Gradient 3": path = "ms-appx:///Images/quotetilebg_-_copia.png"; break;
                        case "Dark Teal 1": path = "ms-appx:///Images/dark-teal-n40bzelmlt0mbavq.jpg"; break;
                        case "Dark Teal 2": path = "ms-appx:///Images/dark-teal-background-i1g1tp72e271b8ny.jpg"; break;
                        case "Dark Teal 3": path = "ms-appx:///Images/dark-teal.jpg"; break;
                        case "Dark Ocean": path = "ms-appx:///Images/dark-ocean-4896-x-3264-wallpaper-zw0nf7tvntz2mlob.jpg"; break;
                        case "Sunrise": path = "ms-appx:///Images/sunrise-desktop-g2e998omhun2lfym.jpg"; break;
                        case "Mountain 1": path = "ms-appx:///Images/wallpaperflare.com_wallpaper (2).jpg"; break;
                        case "Mountain 2": path = "ms-appx:///Images/Snow Mountains Desktop Background Wallpaper.jpg"; break;
                        case "Mountain 3": path = "ms-appx:///Images/snow-mountainer-night-sky-stars-scenery-31-4K.jpg"; break;
                        case "Dark Blue": path = "ms-appx:///Images/282.jpg"; break;
                        case "Metro Horizon": path = "ms-appx:///Images/81squares.png"; break;
                    }

                    if (path == savedBackground)
                    {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }

                if (comboBox.SelectedIndex == -1)
                    comboBox.SelectedIndex = 0;
            }
            // Restore Favorites toggle
            var favValue = settings.Values["FavoritesInTile"];

            if (favValue != null && favValue is bool)
                FavoritesToggle.IsOn = (bool)favValue;
            else
                FavoritesToggle.IsOn = true; // default ON

            // Restore Tile Icon toggle
            var iconValue = settings.Values["TileIconEnabled"];

            if (iconValue != null && iconValue is bool)
                TileIconToggle.IsOn = (bool)iconValue;
            else
                TileIconToggle.IsOn = true; // default ON (shows mini icon)
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Make sure something is selected
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string imagePath = "";

                switch (selectedItem.Content.ToString())
                {
                    case "Beach (default)":
                        imagePath = "ms-appx:///Images/tnm0ZI.jpg";
                        break;

                    case "Gradient 1":
                        imagePath = "ms-appx:///Images/milestone.png";
                        break;

                    case "Gradient 2":
                        imagePath = "ms-appx:///Images/remove.png";
                        break;

                    case "Gradient 3":
                        imagePath = "ms-appx:///Images/quotetilebg_-_copia.png";
                        break;

                    case "Dark Teal 1":
                        imagePath = "ms-appx:///Images/dark-teal-n40bzelmlt0mbavq.jpg";
                        break;

                    case "Dark Teal 2":
                        imagePath = "ms-appx:///Images/dark-teal-background-i1g1tp72e271b8ny.jpg";
                        break;

                    case "Dark Teal 3":
                        imagePath = "ms-appx:///Images/dark-teal.jpg";
                        break;

                    case "Dark Ocean":
                        imagePath = "ms-appx:///Images/dark-ocean-4896-x-3264-wallpaper-zw0nf7tvntz2mlob.jpg";
                        break;

                    case "Sunrise":
                        imagePath = "ms-appx:///Images/sunrise-desktop-g2e998omhun2lfym.jpg";
                        break;

                    case "Mountain 1":
                        imagePath = "ms-appx:///Images/wallpaperflare.com_wallpaper (2).jpg";
                        break;

                    case "Mountain 2":
                        imagePath = "ms-appx:///Images/Snow Mountains Desktop Background Wallpaper.jpg";
                        break;

                    case "Mountain 3":
                        imagePath = "ms-appx:///Images/snow-mountainer-night-sky-stars-scenery-31-4K.jpg";
                        break;

                    case "Dark Blue":
                        imagePath = "ms-appx:///Images/282.jpg";
                        break;

                    case "Metro Horizon":
                        imagePath = "ms-appx:///Images/81squares.png";
                        break;

                    default:
                        imagePath = ""; // fallback
                        break;
                }

                // Save the selected background
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                settings.Values["AppBackground"] = imagePath;

                // Access MainPage
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    MainPage mainPage = rootFrame.Content as MainPage;
                    if (mainPage != null)
                    {
                        mainPage.ChangeBackground(imagePath);
                    }
                }
            }
        }
        private void DimCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            var mainPage = frame?.Content as MainPage;
            if (mainPage != null)
            {
                mainPage.SetDimOverlay(true);

                // Save the state
                ApplicationData.Current.LocalSettings.Values["DimBackground"] = true;
            }
        }

        private void DimCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            var mainPage = frame?.Content as MainPage;
            if (mainPage != null)
            {
                mainPage.SetDimOverlay(false);

                // Save the state
                ApplicationData.Current.LocalSettings.Values["DimBackground"] = false;
            }
        }

        private void FavoritesToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["FavoritesInTile"] = FavoritesToggle.IsOn;
        }

        private void TileIconToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["TileIconEnabled"] = TileIconToggle.IsOn;
        }
    }
}
