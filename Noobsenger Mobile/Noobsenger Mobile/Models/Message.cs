using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Noobsenger_Mobile.Models
{
    public enum MessageSender
    {
        Me,
        Other,
        OtherAgain
    }
    public class MessageItem : Message
    {
        internal static Message Create(Message msg,ObservableCollection<Message> messages)
        {
            var lastMsg = messages[messages.Count - 1];
            if (lastMsg is MessageItem itm && msg is MessageItem mitm)
            {
                if(itm.From == mitm.From)
                {
                    if (itm.Sender != MessageSender.Me)
                    {
                        mitm.Sender = MessageSender.OtherAgain;
                    }
                    return mitm;
                }
            }
            return msg;
        }
        public string TimeString { get { return Time.ToShortTimeString(); } }
        public string From { get; set; }
        public ImageSource Avatar { get; set; }
        public string Message { get; set; }
        public MessageSender Sender { get; set; }
        public int Count { get; set; }
        public bool AvatarVisibility
        {
            get
            {
                if (Sender == MessageSender.Other)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public LayoutOptions HorizontalAlignment
        {
            get
            {
                LayoutOptions or;
                if (Sender == MessageSender.Other || Sender == MessageSender.OtherAgain)
                {
                    or = LayoutOptions.Start;
                }
                else
                {
                    or = LayoutOptions.End;
                }
                return or;
            }
        }
        public Color Background
        {
            get
            {
                Color or;
                if (Sender == MessageSender.Other || Sender == MessageSender.OtherAgain)
                {
                    or = (Color)App.Current.Resources["SecondaryColor"];
                }
                else
                {
                    or = (Color)App.Current.Resources["PrimaryColor"];
                }
                return or;
            }
        }
        public DateTime Time { get; set; }
    }
    public class InfoItem : Message
    {
        public string TimeString { get { return Time.ToShortTimeString(); } }
        public string Info { get; set; }
        public DateTime Time { get; set; }
        public int Count { get; set; }
    }
    public interface Message
    {
        int Count { get; set; }
        DateTime Time { get; set; }
    }
}