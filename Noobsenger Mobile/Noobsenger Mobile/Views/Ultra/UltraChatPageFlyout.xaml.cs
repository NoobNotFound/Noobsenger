using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

namespace Noobsenger.Mobile.Views.Ultra
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UltraChatPageFlyout : ContentPage
    {
        public event NavigationFlyoutItemInvokedHandler? ItemInvoked;
        public delegate void NavigationFlyoutItemInvokedHandler(UltraChatPageFlyout sender, Models.MenuFlyoutItem item);
        public readonly ObservableCollection<Models.MenuFlyoutItem> MenuItems = new ObservableCollection<Models.MenuFlyoutItem>();
        private string _HeaderButtonText;
       /*
        public string HeaderButtonText
        {
            get => _HeaderButtonText;
            set
            {
                _HeaderButtonText = value;
                UpdateHeader();
            }
        }
        private void UpdateHeader()
        {
            Header.View.IsVisible = HeaderButtonText != null;
            txtHeader.Text = HeaderButtonText ?? "";
        }
       */
        public UltraChatPageFlyout()
        {
            InitializeComponent();
            MenuItemsListView.ItemsSource = MenuItems;
            MenuItems.Add(new Models.MenuFlyoutItem(1) { Title = "ww 0" });
            MenuItems.Add(new Models.MenuFlyoutItem(2) { Title = "ww 1" });
            MenuItems.Add(new Models.MenuFlyoutItem(3) { Title = "ww 2" });
            MenuItems.Add(new Models.MenuFlyoutItem(4) { Title = "ww 3" });
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            var itm = (sender as ViewCell);
            if (itm != null)
            {
                var mitm = MenuItems.Where(x => x.ID == ((Models.MenuFlyoutItem)itm.BindingContext).ID).FirstOrDefault();
                if (mitm != null)
                {
                    ItemInvoked?.Invoke(this, mitm);
                    foreach (var item in MenuItems)
                    {
                        item.IsSelected = item.ID == mitm.ID;
                    }
                }
            }
        }

        private void HeaderViewCell_Tapped(object sender, EventArgs e)
        {

        }
    }
}