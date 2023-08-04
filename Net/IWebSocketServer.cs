using System;

namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocketServer接口
    /// </summary>
    public interface IWebSocketServer : ISocketServer
    {
        /// <summary>
        /// 网络地址
        /// </summary>
        Uri Uri { get; set; }
    }
}