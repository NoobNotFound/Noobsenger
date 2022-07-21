using Noobsenger_Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Noobsenger_Mobile.Helpers
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
}
