using Noobsenger.MAUI.Pages;

namespace Noobsenger.MAUI
{
    public partial class App : Application
    
    {
        public static Core.Ultra.UltraServer UltraServer = new();
        public static Core.Ultra.UltraClient UltraClient = new();
        public new static App Current => (App)Application.Current;
        public UltraChatPage UltraChatPage { get; set; }
        public App()
        {
            InitializeComponent();
            UltraChatPage = new UltraChatPage();
            MainPage = new Pages.MainPage();
        }
    }
}