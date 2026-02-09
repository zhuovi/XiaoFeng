using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Collections;
using XiaoFeng.Data;
using XiaoFeng.Threading;

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
    public partial class RedisClient : Disposable, IRedisClient
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
        public RedisClient(ConnectionConfig config) { this.ConnConfig = config.ToRedisConfig(); }
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
        /// Redis
        /// </summary>
        public IO.IRedisSocket Redis { get; set; }
        /// <summary>
        /// 连接池
        /// </summary>
        private readonly static ConcurrentDictionary<string, RedisPool> RedisPools = new ConcurrentDictionary<string, RedisPool>();
        /// <summary>
        /// 连接池
        /// </summary>
        public RedisPool RedisPool
        {
            get
            {
                return Synchronized.Run(() =>
                {
                    if (RedisPools.TryGetValue(this.ConnConfig.ToString(), out var _pool))
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
                        RedisPools.TryAdd(this.ConnConfig.ToString(), pool);
                        return pool;
                    }
                });
            }
        }
        /// <summary>
        /// RedisSocket 项
        /// </summary>
        public PoolItem<IO.IRedisSocket> RedisItem { get; set; }
        /// <summary>
        /// 排它锁
        /// </summary>
        private static Mutex Mutex = new Mutex(false, "RedisMutex");
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object RedisLock = new object();
        /// <summary>
        /// 流异步锁
        /// </summary>
        readonly AsyncLock StreamAsyncLock = new AsyncLock();
        /// <summary>
        /// 是否调试
        /// </summary>
        public Boolean Debug { get; set; }
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.TraceInfo("开始初始化.");
            if (this.Redis == null)
                if (this.ConnConfig.IsPool && this.ConnConfig.MaxPool > 0)
                {
                    this.TraceInfo("从应用池里获取一个新的连接.");
                    this.RedisItem = this.RedisPool.Get();
                    this.Redis = this.RedisItem.Value;
                }
                else
                {
                    this.TraceInfo($"重新实例化一个新的连接[{this.ConnConfig}].");
                    this.Redis = new IO.RedisSocket(this.ConnConfig)
                    {
                        AddressFamily = this.AddressFamily,
                        ProtocolType = this.ProtocolType,
                        SocketType = this.SocketType,
                        ReceiveTimeout = this.ReceiveTimeout,
                        SendTimeout = this.SendTimeout
                    };
                }
            lock (this.Redis)
            {
                this.TraceInfo($"连接实例连接状态:{this.Redis.IsConnected}");
                if (!this.Redis.IsConnected)
                    this.Redis.Connect();
            }
        }
        #endregion

        #region 归还
        /// <summary>
        /// 归还
        /// </summary>
        private void Put()
        {
            if(this.ConnConfig.IsPool && this.ConnConfig.MaxPool > 0)
            {
                if (this.RedisItem != null && this.RedisItem.Value != null)
                    this.RedisPool.Put(this.RedisItem);
            }
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (this.ConnConfig.IsPool && this.ConnConfig.MaxPool > 0)
            {
                this.TraceInfo("还给连接池一个实例.");
                RedisPool.Put(this.RedisItem);
            }
            else
            {
                this.TraceInfo("关闭连接实例.");
                this.Redis.Close();
            }
            Mutex = new Mutex(false, "RedisMutex");
            StreamAsyncLock?.Dispose();
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
        protected T Execute<T>(CommandType commandType, int? dbNum, Func<RedisReader, T> func, params object[] args)
        {
            this.TraceInfo($"请求参数:CommandType={commandType},DbNum={dbNum.GetValueOrDefault()},参数={args.ToJson()}");
            this.Init();
            if (!this.Redis.IsAuth && commandType != CommandType.AUTH && this.ConnConfig.Password.IsNotNullOrEmpty())
            {
                this.Redis.IsAuth = this.Auth(this.ConnConfig.Password);
                this.TraceInfo($"Redis认证{(this.Redis.IsAuth ? "成功" : "失败")}.");
                if (!this.Redis.IsAuth)
                    throw new RedisException("认证失败.");
            }
            if (!dbNum.HasValue && !this.Redis.DbNum.HasValue && this.ConnConfig.DbNum > 0)
                this.Redis.DbNum = this.ConnConfig.DbNum;
            if (dbNum.HasValue && dbNum > -1 && dbNum != this.Redis.DbNum)
            {
                if (this.Select(dbNum.Value))
                    this.Redis.DbNum = dbNum;
                else
                    throw new RedisException("选库出错.");
            }
            try
            {
                //Mutex.WaitOne(TimeSpan.FromMilliseconds(1000));
                lock (RedisLock)
                {
                    var cmd = new CommandPacket(commandType, args);
                    this.TraceInfo($"发送 [{commandType}] <{commandType.Description}> 命令行:\r\n{cmd}");
                    cmd.SendCommand(this.Redis.GetStream() as NetworkStream);
                    var bytes = this.ReadResponseBytes().ConfigureAwait(false).GetAwaiter().GetResult();
                    var ms = new MemoryStream(bytes);
                    ms.Seek(0, SeekOrigin.Begin);
                    this.TraceInfo($"响应数据:\r\n{bytes.GetString()}");
                    var result = func.Invoke(new RedisReader(commandType, ms, args, this.TraceInfo));
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return default;
            }
            finally
            {
                this.Put();
                //Mutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// 读取流数据
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> ReadResponseBytes()
        {
            var reader = this.Redis.GetStream() as NetworkStream;
            //await Task.Delay(10).ConfigureAwait(false);
            var ms = new MemoryStream();
            var bytes = new byte[1024];
            do
            {
                var length = await reader.ReadAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                if (length == 0) break;
                ms.Write(bytes, 0, (int)length);
                if (length < bytes.Length) break;
            } while (reader.DataAvailable);
            return ms.ToArray();
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
        protected async Task<T> ExecuteAsync<T>(CommandType commandType, int? dbNum, Func<RedisReader, Task<T>> func, params object[] args)
        {
            this.TraceInfo($"请求参数:CommandType={commandType},DbNum={dbNum.GetValueOrDefault()},参数={args.ToJson()}");
            this.Init();
            if (commandType != CommandType.AUTH && this.ConnConfig.Password.IsNotNullOrEmpty())
            {
                this.Redis.IsAuth = await this.AuthAsync(this.ConnConfig.Password).ConfigureAwait(false);
                this.TraceInfo($"Redis认证{(this.Redis.IsAuth ? "成功" : "失败")}.");
                if (!this.Redis.IsAuth)
                    throw new RedisException("认证失败.");
            }
            if (!dbNum.HasValue && !this.Redis.DbNum.HasValue && this.ConnConfig.DbNum > 0)
                this.Redis.DbNum = this.ConnConfig.DbNum;
            if (dbNum.HasValue && dbNum > -1 && dbNum != this.Redis.DbNum)
            {
                if (await this.SelectAsync(dbNum.Value).ConfigureAwait(false))
                    this.Redis.DbNum = dbNum;
                else
                    throw new RedisException("选库出错.");
            }
            try
            {
                //Mutex.WaitOne(TimeSpan.FromMilliseconds(1000));
                using (await StreamAsyncLock.EnterAsync().ConfigureAwait(false))
                {
                    var cmd = new CommandPacket(commandType, args);
                    this.TraceInfo($"发送 [{commandType}]<{commandType.Description}> 命令行:\r\n{cmd}");
                    await cmd.SendCommandAsync(this.Redis.GetStream() as NetworkStream).ConfigureAwait(false);
                    var bytes = await this.ReadResponseBytes().ConfigureAwait(false);
                    var ms = new MemoryStream(bytes);
                    ms.Position = 0;
                    this.TraceInfo($"响应数据:\r\n{bytes.GetString()}");
                    var result = await func.Invoke(new RedisReader(commandType, ms, args, this.TraceInfo)).ConfigureAwait(false);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return default;
            }
            finally
            {
                this.Put();
                //Mutex.ReleaseMutex();
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
            this.TraceInfo($"开始认证,认证密码:{password}");
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
            this.TraceInfo($"开始认证,认证密码:{password}");
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
        public async Task<Boolean> PingAsync() => await this.ExecuteAsync(CommandType.PING, null, async result => await Task.FromResult(result.OK && result.Value.ToString() == "PONG"));
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
        public Boolean Select(int dbNum = 0)
        {
            var curDbNum = this.Redis.DbNum.GetValueOrDefault();
            if (Interlocked.CompareExchange(ref curDbNum, dbNum, dbNum) == dbNum) return true;
            this.TraceInfo($"开始切换库,原来库为 {curDbNum},目的库为 {dbNum}");
            if (this.Execute(CommandType.SELECT, null, result => result.OK, Math.Abs(dbNum)))
            {
                this.Redis.DbNum = dbNum;
                this.TraceInfo($"切换库成功,当前库为 {dbNum}");
                return true;
            }
            else
            {
                this.TraceInfo($"切换库失败.");
                return false;
            }
        }
        /// <summary>
        /// 选择数据库 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SelectAsync(int dbNum = 0)
        {
            var curDbNum = this.Redis.DbNum.GetValueOrDefault();
            if (Interlocked.CompareExchange(ref curDbNum, dbNum, dbNum) == dbNum) return true;
            this.TraceInfo($"开始切换库,原来库为 {curDbNum},目的库为 {dbNum}");
            if( await this.ExecuteAsync(CommandType.SELECT, null, result => Task.FromResult(result.OK), Math.Abs(dbNum)))
            {
                this.Redis.DbNum = dbNum;
                this.TraceInfo($"切换库成功,当前库为 {dbNum}");
                return true;
            }
            else
            {
                this.TraceInfo($"切换库失败.");
                return false;
            }
        }
        #endregion

        #region 交换两个数据库
        /// <summary>
        /// 交换两个数据库
        /// </summary>
        /// <param name="oldDb">原库索引</param>
        /// <param name="newDb">新库索引</param>
        /// <returns></returns>
        public bool Swapdb(uint oldDb, uint newDb)
        {
            if (oldDb == newDb) return false;

            this.TraceInfo($"开始交换库,原来库为 {oldDb},目的库为 {newDb}");
            if (this.Execute(CommandType.SWAPDB, null, result => result.OK, oldDb, newDb))
            {
                this.TraceInfo($"交换库成功.");
                return true;
            }
            else
            {
                this.TraceInfo($"交换库失败.");
                return false;
            }
        }
        /// <summary>
        /// 交换两个数据库
        /// </summary>
        /// <param name="oldDb">原库索引</param>
        /// <param name="newDb">新库索引</param>
        /// <returns></returns>
        public async Task<bool> SwapdbAsync(uint oldDb, uint newDb)
        {
            if (oldDb == newDb) return false;

            this.TraceInfo($"开始交换库,原来库为 {oldDb},目的库为 {newDb}");
            if (await this.ExecuteAsync(CommandType.SWAPDB, null, result => Task.FromResult(result.OK), oldDb, newDb).ConfigureAwait(false))
            {
                this.TraceInfo($"交换库成功.");
                return true;
            }
            else
            {
                this.TraceInfo($"交换库失败.");
                return false;
            }
        }
        #endregion

        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <param name="isValue">是否是值类型</param>
        /// <returns></returns>
        private string GetValue<T>(T value, out Boolean isValue)
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
            this.Close();
            this.Dispose(true);
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">释放状态</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing, () =>
            {
                if (this.Redis != null)
                {
                    this.Redis.Dispose();
                    this.Redis = null;
                }
                this.RedisPool?.Dispose();
                if (StreamAsyncLock != null) StreamAsyncLock.Dispose();
            });
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~RedisClient()
        {
            this.Dispose(false);
        }
        #endregion

        #region 输出调试日志
        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="data">数据</param>
        public void TraceInfo(object data)
        {
            if (!this.Debug) return;
            var message = string.Empty;
            if (data.IsNotNullOrEmpty())
            {
                var valType = data.GetType().GetValueType();
                if (valType == ValueTypes.Value || valType == ValueTypes.String || valType == ValueTypes.Enum) message = data.ToString();
                else message = data.ToJson();
            }
            LogHelper.Debug($"{message}");
        }
        #endregion

        #endregion
    }
}