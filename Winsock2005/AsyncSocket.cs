using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Winsock2005DLL
{
    public class AsyncSocket
    {
        private WinsockMonitor m_Monitor;
        private Socket m_sock;
        private int incBufferSize;
        private Collection bufferCol;
        private byte[] byteBuffer;
        private UdpClient m_udp;
        private ByteBufferCol _buff;
        private int _sizeComing;
        private byte _firstDelim;

        public AsyncSocket(WinsockMonitor winsock_monitor, int inc_size)
        {
            this.incBufferSize = 1024;
            this.byteBuffer = new byte[checked(this.incBufferSize + 1)];
            this._sizeComing = -1;
            this._firstDelim = (byte)0;
            this.bufferCol = new Collection();
            this._buff = new ByteBufferCol();
            this.incBufferSize = inc_size;
            this.byteBuffer = new byte[checked(this.incBufferSize + 1)];
            this.m_Monitor = winsock_monitor;
        }

        public AsyncSocket(WinsockMonitor winsock_monitor, Socket client, int inc_size)
        {
            this.incBufferSize = 1024;
            this.byteBuffer = new byte[checked(this.incBufferSize + 1)];
            this._sizeComing = -1;
            this._firstDelim = (byte)0;
            this.bufferCol = new Collection();
            this._buff = new ByteBufferCol();
            this.incBufferSize = inc_size;
            this.byteBuffer = new byte[checked(this.incBufferSize + 1)];
            this.m_Monitor = winsock_monitor;
            this.m_sock = client;
            this.Receive();
        }

        public byte[] GetData()
        {
            if (this.bufferCol.Count == 0)
                return (byte[])null;
            byte[] numArray = (byte[])this.bufferCol[1];
            this.bufferCol.Remove(1);
            return numArray;
        }

        public int BufferCount() => this.bufferCol.Count;

        public void Close()
        {
            try
            {
                if (this.m_Monitor.State == WinsockStates.Closed)
                    return;
                if (this.m_Monitor.State == WinsockStates.Listening)
                {
                    this.m_Monitor.ClosingBegun();
                    if (this.m_Monitor.Protocol == WinsockProtocols.Tcp)
                        this.m_sock.Close();
                    else
                        this.m_udp.Close();
                    this.m_Monitor.CloseDone();
                }
                else if (this.m_Monitor.State == WinsockStates.Connected)
                {
                    this.m_Monitor.ClosingBegun();
                    this.m_sock.BeginDisconnect(false, new AsyncCallback(this.CloseCallback), (object)this.m_sock);
                }
                else
                    this.m_Monitor.CloseDone();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                this.m_Monitor.ErrorNotify(ex.Message, "AsyncSocket.Close");
                ProjectData.ClearProjectError();
            }
        }

        private void CloseCallback(IAsyncResult ar)
        {
            try
            {
                Socket asyncState = (Socket)ar.AsyncState;
                asyncState.EndDisconnect(ar);
                asyncState.Close();
                this.m_Monitor.CloseDone();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                if (exception is SocketException)
                {
                    SocketException socketException = (SocketException)exception;
                    if (socketException.SocketErrorCode != SocketError.Success)
                    {
                        this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.CloseCallback", socketException.SocketErrorCode, exception.ToString());
                        ProjectData.ClearProjectError();
                        return;
                    }
                }
                if (exception is ObjectDisposedException)
                {
                    ProjectData.ClearProjectError();
                }
                else
                {
                    this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.CloseCallback", exDetails: exception.ToString());
                    ProjectData.ClearProjectError();
                }
            }
        }

        public void Listen(int port, int max_pending)
        {
            try
            {
                if (this.m_Monitor.Protocol == WinsockProtocols.Tcp)
                {
                    this.m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.m_sock.Bind((EndPoint)new IPEndPoint(IPAddress.Any, port));
                    this.m_sock.Listen(max_pending);
                    this.m_Monitor.ListeningStarted();
                    this.m_sock.BeginAccept(new AsyncCallback(this.ListenCallback), (object)this.m_sock);
                }
                else
                {
                    if (this.m_Monitor.Protocol != WinsockProtocols.Udp)
                        return;
                    this.m_udp = new UdpClient(port);
                    this.m_Monitor.ListeningStarted();
                    this.m_udp.BeginReceive(new AsyncCallback(this.ReceiveCallbackUDP), (object)null);
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                this.m_Monitor.ErrorNotify(ex.Message, "AsyncSocket.Listen");
                ProjectData.ClearProjectError();
            }
        }

        private void ListenCallback(IAsyncResult ar)
        {
            try
            {
                Socket asyncState = (Socket)ar.AsyncState;
                this.m_Monitor.ClientReceived(asyncState.EndAccept(ar));
                if (this.m_Monitor.State != WinsockStates.Listening)
                {
                    asyncState.Close();
                    return;
                }
                asyncState.BeginAccept(new AsyncCallback(this.ListenCallback), (object)asyncState);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                switch (exception)
                {
                    case SocketException _:
                        SocketException socketException = (SocketException)exception;
                        if (socketException.SocketErrorCode != SocketError.Success)
                        {
                            this.Close();
                            this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ListenCallback", socketException.SocketErrorCode);
                            ProjectData.ClearProjectError();
                            goto label_9;
                        }
                        else
                            break;
                    case ObjectDisposedException _:
                        ProjectData.ClearProjectError();
                        goto label_9;
                }
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ListenCallback");
                ProjectData.ClearProjectError();
            }
        label_9:;
        }

        public void Connect(IPAddress remIP, int port)
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(remIP, port);
                this.m_sock = new Socket(remIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                if (this.m_Monitor.State != WinsockStates.HostResolved)
                    return;
                this.m_Monitor.ConnectStarted();
                this.m_sock.BeginConnect((EndPoint)ipEndPoint, new AsyncCallback(this.ConnectCallback), (object)this.m_sock);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.Connect", exDetails: exception.ToString());
                ProjectData.ClearProjectError();
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (this.m_Monitor.State != WinsockStates.Connecting)
                    return;
                ((Socket)ar.AsyncState).EndConnect(ar);
                if (this.m_Monitor.State != WinsockStates.Connecting)
                    return;
                this.Receive();
                this.m_Monitor.ConnectFinished();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                if (exception is SocketException)
                {
                    SocketException socketException = (SocketException)exception;
                    if (socketException.SocketErrorCode != SocketError.Success)
                    {
                        this.Close();
                        this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ConnectCallback", socketException.SocketErrorCode);
                        ProjectData.ClearProjectError();
                        goto label_9;
                    }
                }
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ConnectCallback");
                ProjectData.ClearProjectError();
            }
        label_9:;
        }

        private void Receive()
        {
            try
            {
                SocketError errorCode;
                this.m_sock.BeginReceive(this.byteBuffer, 0, this.incBufferSize, SocketFlags.None, out errorCode, new AsyncCallback(this.ReceiveCallback), (object)errorCode);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                this.m_Monitor.ErrorNotify(ex.Message, "AsyncSocket.Receive");
                ProjectData.ClearProjectError();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                SocketError errorCode = (SocketError)Conversions.ToInteger(ar.AsyncState);
                int iSize = this.m_sock.EndReceive(ar);
                if (iSize < 1)
                {
                    if (this._buff.Count > 0)
                        this._buff.Clear();
                    this.Close();
                    return;
                }
                IPEndPoint remoteEndPoint = (IPEndPoint)this.m_sock.RemoteEndPoint;
                this.ProcessIncoming(this.byteBuffer, iSize, remoteEndPoint.Address.ToString(), remoteEndPoint.Port);
                this.byteBuffer = new byte[checked(this.incBufferSize + 1)];
                this.m_sock.BeginReceive(this.byteBuffer, 0, this.incBufferSize, SocketFlags.None, out errorCode, new AsyncCallback(this.ReceiveCallback), (object)errorCode);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                switch (exception)
                {
                    case SocketException _:
                        SocketException socketException = (SocketException)exception;
                        if (socketException.SocketErrorCode != SocketError.Success)
                        {
                            this.Close();
                            this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ReceiveCallback", socketException.SocketErrorCode);
                            ProjectData.ClearProjectError();
                            goto label_11;
                        }
                        else
                            break;
                    case ObjectDisposedException _:
                        ProjectData.ClearProjectError();
                        goto label_11;
                }
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ReceiveCallback");
                ProjectData.ClearProjectError();
            }
        label_11:;
        }

        private void ReceiveCallbackUDP(IAsyncResult ar)
        {
            try
            {
                IPEndPoint remoteEP = (IPEndPoint)null;
                this.byteBuffer = this.m_udp.EndReceive(ar, ref remoteEP);
                if (this.byteBuffer == null)
                {
                    if (this._buff.Count <= 0)
                        return;
                    this._buff.Clear();
                    return;
                }
                if (this.byteBuffer.Length < 1)
                {
                    if (this._buff.Count <= 0)
                        return;
                    this._buff.Clear();
                    return;
                }
                this.ProcessIncoming(this.byteBuffer, this.byteBuffer.Length, remoteEP.Address.ToString(), remoteEP.Port);
                this.m_udp.BeginReceive(new AsyncCallback(this.ReceiveCallbackUDP), (object)null);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                switch (exception)
                {
                    case SocketException _:
                        SocketException socketException = (SocketException)exception;
                        if (socketException.SocketErrorCode != SocketError.Success)
                        {
                            this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ReceiveCallbackUDP", socketException.SocketErrorCode);
                            ProjectData.ClearProjectError();
                            goto label_14;
                        }
                        else
                            break;
                    case ObjectDisposedException _:
                        ProjectData.ClearProjectError();
                        goto label_14;
                }
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.ReceiveCallbackUDP");
                ProjectData.ClearProjectError();
            }
        label_14:;
        }

        public void Send(byte[] byt)
        {
            try
            {
                if (this.m_Monitor.Legacy_Support)
                {
                    AsyncSocket.SendState sendState = new AsyncSocket.SendState();
                    sendState.Bytes = byt;
                    sendState.Length = byt.Length;
                    sendState.StartIndex = 0;
                    sendState.SendLength = Information.UBound((Array)byt) <= this.incBufferSize ? byt.Length : checked(this.incBufferSize + 1);
                    if (this.m_Monitor.Protocol == WinsockProtocols.Tcp)
                    {
                        this.m_sock.BeginSend(byt, 0, sendState.SendLength, SocketFlags.None, out sendState.ErrCode, new AsyncCallback(this.SendCallback), (object)sendState);
                    }
                    else
                    {
                        if (this.m_Monitor.Protocol != WinsockProtocols.Udp)
                            return;
                        this.m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        IPEndPoint ipEndPoint = new IPEndPoint(Dns.GetHostEntry(this.m_Monitor.RemoteServer).AddressList[0], this.m_Monitor.RemotePort);
                        this.m_sock.BeginSendTo(byt, 0, sendState.SendLength, SocketFlags.None, (EndPoint)ipEndPoint, new AsyncCallback(this.SendToCallback), (object)sendState);
                    }
                }
                else
                {
                    byt = this.AppendSize(byt);
                    AsyncSocket.SendState sendState = new AsyncSocket.SendState();
                    sendState.Bytes = byt;
                    sendState.Length = byt.Length;
                    sendState.StartIndex = 0;
                    sendState.SendLength = Information.UBound((Array)byt) <= this.incBufferSize ? byt.Length : checked(this.incBufferSize + 1);
                    this.m_sock.BeginSend(byt, 0, sendState.SendLength, SocketFlags.None, out sendState.ErrCode, new AsyncCallback(this.SendCallback), (object)sendState);
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                this.m_Monitor.ErrorNotify(ex.Message, "AsyncSocket.Send");
                ProjectData.ClearProjectError();
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                AsyncSocket.SendState asyncState = (AsyncSocket.SendState)ar.AsyncState;
                SocketError errCode = asyncState.ErrCode;
                checked { asyncState.TotalSent += this.m_sock.EndSend(ar); }
                if (checked(asyncState.StartIndex + asyncState.SendLength) < Information.UBound((Array)asyncState.Bytes))
                {
                    checked { asyncState.StartIndex += asyncState.SendLength; }
                    if (asyncState.Bytes.Length <= checked(asyncState.StartIndex + asyncState.SendLength + 1))
                        asyncState.SendLength = checked(Information.UBound((Array)asyncState.Bytes) - asyncState.StartIndex - 1);
                    this.m_Monitor.SendProgress(asyncState.TotalSent, asyncState.Length);
                    this.m_sock.BeginSend(asyncState.Bytes, asyncState.StartIndex, asyncState.SendLength, SocketFlags.None, out asyncState.ErrCode, new AsyncCallback(this.SendCallback), (object)asyncState);
                }
                else
                    this.m_Monitor.SendCompleted(asyncState.TotalSent, asyncState.Length);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                if (exception is SocketException)
                {
                    SocketException socketException = (SocketException)exception;
                    if (socketException.SocketErrorCode != SocketError.Success)
                    {
                        this.Close();
                        this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.SendCallback", socketException.SocketErrorCode);
                        ProjectData.ClearProjectError();
                        return;
                    }
                }
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.SendCallback");
                ProjectData.ClearProjectError();
            }
        }

        private void SendToCallback(IAsyncResult ar)
        {
            try
            {
                AsyncSocket.SendState asyncState = (AsyncSocket.SendState)ar.AsyncState;
                SocketError errCode = asyncState.ErrCode;
                checked { asyncState.TotalSent += this.m_sock.EndSendTo(ar); }
                if (checked(asyncState.StartIndex + asyncState.SendLength) < Information.UBound((Array)asyncState.Bytes))
                {
                    checked { asyncState.StartIndex += asyncState.SendLength; }
                    if (asyncState.Bytes.Length < checked(asyncState.StartIndex + asyncState.SendLength + 1))
                        asyncState.SendLength = checked(Information.UBound((Array)asyncState.Bytes) - asyncState.StartIndex - 1);
                    this.m_Monitor.SendProgress(asyncState.TotalSent, asyncState.Length);
                    IPEndPoint ipEndPoint = new IPEndPoint(Dns.GetHostEntry(this.m_Monitor.RemoteServer).AddressList[0], this.m_Monitor.RemotePort);
                    this.m_sock.BeginSendTo(asyncState.Bytes, asyncState.StartIndex, asyncState.SendLength, SocketFlags.None, (EndPoint)ipEndPoint, new AsyncCallback(this.SendToCallback), (object)asyncState);
                }
                else
                    this.m_Monitor.SendCompleted(asyncState.TotalSent, asyncState.Length);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                if (exception is SocketException)
                {
                    SocketException socketException = (SocketException)exception;
                    if (socketException.SocketErrorCode != SocketError.Success)
                    {
                        this.Close();
                        this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.SendToCallback", socketException.SocketErrorCode);
                        ProjectData.ClearProjectError();
                        return;
                    }
                }
                this.m_Monitor.ErrorNotify(exception.Message, "AsyncSocket.SendToCallback");
                ProjectData.ClearProjectError();
            }
        }

        private byte[] AppendSize(byte[] byt)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Information.UBound((Array)byt).ToString());
            byt = this.AppendByte(this.EncloseByte(this.FreeByte(bytes), bytes), byt);
            return byt;
        }

        private void ProcessIncoming(byte[] byt, int iSize, string source_ip, int source_port)
        {
            if (this.m_Monitor.Legacy_Support)
            {
                if (iSize > 0)
                {
                    if (checked(iSize - 1) < Information.UBound((Array)byt))
                        byt = (byte[])Utils.CopyArray((Array)byt, (Array)new byte[checked(iSize - 1 + 1)]);
                    this.bufferCol.Add((object)byt, (string)null, (object)null, (object)null);
                    this.m_Monitor.DataReceived(iSize, source_ip, source_port);
                }
            }
            else
            {
                if (iSize <= 0)
                    return;
                byt = (byte[])Utils.CopyArray((Array)byt, (Array)new byte[checked(iSize - 1 + 1)]);
                if (this._sizeComing == -1)
                {
                    if (this._firstDelim == (byte)0)
                    {
                        this._firstDelim = byt[0];
                        byte[] byt1 = new byte[checked(Information.UBound((Array)byt) - 1 + 1)];
                        Array.Copy((Array)byt, 1, (Array)byt1, 0, checked(byt.Length - 1));
                        this.ProcessIncoming(byt1, byt1.Length, source_ip, source_port);
                    }
                    else
                    {
                        if (this._buff.Count > 1)
                        {
                            this._firstDelim = (byte)0;
                            this._buff.Clear();
                            throw new Exception("Unable to determine size of incoming packet.  It's possible you may need to use Legacy Support.");
                        }
                        int length = Array.IndexOf<byte>(byt, this._firstDelim);
                        if (length == -1)
                        {
                            this._buff.Add(byt);
                        }
                        else
                        {
                            byte[] byt2 = new byte[checked(length - 1 + 1)];
                            Array.Copy((Array)byt, 0, (Array)byt2, 0, length);
                            this._buff.Add(byt2);
                            byte[] bytes = this._buff.Combine();
                            this._buff.Clear();
                            if (!int.TryParse(Encoding.ASCII.GetString(bytes), out this._sizeComing))
                            {
                                this._firstDelim = (byte)0;
                                this._sizeComing = -1;
                                this._buff.Clear();
                                throw new Exception("Unable to determine size of incoming packet.  It's possible you may need to use Legacy Support.");
                            }
                            byte[] byt3 = new byte[checked(Information.UBound((Array)byt) - (length + 1) + 1)];
                            Array.Copy((Array)byt, checked(length + 1), (Array)byt3, 0, byt3.Length);
                            if (this._sizeComing == 0)
                            {
                                this._sizeComing = -1;
                                this._firstDelim = (byte)0;
                            }
                            else
                                this.ProcessIncoming(byt3, byt3.Length, source_ip, source_port);
                        }
                    }
                }
                else if (this._buff.Count == 0 && Information.UBound((Array)byt) >= this._sizeComing)
                {
                    byte[] numArray = new byte[checked(this._sizeComing + 1)];
                    if (Information.UBound((Array)byt) > this._sizeComing)
                    {
                        Array.Copy((Array)byt, 0, (Array)numArray, 0, numArray.Length);
                        int length = numArray.Length;
                        this.bufferCol.Add((object)numArray, (string)null, (object)null, (object)null);
                        byte[] byt4 = new byte[checked(Information.UBound((Array)byt) - (this._sizeComing + 1) + 1)];
                        Array.Copy((Array)byt, checked(this._sizeComing + 1), (Array)byt4, 0, byt4.Length);
                        this._sizeComing = -1;
                        this._firstDelim = (byte)0;
                        this.m_Monitor.DataReceived(length, source_ip, source_port);
                        this.ProcessIncoming(byt4, byt4.Length, source_ip, source_port);
                    }
                    else
                    {
                        int length = byt.Length;
                        this.bufferCol.Add((object)byt, (string)null, (object)null, (object)null);
                        this._sizeComing = -1;
                        this._firstDelim = (byte)0;
                        this.m_Monitor.DataReceived(length, source_ip, source_port);
                    }
                }
                else if (this._buff.Count > 0 && checked(Information.UBound((Array)this._buff.Combine()) + byt.Length) >= this._sizeComing)
                {
                    this._buff.Add(byt);
                    byte[] numArray1 = this._buff.Combine();
                    this._buff.Clear();
                    byte[] numArray2 = new byte[checked(this._sizeComing + 1)];
                    if (Information.UBound((Array)numArray1) > this._sizeComing)
                    {
                        Array.Copy((Array)numArray1, 0, (Array)numArray2, 0, numArray2.Length);
                        int length = numArray2.Length;
                        this.bufferCol.Add((object)numArray2, (string)null, (object)null, (object)null);
                        byte[] byt5 = new byte[checked(Information.UBound((Array)numArray1) - (this._sizeComing + 1) + 1)];
                        Array.Copy((Array)numArray1, checked(this._sizeComing + 1), (Array)byt5, 0, byt5.Length);
                        this._sizeComing = -1;
                        this._firstDelim = (byte)0;
                        this.m_Monitor.DataReceived(length, source_ip, source_port);
                        this.ProcessIncoming(byt5, byt5.Length, source_ip, source_port);
                    }
                    else
                    {
                        int length = numArray1.Length;
                        this.bufferCol.Add((object)numArray1, (string)null, (object)null, (object)null);
                        this._sizeComing = -1;
                        this._firstDelim = (byte)0;
                        this.m_Monitor.DataReceived(length, source_ip, source_port);
                    }
                }
                else
                    this._buff.Add(byt);
            }
        }

        private byte FreeByte(byte[] byt) => checked((byte)new ByteCollection().FreeByt(byt));

        private byte[] EncloseByte(byte byt, byte[] bytArr)
        {
            int index = checked(Information.UBound((Array)bytArr) + 2);
            byte[] numArray = new byte[checked(index + 1)];
            numArray[0] = byt;
            Array.Copy((Array)bytArr, 0, (Array)numArray, 1, bytArr.Length);
            numArray[index] = byt;
            return numArray;
        }

        private byte[] AppendByte(byte[] first, byte[] sec)
        {
            byte[] numArray = new byte[checked(Information.UBound((Array)first) + sec.Length + 1)];
            Array.Copy((Array)first, 0, (Array)numArray, 0, first.Length);
            Array.Copy((Array)sec, 0, (Array)numArray, checked(Information.UBound((Array)first) + 1), sec.Length);
            return numArray;
        }

        private class SendState
        {
            public int Length;
            public SocketError ErrCode;
            public byte[] Bytes;
            public int StartIndex;
            public int SendLength;
            public int TotalSent;

            [DebuggerNonUserCode]
            public SendState()
            {
            }
        }
    }
}
