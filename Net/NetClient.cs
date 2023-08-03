using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络客户端
    /// </summary>
    public class NetClient<TSession> : INetClient where TSession : IClientSession,new()
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public NetClient() { this.Session = new TSession(); }
        /// <summary>
        /// 设置IP和端口
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="Port">端口</param>
        public NetClient(string IP, int Port = 0)
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
                this.Session = new TSession
                {
                    EndPoint = new IPEndPoint(_ip,port),
                    Path = path,
                    WsType = ws.ToEnum<WebSocketType>(),
                    Host = ip,
                    Port = Port == 0 ? (port == 0 ? 80 : port) : Port
                };
            }
            else
            {
                this.Session = new TSession
                {
                    EndPoint = new IPEndPoint(IPAddress.Parse(IP), Port),
                    Path = "",
                    Port = Port,
                    WsType = WebSocketType.Null,
                    Host = ""
                };
            }
        }
        /// <summary>
        /// 设置IP和端口
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="Port">端口</param>
        public NetClient(IPAddress IP, int Port = 1006)
        {
            this.Session = new TSession
            {
                EndPoint = new IPEndPoint(IP, Port),
                Port = Port,
                Host = "",
                WsType = WebSocketType.Null
            };
        }
        /// <summary>
        /// 设置IP和端口
        /// </summary>
        /// <param name="IpPort">IP和端口</param>
        public NetClient(IPEndPoint IpPort)
        {
            this.Session = new TSession
            {
                EndPoint = IpPort,
                Port = IpPort.Port,
                Host = "",
                Path = "",
                WsType = WebSocketType.Null
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
        public Socket ClientSocket { get; set; }
        /// <summary>
        /// 是否有回车
        /// </summary>
        public Boolean IsNewLine { get; set; } = true;
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
        /// 连接数据
        /// </summary>
        public TSession Session { get; set; }
		/// <summary>
		/// 获取或设置 System.Boolean 值，该值指定流 System.Net.Sockets.Socket 是否正在使用 Nagle 算法。使用 Nagle 算法，则为 false；否则为 true。 默认值为 false。
		/// </summary>
		public Boolean NoDelay { get; set; } = false;
        /// <summary>
        /// 获取或设置一个值，该值指定之后同步 Overload:System.Net.Sockets.Socket.Receive 调用将超时的时间长度。默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
        /// </summary>
        public int ReceiveTimeout { get; set; } = 0;
        /// <summary>
        /// 获取或设置一个值，该值指定之后同步 Overload:System.Net.Sockets.Socket.Send 调用将超时的时间长度。超时值（以毫秒为单位）。 如果将该属性设置为 1 到 499 之间的值，该值将被更改为 500。 默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
        /// </summary>
        public int SendTimeout { get; set; } = 0;
		/// <summary>
		/// 获取或设置一个值，它指定 System.Net.Sockets.Socket 接收缓冲区的大小。System.Int32，它包含接收缓冲区的大小（以字节为单位）。 默认值为 8192。
		/// </summary>
		public int ReceiveBufferSize { get; set; } = 8192;
		/// <summary>
		/// 获取或设置一个值，该值指定 System.Net.Sockets.Socket 发送缓冲区的大小。System.Int32，它包含发送缓冲区的大小（以字节为单位）。 默认值为 8192。
		/// </summary>
		public int SendBufferSize { get; set; } = 8192;
        /// <summary>
        /// 协议版本
        /// </summary>
        public SslProtocols SslProtocols { get; set; }
        /// <summary>
        /// SSL 证书
        /// </summary>
        public X509Certificate Certificate { get; set; }
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
        public event StartEventHandler OnStart;
        /// <summary>
        /// 服务器关闭事件
        /// </summary>
        public event StopEventHandler OnClose;
        #endregion

        #region 方法

        #region 运行
        /// <summary>
        /// 运行
        /// </summary>
        public void Start()
        {
            this.Session.Encoding = this.Encoding;
            this.Session.CancelToken = this.CancelToken;
            this.Session.DataType = this.DataType;
            this.Session.IsNewLine = this.IsNewLine;
            /*设置头信息*/
            if (this.Session.WsType != WebSocketType.Null)
            {
                this.Session.Header = new Dictionary<string, string>
                {
                    {"UserAgent",this.UserAgent.IsNullOrEmpty()?"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36":this.UserAgent },
                    {"WebKey",new RandomHelper().GetRandom(16, RandomType.Letter | RandomType.Number).ToBase64String() },
                    {"Host",this.Session.Host+":"+this.Session.Port.ToString() },
                    {"Origin",this.Origin?? this.Session.EndPoint.Address.ToString() },
                    {"Path",this.Session.Path },
                    {"ws",this.Session.WsType.ToString() }
                };
                if (Cookie != null) this.Session.Header.Add("Cookie", Cookie);
            }
            /*定义一个套接字监听*/
            this.ClientSocket = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType)
            {
                NoDelay = this.NoDelay,
                ReceiveTimeout = this.ReceiveTimeout,
                SendTimeout = this.SendTimeout,
                ReceiveBufferSize = this.ReceiveBufferSize,
                SendBufferSize = this.SendBufferSize
            };
			IPEndPoint endPoint = this.Session.EndPoint;
            try
            {
                /*客户端套接字连接到网络节点上,用的是Connect*/
                this.ClientSocket.Connect(endPoint);
                /*创建握手*/
                if (this.Session.WsType != WebSocketType.Null)
                {
                    this.Send(CreateHeader());
                }
                this.Session.ConnectionSocket = this.ClientSocket;
                /*连接成功事件*/
                OnStart?.Invoke(this, EventArgs.Empty);
                Task.Factory.StartNew(ClientSocket =>
                {
                    /*设置为后台线程,根据主线程销毁而销毁*/
                    Thread.CurrentThread.IsBackground = true;
                    var socket = (TSession)ClientSocket;
                    this.Receives(socket);
                }, this.Session);
            }
            catch (SocketException ex)
            {
                OnError?.Invoke(this, ex);
            }
        }
		/// <summary>
		/// 运行
		/// </summary>
		/// <param name="iPEndPoint">远程host及端口</param>
		public void Start(IPEndPoint iPEndPoint)
        {
            this.Session.EndPoint = iPEndPoint;
            this.Start();
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="iPAddress">远程host</param>
        /// <param name="port">端口</param>
        public void Start(IPAddress iPAddress, int port)
        {
            this.Session.EndPoint = new IPEndPoint(iPAddress, port);
            this.Start();
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="host">远程host</param>
        /// <param name="port">端口</param>
        public void Start(string host, int port)
        {
            this.Session.EndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            this.Start();
        }
        #endregion

        #region 处理连接
        /// <summary>
        /// 处理连接
        /// </summary>
        /// <param name="session">连接</param>
        private void Receives(TSession session)
        {
            if (session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected) return;
            while (!this.CancelToken.IsCancellationRequested)
            {
                if (!session.ConnectionSocket.Connected)
                {
                    if (this.OnDisconnected != null && session != null && session.ConnectionSocket != null && session.ConnectionSocket.Connected && session.ConnectionSocket.RemoteEndPoint != null)
                        this.OnDisconnected?.Invoke(session, EventArgs.Empty);
                    this.CancelToken.Cancel();
                    break;
                }
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    /*接收服务端的数据*/
                    int length = session.ConnectionSocket.Receive(buffer);
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
                        msg = buffer.GetString(session.Encoding, 0, length);
                    }
                    if (session.WsType != WebSocketType.Null && msg.IndexOf("Sec-WebSocket-Accept") == -1)
                    {
                        DataFrame df = new DataFrame(buffer)
                        {
                            Encoding = session.Encoding,
                            DataType = session.DataType
                        };
                        msg = df.Text;
                    }
                    if (this.OnMessage != null) OnMessage.Invoke(session, msg, EventArgs.Empty);
                    if (this.OnMessageByte != null)
                    {
                        byte[] msgBytes = new byte[length];
                        Buffer.BlockCopy(buffer, 0, msgBytes, 0, length);
                        OnMessageByte?.Invoke(session, msgBytes, EventArgs.Empty);
                    }
                }
                catch (SocketException ex)
                {
                    if (this.OnError != null && session.ConnectionSocket != null) this.OnError(this, ex);
                    this.CancelToken.Cancel();
                }
            }
            /*断开连接*/
            if (this.OnDisconnected != null && session != null)
                this.OnDisconnected(session, EventArgs.Empty);
            if (session.ConnectionSocket.Connected)
            {
                session.ConnectionSocket.Shutdown(SocketShutdown.Both);
                session.ConnectionSocket.Disconnect(false);
            }
            session.ConnectionSocket.Close();
            session.ConnectionSocket.Dispose();
        }
        #endregion

        #region 发送信息
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">信息</param>
        public Boolean Send(string msg)
        {
            if (msg.IsNullOrEmpty()) return false;
            try
            {
                return this.Session.Send(msg);
            }
            catch (SocketException ex)
            {
                this.OnError?.Invoke(this, ex);
                return false;
            }
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">信息</param>
        public Boolean Send(byte[] msg)
        {
            if (msg.IsNullOrEmpty() || msg.Length == 0) return false;
            try
            {
                return this.Session.Send(msg);
            }
            catch (SocketException ex)
            {
                this.OnError?.Invoke(this, ex);
                return false;
            }
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public Boolean SendFile(string fileName)
        {
            if (fileName.IsNullOrEmpty()) return false;
            return this.Session.SendFile(fileName);
        }
        #endregion

        #region 创建头信息
        /// <summary>
        /// 创建头信息
        /// </summary>
        /// <returns></returns>
        private string CreateHeader()
        {
            //GET {ws}://{Host}{Path} HTTP/1.1
            this.Session.Headers = @"
Host: {Host}
Connection: Upgrade
Pragma: no-cache
Cache-Control: no-cache
Upgrade: websocket";
            if (this.Session.Header.ContainsKey("Cookie"))
                this.Session.Headers += @"
Cookie: {Cookie}";
            if (this.Session.Header.ContainsKey("Origin"))
                this.Session.Headers += @"
Origin: {Origin}";
            this.Session.Headers += @"
Sec-WebSocket-Version: 13
User-Agent: {UserAgent}
Accept-Encoding: gzip, deflate
Accept-Language: zh-CN,zh;q=0.9
Sec-WebSocket-Key: {WebKey}
Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits
";
            this.Session.Headers = this.Session.Headers.format(this.Session.Header);
            return this.Session.Headers;
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Stop()
        {
            this.CancelToken.Cancel();
            Task.Delay(100).Wait();
            if (this.ClientSocket != null && this.ClientSocket.Connected)
            {
                this.ClientSocket?.Disconnect(false);
                this.ClientSocket?.Close();
                this.ClientSocket?.Dispose();
            }
            this.Session?.TryDispose();
            this.OnClose?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 订阅频道
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channel">频道</param>
        public void Subscribe(string channel)
        {
            Session.AddChannel(channel);
            this.Send("SUBSCRIBE:" + channel);
        }
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channels">频道</param>
        public void Subscribe(IEnumerable<string> channels)
        {
            Session.AddChannel(channels);
            this.Send("SUBSCRIBE:" + channels.Join(","));
        }
        /// <summary>
        /// 取消订阅频道
        /// </summary>
        /// <param name="channel">频道</param>
        public void UnSubscribe(string channel)
        {
            Session.RemoveChannel(channel);
            this.Send("UNSUBSCRIBE:" + channel);
        }
        /// <summary>
        /// 取消订阅频道
        /// </summary>
        /// <param name="channels">频道</param>
        public void UnSubscribe(IEnumerable<string> channels)
        {
            Session.RemoveChannel(channels);
            this.Send("UNSUBSCRIBE:" + channels.Join(","));
        }
        #endregion

        #endregion
    }
}