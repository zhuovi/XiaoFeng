using System.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 17:24:04                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流基类接口
    /// </summary>
    public interface IMemoryBufferBase
    {
        /// <summary>
        /// 读取位置
        /// </summary>
        long Position { get; set; }
        /// <summary>
        /// 包长度
        /// </summary>
        long Length { get; }
        /// <summary>
        /// 是否是结束
        /// </summary>
        bool EndOfStream { get; }
        /// <summary>
        /// 将当前流的位置设置为指定值
        /// </summary>
        /// <param name="offset">流内的新位置。 它是相对于 origin 参数的位置，而且可正可负。</param>
        /// <param name="origin">类型 System.IO.SeekOrigin 的值，它用作查找引用点。</param>
        /// <returns>流内的新位置，通过将初始引用点和偏移量合并计算而得。</returns>
        long Seek(long offset, SeekOrigin origin);
        /// <summary>
        /// 移位到最开始
        /// </summary>
        void SeekFirst();
        /// <summary>
        /// 移位到结束
        /// </summary>
        void SeekLast();
        /// <summary>
        /// 向前移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        long Prev(ulong offset = 1);
        /// <summary>
        /// 向后移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        long Next(ulong offset = 1);
        /// <summary>
        /// 清空流
        /// </summary>
        void Clear();
        /// <summary>
        /// 创建此流的无符号字节的数组。
        /// </summary>
        byte[] GetBuffer();
        /// <summary>
        /// 将流内容写入字节数组，而与 Position 属性无关。
        /// </summary>
        /// <returns></returns>
        byte[] ToArray();
        /// <summary>
        /// 字节转数值
        /// </summary>
        /// <param name="values">字节组</param>
        /// <returns></returns>
        long BytesToValue(byte[] values);
        /// <summary>
        /// 数值转字节
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="length">字节数组长度</param>
        /// <returns></returns>
        byte[] ValueToBytes(long val, int length);
    }
}