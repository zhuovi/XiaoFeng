﻿using System;
using System.Net.Sockets;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-08-02 10:56:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocket 客户端
    /// </summary>
    public class WebSocketClient : SocketClient, IWebSocketClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public WebSocketClient() { }
        /// <summary>
        /// 设置连接地址
        /// </summary>
        /// <param name="url">连接地址</param>
        public WebSocketClient(string url) : this(new Uri(url)) { }
        /// <summary>
        /// 设置连接地址
        /// </summary>
        /// <param name="uri">连接地址</param>
        public WebSocketClient(Uri uri)
        {
            this.Uri = uri;
            base.ConnectionType = ConnectionType.WebSocket;
            this.WebSocketRequestOptions = new WebSocketRequestOptions
            {
                Uri = uri
            };
        }
        #endregion

        #region 属性
        ///<inheritdoc/>
        public Uri Uri { get; }
        ///<inheritdoc/>
        public WebSocketRequestOptions WebSocketRequestOptions { get; private set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        private WebSocketRequest _Request;
        ///<inheritdoc/>
        public WebSocketRequest Request
        {
            get
            {
                if (!this.IsServer) return null;
                if (this._Request == null)
                {
                    this._Request = new WebSocketRequest(this.HostName.IsNullOrEmpty() ? "ws" : "wss", this.RequestHeader);
                }
                return this._Request;
            }
            set => this._Request = value;
        }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override void Start()
        {
            if (base.IsServer)
            {
                base.Start();
                return;
            }
            if (this.Uri == null)
            {
                throw new Exception("请求地址出错.");
            }
            var status = this.Connect();
            if (status != SocketError.Success) return;
            base.StartEventHandler();
            Task.Run(() =>
            {
                this.ReceviceDataAsync().ConfigureAwait(false);
            }, this.CancelToken.Token);
            if (!this.IsPing) return;
        }
        ///<inheritdoc/>
        public override SocketError Connect()
        {
            return this.Connect(this.Uri);
        }
        /// <summary>
        /// 连接到服务端
        /// </summary>
        /// <param name="uri">服务端网址</param>
        /// <returns></returns>
        public SocketError Connect(Uri uri)
        {
            if (uri == null)
            {
                throw new Exception("请求地址出错.");
            }
            if (uri.Scheme == "wss")
            {
                this.HostName = uri.Host;
                if (this.ClientCertificates == null)
                    this.SslProtocols = System.Security.Authentication.SslProtocols.None;
                else
                    this.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            }
            base.Connect(uri.Host, uri.Port);
            //发请求包
            var stream = base.GetSslStream();
            if (stream == null)
            {
                ClientErrorEventHandler(this.EndPoint, new Exception(stream is NetworkStream ? "请求网络流失败." : "注册SSL失败."));
                base.Stop();
                return SocketError.NetworkDown;
            }
            if (this.WebSocketRequestOptions == null) this.WebSocketRequestOptions = new WebSocketRequestOptions { Uri = uri };
            this.WebSocketRequestOptions.SecWebSocketKey = RandomHelper.GetRandomString(16).ToBase64String();
            if (this.WebSocketRequestOptions.Origin.IsNullOrEmpty()) this.WebSocketRequestOptions.Origin = $"{uri.Scheme.ReplacePattern(@"^ws", "http")}://{uri.Host}{(uri.Port == 80 || uri.Port == 443 ? "" : (":" + uri.Port))}";
            var packet = new WebSocketPacket(this, this.WebSocketRequestOptions);
            var data = packet.GetRequestData();
            var bytes = data.GetBytes(this.Encoding);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            bytes = base.ReceviceMessageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var msg = bytes.GetString(this.Encoding);
            var AcceptCode = packet.ComputeWebSocketHandshakeSecurityHash09(this.WebSocketRequestOptions.SecWebSocketKey);
            packet = new WebSocketPacket(this, msg);
            if (packet.SecWebSocketAccept.IsNullOrEmpty() || packet.SecWebSocketAccept != AcceptCode)
            {
                ClientErrorEventHandler(this.EndPoint, new Exception("握手失败.\r\n请求包:\r\n" + data + "握手包:\r\n" + msg));
                base.Stop();
                return SocketError.OperationAborted;
            }
            base.ConnectionType = ConnectionType.WebSocket;
            return SocketError.Success;
        }
        ///<inheritdoc/>
        public void Start(WebSocketRequestOptions options)
        {
            this.WebSocketRequestOptions = options;
            this.Start();
        }
        #endregion
    }
}