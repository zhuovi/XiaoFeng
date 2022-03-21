using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using XiaoFeng;
using XiaoFeng.IO;
using System.Reflection;

namespace XiaoFeng.Data
{
    /*
    ===================================================================
        Author : jacky
        Email : jacky @zhuovi.com
        QQ : 7092734
        Site : www.zhuovi.com
        Create Time : 2017/9/22 0:11:53
        Update Time : 2017/9/22 0:11:53
    ===================================================================
    */
    /// <summary>
    /// SQLite 数据库操作类
    /// Verstion : 1.0.0
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2017/9/22 0:11:53
    /// Update Time : 2017/9/22 0:11:53
    /// </summary>
    public class SQLiteHelper : DataHelper, IDbHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SQLiteHelper() { this.ProviderType = DbProviderType.SQLite; }
        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public SQLiteHelper(string ConnectionString) : this() { this.ConnectionString = ConnectionString; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="connectionConfig">数据库连接配置</param>
        public SQLiteHelper(ConnectionConfig connectionConfig) : base(connectionConfig)
        {
            this.ProviderType = DbProviderType.SQLite;
        }
        #endregion

        #region 属性
        /// <summary>
        /// SQLite类型
        /// </summary>
        private Dictionary<string, string> _DataType = null;
        /// <summary>
        /// SQLite类型
        /// </summary>
        public Dictionary<string, string> DataType
        {
            get
            {
                if (this._DataType == null)
                {
                    this._DataType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                    {"bigint,uniqueidentifier","INTEGER"},
                    {"image,binary,tinyint,varbinary","BLOB"},
                    {"Boolean,bit","BOOLEAN"},
                    {"char","CHAR"},
                    {"Date","DATE"},
                    {"Decimal,smallmoney,money","DECIMAL"},
                    {"real,Double,float","DOUBLE"},
                    {"Int64","BIGINT"},
                    {"int,smallint","INT"},
                    {"Object","NONE"},
                    {"numeric","NUMERIC"},
                    {"Double","REAL"},
                    {"String","STRING"},
                    {"ntext,text","TEXT"},
                    {"smalldatetime,DateTime","DATETIME"},
                    {"varchar,nvarchar,nchar","VARCHAR"}
                    };
                }
                return this._DataType;
            }
        }
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type">配置类型</param>
        /// <returns></returns>
        public string getType(string type)
        {
            string _type = type;
            this.DataType.Each(t =>
            {
                if (("," + t.Key.ToLower() + ",").IndexOf("," + type.ToLower() + ",") > -1)
                {
                    _type = t.Value; return false;
                }
                return true;
            });
            return _type;
        }
        #endregion

        #region 方法

        #region 获取当前数据库所有用户表
        /// <summary>
        /// 获取当前数据库所有用户表
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetTables()
        {
            return this.ExecuteDataTable(@"SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;").ToList<string>();
        }
        #endregion

        #region 获取当前数据库所有用户视图
        /// <summary>
        /// 获取当前数据库所有用户视图
        /// </summary>
        /// <returns></returns>
        public virtual List<ViewAttribute> GetViews()
        {
            return this.QueryList<ViewAttribute>(@"SELECT Name,sql as Definition FROM sqlite_master Where type='view' ORDER BY name;");
        }
        #endregion

        #region 获取当前数据库所有用户存储过程
        /// <summary>
        /// 获取当前数据库所有用户存储过程
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetProcedures()
        {
            return new List<string>();
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
            var UniqueKeys = this.QueryList<string>($"select name from sqlite_master where type='index' and sql not null and tbl_name = '{tableName}';");
            if (UniqueKeys == null) UniqueKeys = new List<string>();
            this.ExecuteDataTable(@"pragma table_info ('{0}');".format(tableName)).Rows.Each<DataRow>(dr =>
            {
                var dbType = dr["type"].ToString();
                var Length = 0;
                var Digits = 0;
                var data = dbType.GetMatchs(@"^\s*(?<t>[a-z]+)\s*(\(\s*(?<l>\d+)(\s*\,\s*(?<d>\d+)\s*)?\s*\))?$");
                if (data != null && data.Count > 0)
                {
                    dbType = data["t"];
                    Length = data["l"].ToCast<int>();
                    Digits = data["d"].ToCast<int>();
                }
                /*if (dbType.IsMatch(@"^\s*(nvarchar|varchar|INT|INTEGER)\s*\(\s*\d+\s*\)\s*$"))
                {
                    var d = dbType.GetMatchs(@"^\s*(?<t>(nvarchar|varchar|INT|INTEGER))\s*\(\s*(?<l>\d+)\s*\)\s*$");
                    dbType = d["t"];
                    Length = d["l"].ToCast<int>();
                }
                else if (dbType.IsMatch(@"^\s*(double|decimal|number)\s*\(\s*\d+\s*\,\s*\d+\s*\)\s*$"))
                {
                    var d = dbType.GetMatchs(@"^\s*(?<t>(double|decimal|number))\s*\(\s*(?<l>\d+)\s*\,\s*(?<d>\d+)\s*\)\s*$");
                    dbType = d["t"];
                    Length = d["l"].ToCast<int>();
                    Digits = d["d"].ToCast<int>();
                }*/
                var isIdentity = dbType.ToUpper() == "INTEGER";
                var IsAutoID = isIdentity && dr["pk"].ToString() == "1";
                var defaultValue = dr["dflt_value"].ToString();
                if (defaultValue.IsMatch(@"hex\(randomblob\(4\)"))
                {
                    defaultValue = "UUID";
                }
                else if (defaultValue.IsMatch(@"datetime\('now'"))
                {
                    defaultValue = "NOW";
                }
                else if (defaultValue.IsMatch(@"strftime\('%s', 'now'\)"))
                {
                    defaultValue = "TIMESTAMP";
                }
                
                DataColumns.Add(new DataColumns
                {
                    Name = dr["name"].ToString(),
                    IsNull = dr["notnull"].ToString() == "0",
                    DefaultValue = defaultValue,
                    Description = dr["name"].ToString(),
                    Digits = Digits,
                    IsIdentity = IsAutoID,
                    Length = Length,
                    PrimaryKey = dr["pk"].ToString() == "1",
                    SortID = dr["cid"].ToCast<int>(),
                    Type = dbType,
                    AutoIncrementSeed = IsAutoID ? 1 : 0,
                    AutoIncrementStep = IsAutoID ? 1 : 0,
                    IsUnique = UniqueKeys.Contains(dr["name"])
                });
            });
            return DataColumns;
        }
        /// <summary>
        /// 获取当前表所有列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public virtual DataColumnCollection GetDataColumns(string tableName)
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
            PageCount = 1; Counts = 0;
            return this.Select(tableName, Columns, Condition, OrderColumnName, OrderType, PageIndex, PageSize, out PageCount, out Counts).ToList<T>();
        }
        #endregion

        #region 创建数据库表
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="modelType">model类型</param>
        /// <returns></returns>
        public virtual Boolean CreateTable(Type modelType)
        {
            /*CREATE TABLE A 
                 * (ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                 * A BIGINT DEFAULT (0), 
                 * B BOOLEAN DEFAULT (1),
                 * C CHAR DEFAULT (2),
                 * D DATE DEFAULT (date()),
                 * E DATETIME DEFAULT (datetime()),
                 * F DECIMAL (4, 8) DEFAULT (0),
                 * G DOUBLE (2, 6) DEFAULT (3.567), 
                 * H INT DEFAULT (0), 
                 * I NUMERIC DEFAULT (2), 
                 * J STRING, 
                 * K VARCHAR , 
                 * L TEXT, 
                 * M TIME DEFAULT (time()));*/

            string SQLString = @"
DROP TABLE IF EXISTS {0};
CREATE TABLE {0} (
   {1}
);
DROP INDEX IF EXISTS IX_{0};
CREATE INDEX IX_{0} ON {0} (
    {2}
);
select 1;
";
            TableAttribute Table = modelType.GetTableAttribute();
            Table = Table ?? new TableAttribute();
            string Fields = "", Indexs = "";
            Table.Name = (Table.Name == null || Table.Name.IsNullOrEmpty()) ? modelType.Name : Table.Name;
            DataType dType = new DataType(this.ProviderType);
            modelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
            {
                if (",ConnectionString,ConnectionTimeOut,CommandTimeOut,ProviderType,IsTransaction,ErrorMessage,tableName,".IndexOf("," + p.Name + ",") == -1)
                {
                    ColumnAttribute Column = p.GetColumnAttribute(false);
                    Column = Column ?? new ColumnAttribute { AutoIncrement = false, IsIndex = false, IsNullable = true, IsUnique = false, PrimaryKey = false, Length = 0, DefaultValue = "" };
                    Column.Name = (Column.Name == null || Column.Name.IsNullOrEmpty()) ? p.Name : Column.Name;
                    Column.DataType = Column.DataType.IsNullOrEmpty() ? dType[p.PropertyType] : Column.DataType;
                    if (Column.AutoIncrement && Column.PrimaryKey)
                    {
                        string FieldType = "INTEGER", DefaultValue = "";
                        if (Column.DataType.ToString() == "bigint" || Column.DataType.ToString() == "int")
                        {
                            FieldType = "INTEGER";
                            DefaultValue = "AUTOINCREMENT";
                        }
                        else if (Column.DataType.ToString() == "uniqueidentifier")
                        {
                            FieldType = "STRING";
                            DefaultValue = "DEFAULT('{' || hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4), 1) || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)) || '}')";
                        }
                        else
                        {
                            FieldType = Column.DataType.ToString(); DefaultValue = "DEFAULT({0})".format(Column.DefaultValue);
                        }
                        Fields += "[{0}] {1} UNIQUE NOT NULL PRIMARY KEY ASC {2},".format(Column.Name, FieldType, DefaultValue);
                    }
                    else
                    {
                        string DefaultValue = Column.DefaultValue.ToString();
                        if (DefaultValue == "now()" || DefaultValue == "getdate()") DefaultValue = "datetime('now','localtime')";
                        else if (Column.Name.ToLower().IndexOf("timestamp") > -1)
                        {
                            string date = DefaultValue.GetMatch(@"(?<a>\d{4}-\d{2}-\d{2})");
                            DefaultValue = "strftime('%s','now') - strftime('%s','{0}')".format(date);
                        }
                        Fields += String.Format(@"
                        [{0}] {1}{2}{3},", Column.Name, this.getType(Column.DataType.ToString()), (Column.Length == 0 ? " " : ("(" + Column.Length + ") ")) + ((Column.IsNullable && !Column.PrimaryKey) ? "NULL" : "NOT NULL") + (Column.DefaultValue.ToString() == "" ? "" : (" DEFAULT (" + (DefaultValue.IsNumberic() ? Column.DefaultValue : ((DefaultValue.StartsWith("'") && DefaultValue.EndsWith("'")) || DefaultValue.ToLower() == "datetime('now','localtime')" || Column.Name.ToLower().IndexOf("timestamp") > -1) ? DefaultValue : ("'" + DefaultValue + "'")) + ")")), Column.PrimaryKey ? " PRIMARY KEY ASC" : "");
                    }
                    if (Column.PrimaryKey || Column.IsIndex)
                    {
                        Indexs += Column.Name + " ASC,";
                    }
                }
            });
            Indexs = Indexs.TrimEnd(',');
            return base.ExecuteScalar(SQLString.format(Table.Name, Fields.TrimEnd(','), Indexs.TrimEnd(','))).ToString().ToInt16() == 1;
        }
        /// <summary>
        /// 创建数据库表 属性用 TableAttribute,ColumnAttribute
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public virtual Boolean CreateTable<T>()
        {
            return CreateTable(typeof(T));
        }
        #endregion

        #region 创建视图
        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="modelType">视图model类型</param>
        /// <returns></returns>
        public virtual Boolean CreateView(Type modelType)
        {
            return true;
        }
        /// <summary>
        /// 创建视图
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public virtual Boolean CreateView<T>() => CreateView(typeof(T));
        #endregion

        #region 压缩数据库
        /// <summary>
        /// 压缩数据库
        /// </summary>
        /// <returns></returns>
        public Boolean Compression() => this.ExecuteNonQuery("VACUUM") > 0;
        #endregion

        #region 备份
        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="dest">备份路径</param>
        /// <returns></returns>
        public Boolean Backup(string dest)
        {
            var source = Encrypt.get(this.ConnectionString).GetMatch(@"data source=\s*(.*?)\s*;");
            if (source.IsNullOrEmpty()) return false;
            return FileHelper.CopyFile(source.GetBasePath(), dest.GetBasePath());
        }
        #endregion

        #region 还原
        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="source">源路径</param>
        /// <returns></returns>
        public Boolean Restore(string source)
        {
            var dest = Encrypt.get(this.ConnectionString).GetMatch(@"data source=\s*(.*?)\s*;");
            if (dest.IsNullOrEmpty()) return false;
            return FileHelper.CopyFile(source.GetBasePath(), dest.GetBasePath());
        }
        #endregion

        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~SQLiteHelper() { }
        #endregion
    }
}