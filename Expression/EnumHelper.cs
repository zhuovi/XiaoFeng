using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-09-18 00:51:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Expressions
{
    /// <summary>
    /// 指定排序操作的方向
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 按升序排序
        /// </summary>
        [Description("按升序排序")]
        Ascending = 0,
        /// <summary>
        /// 按降序排序
        /// </summary>
        [Description("按降序排序")]
        Descending = 1
    }
}
