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
        Socket ClientSocket { get; set; }
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
		/// 获取或设置 System.Boolean 值，该值指定流 System.Net.Sockets.Socket 是否正在使用 Nagle 算法。使用 Nagle 算法，则为 false；否则为 true。 默认值为 false。
		/// </summary>
		Boolean NoDelay { get; set; }
		/// <summary>
		/// 获取或设置一个值，该值指定之后同步 Overload:System.Net.Sockets.Socket.Receive 调用将超时的时间长度。默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
		/// </summary>
		int ReceiveTimeout { get; set; }
		/// <summary>
		/// 获取或设置一个值，该值指定之后同步 Overload:System.Net.Sockets.Socket.Send 调用将超时的时间长度。超时值（以毫秒为单位）。 如果将该属性设置为 1 到 499 之间的值，该值将被更改为 500。 默认值为 0，指示超时期限无限大。 指定 -1 还会指示超时期限无限大。
		/// </summary>
		int SendTimeout { get; set; }
		/// <summary>
		/// 获取或设置一个值，它指定 System.Net.Sockets.Socket 接收缓冲区的大小。System.Int32，它包含接收缓冲区的大小（以字节为单位）。 默认值为 8192。
		/// </summary>
		int ReceiveBufferSize { get; set; }
		/// <summary>
		/// 获取或设置一个值，该值指定 System.Net.Sockets.Socket 发送缓冲区的大小。System.Int32，它包含发送缓冲区的大小（以字节为单位）。 默认值为 8192。
		/// </summary>
		int SendBufferSize { get; set; }
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
