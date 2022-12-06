using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using XiaoFeng.Config;
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
    /// 网络服务端
    /// </summary>
    public class NetServer<TSession> : INetServer where TSession : IServerSession, new()
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public NetServer()
        {
            Init();
        }
        /// <summary>
        /// 设置服务器IP和端口
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">端口</param>
        public NetServer(IPAddress ip, int port)
        {
            this.IP = ip;
            this.Port = port;
            this.Init();
        }
        /// <summary>
        /// 设置服务器IP和端口
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">端口</param>
        public NetServer(string ip, int port) : this(IPAddress.Parse(ip), port) { }
        /// <summary>
        /// 设置端口
        /// </summary>
        /// <param name="port">端口</param>
        public NetServer(int port) : this(IPAddress.Any, port) { }
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
        /// 消息后是否加回车
        /// </summary>
        public Boolean IsNewLine { get; set; } = true;
        /// <summary>
        /// Socket状态
        /// </summary>
        public SocketState SocketState { get; set; } = SocketState.Idle;
        /// <summary>
        /// 是否启动心跳
        /// </summary>
        public Boolean IsPing { get; set; } = false;
        /// <summary>
        /// ping 间隔时长 单位 毫秒
        /// </summary>
        public int PingTimer { get; set; } = 10 * 1000;
        /// <summary>
        /// 几次不回应断开
        /// </summary>
        public int PingCount { get; set; } = 3;
        /// <summary>
        /// 侦听网络IP
        /// </summary>
        public IPAddress IP { get; set; } = IPAddress.Any;
        /// <summary>
        /// 侦听网络端口
        /// </summary>
        public int Port { get; set; } = 1006;
        /// <summary>
        /// 使用的寻址方案
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;
        /// <summary>
        /// 连接类型
        /// </summary>
        public SocketType SocketType { get; set; } = SocketType.Stream;
        /// <summary>
        /// 协议类型
        /// </summary>
        public ProtocolType ProtocolType { get; set; } = ProtocolType.IP;
        /// <summary>
        /// 连接请求数
        /// </summary>
        public int ListenCount { get; set; } = int.MaxValue;
        /// <summary>
        /// 取消通知
        /// </summary>
        private CancellationTokenSource CancelToken = new CancellationTokenSource();
        /// <summary>
        /// 验证Socket请求的合法性
        /// </summary>
        public Func<IServerSession, Boolean> SocketAuth { get; set; }
        /// <summary>
        /// 黑名单列表
        /// </summary>
        public ArrayList IpBlackList { get; set; } = ArrayList.Synchronized(new ArrayList());
        /// <summary>
        /// 读写锁
        /// </summary>
        private readonly ReaderWriterLockSlim RWLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 准备释放
        /// </summary>
        private bool AlreadyDisposed = false;
        /// <summary>
        /// 服务监听
        /// </summary>
        public Socket ServerSocket { get; set; }
        /// <summary>
        /// 最大接收数
        /// </summary>
        private readonly int MaxBufferSize = 1024 * 100;
        /// <summary>
        /// 开始字节
        /// </summary>
        private byte[] FirstByte;
        /// <summary>
        /// 结尾字节
        /// </summary>
        private byte[] LastByte;
        /// <summary>
        /// 连接列表
        /// </summary>
        public ConcurrentDictionary<IPEndPoint, IServerSession> ConnectionSocketList { get; private set; } = new ConcurrentDictionary<IPEndPoint, IServerSession>();
        /// <summary>
        /// WS地址
        /// </summary>
        private string _ServerLocation = "";
        /// <summary>
        /// WS地址
        /// </summary>
        public string ServerLocation
        {
            get { return this._ServerLocation.IsNullOrEmpty() ? string.Format("WebSocket地址: ws://{0}:{1}\nSocket地址:{0}:{1}", this.IP.ToString() == "0.0.0.0" ? GetLocalmachineIPAddress() : this.IP, this.Port) : this._ServerLocation; }
            set { this._ServerLocation = value; }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 新的连接事件
        /// </summary>
        public event NewConnectionEventHandler OnNewConnection;
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
        /// 停止服务事件
        /// </summary>
        public event StopEventHandler OnStop;
        /// <summary>
        /// 出错事件
        /// </summary>
        public event ErrorEventHandler OnError;
        /// <summary>
        /// 客户端错误事件
        /// </summary>
        public event SessionErrorEventHandler OnClientError;
        /// <summary>
        /// 服务器启动事件
        /// </summary>
        public event StartEventHandler OnStart;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.FirstByte = new byte[MaxBufferSize];
            this.LastByte = new byte[MaxBufferSize];
            this.FirstByte[0] = 0x00;
            this.LastByte[0] = 0xFF;
        }
        #endregion

        #region 获取地址
        /// <summary>
        /// 获取地址
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalmachineIPAddress()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

            foreach (IPAddress ip in ipEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }
            return ipEntry.AddressList[0];
        }
        #endregion

        #region 启动
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            try
            {
                /*定义一个套接字用于监听客户端发来的消息,包含三个参数（IP4寻址协议，流式连接，Tcp协议）*/
                ServerSocket = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType);
                ServerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                /*绑定IP和端口*/
                ServerSocket.Bind(new IPEndPoint(this.IP, this.Port));
                /*设定排队连接请求数*/
                ServerSocket.Listen(this.ListenCount);
                /*设定服务器运行状态*/
                this.SocketState = SocketState.Runing;
                /*启动事件*/
                OnStart?.Invoke(this, EventArgs.Empty);
            }
            catch (SocketException ex)
            {
                this.SocketState = SocketState.Stop;
                this.OnError?.Invoke(this, ex);
                return;
            }
            /*创建线程*/
            Task.Factory.StartNew(() =>
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
                        ServerSocket.Close(3);
                        break;
                    }
                    try
                    {
                        /*建立异步监听*/
                        Socket socket = ServerSocket.Accept();
                        if (socket != null)
                        {
                            /*判断是否在黑名单*/
                            if (this.IsBlackIP(socket.RemoteEndPoint.ToString()))
                            {
                                this.Close(new TSession
                                {
                                    ConnectionSocket = socket,
                                    Encoding = this.Encoding,
                                    DataType = this.DataType
                                });
                                continue;
                            }
                            /*休息100毫秒*/
                            Task.Delay(100).Wait();
                            /*创建连接对象*/
                            TSession session = new TSession
                            {
                                ConnectionSocket = socket,
                                EndPoint = socket.RemoteEndPoint as IPEndPoint,
                                Port = this.Port,
                                Encoding = this.Encoding,
                                DataType = this.DataType,
                                IsNewLine = this.IsNewLine,
                                ConnectionTime = DateTime.Now,
                                SocketAuth = this.SocketAuth,
                                //SetSocketType = this.UpdateQueue
                            };
                            session.OnNewConnection += (o,e)=>
                            {
                                this.AddQueue((TSession)o);
                                //OnNewConnection?.Invoke(o, EventArgs.Empty);
                            };
                            session.OnMessage += new MessageEventHandler((sender, message, e) =>
                            {
                                if (sender == null || sender.ConnectionSocket == null || !sender.ConnectionSocket.Connected) return;
                                /*检查是否启动心跳*/
                                if (this.IsPing)
                                {
                                    if (message.IsMatch(@"{\s*""pong""\s*:\s*""\d+""\s*}"))
                                    {
                                        Int64 TimeStamp = message.GetMatch(@"{\s*""pong""\s*:\s*""(?<a>\d+)""\s*}").ToInt64();
                                        if (TimeStamp == sender.PingTimeStamp) sender.PingTimeStamp = 0;
                                    }
                                }
                                OnMessage?.Invoke(sender, message, e);
                            });
                            session.OnMessageByte += new MessageByteEventHandler((sender, message, e) =>
                            {
                                if (sender == null || sender.ConnectionSocket == null || !sender.ConnectionSocket.Connected) return;
                                OnMessageByte?.Invoke(sender, message, e);
                            });
                            session.OnDisconnected += new DisconnectedEventHandler((sender, e) =>
                            {
                                /*移除列表*/
                                this.RemoveQueue(sender);
                                /*断开消息*/
                                //this.OnDisconnected?.Invoke(sender, EventArgs.Empty);
                            });
                            session.OnSessionError += this.OnClientError;
                            //session.SocketAuth = this.SocketAuth;
                            //session.SetSocketType = this.UpdateQueue;
                            session.ConnectionSocket.BeginReceive(
                                session.ReceivedDataBuffer,
                                0,
                                session.ReceivedDataBuffer.Length,
                                SocketFlags.None, 
                                new AsyncCallback(session.ManageHandshake),
                                session.ConnectionSocket.Available
                            );
                            this.AddQueue(session);
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (!this.CancelToken.IsCancellationRequested)
                            OnError?.Invoke(this, ex);
                    }
                }
            }, this.CancelToken.Token);
            /*ping线程*/
            if (this.IsPing)
            {
                Task.Factory.StartNew(() =>
                {
                    /*设置当前线程为后台线程*/
                    var setting = Setting.Current;
                    while (true)
                    {
                        if (this.CancelToken.IsCancellationRequested) break;
                        ManualResetEvent mre = new ManualResetEvent(false);
                        IServerSession[] values = this.GetData();
                        int _Count = values.Length, Count = _Count;
                        if (_Count > 0)
                        {
                            for (int i = 0; i < Count; i++)
                            {
                                try
                                {
                                    ThreadPool.QueueUserWorkItem(s =>
                                    {
                                        var session = s as IServerSession;
                                        if (session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected)
                                        {
                                            this.RemoveQueue(session);
                                            session.Close();
                                        }
                                        else
                                        {
                                            Int64 TimeStamp = DateTimeHelper.GetTimeStamp();
                                            if (session.PingTimeStamp == 0)
                                                session.PingTimeStamp = TimeStamp;
                                            else
                                            {
                                                Int64 TimeSpan = TimeStamp - session.PingTimeStamp;
                                                if (TimeSpan > 1 * this.PingCount * this.PingTimer / 1000)
                                                {
                                                    this.RemoveQueue(session);
                                                    session.Close();
                                                }
                                            }

                                            this.Send(@"{{""ping"":""{0}""}}".format(TimeStamp), session);
                                        }
                                        if (Interlocked.Decrement(ref _Count) == 0)
                                            mre.Set();
                                    }, values[i]);
                                }
                                catch
                                {
                                    for (int m = 0; m < Count - i; m++)
                                    {
                                        if (Interlocked.Decrement(ref _Count) == 0)
                                            mre.Set();
                                    }
                                    break;
                                }
                            }
                            mre.WaitOne();
                        }
                        /*休息一段时间*/
                        Thread.Sleep(this.PingTimer);
                    }
                }, this.CancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
        }
        #endregion

        #region 发送文件
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public void SendFile(string fileName)
        {
            if (fileName.IsNullOrEmpty()) return;
            this.ConnectionSocketList.Values.Each(socket =>
            {
                this.SendFile(fileName, socket);
            });
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="session">连接</param>
        public void SendFile(string fileName, IServerSession session)
        {
            if (fileName.IsNullOrEmpty() || session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected) return;
            try
            {
                if (session.SocketType == SocketTypes.Socket)
                {
                    session.ConnectionSocket?.SendFile(fileName);
                }
            }
            catch (SocketException ex)
            {
                OnClientError?.Invoke(session, ex);
                //OnError?.Invoke(this, ex);
                this.RemoveQueue(session);
            }
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        public void Send(string message)
        {
            if (message.IsNullOrEmpty()) return;
            this.ConnectionSocketList.Values.Each(socket =>
            {
                this.Send(message, socket);
            });
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="socket">连接</param>
        public void Send(string message, Socket socket)
        {
            if (message.IsNullOrEmpty() || socket == null || !socket.Connected) return;
            var _socket = this.GetData().FirstOrDefault(s => s.ConnectionSocket == socket);
            if (_socket != null) this.Send(message, _socket);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="session">连接</param>
        public void Send(string message, IServerSession session)
        {
            if (message.IsNullOrEmpty() || session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected) return;
            try
            {
                session.Send(message);
            }
            catch (SocketException ex)
            {
                OnError?.Invoke(this, ex);
                this.RemoveQueue(session);
            }
        }
        #endregion

        #region 发送字节
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        public void Send(byte[] bytes)
        {
            if (bytes.IsNullOrEmpty()) return;
            this.ConnectionSocketList.Values.Each(socket =>
            {
                this.Send(bytes, socket);
            });
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        /// <param name="socket">连接</param>
        public void Send(byte[] bytes, Socket socket)
        {
            if (bytes.IsNullOrEmpty() || socket == null || !socket.Connected) return;
            var _socket = this.GetData().FirstOrDefault(s => s.ConnectionSocket == socket);
            if (_socket != null) this.Send(bytes, _socket);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="bytes">消息</param>
        /// <param name="session">连接</param>
        public void Send(byte[] bytes, IServerSession session)
        {
            if (bytes.IsNullOrEmpty() || session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected) return;
            try
            {
                session.Send(bytes);
            }
            catch (SocketException ex)
            {
                //OnError?.Invoke(this, ex);
                OnClientError?.Invoke(session, ex);
                this.RemoveQueue(session);
            }
        }
        #endregion

        #region 队列
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="session"></param>
        public void AddQueue(IServerSession session)
        {
            if (session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected) return;
            if (this.ConnectionSocketList == null) this.ConnectionSocketList = new ConcurrentDictionary<IPEndPoint, IServerSession>();
            this.ConnectionSocketList.TryAdd(session.EndPoint, session);
            string Message = session.ReceivedDataBuffer.GetString(this.Encoding);
            if (!Message.Contains("Sec-WebSocket-Key"))
            {
                session.SocketType = SocketTypes.Socket;
                /*连接成功*/
                //OnNewConnection?.Invoke(session, EventArgs.Empty);
            }
            else
                session.SocketType = SocketTypes.WebSocket;
            OnNewConnection?.Invoke(session, EventArgs.Empty);
        }
        /// <summary>
        /// 移除队列
        /// </summary>
        /// <param name="session">会话</param>
        public void RemoveQueue(INetSession session)
        {
            if (this.ConnectionSocketList == null || session == null || session.ConnectionSocket == null) return;
            try
            {
                lock (this.ConnectionSocketList)
                {
                    this.ConnectionSocketList.TryRemove(session.EndPoint, out IServerSession _);
                }
            }
            catch (SocketException sex)
            {
                LogHelper.Error(sex);
            }
            finally
            {
                OnDisconnected?.Invoke(session, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 移除队列
        /// </summary>
        /// <param name="endPoint">网络地址</param>
        public void RemoveQueue(IPEndPoint endPoint)
        {
            if (endPoint == null) return;
            try
            {
                lock (this.ConnectionSocketList)
                {
                    if (this.ConnectionSocketList != null && this.ConnectionSocketList.Count > 0)
                    {
                        this.ConnectionSocketList.TryRemove(endPoint, out IServerSession _);
                    }
                }
            }
            catch (SocketException sex)
            {
                LogHelper.Error(sex);
            }
            finally
            {
                OnDisconnected?.Invoke(new ServerSession { EndPoint = endPoint }, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 清空队列
        /// </summary>
        public void ClearQueue()
        {
            if (this.ConnectionSocketList != null && this.ConnectionSocketList.Count > 0)
            {
                this.ConnectionSocketList.Clear();
                this.ConnectionSocketList = new ConcurrentDictionary<IPEndPoint, IServerSession>();
            }
        }
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <param name="socket">连接</param>
        /// <returns></returns>
        public IServerSession GetQueue(Socket socket)
        {
            if (socket == null) return null;
            IServerSession _val = null;
            var point = socket.RemoteEndPoint.ToIPEndPoint();
            if (this.ConnectionSocketList != null && this.ConnectionSocketList.Count > 0 && this.ConnectionSocketList.ContainsKey(point))
                this.ConnectionSocketList.TryGetValue(point, out _val);
            return _val;
        }
        /// <summary>
        /// 获取在线列表中的客户端
        /// </summary>
        /// <param name="func">满足条件的函数</param>
        /// <returns></returns>
        public IServerSession GetQueue(Func<IServerSession, bool> func)
        {
            if (func == null) return null;
            return this.ConnectionSocketList.Values.Where(func).FirstOrDefault();
        }
        /// <summary>
        /// 获取队列数
        /// </summary>
        /// <returns></returns>
        public int CountQueue()
        {
            int Count = 0;
            try
            {
                if (this.ConnectionSocketList != null) Count = this.ConnectionSocketList.Count;
            }
            finally
            {
            }
            return Count;
        }
        /// <summary>
        /// 更新队列[不用调用更新队列直接可以更改]
        /// </summary>
        /// <param name="session">连接</param>
        [Obsolete("操作对象就是队列中的对象,直接更新就更新到队列去了,不用再调当前方法")]
        public void UpdateQueue(IServerSession session)
        {
            if (session == null || session.ConnectionSocket == null || !session.ConnectionSocket.Connected) return;

            if (this.ConnectionSocketList == null) this.ConnectionSocketList = new ConcurrentDictionary<IPEndPoint, IServerSession>();
            var point = session.ConnectionSocket.RemoteEndPoint.ToIPEndPoint();
            if (this.ConnectionSocketList.Count == 0 || !this.ConnectionSocketList.ContainsKey(point))
            {
                this.ConnectionSocketList.TryAdd(point, session);
            }
            else
            {
                this.ConnectionSocketList[point] = session;
            }
        }

        #region 复制出一个在线列表
        /// <summary>
        /// 复制出一个在线列表
        /// </summary>
        /// <returns></returns>
        public IServerSession[] GetData()
        {
            try
            {
                var data = new IServerSession[this.CountQueue()];
                this.ConnectionSocketList.Values.CopyTo(data, 0);
                return data;
            }
            finally { }
        }
        #endregion

        #endregion

        #region 黑名单
        /// <summary>
        /// 批量加入黑名单
        /// </summary>
        /// <param name="list">列表</param>
        public void BulkAddIpBlack(List<string> list)
        {
            lock (this.IpBlackList.SyncRoot)
            {
                this.IpBlackList.AddRange(list);
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        public void AddIpBlack(string ip)
        {
            if (ip.IsNullOrEmpty()) return;
            this.RWLock.EnterWriteLock();
            try
            {
                if (this.IpBlackList == null) this.IpBlackList = ArrayList.Synchronized(new ArrayList());
                lock (this.IpBlackList.SyncRoot)
                {
                    if (this.IpBlackList.Count == 0 || !this.IpBlackList.Contains(ip)) this.IpBlackList.Add(ip);
                }
            }
            finally { this.RWLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        public void RemoveIpBlack(string ip)
        {
            if (ip.IsNullOrEmpty()) return;
            this.RWLock.EnterWriteLock();
            try
            {
                lock (this.IpBlackList.SyncRoot)
                {
                    if (this.IpBlackList != null && this.IpBlackList.Count > 0)
                    {
                        ip = ip.ReplacePattern(@"\s+", "");
                        if (this.IpBlackList.Contains(ip)) this.IpBlackList.Remove(ip);
                    }
                }
            }
            finally { this.RWLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 清空黑名单
        /// </summary>
        public void ClearIpBlack()
        {
            this.RWLock.EnterWriteLock();
            try
            {
                lock (this.IpBlackList.SyncRoot)
                {
                    if (this.IpBlackList != null && this.IpBlackList.Count > 0) this.IpBlackList.Clear();
                }
            }
            finally { this.RWLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 是否在黑名单
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public Boolean IsBlackIP(string ip)
        {
            if (ip.IsNullOrEmpty()) return false;
            this.RWLock.EnterReadLock();
            try
            {
                if (this.IpBlackList == null || this.IpBlackList.Count == 0) return false;
                foreach (string a in this.IpBlackList)
                {
                    if (a.IsMatch(ip) ||
                        a.IsMatch(ip.ReplacePattern(@"(\d+)\.(\d+)\.(\d+)\.(\d+)", @"$1.$2.$3.*").ToRegexEscape() + @"(\r\n|$)") ||
                        a.IsMatch(ip.ReplacePattern(@"(\d+)\.(\d+)\.(\d+)\.(\d+)", @"$1.$2.*.*").ToRegexEscape() + @"(\r\n|$)") ||
                        a.IsMatch(ip.ReplacePattern(@"(\d+)\.(\d+)\.(\d+)\.(\d+)", @"$1.*.*.*").ToRegexEscape() + @"(\r\n|$)"))
                    {
                        this.RWLock.ExitReadLock();
                        return true;
                    }
                    List<Dictionary<string, string>> list = a.GetMatches(@"(\d+\.\d+\.\d+\.\d+)-(\d+\.\d+\.\d+\.\d+)");
                    Int64 IP = ip.ReplacePattern(@"(\d+)", m =>
                    {
                        return m.Groups[0].ToString().PadLeft(3, '0');
                    }).ReplacePattern(@"\.", "").ToInt64();
                    Boolean f = false;
                    list.Each(b =>
                    {
                        Int64 begin = b["1"].ReplacePattern(@"(\d+)", m =>
                        {
                            return m.Groups[0].ToString().PadLeft(3, '0');
                        }).ReplacePattern(@"\.", "").ToInt64();
                        Int64 end = b["2"].ReplacePattern(@"(\d+)", m =>
                        {
                            return m.Groups[0].ToString().PadLeft(3, '0');
                        }).ReplacePattern(@"\.", "").ToInt64();
                        if (IP >= begin && IP <= end) { f = true; return false; }
                        else return true;
                    });
                    if (f) { this.RWLock.ExitReadLock(); return true; }
                }
            }
            finally { this.RWLock.ExitReadLock(); }
            return false;
        }
        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~NetServer()
        {
            this.Stop();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭在线列表中的客户端
        /// </summary>
        /// <param name="func">满足条件的函数</param>
        public void Close(Func<IServerSession, bool> func)
        {
            var session = this.GetQueue(func);
            this.RemoveQueue(session);
            session.Close();
        }
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="session">连接</param>
        public void Close(IServerSession session = null)
        {
            if (session != null)
            {
                this.RemoveQueue(session);
                session.Close();
            }
        }
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="endPoint">IP节点</param>
        public void Close(IPEndPoint endPoint)
        {
            if (this.ConnectionSocketList.TryGetValue(endPoint, out var socket))
            {
                socket.Close();
            }
        }
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="endPoint">IP节点</param>
        public void Close(EndPoint endPoint)
        {
            this.Close(endPoint.ToIPEndPoint());
        }
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="ip">客户端IP</param>
        /// <param name="port">端口</param>
        public void Close(string ip, string port)
        {
            this.Close(ip, port.ToCast<int>());
        }
        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="ip">客户端IP</param>
        /// <param name="port">端口</param>
        public void Close(string ip, int port)
        {
            this.Close(new IPEndPoint(IPAddress.Parse(ip), port));
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void Stop()
        {
            this.CancelToken.Cancel();
            if (!AlreadyDisposed)
            {
                AlreadyDisposed = true;
                if (ServerSocket != null)
                {
                    ServerSocket.Close();
                }
                this.RWLock.EnterWriteLock();
                try
                {
                    this.ConnectionSocketList.Values.Each(item =>
                    {
                        item.Close();
                    });
                    this.ConnectionSocketList.Clear();
                }
                finally { this.RWLock.ExitWriteLock(); }
                GC.SuppressFinalize(this);
            }
            this.SocketState = SocketState.Stop;
        }
        #endregion

        #region 是否在线
        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="endPoint">IP和port</param>
        /// <returns></returns>
        public Boolean IsConnected(IPEndPoint endPoint)
        {
            if (this.ConnectionSocketList.TryGetValue(endPoint, out var socket))
            {
                return socket.IsConnected();
            }
            return false;
        }
        #endregion
    }
}