using Noobsenger.Core;
using Noobsenger.MAUI.Helpers;

namespace Noobsenger.MAUI.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		cmbxAvatar.ItemsSource = Enum.GetValues<Avatars>();
		cmbxAvatar.SelectedIndex = 0;
		img.Source = "boy.png";
	}

	private void cmbxAvatar_SelectedIndexChanged(object sender, EventArgs e) =>
		img.Source = ((Avatars)cmbxAvatar.SelectedItem).ToImageName();
    
}