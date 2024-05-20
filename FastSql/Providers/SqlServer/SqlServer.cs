using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.Data.SQL;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-08 14:58:04                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql.Providers
{
    /// <summary>
    /// SqlServer 操作类
    /// </summary>
    public class SqlServer : BaseProvider, IFastSql
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public SqlServer()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="connConfig">数据库连接配置</param>
        public SqlServer(ConnectionConfig connConfig) : base(connConfig)
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// SQL 函数集
        /// </summary>
        private Dictionary<string, string> _SqlFunction;
        /// <summary>
        /// SQL 函数集
        /// </summary>
        public Dictionary<string, string> SqlFunction
        {
            get
            {
                if (this._SqlFunction == null)
                {
                    this._SqlFunction = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","CHARINDEX({1},{0}) > 0"},
                        {"Length","LEN({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} LIKE {1}"},
                        {"LikeSQLX","{0} LIKE {1}"},
                        {"NotLikeSQL","{0} NOT LIKE {1}"},
                        {"NotLikeSQLX","{0} NOT LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","CHARINDEX({1},{0})"},
                        {"CharIndexSQL","CHARINDEX({1},{0})"},
                        {"PatindexSQL","PATINDEX({1},{0})"},
                        {"DateAddSQL","DATEADD({2},{1},{0})"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","CONVERT(VARCHAR, {0}, {1})" },
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"AbsSQL","ABS({0})"},
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LEN({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" },
                        {"CastSQL","CAST({0} as {1})" },
                        {"StDevSQL","STDEV({0})" },
                        {"StDevpSQL","STDEVP({0})" }
                    };
                }
                return this._SqlFunction;
            }
            set => this._SqlFunction = value;
        }
        /// <summary>
        /// 取反 SQL 函数集
        /// </summary>
        private Dictionary<string, string> _SqlUnFunction;
        /// <summary>
        /// 取反 SQL 函数集
        /// </summary>
        public Dictionary<string, string> SqlUnFunction
        {
            get
            {
                if (this._SqlUnFunction == null)
                {
                    this._SqlUnFunction = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","CHARINDEX({1},{0}) <= 0"},
                        {"Length","LEN({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} NOT IN ({1})"},
                        {"NotInSQL","{0} IN ({1})"},
                        {"IndexOf","CHARINDEX({1},{0})"},
                        {"CharIndexSQL","CHARINDEX({1},{0})"},
                        {"PatindexSQL","PATINDEX({1},{0})"},
                        {"DateAddSQL","DATEADD({2},{1},{0})"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","CONVERT(VARCHAR, {0}, {1})" },
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"AbsSQL","ABS({0})"},
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LEN({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" },
                        {"CastSQL","CAST({0} as {1})" },
                        {"StDevSQL","STDEV({0})" },
                        {"StDevpSQL","STDEVP({0})" }
                    };
                }
                return this._SqlUnFunction;
            }
            set => this._SqlUnFunction = value;
        }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override string GetFormatFieldString()
        {
            if (!this.Config.IsFormatField) return string.Empty;
            return this.Config.FormatField.IsNullOrEmpty() ? "[{0}]" : this.Config.FormatField;
        }
        ///<inheritdoc/>
        public override string GetSqlString(ISqlBuilder builder)
        {
            if(builder == null)return string.Empty;
            StringBuilder sbr = new StringBuilder();
            string where = string.Empty, order = string.Empty, group = string.Empty, fieldKeys = string.Empty;
            int? top = 0, skip = 0;
            ICollection<string> fields = null;
            switch (builder.SQLType)
            {
                case SQLType.insert:
                case SQLType.AutoIncrement:
                    sbr.Append("insert into ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    sbr.Append("(");
                    sbr.Append(builder.GetFieldKeys().Select(a => this.FormattingField(a)).Join(","));
                    sbr.Append(")");
                    sbr.Append(" values ");
                    sbr.Append("(");
                    sbr.Append(builder.GetFieldValues().Join(","));
                    sbr.Append(")");
                    if (builder.SQLType == SQLType.AutoIncrement)
                    {
                        sbr.Append(";");
                        sbr.Append("select SCOPE_IDENTITY() as ID");
                    }
                    break;
                case SQLType.update:
                    sbr.Append("update ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    sbr.Append(" set ");
                    sbr.Append(builder.GetFields().Select(a => $"{this.FormattingField(a.Key)}={a.Value.GetSqlValue()}").Join(","));
                    where = builder.GetWhereString();
                    if (where.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(" where ");
                    sbr.Append(where);
                    break;
                case SQLType.truncate:
                    sbr.Append("truncate table ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    break;
                case SQLType.drop:
                    sbr.Append("drop table ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    break;
                case SQLType.delete:
                    sbr.Append("delete from ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    where = builder.GetWhereString();
                    if (where.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(" where ");
                    sbr.Append(where);
                    break;
                case SQLType.select:
                    sbr.Append("select ");
                    top = builder.GetTop().GetValueOrDefault();
                    if (top > 0) sbr.Append($"top {top} ");
                    fields = builder.GetFieldKeys();
                    if (fields == null || fields.Count == 0) 
                        sbr.Append("* ");
                    else
                        sbr.Append(fields.Select(a=>this.FormattingField(a)).Join(","));
                    sbr.Append(" from ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    where = builder.GetWhereString();
                    if (where.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(" where ");
                    sbr.Append(where);
                    order = builder.GetOrderBy();
                    if (order.IsNotNullOrEmpty()) sbr.Append($" order {order}");
                    break;
                case SQLType.exists:
                    sbr.Append("if (exists(select ");
                    top = builder.GetTop().GetValueOrDefault();
                    if (top > 0) sbr.Append($"top {top} ");
                    fields = builder.GetFieldKeys();
                    if (fields == null || fields.Count == 0)
                        sbr.Append("* ");
                    else
                        sbr.Append(fields.Select(a => this.FormattingField(a)).Join(","));
                    sbr.Append(" from ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    where = builder.GetWhereString();
                    if (where.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(" where ");
                    sbr.Append(where);
                    sbr.Append("))");
                    sbr.Append(" select 1;else select 0");
                    break;
                case SQLType.join:

                    break;
                case SQLType.limit:
                    sbr.Append("select ");
                    top = builder.GetTop().GetValueOrDefault();
                    if (top <= 0) top = 10;
                    if (top > 0) sbr.Append($"top {top} ");
                    fields = builder.GetFieldKeys();
                    if (fields == null || fields.Count == 0) {
                        fieldKeys = "*";
                        sbr.Append(fieldKeys);
                    }
                    else {
                        fieldKeys = fields.Select(a => this.FormattingField(a)).Join(",");
                        sbr.Append(fieldKeys.ReplacePattern(@"(\s+as\s+[^\s]+)(,|$)", "$2"));
                    }
                    sbr.Append(" from (select ");
                    skip = builder.GetSkip();
                    if (skip < 0) skip = 0;
                    sbr.Append(top + skip);
                    sbr.Append(" row_number() over(");
                    order = builder.GetOrderBy();
                    if (order.IsNullOrEmpty())
                    {
                        builder.ModelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
                        {
                            var Column = p.GetCustomAttribute<ColumnAttribute>();
                            if (Column == null || !Column.PrimaryKey)
                                return true;
                            else
                            {
                                order = " order by {0} asc".format(this.FormattingField(Column.Name ?? p.Name));
                                return false;
                            }
                        });
                    }
                    if (order.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(") as TempID,");
                    sbr.Append(fieldKeys);
                    sbr.Append(" from ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    where = builder.GetWhereString();
                    if (where.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(" where ");
                    sbr.Append(where);
                    group = builder.GetGroupBy();
                    if (group.IsNotNullOrEmpty()) sbr.Append($" group by {group}");
                    sbr.Append(") as A where A.TempID > ");
                    sbr.Append(skip);
                    break;
                case SQLType.groupby:
                    sbr.Append("select ");
                    fields = builder.GetFieldKeys();
                    if (fields == null || fields.Count == 0)
                        sbr.Append("* ");
                    else
                        sbr.Append(fields.Select(a => this.FormattingField(a)).Join(","));
                    sbr.Append(" from ");
                    sbr.Append(this.FormattingField(builder.TableName));
                    where = builder.GetWhereString();
                    if (where.IsNullOrEmpty()) return string.Empty;
                    sbr.Append(" where ");
                    sbr.Append(where);
                    group = builder.GetGroupBy();
                    if (group.IsNotNullOrEmpty()) sbr.Append($" group by {group}");
                    order = builder.GetOrderBy();
                    if (order.IsNotNullOrEmpty()) sbr.Append($" order {order}");
                    break;
            }
            sbr.Append(";");
            return sbr.ToString();
        }
        /// <summary>
        /// 插入SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetInsertSqlString() { return string.Empty; }
        /// <summary>
        /// 更新SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetUpdateSqlString() { return string.Empty; }
        /// <summary>
        /// 删除SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetDeleteSqlString() { return string.Empty; }
        /// <summary>
        /// 查询SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetSelectcSqlString() { return string.Empty; }
        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns></returns>
        public string ToSql() { return string.Empty; }
        #endregion
    }
}