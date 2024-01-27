namespace Noobsenger.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void btnHost_Clicked(object sender, EventArgs e)
        {
            LoginPage.IsHost = true;
            App.Current.MainPage = new HostPage();
        }

        private void btnJoin_Clicked(object sender, EventArgs e)
        {
            LoginPage.IsHost = false;
            App.Current.MainPage = new JoinPage();
        }
    }
}