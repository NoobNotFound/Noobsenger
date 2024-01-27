using Noobsenger.MAUI.Helpers;

namespace Noobsenger.MAUI.Pages;

public partial class NewChannelPage : ContentPage
{
	public NewChannelPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {

        if (txtName.Text.IsNullEmptyOrWhiteSpace())
        {
            txtInfo.Text = "Please provide a name!";
            txtInfo.IsVisible = true;
            return;
        }
        App.Current.MainPage = App.Current.UltraChatPage;
        App.Current.UltraChatPage.RequestNewChannel(txtName.Text, GPTswitch.IsToggled);

    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        App.Current.MainPage = App.Current.UltraChatPage;
    }
}