using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Internal;
using XiaoFeng.Memcached.Transform;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-13 09:48:03                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcached客户端接口
    /// </summary>
    public interface IMemcachedClient
    {
        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        MemcachedConfig Config { get; set; }
        #endregion

        #region GET
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Task<GetOperationResult> GetAsync(params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Task<GetOperationResult> GetsAsync(params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        /// <remarks>注:<see langword="Text"/> 协议提示不识别 Gat命令;</remarks>
        Task<GetOperationResult> GatAsync(uint exptime, params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        /// <remarks>注:<see langword="Text"/> 协议提示不识别 Gat命令;</remarks>
        Task<GetOperationResult> GatsAsync(uint exptime, params string[] keys);
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns></returns>
        Task<GetOperationResult> DeleteAsync(params string[] keys);
        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="step">步长</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task<GetOperationResult> IncrementAsync(string key, ulong step, ulong defaultValue = 1, uint expiry = 0);
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="step">步长</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task<GetOperationResult> DecrementAsync(string key, ulong step, ulong defaultValue = 1, uint expiry = 0);
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <param name="keys">key</param>
        /// <returns></returns>
        Task<GetOperationResult> TouchAsync(uint expiry, params string[] keys);
        #endregion

        #region STORE
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        Task<StoreOperationResult> SetAsync(string key, object value, uint expiry = 0);
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        Task<StoreOperationResult> AddAsync(string key, object value, uint expiry = 0);
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        Task<StoreOperationResult> ReplaceAsync(string key, object value, uint expiry = 0);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        Task<StoreOperationResult> AppendAsync(string key, object value, uint expiry = 0);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <returns></returns>
        Task<StoreOperationResult> PrependAsync(string key, object value, uint expiry = 0);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中 仅Text支持
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> CasAsync(string key, object value, ulong casToken, uint expiry = 0);
        #endregion

        #region STAT
        /// <summary>
        /// 统计信息
        /// </summary>
        /// <returns></returns>
        Task<StatsOperationResult> StatsAsync();
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <returns></returns>
        Task<StatsOperationResult> StatsItemsAsync();
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <returns></returns>
        Task<StatsOperationResult> StatsSlabsAsync();
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <returns></returns>
        Task<StatsOperationResult> StatsSizesAsync();
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <param name="timeout">延迟多长时间执行清理 单位为秒</param>
        /// <returns></returns>
        Task<StatsOperationResult> FulshAllAsync(uint timeout = 0);
        /// <summary>
        /// 服务器版本
        /// </summary>
        /// <returns></returns>
        Task<StatsOperationResult> VersionAsync();
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        #endregion
    }
}