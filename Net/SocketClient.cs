﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.IO;
using XiaoFeng.Threading;

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
    public class SocketClient : BaseSocket, ISocketClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SocketClient() { this.ClientConnectionId = Guid.NewGuid(); }
        /// <summary>
        /// 连接socket
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="networkDelay">网络延时 单位为毫秒</param>
        /// <param name="authentication">认证</param>
        /// <param name="certificate">ssl证书</param>
        internal SocketClient(Socket socket, int networkDelay, Func<ISocketClient, bool> authentication = null, X509Certificate certificate = null) : this()
        {
            this.SetSocket(socket, networkDelay, authentication, certificate);
        }
        /// <summary>
        /// 设置连接主机和端口
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        public SocketClient(string host, int port) : this()
        {
            if (host.IsNullOrEmpty() || host.EqualsIgnoreCase("localhost")) host = "127.0.0.1";
            if (!ValidatePortNumber(port))
            {
                InitializeClientSocket(); return;
            }
            if (IPAddress.TryParse(host, out var address))
                this.EndPoint = new IPEndPoint(address, port);
            else
            {
                try
                {
                    var addrs = Dns.GetHostAddresses(host);
                    this.EndPoint = new IPEndPoint(addrs.First(a => (a.AddressFamily == AddressFamily.InterNetwork && Socket.OSSupportsIPv4) || Socket.OSSupportsIPv6), port);
                }
                catch { }
            }
            InitializeClientSocket();
        }
        /// <summary>
        /// 设置连接终端
        /// </summary>
        /// <param name="endPoint">终端</param>
        public SocketClient(IPEndPoint endPoint) : this()
        {
            this.EndPoint = endPoint;
            InitializeClientSocket();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 客户端连接Id
        /// </summary>
        public Guid ClientConnectionId { get; private set; }
        /// <summary>
        /// 客户端SOCKET
        /// </summary>
        private Socket Client { get; set; }
        ///<inheritdoc/>
        public ConnectionType ConnectionType { get; set; } = ConnectionType.Socket;
        /// <summary>
        /// 标签数据
        /// </summary>
        private Dictionary<string, object> TagsData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        ///<inheritdoc/>
        public X509CertificateCollection ClientCertificates { get; set; }
        ///<inheritdoc/>
        private X509Certificate Certificate { get; set; }
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
        public override Boolean ExclusiveAddressUse
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
        /// <summary>
        /// 权限认证
        /// </summary>
        private Func<ISocketClient, Boolean> Authentication { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        private Boolean? _IsAuthenticated = false;
        ///<inheritdoc/>
        public Boolean IsAuthenticated => this._IsAuthenticated.GetValueOrDefault();
        /// <summary>
        /// 请求头
        /// </summary>
        private string _RequestHeader;
        ///<inheritdoc/>
        public string RequestHeader => this._RequestHeader;
        ///<inheritdoc/>
        public Boolean IsPing { get; set; }
        ///<inheritdoc/>
        public int PingTime { get; set; } = 120;
        ///<inheritdoc/>
        private IJob Job { get; set; }
        ///<inheritdoc/>
        public DateTime LastMessageTime { get; set; }
        ///<inheritdoc/>
        public DateTime ConnectedTime { get; set; }
        /// <summary>
        /// 频道KEY
        /// </summary>
        private const string CHANNEL_KEY = "CHANNELS";
        /// <summary>
        /// 流异步锁
        /// </summary>
        readonly AsyncLock StreamAsyncLock = new AsyncLock();
        /// <summary>
        /// 流锁
        /// </summary>
        readonly object StreamLock = new object();
        #endregion

        #region 事件
        ///<inheritdoc/>
        public override event OnStartEventHandler OnStart;
        ///<inheritdoc/>
        public override event OnStopEventHandler OnStop;
        ///<inheritdoc/>
        public override event OnClientErrorEventHandler OnClientError;
        ///<inheritdoc/>
        public override event OnMessageEventHandler OnMessage;
        ///<inheritdoc/>
        public override event OnMessageByteEventHandler OnMessageByte;
        ///<inheritdoc/>
        public override event OnAuthenticationEventHandler OnAuthentication;
        #endregion

        #region 方法

        #region 获取Socket
        /// <inheritdoc/>
        public override Socket GetSocket() => this.Client;
        #endregion

        #region 连接
        ///<inheritdoc/>
        public virtual SocketError Connect() => this.Connect(this.EndPoint);
        ///<inheritdoc/>
        public virtual SocketError Connect(string hostname, int port)
        {
            if (hostname.IsNullOrEmpty()) hostname = "127.0.0.1";
            if (!ValidatePortNumber(port)) port = 1006;
            if (this.Active)
                return SocketError.AddressAlreadyInUse;
            
            if (IPAddress.TryParse(hostname, out var address))
                return this.Connect(address, port);

            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(hostname);
                return this.Connect(addresses, port);
            }
            catch
            {
                this.OnClientError?.Invoke(this, this.EndPoint, new Exception("网络错误."));
                return SocketError.NetworkDown;
            }
        }
        ///<inheritdoc/>
        public virtual SocketError Connect(IPAddress address, int port)
        {
            if (address == null) IPAddress.Parse("127.0.0.1");
            if (!ValidatePortNumber(port)) port = 1006;
            var remoteEP = new IPEndPoint(address, port);
            return this.Connect(remoteEP);
        }
        ///<inheritdoc/>
        public virtual SocketError Connect(IPEndPoint remoteEP)
        {
#if NETSTANDARD2_0
            if (remoteEP == null) remoteEP =
#else
            remoteEP ??=
#endif
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1006);

            this.EndPoint = remoteEP;
            return this.CompleteConnect(() =>
            {
                try
                {
                    if (this.ConnectTimeout <= 0)
                        this.ConnectTimeout = 10_000;
                    var result = this.Client.BeginConnect(this.EndPoint, null, null);
                    if (!result.AsyncWaitHandle.WaitOne(Math.Max(ConnectTimeout, 500), true))
                    {
                        this.OnClientError?.Invoke(this, this.EndPoint, new Exception());
                        return SocketError.TimedOut;
                    }
                    this.Client.EndConnect(result);
                    return SocketError.Success;
                }
                catch (Exception ex)
                {
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                    return SocketError.SocketError;
                }
            });
        }
        ///<inheritdoc/>
        public virtual SocketError Connect(IPAddress[] ipAddresses, int port)
        {
            if (ipAddresses == null || ipAddresses.Length == 0) throw new Exception("服务端地址为空.");
            if (!ValidatePortNumber(port)) port = 1006;
            
            return this.CompleteConnect(() =>
            {
                ExceptionDispatchInfo lastex = null;

                try
                {
                    foreach (IPAddress address in ipAddresses)
                    {
                        this.EndPoint = new IPEndPoint(address, port);
                        InitializeClientSocket();
                        try
                        {
                            if (address.AddressFamily == AddressFamily.Unknown)
                            {
                                return this.Connect(this.EndPoint);
                            }
                            else
                            {
                                if ((address.AddressFamily == AddressFamily.InterNetwork && Socket.OSSupportsIPv4) || Socket.OSSupportsIPv6)
                                {
                                    if (address.IsIPv4MappedToIPv6)
                                    {
                                        this.Client.DualMode = true;
                                    }
                                    try
                                    {
                                        if (this.ConnectTimeout <= 0)
                                            this.ConnectTimeout = 10_000;

                                        var result = this.Client.BeginConnect(address, port, null, null);
                                        if (!result.AsyncWaitHandle.WaitOne(Math.Max(ConnectTimeout, 500), true))
                                        {
                                            this.OnClientError?.Invoke(this, this.EndPoint, new Exception());
                                            return SocketError.TimedOut;
                                        }
                                        this.Client.EndConnect(result);
                                        this._Active = true;
                                        return SocketError.Success;
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Client = null;
                                        this.OnClientError?.Invoke(this, this.EndPoint, ex);
                                    }
                                }
                                break;
                            }
                        }
                        catch (Exception ex) when (!(ex is OutOfMemoryException))
                        {
                            lastex = ExceptionDispatchInfo.Capture(ex);
                        }
                    }
                    return SocketError.SocketError;
                }
                finally
                {
                    if (!this.Active)
                    {
                        lastex?.Throw();
                    }
                }
            });
        }
        ///<inheritdoc/>
        public virtual async Task<SocketError> ConnectAsync(IPAddress address, int port)
        {
#if NETSTANDARD2_0
            if (address == null) address =
#else
            address ??=
#endif
            IPAddress.Parse("127.0.0.1");
            if (!ValidatePortNumber(port)) port = 1006;
            var remoteEP = new IPEndPoint(address, port);
            return await this.ConnectAsync(remoteEP).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public virtual async Task<SocketError> ConnectAsync() => await this.ConnectAsync(this.EndPoint).ConfigureAwait(false);
        ///<inheritdoc/>
        public virtual async Task<SocketError> ConnectAsync(string host, int port)
        {
            if (host.IsNullOrEmpty()) host = "127.0.0.1";
            if (!ValidatePortNumber(port)) port = 1006;
            if (this.Active)
                return await Task.FromResult(SocketError.AddressAlreadyInUse);

            if (IPAddress.TryParse(host, out var address))
                return await this.ConnectAsync(address, port);

            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                return await this.ConnectAsync(addresses, port);
            }
            catch
            {
                this.OnClientError?.Invoke(this, this.EndPoint, new Exception("网络错误."));
                return await Task.FromResult(SocketError.NetworkDown);
            }
        }
        ///<inheritdoc/>
        public virtual async Task<SocketError> ConnectAsync(IPAddress[] addresses, int port)
        {
            if (addresses == null || addresses.Length == 0) addresses = new IPAddress[] { IPAddress.Parse("127.0.0.1") };
            if (!ValidatePortNumber(port)) port = 1006;
            return await this.CompleteConnectAsync(async () =>
            {
                ExceptionDispatchInfo lastex = null;

                try
                {
                    foreach (IPAddress address in addresses)
                    {
                        this.EndPoint = new IPEndPoint(address, port);
                        InitializeClientSocket();
                        try
                        {
                            if (address.AddressFamily == AddressFamily.Unknown)
                            {
                                return await this.ConnectAsync(this.EndPoint);
                            }
                            else
                            {
                                if ((address.AddressFamily == AddressFamily.InterNetwork && Socket.OSSupportsIPv4) || Socket.OSSupportsIPv6)
                                {
                                    if (address.IsIPv4MappedToIPv6)
                                    {
                                        this.Client.DualMode = true;
                                    }
                                    try
                                    {
                                        if (this.ConnectTimeout <= 0)
                                            this.ConnectTimeout = 10_000;

                                        var result = this.Client.BeginConnect(address, port, null, null);
                                        if (!result.AsyncWaitHandle.WaitOne(Math.Max(ConnectTimeout, 500), true))
                                        {
                                            this.OnClientError?.Invoke(this, this.EndPoint, new Exception());
                                            return await Task.FromResult(SocketError.TimedOut);
                                        }
                                        this.Client.EndConnect(result);
                                        this._Active = true;
                                        return await Task.FromResult(SocketError.Success);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Client = null;
                                        this.OnClientError?.Invoke(this, this.EndPoint, ex);
                                    }
                                }
                                break;
                            }
                        }
                        catch (Exception ex) when (!(ex is OutOfMemoryException))
                        {
                            lastex = ExceptionDispatchInfo.Capture(ex);
                        }
                    }
                    return await Task.FromResult(SocketError.SocketError);
                }
                finally
                {
                    if (!this.Active)
                    {
                        lastex?.Throw();
                    }
                }
            });
        }
        ///<inheritdoc/>
        public virtual async Task<SocketError> ConnectAsync(IPEndPoint remoteEP)
        {
#if NETSTANDARD2_0
            if (remoteEP == null) remoteEP =
#else
            remoteEP ??=
#endif
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1006);
            this.EndPoint = remoteEP;
            return await this.CompleteConnectAsync(async () =>
            {
                try
                {
                    if (this.ConnectTimeout <= 0)
                        this.ConnectTimeout = 10_000;

                    var result = this.Client.BeginConnect(remoteEP, null, null);
                    if (!result.AsyncWaitHandle.WaitOne(Math.Max(ConnectTimeout, 500), true))
                    {
                        this.OnClientError?.Invoke(this, this.EndPoint, new Exception());
                        return await Task.FromResult(SocketError.TimedOut).ConfigureAwait(false);
                    }
                    this.Client.EndConnect(result);
                    return await Task.FromResult(SocketError.Success).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Client = null;
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                    return await Task.FromResult(SocketError.SocketError).ConfigureAwait(false);
                }
            });
        }
        /// <summary>
        /// 连接完成
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        private async Task<SocketError> CompleteConnectAsync(Func<Task<SocketError>> task)
        {
            InitializeClientSocket();
            var result = await task.Invoke().ConfigureAwait(false);
            if (result == SocketError.Success)
            {
                this.EndPoint = this.Client.RemoteEndPoint.ToIPEndPoint();
                this._Active = true;
                this.SocketState = SocketState.Runing;
                this.ConnectedTime = DateTime.Now;
            }
            else
            {
                this.SocketState = SocketState.Idle;
            }
            return await Task.FromResult(result);
        }
        /// <summary>
        /// 连接完成
        /// </summary>
        /// <param name="func">回调</param>
        /// <returns></returns>
        private SocketError CompleteConnect(Func<SocketError> func)
        {
            InitializeClientSocket();
            var result = func.Invoke();
            if (result == SocketError.Success)
            {
                this.EndPoint = this.Client.RemoteEndPoint.ToIPEndPoint();
                this._Active = true;
                this.SocketState = SocketState.Runing;
                this.ConnectedTime = DateTime.Now;
            }
            else
            {
                this.SocketState = SocketState.Idle;
            }
            return result;
        }
        #endregion

        #region 启动
        ///<inheritdoc/>
        public override void Start()
        {
            if (this.IsServer)
                this.CheckClientConnectionTypeAsync().ConfigureAwait(false);
            else
            {
                this.Connect(this.EndPoint);
                this.OnStart?.Invoke(this, EventArgs.Empty);
                this.ReceviceDataAsync().ConfigureAwait(false);
                this.PingAsync().ConfigureAwait(false);
            }
        }
        ///<inheritdoc/>
        public virtual void Start(string host, int port)
        {
            this.Connect(host, port);
            this.AutoReceviceData();
        }
        ///<inheritdoc/>
        public virtual async Task StartAsync(string host, int port)
        {
            await this.ConnectAsync(host, port).ConfigureAwait(false);
            this.AutoReceviceData().ConfigureAwait(false).GetAwaiter();
        }
        ///<inheritdoc/>
        public virtual void Start(IPAddress address, int port)
        {
            this.Connect(address, port);
            this.AutoReceviceData();
        }
        ///<inheritdoc/>
        public virtual async Task StartAsync(IPAddress address, int port)
        {
            await this.ConnectAsync(address, port).ConfigureAwait(false);
            this.AutoReceviceData().ConfigureAwait(false).GetAwaiter();
        }
        ///<inheritdoc/>
        public virtual void Start(IPEndPoint endPoint)
        {
            this.Connect(endPoint);
            this.AutoReceviceData();
        }
        ///<inheritdoc/>
        public virtual async Task StartAsync(IPEndPoint endPoint)
        {
            await this.ConnectAsync(endPoint).ConfigureAwait(false);
            this.AutoReceviceData().ConfigureAwait(false).GetAwaiter();
        }
        ///<inheritdoc/>
        public virtual void Start(IPAddress[] addresses, int port)
        {
            this.Connect(addresses, port);
            this.AutoReceviceData();
        }
        ///<inheritdoc/>
        public virtual async Task StartAsync(IPAddress[] addresses, int port)
        {
            await this.ConnectAsync(addresses, port).ConfigureAwait(false);
            this.AutoReceviceData().ConfigureAwait(false).GetAwaiter();
        }
        /// <summary>
        /// 自动接收数据
        /// </summary>
        /// <returns></returns>
        Task AutoReceviceData() =>
            Task.Factory.StartNew(() =>
            {
                this.PingAsync().ConfigureAwait(false);
                this.OnStart?.Invoke(this, EventArgs.Empty);
                this.ReceviceDataAsync().ConfigureAwait(false);
            }, this.CancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        #endregion

        #region 停止
        ///<inheritdoc/>
        public override void Stop()
        {
            this.OnStop?.Invoke(this, EventArgs.Empty);
            if (this.Active)
            {
                this._Active = false;
            }
            this.Client?.Close();
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
#if NETSTANDARD2_0
            if (this.NetStream == null) this.NetStream =
#else
            this.NetStream ??=
#endif
            new NetworkStream(this.Client, true);
            return this.NetStream;
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitializeClientSocket()
        {
            if (this.EndPoint == null)
                this.Client = new Socket(this.SocketType, this.ProtocolType);
            else
            {
                if (this.EndPoint.AddressFamily == AddressFamily.Unknown)
                {
                    this.Client = new Socket(this.SocketType, this.ProtocolType);
                }
                else
                {
                    this.Client = new Socket(this.EndPoint.AddressFamily, this.SocketType, this.ProtocolType);
                }
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
            if (this.SslStream != null && this.IsAuthenticated) return this.SslStream;
            var stream = this.GetStream();
            if (stream == null) return null;
            if ((this.IsServer && (this.SslProtocols != SslProtocols.None || this.Certificate != null)) || (!this.IsServer && (this.HostName.IsNotNullOrEmpty() || this.SslProtocols != SslProtocols.None && this.Certificate != null)))
            {
                var sslStream = new SslStream(stream, false, (sender, cert, chain, error) =>
                {
                    return true;
                    //return error == SslPolicyErrors.None;
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
#if NETSTANDARD2_0
                            if (this.ClientCertificates == null) this.ClientCertificates =
#else
                            this.ClientCertificates ??=
#endif
                            new X509CertificateCollection();
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
                var b = stream.ReadByte();
                this.LastMessageTime = DateTime.Now;
                return await Task.FromResult(b).ConfigureAwait(false);
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
        public virtual async Task<int> ReceviceMessageAsync(byte[] bytes, int offset = -1, int count = -1)
        {
            if (offset < 0) offset = 0;
            if (count <= 0) count = bytes.Length - offset;
            if (bytes == null || bytes.Length == 0 || bytes.Length < offset || bytes.Length < count + offset) return -1;
            var stream = this.GetSslStream();
            if (stream == null) return -1;
            try
            {
                var dataBuffer = new byte[count];
                var readsize = await stream.ReadAsync(dataBuffer, 0, count, this.CancelToken.Token).ConfigureAwait(false);
                Array.Copy(dataBuffer, 0, bytes, offset, readsize);
                this.LastMessageTime = DateTime.Now;
                return readsize;
            }
            catch (SocketException ex)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return -1;
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
                return -1;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SocketException err)
                    this.OnClientError?.Invoke(this, this.EndPoint, err);
                else
                    this.OnClientError?.Invoke(this, this.EndPoint, ex);
                return -1;
            }
        }
        ///<inheritdoc/>
        public virtual async Task<byte[]> ReceviceMessageAsync(int count)
        {
            if (count == 0) return Array.Empty<byte>();
            var stream = this.GetSslStream();
            if (stream == null) return Array.Empty<byte>();
            try
            {
                var dataBuffer = new byte[count];
                var readsize = await stream.ReadAsync(dataBuffer, 0, count, this.CancelToken.Token).ConfigureAwait(false);
                this.LastMessageTime = DateTime.Now;
                return await Task.FromResult(dataBuffer.ReadBytes(0, readsize));
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
                    readsize = await stream.ReadAsync(dataBuffer, 0, this.ReceiveBufferSize, this.CancelToken.Token).ConfigureAwait(false);
                    await buffer.WriteAsync(dataBuffer, 0, readsize, this.CancelToken.Token).ConfigureAwait(false);
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
            this.LastMessageTime = DateTime.Now;
            if (buffer.Length == 0) return Array.Empty<byte>();
            var bytes = buffer.ToArray();
            buffer.Close();
            buffer.Dispose();
            return bytes;
        }
        #endregion

        #region 清空通道数据
        ///<inheritdoc/>
        public async Task<byte[]> ClearPipeDataAsync()
        {
            var stream = this.GetStream();
            if (stream != null && stream.DataAvailable)
            {
                return await this.ReceviceMessageAsync().ConfigureAwait(false);
            }
            return await Task.FromResult(Array.Empty<byte>());
        }
        #endregion

        #region 检测客户端连接类型
        /// <summary>
        /// 检测客户端连接类型
        /// </summary>
        /// <returns></returns>
        private async Task CheckClientConnectionTypeAsync()
        {
            var stream = this.GetStream();
            //第一次接收消息延时 10 毫秒,防止websocket网络抖动导致连接上,发送消息延时导致关闭客户端;
            await Task.Delay(this.NetworkDelay).ConfigureAwait(false);
            if (stream.DataAvailable)
            {
                var bytes = await ReceviceMessageAsync().ConfigureAwait(false);
                if (bytes.Length == 0)
                {
                    //断开了
                    this.Stop();
                    return;
                }
                var ReceiveMessage = this.DataType == SocketDataType.String ? bytes.GetString(this.Encoding) : bytes.ToHexString();

                var flags = await this.CheckWebSocketAsync(ReceiveMessage).ConfigureAwait(false);
                if (flags == -1) return;
                if (flags == 0)
                {
                    this._Active = true;
                    this.ConnectionType = ConnectionType.Socket;
                    this.OnStart?.Invoke(this, EventArgs.Empty);
                    await this.CheckClientAuthenticatedAsync().ConfigureAwait(false);
                    this.OnMessage?.Invoke(this, ReceiveMessage, EventArgs.Empty);
                    this.OnMessageByte?.Invoke(this, bytes, EventArgs.Empty);
                }
            }
            else
            {
                this.ConnectionType = ConnectionType.Socket;
                this._Active = true;
                this.OnStart?.Invoke(this, EventArgs.Empty);
                await this.CheckClientAuthenticatedAsync().ConfigureAwait(false);
            }
            await this.ReceviceDataAsync().ConfigureAwait(false);
        }
        #endregion

        #region 检测是否是websocket请求
        /// <summary>
        /// 检测是否是websocket请求
        /// </summary>
        /// <param name="receiveMessage">接收数据</param>
        /// <returns></returns>
        private async Task<int> CheckWebSocketAsync(string receiveMessage)
        {
            if (this.ConnectionType == ConnectionType.WebSocket) return await Task.FromResult(0);
            if (receiveMessage.IndexOf("Sec-WebSocket-Key", StringComparison.OrdinalIgnoreCase) > -1)
            {
                this.ConnectionType = ConnectionType.WebSocket;
                this._RequestHeader = receiveMessage;
                if (this is IWebSocketClient webSocket)
                {
                    webSocket.Request = new WebSocketRequest(webSocket.HostName.IsNullOrEmpty() ? "ws" : "wss", this.RequestHeader);
                }
                //开始握手
                var packet = new WebSocketPacket(this, receiveMessage)
                {
                    Encoding = this.Encoding,
                    DataType = this.DataType
                };
                var len = await packet.HandshakeAsync().ConfigureAwait(false);
                if (len == 0)
                {
                    this.OnClientError?.Invoke(this, this.EndPoint, new Exception("网络客户端已经关闭."));
                    this.Stop();
                    return await Task.FromResult(-1);
                }
                else if (len == -1)
                {
                    this.OnClientError?.Invoke(this, this.EndPoint, new Exception("Sec-WebSocket-Key接收到是空数据,握手失败."));
                    this.Stop();
                    return await Task.FromResult(-1);
                }
                this._Active = true;
                this.OnStart?.Invoke(this, EventArgs.Empty);
                await this.CheckClientAuthenticatedAsync().ConfigureAwait(false);
                return await Task.FromResult(1);
            }
            return await Task.FromResult(0);
        }
        #endregion

        #region 检测认证
        /// <summary>
        /// 检测认证
        /// </summary>
        /// <returns></returns>
        public async Task CheckClientAuthenticatedAsync()
        {
            //认证
            this._IsAuthenticated = this.Authentication(this);
            if (!this.IsAuthenticated)
            {
                var msg = "认证失败.";
                var msgBytes = msg.GetBytes(this.Encoding);
                await this.SendAsync(msgBytes);
                this.OnAuthentication?.Invoke(this, msg, EventArgs.Empty);
            }
        }
        #endregion

        #region 循环接收数据
        ///<inheritdoc/>
        public virtual async Task ReceviceDataAsync()
        {
            do
            {
                var bytes = await ReceviceMessageAsync().ConfigureAwait(false);
                if (bytes.Length == 0)
                {
                    //断开了
                    break;
                }
                var ReceiveMessage = this.DataType == SocketDataType.String ? bytes.GetString(this.Encoding) : bytes.ToHexString();
                if (this.IsServer)
                {
                    var flags = await this.CheckWebSocketAsync(ReceiveMessage).ConfigureAwait(false);
                    if (flags == -1) break;
                    if (flags == 1) continue;
                    if (!this._IsAuthenticated.HasValue || !this.IsAuthenticated)
                        this._IsAuthenticated = this.Authentication(this);
                    if (!this.IsAuthenticated)
                    {
                        var msg = "请先认证后再通讯.";
                        var msgBytes = msg.GetBytes(this.Encoding);
                        await this.SendAsync(msgBytes);
                        this.OnMessage?.Invoke(this, ReceiveMessage, EventArgs.Empty);
                        this.OnMessageByte?.Invoke(this, bytes, EventArgs.Empty);
                        continue;
                    }
                }
                if (this.ConnectionType == ConnectionType.WebSocket)
                {
                    var packet = new WebSocketPacket(this);
                    bytes = packet.UnPacket(bytes);
                    ReceiveMessage = bytes.GetString(this.Encoding);
                    if (packet.OpCode == OpCode.Close) break;
                    if (packet.OpCode == OpCode.Pong)
                    {
                        if (bytes.Length == 0) continue;
                    }
                    if (packet.OpCode == OpCode.Ping)
                    {
                        SendPongAsync().ConfigureAwait(false).GetAwaiter();
                        if (bytes.Length == 0) continue;
                    }
                    /*
                     * 2023-08-23 20:43 Jacky
                     * Postman 在ssl下 连续发消息接收不到,只能通过回应一个pong来解决
                     */
                    if (this.Certificate != null)
                        await this.SendPongAsync().ConfigureAwait(false);
                }

                if (ReceiveMessage.StartsWith("SUBSCRIBE#", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = ReceiveMessage.RemovePattern("^SUBSCRIBE#");
                    var index = msg.IndexOf(":");
                    if (index > -1)
                    {
#if NETSTANDARD2_0
                        var type = msg.Substring(0, index);
                        var data = msg.Substring(index + 1);
#else
                        var type = msg[..index];
                        var data = msg[(index + 1)..];
#endif
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
                this.OnMessage?.Invoke(this, ReceiveMessage, EventArgs.Empty);
                this.OnMessageByte?.Invoke(this, bytes, EventArgs.Empty);

            } while (!this.CancelToken.IsCancellationRequested && this.Connected);
            this.Stop();
        }
        #endregion

        #region 发送数据
        ///<inheritdoc/>
        public virtual void SendPing(byte[] buffers = null) => this.NetStreamSend(buffers, OpCode.Ping);
        ///<inheritdoc/>
        public virtual async Task SendPingAsync(byte[] buffers = null) => await this.NetStreamSendAsync(buffers, OpCode.Ping).ConfigureAwait(false);
        ///<inheritdoc/>
        public virtual void SendPong(byte[] buffers = null) => this.NetStreamSend(buffers, OpCode.Pong);
        ///<inheritdoc/>
        public virtual async Task SendPongAsync(byte[] buffers = null) => await this.NetStreamSendAsync(buffers, OpCode.Pong).ConfigureAwait(false);
        ///<inheritdoc/>
        public virtual int Send(byte[] buffers)
        {
            if (buffers.IsNullOrEmpty() || buffers.Length == 0) return 0;
            return this.NetStreamSend(buffers);
        }
        ///<inheritdoc/>
        public virtual int Send(byte[] buffers, MessageType messageType)
        {
            if (buffers.IsNullOrEmpty() || buffers.Length == 0) return 0;
            return this.NetStreamSend(buffers, ((int)messageType).ToEnum<OpCode>());
        }
        ///<inheritdoc/>
        public virtual async Task<int> SendAsync(byte[] buffers)
        {
            if (buffers.IsNullOrEmpty() || buffers.Length == 0) return 0;
            return await this.NetStreamSendAsync(buffers).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public virtual async Task<int> SendAsync(byte[] buffers, MessageType messageType)
        {
            if (buffers.IsNullOrEmpty() || buffers.Length == 0) return 0;
            return await this.NetStreamSendAsync(buffers, ((int)messageType).ToEnum<OpCode>()).ConfigureAwait(false);
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
            return await this.SendAsync(this.DataType == SocketDataType.String ? message.GetBytes(this.Encoding) : message.HexStringToBytes()).ConfigureAwait(false);
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
            await this.SendAsync(buffers).ConfigureAwait(false);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="opCode">操作码 <paramref name="opCode"/></param>
        /// <returns>发送数据包的字节长度
        /// <para><term>0</term> 无发送数据</para>
        /// <para><term>-1</term> 网络通道未连接</para>
        /// <para><term>-2</term> 网络通道还未准备好</para>
        /// </returns>
        private int NetStreamSend(byte[] buffers, OpCode opCode = OpCode.Text)
        {
            if (!this.Active) return -2;
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
#if NETSTANDARD2_0
                using (var packet = new WebSocketPacket(this))
#else
                using var packet = new WebSocketPacket(this);
#endif
                buffers = packet.Packet(buffers, opCode);
            }
            else
            {
                if (buffers == null || buffers.Length == 0)
                    return 0;
            }
            try
            {
                lock (StreamLock)
                {
                    stream.Write(buffers, 0, buffers.Length);
                    stream.Flush();
                }
                this.LastMessageTime = DateTime.Now;
            }
            catch (IOException)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, new SocketException((int)SocketError.SocketError));
                this.Stop();
                return -1;
            }
            return buffers.Length;
        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="opCode">操作码 <paramref name="opCode"/></param>
        /// <returns>发送数据包的字节长度
        /// <para><term>0</term> 无发送数据</para>
        /// <para><term>-1</term> 网络通道未连接</para>
        /// <para><term>-2</term> 网络通道还未准备好</para>
        /// </returns>
        private async Task<int> NetStreamSendAsync(byte[] buffers, OpCode opCode = OpCode.Text)
        {
            if (!this.Active) return await Task.FromResult(-2).ConfigureAwait(false);
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
#if NETSTANDARD2_0
                using (var packet = new WebSocketPacket(this))
#else
                using var packet = new WebSocketPacket(this);
#endif
                buffers = packet.Packet(buffers, opCode);
            }
            else
                if (buffers == null || buffers.Length == 0)
                return await Task.FromResult(0).ConfigureAwait(false);
            try
            {
                using (await StreamAsyncLock.EnterAsync().ConfigureAwait(false))
                {
                    await stream.WriteAsync(buffers, 0, buffers.Length, this.CancelToken.Token).ConfigureAwait(false);
                    await stream.FlushAsync(this.CancelToken.Token);
                }
                this.LastMessageTime = DateTime.Now;
            }
            catch (IOException)
            {
                this.OnClientError?.Invoke(this, this.EndPoint, new SocketException((int)SocketError.SocketError));
                this.Stop();
                return -1;
            }
            return await Task.FromResult(buffers.Length).ConfigureAwait(false);
        }
        #endregion

        #region 事件回调
        ///<inheritdoc/>
        public void ClientErrorEventHandler(IPEndPoint endPoint, Exception e) => OnClientError?.Invoke(this, endPoint, e);
        ///<inheritdoc/>
        public void MessageEventHandler(string message) => OnMessage?.Invoke(this, message, EventArgs.Empty);
        ///<inheritdoc/>
        public void MessageByteEventHandler(byte[] message) => OnMessageByte?.Invoke(this, message, EventArgs.Empty);
        ///<inheritdoc/>
        public void AuthenticationEventHandler(string message) => OnAuthentication?.Invoke(this, message, EventArgs.Empty);
        ///<inheritdoc/>
        public override void StartEventHandler() => OnStart?.Invoke(this, EventArgs.Empty);
        ///<inheritdoc/>
        public override void StopEventHandler() => OnStop?.Invoke(this, EventArgs.Empty);
        #endregion

        #region 设置Socket
        ///<inheritdoc/>
        public void SetSocket(Socket socket, int networkDelay, Func<ISocketClient, bool> authentication = null, X509Certificate certificate = null)
        {
            this.Client = socket;
            this.NetworkDelay = networkDelay;
            this.SocketState = SocketState.Runing;
            this.EndPoint = socket.RemoteEndPoint as IPEndPoint;
            this._IsServer = true;
#if NETSTANDARD2_0
            if (authentication == null) authentication =
#else
            authentication ??=
#endif
            c => true;
            this.Authentication = authentication;
            this.Certificate = certificate;
        }
        #endregion

        #region 处理定义数据
        ///<inheritdoc/>
        public virtual void AddChannel(params string[] channel)
        {
#if NETSTANDARD2_0
            if (this.TagsData == null) this.TagsData =
#else
            this.TagsData ??=
#endif
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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
            if (this.TryGetData(key, out var data))
                return data;
            return null;
        }
        ///<inheritdoc/>
        public virtual bool TryGetData(string key, out object data)
        {
            return this.TagsData.TryGetValue(key, out data);
        }
        ///<inheritdoc/>
        public virtual void AddData(string key, object value)
        {
            if (key.IsNullOrEmpty() || key.EqualsIgnoreCase(CHANNEL_KEY)) return;
#if NETSTANDARD2_0
            if (this.TagsData == null) this.TagsData =
#else
            this.TagsData ??=
#endif
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            lock (this.TagsData)
            {
                if (this.ContainsData(key))
                    this.TagsData[key] = value;
                else this.TagsData.Add(key, value);
            }
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
            this.Job = new Job().SetCancelToken(this.CancelToken.Token).SetName("SocketClient自动Ping作业").Interval(this.PingTime * 1000).SetCompleteCallBack(job =>
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
            base.Dispose(disposing, () =>
            {
                if (!this.IsServer)
                    this.CancelToken.Cancel(true);
                if (this.Client != null)
                {
                    if (this.Client.Connected)
                    {
                        try
                        {
                            this.Client.Shutdown(SocketShutdown.Both);
                            this.Client.Close(3);
                        }
                        catch { }
                    }
                    this.Client.Dispose();
                    this.Client = null;
                }
                this.StreamAsyncLock?.Dispose();
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
            });
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