using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;
using QuoteTile.Models;
using QuoteTile.Services;
using Windows.UI.Xaml.Shapes;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuoteTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private QuoteModel _currentQuote;
        private FavoriteService _favoriteService = FavoriteService.Instance;
        public MainPage()
        {
            this.InitializeComponent();
            _favoriteService = FavoriteService.Instance;

            var value = ApplicationData.Current.LocalSettings.Values["DimBackground"];
            bool isDimmed = false;

            if (value != null && value is bool)
            {
                isDimmed = (bool)value;
            }

            // Apply to overlay
            SetDimOverlay(isDimmed);

            // Start animations
            SlideInStoryboard.Begin();
            BottomSlideStoryboard.Begin();

            // Attach Completed event for QuoteFadeStartupStoryboard
            QuoteFadeStartupStoryboard.Completed += QuoteFadeStartupStoryboard_Completed;
            QuoteFadeStartupStoryboard.Begin();

            StartQuoteAutoRefresh();
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        // Runs after QuoteFadeStartupStoryboard finishes
        private async void QuoteFadeStartupStoryboard_Completed(object sender, object e)
        {
            LoadQuote();
            await Task.Delay(1200);
            LoadQuote1();
            await Task.Delay(800);
            LoadQuote2();
            await Task.Delay(800);
            LoadQuote3();
            await Task.Delay(800);
            LoadQuote4();
            await Task.Delay(800);
            LoadQuote5();
            await Task.Delay(800);
            LoadQuote6();
            await Task.Delay(800);
            LoadQuote7();
            await Task.Delay(800);
            LoadQuote8();
            await Task.Delay(800);
            LoadQuote9();
        }

        private DispatcherTimer _quoteTimer;

        private int lastColorIndex = -1;

        public void ChangeBackground(string imagePath)
        {
            MainBackground.ImageSource = new BitmapImage(new Uri(imagePath));
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Optional: load saved background from LocalSettings
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("AppBackground"))
            {
                string path = settings.Values["AppBackground"].ToString();
                ChangeBackground(path);
            }
        }

        private async void LoadQuote()
        {
            try
            {
                HttpClient client = new HttpClient();
                Uri uri = new Uri($"http://api.quotable.io/random?cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                // TAGS
                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                _currentQuote = new QuoteModel
                {
                    Content = quote,
                    Author = author
                };

                // --- Update UI ---
                QuoteText.Text = $"“{_currentQuote.Content}”";
                AuthorText.Text = $"— {_currentQuote.Author}";
                DateAddedText.Text = "Date added: " + dateAdded;
                DateModifiedText.Text = "Date modified: " + dateModified;
                StatusText.Text = "Status: Online!";

                TagsPanel.ItemsSource = tagList;
            }

            catch (Exception)
            {
                Random rand = new Random();

                string rawQuote = fallbackQuotes[rand.Next(fallbackQuotes.Count)];

                // Split quote and tags
                string[] parts = rawQuote.Split(new[] { "\nTags: " }, StringSplitOptions.None);
                string quoteAuthorPart = parts[0];
                string tagsPart = parts.Length > 1 ? parts[1] : "";

                // Split quote and author
                string[] qa = quoteAuthorPart.Split(new[] { " — " }, StringSplitOptions.None);

                string quoteText = qa[0].Trim();
                string authorText = qa.Length > 1 ? qa[1].Trim() : "Unknown";

                QuoteText.Text = "Failed to load online quotes. Proceeding to fallback quotes.\n" + quoteText;
                AuthorText.Text = "— " + authorText;

                DateAddedText.Text = "";
                DateModifiedText.Text = "";

                // Load tags
                TagsPanel.ItemsSource = tagsPart
                    .Split(',')
                    .Select(t => t.Trim())
                    .Where(t => t.Length > 0)
                    .ToList();

                StatusText.Text = "Status: Offline! (Are you trying to access the API in a restricted network?)";
            }

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
            QuoteFadeStoryboard.Begin();
        }

        private async void LoadQuote1()
        {
            try
            {
                HttpClient client = new HttpClient();
                Uri uri = new Uri($"http://api.quotable.io/random?tags=inspirational&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText1.Text = "“" + quote + "”";
                AuthorText1.Text = author;
                DateAddedText1.Text = "Date added: " + dateAdded;
                DateModifiedText1.Text = "Date modified: " + dateModified;

                TagsPanel1.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText1.Text = "Failed to load online quotes.";
                AuthorText1.Text = "";
                DateAddedText1.Text = "";
                DateModifiedText1.Text = "";
                TagsPanel1.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard1.Background = cardColors[colorIndex];
            QuoteFadeStoryboard1.Begin();
        }

        private async void LoadQuote2()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=technology&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText2.Text = "“" + quote + "”";
                AuthorText2.Text = author;
                DateAddedText2.Text = "Date added: " + dateAdded;
                DateModifiedText2.Text = "Date modified: " + dateModified;

                TagsPanel2.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText2.Text = "Failed to load online quotes.";
                AuthorText2.Text = "";
                DateAddedText2.Text = "";
                DateModifiedText2.Text = "";
                TagsPanel2.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard2.Background = cardColors[colorIndex];
            QuoteFadeStoryboard2.Begin();
        }

        private async void LoadQuote3()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=wisdom&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText3.Text = "“" + quote + "”";
                AuthorText3.Text = author;
                DateAddedText3.Text = "Date added: " + dateAdded;
                DateModifiedText3.Text = "Date modified: " + dateModified;

                TagsPanel3.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText3.Text = "Failed to load online quotes.";
                AuthorText3.Text = "";
                DateAddedText3.Text = "";
                DateModifiedText3.Text = "";
                TagsPanel3.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard3.Background = cardColors[colorIndex];
            QuoteFadeStoryboard3.Begin();
        }

        private async void LoadQuote4()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=success&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText4.Text = "“" + quote + "”";
                AuthorText4.Text = author;
                DateAddedText4.Text = "Date added: " + dateAdded;
                DateModifiedText4.Text = "Date modified: " + dateModified;

                TagsPanel4.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText4.Text = "Failed to load online quotes.";
                AuthorText4.Text = "";
                DateAddedText4.Text = "";
                DateModifiedText4.Text = "";
                TagsPanel4.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard4.Background = cardColors[colorIndex];
            QuoteFadeStoryboard4.Begin();
        }

        private async void LoadQuote5()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=motivational&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText5.Text = "“" + quote + "”";
                AuthorText5.Text = author;
                DateAddedText5.Text = "Date added: " + dateAdded;
                DateModifiedText5.Text = "Date modified: " + dateModified;

                TagsPanel5.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText5.Text = "Failed to load online quotes.";
                AuthorText5.Text = "";
                DateAddedText5.Text = "";
                DateModifiedText5.Text = "";
                TagsPanel5.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard5.Background = cardColors[colorIndex];
            QuoteFadeStoryboard5.Begin();
        }

        private async void LoadQuote6()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=life&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText6.Text = "“" + quote + "”";
                AuthorText6.Text = author;
                DateAddedText6.Text = "Date added: " + dateAdded;
                DateModifiedText6.Text = "Date modified: " + dateModified;

                TagsPanel6.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText6.Text = "Failed to load online quotes.";
                AuthorText6.Text = "";
                DateAddedText6.Text = "";
                DateModifiedText6.Text = "";
                TagsPanel6.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard6.Background = cardColors[colorIndex];
            QuoteFadeStoryboard6.Begin();
        }

        private async void LoadQuote7()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=famous-quotes&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText7.Text = "“" + quote + "”";
                AuthorText7.Text = author;
                DateAddedText7.Text = "Date added: " + dateAdded;
                DateModifiedText7.Text = "Date modified: " + dateModified;

                TagsPanel7.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText7.Text = "Failed to load online quotes.";
                AuthorText7.Text = "";
                DateAddedText7.Text = "";
                DateModifiedText7.Text = "";
                TagsPanel7.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard7.Background = cardColors[colorIndex];
            QuoteFadeStoryboard7.Begin();
        }

        private async void LoadQuote8()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=philosophy&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText8.Text = "“" + quote + "”";
                AuthorText8.Text = author;
                DateAddedText8.Text = "Date added: " + dateAdded;
                DateModifiedText8.Text = "Date modified: " + dateModified;

                TagsPanel8.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText8.Text = "Failed to load online quotes.";
                AuthorText8.Text = "";
                DateAddedText8.Text = "";
                DateModifiedText8.Text = "";
                TagsPanel8.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard8.Background = cardColors[colorIndex];
            QuoteFadeStoryboard8.Begin();
        }

        private async void LoadQuote9()
        {
            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri($"http://api.quotable.io/random?tags=happiness&cb={DateTime.UtcNow.Ticks}");
                string json1 = await client.GetStringAsync(uri);
                JObject data1 = JObject.Parse(json1);
                string quote = data1["content"].ToString();
                string author = "— " + data1["author"].ToString();
                string dateAdded = data1["dateAdded"].ToString();
                string dateModified = data1["dateModified"].ToString();

                var tagsArray = data1["tags"];
                List<string> tagList = new List<string>();

                foreach (var tag in tagsArray)
                {
                    tagList.Add(tag.ToString());
                }

                QuoteText9.Text = "“" + quote + "”";
                AuthorText9.Text = author;
                DateAddedText9.Text = "Date added: " + dateAdded;
                DateModifiedText9.Text = "Date modified: " + dateModified;

                TagsPanel9.ItemsSource = tagList;
            }

            catch (Exception)
            {
                QuoteText9.Text = "Failed to load online quotes.";
                AuthorText9.Text = "";
                DateAddedText9.Text = "";
                DateModifiedText9.Text = "";
                TagsPanel9.ItemsSource = null;
            }

            // Pick random color
            Random colorRand = new Random();
            int colorIndex; // renamed to avoid duplicate 'index'
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;

            QuoteCard.Background = cardColors[colorIndex];
            QuoteFadeStoryboard9.Begin();
        }

        private void StartQuoteAutoRefresh()
        {
            _quoteTimer = new DispatcherTimer();
            _quoteTimer.Interval = TimeSpan.FromSeconds(15);
            _quoteTimer.Tick += QuoteTimer_Tick;
            _quoteTimer.Start();
        }

        private async void QuoteTimer_Tick(object sender, object e)
        {
            LoadQuote();
            await Task.Delay(1200);
            LoadQuote1();
            await Task.Delay(800);
            LoadQuote2();
            await Task.Delay(800);
            LoadQuote3();
            await Task.Delay(800);
            LoadQuote4();
            await Task.Delay(800);
            LoadQuote5();
            await Task.Delay(800);
            LoadQuote6();
            await Task.Delay(800);
            LoadQuote7();
            await Task.Delay(800);
            LoadQuote8();
            await Task.Delay(800);
            LoadQuote9();
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

        private void Refresh1_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote1();
        }

        private void Refresh2_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote2();
        }

        private void Refresh3_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote3();
        }

        private void Refresh4_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote4();
        }

        private void Refresh5_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote5();
        }

        private void Refresh6_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote6();
        }

        private void Refresh7_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote7();
        }

        private void Refresh8_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote8();
        }

        private void Refresh9_Click(object sender, RoutedEventArgs e)
        {
            LoadQuote9();
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

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText1.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText1.Text} {AuthorText1.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText2.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText2.Text} {AuthorText2.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote3_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText3.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText3.Text} {AuthorText3.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote4_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText4.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText4.Text} {AuthorText4.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote5_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText5.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText5.Text} {AuthorText5.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote6_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText6.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText6.Text} {AuthorText6.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote7_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText7.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText7.Text} {AuthorText7.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote8_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText8.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText8.Text} {AuthorText8.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            await dialog.ShowAsync();
        }

        private async void CopyQuote9_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuoteText9.Text))
                return;

            // Combine quote and author text
            string fullQuote = $"{QuoteText9.Text} {AuthorText9.Text}";

            // Copy to clipboard
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(fullQuote);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Fancy MessageDialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                "📋 Quote copied to clipboard!"
            )
            {
                Title = "QuoteTile Copy"
            };

            // Add OK button
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });

            // Default button
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

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

        private async void Favorite_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuote == null)
                return;

            try
            {
                // Add the current quote to favorites safely
                await _favoriteService.AddFavoriteAsync(_currentQuote);

                // Create a fancier MessageDialog
                var dialog = new Windows.UI.Popups.MessageDialog(
                    "⭐ The quote was added to your favorites!"
                )
                {
                    Title = "QuoteTile Favorites"
                };

                dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("View Favorites") { Id = 1 });
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 0;

                var result = await dialog.ShowAsync();

                if ((int)result.Id == 1)
                {
                    Frame.Navigate(typeof(Favorites));
                }
            }
            catch (Exception ex)
            {
                // Show the error if adding favorites fails
                var errorDialog = new Windows.UI.Popups.MessageDialog($"Error: {ex.Message}");
                await errorDialog.ShowAsync();
            }
        }

        private readonly List<SolidColorBrush> cardColors =
            new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromArgb(170, 18, 18, 22)),   // near black
            new SolidColorBrush(Color.FromArgb(170, 20, 24, 28)),   // charcoal
            new SolidColorBrush(Color.FromArgb(170, 16, 22, 30)),   // deep navy
            new SolidColorBrush(Color.FromArgb(170, 22, 18, 28)),   // dark purple
            new SolidColorBrush(Color.FromArgb(170, 24, 26, 32)),   // graphite
            new SolidColorBrush(Color.FromArgb(170, 18, 26, 34)),   // deep steel blue
            new SolidColorBrush(Color.FromArgb(170, 20, 20, 24)),   // soft black
            new SolidColorBrush(Color.FromArgb(170, 26, 20, 34)),   // midnight purple
            new SolidColorBrush(Color.FromArgb(170, 18, 28, 26)),   // deep teal
            new SolidColorBrush(Color.FromArgb(170, 24, 18, 18))    // dark maroon
        };

        private readonly List<string> fallbackQuotes = new List<string>
        {
            "“Stay hungry. Stay foolish. Your time is limited, so don’t waste it living someone else’s life.” — Steve Jobs\nTags: Motivation, Success, Life",
            "“Success is not final, failure is not fatal: it is the courage to continue that counts.” — Winston Churchill\nTags: Motivation, Courage, Perseverance",
            "“It always seems impossible until it’s done. The greatest glory in living lies not in never falling, but in rising every time we fall.” — Nelson Mandela\nTags: Motivation, Perseverance, Courage",
            "“In the middle of every difficulty lies opportunity, if you are willing to look for it.” — Albert Einstein\nTags: Opportunity, Wisdom, Optimism",
            "“The only way to do great work is to love what you do and keep pushing forward despite setbacks.” — Steve Jobs\nTags: Passion, Work, Motivation",
            "“Do what you can, with what you have, where you are, and never underestimate the difference small actions can make.” — Theodore Roosevelt\nTags: Action, Motivation, Effort",
            "“Life is what happens when you're busy making other plans, so make sure you are present along the way.” — John Lennon\nTags: Life, Awareness, Wisdom",
            "“The future belongs to those who believe in the beauty of their dreams and are willing to work for them.” — Eleanor Roosevelt\nTags: Dreams, Future, Inspiration",
            "“Whether you think you can or you think you can’t, you’re right, because your mindset shapes your reality.” — Henry Ford\nTags: Mindset, Motivation, Belief",
            "“Our greatest glory is not in never falling, but in rising every time we fall and learning from it.” — Confucius\nTags: Perseverance, Wisdom, Growth",
            "“The only limit to our realization of tomorrow will be our doubts of today, so believe bigger.” — Franklin D. Roosevelt\nTags: Belief, Future, Motivation",
            "“Success usually comes to those who are too busy to be looking for it and too determined to give up.” — Henry David Thoreau\nTags: Success, Determination, Hard Work",
            "“You miss 100% of the shots you don’t take, so take the risk and trust yourself.” — Wayne Gretzky\nTags: Risk, Courage, Opportunity",
            "“If you want to lift yourself up, lift up someone else and grow together.” — Booker T. Washington\nTags: Kindness, Leadership, Community",
            "“I have not failed. I’ve just found 10,000 ways that won’t work, and that’s part of learning.” — Thomas Edison\nTags: Learning, Perseverance, Innovation",
            "“The journey of a thousand miles begins with one step, but persistence carries you the rest of the way.” — Lao Tzu\nTags: Beginnings, Persistence, Wisdom",
            "“Happiness depends upon ourselves and how we choose to respond to the world around us.” — Aristotle\nTags: Happiness, Wisdom, Mindset",
            "“Everything you’ve ever wanted is on the other side of fear, so move toward it.” — George Addair\nTags: Courage, Growth, Motivation",
            "“Hardships often prepare ordinary people for an extraordinary destiny if they endure.” — C.S. Lewis\nTags: Hardship, Destiny, Perseverance",
            "“If opportunity doesn’t knock, build a door and create your own path forward.” — Milton Berle\nTags: Opportunity, Initiative, Success",
            "“The secret of getting ahead is getting started and staying consistent.” — Mark Twain\nTags: Action, Consistency, Success",
            "“Dream big and dare to fail, because bold attempts lead to remarkable outcomes.” — Norman Vaughan\nTags: Dreams, Courage, Ambition",
            "“Act as if what you do makes a difference, because it truly does.” — William James\nTags: Impact, Purpose, Motivation",
            "“Quality is not an act, it is a habit formed by consistent effort and intention.” — Aristotle\nTags: Discipline, Excellence, Habits",
            "“Don’t watch the clock; do what it does. Keep going until you reach your goal.” — Sam Levenson\nTags: Persistence, Focus, Motivation",
            "“Believe you can and you're halfway there; belief fuels persistence.” — Theodore Roosevelt\nTags: Belief, Motivation, Confidence",
            "“Doubt kills more dreams than failure ever will, so trust your potential.” — Suzy Kassem\nTags: Confidence, Dreams, Mindset",
            "“The harder I work, the luckier I get, because preparation meets opportunity.” — Gary Player\nTags: Hard Work, Success, Preparation",
            "“Start where you are. Use what you have. Do what you can, and improve daily.” — Arthur Ashe\nTags: Action, Improvement, Growth",
            "“If you can dream it, you can do it, provided you are willing to commit fully.” — Walt Disney\nTags: Dreams, Commitment, Success",
            "“Turn your wounds into wisdom and let experience shape your strength.” — Oprah Winfrey\nTags: Growth, Wisdom, Resilience",
            "“Creativity is intelligence having fun while solving meaningful problems.” — Albert Einstein\nTags: Creativity, Intelligence, Innovation",
            "“The best revenge is massive success achieved through focus and resilience.” — Frank Sinatra\nTags: Success, Focus, Determination",
            "“Don’t let yesterday take up too much of today; every day is a new chance.” — Will Rogers\nTags: Growth, Time, Motivation",
            "“You become what you believe, so nurture empowering thoughts.” — Oprah Winfrey\nTags: Mindset, Belief, Personal Growth",
            "“The purpose of our lives is to be happy and to help others find happiness.” — Dalai Lama\nTags: Happiness, Purpose, Compassion",
            "“A person who never made a mistake never tried anything new or courageous.” — Albert Einstein\nTags: Learning, Courage, Growth",
            "“Strive not to be a success, but rather to be of value to others.” — Albert Einstein\nTags: Value, Service, Purpose",
            "“Everything has beauty, but not everyone sees it; perspective matters.” — Confucius\nTags: Perspective, Beauty, Wisdom",
            "“Limit your 'always' and your 'nevers' and stay open to growth.” — Amy Poehler\nTags: Growth, Mindset, Wisdom",
            "“Opportunities don't happen. You create them through consistent action.” — Chris Grosser\nTags: Opportunity, Action, Success",
            "“Keep your face always toward the sunshine and shadows will fall behind you.” — Walt Whitman\nTags: Positivity, Optimism, Life",
            "“What lies behind us and what lies before us are tiny matters compared to what lies within us.” — Ralph Waldo Emerson\nTags: Inner Strength, Wisdom, Inspiration",
            "“The best way to predict the future is to invent it with courage and vision.” — Alan Kay\nTags: Future, Innovation, Vision",
            "“Perfection is not attainable, but if we chase perfection we can catch excellence.” — Vince Lombardi\nTags: Excellence, Effort, Growth",
            "“Do not wait to strike till the iron is hot; make it hot by striking.” — William Butler Yeats\nTags: Action, Opportunity, Courage",
            "“Failure will never overtake me if my determination to succeed is strong enough.” — Og Mandino\nTags: Determination, Success, Persistence",
            "“The mind is everything. What you think you become through repeated thought.” — Buddha\nTags: Mindset, Thought, Wisdom",
            "“To handle yourself, use your head; to handle others, use your heart.” — Eleanor Roosevelt\nTags: Leadership, Wisdom, Compassion",
            "“It does not matter how slowly you go as long as you do not stop.” — Confucius\nTags: Persistence, Patience, Progress",
            "“Courage is resistance to fear, mastery of fear — not absence of fear.” — Mark Twain\nTags: Courage, Fear, Strength",
            "“It always seems impossible until it is done.” — Nelson Mandela\nTags: Motivation, Perseverance, Courage",
            "“Do not go where the path may lead, go instead where there is no path and leave a trail.” — Ralph Waldo Emerson\nTags: Leadership, Courage, Innovation",
            "“A champion is defined not by their wins but by how they can recover when they fall.” — Serena Williams\nTags: Resilience, Strength, Perseverance",
            "“Do not judge me by my successes, judge me by how many times I fell and got back up.” — Nelson Mandela\nTags: Perseverance, Character, Strength",
            "“Keep your eyes on the stars and your feet on the ground.” — Theodore Roosevelt\nTags: Ambition, Balance, Vision",
            "“The only impossible journey is the one you never begin.” — Tony Robbins\nTags: Beginnings, Courage, Motivation",
            "“Motivation is what gets you started. Habit is what keeps you going.” — Jim Ryun\nTags: Habits, Motivation, Discipline",
            "“An investment in knowledge pays the best interest.” — Benjamin Franklin\nTags: Knowledge, Learning, Wisdom",
            "“If you want to achieve greatness stop asking for permission.” — Unknown\nTags: Courage, Leadership, Ambition",
            "“Be yourself; everyone else is already taken.” — Oscar Wilde\nTags: Authenticity, Identity, Wisdom",
            "“Small deeds done are better than great deeds planned.” — Peter Marshall\nTags: Action, Effort, Progress",
            "“Don’t let what you cannot do interfere with what you can do.” — John Wooden\nTags: Focus, Strength, Motivation",
            "“The way to get started is to quit talking and begin doing.” — Walt Disney\nTags: Action, Motivation, Success",
            "“It is during our darkest moments that we must focus to see the light.” — Aristotle\nTags: Hope, Resilience, Wisdom",
            "“Success is walking from failure to failure with no loss of enthusiasm.” — Winston Churchill\nTags: Success, Perseverance, Attitude",
            "“You are never too old to set another goal or to dream a new dream.” — C.S. Lewis\nTags: Dreams, Growth, Motivation",
            "“Happiness is not something ready made. It comes from your own actions.” — Dalai Lama\nTags: Happiness, Responsibility, Life"
        };
        public void SetDimOverlay(bool isVisible)
        {
            if (DimOverlay != null) // DimOverlay comes from x:Name
                DimOverlay.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool GetDimOverlayState()
        {
            return DimOverlay != null && DimOverlay.Visibility == Visibility.Visible;
        }
    }
}
