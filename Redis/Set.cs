using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-07 11:10:08                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 集合(Set)
    /// </summary>
    public partial class RedisClient:Disposable
    {
        #region 集合(Set)

        #region 设置
        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        public int SetSetMember(string key, int? dbNum, params object[] values)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.SADD, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { key }.Concat(this.GetValues(values)).ToArray());
        }
        /// <summary>
        /// 向集合添加一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        public async Task<int> SetSetMemberAsync(string key, int? dbNum, params object[] values)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.SADD, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { key }.Concat(this.GetValues(values)).ToArray());
        }
        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        public int SetSetMember(string key, params object[] values) => this.SetSetMember(key, null, values);
        /// <summary>
        /// 向集合添加一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        public async Task<int> SetSetMemberAsync(string key, params object[] values) => await this.SetSetMemberAsync(key, null, values);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        public Boolean MoveSetMember<T>(string key, string destKey, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || destKey.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.SMOVE, dbNum, result => result.OK && (int)result.Value > 0, key, destKey, this.GetValue(value));
        }
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        public async Task<Boolean> MoveSetMemberAsync<T>(string key, string destKey, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.SMOVE, dbNum, async result => await Task.FromResult(result.OK && (int)result.Value > 0), key, destKey, this.GetValue(value));
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        public Boolean MoveSetMember(string key, string destKey, string value, int? dbNum = null) => this.MoveSetMember<string>(key, destKey, value, dbNum);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        public async Task<Boolean> MoveSetMemberAsync(string key, string destKey, string value, int? dbNum = null) => await this.MoveSetMemberAsync<string>(key, destKey, value, dbNum);
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        public int DelSetMember(string key, int? dbNum, params object[] values) => this.Execute(CommandType.SREM, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { key }.Concat(this.GetValues(values)).ToArray());
        /// <summary>
        /// 移除集合中一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        public async Task<int> DelSetMemberAsync(string key, int? dbNum, params object[] values) => await this.ExecuteAsync(CommandType.SREM, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { key }.Concat(this.GetValues(values)).ToArray());
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        public int DelSetMember(string key, params object[] values) => this.DelSetMember(key, null, values);
        /// <summary>
        /// 移除集合中一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        public async Task<int> DelSetMemberAsync(string key, params object[] values) => await this.DelSetMemberAsync(key, null, values);
        #endregion

        #region 获取
        /// <summary>
        /// 获取集合的成员数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员数</returns>
        public int GetSetCount(string key, int? dbNum = null) => this.Execute(CommandType.SCARD, dbNum, result => result.OK ? (int)result.Value : -1, key);
        /// <summary>
        /// 获取集合的成员数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员数</returns>
        public async Task<int> GetSetCountAsync(string key, int? dbNum = null) => await this.ExecuteAsync(CommandType.SCARD, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), key);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public List<T> GetSetDiff<T>(string key, int? dbNum, params object[] otherKey) => this.Execute(CommandType.SDIFF, dbNum, result => result.OK ? result.Value.ToList<T>() : null, new object[] { key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public List<string> GetSetDiff(string key, int? dbNum, params object[] otherKey) => this.GetSetDiff<string>(key, dbNum, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public List<string> GetSetDiff(string key, params object[] otherKey) => this.GetSetDiff(key, null, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<List<T>> GetSetDiffAsync<T>(string key, int? dbNum, params object[] otherKey) => await this.ExecuteAsync(CommandType.SDIFF, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<T>() : null), new object[] { key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<List<string>> GetSetDiffAsync(string key, int? dbNum, params object[] otherKey) => await this.GetSetDiffAsync<string>(key, dbNum, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<List<string>> GetSetDiffAsync(string key, params object[] otherKey) => await this.GetSetDiffAsync(key, null, otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public int GetSetDiffStore(string key, string storeKey, int? dbNum, params object[] otherKey) => this.Execute(CommandType.SDIFFSTORE, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { storeKey, key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public int GetSetDiffStore(string key, string storeKey, params object[] otherKey) => this.GetSetDiffStore(key, storeKey, null, otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<int> GetSetDiffStoreAsync(string key, string storeKey, int? dbNum, params object[] otherKey) => await this.ExecuteAsync(CommandType.SDIFFSTORE, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { storeKey, key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<int> GetSetDiffStoreAsync(string key, string storeKey, params object[] otherKey) => await this.GetSetDiffStoreAsync(key, storeKey, null, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        public List<T> GetSetInter<T>(string key, int? dbNum, params object[] otherKey) => this.Execute(CommandType.SINTER, dbNum, result => result.OK ? result.Value.ToList<T>() : null, new object[] { key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        public List<string> GetSetInter(string key, int? dbNum, params object[] otherKey) => this.GetSetInter<string>(key, dbNum, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        public List<string> GetSetInter(string key, params object[] otherKey) => this.GetSetInter(key, null, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        public async Task<List<T>> GetSetInterAsync<T>(string key, int? dbNum, params object[] otherKey) => await this.ExecuteAsync(CommandType.SINTER, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<T>() : null), new object[] { key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        public async Task<List<string>> GetSetInterAsync(string key, int? dbNum, params object[] otherKey) => await this.GetSetInterAsync<string>(key, dbNum, otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        public async Task<List<string>> GetSetInterAsync(string key, params object[] otherKey) => await this.GetSetInterAsync(key, null, otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        public int GetSetInterStore(string key, string storeKey, int? dbNum, params object[] otherKey) => this.Execute(CommandType.SINTERSTORE, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { storeKey, key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        public int GetSetInterStore(string key, string storeKey, params object[] otherKey) => this.GetSetInterStore(key, storeKey, null, otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        public async Task<int> GetSetInterStoreAsync(string key, string storeKey, int? dbNum, params object[] otherKey) => await this.ExecuteAsync(CommandType.SINTERSTORE, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { storeKey, key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        public async Task<int> GetSetInterStoreAsync(string key, string storeKey, params object[] otherKey) => await this.GetSetInterStoreAsync(key, storeKey, null, otherKey);
        /// <summary>
        /// 所有给定集合的并集
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public List<T> GetSetUnion<T>(string key, int? dbNum, params object[] otherKey) => this.Execute(CommandType.SUNION, dbNum, result => result.OK ? result.Value.ToList<T>() : null, new object[] { key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 所有给定集合的并集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public List<string> GetSetUnion(string key, int? dbNum, params object[] otherKey) => this.GetSetUnion<string>(key, dbNum, otherKey);
        /// <summary>
        /// 所有给定集合的并集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public List<string> GetSetUnion(string key, params object[] otherKey) => this.GetSetUnion(key, null, otherKey);
        /// <summary>
        /// 所有给定集合的并集 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<List<T>> GetSetUnionAsync<T>(string key, int? dbNum, params object[] otherKey) => await this.ExecuteAsync(CommandType.SUNION, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<T>() : null), new object[] { key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 所有给定集合的并集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<List<string>> GetSetUnionAsync(string key, int? dbNum, params object[] otherKey) => await this.GetSetUnionAsync<string>(key, dbNum, otherKey);
        /// <summary>
        /// 所有给定集合的并集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<List<string>> GetSetUnionAsync(string key, params object[] otherKey) => await this.GetSetUnionAsync(key, null, otherKey);
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public int GetSetUnionStore(string key, string storeKey, int? dbNum, params object[] otherKey) => this.Execute(CommandType.SUNIONSTORE, dbNum, result => result.OK ? (int)result.Value : -1, new object[] { storeKey, key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public int GetSetUnionStore(string key, string storeKey, params object[] otherKey) => this.GetSetUnionStore(key, storeKey, null, otherKey);
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<int> GetSetUnionStoreAsync(string key, string storeKey, int? dbNum, params object[] otherKey) => await this.ExecuteAsync(CommandType.SUNIONSTORE, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), new object[] { storeKey, key }.Concat(otherKey).ToArray());
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        public async Task<int> GetSetUnionStoreAsync(string key, string storeKey, params object[] otherKey) => await this.GetSetUnionStoreAsync(key, storeKey, null, otherKey);
        /// <summary>
        /// 判断成员元素是否是集合的成员
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否存在</returns>
        public Boolean ExistsSetMember<T>(string key, T value, int? dbNum = null) => this.Execute(CommandType.SISMEMBER, dbNum, result => result.OK&& (int)result.Value>0, key, this.GetValue(value));
        /// <summary>
        /// 判断成员元素是否是集合的成员 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否存在</returns>
        public async Task<Boolean> ExistsSetMemberAsync<T>(string key, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.SISMEMBER, dbNum, async result => await Task.FromResult(result.OK && (int)result.Value > 0), key, this.GetValue(value));
        /// <summary>
        /// 获取集合中的所有成员
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        public List<T> GetSetMemberList<T>(string key, int? dbNum = null) => this.Execute(CommandType.SMEMBERS, dbNum, result => result.OK ? result.Value.ToList<T>() : null, key);
        /// <summary>
        /// 获取集合中的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        public List<string> GetSetMemberList(string key, int? dbNum = null) => this.GetSetMemberList<string>(key, dbNum);
        /// <summary>
        /// 获取集合中的所有成员 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        public async Task<List<T>> GetSetMemberListAsync<T>(string key, int? dbNum = null) => await this.ExecuteAsync(CommandType.SMEMBERS, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<T>() : null), key);
        /// <summary>
        /// 获取集合中的所有成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        public async Task<List<string>> GetSetMemberListAsync(string key, int? dbNum = null) => await this.GetSetMemberListAsync<string>(key, dbNum);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        public List<T> GetSetPop<T>(string key, int count = 1, int? dbNum = null) => this.Execute(CommandType.SPOP, dbNum, result => result.OK ? result.Value.ToList<T>() : null, key, count);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        public List<string> GetSetPop(string key, int count = 1, int? dbNum = null) => this.GetSetPop<string>(key, count, dbNum);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        public async Task<List<T>> GetSetPopAsync<T>(string key, int count = 1, int? dbNum = null) => await this.ExecuteAsync(CommandType.SPOP, dbNum, async result => await Task.FromResult(result.OK ?result.Value.ToList<T>() : null), key, count);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        public async Task<List<string>> GetSetPopAsync(string key, int count = 1, int? dbNum = null) => await this.GetSetPopAsync<string>(key, count, dbNum);
        /// <summary>
        /// 获取集合中一个或多个随机数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        public List<T> GetSetRandomMember<T>(string key, int count = 1, int? dbNum = null) => this.Execute(CommandType.SRANDMEMBER, dbNum, result => result.OK ? result.Value.ToList<T>() : null, key, count);
        /// <summary>
        /// 获取集合中一个或多个随机数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        public List<string> GetSetRandomMember(string key, int count = 1, int? dbNum = null) => this.GetSetRandomMember<string>(key, count, dbNum);
        /// <summary>
        /// 获取集合中一个或多个随机数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        public async Task<List<T>> GetSetRandomMemberAsync<T>(string key, int count = 1, int? dbNum = null) => await this.ExecuteAsync(CommandType.SRANDMEMBER, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<T>() : null), key, count);
        /// <summary>
        /// 获取集合中一个或多个随机数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        public async Task<List<string>> GetSetRandomMemberAsync(string key, int count = 1, int? dbNum = null) => await this.GetSetRandomMemberAsync<string>(key, count, dbNum);
        /// <summary>
        /// 查找Set中的元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        public List<T> SearchSetMember<T>(string key, string pattern, int start = 0, int count = 10, int? dbNum = null) => this.Execute(CommandType.SSCAN, dbNum, result => result.OK ? result.Value.ToList<T>() : null, key, start, "MATCH", pattern, "COUNT", count);
        /// <summary>
        /// 查找Set中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        public List<string> SearchSetMember(string key, string pattern, int start = 0, int count = 10, int? dbNum = null) => this.SearchSetMember<string>(key, pattern, start, count, dbNum);
        /// <summary>
        /// 查找Set中的元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        public async Task<List<T>> SearchSetMemberAsync<T>(string key, string pattern, int start = 0, int count = 10, int? dbNum = null) => await this.ExecuteAsync(CommandType.SSCAN, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<T>() : null), key, start, "MATCH", pattern, "COUNT", count);
        /// <summary>
        /// 查找Set中的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        public async Task<List<string>> SearchSetMemberAsync(string key, string pattern, int start = 0, int count = 10, int? dbNum = null) => await this.SearchSetMemberAsync<string>(key, pattern, start, count, dbNum);
        #endregion

        #endregion
    }
}