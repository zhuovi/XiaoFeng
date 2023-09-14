using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-13 17:36:14                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /// <summary>
    /// 命令包
    /// </summary>
    public class CommandPacket
    {
        #region 构造器
        public CommandPacket(CommandOpcode opcode, string key, object value, string opaque, string cas)
        {
            this.Magic = MagicType.Request;
            this.Opcode = opcode;
            this.Opaque = opaque;
            this.Cas = cas;
            var writer = new MemoryStream();
            writer.WriteByte((byte)this.Magic);
            writer.WriteByte((byte)this.Opcode);
            writer.Write(key.Length.ToString("X").PadLeft(4, '0').HexStringToBytes(), 0, 2);
            writer.WriteByte((byte)opcode);
            writer.WriteByte((byte)this.DataType);
            writer.Write(new byte[] { 0x00, 0x00 }, 0, 2);
            var val = Helper.Serialize(value, out var type, 1024 * 1024);
            writer.Write(new byte[] { 0x00, 0x00 }, 0, 2);
            writer.Write(val.Length.ToString("X").PadLeft(4, '0').HexStringToBytes(), 0, 4);
            var Opaques = new byte[4];
            var oBytes = this.Opaque.GetBytes();
            Array.Copy(oBytes, 0, Opaques, 4 - oBytes.Length, oBytes.Length);
            writer.Write(Opaques, 0, 4);

            var Cass = new byte[8];
            var cBytes = this.Cas.GetBytes();
            Array.Copy(cBytes, 0, Cass, 8 - oBytes.Length, cBytes.Length);
            writer.Write(Cass, 0, 8);
            var body = (key + val).GetBytes();
            writer.Write(body, 0, body.Length);
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="buffers">响应数字</param>
        public CommandPacket(byte[] buffers)
        {
            if (buffers.Length < 20) return;
            var reader = new MemoryStream(buffers);
            this.Magic = reader.ReadByte().ToEnum<MagicType>();
            this.Opcode = reader.ReadByte().ToEnum<CommandOpcode>();
            var Keys = new byte[2];
            reader.Read(Keys, 0, 2);
            this.KeyLength = ToValue<int>(Keys);
            this.ExtrasLength = reader.ReadByte();
            this.DataType = reader.ReadByte();
            this.Status = reader.ReadByte().ToEnum<ResponseStatus>();
            var Bodys = new byte[4];
            reader.Read(Bodys, 0, 4);
            this.TotalBodyLength = ToValue<long>(Bodys);
            var Opaques = new byte[4];
            reader.Read(Opaques, 0, 4);
            this.Opaque = Opaques.GetString();
            var Cas = new byte[8];
            reader.Read(Cas, 0, 8);
            this.Cas = Cas.GetString();
            this.PayLoad = buffers.GetString(Encoding.UTF8, (int)reader.Position);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 魔法数字，用来区分包头是请求包头还是响应包头
        /// </summary>
        public MagicType Magic { get; set; }
        /// <summary>
        /// 操作符，也就是对应的命令
        /// </summary>
        public CommandOpcode Opcode { get; set; }
        /// <summary>
        /// key的长度
        /// </summary>
        public int KeyLength { get; set; }
        /// <summary>
        /// 保留字段 目前只有一个固定的值：0x00
        /// </summary>
        public int DataType { get; set; } = 0x00;
        /// <summary>
        /// 命令对应的virtual bucket
        /// </summary>
        public long VBucketId { get; set; }
        /// <summary>
        /// 请求响应response的状态
        /// </summary>
        public ResponseStatus Status { get; set; }
        /// <summary>
        /// command extras的长度
        /// </summary>
        public int ExtrasLength { get; set; }
        /// <summary>
        /// extra + key + value的总长度
        /// </summary>
        public long TotalBodyLength { get; set; }
        /// <summary>
        /// 请求生成的一个数据，会被原封不动在对应的响应中返回
        /// </summary>
        public string Opaque { get; set; }
        /// <summary>
        /// 数据的一个唯一标记
        /// </summary>
        public string Cas { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public string PayLoad { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 转换整型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bytes">字节组</param>
        /// <returns></returns>
        private T ToValue<T>(byte[] bytes)
        {
            var hexs = "";
            for (var i = 0; i < bytes.Length; i++)
                hexs += bytes[i].ToString("X");
            return XiaoFeng.MathHelper.StringToDecimal(hexs, 16).ToCast<T>();
        }
        /// <summary>
        /// 打包
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            var writer = new MemoryStream();
            writer.WriteByte((byte)this.Magic);
            writer.WriteByte((byte)this.Opcode);
            return Array.Empty<byte>();
        }
        #endregion
    }
}