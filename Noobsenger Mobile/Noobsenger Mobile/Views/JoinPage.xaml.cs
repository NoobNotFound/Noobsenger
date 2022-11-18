using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noobsenger.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JoinPage : ContentPage
    {
        public JoinPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            Application.Current.MainPage = new WelcomePage();
            return true;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (txtIp.Text == null || nbrPort.Text == null)
            {
                txtInfo.Text = "Something went wrong with these info,\nPlease Check again man!";
                txtInfo.IsVisible = true;
                return;
            }
            if (Noobsenger.Core.Util.ParseIPAddress(txtIp.Text.ToString()) != null && int.Parse(nbrPort.Text) > 1023 && int.Parse(nbrPort.Text) < 49152)
            {
                Noobsenger.Core.Server.IP = Noobsenger.Core.Util.ParseIPAddress(txtIp.Text.ToString());
                Noobsenger.Core.Server.Port = Convert.ToInt32(nbrPort.Text);
                Application.Current.MainPage = new LoginPage();
            }
            else
            {
                txtInfo.Text = "Something went wrong with these info,\nPlease Check again man!";
                txtInfo.IsVisible = true;
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Application.Current.MainPage = new WelcomePage();
        }
    }
}