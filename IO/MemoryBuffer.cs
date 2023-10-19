using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 14:40:10                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流
    /// </summary>
    public class MemoryBuffer : Disposable, IMemoryBuffer
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemoryBuffer() => this.BufferData = new MemoryStream();
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        /// <param name="offset">buffer 内的索引，流从此处开始。</param>
        /// <param name="length">流的长度（以字节为单位）。</param>
        /// <param name="writable">该流是否支持写入</param>
        /// <param name="publiclyVisible">设置为 true 可以启用 XiaoFeng.IO.MemoryBuffer.GetBuffer，它返回无符号字节数组，流从该数组创建；否则为 false。</param>
        public MemoryBuffer(byte[] buffer, int offset, int length, bool writable, bool publiclyVisible) => this.BufferData = new MemoryStream(buffer, offset, length, writable, publiclyVisible);
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        public MemoryBuffer(byte[] buffer) : this(buffer, 0, buffer.Length) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        /// <param name="offset">buffer 内的索引，流从此处开始。</param>
        /// <param name="length">流的长度（以字节为单位）。</param>
        public MemoryBuffer(byte[] buffer, int offset, int length) : this(buffer, offset, length, false, true) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        /// <param name="writable">该流是否支持写入</param>
        public MemoryBuffer(byte[] buffer, bool writable):this(buffer,0,buffer.Length,writable) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        /// <param name="offset">buffer 内的索引，流从此处开始。</param>
        /// <param name="length">流的长度（以字节为单位）。</param>
        /// <param name="writable">该流是否支持写入</param>
        public MemoryBuffer(byte[] buffer, int offset, int length, bool writable) : this(buffer, offset, length, writable, true) { }
        #endregion

        #region 属性
        /// <summary>
        /// 内存流数据
        /// </summary>
        private MemoryStream BufferData { get; set; }
        /// <summary>
        /// 最大可变长度
        /// </summary>
        public ulong VariableByteIntegerMaxValue { get; set; } = 0xFFFFFF7F;
        /// <summary>
        /// 读取位置
        /// </summary>
        public long Position
        {
            get => (long)this.BufferData?.Position;
            set => this.BufferData.Position = value;
        }
        /// <summary>
        /// 包长度
        /// </summary>
        public long Length => (long)this.BufferData?.Length;
        /// <summary>
        /// 是否是结束
        /// </summary>
        public bool EndOfStream => this.Length == this.Position;
        /// <summary>
        /// 剩余未读长度
        /// </summary>
        public long RemainingLength => this.Length - this.Position;
        #endregion

        #region 方法

        #region 将当前流的位置设置为指定值
        /// <summary>
        /// 将当前流的位置设置为指定值
        /// </summary>
        /// <param name="offset">流内的新位置。 它是相对于 origin 参数的位置，而且可正可负。</param>
        /// <param name="origin">类型 System.IO.SeekOrigin 的值，它用作查找引用点。</param>
        /// <returns>流内的新位置，通过将初始引用点和偏移量合并计算而得。</returns>
        public long Seek(long offset, SeekOrigin origin) => this.BufferData.Seek(offset, origin);
        #endregion

        #region 移位
        /// <summary>
        /// 移位到最开始
        /// </summary>
        public void SeekFirst() => this.Seek(0, SeekOrigin.Begin);
        /// <summary>
        /// 移位到结束
        /// </summary>
        public void SeekLast() => this.Seek(0, SeekOrigin.End);
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
        #endregion

        #region 清空流
        /// <summary>
        /// 清空流
        /// </summary>
        public void Clear() => this.BufferData = new MemoryStream();
        #endregion

        #region 创建此流的无符号字节的数组
        /// <summary>
        /// 创建此流的无符号字节的数组。
        /// </summary>
        public byte[] GetBuffer() => this.BufferData == null ? Array.Empty<byte>() : this.BufferData.GetBuffer();
        #endregion

        #region 将流内容写入字节数组，而与 Position 属性无关。
        /// <summary>
        /// 将流内容写入字节数组，而与 Position 属性无关。
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() => this.BufferData == null ? Array.Empty<byte>() : this.BufferData.ToArray();
        #endregion

        #region 向当前内存流写入数据
        /// <summary>
        /// 向当前内存流写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">位置</param>
        /// <param name="count">长度</param>
        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            if (this.BufferData.CanWrite)
                this.BufferData.Write(buffer, offset, count);
            else
            {
                var position = this.Position;
                this.BufferData = new MemoryStream(this.ToArray(), true);
                this.BufferData.Write(buffer, offset, count);
                this.Position = position;
            }
        }
        /// <summary>
        /// 向当前内存流写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public void WriteBuffer(byte[] buffer) => this.WriteBuffer(buffer, 0, buffer.Length);
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

        #region 写字节
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        public void WriteByte(byte @byte) => this.BufferData.WriteByte(@byte);
        /// <summary>
        /// 写字符组
        /// </summary>
        /// <param name="bytes">字节组</param>
        public void WriteBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;
            this.BufferData.Write(bytes, 0, bytes.Length);
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
        public byte[] EncodeVariableByteInteger(ulong length)
        {
            var bytes = new List<byte>();
            if (length < 128)
            {
                bytes.Add((byte)length);
                return bytes.ToArray();
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
                bytes.Add((byte)encodedByte);
            } while (x > 0);
            return bytes.ToArray();
        }
        #endregion

        #region 读取可变小字节整型数字
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        public uint DecodeVariableByteInteger()
        {
            return this.DecodeVariableByteInteger(out var _);
        }
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">异常</exception>
        public uint DecodeVariableByteInteger(out int size)
        {
            var multiplier = 1U;
            var value = 0U;
            byte encodedByte;
            size = 0;
            do
            {
                encodedByte = (byte)this.ReadByte();
                value += (uint)((ulong)(encodedByte & 127) * multiplier);
                if (multiplier > (128 * 128 * 128))
                {
                    throw new Exception("无效长度.");
                }
                multiplier *= 128;
                size++;
            } while ((encodedByte & 128) != 0 && size < 4);

            return value;
        }
        #endregion

        #region 读取字节
        /// <summary>
        /// 从当前流中读取一个字节
        /// </summary>
        /// <returns></returns>
        public int ReadByte() => this.BufferData.ReadByte();
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
            if (length == 0) return Array.Empty<byte>();
            var bytes = new byte[length];
            this.BufferData.Read(bytes, 0, (int)length);
            return bytes;
        }
        /// <summary>
        /// 读取剩余所有数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes() => ReadBytes(this.Length - this.Position);
        /// <summary>
        /// 读取2个字节整型数字
        /// </summary>
        /// <returns></returns>
        public ushort ReadTwoByteInteger()
        {
            var msb = this.ReadByte();
            var lsb = this.ReadByte();
            return (ushort)(msb << 8 | lsb);
        }
        /// <summary>
        /// 读取4个字节整型数字
        /// </summary>
        /// <returns></returns>
        public uint ReadFourByteInteger()
        {
            var byte0 = this.ReadByte();
            var byte1 = this.ReadByte();
            var byte2 = this.ReadByte();
            var byte3 = this.ReadByte();

            return (uint)((byte0 << 24) | (byte1 << 16) | (byte2 << 8) | byte3);
        }
        #endregion

        #endregion
    }
}