using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace XiaoFeng.Sockets
{
    /// <summary>
    /// 客户端连接存储对象
    /// Verstion : 1.0.0
    /// Create Time : 2018/2/5 11:57:59
    /// Update Time : 2018/2/5 11:57:59
    /// </summary>
    public class ClientSocketData
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ClientSocketData() { this.ConnectTime = DateTime.Now; Buffer = new byte[1024 * 1024]; this.IsWebSocket = false; }
        #endregion

        #region 属性
        /// <summary>
        /// 连接
        /// </summary>
        public Socket ClientSocket { get; set; }
        /// <summary>
        /// 网络接点
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// 是否是WebSocket
        /// </summary>
        public Boolean IsWebSocket { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectTime { get; set; }
        /// <summary>
        /// 接收的数据
        /// </summary>
        public byte[] Buffer { get; set; }
        #endregion        
    }
}