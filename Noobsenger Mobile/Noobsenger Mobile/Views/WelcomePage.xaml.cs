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
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void btnJoin_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new JoinPage();
        }

        private void btnHost_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new HostPage();
        }
    }
}