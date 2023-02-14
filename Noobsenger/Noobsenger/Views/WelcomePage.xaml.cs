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
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Noobsenger.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();
        }
        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
         //new NoobNotFound.WinUI.Common.UI.Controls.TenorFlyout() { APIKey = "AIzaSyARqNY-2kB-gvNvhoPEdTgNa7WTSUT28qc" }.ShowAt(btnHost);
            MainWindow.NavigateFrame(typeof(HostPage));
        }

        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavigateFrame(typeof(JoinPage));
        }
    }
}
