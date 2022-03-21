using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-08-26 11:08:58                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 作业状态
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// 等待
        /// </summary>
        [Description("等待中")]
        Wait = 0,
        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Runing = 1,
        /// <summary>
        /// 停止中
        /// </summary>
        [Description("停止中")]
        Stoping = 2
    }
}