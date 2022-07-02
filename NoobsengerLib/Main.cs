using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NoobsengerLib
{
    public class InfoCodes
    {
        public const string Join = "IC1";
        public const string ServerNameReceived = "IC2";
    }
    public static class AvatarManager
    {
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
            Woman2
        }
    }
    public static class Util
    {

        public static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
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
    public static class Server
    {
        public static IPAddress IP;
        public static int Port;
        private static string serverName;
        public static string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                BroadcastAll(ServerName, ServerName, DataType.InfoMessage, MsgCode: InfoCodes.ServerNameReceived);
            }
        }
        public static Hashtable ClientsList = new Hashtable();
        public static bool IsRuns = true;
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
            int counter = 0;
            while (IsRuns)
            {
                counter++;
                clientSocket = ServerSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[10024];
                ByteData dataFromClient;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom);
                dataFromClient = DataEncoder.ByteArrayToData(bytesFrom);

                if (dataFromClient.DataType == DataType.InfoMessage)
                {
                    if (dataFromClient.InfoCode == InfoCodes.Join)
                    {
                        ClientsList.Add(dataFromClient.ClientName, clientSocket);
                        Broadcast(clientSocket, ServerName, dataFromClient.ClientName, DataType.InfoMessage, MsgCode: InfoCodes.ServerNameReceived);
                        BroadcastAll(dataFromClient.ClientName + " Joined.", dataFromClient.ClientName, DataType.InfoMessage, MsgCode: InfoCodes.Join, avatar: dataFromClient.Avatar);

                        var client = new ClientHandler();
                        client.StartClient(clientSocket, dataFromClient.ClientName);
                    }
                }
            }
            counter = 0;
            clientSocket.Close();
            ServerSocket.Stop();
        }
        public static void BroadcastAll(string msg, string uName, DataType type, AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, string MsgCode = null)
        {
            foreach (DictionaryEntry Item in ClientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes;

                broadcastBytes = DataEncoder.DataToByteArray(new ByteData(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode));

                broadcastStream.Write(broadcastBytes);
                broadcastStream.Flush();
            }
        }
        public static void Broadcast(TcpClient broadcastSocket, string msg, string uName, DataType type, AvatarManager.Avatars avatar = AvatarManager.Avatars.Gamer, Uri[] uploads = null, string MsgCode = "")
        {
            NetworkStream broadcastStream = broadcastSocket.GetStream();
            Byte[] broadcastBytes;
            broadcastBytes = DataEncoder.DataToByteArray(new ByteData(uName, msg, avatar: avatar, uploads: uploads, dataType: type, infoCode: MsgCode));
            broadcastStream.Write(broadcastBytes);
            broadcastStream.Flush();
        }

        public class ClientHandler
        {
            public TcpClient ClientSocket;
            private string clNo;
            public bool IsRuns = true;

            public void StartClient(TcpClient inClientSocket, string clineNo)
            {
                ClientSocket = inClientSocket;
                clNo = clineNo;
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }

            public void doChat()
            {
                int requestCount = 0;
                byte[] bytesFrom = new byte[10025];
                ByteData dataFromClient = null;
                string rCount = null;
                requestCount = 0;

                while (IsRuns)
                {
                    try
                    {
                        requestCount = requestCount + 1;
                        NetworkStream networkStream = ClientSocket.GetStream();
                        networkStream.Read(bytesFrom);
                        dataFromClient = DataEncoder.ByteArrayToData(bytesFrom);
                        rCount = Convert.ToString(requestCount);
                        Server.BroadcastAll(dataFromClient.Message, dataFromClient.ClientName, dataFromClient.DataType, dataFromClient.Avatar, dataFromClient.Uploads, dataFromClient.InfoCode);

                    }
                    catch (Exception ex)
                    {
                    }
                }//end while
            }

            //
            //end doChat
        }
    }
    public class Client
    {
        public event EventHandler<ByteData>? ChatRecieved;
        public event TypedEventHandler<Client, EventArgs>? ServerNameChanged;
        private TcpClient clientSocket = new TcpClient();
        private NetworkStream serverStream = default;
        public string UserName { get; set; }
        public AvatarManager.Avatars Avatar { get; set; }
        public string ServerName { get; set; }
        public void Connect(IPAddress ip, int port, string userName, AvatarManager.Avatars avatar)
        {
            clientSocket.Connect(ip, port);
            serverStream = clientSocket.GetStream();
            this.UserName = userName;
            this.Avatar = avatar;
            Avatar = avatar;
            byte[] outStream = DataEncoder.DataToByteArray(new ByteData(UserName, "", avatar, null, DataType.InfoMessage, InfoCodes.Join));

            serverStream.Write(outStream);

            serverStream.Flush();

            var ctThread = new Thread(GetMessage);

            ctThread.Start();
        }
        public void SendMessage(ByteData data)
        {

            byte[] outStream = DataEncoder.DataToByteArray(data);

            serverStream.Write(outStream, 0, outStream.Length);

            serverStream.Flush();
        }
        private void GetMessage()
        {
            while (true)
            {
                try
                {
                    serverStream = clientSocket.GetStream();

                    int buffSize = 0;

                    byte[] inStream = new byte[10024];

                    buffSize = clientSocket.ReceiveBufferSize;

                    serverStream.Read(inStream);

                    var returndata = DataEncoder.ByteArrayToData(inStream);
                    if (returndata.DataType == DataType.Chat)
                    {
                        ChatRecieved?.Invoke(returndata.ClientName, returndata);
                    }
                    else if (returndata.DataType == DataType.InfoMessage)
                    {
                        if (returndata.InfoCode == InfoCodes.ServerNameReceived)
                        {
                            this.ServerName = returndata.Message;
                            ServerNameChanged?.Invoke(this, new EventArgs());
                        }
                        else if (returndata.InfoCode == InfoCodes.Join)
                        {
                            ChatRecieved?.Invoke(returndata.ClientName, returndata);
                        }
                    }
                }
                catch { }
            }

        }
    }
}
