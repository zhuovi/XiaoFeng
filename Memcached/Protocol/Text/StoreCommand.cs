using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Internal;
using XiaoFeng.Net;
using System.Linq;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-18 19:07:33                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Text
{
    /// <summary>
    /// StoreCommand 类说明
    /// </summary>
    public class StoreCommand:BaseCommand
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="socket">网络</param>
        /// <param name="config">配置</param>
        /// <param name="commandType">命令</param>
        /// <param name="values">值</param>
        public StoreCommand(ISocketClient socket,Internal.MemcachedConfig config, CommandType commandType, object[] values)
        {
            this.SocketClient = socket;
            this.CommandType = commandType;
            this.Values = values;
            this.Config = config;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 值
        /// </summary>
        public object[] Values { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public Internal.MemcachedConfig Config { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override Task<Internal.GetOperationResult> GetGetResponseAsync()
        {
            throw new NotImplementedException();
        }
        ///<inheritdoc/>
        public async override Task<Internal.StoreOperationResult> GetStoreResponseAsync()
        {
            var args = this.Values.ToList();
            var val = this.Values.Last().ToString();
            args.RemoveAt(args.Count - 1);
            var value = Helper.Serialize(val, out var type, (uint)this.Config.CompressLength);
            args.Insert(1, (int)type);
            if (this.CommandType == CommandType.Cas)
            {
                args.Insert(args.Count - 1, val.GetByteCount());
            }
            else
            {
                args.Add(val.GetByteCount());
            }
            var line = this.CommandType.GetEnumName().ToLower() + " " + args.Join(" ") + this.CRLF;
            await this.SocketClient.SendAsync(line).ConfigureAwait(false);
            await this.SocketClient.SendAsync(value).ConfigureAwait(false);
            await this.SocketClient.SendAsync(this.CRLF).ConfigureAwait(false);
            var bytes = await this.SocketClient.ReceviceMessageAsync().ConfigureAwait(false);
            return await Task.FromResult(new StoreOperationResult(bytes, this.SocketClient.Encoding, this.CommandType));
        }
        ///<inheritdoc/>
        public override Task<Internal.StatsOperationResult> GetStatResponseAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    /// <summary>
    /// Store操作结果
    /// </summary>
    public class StoreOperationResult : Internal.StoreOperationResult
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public StoreOperationResult() { this.Value = false; }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="commandType">命令</param>
        public StoreOperationResult(byte[] buffers, Encoding encoding, CommandType commandType)
        {
            this.PayLoad = buffers;
            var val = buffers.GetString(encoding);
            this.Status = val.StartsWith(ReturnResult.STORED.GetEnumName(), StringComparison.OrdinalIgnoreCase) ? OperationStatus.Success : OperationStatus.Error;
            if (this.Status == OperationStatus.Success) this.Value = true;
        }
    }
}