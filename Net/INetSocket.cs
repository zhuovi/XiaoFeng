using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-27 17:30:56                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络库接口
    /// </summary>
    public interface INetSocket
    {
        #region 构造器

        #endregion

        #region 属性
        /// <summary>
        /// 激活状态
        /// </summary>
        Boolean Active { get; }
        /// <summary>
        /// Socket状态
        /// </summary>
        SocketState SocketState { get; set; }
        /// <summary>
        /// 协议版本
        /// </summary>
        SslProtocols SslProtocols { get; set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        SocketType SocketType { get; set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        ProtocolType ProtocolType { get; set; }
        /// <summary>
        /// 获取或设置 System.Boolean 值，该值指定流 System.Net.Sockets.Socket 是否正在使用 Nagle 算法。使用 Nagle 算法，则为 false；否则为 true。 默认值为 false。
        /// </summary>
        Boolean NoDelay { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指定之后同步 Overload:System.Net.Sockets.Socket.Receive 调用将超时的时间长度。默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
        /// </summary>
        int ReceiveTimeout { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指定之后同步 Overload:System.Net.Sockets.Socket.Send 调用将超时的时间长度。超时值（以毫秒为单位）。 如果将该属性设置为 1 到 499 之间的值，该值将被更改为 500。 默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
        /// </summary>
        int SendTimeout { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指定之后连接服务端将超时的时间长度。超时值（以毫秒为单位）。 如果将该属性设置为 1 到 499 之间的值，该值将被更改为 500。 默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
        /// </summary>
        int ConnectTimeout { get; set; }
        /// <summary>
        /// 获取或设置一个值，它指定 System.Net.Sockets.Socket 接收缓冲区的大小。System.Int32，它包含接收缓冲区的大小（以字节为单位）。 默认值为 8192。
        /// </summary>
        int ReceiveBufferSize { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指定 System.Net.Sockets.Socket 发送缓冲区的大小。System.Int32，它包含发送缓冲区的大小（以字节为单位）。 默认值为 8192。
        /// </summary>
        int SendBufferSize { get; set; }
        /// <summary>
        /// IP 地址
        /// </summary>
        IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// 取消通知
        /// </summary>
        CancellationTokenSource CancelToken { get; set; }
        /// <summary>
        /// 是否独占地址使用
        /// </summary>
        Boolean ExclusiveAddressUse { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        SocketDataType DataType { get; set; }
        /// <summary>
        /// 网络延时 单位为毫秒 默认为 10 毫秒。
        /// <para>在服务端接收客户端websocket连接时使用</para>
        /// </summary>
        int NetworkDelay { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 启动事件
        /// </summary>
        event OnStartEventHandler OnStart;
        /// <summary>
        /// 停止事件
        /// </summary>
        event OnStopEventHandler OnStop;
        /// <summary>
        /// 客户端错误信息事件
        /// </summary>
        event OnClientErrorEventHandler OnClientError;
        /// <summary>
        /// 接收消息(string)事件
        /// </summary>
        event OnMessageEventHandler OnMessage;
        /// <summary>
        /// 接收消息(byte[])事件
        /// </summary>
        event OnMessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 认证事件
        /// </summary>
        event OnAuthenticationEventHandler OnAuthentication;
        #endregion

        #region 事件回调
        /// <summary>
        /// 启动事件回调
        /// </summary>
        void StartEventHandler();
        /// <summary>
        /// 停止事件回调
        /// </summary>
        void StopEventHandler();
        #endregion

        #region 方法
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        /// <summary>
        /// 获取Socket
        /// </summary>
        /// <returns>返回Soccket</returns>
        Socket GetSocket();
        #endregion
    }
}