using Noobsenger.Core;
using Noobsenger_Mobile.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noobsenger_Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new WelcomePage();
        }
        protected override void OnStart()
        {
        }
        protected override void OnSleep()
        {
        }
        protected override void OnResume()
        {
            if(MainPage is ContentPage pg)
            {
                pg.DisplayAlert("Whoops", "You have left the app.You should't do it next time. Ok ?", "Ok");
                this.Quit();
            }
        }
    }
}
