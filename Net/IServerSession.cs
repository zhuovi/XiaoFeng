using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 频道
        /// </summary>
        String Channel { get; set; }
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
        #endregion
    }
}
