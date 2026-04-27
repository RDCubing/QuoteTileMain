using QuoteTile.Common;
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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using QuoteTile.Models;
using QuoteTile.Services;
using Windows.UI.Xaml.Shapes;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace QuoteTile
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CommQuotes : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public CommQuotes()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            MainGrid.Background = AppBackgroundHelper.GetCurrentBackground();

            StartQuoteAutoRefresh();

            // Attach Completed event for QuoteFadeStartupStoryboard
            QuoteFadeStartupStoryboard.Completed += QuoteFadeStartupStoryboard_Completed;
            QuoteFadeStartupStoryboard.Begin();
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="Common.SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="Common.NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void QuoteFadeStartupStoryboard_Completed(object sender, object e)
        {
            LoadQuote();
        }

        private DispatcherTimer _quoteTimer;

        private int lastColorIndex = -1;

        private void LoadQuote()
        {
            Random rand = new Random();

            // Pick a random fallback quote
            string rawQuote = fallbackQuotes[rand.Next(fallbackQuotes.Count)];

            // Split lines: quote+author, tags, dates
            string[] lines = rawQuote.Split('\n');
            string quoteAuthorPart = lines[0];
            string tagsPart = lines.Length > 1 ? lines[1] : "";
            string datePart = lines.Length > 2 ? lines[2] : "";

            // Extract quote text and author
            string[] qa = quoteAuthorPart.Split(new[] { " — " }, StringSplitOptions.None);
            string quoteText = qa[0].Trim();
            string authorText = qa.Length > 1 ? qa[1].Trim() : "Unknown";

            // Extract tags
            string tags = tagsPart.Replace("Tags: ", "");
            var tagList = tags.Split(',')
                              .Select(t => t.Trim())
                              .Where(t => t.Length > 0)
                              .ToList();

            // Extract dates
            string dateAdded = "";
            string dateModified = "";
            if (datePart.StartsWith("Date added:"))
            {
                string[] dateSplit = datePart.Split(new[] { "Date modified:" }, StringSplitOptions.None);
                dateAdded = dateSplit[0].Replace("Date added:", "").Trim();
                dateModified = dateSplit.Length > 1 ? dateSplit[1].Trim() : "";
            }

            // Set UI
            QuoteText.Text = quoteText;
            AuthorText.Text = $"— {authorText}";
            TagsPanel.ItemsSource = tagList;
            DateAddedText.Text = "Date added: " + dateAdded;
            DateModifiedText.Text = "Date modified: " + dateModified;

            // Random card color
            Random colorRand = new Random();
            int colorIndex;
            do
            {
                colorIndex = colorRand.Next(cardColors.Count);
            } while (colorIndex == lastColorIndex);

            lastColorIndex = colorIndex;
            QuoteCard.Background = cardColors[colorIndex];

            // Fade animation
            QuoteFadeStoryboard.Begin();
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
            "“Never let the light of hope fade out from your life.” — ChrisRLillo\nTags: Motivational, Positive\nDate added: 2026-03-13 Date modified: 2026-03-13",
            "“Sometimes, people don’t understand the meaning of your talents, so they may ignore you. Keep going anyway.” — Andrew Simson (AndrewTheGeek)\nTags: Motivational, Inspirational\nDate added: 2026-03-13 Date modified: 2026-03-13",
            "“The heart that forgives builds bridges, while anger only builds walls.” — Alex (KeyboardGremlin)\nTags: Forgiveness, Peace, Kindness\nDate added: 2026-03-13 Date modified: 2026-03-13",
            "“Ideas are whispers of tomorrow; listen closely, and you’ll hear the future.” — Xylo\nTags: Creativity, Inspiration, Innovation\nDate added: 2026-03-13 Date modified: 2026-03-13"
        };
    }
}
