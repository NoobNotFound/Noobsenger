using Noobsenger.Core;
using Noobsenger.Mobile.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noobsenger.Mobile
{
    public partial class App : Application
    {
        public static Core.Ultra.UltraServer UltraServer = new Core.Ultra.UltraServer();
        public static Core.Ultra.UltraClient UltraClient = new Core.Ultra.UltraClient();

        public App()
        {
            InitializeComponent();
            MainPage = new Views.Ultra.UltraChat();
        }
        protected override void OnStart()
        {
        }
        protected override void OnSleep()
        {
        }
        protected override async void OnResume()
        {
            if(MainPage is ContentPage pg)
            {
                await pg.DisplayAlert("Whoops", "You have left the app.You should't do it next time. Ok ?", "Ok");
            }
        }
    }
}
