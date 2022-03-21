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
*  Create Time : 2021-07-07 11:11:58                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// HyperLogLog
    /// </summary>
    public partial class RedisClient : Disposable
    {
        #region HyperLogLog
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="elements">元素</param>
        /// <returns></returns>
        public Boolean SetHyperLogLog(string key, int? dbNum, params object[] elements)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.PFADD, dbNum, result => result.OK && result.Value.ToString() == "1", new object[] { key }.Concat(elements).ToArray());
        }
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="elements">元素</param>
        /// <returns></returns>
        public async Task<Boolean> SetHyperLogLogAsync(string key, int? dbNum, params object[] elements)
        {
            if (key.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.PFADD, dbNum, async result => await Task.FromResult(result.OK && result.Value.ToString() == "1"), new object[] { key }.Concat(elements).ToArray());
        }
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="elements">元素</param>
        /// <returns></returns>
        public Boolean SetHyperLogLog(string key, params object[] elements) => this.SetHyperLogLog(key, null, elements);
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="elements">元素</param>
        /// <returns></returns>
        public async Task<Boolean> SetHyperLogLogAsync(string key, params object[] elements) => await this.SetHyperLogLogAsync(key, null, elements);
        /// <summary>
        /// 获取基数估算值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public int GetHyperLogLog(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.PFCOUNT, dbNum, result => result.OK ? (int)result.Value : -1, key);
        }
        /// <summary>
        /// 获取基数估算值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        public async Task<int> GetHyperLogLogAsync(string key, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.PFCOUNT, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), key);
        }
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="sourceKey">源key</param>
        /// <returns></returns>
        public Boolean MergeHyperLogLog(string destKey, int? dbNum, params object[] sourceKey)
        {
            if (destKey.IsNullOrEmpty()) return false;
            return this.Execute(CommandType.PFMERGE, dbNum, result => result.OK, new object[] { destKey }.Concat(sourceKey).ToArray());
        }

        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog 异步
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="sourceKey">源key</param>
        /// <returns></returns>
        public async Task<Boolean> MergeHyperLogLogAsync(string destKey, int? dbNum, params object[] sourceKey)
        {
            if (destKey.IsNullOrEmpty()) return false;
            return await this.ExecuteAsync(CommandType.PFMERGE, dbNum, async result => await Task.FromResult(result.OK), new object[] { destKey }.Concat(sourceKey).ToArray());
        }
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="sourceKey">源key</param>
        /// <returns></returns>
        public Boolean MergeHyperLogLog(string destKey, params object[] sourceKey) => this.MergeHyperLogLog(destKey, null, sourceKey);
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog 异步
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="sourceKey">源key</param>
        /// <returns></returns>
        public async Task<Boolean> MergeHyperLogLogAsync(string destKey, params object[] sourceKey) => await this.MergeHyperLogLogAsync(destKey, null, sourceKey);
        #endregion
    }
}