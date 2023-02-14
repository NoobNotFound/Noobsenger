// Decompiled with JetBrains decompiler
// Type: Winsock2005DLL.Winsock
// Assembly: Winsock2005DLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ECEF2C98-41EF-49F7-98B4-C374A985F23E
// Assembly location: C:\Users\user\Desktop\source\repos\LAN File Sender\LAN File Sender\bin\Debug\Winsock2005DLL.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Winsock2005DLL.My;

namespace Winsock2005DLL
{
    [Designer(typeof(WinsockDesigner), typeof(IDesigner))]
    [DefaultEvent("ErrorReceived")]
    public class Winsock : Component
    {
        private static ArrayList __ENCList = new ArrayList();
        private WinsockMonitor m_Monitor;
        private int m_LocalPort;
        private int m_maxListenPending;
        private WinsockStates m_State;
        private string m_remoteServer;
        private int m_remotePort;
        private bool m_legacy;
        private int m_BufferSize;
        private WinsockProtocols m_Protocol;
        private WinsockIPTypes m_IPProtocol;
        private ISynchronizeInvoke _syncObject;

        [DebuggerNonUserCode]
        static Winsock()
        {
        }

        public Winsock()
        {
            Winsock.__ENCList.Add((object)new WeakReference((object)this));
            this.m_Monitor = new WinsockMonitor(this);
            this.m_State = WinsockStates.Closed;
            this.m_LocalPort = 8080;
            this.m_maxListenPending = 1;
            this.m_remoteServer = "localhost";
            this.m_remotePort = 8080;
            this.m_legacy = false;
            this.m_BufferSize = 8192;
            this.m_Protocol = WinsockProtocols.Tcp;
            this.m_IPProtocol = WinsockIPTypes.IPv4;
        }

        public Winsock(ISynchronizeInvoke Synchronizing_Object)
          : this()
        {
            this._syncObject = Synchronizing_Object;
        }

        public event Winsock.DataArrivalEventHandler DataArrival;

        public event Winsock.SendCompleteEventHandler SendComplete;

        public event Winsock.DisconnectedEventHandler Disconnected;

        public event Winsock.ConnectedEventHandler Connected;

        public event Winsock.ConnectionRequestEventHandler ConnectionRequest;

        public event Winsock.ErrorReceivedEventHandler ErrorReceived;

        public event Winsock.StateChangedEventHandler StateChanged;

        public event Winsock.SendProgressEventHandler SendProgress;

        public int LocalPort
        {
            get => this.m_LocalPort;
            set
            {
                if (this.State() == WinsockStates.Listening)
                    throw new Exception("Cannot change the local port while already listening on a port.");
                this.m_LocalPort = value;
            }
        }

        public int MaxPendingConnections
        {
            get => this.m_maxListenPending;
            set
            {
                if (this.State() == WinsockStates.Listening)
                    throw new Exception("Cannot change the pending connections value while already listening.");
                this.m_maxListenPending = value;
            }
        }

        public string RemoteServer
        {
            get => this.m_remoteServer;
            set
            {
                if (this.State() != WinsockStates.Closed && this.State() != WinsockStates.Listening)
                    throw new Exception("Cannot change the remote address while already connected to a remote server.");
                this.m_remoteServer = value;
            }
        }

        public int RemotePort
        {
            get => this.m_remotePort;
            set
            {
                if (this.State() != WinsockStates.Closed && this.State() != WinsockStates.Listening)
                    throw new Exception("Cannot change the remote port while already connected to a remote server.");
                this.m_remotePort = value;
            }
        }

        public bool LegacySupport
        {
            get => this.m_legacy;
            set => this.m_legacy = value || this.Protocol != WinsockProtocols.Udp ? value : throw new Exception("Cannot disable legacy support when using UDP.");
        }

        [Browsable(false)]
        public bool HasData => this.m_Monitor.CountBuffer() > 0;

        public int BufferSize
        {
            get => this.m_BufferSize;
            set
            {
                if (value < 64)
                    value = 64;
                this.m_BufferSize = value;
            }
        }

        [Browsable(false)]
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                if (this._syncObject == null & this.DesignMode)
                {
                    IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                    if (service != null)
                        this._syncObject = (ISynchronizeInvoke)service.RootComponent;
                }
                return this._syncObject;
            }
            set
            {
                if (this.DesignMode)
                    return;
                this._syncObject = !(this._syncObject != null & this._syncObject != value) ? value : throw new Exception("Property cannot be set at run-time");
            }
        }

        public WinsockProtocols Protocol
        {
            get => this.m_Protocol;
            set
            {
                if (this.State() != WinsockStates.Closed)
                    throw new Exception("Cannot change the protocol while listening or connected to a remote server.");
                this.m_Protocol = value;
                if (value != WinsockProtocols.Udp)
                    return;
                this.LegacySupport = true;
            }
        }

        public WinsockIPTypes IPType
        {
            get => this.m_IPProtocol;
            set
            {
                if (this.State() != WinsockStates.Closed)
                    throw new Exception("Cannot change the IP type while listening or connected to a remote server.");
                this.m_IPProtocol = value;
            }
        }

        public void Listen() => this.m_Monitor.BeginListen(this.m_LocalPort, this.m_maxListenPending);

        public void Listen(int port)
        {
            this.LocalPort = port;
            this.Listen();
        }

        public void Close() => this.m_Monitor.BeginClose();

        public void Accept(Socket client) => this.m_Monitor.DoAccept(client);

        public void Connect() => this.m_Monitor.DoConnect(this.RemoteServer, this.RemotePort);

        public void Connect(string remoteHostOrIP, int remote_Port)
        {
            this.RemoteServer = remoteHostOrIP;
            this.RemotePort = remote_Port;
            this.Connect();
        }

        public string LocalIP() => ((IPAddress)Dns.GetHostEntry(Dns.GetHostName()).AddressList.GetValue(0)).ToString();

        public WinsockStates State() => this.m_State;

        public void Send(byte[] byt) => this.m_Monitor.Send(byt);

        public void Send(string data) => this.m_Monitor.Send(data);

        public void Send(Bitmap data) => this.m_Monitor.Send(data);

        public void SendFile(string filename) => this.m_Monitor.SendFile(filename);

        public void Get(ref string data) => this.m_Monitor.GetData(ref data);

        public void Get(ref Bitmap data)
        {
            MemoryStream memoryStream = new MemoryStream(this.m_Monitor.GetDataFile(), false);
            data = (Bitmap)Image.FromStream((Stream)memoryStream);
            memoryStream.Close();
        }

        public void GetFile(string filename, bool append = false)
        {
            byte[] dataFile = this.m_Monitor.GetDataFile();
            MyProject.Computer.FileSystem.WriteAllBytes(filename, dataFile, append);
        }

        protected internal void OnStateChanged(WinsockStates new_state)
        {
            if (this.m_State == new_state)
                return;
            WinsockStateChangingEventArgs e = new WinsockStateChangingEventArgs(this.m_State, new_state);
            this.m_State = new_state;
            Winsock.dStateChanged dStateChanged = new Winsock.dStateChanged(this.RaiseState);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dStateChanged, new object[2]
                {
          (object) this,
          (object) e
                });
            else
                this.RaiseState((object)this, e);
        }

        protected internal void OnError(
          string msg,
          string func = null,
          SocketError errCode = SocketError.SocketError,
          string exDetails = "")
        {
            WinsockErrorEventArgs e = new WinsockErrorEventArgs(msg, func, errCode, exDetails);
            Winsock.dErrorReceived dErrorReceived = new Winsock.dErrorReceived(this.RaiseError);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dErrorReceived, new object[2]
                {
          (object) this,
          (object) e
                });
            else
                this.RaiseError((object)this, e);
        }

        protected internal void OnConnectionRequest(Socket sock)
        {
            WinsockClientReceivedEventArgs e = new WinsockClientReceivedEventArgs(sock);
            Winsock.dConnectionRequest connectionRequest = new Winsock.dConnectionRequest(this.RaiseConReq);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)connectionRequest, new object[2]
                {
          (object) this,
          (object) e
                });
            else
                this.RaiseConReq((object)this, e);
        }

        protected internal void OnConnected()
        {
            this.OnStateChanged(WinsockStates.Connected);
            Winsock.dConnected dConnected = new Winsock.dConnected(this.RaiseConnected);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dConnected, new object[2]
                {
          (object) this,
          (object) new EventArgs()
                });
            else
                this.RaiseConnected((object)this, new EventArgs());
        }

        protected internal void OnDisconnected()
        {
            Winsock.dDisconnected dDisconnected = new Winsock.dDisconnected(this.RaiseDisconnected);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dDisconnected, new object[2]
                {
          (object) this,
          (object) new EventArgs()
                });
            else
                this.RaiseDisconnected((object)this, new EventArgs());
        }

        protected internal void OnSendComplete(int bytes_sent, int bytes_total)
        {
            WinsockSendEventArgs e = new WinsockSendEventArgs(bytes_sent, bytes_total);
            Winsock.dSendComplete dSendComplete = new Winsock.dSendComplete(this.RaiseSendComplete);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dSendComplete, new object[2]
                {
          (object) this,
          (object) e
                });
            else
                this.RaiseSendComplete((object)this, e);
        }

        protected internal void OnSendProgress(int bytes_sent, int bytes_total)
        {
            WinsockSendEventArgs e = new WinsockSendEventArgs(bytes_sent, bytes_total);
            Winsock.dSendProgress dSendProgress = new Winsock.dSendProgress(this.RaiseSendProgress);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dSendProgress, new object[2]
                {
          (object) this,
          (object) e
                });
            else
                this.RaiseSendProgress((object)this, e);
        }

        protected internal void OnDataArrival(int totalBytes, string source_ip, int source_port)
        {
            WinsockDataArrivalEventArgs e = new WinsockDataArrivalEventArgs(totalBytes, source_ip, source_port);
            Winsock.dDataArrival dDataArrival = new Winsock.dDataArrival(this.RaiseDataArrival);
            if (this._syncObject != null)
                this._syncObject.Invoke((Delegate)dDataArrival, new object[2]
                {
          (object) this,
          (object) e
                });
            else
                this.RaiseDataArrival((object)this, e);
        }

        private void RaiseState(object sender, WinsockStateChangingEventArgs e)
        {
            Winsock.StateChangedEventHandler stateChangedEvent = this.StateChangedEvent;
            if (stateChangedEvent == null)
                return;
            stateChangedEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void RaiseError(object sender, WinsockErrorEventArgs e)
        {
            Winsock.ErrorReceivedEventHandler errorReceivedEvent = this.ErrorReceivedEvent;
            if (errorReceivedEvent == null)
                return;
            errorReceivedEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void RaiseConReq(object sender, WinsockClientReceivedEventArgs e)
        {
            Winsock.ConnectionRequestEventHandler connectionRequestEvent = this.ConnectionRequestEvent;
            if (connectionRequestEvent != null)
                connectionRequestEvent(RuntimeHelpers.GetObjectValue(sender), e);
            if (!e.Cancel)
                return;
            e.Client.Disconnect(false);
            e.Client.Close();
        }

        private void RaiseConnected(object sender, EventArgs e)
        {
            Winsock.ConnectedEventHandler connectedEvent = this.ConnectedEvent;
            if (connectedEvent == null)
                return;
            connectedEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void RaiseDisconnected(object sender, EventArgs e)
        {
            Winsock.DisconnectedEventHandler disconnectedEvent = this.DisconnectedEvent;
            if (disconnectedEvent == null)
                return;
            disconnectedEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void RaiseSendComplete(object sender, WinsockSendEventArgs e)
        {
            Winsock.SendCompleteEventHandler sendCompleteEvent = this.SendCompleteEvent;
            if (sendCompleteEvent == null)
                return;
            sendCompleteEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void RaiseSendProgress(object sender, WinsockSendEventArgs e)
        {
            Winsock.SendProgressEventHandler sendProgressEvent = this.SendProgressEvent;
            if (sendProgressEvent == null)
                return;
            sendProgressEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void RaiseDataArrival(object sender, WinsockDataArrivalEventArgs e)
        {
            Winsock.DataArrivalEventHandler dataArrivalEvent = this.DataArrivalEvent;
            if (dataArrivalEvent == null)
                return;
            dataArrivalEvent(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private delegate void dDataArrival(object sender, WinsockDataArrivalEventArgs e);

        private delegate void dSendComplete(object sender, WinsockSendEventArgs e);

        private delegate void dDisconnected(object sender, EventArgs e);

        private delegate void dConnected(object sender, EventArgs e);

        private delegate void dConnectionRequest(object sender, WinsockClientReceivedEventArgs e);

        private delegate void dErrorReceived(object sender, WinsockErrorEventArgs e);

        private delegate void dStateChanged(object sender, WinsockStateChangingEventArgs e);

        private delegate void dSendProgress(object sender, WinsockSendEventArgs e);

        public delegate void DataArrivalEventHandler(object sender, WinsockDataArrivalEventArgs e);

        public delegate void SendCompleteEventHandler(object sender, WinsockSendEventArgs e);

        public delegate void DisconnectedEventHandler(object sender, EventArgs e);

        public delegate void ConnectedEventHandler(object sender, EventArgs e);

        public delegate void ConnectionRequestEventHandler(
          object sender,
          WinsockClientReceivedEventArgs e);

        public delegate void ErrorReceivedEventHandler(object sender, WinsockErrorEventArgs e);

        public delegate void StateChangedEventHandler(object sender, WinsockStateChangingEventArgs e);

        public delegate void SendProgressEventHandler(object sender, WinsockSendEventArgs e);
    }
}
