using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Noobsenger.Helpers
{
    public static class Extentions
    {
        public static AppWindow ToAppWindow(this Window window) =>
            AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(window)));
        public static bool IsNullEmptyOrWhiteSpace(this string str) =>
            string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}
