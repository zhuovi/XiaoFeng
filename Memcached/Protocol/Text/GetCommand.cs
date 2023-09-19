using System;
using System.Collections.Generic;
using System.IO;
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
*  Create Time : 2023-09-15 16:15:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Text
{
    /// <summary>
    /// 获取操作类
    /// </summary>
    public class GetCommand : BaseCommand
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="socket">网络</param>
        /// <param name="commandType">命令</param>
        /// <param name="keys">key</param>
        public GetCommand(ISocketClient socket, CommandType commandType, params string[] keys)
        {
            this.SocketClient = socket;
            this.CommandType = commandType;
            this.Keys = keys;
        }
        #endregion

        #region 属性
        /// <summary>
        /// Key
        /// </summary>
        public string[] Keys { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public override async Task<Internal.GetOperationResult> GetGetResponseAsync()
        {
            var line = this.CommandType.GetEnumName() + " " + this.Keys.Join(" ") + this.CRLF;
            await this.SocketClient.SendAsync(line).ConfigureAwait(false);
            var bytes = await this.SocketClient.ReceviceMessageAsync().ConfigureAwait(false);
            return await Task.FromResult(new GetOperationResult(bytes, this.SocketClient.Encoding, this.CommandType));
        }
        ///<inheritdoc/>
        public override Task<Internal.StoreOperationResult> GetStoreResponseAsync()
        {
            throw new NotImplementedException();
        }
        ///<inheritdoc/>
        public override Task<Internal.StatsOperationResult> GetStatResponseAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    /// <summary>
    /// Get操作结果
    /// </summary>
    public class GetOperationResult : Internal.GetOperationResult
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public GetOperationResult() { this.Value = new List<MemcachedValue>(); }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="commandType">命令</param>
        public GetOperationResult(byte[] buffers, Encoding encoding, CommandType commandType)
        {
            this.PayLoad = buffers;
            var val = buffers.GetString(encoding);
            if (val.StartsWith(ReturnResult.END.GetEnumName(), StringComparison.OrdinalIgnoreCase))
            {
                this.Status = OperationStatus.Success;
                this.Value = null;
                return;
            }
            if (val.StartsWith(ReturnResult.DELETED.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    val.StartsWith(ReturnResult.TOUCHED.GetEnumName(), StringComparison.OrdinalIgnoreCase))
            {
                this.Status = OperationStatus.Success;
                return;
            }
            if (val.StartsWith(ReturnResult.ERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                val.StartsWith(ReturnResult.CLIENTERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                val.StartsWith(ReturnResult.NOT_FOUND.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                val.StartsWith(ReturnResult.SERVERERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase))
            {
                this.Status = OperationStatus.Error;
                this.Message = val;
                return;
            }
            if (commandType == CommandType.Increment || commandType == CommandType.Decrement)
            {
                if (val.IsNumberic())
                {
                    this.Status = OperationStatus.Success;
                    if (this.Value == null) this.Value = new List<MemcachedValue>();
                    this.Value.Add(new MemcachedValue("", ValueType.ULong, val.ToCast<ulong>(), 0));
                }
                else
                {
                    this.Status = OperationStatus.Error;
                    this.Message = val;
                }
                return;
            }
            var reader = new StringReader(val);
            var line = reader.ReadLine();
            while (line.StartsWith("VALUE", StringComparison.OrdinalIgnoreCase) && !line.EqualsIgnoreCase(ReturnResult.END.GetEnumName()))
            {
                var value = new MemcachedValue();
                var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (args[0].EqualsIgnoreCase("VALUE"))
                {
                    this.Status = OperationStatus.Success;
                    if (args.Length > 4)
                        value.Unique = args[4].ToCast<ulong>();
                    value.Key = args[1];
                    value.Type = args[2].ToEnum<ValueType>();
                    var length = args[3].ToCast<int>();
                    var bytes = new char[length];
                    reader.Read(bytes, 0, bytes.Length);
                    //line = reader.ReadLine();
                    value.Value = Helper.Deserialize(bytes.ToArray<byte>(), value.Type);
                    if (this.Value == null) this.Value = new List<MemcachedValue>();
                    this.Value.Add(value);
                }
                line = reader.ReadLine();
            }
        }
    }
}