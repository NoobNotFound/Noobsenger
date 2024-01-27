using Noobsenger.Core;
using Noobsenger.MAUI.Helpers;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Noobsenger.MAUI.Pages;

public partial class LoginPage : ContentPage
{
	public static string UserName { get; private set; } = "User";
	public static bool IsHost { get; set; }
	public static Avatars Avatar { get; private set; } = Avatars.Boy;

    public LoginPage()
	{
		InitializeComponent();
		cmbxAvatar.ItemsSource = Enum.GetValues<Avatars>().Where(x=> x!= Avatars.OpenAI).ToList();
		cmbxAvatar.SelectedIndex = 0;
		img.Source = "boy.png";
	}


    private void cmbxAvatar_SelectedIndexChanged(object sender, EventArgs e) =>
		img.Source = ((Avatars)cmbxAvatar.SelectedItem).ToImageName();

    private void Button_Clicked(object sender, EventArgs e)
    {
        UserName = txtUsername.Text;
        Avatar = ((Avatars)cmbxAvatar.SelectedItem);

        if (string.IsNullOrEmpty(UserName) || string.IsNullOrWhiteSpace(UserName)) return;

		App.Current.MainPage = App.Current.UltraChatPage;
    }
}