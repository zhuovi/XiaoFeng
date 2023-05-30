using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-05-26 17:39:04                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 调度作业
    /// </summary>
    public interface IJobWoker
    {
        /// <summary>
        /// 作业任务
        /// </summary>
        /// <returns></returns>
        Task Invoke();
    }
}