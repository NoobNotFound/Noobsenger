using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Noobsenger.Core;
namespace Noobsenger.Mobile.Helpers
{
    public class AvatarUtil
    {
        public static ImageSource AvatarToBitmap(AvatarManager.Avatars avatar)
        {
          return  Device.RuntimePlatform == Device.Android
                ? ImageSource.FromFile(avatar.ToString() + ".png")
                : ImageSource.FromFile("Images/" + avatar.ToString() + ".png");
        }
    }
}
