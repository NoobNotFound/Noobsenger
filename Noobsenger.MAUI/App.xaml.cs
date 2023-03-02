namespace Noobsenger.MAUI
{
    public partial class App : Application
    
    {
        public static Core.Ultra.UltraServer UltraServer = new();
        public static Core.Ultra.UltraClient UltraClient = new();
        public App()
        {
            InitializeComponent();

            MainPage = new Views.MainPage();
        }
    }
}