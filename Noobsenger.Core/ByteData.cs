using Newtonsoft.Json;
using System.Text;
namespace Noobsenger.Core
{
    public class ChatData : Interfaces.IData
    {
        public int Count { get; set; }
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string InfoCode { get; set; }
        public AvatarManager.Avatars Avatar { get; set; }
        public Uri[] Uploads { get; set; }
        public DataType DataType { get; set; }
        public ChatData(string clientName = "", string message = "", AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, DataType dataType = DataType.Chat, string infoCode = null, int count = 0)
        {
            ClientName = clientName;
            Message = message;
            Avatar = avatar;
            Uploads = uploads;
            DataType = dataType;
            InfoCode = infoCode;
            Count = count;
        }
    }

    public class ChatDataString
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
        public static byte[] DataToByteArray(ChatData data)
        {
            ChatDataString byteData = new ChatDataString();
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
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(byteData));
        }

        // Convert a byte array to an Object
        public static ChatData ByteArrayToData(byte[] arrBytes)
        {
            string str = Encoding.UTF8.GetString(arrBytes);
            ChatDataString r = JsonConvert.DeserializeObject<ChatDataString>(str);
            ChatData data = new ChatData(r.ClientName, r.Message, (AvatarManager.Avatars)Enum.Parse(typeof(AvatarManager.Avatars), r.Avatar), dataType: (DataType)Enum.Parse(typeof(DataType), r.DataType), infoCode: r.InfoCode);
            if (data.Uploads != null)
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
