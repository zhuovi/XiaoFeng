using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-17 10:19:14                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 消息包的编码及解码接口
    /// </summary>
    public interface IMessagePacket
    {
        #region 属性
        /// <summary>
        /// 读取位置
        /// </summary>
        long Position { get; }
        /// <summary>
        /// 包长度
        /// </summary>
        long Length { get; }
        /// <summary>
        /// 是否是结束
        /// </summary>
        bool EndOfStream { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 将当前流的位置设置为指定值
        /// </summary>
        /// <param name="offset">流内的新位置。 它是相对于 origin 参数的位置，而且可正可负。</param>
        /// <param name="origin">类型 System.IO.SeekOrigin 的值，它用作查找引用点。</param>
        /// <returns>流内的新位置，通过将初始引用点和偏移量合并计算而得。</returns>
        long Seek(long offset, SeekOrigin origin);
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
        /// 重置流的当前位置为0
        /// </summary>
        void Reset();
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
        /// 读一个字节
        /// </summary>
        /// <returns></returns>
        int ReadByte();
        /// <summary>
        /// 读取指定长度字节
        /// </summary>
        /// <param name="length">长度 长度为0时读取剩下的所有字节</param>
        /// <returns></returns>
        byte[] ReadBytes(long length);
        /// <summary>
        /// 读取剩余所有字节
        /// </summary>
        /// <returns></returns>
        byte[] ReadBytes();
        /// <summary>
        /// 把当前流数据设置为当前buffer
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        void SetBuffer(byte[] buffer);
        /// <summary>
        /// 附加缓存
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        void AppendBuffer(byte[] buffer);
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        void WriteByte(byte @byte);
        /// <summary>
        /// 写字符组
        /// </summary>
        /// <param name="bytes">字节组</param>
        void WriteBytes(byte[] bytes);
        /// <summary>
        /// 把数据包数据写入到当前包中
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        long WritePacket(IMessagePacket packet);
        /// <summary>
        /// 获取可变字节长度占用字节数
        /// </summary>
        /// <param name="length">可变字节长度</param>
        /// <returns></returns>
        /// <exception cref="Exception">超过指定长度则抛出异常</exception>
        int GetVariableByteIntegerSize(ulong length);
        /// <summary>
        /// 获取可变字节长度字节数据
        /// </summary>
        /// <param name="length">值</param>
        /// <exception cref="Exception">异常</exception>
        void WriteVariableByteInteger(ulong length);
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">异常</exception>
        ulong ReadVariableByteInteger();
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
        #endregion
    }
}