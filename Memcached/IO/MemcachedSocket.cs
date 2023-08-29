using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-08 11:01:29                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.IO
{
    /// <summary>
    /// Memcached 操作类
    /// </summary>
    public class MemcachedSocket : Disposable, IMemcachedSocket
    {
        #region 构造器
        /// <summary>
        /// 设置SSL
        /// </summary>
        /// <param name="isSSL">是否是SSL</param>
        public MemcachedSocket(Boolean isSSL)
        {
            this.IsSsl = IsSsl;
        }
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="config">配置</param>
        public MemcachedSocket(MemcachedConfig config)
        {
            this.ConnConfig = config;
            this.IsSsl = config.IsSsl;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否是SSL
        /// </summary>
        public Boolean IsSsl { get; set; }
        /// <summary>
        /// 连接配置
        /// </summary>
        public MemcachedConfig ConnConfig { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        public Boolean IsConnected => this.Client != null && Client.Connected;
        /// <summary>
        /// 寻址方案
        /// </summary>
        public AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;
        /// <summary>
        /// 套接字类型
        /// </summary>
        public SocketType SocketType { get; set; } = SocketType.Stream;
        /// <summary>
        /// 支持协议
        /// </summary>
        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        /// <summary>
        /// 发送超时
        /// </summary>
        public int SendTimeout { get; set; } = 10000;
        /// <summary>
        /// 接收超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 10000;
        /// <summary>
        /// 网络流
        /// </summary>
        public Stream Stream { get; set; }
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket SocketClient { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public Boolean IsAuth { get; set; }
        /// <summary>
        /// 库索引
        /// </summary>
        public int? DbNum { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>
        private ISocketClient Client { get; set; }
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (this.Client != null) this.Client.Stop();
            this.Client = new SocketClient()
            {
                SocketType = this.SocketType,
                ConnectTimeout = this.ConnConfig.ConnectionTimeout,
                ReceiveTimeout = this.ReceiveTimeout,
                SendTimeout = this.SendTimeout,
                ProtocolType = this.ProtocolType
            };
            /*if (this.Stream != null)
            {
                this.Stream.Close();
                this.Stream.Dispose();
            }
            if (this.SocketClient != null)
            {
                if(this.SocketClient.Connected)
                    this.SocketClient.Shutdown(SocketShutdown.Both);
                this.SocketClient.Close();
                this.SocketClient.Dispose();
            }
            this.SocketClient = new Socket(this.AddressFamily, this.SocketType, this.ProtocolType);
            this.SocketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.SendTimeout);
            this.SocketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.ReceiveTimeout);
            this.SocketClient.NoDelay = true;*/
        }
        #endregion

        #region 连接
        ///<inheritdoc/>
        public void Connect()
        {
            Init();
            var state = this.Client.Connect(this.ConnConfig.Host, this.ConnConfig.Port);
            if (state == SocketError.Success)
            {
                this.GetStream();
            }
            else if (state == SocketError.TimedOut)
            {
                throw new MemcachedException($"连接服务器超时.{this.ConnConfig.ToJson()}");
            }
            /*
            IAsyncResult result = this.SocketClient.BeginConnect(this.ConnConfig.Host, this.ConnConfig.Port, null, null);
            if (!result.AsyncWaitHandle.WaitOne(Math.Max(this.ConnConfig.ConnectionTimeout, 10000), true))
                throw new MemcachedException($"连接服务器超时.{this.ConnConfig.ToJson()}");

            this.SocketClient.EndConnect(result);
            this.GetStream();
            */
        }
        #endregion

        ///<inheritdoc/>
        public Stream GetStream()
        {
            if (this.Stream != null) return this.Stream;

            return this.Stream = this.Client.GetSslStream();
            /*var ns = new NetworkStream(this.SocketClient, true);

            if (!this.IsSsl) { this.Stream = ns; return ns; }

            var sns = new SslStream(ns, true, new RemoteCertificateValidationCallback((o, cert, chain, errors) => errors == SslPolicyErrors.None));
            sns.AuthenticateAsClient(this.ConnConfig.Host);
            this.Stream = sns;
            return sns;*/
        }

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                this.IsAuth = false;
                if (this.Client != null)
                    this.Client?.Stop();
                if (this.Stream != null)
                {
                    this.Stream?.Close();
                    this.Stream?.Dispose();
                    this.Stream = null;
                }
                if (this.SocketClient != null)
                {
                    this.SocketClient?.Close();
                    this.SocketClient = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">值</param>
        protected override void Dispose(bool disposing)
        {
            this.Close();
            base.Dispose(disposing);
        }
        #endregion

        #endregion
    }
}