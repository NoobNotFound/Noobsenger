using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noobsenger.Helpers
{
    public class MessageItem : Message
    {
        public string From;
        public BitmapImage Avatar;
        public string Message;
        public DateTime Time;
    }
    public class InfoItem : Message
    {
        public string Info;
        public DateTime Time;
    }
    public interface Message
    {
    }
    public class ChatItemTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate MessageTemplate { get; set; }

        public DataTemplate InfoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
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
                return null;
            }
        }
    }
}