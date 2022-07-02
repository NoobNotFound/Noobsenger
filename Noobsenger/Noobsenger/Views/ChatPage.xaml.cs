using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using NoobsengerLib;
using Noobsenger.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Noobsenger.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        public Client Client { get; set; }
        public static ObservableCollection<Message> Messages { get; set; }
        public ChatPage()
        {
            this.InitializeComponent();
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
                Client.ServerNameChanged += Client_ServerNameChanged;
            }
            txtIP.Text = Server.IP.ToString();
            txtPort.Text = Server.Port.ToString();
            Client.ChatRecieved += Client_ChatRecieved;
            ChatView.ItemsSource = Messages;
            txtMessage.Focus(FocusState.Programmatic);
        }

        private void Client_ChatRecieved(object sender, ByteData e)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                if (e.DataType == DataType.Chat)
                {
                    Messages.Add(new MessageItem { Avatar = await AvatarUtil.AvatarToBitmap(e.Avatar), From = e.ClientName, Message = e.Message, Time = DateTime.Now });
                }
                else if (e.DataType == DataType.InfoMessage)
                {
                    if (e.InfoCode == InfoCodes.Join)
                    {
                        Messages.Add(new InfoItem { Info = e.Message, Time = DateTime.Now });
                    }
                }
            });
        }

        private void Client_ServerNameChanged(Client sender, EventArgs args)
        {

            DispatcherQueue.TryEnqueue(() => txtServerName.Text = Client.ServerName);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Login";
            dialog.PrimaryButtonText = "Login";
            dialog.SecondaryButtonText = "Cancel";
            dialog.PrimaryButtonClick += Login;
            dialog.SecondaryButtonClick += delegate { Application.Current.Exit(); };
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new Login();

            var result = await dialog.ShowAsync();

        }

        private void Login(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Client.Connect(Server.IP, Server.Port, Views.Login.UserName, Views.Login.Avatar);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMessage.Text.Replace("\n", "").Replace(" ", "")))
            {
                Client.SendMessage(new ByteData(Client.UserName, txtMessage.Text, Client.Avatar, dataType: DataType.Chat));
                txtMessage.Text = "";
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

        private void btnCopyIP_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Server.IP.ToString());
            Clipboard.SetContent(dataPackage);
        }

        private void btnCopyPort_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Server.Port.ToString());
            Clipboard.SetContent(dataPackage);
        }
    }
}
