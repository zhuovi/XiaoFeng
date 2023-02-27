using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-17 20:17:05                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
    /// <summary>
    /// Http请求内核
    /// </summary>
    public enum HttpCore
    {
        /// <summary>
        /// HttpClient
        /// </summary>
        [Description("HttpClient")]
        HttpClient = 0,
        /// <summary>
        /// HttpWebRequest
        /// </summary>
        [Description("HttpWebRequest")]
        HttpWebRequest = 1,
        /// <summary>
        /// Socket
        /// </summary>
        [Description("Socket")]
        HttpSocket = 2
    }
}