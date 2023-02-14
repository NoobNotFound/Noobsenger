using Noobsenger.Core.Ultra.DataManager;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Noobsenger.Core.Ultra
{
    public class UltraServer
    {
        public IPAddress IP;
        public int Port;
        private string serverName;

        public List<Channel> Channels = new List<Channel>();
        private int ChannelCount = 0;
        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                BroadcastAll(ServerName, ServerName, DataType.InfoMessage, MsgCode: InfoCodes.ServerNameReceived);
            }
        }
        public Hashtable ClientsList = new Hashtable();
        private bool IsRuns = true;
        public bool IsHosted { get; private set; } = false;
        public TcpListener ServerSocket;
        public int AddChannel(string channelName)
        {
            if (IsHosted)
            {
                ChannelCount++;
                var cnl = new Channel(ChannelCount);
                bool exists = true;
                int p = 0;
                while (exists)
                {
                    p = new Random().Next(1024, 49151);
                    bool exitss = false;
                    foreach (var item in Channels)
                    {
                        if (item.Port == p || p == Port)
                        {
                            exists = true;
                        }
                    }
                    exists = exitss;
                }
                if (cnl.Host(IP, p, channelName))
                {
                    Channels.Add(cnl);

                    BroadcastAll(new Data("" + p, "" + p, dataType: DataType.InfoMessage, infoCode: InfoCodes.AddChannel));
                }
                else
                {
                    throw new Exception("System.Net.Sockets.SocketException Please try again!");
                }
                return cnl.ChannelCount;
            }
            else
            {
                throw new Exception("Server was not hosted");
            }
        }
        public void CloseServer()
        {
            if (IsHosted)
            {
                BroadcastAll(new Data(infoCode: InfoCodes.ServerClosed));
                IsRuns = false;
            }
        }
        public void RemoveChannel(int channelCount)
        {
            foreach (var item in Channels)
            {
                if (item.ChannelCount == channelCount)
                {
                    item.IsHosted = false;
                    item.IsRuns = false;
                    BroadcastAll(new Data("" + item.Port, "" + item.Port, dataType: DataType.InfoMessage, infoCode: InfoCodes.RemoveChannel));
                    Channels.Remove(item);
                    return;
                }
            }
        }
        public void Host(IPAddress address, int port, string serverName)
        {
            IsRuns = true;
            IsHosted = true;
            ServerName = serverName;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServerSocket = new TcpListener(address, port);
            IP = address;
            Port = port;
            ServerSocket.Start();
            var t = new Thread(Reciver);
            t.Start();
        }
        public void BroadcastAll(Data data)
        {
            foreach (DictionaryEntry Item in ClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                byte[] broadcastBytes;

                broadcastBytes = data.ToBytes();

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
        private int ClientsCount = 0;
        private void Reciver()
        {
            TcpClient clientSocket = default;
            while (IsRuns)
            {
                clientSocket = ServerSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[10025];
                Data dataFromClient;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                dataFromClient = bytesFrom.ToData();

                if (dataFromClient.DataType == DataType.InfoMessage)
                {
                    if (dataFromClient.InfoCode == InfoCodes.Join)
                    {
                        ClientsCount++;
                        BroadcastAll(ServerName, ServerName, DataType.InfoMessage, MsgCode: InfoCodes.ServerNameReceived);

                        var client = new ClientHandler(clientSocket, dataFromClient.ClientName, ClientsCount);
                        client.Disconnected += (sender, e) =>
                        {
                            try
                            {
                                foreach (DictionaryEntry Item in ClientsList)
                                {
                                    if ((int)Item.Key == ((ClientHandler)sender).ClientNumber)
                                    {
                                        ClientsList.Remove(Item);
                                        return;
                                    }
                                }
                            }
                            catch { }
                            BroadcastAll(new Data(((ClientHandler)sender).ClientName, ((ClientHandler)sender).ClientName + " Left.", dataType: DataType.InfoMessage, infoCode: InfoCodes.Left));
                        };
                        BroadcastAll(new Data(ServerName, ServerName, dataType: DataType.InfoMessage, infoCode: InfoCodes.ServerNameReceived));
                        client.BytesRecieved += (sender, e) => BroadcastAll(e.Bytes, e.Length);
                        ClientsList.Add(ClientsCount, clientSocket);
                        client.Start();
                        List<int> channelPorts = new List<int>();
                        foreach (var item in Channels)
                        {
                            channelPorts.Add(item.Port);
                        }
                        var data = new Data(message: string.Join(",", channelPorts.ToArray()), dataType: DataType.InfoMessage, infoCode: InfoCodes.AddChannels).ToBytes();
                        var s = client.ClientSocket.GetStream();
                        s.Write(data, 0, data.Length);
                        s.Flush();
                    }
                }
            }
            clientSocket.Close();
            ServerSocket.Stop();
        }
        public void BroadcastAll(byte[] data, int length)
        {
            foreach (DictionaryEntry Item in ClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                if (broadcastSocket.Connected)
                {
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    broadcastStream.Write(data, 0, length);
                    broadcastStream.Flush();
                }
            }
        }
        public void BroadcastAll(string msg, string uName, DataType type, AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, string MsgCode = null, object[] objs = null, int msgCount = 0)
        {
            foreach (DictionaryEntry Item in ClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                if (broadcastSocket.Connected)
                {
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    byte[] broadcastBytes;

                    broadcastBytes = new Data(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode, count: msgCount, objects: objs).ToBytes();

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                }
            }
        }
        public void Broadcast(TcpClient broadcastSocket, string msg, string uName, DataType type, AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, string MsgCode = null, object[] objs = null, int msgCount = 0)
        {
            if (broadcastSocket.Connected)
            {
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                byte[] broadcastBytes;
                broadcastBytes = new Data(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode, count: msgCount, objects: objs).ToBytes();
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }

    }

}

