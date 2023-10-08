using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Internal;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 17:38:28                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /// <summary>
    /// 二进制协议操作工厂
    /// </summary>
    public class BinaryOperationFactory : BaseOperationFactory, IOperationFactory
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BinaryOperationFactory()
        {

        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">配置</param>
        public BinaryOperationFactory(MemcachedConfig config)
        {
            this.MemcachedConfig = config;
        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region GET
        ///<inheritdoc/>
        public async Task<GetOperationResult> GetAsync(params string[] keys)
        {
            return await GetAsync(CommandType.Get,null, keys);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> GetsAsync(params string[] keys)
        {
            return await GetAsync(CommandType.GetK,null, keys);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> GatAsync(uint expiry, params string[] keys)
        {
            return await GetAsync(CommandType.Gat, ToBytes(expiry, 4), keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> GatsAsync(uint expiry, params string[] keys)
        {
            return await GetAsync(CommandType.GATQ, ToBytes(expiry, 4), keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> DeleteAsync(params string[] keys)
        {
            return await GetAsync(CommandType.Delete, null, keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> IncrementAsync(string key, ulong step, ulong defaultValue, uint expiry)
        {
            var delta = ToBytes((long)step, 8);
            var initial = ToBytes((long)defaultValue, 8);
            var expire = ToBytes(expiry, 4);
            return await GetAsync(CommandType.Increment, delta.Concat(initial).Concat(expire).ToArray(), new string[] { key }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> DecrementAsync(string key, ulong step, ulong defaultValue, uint expiry)
        {
            var delta = ToBytes((long)step, 8);
            var initial = ToBytes((long)defaultValue, 8);
            var expire = ToBytes(expiry, 4);
            return await GetAsync(CommandType.Decrement, delta.Concat(initial).Concat(expire).ToArray(), new string[] { key }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<GetOperationResult> TouchAsync(uint expiry, params string[] keys)
        {
            return await GetAsync(CommandType.Touch, ToBytes(expiry, 4), keys).ConfigureAwait(false);
        }
        /// <summary>
        /// 获取Get数据
        /// </summary>
        /// <param name="commandOpcode">命令</param>
        /// <param name="extras">扩展数据</param>
        /// <param name="keys">键</param>
        /// <returns></returns>
        private async Task<GetOperationResult> GetAsync(CommandType commandOpcode, byte[] extras, string[] keys)
        {
            var result = new GetOperationResult() { Value = new List<MemcachedValue>() };

            foreach (var k in keys)
            {
                var val = await this.Execute(k, async socket =>
                {
                    var request = new RequestPacket(socket, this.MemcachedConfig, "elf")
                    {
                        Opcode = commandOpcode,
                        Key = k.GetBytes()
                    };
                    if (extras != null && extras.Length > 0)
                    {
                        request.Extras = extras;
                    }
                    return await request.GetResponseValueAsync().ConfigureAwait(false);
                });
                result.Value.Add(val);
            }
            if (result.Value.Count > 0) result.Status = OperationStatus.Success;
            return await Task.FromResult(result);
        }
        #endregion

        #region STORE
        ///<inheritdoc/>
        public async Task<StoreOperationResult> SetAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Set, key, value, expiry).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StoreOperationResult> AddAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Add, key, value, expiry).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StoreOperationResult> ReplaceAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Replace, key, value, expiry).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StoreOperationResult> AppendAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Append, key, value, expiry).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StoreOperationResult> PrependAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Prepend, key, value, expiry).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StoreOperationResult> CasAsync(string key, object value, ulong casToken, uint expiry)
        {
            return await Task.FromResult(new StoreOperationResult { Status = OperationStatus.Error, Message = "当前Binary暂不支持." });
            /*var val = Helper.Serialize(value, out var type, (uint)this.MemcachedConfig.CompressLength);
            var Flags = ToBytes((int)type, 4);
            if (expiry > 0) Flags = Flags.Concat(ToBytes((long)expiry, 4)).ToArray();
            return await StoreAsync(CommandOpcode.Set, Flags, key.GetBytes(), val).ConfigureAwait(false);*/
        }
        /// <summary>
        /// 获取Store数据
        /// </summary>
        /// <param name="commandOpcode">命令</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        private async Task<StoreOperationResult> StoreAsync(CommandType commandOpcode, string key, object value, uint expiry)
        {
            return await this.Execute(key, async socket =>
            {
                var result = new StoreOperationResult() { Value = false };
                var val = Helper.Serialize(value, out var type, (uint)this.MemcachedConfig.CompressLength);
                var Extras = ToBytes((int)type, 4);
                Extras = Extras.Concat(ToBytes(expiry, 4)).ToArray();
                var request = new RequestPacket(socket, this.MemcachedConfig, "elf")
                {
                    Opcode = commandOpcode,
                    Key = key.GetBytes(this.MemcachedConfig.Encoding),
                    Value = val,
                    Extras = Extras
                };

                var response = await request.GetResponseValueAsync().ConfigureAwait(false);
                if (response != null) { }
                if (result.Value) result.Status = OperationStatus.Success;
                return result;
            });
        }
        #endregion

        #region STAT
        ///<inheritdoc/>
        public async Task<StatsOperationResult> StatsAsync()
        {
            return await StatAsync(CommandType.Stats, "", 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StatsOperationResult> StatsItemsAsync()
        {
            return await StatAsync(CommandType.Stats, "items", 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StatsOperationResult> StatsSlabsAsync()
        {
            return await StatAsync(CommandType.Stats, "slabs", 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StatsOperationResult> StatsSizesAsync()
        {
            return await StatAsync(CommandType.Stats, "sizes", 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StatsOperationResult> StatsKeysAsync(int itemCount = 10000, int itemValue = 1)
        {
            return await StatAsync(CommandType.Stats, "cachedump", 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StatsOperationResult> FulshAllAsync(uint timeout)
        {
            return await StatAsync(CommandType.FlushAll, "", 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<StatsOperationResult> VersionAsync()
        {
            return await StatAsync(CommandType.Version, "", 0).ConfigureAwait(false);
        }
        /// <summary>
        /// 获取Store数据
        /// </summary>
        /// <param name="commandOpcode">命令</param>
        /// <param name="key">key</param>
        /// <param name="timeout">延迟多长时间执行清理</param>
        /// <returns></returns>
        private async Task<StatsOperationResult> StatAsync(CommandType commandOpcode,string key, uint timeout)
        {
            var result = new StatsOperationResult() { Value = new Dictionary<System.Net.IPEndPoint, Dictionary<string, string>>() };
            var val = this.Execute(socket =>
            {
                var dict = new Dictionary<string, string>();
                var request = new RequestPacket(socket, this.MemcachedConfig, "")
                {
                    Opcode = commandOpcode
                };
                if (key.IsNotNullOrEmpty()) request.Key = key.GetBytes();
                if (timeout > 0)
                {
                    request.Extras = this.ToBytes(timeout, 4);
                }
                var response = request.GetResponseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                if (response != null && response.Key.IsNullOrEmpty() && commandOpcode == CommandType.Version)
                {
                    dict.Add("Version", response.Value.GetString());
                    return dict;
                }
                if (response != null && response.Key?.Length > 0)
                {
                    if (key.EqualsIgnoreCase("sizes"))
                    {
                        if (dict.ContainsKey("Item Size") || dict.ContainsKey("Item Count")) return dict;
                        dict.Add("Item Size", response.Key.GetString());
                        dict.Add("Item Count", response.Value.GetString());
                    }
                    else
                    {
                        var startIndex = 0;
                        var Key = response.Key.GetString();
                        //if (key.EqualsIgnoreCase("slabs")) Key = Key.TrimStart(new char[] { '1', ':' });
                        dict.Add(Key.Substring(startIndex), response.Value.GetString());
                        do
                        {
                            response = new ResponsePacket(response.PayLoad);
                            if (response != null && response.Key != null && response.Key.Length > 0)
                            {
                                Key = response.Key.GetString();
                                if (key.EqualsIgnoreCase("slabs")) Key = Key.TrimStart(new char[] { '1', ':' });
                                dict.Add(Key, response.Value.GetString());
                            }
                        } while (response.PayLoad != null && response.PayLoad.Length > 0);
                    }
                }
                return dict;
            });
            result.Value = val;
            return await Task.FromResult(result);
        }
        #endregion

        #region 数值转字节
        /// <summary>
        /// 数值转字节
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="length">字节数组长度</param>
        /// <returns></returns>
        private byte[] ToBytes(long val, int length)
        {
            return val.ToString("X").PadLeft(2 * length, '0').HexStringToBytes();
        }
        #endregion

        #endregion
    }
}