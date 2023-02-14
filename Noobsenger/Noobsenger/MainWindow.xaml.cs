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
using WinRT.Interop;
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
            Title = "Noobsenger";

            TitleBarHelper.SetExtendedTitleBar(this);
            AppTitleBar.Loaded += (_,_)=> SetDragRegionForCustomTitleBar(this.ToAppWindow());
            AppTitleBar.SizeChanged += (_,_)=> SetDragRegionForCustomTitleBar(this.ToAppWindow());
            RootMainFrame = MainFrame;
            RootMainFrame.Navigated += RootMainFrame_Navigated;
            RootMainFrame.Navigate(typeof(Views.WelcomePage));
        }

        private void RootMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (RootMainFrame.CanGoBack && !DisableGoBack)
            {
                btnFrameBack.Visibility = Visibility.Visible;
                AppTitleBar.Margin = new Thickness(5, 0, 0, 0);
                SetDragRegionForCustomTitleBar(this.ToAppWindow());
            }
            else
            {
                btnFrameBack.Visibility = Visibility.Collapsed;
                AppTitleBar.Margin = new Thickness(12, 0, 0, 0);
                SetDragRegionForCustomTitleBar(this.ToAppWindow());
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

        }
        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }
        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }

            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }
        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported()
                && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();


                List<Windows.Graphics.RectInt32> dragRectsList = new();
                 

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((btnFrameBack.Visibility == Visibility.Visible ? 45 : 0) * scaleAdjustment);
                dragRectR.Y = 0;
                dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                dragRectR.Width = (int)((AppTitleBar.Margin.Left + AppTitleBar.Margin.Right + AppTitleBar.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }
    }
}
