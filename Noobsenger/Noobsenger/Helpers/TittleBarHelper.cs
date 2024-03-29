﻿using Microsoft.UI;
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
    public static class TitleBarHelper
    {
        public static void SetExtendedTitleBar(Window window, UIElement AppTitleBar = null)
        {
            FrameworkElement RootUI = (FrameworkElement)window.Content;
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                AppWindow AppWindow = window.ToAppWindow();
                var titlebar = AppWindow.TitleBar;
                titlebar.ExtendsContentIntoTitleBar = true;
                void SetColor(ElementTheme acualTheme)
                {
                    titlebar.ButtonBackgroundColor = titlebar.ButtonInactiveBackgroundColor = titlebar.ButtonPressedBackgroundColor = Colors.Transparent;
                    switch (acualTheme)
                    {
                        case ElementTheme.Dark:
                            titlebar.ButtonHoverBackgroundColor = Colors.Black;
                            titlebar.ButtonForegroundColor = Colors.White;
                            titlebar.ButtonHoverForegroundColor = Colors.White;
                            titlebar.ButtonPressedForegroundColor = Colors.Silver;
                            break;
                        case ElementTheme.Light:
                            titlebar.ButtonHoverBackgroundColor = Colors.White;
                            titlebar.ButtonForegroundColor = Colors.Black;
                            titlebar.ButtonHoverForegroundColor = Colors.Black;
                            titlebar.ButtonPressedForegroundColor = Colors.DarkGray;
                            break;
                    }
                }
                RootUI.ActualThemeChanged += (s, _) => SetColor(s.ActualTheme);
                if (AppTitleBar != null)
                {
                    window.SetTitleBar(AppTitleBar);
                }
                SetColor(RootUI.ActualTheme);
            }
            else
            {
                window.ExtendsContentIntoTitleBar = true;
                if (AppTitleBar != null)
                {
                    window.SetTitleBar(AppTitleBar);
                }
            }
        }
    }
}
