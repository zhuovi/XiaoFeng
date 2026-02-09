using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-02-09 18:00:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 读写
    /// </summary>
    public enum ReadOrWrite
    {
        /// <summary>
        /// 读写
        /// </summary>
        Read = 0,
        /// <summary>
        /// 写
        /// </summary>
        Write = 1,
        /// <summary>
        /// 读写
        /// </summary>
        ReadAndWrite = 2,
    }
}