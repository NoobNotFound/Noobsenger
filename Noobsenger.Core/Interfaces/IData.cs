using System;
using System.Collections.Generic;
using System.Text;

namespace Noobsenger.Core.Interfaces
{
    public interface IData
    {
        int Count { get; set; }
        string ClientName { get; set; }
        string Message { get; set; }
        string InfoCode { get; set; }
        string GUID { get; set; }
        Avatars Avatar { get; set; }
        Uri[] Uploads { get; set; }
        DataType DataType { get; set; }
        byte[] Files { get; set; }
    }
}
