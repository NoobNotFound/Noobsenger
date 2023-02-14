using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using WinRT.Interop;

namespace Noobsenger.Helpers
{
    public static class IconHelper
    {
        public static void SetIcon(Window window)
        {
            /*
            var icon = User32.LoadImage(
                hInst: IntPtr.Zero,
                name: $@"{Package.Current.InstalledLocation.Path}\Images\iconCopy.png.ico".ToCharArray(),
                type: User32.ImageType.IMAGE_ICON,
                cx: 0,
                cy: 0,
                fuLoad: User32.LoadImageFlags.LR_LOADFROMFILE | User32.LoadImageFlags.LR_DEFAULTSIZE | User32.LoadImageFlags.LR_SHARED
            );
            var Handle = WindowNative.GetWindowHandle(window);
            User32.SendMessage(Handle, User32.WindowMessage.WM_SETICON, (IntPtr)1, icon);
            User32.SendMessage(Handle, User32.WindowMessage.WM_SETICON, (IntPtr)0, icon);
            */
        }
    }
}
