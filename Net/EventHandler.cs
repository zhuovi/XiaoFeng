using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
    #endregion
}
