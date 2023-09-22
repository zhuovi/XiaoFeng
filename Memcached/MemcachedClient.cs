using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Internal;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 16:32:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcatched客户端
    /// </summary>
    public class MemcachedClient : Disposable, IMemcachedClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedClient()
        {

        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="connString">连接串</param>
        /// <remarks>
        /// <para><see langword="格式"/> : [&lt;protocol&gt;]://[[&lt;username&gt;:&lt;password&gt;@]&lt;host&gt;:&lt;port&gt;][?&lt;ConnectionTimeout&gt;=10[&amp;&lt;ReadTimeout&gt;=10][&amp;&lt;WiteTimeout&gt;=10][&amp;&lt;PoolSize&gt;=10]]</para>
        /// <para><term>protocol</term> 协议，固定值为memcached</para>
        /// <para><term>username</term> 帐号</para>
        /// <para><term>password</term> 密码</para>
        /// <para><term>host</term> 服务器地址或DNS</para>
        /// <para><term>port</term> 服务器端口 默认为11211</para>
        /// <para><term>ConnectionTimeout</term> 连接超时时间 单位为秒</para>
        /// <para><term>ReadTimeout</term> 读取超时时间 单位为秒</para>
        /// <para><term>WiteTimeout</term> 写入超时时间 单位为秒</para>
        /// <para><term>PoolSize</term> 连接池数量</para>
        /// <para>例子 : memcached://memcached:7092734@127.0.0.01:11211?ConnectionTimeout=10&amp;ReadTimeout=10&amp;WiteTimeout=10&amp;PoolSize=10
        /// </para>
        /// </remarks>
        public MemcachedClient(string connString)
        {
            this.Config = new MemcachedConfig(connString);
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">配置</param>
        public MemcachedClient(MemcachedConfig config)
        {
            this.Config = config;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        public MemcachedConfig Config { get; set; }
        /// <summary>
        /// 操作工厂
        /// </summary>
        private IOperationFactory _Factory;
        /// <summary>
        /// 操作工厂
        /// </summary>
        private IOperationFactory Factory
        {
            get
            {
                if (this._Factory == null)
                    this._Factory = GetOperationFactory();
                return this._Factory;
            }
            set => this._Factory = value;
        }
        #endregion

        #region 方法

        #region 获取操作工厂
        /// <summary>
        /// 获取操作工厂
        /// </summary>
        /// <returns>操作工厂</returns>
        private IOperationFactory GetOperationFactory()
        {
            if (this.Config.Protocol == MemcachedProtocol.Binary)
            {
                return new Protocol.Binary.BinaryOperationFactory(this.Config);
            }
            else if (this.Config.Protocol == MemcachedProtocol.Text)
            {
                return new Protocol.Text.TextOperationFactory(this.Config);
            }
            return null;
        }
        #endregion

        #region GET
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        public async Task<GetOperationResult> GetAsync(params string[] keys) => await Factory.GetAsync(keys).ConfigureAwait(false);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        public async Task<GetOperationResult> GetsAsync(params string[] keys) => await Factory.GetsAsync(keys).ConfigureAwait(false);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        /// <remarks>注:<see langword="Text"/> 协议提示不识别 Gat命令;</remarks>
        public async Task<GetOperationResult> GatAsync(uint exptime, params string[] keys) => await Factory.GatAsync(exptime, keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        /// <remarks>注:<see langword="Text"/> 协议提示不识别 Gat命令;</remarks>
        public async Task<GetOperationResult> GatsAsync(uint exptime, params string[] keys) => await Factory.GatsAsync(exptime, keys);
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns></returns>
        public async Task<GetOperationResult> DeleteAsync(params string[] keys) => await Factory.DeleteAsync(keys);
        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="step">步长</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<GetOperationResult> IncrementAsync(string key, ulong step, ulong defaultValue = 1, uint expiry = 0) => await Factory.IncrementAsync(key, step, defaultValue, expiry);
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="step">步长</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<GetOperationResult> DecrementAsync(string key, ulong step, ulong defaultValue = 1, uint expiry = 0) => await Factory.DecrementAsync(key, step, defaultValue, expiry);
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <param name="keys">key</param>
        /// <returns></returns>
        public async Task<GetOperationResult> TouchAsync(uint expiry, params string[] keys) => await Factory.TouchAsync(expiry, keys).ConfigureAwait(false);
        #endregion

        #region STORE
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        public async Task<StoreOperationResult> SetAsync(string key, object value, uint expiry = 0) => await Factory.SetAsync(key, value, expiry).ConfigureAwait(false);
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        public async Task<StoreOperationResult> AddAsync(string key, object value, uint expiry = 0) => await Factory.AddAsync(key, value, expiry).ConfigureAwait(false);
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        public async Task<StoreOperationResult> ReplaceAsync(string key, object value, uint expiry = 0) => await Factory.ReplaceAsync(key, value, expiry).ConfigureAwait(false);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        public async Task<StoreOperationResult> AppendAsync(string key, object value, uint expiry = 0) => await Factory.AppendAsync(key, value, expiry).ConfigureAwait(false);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        public async Task<StoreOperationResult> PrependAsync(string key, object value, uint expiry = 0) => await Factory.PrependAsync(key, value, expiry).ConfigureAwait(false);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中 仅Text支持
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        public async Task<StoreOperationResult> CasAsync(string key, object value, ulong casToken, uint expiry = 0) => await Factory.CasAsync(key, value, casToken, expiry).ConfigureAwait(false);
        #endregion

        #region STAT
        /// <summary>
        /// 统计信息
        /// </summary>
        /// <returns></returns>
        public async Task<StatsOperationResult> StatsAsync() => await Factory.StatsAsync().ConfigureAwait(false);
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <returns></returns>
        public async Task<StatsOperationResult> StatsItemsAsync() => await Factory.StatsItemsAsync().ConfigureAwait(false);
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <returns></returns>
        public async Task<StatsOperationResult> StatsSlabsAsync() => await Factory.StatsSlabsAsync().ConfigureAwait(false);
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <returns></returns>
        public async Task<StatsOperationResult> StatsSizesAsync() => await Factory.StatsSizesAsync().ConfigureAwait(false);
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <param name="timeout">延迟多长时间执行清理 单位为秒</param>
        /// <returns></returns>
        public async Task<StatsOperationResult> FulshAllAsync(uint timeout = 0) => await Factory.FulshAllAsync(timeout).ConfigureAwait(false);
        /// <summary>
        /// 服务器版本
        /// </summary>
        /// <returns></returns>
        public async Task<StatsOperationResult> VersionAsync() => await Factory.VersionAsync().ConfigureAwait(false);
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.Factory?.TryDispose();
            base.Dispose(disposing);
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~MemcachedClient()
        {
            this.Factory?.Dispose();
            this.Dispose(true);
        }
        #endregion

        #endregion
    }
}