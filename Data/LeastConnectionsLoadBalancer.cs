using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-07-16 10:43:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 最少连接均衡器
    /// </summary>
    public class LeastConnectionsLoadBalancer
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public LeastConnectionsLoadBalancer()
        {
            this.LoadBalancer = new ConcurrentDictionary<string, LeastConnections>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 均衡器
        /// </summary>
        private ConcurrentDictionary<string, LeastConnections> LoadBalancer { get; set; }
        /// <summary>
        /// 连接均衡器
        /// </summary>
        public static readonly LeastConnectionsLoadBalancer Current = new LeastConnectionsLoadBalancer();
        /// <summary>
        /// 互斥锁
        /// </summary>
        private readonly Mutex mutex = new Mutex();
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">配置</param>
        public void Initialize(ConnectionConfig config)
        {
            if (config.Id.IsNullOrEmpty()) return;
            mutex.WaitOne();
            {
                if (this.LoadBalancer.TryGetValue(config.Id, out var vals))
                {
                    vals.Initialize(config);
                }
                else
                {
                    var conns = new LeastConnections();
                    conns.Initialize(config);
                    this.LoadBalancer.TryAdd(config.Id, conns);
                }
            }
            mutex.ReleaseMutex();
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="connection">连接配置</param>
        public void Add(string id, ReadOnlyDbConfig connection)
        {
            mutex.WaitOne();
            {
                if (this.LoadBalancer.TryGetValue(id, out var value))
                {
                    value.Add(connection.ToString());
                }
                else
                {
                    var connections = new LeastConnections();
                    connections.Add(connection.ToString());
                    LoadBalancer.TryAdd(id, connections);
                }
            }
            mutex.ReleaseMutex();
        }
        #endregion

        #region 读取连接最少的连接对象
        /// <summary>
        /// 读取连接最少的连接对象
        /// </summary>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public ReadOnlyDbConfig GetLeastConnection(ConnectionConfig config)
        {
            ReadOnlyDbConfig readOnlyDbConfig;
            mutex.WaitOne();
            {
                if (this.LoadBalancer.TryGetValue(config.Id, out var vals))
                {
                    readOnlyDbConfig = vals.GetLeastConnection();
                    vals.Add(readOnlyDbConfig.ToString());
                }
                else
                {
                    readOnlyDbConfig = config.ReadDbs.First();
                    this.Initialize(config);
                }
            }
            mutex.ReleaseMutex();
            return readOnlyDbConfig;
        }
        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~LeastConnectionsLoadBalancer()
        {

        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 连接对象
    /// </summary>
    public class LeastConnections
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public LeastConnections()
        {
            this.Connections = new ConcurrentDictionary<string, long>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 连接数数据
        /// </summary>
        private ConcurrentDictionary<string,long> Connections { get; set; }

        #endregion

        #region 方法

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="count">数量</param>
        public void Add(string connectionString, long count = 1)
        {
            this.Connections.AddOrUpdate(connectionString, count, (a, v) =>
            {
                return v + 1;
            });
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">配置</param>
        public void Initialize(ConnectionConfig config)
        {
            if (config.ReadDbs != null && config.ReadDbs.Count > 0)
            {
                config.ReadDbs.Each(d =>
                {
                    var key = d.ToString();
                    if (!this.Connections.ContainsKey(key))
                        this.Add(key, 0);
                });
            }
        }
        #endregion

        #region 获取最少连接对象
        /// <summary>
        /// 获取最少连接对象
        /// </summary>
        /// <returns></returns>
        public ReadOnlyDbConfig GetLeastConnection()
        {
            if (this.Connections == null || this.Connections.Count == 0) return null;
            var values = this.Connections.OrderBy(a => a.Value).First();
            return new ReadOnlyDbConfig(values.Key);
        }
        #endregion

        #endregion
    }
}