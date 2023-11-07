using System;
using System.Net.Sockets;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-08-03 11:47:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocketClient接口
    /// </summary>
    public interface IWebSocketClient : ISocketClient
    {
        /// <summary>
        /// Uri地址
        /// </summary>
        Uri Uri { get; }
        /// <summary>
        /// 请求配置
        /// </summary>
        WebSocketRequestOptions WebSocketRequestOptions { get; }
        /// <summary>
        /// 请求数据
        /// </summary>
        WebSocketRequest Request { get; set; }
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="options">请求配置</param>
        void Start(WebSocketRequestOptions options);
        /// <summary>
        /// 连接到服务端
        /// </summary>
        /// <returns></returns>
        new SocketError Connect();
        /// <summary>
        /// 连接到服务端
        /// </summary>
        /// <param name="uri">服务端网址</param>
        /// <returns></returns>
        SocketError Connect(Uri uri);
    }
}