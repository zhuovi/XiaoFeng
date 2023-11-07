using System;
using System.Collections.Generic;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 17:18:20                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流写入器
    /// </summary>
    public class MemoryBufferWriter : MemoryBufferBase, IMemoryBufferWriter
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemoryBufferWriter()
        {
            this.BufferData = new System.IO.MemoryStream();
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        /// <param name="offset">buffer 内的索引，流从此处开始。</param>
        /// <param name="count">流的长度（以字节为单位）。</param>
        public MemoryBufferWriter(byte[] buffer, int offset, int count)
        {
            this.BufferData = new System.IO.MemoryStream();
            this.WriteBytes(buffer, offset, count);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">从中创建此流的无符号字节的数组。</param>
        public MemoryBufferWriter(byte[] buffer) : this(buffer, 0, buffer.Length) { }
        #endregion

        #region 属性
        /// <summary>
        /// 最大可变长度
        /// </summary>
        public ulong VariableByteIntegerMaxValue { get; set; } = 0xFFFFFF7F;
        #endregion

        #region 方法

        #region 写字节
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        public void WriteByte(byte @byte) => this.BufferData.WriteByte(@byte);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes">数据</param>
        public void WriteBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;
            this.WriteBytes(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="offset">开始位置</param>
        /// <param name="length">长度</param>
        public void WriteBytes(byte[] bytes, int offset, int length)
        {
            this.BufferData.Write(bytes, offset, length);
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

        #region 把写入器写入当前写入器
        /// <summary>
        /// 把写入器写入当前写入器
        /// </summary>
        /// <param name="writer">写入器</param>
        public void WriteBuffer(IMemoryBufferWriter writer)
        {
            this.WriteBytes(writer.ToArray());
        }
        /// <summary>
        /// 把读取器剩下数据写入当前写入器
        /// </summary>
        /// <param name="reader">读取器</param>
        public void WriteBuffer(IMemoryBufferReader reader)
        {
            this.WriteBytes(reader.ReadBytes());
        }
        #endregion

        #endregion
    }
}