using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuoteTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _quoteTimer;
        private int lastColorIndex = -1;
        public MainPage()
        {
            this.InitializeComponent();
            LoadQuote();
            StartQuoteAutoRefresh();

            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        public void UpdateLiveTile(string message)
        {
            // 1. Get a built-in XML template (e.g., a wide tile with text)
            XmlDocument tileXml = 
                TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text04);


            // 2. Find the text node and inject your content
            XmlNodeList textNodes = tileXml.GetElementsByTagName("text");
            if (textNodes.Length > 0)
                textNodes[0].InnerText = message;

            // 3. Create the notification and update the tile
            TileNotification notification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private async void LoadQuote()
        {
            try
            {
                HttpClient client = new HttpClient();
                string json = await client.GetStringAsync("http://api.quotable.io/random");

                JObject data = JObject.Parse(json);

                string quote = data["content"].ToString();
                string author = "— " + data["author"].ToString();

                // --- Update UI ---
                QuoteText.Text = "“" + quote + "”";
                AuthorText.Text = author;

                // Combine quote + author for the tile
                UpdateLiveTile(quote + "\n" + author);


                Random rand = new Random();
                int index;
                do
                {
                    index = rand.Next(cardColors.Count);
                }
                while (index == lastColorIndex);

                lastColorIndex = index;
                QuoteCard.Background = cardColors[index];
            }
            catch (Exception ex)
            {
                Random rand = new Random();
                string quote = fallbackQuotes[rand.Next(fallbackQuotes.Count)];
                QuoteText.Text = "Failed to load online quotes. Proceeding to fallback quotes.\n" + quote;
                AuthorText.Text = "";
            }
            // Prepare for fade animation
            QuoteText.Opacity = 0;
            AuthorText.Opacity = 0;
            QuoteCard.Opacity = 0;

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;
            QuoteCard.Background = cardColors[colorIndex];

            // Fade card + text back in
            QuoteCard.Opacity = 1;
            QuoteFadeStoryboard.Begin();

        }
        private void StartQuoteAutoRefresh()
        {
            _quoteTimer = new DispatcherTimer();
            _quoteTimer.Interval = TimeSpan.FromSeconds(15);
            _quoteTimer.Tick += QuoteTimer_Tick;
            _quoteTimer.Start();
        }
        private void QuoteTimer_Tick(object sender, object e)
        {
            LoadQuote();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _quoteTimer?.Stop();
            base.OnNavigatedFrom(e);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote();
        }

        private async void CopyQuote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText.Text} {AuthorText.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Optional: notify user
            var dialog = new Windows.UI.Popups.MessageDialog("Quote copied to clipboard!");
            await dialog.ShowAsync();
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText.Text))
                return;
            DataTransferManager.ShowShareUI();
        }
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;

            string quoteToShare = $"{QuoteText.Text} {AuthorText.Text}";

            request.Data.SetText(quoteToShare);
            request.Data.Properties.Title = "Share Quote";
            request.Data.Properties.Description = "A quote from QuoteTile";
        }
        private readonly List<SolidColorBrush> cardColors =
            new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromArgb(255, 44, 62, 80)),  // dark blue-gray
            new SolidColorBrush(Color.FromArgb(255, 27, 39, 53)),  // navy
            new SolidColorBrush(Color.FromArgb(255, 46, 46, 62)),  // dark purple-gray
            new SolidColorBrush(Color.FromArgb(255, 33, 52, 68)),  // steel blue
            new SolidColorBrush(Color.FromArgb(255, 48, 63, 80))   // slate
        };
        private readonly List<string> quoteHistory = new List<string>();
        private int currentQuoteIndex = -1;
        private readonly List<string> fallbackQuotes = new List<string>
        {
            "Stay hungry. Stay foolish. — Steve Jobs",
            "Simplicity is the ultimate sophistication. — Leonardo da Vinci",
            "The best way to predict the future is to invent it. — Alan Kay",
            "Do what you can, with what you have, where you are. — Theodore Roosevelt",
            "Life is what happens when you're busy making other plans. — John Lennon"
        };
        private void ShowFallbackQuote()
        {
            Random rand = new Random();
            string quote = fallbackQuotes[rand.Next(fallbackQuotes.Count)];

            QuoteText.Text = quote;
            AuthorText.Text = "";
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Show();
        }
        private void Personalize_Click(object sender, RoutedEventArgs e)
        {
            var personalize = new Personalize();
            personalize.Show();
        }
    }
}
