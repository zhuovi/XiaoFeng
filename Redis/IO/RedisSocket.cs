using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;

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
namespace XiaoFeng.Redis.IO
{
    /// <summary>
    /// RedisSocket 操作类
    /// </summary>
    public class RedisSocket : Disposable, IRedisSocket
    {
        #region 构造器
        /// <summary>
        /// 设置SSL
        /// </summary>
        /// <param name="isSSL">是否是SSL</param>
        public RedisSocket(Boolean isSSL)
        {
            this.IsSsl = IsSsl;
        }
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="config">配置</param>
        public RedisSocket(RedisConfig config)
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
        public RedisConfig ConnConfig { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        public Boolean IsConnected => this.SocketClient != null && SocketClient.Connected;
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
        #endregion

        #region 方法
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (this.Stream != null)
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
            this.SocketClient.SendTimeout = this.SendTimeout;
            this.SocketClient.ReceiveTimeout = this.ReceiveTimeout;
        }
        ///<inheritdoc/>
        public void Connect()
        {
            Init();
            
			IAsyncResult result = this.SocketClient.BeginConnect(this.ConnConfig.Host, this.ConnConfig.Port, null, null);
            if (!result.AsyncWaitHandle.WaitOne(Math.Max(this.ConnConfig.ConnectionTimeout, 10000), true))
                throw new RedisException($"连接服务器超时.{this.ConnConfig.ToJson()}");

			this.SocketClient.EndConnect(result);
			this.GetStream();
		}
        ///<inheritdoc/>
        public Stream GetStream()
        {
            if (this.Stream != null) return this.Stream;

            var ns = new NetworkStream(this.SocketClient, true);

            if (!this.IsSsl) { this.Stream = ns; return ns; }

            var sns = new SslStream(ns, true, new RemoteCertificateValidationCallback((o, cert, chain, errors) => errors == SslPolicyErrors.None));
#if NETSTANDARD2_0
            sns.AuthenticateAsClient(this.ConnConfig.Host);
#else
            sns.AuthenticateAsClient(this.ConnConfig.Host);
#endif
            this.Stream = sns;
            return sns;
        }

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                if (this.Stream != null)
                {
                    this.Stream.Close();
                    this.Stream.Dispose();
                }
                if (this.SocketClient != null)
                {
                    this.SocketClient.Shutdown(SocketShutdown.Both);
                    this.SocketClient.Disconnect(false);
                    this.SocketClient.Close();
                    this.SocketClient.Dispose();
                }
            }
            catch { }
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