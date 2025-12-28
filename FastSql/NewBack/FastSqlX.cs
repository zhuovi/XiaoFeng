using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.FastSql.NewBack.Interface;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-01-03 11:23:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql.NewBack
{
    /// <summary>
    /// FastSql 入口主类
    /// </summary>
    public class FastSqlX
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public FastSqlX()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// SQL 创建器
        /// </summary>
        public ISqlBuilder SqlBuilder { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}