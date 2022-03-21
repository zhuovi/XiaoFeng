using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020/9/22 15:00:43                *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 4bit,定义有效负载数据,如果收到了一个未知的操作码,连接也必须断
    /// </summary>
    public enum OpCode : sbyte
    {
        /// <summary>
        /// 握手
        /// </summary>
        [Description("握手")]
        Handshake = -1,
        /// <summary>
        /// 连续消息片断
        /// </summary>
        [Description("连续消息片断")]
        Continuation = 0,
        /// <summary>
        /// 文本消息片断
        /// </summary>
        [Description("文本消息片断")]
        Text = 1,
        /// <summary>
        /// 二进制消息片断
        /// </summary>
        [Description("二进制消息片断")]
        Binary = 2,
        /// <summary>
        /// 连接关闭
        /// </summary>
        [Description("连接关闭")]
        Close = 8,
        /// <summary>
        /// 心跳检查的ping
        /// </summary>
        [Description("心跳检查的ping")]
        Ping = 9,
        /// <summary>
        /// 心跳检查的pong
        /// </summary>
        [Description("心跳检查的pong")]
        Pong = 10
    }
}