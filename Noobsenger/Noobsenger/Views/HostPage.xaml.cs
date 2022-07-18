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
using Noobsenger.Core;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Noobsenger.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HostPage : Page
    {
        public HostPage()
        {
            this.InitializeComponent();
            cmbxIps.ItemsSource = Util.GetIPAddresses();
            cmbxIps.SelectedIndex = 0;
            nbrPort.Value = new Random().Next(1024, 49151);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbxIps.SelectedItem != null && !string.IsNullOrEmpty(txtName.Text.Replace(" ", "")) && nbrPort.Value > 1023 && nbrPort.Value < 49152)
            {
                Server.Host((System.Net.IPAddress)cmbxIps.SelectedItem, Convert.ToInt32(nbrPort.Value), txtName.Text);
                MainWindow.DisableGoBack = true;
                MainWindow.NavigateFrame(typeof(ChatPage));
            }
            else
            {
                txtInfo.Text = "Something went wrong with these info,\nPlease Check again man!";
                txtInfo.Visibility = Visibility.Visible;
            }

        }
    }
}
