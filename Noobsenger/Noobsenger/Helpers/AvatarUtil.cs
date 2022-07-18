using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using static Noobsenger.Core.AvatarManager;

namespace Noobsenger.Helpers
{
    public static class AvatarUtil
    {
        public static async Task<BitmapImage> AvatarToBitmap(Avatars avatar)
        {
            var f = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Images/Avatars/" + avatar.ToString() + ".png"));
            IRandomAccessStream stream = await f.OpenAsync(FileAccessMode.Read);
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(stream);
            return bmp;
        }
    }
}
