using System;
using System.Collections.Generic;
using System.Linq;
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
namespace XiaoFeng.Net
{
    /// <summary>
    /// 通用网络接口
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// 服务监听
        /// </summary>
        Socket ServerSocket { get; set; }
        /// <summary>
        /// 使用的寻址方案
        /// </summary>
        AddressFamily AddressFamily { get; set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        SocketType SocketType { get; set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        ProtocolType ProtocolType { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// Socket数据类型
        /// </summary>
        SocketDataType DataType { get; set; }
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}
