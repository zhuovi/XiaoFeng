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
*  Create Time : 2024-04-18 15:13:31                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// 表结构数据
    /// </summary>
    public class TableData
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        public TableData() { }
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="tableName">表名</param>
        public TableData(TableType tableType, string tableName)
        {
            this.TableType = tableType;
            this.TableName = tableName;
        }
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="modelType">模型类型</param>
        public TableData(TableType tableType,string tableName,Type modelType)
        {
            this.TableType = tableType;
            this.TableName = tableName;
            this.ModelType = modelType;
        }
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="where">表条件</param>
        public TableData(TableType tableType, string tableName, string where)
        {
            this.TableType = tableType;
            this.TableName = tableName;
            this.WhereString = where;
        }
        /// <summary>
        /// 初始化一个新的实例
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="modelType">模型类型</param>
        /// <param name="where">表条件</param>
        public TableData(TableType tableType, string tableName,Type modelType, string where)
        {
            this.TableType = tableType;
            this.TableName = tableName;
            this.ModelType = modelType;
            this.WhereString = where;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表类型
        /// </summary>
        public TableType TableType { get; set; }
        /// <summary>
        /// 模型类型
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表条件
        /// </summary>
        public string WhereString { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}