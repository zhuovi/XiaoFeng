using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoFeng.Redis
{
    #region 委托
    /// <summary>
    /// 接收频道消息
    /// </summary>
    /// <param name="channel">频道</param>
    /// <param name="message">消息</param>
    public delegate void OnMessageEventHandler(string channel, string message);
    /// <summary>
    /// 订阅频道
    /// </summary>
    /// <param name="channel">频道</param>
    public delegate void OnSubscribeEventHandler(string channel);
    /// <summary>
    /// 取消订阅频道
    /// </summary>
    /// <param name="channel">频道</param>
    public delegate void OnUnSubscribeEventHandler(string channel);
    /// <summary>
    /// 出错
    /// </summary>
    /// <param name="channel">频道</param>
    /// <param name="message">错误消息</param>
    public delegate void OnErrorEventHandler(string channel,string message);
    #endregion
}