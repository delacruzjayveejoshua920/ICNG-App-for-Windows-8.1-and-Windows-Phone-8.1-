using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Newtonsoft.Json;
using CustomLiveTiles;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Text;
using Windows.Data.Xml.Dom;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.System.Threading;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Windows.Web.Syndication;
using Windows.UI.Notifications;
using Windows.ApplicationModel.Background;
using App8.Common;
using App8.Helpers;
using App8.ViewModels;
using App8.Views;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Office365.OAuth;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using Windows.UI.Popups;

namespace App8
{
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

        bool _found = true;
        public bool _Deleted
        {
            get
            {
                return this._found;
            }
            set
            {
                // Can only be called in this class.
                this._found = true;
            }
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

        public DateTime created_At
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
            return string.Format("[{0}] {1} Says: {2}", DateTime, From, Content); //
        }   
     }

   
    public class Channel
    {
        public int Id { get; set; }
        public string Uri { get; set; }
    }


    public sealed partial class MainPage : Page
    {
        
        private string name = string.Empty;
        private IMobileServiceTable<ChatMessage> chatTable = MobileService.GetTable<ChatMessage>();
        StringBuilder messages = null;
        public bool fbflag = false;
        public string myemail; //
        public string result;
        public string result1;
        public string result2;
        public static ObservableCollection<ChatMessage> iMessages = null;
        private const string TASK_NAME = "TileUpdater";
        private const string TASK_ENTRY = "BackgroundTasks.TileUpdater";
        private NavigationHelper navigationHelper;
        
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState( out int Description, int ReservedValue ) ;
        //Creating a function that uses the API function...
        public static bool IsConnectedToInternet( ){
            int Desc ;
            return InternetGetConnectedState( out Desc, 0 ) ;
        }


        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        
        public MainPage()
        {
                this.InitializeComponent();
                messages = new StringBuilder();
                this.navigationHelper = new NavigationHelper(this);
                this.navigationHelper.LoadState += navigationHelper_LoadState;
                this.DataContext = App.CurrentUser;
                iMessages = new ObservableCollection<ChatMessage>();
                iMessages.CollectionChanged += Messages_CollectionChanged;
                getfacebookstream();
                GetTweets();
                loadInstaFeed();
                //readMessage();
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == TASK_NAME)
                        task.Value.Unregister(true);
                }

                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = TASK_NAME;
                builder.TaskEntryPoint = TASK_ENTRY;
                builder.SetTrigger(new TimeTrigger(15, false));
                var registration = builder.Register();
            }

        }


        private async void getfacebookstream()
        {
            try
            {
                var url = "https://graph.facebook.com/1501735393442310/posts?access_token=681471118637012|HARByNx18pD7X-jPskRr4dyxVlg";
                var client = new HttpClient();
                var response = await client.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                if (result != null)
                {
                    fbflag = true;
                    dynamic d = JsonConvert.DeserializeObject(result);
                    var test = d.data;
                    string feed = test[0].message;
                    string feed1 = test[1].message;
                    string feed2 = test[2].message;
                    string date = test[0].created_time;
                    string date1 = test[1].created_time;
                    string date2 = test[2].created_time;

                    WideTileImage tile = (WideTileImage)this.FindName("facebook");
                    WideTileImage tile1 = (WideTileImage)this.FindName("facebook1");
                    WideTileImage tile2 = (WideTileImage)this.FindName("facebook2");

                    facebookpost1.Text = feed;
                    facebookpostdate1.Text = date;

                    facebookpost2.Text = feed1;
                    facebookpostdate2.Text = date1;

                    facebookpost3.Text = feed2;
                    facebookpostdate3.Text = date2;

                    tile.Title = feed1;
                    tile1.Title = feed;
                    tile2.Title = feed2;
                }
            }
            catch{ }
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

            string json = await response.Content.ReadAsStringAsync();
            dynamic item = JsonConvert.DeserializeObject(json);
            return item["access_token"];
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
                    string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?user_id=2970941137&screen_name=ICNGOfficial"));
                requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
                var httpClient = new HttpClient();
                HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
                //dynamic serializer = JsonConvert.DeserializeObject;
                var result3 = await responseUserTimeLine.Content.ReadAsStringAsync();
                dynamic d = JsonConvert.DeserializeObject(result3);
                string tweet = d[0].text;
                string tweet2 = d[1].text;
                string tweet3 = d[2].text;
                string tweetdate = d[0].created_at;
                string tweetdate2 = d[1].created_at;
                string tweetdate3 = d[2].created_at;

                twitterpost1.Text = tweet;
                twitterpostdate1.Text = tweetdate;

                twitterpost2.Text = tweet2;
                twitterpostdate2.Text = tweetdate2;

                twitterpost3.Text = tweet3;
                twitterpostdate3.Text = tweetdate3;

                WideTileImage tile = (WideTileImage)this.FindName("twitter");
                WideTileImage tile2 = (WideTileImage)this.FindName("twitter2");
                WideTileImage tile3 = (WideTileImage)this.FindName("twitter3");
                tile.Title = tweet;
                tile2.Title = tweet2;
                tile3.Title = tweet3;
            }

            catch
            {

            }
        }

        public void disableloginbutton()
        {
            loginbutton.IsEnabled = false;

        }

        public void enableloginbutton()
        {
            loginbutton.IsEnabled = true;

        }

        

        private async void loadInstaFeed()
        {
            try
            {
                //var feedUrl = "https://api.instagram.com/v1/users/1348022056/media/recent?//access_token=1348022056.1fb234f.6260dd14613c4fb1aac2af3d7e9053d8";

                var feedUrl = "https://api.instagram.com/v1/users/1643042045/media/recent?access_token=1643042045.1fb234f.766aca2f4e09402681cc847431071725";
                var client = new HttpClient();
                var response = await client.GetAsync(feedUrl);
                result1 = await response.Content.ReadAsStringAsync();
                dynamic d = JsonConvert.DeserializeObject(result1);
                var test = d.data;

                var Instafeed1 = test[0].images.standard_resolution.url;
                string temp = Instafeed1;
                ImageSource _imageLocation = new BitmapImage(new Uri(temp));
                LargeTileImage tile = (LargeTileImage)this.FindName("instagram1");
                instagramcontainer1.ImageSource = _imageLocation;
                instagrampic1.Source = _imageLocation;
                if (test[0].caption != null)
                {
                    if (test[0].caption != null)
                    {
                        string instaCaption1 = test[0].caption.text;
                        instagampost1.Text = instaCaption1;
                    }
                }
                
                System.DateTime dateTime1 = new System.DateTime(1970, 1, 1, 8, 0, 0, 0);
                Double time1 = test[0].created_time;
                DateTime printDate = dateTime1.ToLocalTime();
                dateTime1 = dateTime1.AddSeconds(time1);
                string datetime1 = dateTime1.ToString();
                instagrampostdate1.Text = datetime1;



                var Instafeed2 = test[1].images.standard_resolution.url;
                string temp1 = Instafeed2;
                ImageSource _imageLocation1 = new BitmapImage(new Uri(temp1));
                LargeTileImage tile1 = (LargeTileImage)this.FindName("instagram2");
                //tile1.BackgroundImage = _imageLocation1;
                instagramcontainer2.ImageSource = _imageLocation1;
                instagrampic2.Source = _imageLocation1;
                if (test[1].caption != null)
                {
                    if (test[1].caption != null)
                    {
                        string instaCaption2 = test[1].caption.text;
                        instagampost2.Text = instaCaption2;
                    }
                }


                System.DateTime dateTime2 = new System.DateTime(1970, 1, 1, 8, 0, 0, 0);
                Double time2 = test[1].created_time;
                DateTime printDate2 = dateTime2.ToLocalTime();
                dateTime2 = dateTime2.AddSeconds(time2);
                string datetime2 = dateTime2.ToString();
                instagrampostdate2.Text = datetime2;



                var Instafeed3 = test[2].images.standard_resolution.url;
                string temp2 = Instafeed3;
                ImageSource _imageLocation2 = new BitmapImage(new Uri(temp2));
                LargeTileImage tile3 = (LargeTileImage)this.FindName("instagram3");
                //tile3.BackgroundImage = _imageLocation2;
                instagramcontainer3.ImageSource = _imageLocation2;
                instagrampic3.Source = _imageLocation2;
                if (test[2].caption != null)
                {
                    if (test[2].caption != null)
                    {
                        string instaCaption3 = test[2].caption.text;
                        instagampost3.Text = instaCaption3;
                    }
                }

                System.DateTime dateTime3 = new System.DateTime(1970, 1, 1, 8, 0, 0, 0);
                Double time3 = test[2].created_time;
                DateTime printDate3 = dateTime3.ToLocalTime();
                dateTime3 = dateTime3.AddSeconds(time3);
                string datetime3 = dateTime2.ToString();
                instagrampostdate3.Text = datetime3;
            }
            catch
            {

            }
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
            //var user = new UserViewModel();
            //name = user._DisplayName;
            //name = user.DisplayName;
            //if (!string.IsNullOrWhiteSpace(name))
            UserViewModel model = new UserViewModel();
            string mail = myemail;
            //string email = model;
             if (!string.IsNullOrWhiteSpace(TextBoxName.Text) && TextBoxName.Text != "(not connected, Please Sign In)")
            {
                 bool isInternetConnected = IsConnectedToInternet();
                 if (isInternetConnected == true)
                 {
                     name = TextBoxName.Text.Trim();
                     GridRegister.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                     GridChat.Visibility = Windows.UI.Xaml.Visibility.Visible;
                     ReadyForNextMessage();
                     
                     //readMessage(); 
                     Timer();
                 }
                 else
                 {
                    // TextBoxName.Text = "No Internet Connection";
                     var messageDialog = new MessageDialog("No internet connection has been found.");
                     
                 }
            }
        }

        public void CollapsedChatRoom()
        {
            GridChat.Visibility = Visibility.Collapsed;
            GridRegister.Visibility = Visibility.Collapsed;
            TextBoxChatMessages.Text = "";
        }

        private void ButtonContent_Click_1(object sender, RoutedEventArgs e)
        {
            string[] badWords = new[] { "fuck", "f u c k","sex", "s e x", "bobo", "b o b o", "f.u.c.k", "s.e.x", "b.o.b.o",
            "puta", "tanga", "p u t a", "t a n g a", "p.u.t.a", "t.a.n.g.a", "tangina", "t.a.n.g.i.n.a", "putangina",
            "p.u.t.a.n.g.i.n.a"};
            string input = TextBoxMessage.Text;
            string output = Filter(input, badWords);
            string date = DateTime.Now.ToString();
            string temp = date;
            if (!string.IsNullOrWhiteSpace(output))
            {
                var chatMessage = new ChatMessage()
                {
                    From = name,
                    Content = output.Trim(),
                    //DateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime = DateTime.Now.ToString()
                };
                InsertChatMessage(chatMessage);
            }
            ReadyForNextMessage();
            TextBoxMessage.Text = "";
            //ticktoDisable();
            disable(); 
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
            //TextBoxMessage.Text = "";
            TextBoxMessage.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        public static MobileServiceClient MobileService  = new MobileServiceClient(
            "https://informaticsng.azure-mobile.net/",
            "uUTkMiFVMrLfXJkedIVvlamrMHPKUl43"
        );

        public string email(string email)
        {
          
                return email;
        }

        
        
        public async void readMessage()
        {

            try
            {
                //IMobileServiceTable<ChatMessage> todoTable = MobileService.GetTable<ChatMessage>();
                //IMobileServiceTableQuery<ChatMessage> query = todoTable.Skip(50).Take(50);
                //List<ChatMessage> items = await query.ToListAsync();
                //var temp = items;
                //query = query.IncludeTotalCount();

                //var items = await chatTable.ToCollectionAsync();

                //var myTable = MobileService.GetTable<ChatMessage>();
                // var myList = await myTable.Take(500).Where(datetime).ToListAsync();

                var items = await chatTable.ToCollectionAsync();
                var temp = items;


                for (var i = 0; i < items.Count; i++)
                {
                    var oldChat = new ChatMessage();
                    oldChat.Id = items[i].Id;
                    oldChat.created_At = items[i].created_At;
                    oldChat.From = items[i].From;
                    oldChat._Deleted = items[i]._Deleted;
                    //oldChat._deleted = items[i]._deleted;
                    oldChat.DateTime = items[i].DateTime;
                    oldChat.Content = items[i].Content;
                    TextBoxChatMessages.Text += Environment.NewLine + oldChat.ToString();
                    //JObject jo = new JObject();
                    //jo.Add("Id", items[i].Id.ToString());
                    ////var c = items.Count;
                    //await chatTable.DeleteAsync(jo);

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
             //   TextBoxChatMessages.Text = "";
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

        private void BSIT(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BSIT));
        }

        private void BSCS(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BSCS));
        }

        private void BSBA(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BSBA));
        }

        private void ADGAT(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ADGAT));
        }

        private void SchoolActivities(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SchoolActivities));
        }

        private void AboutUs(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Aboutus));
        }

        private void Gallery(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Gallery));
        }

        private void Refresh_Click(object sender, TappedRoutedEventArgs e)
        {
            getfacebookstream();
            loadInstaFeed();
            GetTweets();
        }

        async Task loadchat()
        {
            try
            {
                var item = await chatTable.ToCollectionAsync();
                //linemessage(item);
                int i;
                for (i = 0; i < item.Count; i++)
                {
                    var oldChat = new ChatMessage();
                    oldChat.From = item[i].From;
                    oldChat.DateTime = item[i].DateTime;
                    oldChat.Content = item[i].Content;
                    TextBoxChatMessages.Text += oldChat.ToString() + Environment.NewLine;
                }
                i = 0;
            }
            catch
            {

            }
        }



        private void linemessage(MobileServiceCollection<ChatMessage, ChatMessage> items)
        {
            int i;
            for (i = 0; i < items.Count; i++)
            {
                var oldChat = new ChatMessage();
                oldChat.From = items[i].From;
                oldChat.DateTime = items[i].DateTime;
                oldChat.Content = items[i].Content;
                TextBoxChatMessages.Text += Environment.NewLine + oldChat.ToString();
            }
            i = 0;
        }

        public void Timer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += ticker;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            
        }
        DispatcherTimer disabletimer = new DispatcherTimer();
        //DispatcherTimer disabletimer = new DispatcherTimer();
        public void disable()
        {
            
            disabletimer.Tick += ticktoDisable;
            disabletimer.Interval = new TimeSpan(0, 0, 1);
            disabletimer.Start();
            //disabletimer.Stop();
        }
        int time = 0;
        int timectr = 0;

        int timed = 0;
        int timectrd = 0;

        public  async void ticktoDisable(object sender, object e)
        {
            time++;
            timectrd++;

            ButtonContent.IsEnabled = false;

            if (timectr % 7 == 0)
            {

                //bool IsEnabled = true;
                ButtonContent.IsEnabled = true;
                //int t = await Task.Run(() => EnabledButtonSend());
                disabletimer.Stop();
                
            }
        }

        public int EnabledButtonSend()
        {
            int o = 2;
            bool e = true;
            
            
            return o;
           
        }

    
        

        public async void ticker(object sender, object e)
        {
            time++;
            timectr++;

            if (timectr % 15 == 0)
            {
                //TextBoxChatMessages.Text = "";
                
                await loadchat();
            }
        }

        private void S(object sender, PointerRoutedEventArgs e)
        {

        }

        private void facebookcontainer1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //parent.IsEnabled = false;
            container.IsEnabled = false;
            this.Opacity = .4;
            
            facebookpopup1.IsOpen = true;
            pop.Width = Window.Current.Bounds.Width;
        }

        private void facebookcontainer2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            facebookpopup2.IsOpen = true;
            pop2.Width = Window.Current.Bounds.Width;
        }

        private void facebookcontainer3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            facebookpopup3.IsOpen = true;
            pop3.Width = Window.Current.Bounds.Width;
        }

        private void BackButtonMain(object sender, TappedRoutedEventArgs e)
        {
            facebookpopup1.IsOpen = false;
            facebookpopup2.IsOpen = false;
            facebookpopup3.IsOpen = false;
            twitterpopup1.IsOpen = false;
            twitterpopup2.IsOpen = false;
            twitterpopup3.IsOpen = false;
            instagrampopup1.IsOpen = false;
            instagrampopup2.IsOpen = false;
            instagrampopup3.IsOpen = false;
            this.Opacity = 1.0;
            container.IsEnabled = true;
        }

        private void twittercontainer1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4; 
            twitterpopup1.IsOpen = true;
            pop4.Width = Window.Current.Bounds.Width;
        }

        private void twittercontainer2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            twitterpopup2.IsOpen = true;
            pop5.Width = Window.Current.Bounds.Width;
        }

        private void twittercontainer3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            twitterpopup3.IsOpen = true;
            pop6.Width = Window.Current.Bounds.Width;
        }

        private void instaParentContainer1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            instagrampopup1.IsOpen = true;
            pop7.Width = Window.Current.Bounds.Width;
        }

        private void instaParentContainer2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            instagrampopup2.IsOpen = true;
            pop8.Width = Window.Current.Bounds.Width;
        }

        private void instaParentContainer3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            container.IsEnabled = false;
            this.Opacity = .4;
            instagrampopup3.IsOpen = true;
            pop9.Width = Window.Current.Bounds.Width;
        }

        private void TextBoxMessage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (ButtonContent.IsEnabled == true)
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

        private void loginbutton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((string)loginbutton.Content.ToString() == "Sign Out")
            {
                TextBoxChatMessages.Text = "";
                GridChat.Visibility = Visibility.Collapsed;
                GridRegister.Visibility = Visibility.Visible;
            }
        }
    }
}
