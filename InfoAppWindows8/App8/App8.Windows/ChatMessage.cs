using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.MobileServices;

namespace App8
{
   
        public class ChatMessage
        {
            public int Id
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

            [DataMember(Name = "channel")]
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


            [DataMember(Name = "uri")]
            public string Uri { get; set; }


        }

        public sealed partial class MainPage : Page
        {
            private string name = string.Empty;

            private IMobileServiceTable<ChatMessage> chatTable = App.MobileService.GetTable<ChatMessage>();

            StringBuilder messages = null;

            public MainPage()
            {
                this.InitializeComponent();
                App.Messages.CollectionChanged += Messages_CollectionChanged;
                messages = new StringBuilder();
            }

            void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        var chatMessage = item as ChatMessage;
                        App.Messages.Remove(chatMessage);
                        if (chatMessage.From != name)
                        {
                            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                TextBoxChatMessages.Text += string.Format("{0}\n", chatMessage.ToString());
                            });
                        }
                    }
                }
            }


            private void ButtonLetsGo_Click_1(object sender, RoutedEventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(TextBoxName.Text))
                {
                    name = TextBoxName.Text.Trim();
                    GridRegister.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    GridChat.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ReadyForNextMessage();
                }
            }

            private void ButtonContent_Click_1(object sender, RoutedEventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(TextBoxMessage.Text))
                {
                    var chatMessage = new ChatMessage()
                    {
                        From = name,
                        Content = TextBoxMessage.Text.Trim(),
                        DateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        Channel = App.CurrentChannel.Uri
                    };
                    InsertChatMessage(chatMessage);
                }
                ReadyForNextMessage();
            }

            private async void InsertChatMessage(ChatMessage message)
            {
                TextBoxChatMessages.Text += string.Format("{0}\n", message.ToString());
                await chatTable.InsertAsync(message);
                ReadyForNextMessage();
            }

            private void ReadyForNextMessage()
            {
                TextBoxMessage.Text = "";
                TextBoxMessage.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }
    }
}
