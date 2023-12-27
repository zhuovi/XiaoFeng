using System;
using System.Collections.Generic;
using System.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 17:18:43                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流读取器
    /// </summary>
    public class MemoryBufferReader : MemoryBufferBase, IMemoryBufferReader
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        /// <param name="offset">buffer 内的索引，流从此处开始。</param>
        /// <param name="count">流的长度（以字节为单位）。</param>
        public MemoryBufferReader(byte[] buffer, int offset, int count)
        {
            this.BufferData = new System.IO.MemoryStream();
            this.BufferData.Write(buffer, offset, count);
            this.SeekFirst();
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        public MemoryBufferReader(byte[] buffer) : this(buffer, 0, buffer.Length) { }
        #endregion

        #region 属性
        /// <summary>
        /// 剩余未读长度
        /// </summary>
        public long RemainingLength => this.Length - this.Position;
        #endregion

        #region 方法

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

        #region 往读取器中写入数据
        /// <summary>
        /// 往读取器中写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public void WriteBuffer(byte[] buffer)
        {
            this.WriteBuffer(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// 往读取器中写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">开始位置</param>
        /// <param name="length">长度</param>
        public void WriteBuffer(byte[] buffer, int offset, int length)
        {
            var index = this.Position;
            this.SeekLast();
            this.BufferData.Write(buffer, offset, length);
            this.Position = index;
        }
        /// <summary>
        /// 把写入器数据写入当前读取器
        /// </summary>
        /// <param name="writer">写入器</param>
        public void WriteBuffer(IMemoryBufferWriter writer)
        {
            this.WriteBuffer(writer.ToArray());
        }
        #endregion

        #region 读取一行数据
        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadLine()
        {
            var IsR = false;
            var ms = new MemoryStream();
            while (!this.EndOfStream)
            {
                var c = this.ReadByte();
                if (c == 13)
                {
                    IsR = true;
                }
                else if (c == 10)
                {
                    if (IsR)
                    {
                        IsR = false;
                        break;
                    }
                    else
                    {
                        ms.WriteByte((byte)c);
                        IsR = false;
                    }
                }
                else
                {
                    if (IsR)
                    {
                        ms.WriteByte(13);
                        IsR = false;
                    }
                    ms.WriteByte((byte)c);
                }
            }
            return ms.ToArray();
        }
        #endregion

        #endregion
    }
}