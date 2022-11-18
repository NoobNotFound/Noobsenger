// Decompiled with JetBrains decompiler
// Type: Winsock2005DLL.WinsockActionList
// Assembly: Winsock2005DLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ECEF2C98-41EF-49F7-98B4-C374A985F23E
// Assembly location: C:\Users\user\Desktop\source\repos\LAN File Sender\LAN File Sender\bin\Debug\Winsock2005DLL.dll

using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Winsock2005DLL
{
    public enum WinsockIPTypes
    {
        IPv4,
        IPv6,
    }
    public class WinsockErrorEventArgs : EventArgs
    {
        private string m_errorMsg;
        private string m_function;
        private SocketError m_errorCode;
        private string m_Details;

        public WinsockErrorEventArgs(
          string error_message,
          string function_name = null,
          SocketError error_code = SocketError.SocketError,
          string extra_details = "")
        {
            this.m_errorMsg = error_message;
            this.m_function = function_name;
            this.m_errorCode = error_code;
            this.m_Details = extra_details;
        }

        public string Message => this.m_errorMsg;

        public string Function => this.m_function;

        public SocketError ErrorCode => this.m_errorCode;

        public string Details => this.m_Details;
    }
    public class WinsockDesigner : ComponentDesigner
    {
        private DesignerActionListCollection lists;

        public override void Initialize(IComponent component) => base.Initialize(component);

        public override DesignerVerbCollection Verbs => new DesignerVerbCollection();

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this.lists == null)
                {
                    this.lists = new DesignerActionListCollection();
                    this.lists.Add((DesignerActionList)new WinsockActionList(this.Component));
                }
                return this.lists;
            }
        }
    }
    public class WinsockDataArrivalEventArgs : EventArgs
    {
        private int _bTotal;
        private string _IP;
        private int _Port;

        public WinsockDataArrivalEventArgs(int bytes_total, string source_ip, int source_port)
        {
            this._bTotal = bytes_total;
            this._IP = source_ip;
            this._Port = source_port;
        }

        public int TotalBytes => this._bTotal;

        public string SourceIP => this._IP;

        public int SourcePort => this._Port;
    }
    public class WinsockClientReceivedEventArgs : EventArgs
    {
        private Socket _client;
        private bool _cancel;

        public WinsockClientReceivedEventArgs(Socket new_client)
        {
            this._cancel = false;
            this._client = new_client;
        }

        public Socket Client => this._client;

        public string ClientIP => ((IPEndPoint)this._client.RemoteEndPoint).Address.ToString();

        public bool Cancel
        {
            get => this._cancel;
            set => this._cancel = value;
        }
    }
    public class WinsockActionList : DesignerActionList
    {
        private Winsock _wsk;
        private DesignerActionUIService designerActionUISvc;

        public WinsockActionList(IComponent component)
          : base(component)
        {
            this.designerActionUISvc = (DesignerActionUIService)null;
            this._wsk = (Winsock)component;
            this.designerActionUISvc = (DesignerActionUIService)this.GetService(typeof(DesignerActionUIService));
        }

        public bool LegacySupport
        {
            get => this._wsk.LegacySupport;
            set
            {
                this.GetPropertyByName(nameof(LegacySupport)).SetValue((object)this._wsk, (object)value);
                this.designerActionUISvc.Refresh(this.Component);
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems() => new DesignerActionItemCollection()
    {
      (DesignerActionItem) new DesignerActionPropertyItem("LegacySupport", "Legacy Support", "Legacy", "Enables legacy (VB6) send/receive support.")
    };

        private PropertyDescriptor GetPropertyByName(string propName) => TypeDescriptor.GetProperties((object)this._wsk)[propName] ?? throw new ArgumentException("Invalid property.", propName);
    }
}
