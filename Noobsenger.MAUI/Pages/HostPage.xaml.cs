using Noobsenger.Core;
using Noobsenger.Core.Ultra;

namespace Noobsenger.MAUI.Pages;

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
            Server.IP = (System.Net.IPAddress)cmbxIps.SelectedItem;
            Server.Port = Convert.ToInt32(nbrPort.Text);
            try
            {
                App.UltraServer.Host((System.Net.IPAddress)cmbxIps.SelectedItem, Convert.ToInt32(nbrPort.Text), txtName.Text);
                Application.Current.MainPage = new LoginPage();
            }
            catch (Exception ex)
            {
                txtError.Text = ex.Message;
                txtError.IsVisible = true;
            }
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