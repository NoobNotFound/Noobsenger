using System;
using System.Collections.Generic;
using System.Text;

namespace Noobsenger.Core.Ultra.DataManager
{
    public class Data : Interfaces.IData
    {
        public int Count { get; set; }
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string InfoCode { get; set; }
        public AvatarManager.Avatars Avatar { get; set; }
        public Uri[] Uploads { get; set; }
        public DataType DataType { get; set; }
        public object[] Objects { get; set; }
        public Data(string clientName = "", string message = "", AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, DataType dataType = DataType.Chat, string infoCode = null,object[] objects = null, int count = 0)
        {
            ClientName = clientName;
            Message = message;
            Avatar = avatar;
            Uploads = uploads;
            DataType = dataType;
            InfoCode = infoCode;
            Objects = objects;
            Count = count;
        }
    }
    internal class DataString
    {
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string InfoCode { get; set; }
        public string Avatar { get; set; }
        public string[] Uploads { get; set; }
        public string DataType { get; set; }
    }

}
