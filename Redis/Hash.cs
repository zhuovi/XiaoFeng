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
*  Create Time : 2021-07-07 11:03:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 哈希(Hash)
    /// </summary>
    public partial class RedisClient : Disposable
    {
        #region 哈希(Hash)

        #region 设置Hash
        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public Boolean SetHash<T>(string key, string fieldName, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || fieldName.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.HSET, dbNum, result => result.OK, key, fieldName, this.GetValue(value));
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public async Task<Boolean> SetHashAsync<T>(string key, string fieldName, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || fieldName.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.HSET, dbNum, async result => await Task.FromResult(result.OK), key, fieldName, this.GetValue(value));
        }

        /// <summary>
        /// 只有在字段 field 不存在时，设置哈希表字段的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetHashNoExists<T>(string key, string fieldName, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || fieldName.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.HSETNX, dbNum, result => result.OK, key, fieldName, this.GetValue(value));
        }

        /// <summary>
        /// 只有在字段 field 不存在时，设置哈希表字段的值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public async Task<Boolean> SetHashNoExistsAsync<T>(string key, string fieldName, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || fieldName.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.HSETNX, dbNum, async result => await Task.FromResult(result.OK), key, fieldName, this.GetValue(value));
        }

        /// <summary>
        /// 批量设置Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">字段值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetHash(string key, Dictionary<string, object> values, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || values.IsNullOrEmpty() || !values.Any()) return false;
            var list = new List<object>
            {
                key
            };
            values.Each(v =>
            {
                list.Add(v.Key);
                list.Add(this.GetValue(v.Value));
            });
            return this.Execute(CommandType.HMSET, dbNum, result => result.OK, list.ToArray());
        }
        /// <summary>
        /// 批量设置Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">字段值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetHashAsync(string key, Dictionary<string, object> values, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || values.IsNullOrEmpty() || !values.Any()) return false;
            var list = new List<object>
            {
                key
            };
            values.Each(v =>
            {
                list.Add(v.Key);
                list.Add(this.GetValue(v.Value));
            });
            return await this.ExecuteAsync(CommandType.HMSET, dbNum, async result => await Task.FromResult(result.OK), list.ToArray());
        }
        #endregion

        #region 获取Hash
        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public T GetHash<T>(string key, string field, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || field.IsNullOrEmpty()) return default(T);
            return this.Execute(CommandType.HGET, dbNum, result => this.SetValue<T>(result?.Value), key, field);
        }
        /// <summary>
        /// 获取Hash值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<T> GetHashAsync<T>(string key, string field, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || field.IsNullOrEmpty()) return default(T);
            return await this.Execute(CommandType.HGET, dbNum, async result => await Task.FromResult(this.SetValue<T>(result.Value)), key, field);
        }
        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>Hash值</returns>
        public string GetHash(string key, string field, int? dbNum = null) => this.GetHash<string>(key, field, dbNum);
        /// <summary>
        /// 获取Hash值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>Hash值</returns>
        public async Task<string> GetHashAsync(string key, string field, int? dbNum = null) => await this.GetHashAsync<string>(key, field, dbNum);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Dictionary<string, string> GetHash(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.HGETALL, dbNum, result => (Dictionary<string, string>)result.Value, key);
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetHashAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.HGETALL, dbNum, async result => await Task.FromResult((Dictionary<string, string>)result.Value), key);
        }
        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        public List<string> GetHashKeys(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.HKEYS, dbNum, result => (List<string>)result.Value, key);
        }
        /// <summary>
        /// 获取所有哈希表中的字段 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        public async Task<List<string>> GetHashKeysAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.HKEYS, dbNum, async result => await Task.FromResult((List<string>)result.Value), key);
        }

        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        public List<string> GetHashValues(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.HVALS, dbNum, result => (List<string>)result.Value, key);
        }
        /// <summary>
        /// 获取所有哈希表中的字段 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        public async Task<List<string>> GetHashValuesAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.HVALS, dbNum, async result => await Task.FromResult((List<string>)result.Value), key);
        }
        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>哈希表中字段的数量</returns>
        public int GetHashKeysCount(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.HLEN, dbNum, result => result.Value.ToCast<int>(), key);
        }
        /// <summary>
        /// 获取哈希表中字段的数量 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>哈希表中字段的数量</returns>
        public async Task<int> GetHashKeysCountAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.HLEN, dbNum, async result => await Task.FromResult(result.Value.ToCast<int>()), key);
        }
        /// <summary>
        /// 获取所有给定字段的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns>返回所有给定字段的值</returns>
        public List<string> GetHash(string key, int? dbNum, params object[] fields)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.HMGET, dbNum, result => (List<string>)result.Value, new object[] { key }.Concat(fields).ToArray());
        }
        /// <summary>
        /// 获取所有给定字段的值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns>返回所有给定字段的值</returns>
        public async Task<List<string>> GetHashAsync(string key, int? dbNum, params object[] fields)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.HMGET, dbNum, async result => await Task.FromResult((List<string>)result.Value), new object[] { key }.Concat(fields).ToArray());
        }
        /// <summary>
        /// 获取所有给定字段的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns>返回字段值</returns>
        public List<string> GetHash(string key, params object[] fields) => this.GetHash(key, null, fields);
        /// <summary>
        /// 获取所有给定字段的值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns>返回字段值</returns>
        public async Task<List<string>> GetHashAsync(string key, params object[] fields) => await this.GetHashAsync(key, null, fields);
        /// <summary>
        /// 查找Hash中字段名
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        public Dictionary<string, string> SearchHashMember(string key, string pattern, int start = 0, int count = 10, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.HSCAN, dbNum, result => (Dictionary<string, string>)result.Value, key, start, "MATCH", pattern, "COUNT", count);
        }

        /// <summary>
        /// 查找Hash中字段名 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        public async Task<Dictionary<string, string>> SearchHashMemberAsync(string key, string pattern, int start = 0, int count = 10, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.HSCAN, dbNum, async result => await Task.FromResult((Dictionary<string, string>)result.Value), key, start, "MATCH", pattern, "COUNT", count);
        }
        #endregion

        #region 删除Hash
        /// <summary>
        /// 删除Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public Boolean DelHash(string key, int? dbNum, params object[] fields)
        {
            if (key.IsNullOrEmpty() || fields.Length == 0) return false;
            return this.Execute(CommandType.HDEL, dbNum, result => result.OK, new object[] { key }.Concat(fields).ToArray());
        }
        /// <summary>
        /// 删除Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public async Task<Boolean> DelHashAsync(string key, int? dbNum, params object[] fields)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.HDEL, dbNum, async result => await Task.FromResult(result.OK), new object[] { key }.Concat(fields).ToArray());
        }
        /// <summary>
        /// 删除Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public Boolean DelHash(string key, params object[] fields) => this.DelHash(key, null, fields);
        /// <summary>
        /// 删除Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        public async Task<Boolean> DelHashAsync(string key, params object[] fields) => await this.DelHashAsync(key, null, fields);
        #endregion

        #region 是否存在Hash
        /// <summary>
        /// 是否存在Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean ExistsHash(string key, string field, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || field.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.HEXISTS, dbNum, result => result.OK, key, field);
        }
        /// <summary>
        /// 是否存在Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> ExistsHashAsync(string key, string field, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || field.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.HEXISTS, dbNum, async result => await Task.FromResult(result.OK), key, field);
        }
        #endregion

        #region 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        public T HashIncrement<T>(string key, string field, T increment, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || field.IsNullOrEmpty()) return default(T);
            return this.Execute(typeof(T) == typeof(int) ? CommandType.HINCRBY : CommandType.HINCRBYFLOAT, dbNum, result => result.Value.ToCast<T>(), key, field, increment);
        }
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        public async Task<T> HashIncrementAsync<T>(string key, string field, T increment, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || field.IsNullOrEmpty()) return default(T);
            return await this.ExecuteAsync(typeof(T) == typeof(int) ? CommandType.HINCRBY : CommandType.HINCRBYFLOAT, dbNum, async result => await Task.FromResult(result.Value.ToCast<T>()), key, field, increment);
        }
        #endregion

        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">排序选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<string> Sort(string key, SortOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.SORT, dbNum, result => options.Store.IsNullOrEmpty() ? (List<string>)result.Value : new List<string> { result.Value.ToString() }, new object[] { key }.Concat(options.ToArgments()).ToArray());
        }
        /// <summary>
        /// 排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">排序选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<string>> SortAsync(string key, SortOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.SORT, dbNum, async result => await Task.FromResult((List<string>)result.Value), new object[] { key }.Concat(options.ToArgments()).ToArray());
        }
        #endregion

        #endregion
    }
}