using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Redis.IO;

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
namespace XiaoFeng.Redis
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
        /// <param name="datas">参数集</param>
        public CommandPacket(CommandType commandType, params object[] datas)
        {
            this.CommandType = commandType;
            this.Datas = datas;
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
        #endregion

        #region 方法
        /// <summary>
        /// 命令行
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var SubCommands = this.CommandType.Commands;
            var line = $"*{this.Datas.Length + (SubCommands != null && SubCommands.Length > 0 ? SubCommands.Length : 1)}\r\n";
            if (SubCommands != null && SubCommands.Length > 0)
                SubCommands.Each(a => line += $"${a.Length}\r\n{a}\r\n");
            else
                line += $"${this.CommandType.ToString().Length}\r\n{this.CommandType}\r\n";
            this.Datas.Each(d => line += $"${Encoding.UTF8.GetByteCount(d.ToString())}\r\n{d}\r\n");
            return line;
        }
        /// <summary>
        /// 命令行字节组
        /// </summary>
        public byte[] ToBytes() => this.ToString().GetBytes();
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="redis">socket</param>
        public void SendCommand(IRedisSocket redis)
        {
            if (redis == null) return;
            if (!redis.IsConnected) redis.Connect();
            if (!redis.IsConnected) return;
            var stream = redis.GetStream() as NetworkStream;
            if (stream == null || !stream.CanWrite) return;
            var lines = this.ToBytes();
            stream.Write(lines, 0, lines.Length);
            stream.Flush();
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="redis">socket</param>
        public async Task SendCommandAsync(IRedisSocket redis)
        {
            if (redis == null) return;
            if (!redis.IsConnected) redis.Connect();
            if (!redis.IsConnected) return;
            var stream = redis.GetStream() as NetworkStream;
            if (stream == null || !stream.CanWrite) return;
            var lines = this.ToBytes();
            await stream.WriteAsync(lines, 0, lines.Length).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }
        #endregion
    }
}