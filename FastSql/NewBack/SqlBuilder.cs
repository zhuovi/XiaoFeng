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
*  Create Time : 2025-01-03 11:23:20                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql.NewBack
{
    /// <summary>
    /// SQL 创造器
    /// </summary>
    public abstract class SqlBuilder: Interface.ISqlBuilder
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public SqlBuilder()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据仓
        /// </summary>
        public virtual IDataBuilder DataBuilder { get; set; }
        #endregion

        #region 方法

        #endregion
    }
    public abstract class SqlBuilder<T> : SqlBuilder//, ISqlBuilder<T>
    {

    }
}