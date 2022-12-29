using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 11:48:46                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 单元格数值格式
    /// </summary>
    public enum NumberFormat
    {
        /// <summary>
        /// 常规
        /// </summary>
        [Description("常规")]
        General = 0,
        /// <summary>
        /// 数值
        /// </summary>
        [Description("数值")]
        Number = 2,
        /// <summary>
        /// 货币
        /// </summary>
        [Description("货币")]
        Currency = 164,
        /// <summary>
        /// 会计专用
        /// </summary>
        [Description("会计专用")]
        Accounting = 44,
        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        Date = 14,
        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间")]
        Time=165,
        /// <summary>
        /// 百分比
        /// </summary>
        [Description("百分比")]
        Percentage = 10,
        /// <summary>
        /// 分数
        /// </summary>
        [Description("分数")]
        Fraction = 13,
        /// <summary>
        /// 科学计数
        /// </summary>
        [Description("科学计数")]
        Scientific=11,
        /// <summary>
        /// 文本
        /// </summary>
        [Description("文本")]
        Text=49,
        /// <summary>
        /// 特殊
        /// </summary>
        [Description("特殊")]
        Special = 50,
        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        Custom = 166,
        /// <summary>
        /// 未支持
        /// </summary>
        [Description("未支持")]
        Unsupported = -1
    }
}