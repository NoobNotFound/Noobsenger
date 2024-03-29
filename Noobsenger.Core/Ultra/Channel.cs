﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Noobsenger.Core.Ultra.DataManager;

namespace Noobsenger.Core.Ultra
{
    public class Channel
    {
        public int ChannelCount { get; set; }
        private int ClientsCount = 0;
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        private ObservableCollection<ClientHandler> ClientHandlersList = new();
        private string serverName;
        public int MessagesCount { get; private set; }
        public string ChannelName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                BroadcastAll(new Data(ChannelName, ChannelName, dataType: DataType.InfoMessage, infoCode: InfoCodes.ServerNameReceived));
            }
        }
        public Hashtable TcpClientsList { get; set; } = new Hashtable();
        public bool IsRunning = false;
        public bool IsHosted = false;
        public TcpListener ServerSocket;
        public Channel(int Count)
        {
            this.ChannelCount = Count;
        }
        public bool Host(IPAddress address, int port, string serverName)
        {
            try
            {
                IsHosted = true;
                ChannelName = serverName;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServerSocket = new TcpListener(address, port);
                IP = address;
                Port = port;
                IsRunning = true;
                ServerSocket.Start();
                var t = new Thread(Reciver);
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
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
                        var client = new ClientHandler(clientSocket, dataFromClient.ClientName, ClientsCount,guid:dataFromClient.GUID);
                        client.Disconnected += (sender, e) =>
                        {
                            try
                            {
                                foreach (DictionaryEntry Item in TcpClientsList)
                                {
                                    if ((int)Item.Key == ((ClientHandler)sender).ClientNumber)
                                    {
                                        TcpClientsList.Remove(Item);
                                        return;
                                    }
                                }
                                foreach (var Item in ClientHandlersList)
                                {
                                    if (Item.ClientNumber == ((ClientHandler)sender).ClientNumber)
                                    {
                                        Item.ClientSocket.Dispose();
                                        ClientHandlersList.Remove(Item);
                                        return;
                                    }
                                }
                            }
                            catch { }
                            BroadcastAll(new Data(((ClientHandler)sender).ClientName, ((ClientHandler)sender).ClientName + " Left.", dataType: DataType.InfoMessage, infoCode: InfoCodes.Left));
                        };
                        client.BytesRecieved += (sender, e) => BroadcastAll(e.Bytes, e.Length);
                        TcpClientsList.Add(ClientsCount, clientSocket);
                        ClientHandlersList.Add(client);
                        client.Start();
                        BroadcastAll(new Data(dataFromClient.ClientName, dataFromClient.ClientName + " Joined.", dataFromClient.Avatar, null, DataType.InfoMessage, InfoCodes.Join));
                        Task.Delay(200).Wait();
                        BroadcastAll(new Data(ChannelName, ChannelName, dataType: DataType.InfoMessage, infoCode: InfoCodes.ServerNameReceived));
                    }
                    else if (dataFromClient.InfoCode == InfoCodes.ServerClosed)
                    {
                        try
                        {
                            foreach (DictionaryEntry Item in TcpClientsList)
                            {
                                TcpClientsList.Remove(Item);

                            }
                            foreach (var Item in ClientHandlersList)
                            {
                                Item.ClientSocket.Dispose();
                                ClientHandlersList.Remove(Item);

                            }
                        }
                        catch { }
                    }
                }
            }
            clientSocket.Close();
            ServerSocket.Stop();
        }
        public void BroadcastAll(Data data)
        {
            foreach (DictionaryEntry Item in TcpClientsList)
            {
                try
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    if (broadcastSocket.Connected)
                    {
                        NetworkStream broadcastStream = broadcastSocket.GetStream();
                        byte[] broadcastBytes;

                        broadcastBytes = data.ToBytes();

                        broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        broadcastStream.Flush();
                    }
                }
                catch { }
            }
        }
        public void BroadcastAll(byte[] data,int length)
        {
            foreach (DictionaryEntry Item in TcpClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                broadcastStream.Write(data, 0, length);
                broadcastStream.Flush();
            }
        }

        
    }
    internal class ClientHandler
    {
        public TcpClient ClientSocket;
        public event EventHandler<BytesRecievedEventArgs> BytesRecieved = delegate { };
        public event EventHandler Disconnected = delegate { };
        public int ClientNumber;
        public bool IsRunning = true;
        public string ClientName;
        public Guid ClientId;
        public ClientHandler(TcpClient clientSocket, string clientName, int clientNumber, bool start = false,string guid = null)
        {
            if (!Guid.TryParse(guid, out ClientId))
                ClientId = Guid.NewGuid();
            ClientSocket = clientSocket;
            ClientName = clientName;
            ClientNumber = clientNumber;
            if (start)
            {
                Thread ctThread = new(GetData);
                ctThread.Start();
            }
        }
        public void Start()
        {
            Thread ctThread = new(GetData);
            ctThread.Start();
        }
        private void GetData()
        {
            byte[] bytesFrom = new byte[10025];

            while (ClientSocket.Connected)
            {
                try
                {
                    NetworkStream networkStream = ClientSocket.GetStream();
                    int l = networkStream.Read(bytesFrom, 0, bytesFrom.Length);

                    this.BytesRecieved(this, new BytesRecievedEventArgs(bytesFrom, l));

                }
                catch
                {
                }
            }
            Disconnected(this, new EventArgs());
        }
        internal class BytesRecievedEventArgs : EventArgs
        {
            public byte[] Bytes { get; set; }
            public int Length { get; set; }
            public BytesRecievedEventArgs(byte[] bytes, int length)
            {
                Bytes = bytes;
                Length = length;
            }
        }

    }
    
}
