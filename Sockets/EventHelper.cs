using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Sockets
{
    #region 委托
    /// <summary>
    /// 新连接委托
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="e">事件</param>
    public delegate void NewConnectionEventHandler(SocketConnection sender, EventArgs e);
    /// <summary>
    /// 接收消息委托
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void MessageEventHandler(object sender, string message, EventArgs e);
    /// <summary>
    /// 接收消息委托
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="message">消息</param>
    /// <param name="e">事件</param>
    public delegate void MessageByteEventHandler(object sender, byte[] message, EventArgs e);
    /// <summary>
    /// 断开连接委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DisconnectedEventHandler(EndPoint sender, EventArgs e);
    /// <summary>
    /// 关闭服务委托
    /// </summary>
    /// <param name="e">事件</param>
    public delegate void CloseEventHandler(EventArgs e);
    /// <summary>
    /// 错误信息委托
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="e">事件</param>
    public delegate void ErrorEventHandler(object sender, SocketException e);
    /// <summary>
    /// 启动委托
    /// </summary>
    /// <param name="sender">服务对象</param>
    /// <param name="e">事件</param>
    public delegate void OpenEventHandler(object sender, EventArgs e);
    #endregion
}
