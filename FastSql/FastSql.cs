using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.Model;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-01-02 15:47:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// FastSql 主类
    /// </summary>
    public class FastSqlX:IFastSql
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
        /// Sql 创建器
        /// </summary>
        public ISqlBuilder SqlBuilder { get; set; }
        #endregion

        #region 方法

        #endregion
    }
    /// <summary>
    /// FastSql 实体类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class FastSqlX<T> : FastSqlX, IFastSql<T> where T : IEntity<T>, new()
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        public FastSqlX()
        {
            this.SqlBuilder = new SqlBuilder<T>();
        }
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        /// <param name="connectionConfig">数据库连接</param>
        public FastSqlX(ConnectionConfig connectionConfig)
        {
            this.SqlBuilder = new SqlBuilder<T>
            {
                Config = connectionConfig
            };
        }
        #endregion

        #region 属性
        /// <summary>
        /// Sql 创建器
        /// </summary>
        public new ISqlBuilder<T> SqlBuilder { get; set; }
        #endregion

        #region 方法

        #region 扩展条件算法
        /// <summary>
        /// 扩展条件算法
        /// </summary>
        /// <param name="where">条件表达式</param>
        /// <returns></returns>
        public IFastSql<T> Where(Expression<Func<T,bool>> where)
        {
            if (where == null) return this;
            if(where.Body is ConstantExpression cex)
            {
                if(cex.Value is bool val)
                {
                    this.SqlBuilder.AddWhere($"1 = {(val ? 1 : 0)}");
                }
                else
                {
                    //不预处理，肯定是写错了；
                }
            }
            return this;
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// FastSql 实体类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    public class FastSqlX<T, T2> : FastSqlX<T>, IFastSql<T, T2> where T : IEntity<T>, new() where T2 : IEntity<T2>, new()
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        public FastSqlX()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// Sql 创建器
        /// </summary>
        public new ISqlBuilder<T, T2> SqlBuilder { get; set; }
        #endregion

        #region 方法

        #endregion
    }
    /// <summary>
    /// FastSql 实体类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    /// <typeparam name="T3">T3类型</typeparam>
    public class FastSqlX<T, T2,T3> : FastSqlX<T>, IFastSql<T, T2,T3> where T : IEntity<T>, new() where T2 : IEntity<T2>, new() where T3 : IEntity<T3>, new()
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        public FastSqlX()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// Sql 创建器
        /// </summary>
        public new ISqlBuilder<T, T2,T3> SqlBuilder { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}