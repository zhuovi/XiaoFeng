using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XiaoFeng.Json;
using XiaoFeng.Xml;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 10:18:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region 两表存储结构
    /// <summary>
    /// 两表存储结构
    /// </summary>
    public class DataSQL2 : DataSQL
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataSQL2()
        {
            this.PageCount = 0;
            this.Counts = 0;
            this.SQLType = SQLType.join;
            this.GroupByString = new List<string>();
            this.Columns = new List<string>();
            this.WhereString = new Dictionary<TableType, string>();
            this.ModelType = new Dictionary<TableType, Type>();
            this.TableName = new Dictionary<TableType, string>();
            this.JoinType = JoinType.Left;
        }
        #endregion

        #region 属性
        /// <summary>
        /// Model类型
        /// </summary>
        public new Dictionary<TableType, Type> ModelType { get; set; }
        /// <summary>
        /// 表前缀
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public Dictionary<TableType, string> Prefix = new Dictionary<TableType, string> { { TableType.TResult, "TT" }, { TableType.T1, "A" }, { TableType.T2, "B" }, { TableType.T3, "C" }, { TableType.T4, "D" }, { TableType.T5, "E" }, { TableType.T6, "F" }, { TableType.T7, "G" }, { TableType.T8, "H" }, { TableType.T9, "I" }, { TableType.T10, "J" }, { TableType.T11, "K" }, { TableType.T12, "L" }, { TableType.T13, "M" }, { TableType.T14, "N" }, { TableType.T15, "O" }, { TableType.T16, "P" }, { TableType.T17, "Q" }, { TableType.T18, "R" }, { TableType.T19, "S" }, { TableType.T20, "T" }, { TableType.T21, "U" }, { TableType.T22, "V" }, { TableType.T23, "W" }, { TableType.T24, "X" }, { TableType.T25, "Y" }, { TableType.T26, "Z" } };
        /// <summary>
        /// 表名
        /// </summary>
        public new Dictionary<TableType, string> TableName { get; set; }
        /// <summary>
        /// 表条件
        /// </summary>
        public new Dictionary<TableType, string> WhereString { get; set; }
        /// <summary>
        /// ON条件
        /// </summary>
        public string OnString { get; set; }
        /// <summary>
        /// 联表类型
        /// </summary>
        [JsonConverter(typeof(Json.StringEnumConverter))]
        [XmlConverter(typeof(Xml.StringEnumConverter))]
        public JoinType JoinType { get; set; }
        /// <summary>
        /// JoinType字符串
        /// </summary>
        private Dictionary<JoinType, string> _JoinTypes;
        /// <summary>
        /// JoinType字符串
        /// </summary>
        [JsonIgnore]
        public Dictionary<JoinType, string> JoinTypes
        {
            get
            {
                if (this._JoinTypes == null || this._JoinTypes.Count == 0)
                {
                    this._JoinTypes = new Dictionary<JoinType, string> {
                    { JoinType.Full, " FULL OUTER JOIN " },
                    { JoinType.Left, " LEFT OUTER JOIN " },
                    { JoinType.Right, " RIGHT OUTER JOIN " },
                    { JoinType.Inner, " INNER JOIN " },
                    { JoinType.Union, " UNION ALL " }
                    };
                }
                return this._JoinTypes;
            }
        }
        #endregion

        #region 方法

        #region 获取SQL
        /// <summary>
        /// 获取SQL表
        /// </summary>
        /// <returns></returns>
        public virtual string GetSQLTable()
        {
            return this.GetSQLString("select {Columns} from {TableNameA} {JoinType} {TableNameB} {OnString} {WhereString} {GroupBy} {OrderBy}");
        }
        /// <summary>
        /// 获取SQL
        /// </summary>
        /// <param name="SQLString">SQL模板</param>
        /// <returns></returns>
        public virtual string GetSQLString(string SQLString = "")
        {
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            string SQLTemplate = "";
            if ((DbProviderType.SQLite | DbProviderType.MySql | DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.Config.ProviderType))
            {
                SQLTemplate = @"select {Column} from {TableNameA} {JoinType} {TableNameB} {OnString} {GroupBy} {OrderBy} limit {Limit},{Top}";
            }
            else
            {
                SQLTemplate = @"select {Top} {Column} from (
select {Limits} row_number() over({OrderBy}) as TempID, * from 
(
    select {Columns} from {TableNameA} {JoinType} {TableNameB} {OnString} 
) as YYYY {WhereString}
) as ZZZZ where TempID > {Limit};";
            }
            if (!SQLString.IsNullOrEmpty()) SQLTemplate = SQLString;
            if (this.TableName == null) this.TableName = new Dictionary<TableType, string>();
            if (!this.TableName.ContainsKey(TableType.T1))
            {
                TableAttribute Table = this.ModelType[TableType.T1].GetTableAttribute();
                TableName.Add(TableType.T1,
                    Table == null ?
                    this.ModelType[TableType.T1].Name :
                    Table.Name ?? this.ModelType[TableType.T1].Name);
            }
            if (!this.TableName.ContainsKey(TableType.T2))
            {
                TableAttribute Table = this.ModelType[TableType.T2].GetTableAttribute();
                TableName.Add(TableType.T2,
                    Table == null ?
                    this.ModelType[TableType.T2].Name :
                    Table.Name ?? this.ModelType[TableType.T2].Name);
            }
            string Columns = this.GetColumns(), Jointype = this.JoinTypes[this.JoinType], OrderBy = this.GetOrderBy(), OnString = this.GetOn();
            int Top = this.Top ?? 0;
            string Column = Columns.ReplacePattern(@"[a-z]+\.\[[a-z0-9_-]+\]\s+as\s+\[([a-z-0-9_-]+)\]\s*", "$1");
            /*处理结果集Where*/
            string WhereStrings = "";
            /*判断表是否有条件*/
            if (this.WhereString != null && this.WhereString.Count > 0)
            {
                this.WhereString.Each(w =>
                {
                    if (w.Key == TableType.TResult)
                    {
                        WhereStrings = (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + w.Value;
                    }
                    else
                    {
                        string TableName = this.TableName[w.Key];
                        if (TableName.IsMatch(@"^\s*select\s+"))
                        {
                            this.TableName[w.Key] += (TableName.IsMatch(@"\s+where\s+") ? " and " : " where ") + w.Value;
                        }
                        else
                        {
                            //如果是Left Join 则左表条件放后边 右边条件放on里面 如果是Right Join 右表放条件 左表放 on里面
                            //优化SQL语句，把子条件拿到 on 后执行
                            switch (this.JoinType)
                            {
                                case JoinType.Inner:
                                    //WhereStrings += (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + w.Value.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + this.Prefix[w.Key] + ".$2");
                                    WhereStrings += (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + this.SetColumnPrefix(w.Value, this.Prefix[w.Key]);
                                    break;
                                case JoinType.Right:
                                    if (w.Key == TableType.T1)
                                    {
                                        //OnString += " AND " + w.Value.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + this.Prefix[w.Key] + ".$2");
                                        OnString += " AND " + this.SetColumnPrefix(w.Value, this.Prefix[w.Key]);
                                    }
                                    else if (w.Key == TableType.T2)
                                    {
                                        //WhereStrings += (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + w.Value.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + this.Prefix[w.Key] + ".$2");
                                        WhereStrings += (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + this.SetColumnPrefix(w.Value, this.Prefix[w.Key]);
                                    }
                                    break;
                                case JoinType.Union:
                                case JoinType.Full:
                                    Columns = Columns.ReplacePattern(@"(^|\s|,)[a-z]+\.\[", "$1[");
                                    Column = "*";
                                    this.TableName[w.Key] = "select {0} from {1} where {2}".format(Columns, FieldFormat(TableName), w.Value);
                                    break;
                                case JoinType.Left:
                                default:
                                    if (w.Key == TableType.T1)
                                    {
                                        //WhereStrings += (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + w.Value.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + this.Prefix[w.Key] + ".$2");
                                        /*加上字段前缀，如果条件里有字语句不应该加前缀
                                         2022-01-14 修改
                                         */
                                        WhereStrings += (WhereStrings.IsNullOrEmpty() ? " where " : " and ") + this.SetColumnPrefix(w.Value, this.Prefix[w.Key]);
                                    }
                                    else if (w.Key == TableType.T2)
                                    {
                                        //OnString+= " AND "+ w.Value.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + this.Prefix[w.Key] + ".$2");
                                        OnString += " AND " + this.SetColumnPrefix(w.Value, this.Prefix[w.Key]);
                                    }
                                    break;
                            }
                        }
                    }
                });
            }
            string TableNameA = this.TableName[TableType.T1], TableNameB = this.TableName[TableType.T2];
            if (this.JoinType == JoinType.Union)
            {
                TableNameA = (TableNameA.IsMatch(@"select ") ? "({0}) as " + this.Prefix[TableType.T1] : "{0}").format(TableNameA);
                TableNameB = (TableNameB.IsMatch(@"select ") ? "({0})" : "select * from {0}").format(TableNameB);
            }
            else
            {
                TableNameA = ((TableNameA.IsMatch(@"^\s*select\s+") ? "({0})" : "{0}") + " as " + this.Prefix[TableType.T1]).format(TableNameA);
                TableNameB = ((TableNameB.IsMatch(@"^\s*select\s+") ? "({0})" : "{0}") + " as " + this.Prefix[TableType.T2]).format(TableNameB);
            }

            if (SQLTemplate.IsMatch(@"row_number\(\)"))
                OrderBy = OrderBy.IsNullOrEmpty() ? "order by {0} asc".format(Column.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0]) : OrderBy;

            if (this.Top == -1)
            {
                OrderBy = OrderBy.ReplacePattern(@"(\s+)asc([,\s]?)", "$1{asc}$2").ReplacePattern(@"(\s+)desc([,\s]?)", "$1asc$2").ReplacePattern(@"(\s+)\{asc\}([,\s]?)", "$1desc$2");
                this.OrderByString = OrderBy;
                this.Top = 1;
            }
            /*GroupBy*/
            string GroupBy = this.GetGroupBy();
            /*OrderBy*/
            if (SQLTemplate.IsMatch(@"row_number\(\)"))
                OrderBy = OrderBy.IsNullOrEmpty() ? "order by {0} asc".format(Column.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0]) : OrderBy;

            string SQL = this.SQLString = SQLTemplate.format(new Dictionary<string, string>()
            {
                { "Top", this.GetTop() },
                { "Columns", Columns },
                { "Column", Column },
                { "GroupBy", GroupBy},
                { "OrderBy", OrderBy },
                { "Limit", this.GetLimit() },
                { "OnString", OnString },
                { "JoinType", Jointype },
                { "Limits", this.GetLimits() },
                { "TableNameA", TableNameA },
                { "TableNameB", TableNameB },
                { "WhereString", WhereStrings }
            }).ReplacePattern(@"limit \d+,[0\s]*;?\s*$", ";").ReplacePattern(@"\s*(;?)\s*$", "$1");
            if (this.Config.ProviderType == DbProviderType.OleDb)
            {
                SQL = this.SQLString = SQL.ReplacePattern(@"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\.\d+", "$1");
            }
            sTime.Stop();
            this.SpliceSQLTime += sTime.ElapsedMilliseconds;
            this.CreateCacheKey();
            //if (this.Config.ProviderType != DbProviderType.SqlServer) SQL = SQL.RemovePattern(@"[\[\]]");
            return SQL;
        }
        #endregion

        #region On字符串
        /// <summary>
        /// 设置On字符串
        /// </summary>
        /// <param name="onString">On字符串</param>
        public virtual void SetOn(string onString)
        {
            if (this.OnString.IsNullOrEmpty()) this.OnString = "";
            this.OnString += (this.OnString.IsNullOrEmpty() ? "{0}" : " AND ({0})").format(onString);
        }
        /// <summary>
        /// 获取On字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetOn()
        {
            if (this.OnString.IsNullOrEmpty()) return "";
            return this.OnString.IsMatch(@"\s*on\s+") ? this.OnString : ("ON " + this.OnString);
        }
        #endregion

        #region 设置条件
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="tableType">表类型</param>
        public void SetWhere(string where, TableType tableType = TableType.T1)
        {
            if (where.IsNullOrEmpty()) return;
            if (this.WhereString == null) this.WhereString = new Dictionary<TableType, string>();
            string _WhereString = GetValue(this.WhereString, tableType);
            _WhereString += "{0}{1}".format(_WhereString.IsNullOrEmpty() ? "" : " AND ", where);
            SetValue(this.WhereString, tableType, _WhereString);
        }
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="tableType">表类型</param>
        /// <returns></returns>
        public virtual T GetValue<T>(Dictionary<TableType, T> data, TableType tableType)
        {
            return (data == null || !data.ContainsKey(tableType)) ? default(T) : data[tableType];
        }
        #endregion

        #region 设置值
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="tableType">表类型</param>
        /// <param name="value">值</param>
        public virtual void SetValue<T>(Dictionary<TableType, T> data, TableType tableType, T value)
        {
            if (data == null) data = new Dictionary<TableType, T>();
            if (data.ContainsKey(tableType))
                data[tableType] = value;
            else
                data.Add(tableType, value);
        }
        #endregion

        #region 复制数据
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="dSQL">DataSQL2对象</param>
        public void Done(DataSQL2 dSQL)
        {
            if (dSQL == null) return;
            DataSQL2 SQL = dSQL.ToJson().JsonToObject<DataSQL2>();
            PropertyInfo[] ps = this.GetType().GetProperties();
            PropertyInfo[] pd = SQL.GetType().GetProperties();
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].CanWrite && ps[i].CanRead)
                {
                    ps[i].SetValue(this, pd[i].GetValue(SQL, null), null);
                }
            }
        }
        /// <summary>
        /// 复制当前对象
        /// </summary>
        /// <returns></returns>
        public new virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region 遍历字符
        /// <summary>
        /// 遍历字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public string NextChar(string str)
        {
            var _ = str;
            var index = -1;
            Pair Current = null;
            int fBreak = 0;
            for (var i = 0; i < _.Length; i++)
            {
                var c = _[i];
                switch (c)
                {
                    case '{':
                    case '[':
                    case '(':
                    case '<':
                        if (Current == null)
                            Current = new Pair(c);
                        else
                        {
                            var cp = new Pair(c);
                            cp.ParentPair = Current;
                            if (Current.ChildPair == null)
                                Current.ChildPair = new List<Pair> { cp };
                            else
                                Current.ChildPair.Add(cp);
                            Current = cp;
                        }
                        break;
                    case '}':
                    case ']':
                    case ')':
                    case '>':
                        if (Current == null)
                        {
                            fBreak = -1;
                            break;
                        }
                        Current.IsPair = true;
                        if (Current.ParentPair == null)
                        {
                            fBreak = 1;
                            index = i;
                            break;
                        }
                        Current = Current.ParentPair;
                        break;
                    default:

                        break;
                }
                if (fBreak != 0) break;
            }
            if (index > -1)
                return _.Substring(index + 1);
            return String.Empty;
        }
        #endregion

        #region 设置字段前缀
        /// <summary>
        /// 设置字段前缀
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public StringBuilder SetColumnPrefix(string str, string prefix)
        {
            var sbr = new StringBuilder();
            while (str.IsMatch(@"\s+(IN|EXISTS)\s+"))
            {
                var splitStr = str.GetMatch(@"\s+(IN|EXISTS)\s+");
                var strs = str.Split(new string[] { splitStr }, StringSplitOptions.RemoveEmptyEntries);
                var str0 = strs[0];
                var dic = new Dictionary<string, string>();
                var index = 0;
                var _str = str0.ReplacePattern(@"'[^']*?'", m =>
                {
                    var a = m.Groups[0].Value;
                    dic.Add(index.ToString(), a);
                    return $"{{R:{index++}}}";
                });
                _str = _str.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + prefix + ".$2");
                if (dic.Count > 0)
                {
                    _str = _str.ReplacePattern(@"\{R:(?<a>\d+)\}", m =>
                    {
                        var a = m.Groups["a"].Value;
                        if (dic.TryGetValue(a, out var val))
                        {
                            return val;
                        }
                        return $"{{R:{a}}}";
                    });
                }
                dic.Clear();
                sbr.Append(_str + splitStr);
                //sbr.Append(str0.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + prefix + ".$2") + splitStr);

                var NextString = str.Substring(str0.Length + splitStr.Length);
                str = NextChar(NextString);
                sbr.Append(NextString.Substring(0, NextString.Length - str.Length));
            }
            if (str.IsNotNullOrEmpty())
            {
                var dic = new Dictionary<string, string>();
                var index = 0;
                var _str = str.ReplacePattern(@"'[^']*?'", m =>
                {
                    var a = m.Groups[0].Value;
                    dic.Add(index.ToString(), a);
                    return $"{{R:{index++}}}";
                });
                _str = _str.ReplacePattern(@"(\s|,|\(|^)([\[`""])", "$1" + prefix + ".$2");
                if (dic.Count > 0)
                {
                    _str = _str.ReplacePattern(@"\{R:(?<a>\d+)\}", m =>
                    {
                        var a = m.Groups["a"].Value;
                        if (dic.TryGetValue(a, out var val))
                        {
                            return val;
                        }
                        return $"{{R:{a}}}";
                    });
                }
                dic.Clear();
                sbr.Append(_str);
            }
            return sbr;
        }
        #endregion

        #endregion
    }
    #endregion
}
