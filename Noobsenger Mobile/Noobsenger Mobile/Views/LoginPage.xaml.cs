using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noobsenger.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noobsenger.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public static string UserName;
        public static AvatarManager.Avatars Avatar;
        public LoginPage()
        {
            this.InitializeComponent();

            var avatars = Util.GetEnumList<AvatarManager.Avatars>();

            FlipAvatars.ItemsSource = avatars;
            FlipAvatars.SelectedIndex = 0;
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserName = txtUsername.Text;
        }

        private void FlipAvatars_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Avatar = ((AvatarManager.Avatars)FlipAvatars.SelectedItem);
            }
            catch { }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || FlipAvatars.SelectedItem == null)
            {
                txtInfo.Text = "Something went wrong with these info,\nPlease Check again man!";
                txtInfo.IsVisible = true;
            }
            else
            {
                Application.Current.MainPage = new Ultra.UltraChat();
            }
        }
    }
}