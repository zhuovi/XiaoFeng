using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-08-12 11:44:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Json
{
    /// <summary>
    /// key命名规则 
    /// </summary>
    public enum PropertyNamingPolicy
    {
        /// <summary>
        /// 正常输出
        /// </summary>
        Null = 0,
        /// <summary>
        /// 小驼峰方案
        /// </summary>
        SmallCamelCase = 1,
        /// <summary>
        /// 大驼峰方案
        /// </summary>
        GreatCamelCase = 2,
        /// <summary>
        /// 小写方案
        /// </summary>
        LowerCase = 3,
        /// <summary>
        /// 大写方案
        /// </summary>
        UpperCase = 4
    }
}