﻿using Microsoft.UI.Xaml;
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
using Noobsenger.Core;
using Noobsenger.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Net;
using Noobsenger.Core.Interfaces;
using Noobsenger.Core.Ultra.DataManager;
using Windows.UI.Notifications;
using Windows.Storage;
using System.Globalization;
using NoobSharp.Common.WinUI;
using NoobSharp.Common.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Noobsenger.Core.Ultra;
using System.ComponentModel;
using NoobSharp.Common.WinUI.Helpers;


namespace Noobsenger.Views
{
    public sealed partial class ChatPage : Page, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        internal void Set<T>(ref T obj, T value, string name = null)
        {
            obj = value;
            InvokePropertyChanged(name);
        }
        public void InvokePropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool _ShowThink;
        public bool ShowThink
        {
            get => _ShowThink;
            set => Set(ref _ShowThink, value);
        }
        private string _ThinkText;
        public string ThinkText
        {
            get => _ThinkText;
            set => Set(ref _ThinkText, value);
        }
        public IClient Client { get; set; }
        public ObservableCollection<Message> Messages { get; set; } = new();
        public ObservableCollection<NoobSharp.Common.WinUI.Helpers.Tenor.JSON.SearchResult.Result> ImageUploads { get; set; } = new();
        public bool IsUltra { get; set; }
        public ChatPage()
        {
            IsUltra = false;
            this.InitializeComponent();
        }
        public ChatPage(Core.Ultra.ChannelClient channelClient)
        {
            IsUltra = true;
            this.Client = channelClient;
            TenorFlyout.ItemInvoked += TenorFlyout_ItemInvoked;
            Client.NameChanged += Client_ServerNameChanged;
            Client_ServerNameChanged(null, null);
            Client.ChatRecieved += Client_ChatRecieved;
            (Client as ChannelClient).ThinkingGuys.CollectionChanged += ThinkingGuys_Changed;
            this.InitializeComponent();
        }

        private void ThinkingGuys_Changed(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ThinkText = string.Join(", ", (Client as ChannelClient).ThinkingGuys.Select(x => x.Nme)) + ((Client as ChannelClient).ThinkingGuys.Count == 1 ? " is thinking..." : " are thinking...");
                ShowThink = (Client as ChannelClient).ThinkingGuys.Any();

            });
        }

        int msgCount = 0;
        private void Client_ChatRecieved(object sender, IData e)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                if (e.DataType == DataType.Chat)
                {
                    if (e.GUID == Client.GUID.ToString())
                    {
                        Messages.Add(new MessageItem { GUID = Guid.Parse(e.GUID), Avatar = await AvatarUtil.AvatarToBitmap(e.Avatar), From = e.ClientName, Message = e.Message, Time = DateTime.Now, Sender = MessageSender.Me, Count = e.Count });
                    }
                    else
                    {
                        Messages.Add(MessageItem.Create(new MessageItem { GUID = Guid.Parse(e.GUID), Avatar = await AvatarUtil.AvatarToBitmap(e.Avatar), From = e.ClientName, Message = e.Message, Time = DateTime.Now, Sender = MessageSender.Other, Count = e.Count }, Messages));
                        Notify(e.ClientName, e.Message, AvatarUtil.AvatarToPathString(e.Avatar));
                    }
                }
                else if (e.DataType == DataType.InfoMessage)
                {
                    if (e.InfoCode == InfoCodes.Join)
                    {
                        Messages.Add(new InfoItem { Info = e.Message, Time = DateTime.Now, Count = msgCount });
                    } 
                    else if(e.InfoCode == InfoCodes.MessageDelete)
                    {
                        try
                        {
                            Messages.Remove(Messages.First(x => (x is MessageItem m && m.Message == e.Message) && x.GUID.ToString() == e.GUID && x.Count == e.Count));
                        }
                        catch { }
                    }else if (e.InfoCode == InfoCodes.ImgFromWeb)
                    {
                        Messages.Add(MessageItem.Create(new MessageItem { Avatar = await AvatarUtil.AvatarToBitmap(e.Avatar), From = e.ClientName, Message = e.Message, Time = DateTime.Now, Sender = MessageSender.Other, Count = msgCount }, Messages));
                    }
                    else if(e.InfoCode == InfoCodes.Left)
                    {
                        Messages.Add(new InfoItem { Info = e.Message, Time = DateTime.Now, Count = msgCount });
                    }
                }
                msgCount++;
            });
        }
        private async void Notify(string sender,string message,string avatarUri)
        {
            try
            {
                var f = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///MessageToast.txt"));
                var xml = (await FileIO.ReadTextAsync(f)).Replace("{Sender}", sender).Replace("{Message}", message).Replace("{AvatarUri}", avatarUri);
                /* var toastContent = new ToastContent()
                {
                    Visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = sender
                    },
                    new AdaptiveText()
                    {
                        Text = message
                    }
                },
                            AppLogoOverride = new ToastGenericAppLogo()
                            {
                                Source = avatarUri,
                                HintCrop = ToastGenericAppLogoCrop.Circle
                            }
                        }
                    },
                    Actions = new ToastActionsCustom()
                };
                */

                // Create the toast notification
                var doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc.LoadXml(xml);
                var toastNotif = new ToastNotification(doc)
                {
                    ExpirationTime = DateTime.Now.AddSeconds(7),
                    ExpiresOnReboot = true,
                    Group = txtServerName.Text
                };
                // And send the notification
                ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
            }
            catch { }
        }
        private void Client_ServerNameChanged(object sender, EventArgs args)
        {
            if (!IsUltra)
            {
                DispatcherQueue.TryEnqueue(() => txtServerName.Text = ((Client)sender).ServerName);
            }
            else
            {
                if (!App.UltraServer.IsHosted)
                {
                    DispatcherQueue.TryEnqueue(() => txtServerName.Text = ((Core.Ultra.ChannelClient)Client).ChannelName);
                }
            }
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsUltra)
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
            else
            {
                DispatcherQueue.TryEnqueue(() => txtServerName.Text = ((Core.Ultra.ChannelClient)Client).ChannelName);
                ChatView.ItemsSource = Messages;
                txtPort.Text = ((Core.Ultra.ChannelClient)Client).Port + "";
                txtIP.Text = ((Core.Ultra.ChannelClient)Client).IP.ToString();
                txtServerName.IsReadOnly = !App.UltraServer.IsHosted;
            }

        }

        private void Login(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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
                Client.NameChanged += Client_ServerNameChanged;
            }
            txtIP.Text = Server.IP.ToString();
            txtPort.Text = Server.Port.ToString();
            Client.ChatRecieved += Client_ChatRecieved;
            ChatView.ItemsSource = Messages;
            txtMessage.Focus(FocusState.Programmatic);
            Client.Connect(Server.IP, Server.Port, Views.Login.UserName, Views.Login.Avatar,Guid.Empty);
        }

        int MEmsgCount = 0;
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var gifs = ImageUploads.Any() ? "\n\n"  + string.Join('\n', ImageUploads.Select(x => $"![GIF]({x.media_formats.tinygif.url})")) : "";
            ImageUploads.Clear();
            if (!IsUltra)
            {
                if (!string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    this.IsEnabled = false;
                    await Client.SendMessage(new Data(Client.UserName, txtMessage.Text + gifs, Client.Avatar, count: MEmsgCount, dataType: DataType.Chat));
                    txtMessage.Text = "";
                    this.IsEnabled = true;
                    txtMessage.Focus(FocusState.Programmatic);
                    MEmsgCount++;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    this.IsEnabled = false;
                    await Client.SendMessage(new Data(Client.UserName, txtMessage.Text + gifs, Client.Avatar, count: MEmsgCount, dataType: DataType.Chat));
                    txtMessage.Text = "";
                    this.IsEnabled = true;
                    txtMessage.Focus(FocusState.Programmatic);
                    MEmsgCount++;
                }
            }
            txtMessage.Text = "";
        }

        private async void txtServerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsUltra)
            {
                string txt = txtServerName.Text;
                await Task.Delay(new TimeSpan(0, 0, 2));
                if (txt != txtServerName.Text)
                    return;
                
                try
                {
                    if (!string.IsNullOrWhiteSpace(txtServerName.Text))
                    {
                        Server.ServerName = txtServerName.Text;
                    }
                }
                catch { }
            }
            else
            {
                if (App.UltraServer.IsHosted)
                {
                    try
                    {
                        foreach (var item in App.UltraServer.Channels)
                        {
                            if (item.Port == ((Core.Ultra.ChannelClient)Client).Port)
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
                                        item.ChannelName = txtServerName.Text;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
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
            dataPackage.SetText(txtPort.Text);
            Clipboard.SetContent(dataPackage);
        }

        private async void mitDelmsg_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuFlyoutItem mit)
            {
                var d = mit.DataContext as Message;
                if (mit.Text == "Delete for me")
                {
                    Messages.Remove(d);
                }
                else
                {
                    await Client.SendMessage(new Data(Client.UserName, (d as MessageItem).Message, Client.Avatar, count: d.Count, dataType: DataType.InfoMessage,infoCode:InfoCodes.MessageDelete));
                }
            }
        }

        private void mitCopyMsg_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem mit)
            {
                foreach (var item in Messages)
                {
                    if (item.Count == int.Parse(mit.Tag.ToString()))
                    {
                        var dataPackage = new DataPackage();
                        dataPackage.SetText(((MessageItem)item).Message);
                        Clipboard.SetContent(dataPackage);
                        return;
                    }
                }
            }
        }

        private async void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            btnSend_Click(null, null);
            await Task.Delay(10);
            txtMessage.Text = "";
            txtMessage_TextChanged(null, null);
        }

        private TenorFlyout TenorFlyout = new() { APIKey = "AIzaSyARqNY-2kB-gvNvhoPEdTgNa7WTSUT28qc"}; //This is my key please do not steal I'm begging you!

        private void TenorFlyout_ItemInvoked(object sender, NoobSharp.Common.WinUI.Helpers.Tenor.JSON.SearchResult.Result e)
        {
            ImageUploads.Add(e);
        }
        private void btnGif_Click(object sender, RoutedEventArgs e)
        {
            TenorFlyout.ShowAt(btnGif);
        }

        private void KeyboardAccelerator_Invoked_1(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            txtMessage.Text += "\n";
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            ImageUploads.Remove((sender as Button).DataContext as NoobSharp.Common.WinUI.Helpers.Tenor.JSON.SearchResult.Result);
        }

        private void txtMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!txtMessage.Text.IsNullEmptyOrWhiteSpace())
                _ = (Client as ChannelClient).Thinking(true);
            else
                _ = (Client as ChannelClient).Thinking(false);
        }

        private void btnAttach_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
