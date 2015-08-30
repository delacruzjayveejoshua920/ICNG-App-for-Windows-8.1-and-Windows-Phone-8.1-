using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    public sealed class TileUpdater : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var defferal = taskInstance.GetDeferral();

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);

            updater.Clear();

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    var url = "https://graph.facebook.com/1501735393442310/posts?access_token=681471118637012|HARByNx18pD7X-jPskRr4dyxVlg";
                    var client = new HttpClient();
                    var response = await client.GetAsync(url);
                    string result = await response.Content.ReadAsStringAsync();
                    if (result != null)
                    {
                        bool fbflag = true;
                        dynamic d = JsonConvert.DeserializeObject(result);
                        var test = d.data;
                        string feed = test[i].message;
                        string feeddate = test[i].created_time;

                        var tile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText01);
                        tile.GetElementsByTagName("text")[0].InnerText = feeddate;
                        tile.GetElementsByTagName("text")[1].InnerText = feed;

                        updater.Update(new TileNotification(tile));
                    }
                }
                catch
                {

                }
            }

            defferal.Complete();
        }
    }
}
