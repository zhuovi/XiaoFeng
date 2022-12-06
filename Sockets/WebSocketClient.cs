using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
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
namespace XiaoFeng.Sockets
{
    /// <summary>
    /// WebSocket客户端连接操作类
    /// </summary>
    [Obsolete]
    public class WebSocketClient : SocketClient
    {
        #region 构造器
        /// <summary>
        /// 设置连接数据
        /// </summary>
        /// <param name="Host">Host</param>
        /// <param name="Port">端口</param>
        public WebSocketClient(string Host, int? Port = null) : base(Host, Port ?? 0) { }
        #endregion

        #region 属性
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:删除未使用的私有成员", Justification = "<挂起>")]
        private new AddressFamily AddressFamily { get; set; }
        #endregion
    }
}