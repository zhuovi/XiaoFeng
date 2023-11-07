using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Threading;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-27 16:08:29                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// Socket服务端
    /// </summary>
    public class SocketServer : SocketServer<SocketClient>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SocketServer() : base(1006) { }
        /// <summary>
        /// 设置监听端口
        /// </summary>
        /// <param name="port">监听端口</param>
        public SocketServer(int port) : base(port) { }
        /// <summary>
        /// 设置监听终点
        /// </summary>
        /// <param name="localEP">监听终点</param>
        public SocketServer(IPEndPoint localEP) : base(localEP) { }
        /// <summary>
        /// 设置监听地址和端口
        /// </summary>
        /// <param name="localaddr">监听地址</param>
        /// <param name="port">监听端口</param>
        public SocketServer(IPAddress localaddr, int port) : base(localaddr, port) { }
        #endregion
    }
    /// <summary>
    /// Socket服务端
    /// </summary>
    public class SocketServer<T> : BaseSocket, ISocketServer where T : ISocketClient, new()
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SocketServer() : this(1006) { }
        /// <summary>
        /// 设置监听端口
        /// </summary>
        /// <param name="port">监听端口</param>
        public SocketServer(int port) : this(IPAddress.Any, port) { }
        /// <summary>
        /// 设置监听终点
        /// </summary>
        /// <param name="localEP">监听终点</param>
        public SocketServer(IPEndPoint localEP)
        {
            this.EndPoint = localEP;
        }
        /// <summary>
        /// 设置监听地址和端口
        /// </summary>
        /// <param name="localaddr">监听地址</param>
        /// <param name="port">监听端口</param>
        public SocketServer(IPAddress localaddr, int port)
        {
            this.EndPoint = new IPEndPoint(localaddr, port);
        }
        #endregion

        #region 属性
        ///<inheritdoc/>
        public int ListenCount { get; set; } = int.MaxValue;
        ///<inheritdoc/>
        public Boolean ReuseAddress { get; set; } = true;
        ///<inheritdoc/>
        public Func<ISocketClient, Boolean> Authentication { get; set; }
        /// <summary>
        /// 黑名单列表
        /// </summary>
        private ConcurrentDictionary<long, string> BlackList { get; set; } = new ConcurrentDictionary<long, string>();
        ///<inheritdoc/>
        public X509Certificate Certificate { get; set; }
        /// <summary>
        /// 服务端SOCKET
        /// </summary>
        private Socket Server { get; set; }
        /// <summary>
        /// 是否独占地址使用
        /// </summary>
        private Boolean _ExclusiveAddressUse;
        ///<inheritdoc/>
        public override Boolean ExclusiveAddressUse
        {
            get
            {
                return this.Server != null ? this.Server.ExclusiveAddressUse : this._ExclusiveAddressUse;
            }
            set
            {
                if (this.Active)
                {
                    throw new Exception("当前SOCKET已经在运行,无法设置.");
                }

                if (this.Server != null)
                {
                    this.Server.ExclusiveAddressUse = value;
                }
                this._ExclusiveAddressUse = value;
            }
        }
        /// <summary>
        /// 允许网络地址转换
        /// </summary>
        private Boolean? _AllowNatTraversal;
        /// <summary>
        /// 客户端列表
        /// </summary>
        public ICollection<ISocketClient> Clients => this._Clients?.Values ?? null;
        /// <summary>
        /// 连接列表
        /// </summary>
        private ConcurrentDictionary<IPEndPoint, ISocketClient> _Clients { get; set; } = new ConcurrentDictionary<IPEndPoint, ISocketClient>();
        /// <summary>
        /// 是否自动Pong
        /// </summary>
        public Boolean IsPong { get; set; }
        /// <summary>
        /// pong间隔 单位为秒
        /// </summary>
        public int PongTime { get; set; } = 120;
        /// <summary>
        /// 定时作业
        /// </summary>
        private IJob Job { get; set; }
        /// <summary>
        /// 主任务
        /// </summary>
        private Task MainTask;
        #endregion

        #region 事件
        ///<inheritdoc/>
        public event OnNewConnectionEventHandler OnNewConnection;
        ///<inheritdoc/>
        public override event OnMessageEventHandler OnMessage;
        ///<inheritdoc/>
        public override event OnMessageByteEventHandler OnMessageByte;
        ///<inheritdoc/>
        public event OnDisconnectedEventHandler OnDisconnected;
        ///<inheritdoc/>
        public override event OnStopEventHandler OnStop;
        ///<inheritdoc/>
        public event OnErrorEventHandler OnError;
        ///<inheritdoc/>
        public override event OnClientErrorEventHandler OnClientError;
        ///<inheritdoc/>
        public override event OnStartEventHandler OnStart;
        ///<inheritdoc/>
        public override event OnAuthenticationEventHandler OnAuthentication;
        #endregion

        #region 方法

        #region 获取Socket
        /// <inheritdoc/>
        public override Socket GetSocket() => this.Server;
        #endregion

        #region 启动
        /// <inheritdoc/>
        public virtual void Start(int backlog)
        {
            CreateNewSocketIfNeeded();
            try
            {
                if (this.EndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    this.Server?.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                    this.Server?.Bind(new IPEndPoint(IPAddress.IPv6Any, this.EndPoint.Port));
                }
                else
                {
                    this.Server?.Bind(this.EndPoint);
                }
                this.Server?.Listen(backlog);
                this.ListenCount = backlog;
                /*设定服务器运行状态*/
                this.SocketState = SocketState.Runing;
                /*启动事件*/
                OnStart?.Invoke(this, EventArgs.Empty);
                this.AcceptSocketClient();
                this.RunPongAsync().ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                Stop();
                this.OnError?.Invoke(this, ex);
            }
            this._Active = true;
        }
        /// <inheritdoc/>
        public override void Start()
        {
            this.Start(this.ListenCount);
        }
        #endregion

        #region 停止
        /// <inheritdoc/>
        public override void Stop()
        {
            if (this.Active)
            {
                this._Active = false;
                this.Dispose(true);
            }
            this.OnStop?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 发送消息
        ///<inheritdoc/>
        public virtual void Send(string message) => this.Send(message.GetBytes(this.Encoding));
        ///<inheritdoc/>
        public virtual void Send(byte[] buffers)
        {
            if (buffers == null || buffers.Length == 0) return;
            if (this.Clients == null) { this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>(); return; }
            if (this.Clients.Count == 0) return;
            this.Clients.Each(a => a.Send(buffers));
        }
        ///<inheritdoc/>
        public virtual void Send(byte[] buffers, ISocketClient client)
        {
            if (buffers == null || buffers.Length == 0) return;
            if (client.Connected)
                client.Send(buffers);
        }
        ///<inheritdoc/>
        public virtual void Send(byte[] buffers, int offset, int count, ISocketClient client)
        {
            if (buffers == null || buffers.Length == 0 || buffers.Length - count <= 0) return;
            var buffer = new byte[count];
            Array.Copy(buffers, offset, buffer, 0, count);
            this.Send(buffer, client);
        }
        ///<inheritdoc/>
        public virtual void Send(byte[] buffers, params string[] channel)
        {
            if (buffers == null || buffers.Length == 0) return;
            if (this.Clients == null) { this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>(); return; }
            if (this.Clients.Count == 0) return;
            if (channel.IsNullOrEmpty())
            {
                this.Send(buffers);
                return;
            }
            this.Clients.Where(a => a.ContainsChannel(channel)).Each(c => c.Send(buffers));
        }
        ///<inheritdoc/>
        public virtual void Send(byte[] buffers, string key, object value)
        {
            if (buffers == null || buffers.Length == 0) return;
            if (this.Clients == null) { this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>(); return; }
            if (this.Clients.Count == 0) return;
            this.Clients.Where(a => a.GetData(key) == value).Each(c => c.Send(buffers));
        }
        ///<inheritdoc/>
        public virtual async Task SendAsync(string message) => await this.SendAsync(message.GetBytes(this.Encoding)).ConfigureAwait(false);
        ///<inheritdoc/>
        public virtual Task SendAsync(byte[] buffers)
        {
            if (buffers == null || buffers.Length == 0) return Task.CompletedTask;
            if (this.Clients == null) { this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>(); return Task.CompletedTask; }
            if (this.Clients.Count == 0) return Task.CompletedTask;
            this.Clients.Each(async a => await a.SendAsync(buffers).ConfigureAwait(false));
            return Task.CompletedTask;
        }
        ///<inheritdoc/>
        public virtual async Task SendAsync(byte[] buffers, ISocketClient client)
        {
            if (buffers == null || buffers.Length == 0) return;
            if (client.Connected)
                await client.SendAsync(buffers).ConfigureAwait(false);
            await Task.CompletedTask;
        }
        ///<inheritdoc/>
        public virtual async Task SendAsync(byte[] buffers, int offset, int count, ISocketClient client)
        {
            if (buffers == null || buffers.Length == 0 || buffers.Length - count <= 0) return;
            var buffer = new byte[count];
            Array.Copy(buffers, offset, buffer, 0, count);
            await this.SendAsync(buffer, client).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public virtual Task SendAsync(byte[] buffers, params string[] channel)
        {
            if (buffers == null || buffers.Length == 0) return Task.CompletedTask;
            if (this.Clients == null) { this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>(); return Task.CompletedTask; }
            if (this.Clients.Count == 0) return Task.CompletedTask;
            this.Clients.Where(a => a.ContainsChannel(channel)).Each(async c => await c.SendAsync(buffers).ConfigureAwait(false));
            return Task.CompletedTask;
        }
        ///<inheritdoc/>
        public virtual Task SendAsync(byte[] buffers, string key, object value)
        {
            if (buffers == null || buffers.Length == 0) return Task.CompletedTask;
            if (this.Clients == null) { this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>(); return Task.CompletedTask; }
            if (this.Clients.Count == 0) return Task.CompletedTask;
            this.Clients.Where(a => a.GetData(key) == value).Each(async c => await c.SendAsync(buffers).ConfigureAwait(false));
            return Task.CompletedTask;
        }
        #endregion

        #region 接受客户端连接请求
        /// <summary>
        /// 接受客户端连接请求
        /// </summary>
        public virtual void AcceptSocketClient()
        {
            this.CancelToken.Token.Register(() =>
            {
                this.Stop();
            });
            /*创建线程*/
            this.MainTask = Task.Factory.StartNew(() =>
            {
                /*设置当前线程为后台线程*/
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    if (this.CancelToken.IsCancellationRequested)
                    {
                        /*取消事件*/
                        OnStop?.Invoke(this, EventArgs.Empty);
                        /*清空队列*/
                        this.ClearQueue();
                        if (this.Server != null && this.Server.Connected)
                        {
                            this.Stop();
                        }
                        break;
                    }
                    var client = this.AcceptTcpClientAsync(this.CancelToken.Token).ConfigureAwait(false).GetAwaiter().GetResult();
                    this.CreateClientAsync(client).ConfigureAwait(false);
                }
            }, this.CancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }
        #endregion

        #region 创建网络客户端
        /// <summary>
        /// 创建网络客户端
        /// </summary>
        /// <param name="client">网络客户端 <see cref="ISocketClient"/></param>
        /// <returns>任务</returns>
        private async Task CreateClientAsync(T client)
        {
            if (client == null)
            {
                this.OnError?.Invoke(this, new Exception("网络客户端转换实体出错."));
                await Task.CompletedTask;
                return;
            }
            //判断黑名单
            if (this.ContainsBlack(client.EndPoint.Address.ToString()))
            {
                var msg = "当前客户端IP在黑名单中,禁止连接服务端.";
                client.Send(msg);
                client.Stop();
                this.OnAuthentication?.Invoke(client, msg, EventArgs.Empty);
                await Task.CompletedTask;
                return;
            }
            if (this.ReceiveBufferSize >= 0)
                client.ReceiveBufferSize = this.ReceiveBufferSize;
            if (this.ReceiveTimeout >= 0) client.ReceiveTimeout = this.ReceiveTimeout;
            if (this.SendTimeout >= 0) client.SendTimeout = this.SendTimeout;
            if (this.SslProtocols >= 0)
                client.SslProtocols = this.SslProtocols;
            client.Encoding = this.Encoding;
            client.DataType = this.DataType;
            client.CancelToken = this.CancelToken;
            client.OnClientError += this.OnClientError;
            client.OnMessage += this.OnMessage;
            client.OnMessageByte += this.OnMessageByte;
            client.OnAuthentication += this.OnAuthentication;
            client.OnStart += (c, e) =>
            {
                //加入队列
                this.AddQueue(client);
                this.OnNewConnection?.Invoke(client, e);
            };
            client.OnStop += (c, e) =>
            {
                //移除队列
                this.RemoveQueue(client);
            };
            client.ConnectedTime = DateTime.Now;
            client.Start();
            await Task.CompletedTask;
        }
        #endregion

        #region 确定是否存在挂起的连接请求
        /// <summary>
        /// 确定是否存在挂起的连接请求。
        /// </summary>
        /// <returns>是否存在
        /// <para><term>true</term> 存在</para>
        /// <para><term>false</term> 不存在</para>
        /// </returns>
        public virtual Boolean Pending()
        {
            return this.Active ? this.Server.Poll(0, SelectMode.SelectRead) : false;
        }
        #endregion

        #region 接受第一个挂起的连接
        /// <summary>
        /// 接受第一个挂起的连接
        /// </summary>
        /// <returns>一个 <see cref="Socket"/> 对象</returns>
        public virtual Socket AcceptSocket()
        {
            return this.Server?.Accept();
        }
        /// <summary>
        /// 接受第一个挂起的连接
        /// </summary>
        /// <param name="cancellationToken">取消指令</param>
        /// <returns>一个 <see cref="Socket"/> <see cref="Task"/></returns>
        public virtual async Task<Socket> AcceptSocketAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<Socket>(cancellationToken).ConfigureAwait(false);
            }
            return await this.Server.AcceptAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// 接受第一个挂起的连接
        /// </summary>
        /// <returns></returns>
        public virtual T AcceptTcpClient()
        {
            var AcceptSocket = this.AcceptSocket();
            try
            {
                var client = Activator.CreateInstance<T>();
                client.SetSocket(AcceptSocket, this.NetworkDelay, this.Authentication, this.Certificate);
                return client;
            }
            catch (Exception ex)
            {
                AcceptSocket.Shutdown(SocketShutdown.Both);
                AcceptSocket.Close();
                AcceptSocket.Dispose();
                AcceptSocket = null;
                LogHelper.Error(ex);
                return default(T);
            }
        }
        /// <summary>
        /// 接受第一个挂起的连接
        /// </summary>
        /// <param name="cancellationToken">取消指令</param>
        /// <returns></returns>
        public virtual async Task<T> AcceptTcpClientAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await Task.FromCanceled<T>(cancellationToken).ConfigureAwait(false);
            }
            var AcceptSocket = await this.AcceptSocketAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var client = Activator.CreateInstance<T>();
                client.SetSocket(AcceptSocket, this.NetworkDelay, this.Authentication, this.Certificate);
                return await Task.FromResult(client).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                AcceptSocket.Shutdown(SocketShutdown.Both);
                AcceptSocket.Close();
                AcceptSocket.Dispose();
                AcceptSocket = null;
                LogHelper.Error(ex);
                return await Task.FromResult(default(T)).ConfigureAwait(false);
            }
        }
        #endregion

        #region 允许网络地址转换
        /// <summary>
        /// 允许网络地址转换 仅支持 <see langword="windows"/>
        /// </summary>
        /// <param name="allowed"><para><term>true</term> 允许 </para><para><term>false</term> 不允许</para></param>
        public void AllowNatTraversal(bool allowed)
        {
            if (this.Active)
            {
                throw new Exception("当前SOCKET已经在运行,无法设置.");
            }
            if (this.Server != null)
            {
                this.Server?.SetIPProtectionLevel(allowed ? IPProtectionLevel.Unrestricted : IPProtectionLevel.EdgeRestricted);
            }
            else
            {
                _AllowNatTraversal = allowed;
            }
        }
        #endregion

        #region 创建新的SOCKET
        /// <summary>
        /// 创建新的SOCKET
        /// </summary>
        private void CreateNewSocketIfNeeded()
        {
            if (this.Server == null)
            {
                /*定义一个套接字用于监听客户端发来的消息,包含三个参数（IP4寻址协议，流式连接，Tcp协议）*/
                this.Server = new Socket(this.EndPoint.AddressFamily, this.SocketType, this.ProtocolType)
                {
                    NoDelay = this.NoDelay,
                    ReceiveTimeout = this.ReceiveTimeout,
                    SendTimeout = this.SendTimeout,
                    ReceiveBufferSize = this.ReceiveBufferSize,
                    SendBufferSize = this.SendBufferSize
                };
                if (this.ReuseAddress)
                    this.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }

            if (this.ExclusiveAddressUse)
            {
                this.Server.ExclusiveAddressUse = true;
            }

            if (OS.Platform.GetOSPlatform() == PlatformOS.Windows && this._AllowNatTraversal != null)
            {
                this.AllowNatTraversal(this._AllowNatTraversal.GetValueOrDefault());
                this._AllowNatTraversal = null;
            }
        }
        #endregion

        #region 调用当前事件
        ///<inheritdoc/>
        public override void StartEventHandler() => OnStart?.Invoke(this, EventArgs.Empty);
        ///<inheritdoc/>
        public override void StopEventHandler() => OnStop?.Invoke(this, EventArgs.Empty);
        ///<inheritdoc/>
        public void NewConnectionEventHandler(ISocketClient client) => OnNewConnection?.Invoke(client, EventArgs.Empty);
        ///<inheritdoc/>
        public void DisconnectedEventHandler(ISocketClient client) => OnDisconnected?.Invoke(client, EventArgs.Empty);
        ///<inheritdoc/>
        public void ClientErrorEventHandler(ISocketClient client, IPEndPoint endPoint, Exception e) => OnClientError?.Invoke(client, endPoint, e);
        ///<inheritdoc/>
        public void MessageEventHandler(ISocketClient client, string message) => OnMessage?.Invoke(client, message, EventArgs.Empty);
        ///<inheritdoc/>
        public void MessageByteEventHandler(ISocketClient client, byte[] message) => OnMessageByte?.Invoke(client, message, EventArgs.Empty);
        ///<inheritdoc/>
        public void AuthenticationEventHandler(ISocketClient client, string message) => OnAuthentication?.Invoke(client, message, EventArgs.Empty);
        #endregion

        #region 队列
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="client">socket客户端</param>
        private void AddQueue(ISocketClient client)
        {
            if (client == null || client.SocketState != SocketState.Runing || !client.Connected) return;
            if (this.Clients == null) this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>();
            this._Clients.TryAdd(client.EndPoint, client);
        }
        /// <summary>
        /// 移除队列
        /// </summary>
        /// <param name="client">Socket客户端</param>
        private void RemoveQueue(ISocketClient client)
        {
            if (this.Clients == null || this.Clients.Count == 0 || client == null) return;
            OnDisconnected?.Invoke(client, EventArgs.Empty);
            try
            {
                lock (this._Clients)
                {
                    this._Clients.TryRemove(client.EndPoint, out client);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 移除队列
        /// </summary>
        /// <param name="endPoint">网络地址</param>
        private void RemoveQueue(IPEndPoint endPoint)
        {
            if (this.Clients == null || this.Clients.Count == 0 || endPoint == null) return;
            try
            {
                ISocketClient client;
                lock (this._Clients)
                {
                    this._Clients.TryRemove(endPoint, out client);
                }
                if (client.Connected)
                {
                    client.Stop();
                }
                OnDisconnected?.Invoke(client, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 清空队列
        /// </summary>
        private void ClearQueue()
        {
            if (this.Clients != null && this.Clients.Count > 0)
            {
                //先断开所有连接
                this.Clients.Each(a =>
                {
                    a.Stop();
                    OnDisconnected?.Invoke(a, EventArgs.Empty);
                });
                this._Clients.Clear();
                this._Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>();
            }
        }
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <param name="socket">socket</param>
        /// <returns></returns>
        private ISocketClient GetQueue(Socket socket)
        {
            if (socket == null) return null;
            ISocketClient _val = null;
            var point = socket.RemoteEndPoint.ToIPEndPoint();
            if (this.Clients != null && this.Clients.Count > 0 && this._Clients.ContainsKey(point))
                this._Clients.TryGetValue(point, out _val);
            return _val;
        }
        /// <summary>
        /// 获取在线列表中的客户端
        /// </summary>
        /// <param name="func">满足条件的函数</param>
        /// <returns></returns>
        private ISocketClient GetQueue(Func<ISocketClient, bool> func)
        {
            if (this.Clients == null || this.Clients.Count == 0 || func == null) return null;
            return this.Clients.Where(func).FirstOrDefault();
        }
        /// <summary>
        /// 获取队列数
        /// </summary>
        /// <returns></returns>
        private int CountQueue()
        {
            int Count = 0;
            try
            {
                if (this.Clients != null) Count = this.Clients.Count;
            }
            finally
            {
            }
            return Count;
        }
        #endregion

        #region 黑名单
        ///<inheritdoc/>
        public void AddBlack(params string[] ips)
        {
            if (this.BlackList == null) this.BlackList = new ConcurrentDictionary<long, string>();
            ips.Each(ip =>
            {
                var val = ParseIp(ip);
                if (val == 0) return;
                if (!this.ContainsBlack(val)) this.BlackList.TryAdd(val, ip);
            });
        }
        ///<inheritdoc/>
        public Boolean ContainsBlack(string ip)
        {
            var val = ParseIp(ip);
            if (val == 0) return true;
            return this.ContainsBlack(val);
        }
        ///<inheritdoc/>
        public Boolean ContainsBlack(long ip)
        {
            return this.BlackList.ContainsKey(ip);
        }
        ///<inheritdoc/>
        public void AddBlack(IEnumerable<string> ips) => this.AddBlack(ips.ToArray());
        ///<inheritdoc/>
        public void RemoveBlack(string ip)
        {
            if (this.BlackList == null || this.BlackList.Count == 0) return;
            var val = ParseIp(ip);
            if (val == 0) return;
            if (!this.ContainsBlack(val)) this.BlackList.TryAdd(val, ip);
        }
        ///<inheritdoc/>
        public void ClearBlack()
        {
            this.BlackList = new ConcurrentDictionary<long, string>();
        }
        /// <inheritdoc/>
        public ICollection<string> GetBlackList() => this.BlackList?.Values;
        /// <summary>
        /// 转换IP直long
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>ip值</returns>
        private long ParseIp(string ip)
        {
            if (!ip.IsIP()) return 0;
            string[] ipArr = ip.Split(new char[] { '.' });
            return long.Parse(ipArr[0]) * 256 * 256 * 256 + long.Parse(ipArr[1]) * 256 * 256 + long.Parse(ipArr[2]) * 256 + long.Parse(ipArr[3]);
        }
        #endregion

        #region 启动pong功能
        /// <summary>
        /// 启动pong功能
        /// </summary>
        /// <returns></returns>
        private Task RunPongAsync()
        {
            if (!this.IsPong) return Task.CompletedTask;
            if (this.PongTime <= 3) this.PongTime = 10;
            //if (this.Clients == null || !this.Clients.Any()) return Task.CompletedTask;
            this.Job = new Job().SetName("SocketServer自动Pong作业").Interval(this.PongTime * 1000).SetCompleteCallBack(job =>
            {
                this.Clients.Each(a =>
                {
                    a.SendPongAsync(new
                    {
                        type = "pong",
                        time = DateTime.Now.ToTimeStamp()
                    }.ToJson().GetBytes(a.Encoding)).ConfigureAwait(false);
                });
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
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.CancelToken.Cancel();
            this.CancelToken = new CancellationTokenSource();
            this.ClearQueue();
            if (this.Server.Connected)
            {
                this.Server.Shutdown(SocketShutdown.Both);
                this.Server.Disconnect(false);
            }
            this.Server.Close(3);
            this.Server.Dispose();
            this.Server = null;
            this.SocketState = SocketState.Stop;
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
        ~SocketServer()
        {
            this.Dispose(false);
        }
        #endregion

        #endregion
    }
}