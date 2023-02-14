using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Noobsenger.Core.Interfaces
{
    public interface IClient
    {
        event EventHandler<IData> ChatRecieved;
        event EventHandler NameChanged;
        string UserName { get; set; }
        TcpClient clientSocket { get; set; }
        AvatarManager.Avatars Avatar { get; set; }
        void Connect(IPAddress ip, int port, string userName, AvatarManager.Avatars avatar);
        Task SendMessage(IData data);
    }
}
