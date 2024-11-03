﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using XiaoFeng.Model;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-08-10 11:45:08                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// SQLServer 数据库操作类
    /// Verstion : 1.0.0
    /// Create Time : 2017/8/10 11:45:08
    /// Update Time : 2017/8/10 11:45:08
    /// </summary>
    public class SqlHelper : DataHelper, IDbHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SqlHelper() { this.ProviderType = DbProviderType.SqlServer; }
        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public SqlHelper(string ConnectionString) : this() { this.ConnectionString = ConnectionString; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="connectionConfig">数据库连接配置</param>
        public SqlHelper(ConnectionConfig connectionConfig) : base(connectionConfig)
        {
            this.ProviderType = DbProviderType.SqlServer;
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
            return this.ExecuteDataTable(@"SELECT Name FROM SysObjects Where XType='U' ORDER BY Name;").ToList<string>();
        }
        #endregion

        #region 获取当前数据库所有用户视图
        /// <summary>
        /// 获取当前数据库所有用户视图
        /// </summary>
        /// <returns></returns>
        public virtual List<ViewAttribute> GetViews()
        {
            return this.QueryList<ViewAttribute>(@"SELECT o.Name,m.Definition FROM sys.sql_modules AS m INNER JOIN sys.all_objects AS o ON m.object_id = o.object_id WHERE o.[type] = 'v';");
        }
        #endregion

        #region 获取当前数据库所有用户存储过程
        /// <summary>
        /// 获取当前数据库所有用户存储过程
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetProcedures()
        {
            return this.ExecuteDataTable(@"SELECT Name FROM SysObjects Where XType='P' ORDER BY Name;").ToList<string>();
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
            var list = this.ExecuteDataTable(@"
declare @TableName nvarchar(50);
set @TableName = '{0}';
SELECT DISTINCT * FROM (
SELECT 
    --TableName = CASE WHEN C.column_id = 1 THEN O.name ELSE N'' END,
    --TableDesc = ISNULL(CASE WHEN C.column_id = 1 THEN PTB.[value] END,N''),
    SortID = C.column_id,
    Name = C.name,
    IsIdentity = CASE WHEN C.is_identity = 1 THEN N'1' ELSE N'0' END,
    AutoIncrementStep = CASE WHEN C.is_identity = 1 THEN IDENT_INCR(@TableName) ELSE 0 END,
    AutoIncrementSeed = CASE WHEN C.is_identity = 1 THEN IDENT_SEED(@TableName) ELSE 0 END,
    PrimaryKey = ISNULL(IDX.PrimaryKey,N'0'),
    --Computed=CASE WHEN C.is_computed=1 THEN N'1' ELSE N'0' END,
    Type = T.name,
    Bytes = C.max_length,
    Length = COLUMNPROPERTY(C.object_id,C.name,'PRECISION'),
    --Precision = C.precision,
    Digits = C.scale,
    isNull = CASE WHEN C.is_nullable=1 THEN N'1' ELSE N'0' END,
    DefaultValue = ISNULL(D.definition,N''),
    Description = ISNULL(PFD.[value],N''),
    IndexName = ISNULL(IDX.IndexName,N''),
    IndexSort = ISNULL(IDX.Sort,N''),
    IsUnique = ISNULL(IDX.IsUnique,0),
    Create_Date = O.Create_Date,
    Modify_Date = O.Modify_date
FROM sys.columns C
    INNER JOIN sys.objects O
        ON C.[object_id]=O.[object_id]
            --AND (O.type='U' or O.type='V')
            AND O.is_ms_shipped=0
    INNER JOIN sys.types T
        ON C.user_type_id=T.user_type_id
    LEFT JOIN sys.default_constraints D
        ON C.[object_id]=D.parent_object_id
            AND C.column_id=D.parent_column_id
            AND C.default_object_id=D.[object_id]
    LEFT JOIN sys.extended_properties PFD
        ON PFD.class=1 
            AND C.[object_id]=PFD.major_id 
            AND C.column_id=PFD.minor_id
		  --AND PFD.name='Caption'  -- 字段说明对应的描述名称(一个字段可以添加多个不同name的描述)
    LEFT JOIN sys.extended_properties PTB
        ON PTB.class=1 
            AND PTB.minor_id=0 
            AND C.[object_id]=PTB.major_id
		  --AND PFD.name='Caption'  -- 表说明对应的描述名称(一个表可以添加多个不同name的描述)
    LEFT JOIN                       -- 索引及主键信息
    (
        SELECT 
            IDXC.[object_id],
            IDXC.column_id,
            Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending')
                WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END,
            PrimaryKey=CASE WHEN IDX.is_primary_key=1 THEN N'1' ELSE N'0' END,
            IndexName=IDX.Name,
            IsUnique = IDX.is_unique
        FROM sys.indexes IDX
        INNER JOIN sys.index_columns IDXC
            ON IDX.[object_id]=IDXC.[object_id]
                AND IDX.index_id=IDXC.index_id
        LEFT JOIN sys.key_constraints KC
            ON IDX.[object_id]=KC.[parent_object_id]
                AND IDX.index_id=KC.unique_index_id
        INNER JOIN  -- 对于一个列包含多个索引的情况,只显示第1个索引信息
        (
            SELECT [object_id], Column_id, index_id=MIN(index_id)
            FROM sys.index_columns
            GROUP BY [object_id], Column_id
        ) IDXCUQ
            ON IDXC.[object_id]=IDXCUQ.[object_id]
                AND IDXC.Column_id=IDXCUQ.Column_id
                AND IDXC.index_id=IDXCUQ.index_id
    ) IDX
        ON C.[object_id]=IDX.[object_id]
            AND C.column_id=IDX.column_id
 WHERE O.name= @TableName       -- 如果只查询指定表,加上此条件
--ORDER BY O.name,C.column_id;
) as A".format(tableName)).ToList<DataColumns>();
            list.Each(d =>
            {
                var defaultValue = d.DefaultValue;
                if (defaultValue.IsMatch(@"newid"))
                {
                    defaultValue = "UUID";
                }
                else if (defaultValue.IsMatch(@"getdate\(\)"))
                {
                    defaultValue = "NOW";
                }
                else if (defaultValue.IsMatch(@"datediff\(s"))
                {
                    defaultValue = "TIMESTAMP";
                }
                d.DefaultValue = defaultValue;
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
            PageCount = 0; Counts = 0;
            if (tableName == "") return new DataTable();
            DbParameter[] Param = new DbParameter[]{
                    this.MakeParam("@columns",Columns),this.MakeParam("@tableName",tableName),
                    this.MakeParam("@orderColumnName",OrderColumnName),this.MakeParam("@order",OrderType),
                    this.MakeParam("@where",Condition),this.MakeParam("@pageIndex",PageIndex),
                    this.MakeParam("@pageSize",PageSize),this.MakeParam("@pageCount",PageCount,ParameterDirection.Output),
                    this.MakeParam("@counts",Counts,ParameterDirection.Output)
            };
            DataTable data = this.ExecuteDataTable(@"proc_ReadData", CommandType.StoredProcedure, Param);
            if (this.ErrorMessage != null && this.ErrorMessage.IndexOf("找不到存储过程") > -1)
            {
                if (this.ExecuteScalar(@"
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[proc_ReadData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
declare @SQL nvarchar(2000);
set @SQL = '-- =============================================
-- Author:		<Author,Jacky>
-- Create date: <Create Date,2015-07-14>
-- Description:	<Description,通用分页>
-- =============================================
CREATE PROCEDURE [dbo].[proc_ReadData] 
	@columns nvarchar(100), --要显示的列名，用逗号隔开
	@tableName nvarchar(50), --要查询的表名
	@orderColumnName nvarchar(50), --排序的列名
	@order varchar(10), --排序的方式，升序为asc,降序为 desc
	@where nvarchar(2000), --where 条件，如果不带查询条件，请用 1=1
	@pageIndex int, --当前页索引
	@pageSize int, --页大小(每页显示的记录条数)
	@pageCount int output, --总页数，输出参数 
	@counts int output--总条数,输出参数
AS
BEGIN
	SET NOCOUNT ON;
	declare @SQLRecordCount nvarchar(2000); --得到总记录条数的语句
	declare @SQLSelect nvarchar(4000); --查询语句
	
	if(@where = '''')set @where = ''1=1'';
	if(@columns = '''')set @columns='' * '';
	set @SQLRecordCount = N''select @recordCount = count(*) from ''
	+@tableName + '' where ''+ @where;
	declare @recordCount int; --保存总记录条数的变量
	
	exec sp_executeSQL @SQLRecordCount,N''@recordCount int output'',@recordCount output;
	
	set @counts = @recordCount;
	
	--动态 SQL 传参
	if( @recordCount % @pageSize = 0) --如果总记录条数可以被页大小整除
		set @pageCount = @recordCount / @pageSize; --总页数就等于总记录条数除以页大小
	else --如果总记录条数不能被页大小整除
		set @pageCount = @recordCount / @pageSize + 1; --总页数就等于总记录条数除以页大小加1
	--if(@pageIndex > @pageCount)set @pageIndex = @pagecount;
	set @SQLSelect =
	N''select ''+ @columns +'' from (
	select row_number() over (order by ''
	+@orderColumnName +'' ''+ @order
	+'') as tempID,* from ''
	+@tableName+'' where ''+ @where
	+'') as tempTableName where tempID between ''
	+str((@pageIndex - 1)*@pageSize + 1 )
	+'' and ''+ str( @pageIndex * @pageSize);
	exec sp_executeSQL @SQLSelect; --执行动态SQL
END'
exec sp_executeSQL @SQL;
end
select 1;
").ToString().ToInt32() > 0)
                {
                    this.Select(tableName, Columns, Condition, OrderColumnName, OrderType, PageIndex, PageSize, out _, out _);
                }
            }
            PageCount = int.Parse(Param[7].Value.ToString());
            Counts = int.Parse(Param[8].Value.ToString());
            return data;
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
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual Boolean CreateTable(Type modelType, string tableName = "")
        {
            string SQLString = @"
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE ID = OBJECT_ID(N'[dbo].[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
    DROP TABLE [{0}];
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='PK_{0}')
    DROP INDEX {0}.PK_{0};

CREATE TABLE [dbo].[{0}]({1}
) ON [PRIMARY]

{2}
WITH FILLFACTOR = 30;

{3}
";
            TableAttribute Table = modelType.GetTableAttribute();
            Table = Table ?? new TableAttribute();
            if (tableName.IsNullOrEmpty())
                Table.Name = (Table.Name == null || Table.Name.IsNullOrEmpty()) ? modelType.Name : Table.Name;
            else Table.Name = tableName;
            string Fields = "", PrimaryKey = "", Description = "", Unique = "";
            var Indexs = new List<string>();
            DataType dType = new DataType(this.ProviderType);
            modelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
            {
                if (p.GetCustomAttribute<FieldIgnoreAttribute>() != null) return;
                if (",ConnectionString,ConnectionTimeOut,CommandTimeOut,ProviderType,IsTransaction,ErrorMessage,tableName,QueryableX,".IndexOf("," + p.Name + ",") == -1)
                {
                    ColumnAttribute Column = p.GetCustomAttribute<ColumnAttribute>();
                    if (Column != null)
                    {
                        if (Column.DataType.IsNotNullOrEmpty())
                        {
                            switch (Column.DataType.ToString().ToUpper())
                            {
                                case "VARCHAR":
                                    Column.DataType = "varchar";break;
                                    case "NVARCHAR":
                                    Column.DataType = "nvarchar"; break;
                                case "TEXT":
                                    Column.DataType = "text"; break;
                                case "NTEXT":
                                    Column.DataType = "ntext"; break;
                                default:
                                    Column.DataType = p.PropertyType.IsGenericType ? p.PropertyType.GetGenericArguments()[0].Name : p.PropertyType.Name;
                                    break;
                            }
                        }
                    }
                    else 
                        Column = new ColumnAttribute { 
                            AutoIncrement = false, 
                            IsIndex = false, 
                            IsNullable = true, 
                            IsUnique = false, 
                            PrimaryKey = false, 
                            Length = 0,
                            DefaultValue = "",
                            DataType = p.PropertyType.IsGenericType? p.PropertyType.GetGenericArguments()[0].Name: p.PropertyType.Name
                        };
                    Column.Name = (Column.Name == null || Column.Name.IsNullOrEmpty()) ? p.Name : Column.Name;
                    //Column.DataType = Column.DataType.IsNullOrEmpty() ? dType[p.PropertyType] : Column.DataType;
                    Column.DataType = dType[Column.DataType.ToString()];

                    var defaultValue = Column.DefaultValue.ToString();
                    switch (defaultValue.ToUpper())
                    {
                        case "UUID":
                            defaultValue = "newid()";
                            break;
                        case "NOW":
                            defaultValue = "getdate()";
                            break;
                        case "TIMESTAMP":
                            defaultValue = "0";
                            break;
                    }

                    if (Column.Length == 0 && ",varchar,nvarchar,".IndexOf("," + Column.DataType.ToString() + ",", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        Column.Length = 2000;
                    }

                    if (Column.AutoIncrement && Column.DataType.ToString().ToLower() != "uniqueidentifier")
                    {
                        Fields += String.Format(@"
    [{0}] {1} IDENTITY({2},{3}) NOT NULL,", Column.Name, Column.DataType, (defaultValue.IsNullOrEmpty() ? 1 : defaultValue.ToCast<int>()), Column.AutoIncrementStep);
                    }
                    else
                    {
                        Fields += String.Format(@"
    [{0}] {1}{2},",
                                      Column.Name,
                                      Column.DataType,
                                      (((Column.Length == 0 && ",varchar,nvarchar,".IndexOf("," + Column.DataType.ToString().ToLower() + ",") == -1) ||
                                      ",int,bigint,smallint,tinyint,".IndexOf("," + Column.DataType.ToString().ToLower() + ",") > -1
                                      ) ? " " : ("(" + (Column.Length == 0 ? 20 : Column.Length) + ") ")) +
                                      ((Column.IsNullable && !Column.PrimaryKey) ? "NULL" : "NOT NULL") + (defaultValue.IsNullOrEmpty() ? "" : (" DEFAULT (" + ((",int,bigint,smallint,tinyint,".IndexOf("," + Column.DataType.ToString().ToLower() + ",") > -1 || (defaultValue.StartsWith("'") && defaultValue.EndsWith("'")) || defaultValue == "newid()" || defaultValue.ToString().ToLower() == "getdate()") ? defaultValue : ("'" + defaultValue + "'")) + ")")));
                    }
                    if (Column.PrimaryKey)
                    {
                        PrimaryKey = String.Format(@"
                    [{0}] ASC,", Column.Name);
                        if (!Indexs.Contains($"[{Column.Name}]"))
                            Indexs.Add($"[{Column.Name}]");
                    }
                    if (Column.IsUnique) Unique += Column.Name + ",";
                    if (Column.IsIndex && !Indexs.Contains($"[{Column.Name}]")) Indexs.Add($"[{Column.Name}]");
                    Description += @"
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{0}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{1}', @level2type=N'COLUMN',@level2name=N'{2}';".format(Column.Description, Table.Name, Column.Name);
                }
            });
            PrimaryKey = PrimaryKey.TrimEnd(',');
            Unique = Unique.TrimEnd(',');
            if (Unique.IsNotNullOrEmpty()) Fields += "CONSTRAINT [UN_{0}] UNIQUE ({1}),{2}".format(Table.Name, Unique, Environment.NewLine);
            if (PrimaryKey.IsNullOrEmpty()) PrimaryKey = "ID";
            if (PrimaryKey.IsNotNullOrEmpty()) Fields += @"CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED({1})
WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]".format(Table.Name, PrimaryKey);
            var tableIndexs = modelType.GetTableIndexAttributes();
            var SbrIndexs = new StringBuilder();
            if (tableIndexs == null || tableIndexs.Length == 0)
            {
                SbrIndexs.AppendLine($@"
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='IX_{Table.Name}')
    DROP INDEX {Table.Name}.IX_{Table.Name};
CREATE NONCLUSTERED INDEX IX_{Table.Name}
ON {Table.Name}({Indexs.Join(",")})");
            }
            else
            {
                tableIndexs.Each(index =>
                {
                    SbrIndexs.AppendLine($@"
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='{index.Name}')
    DROP INDEX {Table.Name}.{index.Name};
CREATE {index.TableIndexType.ToString().ToUpper()} INDEX {index.Name}
ON {Table.Name}({(from a in index.Keys select a.RemovePattern(@",[a-z]*$").Replace(",", " ").ReplacePattern(@"([^\s]+)(\s+(ASC|DESC))$","[$1]$2")).Join(",")})");
                });
            }
            SQLString = SQLString.format(Table.Name, Fields, SbrIndexs.ToString(), Description);

            base.ExecuteNonQuery(SQLString);
            return true;
        }
        /// <summary>
        /// 创建数据库表 属性用 TableAttribute,ColumnAttribute
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public virtual Boolean CreateTable<T>(string tableName = "") => CreateTable(typeof(T), tableName);
        #endregion

        #region 当前表的所有索引
        /// <summary>
        /// 获取表所有索引
        /// </summary>
        /// <param name="tbName">表名</param>
        /// <returns></returns>
        public List<TableIndexAttribute> GetTableIndexs(string tbName)
        {
            if (tbName.IsNullOrEmpty()) return null;
            var dt = this.ExecuteDataTable("sp_helpindex", CommandType.StoredProcedure, new DbParameter[]
            {
                this.MakeParam(@"objname",tbName)
            });
            if (dt == null || dt.Rows.Count == 0) return null;
            var list = new List<TableIndexAttribute>();
            dt.Rows.Each<DataRow>(a =>
            {
                var index = new TableIndexAttribute
                {
                    TableName = tbName,
                    Name = a["index_name"].ToString()
                };
                var description = a["index_description"].ToString();
                var keys = a["index_keys"].ToString();
                var indexType = description.IsMatch(@"nonclustered") ? TableIndexType.NonClustered : TableIndexType.Clustered;
                if (description.IsMatch(@"primary key"))
                {
                    index.Primary = true;
                    if (description.IsMatch(@"unique"))
                        index.TableIndexType = TableIndexType.Unique;
                }
                index.Keys = (from b in keys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select b.EndsWith("(-)") ? b.Replace("(-)", ",DESC,") : (b + ",ASC,")).ToList();
                list.Add(index);
            });
            return list;
        }
        #endregion

        #region 是否存在表或视图
        /// <summary>
        /// 是否存在表或视图
        /// </summary>
        /// <param name="tableName">表或视图名</param>
        /// <param name="modelType">类型</param>
        /// <returns></returns>
        public Boolean ExistsTable(string tableName, ModelType modelType = ModelType.Table)
        {
            var xtype = "";
            if (modelType == ModelType.Table)
                xtype = "U";
            else if (modelType == ModelType.View)
                xtype = "V";
            else if (ModelType.Procedure == modelType)
                xtype = "P";
            else if (modelType == ModelType.Function)
                xtype = "FN";
            return this.ExecuteScalar("select count(0) from SysObjects where xtype=@xtype and name=@tbName;", CommandType.Text, new DbParameter[]{
                this.MakeParam(@"xtype",xtype),
                this.MakeParam(@"tbName",tableName)
            }).ToCast<int>() > 0;
        }
        #endregion

        #region 创建视图
        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="modelType">类型</param>
        /// <param name="viewName">视图名称</param>
        /// <returns></returns>
        public Boolean CreateView(Type modelType, string viewName = "")
        {
            var type = modelType;
            var view = modelType.GetViewAttribute(false);
            var table = modelType.GetTableAttribute(false);
            if (view == null && table == null) return false;
            if (table != null && table.ModelType != ModelType.View) return false;
            if (view == null) return false;
            if (viewName.IsNotNullOrEmpty()) view.Name = viewName;
            if (view.Definition.IsNullOrEmpty()) return false;
            else
            {
                var count = this.ExecuteScalar($@"SELECT count(0) FROM sys.sql_modules AS m INNER JOIN sys.all_objects AS o ON m.object_id = o.object_id WHERE o.[type] = 'v' and o.Name = '{view.Name}'").ToCast<int>();
                if (count > 0) return true;
                else 
                    this.ExecuteNonQuery($@"CREATE VIEW [dbo].[{view.Name}] AS
    {view.Definition.ReplacePattern(@"ifnull", "ISNULL")};");
                return true;
            }
        }
        #endregion

        #endregion
    }
}