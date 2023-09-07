using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-07 11:32:50                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 基础网络库
    /// </summary>
    public abstract class BaseSocket : Disposable, INetSocket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseSocket()
        {

        }
        #endregion

        #region 事件
        /// <summary>
        /// 启动事件
        /// </summary>
        public abstract event OnStartEventHandler OnStart;
        /// <summary>
        /// 停止事件
        /// </summary>
        public abstract event OnStopEventHandler OnStop;
        /// <summary>
        /// 客户端错误信息事件
        /// </summary>
        public abstract event OnClientErrorEventHandler OnClientError;
        /// <summary>
        /// 接收消息(string)事件
        /// </summary>
        public abstract event OnMessageEventHandler OnMessage;
        /// <summary>
        /// 接收消息(byte[])事件
        /// </summary>
        public abstract event OnMessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 认证事件
        /// </summary>
        public abstract event OnAuthenticationEventHandler OnAuthentication;
        #endregion

        #region 属性
        ///<inheritdoc/>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 激活状态
        /// </summary>
        internal Boolean _Active = false;
        ///<inheritdoc/>
        public Boolean Active { get => this._Active; }
        ///<inheritdoc/>
        public SocketState SocketState { get; set; } = SocketState.Idle;
        ///<inheritdoc/>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.None;
        ///<inheritdoc/>
        public SocketType SocketType { get; set; } = SocketType.Stream;
        ///<inheritdoc/>
        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
        ///<inheritdoc/>
        public Boolean NoDelay { get; set; } = false;
        ///<inheritdoc/>
        public int ReceiveTimeout { get; set; } = -1;
        ///<inheritdoc/>
        public int SendTimeout { get; set; } = -1;
        /// <summary>
        /// 指定之后连接服务端将超时的时间长度
        /// </summary>
        private int _ConnectTimeout = 0;
        /// <inheritdoc/>
        public int ConnectTimeout
        {
            get
            {
                if (this._ConnectTimeout >= 1 && this._ConnectTimeout < 500)
                    this._ConnectTimeout = 500;
                if (this._ConnectTimeout <= 0) this._ConnectTimeout = 0;
                return this._ConnectTimeout;
            }
            set
            {
                if (value >= 1 && value < 500) this._ConnectTimeout = 500;
                if (value <= 0) this._ConnectTimeout = 0;
                this._ConnectTimeout = value;
            }
        }
        ///<inheritdoc/>
        public int ReceiveBufferSize { get; set; } = 8192;
        ///<inheritdoc/>
        public int SendBufferSize { get; set; } = 8192;
        ///<inheritdoc/>
        public IPEndPoint EndPoint { get; set; }
        ///<inheritdoc/>
        public CancellationTokenSource CancelToken { get; set; } = new CancellationTokenSource();
        ///<inheritdoc/>
        public virtual Boolean ExclusiveAddressUse { get; set; }
        ///<inheritdoc/>
        public SocketDataType DataType { get; set; } = SocketDataType.String;
        /// <summary>
        /// 网络延时时长 默认为10毫秒
        /// </summary>
        private int _NetworkDelay = 10;
        ///<inheritdoc/>
        public int NetworkDelay
        {
            get
            {
                if (this._NetworkDelay < 0) this._NetworkDelay = 1;
                else if (this._NetworkDelay > 10_000) this._NetworkDelay = 5_000;
                return this._NetworkDelay;
            }
            set
            {
                if (value < 0) this._NetworkDelay = 0;
                else if (value > 10_000) this._NetworkDelay = 5_000;
                this._NetworkDelay = value;
            }
        }
        #endregion

        #region 回调事件
        ///<inheritdoc/>
        public abstract void StartEventHandler();
        ///<inheritdoc/>
        public abstract void StopEventHandler();
        #endregion

        #region 方法
        /// <summary>
        /// 启动
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// 停止
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// 获取Socket
        /// </summary>
        /// <returns>返回Soccket</returns>
        public abstract Socket GetSocket();
        #endregion
    }
}