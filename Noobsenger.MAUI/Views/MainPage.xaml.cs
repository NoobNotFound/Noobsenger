namespace Noobsenger.MAUI.Views
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
            App.Current.MainPage = new HostPage();
        }

        private void btnJoin_Clicked(object sender, EventArgs e)
        {

        }
    }
}