using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.IO;
using XiaoFeng.Memcached.Protocol.Text;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 16:30:17                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// 操作工厂
    /// </summary>
    public interface IOperationFactory
    {
        #region 构造器

        #endregion

        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        MemcachedConfig MemcachedConfig { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Task<GetOperationResult> GetAsync(params string[] keys);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Task<GetOperationResult> GetsAsync(params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="expiry">缓存时间 单位为秒</param>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Task<GetOperationResult> GatAsync(uint expiry, params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="expiry">缓存时间 单位为秒</param>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Task<GetOperationResult> GatsAsync(uint expiry, params string[] keys);
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="keys">键</param>
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
        Task<GetOperationResult> IncrementAsync(string key, ulong step, ulong defaultValue, uint expiry);
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="step">步长</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task<GetOperationResult> DecrementAsync(string key, ulong step, ulong defaultValue,uint expiry);
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="expiry">过期时间 单位为秒</param>
        /// <param name="keys">key</param>
        /// <returns></returns>
        Task<GetOperationResult> TouchAsync(uint expiry, params string[] keys);
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> SetAsync(string key, object value, uint expiry);
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> AddAsync(string key, object value, uint expiry);
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> ReplaceAsync(string key, object value, uint expiry);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> AppendAsync(string key, object value, uint expiry);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> PrependAsync(string key, object value, uint expiry);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <param name="expiry">过期时间 单位为秒 默认不限制</param>
        /// <returns></returns>
        Task<StoreOperationResult> CasAsync(string key, object value,ulong casToken, uint expiry);
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
        Task<StatsOperationResult> FulshAllAsync(uint timeout);
        /// <summary>
        /// 服务器版本
        /// </summary>
        /// <returns></returns>
        Task<StatsOperationResult> VersionAsync();
        #endregion
    }
}