using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Noobsenger.Core;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Noobsenger.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public static string UserName;
        public static Avatars Avatar;
        public Login()
        {
            this.InitializeComponent();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserName = txtUsername.Text;
        }

        private void FlipAvatars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Avatar = ((AvatarWithBitmap)FlipAvatars.SelectedItem).Avatar;
            }
            catch
            {

            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var avatars = Util.GetEnumList<Avatars>();


            List<AvatarWithBitmap> avatarsList = new List<AvatarWithBitmap>();
            foreach (var item in avatars)
            {
                avatarsList.Add(new AvatarWithBitmap(item, await Helpers.AvatarUtil.AvatarToBitmap(item)));
            }
            FlipAvatars.ItemsSource = avatarsList;
            FlipAvatars.SelectedIndex = 0;

        }
    }
    public class AvatarWithBitmap
    {
        public Avatars Avatar { get; set; }
        public BitmapImage Image { get; set; }

        public AvatarWithBitmap(Avatars avatar, BitmapImage image)
        {
            Avatar = avatar;
            Image = image;
        }
    }
}
