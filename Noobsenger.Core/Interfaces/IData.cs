﻿namespace Noobsenger.Core.Interfaces
{
    public interface IData
    {
        int Count { get; set; }
        string ClientName { get; set; }
        string Message { get; set; }
        string InfoCode { get; set; }
        AvatarManager.Avatars Avatar { get; set; }
        Uri[] Uploads { get; set; }
        DataType DataType { get; set; }
    }
}
