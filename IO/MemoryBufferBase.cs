using System;
using System.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 17:20:55                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流基类
    /// </summary>
    public class MemoryBufferBase : IMemoryBufferBase
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemoryBufferBase() { }
        /// <summary>
        /// 内存流
        /// </summary>
        protected MemoryStream BufferData { get; set; }
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
        #endregion
    }
}