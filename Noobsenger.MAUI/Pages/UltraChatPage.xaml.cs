using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Noobsenger.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Noobsenger.MAUI.Pages;

public partial class UltraChatPage : ContentPage
{
	public ObservableCollection<ChatPage> ChatPages { get; set; } = new();
	public UltraChatPage()
	{
		InitializeComponent();
        
        App.UltraClient = new();
        App.UltraClient.NameChanged += UltraClient_NameChanged;
        App.UltraClient.ChannelAdded += UltraClient_ChannelAdded;
        App.UltraClient.ChannelRemoved += UltraClient_ChannelRemoved;
        App.UltraClient.ChatRecieved += UltraClient_ChatRecieved;
        App.UltraClient.ServerClosed += UltraClient_ServerClosed;
    }
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        bool loginsuccess = false;
        while (!loginsuccess)
        {

            try
            {
                App.UltraClient.Connect(Server.IP, Server.Port, LoginPage.UserName, LoginPage.Avatar, Guid.NewGuid());
                loginsuccess = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
            }
        }
        lvChats.ItemsSource = ChatPages;
    }

    private void UltraClient_ServerClosed(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void UltraClient_ChatRecieved(object? sender, Core.Interfaces.IData e)
    {
        throw new NotImplementedException();
    }

    private void UltraClient_ChannelRemoved(object? sender, int e)
    {
        App.Current.Dispatcher.Dispatch(() =>
        {
            if (App.Current.MainPage == ChatPages.First(x => x.ChannelClient.Port == e))
                App.Current.MainPage = App.Current.UltraChatPage;
            ChatPages.Remove(ChatPages.First(x => x.ChannelClient.Port == e));
        });
    }

    private void UltraClient_ChannelAdded(object? sender, int e)
    {
        App.Current.Dispatcher.Dispatch(() =>
        {
            ChatPages.Add(new ChatPage(App.UltraClient.Channels[e]));
        });
    }

    private void UltraClient_NameChanged(object? sender, EventArgs e)
    {
        App.Current.Dispatcher.Dispatch(() =>
        {
            ServerName.Text = App.UltraClient.ServerName;
        });
    }

    private void ViewCell_Tapped(object sender, EventArgs e)
    {

    }

    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NewChannelPage();
    }

    private void btnInfo_Clicked(object sender, EventArgs e)
    {
    }

    public void RequestNewChannel(string name, bool quickGPT)
    {
        bool loginsuccess = false;
        while (!loginsuccess)
        {

            try
            {
                App.UltraServer.AddChannel(name, quickGPT);
                loginsuccess = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
            }
        }
    }
}