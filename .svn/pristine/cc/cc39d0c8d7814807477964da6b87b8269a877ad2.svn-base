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
*  Create Time : 2021-07-07 11:04:59                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 列表(List)
    /// </summary>
    public partial class RedisClient : Disposable
    {
        #region 列表(List)

        #region 设置列表
        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetListItem<T>(string key, int index, T value, int? dbNum = null) => this.Execute(CommandType.LSET, dbNum, result => result.OK, key, index, this.GetValue(value));
        /// <summary>
        /// 通过索引设置列表元素的值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemAsync<T>(string key, int index, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.LSET, dbNum, async result => await Task.FromResult(result.OK), key, index, this.GetValue(value));
        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetListItem(string key, int index, string value, int? dbNum = null) => this.SetListItem<string>(key, index, value, dbNum);
        /// <summary>
        /// 通过索引设置列表元素的值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemAsync(string key, int index, string value, int? dbNum = null) => await this.SetListItemAsync<string>(key, index, value, dbNum);
        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public Boolean SetListItemBefore(string key, int? dbNum, params object[] values)
        {
            if (values == null || !values.Any()) return false;
            var list = new List<string> { key };
            values.Each(v => list.Add(this.GetValue(v)));
            return this.Execute(CommandType.LPUSH, dbNum, result => result.OK, list.ToArray());
        }
        /// <summary>
        /// 将一个或多个值插入到列表头部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemBeforeAsync(string key, int? dbNum, params object[] values)
        {
            if (values == null || !values.Any()) return false;
            var list = new List<string> { key };
            values.Each(v => list.Add(this.GetValue(v)));
            return await this.ExecuteAsync(CommandType.LPUSH, dbNum, async result => await Task.FromResult(result.OK), list.ToArray());
        }
        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public Boolean SetListItemBefore(string key, params object[] values) => this.SetListItemBefore(key, null, values);
        /// <summary>
        /// 将一个或多个值插入到列表头部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemBeforeAsync(string key, params object[] values) => await this.SetListItemBeforeAsync(key, null, values);
        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public Boolean SetListItem(string key, int? dbNum, params object[] values)
        {
            if (values == null || !values.Any()) return false;
            var list = new List<string> { key };
            values.Each(v => list.Add(this.GetValue(v)));
            return this.Execute(CommandType.RPUSH, dbNum, result => result.OK, list.ToArray());
        }
        /// <summary>
        /// 在列表中添加一个或多个值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemAsync(string key, int? dbNum, params object[] values)
        {
            if (values == null || !values.Any()) return false;
            var list = new List<string> { key };
            values.Each(v => list.Add(this.GetValue(v)));
            return await this.ExecuteAsync(CommandType.RPUSH, dbNum, async result => await Task.FromResult(result.OK), list.ToArray());
        }
        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public Boolean SetListItem(string key, params object[] values) => this.SetListItem(key, null, values);
        /// <summary>
        /// 在列表中添加一个或多个值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemAsync(string key, params object[] values) => await this.SetListItemAsync(key, null, values);
        /// <summary>
        /// 在列表的元素前插入元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertListItemBefore<T>(string key, string item, T value, int? dbNum = null) => this.Execute(CommandType.LINSERT, dbNum, result => result.OK, key, "BEFORE", item, this.GetValue(value));
        /// <summary>
        /// 在列表的元素前插入元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public async Task<Boolean> InsertListItemBeforeAsync<T>(string key, string item, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.LINSERT, dbNum, async result => await Task.FromResult(result.OK), key, "BEFORE", item, this.GetValue(value));
        /// <summary>
        /// 在列表的元素前插入元素
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertListItemBefore(string key, string item, string value, int? dbNum = null) => this.InsertListItemBefore<string>(key, item, value, dbNum);
        /// <summary>
        /// 在列表的元素前插入元素 异步
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public async Task<Boolean> InsertListItemBeforeAsync(string key, string item, string value, int? dbNum = null) => await this.InsertListItemBeforeAsync<string>(key, item, value, dbNum);
        /// <summary>
        /// 在列表的元素后插入元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertListItemAfter<T>(string key, string item, T value, int? dbNum = null) => this.Execute(CommandType.LINSERT, dbNum, result => result.OK, key, "AFTER", item, this.GetValue(value));
        /// <summary>
        /// 在列表的元素后插入元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public async Task<Boolean> InsertListItemAfterAsync<T>(string key, string item, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.LINSERT, dbNum, async result => await Task.FromResult(result.OK), key, "AFTER", item, this.GetValue(value));
        /// <summary>
        /// 在列表的元素后插入元素
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public Boolean InsertListItemAfter(string key, string item, string value, int? dbNum = null) => this.InsertListItemAfter<string>(key, item, value, dbNum);
        /// <summary>
        /// 在列表的元素后插入元素 异步
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否插入成功</returns>
        public async Task<Boolean> InsertListItemAfterAsync(string key, string item, string value, int? dbNum = null) => await this.InsertListItemAfterAsync<string>(key, item, value, dbNum);
        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public int GetListCount(string key, int? dbNum = null) => this.Execute(CommandType.LLEN, dbNum, result => result.OK ? (int)result.Value : -1, key);
        /// <summary>
        /// 获取列表长度 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<int> GetListCountAsync(string key, int? dbNum = null) => await this.ExecuteAsync(CommandType.LLEN, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), key);
        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetListItemBeforeExists<T>(string key, T value, int? dbNum = null) => this.Execute(CommandType.LPUSHX, dbNum, result => result.OK, key, this.GetValue(value));
        /// <summary>
        /// 将一个值插入到已存在的列表头部 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemBeforeExistsAsync<T>(string key, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.LPUSHX, dbNum, async result => await Task.FromResult(result.OK), key, this.GetValue(value));
        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetListItemBeforeExists(string key, string value, int? dbNum = null) => this.SetListItemBeforeExists<string>(key, value, dbNum);
        /// <summary>
        /// 将一个值插入到已存在的列表头部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemBeforeExistsAsync(string key, string value, int? dbNum = null) => await this.SetListItemBeforeExistsAsync<string>(key, value, dbNum);
        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetListItemExists<T>(string key, T value, int? dbNum = null) => this.Execute(CommandType.RPUSHX, dbNum, result => result.OK, key, this.GetValue(value));
        /// <summary>
        /// 为已存在的列表添加值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemExistsAsync<T>(string key, T value, int? dbNum = null) => await this.ExecuteAsync(CommandType.RPUSHX, dbNum, async result => await Task.FromResult(result.OK), key, this.GetValue(value));
        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetListItemExists(string key, string value, int? dbNum = null) => this.SetListItemExists<string>(key, value, dbNum);
        /// <summary>
        /// 为已存在的列表添加值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetListItemExistsAsync(string key, string value, int? dbNum = null) => await this.SetListItemExistsAsync<string>(key, value, dbNum);
        #endregion

        #region 获取并移除
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        public Dictionary<string, string> GetListFirstItem(int? dbNum, int? timeout, params object[] keys) => this.Execute(CommandType.BLPOP, dbNum, result => (Dictionary<string, string>)result.Value, keys.Concat(new object[] { timeout ?? 0 }).ToArray());
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        public async Task<Dictionary<string, string>> GetListFirstItemAsync(int? dbNum, int? timeout, params object[] keys) => await this.ExecuteAsync(CommandType.BLPOP, dbNum, async result => await Task.FromResult((Dictionary<string, string>)result.Value), keys.Concat(new object[] { timeout ?? 0 }).ToArray());
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        public Dictionary<string, string> GetListFirstItem(int? timeout, params object[] keys) => this.GetListFirstItem(null, timeout, keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        public async Task<Dictionary<string, string>> GetListFirstItemAsync(int? timeout, params object[] keys) => await this.GetListFirstItemAsync(null, timeout, keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        public Dictionary<string, string> GetListFirstItem(params object[] keys) => this.GetListFirstItem(null, null, keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        public async Task<Dictionary<string, string>> GetListFirstItemAsync(params object[] keys) => await this.GetListFirstItemAsync(null, null, keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        public Dictionary<string, string> GetListLastItem(int? dbNum, int? timeout, params object[] keys) => this.Execute(CommandType.BRPOP, dbNum, result => (Dictionary<string, string>)result.Value, keys.Concat(new object[] { timeout ?? 0 }).ToArray());
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        public async Task<Dictionary<string, string>> GetListLastItemAsync(int? dbNum, int? timeout, params object[] keys) => await this.ExecuteAsync(CommandType.BRPOP, dbNum, async result => await Task.FromResult((Dictionary<string, string>)result.Value), keys.Concat(new object[] { timeout ?? 0 }).ToArray());
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        public Dictionary<string, string> GetListLastItem(int? timeout, params object[] keys) => this.GetListLastItem(null, timeout, keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        public async Task<Dictionary<string, string>> GetListLastItemAsync(int? timeout, params object[] keys) => await this.GetListLastItemAsync(null, timeout, keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        public Dictionary<string, string> GetListLastItem(params object[] keys) => this.GetListLastItem(null, null, keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        public async Task<Dictionary<string, string>> GetListLastItemAsync(params object[] keys) => await this.GetListLastItemAsync(null, null, keys);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public T GetListLastItemToOtherListFirst<T>(string key, string otherKey, int? timeout = 0, int? dbNum = null) => this.Execute(CommandType.BRPOPLPUSH, dbNum, result => result.Value.ToCast<T>(), key, otherKey, timeout ?? 0);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public async Task<T> GetListLastItemToOtherListFirstAsync<T>(string key, string otherKey, int? timeout = 0, int? dbNum = null) => await this.ExecuteAsync(CommandType.BRPOPLPUSH, dbNum, async result => await Task.FromResult(result.Value.ToCast<T>()), key, otherKey, timeout ?? 0);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public string GetListLastItemToOtherListFirst(string key, string otherKey, int? timeout = 0, int? dbNum = null) => this.GetListLastItemToOtherListFirst<string>(key, otherKey, timeout, dbNum);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public async Task<string> GetListLastItemToOtherListFirstAsync(string key, string otherKey, int? timeout = 0, int? dbNum = null) => await this.GetListLastItemToOtherListFirstAsync<string>(key, otherKey, timeout, dbNum);
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        public T GetListItem<T>(string key, int index, int? dbNum = null) => this.Execute(CommandType.LINDEX, dbNum, result => result.Value.ToCast<T>(), key, index);
        /// <summary>
        /// 通过索引获取列表中的元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        public async Task<T> GetListItemAsync<T>(string key, int index, int? dbNum = null) => await this.ExecuteAsync(CommandType.LINDEX, dbNum, async result => await Task.FromResult(result.Value.ToCast<T>()), key, index);
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        public string GetListItem(string key, int index, int? dbNum = null) => this.GetListItem<string>(key, index, dbNum);
        /// <summary>
        /// 通过索引获取列表中的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        public async Task<string> GetListItemAsync(string key, int index, int? dbNum = null) => await this.GetListItemAsync<string>(key, index, dbNum);
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        public T GetListFirstItem<T>(string key, int? dbNum = null) => this.Execute(CommandType.LPOP, dbNum, result => result.Value.ToCast<T>(), key);
        /// <summary>
        /// 移出并获取列表的第一个元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        public async Task<T> GetListFirstItemAsync<T>(string key, int? dbNum = null) => await this.ExecuteAsync(CommandType.LPOP, dbNum, async result => await Task.FromResult(result.Value.ToCast<T>()), key);
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        public string GetListFirstItem(string key, int? dbNum = null) => this.GetListFirstItem<string>(key, dbNum);
        /// <summary>
        /// 移出并获取列表的第一个元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        public async Task<string> GetListFirstItemAsync(string key, int? dbNum = null) => await this.GetListFirstItemAsync<string>(key, dbNum);
        /// <summary>
        /// 移出并获取列表的最后一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public T GetListLastItem<T>(string key, int? dbNum = null) => this.Execute(CommandType.RPOP, dbNum, result => result.Value.ToCast<T>(), key);
        /// <summary>
        /// 移出并获取列表的最后一个元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public async Task<T> GetListLastItemAsync<T>(string key, int? dbNum = null) => await this.ExecuteAsync(CommandType.RPOP, dbNum, async result => await Task.FromResult(result.Value.ToCast<T>()), key);
        /// <summary>
        /// 移出并获取列表的最后一个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public string GetListLastItem(string key, int? dbNum = null) => this.GetListLastItem<string>(key, dbNum);
        /// <summary>
        /// 移出并获取列表的最后一个元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        public async Task<string> GetListLastItemAsync(string key, int? dbNum = null) => await this.GetListLastItemAsync<string>(key, dbNum);
        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引 可以用负数 如 -1代表最后一个</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<string> GetListItems(string key, int start, int stop, int? dbNum = null) => this.Execute(CommandType.LRANGE, dbNum, result => (List<string>)result.Value, key, start, stop);
        /// <summary>
        /// 获取列表中指定区间内的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引 可以用负数 如 -1代表最后一个</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<string>> GetListItemsAsync(string key, int start, int stop, int? dbNum = null) => await this.ExecuteAsync(CommandType.LRANGE, dbNum, async result => await Task.FromResult((List<string>)result.Value), key, start, stop);

        #endregion

        #region 移除
        /// <summary>
        /// 根据参数 COUNT 的值，移除列表中与参数 VALUE 相等的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="count">移除数量 取绝对值 负数从表头开始向表尾搜索，正数从表尾开始向表头搜索 0移除所有</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean DelListItem(string key, string value, int count, int? dbNum = null) => this.Execute(CommandType.LREM, dbNum, result => result.OK, key, count, value);
        /// <summary>
        /// 根据参数 COUNT 的值，移除列表中与参数 VALUE 相等的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="count">移除数量 取绝对值 负数从表头开始向表尾搜索，正数从表尾开始向表头搜索 0移除所有</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> DelListItemAsync(string key, string value, int count, int? dbNum = null) => await this.ExecuteAsync(CommandType.LREM, dbNum, async result => await Task.FromResult(result.OK), key, count, value);
        /// <summary>
        /// 删除不在区间内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean DelListItem(string key, int start, int stop, int? dbNum = null) => this.Execute(CommandType.LTRIM, dbNum, result => result.OK, key, start, stop);
        /// <summary>
        /// 删除不在区间内的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> DelListItemAsync(string key, int start, int stop, int? dbNum = null) => await this.ExecuteAsync(CommandType.LTRIM, dbNum, async result => await Task.FromResult(result.OK), key, start, stop);
        #endregion

        #endregion
    }
}