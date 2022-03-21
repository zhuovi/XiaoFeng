using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-20 上午 02:46:17                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 聚合联合类型
    /// </summary>
    public enum AggregateType
    {
        /// <summary>
        /// 元素分数总和
        /// </summary>
        SUM = 0,
        /// <summary>
        /// 元素分数最小
        /// </summary>
        MIN = 1,
        /// <summary>
        /// 元素分数最大
        /// </summary>
        MAX = 2
    }
}