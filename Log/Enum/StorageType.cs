using System;
using System.ComponentModel;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021-05-07 11:21:34                           *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 存储类型
    /// </summary>
    [Flags]
    public enum StorageType : int
    {
        /// <summary>
        /// 不存储
        /// </summary>
        [Description("不存储")]
        No = 0,
        /// <summary>
        /// 文件
        /// </summary>
        [Description("文件")]
        File = 1 << 0,
        /// <summary>
        /// 数据库
        /// </summary>
        [Description("数据库")]
        Database = 1 << 1,
        /// <summary>
        /// 控制台
        /// </summary>
        [Description("控制台")]
        Console = 1 << 2
    }
}