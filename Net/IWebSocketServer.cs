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
        /// <summary>
        /// 是否自动Pong
        /// </summary>
        Boolean IsPong { get; set; }
        /// <summary>
        /// pong间隔 单位为秒
        /// </summary>
        int PongTime { get; set; }
    }
}