using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noobsenger.Helpers
{
    public enum MessageSender
    {
        Me,
        Other
    }
    public class MessageItem : Message
    {
        public string From;
        public BitmapImage Avatar;
        public string Message;
        public MessageSender Sender;
        public int Count { get; set; }
        public Visibility AvatarVisibility { get
            {
                Visibility or;
                if (Sender == MessageSender.Other)
                {
                    or = Visibility.Visible;
                }
                else
                {
                    or = Visibility.Collapsed;
                }
                return or;
            } }
        public HorizontalAlignment HorizontalAlignment 
        { get
            {
                HorizontalAlignment or;
                if(Sender == MessageSender.Other)
                {
                    or = HorizontalAlignment.Left;
                }
                else
                {
                    or = HorizontalAlignment.Right;
                }
                return or;
            } }
        public Brush Background
        {
            get
            {
                Brush or;
                if (Sender == MessageSender.Other)
                {
                    or = (Brush)App.Current.Resources["LayerFillColorDefaultBrush"];
                }
                else
                {
                    or = (Brush)App.Current.Resources["AccentAAFillColorTertiaryBrush"];
                }
                return or;
            }
        }
        public DateTime Time { get; set; }
    }
    public class InfoItem : Message
    {
        public string Info;
        public DateTime Time { get; set; }
        public int Count { get; set; }
    }
    public interface Message
    {
        public int Count { get; set; }
        public DateTime Time { get; set; }
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