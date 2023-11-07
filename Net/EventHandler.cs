using System;
using System.Net;
using System.Net.Sockets;
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
    #region 委托
    /// <summary>
    /// 新连接委托
    /// </summary>
    /// <param name="session">对象</param>
    /// <param name="e">事件</param>
    public delegate void NewConnectionEventHandler(INetSession session, EventArgs e);
    /// <summary>
    /// 接收消息委托
    /// </summary>
    /// <param name="session">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void MessageEventHandler(INetSession session, string message, EventArgs e);
    /// <summary>
    /// 接收消息委托
    /// </summary>
    /// <param name="session">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void MessageByteEventHandler(INetSession session, byte[] message, EventArgs e);
    /// <summary>
    /// 断开连接委托
    /// </summary>
    /// <param name="session"></param>
    /// <param name="e"></param>
    public delegate void DisconnectedEventHandler(INetSession session, EventArgs e);
    /// <summary>
    /// 错误信息委托
    /// </summary>
    /// <param name="session">对象</param>
    /// <param name="e">事件</param>
    public delegate void SessionErrorEventHandler(INetSession session, SocketException e);
    /// <summary>
    /// 停止服务委托
    /// </summary>
    /// <param name="socket">对象</param>
    /// <param name="e">事件</param>
    public delegate void StopEventHandler(ISocket socket, EventArgs e);
    /// <summary>
    /// 错误信息委托
    /// </summary>
    /// <param name="socket">对象</param>
    /// <param name="e">事件</param>
    public delegate void ErrorEventHandler(ISocket socket, SocketException e);
    /// <summary>
    /// 启动委托
    /// </summary>
    /// <param name="socket">服务对象</param>
    /// <param name="e">事件</param>
    public delegate void StartEventHandler(ISocket socket, EventArgs e);


    /// <summary>
    /// 新连接委托
    /// </summary>
    /// <param name="client">对象</param>
    /// <param name="e">事件</param>
    public delegate void OnNewConnectionEventHandler(ISocketClient client, EventArgs e);
    /// <summary>
    /// 接收消息委托
    /// </summary>
    /// <param name="client">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void OnMessageEventHandler(ISocketClient client, string message, EventArgs e);
    /// <summary>
    /// 接收消息委托
    /// </summary>
    /// <param name="client">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void OnMessageByteEventHandler(ISocketClient client, byte[] message, EventArgs e);
    /// <summary>
    /// 断开连接委托
    /// </summary>
    /// <param name="client"></param>
    /// <param name="e"></param>
    public delegate void OnDisconnectedEventHandler(ISocketClient client, EventArgs e);
    /// <summary>
    /// 错误信息委托
    /// </summary>
    /// <param name="client">对象</param>
    /// <param name="endPoint">客户端端点</param>
    /// <param name="e">事件</param>
    public delegate void OnClientErrorEventHandler(ISocketClient client, IPEndPoint endPoint, Exception e);
    /// <summary>
    /// 停止服务委托
    /// </summary>
    /// <param name="socket">对象</param>
    /// <param name="e">事件</param>
    public delegate void OnStopEventHandler(INetSocket socket, EventArgs e);
    /// <summary>
    /// 错误信息委托
    /// </summary>
    /// <param name="socket">对象</param>
    /// <param name="e">事件</param>
    public delegate void OnErrorEventHandler(INetSocket socket, Exception e);
    /// <summary>
    /// 启动委托
    /// </summary>
    /// <param name="socket">服务对象</param>
    /// <param name="e">事件</param>
    public delegate void OnStartEventHandler(INetSocket socket, EventArgs e);
    /// <summary>
    /// 认证事件
    /// </summary>
    /// <param name="client">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void OnAuthenticationEventHandler(ISocketClient client, string message, EventArgs e);
    #endregion
}
