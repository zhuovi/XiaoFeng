using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-09 19:39:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 命令请求类
    /// </summary>
    public class CommandPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="compressLength">压缩阀值</param>
        /// <param name="datas">参数集</param>
        public CommandPacket(CommandType commandType, uint compressLength, params object[] datas)
        {
            this.CommandType = commandType;
            this.Datas = datas;
            this.CompressLength = compressLength;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 网络流
        /// </summary>
        public NetworkStream Stream { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 命令数据
        /// </summary>
        public object[] Datas { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public byte[] Value { get; set; }
        /// <summary>
        /// 压缩阀值
        /// </summary>
        private uint CompressLength { get; set; }
        /// <summary>
        /// 结束
        /// </summary>
        private byte[] EOF = new byte[] { 13,10 };
        #endregion

        #region 方法
        /// <summary>
        /// 命令行
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var SubCommands = this.CommandType.Commands;
            var line = SubCommands.Join(" ") + " ";
            var value = this.Datas.Join(" ");
            if (this.CommandType == CommandType.AUTH)
            {
                this.Value = Helper.Serialize(value, out var type, this.CompressLength);
                line += $"auth   {value.GetByteCount()}";
            }
            else
            {
                if (this.CommandType.Flags == CommandFlags.Store)
                {
                    //set key flags exptime bytes [noreply]
                    var args = this.Datas.ToList();
                    var val = this.Datas.Last().ToString();
                    args.RemoveAt(args.Count - 1);
                   this.Value = Helper.Serialize(val, out var type, this.CompressLength);
                    args.Insert(1, (int)type);
                    if (this.CommandType == CommandType.CAS)
                    {
                        args.Insert(args.Count - 1, val.GetByteCount());
                    }
                    else
                    {
                        args.Add(val.GetByteCount());
                    }
                    line += args.Join(" ");
                }
                else
                {
                    line += value;
                }
            }
            return line;
        }
        /// <summary>
        /// 命令行字节组
        /// </summary>
        public byte[] ToBytes() => this.ToString().GetBytes();
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="stream">流</param>
        public void SendCommand(NetworkStream stream)
        {
            if (stream == null || !stream.CanWrite) return;
            var lines = this.ToBytes();
            stream.Write(lines, 0, lines.Length);
            stream.Write(this.EOF, 0, this.EOF.Length);

            if (this.Value != null)
            {
                stream.Write(this.Value, 0, this.Value.Length);
                stream.Write(this.EOF, 0, this.EOF.Length);
            }
            stream.Flush();
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="stream">流</param>
        public async Task SendCommandAsync(NetworkStream stream)
        {
            if (stream == null || !stream.CanWrite) return;
            var lines = this.ToBytes();
            await stream.WriteAsync(lines, 0, lines.Length).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }
        #endregion
    }
}