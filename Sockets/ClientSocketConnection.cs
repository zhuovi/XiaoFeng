using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
namespace XiaoFeng.Sockets
{
    #region 客户端连接类型
    /// <summary>
    /// 客户端连接类型
    /// </summary>
    public class ClientSocketConnection
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ClientSocketConnection() { }
        #endregion

        #region 属性
        /// <summary>
        /// WS类型
        /// </summary>
        public WsType WsType { get; set; } = WsType.Null;
        /// <summary>
        /// Socket
        /// </summary>
        public Socket Socket { get; set; }
        /// <summary>
        /// 网址或IP
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; } = 1006;
        /// <summary>
        /// 连接服务器地址
        /// </summary>
        public IPAddress IPAddress { get; set; } = IPAddress.Parse("127.0.0.1");
        /// <summary>
        /// Header头信息
        /// </summary>
        public Dictionary<string, string> Header { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Header头信息
        /// </summary>
        public string Headers { get; set; }
        #endregion
    }
    #endregion
}