using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Noobsenger.Core;
using Noobsenger.Mobile.Helpers;
using Noobsenger.Mobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noobsenger.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public Client Client { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        int msgCount = 0;
        public ChatPage()
        {
            InitializeComponent();
            Client = new Client();
            Messages = new ObservableCollection<Message>();
            if (Server.IsHosted)
            {
                txtServerName.IsReadOnly = false;
                txtServerName.Text = Server.ServerName;
            }
            else
            {
                txtServerName.IsReadOnly = true;
                Client.NameChanged += Client_ServerNameChanged; ;
            }
            Client.ChatRecieved += Client_ChatRecieved;
            lvChat.ItemsSource = Messages;
            Client.Connect(Server.IP, Server.Port, LoginPage.UserName, LoginPage.Avatar);

        }

        private void Client_ServerNameChanged(object sender, EventArgs e)
        {
            if (this.Dispatcher.IsInvokeRequired)
            {
                Dispatcher.BeginInvokeOnMainThread(() => txtServerName.Text = ((Client)sender).ServerName);
            }
            else
            {
                txtServerName.Text = ((Client)sender).ServerName;
            }
        }

        private void Client_ChatRecieved(object sender, Core.Interfaces.IData e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (e.DataType == DataType.Chat)
                    {
                        if (e.ClientName == Client.UserName)
                        {
                            Messages.Add(
                                MessageItem.Create(
                                    new MessageItem
                                    {
                                        Avatar = AvatarUtil.AvatarToBitmap(e.Avatar),
                                        From = e.ClientName,
                                        Message = e.Message,
                                        Time = DateTime.Now,
                                        Sender = MessageSender.Me,
                                        Count = msgCount
                                    },
                                Messages));
                        }
                        else
                        {
                            Messages.Add(MessageItem.Create(new MessageItem { Avatar = AvatarUtil.AvatarToBitmap(e.Avatar), From = e.ClientName, Message = e.Message, Time = DateTime.Now, Sender = MessageSender.Other, Count = msgCount }, Messages));
                        }
                    }
                    else if (e.DataType == DataType.InfoMessage)
                    {
                        if (e.InfoCode == InfoCodes.Join)
                        {
                            Messages.Add(new InfoItem { Info = e.Message, Time = DateTime.Now, Count = msgCount });
                        }
                    }
                    msgCount++;
                });
            }
            catch { }
        }

        private async void btnSend_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                this.IsEnabled = false;
                await Client.SendMessage(new ChatData(Client.UserName, txtMessage.Text, Client.Avatar, dataType: DataType.Chat));
                txtMessage.Text = "";
                this.IsEnabled = true;
                txtMessage.Focus();
            }
        }

        private async void txtServerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = txtServerName.Text;
            await Task.Delay(new TimeSpan(0, 0, 2));
            if (txt != txtServerName.Text)
            {
                return;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(txtServerName.Text))
                {
                    Server.ServerName = txtServerName.Text;
                }
            }
            catch { }

        }
    }
}