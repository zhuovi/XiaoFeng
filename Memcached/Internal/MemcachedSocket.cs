using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Collections;
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
    public class MemcachedSocket
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
        private ConcurrentDictionary<IPEndPoint, MemcachedSocketPool> Pools { get; set; }
        /// <summary>
        /// 网络端
        /// </summary>
        public ConcurrentDictionary<IPEndPoint, ISocketClient> Clients { get; set; }
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
            if (this.MemcachedConfig.Pool <= 1)
            {
                this.Clients = new ConcurrentDictionary<IPEndPoint, ISocketClient>();
                this.Manuals = new List<AutoResetEvent>();
                this.MemcachedConfig.Servers.Each(s =>
                {
                    var client = new SocketClient(s)
                    {
                        ReceiveTimeout = this.MemcachedConfig.ReadTimeout,
                        SendTimeout = this.MemcachedConfig.WriteTimeout,
                        Encoding = this.MemcachedConfig.Encoding
                    };
                    if (this.MemcachedConfig.Certificates != null && this.MemcachedConfig.Certificates.Count > 0)
                    {
                        client.ClientCertificates = this.MemcachedConfig.Certificates;
                        client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    }
                    client.OnClientError += (c,e,ex) =>
                    {
                        Console.WriteLine($"{c.EndPoint}出错[{ex.Message}].{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}");
                    };
                    client.OnStart += (c,e) =>
                    {
                        Console.WriteLine($"{c.EndPoint}已启动.{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}");

                    };
                    client.OnStop += (c, e) =>
                    {
                        Console.WriteLine($"{c.EndPoint}已停止.{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}");

                    };
                    var status = client.Connect();
                    if (status == SocketError.Success)
                    {
                        this.Clients.TryAdd(s, client);
                        IsInit = true;
                    }
                    this.Manuals.Add(new AutoResetEvent(true));
                });
            }
            else
            {
                this.Pools = new ConcurrentDictionary<IPEndPoint, MemcachedSocketPool>();
                this.MemcachedConfig.Servers.Each(s =>
                {
                    var pool = new MemcachedSocketPool(this.MemcachedConfig, s);
                    this.Pools.TryAdd(s, pool);
                });
                IsInit = true;
            }
        }
        #endregion

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">索引</param>
        /// <param name="func">回调</param>
        /// <returns></returns>
        public T Execute<T>(int index, Func<ISocketClient,T> func) where T : class
        {
            if (this.MemcachedConfig.Pool >= 1)
            {
                if (this.Pools.TryGetValue(this.MemcachedConfig.Servers[index], out var pool))
                {
                    var item = pool.Get();
                    var v = func?.Invoke(item.Value);
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
                    var v = func?.Invoke(client);
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
            if (this.MemcachedConfig.Pool >= 1)
            {
                for (var i = 0; i < this.MemcachedConfig.Servers.Count; i++)
                {
                    var endpoint = this.MemcachedConfig.Servers[i];
                    if (this.Pools.TryGetValue(endpoint, out var pool))
                    {
                        var item = pool.Get();
                        var v = func.Invoke(item.Value);
                        pool.Put(item);
                        dic.Add(endpoint, v);
                    }
                }
            }
            else
            {
                for (var i = 0; i < this.MemcachedConfig.Servers.Count; i++)
                {
                    var endpoint = this.MemcachedConfig.Servers[i];
                    var ma = this.Manuals[i];
                    if (this.Clients.TryGetValue(endpoint, out var client))
                    {
                        ma.WaitOne(TimeSpan.FromSeconds(this.Timeout));
                        if (!client.Connected) client.Connect();
                        var v = func.Invoke(client);
                        ma.Set();
                        dic.Add(endpoint, v);
                    }
                }
            }
            return dic;
        }
        #endregion
    }
}