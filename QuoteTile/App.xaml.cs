using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Collections.Generic;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Windows.System;

namespace QuoteTile
{
    sealed partial class App : Application
    {
        private DispatcherTimer tileTimer;
        private static readonly HttpClient client = new HttpClient();
        private static readonly Random rand = new Random();

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            // Check first launch
            var localSettings = ApplicationData.Current.LocalSettings;
            bool isFirstLaunch = !localSettings.Values.ContainsKey("IsFirstLaunch") ||
                                 (bool)localSettings.Values["IsFirstLaunch"];

            if (isFirstLaunch)
            {
                // Navigate to WelcomePage
                rootFrame.Navigate(typeof(WelcomePage), e.Arguments);

                // Do NOT subscribe to SettingsPane yet
            }
            else
            {
                // Navigate to MainPage
                rootFrame.Navigate(typeof(MainPage), e.Arguments);

            }

            Window.Current.Activate();
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
            LoadQuote();
            tileTimer = new DispatcherTimer();
            tileTimer.Interval = TimeSpan.FromSeconds(5);
            tileTimer.Tick += TileTimer_Tick;
            tileTimer.Start();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        public void OnCommandsRequested(SettingsPane sender,
                                         SettingsPaneCommandsRequestedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame?.Content is WelcomePage)
            {
                // If the current page is WelcomePage, do not show commands
                return;
            }

            // Favorites
            var favoritesCommand = new SettingsCommand("favorites", "Favorites", handler =>
            {
                rootFrame.Navigate(typeof(Favorites));
            });
            args.Request.ApplicationCommands.Add(favoritesCommand);

            // Community Quotes
            var commquotesCommand = new SettingsCommand("commquotes", "Community Quotes", handler =>
            {
                rootFrame.Navigate(typeof(CommQuotes));
            });
            args.Request.ApplicationCommands.Add(commquotesCommand);

            // Developers Hub
            var devhubCommand = new SettingsCommand("devhub", "Developers Hub", handler =>
            {
                rootFrame.Navigate(typeof(DevHub));
            });
            args.Request.ApplicationCommands.Add(devhubCommand);

            // About Us
            var aboutusCommand = new SettingsCommand("aboutus", "About Us", handler =>
            {
                rootFrame.Navigate(typeof(AboutUs));
            });
            args.Request.ApplicationCommands.Add(aboutusCommand);

            // Personalize
            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "personalize",
                "Personalize",
                handler => { new Personalize().Show(); }
            ));

            args.Request.ApplicationCommands.Add(
                new SettingsCommand("openWebsite", "Visit Website", async (p) =>
                {
                    var uri = new Uri("https://discord.gg/YBsVhkcHT4");
                    await Launcher.LaunchUriAsync(uri);
                })
            );

            // About
            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "about",
                "About",
                handler => { new About().Show(); }
            ));
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private readonly List<string> fallbackQuotes = new List<string>
        {
            "Stay hungry. Stay foolish. Your time is limited, so don’t waste it living someone else’s life. — Steve Jobs\nTags: Motivation, Success, Life",
            "Success is not final, failure is not fatal: it is the courage to continue that counts. — Winston Churchill\nTags: Motivation, Courage, Perseverance",
            "It always seems impossible until it’s done. The greatest glory in living lies not in never falling, but in rising every time we fall. — Nelson Mandela\nTags: Motivation, Perseverance, Courage",
            "In the middle of every difficulty lies opportunity, if you are willing to look for it. — Albert Einstein\nTags: Opportunity, Wisdom, Optimism",
            "The only way to do great work is to love what you do and keep pushing forward despite setbacks. — Steve Jobs\nTags: Passion, Work, Motivation",
            "Do what you can, with what you have, where you are, and never underestimate the difference small actions can make. — Theodore Roosevelt\nTags: Action, Motivation, Effort",
            "Life is what happens when you're busy making other plans, so make sure you are present along the way. — John Lennon\nTags: Life, Awareness, Wisdom",
            "The future belongs to those who believe in the beauty of their dreams and are willing to work for them. — Eleanor Roosevelt\nTags: Dreams, Future, Inspiration",
            "Whether you think you can or you think you can’t, you’re right, because your mindset shapes your reality. — Henry Ford\nTags: Mindset, Motivation, Belief",
            "Our greatest glory is not in never falling, but in rising every time we fall and learning from it. — Confucius\nTags: Perseverance, Wisdom, Growth",
            "The only limit to our realization of tomorrow will be our doubts of today, so believe bigger. — Franklin D. Roosevelt\nTags: Belief, Future, Motivation",
            "Success usually comes to those who are too busy to be looking for it and too determined to give up. — Henry David Thoreau\nTags: Success, Determination, Hard Work",
            "You miss 100% of the shots you don’t take, so take the risk and trust yourself. — Wayne Gretzky\nTags: Risk, Courage, Opportunity",
            "If you want to lift yourself up, lift up someone else and grow together. — Booker T. Washington\nTags: Kindness, Leadership, Community",
            "I have not failed. I’ve just found 10,000 ways that won’t work, and that’s part of learning. — Thomas Edison\nTags: Learning, Perseverance, Innovation",
            "The journey of a thousand miles begins with one step, but persistence carries you the rest of the way. — Lao Tzu\nTags: Beginnings, Persistence, Wisdom",
            "Happiness depends upon ourselves and how we choose to respond to the world around us. — Aristotle\nTags: Happiness, Wisdom, Mindset",
            "Everything you’ve ever wanted is on the other side of fear, so move toward it. — George Addair\nTags: Courage, Growth, Motivation",
            "Hardships often prepare ordinary people for an extraordinary destiny if they endure. — C.S. Lewis\nTags: Hardship, Destiny, Perseverance",
            "If opportunity doesn’t knock, build a door and create your own path forward. — Milton Berle\nTags: Opportunity, Initiative, Success",
            "The secret of getting ahead is getting started and staying consistent. — Mark Twain\nTags: Action, Consistency, Success",
            "Dream big and dare to fail, because bold attempts lead to remarkable outcomes. — Norman Vaughan\nTags: Dreams, Courage, Ambition",
            "Act as if what you do makes a difference, because it truly does. — William James\nTags: Impact, Purpose, Motivation",
            "Quality is not an act, it is a habit formed by consistent effort and intention. — Aristotle\nTags: Discipline, Excellence, Habits",
            "Don’t watch the clock; do what it does. Keep going until you reach your goal. — Sam Levenson\nTags: Persistence, Focus, Motivation",
            "Believe you can and you're halfway there; belief fuels persistence. — Theodore Roosevelt\nTags: Belief, Motivation, Confidence",
            "Doubt kills more dreams than failure ever will, so trust your potential. — Suzy Kassem\nTags: Confidence, Dreams, Mindset",
            "The harder I work, the luckier I get, because preparation meets opportunity. — Gary Player\nTags: Hard Work, Success, Preparation",
            "Start where you are. Use what you have. Do what you can, and improve daily. — Arthur Ashe\nTags: Action, Improvement, Growth",
            "If you can dream it, you can do it, provided you are willing to commit fully. — Walt Disney\nTags: Dreams, Commitment, Success",
            "Turn your wounds into wisdom and let experience shape your strength. — Oprah Winfrey\nTags: Growth, Wisdom, Resilience",
            "Creativity is intelligence having fun while solving meaningful problems. — Albert Einstein\nTags: Creativity, Intelligence, Innovation",
            "The best revenge is massive success achieved through focus and resilience. — Frank Sinatra\nTags: Success, Focus, Determination",
            "Don’t let yesterday take up too much of today; every day is a new chance. — Will Rogers\nTags: Growth, Time, Motivation",
            "You become what you believe, so nurture empowering thoughts. — Oprah Winfrey\nTags: Mindset, Belief, Personal Growth",
            "The purpose of our lives is to be happy and to help others find happiness. — Dalai Lama\nTags: Happiness, Purpose, Compassion",
            "A person who never made a mistake never tried anything new or courageous. — Albert Einstein\nTags: Learning, Courage, Growth",
            "Strive not to be a success, but rather to be of value to others. — Albert Einstein\nTags: Value, Service, Purpose",
            "Everything has beauty, but not everyone sees it; perspective matters. — Confucius\nTags: Perspective, Beauty, Wisdom",
            "Limit your 'always' and your 'nevers' and stay open to growth. — Amy Poehler\nTags: Growth, Mindset, Wisdom",
            "Opportunities don't happen. You create them through consistent action. — Chris Grosser\nTags: Opportunity, Action, Success",
            "Keep your face always toward the sunshine and shadows will fall behind you. — Walt Whitman\nTags: Positivity, Optimism, Life",
            "What lies behind us and what lies before us are tiny matters compared to what lies within us. — Ralph Waldo Emerson\nTags: Inner Strength, Wisdom, Inspiration",
            "The best way to predict the future is to invent it with courage and vision. — Alan Kay\nTags: Future, Innovation, Vision",
            "Perfection is not attainable, but if we chase perfection we can catch excellence. — Vince Lombardi\nTags: Excellence, Effort, Growth",
            "Do not wait to strike till the iron is hot; make it hot by striking. — William Butler Yeats\nTags: Action, Opportunity, Courage",
            "Failure will never overtake me if my determination to succeed is strong enough. — Og Mandino\nTags: Determination, Success, Persistence",
            "The mind is everything. What you think you become through repeated thought. — Buddha\nTags: Mindset, Thought, Wisdom",
            "To handle yourself, use your head; to handle others, use your heart. — Eleanor Roosevelt\nTags: Leadership, Wisdom, Compassion",
            "It does not matter how slowly you go as long as you do not stop. — Confucius\nTags: Persistence, Patience, Progress",
            "Courage is resistance to fear, mastery of fear — not absence of fear. — Mark Twain\nTags: Courage, Fear, Strength",
            "It always seems impossible until it is done. — Nelson Mandela\nTags: Motivation, Perseverance, Courage",
            "Do not go where the path may lead, go instead where there is no path and leave a trail. — Ralph Waldo Emerson\nTags: Leadership, Courage, Innovation",
            "A champion is defined not by their wins but by how they can recover when they fall. — Serena Williams\nTags: Resilience, Strength, Perseverance",
            "Do not judge me by my successes, judge me by how many times I fell and got back up. — Nelson Mandela\nTags: Perseverance, Character, Strength",
            "Keep your eyes on the stars and your feet on the ground. — Theodore Roosevelt\nTags: Ambition, Balance, Vision",
            "The only impossible journey is the one you never begin. — Tony Robbins\nTags: Beginnings, Courage, Motivation",
            "Motivation is what gets you started. Habit is what keeps you going. — Jim Ryun\nTags: Habits, Motivation, Discipline",
            "An investment in knowledge pays the best interest. — Benjamin Franklin\nTags: Knowledge, Learning, Wisdom",
            "If you want to achieve greatness stop asking for permission. — Unknown\nTags: Courage, Leadership, Ambition",
            "Be yourself; everyone else is already taken. — Oscar Wilde\nTags: Authenticity, Identity, Wisdom",
            "Small deeds done are better than great deeds planned. — Peter Marshall\nTags: Action, Effort, Progress",
            "Don’t let what you cannot do interfere with what you can do. — John Wooden\nTags: Focus, Strength, Motivation",
            "The way to get started is to quit talking and begin doing. — Walt Disney\nTags: Action, Motivation, Success",
            "It is during our darkest moments that we must focus to see the light. — Aristotle\nTags: Hope, Resilience, Wisdom",
            "Success is walking from failure to failure with no loss of enthusiasm. — Winston Churchill\nTags: Success, Perseverance, Attitude",
            "You are never too old to set another goal or to dream a new dream. — C.S. Lewis\nTags: Dreams, Growth, Motivation",
            "Happiness is not something ready made. It comes from your own actions. — Dalai Lama\nTags: Happiness, Responsibility, Life"
        };

        public void UpdateLiveTile(string quote1, string quote2, string quote3)
        {
            var settings = ApplicationData.Current.LocalSettings;
            bool showIcon = false;

            if (settings.Values.ContainsKey("TileIconEnabled"))
            {
                showIcon = (bool)settings.Values["TileIconEnabled"];
            }

            XmlDocument wideTileXml =
                TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text04);

            wideTileXml.GetElementsByTagName("text")[0].InnerText = quote1;

            XmlDocument mediumTileXml =
                TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);

            mediumTileXml.GetElementsByTagName("text")[0].InnerText = quote1;

            XmlDocument largeTileXml =
                TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310TextList02);

            var largeTexts = largeTileXml.GetElementsByTagName("text");

            largeTexts[0].InnerText = quote1;
            largeTexts[1].InnerText = quote2;
            largeTexts[2].InnerText = quote3;

            // 👇 APPLY BRANDING (THIS IS THE KEY PART)
            string brandingValue = showIcon ? "logo" : "name";

            SetBranding(wideTileXml, brandingValue);
            SetBranding(mediumTileXml, brandingValue);
            SetBranding(largeTileXml, brandingValue);

            // Combine tiles (unchanged)
            IXmlNode visualNode =
                wideTileXml.GetElementsByTagName("visual").Item(0);

            visualNode.AppendChild(
                wideTileXml.ImportNode(
                    mediumTileXml.GetElementsByTagName("binding").Item(0), true));

            visualNode.AppendChild(
                wideTileXml.ImportNode(
                    largeTileXml.GetElementsByTagName("binding").Item(0), true));

            TileNotification notification = new TileNotification(wideTileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private void SetBranding(XmlDocument tileXml, string value)
        {
            var binding = tileXml.GetElementsByTagName("binding").Item(0);

            // Remove existing attributes first (prevents duplicates/bugs)
            if (binding.Attributes.GetNamedItem("branding") != null)
                binding.Attributes.RemoveNamedItem("branding");

            if (binding.Attributes.GetNamedItem("displayName") != null)
                binding.Attributes.RemoveNamedItem("displayName");

            // Set branding
            var brandingAttr = tileXml.CreateAttribute("branding");
            brandingAttr.InnerText = value;
            binding.Attributes.SetNamedItem(brandingAttr);

            // 👇 THIS FIXES YOUR PROBLEM
            if (value == "none")
            {
                var displayNameAttr = tileXml.CreateAttribute("displayName");
                displayNameAttr.InnerText = ""; // prevents tile name fallback
                binding.Attributes.SetNamedItem(displayNameAttr);
            }
        }

        public async void LoadQuote()
        {
            var settings = ApplicationData.Current.LocalSettings;

            bool useFavorites =
                settings.Values.ContainsKey("FavoritesInTile") &&
                (bool)settings.Values["FavoritesInTile"];

            // 1. TRY FAVORITES FIRST
            if (useFavorites && await LoadFavoritesTile())
                return;

            // 2. TRY API
            if (await LoadApiTile())
                return;

            // 3. FINAL FALLBACK
            HandleFallback();
        }

        private async Task<bool> LoadApiTile()
        {
            var q1 = await SafeGetQuote("https://api.quotable.io/random?tags=tech");
            var q2 = await SafeGetQuote("https://api.quotable.io/random?tags=inspirational");
            var q3 = await SafeGetQuote("https://api.quotable.io/random?tags=wisdom");

            if (q1 == null && q2 == null && q3 == null)
                return false;

            UpdateLiveTile(
                Format(q1),
                Format(q2),
                Format(q3)
            );

            return true;
        }

        private async Task<QuoteData> SafeGetQuote(string url)
        {
            try
            {
                string json = await client.GetStringAsync(new Uri(url));

                var obj = JObject.Parse(json);

                return new QuoteData
                {
                    Quote = obj["content"]?.ToString(),
                    Author = obj["author"]?.ToString()
                };
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> LoadFavoritesTile()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync("favorites.json");

                string json = await FileIO.ReadTextAsync(file);

                var quotes = JsonConvert.DeserializeObject<List<QuoteData>>(json);

                if (quotes == null || quotes.Count == 0)
                    return false;

                var shuffled = quotes.OrderBy(_ => Guid.NewGuid()).ToList();

                UpdateLiveTile(
                    Format(shuffled.ElementAtOrDefault(0)),
                    Format(shuffled.ElementAtOrDefault(1)),
                    Format(shuffled.ElementAtOrDefault(2))
                );

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string BuildQuote(JObject data)
        {
            string content = data?["content"]?.ToString() ?? "No quote";
            string author = data?["author"]?.ToString() ?? "Unknown";

            return $"“{content}” — {author}";
        }

        private string Format(QuoteData q)
        {
            if (q == null || string.IsNullOrWhiteSpace(q.Quote))
                return "No quote available — Unknown";

            return $"“{q.Quote}” — {q.Author ?? "Unknown"}";
        }

        private void HandleFallback()
        {
            UpdateLiveTile(
                Format(ParseFallback(fallbackQuotes[rand.Next(fallbackQuotes.Count)])),
                Format(ParseFallback(fallbackQuotes[rand.Next(fallbackQuotes.Count)])),
                Format(ParseFallback(fallbackQuotes[rand.Next(fallbackQuotes.Count)]))
            );
        }

        private QuoteData ParseFallback(string raw)
        {
            int dashIndex = raw.IndexOf("—");

            if (dashIndex == -1)
            {
                return new QuoteData
                {
                    Quote = raw.Trim(),
                    Author = "Unknown"
                };
            }

            string quote = raw.Substring(0, dashIndex).Trim();
            string author = raw.Substring(dashIndex + 1).Trim();

            int tagsIndex = author.IndexOf("\nTags:");
            if (tagsIndex != -1)
                author = author.Substring(0, tagsIndex).Trim();

            return new QuoteData
            {
                Quote = quote,
                Author = author
            };
        }

        private void TileTimer_Tick(object sender, object e)
        {
            LoadQuote();
        }
    }
    public class QuoteData
    {
        [Newtonsoft.Json.JsonProperty("Content")]
        public string Quote { get; set; }

        public string Author { get; set; }
    }
}