using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noobsenger.Core.Ultra.DataManager
{
    public static class DataEncoder
    {
        public static byte[] ToBytes(this Data data)
        {
            DataString byteData = new()
            {
                Avatar = data.Avatar.ToString(),
                Message = data.Message.ToString(),
                ClientName = data.ClientName,
                DataType = data.DataType.ToString(),
                InfoCode = data.InfoCode
            };
            if (data.Uploads != null)
            {
                List<string> uploads = new();
                foreach (var item in data.Uploads)
                {
                    uploads.Add(item.OriginalString);
                }
                byteData.Uploads = uploads.ToArray();
            }
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(byteData));
        }

        // Convert a byte array to an Object
        public static Data ToData(this byte[] arrBytes)
        {
            string str = Encoding.UTF8.GetString(arrBytes);
            DataString r = JsonConvert.DeserializeObject<DataString>(str);
            Data data = new(r.ClientName, r.Message, (Avatars)Enum.Parse(typeof(Avatars), r.Avatar), dataType: (DataType)Enum.Parse(typeof(DataType), r.DataType), infoCode: r.InfoCode);
            if (data.Uploads != null)
            {

                List<Uri> uploads = new();
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
