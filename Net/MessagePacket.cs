using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-17 10:18:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 消息包的编码及解码
    /// </summary>
    public class MessagePacket : Disposable, IMessagePacket,INetPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MessagePacket() : this(Array.Empty<byte>()) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public MessagePacket(byte[] buffer)
        {
            this.Data = new MemoryStream(buffer);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据包队列
        /// </summary>
        public static ConcurrentQueue<IMessagePacket> Packets { get; set; }
        /// <summary>
        /// 最大可变长度
        /// </summary>
        public ulong VariableByteIntegerMaxValue { get; set; } = 0xFFFFFF7F;
        /// <summary>
        /// 数据
        /// </summary>
        private MemoryStream Data { get; set; }
        /// <summary>
        /// 读取位置
        /// </summary>
        public long Position => (long)this.Data?.Position;
        /// <summary>
        /// 包长度
        /// </summary>
        public long Length => (long)this.Data?.Length;
        /// <summary>
        /// 是否是结束
        /// </summary>
        public bool EndOfStream => this.Length == this.Position;
        #endregion

        #region 方法

        #region 将当前流的位置设置为指定值
        /// <summary>
        /// 将当前流的位置设置为指定值
        /// </summary>
        /// <param name="offset">流内的新位置。 它是相对于 origin 参数的位置，而且可正可负。</param>
        /// <param name="origin">类型 System.IO.SeekOrigin 的值，它用作查找引用点。</param>
        /// <returns>流内的新位置，通过将初始引用点和偏移量合并计算而得。</returns>
        public long Seek(long offset, SeekOrigin origin) => this.Data.Seek(offset, origin);
        #endregion

        #region 移位
        /// <summary>
        /// 向前移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        public long Prev(ulong offset = 1) => this.Seek(-(long)offset, SeekOrigin.Current);
        /// <summary>
        /// 向后移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        public long Next(ulong offset = 1) => this.Seek((long)offset, SeekOrigin.Current);
        /// <summary>
        /// 重置流的当前位置为0
        /// </summary>
        public void Reset() => this.Seek(0, SeekOrigin.Begin);
        #endregion

        #region 清空流
        /// <summary>
        /// 清空流
        /// </summary>
        public void Clear() => this.Data = new MemoryStream();
        #endregion

        #region 创建此流的无符号字节的数组
        /// <summary>
        /// 创建此流的无符号字节的数组。
        /// </summary>
        public byte[] GetBuffer() => this.Data == null ? Array.Empty<byte>() : this.Data.GetBuffer();
        #endregion

        #region 将流内容写入字节数组，而与 Position 属性无关。
        /// <summary>
        /// 将流内容写入字节数组，而与 Position 属性无关。
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() => this.Data == null ? Array.Empty<byte>() : this.Data.ToArray();
        #endregion

        #region 读一个字节
        /// <summary>
        /// 读一个字节
        /// </summary>
        /// <returns></returns>
        public int ReadByte() => this.Data.ReadByte();
        #endregion

        #region 读取指定长度字节
        /// <summary>
        /// 读取指定长度字节
        /// </summary>
        /// <param name="length">长度 长度为0时读取剩下的所有字节</param>
        /// <returns></returns>
        public byte[] ReadBytes(long length)
        {
            if (length <= 0) length = this.Length - this.Position;
            if (length > this.Length - this.Position) length = this.Length - this.Position;
            var bytes = new byte[length];
            this.Data.Read(bytes, 0, (int)length);
            return bytes;
        }
        #endregion

        #region 读取剩余所有字节
        /// <summary>
        /// 读取剩余所有字节
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes() => this.ReadBytes(0);
        #endregion

        #region 设置缓存
        /// <summary>
        /// 把当前流数据设置为当前buffer
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public void SetBuffer(byte[] buffer)
        {
            this.Data = new MemoryStream(buffer);
        }
        /// <summary>
        /// 附加缓存
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public void AppendBuffer(byte[] buffer)
        {
            this.WriteBytes(buffer);
        }
        #endregion

        #region 写字节
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        public void WriteByte(byte @byte) => this.Data.WriteByte(@byte);
        /// <summary>
        /// 写字符组
        /// </summary>
        /// <param name="bytes">字节组</param>
        public void WriteBytes(byte[] bytes) => this.Data.Write(bytes, 0, bytes.Length);
        #endregion

        #region 把数据包数据写入到当前包中
        /// <summary>
        /// 把数据包数据写入到当前包中
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public long WritePacket(IMessagePacket packet)
        {
            if (packet == null || packet.Length == 0) return 0;
            this.WriteBytes(packet.ToArray());
            return packet.Length;
        }
        #endregion

        #region 获取可变字节长度占用字节数
        /// <summary>
        /// 获取可变字节长度占用字节数
        /// </summary>
        /// <param name="length">可变字节长度</param>
        /// <returns></returns>
        /// <exception cref="Exception">超过指定长度则抛出异常</exception>
        public int GetVariableByteIntegerSize(ulong length)
        {
            if (length < 0x7F) return 1;
            if (length < 0xFF7F) return 2;
            if (length < 0xFFFF7F) return 3;
            if (length < 0xFFFFFF7F) return 4;
            if (length > VariableByteIntegerMaxValue) throw new Exception($"指定的值( {length} )对于可变字节整数来说太大.");
            return 5;
        }
        #endregion

        #region 写可变字节长度
        /// <summary>
        /// 获取可变字节长度字节数据
        /// </summary>
        /// <param name="length">值</param>
        /// <exception cref="Exception">异常</exception>
        public void WriteVariableByteInteger(ulong length)
        {
            if (length < 128)
            {
                this.WriteByte((byte)length);
                return;
            }
            if (length > VariableByteIntegerMaxValue) throw new Exception($"指定的值( {length} )对于可变字节整数来说太大.");

            var x = length;
            do
            {
                var encodedByte = x % 128;
                x /= 128;
                if (x > 0)
                {
                    encodedByte |= 128;
                }
                this.WriteByte((byte)encodedByte);
            } while (x > 0);
        }
        #endregion

        #region 读取可变小字节整型数字
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">异常</exception>
        public ulong ReadVariableByteInteger()
        {
            var multiplier = 1UL;
            var value = 0UL;
            byte encodedByte;

            do
            {
                encodedByte = (byte)this.ReadByte();
                value += (ulong)((ulong)(encodedByte & 127) * multiplier);
                if (multiplier > (128 * 128 * 128))
                {
                    throw new Exception("无效长度.");
                }
                multiplier *= 128;
            } while ((encodedByte & 128) != 0);

            return value;
        }
        #endregion

        #region 字节转数值
        /// <summary>
        /// 字节转数值
        /// </summary>
        /// <param name="values">字节组</param>
        /// <returns></returns>
        public long BytesToValue(byte[] values)
        {
            if (values == null || values.Length == 0) return 0;
            var hexs = "";
            for (var i = 0; i < values.Length; i++)
                hexs += values[i].ToString("X");
            return Convert.ToInt64(hexs, 16);
        }
        #endregion

        #region 数值转字节
        /// <summary>
        /// 数值转字节
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="length">字节数组长度</param>
        /// <returns></returns>
        public byte[] ValueToBytes(long val, int length)
        {
            var bytes = new byte[length];
            for (var i = length - 1; i >= 0; i--)
            {
                bytes[length - 1 - i] = i == 0 ? (byte)(val & 0xff) : (byte)(val >> (i * 8));
            }
            return bytes;
        }
        #endregion

        #region 编码
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public virtual byte[] Encode(byte[] bytes) => Array.Empty<byte>();
        #endregion

        #region 解码
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public virtual byte[] Decode(byte[] bytes) => Array.Empty<byte>();
        #endregion
        /// <summary>
        /// 是否是完整包
        /// </summary>
        /// <returns></returns>
        public virtual bool IsPacket() => true;
        #endregion
    }
}