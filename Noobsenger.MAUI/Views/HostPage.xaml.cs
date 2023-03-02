using Noobsenger.Core;
using Noobsenger.Core.Ultra;

namespace Noobsenger.MAUI.Views;

public partial class HostPage : ContentPage
{
    public HostPage()
    {
        InitializeComponent();
        cmbxIps.ItemsSource = Util.GetIPAddresses();
        cmbxIps.SelectedIndex = 0;
        nbrPort.Text = "" + new Random().Next(1024, 49151);
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.MainPage = new MainPage();
        return true;
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        if (txtName.Text == null || nbrPort.Text == null)
        {
            txtInfo.Text = "Something went wrong with these info,\nPlease Check again!";
            txtInfo.IsVisible = true;
            return;
        }

        if (cmbxIps.SelectedItem != null && !string.IsNullOrEmpty(txtName.Text.Replace(" ", "")) && int.Parse(nbrPort.Text) > 1023 && int.Parse(nbrPort.Text) < 49152)
        {
            App.UltraServer.Host((System.Net.IPAddress)cmbxIps.SelectedItem, Convert.ToInt32(nbrPort.Text), txtName.Text);
            Application.Current.MainPage = new LoginPage();
        }
        else
        {
            txtInfo.Text = "Something went wrong with these info,\nPlease Check again!";
            txtInfo.IsVisible = true;
        }
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Application.Current.MainPage = new MainPage();
    }

}