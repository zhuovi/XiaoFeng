using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XiaoFeng.Sockets
{
    /// <summary>
    /// Socket客户端帮助类
    /// Verstion : 1.0.2
    /// Create Time : 2018/2/5 13:42:40
    /// Update Time : 2018/06/13 09:31:39
    /// </summary>
    [Obsolete("当前类以后被XiaoFeng.Net.NetClient代替")]
    public class SocketClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SocketClient() { this.ClientSocketConnection = new ClientSocketConnection(); }
        /// <summary>
        /// 设置IP和端口
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="Port">端口</param>
        public SocketClient(string IP, int Port = 0)
        {
            if (IP.IsNullOrEmpty()) IP = "127.0.0.1";
            if (IP.IsMatch(@"^wss?://([^:/]+)(:\d+)?((/.*)*)?$"))
            {
                Dictionary<string, string> d = IP.GetMatchs(@"^(?<ws>wss?)://(?<ip>[^:/]+)(:(?<port>\d+))?(?<path>((/.*)*)?)$");
                string ws = d.Value("ws"), ip = d.Value("ip"), path = d.Value("path");
                int port = d.Value("port").ToInt32();
                IPAddress _ip;
                if (ip.IsIP())
                    _ip = IPAddress.Parse(ip);
                else
                {
                    IPHostEntry host = Dns.GetHostEntry(ip);
                    _ip = host.AddressList[0];
                }
                this.ClientSocketConnection = new ClientSocketConnection
                {
                    IPAddress = _ip,
                    Path = path,
                    WsType = ws.ToEnum<WsType>(),
                    Host = ip,
                    Port = Port == 0 ? (port == 0 ? 80 : port) : Port
                };
            }
            else
            {
                this.ClientSocketConnection = new ClientSocketConnection
                {
                    IPAddress = IPAddress.Parse(IP),
                    Path = "",
                    Port = Port,
                    WsType = WsType.Null,
                    Host = ""
                };
            }
        }
        /// <summary>
        /// 设置IP和端口
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="Port">端口</param>
        public SocketClient(IPAddress IP, int Port = 1006)
        {
            this.ClientSocketConnection = new ClientSocketConnection
            {
                IPAddress = IP,
                Port = Port,
                Host = "",
                WsType = WsType.Null
            };
        }
        /// <summary>
        /// 设置IP和端口
        /// </summary>
        /// <param name="IpPort">IP和端口</param>
        public SocketClient(IPEndPoint IpPort)
        {
            this.ClientSocketConnection = new ClientSocketConnection
            {
                IPAddress = IpPort.Address,
                Port = IpPort.Port,
                Host = "",
                Path = "",
                WsType = WsType.Null
            };
            this.AddressFamily = IpPort.AddressFamily;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// Socket数据类型
        /// </summary>
        public SocketDataType DataType { get; set; } = SocketDataType.String;
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// Origin
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 与客户端通信的套接字
        /// </summary>
        private Socket _SocketClient { get; set; }
        /// <summary>
        /// 使用的寻址方案
        /// </summary>
        public virtual AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;
        /// <summary>
        /// 连接类型
        /// </summary>
        public virtual SocketType SocketType { get; set; } = SocketType.Stream;
        /// <summary>
        /// 协议类型
        /// </summary>
        public virtual ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        /// <summary>
        /// 取消通知
        /// </summary>
        private CancellationTokenSource CancelToken = new CancellationTokenSource();
        /// <summary>
        /// 客户端连接数据
        /// </summary>
        public ClientSocketConnection ClientSocketConnection { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event MessageEventHandler OnMessage;
        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event MessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 断开连接事件
        /// </summary>
        public event DisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 出错事件
        /// </summary>
        public event ErrorEventHandler OnError;
        /// <summary>
        /// 服务器启动事件
        /// </summary>
        public event OpenEventHandler OnOpen;
        #endregion

        #region 方法

        #region 运行
        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            /*设置头信息*/
            if (this.ClientSocketConnection.WsType != WsType.Null)
            {
                this.ClientSocketConnection.Header = new Dictionary<string, string>
                {
                    {"UserAgent",this.UserAgent.IsNullOrEmpty()?"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36":this.UserAgent },
                    {"WebKey",new RandomHelper().GetRandom(16, RandomType.Letter | RandomType.Number).ToBase64String() },
                    {"Host",this.ClientSocketConnection.Host+":"+this.ClientSocketConnection.Port.ToString() },
                    {"Origin",this.Origin?? this.ClientSocketConnection.IPAddress.ToString() },
                    {"Path",this.ClientSocketConnection.Path },
                    {"ws",this.ClientSocketConnection.WsType.ToString() }
                };
                if (Cookie != null) this.ClientSocketConnection.Header.Add("Cookie", Cookie);
            }
            /*定义一个套接字监听*/
            this._SocketClient = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType);
            IPEndPoint endPoint = new IPEndPoint(this.ClientSocketConnection.IPAddress, this.ClientSocketConnection.Port);
            try
            {
                /*客户端套接字连接到网络节点上,用的是Connect*/
                this._SocketClient.Connect(endPoint);
                /*创建握手*/
                if (this.ClientSocketConnection.WsType != WsType.Null)
                {
                    this.Send(CreateHeader());
                }
                this.ClientSocketConnection.Socket = this._SocketClient;
                /*连接成功事件*/
                 OnOpen?.Invoke(this.ClientSocketConnection, EventArgs.Empty);
                Task.Factory.StartNew(ClientSocket =>
                {
                    /*设置为后台线程,根据主线程销毁而销毁*/
                    Thread.CurrentThread.IsBackground = true;
                    Socket socket = ClientSocket as Socket;
                    this.Receives(socket);
                }, this._SocketClient);
            }
            catch (SocketException ex)
            {
                OnError?.Invoke(this.ClientSocketConnection, ex);
            }
        }
        #endregion

        #region 处理连接
        /// <summary>
        /// 处理连接
        /// </summary>
        /// <param name="socket">连接</param>
        private void Receives(Socket socket)
        {
            if (socket == null) return;
            while (!this.CancelToken.IsCancellationRequested)
            {
                if (!socket.Connected)
                {
                    if (this.OnDisconnected != null && socket != null && socket.RemoteEndPoint != null)
                        this.OnDisconnected(socket.RemoteEndPoint, EventArgs.Empty);
                    this.CancelToken.Cancel();
                    break;
                }
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    /*接收服务端的数据*/
                    int length = socket.Receive(buffer);
                    if (length == 0)
                    {
                        this.CancelToken.Cancel();
                        break;
                    }
                    string msg;
                    if (this.DataType == SocketDataType.HexString)
                    {
                        msg = buffer.ByteToHexString(0, length);
                    }
                    else
                    {
                        msg = buffer.GetString(this.Encoding, 0, length);
                    }
                    if (this.ClientSocketConnection.WsType != WsType.Null && msg.IndexOf("Sec-WebSocket-Accept") == -1)
                    {
                        DataFrame df = new DataFrame(buffer)
                        {
                            Encoding = this.Encoding,
                            DataType = this.DataType
                        };
                        msg = df.Text;
                    }
                    if (this.OnMessage != null) OnMessage.Invoke(this.ClientSocketConnection, msg, EventArgs.Empty);
                    if (this.OnMessageByte != null)
                    {
                        byte[] msgBytes = new byte[length];
                        Buffer.BlockCopy(buffer, 0, msgBytes, 0, length);
                        OnMessageByte?.Invoke(this.ClientSocketConnection, msgBytes, EventArgs.Empty);
                    }
                }
                catch (SocketException ex)
                {
                    if (this.OnError != null && this.ClientSocketConnection != null) this.OnError(this.ClientSocketConnection, ex);
                    this.CancelToken.Cancel();
                }
            }
            /*断开连接*/
            if (this.OnDisconnected != null && socket != null && socket.RemoteEndPoint != null)
                this.OnDisconnected(socket.RemoteEndPoint, EventArgs.Empty);
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(false);
            }
            socket.Close(); 
            socket.Dispose();
        }
        #endregion

        #region 发送信息
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">信息</param>
        public void Send(string msg)
        {
            if (msg.IsNullOrEmpty()) return;
            this.Send(this.ClientSocketConnection.WsType != WsType.Null &&
                msg.IndexOf("Sec-WebSocket-Key") == -1 ?
                new DataFrame(msg)
                {
                    Encoding = this.Encoding,
                    DataType = this.DataType
                }.GetBytes() :
                this.GetBytes(msg));
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public void SendFile(string fileName)
        {
            if (fileName.IsNullOrEmpty()) return;
            this.SendFile(fileName);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="bytes">信息字节</param>
        private void Send(byte[] bytes)
        {
            if (bytes.Length == 0) return;
            try
            {
                if (this.IsConnected())
                    this._SocketClient.Send(bytes);
            }
            catch (SocketException ex)
            {
                this.OnError?.Invoke(this._SocketClient, ex);
            }
        }
        #endregion

        #region 获取字节组
        /// <summary>
        /// 获取字节组
        /// </summary>
        /// <param name="content">数据</param>
        /// <returns></returns>
        private byte[] GetBytes(string content)
        {
            if (this.DataType == SocketDataType.HexString)
            {
                return content.HexStringToBytes();
            }
            else
            {
                return content.GetBytes(this.Encoding);
            }
        }
        #endregion

        #region 连接状态
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public Boolean IsConnected()
        {
            if (this._SocketClient == null) return false;
            return !(!this._SocketClient.Connected || (this._SocketClient.Poll(1000, SelectMode.SelectRead) && (this._SocketClient.Available == 0)));
        }
        #endregion

        #region 创建头信息
        /// <summary>
        /// 创建头信息
        /// </summary>
        /// <returns></returns>
        private string CreateHeader()
        {//GET {ws}://{Host}{Path} HTTP/1.1
            this.ClientSocketConnection.Headers = @"
Host: {Host}
Connection: Upgrade
Pragma: no-cache
Cache-Control: no-cache
Upgrade: websocket";
            if (this.ClientSocketConnection.Header.ContainsKey("Cookie"))
                this.ClientSocketConnection.Headers += @"
Cookie: {Cookie}";
            if (this.ClientSocketConnection.Header.ContainsKey("Origin"))
                this.ClientSocketConnection.Headers += @"
Origin: {Origin}";
            this.ClientSocketConnection.Headers += @"
Sec-WebSocket-Version: 13
User-Agent: {UserAgent}
Accept-Encoding: gzip, deflate
Accept-Language: zh-CN,zh;q=0.9
Sec-WebSocket-Key: {WebKey}
Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits
";
            this.ClientSocketConnection.Headers = this.ClientSocketConnection.Headers.format(this.ClientSocketConnection.Header);
            return this.ClientSocketConnection.Headers;
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            this.CancelToken.Cancel();
            Task.Delay(100).Wait();
            if (this._SocketClient.Connected)
            {
                this._SocketClient.Shutdown(SocketShutdown.Both);
                this._SocketClient.Disconnect(false);
            }
            this._SocketClient.Close();
            this._SocketClient.Dispose();
        }
        #endregion

        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~SocketClient() { }
        #endregion
    }
}