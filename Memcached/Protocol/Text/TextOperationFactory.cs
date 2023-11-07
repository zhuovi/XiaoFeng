using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Internal;

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
namespace XiaoFeng.Memcached.Protocol.Text
{
    /// <summary>
    /// 文本协议操作工厂
    /// </summary>
    public class TextOperationFactory : BaseOperationFactory, IOperationFactory
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TextOperationFactory()
        {

        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config">配置</param>
        public TextOperationFactory(MemcachedConfig config)
        {
            this.MemcachedConfig = config;
        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region GET
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> GetAsync(params string[] keys)
        {
            return await GetAsync(CommandType.Get, 0, keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> GetsAsync(params string[] keys)
        {
            return await GetAsync(CommandType.Gets, 0, keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> GatAsync(uint expiry, params string[] keys)
        {
            return await GetAsync(CommandType.Gat, expiry, keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> GatsAsync(uint expiry, params string[] keys)
        {
            return await GetAsync(CommandType.Gats, expiry, keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> DeleteAsync(params string[] keys)
        {
            return await GetAsync(CommandType.Delete, 0, keys).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> IncrementAsync(string key, ulong step, ulong defaultValue, uint expiry)
        {
            return await GetAsync(CommandType.Increment, 0, new string[] { key, step.ToString() }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> DecrementAsync(string key, ulong step, ulong defaultValue, uint expiry)
        {
            return await GetAsync(CommandType.Decrement, 0, new string[] { key, step.ToString() }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.GetOperationResult> TouchAsync(uint expiry, params string[] keys)
        {
            return await GetAsync(CommandType.Touch, expiry, keys).ConfigureAwait(false);
        }
        /// <summary>
        /// Get操作
        /// </summary>
        /// <param name="commandType">命令</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="keys">key值</param>
        /// <returns></returns>
        private async Task<Internal.GetOperationResult> GetAsync(CommandType commandType, uint expiry, params string[] keys)
        {
            var result = new GetOperationResult()
            {
                Status = OperationStatus.Success
            };
            if (this.MemcachedConfig.Servers.Count == 1)
            {
                if (commandType == CommandType.Delete || commandType == CommandType.Touch)
                {
                    foreach (var k in keys)
                    {
                        string[] ks;
                        if (commandType == CommandType.Touch)
                            ks = new string[] { k, expiry.ToString() };
                        else
                            ks = new string[] { k };
                        var val = await this.Execute(k, async socket =>
                        {
                            var cmd = new GetCommand(socket, commandType, ks);
                            return await cmd.GetGetResponseAsync().ConfigureAwait(false);
                        });

                        if (val != null && val.Status == OperationStatus.Success)
                        {
                            result.Value.Add(new MemcachedValue(k, ValueType.String, null, 0));
                        }
                    }
                }
                else
                {
                    string[] ks;
                    if (commandType == CommandType.Gat || commandType == CommandType.Gats)
                    {
                        if (expiry > 0)
                            ks = ks = new string[] { expiry.ToString() }.Concat(keys).ToArray();
                        else
                            ks = keys;
                    }
                    else
                        ks = keys;
                    return await this.Execute("", async socket =>
                    {
                        var cmd = new GetCommand(socket, commandType, ks);
                        return await cmd.GetGetResponseAsync().ConfigureAwait(false);
                    });
                }
            }
            else
            {
                keys.Each(async k =>
                {
                    var val = await this.Execute(k, async socket =>
                    {
                        var cmd = new GetCommand(socket, commandType, k);
                        return await cmd.GetGetResponseAsync().ConfigureAwait(false);
                    });
                    if (val.Status == OperationStatus.Success)
                    {
                        result.Value.AddRange(val.Value);
                    }
                });
            }
            if (result.Value.Count == 0) result.Status = OperationStatus.Error;
            return result;
        }
        #endregion

        #region Store
        ///<inheritdoc/>
        public async Task<Internal.StoreOperationResult> SetAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Set, new object[] { key, expiry, value }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StoreOperationResult> AddAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Add, new object[] { key, expiry, value }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StoreOperationResult> ReplaceAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Replace, new object[] { key, expiry, value }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StoreOperationResult> AppendAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Append, new object[] { key, expiry, value }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StoreOperationResult> PrependAsync(string key, object value, uint expiry)
        {
            return await StoreAsync(CommandType.Prepend, new object[] { key, expiry, value }).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StoreOperationResult> CasAsync(string key, object value, ulong casToken, uint expiry)
        {
            return await StoreAsync(CommandType.Cas, new object[] { key, expiry, casToken, value }).ConfigureAwait(false);
        }
        /// <summary>
        /// Store操作
        /// </summary>
        /// <param name="commandType">命令</param>
        /// <param name="values">key值</param>
        /// <returns></returns>
        private async Task<Internal.StoreOperationResult> StoreAsync(CommandType commandType, object[] values)
        {
            return await base.Execute(values.First().ToString(), async socket =>
            {
                var result = new Internal.StoreOperationResult()
                {
                    Status = OperationStatus.Success
                };
                var cmd = new StoreCommand(socket, this.MemcachedConfig, commandType, values);
                var response = await cmd.GetStoreResponseAsync().ConfigureAwait(false);
                if (response != null && response.Status == OperationStatus.Success)
                {
                    result.Value = response.Value;
                }
                return await Task.FromResult(result);
            }).ConfigureAwait(false);
        }
        #endregion

        #region Stat
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> StatsAsync()
        {
            return await this.StatAsync(CommandType.Stats, 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> StatsItemsAsync()
        {
            return await this.StatAsync(CommandType.StatsItems, 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> StatsSlabsAsync()
        {
            return await this.StatAsync(CommandType.StatsSlabs, 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> StatsSizesAsync()
        {
            return await this.StatAsync(CommandType.StatsSizes, 0).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> StatsKeysAsync(int itemCount = 10000, int itemValue = 1)
        {
            return await this.StatAsync(CommandType.StatsKeys, 0, itemValue, itemCount).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> FulshAllAsync(uint timeout)
        {
            return await this.StatAsync(CommandType.FlushAll, timeout).ConfigureAwait(false);
        }
        ///<inheritdoc/>
        public async Task<Internal.StatsOperationResult> VersionAsync()
        {
            return await this.StatAsync(CommandType.Version, 0).ConfigureAwait(false);
        }
        /// <summary>
        /// Store操作
        /// </summary>
        /// <param name="commandType">命令</param>
        /// <param name="timeout">延迟多长时间执行清理</param>
        /// <param name="itemValue">指定需要查看的items的值 默认是1</param>
        /// <param name="itemCount">指定需要查看多少个key</param>
        /// <returns></returns>
        private async Task<Internal.StatsOperationResult> StatAsync(CommandType commandType, uint timeout, int itemValue = 0, int itemCount = 0)
        {
            var result = new StatsOperationResult()
            {
                Status = OperationStatus.Success
            };
            var val = this.Execute(socket =>
            {
                var cmd = new StatsCommand(socket, commandType, timeout, itemValue, itemCount);
                var response = cmd.GetStatResponseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                if (response != null && response.Status == OperationStatus.Success)
                {
                    return response.Value.Values.First();
                }
                return new Dictionary<string, string>();
            });
            result.Value = val;
            return await Task.FromResult(result);
        }
        #endregion

        #endregion
    }
}