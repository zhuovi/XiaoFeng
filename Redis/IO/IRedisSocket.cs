using System;
using System.IO;
using System.Net.Sockets;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-08 11:03:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis.IO
{
    /// <summary>
    /// IRedisSocket
    /// </summary>
    public interface IRedisSocket: IDisposable
    {
        /// <summary>
        /// 是否是SSL
        /// </summary>
        Boolean IsSsl { get; set; }
        /// <summary>
        /// 连接配置
        /// </summary>
        RedisConfig ConnConfig { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 寻址方案
        /// </summary>
        AddressFamily AddressFamily { get; set; }
        /// <summary>
        /// 套接字类型
        /// </summary>
        SocketType SocketType { get; set; }
        /// <summary>
        /// 支持协议
        /// </summary>
        ProtocolType ProtocolType { get; set; }
        /// <summary>
        /// 网络流
        /// </summary>
        Stream Stream { get; set; }
        /// <summary>
        /// 接收超时时长
        /// </summary>
        int ReceiveTimeout { get; set; }
        /// <summary>
        /// 发送超时时长
        /// </summary>
        int SendTimeout { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        Boolean IsAuth { get; set; }
        /// <summary>
        /// 库索引
        /// </summary>
        int? DbNum { get; set; }
        /// <summary>
        /// 连接
        /// </summary>
        void Connect();
        /// <summary>
        /// 获取流
        /// </summary>
        /// <returns></returns>
        Stream GetStream();
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}