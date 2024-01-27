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
using Noobsenger.Core.Ultra;
using Noobsenger.Core;
using Noobsenger.Core.Interfaces;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Noobsenger.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UltraChatPage : Page
    {
        public ObservableCollection<ChatPage> ChannelPages = new ObservableCollection<ChatPage>();
        public UltraChatPage()
        {
            this.InitializeComponent();
            txtIP.Text = Core.Server.IP.ToString();
            txtPort.Text = Core.Server.Port.ToString();
            App.UltraClient = new();
            App.UltraClient.NameChanged += UltraClient_NameChanged;
            App.UltraClient.ChannelAdded += UltraClient_ChannelAdded;
            App.UltraClient.ChannelRemoved += UltraClient_ChannelRemoved;
            App.UltraClient.ChatRecieved += UltraClient_ChatRecieved;
            App.UltraClient.ServerClosed += UltraClient_ServerClosed;
        }

        private void UltraClient_ServerClosed(object? sender, EventArgs e)
        {
            this.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    ChatFrame.IsEnabled = false;
                    _=new ContentDialog()
                    {
                        XamlRoot = ((MainWindow)App.MainWindow).Content.XamlRoot,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        Title = "This Server was closed by the owner",
                        PrimaryButtonText = "Ok",
                        DefaultButton = ContentDialogButton.Primary
                    }.ShowAsync();
                }
                catch { }
            });
        }

        private void UltraClient_ChatRecieved(object sender, IData e)
        {
        }

        private void UltraClient_ChannelRemoved(object sender, int e)
        {
            this.DispatcherQueue.TryEnqueue(() =>
            {
                foreach (var item in navView.MenuItems)
                {
                    if (item is NavigationViewItem itm)
                    {
                        if (Convert.ToInt32(itm.Tag) == e)
                        {
                            navView.MenuItems.Remove(item);
                        }
                    }
                }
            });
        }

        private void UltraClient_ChannelAdded(object sender, int e)
        {
            this.DispatcherQueue.TryEnqueue(() =>
            {
            ChatPage pg = new();
                var itm = new NavigationViewItem
                {
                    Icon = new FontIcon { Glyph = "\xe8f2" }
                };
                foreach (var item in App.UltraClient.Channels)
            {
                if (item.Port == e)
                {
                    pg = new ChatPage(item);
                    itm.Content = item.ChannelName;
                    itm.Tag = item.Port;
                    item.NameChanged += (sender, e) => { this.DispatcherQueue.TryEnqueue(() => itm.Content = item.ChannelName); };
                     this.DispatcherQueue.TryEnqueue(() => itm.Content = item.ChannelName);
                    }
                }
                ChannelPages.Add(pg);
                navView.MenuItems.Add(itm);
            });
        }

        private void UltraClient_NameChanged(object sender, EventArgs e)
        {
            
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(!App.UltraServer.IsHosted)
                btnMitAddChannel.Visibility = Visibility.Collapsed;

            ContentDialog dialog = new()
            {
                XamlRoot = ((MainWindow)App.MainWindow).Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Login",
                PrimaryButtonText = "Login",
                SecondaryButtonText = "Cancel"
            };
            dialog.PrimaryButtonClick += Login;
            dialog.SecondaryButtonClick += delegate { Application.Current.Exit(); };
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new Login();

           await dialog.ShowAsync();
        }

        private void Login(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                App.UltraClient.Connect(Server.IP, Server.Port, Views.Login.UserName, Views.Login.Avatar,Guid.NewGuid());
            }
            catch(Exception ex)
            {
                ((MainWindow)App.MainWindow).Notify("Errror", ex.Message, InfoBarSeverity.Error);
                MainWindow.DisableGoBack = false;
                MainWindow.NavigateFrame(typeof(JoinPage));
            }
            
        }

        private void btnAddChannel_Click(object sender, RoutedEventArgs e)
        {
            addNewFly.Hide();
            if(App.UltraServer.IsHosted)
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtAddNewServer.Text))
                    {
                        App.UltraServer.AddChannel(txtAddNewServer.Text, chkGPT.IsChecked.Value);
                        chkGPT.IsChecked = false;
                    }
                    else
                    {
                        ((MainWindow)App.MainWindow).Notify("Errror", "Enter a correct name.", InfoBarSeverity.Error);
                    }
                }
                catch (Exception ex)
                {
                    ((MainWindow)App.MainWindow).Notify("Errror", ex.Message, InfoBarSeverity.Error);
                }
            }
        }

        private void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!args.IsSettingsInvoked)
            {
                if (navView.SelectedItem is NavigationViewItem itm)
                {
                    foreach (var item in ChannelPages)
                    {
                        if (((ChannelClient)item.Client).Port == Convert.ToInt32(itm.Tag))
                        {
                            ChatFrame.Content = item;
                        }
                    }
                }
            }
        }

        private void btnMitAddChannel_Click(object sender, RoutedEventArgs e)
        {
            txtAddNewServer.Text = "";
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
