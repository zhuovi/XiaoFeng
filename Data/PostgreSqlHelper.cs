using System;
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
*  Create Time : 2017-06-29 09:11:53                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// MySql 数据库操作类
    /// Verstion : 1.0.0
    /// Create Time : 2018/06/29 09:11:53
    /// Update Time : 2018/06/29 09:11:53
    /// </summary>
    public class PostgreSqlHelper : DataHelper, IDbHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PostgreSqlHelper() { this.ProviderType = DbProviderType.PostgreSql; }
        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public PostgreSqlHelper(string ConnectionString) : this() { this.ConnectionString = ConnectionString; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="connectionConfig">数据库连接配置</param>
        public PostgreSqlHelper(ConnectionConfig connectionConfig) : base(connectionConfig)
        {
            this.ProviderType = DbProviderType.PostgreSql;
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
            return this.ExecuteDataTable(@"select tablename from pg_tables where schemaname = 'public'  order by tablename asc;").ToList<string>();
        }
        #endregion

        #region 获取当前数据库所有用户视图
        /// <summary>
        /// 获取当前数据库所有用户视图
        /// </summary>
        /// <returns></returns>
        public virtual List<ViewAttribute> GetViews()
        {
            return this.QueryList<ViewAttribute>(@"select viewname as Name,Definition as Definition from pg_views where schemaname='public' order by viewname asc;");
        }
        #endregion

        #region 获取当前数据库所有用户存储过程
        /// <summary>
        /// 获取当前数据库所有用户存储过程
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetProcedures()
        {
            return this.ExecuteDataTable(@"select  p.proname from pg_proc p join pg_namespace n on p.pronamespace = n.oid where p.prokind = 'p' and n.nspname not in ('pg_catalog', 'information_schema') order by schema, procedure_name;").ToList<string>();
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
            List<DataColumns> DataColumns = new List<DataColumns>();
            var primarys = this.ExecuteDataTable(@"select kcu.*,
tc.constraint_type,
    kcu.ordinal_position AS pk_position  -- 主键列的顺序
	from 
 information_schema.key_column_usage kcu
  LEFT JOIN
    information_schema.table_constraints tc
    ON kcu.constraint_schema = tc.constraint_schema
    AND kcu.constraint_name = tc.constraint_name
 WHERE kcu.table_name='{0}'".format(tableName));

            this.ExecuteDataTable(@"select cs.*,col_description(a.attrelid, a.attnum) AS column_comment
from 
information_schema.columns as cs
left JOIN 
pg_attribute a on cs.column_name = a.attname
JOIN
  pg_class c ON a.attrelid = c.oid AND c.relname = cs.table_name
 where cs.table_name='{0}' and cs.table_schema='public'
 ;".format(tableName)).Rows.Each<DataRow>(dr =>
            {
                string _CharLength = dr["character_maximum_length"].ToString();
                _CharLength = _CharLength == "null" ? "0" : _CharLength;
                //string ColumnType = dr["COLUMN_TYPE"].ToString();
                int CharLength = _CharLength.ToCast<int>();
                int NumberLength = dr["numeric_precision_radix"].ToCast<int>();
                string type = dr["udt_name"].ToString();
                if (type.Contains("timestamp")) type = "timestamp";
                if (type == "int2")
                    type = "smallint";
                else if (type == "int4")
                    type = "int";
                else if (type == "int8")
                    type = "bigint";

                    var defaultValue = dr["column_default"].ToString();
                if (defaultValue.Contains(@"uuid"))
                {
                    defaultValue = "UUID";
                }
                else if (defaultValue.IsMatch(@"(now)"))
                {
                    defaultValue = "NOW";
                }
                else if (defaultValue.IsMatch(@"EXTRACT(EPOCH FROM CURRENT_TIMESTAMP)"))
                {
                    defaultValue = "TIMESTAMP";
                }
                bool isPRI = false, isUNI = false;
                if (primarys.Rows != null && primarys.Rows.Count > 0)
                {
                    var columnPrimarys = primarys.Rows?.ToList<DataRow>().Where(a => a["column_name"].ToString() == dr["column_name"].ToString());
                    var count = columnPrimarys.Count();
                    if (count > 1)
                    {
                        isPRI = true; isUNI=true;
                    }else if (count == 1)
                    {
                        if (columnPrimarys.First()["constraint_type"].ToString() == "UNIQUE")
                        {
                            isUNI = true;
                        }
                        else
                        {
                            isPRI = true;
                        }
                    }
                }
                
                DataColumns.Add(new DataColumns
                {
                    Name = dr["column_name"].ToString(),
                    IsNull = dr["is_nullable"].ToString() == "YES",
                    DefaultValue = defaultValue.Replace("\"","\\\""),
                    Description = dr["column_comment"].ToString(),
                    Digits = dr["numeric_scale"].ToCast<int>(),
                    IsIdentity = defaultValue.ToString().ToLower().Contains("nextval"),
                    Length = CharLength == 0 ? NumberLength : CharLength,
                    PrimaryKey = isPRI,
                    SortID = dr["ordinal_position"].ToCast<int>(),
                    Type = type,
                    IsUnique = isUNI
                });
            });
            return DataColumns;
        }
        /// <summary>
        /// 获取当前表所有列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataColumnCollection GetDataColumns(string tableName)
        {
            return this.ExecuteDataTable("select * from {0} limit 0,0".format(tableName)).Columns;
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
            return this.Select(tableName, Columns, Condition, OrderColumnName, OrderType, PageIndex, PageSize, out PageCount, out Counts).ToList<T>();
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
            /*DROP TABLE IF EXISTS "public"."ELF_Tb_User";
CREATE TABLE "public"."ELF_Tb_User" (
  "ID" int8 NOT NULL DEFAULT nextval('"ELF_Tb_User_ID_seq"'::regclass),
  "Account" varchar(50) COLLATE "pg_catalog"."default",
  "PassWord" varchar(50) COLLATE "pg_catalog"."default",
  "Sex" int2 DEFAULT 0,
  "Address" varchar(255) COLLATE "pg_catalog"."default",
  "Pass" bool DEFAULT false,
  "AddDate" timestamp(0) DEFAULT now(),
  "AddTimestamp" int8 NOT NULL,
  "Money" numeric(10,2),
  "Bi" bit(1),
  PRIMARY KEY ("ID"),
  UNIQUE ("Account", "PassWord", "ID")
);
COMMENT ON COLUMN "public"."ELF_Tb_User"."ID" IS 'ID';
COMMENT ON COLUMN "public"."ELF_Tb_User"."Account" IS '账号';
COMMENT ON COLUMN "public"."ELF_Tb_User"."PassWord" IS '密码';
COMMENT ON COLUMN "public"."ELF_Tb_User"."Sex" IS '性别';
COMMENT ON COLUMN "public"."ELF_Tb_User"."Address" IS '地址';
COMMENT ON COLUMN "public"."ELF_Tb_User"."Pass" IS '状态';
COMMENT ON COLUMN "public"."ELF_Tb_User"."AddDate" IS '添加时间';
COMMENT ON COLUMN "public"."ELF_Tb_User"."AddTimestamp" IS '时间戳';
COMMENT ON COLUMN "public"."ELF_Tb_User"."Money" IS '余额';
COMMENT ON COLUMN "public"."ELF_Tb_User"."Bi" IS 'BIT';
*/

            string SQLString = @"
DROP TABLE IF EXISTS ""public"".""{0}"";
CREATE TABLE ""public"".""{0}"" ({1}
   {2}
   {3}
);
select 1;
";
            TableAttribute Table = modelType.GetTableAttribute();
            Table = Table ?? new TableAttribute();
            string Fields = "", Indexs = "", PrimaryKey = "";

            if (tableName.IsNullOrEmpty())
                Table.Name = (Table.Name == null || Table.Name.IsNullOrEmpty()) ? modelType.Name : Table.Name;
            else Table.Name = tableName;

            DataType dType = new DataType(this.ProviderType);
            modelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
            {
                if (p.GetCustomAttribute<FieldIgnoreAttribute>() != null) return;
                if (",ConnectionString,ConnectionTimeOut,CommandTimeOut,ProviderType,IsTransaction,ErrorMessage,tableName,QueryableX,".IndexOf("," + p.Name + ",", StringComparison.OrdinalIgnoreCase) > -1) return;
                ColumnAttribute Column = p.GetColumnAttribute();
                Column = Column ?? new ColumnAttribute { AutoIncrement = false, IsIndex = false, IsNullable = true, IsUnique = false, PrimaryKey = false, Length = 0, DefaultValue = "" };
                Column.Name = (Column.Name == null || Column.Name.IsNullOrEmpty()) ? p.Name : Column.Name;
                Column.DataType = Column.DataType.IsNullOrEmpty() ? dType[p.PropertyType] : Column.DataType;

                if (Column.Length == 0)
                {
                    switch (Column.DataType.ToString().ToUpper())
                    {
                        case "CHAR":
                            Column.Length = 255;
                            break;
                        case "VARCHAR":
                        case "NVARCHAR":
                            Column.Length = 3000;
                            break;
                        case "TEXT":
                            Column.Length = 65534;
                            break;
                        case "LONGTEXT":
                            Column.Length = 4294967295;
                            break;
                        case "MEDIUMTEXT":
                            Column.Length = 16777215;
                            break;
                        case "BLOB":
                            Column.Length = 65534;
                            break;
                        case "TINYBLOB":
                            Column.Length = 254;
                            break;
                        case "MEDIUMBLOB":
                            Column.Length = 16777215;
                            break;
                        case "LONGBLOB":
                            Column.Length = 4294967295;
                            break;
                    }
                }

                if (Column.AutoIncrement && Column.PrimaryKey)
                {
                    string type = "int", DefaultValue = "";
                    if (",INTEGER,bigint,int,".IndexOf("," + Column.DataType.ToString() + ",", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        type = "int8";
                        DefaultValue = $@"nextval('""{tableName}_{Column.Name}_seq""'::regclass)";
                    }
                    else if (Column.DataType.ToString() == "uniqueidentifier")
                    {
                        type = "varchar(50)";
                        DefaultValue = "";
                    }else if(Column.DataType.ToString() == "smallint")
                    {
                        type = "int2";DefaultValue = "0";
                    }
                    else if (Column.DataType.ToString() == "int")
                    {
                        type = "int4"; DefaultValue = "0";
                    }
                    else if (Column.DataType.ToString() == "bigint")
                    {
                        type = "int8"; DefaultValue = "0";
                    }
                    else
                    {
                        type = Column.DataType.ToString();
                        DefaultValue = "DEFAULT '{0}' ".format(Column.DefaultValue);
                    }
                    Fields += @"""{0}"" {1} NOT NULL {2} COMMENT '{3}',".format(Column.Name, type, DefaultValue, Column.Description);
                }
                else
                {
                    string DefaultValue = Column.DefaultValue.ToString();
                    if (DefaultValue == "NOW") DefaultValue = "now()";
                    else if (DefaultValue == "UUID")
                    {
                        DefaultValue = "uuid_generate_v4()";
                    }
                    else if (DefaultValue == "TIMESTAMP")
                        DefaultValue = "EXTRACT(EPOCH FROM CURRENT_TIMESTAMP)";
                        //var FieldType = dType[p.PropertyType.GetBaseType()];
                        var FieldType = Column.DataType;
                    Fields += String.Format(@"
    `{0}` {1}{2}{3},",
                    Column.Name,
                    FieldType,
                    ((Column.Length == 0 || ",datetime,".IndexOf("," + FieldType.ToString().ToLower() + ",") > -1) ? " " : ("(" + Column.Length + ") ")) + ((Column.IsNullable && !Column.PrimaryKey) ? "NULL" : "NOT NULL") + (DefaultValue.IsNullOrEmpty() ? "" : (" DEFAULT " + (DefaultValue.IsNumberic() ? DefaultValue : ((DefaultValue.StartsWith("'") && DefaultValue.EndsWith("'")) || DefaultValue == "now()" || Column.Name.ToLower().IndexOf("timestamp") > -1) ? DefaultValue : ("'" + DefaultValue + "'")) + "")),
                     " COMMENT '{0}'".format(Column.Description));
                }
                if (Column.PrimaryKey)
                {
                    PrimaryKey = @",PRIMARY KEY (""{0}"")".format(Column.Name);
                }
                if (Column.IsIndex)
                {
                    Indexs += @",""{0}""".format(Column.Name);
                }
            });
            var SbrIndexs = new StringBuilder();
            var tableIndexs = modelType.GetTableIndexAttributes();
            if (tableIndexs == null || tableIndexs.Length == 0)
            {
                Indexs = Indexs.TrimEnd(',');
                if (Indexs.IsNotNullOrEmpty())
                {
                    SbrIndexs.Append(",UNIQUE ({1})".format(Table.Name, Indexs));
                }
            }
            else
            {
                tableIndexs.Each(index =>
                {
                    SbrIndexs.AppendLine($@",{(index.TableIndexType == TableIndexType.Unique ? "UNIQUE" : "")} INDEX {index.Name} ({(from a in index.Keys select "`" + a.Substring(0, a.IndexOf(",")) + "`").Join(",")})");
                });
            }
            var sql = SQLString.format(Table.Name, Fields.TrimEnd(','), PrimaryKey, SbrIndexs.ToString(), Table.Description.IsNullOrEmpty() ? Table.Name : Table.Description);
            return base.ExecuteScalar(sql).ToString().ToInt16() == 1;
        }
        /// <summary>
        /// 创建数据库表 属性用 TableAttribute,ColumnAttribute
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual Boolean CreateTable<T>(string tableName = "") => CreateTable(typeof(T), tableName);
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
                if (view.Definition.IsNullOrEmpty()) return false;
                else
                {
                    var count = this.ExecuteScalar($@"select count(0) from pg_views where viewname='{table.Name}';").ToCast<int>();
                    if (count > 0) return false;
                    else return this.ExecuteNonQuery($@"CREATE VIEW ""public"".""{view.Name}"" AS
    {view.Definition};") > 0;
                }
            }
        }
        #endregion

        #region 当前表的所有索引
        /// <summary>
        /// 当前表的所有索引
        /// </summary>
        /// <param name="tbName">表名</param>
        /// <returns></returns>
        public List<TableIndexAttribute> GetTableIndexs(string tbName)
        {
            if (tbName.IsNullOrEmpty()) return null;
            var dt = this.ExecuteDataTable(@" select kcu.*,
tc.constraint_type,
    kcu.ordinal_position AS pk_position  -- 主键列的顺序
	from 
 information_schema.key_column_usage kcu
  LEFT JOIN
    information_schema.table_constraints tc
    ON kcu.constraint_schema = tc.constraint_schema
    AND kcu.constraint_name = tc.constraint_name
 WHERE kcu.table_name='@tbname';", CommandType.Text, new DbParameter[]
            {
                this.MakeParam(@"tbname",tbName)
            });
            if (dt == null || dt.Rows.Count == 0) return null;
            var list = new List<TableIndexAttribute>();
            dt.Rows.Each<DataRow>(a =>
            {
                var indexName = a["constraint_name"].ToString();
                var index = new TableIndexAttribute
                {
                    TableName = tbName,
                    Name = indexName
                };
                var findex = list.Find(b => b.Name == indexName);
                if (findex != null)
                {
                    index = findex;
                }
                else
                {
                    if (index.Name == "PRIMARY KEY") index.Primary = true;
                    var UNIQUE = a["UNIQUE"].ToCast<int>();
                    index.TableIndexType = UNIQUE == 0 ? TableIndexType.Unique : TableIndexType.NonClustered;
                    list.Add(index);
                }
                if (index.Keys == null) index.Keys = new List<string>();
                var key = a["column_name"].ToString();
                index.Keys.Add(key);
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
            var sql = "";
            if (modelType == ModelType.Table)
                sql = "select count(0) from pg_tables where tablename=@tbName;";
            else if (modelType == ModelType.View)
                sql = "select count(0) from pg_views where viewname=@tbName;";
            else if (ModelType.Procedure == modelType)
                sql = "select count(0) from pg_proc where proname=@tbName;";
            else if (modelType == ModelType.Function)
                sql = "select count(0) from pg_proc where proname=@tbName;";
            return this.ExecuteScalar(sql, CommandType.Text, new DbParameter[]{
                this.MakeParam(@"tbName",tableName)
            }).ToCast<int>() > 0;
        }
        #endregion

        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~PostgreSqlHelper() { }
        #endregion
    }
}