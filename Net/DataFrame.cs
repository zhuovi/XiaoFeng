using System;
using System.Collections.Generic;
using System.Text;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /*
   详见<5.2 基本帧协议>和<11.8.WebSocket 操作码注册>
    0                   1                   2                   3
    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    +-+-+-+-+-------+-+-------------+-------------------------------+
    |F|R|R|R| opcode|M| Payload len |    Extended payload length    |
    |I|S|S|S|  (4)  |A|     (7)     |             (16/64)           |
    |N|V|V|V|       |S|             |   (if payload len==126/127)   |
    | |1|2|3|       |K|             |                               |
    +-+-+-+-+-------+-+-------------+ - - - - - - - - - - - - - - - +
    |     Extended payload length continued, if payload len == 127  |
    + - - - - - - - - - - - - - - - +-------------------------------+
    |                               |Masking-key, if MASK set to 1  |
    +-------------------------------+-------------------------------+
    | Masking-key (continued)       |          Payload Data         |
    +-------------------------------- - - - - - - - - - - - - - - - +
    :                     Payload Data continued ...                :
    + - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - +
    |                     Payload Data continued ...                |
    +---------------------------------------------------------------+
*/
    /// <summary>
    /// 数据帧头,就是包头结构
    /// </summary>
    public class DataFrame
    {
        #region 属性
        /// <summary>
        /// opcode
        /// </summary>
        public OpCode Opcode { get; set; } = OpCode.Text;
        /// <summary>
        /// 帧头
        /// </summary>
        DataFrameHeader _Header;
        /// <summary>
        /// 标识
        /// </summary>
        private byte[] _Extend = new byte[0];
        /// <summary>
        /// 0或4个字节，客户端发送给服务端的数据，都是通过内嵌的一个32位值作为掩码的；掩码键只有在掩码位设置为1的时候存在。
        /// </summary>
        private byte[] _Mask = new byte[0];
        /// <summary>
        /// 内容
        /// </summary>
        private byte[] _Content = new byte[0];
        /// <summary>
        /// Socket 数据类型
        /// </summary>
        public SocketDataType DataType { get; set; } = SocketDataType.String;
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        #endregion

        #region 方法

        #region 设置消息
        /// <summary>
        /// 设置消息
        /// </summary>
        /// <param name="buffer">字节</param>
        public DataFrame(byte[] buffer)
        {
            //帧头
            _Header = new DataFrameHeader(buffer);
            //扩展长度
            if (_Header.Length == 126)
            {
                _Extend = new byte[2];
                Buffer.BlockCopy(buffer, 2, _Extend, 0, 2);
            }
            else if (_Header.Length == 127)
            {
                _Extend = new byte[8];
                Buffer.BlockCopy(buffer, 2, _Extend, 0, 8);
            }
            //是否有掩码
            if (_Header.HasMask)
            {
                _Mask = new byte[4];
                Buffer.BlockCopy(buffer, _Extend.Length + 2, _Mask, 0, 4);
            }
            //消息体
            if (_Extend.Length == 0)
            {
                _Content = new byte[_Header.Length];
                Buffer.BlockCopy(buffer, _Extend.Length + _Mask.Length + 2 , _Content, 0, _Content.Length);
            }
            else if (_Extend.Length == 2)
            {
                int contentLength = (int)_Extend[0] * 256 + (int)_Extend[1];
                _Content = new byte[contentLength];
                Buffer.BlockCopy(buffer, _Extend.Length + _Mask.Length + 2, _Content, 0, contentLength > 1024 * 100 ? 1024 * 100 : contentLength);
            }
            else
            {
                long len = 0;
                int n = 1;
                for (int i = 7; i >= 0; i--)
                {
                    len += (int)_Extend[i] * n;
                    n *= 256;
                }
                _Content = new byte[len];
                Buffer.BlockCopy(buffer, _Extend.Length + _Mask.Length + 2, _Content, 0, _Content.Length);
            }
            if (_Header.HasMask) _Content = Mask(_Content, _Mask);
            this.Opcode = (OpCode)_Header.OpCode;
        }
        /// <summary>
        /// 设置消息
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="opcode">4位操作码，定义有效负载数据</param>
        public DataFrame(string content, Encoding encoding, OpCode opcode = OpCode.Text, SocketDataType dataType = SocketDataType.String)
        {
            this.Encoding = encoding;
            if (dataType == SocketDataType.String)
                _Content = content.GetBytes(encoding);
            else
                _Content = content.HexStringToBytes();
            int length = _Content.Length;

            if (length < 126)
            {
                _Extend = new byte[0];
                _Header = new DataFrameHeader(true, false, false, false, (sbyte)opcode, false, length);
            }
            else if (length < 65536)
            {
                _Extend = new byte[2];
                _Header = new DataFrameHeader(true, false, false, false, (sbyte)opcode, false, 126);
                _Extend[0] = (byte)(length / 256);
                _Extend[1] = (byte)(length % 256);
            }
            else
            {
                _Extend = new byte[8];
                _Header = new DataFrameHeader(true, false, false, false, (sbyte)opcode, false, 127);
                int left = length;
                int unit = 256;
                for (int i = 7; i > 1; i--)
                {
                    _Extend[i] = (byte)(left % unit);
                    left /= unit;
                    if (left == 0)
                        break;
                }
            }
        }
        #endregion

        #region 获取消息数据
        /// <summary>
        /// 获取消息数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[2 + _Extend.Length + _Mask.Length + _Content.Length];
            Buffer.BlockCopy(_Header.GetBytes(), 0, buffer, 0, 2);
            Buffer.BlockCopy(_Extend, 0, buffer, 2, _Extend.Length);
            Buffer.BlockCopy(_Mask, 0, buffer, 2 + _Extend.Length, _Mask.Length);
            Buffer.BlockCopy(_Content, 0, buffer, 2 + _Extend.Length + _Mask.Length, _Content.Length);
            return buffer;
        }
        #endregion

        #region 消息
        /// <summary>
        /// 消息
        /// </summary>
        public string Text 
        {
            get
            {
                //if (_Header.OpCode != 1)
                   // return string.Empty;
                if (this.DataType == SocketDataType.HexString)
                {
                    return _Content.ByteToHexString();
                }
                else
                    return _Content.GetString(this.Encoding);
            } 
        }
        #endregion

        #region 0或4个字节，客户端发送给服务端的数据，都是通过内嵌的一个32位值作为掩码的；掩码键只有在掩码位设置为1的时候存在
        /// <summary>
        /// 0或4个字节，客户端发送给服务端的数据，都是通过内嵌的一个32位值作为掩码的；掩码键只有在掩码位设置为1的时候存在。
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="mask">数组</param>
        /// <returns></returns>
        private byte[] Mask(byte[] data, byte[] mask)
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ mask[i % 4]);
            }

            return data;
        }
        #endregion

        #endregion
    }
}
