using QuoteTile.Common;
using QuoteTile.Services;
using System.Collections.ObjectModel;
using QuoteTile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace QuoteTile
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Favorites : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private FavoriteService _favoriteService;
        private ObservableCollection<QuoteModel> FavoritesCollection =
            new ObservableCollection<QuoteModel>();

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


        public Favorites()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            MainGrid.Background = AppBackgroundHelper.GetCurrentBackground();
            _favoriteService = FavoriteService.Instance;
            FavoritesListView.ItemsSource = FavoritesCollection;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            FavoritesCollection.Clear(); // clear old items

            var list = await _favoriteService.GetFavoritesAsync(); // await here
            foreach (var q in list) // foreach works
            {
                FavoritesCollection.Add(q);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void FavoritesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedQuote = (QuoteModel)e.ClickedItem;

            // Show confirmation dialog
            var dialog = new Windows.UI.Popups.MessageDialog(
                $"Are you sure you want to remove this quote from Favorites?\n\n“{selectedQuote.Content}” — {selectedQuote.Author}",
                "Confirm Remove"
            );

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });
            dialog.DefaultCommandIndex = 1; // default to No
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0) // User clicked Yes
            {
                // Remove from collection
                FavoritesCollection.Remove(selectedQuote);

                // Remove from storage
                await FavoriteService.Instance.RemoveFavoriteAsync(selectedQuote);
            }
        }
    }
}
