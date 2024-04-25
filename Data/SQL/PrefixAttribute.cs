using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-18 15:23:01                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 表前缀
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class PrefixAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">表前缀</param>
        public PrefixAttribute(string name)
        {
            this.Name = name;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表前缀
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}