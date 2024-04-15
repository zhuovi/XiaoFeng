using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using XiaoFeng.Cache;
using XiaoFeng.FastSql;
using XiaoFeng.Json;
/****************************************************************
*  Copyright © (2017) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2017-12-18 11:05:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region 委托
    /// <summary>
    /// 执行完SQL委托方法
    /// </summary>
    /// <param name="sender">对象</param>
    public delegate void RunSQLEventHandler(object sender);
    #endregion

    #region 单表存储结构
    /// <summary>
    /// DataSQL操作类
    /// </summary>
    public class DataSQL
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataSQL()
        {
            this.PageCount = 0;
            this.Counts = 0;
            this.SQLType = SQLType.NULL;
            this.GroupByString = new List<string>();
            this.Columns = new List<string>();
            this.UpdateColumns = new List<object>();
            this.RunSQLTime = 0;
            this.SpliceSQLTime = 0;
            this.LinqToSQLTime = 0;
            this.CacheState = CacheState.Null;
            this.TableSplitConfig = TableSplitConfig.Current;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否缓存
        /// </summary>
        [JsonConverter(typeof(Json.StringEnumConverter))]
        public CacheState CacheState { get; set; }
        /// <summary>
        /// 缓存时长 单位为秒
        /// </summary>
        public int? CacheTimeOut { get; set; }
        /// <summary>
        /// 是否命中缓存
        /// </summary>
        public Boolean IsHitCache { get; set; }
        /// <summary>
        /// 命中缓存次数
        /// </summary>
        public long HitCacheCount { get; set; }
        /// <summary>
        /// 缓存Key
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// Model类型
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// 分表配置
        /// </summary>
        public ITableSplit TableSplit { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        private TableSplitConfig TableSplitConfig { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        private string _TableName;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                if (this._TableName.IsNullOrEmpty())
                {
                    TableAttribute Table = this.ModelType.GetTableAttribute();
                    if (Table == null)
                        this._TableName = this.ModelType.Name;
                    else
                        this._TableName = Table.Name ?? this.ModelType.Name;
                    var tables = this.TableSplitConfig.List;
                    if (tables.Any())
                    {
                        var table = tables.FirstOrDefault(a => a.Name.EqualsIgnoreCase(this._TableName));
                        if (table != null)
                        {
                            this.TableSplit = table;
                        }
                    }
                }
                return this._TableName;
            }
            set
            {
                var val = value;
                if (val.IndexOf(" ") == -1)
                {
                    var tables = this.TableSplitConfig.List;
                    if (tables.Any())
                    {
                        var table = tables.FirstOrDefault(a => a.Name.EqualsIgnoreCase(val));
                        if (table != null)
                        {
                            this.TableSplit = table;
                        }
                    }
                }
                this._TableName = val;
            }
        }
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQLString { get; set; }
        /// <summary>
        /// SQL语句带入存储过程参数
        /// </summary>
        public string SQLParameter { get; set; }
        /// <summary>
        /// 上一次执行SQL语句
        /// </summary>
        public string LastSQLString { get; set; }
        /// <summary>
        /// 上一次执行SQL语句带入存储过程参数
        /// </summary>
        public string LastSQLParameter { get; set; }
        /// <summary>
        /// 显示字段
        /// </summary>
        public List<string> Columns { get; set; }
        /// <summary>
        /// 设置字段
        /// </summary>
        public List<object> UpdateColumns { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public string WhereString { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public Dictionary<string, object> Where { get; set; } = new Dictionary<string, object>();
        /// <summary>
        /// 一页多少条
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// 共有多少页
        /// </summary>
        public int? PageCount { get; set; }
        /// <summary>
        /// 共有多少条
        /// </summary>
        public int? Counts { get; set; }
        /// <summary>
        /// 前多少条
        /// </summary>
        public int? Top { get; set; }
        /// <summary>
        /// 跳过多少条
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string OrderByString { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public List<string> GroupByString { get; set; }
        /// <summary>
        /// 处理类型
        /// </summary>
        [JsonConverter(typeof(Json.StringEnumConverter))]
        public SQLType SQLType { get; set; }
        /// <summary>
        /// FastSql 驱动
        /// </summary>
        public IFastSql FastSql { get; set; }
        /// <summary>
        /// 数据库配置
        /// </summary>
        private ConnectionConfig _Config;
        /// <summary>
        /// 数据库配置
        /// </summary>
        public ConnectionConfig Config
        {
            get => this._Config; set
            {
                this._Config = value;
                var FastSqlProviderType = Type.GetType($"XiaoFeng.FastSql.Providers.{value.ProviderType}");
                if (FastSqlProviderType == null) return;
                this.FastSql = (IFastSql)Activator.CreateInstance(FastSqlProviderType, new object[] { value });
            }
        }
        /// <summary>
        /// 执行SQL时长 单位为毫秒
        /// </summary>
        public Int64 RunSQLTime { get; set; }
        /// <summary>
        /// 拼接SQL时长 单位为毫秒
        /// </summary>
        public Int64 SpliceSQLTime { get; set; }
        /// <summary>
        /// LinqToSql时长
        /// </summary>
        public double LinqToSQLTime { get; set; }
        /// <summary>
        /// 初始化时间
        /// </summary>
        private DateTime BeginTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 存储过程参数集
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        #endregion

        #region 方法

        #region 数据库格式
        /// <summary>
        /// 数据库格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        internal string FieldFormat(string _)
        {
            if (this.Config.IsNullOrEmpty()) return _;
            return DataSQLFun.FieldFormat(this.Config, _);
            /*switch (this.Config.ProviderType)
            {
                case DbProviderType.SqlServer:
                case DbProviderType.OleDb:
                case DbProviderType.SQLite:
                    _ = "[" + _ + "]"; break;
                case DbProviderType.Oracle:
                case DbProviderType.MySql:
                    _ = "`" + _ + "`"; break;
                case DbProviderType.Dameng:
                     break;
                default:
                    break;
            }
            return _;*/
        }
        #endregion

        #region 设置Where
        /// <summary>
        /// 设置Where
        /// </summary>
        /// <param name="where">条件字符串</param>
        public virtual void SetWhere(string where)
        {
            if (where.IsNullOrEmpty()) return;
            if (this.WhereString.IsNullOrEmpty()) this.WhereString = "";
            if (where.IsNotNullOrEmpty()) where = where.RemovePattern(@"^\s*(and|where|or)\s+");
            this.WhereString += "{0}{1}".format(this.WhereString == "" ? "" : " AND ", where);

        }
        #endregion

        #region 设置排序
        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        public virtual void SetOrderBy(string orderString)
        {
            if (orderString.IsNullOrEmpty()) return;
            orderString = orderString.RemovePattern(@"\s+order\s+by\s+").Trim(',');
            if (this.OrderByString.IsNullOrEmpty()) this.OrderByString = "";
            this.OrderByString += "{0}{1}".format(this.OrderByString == "" ? "" : ",", orderString);
        }
        #endregion

        #region 获取拼接后的SQL语句
        /// <summary>
        /// 获取拼接后的SQL语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetSQLString()
        {
            this.LinqToSQLTime = (DateTime.Now - this.BeginTime).TotalMilliseconds;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            string SQL = "";
            this.SQLType = this.SQLType == SQLType.NULL ? SQLType.select : this.SQLType;
            if (this.SQLType != SQLType.join && !this.SQLString.IsNullOrEmpty())
            {
                SQL = this.SQLString; return SQL;
            }
            string SQLTemplate = "", TableName = this.TableName, Columns = this.GetColumns(), _Columns = Columns.ReplacePattern(@"(\s+as\s+[^\s]+)(,|$)", "$2"), OrderByString = this.GetOrderBy();

            switch (this.SQLType)
            {
                case SQLType.select:
                    if ((DbProviderType.SQLite | DbProviderType.MySql | DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.Config.ProviderType))
                        SQLTemplate = "select {Columns} from {TableName} {Where} {GroupBy} {OrderBy} limit 0,{Top};";
                    else
                        SQLTemplate = "select {Top} {Columns} from {TableName} {Where} {GroupBy} {OrderBy};";
                    break;
                case SQLType.insert:
                    SQLTemplate = "insert into {TableName}({Columns}) values({Values});";
                    break;
                case SQLType.AutoIncrement:
                    if (this.Config.ProviderType == DbProviderType.SQLite)
                    {
                        SQLTemplate = "insert into {TableName}({Columns}) values({Values});select last_insert_rowid() as ID;";
                    }
                    else if (this.Config.ProviderType == DbProviderType.MySql)
                    {
                        SQLTemplate = "insert into {TableName}({Columns}) values({Values});select LAST_INSERT_ID() as ID;";
                    }
                    else
                        SQLTemplate = "insert into {TableName}({Columns}) values({Values});select SCOPE_IDENTITY() as ID;";
                    /*
                        IDENT_CURRENT 返回为任何会话和任何作用域中的特定表最后生成的标识值。IDENT_CURRENT 不受作用域和会话的限制，而受限于指定的表。IDENT_CURRENT 返回为任何会话和作用域中的特定表所生成的值。
                        IDENT_CURRENT('表名');
                        @@IDENTITY 返回为当前会话的所有作用域中的任何表最后生成的标识值。
                        SCOPE_IDENTITY 返回为当前会话和当前作用域中的任何表最后生成的标识值
                        SCOPE_IDENTITY();
                     */
                    break;
                case SQLType.update:
                    SQLTemplate = "update {TableName} set {UpdateColumns} {Where};";
                    break;
                case SQLType.delete:
                    SQLTemplate = "delete from {TableName} {Where};";
                    break;
                case SQLType.limit:
                    if ((DbProviderType.SQLite | DbProviderType.MySql | DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.Config.ProviderType))
                    {
                        SQLTemplate = @"select {Column} from {TableName} {Where} {GroupBy} {OrderBy} limit {Limit},{Top}";
                    }
                    else if (this.Config.ProviderType == DbProviderType.OleDb)
                    {
                        SQLTemplate = @"select {Top} {Column} from {TableName} {Wheres} ID not in(select top {Limit} ID from {TableName} {Where} {OrderBy}) {OrderBy}";
                    }
                    else
                    {
                        SQLTemplate = @"select {Top} {Columns} from (
select {Limits} row_number() over({OrderBy}) as TempID, {Column} from {TableName} {Where} {GroupBy}
) as A where A.TempID > {Limit};";
                        if (OrderByString.IsNullOrEmpty() || OrderByString.RemovePattern(@"\s*order\s+by\s+").IsNullOrEmpty())
                        {
                            this.ModelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
                            {
                                ColumnAttribute Column = p.GetCustomAttribute<ColumnAttribute>();
                                if (Column == null || !Column.PrimaryKey)
                                    return true;
                                else
                                {
                                    OrderByString = " order by {0} asc".format(FieldFormat(Column.Name ?? p.Name));
                                    return false;
                                }
                            });
                        }
                    }
                    break;
                case SQLType.exists:
                    if ((DbProviderType.SQLite | DbProviderType.MySql).HasFlag(this.Config.ProviderType))
                    {
                        SQLTemplate = @"if (exists(select {Columns} from {TableName} {Where} {GroupBy} {OrderBy} limit 0,1)) select 1;else select 0;";
                    }
                    else if ((DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.Config.ProviderType))
                    {
                        SQLTemplate = @"declare cnt number;begin select count(0) into cnt from {TableName} {Where} {GroupBy} {OrderBy} limit 0,1;if (cnt > 0) then select 1;else select 0;end if;end;";
                    }
                    else
                        SQLTemplate = @"if (exists(select {Top} {Columns} from {TableName} {Where} {GroupBy} {OrderBy})) select 1;else select 0;";
                    break;
                case SQLType.join:
                    if ((DbProviderType.SQLite | DbProviderType.MySql | DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.Config.ProviderType))
                    {
                        SQLTemplate = @"select {Columns} from {TableName} limit {Limit},{Top}";
                    }
                    else
                    {
                        SQLTemplate = @"select {Top} {Columns} from (
select {Limits} row_number() over({OrderBy}) as TempID, * from 
(
{TableName}
) as YYYY
) as ZZZZ where TempID > {Limit};";
                    }
                    TableName = this.SQLString;
                    break;
                case SQLType.drop:
                    SQLTemplate = "drop table {TableName}"; break;
                case SQLType.truncate:
                    SQLTemplate = "truncate table {TableName}"; break;
                case SQLType.groupby:
                    SQLTemplate = "select {Columns} from {TableName} {Where} {GroupBy} {OrderBy}"; break;
            }
            if (this.Top == -1)
            {
                if (OrderByString.IsNullOrEmpty() || OrderByString.RemovePattern(@"\s*order\s+by\s+").IsNullOrEmpty())
                {
                    this.ModelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
                    {
                        ColumnAttribute Column = p.GetCustomAttribute<ColumnAttribute>();
                        if (Column == null || !Column.PrimaryKey)
                            return true;
                        else
                        {
                            OrderByString = " order by {0} desc".format(FieldFormat(Column.Name ?? p.Name));
                            return false;
                        }
                    });
                }
                else
                {
                    OrderByString = OrderByString.ReplacePattern(@"(\s+)asc([,\s]?)", "$1{asc}$2").ReplacePattern(@"(\s+)desc([,\s]?)", "$1asc$2").ReplacePattern(@"(\s+)\{asc\}([,\s]?)", "$1desc$2");
                }
                this.OrderByString = OrderByString;
                this.Top = 1;
            }
            if (this.GroupByString != null && this.GroupByString.Count > 0)
            {
                if (this.Columns == null) this.Columns = new List<string>();
                this.GroupByString.Each(g =>
                {
                    if (!this.Columns.Contains(g)) this.Columns.Add(g);
                });
            }
            string Wheres = "", Where = this.GetWhere();
            if (this.Config.ProviderType == DbProviderType.OleDb && SQLType == SQLType.limit)
            {
                //Wheres = Where;
                if (Where.IsNullOrWhiteSpace())
                {
                    Wheres = " where ";
                }
                else
                {
                    Wheres = " where " + Where + " and ";
                }
            }
            string ColumnValues = this.GetUpdateColumns();
            SQL = this.SQLString = SQLTemplate.format(new Dictionary<string, string>
            {
                {"Top",this.GetTop() },
                {"Limit",this.GetLimit() },
                {"Limits",this.GetLimits() },
                {"Columns",Columns },
                {"Column",_Columns },
                {"TableName",(TableName.IsMatch(@"select ")?"({0}) as XXXX":FieldFormat("{0}")).format(TableName)},
                {"Where",Where.IsNullOrWhiteSpace()?"":(" where "+ Where) },
                {"GroupBy",this.GetGroupBy() },
                {"OrderBy",OrderByString },
                {"UpdateColumns",ColumnValues },
                {"Values",ColumnValues },
                {"Wheres",Wheres }
            }).ReplacePattern(@"limit \d+,[0\s]*;?\s*$", ";").ReplacePattern(@"\s*(;?)\s*$", "$1");
            /*if (this.Config.ProviderType == DbProviderType.OleDb)
            {
                SQL = this.SQLString = SQL.ReplacePattern(@"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\.\d+", "$1");
                SQL = this.SQLString = SQL.ReplacePattern(@"['""](\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})['""]", "#$1#");
            }
            else if (this.Config.ProviderType == DbProviderType.SqlServer)
            {
                SQL = this.SQLString = SQL.ReplacePattern(@"0001-01-01 00:00:00.000", "1753-01-01 00:00:00.000");
            }
            else if (this.Config.ProviderType == DbProviderType.MySql)
            {
                SQL = this.SQLString = SQL.ReplacePattern(@"0001-01-01 00:00:00.000", "1000-01-01 00:00:00.000");
            }*/
            this.SQLParameter = this.GetSQLParameter();
            sTime.Stop();
            this.SpliceSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLType == SQLType.select || this.SQLType == SQLType.limit || this.SQLType == SQLType.join || this.SQLType == SQLType.groupby)
                this.CreateCacheKey();
            else this.CacheKey = Guid.NewGuid().ToString("N");
            return SQL;
        }
        #endregion

        #region 获取拼接后的SQLParameter
        /// <summary>
        /// 获取拼接后的SQLParameter
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <returns></returns>
        public virtual string GetSQLParameter(string SqlString = "")
        {
            if (SqlString.IsNullOrEmpty() && this.SQLString.IsNullOrEmpty()) return "";
            if (SqlString.IsNullOrEmpty()) SqlString = this.SQLString;

            this.Parameters.Each(a =>
            {
                SqlString = SqlString.ReplacePattern(a.Key + @"([^0-9]|$)", "" + a.Value.GetSqlValue() + "$1");
            });
            return SqlString;
        }
        #endregion

        #region 获取跳过多少条
        /// <summary>
        /// 获取跳过多少条
        /// </summary>
        /// <returns></returns>
        public virtual string GetLimit()
        {
            this.Limit = this.Limit ?? 0;
            return this.Limit.ToString();
        }
        #endregion

        #region 获取要获取多少条
        /// <summary>
        /// 获取要获取多少条
        /// </summary>
        /// <returns></returns>
        public virtual string GetLimits()
        {
            this.Limit = this.Limit ?? 0;
            this.Top = this.Top ?? 0;
            if (Top > 0) return " top " + (this.Limit + this.Top);
            return "";
        }
        #endregion

        #region 获取头几条
        /// <summary>
        /// 获取头几条
        /// </summary>
        /// <returns></returns>
        public virtual string GetTop()
        {
            this.Top = this.Top ?? 0;
            if ((DbProviderType.SQLite | DbProviderType.MySql | DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.Config.ProviderType))
                return this.Top == 0 ? "" : this.Top.ToString();
            else
                return this.Top == 0 ? "" : (" top " + this.Top);
        }
        #endregion

        #region 获取条件
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns></returns>
        public virtual string GetWhere()
        {
            if (this.WhereString.IsNullOrEmpty()) return "";
            else
                return this.WhereString.IsMatch(@"^\s*where\s+") ? this.WhereString.RemovePattern(@"^\s*where\s+") : this.WhereString;
        }
        #endregion

        #region 获取分组数据
        /// <summary>
        /// 获取分组数据
        /// </summary>
        /// <returns></returns>
        public virtual string GetGroupBy()
        {
            if (this.GroupByString == null || this.GroupByString.Count == 0) return "";
            return "group by " + this.GroupByString.Join(",");
        }
        #endregion

        #region 获取排序
        /// <summary>
        /// 获取排序
        /// </summary>
        /// <returns></returns>
        public virtual string GetOrderBy()
        {
            if (this.OrderByString.IsNullOrEmpty()) return "";
            else return this.OrderByString.IsMatch(@"\s*order\s+by\s+") ? this.OrderByString : ("order by " + this.OrderByString);
        }
        #endregion

        #region 获取显示列数据
        /// <summary>
        /// 获取显示列数据
        /// </summary>
        /// <returns></returns>
        public virtual string GetColumns()
        {
            if (this.Columns == null || this.Columns.Count == 0) return "*";
            var fields = from a in this.Columns select this.FieldFormat(a);
            var _columns = fields.Join(",");
            if (_columns.IsMatch(@"@ParamName\d+"))
            {
                _columns.GetPatterns(@"@ParamName\d+").Distinct().Each(a =>
                {
                    var _val = this.Parameters[a].ToString();
                    _columns = _columns.ReplacePattern(@"" + a, (_val.IsFloat() ? "{0}" : "'{0}'").format(_val));
                    this.Parameters.Remove(a);
                });
            }
            return _columns;
        }
        #endregion

        #region 设置显示列数据
        /// <summary>
        /// 设置显示列数据
        /// </summary>
        /// <param name="columns">列</param>
        public virtual void SetColumns(string columns)
        {
            if (columns.IsNullOrWhiteSpace()) return;
            if (this.Columns == null) this.Columns = new List<string>();
            if (!this.Columns.Contains(columns)) this.Columns.Add(columns);
        }
        /// <summary>
        /// 设置显示列数据
        /// </summary>
        /// <param name="list">列集</param>
        public virtual void SetColumns(IEnumerable<string> list)
        {
            if (list == null) return;
            if (this.Columns == null) this.Columns = new List<string>();
            list.Each(a =>
            {
                if (!this.Columns.Contains(a)) this.Columns.Add(a);
            });
            //this.Columns.AddRange(list);
        }
        /*
        /// <summary>
        /// 设置显示列数据
        /// </summary>
        /// <param name="list">列集</param>
        public virtual void SetColumns(string[] list)
        {
            if (list == null) return;
            if (this.Columns == null) this.Columns = new List<string>();
            list.Each(a =>
            {
                if (!this.Columns.Contains(a)) this.Columns.Add(a);
            });
            //this.Columns.AddRange(list);
        }*/
        #endregion

        #region 获取更新列数据
        /// <summary>
        /// 获取更新列数据
        /// </summary>
        /// <returns></returns>
        public virtual string GetUpdateColumns()
        {
            if (this.UpdateColumns == null || this.UpdateColumns.Count == 0) return "";

            if (this.SQLType == SQLType.insert || this.SQLType == SQLType.AutoIncrement)
            {
                string _ = "";
                this.UpdateColumns.Each(v =>
                {
                    _ += v.GetValue() + ",";
                });
                return _.Trim(',');
            }
            else return this.UpdateColumns.Join(",");
        }
        #endregion

        #region 设置更新列数据
        /// <summary>
        /// 设置更新列数据
        /// </summary>
        /// <param name="columns">列</param>
        public virtual void SetUpdateColumns(object columns)
        {
            if (this.SQLType == SQLType.update && (columns == null || columns.ToString().IsNullOrEmpty())) return;
            if (this.UpdateColumns == null) this.UpdateColumns = new List<object>();
            //if ((DbProviderType.Dameng | DbProviderType.MySql).HasFlag(this.Config.ProviderType))
            //    columns = columns.ToString().ReplacePattern(@"@[\s\S]+$", "?");
            if (this.SQLType == SQLType.insert || !this.UpdateColumns.Contains(columns)) this.UpdateColumns.Add(columns);
        }
        /// <summary>
        /// 设置更新列
        /// </summary>
        /// <param name="list">列集</param>
        public virtual void SetUpdateColumns(IEnumerable<string> list)
        {
            if (list == null || !list.Any()) return;
            if (this.UpdateColumns == null) this.UpdateColumns = new List<object>();
            var SQLFunString = DataSQLFun.SQLFun.Join("|").ToRegexUnescape();
            list.Each(a =>
            {
                var _ = a.TrimStart('(');
                var m = _.GetMatches(@"(?<a>(" + SQLFunString + @")\()");
                if (m.Count == 0)
                    _ = _.TrimEnd(')');
                else
                {
                    var num = _.GetMatch(@"(?<a>\)+)$").Length;
                    var nums = _.GetMatches(@"(?<a>\))").Count;
                    var _n = m.Count - (nums - num);
                    if (_n > num)
                        _ += new String(')', _n - num);
                    else if (_n < num)
                        _ = _.RemovePattern(@"\){" + (nums - m.Count) + @"}$");
                }
                _ = _.ReplacePattern(@"\s+is\s+null\s+", " = null");
                if (!this.UpdateColumns.Contains(_)) this.UpdateColumns.Add(_);
            });
            //this.UpdateColumns.AddRange(list);
        }
        /// <summary>
        /// 设置更新列 当时为什么会写两个 2021-06-04
        /// </summary>
        /// <param name="list">列集</param>
        public virtual void SetUpdateColumns_BAK(string[] list)
        {
            if (list == null) return;
            if (this.UpdateColumns == null) this.UpdateColumns = new List<object>();
            list.Each(a =>
            {
                var _ = a.ReplacePattern(@"\s+is\s+null\s+", " = null");
                if (!this.UpdateColumns.Contains(_)) this.UpdateColumns.Add(_);
            });
            //this.UpdateColumns.AddRange(list);
        }
        #endregion

        #region 清空数据
        /// <summary>
        /// 清空数据
        /// </summary>
        public virtual void Clear()
        {
            this.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
            {
                if (p.Name.IsMatch(@"^(providertype|lastsqlstring|lastsqlparameter|datatype|tablename|modeltype|prefix|jointypes|config)$"))
                { }
                else if (p.Name == "SQLString")
                {
                    this.LastSQLString = this.SQLString;
                    if (p.GetValue(this) != null && p.CanWrite) p.SetValue(this, null);
                }
                else if (p.Name == "SQLParameter")
                {
                    this.LastSQLParameter = this.SQLParameter;
                    if (p.GetValue(this) != null && p.CanWrite) p.SetValue(this, null);
                }
                else
                {
                    try
                    {
                        object val = p.GetValue(this);
                        if (val != null && p.CanWrite) p.SetValue(this, null);
                    }
                    catch { }
                }
            });
        }
        #endregion

        #region 复制数据
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="dSQL">DataSQL对象</param>
        public virtual void Done(DataSQL dSQL)
        {
            if (dSQL == null) return;
            DataSQL SQL = dSQL.ToJson().JsonToObject<DataSQL>() ?? default(DataSQL);
            if (SQL == null) return;
            PropertyInfo[] ps = this.GetType().GetProperties();
            PropertyInfo[] pd = SQL.GetType().GetProperties();
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].CanWrite && ps[i].CanRead && !ps[i].IsIndexer())
                {
                    ps[i].SetValue(this, pd[i].GetValue(SQL, null), null);
                }
            }
        }
        /// <summary>
        /// 复制当前对象
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region 缓存操作
        /// <summary>
        /// 创建CacheKey
        /// </summary>
        /// <returns></returns>
        public virtual string CreateCacheKey()
        {
            return this.CacheKey = new { Config = this.Config.ConnectionString, SQLString = this.SQLString, Parameter = this.SQLParameter }.ToJson().MD5();
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        public virtual object GetCacheData<T>() where T : class
        {
            if (this.CacheKey.IsNullOrEmpty()) this.CreateCacheKey();
            object data = CacheFactory.Create(this.Config.CacheType).Get(this.CacheKey);
            /*if (this.Config.CacheType == CacheType.Disk)
            {
                var fileCache = new FileCache();
                data = fileCache.Get(this.CacheKey);
            }
            else
            {
                data = CacheHelper.Get<T>(this.CacheKey);
            }*/
            this.IsHitCache = data != null;
            if (this.IsHitCache)
            {
                var count = CacheHelper.Get("HitCount-" + this.CacheKey);
                this.HitCacheCount = count == null ? 1 : (count.ToCast<long>() + 1);
                if (this.HitCacheCount > 0)
                    CacheHelper.Set("HitCount-" + this.CacheKey, this.HitCacheCount);
            }
            return data;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="data">缓存值</param>
        /// <param name="timeOut">过期时间 单位为秒 0为永久</param>
        public virtual void SetCacheData(object data, int timeOut)
        {
            if (this.CacheKey.IsNullOrEmpty()) this.CreateCacheKey();
            var cache = CacheFactory.Create(this.Config.CacheType);
            if (timeOut < 0)
            {
                cache.Remove(this.CacheKey);
                CacheHelper.Remove("HitCount-" + this.CacheKey);
            }
            else if (timeOut == 0)
            {
                cache.Set(this.CacheKey, data);
                CacheHelper.Set("HitCount-" + this.CacheKey, 0);
            }
            else
            {
                cache.Set(this.CacheKey, data, timeOut * 1000);
                CacheHelper.Set("HitCount-" + this.CacheKey, 0, timeOut * 1000);
            }
            /*DateTime dateTime;
            if (timeOut < 0)
                dateTime = DateTime.Now.AddMinutes(5);
            else if (timeOut == 0)
                dateTime = DateTime.Now.AddYears(1);
            else
                dateTime = DateTime.Now.AddSeconds(timeOut);
            
            if (this.Config.CacheType == CacheType.Memory)
                CacheHelper.Set(this.CacheKey, data, dateTime);
            else if (this.Config.CacheType == CacheType.Disk)
            {
                var fileCache = new FileCache();
                fileCache.Set(this.CacheKey, data);
            }
            else if (this.Config.CacheType == CacheType.Redis)
            {
                new RedisCache().Set(this.CacheKey, data, TimeSpan.FromSeconds(timeOut));
            }*/

        }
        #endregion

        #region 是否要缓存
        /// <summary>
        /// 是否要缓存
        /// </summary>
        /// <param name="cacheData">Model属性</param>
        /// <returns></returns>
        public virtual bool IsCache(CacheDataAttribute cacheData)
        {
            bool IsCache = false;
            if (this.CacheState == CacheState.Null)
            {
                if (cacheData == null)
                {
                    if (this.Config.CacheType != CacheType.No)
                    {
                        IsCache = true;
                        this.CacheState = CacheState.Yes;
                        this.CacheTimeOut = this.Config.CacheTimeOut;
                    }
                }
                else
                {
                    if (cacheData.CacheType != CacheType.No)
                    {
                        IsCache = true;
                        this.CacheState = CacheState.Yes;
                        this.CacheTimeOut = cacheData.TimeOut;
                    }
                }
            }
            else if (this.CacheState == CacheState.Yes)
            {
                IsCache = true;
                this.CacheTimeOut = this.CacheTimeOut ?? this.Config.CacheTimeOut;
            }
            return IsCache;
        }
        #endregion



        #endregion
    }
    #endregion
}