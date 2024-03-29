﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Noobsenger.Core.Ultra.DataManager
{
    public class Data : Interfaces.IData
    {
        public int Count { get; set; }
        public string ClientName { get; set; }
        public string GUID { get; set; }
        public string Message { get; set; }
        public string InfoCode { get; set; }
        public Avatars Avatar { get; set; }
        public Uri[] Uploads { get; set; }
        public DataType DataType { get; set; }
        public byte[] Files { get; set; }
        public Data(string clientName = "", string message = "", Avatars avatar = Avatars.Gamer, Uri[] uploads = null, DataType dataType = DataType.Chat, string infoCode = null,byte[] Files = null, int count = 0, string gUID = null)
        {
            ClientName = clientName;
            Message = message;
            Avatar = avatar;
            Uploads = uploads;
            DataType = dataType;
            InfoCode = infoCode;
            this.Files = Files;
            Count = count;
            GUID = gUID;    
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
        public string GUID { get; set; }
        public string Count { get; set; }
        public string Files { get; set; }
    }

}