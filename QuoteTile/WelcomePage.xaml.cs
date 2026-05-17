using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QuoteTile
{
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();

            MainGrid.Background = AppBackgroundHelper.GetCurrentBackground();

            // Set dynamic slide distance for SlideFadeInStoryboard
            double screenWidth = Window.Current.Bounds.Width;

            // Slide in from left (negative screen width)
            SlideInAnimation.From = -screenWidth;
            SlideInAnimation.To = 0;

            // Slide out to right (positive screen width)
            SlideOutAnimation.From = 0;
            SlideOutAnimation.To = screenWidth;

            // Start slide + fade in
            SlideFadeInStoryboard.Begin();
        }

        private void GetStarted_Click(object sender, RoutedEventArgs e)
        {
            // Start slide + fade out
            SlideFadeOutStoryboard.Begin();
        }

        private void SlideFadeOutStoryboard_Completed(object sender, object e)
        {
            // Mark first launch complete
            ApplicationData.Current.LocalSettings.Values["IsFirstLaunch"] = false;

            // Navigate to main page
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}