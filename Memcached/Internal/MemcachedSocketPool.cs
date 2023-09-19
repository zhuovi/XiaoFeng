using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XiaoFeng.Collections;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-19 14:33:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// MemcachedSocketPool 类说明
    /// </summary>
    public class MemcachedSocketPool : ObjectPool<ISocketClient>
    {
        #region 构造器
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="memcachedConfig">配置</param>
        /// <param name="endPoint">网络终结点</param>
        public MemcachedSocketPool(MemcachedConfig memcachedConfig,IPEndPoint endPoint)
        {
            this.MemcachedConfig = memcachedConfig;
            this.EndPoint = endPoint;
            this.Max = this.MemcachedConfig.Pool;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 连接配置
        /// </summary>
        private MemcachedConfig MemcachedConfig { get; set; }
        /// <summary>
        /// 网络端结点
        /// </summary>
        private IPEndPoint EndPoint { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        protected override ISocketClient OnCreate()
        {
            var socket = new SocketClient(this.EndPoint)
            {
                ReceiveTimeout = this.MemcachedConfig.ReadTimeout,
                SendTimeout = this.MemcachedConfig.WriteTimeout,
                Encoding = this.MemcachedConfig.Encoding
            };
            if (this.MemcachedConfig.Certificates != null && this.MemcachedConfig.Certificates.Count > 0)
            {
                socket.ClientCertificates = this.MemcachedConfig.Certificates;
                socket.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            }
            var status = socket.Connect();
            if (status == System.Net.Sockets.SocketError.Success)
            {
                return socket;
            }
            else
                return null;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="item">项</param>
        public override void Close(ISocketClient item)
        {
            item.Stop();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="value">项</param>
        public override void OnDispose(ISocketClient value)
        {
            this.Close(value);
            base.OnDispose(value);
        }
        #endregion
    }
}