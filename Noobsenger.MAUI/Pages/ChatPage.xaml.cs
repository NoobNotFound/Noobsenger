using CommunityToolkit.Mvvm.ComponentModel;
using Noobsenger.Core.Ultra;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Noobsenger.MAUI.Pages;

public partial class ChatPage : ContentPage, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;
    internal void Set<T>(ref T obj, T value,[CallerMemberName] string name = null)
    {
        obj = value;
        InvokePropertyChanged(name);
    }
    public void InvokePropertyChanged(string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    private string _Title;
    public string Title
    {
        get => _Title;
        set => Set(ref _Title, value);
    }
    public ChannelClient ChannelClient { get; private set; }
    public string LastMsg { get; set; }
    public ChatPage(ChannelClient channelClient)
	{
		InitializeComponent();
        ChannelClient = channelClient;
        ChannelClient.NameChanged += (_, _) => Title = ChannelClient.ChannelName;
        Title = ChannelClient.ChannelName;
	}
}