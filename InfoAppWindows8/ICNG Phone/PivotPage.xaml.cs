using ICNG_Phone.Common;
using ICNG_Phone.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using System.Runtime.InteropServices;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CustomLiveTiles;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Windows.Data.Json;
using App8.ViewModels;
using App8.Views;
using App8.Helpers;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
//using Microsoft.Phone.Controls.Maps;




// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace ICNG_Phone
{
    interface IWebAuthenticationContinuable
    {
        void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args);
    }
    public class ChatMessage
    {
        public string Id
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string DateTime
        {
            get;
            set;
        }

        public string Channel
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} Says: {2}", DateTime, From, Content);
        }
    }

    public class Channel
    {
        public int Id { get; set; }
        public string Uri { get; set; }
    }
    public sealed partial class PivotPage : Page, IWebAuthenticationContinuable
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private string result1;
        private string result;
        private bool fbflag;
        private string name = string.Empty;
        private IMobileServiceTable<ChatMessage> chatTable = MobileService.GetTable<ChatMessage>();
        StringBuilder messages = null;

        string Authority = "https://login.windows.net/developertenant.onmicrosoft.com";
        string Resource = "https://outlook.office365.com/";
        string ClientID = "43ba3c74-34e2-4dde-9a6a-2671b53c181c";
        string RedirectUri = "http://l";

        public static ObservableCollection<ChatMessage> iMessages = null;

        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args) {
            try
            {
                string access_token = await RequestToken(args.WebAuthenticationResult);
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                HttpResponseMessage response = httpClient.GetAsync("https://outlook.office365.com/EWS/OData/Me/Inbox/Messages?$filter=HasAttachments eq true&$select=Subject,Sender,DateTimeReceived").Result;
                if (response.IsSuccessStatusCode)
                {
                    ShowMessage(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch
            {

            }
        }

        private void ShowMessage(string p)
        {
            
        }

        private void loginbutton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((string)loginbutton.Content.ToString() == "Sign In")
            {
                RequestCode();
            }
            else
            {
                TextBoxName.Text = "(not connected, Please Sign In)";
                loginbutton.Content = "Sign In";
            }
        }

        private void RequestCode()
        {
             
            string authURL = string.Format(
                "{0}/oauth2/authorize?response_type=code&resource={1}&client_id={2}&redirect_uri={3}",
                Authority,
                Resource,
                ClientID,
                RedirectUri);
            WebAuthenticationBroker.AuthenticateAndContinue(new Uri(authURL), new Uri(RedirectUri), null, WebAuthenticationOptions.None);
            //RequestToken();

        }

        private async Task<string> RequestToken(WebAuthenticationResult rez)
        {
            if (rez.ResponseStatus == WebAuthenticationStatus.Success)
            {
                string code = ParseCode(rez.ResponseData);
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/oauth2/token", Authority));
                string tokenreq = string.Format(
                        "grant_type=authorization_code&code={0}&client_id={1}&redirect_uri={2}",
                        code, ClientID, Uri.EscapeDataString(RedirectUri));
                request.Content = new StringContent(tokenreq, Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();

                var jResult = JObject.Parse(responseString);
                var id = DecodeIdToken((string)jResult["access_token"]);
                return (string)jResult["access_token"];
            }
            else
            {
                throw new Exception(String.Format("Something went wrong: {0}", rez.ResponseErrorDetail.ToString()));
            }
        }

        private string ParseCode(string result)
        {
            int codeIndex = result.IndexOf("code=", 0) + 5;
            int endCodeIndex = result.IndexOf("&", codeIndex);
            // Return the access code as a string
            return result.Substring(codeIndex, endCodeIndex - codeIndex);
        }

        private string DecodeIdToken(string start)
        {

            if (start == null)
                return "[No id_token returned]";

            string idcmp = start.Split('.')[1];
            if ((idcmp.Length % 4) != 0)
            {
                idcmp = idcmp.PadRight(idcmp.Length + (4 - (idcmp.Length % 4)), '=');
            }
            byte[] dec = Convert.FromBase64String(idcmp);
            // dynamic d = JsonConvert.DeserializeObject(idcmp);

            JObject jo = JObject.Parse(Encoding.UTF8.GetString(dec, 0, dec.Count())) as JObject;
            dynamic d = JsonConvert.SerializeObject(jo, Formatting.Indented);
            JsonValue d1 = JsonValue.Parse(d);
            var name = d1.GetObject().GetNamedString("name");
            var email = d1.GetObject().GetNamedString("email");
            loginbutton.Content = "Sign Out";
            TextBoxName.Text = name;
            return JsonConvert.SerializeObject(jo, Formatting.Indented);
        }

        public PivotPage()
        {
            this.InitializeComponent();
            messages = new StringBuilder();
            iMessages = new ObservableCollection<ChatMessage>();
            iMessages.CollectionChanged += Messages_CollectionChanged;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.DataContext = App.CurrentUser;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            //readMessage();
            facebook();
            instagram();
            map();
            GetTweets();
        }

        

        void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var chatMessage = item as ChatMessage;
                    if (chatMessage.From != name)
                    {
                        
                        this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                        {
                            TextBoxChatMessages.Text += string.Format("{0}\n", chatMessage.ToString());
                        });
                    }
                }
            }
        }

        private void ButtonLetsGo_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxName.Text) && TextBoxName.Text != "(not connected, Please Sign In)")
            {
                name = TextBoxName.Text.Trim();
                GridRegister.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                GridChat.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ReadyForNextMessage();
                Timer();
            }
        }

        private void ButtonContent_Click_1(object sender, RoutedEventArgs e)
        {
            string[] badWords = new[] { "fuck", "f u c k","sex", "s e x", "bobo", "b o b o", "f.u.c.k", "s.e.x", "b.o.b.o",
            "puta", "tanga", "p u t a", "t a n g a", "p.u.t.a", "t.a.n.g.a", "tangina", "t.a.n.g.i.n.a", "putangina",
            "p.u.t.a.n.g.i.n.a"};
            string input = TextBoxMessage.Text;
            string output = Filter(input, badWords);

            if (!string.IsNullOrWhiteSpace(output))
            {
                var chatMessage = new ChatMessage()
                {
                    From = name,
                    Content = output.Trim(),
                    //DateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime = DateTime.Now.ToString(),
                };
                InsertChatMessage(chatMessage);
            }
            
            ReadyForNextMessage();
            TextBoxMessage.Text = "";
        }

        void map()
        {
            this.myMap.Center = new Geopoint(new BasicGeoposition() { Latitude = 14.425724, Longitude = 121.040133 });
            MapIcon MapIcon1 = new MapIcon();
            
            MapIcon1.Location = new Geopoint(new BasicGeoposition()
            {
                Latitude = 14.425724,
                Longitude = 121.040133
            });
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = "Informatics College Northgate";
            myMap.MapElements.Add(MapIcon1);
            MapIcon1.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/PushPin.png"));            
        }

        private async void InsertChatMessage(ChatMessage message)
        {
            try
            {
                TextBoxChatMessages.Text += string.Format("{0}\n", message.ToString());
                //message = output;
                await chatTable.InsertAsync(message);
                ReadyForNextMessage();
            }
            catch
            {

            }
        }

        public static string Filter(string input, string[] badWords)
        {
            var re = new Regex(
                @"\b("
                + string.Join("|", badWords.Select(word =>
                    string.Join(@"\s*", word.ToCharArray())))
                + @")\b", RegexOptions.IgnoreCase);
            return re.Replace(input, match =>
            {
                return new string('*', match.Length);
            });
        }

        private void ReadyForNextMessage()
        {
            TextBoxMessage.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://informaticsng.azure-mobile.net/",
            "uUTkMiFVMrLfXJkedIVvlamrMHPKUl43"
        );

        public async void readMessage()
        {
            try
            {
                var items = await chatTable.ToCollectionAsync();
                var temp = items;

                for (var i = 0; i < items.Count; i++)
                {
                    var oldChat = new ChatMessage();
                    oldChat.From = items[i].From;
                    oldChat.DateTime = items[i].DateTime;
                    oldChat.Content = items[i].Content;
                    TextBoxChatMessages.Text += Environment.NewLine + oldChat.ToString();
                }
            }
            catch
            {

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var grid = (Grid)VisualTreeHelper.GetChild(TextBoxChatMessages, 0);
                for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                {
                    object obj = VisualTreeHelper.GetChild(grid, i);
                    if (!(obj is ScrollViewer)) continue;
                    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                    break;
                }
            }
            catch
            {

            }
        }

        async Task loadchat()
        {
            try
            {
                var item = await chatTable.ToCollectionAsync();
                linemessage(item);
            }
            catch
            {

            }
        }

        private void linemessage(MobileServiceCollection<ChatMessage, ChatMessage> items)
        {
            //throw new NotImplementedException();
            for (int i = 0; i < items.Count; i++)
            {
                var oldChat = new ChatMessage();
                oldChat.From = items[i].From;
                oldChat.DateTime = items[i].DateTime;
                oldChat.Content = items[i].Content;
                TextBoxChatMessages.Text += Environment.NewLine + oldChat.ToString();
            }
        }

        public void Timer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += ticker;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        int time = 0;
        int timectr = 0;
        public async void ticker(object sender, object e)
        {
            time++;
            timectr++;
            if (timectr % 30 == 0)
            {
                TextBoxChatMessages.Text = "";
                await loadchat();
            }
            //testing.Text = time.ToString();
        }

        async private void facebook()
        {
            try
            {
                var client = new HttpClient(); // Add: using System.Net.Http;
                var response = await client.GetAsync(new Uri("https://graph.facebook.com/1501735393442310/posts?access_token=681471118637012|HARByNx18pD7X-jPskRr4dyxVlg"));
                var result = await response.Content.ReadAsStringAsync();
                JsonValue jsonList = JsonValue.Parse(result); // Add: using Windows.Data.Json;
                JsonArray message = jsonList.GetObject().GetNamedArray("data");

                var facebookfeed1 = message.GetObjectAt(0).GetObject().GetNamedString("message");
                var facebookfeeddate1 = message.GetObjectAt(0).GetObject().GetNamedString("created_time");
                var facebookfeed2 = message.GetObjectAt(1).GetObject().GetNamedString("message");
                var facebookfeeddate2 = message.GetObjectAt(1).GetObject().GetNamedString("created_time");
                var facebookfeed3 = message.GetObjectAt(2).GetObject().GetNamedString("message");
                var facebookfeeddate3 = message.GetObjectAt(2).GetObject().GetNamedString("created_time");
                //string messagefeed1 = facebookfeed1.GetObject().GetNamedString("message");
                newsfeed1.Text = facebookfeed1;
                newsfeed2.Text = facebookfeed2;
                newsfeed3.Text = facebookfeed3;

                newsfeeddate1.Text = "Posted Last " + DateTime.Parse(facebookfeeddate1);
                newsfeeddate2.Text = "Posted Last " + DateTime.Parse(facebookfeeddate2);
                newsfeeddate3.Text = "Posted Last " + DateTime.Parse(facebookfeeddate3);
            }
            catch { }

        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        /// 

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }

        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
           // var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-1");
            //this.DefaultViewModel[FirstGroupName] = sampleDataGroup;
            
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Adds an item to the list when the app bar button is clicked.
        /// </summary>
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string groupName = this.pivot.SelectedIndex == 0 ? FirstGroupName : SecondGroupName;
            var group = this.DefaultViewModel[groupName] as SampleDataGroup;
            var nextItemId = group.Items.Count + 1;
            var newItem = new SampleDataItem(
                string.Format(CultureInfo.InvariantCulture, "Group-{0}-Item-{1}", this.pivot.SelectedIndex + 1, nextItemId),
                string.Format(CultureInfo.CurrentCulture, this.resourceLoader.GetString("NewItemTitle"), nextItemId),
                string.Empty,
                string.Empty,
                this.resourceLoader.GetString("NewItemDescription"),
                string.Empty);

            group.Items.Add(newItem);

            // Scroll the new item into view.
            var container = this.pivot.ContainerFromIndex(this.pivot.SelectedIndex) as ContentControl;
            var listView = container.ContentTemplateRoot as ListView;
            listView.ScrollIntoView(newItem, ScrollIntoViewAlignment.Leading);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
                
            }
        }

        /// <summary>
        /// Loads the content for the second pivot item when it is scrolled into view.
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-2");
        }

        

        async private void instagram()
        {
            try
            {
                var client = new HttpClient(); // Add: using System.Net.Http;
                var response = await client.GetAsync(new Uri("https://api.instagram.com/v1/users/1643042045/media/recent?access_token=1643042045.1fb234f.766aca2f4e09402681cc847431071725"));
                var result = await response.Content.ReadAsStringAsync();
                JsonValue jsonList = JsonValue.Parse(result); // Add: using Windows.Data.Json;
                JsonArray message = jsonList.GetObject().GetNamedArray("data");
                var image1 = message.GetObjectAt(0).GetNamedObject("images").GetNamedObject("standard_resolution").GetNamedString("url");
                var image2 = message.GetObjectAt(1).GetNamedObject("images").GetNamedObject("standard_resolution").GetNamedString("url");
                var image3 = message.GetObjectAt(2).GetNamedObject("images").GetNamedObject("standard_resolution").GetNamedString("url");
                var temp = image1;
                ImageSource InstgramImage1 = new BitmapImage(new Uri(image1));
                ImageSource InstgramImage2 = new BitmapImage(new Uri(image2));
                ImageSource InstgramImage3 = new BitmapImage(new Uri(image3));
                instafeed1.Source = InstgramImage1;
                instafeed2.Source = InstgramImage2;
                instafeed3.Source = InstgramImage3;
            }
            catch { }
            
        }

        private static DateTime TimeFromUnixTimestamp(int unixTimestamp)
        {
            DateTime unixYear0 = new DateTime(1970, 1, 1);
            long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
            DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
            return dtUnix;
        }

       public DateTime UnixTimestampToDateTime(double _UnixTimeStamp)
       {
           return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(_UnixTimeStamp);
       }

        
        async private void twitter()
        {
            try
            {
                var client = new HttpClient(); // Add: using System.Net.Http;
                var feedburnerUrl = "http://twitrss.me/twitter_user_to_rss/?user=ohmynilo";
                var response = await client.GetAsync(new Uri("http://ajax.googleapis.com/ajax/services/feed/load?v=1.0&output=json&num=999&q=" + Uri.EscapeUriString(feedburnerUrl)));
                var result = await response.Content.ReadAsStringAsync();
                JsonValue jsonList = JsonValue.Parse(result); // Add: using Windows.Data.Json;
                JsonObject message = jsonList.GetObject().GetNamedObject("responseData").GetObject().GetNamedObject("feed");
                var tweet1 = message.GetObject().GetNamedArray("entries").GetObjectAt(0).GetNamedString("content");
                var tweet2 = message.GetObject().GetNamedArray("entries").GetObjectAt(1).GetNamedString("content");
                var tweet3 = message.GetObject().GetNamedArray("entries").GetObjectAt(2).GetNamedString("content");

                var tweetdate1 = message.GetObject().GetNamedArray("entries").GetObjectAt(0).GetNamedString("publishedDate");
                var tweetdate2 = message.GetObject().GetNamedArray("entries").GetObjectAt(1).GetNamedString("publishedDate");
                var tweetdate3 = message.GetObject().GetNamedArray("entries").GetObjectAt(2).GetNamedString("publishedDate");
                twitterfeed1.Text = tweet1;
                twitterfeed2.Text = tweet2;
                twitterfeed3.Text = tweet3;

                twitterfeeddate1.Text = "Posted Last " + DateTime.Parse(tweetdate1);
                twitterfeeddate2.Text = "Posted Last " + DateTime.Parse(tweetdate2);
                twitterfeeddate3.Text = "Posted Last " + DateTime.Parse(tweetdate3);
            }
            catch
            {

            }
        }

        public async Task<string> GetAccessToken()
        {

            string OAuthConsumerKey = "jK6qjzcQkhrF62O6uVW8EJKci";
            string OAuthConsumerSecret = "EAG29qiz0yP8LO4CZs1mpRCHZcjeWeEffThJeGaimJERN1l14s";
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
            var customerInfo = Convert.ToBase64String(new UTF8Encoding()
                                      .GetBytes(OAuthConsumerKey + ":" + OAuthConsumerSecret));
            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
                                                                      "application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();
            JsonValue jsonList = JsonValue.Parse(result);

            //dynamic item = JsonConvert.DeserializeObject(json);
            //JsonObject message = jsonList.GetObject().GetNamedObject("access_token");
            string message = jsonList.GetObject().GetNamedString("access_token");
            var token = message.ToString();
            //dynamic item = serializer.Deserialize<object>(json);
            return token;
        }


        public async void GetTweets()
        {
            try
            {
                string accessToken = null;
                if (accessToken == null)
                {
                    accessToken = await GetAccessToken();
                }
                var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get,
                    string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?user_id=2915716304&screen_name=delacruz_jayvee"));
                requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
                var httpClient = new HttpClient();
                HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
                //dynamic serializer = JsonConvert.DeserializeObject;
                var result3 = await responseUserTimeLine.Content.ReadAsStringAsync();
                //dynamic d = JsonConvert.DeserializeObject(result3);
                JsonValue jsonList = JsonValue.Parse(result3);
                JsonArray jsonArray = jsonList.GetArray();
                JsonObject jsonObject1 = jsonArray[0].GetObject();
                string content1 = jsonObject1.GetNamedString("text");
                string date1 = jsonObject1.GetNamedString("created_at");

                JsonObject jsonObject2 = jsonArray[1].GetObject();
                string content2 = jsonObject2.GetNamedString("text");
                string date2 = jsonObject2.GetNamedString("created_at");

                JsonObject jsonObject3 = jsonArray[2].GetObject();
                string content3 = jsonObject3.GetNamedString("text");
                string date3 = jsonObject3.GetNamedString("created_at");
                twitterfeed1.Text = content1;
                twitterfeed2.Text = content2;
                twitterfeed3.Text = content3;

                twitterfeeddate1.Text = date1;
                twitterfeeddate2.Text = date2;
                twitterfeeddate3.Text = date3;
                //twitterfeeddate2.Text = tweet2;
                //twitterfeeddate3.Text = tweet3;
            }

            catch
            {

            }
        }
        

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private void Refresh_Click(object sender, TappedRoutedEventArgs e)
        {
            facebook();
            instagram();
            twitter();
        }

        private void RefreshFeeds(object sender, TappedRoutedEventArgs e)
        {
            facebook();
            instagram();
            twitter();
        }

        private void loginbutton1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBoxName.Text = "";
            loginbutton.Content = "Sign In";
            GridChat.Visibility = Visibility.Collapsed;
            GridRegister.Visibility = Visibility.Visible;

        }

        private void TextBoxMessage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    //TextBoxMessage.Text = "Xin chào - Hello " + ((TextBox)sender).Text + "!";
                    ButtonContent_Click_1(sender, e);
                    TextBoxMessage.Text = "";
                    break;
                default:
                    break;
            }
        }
    }
}
