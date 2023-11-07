using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-19 14:30:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// Memcached客户端
    /// </summary>
    public class MemcachedSocket : Disposable
    {
        #region 构造器
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="memcachedConfig">配置</param>
        /// <param name="timeout">获取超时时长 单位为秒</param>
        public MemcachedSocket(MemcachedConfig memcachedConfig, int timeout)
        {
            MemcachedConfig = memcachedConfig;
            this.Timeout = timeout;
            this.Init();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        private MemcachedConfig MemcachedConfig { get; set; }
        /// <summary>
        /// 网络池
        /// </summary>
        private ConcurrentDictionary<IPEndPoint, MemcachedSocketClientPool> Pools { get; set; }
        /// <summary>
        /// 网络端
        /// </summary>
        public ConcurrentDictionary<IPEndPoint, IMemcachedSocketClient> Clients { get; set; }
        /// <summary>
        /// 是否初始化
        /// </summary>
        private Boolean IsInit { get; set; }
        /// <summary>
        /// 同步信号
        /// </summary>
        private List<AutoResetEvent> Manuals { get; set; }
        /// <summary>
        /// 多长时间取不到数据新加 单位为秒
        /// </summary>
        private int Timeout { get; set; } = 10;
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (this.MemcachedConfig == null) return;
            IsInit = true;
            if (this.MemcachedConfig.PoolSize >= 1)
            {
                this.Pools = new ConcurrentDictionary<IPEndPoint, MemcachedSocketClientPool>();
                this.MemcachedConfig.Servers.Each(s =>
                {
                    var pool = new MemcachedSocketClientPool(this.MemcachedConfig, s);
                    this.Pools.TryAdd(s, pool);
                });
                return;
            }
            this.Clients = new ConcurrentDictionary<IPEndPoint, IMemcachedSocketClient>();
            this.Manuals = new List<AutoResetEvent>();
            this.MemcachedConfig.Servers.Each(s =>
            {
                var client = new MemcachedSocketClient(this.MemcachedConfig, s);

                this.Clients.TryAdd(s, client);

                this.Manuals.Add(new AutoResetEvent(true));
            });
        }
        #endregion

        #region 执行
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">索引</param>
        /// <param name="func">回调</param>
        /// <returns></returns>
        public T Execute<T>(int index, Func<ISocketClient, T> func) where T : class
        {
            if (!this.IsInit) return default(T);
            if (this.MemcachedConfig.PoolSize >= 1)
            {
                if (this.Pools.TryGetValue(this.MemcachedConfig.Servers[index], out var pool))
                {
                    var item = pool.Get();
                    var client = item.Value;
                    if (!client.Connected && client.Connect() != SocketError.Success)
                    {
                        return default(T);
                    }
                    var v = func?.Invoke(client.Socket);
                    pool.Put(item);
                    return v;
                }
                return default(T);
            }
            else
            {
                if (this.Clients.TryGetValue(this.MemcachedConfig.Servers[index], out var client))
                {
                    var ma = this.Manuals[index];
                    ma.WaitOne(TimeSpan.FromSeconds(this.Timeout));
                    if (!client.Connected) client.Connect();
                    var v = func?.Invoke(client.Socket);
                    ma.Set();
                    return v;
                }
                return default(T);
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="func">回调</param>
        /// <returns></returns>
        public Dictionary<IPEndPoint, T> Execute<T>(Func<ISocketClient, T> func)
        {
            var dic = new Dictionary<IPEndPoint, T>();
            if (!this.IsInit) return dic;
            for (var i = 0; i < this.MemcachedConfig.Servers.Count; i++)
            {
                var endpoint = this.MemcachedConfig.Servers[i];
                if (this.MemcachedConfig.PoolSize >= 1)
                {
                    if (this.Pools.TryGetValue(endpoint, out var pool))
                    {
                        var item = pool.Get();
                        var client = item.Value;
                        if (!client.Connected && client.Connect() != SocketError.Success)
                        {
                            continue;
                        }
                        var v = func.Invoke(client.Socket);
                        pool.Put(item);
                        dic.Add(endpoint, v);
                    }
                }
                else
                {
                    var ma = this.Manuals[i];
                    if (this.Clients.TryGetValue(endpoint, out var client))
                    {
                        ma.WaitOne(TimeSpan.FromSeconds(this.Timeout));
                        if (!client.Connected && client.Connect() != SocketError.Success)
                        {
                            ma.Set();
                            continue;
                        }
                        var v = func.Invoke(client.Socket);
                        ma.Set();
                        dic.Add(endpoint, v);
                    }
                }
            }
            return dic;
        }
        #endregion

        #region 关闭
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">标识</param>
        protected override void Dispose(bool disposing)
        {
            if (this.IsInit)
            {
                if (this.Clients?.Count > 0)
                {
                    this.Clients.Values.Each(v =>
                    {
                        v.Dispose();
                    });
                    this.Clients.Clear();
                    this.Clients.TryDispose();
                    this.Clients = null;
                }
                if (this.Pools?.Count > 0)
                {
                    this.Pools.Values.Each(v =>
                    {
                        v.Dispose();
                    });
                    this.Pools.Clear();
                    this.Pools.TryDispose();
                    this.Pools = null;
                }
            }
            base.Dispose(disposing);
            GC.Collect();
        }
        #endregion

        #endregion
    }
}