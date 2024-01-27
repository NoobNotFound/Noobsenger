using System.Net;
using System.Xml.Linq;
using Noobsenger.Core;
using Noobsenger.Core.Ultra;
using Noobsenger.MAUI.Helpers;

namespace Noobsenger.MAUI.Pages;

public partial class JoinPage : ContentPage
{
	public JoinPage()
    {
        InitializeComponent();
        nbrPort.Text = "" + new Random().Next(1024, 49151);
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.MainPage = new MainPage();
        return true;
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        if (nbrPort.Text == null)
        {
            txtInfo.Text = "Something went wrong with these info,\nPlease Check again!";
            txtInfo.IsVisible = true;
            return;
        }

        if (IPAddress.TryParse(txtIP.Text, out var ip) && int.Parse(nbrPort.Text) > 1023 && int.Parse(nbrPort.Text) < 49152)
        {
            Server.IP = ip;
            Server.Port = Convert.ToInt32(nbrPort.Text);
            
            try
            {
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