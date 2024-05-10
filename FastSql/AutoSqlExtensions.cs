using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Data.SQL;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-18 15:32:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class AutoSqlExtensions
    {
        #region 获取枚举前缀
        /// <summary>
        /// 获取枚举前缀
        /// </summary>
        /// <param name="e">枚举</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetPrefix(this Enum e, Boolean inherit = true)
        {
            var f = e.GetType().GetField(e.ToString());
            if (f != null && f.IsDefined(typeof(PrefixAttribute), false))
            {
                var val = f.GetCustomAttributeValue<PrefixAttribute>(a => a.Name, inherit);
                return val == null ? string.Empty : val.ToString();
            }
            return e.ToString();
        }
        #endregion
    }
}