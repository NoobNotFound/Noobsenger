using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT.Interop;

namespace Noobsenger.Helpers
{
    public static class TittleBarHelper
    {
        public static void SetExtendedTitleBar(Window window, UIElement AppTitleBar)
        {
            FrameworkElement RootUI = (FrameworkElement)window.Content;
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                AppWindow AppWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(window)));
                var titlebar = AppWindow.TitleBar;
                titlebar.ExtendsContentIntoTitleBar = true;
                void SetColor(ElementTheme acualTheme)
                {
                    titlebar.ButtonHoverBackgroundColor = App.LayerFillColorDefaultColor;
                    titlebar.ButtonBackgroundColor = titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    switch (acualTheme)
                    {
                        case ElementTheme.Dark:
                            titlebar.ButtonForegroundColor = Colors.White;
                            titlebar.ButtonHoverForegroundColor = Colors.Silver;
                            titlebar.ButtonPressedForegroundColor = Colors.Silver;
                            break;
                        case ElementTheme.Light:
                            titlebar.ButtonForegroundColor = Colors.Black;
                            titlebar.ButtonHoverForegroundColor = Colors.DarkGray;
                            titlebar.ButtonPressedForegroundColor = Colors.DarkGray;
                            break;
                    }
                }
                RootUI.ActualThemeChanged += (s, _) => SetColor(s.ActualTheme);
                window.SetTitleBar(AppTitleBar);
                SetColor(RootUI.ActualTheme);
            }
            else
            {
                window.ExtendsContentIntoTitleBar = true;
                window.SetTitleBar(AppTitleBar);
            }
        }
    }
}
