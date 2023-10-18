using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-31 09:54:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 子命令
    /// </summary>
    public enum PubSubCommand
    {
        /// <summary>
        /// 查询系统中符合模式的频道信息，pattern为空，则查询系统中所有存在的频道
        /// </summary>
        [Description("查询系统中符合模式的频道信息")]
        CHANNELS = 0,
        /// <summary>
        /// 查询一个或多个频道的订阅数
        /// </summary>
        [Description("查询一个或多个频道的订阅数")]
        NUMSUB = 1,
        /// <summary>
        /// 查询当前客户端订阅了多少频道
        /// </summary>
        [Description("查询当前客户端订阅了多少频道")]
        NUMPAT = 2
    }
}