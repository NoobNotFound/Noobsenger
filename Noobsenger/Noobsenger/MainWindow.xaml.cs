using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Runtime.InteropServices;
using WinRT;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Noobsenger.Helpers;
using Microsoft.UI;
using Noobsenger.Core;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Windowing;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Noobsenger
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static Frame RootMainFrame;

        public static bool DisableGoBack = false;
        public static void NavigateFrame(Type pageType)
        {
            RootMainFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;  // enable custom titlebar
            this.SetTitleBar(AppTitleBar);
            Title = "Noobsenger";
            var res = Application.Current.Resources;
            res["WindowCaptionBackground"] = Colors.Transparent;
            res["WindowCaptionBackgroundDisabled"] = Colors.Transparent;
            TriggerTitleBarRepaint();
            RootMainFrame = MainFrame;
            RootMainFrame.Navigated += RootMainFrame_Navigated;
            RootMainFrame.Navigate(typeof(Views.WelcomePage));
        }

        private void RootMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (RootMainFrame.CanGoBack)
            {
                btnFrameBack.Visibility = Visibility.Visible;
                AppTitleBar.Margin = new Thickness(50, 0, 0, 0);
            }
            else
            {
                btnFrameBack.Visibility = Visibility.Collapsed;
                AppTitleBar.Margin = new Thickness(12, 0, 0, 0);
            }
            if (DisableGoBack)
            {
                btnFrameBack.Visibility = Visibility.Collapsed;
                AppTitleBar.Margin = new Thickness(12, 0, 0, 0);
            }
        }
        public void TriggerTitleBarRepaint()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var activeWindow = Win32.GetActiveWindow();
            if (hwnd == activeWindow)
            {
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_INACTIVE, IntPtr.Zero);
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_ACTIVE, IntPtr.Zero);
            }
            else
            {
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_ACTIVE, IntPtr.Zero);
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_INACTIVE, IntPtr.Zero);
            }


        }

        private void btnFrameBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }
        public async void Notify(string title,string message, InfoBarSeverity Severity)
        {
            infBar.Severity = Severity;
            infBar.Title = title;
            infBar.Message = message;
            infBar.Visibility = Visibility.Visible;
            await System.Threading.Tasks.Task.Delay(new TimeSpan(0, 0, 0, 3));
            infBar.Visibility = Visibility.Collapsed;
        }
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            try
            {
                if (Server.IsHosted)
                {
                    Server.IsRuns = false;
                    Application.Current.Exit();
                }
            }
            catch { }
        }
    }
}
