using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Data;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-14 14:33:18                            *
*  Version : v 1.1.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis 客户端操作类
    /// v 1.1.0
    /// 修改提取数据用正则改为字符提取 更加精确
    /// </summary>
    public partial class RedisClient : Disposable
    {
        #region 构造器
        /// <summary>
        /// 设置连接串
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="password">密码</param>
        /// <param name="MaxPool">应用池数量</param>
        public RedisClient(string host = "127.0.0.1", int port = 6379, string password = "", int? MaxPool = null)
        {
            this.ConnConfig = new RedisConfig
            {
                ProviderType = DbProviderType.Redis,
                Host = host,
                Port = port,
                Password = password,
                MaxPool = MaxPool ?? 0,
                IsPool = MaxPool.HasValue
            };
            //var conn = this.CreateConn();
            //if (!conn) throw new Exception("连接失败.");
        }
        /// <summary>
        /// 设置连接串
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="password">密码</param>
        /// <param name="port">端口</param>
        /// <param name="MaxPool">应用池数量</param>
        public RedisClient(string host, string password, int port = 6379, int? MaxPool = null) : this(host, port, password, MaxPool)
        { }
        /// <summary>
        /// 设置连接串
        /// </summary>
        /// <param name="connectionString">连接串</param>
        public RedisClient(string connectionString)
        {
            this.ConnConfig = new RedisConfig(connectionString);
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">配置</param>
        public RedisClient(ConnectionConfig config)  { this.ConnConfig = config.ToRedisConfig(); }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">Redis配置</param>
        public RedisClient(RedisConfig config) { this.ConnConfig = config; }
        #endregion

        #region 属性
        /// <summary>
        /// 连接配置
        /// </summary>
        public RedisConfig ConnConfig { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        public Boolean? IsConnected { get; set; }
        /// <summary>
        /// 寻址方案
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;
        /// <summary>
        /// 套接字类型
        /// </summary>
        public SocketType SocketType { get; set; } = SocketType.Stream;
        /// <summary>
        /// 支持协议
        /// </summary>
        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        /// <summary>
        /// 发送超时
        /// </summary>
        public int SendTimeout { get; set; } = 10000;
        /// <summary>
        /// 接收超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 10000;
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int MemorySize { get; set; } = 1024;
        /// <summary>
        /// 网络流
        /// </summary>
        public NetworkStream Stream { get; set; }
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket SocketClient { get; set; }
        /// <summary>
        /// 连接池
        /// </summary>
        private readonly static ConcurrentDictionary<string, RedisPool> RedisPool = new ConcurrentDictionary<string, RedisPool>();
        /// <summary>
        /// 连接池
        /// </summary>
        public RedisPool _Pool
        {
            get
            {
                return XiaoFeng.Threading.Synchronized.Run(() =>
                {
                    if (RedisPool.TryGetValue(this.ConnConfig.ToString(), out var _pool))
                    {
                        return _pool;
                    }
                    else
                    {
                        var pool = new RedisPool
                        {
                            Host = this.ConnConfig.Host,
                            Port = this.ConnConfig.Port,
                            Password = this.ConnConfig.Password,
                            Max = this.ConnConfig.MaxPool,
                            ReceiveTimeout = this.ConnConfig.ReadTimeout,
                            SendTimeout = this.ConnConfig.CommandTimeOut
                        };
                        RedisPool.TryAdd(this.ConnConfig.ToString(), pool);
                        return pool;
                    }
                });
            }
        }
        #endregion

        #region 方法

        #region 创建连接
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public Boolean CreateConn()
        {
           //return XiaoFeng.Threading.Synchronized.Run(() =>
            {
                if (SocketClient != null && SocketClient.Connected) return true;
                SocketClient = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType)
                {
                    SendTimeout = this.SendTimeout,
                    ReceiveTimeout = this.ReceiveTimeout
                };
                if (this.ConnConfig.Host.IsNullOrEmpty()) this.ConnConfig.Host = "127.0.0.1";
                if (this.ConnConfig.Port == 0) this.ConnConfig.Port = 6379;
                try
                {
                    SocketClient.Connect(this.ConnConfig.Host, this.ConnConfig.Port);
                    Stream = new NetworkStream(SocketClient)
                    {
                        ReadTimeout = this.ReceiveTimeout,
                        WriteTimeout = this.SendTimeout
                    };
                    this.IsConnected = true;
                    return (Boolean)(this.IsConnected = this.ConnConfig.Password.IsNullOrEmpty() || this.Auth(this.ConnConfig.Password));
                }
                catch (SocketException ex)
                {
                    LogHelper.Error(ex, "SOCKET创建失败.");
                    return (Boolean)(this.IsConnected = false);
                }
            }//);
        }
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> CreateConnAsync()
        {
            //return XiaoFeng.Threading.Synchronized.Run(() =>
            {
                if (SocketClient != null && SocketClient.Connected) return true;
                SocketClient = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType)
                {
                    SendTimeout = this.SendTimeout,
                    ReceiveTimeout = this.ReceiveTimeout
                };
                if (this.ConnConfig.Host.IsNullOrEmpty()) this.ConnConfig.Host = "127.0.0.1";
                if (this.ConnConfig.Port == 0) this.ConnConfig.Port = 6379;
                try
                {
                   await  SocketClient.ConnectAsync(this.ConnConfig.Host, this.ConnConfig.Port);
                    Stream = new NetworkStream(SocketClient)
                    {
                        ReadTimeout = this.ReceiveTimeout,
                        WriteTimeout = this.SendTimeout
                    };
                    this.IsConnected = true;
                    return (Boolean)(this.IsConnected = this.ConnConfig.Password.IsNullOrEmpty() || this.Auth(this.ConnConfig.Password));
                }
                catch (SocketException ex)
                {
                    LogHelper.Error(ex, "SOCKET创建失败.");
                    return (Boolean)(this.IsConnected = false);
                }
            }//);
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (this.IsConnected.HasValue && this.IsConnected == true)
            {
                this.Quit();
                if (Stream != null)
                {
                    Stream.Close();
                    Stream.Dispose();
                }
                if (SocketClient != null)
                {
                    if (SocketClient.Connected)
                        SocketClient.Disconnect(true);
                    SocketClient.Close();
                    SocketClient.Dispose();
                }
            }
        }
        #endregion

        #region 获取响应
        /// <summary>
        /// 获取响应
        /// </summary>
        /// <param name="commandType">命令</param>
        /// <returns>响应结果</returns>
        public CommandResult GetCommandResult(CommandType commandType)
        {
            var ms = new MemoryStream();
            var num = 0;
            var length = 0;
            while (this.IsConnected.HasValue && this.IsConnected.Value)
            {
                if (Stream.DataAvailable)
                {
                    var bytes = new byte[MemorySize];
                    do
                    {
                        Array.Clear(bytes, 0, MemorySize);
                        var count = Stream.Read(bytes, 0, bytes.Length);
                        ms.Write(bytes, 0, count);

                    } while (Stream.DataAvailable);
                    num++;
                    if (commandType == CommandType.HGET)
                    {
                        if (num == 1)
                        {
                            var str = ms.ToArray().GetString();
                            if (str.IsMatch(@"^\$\d+\r\n"))
                                length = str.GetMatch(@"^\$(?<a>\d+)\r\n").ToCast<int>();
                            else break;
                        }
                        if (length == ms.Length - length.ToString().Length - 5) break;
                    }else
                        break;
                }
            }
            return new CommandResult(commandType, ms.ToArray());
        }
        #endregion

        #region 执行
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="func">回调方法</param>
        /// <param name="args">参数集</param>
        /// <returns>执行结果</returns>
        public T Execute<T>(CommandType commandType, int? dbNum, Func<CommandResult, T> func, params object[] args)
        {
            if (this.ConnConfig.IsPool)
            {
                return _Pool.Execute(commandType, dbNum, func, args);
            }
            else
            {
                if (!this.IsConnected.HasValue || !this.IsConnected.Value) this.CreateConn();
                if (!this.IsConnected.Value) return default(T);
                if (dbNum.HasValue && dbNum.Value > -1) this.Select(dbNum.Value);
                var line = new Command(commandType, args).ToBytes();
                Stream.Write(line, 0, line.Length);
                Stream.Flush();
                return func.Invoke(this.GetCommandResult(commandType));
            }
        }
        /// <summary>
        /// 执行 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="func">回调方法</param>
        /// <param name="args">参数集</param>
        /// <returns>执行结果</returns>
        public async Task<T> ExecuteAsync<T>(CommandType commandType, int? dbNum, Func<CommandResult, Task<T>> func, params object[] args)
        {
            if (this.ConnConfig.IsPool)
            {
                return await _Pool.ExecuteAsync(commandType, dbNum, func, args);
            }
            else
            {
                if (!this.IsConnected.HasValue) await this.CreateConnAsync();
                if (!this.IsConnected.Value) return default(T);
                if (dbNum.HasValue && dbNum.Value > -1) await this.SelectAsync(dbNum.Value);
                var line = new Command(commandType, args).ToBytes();
                await Stream.WriteAsync(line, 0, line.Length).ConfigureAwait(false);
                await Stream.FlushAsync().ConfigureAwait(false);
                return await func.Invoke(this.GetCommandResult(commandType));
            }
        }
        #endregion

        #region 连接

        #region 验证密码
        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public Boolean Auth(string password)
        {
            if (password.IsNullOrEmpty()) return false;

            return this.Execute(CommandType.AUTH, null, result => result.OK, password);
        }
        /// <summary>
        /// 验证密码 异步
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<Boolean> AuthAsync(string password)
        {
            if (password.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.AUTH, null, async result => await Task.FromResult(result.OK), password);
        }
        #endregion

        #region PING
        /// <summary>
        /// PING
        /// </summary>
        /// <returns></returns>
        public Boolean Ping() => this.Execute(CommandType.PING, null, result => result.OK && result.Value.ToString() == "PONG");
        /// <summary>
        /// PING 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> PingAsync() =>await this.ExecuteAsync(CommandType.PING, null,async result =>await Task.FromResult(result.OK && result.Value.ToString() == "PONG"));
        #endregion

        #region 打印字符串
        /// <summary>
        /// 打印字符串
        /// </summary>
        /// <param name="echoStr">要打印的字符串</param>
        /// <returns></returns>
        public Boolean Echo(string echoStr)
        {
            if (echoStr.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.ECHO, null, result => result.OK && result.Value.ToString() == echoStr, echoStr);
        }
        /// <summary>
        /// 打印字符串 异步
        /// </summary>
        /// <param name="echoStr">要打印的字符串</param>
        /// <returns></returns>
        public async Task<Boolean> EchoAsync(string echoStr)
        {
            if (echoStr.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.ECHO, null, async result => await Task.FromResult(result.OK && result.Value.ToString() == echoStr), echoStr);
        }
        #endregion

        #region 退出
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public Boolean Quit() => this.Execute(CommandType.QUIT, null, result => result.OK);
        /// <summary>
        /// 退出 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> QuitAsync() => await this.ExecuteAsync(CommandType.QUIT, null, async result => await Task.FromResult(result.OK));
        #endregion

        #region 选择数据库
        /// <summary>
        /// 选择数据库
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public Boolean Select(int dbNum = 0) => this.Execute(CommandType.SELECT, null, result => result.OK, Math.Abs(dbNum));
        /// <summary>
        /// 选择数据库 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SelectAsync(int dbNum = 0) => await this.ExecuteAsync(CommandType.SELECT, null, result => Task.FromResult(result.OK), Math.Abs(dbNum));
        #endregion

        #endregion

        #region 订阅发布

        #endregion

        #region Stream
        
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <param name="isValue">是否是值类型</param>
        /// <returns></returns>
        private string GetValue<T>(T value,out Boolean isValue)
        {
            var type = value.GetType();
            var valueType = type.GetValueType();
            isValue = false;
            if (valueType == ValueTypes.Null || valueType == ValueTypes.String || valueType == ValueTypes.Value || valueType == ValueTypes.Other)
            {
                isValue = true;
                return (type == typeof(DateTime) || type == typeof(DateTime?)) ? value.ToCast<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ffffff") : value.ToString();
            }
            else return value.ToJson();
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        private string GetValue<T>(T value) => this.GetValue(value, out _);
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="values">数据集合</param>
        /// <returns></returns>
        private List<string> GetValues(params object[] values)
        {
            var list = new List<string>();
            values.Each(t => list.Add(GetValue(t)));
            return list;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        private T SetValue<T>(object value)
        {
            var type = typeof(T).GetValueType();
            return (type == ValueTypes.Null || type == ValueTypes.String || type == ValueTypes.Value || type == ValueTypes.Other) ? value.ToCast<T>() : value.ToString().JsonToObject<T>();
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            //this.Close();
            base.Dispose();
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~RedisClient(){
            this.Dispose();
        }
        #endregion

        #endregion
    }
}