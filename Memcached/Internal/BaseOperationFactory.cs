using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 17:39:33                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// 操作工厂基本类
    /// </summary>
    public class BaseOperationFactory : Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseOperationFactory() { }
        #endregion

        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        public MemcachedConfig MemcachedConfig { get; set; }
        /// <summary>
        /// 网络库
        /// </summary>
        private MemcachedSocket _Memcached;
        /// <summary>
        /// 网络库
        /// </summary>
        public MemcachedSocket Memcached
        {
            get
            {
                if (this._Memcached == null)
                    this._Memcached = new MemcachedSocket(this.MemcachedConfig, 10);
                return this._Memcached;
            }
            set { this._Memcached = value; }
        }

        #endregion

        #region 方法

        #region 运行
        /// <summary>
        /// 运行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="func">回调</param>
        /// <returns></returns>
        public T Execute<T>(string key, Func<ISocketClient, T> func) where T : class => this.Memcached.Execute((int)GetHash(key) % this.MemcachedConfig.Servers.Count, func);
        /// <summary>
        /// 运行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="func">回调</param>
        /// <returns></returns>
        public Dictionary<IPEndPoint, T> Execute<T>(Func<ISocketClient, T> func) where T : class => this.Memcached.Execute(func);
        #endregion

        #region Hash
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Hash值</returns>
        public uint GetHash(string key)
        {
            if (key.IsNullOrEmpty()) return 0;
            Helper.CheckKey(key);
            return BitConverter.ToUInt32(this.MemcachedConfig.Transform.ComputeHash(key.GetBytes(Encoding.UTF8)), 0);
        }
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="hashValue">值</param>
        /// <returns>Hash值</returns>
        private uint GetHash(uint hashValue)
        {
            return BitConverter.ToUInt32(this.MemcachedConfig.Transform.ComputeHash(BitConverter.GetBytes(hashValue)), 0);
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

        #region 获取网络
        /// <summary>
        /// 获取网络
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public async Task<ISocketClient> GetMemcached(string key = "")
        {
            IPEndPoint endPoint;
            if (key.IsNullOrEmpty())
                endPoint = this.MemcachedConfig.Servers.First();
            else
            {
                var hash = this.GetHash(key);
                var index = (int)hash % this.MemcachedConfig.Servers.Count;
                endPoint = this.MemcachedConfig.Servers[index];
            }
            var client = new SocketClient
            {
                Encoding = this.MemcachedConfig.Encoding
            };
            if (this.MemcachedConfig.Certificates != null) client.ClientCertificates = this.MemcachedConfig.Certificates;

            var status = await client.ConnectAsync(endPoint).ConfigureAwait(false);
            return await Task.FromResult(status == System.Net.Sockets.SocketError.Success && client.Connected ? client : null);
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">标识</param>
        protected override void Dispose(bool disposing)
        {
            this.Memcached.Dispose();
            this.Memcached = null;
            base.Dispose(disposing);
            GC.Collect();
        }
        #endregion

        #endregion
    }
}