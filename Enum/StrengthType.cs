using System;
using System.ComponentModel;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-04 14:07:56                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 强度类型
    /// </summary>
    [Flags]
    public enum StrengthType
    {
        /// <summary>
        /// 数字
        /// </summary>
        [Description("数字")]
        [DefaultValue(@"\d")]
        Number = 1 << 0,
        /// <summary>
        /// 小写字母
        /// </summary>
        [Description("小写字母")]
        [DefaultValue(@"[a-z]")]
        LowwerLetter = 1 << 1,
        /// <summary>
        /// 大写字母
        /// </summary>
        [Description("大写字母")]
        [DefaultValue(@"[A-Z]")]
        UpperLetter = 1 << 2,
        /// <summary>
        /// 半角特殊符号
        /// </summary>
        [Description("半角特殊符号")]
        [DefaultValue(@"[~!@#$%^&\*\(\)_\+{}\|:""<>\?`-=\[\]\\;',\./]")]
        HalfSymbol = 1 << 3,
        /// <summary>
        /// 中文
        /// </summary>
        [Description("中文")]
        [DefaultValue(@"[\u4e00-\u9fa5]")]
        Chinese = 1 << 4,
        /// <summary>
        /// 全角特殊符号
        /// </summary>
        [Description("全角特殊符号")]
        [DefaultValue(@"[～！＠＃￥％…＆＊（）—＋｛｝｜：“”《》？·－＝【】、；‘’，。／]")]
        FullSymbol = 1 << 5,
    }
}