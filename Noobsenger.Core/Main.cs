﻿using Noobsenger.Core.Interfaces;
using Noobsenger.Core.Ultra.DataManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Noobsenger.Core
{
    public class InfoCodes
    {
        public static string Join { get; set; } = "IC1";
        public static string ServerNameReceived { get; set; } = "IC2";
        public static string Left { get; set; } = "IC3";
        public static string AddChannel { get; set; } = "IC4";
        public static string RemoveChannel { get; set; } = "IC5";
        public static string AddChannels { get; set; } = "IC6";
        public static string ServerClosed  { get; set; } = "IC7";
        public static string ChannelClosed { get; set; } = "IC8";
        public static string ImgFromWeb { get; set; } = "IC9";
        public static string AddGPTChannel { get; set; } = "IC10";
        public static string Thinking { get; set; } = "IC11";
        public static string NotThinking { get; set; } = "IC12";
        public static string MembersUpdated { get; set; } = "IC13";
        public static string MessageDelete { get; set; } = "IC14";
        public static string ChannelsRequest { get; set; } = "IC15";
    }

    public enum Avatars
    {
        Boy,
        Gamer,
        Girl,
        Man,
        Man2,
        Man3,
        MaskedMan,
        Nerd,
        Sir,
        Woman,
        Woman1,
        Woman2,
        OpenAI
    }
    public static class Util
    {

        public static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new(array);
            return list;
        }
        public static List<IPAddress> GetIPAddresses()
        {
            return NetworkInterface
             .GetAllNetworkInterfaces()
             .SelectMany(i => i.GetIPProperties().UnicastAddresses)
             .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
             .Select(a => a.Address)
             .ToList();
        }
        public static IPAddress ParseIPAddress(string ipString)
        {
            try
            {
                return IPAddress.Parse(ipString);
            }
            catch
            {
                return null;
            }
        }
    }
    [Obsolete]
    public static class Server
    {
        public static IPAddress IP;
        public static int Port;
        private static string serverName;
        public static int MessagesCount { get; private set; }
        public static string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                BroadcastAll(ServerName, ServerName, DataType.InfoMessage, MsgCode: InfoCodes.ServerNameReceived);
            }
        }
        public static Hashtable ClientsList = new();
        public static bool IsRunning = true;
        public static bool IsHosted = false;
        public static TcpListener ServerSocket;
        public static void Host(IPAddress address, int port, string serverName)
        {
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
        private static void Reciver()
        {
            TcpClient clientSocket = default;
            while (IsRunning)
            {
                clientSocket = ServerSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[10025];
                ChatData dataFromClient;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                dataFromClient = DataEncoder.ByteArrayToData(bytesFrom);

                if (dataFromClient.DataType == DataType.InfoMessage)
                {
                    if (dataFromClient.InfoCode == InfoCodes.Join)
                    {
                        ClientsList.Add(dataFromClient.ClientName, clientSocket);
                        BroadcastAll(ServerName, ServerName, DataType.InfoMessage, MsgCode: InfoCodes.ServerNameReceived);
                        BroadcastAll(dataFromClient.ClientName + " Joined.", dataFromClient.ClientName, DataType.InfoMessage, MsgCode: InfoCodes.Join, avatar: dataFromClient.Avatar);

                        var client = new ClientHandler();
                        client.StartClient(clientSocket, dataFromClient.ClientName);
                    }
                }
            }
            clientSocket.Close();
            ServerSocket.Stop();
        }
        public static void BroadcastAll(string msg, string uName, DataType type, Avatars avatar = Avatars.Gamer, Uri[] uploads = null, string MsgCode = null,int msgCount = 0)
        {
            foreach (DictionaryEntry Item in ClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                byte[] broadcastBytes;

                broadcastBytes = DataEncoder.DataToByteArray(new ChatData(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode,count:msgCount));

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
        public static void Broadcast(TcpClient broadcastSocket, string msg, string uName, DataType type, Avatars avatar = Avatars.Gamer, Uri[] uploads = null, string MsgCode = "")
        {
            NetworkStream broadcastStream = broadcastSocket.GetStream();
            byte[] broadcastBytes;
            broadcastBytes = DataEncoder.DataToByteArray(new ChatData(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode));
            broadcastStream.Write(broadcastBytes,0,broadcastBytes.Length);
            broadcastStream.Flush();
        }

        public class ClientHandler
        {
            public TcpClient ClientSocket;
            private string clNo;
            public bool IsRunning = true;

            public void StartClient(TcpClient inClientSocket, string clineNo)
            {
                ClientSocket = inClientSocket;
                clNo = clineNo;
                Thread ctThread = new(doChat);
                ctThread.Start();
            }

            public void doChat()
            {
                byte[] bytesFrom = new byte[10025];

                while (IsRunning)
                {
                    try
                    {
                        NetworkStream networkStream = ClientSocket.GetStream();
                        int l = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                        foreach (DictionaryEntry Item in ClientsList)
                        {
                            TcpClient broadcastSocket;
                            broadcastSocket = (TcpClient)Item.Value;
                            NetworkStream broadcastStream = broadcastSocket.GetStream();
                            broadcastStream.Write(bytesFrom, 0, l);
                            broadcastStream.Flush();
                            broadcastStream.FlushAsync();
                        }

                    }
                    catch
                    {
                    }
                }//end while
            }

            //
            //end doChat
        }
    }
    [Obsolete]
    public class Client : IClient
    {
        public event EventHandler<IData> ChatRecieved = delegate { };
        public event EventHandler NameChanged = delegate { };
        public TcpClient clientSocket { get; set; } = new TcpClient();
        private NetworkStream serverStream = default;
        public string UserName { get; set; }
        public Avatars Avatar { get; set; }
        public string ServerName { get; set; }
        public Guid GUID { get; set; }

        public async void Connect(IPAddress ip, int port, string userName, Avatars avatar,Guid guid)
        {
           await clientSocket.ConnectAsync(ip, port);
            GUID = guid;
            serverStream = clientSocket.GetStream();
            this.UserName = userName;
            this.Avatar = avatar;
            Avatar = avatar;
            var ctThread = new Thread(GetMessage);

            ctThread.Start();
            byte[] outStream = DataEncoder.DataToByteArray(new ChatData(UserName, UserName, avatar, null, DataType.InfoMessage, InfoCodes.Join));

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            await serverStream.FlushAsync();

        }
        public async Task SendMessage(IData data)
        {
            if (data is ChatData d)
            {
                byte[] outStream = DataEncoder.DataToByteArray(d);
                await serverStream.WriteAsync(outStream, 0, outStream.Length);
                serverStream.Flush();
                await serverStream.FlushAsync();
            }
            else
            {
                byte[] outStream = ((Data)data).ToBytes();
                await serverStream.WriteAsync(outStream, 0, outStream.Length);
                serverStream.Flush();
                await serverStream.FlushAsync();
            }
        }
        private void GetMessage()
        {
            while (clientSocket.Connected)
            {
                ChatData returndata = null;
                try
                {
                    serverStream = clientSocket.GetStream();


                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];


                    serverStream.Read(inStream, 0, inStream.Length);
                    
                    returndata = DataEncoder.ByteArrayToData(inStream);
                }
                catch { }
                if (returndata != null)
                {
                    if (returndata.DataType == DataType.Chat)
                    {
                        ChatRecieved.Invoke(returndata.ClientName, returndata);
                    }
                    else if (returndata.DataType == DataType.InfoMessage)
                    {
                        if (returndata.InfoCode == InfoCodes.ServerNameReceived)
                        {
                            this.ServerName = returndata.Message;
                            NameChanged.Invoke(this, new EventArgs());
                        }
                        else if (returndata.InfoCode == InfoCodes.Join)
                        {
                            ChatRecieved.Invoke(returndata.ClientName, returndata);
                        }
                    }
                }
            }

        }
    }
}
