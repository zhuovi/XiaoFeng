using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Net.Security;
using System.IO;
using XiaoFeng.IO;
using System.Linq;
using System.Net.NetworkInformation;
using XiaoFeng.Threading;
using System.Security.Cryptography;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-27 16:08:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// Socket客户端
    /// </summary>
    public class SocketClient : Disposable, ISocketClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SocketClient() { }
        /// <summary>
        /// 连接socket
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="authentication">认证</param>
        /// <param name="certificate">ssl证书</param>
        public SocketClient(Socket socket, Func<ISocketClient, bool> authentication = null, X509Certificate certificate = null)
        {
            this.SetSocket(socket, authentication, certificate);
        }
        /// <summary>
        /// 设置连接主机和端口
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        public SocketClient(string host, int port)
        {
            if (host.IsNullOrEmpty() || host.EqualsIgnoreCase("localhost")) host = "127.0.0.1";
           
           foreach(var ip in Dns.GetHostEntry(host).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.EndPoint = new IPEndPoint(ip, port);
                    break;
                }
            }            
            InitializeClientSocket();
        }
        /// <summary>
        /// 设置连接终端
        /// </summary>
        /// <param name="endPoint">终端</param>
        public SocketClient(IPEndPoint endPoint)
        {
            this.EndPoint = endPoint;
            InitializeClientSocket();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 客户端SOCKET
        /// </summary>
        private Socket Client { get; set; }
        ///<inheritdoc/>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        ///<inheritdoc/>
        public SocketType SocketType { get; set; } = SocketType.Stream;
        ///<inheritdoc/>
        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        ///<inheritdoc/>
        public ConnectionType ConnectionType { get; set; } = ConnectionType.Socket;
        ///<inheritdoc/>
        public Boolean NoDelay { get; set; } = false;
        ///<inheritdoc/>
        public int ReceiveTimeout { get; set; } = -1;
        ///<inheritdoc/>
        public int SendTimeout { get; set; } = -1;
        ///<inheritdoc/>
        public int ReceiveBufferSize { get; set; } = 8192;
        ///<inheritdoc/>
        public int SendBufferSize { get; set; } = 8192;
        /// <summary>
        /// 标签数据
        /// </summary>
        private Dictionary<string, object> TagsData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 激活状态
        /// </summary>
        private Boolean _Active = false;
        ///<inheritdoc/>
        public Boolean Active { get => this._Active; }
        ///<inheritdoc/>
        public SocketState SocketState { get; set; } = SocketState.Idle;
        ///<inheritdoc/>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.None;
        ///<inheritdoc/>
        public X509CertificateCollection ClientCertificates { get; set; }
        ///<inheritdoc/>
        private X509Certificate Certificate { get; set; }
        ///<inheritdoc/>
        public IPEndPoint EndPoint { get; set; }
        ///<inheritdoc/>
        public int Available => this.Client?.Available ?? 0;
        ///<inheritdoc/>
        public bool Connected => Client?.Connected ?? false;
        /// <summary>
        /// 是否是服务端
        /// </summary>
        private Boolean _IsServer;
        ///<inheritdoc/>
        public Boolean IsServer => this._IsServer;
        ///<inheritdoc/>
        public string HostName { get; set; }
        ///<inheritdoc/>
        public bool ExclusiveAddressUse
        {
            get { return this.Client?.ExclusiveAddressUse ?? false; }
            set
            {
                if (this.Client != null)
                {
                    this.Client.ExclusiveAddressUse = value;
                }
            }
        }
        /// <summary>
        /// 网络流
        /// </summary>
        private NetworkStream NetStream;
        /// <summary>
        /// SSL 网络流
        /// </summary>
        private SslStream SslStream;
        ///<inheritdoc/>
        public CancellationTokenSource CancelToken { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// 权限认证
        /// </summary>
        private Func<ISocketClient, Boolean> Authentication { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        private Boolean _IsAuthenticated = false;
        ///<inheritdoc/>
        public Boolean? IsAuthenticated => this._IsAuthenticated;
        /// <summary>
        /// 请求头
        /// </summary>
        private string _RequestHeader;
        ///<inheritdoc/>
        public string RequestHeader => this._RequestHeader;
        ///<inheritdoc/>
        public SocketDataType DataType { get; set; } = SocketDataType.String;
        ///<inheritdoc/>
        public Boolean IsPing { get; set; }
        ///<inheritdoc/>
        public int PingTime { get; set; } = 120;
        ///<inheritdoc/>
        private IJob Job { get; set; }
        /// <summary>
        /// 频道KEY
        /// </summary>
        private const string CHANNEL_KEY = "CHANNELS";
        #endregion

        #region 事件
        /// <summary>
        /// 启动
        /// </summary>
        public event OnStartEventHandler OnStart;
        /// <summary>
        /// 停止
        /// </summary>
        public event OnStopEventHandler OnStop;
        /// <summary>
        /// 客户端错误信息
        /// </summary>
        public event OnClientErrorEventHandler OnClientError;
        /// <summary>
        /// 接收消息
        /// </summary>
        public event OnMessageEventHandler OnMessage;
        /// <summary>
        /// 接收消息
        /// </summary>
        public event OnMessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 认证事件
        /// </summary>
        public event OnAuthenticationEventHandler OnAuthentication;
        #endregion

        #region 方法

        #region 获取Socket
        /// <inheritdoc/>
        public Socket GetSocket() => this.Client;
        #endregion

        #region 连接
        ///<inheritdoc/>
        public virtual void Connect() => this.Connect(this.EndPoint);
        ///<inheritdoc/>
        public virtual void Connect(string hostname, int port)
        {
            if (hostname.IsNullOrEmpty()) hostname = "127.0.0.1";
            if (port <= 0) port = 1006;
            if (this.Active)
                throw new Exception("Socket已经连接.");
            IPAddress[] addresses = Dns.GetHostAddresses(hostname);
            ExceptionDispatchInfo lastex = null;

            try
            {
                foreach (IPAddress address in addresses)
                {
                    try
                    {
                        if (this.Client == null)
                        {
                            if ((address.AddressFamily == AddressFamily.InterNetwork && Socket.OSSupportsIPv4) || Socket.OSSupportsIPv6)
                            {
                                this.Client = new Socket(address.AddressFamily, this.SocketType, this.ProtocolType);
                                if (address.IsIPv4MappedToIPv6)
                                {
                                    this.Client.DualMode = true;
                                }
                                try
                                {
                                    this.Client.Connect(address, port);
                                    this.EndPoint = new IPEndPoint(address, port);
                                }
                                catch (Exception ex)
                                {
                                    this.Client = null;
                                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                                    return;
                                }
                            }
                            this._Active = true;
                            this.SocketState = SocketState.Runing;
                            break;
                        }
                        else if (address.AddressFamily == AddressFamily.Unknown)
                        {
                            this.Connect(new IPEndPoint(address, port));
                            this.EndPoint = new IPEndPoint(address, port);
                            this._Active = true;
                            break;
                        }
                    }
                    catch (Exception ex) when (!(ex is OutOfMemoryException))
                    {
                        lastex = ExceptionDispatchInfo.Capture(ex);
                    }
                }
            }
            finally
            {
                if (!this.Active)
                {
                    lastex?.Throw();
                } 
            }
        }
        ///<inheritdoc/>
        public virtual void Connect(IPAddress address, int port)
        {
            if (address == null) IPAddress.Parse("127.0.0.1");
            if (port <= 0) port = 1006;
            IPEndPoint remoteEP = new IPEndPoint(address, port);
            this.Connect(remoteEP);
        }
        ///<inheritdoc/>
        public virtual void Connect(IPEndPoint remoteEP)
        {
            if (remoteEP == null)
            {
                remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1006);
            }
            this.EndPoint = remoteEP;
            this.Client.Connect(remoteEP);
            this._Active = true;
            this.SocketState = SocketState.Runing;
        }
        ///<inheritdoc/>
        public virtual void Connect(IPAddress[] ipAddresses, int port)
        {
            if (ipAddresses == null || ipAddresses.Length == 0) throw new Exception("服务端地址为空.");
            if (port <= 0) port = 1006;

            this.Client.Connect(ipAddresses, port);
            this.EndPoint = this.Client.RemoteEndPoint.ToIPEndPoint();
            this._Active = true;
            this.SocketState = SocketState.Runing;
        }
        ///<inheritdoc/>
        public virtual async Task ConnectAsync(IPAddress address, int port)
        {
            if (address == null) address = IPAddress.Parse("127.0.0.1");
            if (port <= 0) port = 1006;
            await this.CompleteConnectAsync(this.Client.ConnectAsync(address, port));
        }
        ///<inheritdoc/>
        public virtual async Task ConnectAsync() => await this.ConnectAsync(this.EndPoint);
        ///<inheritdoc/>
        public virtual async Task ConnectAsync(string host, int port)
        {
            if (host.IsNullOrEmpty()) host = "127.0.0.1";
            if (port <= 0) port = 1006;
            await CompleteConnectAsync(this.Client.ConnectAsync(host, port));
        }
        ///<inheritdoc/>
        public virtual async Task ConnectAsync(IPAddress[] addresses, int port)
        {
            if (addresses == null || addresses.Length == 0) addresses = new IPAddress[] { IPAddress.Parse("127.0.0.1") };
            if (port <= 0) port = 1006;
            await this.CompleteConnectAsync(this.Client.ConnectAsync(addresses, port));
        }
        ///<inheritdoc/>
        public virtual async Task ConnectAsync(IPEndPoint remoteEP)
        {
            if (remoteEP == null)
            {
                remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1006);
            }
            await this.CompleteConnectAsync(this.Client.ConnectAsync(remoteEP));
        }
        /// <summary>
        /// 连接完成
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        private async Task CompleteConnectAsync(Task task)
        {
            await task.ConfigureAwait(false);
            this.EndPoint = this.Client.RemoteEndPoint.ToIPEndPoint();
            this._Active = true;
            this.SocketState = SocketState.Runing;
            await Task.CompletedTask;
        }
        #endregion

        #region 启动
        ///<inheritdoc/>
        public virtual void Start()
        {
            if (!this.IsServer)
            {
                this.Connect(this.EndPoint);
                this.PingAsync().ConfigureAwait(false);
                this.OnStart?.Invoke(this, EventArgs.Empty);
            }
            Task.Run(() =>
            {
                this.ReceviceDataAsync().ConfigureAwait(false);
            }, this.CancelToken.Token);
        }
        #endregion

        #region 停止
        ///<inheritdoc/>
        public virtual void Stop()
        {
            this.OnStop?.Invoke(this, EventArgs.Empty);
            if (this.Active)
            {
                this._Active = false;
                this.Dispose(true);
            }
        }
        #endregion

        #region 获取网络流
        ///<inheritdoc/>
        public NetworkStream GetStream()
        {
            if (!this.Connected)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, new SocketException((int)SocketError.NotConnected));
                return null;
            }
            if (this.NetStream == null)
            {
                this.NetStream = new NetworkStream(this.Client, true);
            }
            return this.NetStream;
        }
        #endregion

        #region 初如化
        /// <summary>
        /// 初如化
        /// </summary>
        private void InitializeClientSocket()
        {
            if (this.EndPoint.AddressFamily == AddressFamily.Unknown)
            {
                this.Client = new Socket(this.SocketType, this.ProtocolType);
            }
            else
            {
                this.Client = new Socket(this.EndPoint.AddressFamily, this.SocketType, this.ProtocolType);
            }
            if (this.ReceiveTimeout > 0) this.Client.ReceiveTimeout = this.ReceiveTimeout;
            if (this.SendTimeout > 0) this.Client.SendTimeout = this.SendTimeout;
            this.Client.ReceiveBufferSize = this.ReceiveBufferSize;
            this.Client.SendBufferSize = this.SendBufferSize;
            this._IsServer = false;
        }
        #endregion

        #region 获取SslStream
        ///<inheritdoc/>
        public Stream GetSslStream()
        {
            if (this.SslStream != null && this.IsAuthenticated.GetValueOrDefault()) return this.SslStream;
            var stream = this.GetStream();
            if (stream == null) return null;
            if ((this.IsServer && (this.SslProtocols != SslProtocols.None || this.Certificate != null)) || (!this.IsServer && (this.HostName.IsNotNullOrEmpty() || this.SslProtocols != SslProtocols.None && this.Certificate != null)))
            {
                var sslStream = new SslStream(stream, false, (sender, cert, chain, error) =>
                {
                    return error == SslPolicyErrors.None;
                });
                if (IsServer)
                {
                    try
                    {
                        if (this.SslProtocols == SslProtocols.None)
                        {
#if NETSTANDARD2_1
                            sslStream.AuthenticateAsServer(this.Certificate, clientCertificateRequired: false, checkCertificateRevocation: true);
#else
                            sslStream.AuthenticateAsServer(this.Certificate);
#endif
                        }
                        else
                            sslStream.AuthenticateAsServer(this.Certificate, false, this.SslProtocols, true);
                    }
                    catch (SocketException ex)
                    {
                        this.OnClientError?.Invoke(this, this.EndPoint, ex);
                        return null;
                    }
                    catch (IOException ex)
                    {
                        if (ex.InnerException is SocketException err)
                            this.OnClientError?.Invoke(this, this.EndPoint, err);
                        else
                            this.OnClientError?.Invoke(this, this.EndPoint, ex);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is SocketException err)
                            this.OnClientError?.Invoke(this, this.EndPoint, err);
                        else
                            this.OnClientError?.Invoke(this, this.EndPoint, ex);
                        return null;
                    }
                }
                else
                {
                    if (this.HostName.IsNullOrEmpty()) this.HostName = this.EndPoint.Address.ToString();
                    try
                    {
                        if (this.SslProtocols == SslProtocols.None)
                        {
                            sslStream.AuthenticateAsClient(this.HostName);
                        }
                        else
                        {
                            if (this.ClientCertificates == null) this.ClientCertificates = new X509CertificateCollection();
                            sslStream.AuthenticateAsClient(this.HostName, this.ClientCertificates, this.SslProtocols, false);
                        }
                    }
                    catch (SocketException ex)
                    {
                        this.OnClientError?.Invoke(this, this.EndPoint, ex);
                        return null;
                    }
                    catch (IOException ex)
                    {
                        if (ex.InnerException is SocketException err)
                            this.OnClientError?.Invoke(this, this.EndPoint, err);
                        else
                            this.OnClientError?.Invoke(this, this.EndPoint, new SocketException((int)SocketError.SocketError));
                        return null;
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is SocketException err)
                            this.OnClientError?.Invoke(this, this.EndPoint, err);
                        else
                            this.OnClientError?.Invoke(this, this.EndPoint, new SocketException((int)SocketError.SocketError));
                    }
                }
                if (sslStream.IsAuthenticated)
                {
                    sslStream.ReadTimeout = this.ReceiveTimeout;
                    sslStream.WriteTimeout = this.SendTimeout;
                    this.SslStream = sslStream;
                    this._IsAuthenticated = true;
                    return sslStream;
                }
                else
                    return stream;
            }
            return stream;
        }
        #endregion

        #region 接收数据
        ///<inheritdoc/>
        public virtual async Task<int?> ReceviceByteAsync()
        {
            var stream = this.GetSslStream();
            if (stream == null) return null;
            try
            {
                return await Task.FromResult(stream.ReadByte());
            }
            catch (SocketException ex)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return null;
            }
            catch (IOException ex)
            {
                if (ex.InnerException is SocketException err)
                {
                    this.OnClientError?.Invoke(this, this.EndPoint, err);
                    this.OnStop?.Invoke(this, EventArgs.Empty);
                }
                else
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return null;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SocketException err)
                    this.OnClientError?.Invoke(this, this.EndPoint, err);
                else
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return null;
            }
        }
        ///<inheritdoc/>
        public virtual async Task<byte[]> ReceviceMessageAsync(byte[] bytes, int offset = -1, int count = -1)
        {
            if (bytes == null || bytes.Length == 0 || bytes.Length <= offset || bytes.Length <= count + offset) return Array.Empty<byte>();
            if (offset < 0) offset = 0;
            if (count <= 0) count = bytes.Length - offset;
            var stream = this.GetSslStream();
            if (stream == null) return Array.Empty<byte>();
            try
            {
                var dataBuffer = new byte[count];
                var readsize = await stream.ReadAsync(dataBuffer, 0, count, this.CancelToken.Token);
                Array.Copy(dataBuffer, 0, bytes, offset, readsize);
            }
            catch (SocketException ex)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return Array.Empty<byte>();
            }
            catch (IOException ex)
            {
                if (ex.InnerException is SocketException err)
                {
                    this.OnClientError?.Invoke(this, this.EndPoint, err);
                    this.OnStop?.Invoke(this, EventArgs.Empty);
                }
                else
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SocketException err)
                    this.OnClientError?.Invoke(this, this.EndPoint, err);
                else
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return Array.Empty<byte>();
            }
            return bytes;
        }
        /// <inheritdoc/>
        public virtual async Task<byte[]> ReceviceMessageAsync()
        {
            var stream = this.GetSslStream();
            if (stream == null) return Array.Empty<byte>();
            int readsize = -1;
            var buffer = new MemoryStream();
            var dataBuffer = new byte[this.ReceiveBufferSize];
            do
            {
                try
                {
                    readsize = await stream.ReadAsync(dataBuffer, 0, this.ReceiveBufferSize, this.CancelToken.Token);
                    await buffer.WriteAsync(dataBuffer, 0, readsize, this.CancelToken.Token);
                }
                catch (SocketException ex)
                {
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                    break;
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException err)
                    {
                        this.OnClientError?.Invoke(this, this.EndPoint, err);
                        this.OnStop?.Invoke(this, EventArgs.Empty);
                    }
                    else
                        this.OnClientError?.Invoke(this, this.EndPoint, ex);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is SocketException err)
                        this.OnClientError?.Invoke(this, this.EndPoint, err);
                    else
                        this.OnClientError?.Invoke(this, this.EndPoint, ex);
                    break;
                }
            } while (readsize > 0 && this.GetStream().DataAvailable);

            if (buffer.Length == 0) return Array.Empty<byte>();
            var bytes = buffer.ToArray();
            buffer.Close();
            buffer.Dispose();
            return bytes;
        }
        #endregion

        #region 循环接收数据
        ///<inheritdoc/>
        public virtual async Task ReceviceDataAsync()
        {
            var HandshakesCount = 0;
            do
            {
                var bytes = await ReceviceMessageAsync().ConfigureAwait(false);
                if (bytes.Length == 0)
                {
                    //断开了
                    break;
                }
                string ReceiveMessage;
                if (this.IsServer)
                {
                    if (HandshakesCount == 0)
                    {
                        ReceiveMessage = bytes.GetString(this.Encoding);
                        if (ReceiveMessage.IndexOf("Sec-WebSocket-Key", StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            this.ConnectionType = ConnectionType.WebSocket;
                            this._RequestHeader = ReceiveMessage;
                            if(this.IsServer && this is IWebSocketClient webSocket)
                            {
                                webSocket.Request = new WebSocketRequest(webSocket.HostName.IsNullOrEmpty() ? "ws" : "wss", this.RequestHeader);
                            }
                        }
                        else
                        {
                            this.ConnectionType = ConnectionType.Socket;
                        }
                        //认证
                        this._IsAuthenticated = this.Authentication(this);
                        if (!this.IsAuthenticated.GetValueOrDefault())
                        {
                            var msg = "认证失败.";
                            var msgBytes = msg.GetBytes(this.Encoding);
                            await this.SendAsync(msgBytes);
                            this.OnAuthentication?.Invoke(this, msg, EventArgs.Empty);
                            break;
                        }
                        if (this.IsServer)
                            this.OnStart?.Invoke(this, EventArgs.Empty);
                        if (this.ConnectionType == ConnectionType.WebSocket)
                        {
                            //开始握手
                            var packet = new WebSocketPacket(this, ReceiveMessage)
                            {
                                Encoding = this.Encoding,
                                DataType = this.DataType
                            };
                           var Handshake = packet.HandshakeAsync();
                            await Handshake;
                            
                            HandshakesCount++;
                            continue;
                        }
                    }
                    if (!this.IsAuthenticated.HasValue || !this.IsAuthenticated.GetValueOrDefault())
                    {
                        var msg = "请先认证后再通讯.";
                        var msgBytes = msg.GetBytes(this.Encoding);
                        await this.SendAsync(msgBytes);
                        HandshakesCount = 0;
                        continue;
                    }
                }
                if (this.ConnectionType == ConnectionType.WebSocket)
                {
                    var packet = new WebSocketPacket(this);
                    bytes = packet.UnPacket(bytes);
                    if (packet.OpCode == OpCode.Close) break;
                    if (packet.OpCode == OpCode.Pong)
                    {
                        if (bytes.Length == 0) continue;
                    }
                    if(packet.OpCode == OpCode.Ping)
                    {
                        SendPongAsync().ConfigureAwait(false);
                        if (bytes.Length == 0) continue;
                    }
                }
                ReceiveMessage = this.DataType == SocketDataType.String ? bytes.GetString(this.Encoding) : bytes.ByteToHexString();

                if (ReceiveMessage.StartsWith("SUBSCRIBE#", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = ReceiveMessage.RemovePattern("^SUBSCRIBE#");
                    var index = msg.IndexOf(":");
                    if (index > -1)
                    {
                        var type = msg.Substring(0, index);
                        var data = msg.Substring(index + 1);
                        switch (type.ToUpper())
                        {
                            case "ADD":
                                this.AddChannel(data.Split(',', StringSplitOptions.RemoveEmptyEntries));
                                ReceiveMessage = "订阅频道:" + data;
                                break;
                            case "DEL":
                                this.RemoveChannel(data.Split(',', StringSplitOptions.RemoveEmptyEntries));
                                ReceiveMessage = "取消订阅频道:" + data;
                                break;
                            case "CLS":
                                this.ClearChannel();
                                ReceiveMessage = "取消所有订阅频道";
                                break;
                        }
                        bytes = ReceiveMessage.GetBytes(this.Encoding);
                    }
                }

                Task.Run(() => this.OnMessage?.Invoke(this, ReceiveMessage, EventArgs.Empty));
                Task.Run(() => this.OnMessageByte?.Invoke(this, bytes, EventArgs.Empty));

            } while (!this.CancelToken.IsCancellationRequested && this.Connected);
            this.Stop();
        }
        #endregion

        #region 发送数据
        ///<inheritdoc/>
        public virtual void SendPing(byte[] buffers = null) => this.NetStreamSend(buffers, OpCode.Ping);
        ///<inheritdoc/>
        public virtual async Task SendPingAsync(byte[] buffers = null) => await this.NetStreamSendAsync(buffers, OpCode.Ping);
        ///<inheritdoc/>
        public virtual void SendPong(byte[] buffers = null) => this.NetStreamSend(buffers, OpCode.Pong);
        ///<inheritdoc/>
        public virtual async Task SendPongAsync(byte[] buffers = null) => await this.NetStreamSendAsync(buffers, OpCode.Pong);
        ///<inheritdoc/>
        public virtual int Send(byte[] buffers)
        {
            if (buffers.IsNullOrEmpty() || buffers.Length == 0) return 0;
            return this.NetStreamSend(buffers);
        }
        ///<inheritdoc/>
        public virtual async Task<int> SendAsync(byte[] buffers)
        {
            if (buffers.IsNullOrEmpty() || buffers.Length == 0) return 0;
            return await this.NetStreamSendAsync(buffers);
        }
        ///<inheritdoc/>
        public virtual int Send(string message)
        {
            if (message.IsNullOrEmpty()) return 0;
            return this.Send(this.DataType == SocketDataType.String ? message.GetBytes(this.Encoding) : message.HexStringToBytes());
        }
        ///<inheritdoc/>
        public virtual async Task<int> SendAsync(string message)
        {
            if (message.IsNullOrEmpty()) return 0;
            return await this.SendAsync(this.DataType == SocketDataType.String ? message.GetBytes(this.Encoding) : message.HexStringToBytes());
        }
        ///<inheritdoc/>
        public virtual void SendFile(string fileName)
        {
            if (fileName.IsNullOrEmpty()) return;
            if (!FileHelper.Exists(fileName)) return;
            var buffers = FileHelper.OpenBytes(fileName);
            this.Send(buffers);
        }
        ///<inheritdoc/>
        public virtual async Task SendFileAsync(string fileName)
        {
            if (fileName.IsNullOrEmpty()) return;
            if (!FileHelper.Exists(fileName)) return;
            var buffers = FileHelper.OpenBytes(fileName);
            await this.SendAsync(buffers);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="opCode">操作码</param>
        private int NetStreamSend(byte[] buffers, OpCode opCode = OpCode.Text)
        {
            if (!this.Connected)
            {
                this.OnClientError?.Invoke(this,this.EndPoint, new SocketException((int)SocketError.NotConnected));
                return -1;
            }
            if ((opCode == OpCode.Text || opCode == OpCode.Binary) && (buffers == null || buffers.Length == 0)) return 0;
            var stream = this.GetSslStream();
            if (stream == null) return -1;
            if (this.ConnectionType == ConnectionType.WebSocket)
            {
                using (var packet = new WebSocketPacket(this))
                    buffers = packet.Packet(buffers, opCode);
            }
            else
            {
                
            }
            stream.Write(buffers, 0, buffers.Length);
            stream.Flush();
            return buffers.Length;
        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="opCode">操作码</param>
        /// <returns>发送数据长度</returns>
        private async Task<int> NetStreamSendAsync(byte[] buffers, OpCode opCode = OpCode.Text)
        {
            if (!this.Connected)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, new SocketException((int)SocketError.NotConnected));
                return -1;
            }
            if ((opCode == OpCode.Text || opCode == OpCode.Binary) && (buffers == null || buffers.Length == 0)) return 0;
            var stream = this.GetSslStream();
            if (stream == null) return -1;
            if (this.ConnectionType == ConnectionType.WebSocket)
            {
                using (var packet = new WebSocketPacket(this))
                    buffers = packet.Packet(buffers, opCode);
            }
            await stream.WriteAsync(buffers, 0, buffers.Length, this.CancelToken.Token);
            await stream.FlushAsync(this.CancelToken.Token);
            return await Task.FromResult(buffers.Length);
        }
        #endregion

        #region 事件回调
        ///<inheritdoc/>
        public void StartEventHandler() => OnStart?.Invoke(this, EventArgs.Empty);
        ///<inheritdoc/>
        public void StopEventHandler() => OnStop?.Invoke(this, EventArgs.Empty);
        ///<inheritdoc/>
        public void ClientErrorEventHandler(IPEndPoint endPoint, Exception e) => OnClientError?.Invoke(this, endPoint, e);
        ///<inheritdoc/>
        public void MessageEventHandler(string message) => OnMessage?.Invoke(this, message, EventArgs.Empty);
        ///<inheritdoc/>
        public void MessageByteEventHandler(byte[] message) => OnMessageByte?.Invoke(this, message, EventArgs.Empty);
        ///<inheritdoc/>
        public void AuthenticationEventHandler(string message) => OnAuthentication?.Invoke(this, message, EventArgs.Empty);
        #endregion

        #region 设置Socket
        ///<inheritdoc/>
        public void SetSocket(Socket socket, Func<ISocketClient, bool> authentication = null, X509Certificate certificate = null)
        {
            this.Client = socket;
            this._Active = true;
            this.SocketState = SocketState.Runing;
            this.EndPoint = socket.RemoteEndPoint as IPEndPoint;
            this._IsServer = true;
            if (authentication == null) authentication = c => true;
            this.Authentication = authentication;
            this.Certificate = certificate;
        }
        #endregion

        #region 处理定义数据
        ///<inheritdoc/>
        public virtual void AddChannel(params string[] channel)
        {
            if (this.TagsData == null) this.TagsData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var list = new List<string>();
            if (!this.TagsData.ContainsKey(CHANNEL_KEY))
            {
                this.TagsData.Add(CHANNEL_KEY, list);
            }
            else
                list = this.TagsData[CHANNEL_KEY] as List<string>;
            channel.Each(c =>
            {
                if (!list.Contains(c, StringComparer.OrdinalIgnoreCase))
                    list.Add(c);
            });
            if (!this.IsServer)
            {
                this.SendPing($"SUBCRIBE#ADD:{channel.Join(",")}".GetBytes(this.Encoding));
            }
        }
        ///<inheritdoc/>
        public virtual void RemoveChannel(params string[] channel)
        {
            if (this.TagsData == null) { this.TagsData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); return; }
            if (!this.TagsData.ContainsKey(CHANNEL_KEY)) return;
            if (this.TagsData.TryGetValue(CHANNEL_KEY, out var cs))
            {
                var list = cs as List<string>;
                channel.Each(c => list.Remove(c));
                if (!this.IsServer)
                {
                    this.SendPing($"SUBCRIBE#DEL:{channel.Join(",")}".GetBytes(this.Encoding));
                }
            }
        }
        ///<inheritdoc/>
        public virtual void ClearChannel()
        {
            if (this.TagsData.ContainsKey(CHANNEL_KEY))
            {
                this.TagsData[CHANNEL_KEY] = new List<string>();
                if (!this.IsServer)
                {
                    this.SendPing($"SUBCRIBE#CLS:".GetBytes(this.Encoding));
                }
            }
        }
        ///<inheritdoc/>
        public virtual IList<string> GetChannel()
        {
            if (this.TagsData == null) { this.TagsData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); return null; }
            if (!this.TagsData.ContainsKey(CHANNEL_KEY)) return null;
            if (this.TagsData.TryGetValue(CHANNEL_KEY, out var cs))
                return cs as List<string>;
            return null;
        }
        ///<inheritdoc/>
        public virtual Boolean ContainsChannel(params string[] channel)
        {
            if (channel.IsNotNullOrEmpty() && this.TagsData != null && this.TagsData.Count > 0)
            {
                if (this.TagsData.TryGetValue(CHANNEL_KEY, out var cs))
                {
                    if (cs.IsNotNullOrEmpty())
                    {
                        var list = cs as List<string>;
                        foreach (var c in channel)
                            if (list.Contains(c, StringComparer.OrdinalIgnoreCase)) return true;
                    }
                }
            }
            return false;
        }
        ///<inheritdoc/>
        public virtual object GetData(string key)
        {
            if (this.TagsData.TryGetValue(key, out var data))
                return data;
            else return null;
        }
        ///<inheritdoc/>
        public virtual void AddData(string key, object value)
        {
            if (key.IsNullOrEmpty() || key.EqualsIgnoreCase(CHANNEL_KEY)) return;
            if (this.TagsData == null) this.TagsData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (this.ContainsData(key))
                this.TagsData[key] = value;
            else this.TagsData.Add(key, value);
        }
        ///<inheritdoc/>
        public virtual void RemoveData(string key)
        {
            if (this.ContainsData(key))
                this.TagsData.Remove(key);
        }
        ///<inheritdoc/>
        public virtual void ClearData() => this.TagsData?.Clear();
        ///<inheritdoc/>
        public virtual Boolean ContainsData(string key) => key.IsNotNullOrEmpty() && this.TagsData != null && this.TagsData.Count != 0 && this.TagsData.ContainsKey(key);
        #endregion

        #region 运行ping作业
        /// <summary>
        /// 启动ping作业
        /// </summary>
        /// <returns></returns>
        public Task PingAsync()
        {
            if (!this.IsPing || this.IsServer) return Task.CompletedTask;
            if (this.PingTime <= 3) this.PingTime = 10;
            this.Job = new Job().SetName("SocketClient自动Ping作业").Interval(this.PingTime * 1000).SetCompleteCallBack(job =>
            {
                this.SendPingAsync(new
                {
                    type = "ping",
                    time = DateTime.Now.ToTimeStamp()
                }.ToJson().GetBytes(this.Encoding)).ConfigureAwait(false);
            });
            this.Job.Start();
            return Task.CompletedTask;
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">状态</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsServer)
                this.CancelToken.Cancel(true);
            if (this.Client != null)
            {
                if (this.Client.Connected)
                {
                    this.Client.Shutdown(SocketShutdown.Both);
                    this.Client.Close(3);
                }
                this.Client.Dispose();
                this.Client = null;
            }
            this.NetStream?.Close();
            this.NetStream?.Dispose();
            this.NetStream = null;
            this.SslStream?.Close();
            this.SslStream?.Dispose();
            this.SslStream = null;
            this._Active = false;
            this.CancelToken = new CancellationTokenSource();
            if (this.Job != null)
            {
                this.Job.Stop();
                this.Job = null;
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~SocketClient()
        {
            this.Dispose(false);
        }
        #endregion

        #endregion
    }
}