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
*  Create Time : 2021-06-19 下午 09:29:58                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 查找结果根据距离类型
    /// </summary>
    public enum GeoSortType
    {
        /// <summary>
        /// 从近到远排序
        /// </summary>
        [Description("ASC")]
        ASC = 0,
        /// <summary>
        /// 从远到近排序
        /// </summary>
        [Description("DESC")]
        DESC = 1
    }
}