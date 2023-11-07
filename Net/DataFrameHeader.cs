using System;
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
    /// <summary>
    /// 帧头操作类
    /// </summary>
    public class DataFrameHeader
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">字节</param>
        public DataFrameHeader(byte[] buffer)
        {
            if (buffer.Length < 2)
                throw new Exception("无效的数据头.");
            //第一个字节
            _fin = (buffer[0] & 0x80) == 0x80;
            _rsv1 = (buffer[0] & 0x40) == 0x40;
            _rsv2 = (buffer[0] & 0x20) == 0x20;
            _rsv3 = (buffer[0] & 0x10) == 0x10;
            _opcode = (sbyte)(buffer[0] & 0x0f);
            //第二个字节
            _maskcode = (buffer[1] & 0x80) == 0x80;
            _payloadlength = (sbyte)(buffer[1] & 0x7f);
        }
        /// <summary>
        /// 发送封装数据
        /// </summary>
        /// <param name="fin">0表示不是当前消息的最后一帧，后面还有消息,1表示这是当前消息的最后一帧</param>
        /// <param name="rsv1">1位，若没有自定义协议,必须为0,否则必须断开</param>
        /// <param name="rsv2">1位，若没有自定义协议,必须为0,否则必须断开.</param>
        /// <param name="rsv3">1位，若没有自定义协议,必须为0,否则必须断开</param>
        /// <param name="opcode">4位操作码，定义有效负载数据，如果收到了一个未知的操作码，连接必须断开.</param>
        /// <param name="hasmask">1位，定义传输的数据是否有加掩码,如果有掩码则存放在MaskingKey</param>
        /// <param name="length">传输数据的长度</param>
        public DataFrameHeader(bool fin, bool rsv1, bool rsv2, bool rsv3, sbyte opcode, bool hasmask, int length)
        {
            _fin = fin;
            _rsv1 = rsv1;
            _rsv2 = rsv2;
            _rsv3 = rsv3;
            _opcode = opcode;
            //第二个字节
            _maskcode = hasmask;
            _payloadlength = (sbyte)length;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 0表示不是当前消息的最后一帧，后面还有消息,1表示这是当前消息的最后一帧
        /// </summary>
        private bool _fin;
        /// <summary>
        /// 1位，若没有自定义协议,必须为0,否则必须断开
        /// </summary>
        private bool _rsv1;
        /// <summary>
        /// 1位，若没有自定义协议,必须为0,否则必须断开.
        /// </summary>
        private bool _rsv2;
        /// <summary>
        /// 1位，若没有自定义协议,必须为0,否则必须断开
        /// </summary>
        private bool _rsv3;
        /// <summary>
        /// 4位操作码，定义有效负载数据，如果收到了一个未知的操作码，连接必须断开.
        /// </summary>
        private sbyte _opcode;
        /// <summary>
        /// 1位，定义传输的数据是否有加掩码,如果有掩码则存放在MaskingKey
        /// </summary>
        private bool _maskcode;
        /// <summary>
        /// 传输数据的长度
        /// </summary>
        private sbyte _payloadlength;
        /// <summary>
        /// 0表示不是当前消息的最后一帧，后面还有消息,1表示这是当前消息的最后一帧
        /// </summary>
        public bool FIN { get { return _fin; } }
        /// <summary>
        /// 1位，若没有自定义协议,必须为0,否则必须断开
        /// </summary>
        public bool RSV1 { get { return _rsv1; } }
        /// <summary>
        /// 1位，若没有自定义协议,必须为0,否则必须断开.
        /// </summary>
        public bool RSV2 { get { return _rsv2; } }
        /// <summary>
        /// 1位，若没有自定义协议,必须为0,否则必须断开
        /// </summary>
        public bool RSV3 { get { return _rsv3; } }
        /// <summary>
        /// 4位操作码，定义有效负载数据，如果收到了一个未知的操作码，连接必须断开.
        /// </summary>
        public sbyte OpCode { get { return _opcode; } }
        /// <summary>
        /// 1位，定义传输的数据是否有加掩码,如果有掩码则存放在MaskingKey
        /// </summary>
        public bool HasMask { get { return _maskcode; } }
        /// <summary>
        /// 传输数据的长度
        /// </summary>
        public sbyte Length { get { return _payloadlength; } }
        #endregion

        #region 方法

        #region 返回帧头字节
        /// <summary>
        /// 返回帧头字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[2] { 0, 0 };

            if (_fin) buffer[0] ^= 0x80;
            if (_rsv1) buffer[0] ^= 0x40;
            if (_rsv2) buffer[0] ^= 0x20;
            if (_rsv3) buffer[0] ^= 0x10;
            buffer[0] ^= (byte)_opcode;
            if (_maskcode) buffer[1] ^= 0x80;
            buffer[1] ^= (byte)_payloadlength;
            return buffer;
        }
        #endregion

        #endregion
    }
}