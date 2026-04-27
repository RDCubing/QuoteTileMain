using QuoteTile.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace QuoteTile
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class AboutUs : Page
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

        public ObservableCollection<StaffMember> StaffMembers { get; set; }

        public AboutUs()
        {
            this.InitializeComponent();

            // Setup the navigation helper
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            // Setup the logical page navigation components that allow
            // the page to only show one pane at a time.
            this.navigationHelper.GoBackCommand = new QuoteTile.Common.RelayCommand(() => this.GoBack(), () => this.CanGoBack());
            this.itemListView.SelectionChanged += itemListView_SelectionChanged;

            // Start listening for Window size changes 
            // to change from showing two panes to showing a single pane
            Window.Current.SizeChanged += Window_SizeChanged;
            this.InvalidateVisualState();

            RootGrid.Background = AppBackgroundHelper.GetCurrentBackground();

            StaffMembers = new ObservableCollection<StaffMember>()
            {
                new StaffMember
                {
                    Name = "Andrew Simson",
                    Role = "Main Developer, UI Designer, and Owner of GDC",
                    Bio = "Andrew (also known as AndrewTheGeek on Discord) is the primary developer and creator of QuoteTile, a project that began development in February 2026. As the lead developer, Andrew is responsible for the overall design, development, and ongoing maintenance of the application, overseeing every stage of its lifecycle from concept to release. His dedication ensures that QuoteTile not only functions smoothly but also evolves with the needs of its users.\n\nThe first public release of QuoteTile, version 1.0.0.0, was published on 8Store, marking the project’s initial availability to the public. Since that launch, Andrew has continuously expanded and refined the application, achieving multiple development milestones, integrating new features, and enhancing the overall user experience. His methodical approach and attention to detail have helped establish QuoteTile as a reliable and engaging platform for users seeking curated quotes.\n\nBeyond QuoteTile, Andrew is actively engaged in several related and experimental projects. This includes QuoteTile for Windows Phone 8.1, designed to bring the QuoteTile experience to legacy Windows Phone devices, and Project 9600, which focuses on creating a 10-to-8 style Windows installation, allowing Windows 10 systems to be configured with a Windows 8–style interface. These projects demonstrate Andrew’s commitment to both innovation and backward compatibility, highlighting his ability to adapt modern technology to a variety of user needs. Through these endeavors, he continues to refine his programming skills, explore new development techniques, and expand the Windows ecosystem with tools and applications that enhance usability and accessibility.\n\nIn addition to his development work, Andrew is the owner and founder of GDC (Geek Devs Community), the primary hub for all his projects. GDC serves as a collaborative space for developers, testers, and enthusiasts, offering a platform for discussion, beta testing, and community engagement. Under Andrew’s leadership, GDC has grown into a vibrant ecosystem where innovative ideas can flourish, and projects benefit from direct community involvement and support.\n\nAndrew’s vision extends beyond individual applications; he is committed to building tools and experiences that empower users, foster collaboration, and encourage innovation. Through his continuous work on QuoteTile, Project 9600, and other initiatives within GDC, Andrew demonstrates a dedication to quality, creativity, and technical excellence. His ongoing mission is to enhance the Windows experience for users worldwide, bridging the gap between legacy systems and modern software while inspiring a community of developers to innovate alongside him.",
                    Photo = "ms-appx:///Images/user.png"
                },

                new StaffMember
                {
                    Name = "Xylo",
                    Role = "Feature Implementaton, Debugger, Tester, and Administrator of GDC",
                    Bio = "Xylo is a secondary developer for QuoteTile and a key member of the development team. Specializing in feature implementation, bug fixing, and testing, Xylo ensures that every update is stable, reliable, and polished, providing users with a smooth and engaging experience across all platforms. Their work is essential to maintaining the high quality and consistency of the application.\n\nWorking closely with the lead developer, Xylo translates design concepts into functional features, identifies and resolves issues across platforms, and performs thorough testing to ensure the application meets the highest standards of quality and performance. Currently, Xylo is deeply involved in the development of QuoteTile 2.0, contributing to new features, refining existing functionality, and overseeing the release of updates to ensure a seamless, bug-free experience. Their contributions are critical to shaping the next generation of QuoteTile, offering improved usability, enhanced performance, and innovative capabilities for users.\n\nBeyond their work on QuoteTile, Xylo actively participates in other projects within GDC (Geek Devs Community). They support development, testing, and feature implementation across multiple initiatives, collaborating with other developers to maintain consistent quality and to strengthen the community’s ecosystem. Through this involvement, Xylo helps ensure that GDC projects remain reliable, functional, and aligned with community goals.\n\nBy combining technical expertise, dedication, and collaboration, Xylo plays an essential role in both QuoteTile and the wider GDC ecosystem. Their contributions not only enhance individual projects but also help foster a vibrant, innovative community where developers and users alike can engage, learn, and contribute to the ongoing growth of the platform.",
                    Photo = "ms-appx:///Images/user.png"
                },

                new StaffMember
                {
                    Name = "Alex",
                    Role = "Backend Developer, and Administrator of GDC",
                    Bio = "Alex (also known as KeyboardGremlin on Discord) is a developer on the QuoteTile team, specializing in contributions, bug fixing, and testing. Alex plays a crucial role in ensuring that every update is stable, reliable, and polished, maintaining a seamless and consistent experience for users across all platforms.\n\nWorking closely with the lead and secondary developers, Alex identifies and resolves issues, tests new features, and contributes improvements to the project’s codebase. Their meticulous approach and attention to detail help maintain the high quality, responsiveness, and reliability of QuoteTile, ensuring that each release meets the team’s standards for excellence.\n\nBeyond their work on QuoteTile, Alex also supports other initiatives within GDC (Geek Devs Community). By assisting with testing, bug resolution, and code contributions, Alex helps strengthen the overall stability and quality of multiple community projects, enabling the team to deliver reliable and well-polished applications to users.\n\nThrough careful testing, problem-solving, and collaborative development, Alex plays a vital role in both QuoteTile and the wider GDC ecosystem. Their efforts ensure the continued stability, improvement, and growth of the projects, helping the team provide a consistently high-quality experience to the community.",
                    Photo = "ms-appx:///Images/user.png"
                },

                new StaffMember
                {
                    Name = "ChrisRLillo",
                    Role = "Tester, 1st Contributor, and Administrator of GDC",
                    Bio = "ChrisRLillo is recognized as the first-ever supporter of Andrew’s projects and the very first person to try out his work in February 2026. As an early tester, ChrisRLillo played a key role in demonstrating the potential of Andrew’s projects and providing the encouragement that inspired Andrew to create the Geek Devs Community (GDC).\n\nWhile ChrisRLillo’s primary involvement was as a tester, their enthusiasm, support, and motivation were instrumental in shaping the early direction of Andrew’s work and the foundation of GDC. By showing genuine interest from the very beginning, ChrisRLillo helped foster the collaborative and innovative spirit that drives the team today.\n\nOutside of software development, ChrisRLillo is also a musician and content creator on YouTube, sharing music and videos under the handle ChrisRLillo. Their creative work highlights a multifaceted talent, combining technical expertise with artistic expression.\n\nThrough early support, ongoing community involvement, and creative endeavors, ChrisRLillo has had a lasting impact on QuoteTile, GDC, and the broader developer and creator communities. Their contributions continue to be celebrated as a foundational influence within the team.",
                    Photo = "ms-appx:///Images/user.png"
                },
                new StaffMember
                {
                    Name = "Tetify",
                    Role = "Trusted Administrator of GDC",
                    Bio = "Tetify is a Trusted Administrator of the Geek Devs Community (GDC), whose decisive moderation and behind-the-scenes actions keep the community stable and well-managed. Without their intervention, many situations could have escalated, disrupting the server’s organization, fairness, and overall experience. Their consistent leadership ensures that rules are upheld, issues are resolved quickly, and the community remains a safe, collaborative space for all members.",
                    Photo = "ms-appx:///Images/user.png"
                }
            };
        }

        void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation())
            {
                this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a bindable group to Me.DefaultViewModel("Group")
            // TODO: Assign a collection of bindable items to Me.DefaultViewModel("Items")

            if (e.PageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (e.PageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    // TODO: Invoke Me.itemsViewSource.View.MoveCurrentTo() with the selected
                    //       item as specified by the value of pageState("SelectedItem")

                }
            }
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
            if (this.itemsViewSource.View != null)
            {
                // TODO: Derive a serializable navigation parameter and assign it to
                //       pageState("SelectedItem")

            }
        }

        #region Logical page navigation

        // The split page is designed so that when the Window does have enough space to show
        // both the list and the details, only one pane will be shown at at time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        private const int MinimumWidthForSupportingTwoPanes = 768;

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <returns>True if the window should show act as one logical page, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation()
        {
            return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
        }

        /// <summary>
        /// Invoked with the Window changes size
        /// </summary>
        /// <param name="sender">The current Window</param>
        /// <param name="e">Event data that describes the new size of the Window</param>
        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.InvalidateVisualState();
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
        }

        private bool CanGoBack()
        {
            if (this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null)
            {
                return true;
            }
            else
            {
                return this.navigationHelper.CanGoBack();
            }
        }
        private void GoBack()
        {
            if (this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                this.navigationHelper.GoBack();
            }
        }

        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
            this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        private string DetermineVisualState()
        {
            if (!UsingLogicalPageNavigation())
                return "PrimaryView";

            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null;

            return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
        }

        #endregion

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
    }
}
