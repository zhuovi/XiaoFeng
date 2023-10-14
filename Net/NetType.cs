using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 23:12:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络类型
    /// </summary>
    public enum NetType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        [Port(0)]
        Unknown = 0,
        /// <summary>
        /// Tcp
        /// </summary>
        [Description("Tcp")]
        [Port(1006)]
        Tcp = 1,
        /// <summary>
        /// Udp
        /// </summary>
        [Description("Udp")]
        [Port(1006)]
        Udp = 2,
        /// <summary>
        /// WebSocket
        /// </summary>
        [Description("WebSocket")]
        [Port(1006)]
        Ws = 3,
        /// <summary>
        /// WebSocket SSL
        /// </summary>
        [Description("WebSocket SSL")]
        [Port(1006), Ssl]
        Wss = 4,
        /// <summary>
        /// Http
        /// </summary>
        [Description("Http")]
        [Port(80)]
        Http = 5,
        /// <summary>
        /// Https
        /// </summary>
        [Description("Https")]
        [Port(443), Ssl]
        Https = 6,
        /// <summary>
        /// Ftp
        /// </summary>
        [Description("Ftp")]
        [Port(21)]
        Ftp = 7,
        /// <summary>
        /// Ftps
        /// </summary>
        [Description("Ftps")]
        [Port(21), Ssl]
        Ftps = 8,
        /// <summary>
        /// Sftp
        /// </summary>
        [Description("Sftp")]
        [Port(22), Ssl]
        Sftp = 9,
        /// <summary>
        /// Ssh
        /// </summary>
        [Description("Ssh")]
        [Port(22)]
        Ssh = 10,
        /// <summary>
        /// Telnet
        /// </summary>
        [Description("Telnet")]
        [Port(23)]
        Telnet = 11,
        /// <summary>
        /// NetPipe
        /// </summary>
        [Description("NetPipe")]
        [Port(1006)]
        NetPipe = 12,
        /// <summary>
        /// Pop3
        /// </summary>
        [Description("Pop3")]
        [Port(995)]
        Pop3 = 13,
        /// <summary>
        /// Imap
        /// </summary>
        [Description("Imap")]
        [Port(993)]
        Imap = 14,
        /// <summary>
        /// Redis
        /// </summary>
        [Description("Redis")]
        [Port(6379)]
        Redis = 101,
        /// <summary>
        /// Redis SSL
        /// </summary>
        [Description("Redis SSL")]
        [Port(6379), Ssl]
        Rediss = 102,
        /// <summary>
        /// Memcached
        /// </summary>
        [Description("Memcached")]
        [Port(11211)]
        Memcached = 103,
        /// <summary>
        /// Memcached SSL
        /// </summary>
        [Description("Memcached SSL")]
        [Port(11211), Ssl]
        Memcacheds = 104,
        /// <summary>
        /// Mqtt
        /// </summary>
        [Description("Mqtt")]
        [Port(1883)]
        Mqtt = 105,
        /// <summary>
        /// Mqtts
        /// </summary>
        [Description("Mqtts")]
        [Port(1883), Ssl]
        Mqtts = 106
    }
}