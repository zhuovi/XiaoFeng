using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 服务端用户数据接口
    /// </summary>
    public interface IServerSession : INetSession
    {
        /// <summary>
        /// ID
        /// </summary>
        Guid ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        String GroupID { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// 接收数据
        /// </summary>
        byte[] ReceivedDataBuffer { get; set; }
        /// <summary>
        /// 协议版本
        /// </summary>
        SslProtocols SslProtocols { get; set; }
        /// <summary>
        /// SSL证书
        /// </summary>
        X509Certificate Certificate { get; set; }
        /// <summary>
        /// 读超时
        /// </summary>
        int ReadTimeout { get; set; }
        /// <summary>
        /// 写超时
        /// </summary>
        int WriteTimeout { get; set; }

        #region 委托
        /// <summary>
        /// 连接委托
        /// </summary>
        event NewConnectionEventHandler OnNewConnection;
        /// <summary>
        /// 消息委托
        /// </summary>
        event MessageEventHandler OnMessage;
        /// <summary>
        /// 消息委托
        /// </summary>
        event MessageByteEventHandler OnMessageByte;
        /// <summary>
        /// 断开连接委托
        /// </summary>
        event DisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 出错委托
        /// </summary>
        event SessionErrorEventHandler OnSessionError;
        /// <summary>
        /// 连接认证
        /// </summary>
        Func<ServerSession, bool> SocketAuth { get; set; }
        /// <summary>
        /// 设置是Socket类型
        /// </summary>
        Action<ServerSession> SetSocketType { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 握手信息
        /// </summary>
        /// <param name="status">状态</param>
        void ManageHandshake(IAsyncResult status);
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
		#endregion
	}
}
