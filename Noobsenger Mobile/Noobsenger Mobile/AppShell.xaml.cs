using Noobsenger_Mobile.ViewModels;
using Noobsenger_Mobile.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Noobsenger_Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }
        public async void Navigate()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
