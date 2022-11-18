using Noobsenger.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Noobsenger.Mobile.Helpers
{
    public class ChatItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageTemplate { get; set; }

        public DataTemplate InfoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // Return the correct data template based on the item's type.
            if (item.GetType() == typeof(MessageItem))
            {
                return MessageTemplate;
            }
            else if (item.GetType() == typeof(InfoItem))
            {
                return InfoTemplate;
            }
            else
            {
                return InfoTemplate;
            }
        }
    }
    public class BoolToObjectConverter<T> : IValueConverter
    {
        public T TrueObject { set; get; }

        public T FalseObject { set; get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueObject : FalseObject;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((T)value).Equals(TrueObject);
        }
    }
}
