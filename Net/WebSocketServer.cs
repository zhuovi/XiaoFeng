using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using XiaoFeng.Threading;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-08-03 11:52:25                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocketServer操作类
    /// </summary>
    public class WebSocketServer : SocketServer<WebSocketClient>, IWebSocketServer
    {
        #region 构造器
        /// <summary>
        /// 设置监听网址
        /// </summary>
        /// <param name="url">监听网址</param>
        public WebSocketServer(string url) : this(new Uri(url)) { }
        /// <summary>
        /// 设置监听网址
        /// </summary>
        /// <param name="uri">监听网址</param>
        public WebSocketServer(Uri uri)
        {
            this.Uri = uri;
            base.EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.Uri.Port);
        }
        /// <summary>
        /// 设置监听端口
        /// </summary>
        /// <param name="port">监听端口</param>
        public WebSocketServer(int port)
        {
            base.EndPoint = new IPEndPoint(IPAddress.Any, port);
        }
        /// <summary>
        /// 设置监听地址和端口
        /// </summary>
        /// <param name="address">监听地址</param>
        /// <param name="port">监听端口</param>
        public WebSocketServer(IPAddress address, int port)
        {
            if (address == IPAddress.Any)
            {
                this.Uri = new Uri($"ws://localhost:{port}");
                base.EndPoint = new IPEndPoint(IPAddress.Any, port);
            }
            else
            {
                this.Uri = new Uri($"ws://{address}:{port}");
                this.EndPoint = new IPEndPoint(address, port);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 网络地址
        /// </summary>
        public Uri Uri { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override void Start(int backlog)
        {
            var auth = this.Authentication;
            this.Authentication = c =>
            {
                if (c.RequestHeader.IndexOf("Sec-WebSocket-Key", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return auth(c);
                }
                return false;
            };
            base.Start(backlog);
        }
        #endregion
    }
}