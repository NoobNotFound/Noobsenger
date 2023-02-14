using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noobsenger.Mobile.Models
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void Set<T>(ref T obj,T value,string propertyName = null)
        {
            obj = value;
            this.PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MenuFlyoutItem : Model
    {
        public MenuFlyoutItem(int id)
        {
            ID = id;
        }
        public int ID { get; private set; }
        private string _Title;
        public string Title { get => _Title; set => Set(ref _Title, value); }

        private bool _IsSelected;
        public bool IsSelected { get => _IsSelected; set => Set(ref _IsSelected, value); }
    }
}