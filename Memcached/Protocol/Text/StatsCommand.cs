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
*  Create Time : 2023-09-18 20:57:06                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Text
{
    /// <summary>
    /// Stats操作类
    /// </summary>
    public class StatsCommand : BaseCommand
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="socket">网络</param>
        /// <param name="commandType">命令</param>
        /// <param name="timeout">延时时间</param>
        /// <param name="itemValue">指定需要查看的items的值</param>
        /// <param name="itemCount">指定需要查看多少个key</param>
        public StatsCommand(ISocketClient socket, CommandType commandType, uint timeout,int itemValue,int itemCount)
        {
            this.SocketClient = socket;
            this.CommandType = commandType;
            this.Timeout = timeout;
            this.ItemValue = itemValue;
            this.ItemCount = itemCount;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 延时时间
        /// </summary>
        public uint Timeout { get; set; }
        /// <summary>
        /// 指定需要查看的items的值
        /// </summary>
        public int ItemValue { get; set; }
        /// <summary>
        /// 指定需要查看多少个key
        /// </summary>
        public int ItemCount { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override Task<Internal.GetOperationResult> GetGetResponseAsync()
        {
            throw new NotImplementedException();
        }
        ///<inheritdoc/>
        public override Task<Internal.StoreOperationResult> GetStoreResponseAsync()
        {
            throw new NotImplementedException();
        }
        ///<inheritdoc/>
        public async override Task<Internal.StatsOperationResult> GetStatResponseAsync()
        {
            var line = this.CommandType.GetEnumName();
            if (this.CommandType == CommandType.StatsKeys)
                line += $" {this.ItemValue} {this.ItemCount}";
            line += this.CRLF;
            await this.SocketClient.SendAsync(line).ConfigureAwait(false);
            var bytes = await this.SocketClient.ReceviceMessageAsync().ConfigureAwait(false);
            return await Task.FromResult(new StatsOperationResult(bytes, this.SocketClient.Encoding, this.CommandType));
        }
        #endregion
    }
    /// <summary>
    /// Stat操作结果
    /// </summary>
    public class StatsOperationResult : Internal.StatsOperationResult
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public StatsOperationResult() { this.Value = new Dictionary<System.Net.IPEndPoint, Dictionary<string, string>>(); }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffers">数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="commandType">命令</param>
        public StatsOperationResult(byte[] buffers, Encoding encoding, CommandType commandType)
        {
            //this.PayLoad = buffers;
            var val = buffers.GetString(encoding);
            var reader = new StringReader(val);
            var line = reader.ReadLine();
            if (line.StartsWith(ReturnResult.ERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith(ReturnResult.CLIENTERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith(ReturnResult.NOT_FOUND.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith(ReturnResult.SERVERERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase))
                return;
            this.Status = OperationStatus.Success;
            if (this.Value == null) this.Value = new Dictionary<System.Net.IPEndPoint, Dictionary<string, string>>();
            var dict = new Dictionary<string, string>();
            this.Value.Add(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 11211), dict);
            if (line.StartsWith("Version", StringComparison.OrdinalIgnoreCase))
            {
                dict.Add("Version", line.RemovePattern(@"^version\s+"));
                return;
            }
            if(commandType== CommandType.StatsKeys)
            {
                while(line.StartsWith("ITEM", StringComparison.OrdinalIgnoreCase) && !line.EqualsIgnoreCase(ReturnResult.END.GetEnumName()))
                {
                    var args = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length == 6)
                    {
                        var length = args[2].Trim('[');
                        dict.Add(args[1], length);
                    }
                    line = reader.ReadLine();
                }
                return;
            }
            while (line.StartsWith("STAT", StringComparison.OrdinalIgnoreCase) && !line.EqualsIgnoreCase(ReturnResult.END.GetEnumName()))
            {
                var args = line.RemovePattern(@"^STAT\s+").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 1)
                {
                    if (!dict.ContainsKey(args[0]))
                        dict.Add(args[0], string.Empty);
                }
                else if (args.Length == 2)
                {
                    if (commandType == CommandType.StatsSizes)
                    {
                        if (dict.ContainsKey("Item Size") || dict.ContainsKey("Item Count")) return;
                        dict.Add("Item Size", args[0]);
                        dict.Add("Item Count", args[1]);
                    }
                    else
                    {
                        if (!dict.ContainsKey(args[0]))
                            dict.Add(args[0], args[1]);
                    }
                }
                line = reader.ReadLine();
            }
        }
    }
}