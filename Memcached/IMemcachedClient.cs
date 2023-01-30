using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Memcachedcached接口
    /// </summary>
    public interface IMemcachedClient
    {
        #region 属性
        /// <summary>
        /// 连接配置
        /// </summary>
        MemcachedConfig ConnConfig { get; set; }
        /// <summary>
        /// 寻址方案
        /// </summary>
        AddressFamily AddressFamily { get; set; }
        /// <summary>
        /// 套接字类型
        /// </summary>
        SocketType SocketType { get; set; }
        /// <summary>
        /// 支持协议
        /// </summary>
        ProtocolType ProtocolType { get; set; }
        /// <summary>
        /// 发送超时
        /// </summary>
        int SendTimeout { get; set; }
        /// <summary>
        /// 接收超时
        /// </summary>
        int ReceiveTimeout { get; set; }
        /// <summary>
        /// Hash算法
        /// </summary>
        IMemcachedTransform Transform { get; set; }
        /// <summary>
        /// 协议
        /// </summary>
        MemcachedProtocol MemcachedProtocol { get; set; }
        /// <summary>
        /// 压缩值 1M
        /// </summary>
        uint CompressLength { get; set; }
        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
        #endregion

        #region 认证
        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        Boolean Auth();
        /// <summary>
        /// 认证 异步
        /// </summary>
        /// <returns></returns>
        Task<Boolean> AuthAsync();
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
        Boolean Set<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 给key设置一个值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Task<Boolean> SetAsync<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Boolean Add<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 如果key不存在的话，就添加 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Task<Boolean> AddAsync<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Boolean Replace<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 用来替换已知key的value 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Task<Boolean> ReplaceAsync<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Boolean Append<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Task<Boolean> AppendAsync<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Boolean Prepend<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <returns>成功状态</returns>
        Task<Boolean> PrependAsync<T>(string key, T value, uint exptime = 0);
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <returns>成功状态</returns>
        Boolean Cas<T>(string key, T value, ulong casToken, uint exptime = 0);
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="exptime">过期时间 单位为秒 默认不限制</param>
        /// <param name="casToken">通过 gets 命令获取的一个唯一的64位值</param>
        /// <returns>成功状态</returns>
        Task<Boolean> CasAsync<T>(string key, T value, ulong casToken, uint exptime = 0);
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
        Boolean Store<T>(CommandType commandType, string key, T value, uint exptime = 0, ulong casToken = 0);
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
        Task<Boolean> StoreAsync<T>(CommandType commandType, string key, T value, uint exptime = 0, ulong casToken = 0);
        #endregion

        #region Get
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        MemcachedValue Get(string key);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        Task<MemcachedValue> GetAsync(string key);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        List<MemcachedValue> Get(params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        Task<List<MemcachedValue>> GetAsync(params string[] keys);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        MemcachedValue Gets(string key);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        Task<MemcachedValue> GetsAsync(string key);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        List<MemcachedValue> Gets(params string[] keys);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        Task<List<MemcachedValue>> GetsAsync(params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        MemcachedValue Gat(uint exptime, string key);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        Task<MemcachedValue> GatAsync(uint exptime, string key);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        List<MemcachedValue> Gat(uint exptime, params string[] keys);
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        Task<List<MemcachedValue>> GatAsync(uint exptime, params string[] keys);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        MemcachedValue Gats(uint exptime, string key);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        Task<MemcachedValue> GatsAsync(uint exptime, string key);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        List<MemcachedValue> Gats(uint exptime, params string[] keys);
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <param name="exptime">过期时间</param>
        /// <param name="keys">key</param>
        /// <returns>值</returns>
        Task<List<MemcachedValue>> GatsAsync(uint exptime, params string[] keys);
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>状态</returns>
        Boolean Delete(string key);
        /// <summary>
        /// 删除已存在的 key(键)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>状态</returns>
        Task<Boolean> DeleteAsync(string key);
        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递增值</param>
        /// <returns>现在值</returns>
        ulong Increment(string key, uint incrementValue);
        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递增值</param>
        /// <returns>现在值</returns>
        Task<ulong> IncrementAsync(string key, uint incrementValue);
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递减值</param>
        /// <returns>现在值</returns>
        ulong Decrement(string key, uint incrementValue);
        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="incrementValue">递减值</param>
        /// <returns>现在值</returns>
        Task<ulong> DecrementAsync(string key, uint incrementValue);
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="exptime">过期时间 单位为秒</param>
        /// <returns>状态</returns>
        Boolean Touch(string key, uint exptime);
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="exptime">过期时间 单位为秒</param>
        /// <returns>状态</returns>
        Task<Boolean> TouchAsync(string key, uint exptime);
        #endregion

        #region Stats
        /// <summary>
        /// 统计信息
        /// </summary>
        /// <returns>统计数据</returns>
        Dictionary<string, string> Stats();
        /// <summary>
        /// 统计信息
        /// </summary>
        /// <returns>统计数据</returns>
        Task<Dictionary<string, string>> StatsAsync();
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <returns>统计数据</returns>
        Dictionary<string, string> StatsItems();
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <returns>统计数据</returns>
        Task<Dictionary<string, string>> StatsItemsAsync();
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <returns>统计数据</returns>
        Dictionary<string, string> StatsSlabs();
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <returns>统计数据</returns>
        Task<Dictionary<string, string>> StatsSlabsAsync();
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <returns>统计数据</returns>
        Dictionary<string, string> StatsSizes();
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <returns>统计数据</returns>
        Task<Dictionary<string, string>> StatsSizesAsync();
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <param name="time">延迟多长时间执行清理 单位为秒</param>
        /// <returns>状态</returns>
        Boolean FulshAll(ulong time = 0);
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <param name="time">延迟多长时间执行清理 单位为秒</param>
        /// <returns>状态</returns>
        Task<Boolean> FulshAllAsync(ulong time = 0);
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        #endregion
    }
}