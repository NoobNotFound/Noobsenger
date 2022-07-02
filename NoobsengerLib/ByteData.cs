using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace NoobsengerLib
{
    [Serializable()]
    public class ByteData
    {
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string InfoCode { get; set; }
        public AvatarManager.Avatars Avatar { get; set; }
        public Uri[] Uploads { get; set; }
        public DataType DataType { get; set; }
        public ByteData(string clientName = "", string message = "", AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, DataType dataType = DataType.Chat, string infoCode = null)
        {
            ClientName = clientName;
            Message = message;
            Avatar = avatar;
            Uploads = uploads;
            DataType = dataType;
            InfoCode = infoCode;
        }
    }

    public class ByteDataString
    {
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string InfoCode { get; set; }
        public string Avatar { get; set; }
        public string[] Uploads { get; set; }
        public string DataType { get; set; }
    }
    public enum DataType
    {
        Chat,
        Image,
        InfoMessage
    }
    public static class DataEncoder
    {
        public static byte[] DataToByteArray(ByteData data)
        {
            ByteDataString byteData = new ByteDataString();
            byteData.Avatar = data.Avatar.ToString();
            byteData.Message = data.Message.ToString();
            byteData.ClientName = data.ClientName;
            byteData.DataType = data.DataType.ToString();
            byteData.InfoCode = data.InfoCode;
            if (data.Uploads != null)
            {
                List<string> uploads = new List<string>();
                foreach (var item in data.Uploads)
                {
                    uploads.Add(item.OriginalString);
                }
                byteData.Uploads = uploads.ToArray();
            }
            var dataString = JsonConvert.SerializeObject(byteData);
            byte[] barr = Encoding.Default.GetBytes(dataString);
            return barr;
        }

        // Convert a byte array to an Object
        public static ByteData ByteArrayToData(byte[] arrBytes)
        {
            string str = Encoding.Default.GetString(arrBytes);
            ByteDataString r = JsonConvert.DeserializeObject<ByteDataString>(str);
            ByteData data = new ByteData(r.ClientName,r.Message, (AvatarManager.Avatars)Enum.Parse(typeof(AvatarManager.Avatars),r.Avatar),dataType: (DataType)Enum.Parse(typeof(DataType),r.DataType),infoCode:r.InfoCode);
            if(data.Uploads != null)
            {

                List<Uri> uploads = new List<Uri>();
                foreach (var item in r.Uploads)
                {
                    uploads.Add(new Uri(item));
                }
                data.Uploads = uploads.ToArray();
            }
            return data;
        }
    }
}
