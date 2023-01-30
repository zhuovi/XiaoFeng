using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Collections;
using XiaoFeng.Data;
using XiaoFeng.Threading;
using XiaoFeng.Memcached.Transform;
using System.Linq;
/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-06 15:58:27                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcached客户端
    /// </summary>
    public class MemcachedClient : Disposable, IMemcachedClient
    {
        #region 构造器
        /// <summary>
        /// 设置连接串
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="user">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="MaxPool">应用池数量</param>
        public MemcachedClient(string host = "127.0.0.1", int port = 1111, string user = "", string password = "", int? MaxPool = null)
        {
            this.ConnConfig = new MemcachedConfig
            {
                ProviderType = DbProviderType.Memcached,
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
        /// <param name="user">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="port">端口</param>
        /// <param name="MaxPool">应用池数量</param>
        public MemcachedClient(string host, string user, string password, int port = 11211, int? MaxPool = null) : this(host, port, user, password, MaxPool)
        { }
        /// <summary>
        /// 设置连接串
        /// </summary>
        /// <param name="connectionString">连接串</param>
        public MemcachedClient(string connectionString)
        {
            this.ConnConfig = new MemcachedConfig(connectionString);
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">配置</param>
        public MemcachedClient(ConnectionConfig config) { this.ConnConfig = new MemcachedConfig(config); }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">Redis配置</param>
        public MemcachedClient(MemcachedConfig config) { this.ConnConfig = config; }
        #endregion

        #region 属性
        /// <summary>
        /// 连接配置
        /// </summary>
        public MemcachedConfig ConnConfig { get; set; }
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
        public IO.IMemcachedSocket Memcached { get; set; }
        /// <summary>
        /// Hash算法
        /// </summary>
        public IMemcachedTransform Transform { get; set; }
        /// <summary>
        /// 协议
        /// </summary>
        public MemcachedProtocol MemcachedProtocol { get; set; } = MemcachedProtocol.Text;
        /// <summary>
        /// 压缩值 1M
        /// </summary>
        public uint CompressLength { get; set; } = 1024 * 1024;
        /// <summary>
        /// 连接池
        /// </summary>
        private readonly static ConcurrentDictionary<string, MemcachedPool> MemcachedPools = new ConcurrentDictionary<string, MemcachedPool>();
        /// <summary>
        /// 连接池
        /// </summary>
        private MemcachedPool MemcachedPool
        {
            get
            {
                return Synchronized.Run(() =>
                {
                    if (MemcachedPools.TryGetValue(this.ConnConfig.ToString(), out var _pool))
                    {
                        return _pool;
                    }
                    else
                    {
                        var pool = new MemcachedPool
                        {
                            Host = this.ConnConfig.Host,
                            Port = this.ConnConfig.Port,
                            User = this.ConnConfig.User,
                            Password = this.ConnConfig.Password,
                            Max = this.ConnConfig.MaxPool,
                            ReceiveTimeout = this.ConnConfig.ReadTimeout,
                            SendTimeout = this.ConnConfig.CommandTimeOut
                        };
                        MemcachedPools.TryAdd(this.ConnConfig.ToString(), pool);
                        return pool;
                    }
                });
            }
        }
        /// <summary>
        /// IMemcachedSocket 项
        /// </summary>
        private PoolItem<IO.IMemcachedSocket> MemcachedItem { get; set; }
        /// <summary>
        /// 排它锁
        /// </summary>
        private static readonly Mutex Mutex = new Mutex();
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (this.Memcached == null)
                if (this.ConnConfig.IsPool && this.ConnConfig.MaxPool > 0)
                {
                    this.MemcachedItem = this.MemcachedPool.Get();
                    this.Memcached = this.MemcachedItem.Value;
                }
                else
                    this.Memcached = new IO.MemcachedSocket(this.ConnConfig)
                    {
                        AddressFamily = this.AddressFamily,
                        ProtocolType = this.ProtocolType,
                        SocketType = this.SocketType,
                        ReceiveTimeout = this.ReceiveTimeout,
                        SendTimeout = this.SendTimeout
                    };
            if (!this.Memcached.IsConnected)
                this.Memcached.Connect();

            if (this.Transform == null)
                this.Transform = new ModifiedFNV1_32();
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (this.ConnConfig.IsPool && this.ConnConfig.MaxPool > 0)
                MemcachedPool.Put(this.MemcachedItem);
            else
                this.Memcached.Close();
        }
        #endregion

        #region 执行
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="func">回调方法</param>
        /// <param name="args">参数集</param>
        /// <returns>执行结果</returns>
        protected T Execute<T>(CommandType commandType, Func<MemcachedReader, T> func, params object[] args)
        {
            Mutex.WaitOne();
            this.Init();
            if (!this.Memcached.IsAuth && commandType != CommandType.AUTH && this.ConnConfig.Password.IsNotNullOrEmpty())
            {
                this.Memcached.IsAuth = this.Auth();
                if (!this.Memcached.IsAuth)
                    throw new MemcachedException("认证失败.");
            }
            new CommandPacket(commandType, this.CompressLength, args).SendCommand(this.Memcached.GetStream() as NetworkStream);
            var result = func.Invoke(new MemcachedReader(commandType, this.Memcached.GetStream() as NetworkStream, args));
            Mutex.ReleaseMutex();
            return result;
        }
        /// <summary>
        /// 执行 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="func">回调方法</param>
        /// <param name="args">参数集</param>
        /// <returns>执行结果</returns>
        protected async Task<T> ExecuteAsync<T>(CommandType commandType, Func<MemcachedReader, Task<T>> func, params object[] args)
        {
            Mutex.WaitOne();
            this.Init();
            if (commandType != CommandType.AUTH && this.ConnConfig.Password.IsNotNullOrEmpty())
            {
                this.Memcached.IsAuth = await this.AuthAsync();
                if (!this.Memcached.IsAuth)
                    throw new MemcachedException("认证失败.");
            }
            new CommandPacket(commandType, this.CompressLength, args).SendCommand(this.Memcached.GetStream() as NetworkStream);
            var result = func.Invoke(new MemcachedReader(commandType, this.Memcached.GetStream() as NetworkStream, args));
            Mutex.ReleaseMutex();
            return await result;
        }
        #endregion

        #region 认证
        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        public Boolean Auth()
        {
            /*
             * 认证命令一直没试出来，暂只支持没有帐号密码的
             */
            return true;
            if (this.ConnConfig.User.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.AUTH, result => result.OK, this.ConnConfig.User, this.ConnConfig.Password);
        }
        /// <summary>
        /// 认证 异步
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> AuthAsync()
        {
            /*
             * 认证命令一直没试出来，暂只支持没有帐号密码的
             */
            return true;
            if (this.ConnConfig.User.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.AUTH, async result => await Task.FromResult(result.OK), this.ConnConfig.User, this.ConnConfig.Password);
        }
        #endregion

        #region Hash
        /// <summary>
		/// 获取Hash
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Hash值</returns>
		private uint GetHash(string key)
        {
            Helper.CheckKey(key);
            return BitConverter.ToUInt32(this.Transform.ComputeHash(key.GetBytes(Encoding.UTF8)), 0);
        }
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="hashValue">值</param>
        /// <returns>Hash值</returns>
        private uint GetHash(uint hashValue)
        {
            return BitConverter.ToUInt32(this.Transform.ComputeHash(BitConverter.GetBytes(hashValue)), 0);
        }
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Hash值</returns>
        private uint[] GetHash(string[] keys)
        {
            uint[] result = new uint[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                result[i] = GetHash(keys[i]);
            return result;
        }
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="hashValues">值</param>
        /// <returns>Hash值</returns>
        private uint[] GetHash(uint[] hashValues)
        {
            uint[] result = new uint[hashValues.Length];
            for (int i = 0; i < hashValues.Length; i++)
                result[i] = GetHash(hashValues[i]);
            return result;
        }
        #endregion

        #region Store 存储
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public Boolean Set<T>(string key, T value, uint exptime = 0) => this.Store(CommandType.SET, key, value, exptime);
        /// <summary>
        /// 给key设置一个值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> SetAsync<T>(string key, T value, uint exptime = 0) => await this.StoreAsync(CommandType.SET, key, value, exptime);
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public Boolean Add<T>(string key, T value, uint exptime = 0) => this.Store(CommandType.ADD, key, value, exptime);
        /// <summary>
        /// 如果key不存在的话，就添加 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> AddAsync<T>(string key, T value, uint exptime = 0) => await this.StoreAsync(CommandType.ADD, key, value, exptime);
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public Boolean Replace<T>(string key, T value, uint exptime = 0) => this.Store(CommandType.REPLACE, key, value, exptime);
        /// <summary>
        /// 用来替换已知key的value 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> ReplaceAsync<T>(string key, T value, uint exptime = 0) => await this.StoreAsync(CommandType.REPLACE, key, value, exptime);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public Boolean Append<T>(string key, T value, uint exptime = 0) => this.Store(CommandType.APPEND, key, value, exptime);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> AppendAsync<T>(string key, T value, uint exptime = 0) => await this.StoreAsync(CommandType.APPEND, key, value, exptime);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public Boolean Prepend<T>(string key, T value, uint exptime = 0) => this.Store(CommandType.PREPEND, key, value, exptime);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> PrependAsync<T>(string key, T value, uint exptime = 0) => await this.StoreAsync(CommandType.PREPEND, key, value, exptime);
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <returns>成功状态</returns>
        public Boolean Cas<T>(string key, T value, ulong casToken, uint exptime = 0) => this.Store(CommandType.CAS, key, value, exptime, casToken);
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> CasAsync<T>(string key, T value, ulong casToken, uint exptime = 0) => await this.StoreAsync(CommandType.CAS, key, value, exptime, casToken);
        /// <summary>
        /// 存储
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <returns>成功状态</returns>
        public Boolean Store<T>(CommandType commandType, string key, T value, uint exptime = 0, ulong casToken = 0)
        {
            var list = new List<object> { key, exptime };
            if (casToken > 0) list.Add(casToken);
            list.Add(value);
            return this.Execute(commandType, reader => reader.OK, list.ToArray());
        }
        /// <summary>
        /// 存储 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="commandType">命令</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <returns>成功状态</returns>
        public async Task<Boolean> StoreAsync<T>(CommandType commandType, string key, T value, uint exptime = 0, ulong casToken = 0)
        {
            var list = new List<object> { key, exptime };
            if (casToken > 0) list.Add(casToken);
            list.Add(value);
            return await this.ExecuteAsync(commandType, async reader => await Task.FromResult(reader.OK), list.ToArray());
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public MemcachedValue Get(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GET, reader => reader.OK ? reader.Value?[0] : null, key);
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public async Task<MemcachedValue> GetAsync(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GET, async reader => await Task.FromResult(reader.OK ? reader.Value?[0] : null), key);
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public List<MemcachedValue> Get(params string[] keys)
        {
            if (keys.Length == 0) return null;
            return this.Execute(CommandType.GET, reader => reader.OK ? reader.Value : null, keys);
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public async Task<List<MemcachedValue>> GetAsync(params string[] keys)
        {
            if (keys.Length == 0) return null;
            return await this.ExecuteAsync(CommandType.GET, async reader => await Task.FromResult(reader.OK ? reader.Value : null), keys);
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public MemcachedValue Gets(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GETS, reader => reader.OK ? reader.Value?[0] : null, key);
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public async Task<MemcachedValue> GetsAsync(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GETS, async reader => await Task.FromResult(reader.OK ? reader.Value?[0] : null), key);
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public List<MemcachedValue> Gets(params string[] keys)
        {
            if (keys.Length == 0) return null;
            return this.Execute(CommandType.GETS, reader => reader.OK ? reader.Value : null, keys);
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public async Task<List<MemcachedValue>> GetsAsync(params string[] keys)
        {
            if (keys.Length == 0) return null;
            return await this.ExecuteAsync(CommandType.GETS, async reader => await Task.FromResult(reader.OK ? reader.Value : null), keys);
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public MemcachedValue Gat(uint exptime, string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GAT, reader => reader.OK ? reader.Value?[0] : null, exptime, key);
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public async Task<MemcachedValue> GatAsync(uint exptime, string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GAT, async reader => await Task.FromResult(reader.OK ? reader.Value?[0] : null), exptime, key);
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public List<MemcachedValue> Gat(uint exptime, params string[] keys)
        {
            if (keys.Length == 0) return null;
            return this.Execute(CommandType.GAT, reader => reader.OK ? reader.Value : null, new object[] { exptime }.Concat(keys).ToArray());
        }
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public async Task<List<MemcachedValue>> GatAsync(uint exptime, params string[] keys)
        {
            if (keys.Length == 0) return null;
            return await this.ExecuteAsync(CommandType.GAT, async reader => await Task.FromResult(reader.OK ? reader.Value : null), new object[] { exptime }.Concat(keys).ToArray());
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public MemcachedValue Gats(uint exptime, string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GATS, reader => reader.OK ? reader.Value?[0] : null, exptime, key);
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public async Task<MemcachedValue> GatsAsync(uint exptime, string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GATS, async reader => await Task.FromResult(reader.OK ? reader.Value?[0] : null), exptime, key);
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public List<MemcachedValue> Gats(uint exptime, params string[] keys)
        {
            if (keys.Length == 0) return null;
            return this.Execute(CommandType.GATS, reader => reader.OK ? reader.Value : null, new object[] { exptime }.Concat(keys).ToArray());
        }
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        public async Task<List<MemcachedValue>> GatsAsync(uint exptime, params string[] keys)
        {
            if (keys.Length == 0) return null;
            return await this.ExecuteAsync(CommandType.GATS, async reader => await Task.FromResult(reader.OK ? reader.Value : null), new object[] { exptime }.Concat(keys).ToArray());
        }
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>状态</returns>
        public Boolean Delete(string key)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.DELETE, reader => reader.OK, key);
        }
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>状态</returns>
        public async Task<Boolean> DeleteAsync(string key)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.DELETE, async reader => await Task.FromResult(reader.OK), key);
        }
        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递增值</param>
        /// <returns>现在值</returns>
        public ulong Increment(string key, uint incrementValue)
        {
            if (key.IsNullOrEmpty()) return 0;
            return this.Execute(CommandType.INCREMENT, reader => reader.OK ? (ulong)reader.Value?[0].Value : 0, key, incrementValue);
        }
        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递增值</param>
        /// <returns>现在值</returns>
        public async Task<ulong> IncrementAsync(string key, uint incrementValue)
        {
            if (key.IsNullOrEmpty()) return 0;
            return await this.ExecuteAsync(CommandType.INCREMENT, async reader => await Task.FromResult(reader.OK ? (ulong)reader.Value?[0].Value : 0), key, incrementValue);
        }
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递减值</param>
        /// <returns>现在值</returns>
        public ulong Decrement(string key, uint incrementValue)
        {
            if (key.IsNullOrEmpty()) return 0;
            return this.Execute(CommandType.DECREMENT, reader => reader.OK ? (ulong)reader.Value?[0].Value : 0, key, incrementValue);
        }
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递减值</param>
        /// <returns>现在值</returns>
        public async Task<ulong> DecrementAsync(string key, uint incrementValue)
        {
            if (key.IsNullOrEmpty()) return 0;
            return await this.ExecuteAsync(CommandType.DECREMENT, async reader => await Task.FromResult(reader.OK ? (ulong)reader.Value?[0].Value : 0), key, incrementValue);
        }
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="exptime">过期时间 单位为秒</param>
        /// <returns>状态</returns>
        public Boolean Touch(string key, uint exptime)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.TOUCH, reader => reader.OK, key, exptime);
        }
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="exptime">过期时间 单位为秒</param>
        /// <returns>状态</returns>
        public async Task<Boolean> TouchAsync(string key, uint exptime)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.TOUCH, async reader => await Task.FromResult(reader.OK), key, exptime);
        }
        #endregion

        #region Stats
        /// <summary>
        /// 统计信息
        /// </summary>
        /// <returns>统计数据</returns>
        public Dictionary<string, string> Stats() => this.Execute(CommandType.STATS, reader => reader.OK ? reader.DictonaryValue : null);
        /// <summary>
        /// 统计信息
        /// </summary>
        /// <returns>统计数据</returns>
        public async Task<Dictionary<string, string>> StatsAsync() => await this.ExecuteAsync(CommandType.STATS, async reader => await Task.FromResult(reader.OK ? reader.DictonaryValue : null));
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <returns>统计数据</returns>
        public Dictionary<string, string> StatsItems() => this.Execute(CommandType.STATSITEMS, reader => reader.OK ? reader.DictonaryValue : null);
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <returns>统计数据</returns>
        public async Task<Dictionary<string, string>> StatsItemsAsync() => await this.ExecuteAsync(CommandType.STATSITEMS, async reader => await Task.FromResult(reader.OK ? reader.DictonaryValue : null));
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <returns>统计数据</returns>
        public Dictionary<string, string> StatsSlabs() => this.Execute(CommandType.STATSSLABS, reader => reader.OK ? reader.DictonaryValue : null);
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <returns>统计数据</returns>
        public async Task<Dictionary<string, string>> StatsSlabsAsync() => await this.ExecuteAsync(CommandType.STATSSLABS, async reader => await Task.FromResult(reader.OK ? reader.DictonaryValue : null));
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <returns>统计数据</returns>
        public Dictionary<string, string> StatsSizes() => this.Execute(CommandType.STATSSIZES, reader => reader.OK ? reader.DictonaryValue : null);
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <returns>统计数据</returns>
        public async Task<Dictionary<string, string>> StatsSizesAsync() => await this.ExecuteAsync(CommandType.STATSSIZES, async reader => await Task.FromResult(reader.OK ? reader.DictonaryValue : null));
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <param name="time">延迟多长时间执行清理 单位为秒</param>
        /// <returns>状态</returns>
        public Boolean FulshAll(ulong time = 0) => this.Execute(CommandType.FLUSHALL, reader => reader.OK, time);
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <param name="time">延迟多长时间执行清理 单位为秒</param>
        /// <returns>状态</returns>
        public async Task<Boolean> FulshAllAsync(ulong time = 0) => await this.ExecuteAsync(CommandType.FLUSHALL, async reader => await Task.FromResult(reader.OK), time);
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            this.Close();
            base.Dispose();
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~MemcachedClient()
        {
            this.Dispose();
        }
        #endregion

        #endregion
    }
}