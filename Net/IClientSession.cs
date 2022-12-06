using System;
using System.Collections.Generic;
using System.Linq;
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
    /// 客户端用户数据接口
    /// </summary>
    public interface IClientSession : INetSession
    {
        /// <summary>
        /// Host
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Header
        /// </summary>
        Dictionary<string, string> Header { get; set; }
    }
}