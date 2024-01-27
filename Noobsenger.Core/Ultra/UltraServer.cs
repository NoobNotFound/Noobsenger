using Noobsenger.Core.Ultra.DataManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Noobsenger.Core.Interfaces;

namespace Noobsenger.Core.Ultra
{
    public class UltraServer
    {
        public IPAddress IP;
        public int Port;
        private string serverName;

        public List<Channel> Channels = new();
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
        public Hashtable ClientsList = new();
        private bool IsRunning = true;
        public bool IsHosted { get; private set; } = false;
        public TcpListener ServerSocket;
        public int AddChannel(string channelName,bool isQuickGPT = false)
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
                    BroadcastAll(new Data("" + p, "" + p, dataType: DataType.InfoMessage, infoCode: isQuickGPT ? InfoCodes.AddGPTChannel : InfoCodes.AddChannel));
                }
                else
                {
                    throw new Exception("Please try again!",new SocketException());
                }
                return cnl.ChannelCount;
            }
            else
            {
                throw new Exception("Server was not hosted");
            }
        }
        public void TryCloseServer()
        {
            if (IsHosted)
            {
                BroadcastAll(new Data(dataType:DataType.InfoMessage, infoCode: InfoCodes.ServerClosed));
                this.IsRunning = false;
                foreach (var item in Channels)
                {
                    item.IsRunning = false;
                }
                IsRunning = false;
            }
        }
        public void RemoveChannel(int channelCount)
        {
            foreach (var item in Channels)
            {
                if(item.ChannelCount == channelCount)
                {
                    item.IsHosted = false;
                    item.IsRunning = false;
                    BroadcastAll(new Data("" + item.Port, "" + item.Port, dataType: DataType.InfoMessage, infoCode: InfoCodes.RemoveChannel));
                    Channels.Remove(item);
                    return;
                }
            }
        }
        public void Host(IPAddress address, int port, string serverName)
        {
            IsRunning = true;
            IsHosted = true;
            ServerName = serverName;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServerSocket = new TcpListener(address, port);
            IP = address;
            Port = port;
            ServerSocket.Start();
            var t = new Thread(Reciver);
            t.Start();
            var c = new UltraClient();
            c.Connect(address, port, "Braniac", Avatars.OpenAI,Guid.NewGuid());
            var GPT3 = new GPT3(c);

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
        private string memb = "";
        private void Reciver()
        {
            TcpClient clientSocket = default;
            while (IsRunning)
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
                        memb += dataFromClient.Message + ",";

                        var client = new ClientHandler(clientSocket, dataFromClient.ClientName, ClientsCount,false,dataFromClient.GUID);
                        client.Disconnected += (sender, e) =>
                        {
                            try
                            {
                                foreach (DictionaryEntry Item in ClientsList)
                                {
                                    if ((Guid)Item.Key == ((ClientHandler)sender).ClientId)
                                    {
                                        ClientsList.Remove(Item);
                                        return;
                                    }
                                }
                            }
                            catch { }
                            BroadcastAll(new Data(((ClientHandler)sender).ClientName, ((ClientHandler)sender).ClientName + " Left.", dataType: DataType.InfoMessage, infoCode: InfoCodes.Left));
                        };
                        client.BytesRecieved += (sender, e) => BroadcastAll(e.Bytes, e.Length);
                        ClientsList.Add(dataFromClient.GUID, clientSocket);
                        client.Start();

                        var channelPorts = Channels.Select(x => x.Port.ToString());
                        BroadcastAll(string.Join(",", channelPorts.ToArray()), ServerName, DataType.InfoMessage, MsgCode: InfoCodes.AddChannels);

                    }
                    else if (dataFromClient.InfoCode == InfoCodes.ChannelsRequest)
                    {
                        var channelPorts = Channels.Select(x => x.Port.ToString());
                        BroadcastAll(string.Join(",", channelPorts.ToArray()), ServerName, DataType.InfoMessage, MsgCode: InfoCodes.AddChannels);
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
                try
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
                catch { }
            }
        }
        public void BroadcastAll(string msg, string uName, DataType type, Avatars avatar = Avatars.Gamer, Uri[] uploads = null, string MsgCode = null,byte[] files = null, int msgCount = 0, string guid = null)
        {
            foreach (DictionaryEntry Item in ClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                if (broadcastSocket.Connected)
                {
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    byte[] broadcastBytes;

                    broadcastBytes = new Data(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode, count: msgCount,Files:files,gUID:guid).ToBytes();

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                }
            }
        }
        public void Broadcast(TcpClient broadcastSocket, string msg, string uName, DataType type, Avatars avatar = Avatars.Gamer, Uri[] uploads = null, string MsgCode = null, byte[] files = null, int msgCount = 0, string guid = null)
        {
            if (broadcastSocket.Connected)
            {
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                byte[] broadcastBytes;
                broadcastBytes = new Data(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode, count: msgCount, Files: files, gUID: guid).ToBytes();
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }

    }

}

