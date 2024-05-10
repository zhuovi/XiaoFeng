using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XiaoFeng.Data;
using XiaoFeng.Data.SQL;
using XiaoFeng.Json;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-10 16:52:53                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// SQL 创建器
    /// </summary>
    public class SqlBuilder : ISqlBuilder
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SqlBuilder()
        {

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
                    var tables = this.TableSplitConfig?.List;
                    if (tables != null && tables.Any())
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
        /// 字段值
        /// </summary>
        public Dictionary<string,object> Fields { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public StringBuilder WhereString { get; set; }
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
        public int? Skip { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public List<string> OrderBy { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public List<string> GroupBy { get; set; }
        /// <summary>
        /// 处理类型
        /// </summary>
        [JsonConverter(typeof(Json.StringEnumConverter))]
        public SQLType SQLType { get; set; }
        /// <summary>
        /// FastSql 驱动
        /// </summary>
        private IFastSql FastSql { get; set; }
        /// <summary>
        /// 数据库配置
        /// </summary>
        private ConnectionConfig _Config;
        /// <summary>
        /// 数据库配置
        /// </summary>
        public ConnectionConfig Config
        {
            get => this._Config; 
            set
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

        #region 设置条件
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="where">条件字符串</param>
        public virtual void AddWhere(string where)
        {
            if (where.IsNullOrEmpty()) return;
            if (this.WhereString == null) this.WhereString = new StringBuilder();
            if (this.WhereString.Length > 0)
                this.WhereString.Append(" AND ");
            this.WhereString.Append(where.RemovePattern(@"(^\s*(and|where|or)\s+|\s+(and|where|or)\s*$)"));
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
            if (orderString.IsNullOrEmpty()) return;

            if (this.OrderBy == null) this.OrderBy = new List<string>();
            this.OrderBy.AddRange(orderString.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
        #endregion

        #region 设置前几条
        /// <summary>
        /// 设置前几条
        /// </summary>
        /// <param name="top">获取数据条数</param>
        public virtual void SetTop(int top)
        {
            this.Top = top;
        }
        #endregion

        #region 获取前几条
        /// <summary>
        /// 获取前几条
        /// </summary>
        /// <returns></returns>
        public virtual int? GetTop() => this.Top;
        #endregion

        #region 设置跳过多少条
        /// <summary>
        /// 设置跳过多少条
        /// </summary>
        /// <param name="skip">跳过的条数</param>
        public virtual void SetSkip(int skip)
        {
            this.Skip = skip;
        }
        #endregion

        #region  获取跳过多少条
        /// <summary>
        /// 获取跳过多少条
        /// </summary>
        /// <returns></returns>

        public virtual int? GetSkip() => this.Skip;
        #endregion

        #region 获取条件
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns></returns>

        public virtual string GetWhereString()
        {
            if (this.WhereString == null || this.WhereString.Length == 0) return string.Empty;
            return this.WhereString.ToString();
        }
        #endregion

        #region 获取分组
        /// <summary>
        /// 获取分组
        /// </summary>
        /// <returns></returns>
        public virtual string GetGroupBy()
        {
            if (this.GroupBy == null || this.GroupBy.Count == 0) return string.Empty;
            return this.GroupBy.Join(",");
        }
        #endregion

        #region 获取排序
        /// <summary>
        /// 获取排序
        /// </summary>
        /// <returns></returns>
        public virtual string GetOrderBy()
        {
            if (this.OrderBy == null || this.OrderBy.Count == 0) return string.Empty;
            return this.OrderBy.Join(",");
        }
        #endregion

        #region 设置字段
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="field">字段名称</param>
        /// <param name="value">字段值</param>
        public virtual void SetField(string field, string value)
        {
            if (this.Fields == null) this.Fields = new Dictionary<string, object>();
            this.Fields.Add(field, value);
        }
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="field">字段名称</param>
        public virtual void SetField(string field) => this.SetField(field, string.Empty);
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="fields">字段集</param>
        public virtual void SetField(IEnumerable<KeyValuePair<string,string>> fields)
        {
            if (this.Fields == null) this.Fields = new Dictionary<string, object>();
            fields.Each(f =>
            {
                this.Fields.Add(f.Key, f.Value);
            });
        }
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="fields">字段名称集</param>
        public virtual void SetField(IEnumerable<string> fields)
        {
            if (this.Fields == null) this.Fields = new Dictionary<string, object>();
            fields.Each(f =>
            {
                this.Fields.Add(f, string.Empty);
            });
        }
        #endregion

        #region 获取字段集
        /// <summary>
        /// 获取字段名称集
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<string> GetFieldKeys()
        {
            if (this.Fields == null || this.Fields.Count == 0) return null;
            return this.Fields.Keys;
        }
        /// <summary>
        /// 获取字段集
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string,object> GetFields()
        {
            if (this.Fields == null || this.Fields.Count == 0) return null;
            return this.Fields;
        }
        #endregion

        #region 获取字段值集
        /// <summary>
        /// 获取字段值集
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<object> GetFieldValues()
        {
            if (this.Fields == null || this.Fields.Count == 0) return null;
            return this.Fields.Values;
        }
        #endregion

        #region 清空数据
        /// <summary>
        /// 清空数据
        /// </summary>
        public virtual void Clear()
        {
            this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).Each(p =>
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

        #region 硬复制
        /// <summary>
        /// 硬复制
        /// </summary>
        /// <param name="sqlBuilder">SqlBuilder</param>
        public virtual void Clone(SqlBuilder sqlBuilder)
        {
            if (sqlBuilder == null) return;
            SqlBuilder SQL = sqlBuilder.ToJson().JsonToObject<SqlBuilder>() ?? default(SqlBuilder);
            if (SQL == null) return;
            PropertyInfo[] ps = this.GetType().GetProperties(BindingFlags.Public| BindingFlags.Instance| BindingFlags.IgnoreCase);
            PropertyInfo[] pd = SQL.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].CanWrite && ps[i].CanRead && !ps[i].IsIndexer())
                {
                    ps[i].SetValue(this, pd[i].GetValue(SQL, null), null);
                }
            }
        }
        #endregion

        #region 软复制数据
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region 获取SQL语句
        ///<inheritdoc/>
        public string GetSqlString()
        {
            return this.FastSql.GetSqlString(this);
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// SQL 创建器
    /// </summary>
    /// <typeparam name="T">表类型</typeparam>
    public class SqlBuilder<T> : SqlBuilder, ISqlBuilder<T>
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public SqlBuilder() : base()
        {
            this.ModelType = typeof(T);
        }
        #endregion
    }
    /// <summary>
    /// SQL 创建器
    /// </summary>
    /// <typeparam name="T1">表一类型</typeparam>
    /// <typeparam name="T2">表二类型</typeparam>
    public class SqlBuilder<T1, T2> : SqlBuilder<T1>, ISqlBuilder<T1, T2>
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public SqlBuilder() : base()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 表数据
        /// </summary>
        public List<TableData> TableData { get; set; }
        /// <summary>
        /// 关联表数据
        /// </summary>
        public List<TableJoinData> TableJoinData { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 设置on字符串
        /// </summary>
        /// <param name="tableTypeA">第一表类型</param>
        /// <param name="tableTypeB">第二表类型</param>
        /// <param name="joinType">关联类型</param>
        /// <param name="onString">设置on字符串</param>
        public virtual void SetOn(TableType tableTypeA, TableType tableTypeB,JoinType joinType, string onString)
        {
            if (this.TableJoinData == null) this.TableJoinData = new List<TableJoinData>();
            if (this.TableJoinData.Count == 0)
            {
                var join = new FastSql.TableJoinData(joinType, tableTypeA, tableTypeB);
                join.AddOnString(onString);
                this.TableJoinData.Add(join);
                return;
            }

        }
        /// <summary>
        /// 设置on字符串
        /// </summary>
        /// <param name="name">前缀字符串</param>
        /// <param name="onString">on字符串</param>
        public virtual void SetOn(string name,string onString)
        {
            if (name.IsNullOrEmpty() ||  onString.IsNullOrEmpty()) return;
            if (name.Length != 2) return;
            name = name.OrderBy();
            var tableTypeA = ((int)name[0]).ToEnum<TableType>();
            var tableTypeB = ((int)name[1]).ToEnum<TableType>();
            this.SetOn(tableTypeA, tableTypeB, JoinType.Left, onString);
        }
        #endregion
    }
}