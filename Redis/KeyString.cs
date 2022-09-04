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
*  Create Time : 2021-07-07 11:01:29                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// KEY 操作
    /// </summary>
    public partial class RedisClient : Disposable
    {
        #region KEY

        #region 删除key
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        public int DelKey(int? dbNum, params string[] keys)
        {
            if (keys.Length == 0) return -1;
            return this.Execute(CommandType.DEL, dbNum, result => result.OK ? result.Value.ToInt() : -1, keys);
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        public int DelKey(params string[] keys)
        {
            if (keys.Length == 0) return -1;
            return this.DelKey(null, keys);
        }
        /// <summary>
        /// 删除key 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        public async Task<int> DelKeyAsync(int? dbNum, params string[] keys)
        {
            if (keys.Length == 0) return -1;
            return await this.ExecuteAsync(CommandType.DEL, dbNum, async result => await Task.FromResult(result.OK?result.Value.ToInt():-1), keys);
        }
        /// <summary>
        /// 删除key 异步
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        public async Task<int> DelKeyAsync(params string[] keys)
        {
            if (keys.Length == 0) return -1;
            return await this.DelKeyAsync(null, keys);
        }
        /// <summary>
        /// 获取key值 并删除 6.2.0后可用
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>删除key的值</returns>
        public string GetDelKey(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return string.Empty;
            return this.Execute(CommandType.GETDEL, dbNum, result => result.Value.ToString(), key);
        }
        /// <summary>
        /// 获取key值 并删除 6.2.0后可用 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>删除key的值</returns>
        public async Task<string> GetDelKeyAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return string.Empty;
            return await this.ExecuteAsync(CommandType.GETDEL, dbNum, async result => await Task.FromResult(result.Value.ToString()), key);
        }
        #endregion

        #region 序列化key
        /// <summary>
        /// 序列化key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public String DumpKey(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return String.Empty;
            return this.Execute(CommandType.DUMP, dbNum, result => (string)result.Value, key);
        }
        /// <summary>
        /// 序列化key 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<String> DumpKeyAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return String.Empty;
            return await this.ExecuteAsync(CommandType.DUMP, dbNum, async result => await Task.FromResult((string)result.Value), key);
        }
        #endregion

        #region 是否存在key
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean ExistsKey(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.EXISTS, dbNum, result => result.Value.ToInt() > 0, key);
        }
        /// <summary>
        /// 是否存在key 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> ExistsKeyAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.EXISTS, dbNum, async result => await Task.FromResult(result.Value.ToInt() > 0), key);
        }
        #endregion

        #region 设置过期时间
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="seconds">过期时长 单位为秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public int SetKeyExpireSeconds(string key, int seconds, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.EXPIRE, dbNum, result => result.Value.ToInt(), key, seconds);
        }
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="seconds">过期时长 单位为秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public async Task<int> SetKeyExpireSecondsAsync(string key, int seconds, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.EXPIRE, dbNum, result => Task.FromResult(result.Value.ToInt()), key, seconds);
        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="milliseconds">过期时长 单位为毫秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public int SetKeyExpireMilliseconds(string key, long milliseconds, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.PEXPIRE, dbNum, result => result.Value.ToInt(), key, milliseconds);
        }
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="milliseconds">过期时长 单位为毫秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public async Task<int> SetKeyExpireMillisecondsAsync(string key, long milliseconds, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.PEXPIRE, dbNum, result => Task.FromResult(result.Value.ToInt()), key, milliseconds);
        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamp">过期时长 秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public int SetKeyExpireSecondsTimestamp(string key, int timestamp, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.EXPIREAT, dbNum, result => result.Value.ToInt(), key, timestamp);
        }
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamp">过期时长 秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public async Task<int> SetKeyExpireSecondsTimestampAsync(string key, int timestamp, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.EXPIREAT, dbNum, result => Task.FromResult(result.Value.ToInt()), key, timestamp);
        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamps">过期时长 毫秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public int SetKeyExpireMillisecondsTimestamp(string key, long timestamps, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.PEXPIREAT, dbNum, result => result.Value.ToInt(), key, timestamps);
        }
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamps">过期时长 毫秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        public async Task<int> SetKeyExpireMillisecondsTimestampAsync(string key, long timestamps, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.PEXPIREAT, dbNum, result => Task.FromResult(result.Value.ToInt()), key, timestamps);
        }
        #endregion

        #region 重命名key
        /// <summary>
        /// 重命名key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public Boolean RenameKey(string key, string newKey, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || newKey.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.RENAME, dbNum, result => result.OK, key, newKey);
        }
        /// <summary>
        /// 重命名key 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public async Task<Boolean> RenameKeyAsync(string key, string newKey, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || newKey.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.RENAME, dbNum, async result => await Task.FromResult(result.OK), key, newKey);
        }
        /// <summary>
        /// 重命名key 当新key不存在时
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public Boolean RenameKeyNoExists(string key, string newKey, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || newKey.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.RENAMENX, dbNum, result => result.OK, key, newKey);
        }
        /// <summary>
        /// 重命名key 当新key不存在时 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public async Task<Boolean> RenameKeyNoExistsAsync(string key, string newKey, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || newKey.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.RENAMENX, dbNum, async result => await Task.FromResult(result.OK), key, newKey);
        }
        #endregion

        #region 移动key
        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public Boolean MoveKey(string key, int destDbNum, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            if (destDbNum < 0) destDbNum = 0;
            if (dbNum.HasValue && dbNum.Value == destDbNum) return false;
            return this.Execute(CommandType.MOVE, dbNum, result => result.OK, key, destDbNum);
        }
        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public async Task<Boolean> MoveKeyAsync(string key, int destDbNum, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            if (destDbNum < 0) destDbNum = 0;
            if (dbNum.HasValue && dbNum.Value == destDbNum) return false;
            return await this.ExecuteAsync(CommandType.MOVE, dbNum, async result => await Task.FromResult(result.OK), key, destDbNum);
        }
        #endregion

        #region 移除过期时间
        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public Boolean RemoveKeyExpire(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.PERSIST, dbNum, result => result.OK, key);
        }
        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        public async Task<Boolean> RemoveKeyExpireAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.PERSIST, dbNum, async result => await Task.FromResult(result.OK), key);
        }
        #endregion

        #region 获取key剩余过期时间
        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        public int GetKeyExpireSeconds(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.TTL, dbNum, result => result.OK ? (int)result.Value : -1, key);
        }
        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        public async Task<int> GetKeyExpireSecondsAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.TTL, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), key);
        }
        /// <summary>
        /// 以毫秒为单位，返回给定 key 的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        public int GetKeyExpireMilliseconds(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.PTTL, dbNum, result => result.OK ? (int)result.Value : -1, key);
        }
        /// <summary>
        /// 以毫秒为单位，返回给定 key 的剩余生存时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        public async Task<int> GetKeyExpireMillisecondsAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.PTTL, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), key);
        }
        #endregion

        #region 从当前数据库中随机返回一个 key
        /// <summary>
        /// 从当前数据库中随机返回一个 key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回KEY</returns>
        public string GetKeyRandom(int? dbNum = null) => this.Execute(CommandType.RANDOMKEY, dbNum, result => result.Value.ToString());
        /// <summary>
        /// 从当前数据库中随机返回一个 key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回KEY</returns>
        public async Task<string> GetKeyRandomAsync(int? dbNum) => await this.ExecuteAsync(CommandType.RANDOMKEY, dbNum, async result => await Task.FromResult(result.Value.ToString()));
        #endregion

        #region 返回 key 所储存的值的类型
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回数据类型</returns>
        public RedisKeyType GetKeyType(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return RedisKeyType.Error;
            return this.Execute(CommandType.TYPE, dbNum, result => (RedisKeyType)result.Value, key);
        }
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回数据类型</returns>
        public async Task<RedisKeyType> GetKeyTypeAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return RedisKeyType.Error;
            return await this.ExecuteAsync(CommandType.TYPE, dbNum, async result => await Task.FromResult((RedisKeyType)result.Value), key);
        }
        #endregion

        #region 查找key
        /// <summary>
        /// 查找数据库中的数据库键
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">返回条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<string> GetKeys(string pattern, int start = 0, int count = 10, int? dbNum = null) => this.Execute(CommandType.SCAN, dbNum, result => result.Value.ToList<string>(), start, "MATCH", pattern, "COUNT", count);
        /// <summary>
        /// 查找数据库中的数据库键 异步
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<string>> GetKeysAsync(string pattern, int start = 0, int count = 10, int? dbNum = null) => await this.ExecuteAsync(CommandType.SCAN, dbNum, async result => await Task.FromResult(result.Value.ToList<string>()), start, "MATCH", pattern, "COUNT", count);
        /// <summary>
        /// 查找所有符合给定模式(pattern)的 key
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public List<string> SearchKeys(string pattern, int? dbNum = null) => this.Execute(CommandType.KEYS, dbNum, result => result.Value.ToList<string>(), pattern);
        /// <summary>
        /// 查找所有符合给定模式(pattern)的 key 异步
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<string>> SearchKeysAsync(string pattern, int? dbNum = null) => await this.ExecuteAsync(CommandType.KEYS, dbNum, async result => await Task.FromResult(result.Value.ToList<string>()), pattern);
        #endregion

        #region 复制 Key
        /// <summary>
        /// 复制 Key 6.2.0版本
        /// </summary>
        /// <param name="key">源 key</param>
        /// <param name="destKey">目标 key</param>
        /// <param name="isReplace">存在是否替换</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean CopyKey(string key, string destKey, Boolean isReplace = false, int? destDbNum = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || destKey.IsNullOrEmpty()) return false;
            var list = new List<object>() { key, destKey };
            if (destDbNum.HasValue && destDbNum.Value > 0)
            {
                list.Add("DB");
                list.Add(destDbNum.Value);
            }
            if (isReplace) list.Add("REPLACE");
            return this.Execute(CommandType.COPY, dbNum, result => result.OK, list.ToArray());
        }
        /// <summary>
        /// 复制 Key 异步 6.2.0版本
        /// </summary>
        /// <param name="key">源 key</param>
        /// <param name="destKey">目标 key</param>
        /// <param name="isReplace">存在是否替换</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> CopyKeyAsync(string key, string destKey, Boolean isReplace = false, int? destDbNum = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || destKey.IsNullOrEmpty()) return false;
            var list = new List<object>() { key, destKey };
            if (destDbNum.HasValue && destDbNum.Value > 0)
            {
                list.Add("DB");
                list.Add(destDbNum.Value);
            }
            if (isReplace) list.Add("REPLACE");
            return await this.ExecuteAsync(CommandType.COPY, dbNum, async result => await Task.FromResult(result.OK), list.ToArray());
        }
        #endregion

        #endregion

        #region 字符串(String)

        #region 设置字符串
        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <param name="dbNum">数据库</param>
        /// <returns>是否设置成功</returns>
        public Boolean SetString<T>(string key, T value, TimeSpan? timeSpan = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            var args = new List<object>() { key, this.GetValue(value) };
            if (timeSpan != null) args.Insert(1, timeSpan.Value.TotalSeconds);
            return this.Execute(timeSpan != null ? CommandType.SETEX : CommandType.SET, dbNum, result => result.OK, args.ToArray());
        }
        /// <summary>
        /// 设置字符串 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public async Task<Boolean> SetStringAsync<T>(string key, T value, TimeSpan? timeSpan = null, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            var args = new List<object>() { key, this.GetValue(value) };
            if (timeSpan != null) args.Insert(1, timeSpan.Value.TotalSeconds);
            return await this.ExecuteAsync(timeSpan != null ? CommandType.SETEX : CommandType.SET, dbNum, async result => await Task.FromResult(result.OK), args.ToArray());
        }
        /// <summary>
        /// 批量设置值
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetString(Dictionary<string, object> values, int? dbNum = null)
        {
            if (values == null || values.Count == 0) return false;
            var list = new List<object>();
            values.Each(v =>
            {
                list.Add(v.Key);
                list.Add(this.GetValue(v.Value));
            });
            return this.Execute(CommandType.MSET, dbNum, result => result.OK, list.ToArray());
        }
        /// <summary>
        /// 批量设置值 异步
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetStringAsync(Dictionary<string, object> values, int? dbNum = null)
        {
            if (values == null || values.Count == 0) return false;
            var list = new List<object>();
            values.Each(v =>
            {
                list.Add(v.Key);
                list.Add(this.GetValue(v.Value));
            });
            return await this.ExecuteAsync(CommandType.MSET, dbNum, async result => await Task.FromResult(result.OK), list.ToArray());
        }
        /// <summary>
        /// 设置字符串 key不存在时
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public Boolean SetStringNoExists<T>(string key, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.SETNX, dbNum, result => result.OK && result.Value.ToInt() > 0, key, this.GetValue(value));
        }
        /// <summary>
        /// 设置字符串 key不存在时 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public async Task<Boolean> SetStringNoExistsAsync<T>(string key, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.SETNX, dbNum, async result => await Task.FromResult(result.OK && result.Value.ToInt() > 0), key, this.GetValue(value));
        }
        /// <summary>
        /// 批量设置值 key不存在时
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean SetStringNoExists(Dictionary<string, object> values, int? dbNum = null)
        {
            if (values == null || values.Count == 0) return false;
            var list = new List<object>();
            values.Each(v =>
            {
                list.Add(v.Key);
                list.Add(this.GetValue(v.Value));
            });
            return this.Execute(CommandType.MSETNX, dbNum, result => result.OK && result.Value.ToInt() > 0, list.ToArray());
        }
        /// <summary>
        /// 批量设置值 key不存在时 异步
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> SetStringNoExistsAsync(Dictionary<string, object> values, int? dbNum = null)
        {
            if (values == null || values.Count == 0) return false;
            var list = new List<object>();
            values.Each(v =>
            {
                list.Add(v.Key);
                list.Add(this.GetValue(v.Value));
            });
            return await this.ExecuteAsync(CommandType.MSETNX, dbNum, async result => await Task.FromResult(result.OK && result.Value.ToInt() > 0), list.ToArray());
        }
        /// <summary>
        /// 设置字符串 覆盖给定 key 所储存的字符串值，覆盖的位置从偏移量 offset 开始
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="offset">偏移量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public Boolean SetString(string key, string value, int offset, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.SETRANGE, dbNum, result => result.OK, key, offset, value);
        }
        /// <summary>
        /// 设置字符串 覆盖给定 key 所储存的字符串值，覆盖的位置从偏移量 offset 开始 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="offset">偏移量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        public async Task<Boolean> SetStringAsync(string key, string value, int offset, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.SETRANGE, dbNum, async result => await Task.FromResult(result.OK), key, offset, value);
        }
        /// <summary>
        /// 给指定的key值附加到原来值的尾部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public Boolean AppendString(string key, string value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.APPEND, dbNum, result => result.OK, key, value);
        }
        /// <summary>
        /// 给指定的key值附加到原来值的尾部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<Boolean> AppendStringAsync(string key, string value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.APPEND, dbNum, async result => await Task.FromResult(result.OK), key, value);
        }
        #endregion

        #region 获取字符串
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        public T GetString<T>(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return default(T);
            return this.Execute(CommandType.GET, dbNum, result => this.SetValue<T>(result.Value.ToString()), key);
        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        public string GetString(string key, int? dbNum = null) => this.GetString<string>(key, dbNum);
        /// <summary>
        /// 获取字符串 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        public async Task<T> GetStringAsync<T>(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return default(T);
            return await this.ExecuteAsync(CommandType.GET, dbNum, async result => await Task.FromResult(this.SetValue<T>(result.Value.ToString())), key);
        }
        /// <summary>
        /// 获取字符串 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        public async Task<string> GetStringAsync(string key, int? dbNum = null) => await this.GetStringAsync<string>(key, dbNum);
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">终止位置</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值的子字符串</returns>
        public string GetString(string key, int start, int end, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return string.Empty;
            return this.Execute(CommandType.GETRANGE, dbNum, result => result.Value.ToString(), key, start, end);
        }
        /// <summary>
        /// 获取字符串 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">终止位置</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值的子字符串</returns>
        public async Task<string> GetStringAsync(string key, int start, int end, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return string.Empty;
            return await this.ExecuteAsync(CommandType.GETRANGE, dbNum, result => Task.FromResult(result.Value.ToString()), key, start, end);
        }
        /// <summary>
        /// 获取 key 值的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public int GetStringLength(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.STRLEN, dbNum, result => result.Value.ToInt(), key);
        }
        /// <summary>
        /// 获取 key 值的长度 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<int> GetStringLengthAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.STRLEN, dbNum, async result => await Task.FromResult(result.Value.ToInt()), key);
        }
        #endregion

        #region 设置key的新值并返回key旧值
        /// <summary>
        /// 设置key的新值并返回key旧值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">key的新值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的旧值</returns>
        public T GetSetString<T>(string key, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return default(T);
            return this.Execute(CommandType.GETSET, dbNum, result => this.SetValue<T>(result.Value.ToString()), key, this.GetValue(value));
        }
        /// <summary>
        /// 设置key的新值并返回key旧值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">key的新值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的旧值</returns>
        public async Task<T> GetSetStringAsync<T>(string key, T value, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return default(T);
            return await this.ExecuteAsync(CommandType.GETSET, dbNum, async result => await Task.FromResult(this.SetValue<T>(result.Value.ToString())), key, this.GetValue(value));
        }
        #endregion

        #region 获取所有(一个或多个)给定key的值
        /// <summary>
        /// 获取所有(一个或多个)给定key的值
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        public List<string> GetString(int? dbNum, params object[] args)
        {
            if (args.Length == 0) return null;
            return this.Execute(CommandType.MGET, dbNum, result => result.Value.ToList<string>(), args);
        }
        /// <summary>
        /// 获取所有(一个或多个)给定key的值 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        public async Task<List<string>> GetStringAsync(int? dbNum, params object[] args)
        {
            if (args.Length == 0) return null;
            return await this.ExecuteAsync(CommandType.MGET, dbNum, result => Task.FromResult(result.Value.ToList<string>()), args);
        }
        /// <summary>
        /// 获取所有(一个或多个)给定key的值
        /// </summary>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        public List<string> GetString(params object[] args) => this.GetString(null, args);
        /// <summary>
        /// 获取所有(一个或多个)给定key的值 异步
        /// </summary>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        public async Task<List<string>> GetStringAsync(params object[] args) => await this.GetStringAsync(null, args);
        #endregion

        #region 设置自增长
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public double StringIncrement(string key, double increment, int? dbNum = null) => StringIncrement(key, (float)increment, dbNum);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<double> StringIncrementAsync(string key, double increment, int? dbNum = null) => await StringIncrementAsync(key, (float)increment, dbNum);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public float StringIncrement(string key, float increment, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.INCRBYFLOAT, dbNum, result => result.OK ? result.Value.ToCast<float>() : -1, key, increment);
        }
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<float> StringIncrementAsync(string key, float increment, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.INCRBYFLOAT, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToCast<float>() : -1), key, increment);
        }
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public long StringIncrement(string key, long increment, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.INCRBY, dbNum, result => result.OK ? result.Value.ToCast <long>() : -1, key, increment);
        }
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<long> StringIncrementAsync(string key, long increment, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.INCRBY, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToCast<long>() : -1), key, increment);
        }
        /// <summary>
        /// 将 key 中储存的数字值增一
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public int StringIncrement(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.INCR, dbNum, result => result.OK ? result.Value.ToInt() : -1, key);
        }
        /// <summary>
        /// 将 key 中储存的数字值增一 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<int> StringIncrementAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.INCR, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToInt() : -1), key);
        }
        /// <summary>
        /// key 所储存的值减去给定的减量值（decrement）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public long StringDecrement(string key, long decrement, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.DECRBY, dbNum, result => result.OK ? (long)result.Value : -1, key, decrement);
        }
        /// <summary>
        /// key 所储存的值减去给定的减量值（decrement） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<long> StringDecrementAsync(string key, long decrement, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.DECRBY, dbNum, async result => await Task.FromResult(result.OK ? (long)result.Value : -1), key, decrement);
        }
        /// <summary>
        /// 将 key 中储存的数字值减一
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public int StringDecrement(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.DECR, dbNum, result => result.OK ? result.Value.ToInt() : -1, key);
        }
        /// <summary>
        /// 将 key 中储存的数字值减一 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<int> StringDecrementAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.DECR, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToInt() : -1), key);
        }
        #endregion

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
            return this.Execute(CommandType.SORT, dbNum, result => options.Store.IsNullOrEmpty() ? result.Value.ToList<string>() : new List<string> { result.Value.ToString() }, new object[] { key }.Concat(options.ToArgments()).ToArray());
        }
        /// <summary>
        /// 排序 异步 List,Set,SortedSet
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">排序选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<List<string>> SortAsync(string key, SortOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.SORT, dbNum, async result => await Task.FromResult(result.Value.ToList<string>()), new object[] { key }.Concat(options.ToArgments()).ToArray());
        }
        #endregion
    }
}