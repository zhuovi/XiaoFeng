using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-08-03 11:47:44                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// WebSocketClient接口
    /// </summary>
    public interface IWebSocketClient : ISocketClient
    {
        /// <summary>
        /// Uri地址
        /// </summary>
        Uri Uri { get; set; }
        /// <summary>
        /// 是否自动Ping
        /// </summary>
        Boolean IsPing { get; set; }
        /// <summary>
        /// ping间隔 单位为秒
        /// </summary>
        int PingTime { get; set; }
    }
}