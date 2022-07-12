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
*  Create Time : 2022-07-12 20:28:29                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 目录类型
    /// </summary>
    public enum DirectoryType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")] 
        UNKNOW = 0,
        /// <summary>
        /// 家目录
        /// </summary>
        [Description("家目录")]
        HOME = 1,
        /// <summary>
        /// 根目录
        /// </summary>
        [Description("根目录")]
        ROOT = 2
    }
}