using Noobsenger.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noobsenger_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
            Application.Current.MainPage = new WelcomePage();
            return true;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (txtName.Text == null || nbrPort.Text == null)
            {
                txtInfo.Text = "Something went wrong with these info,\nPlease Check again man!";
                txtInfo.IsVisible = true;
                return;
            }

            if (cmbxIps.SelectedItem != null && !string.IsNullOrEmpty(txtName.Text.Replace(" ", "")) && int.Parse( nbrPort.Text) > 1023 && int.Parse(nbrPort.Text) < 49152)
            {
                Server.Host((System.Net.IPAddress)cmbxIps.SelectedItem, Convert.ToInt32(nbrPort.Text), txtName.Text);
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