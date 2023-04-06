using Noobsenger.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Noobsenger.Core.Ultra.DataManager;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Noobsenger.Core.Ultra
{
    public class UltraClient : Interfaces.IClient
    {
        public event EventHandler<IData> ChatRecieved = delegate { };
        public event EventHandler NameChanged = delegate { };
        public event EventHandler ServerClosed = delegate { };
        public event EventHandler<int> ChannelAdded = delegate { };
        public event EventHandler<int> ChannelRemoved = delegate { };
        public TcpClient clientSocket { get; set; } = new TcpClient();
        private NetworkStream serverStream = default;
        public string UserName { get; set; }
        public List<ChannelClient> Channels = new();
        public Avatars Avatar { get; set; }
        public string ServerName { get; set; }
        public IPAddress IP { get; private set; }
        public async void Connect(IPAddress ip, int port, string userName, Avatars avatar)
        {
            IP = ip;
            await clientSocket.ConnectAsync(ip, port);
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
        private void GetMessage()
        {
            while (clientSocket.Connected)
            {
                Data returndata = null;
                try
                {
                    serverStream = clientSocket.GetStream();


                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];


                    serverStream.Read(inStream, 0, inStream.Length);

                    returndata = inStream.ToData();
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
                        if (returndata.InfoCode == InfoCodes.ImgFromWeb)
                        {
                            ChatRecieved.Invoke(returndata.ClientName, returndata);
                        }
                        else if (returndata.InfoCode == InfoCodes.ServerNameReceived)
                        {
                            this.ServerName = returndata.Message;
                            NameChanged.Invoke(this, new EventArgs());
                        }
                        else if (returndata.InfoCode == InfoCodes.Join)
                        {
                            ChatRecieved.Invoke(returndata.ClientName, returndata);
                        }
                        else if (returndata.InfoCode == InfoCodes.AddChannel)
                        {
                            var c = new ChannelClient();
                            c.Connect(IP, int.Parse(returndata.Message), UserName, Avatar);
                            Channels.Add(c);
                            ChannelAdded(this, c.Port);
                        }
                        else if (returndata.InfoCode == InfoCodes.AddGPTChannel)
                        {
                            var c = new ChannelClient() { QuickGPT = true };
                            c.Connect(IP, int.Parse(returndata.Message), UserName, Avatar);
                            Channels.Add(c);
                            ChannelAdded(this, c.Port);
                        }
                        else if (returndata.InfoCode == InfoCodes.RemoveChannel)
                        {
                            foreach (var item in Channels)
                            {
                                if (item.Port == int.Parse(returndata.Message))
                                {
                                    ChannelRemoved(this, item.Port);
                                    item.clientSocket.Close();
                                    item.clientSocket.Dispose();
                                    Channels.Remove(item);
                                    return;
                                }
                            }
                        }
                        else if (returndata.InfoCode == InfoCodes.Left)
                        {
                            ChatRecieved.Invoke(returndata.ClientName, returndata);
                        }
                        else if (returndata.InfoCode == InfoCodes.AddChannels)
                        {
                            var chnls = returndata.Message.Split(':'); 
                            foreach (var item in chnls)
                            {
                                if (int.TryParse(item,out var x))
                                {
                                    var c = new ChannelClient();
                                    c.Connect(IP, x, UserName, Avatar);
                                    Channels.Add(c);
                                    ChannelAdded(this, c.Port);
                                }
                            }
                        }
                        else if (returndata.InfoCode == InfoCodes.ServerClosed)
                        {
                            ServerClosed.Invoke(this, new());
                            try
                            {
                                foreach (var Item in Channels)
                                {
                                    ChannelRemoved(this, Item.Port);
                                    Item.clientSocket.Close();
                                    Item.clientSocket.Dispose();
                                    Channels.Remove(Item);

                                }
                            }
                            catch { }
                        }


                    }
                }
            }

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
    }
    public class ChannelClient : IClient
    {
        public event EventHandler<IData> ChatRecieved = delegate { };
        public event EventHandler NameChanged = delegate { };
        public TcpClient clientSocket { get; set; } = new TcpClient();
        private NetworkStream serverStream = default;
        public string UserName { get; set; }
        public bool QuickGPT { get; set; }
        public Avatars Avatar { get; set; }
        public string ChannelName { get; set; }
        public int Port;
        public IPAddress IP;
        public async void Connect(IPAddress ip, int port, string userName, Avatars avatar)
        {
            IP = ip;
            Port = port;
            await clientSocket.ConnectAsync(ip, port);
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
                Data returndata = null;
                try
                {
                    serverStream = clientSocket.GetStream();


                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];


                    serverStream.Read(inStream, 0, inStream.Length);

                    returndata = inStream.ToData();
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
                            this.ChannelName = returndata.Message;
                            NameChanged.Invoke(this, new EventArgs());
                        }
                        else if (returndata.InfoCode == InfoCodes.Join)
                        {
                            ChatRecieved.Invoke(returndata.ClientName, returndata);
                        }
                        else if (returndata.InfoCode == InfoCodes.Left)
                        {
                            ChatRecieved.Invoke(returndata.ClientName, returndata);
                        }
                    }
                }
            }

        }
    }
}
