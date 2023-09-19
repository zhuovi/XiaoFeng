using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Memcached.Internal;
using XiaoFeng.Memcached.IO;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-14 13:33:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /*
     * 请求包格式
     Byte/     0       |       1       |       2       |       3       |
        /              |               |               |               |
       |0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|
       +---------------+---------------+---------------+---------------+
      0| Magic         | Opcode        | Key length                    |
       +---------------+---------------+---------------+---------------+
      4| Extras length | Data type     | Reserved                      |
       +---------------+---------------+---------------+---------------+
      8| Total body length                                             |
       +---------------+---------------+---------------+---------------+
     12| Opaque                                                        |
       +---------------+---------------+---------------+---------------+
     16| CAS                                                           |
       |                                                               |
       +---------------+---------------+---------------+---------------+
       Total 24 bytes

    ---------------------------------------------------------------------
    Magic               魔法数字，用来区分包头是请求包头还是响应包头
    Opcode              操作码命令码，也就是对应的命令
    Key Length          键长度，附加命令后面的文本键的长度（以字节为单位）
    Extras length       附加命令的长度（以字节为单位）
    Data type           保留以备将来使用（很快会使用）
    Reserved            保留以备将来使用（可供选择）
    Total body length   正文总长度,额外+键+值的长度（以字节为单位）。
    Opaque              将在回复中复制回您。
    CAS                 数据版本检查。
     */
    /// <summary>
    /// 请求包
    /// </summary>
    public class RequestPacket : BasePacket
    {
        #region 构造器
        /// <summary>
        /// 设置请求包
        /// </summary>
        /// <param name="memcachedSocket">socket</param>
        /// <param name="memcachedConfig">配置</param>
        /// <param name="opaque">附加数据</param>
        public RequestPacket(ISocketClient memcachedSocket, Internal.MemcachedConfig memcachedConfig, string opaque)
        {
            this.Magic = MagicType.Request;
            this.MemcachedSocket = memcachedSocket;
            this.Config = memcachedConfig;
            this.Opaque = opaque.GetBytes();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 保留以备将来使用（可供选择）
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[2] { 0x00, 0x00 };
        #endregion

        #region 方法
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> Quit()
        {
            this.Opcode = CommandOpcode.Quit;
            var response = await this.GetResponseAsync().ConfigureAwait(false);
            return response.Status == ResponseStatus.Success;
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> Noop()
        {
            this.Opcode = CommandOpcode.Noop;
            var response = await this.GetResponseAsync().ConfigureAwait(false);
            return response.Status == ResponseStatus.Success;
        }
        /// <summary>
        /// 版本
        /// </summary>
        /// <returns></returns>
        public async Task<string> Version()
        {
            this.Opcode = CommandOpcode.Version;
            var response = await this.GetResponseAsync().ConfigureAwait(false);
            return response.Status == ResponseStatus.Success ? response.Value.GetString() : string.Empty;
        }
        /// <summary>
        /// 获取响应值
        /// </summary>
        /// <returns></returns>
        public async Task<MemcachedValue> GetResponseValueAsync()
        {
            var response = await this.GetResponseAsync().ConfigureAwait(false);

            if (response.Status == ResponseStatus.Success)
            {
                ValueType valueType;
                object val;
                if (Opcode == CommandOpcode.Increment || Opcode == CommandOpcode.Decrement)
                {
                    val = this.ToValue(response.Value);
                    valueType = ValueType.Int;
                }
                else
                {
                    valueType = this.ToValue(response.Extras).ToEnum<ValueType>();
                    val = Helper.Deserialize(response.Value, valueType);
                }
                return await Task.FromResult(new MemcachedValue(this.Key.GetString(), valueType, val, (ulong)response.ToValue(response.CAS)));
            }
            else
                return await Task.FromResult(default(MemcachedValue));
        }
        /// <summary>
        /// 获取响应
        /// </summary>
        /// <returns></returns>
        public async Task<ResponsePacket> GetResponseAsync()
        {
            var bytes = this.Packet();
            await this.MemcachedSocket.SendAsync(bytes).ConfigureAwait(false);
            var responseBytes = await this.MemcachedSocket.ReceviceMessageAsync().ConfigureAwait(false);
            var response = new ResponsePacket(responseBytes);
            return await Task.FromResult(response);
        }

        /// <summary>
        /// 打包
        /// </summary>
        /// <returns></returns>
        private byte[] Packet()
        {
            var writer = new MemoryStream();

            writer.WriteByte((byte)this.Magic);

            writer.WriteByte((byte)this.Opcode);

            if (this.Key == null) this.Key = Array.Empty<byte>();
            writer.Write(ToBytes(this.Key.Length, 2), 0, 2);

            var TotalBodyLength = this.Key.Length;

            var ExtrasLength = 0;
            if (this.Extras != null)
            {
                ExtrasLength = this.Extras.Length;
            }
            writer.WriteByte((byte)ExtrasLength);

            TotalBodyLength += ExtrasLength;

            if (Value != null && Value.Length > 0)
            {
                TotalBodyLength += Value.Length;
            }

            writer.WriteByte((byte)this.DataType);

            writer.Write(this.Reserved, 0, 2);

            writer.Write(ToBytes(TotalBodyLength, 4), 0, 4);

            var Opaques = new byte[4];

            if (this.Opaque.Length > 4)
                Array.Copy(this.Opaque, 0, Opaques, 0, 4);
            else
                Array.Copy(this.Opaque, 0, Opaques, 4 - this.Opaque.Length, this.Opaque.Length);

            writer.Write(Opaques, 0, 4);

            writer.Write(this.CAS, 0, 8);

            if (this.Extras != null && this.Extras.Length > 0)
                writer.Write(this.Extras, 0, this.Extras.Length);

            if (this.Key.Length > 0)
                writer.Write(this.Key, 0, this.Key.Length);

            if (this.Value != null && this.Value.Length > 0)
                writer.Write(this.Value, 0, this.Value.Length);

            return writer.ToArray();
        }
        
        #endregion
    }
}