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
    public sealed partial class JoinPage : Page
    {
        public JoinPage()
        {
            this.InitializeComponent();
            cmbxIps.ItemsSource = Noobsenger.Core.Util.GetIPAddresses();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Noobsenger.Core.Util.ParseIPAddress(cmbxIps.Text.ToString()) != null && nbrPort.Value > 1023 && nbrPort.Value < 49152)
            {
                Noobsenger.Core.Server.IP = Noobsenger.Core.Util.ParseIPAddress(cmbxIps.Text.ToString());
                Noobsenger.Core.Server.Port = Convert.ToInt32(nbrPort.Value);
                MainWindow.DisableGoBack = true;
                MainWindow.NavigateFrame(typeof(UltraChatPage));
            }
            else
            {
                txtInfo.Text = "Something went wrong with these info,\nPlease check again!";
                txtInfo.Visibility = Visibility.Visible;
            }
        }
    }
}
