﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-29 13:55:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// DamengHelper 帮助类
    /// </summary>
    public class DamengHelper : DataHelper, IDbHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DamengHelper() { this.ProviderType = DbProviderType.Dameng; }
        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public DamengHelper(string ConnectionString) : this() { this.ConnectionString = ConnectionString; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="connectionConfig">数据库连接配置</param>
        public DamengHelper(ConnectionConfig connectionConfig) : base(connectionConfig)
        {
            this.ProviderType = DbProviderType.Dameng;
        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region 获取当前数据库所有用户表
        /// <summary>
        /// 获取当前数据库所有用户表
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetTables()
        {
            /*
             * 1.SELECT TABLE_NAME FROM DBA_TABLES WHERE DBA_TABLES.OWNER='模式名称' ORDER BY TABLE_NAME;
             * 2.SELECT TABLE_NAME FROM ALL_TABLES WHERE ALL_TABLES.OWNER='模式名称' ORDER BY TABLE_NAME;        
             * 3.SELECT TABLE_NAME FROM ALL_OBJECTS WHERE OBJECT_TYPE='TABLE' AND ALL_OBJECTS.OWNER='模式名称' ORDER BY TABLE_NAME; 
             * 4.SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME;
             */
            //var Schema = this.ConnConfig.ConnectionString.GetMatch(@"SCHEMA=(?<a>[\s\S]*);").Trim();
            return this.ExecuteDataTable(@"SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME").ToList<string>();
        }
        #endregion

        #region 获取当前数据库所有用户视图
        /// <summary>
        /// 获取当前数据库所有用户视图
        /// </summary>
        /// <returns></returns>
        public virtual List<ViewAttribute> GetViews()
        {
            //var Schema = this.ConnConfig.ConnectionString.GetMatch(@"SCHEMA=(?<a>[\s\S]*);").Trim();
            return this.QueryList<ViewAttribute>(@"SELECT VIEW_NAME as Name,TEXT as Definition FROM USER_VIEWS ORDER BY VIEW_NAME");
        }
        #endregion

        #region 获取当前数据库所有用户存储过程
        /// <summary>
        /// 获取当前数据库所有用户存储过程
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetProcedures()
        {
            //var Schema = this.ConnConfig.ConnectionString.GetMatch(@"SCHEMA=(?<a>[\s\S]*);").Trim();
            return this.ExecuteDataTable(@"SELECT OBJECT_NAME FROM USER_PROCEDURES ORDER BY OBJECT_NAME;").ToList<string>();
        }
        #endregion

        #region 获取当前表所有列
        /// <summary>
        /// 获取当前表所有列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual List<DataColumns> GetColumns(string tableName)
        {
            if (tableName == "") return default(List<DataColumns>);
            var list = this.ExecuteDataTable(@"SELECT 
A.COLUMN_ID AS SortID,A.COLUMN_NAME AS Name,
0 AS IsIdentity,
0 AS PrimaryKey,
0 AS IsUnique,
(SELECT COUNT(0) FROM USER_IND_COLUMNS WHERE USER_IND_COLUMNS.TABLE_NAME=A.TABLE_NAME AND USER_IND_COLUMNS.COLUMN_NAME=A.COLUMN_NAME) AS IsIndex,
A.DATA_TYPE AS Type,A.DATA_LENGTH AS Length,A.DATA_SCALE AS Digits,(CASE NULLABLE WHEN 'Y' THEN 1 ELSE 0 END) AS IsNull,A.DATA_DEFAULT AS DefaultValue,
C.COMMENTS AS DESCRIPTION from SYS.USER_TAB_COLS AS A 
LEFT JOIN USER_COL_COMMENTS AS C
ON A.TABLE_NAME=C.TABLE_NAME AND A.COLUMN_NAME=C.COLUMN_NAME
WHERE A.TABLE_NAME='{0}'".format(tableName)).ToList<DataColumns>();

            var indexs = this.ExecuteDataTable(@"SELECT A.OWNER,A.CONSTRAINT_NAME,A.TABLE_NAME,A.CONSTRAINT_TYPE,B.COLUMN_NAME FROM USER_CONSTRAINTS AS A
LEFT JOIN ALL_CONS_COLUMNS AS B
ON A.CONSTRAINT_NAME = B.CONSTRAINT_NAME
 WHERE A.TABLE_NAME = '{0}';
".format(tableName)).ToList<TableIndexModel>();
            list.Each(a =>
            {
                var index = indexs.Find(b => b.COLUMN_NAME == a.Name);
                if (index != null)
                {
                    if (index.CONSTRAINT_TYPE == "P")
                    {
                        a.PrimaryKey = true;
                        if (a.Type.IsMatch("(bigint|int|INTEGER)"))
                        {
                            a.AutoIncrementSeed = 1; a.AutoIncrementStep = 1; a.IsIdentity = true;
                        }
                    }
                    else if (index.CONSTRAINT_TYPE == "U") a.IsUnique = true;
                }
            });
            return list;
        }
        /// <summary>
        /// 获取当前表所有列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual DataColumnCollection GetDataColumns(string tableName)
        {
            return this.ExecuteDataTable("select top 0 * from {0}".format(tableName)).Columns;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="Columns">显示列</param>
        /// <param name="Condition">条件</param>
        /// <param name="OrderColumnName">排序字段</param>
        /// <param name="OrderType">排序类型ASC或DESC</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">一页多少条</param>
        /// <param name="PageCount">共多少页</param>
        /// <param name="Counts">共多少条</param>
        /// <param name="PrimaryKey">主键</param>
        /// <returns></returns>
        public override DataTable Select(string tableName, string Columns, string Condition, string OrderColumnName, string OrderType, int PageIndex, int PageSize, out int PageCount, out int Counts, string PrimaryKey = "")
        {
            PageCount = 1; Counts = 0;
            if (tableName == "") return new DataTable();
            Columns = Columns == "" ? "*" : Columns;
            if (Condition != "" && !Condition.IsMatch(@"^\s*where")) Condition = " where " + Condition;
            Counts = base.ExecuteScalar("select count(0) from {0}{1}".format(tableName, Condition)).ToString().ToInt32();
            PageSize = PageSize == 0 ? 10 : PageSize;
            PageIndex = PageIndex <= 0 ? 1 : PageIndex;
            if (!OrderColumnName.IsMatch(@"\s*order by")) OrderColumnName = "order by " + OrderColumnName;
            OrderColumnName += " " + OrderType;
            if (Counts == 0) return new DataTable();
            PageCount = Math.Ceiling(Convert.ToDouble(Counts) / Convert.ToDouble(PageSize)).ToCast<int>();
            PageIndex = PageIndex > PageCount ? PageCount : PageIndex;
            if (PrimaryKey == "")
                return base.ExecuteDataTable("select {0} from {1} {2} {3} limit {4},{5};".format(Columns, tableName, Condition, OrderColumnName, (PageIndex - 1) * PageSize, PageSize));
            else
                return base.ExecuteDataTable("select {0} from {1} A JOIN (select {2} from {1} {3} {4} limit {5},{6}) B ON A.{2} = B.{2};".format(Columns, tableName, PrimaryKey, Condition, OrderColumnName, (PageIndex - 1) * PageSize, PageSize));
        }
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="Columns">显示列</param>
        /// <param name="Condition">条件</param>
        /// <param name="OrderColumnName">排序字段</param>
        /// <param name="OrderType">排序类型ASC或DESC</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">一页多少条</param>
        /// <param name="PageCount">共多少页</param>
        /// <param name="Counts">共多少条</param>
        /// <param name="PrimaryKey">主键</param>
        /// <returns></returns>
        public override List<T> Select<T>(string tableName, string Columns, string Condition, string OrderColumnName, string OrderType, int PageIndex, int PageSize, out int PageCount, out int Counts, string PrimaryKey = "")
        {
            return this.Select(tableName, Columns, Condition, OrderColumnName, OrderType, PageIndex, PageSize, out PageCount, out Counts, PrimaryKey).ToList<T>();
        }
        #endregion

        #region 创建数据库表
        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <param name="modelType">表model类型</param>
        /// <returns></returns>
        public virtual Boolean CreateTable(Type modelType)
        {
            string SQLString = @"
DROP TABLE IF EXISTS `{0}`;
CREATE TABLE `{0}` (
   {1}
   {2}
   {3}
)ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='{4}';
select 1;
";
            TableAttribute Table = modelType.GetTableAttribute();
            Table = Table ?? new TableAttribute();
            string Fields = "", Indexs = "", PrimaryKey = "";
            Table.Name = (Table.Name == null || Table.Name.IsNullOrEmpty()) ? modelType.Name : Table.Name;
            DataType dType = new DataType(this.ProviderType);
            modelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase| BindingFlags.Instance).Each(p =>
            {
                if (",ConnectionString,ConnectionTimeOut,CommandTimeOut,ProviderType,IsTransaction,ErrorMessage,tableName,QueryableX,".IndexOf("," + p.Name + ",") == -1)
                {
                    ColumnAttribute Column = p.GetColumnAttribute(false);
                    Column = Column ?? new ColumnAttribute { AutoIncrement = false, IsIndex = false, IsNullable = true, IsUnique = false, PrimaryKey = false, Length = 0, DefaultValue = "" };
                    Column.Name = (Column.Name == null || Column.Name.IsNullOrEmpty()) ? p.Name : Column.Name;
                    Column.DataType = Column.DataType.IsNullOrEmpty() ? dType[p.PropertyType] : Column.DataType;
                    if (Column.AutoIncrement && Column.PrimaryKey)
                    {
                        string type = "int", DefaultValue = "";
                        if (Column.DataType.ToString() == "bigint" || Column.DataType.ToString() == "int")
                        {
                            type = "int(11)";
                            DefaultValue = "AUTO_INCREMENT";
                        }
                        else if (Column.DataType.ToString() == "uniqueidentifier")
                        {
                            type = "STRING";
                            DefaultValue = "";
                        }
                        else
                        {
                            type = Column.DataType.ToString();
                            DefaultValue = "DEFAULT '{0}' ".format(Column.DefaultValue);
                        }
                        Fields += "`{0}` {1} NOT NULL {2} COMMENT '{3}',".format(Column.Name, type, DefaultValue, Column.Description);
                    }
                    else
                    {
                        string DefaultValue = Column.DefaultValue.ToString();
                        if (DefaultValue == "now()" || DefaultValue == "getdate()") DefaultValue = "0000-00-00 00:00:00";
                        else if (Column.Name.ToLower().IndexOf("timestamp") > -1)
                        {
                            DefaultValue = "CURRENT_TIMESTAMP";
                        }
                        var FieldType = dType[p.PropertyType.GetBaseType()];
                        Fields += String.Format(@"
                        `{0}` {1}{2}{3},",
                        Column.Name,
                        FieldType,
                        ((Column.Length == 0 || ",datetime,".IndexOf("," + FieldType.ToString().ToLower() + ",") > -1) ? " " : ("(" + Column.Length + ") ")) + ((Column.IsNullable && !Column.PrimaryKey) ? "NULL" : "NOT NULL") + (Column.DefaultValue.IsNullOrEmpty() ? "" : (" DEFAULT " + (DefaultValue.IsNumberic() ? Column.DefaultValue : ((DefaultValue.StartsWith("'") && DefaultValue.EndsWith("'")) || DefaultValue.ToLower() == "datetime('now','localtime')" || Column.Name.ToLower().IndexOf("timestamp") > -1) ? DefaultValue : ("'" + DefaultValue + "'")) + "")),
                         " COMMENT '{0}'".format(Column.Description));
                    }
                    if (Column.PrimaryKey)
                    {
                        PrimaryKey = ",PRIMARY KEY (`{0}`)".format(Column.Name);
                    }
                    if (Column.IsIndex)
                    {
                        Indexs += ",`{0}`".format(Column.Name);
                    }
                }
            });
            Indexs = Indexs.TrimEnd(',');
            if (Indexs.IsNotNullOrEmpty())
            {
                Indexs = "KEY `{0}Index` ({1})".format(Table.Name, Indexs);
            }
            return base.ExecuteScalar(SQLString.format(Table.Name, Fields.TrimEnd(','), PrimaryKey, Indexs, Table.Description.IsNullOrEmpty() ? Table.Name : Table.Description)).ToString().ToInt16() == 1;
        }
        /// <summary>
        /// 创建数据库表 属性用 TableAttribute,ColumnAttribute
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public virtual Boolean CreateTable<T>() => CreateTable(typeof(T));
        #endregion
        #endregion
    }
    /// <summary>
    /// 表索引模型
    /// </summary>
    public class TableIndexModel
    {
        /// <summary>
        /// 模式
        /// </summary>
        public string OWNER { get; set; }
        /// <summary>
        /// 索引号
        /// </summary>
        public string CONSTRAINT_NAME { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TABLE_NAME { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string CONSTRAINT_TYPE { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string COLUMN_NAME { get; set; }
    }
}