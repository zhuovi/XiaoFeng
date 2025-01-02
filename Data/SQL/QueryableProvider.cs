﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 10:55:55                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region T1
    /// <summary>
    /// Queryable驱动
    /// Verstion : 2.0.5
    /// Create Time : 2017/12/18 10:55:55
    /// Update Time : 2018/06/25 17:50:21
    /// Description : 增加了相关C#扩展接口 如 Replace,Substring,Trim,TrimStart,TrimEnd,StartsWith,EndsWith
    /// v 2.0.3
    /// 增加 boolean值!a.ColumnName 的判断
    /// v 2.0.4
    /// 优化执行Lambda算法
    /// v 2.0.5
    /// 优化执行子IQueryableX的问题
    /// 优化IQueryableX中LinqToSql时增加存储参数效率问题
    /// </summary>
    public class QueryableProvider<T>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public QueryableProvider() { }
        #endregion

        #region 属性
        /// <summary>
        /// 配置数据
        /// </summary>
        public virtual DataSQL DataSQL { get; set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDataHelper DataHelper { get; set; }
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
            if (this.DataHelper == null) return _;
            return DataSQLFun.FieldFormat(this.DataHelper.ConnConfig, _);
            /*switch (this.DataHelper.ProviderType)
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

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="func">Expression</param>
        /// <returns></returns>
        public virtual List<string> SetColumns<TResult>(Expression<Func<T, TResult>> func)
        {
            List<string> list = new List<string>();
            if (func == null) return list;
            if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is BinaryExpression)
                        {
                            BinaryExpression bex = uex.Operand as BinaryExpression;
                        }
                        else if (uex.Operand is MemberExpression mex)
                        {
                            string value = this.ExpressionRouterModel(mex);
                            list.Add("{0} as {1}".format(value, FieldFormat(mex.Member.Name)));
                        }
                    }
                    else if (ex is BinaryExpression)
                    {
                        BinaryExpression bex = ex as BinaryExpression;
                    }
                    else if (ex is MemberExpression mex)
                    {
                        string value = this.ExpressionRouterModel(mex);
                        list.Add("{0} as {1}".format(value, FieldFormat(mex.Member.Name)));
                    }
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = this.ExpressionRouterModel(nex.Arguments[i]);
                    list.Add("{0} as {1}".format(value, FieldFormat(nex.Members[i].Name)));
                }
            }
            else if (func.Body is MemberExpression mex)
            {
                string value = this.ExpressionRouterModel(mex);
                list.Add("{0} as {1}".format(value, FieldFormat(mex.Member.Name)));
            }
            else if (func.Body is MemberInitExpression miex)
            {
                miex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = this.ExpressionRouterModel(b.Expression);
                    list.Add("{0} as {1}".format(value, FieldFormat(b.Member.Name)));
                });
            }
            else if (func.Body is ParameterExpression pex)
            {
                list.Add("[{0}].*".format(pex.Type.Name));
            }
            else if (func.Body is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(fex.IfTrue), ExpressionRouterModel(fex.IfFalse));
                    list.Add(_);
                }
            }
            else
            {
                var Column = this.ExpressionRouterModel(func.Body);

                list.Add(Column + " as GC");
            }
            return list;
        }
        #endregion

        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <param name="orderType">排序类型 asc,desc</param>
        /// <returns></returns>
        public virtual string OrderByString<TModel, TResult>(Expression<Func<TModel, TResult>> func, string orderType = "asc")
        {
            if (func == null) return "";
            string OrderByString = "";
            if (func.Body is NewExpression ex)
            {
                var list = (from a in ex.Members select a.Name).ToList();
                OrderByString += list.Join(",") + " " + orderType;
                /*ex.Members.Each(a =>
                {
                    OrderByString += a.Name + " {0},".format(orderType);
                });*/
            }
            else if (func.Body is MemberExpression mex)
            {
                OrderByString += mex.Member.Name + " " + orderType;
            }
            else if (func.Body is BinaryExpression bex)
            {
                OrderByString += this.ExpressionRouter(bex) + " " + orderType;
            }
            else if (func.Body is NewArrayExpression naex)
            {
                OrderByString += this.ExpressionRouter(naex) + " " + orderType;
            }
            else if (func.Body is MethodCallExpression mce)
            {
                //OrderByString += this.ExpressionRouter(mce).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
                OrderByString += this.ExpressionRouter(mce) + " " + orderType;
            }
            if (OrderByString.IsNullOrEmpty()) return "";
            return OrderByString;
        }
        #endregion

        #region 根据 model 创建表
        /// <summary>
        /// 根据 model 创建表
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <returns></returns>
        public bool CreateTable<TOther>()
        {
            switch (this.DataHelper.ProviderType)
            {
                case DbProviderType.SqlServer:
                    return new SqlHelper { ConnectionString = this.DataHelper.ConnectionString }.CreateTable<TOther>();
                case DbProviderType.SQLite:
                    return new SQLiteHelper { ConnectionString = this.DataHelper.ConnectionString }.CreateTable<TOther>();
                case DbProviderType.MySql:
                    return new MySqlHelper { ConnectionString = this.DataHelper.ConnectionString }.CreateTable<TOther>();
                default: return false;
            }
        }
        #endregion

        #region 处理节点
        /// <summary>
        /// 处理节点
        /// </summary>
        /// <param name="left">左节点</param>
        /// <param name="right">右节点</param>
        /// <param name="type">节点类型</param>
        /// <returns></returns>
        public virtual string BinaryExpressionProvider(Expression left, Expression right, ExpressionType type)
        {
            if (type == ExpressionType.Coalesce)
            {
                string FieldName = ExpressionRouter(left);
                string FieldValue = ExpressionRouter(right);
                return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
            }
            string SubSQL = "(";
            /*先处理左边*/
            var leftSQL = ExpressionRouter(left);
            SubSQL += leftSQL;
            if (left.Type == typeof(bool) && right.Type == typeof(bool) && right.NodeType == ExpressionType.Constant)
                SubSQL = SubSQL.RemovePattern(@" = [01]");
            SubSQL += ExpressionTypeCast(type);
            /*再处理右边*/
            string tmpStr = ExpressionRouter(right);
            if (tmpStr == "null")
            {
                if (SubSQL.Trim().IsMatch(@"\s+=\s*$"))
                    SubSQL = SubSQL.ReplacePattern(@"\s+=\s*$", " is null ");
                else if (SubSQL.Trim().EndsWith("<>"))
                    SubSQL = SubSQL.ReplacePattern(@"<>\s*", " is not null ");
                else
                    SubSQL += "null";
            }
            else
                SubSQL += tmpStr;
            return SubSQL += ")";
        }
        /// <summary>
        /// 处理节点
        /// </summary>
        /// <param name="left">左节点</param>
        /// <param name="right">右节点</param>
        /// <param name="type">节点类型</param>
        /// <returns></returns>
        public virtual string BinaryExpressionProviderModel(Expression left, Expression right, ExpressionType type)
        {
            if (type == ExpressionType.Coalesce)
            {
                string FieldName = ExpressionRouterModel(left);
                string FieldValue = ExpressionRouterModel(right);
                return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
            }
            string SubSQL = "(";
            /*先处理左边*/
            var leftSQL = ExpressionRouterModel(left);
            if (leftSQL == "1") leftSQL = "1 = 1";
            else if (leftSQL == "0") leftSQL = "1 = 0";
            SubSQL += leftSQL;
            if (left.Type == typeof(bool) && right.Type == typeof(bool) && right.NodeType == ExpressionType.Constant)
                SubSQL = SubSQL.RemovePattern(@" = [01]");
            SubSQL += ExpressionTypeCast(type);
            /*再处理右边*/
            string tmpStr = ExpressionRouterModel(right);
            if (tmpStr == "null")
            {
                if (SubSQL.Trim().IsMatch(@"\s+=\s*$"))
                    SubSQL = SubSQL.ReplacePattern(@"\s+=\s*$", " is null ");
                else if (SubSQL.Trim().EndsWith("<>"))
                    SubSQL = SubSQL.ReplacePattern(@"<>\s*", " is not null ");
                else
                    SubSQL += "null";
            }
            else
                SubSQL += tmpStr;
            return SubSQL += ")";
        }
        /// <summary>
        /// 处理节点
        /// </summary>
        /// <param name="left">左节点</param>
        /// <param name="right">右节点</param>
        /// <param name="type">节点类型</param>
        /// <returns></returns>
        public virtual string UnBinaryExpressionProvider(Expression left, Expression right, ExpressionType type)
        {
            if (type == ExpressionType.Coalesce)
            {
                string FieldName = ExpressionRouter(left);
                string FieldValue = ExpressionRouter(right);
                return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
            }
            string SubSQL = "(";
            /*先处理左边*/
            SubSQL += UnExpressionRouter(left);
            if (left.Type == typeof(bool) && right.Type == typeof(bool) && right.NodeType == ExpressionType.Constant)
                SubSQL = SubSQL.RemovePattern(@" = [01]");
            SubSQL += ExpressionTypeCasts(type);
            /*再处理右边*/
            string tmpStr = UnExpressionRouter(right);
            if (tmpStr == "null")
            {
                if (SubSQL.Trim().IsMatch(@"\s+=\s*$"))
                    SubSQL = SubSQL.ReplacePattern(@"\s+=\s*$", " is not null ");
                else if (SubSQL.Trim().EndsWith("<>"))
                    SubSQL = SubSQL.ReplacePattern(@"<>\s*", " is null ");
                else
                    SubSQL += "null";
            }
            else
                SubSQL += tmpStr;
            return SubSQL += ")";
        }
        #endregion

        #region 扩展SQL对象
        /// <summary>
        /// SQL对象集
        /// </summary>
        private Dictionary<string, string> _SqlFun = null;
        /// <summary>
        /// SQL对象集
        /// </summary>
        private Dictionary<string, string> SqlFun
        {
            get
            {
                if (this._SqlFun == null)
                {
                    DataSQLFun fun = new DataSQLFun();
                    this._SqlFun = new Dictionary<string, string>();
                    switch (this.DataHelper.ProviderType)
                    {
                        case DbProviderType.SqlServer:
                            this._SqlFun = fun.MsSqlFun;
                            break;
                        case DbProviderType.MySql:
                            this._SqlFun = fun.MySqlFun;
                            break;
                        case DbProviderType.SQLite:
                            this._SqlFun = fun.SQLiteFun;
                            break;
                        case DbProviderType.Oracle:
                            this._SqlFun = fun.OracleFun;
                            break;
                        case DbProviderType.OleDb:
                            this._SqlFun = fun.OledbFun;
                            break;
                        case DbProviderType.Dameng:
                            this._SqlFun = fun.DamengFun;
                            break;
                        default:
                            this._SqlFun = fun.MsSqlFun;
                            break;
                    }
                }
                return this._SqlFun;
            }
        }
        private Dictionary<string, string> _SqlUnFun = null;
        /// <summary>
        /// SQL对象集
        /// </summary>
        private Dictionary<string, string> SqlUnFun
        {
            get
            {
                if (this._SqlUnFun == null)
                {
                    DataSQLFun fun = new DataSQLFun();
                    this._SqlFun = new Dictionary<string, string>();
                    switch (this.DataHelper.ProviderType)
                    {
                        case DbProviderType.SqlServer:
                            this._SqlFun = fun.MsSqlUnFun;
                            break;
                        case DbProviderType.MySql:
                            this._SqlFun = fun.MySqlUnFun;
                            break;
                        case DbProviderType.SQLite:
                            this._SqlFun = fun.SQLiteUnFun;
                            break;
                        case DbProviderType.Oracle:
                            this._SqlFun = fun.OracleUnFun;
                            break;
                        case DbProviderType.OleDb:
                            this._SqlFun = fun.OledbUnFun;
                            break;
                        default:
                            this._SqlFun = fun.MsSqlUnFun;
                            break;
                    }
                }
                return this._SqlUnFun;
            }
        }
        #endregion

        #region Lambda取值
        /// <summary>
        /// Lambda取值
        /// </summary>
        /// <param name="val">值</param>
        /// <returns></returns>
        public virtual string GetLambdaValue(object val)
        {
            if (val == null) return "null";
            if (val is IQueryableX x) return GetIQueryableXSQL(x);
            Type type = val.GetType();
            ValueTypes types = type.GetValueType();
            if (types == ValueTypes.Value || types == ValueTypes.String)
                return this.GetParamName(val);
            else if (types == ValueTypes.Array || types == ValueTypes.ArrayList || types == ValueTypes.List || types == ValueTypes.IEnumerable)
            {
                string _ = string.Empty;
                //var s = XiaoFeng.Threading.StopWatch.GetTime(() =>
                //{
                foreach (var o in val as IEnumerable)
                {
                    _ += "{0},".format(this.GetParamName(o));
                }
                //});
                //LogHelper.Info($"运行时长:{s}");
                return _.TrimEnd(',');
            }
            return this.GetParamName(val);
        }
        #endregion

        #region 设置存储过程参数
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public virtual string GetParamName(object value)
        {
            if (value == null) return "null";
            if (value is string str && str.IsNullOrEmpty()) return "''";
            if (value.ToString().IsMatch(@"^@Sub_(\d+_)?ParamName\d+$"))
                return value.ToString();
            if (value is bool) value = Convert.ToInt32(value);
            else if (value is Guid guid) value = guid;
            //var ParaName = "@ParamName" + (this.DataSQL.Parameters.Count + 1);
            //var ParamIndex = 1;
            if (this.DataSQL.Parameters == null) this.DataSQL.Parameters = new Dictionary<string, object>();

            /*
             * 2022-11-18 11:16
             * 前期为了 让参数ID号不间断，只是为了好看，性能极具下降，现移除此功能，以参数总量+1
             * var ParamItems = this.DataSQL.Parameters.Where(a => a.Key.StartsWith("@ParamName")).Select(a => a.Key.Replace("@ParamName", "").ToCast<int>());
            if (ParamItems.Count() > 0)
                ParamIndex = ParamItems.Max() + 1;
            var ParaName = $"@ParamName{(this.DataSQL.Parameters.Count == 0 ? 1 : ParamIndex)}";
            */
            var ParaName = $"@ParamName{this.DataSQL.Parameters.Count + 1}";
            this.AddParam(ParaName, value);
            return ParaName;
            //return (this.DataHelper.ProviderType == DbProviderType.Dameng || this.DataHelper.ProviderType == DbProviderType.MySql) ? "?" : ParaName;
        }
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">值</param>
        public virtual void AddParam(string name, object value)
        {
            if (value == null) return;
            if (value is Guid || value is DateTime) value = value.GetValue();
            if (value is bool) value = Convert.ToInt32(value);
            if (this.DataSQL.Parameters == null || !this.DataSQL.Parameters.Any())
            {
                this.DataSQL.Parameters = new Dictionary<string, object>()
                {
                    { name , value }
                };
                return;
            }
            if (!this.DataSQL.Parameters.ContainsKey(name))
                this.DataSQL.Parameters.Add(name, value);
            else
                this.DataSQL.Parameters[name] = value;
        }
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="name">参数错</param>
        public virtual void RemoveParam(string name)
        {
            if (name.IsNullOrEmpty()) return;
            if (this.DataSQL.Parameters == null || !this.DataSQL.Parameters.Any()) return;
            if (this.DataSQL.Parameters.ContainsKey(name))
                this.DataSQL.Parameters.Remove(name);
        }
        /// <summary>
        /// 获取存储过程参数值
        /// </summary>
        /// <returns></returns>
        public virtual DbParameter[] GetDbParameters()
        {
            if (this.DataSQL.Parameters == null || this.DataSQL.Parameters.Count == 0) return Array.Empty<DbParameter>();
            var Params = new DbParameter[this.DataSQL.Parameters.Count];
            var i = 0;

            this.DataSQL.Parameters.Each(a =>
            {
                Params[i++] = this.DataHelper.MakeParam(a.Key, a.Value);
            });
            return Params;
        }
        /// <summary>
        /// 获取存储过程参数值 根据SQL参数排序
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        public virtual DbParameter[] GetDbParameters(string SQLString)
        {
            DbParameter[] Params;
            var _Params = this.GetDbParameters();
            if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
            {
                Params = new DbParameter[_Params.Length];
                var i = 0;
                SQLString.GetMatches(@"@(Sub_(\d+_)?)?ParamName\d+").Each(b =>
                {
                    Params[i++] = _Params.Where(a => a.ParameterName.Trim(new char[] { '@', ':' }) == b["0"].Trim(new char[] { '@', ':' })).FirstOrDefault();
                });
                return Params;
            }
            else
                return _Params;
        }
        /// <summary>
        /// 获取Param值
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <returns></returns>
        public virtual object GetParamValue(string ParamName)
        {
            if (ParamName.IsNullOrEmpty()) return "";
            return ParamName == "?" ? this.DataSQL.Parameters.LastOrDefault().Value : this.DataSQL.Parameters.TryGetValue((ParamName.IsMatch(@"^@") ? "" : "@") + ParamName, out var val) ? val : "";
        }
        /// <summary>
        /// 获取所有的参数集
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, object> GetParameters()
        {
            return this.DataSQL.Parameters;
        }
        #endregion

        #region 设置规则
        /// <summary>
        /// 设置规则
        /// </summary>
        /// <param name="exp">节点</param>
        /// <returns></returns>
        public virtual string ExpressionRouter(Expression exp)
        {
            string SubSQL = string.Empty;
            if (exp is BinaryExpression be)
            {
                return BinaryExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression me)
            {
                if (me.ToString().IsMatch(@"\s*value\([\s\S]+?\)") || me.ToString().IsMatch(@"^DateTime\.Now"))
                {
                    return this.GetLambdaValue(this.Eval(me));
                }
                else if (me.Expression != null && me.Expression.GetType().Name == "TypedParameterExpression")
                {
                    if (me.Member.Name == "Now")
                    {
                        return this.GetParamName(DateTime.Now);
                    }
                    else
                        return FieldFormat(me.Member.Name)/* + (me.Type == typeof(bool) ? " = 1" : "")*/;
                }
                else if (me.Expression is MemberExpression mex)
                {
                    return FieldFormat(mex.Member.Name);
                }
                else
                {
                    return this.GetLambdaValue(this.Eval(me));
                }
            }
            else if (exp is NewArrayExpression ae)
            {
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex));
                    tmpstr.Append(',');
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                var MethodName = mce.Method.Name;
                if (this.SqlFun.TryGetValue(MethodName, out string SqlFun))
                {
                    List<string> args = new List<string>();
                    /*if (mce.Object != null)
                    {
                        var o = mce.Arguments.FirstOrDefault(a => a == mce.Object);
                        if (o == null) args.Add(ExpressionRouter(mce.Object));
                    }*/
                    mce.Arguments.Each(a =>
                    {
                        args.Add(ExpressionRouter(a));
                    });

                    if (MethodName.IsMatch(@"^(IndexOf|StartsWith|EndsWith|Substring|Replace|Sum|Contains|ToLower|ToUpper|Trim|TrimStart|TrimEnd|Length)$"))
                    {
                        var o = ExpressionRouter(mce.Object);
                        if (!args.Contains(o)) args.Insert(0, o);
                    }
                    if (SqlFun.IsMatch(@"^(DATEDIFF|DATEPART|TIMESTAMPDIFF|DATE_FORMAT|DATEADD)\("))
                    {
                        var b = args.Last();
                        args.RemoveAt(args.Count - 1);
                        var dfType = this.GetParamValue(b).ToString();
                        var _dfType = dfType.ToLower();
                        if ((DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.DataHelper.ProviderType))
                        {
                            if (_dfType == "yy" || _dfType == "yyyy")
                                dfType = "YEAR";
                            else if (_dfType == "qq" || _dfType == "q")
                                dfType = "QUARTER";
                            else if (_dfType == "mm" || _dfType == "m")
                                dfType = "MOUTH";
                            else if (_dfType == "dd" || _dfType == "d")
                                dfType = "DAY";
                            else if (_dfType == "wk" || _dfType == "ww" || _dfType == "w")
                                dfType = "WEEK";
                            else if (_dfType == "hh")
                                dfType = "HOUR";
                            else if (_dfType == "mi" || _dfType == "n")
                                dfType = "MINUTE";
                            else if (_dfType == "ss" || _dfType == "s")
                                dfType = "SECOND";
                            else if (_dfType == "ms")
                                dfType = "MICROSECOND";
                        }
                        else if (this.DataHelper.ProviderType == DbProviderType.MySql)
                        {
                            if (SqlFun.IsMatch(@"^(DATEPART|DATE_FORMAT)\("))
                            {
                                if (_dfType == "yy")
                                    dfType = "'%y'";
                                else if (_dfType == "yyyy")
                                    dfType = "'%Y'";
                                else if (_dfType == "qq" || _dfType == "q")
                                    dfType = "QUARTER";
                                else if (_dfType == "mm" || _dfType == "m")
                                    dfType = "'%m'";
                                else if (_dfType == "dd")
                                    dfType = "'%D'";
                                else if (_dfType == "d")
                                    dfType = "'%d'";
                                else if (_dfType == "wk" || _dfType == "ww" || _dfType == "w")
                                    dfType = "'%U'";
                                else if (_dfType == "hh")
                                    dfType = "'%k'";
                                else if (_dfType == "h")
                                    dfType = "'%l'";
                                else if (_dfType == "mi" || _dfType == "n")
                                    dfType = "'%i'";
                                else if (_dfType == "ss" || _dfType == "s")
                                    dfType = "'%S'";
                                else if (_dfType == "ms")
                                    dfType = "'%f'";
                                else
                                    dfType = $"'{dfType.Trim('\'')}'";
                            }
                        }
                        this.RemoveParam(b);
                        if (!MethodName.EqualsIgnoreCase("DateAddSQL") && this.DataHelper.ProviderType == DbProviderType.Dameng)
                        {
                            b = args.Last();
                            args.RemoveAt(args.Count - 1);
                            var val = this.GetParamValue(b).ToCast<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.fff");
                            args.Add("'" + val + "'");
                            this.RemoveParam(b);
                        }
                        args.Add(dfType);
                    }
                    else if (SqlFun.IsMatch("POSITION"))
                    {
                        var b = "";
                        if (this.DataHelper.ProviderType == DbProviderType.Dameng)
                        {
                            b = args.Last();
                            args.RemoveAt(args.Count - 1);
                            args.Add("'" + this.GetParamValue(b) + "'");
                            this.RemoveParam(b);
                        }
                    }
                    else if (SqlFun.IsMatch(@"julianday"))
                    {
                        var val = this.GetParamValue(args.Last()).ToString();
                        //格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns
                        if (val == "dd" || val == "d" || val == "dy" || val == "y")
                            val = "*1";
                        else if (val == "wk" || val == "ww" || val == "dw" || val == "w")
                            val = "/7";
                        else if (val == "hh")
                            val = "*24";
                        else if (val == "mi" || val == "n")
                            val = "*24*60";
                        else if (val == "ss" || val == "s")
                            val = "*24*60*60";
                        else if (val == "ms")
                            val = "*24*60*60*1000";
                        else if (val == "mcs")
                            val = "*24*60*60*1000*1000";
                        else if (val == "ns")
                            val = "*24*60*60*1000*1000*1000";
                        if (val == "yy" || val == "yyyy")
                            SqlFun = "strftime('%Y','{1}') - strftime('%Y','{0}')";
                        else if (val == "mm" || val == "m")
                            SqlFun = "((strftime('%Y','{1}') - strftime('%Y','{0}'))*12 + strftime('%m','{1}') - strftime('%m','{0}'))";
                        else
                        {
                            args.RemoveAt(args.Count - 1);
                            args.Add(val);
                        }
                    }
                    else if (SqlFun.IsMatch(@" like "))
                    {
                        var ev = this.Eval(mce.Arguments[1]);

                        var b =/* args.Last();*/(ev is IQueryableX x) ? GetIQueryableXSQL(x) : ev.ToString();
                        b = b.Trim('\'');
                        string _Start = "", _End = "";
                        if (mce.Method.Name == "StartsWith") _End = "%";
                        else if (mce.Method.Name == "EndsWith") _Start = "%";
                        if (b.IsNotNullOrWhiteSpace())
                        {
                            Dictionary<string, string> d = b.GetMatchs(@"^(?<a>[%_]?)(?<b>[\s\S]*?)(?<c>[%_]?)$");
                            string val = d["b"];
                            /*
                             * 2022-03-01
                             * 由于查询时 里面 %不让转义 就是真的 模糊 查询
                             * 暂时增加一个方法来处理
                             */
                            if (mce.Method.Name.EqualsIgnoreCase("likeSQL") || mce.Method.Name.EqualsIgnoreCase("NotlikeSQL"))
                            {
                                if (this.DataHelper.ProviderType == DbProviderType.SqlServer) val = val.ReplacePattern(@"([%_\[\]\(\)])", "[$1]");
                            }
                            b = d["a"] + val.GetValue() + d["c"];
                        }
                        args.Last().GetMatches(@"(?<a>@ParamName\d+)").Each(a =>
                        {
                            RemoveParam(a["a"]);
                        });
                        args.Insert(args.Count - 1, "" + _Start + b + _End + "");
                    }
                    else if (SqlFun.IsMatch(@"\s+IN\s+\("))
                    {
                        if (args.Count <= 1 || args[1].IsNullOrEmpty() || args[1] == "''" || args[1] == "null") return "1 = 0";
                    }
                    else if (SqlFun.IsMatch(@"CHARINDEX\("))
                    {
                        var _BaseType = mce.Arguments[0].Type.GetValueType();
                        if (mce.Method.Name == "Contains" && (_BaseType == ValueTypes.Array ||
                            _BaseType == ValueTypes.ArrayList || _BaseType == ValueTypes.IEnumerable || _BaseType == ValueTypes.List))
                        {
                            SqlFun = SqlFun.ReplacePattern(@"\{1\}", "','+ Convert(nvarchar(500),{1}) +','").ReplacePattern(@"\{0\}", "',{0},'");
                            var _val = args[0];
                            _val = _val.ReplacePattern(@"(@ParamName\d+)", "{$1}").format(this.GetParameters());
                            args[0] = _val;
                        }
                    }
                    else if (SqlFun.IsMatch(@"CAST\("))
                    {
                        var b = args.Last();
                        args.RemoveAt(args.Count - 1);
                        args.Add(this.GetParamValue(b).ToString());
                        this.RemoveParam(b);
                    }
                    if ((DbProviderType.Dameng).HasFlag(this.DataHelper.ProviderType))
                    {
                        args.Each(a =>
                        {
                            var ParamName = a;
                            if (a.IsMatch(@"^[@:]?ParamName\d+$"))
                            {
                                a = "'" + GetParamValue(a).ToString() + "'";
                                //RemoveParam(ParamName);
                            }
                        });
                    }
                    var _ = SqlFun.format(args.ToArray());
                    return _;
                }
                else if (mce.Type.Name == typeof(IQueryableX<>).Name)
                {
                    //var val = this.Eval(mce) as IQueryableX;
                    return GetIQueryableXSQL(this.Eval(mce) as IQueryableX);
                    //return val == null ? "" : val.GetValue();
                }
                //else if (mce.Type.Name == "Guid" && mce.Method.Name == "NewGuid")
                //{
                //    return this.GetParamName(Guid.NewGuid());
                //}
                //else if (mce.Method.Name == "Parse")
                //{
                //    return this.GetLambdaValue(this.Eval(mce));
                //}
                //else if (mce.Type.Name == "String" && mce.Method.Name == "ToString")
                //{
                //    return "Convert(nvarchar(40)," + ExpressionRouter(mce.Arguments.Count == 0 ? mce.Object : mce.Arguments[0]) + ")";
                //}
                else return this.GetLambdaValue(this.Eval(mce));
            }
            else if (exp is ConstantExpression ce)
            {
                object value = null;
                if (ce.Value == null)
                    return "null";
                else if (ce.Value.ToString() == "")
                    return "''";
                else if (ce.Value is bool)
                    value = Convert.ToInt32(ce.Value);
                else if (ce.Value is DateTime || ce.Value.ToString().IsDate() || ce.Value.ToString().IsDateOrTime() || ce.Value is Guid || ce.Value is string || ce.Value is char)
                    value = ce.Value;
                else if (ce.Value is ValueType)
                    value = ce.Value;
                else
                    value = ce.Value;
                return this.GetParamName(value);
            }
            else if (exp is UnaryExpression ue)
            {
                string val = ExpressionRouter(ue.Operand);
                if (ue.Type == typeof(bool) && ue.NodeType == ExpressionType.Not)
                {
                    if (val.IndexOf("=") > -1)
                        val = val.ReplacePattern(@" = 1", " = 0");
                    else
                        val += " = 0";
                }
                return val;
            }
            else if (exp is NewExpression nexp)
            {
                return ExpressionRouter(nexp.Arguments[0]);
            }
            else if (exp is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouter(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouter(fex.IfTrue), ExpressionRouter(fex.IfFalse));
                    return _;
                }
            }
            return null;
        }
        /// <summary>
        /// 设置规则
        /// </summary>
        /// <param name="exp">节点</param>
        /// <returns></returns>
        public virtual string ExpressionRouterModel(Expression exp)
        {
            string sb = string.Empty;
            if (exp is BinaryExpression be)
            {
                return BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression me)
            {
                if (me.ToString().IsMatch(@"\s*value\([\s\S]+?\)"))
                {
                    return this.GetLambdaValue(this.Eval(me));
                }
                else if (me.Expression is ParameterExpression pex)
                {
                    return "{0}{1}.{2}".format(FieldFormat(pex.Name), me.Member.DeclaringType.Name, FieldFormat(me.Member.Name));
                }
                else if (me.Expression != null && me.Expression.GetType().Name == "TypedParameterExpression")
                {
                    ParameterExpression pexp = me.Expression as ParameterExpression;
                    if (me.Member.Name == "Now")
                        return this.GetParamName(DateTime.Now);
                    else
                        return "{0}{1}.{2}".format(FieldFormat(pexp.Name), me.Member.DeclaringType.Name, FieldFormat(me.Member.Name))/* + (me.Type == typeof(bool) ? " = 1" : "")*/;
                }
                else if (me.Expression is MemberExpression mex)
                {
                    ParameterExpression pexp = mex.Expression as ParameterExpression;
                    return "{0}{1}.{2}".format(FieldFormat(pexp.Name), mex.Member.DeclaringType.Name, FieldFormat(mex.Member.Name));
                }
            }
            else if (exp is NewArrayExpression nae)
            {
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in nae.Expressions)
                {
                    tmpstr.Append(ExpressionRouterModel(ex));
                    tmpstr.Append(',');
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                if (this.SqlFun.TryGetValue(mce.Method.Name, out string SqlFun))
                {
                    List<string> args = new List<string>();
                    mce.Arguments.Each(a =>
                    {
                        args.Add(ExpressionRouterModel(a));
                    });
                    if (mce.Method.Name.IsMatch(@"^(IndexOf|StartsWith|EndsWith|Substring|Replace|Sum|Contains|ToLower|ToUpper|Trim|TrimStart|TrimEnd|Length)$"))
                    {
                        var o = ExpressionRouter(mce.Object);
                        if (!args.Contains(o)) args.Insert(0, o);
                    }
                    else if (SqlFun.IsMatch(@"^(DATEDIFF|DATEPART|TIMESTAMPDIFF|DATE_FORMAT|DATEADD)\("))
                    {
                        var b = args.Last();
                        args.RemoveAt(args.Count - 1);
                        var dfType = this.GetParamValue(b).ToString();
                        var _dfType = dfType.ToLower();
                        if (this.DataHelper.ProviderType == DbProviderType.Oracle)
                        {
                            if (_dfType == "yy" || _dfType == "yyyy")
                                dfType = "YEAR";
                            else if (_dfType == "qq" || _dfType == "qq")
                                dfType = "QUARTER";
                            else if (_dfType == "mm" || _dfType == "m")
                                dfType = "MOUTH";
                            else if (_dfType == "dd" || _dfType == "d")
                                dfType = "DAY";
                            else if (_dfType == "wk" || _dfType == "w")
                                dfType = "WEEK";
                            else if (_dfType == "hh")
                                dfType = "HOUR";
                            else if (_dfType == "mi" || _dfType == "n")
                                dfType = "MINUTE";
                            else if (_dfType == "ss" || _dfType == "s")
                                dfType = "SECOND";
                            else if (_dfType == "ms")
                                dfType = "MICROSECOND";
                        }
                        else if (this.DataHelper.ProviderType == DbProviderType.MySql)
                        {
                            if (SqlFun.IsMatch(@"^(DATEPART|DATE_FORMAT)\("))
                            {
                                if (_dfType == "yy")
                                    dfType = "'%y'";
                                else if (_dfType == "yyyy")
                                    dfType = "'%Y'";
                                else if (_dfType == "qq" || _dfType == "q")
                                    dfType = "QUARTER";
                                else if (_dfType == "mm" || _dfType == "m")
                                    dfType = "'%m'";
                                else if (_dfType == "dd")
                                    dfType = "'%D'";
                                else if (_dfType == "d")
                                    dfType = "'%d'";
                                else if (_dfType == "wk" || _dfType == "ww" || _dfType == "w")
                                    dfType = "'%U'";
                                else if (_dfType == "hh")
                                    dfType = "'%k'";
                                else if (_dfType == "h")
                                    dfType = "'%l'";
                                else if (_dfType == "mi" || _dfType == "n")
                                    dfType = "'%i'";
                                else if (_dfType == "ss" || _dfType == "s")
                                    dfType = "'%S'";
                                else if (_dfType == "ms")
                                    dfType = "'%f'";
                                else
                                    dfType = $"'{dfType.Trim('\'')}'";
                            }
                        }
                        this.RemoveParam(b);
                        if (this.DataHelper.ProviderType == DbProviderType.Dameng)
                        {
                            b = args.Last();
                            args.RemoveAt(args.Count - 1);
                            var val = this.GetParamValue(b).ToCast<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.fff");
                            args.Add("'" + val + "'");
                            this.RemoveParam(b);
                        }
                        args.Add(dfType);
                    }
                    else if (SqlFun.IsMatch(@"julianday"))
                    {
                        var val = this.GetParamValue(args.Last()).ToString();
                        //格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns
                        if (val == "dd" || val == "d" || val == "dy" || val == "y")
                            val = "*1";
                        else if (val == "wk" || val == "ww" || val == "dw" || val == "w")
                            val = "/7";
                        else if (val == "hh")
                            val = "*24";
                        else if (val == "mi" || val == "n")
                            val = "*24*60";
                        else if (val == "ss" || val == "s")
                            val = "*24*60*60";
                        else if (val == "ms")
                            val = "*24*60*60*1000";
                        else if (val == "mcs")
                            val = "*24*60*60*1000*1000";
                        else if (val == "ns")
                            val = "*24*60*60*1000*1000*1000";
                        if (val == "yy" || val == "yyyy")
                            SqlFun = "strftime('%Y','{1}') - strftime('%Y','{0}')";
                        else if (val == "mm" || val == "m")
                            SqlFun = "((strftime('%Y','{1}') - strftime('%Y','{0}'))*12 + strftime('%m','{1}') - strftime('%m','{0}'))";
                        else
                        {
                            args.RemoveAt(args.Count - 1);
                            args.Add(val);
                        }
                    }
                    else if (SqlFun.IsMatch(@" like "))
                    {
                        var ev = this.Eval(mce.Arguments[1]);
                        var b =/* args.Last();*/(ev is IQueryableX x) ? GetIQueryableXSQL(x) : ev.ToString();
                        b = b.Trim('\'');
                        string _Start = "", _End = "";
                        //if (mce.Method.Name == "StartsWith") _End = "%";
                        //else if (mce.Method.Name == "EndsWith") _Start = "%";
                        if (b.IsNotNullOrWhiteSpace())
                        {
                            Dictionary<string, string> d = b.GetMatchs(@"^(?<a>[%_]?)(?<b>[\s\S]*?)(?<c>[%_]?)$");
                            var val = d["b"];
                            if (mce.Method.Name.EqualsIgnoreCase("likeSQL") || mce.Method.Name.EqualsIgnoreCase("NotlikeSQL"))
                            {
                                if (this.DataHelper.ProviderType == DbProviderType.SqlServer)
                                    val = val.ReplacePattern(@"([%_\[\]\(\)])", "[$1]");
                            }
                            b = d["a"] + val.GetValue() + d["c"];
                        }
                        args.Insert(args.Count - 1, "" + _Start + b + _End + "");
                    }
                    else if (SqlFun.IsMatch(@"\s+IN\s+\("))
                    {
                        if (args.Count <= 1 || args[1].IsNullOrEmpty() || args[1] == "''" || args[1] == "null") return "1 = 0";
                    }
                    else if (SqlFun.IsMatch(@"CHARINDEX\("))
                    {
                        var _BaseType = mce.Arguments[0].Type.GetValueType();
                        if (mce.Method.Name == "Contains" && (_BaseType == ValueTypes.Array ||
                            _BaseType == ValueTypes.ArrayList || _BaseType == ValueTypes.IEnumerable || _BaseType == ValueTypes.List))
                        {
                            SqlFun = SqlFun.ReplacePattern(@"\{1\}", "','+ Convert(nvarchar(500),{1}) +','").ReplacePattern(@"\{0\}", "',{0},'");
                            var _val = args[0];
                            _val = _val.ReplacePattern(@"(@ParamName\d+)", "{$1}").format(this.GetParameters());
                            args[0] = _val;
                        }
                    }
                    else if (SqlFun.IsMatch(@"CAST\("))
                    {
                        var b = args.Last();
                        args.RemoveAt(args.Count - 1);
                        args.Add(this.GetParamValue(b).ToString());
                        this.RemoveParam(b);
                    }
                    if ((DbProviderType.Dameng).HasFlag(this.DataHelper.ProviderType))
                    {
                        args.Each(a =>
                        {
                            if (a.IsMatch(@"^[@:]?ParamName\d+$"))
                            {
                                a = "'" + GetParamValue(a).ToString() + "'";
                                RemoveParam(a);
                            }
                        });
                    }
                    var _ = SqlFun.format(args.ToArray());
                    return _;
                }
                else if (mce.Type.Name == typeof(IQueryableX<>).Name)
                {
                    return GetIQueryableXSQL(this.Eval(mce) as IQueryableX);
                    //var val = this.Eval(mce);
                    //if (val == null) return "";
                    //return val.GetValue();
                }
                //else if (mce.Type.Name == "Guid" && mce.Method.Name == "NewGuid")
                //    return this.GetParamName(Guid.NewGuid());
                //else if (mce.Type.Name == "DateTime" && mce.Method.Name == "Parse")
                //    return ExpressionRouterModel(mce.Arguments[0]);
                //else if (mce.Type.Name == "Decimal" && mce.Method.Name == "Parse")
                //{
                //    ConstantExpression ce = mce.Arguments[0] as ConstantExpression;
                //    return this.GetParamName(ce.Value);
                //}
                //else if (mce.Method.Name == "Parse")
                //{
                //    return this.GetLambdaValue(this.Eval(mce));
                //}
                //else if (mce.Type.Name == "String" && mce.Method.Name == "ToString")
                //{
                //    return "Convert(nvarchar(20)," + ExpressionRouterModel(mce.Arguments.Count == 0 ? mce.Object : mce.Arguments[0]) + ")";
                //}
                else return this.GetLambdaValue(this.Eval(mce));
            }
            else if (exp is ConstantExpression ce)
            {
                object value = null;
                if (ce.Value == null)
                    return "null";
                else if (ce.Value.ToString() == "")
                    return "''";
                else if (ce.Value is bool)
                    value = Convert.ToInt32(ce.Value);
                else if (ce.Value is ValueType)
                    value = ce.Value;
                else if (ce.Value is DateTime || ce.Value.ToString().IsDate() || ce.Value.ToString().IsDateTime() || ce.Value is Guid || ce.Value is string || ce.Value is char)
                    value = ce.Value;
                else value = ce.Value;
                return this.GetParamName(value);
            }
            else if (exp is UnaryExpression ue)
            {
                string val = ExpressionRouterModel(ue.Operand);
                if (ue.Type == typeof(bool) && ue.NodeType == ExpressionType.Not)
                {
                    if (val.IndexOf("=") > -1)
                        val = val.ReplacePattern(@" = 1", " = 0");
                    else
                        val += " = 0";
                }
                return val;
            }
            else if (exp is NewExpression nexp)
            {
                return ExpressionRouterModel(nexp.Arguments[0]);
            }
            else if (exp is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(fex.Test);
                    if (!_case.IsMatch(@"(=|\s+is\s+|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(fex.IfTrue), ExpressionRouterModel(fex.IfFalse));
                    /*
                     * 先按+号替换成||处理 日后遇到问题再解决    2021-06-08
                     */
                    if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
                        _ = _.ReplacePattern(@"\+", "||");
                    return _;
                }
            }
            return null;
        }
        /// <summary>
        /// 设置规则
        /// </summary>
        /// <param name="exp">节点</param>
        /// <returns></returns>
        public virtual string UnExpressionRouter(Expression exp)
        {
            string sb = string.Empty;
            if (exp is BinaryExpression be)
            {
                return UnBinaryExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression me)
            {
                if (me.ToString().IsMatch(@"\s*value\([\s\S]*?\)"))
                {
                    return this.GetLambdaValue(this.Eval(me));
                    //return this.GetParamName(value);
                }
                else if (me.Expression != null && me.Expression.GetType().Name == "TypedParameterExpression")
                {
                    if (me.Member.Name == "Now")
                        return this.GetParamName(DateTime.Now);
                    else
                        return FieldFormat(me.Member.Name) /*+ (me.Type == typeof(bool) ? " = 0" : "");*/;
                }
                else if (me.Expression is MemberExpression mex)
                {
                    return FieldFormat(mex.Member.Name);
                }
            }
            else if (exp is NewArrayExpression ae)
            {
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(UnExpressionRouter(ex));
                    tmpstr.Append(',');
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                if (this.SqlUnFun.TryGetValue(mce.Method.Name, out string SqlFun))
                {
                    List<string> args = new List<string>();
                    /*if (mce.Object != null)
                    {
                        var o = mce.Arguments.FirstOrDefault(a => a == mce.Object);
                        if (o == null) args.Add(UnExpressionRouter(mce.Object));
                    }*/
                    mce.Arguments.Each(a =>
                    {
                        args.Add(UnExpressionRouter(a));
                    });
                    if (mce.Method.Name.IsMatch(@"^(IndexOf|StartsWith|EndsWith|Substring|Replace|Sum|Contains|ToLower|ToUpper|Trim|TrimStart|TrimEnd|Length)$"))
                    {
                        var o = ExpressionRouter(mce.Object);
                        if (!args.Contains(o)) args.Insert(0, o);
                    }
                    else if (SqlFun.IsMatch(@"^(DATEDIFF|DATEPART|TIMESTAMPDIFF|DATE_FORMAT|DATEADD)\("))
                    {
                        var b = args.Last();
                        args.RemoveAt(args.Count - 1);
                        var dfType = this.GetParamValue(b).ToString();
                        var _dfType = dfType.ToLower();
                        if (this.DataHelper.ProviderType == DbProviderType.Oracle)
                        {
                            if (_dfType == "yy" || _dfType == "yyyy")
                                dfType = "YEAR";
                            else if (_dfType == "qq" || _dfType == "qq")
                                _dfType = "QUARTER";
                            else if (_dfType == "mm" || _dfType == "m")
                                dfType = "MOUTH";
                            else if (_dfType == "dd" || _dfType == "d")
                                dfType = "DAY";
                            else if (_dfType == "wk" || _dfType == "w")
                                dfType = "WEEK";
                            else if (_dfType == "hh")
                                dfType = "HOUR";
                            else if (_dfType == "mi" || _dfType == "n")
                                dfType = "MINUTE";
                            else if (_dfType == "ss" || _dfType == "s")
                                dfType = "SECOND";
                            else if (_dfType == "ms")
                                dfType = "MICROSECOND";
                        }
                        else if (this.DataHelper.ProviderType == DbProviderType.MySql)
                        {
                            if (SqlFun.IsMatch(@"^(DATEPART|DATE_FORMAT)\("))
                            {
                                if (_dfType == "yy")
                                    dfType = "'%y'";
                                else if (_dfType == "yyyy")
                                    dfType = "'%Y'";
                                else if (_dfType == "qq" || _dfType == "q")
                                    dfType = "QUARTER";
                                else if (_dfType == "mm" || _dfType == "m")
                                    dfType = "'%m'";
                                else if (_dfType == "dd")
                                    dfType = "'%D'";
                                else if (_dfType == "d")
                                    dfType = "'%d'";
                                else if (_dfType == "wk" || _dfType == "ww" || _dfType == "w")
                                    dfType = "'%U'";
                                else if (_dfType == "hh")
                                    dfType = "'%k'";
                                else if (_dfType == "h")
                                    dfType = "'%l'";
                                else if (_dfType == "mi" || _dfType == "n")
                                    dfType = "'%i'";
                                else if (_dfType == "ss" || _dfType == "s")
                                    dfType = "'%S'";
                                else if (_dfType == "ms")
                                    dfType = "'%f'";
                                else
                                    dfType = $"'{dfType.Trim('\'')}'";
                            }
                        }
                        this.RemoveParam(b);
                        if (this.DataHelper.ProviderType == DbProviderType.Dameng)
                        {
                            b = args.Last();
                            args.RemoveAt(args.Count - 1);
                            var val = this.GetParamValue(b).ToCast<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.fff");
                            args.Add("'" + val + "'");
                            this.RemoveParam(b);
                        }
                        args.Add(dfType);
                    }
                    else if (SqlFun.IsMatch(@"julianday"))
                    {
                        var val = this.GetParamValue(args.Last()).ToString();
                        //格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns
                        if (val == "dd" || val == "d" || val == "dy" || val == "y")
                            val = "*1";
                        else if (val == "wk" || val == "ww" || val == "dw" || val == "w")
                            val = "/7";
                        else if (val == "hh")
                            val = "*24";
                        else if (val == "mi" || val == "n")
                            val = "*24*60";
                        else if (val == "ss" || val == "s")
                            val = "*24*60*60";
                        else if (val == "ms")
                            val = "*24*60*60*1000";
                        else if (val == "mcs")
                            val = "*24*60*60*1000*1000";
                        else if (val == "ns")
                            val = "*24*60*60*1000*1000*1000";
                        if (val == "yy" || val == "yyyy")
                            SqlFun = "strftime('%Y','{1}') - strftime('%Y','{0}')";
                        else if (val == "mm" || val == "m")
                            SqlFun = "((strftime('%Y','{1}') - strftime('%Y','{0}'))*12 + strftime('%m','{1}') - strftime('%m','{0}'))";
                        else
                        {
                            args.RemoveAt(args.Count - 1);
                            args.Add(val);
                        }
                    }
                    else if (SqlFun.IsMatch(@" like "))
                    {
                        var ev = this.Eval(mce.Arguments[1]);
                        var b =/* args.Last();*/(ev is IQueryableX x) ? GetIQueryableXSQL(x) : ev.ToString();
                        b = b.Trim('\'');
                        string _Start = "", _End = "";
                        //if (mce.Method.Name == "StartsWith") _End = "%";
                        //else if (mce.Method.Name == "EndsWith") _Start = "%";
                        if (b.IsNotNullOrWhiteSpace())
                        {
                            Dictionary<string, string> d = b.GetMatchs(@"^(?<a>[%_]?)(?<b>[\s\S]*?)(?<c>[%_]?)$");
                            var val = d["b"];
                            if (mce.Method.Name.EqualsIgnoreCase("likeSQL") || mce.Method.Name.EqualsIgnoreCase("NotlikeSQL"))
                            {
                                if (this.DataHelper.ProviderType == DbProviderType.SqlServer)
                                    val = val.ReplacePattern(@"([%_\[\]\(\)])", "[$1]");
                            }
                            b = d["a"] + val.GetValue() + d["c"];
                        }
                        args.Insert(args.Count - 1, "" + _Start + b + _End + "");
                    }
                    else if (SqlFun.IsMatch(@"\s+IN\s+\("))
                    {
                        if (args.Count <= 1 || args[1].IsNullOrEmpty() || args[1] == "''" || args[1] == "null") return "1 = 0";
                    }
                    else if (SqlFun.IsMatch(@"CHARINDEX\("))
                    {
                        var _BaseType = mce.Arguments[0].Type.GetValueType();
                        if (mce.Method.Name == "Contains" && (_BaseType == ValueTypes.Array ||
                            _BaseType == ValueTypes.ArrayList || _BaseType == ValueTypes.IEnumerable || _BaseType == ValueTypes.List))
                        {
                            SqlFun = SqlFun.ReplacePattern(@"\{1\}", "','+ Convert(nvarchar(500),{1}) +','").ReplacePattern(@"\{0\}", "',{0},'");
                            var _val = args[0];
                            _val = _val.ReplacePattern(@"(@ParamName\d+)", "{$1}").format(this.GetParameters());
                            args[0] = _val;
                        }
                    }
                    else if (SqlFun.IsMatch(@"CAST\("))
                    {
                        var b = args.Last();
                        args.RemoveAt(args.Count - 1);
                        args.Add(this.GetParamValue(b).ToString());
                        this.RemoveParam(b);
                    }
                    if ((DbProviderType.Dameng).HasFlag(this.DataHelper.ProviderType))
                    {
                        args.Each(a =>
                        {
                            if (a.IsMatch(@"^[@:]?ParamName\d+$"))
                            {
                                a = "'" + GetParamValue(a).ToString() + "'";
                                RemoveParam(a);
                            }
                        });
                    }
                    var _ = SqlFun.format(args.ToArray());
                    return _;
                }
                else if (mce.Type.Name == typeof(IQueryableX<>).Name)
                {
                    return GetIQueryableXSQL(this.Eval(mce) as IQueryableX);
                    //var val = this.Eval(mce);
                    //if (val == null) return "";
                    //return val.GetValue();
                    /*
                    PropertyInfo info = val.GetType().GetProperty("DataSQL");
                    if (info == null) return "";
                    if (!(info.GetValue(val) is DataSQL dataSQL)) return "";
                    var SQL = dataSQL.GetSQLString().RemovePattern(@"\s*;\s*$");
                    dataSQL.Parameters.Each(a =>
                    {
                        var key = "@Sub_" + a.Key.TrimStart('@');
                        SQL = SQL.ReplacePattern(a.Key + @"([^\d])", key + "$1");
                        this.AddParam(key, a.Value);
                    });
                    return SQL;*/
                }
                //else if (mce.Type.Name == "Guid" && mce.Method.Name == "NewGuid")
                //    return this.GetParamName(Guid.NewGuid());
                //else if (mce.Type.Name == "DateTime" && mce.Method.Name == "Parse")
                //    return ExpressionRouter(mce.Arguments[0]);
                //else if (mce.Type.Name == "Decimal" && mce.Method.Name == "Parse")
                //{
                //    ConstantExpression ce = mce.Arguments[0] as ConstantExpression;
                //    return this.GetParamName(ce.Value);
                //}
                //else if (mce.Type.Name == "String" && mce.Method.Name == "ToString")
                //{
                //    return "Convert(nvarchar(20)," + UnExpressionRouter(mce.Arguments.Count == 0 ? mce.Object : mce.Arguments[0]) + ")";
                //}
                else return this.GetLambdaValue(this.Eval(mce));
            }
            else if (exp is ConstantExpression ce)
            {
                object value = null;
                if (ce.Value == null)
                    return "null";
                else if (ce.Value.ToString() == "")
                    return "''";
                else if (ce.Value is bool)
                    value = Convert.ToInt32(ce.Value);
                else if (ce.Value is DateTime || ce.Value.ToString().IsDate() || ce.Value.ToString().IsDateOrTime() || ce.Value is Guid || ce.Value is string || ce.Value is char)
                    value = ce.Value;
                else if (ce.Value is ValueType)
                    value = ce.Value;
                else value = ce.Value;
                return this.GetParamName(value);
            }
            else if (exp is UnaryExpression ue)
            {
                string val = UnExpressionRouter(ue.Operand);
                if (ue.Type == typeof(bool) && ue.NodeType == ExpressionType.Not)
                {
                    if (val.IndexOf("=") > -1)
                        val = val.ReplacePattern(@" = 0", " = 1");
                    else
                        val += " = 1";
                }
                return val;
            }
            else if (exp is NewExpression nexp)
            {
                return UnExpressionRouter(nexp.Arguments[0]);
            }
            else if (exp is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = UnExpressionRouter(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, UnExpressionRouter(fex.IfTrue), UnExpressionRouter(fex.IfFalse));
                    /*if (_.IsMatch(@"@ParamName\d+"))
                    {
                        _.getPatterns(@"@ParamName\d+").Each(a =>
                        {
                            var _val = this.DataSQL.Parameters[a].ToString();
                            _ = _.ReplacePattern(@"" + a, (_val.isFloat() ? "{0}" : "'{0}'").format(_val));
                        });
                    }*/
                    return _;
                }
            }
            return null;
        }
        #endregion

        #region 设置节点连接类型
        /*表达式树节点的节点类型。
        字段	                值	描述
        Add	                    0	加法运算，如 a + b，针对数值操作数，不进行溢出检查。
        AddChecked	            1	加法运算，如 (a + b)，针对数值操作数，进行溢出检查。
        And	                    2	按位或逻辑 AND 运算，如 C# 中的 (a & b) 和 Visual Basic 中的 (a And b)。
        AndAlso	                3	条件 AND 运算，它仅在第一个操作数的计算结果为 true 时才计算第二个操作数。 它对应于 C# 中的 (a && b) 和 Visual Basic 中的 (a AndAlso b)。
        ArrayLength	            4	获取一维数组长长度的运算，如 array.Length。
        ArrayIndex	            5	一维数组中的索引运算，如 C# 中的 array[index] 或 Visual Basic 中的 array(index)。
        Call	                6	方法调用，如在 obj.sampleMethod() 表达式中。
        Coalesce	            7	表示 null 合并运算的节点，如 C# 中的 (a ?? b) 或 Visual Basic 中的 If(a, b)。
        Conditional	            8	条件运算，如 C# 中的 a > b ? a : b 或 Visual Basic 中的 If(a > b, a, b)。
        Constant	            9	一个常量值。
        Convert	                10	强制转换或转换操作，如 C# 中的 (SampleType)obj 或 Visual Basic 中的 CType(obj, SampleType)。 对于数值转换，如果转换后的值对于目标类型来说太大，这不会引发异常。
        ConvertChecked	        11	强制转换或转换操作，如 C# 中的 (SampleType)obj 或 Visual Basic 中的 CType(obj, SampleType)。 对于数值转换，如果转换后的值与目标类型大小不符，则引发异常。
        Divide	                12	除法运算，如 (a / b)，针对数值操作数。
        Equal	                13	表示相等比较的节点，如 C# 中的 (a == b) 或 Visual Basic 中的 (a = b)。
        ExclusiveOr     	    14	按位或逻辑 XOR 运算，如 C# 中的 (a ^ b) 和 Visual Basic 中的 (a Xor b)。
        GreaterThan	            15	“大于”比较，如 (a > b)。
        GreaterThanOrEqual	    16	“大于或等于”比较，如 (a >= b)。
        Invoke          	    17	调用委托或 lambda 表达式的运算，如 sampleDelegate.Invoke()。
        Lambda	                18	lambda 表达式，如 C# 中的 a => a + a 或 Visual Basic 中的 Function(a) a + a。
        LeftShift	            19	按位左移运算，如 (a << b)。
        LessThan	            20	“小于”比较，如 (a < b)。
        LessThanOrEqual 	    21	“小于或等于”比较，如 (a <= b)。
        ListInit	            22	创建新的 IEnumerable 对象并从元素列表中初始化该对象的运算，如 C# 中的 new List<SampleType>(){ a, b, c } 或 Visual Basic 中的 Dim sampleList = { a, b, c }。
        MemberAccess	        23	从字段或属性进行读取的运算，如 obj.SampleProperty。
        MemberInit	            24	创建新的对象并初始化其一个或多个成员的运算，如 C# 中的 new Point { X = 1, Y = 2 } 或 Visual Basic 中的 New Point With {.X = 1, .Y = 2}。
        Modulo	                25	算术余数运算，如 C# 中的 (a % b) 或 Visual Basic 中的 (a Mod b)。
        Multiply	            26	乘法运算，如 (a * b)，针对数值操作数，不进行溢出检查。
        MultiplyChecked	        27	乘法运算，如 (a * b)，针对数值操作数，进行溢出检查。
        Negate	                28	算术求反运算，如 (-a)。 不应就地修改 a 对象。
        UnaryPlus	            29	一元加法运算，如 (+a)。 预定义的一元加法运算的结果是操作数的值，但用户定义的实现可以产生特殊结果。
        NegateChecked	        30	算术求反运算，如 (-a)，进行溢出检查。 不应就地修改 a对象。
        New	                    31	调用构造函数创建新对象的运算，如 new SampleType()。
        NewArrayInit	        32	创建新的一维数组并从元素列表中初始化该数组的运算，如 C# 中的 new SampleType[]{a, b, c} 或 Visual Basic 中的 New SampleType(){a, b, c}。
        NewArrayBounds	        33	创建新数组（其中每个维度的界限均已指定）的运算，如 C# 中的 new SampleType[dim1, dim2] 或 Visual Basic 中的 New SampleType(dim1, dim2)。
        Not	                    34	按位求补运算或逻辑求反运算。 在 C# 中，它与整型的 (~a)和布尔值的 (!a) 等效。 在 Visual Basic 中，它与 (Not a)等效。 不应就地修改 a 对象。
        NotEqual	            35	不相等比较，如 C# 中的 (a != b) 或 Visual Basic 中的 (a <> b)。
        Or	                    36	按位或逻辑 OR 运算，如 C# 中的 (a | b) 和 Visual Basic 中的 (a Or b)。
        OrElse	                37	短路条件 OR 运算，如 C# 中的 (a || b) 或 Visual Basic 中的 (a OrElse b)。
        Parameter	            38	对在表达式上下文中定义的参数或变量的引用。 有关详情，请参阅ParameterExpression。
        Power	                39	对某个数字进行幂运算的数学运算，如 Visual Basic 中的 (a ^ b)。
        Quote	                40	具有类型为 Expression 的常量值的表达式。 Quote 节点可包含对参数的引用，这些参数在该节点表示的表达式的上下文中定义。
        RightShift	            41	按位右移运算，如 (a >> b)。
        Subtract	            42	减法运算，如 (a - b)，针对数值操作数，不进行溢出检查。
        SubtractChecked	        43	算术减法运算，如 (a - b)，针对数值操作数，进行溢出检查。
        TypeAs	                44	显式引用或装箱转换，其中如果转换失败则提供 null，如 C# 中的 (obj as SampleType) 或 Visual Basic 中的 TryCast(obj, SampleType)。
        TypeIs	                45	类型测试，如 C# 中的 obj is SampleType 或 Visual Basic 中的 TypeOf obj is SampleType。
        Assign	                46	赋值运算，如 (a = b)。
        Block	                47	表达式块。
        DebugInfo       	    48	调试信息。
        Decrement       	    49	一元递减运算，如 C# 和 Visual Basic 中的 (a - 1)。 不应就地修改 a 对象。
        Dynamic	                50	动态操作。
        Default	                51	默认值。
        Extension	            52	扩展表达式。
        Goto	                53	“转到”表达式，如 C# 中的 goto Label 或 Visual Basic 中的 GoTo Label。
        Increment	            54	一元递增运算，如 C# 和 Visual Basic 中的 (a + 1)。 不应就地修改 a 对象。
        Index	                55	索引运算或访问使用参数的属性的运算。
        Label	                56	标签。
        RuntimeVariables	    57	运行时变量的列表。 有关详情，请参阅RuntimeVariablesExpression。
        Loop	                58	一个循环，例如 for 或 while。
        Switch	                59	多分支选择运算，如 C# 中的 switch 或 Visual Basic 中的 Select Case。
        Throw	                60	引发异常的运算，如引发新异常()。
        Try	                    61	try-catch 表达式。
        Unbox	                62	取消装箱值类型运算，如 MSIL 中的 unbox 和 unbox.any 指令。
        AddAssign	            63	加法复合赋值运算，如 (a += b)，针对数值操作数，不进行溢出检查。
        AndAssign	            64	按位或逻辑 AND 复合赋值运算，如 C# 中的 (a &= b)。
        DivideAssign	        65	除法复合赋值运算，如 (a /= b)，针对数值操作数。
        ExclusiveOrAssign	    66	按位或逻辑 XOR 复合赋值运算，如 C# 中的 (a ^= b)。
        LeftShiftAssign	        67	按位左移复合赋值运算，如 (a <<= b)。
        ModuloAssign	        68	算术余数复合赋值运算，如 C# 中的 (a %= b)。
        MultiplyAssign	        69	乘法复合赋值运算，如 (a *= b)，针对数值操作数，不进行溢出检查。
        OrAssign	            70	按位或逻辑 OR 复合赋值运算，如 C# 中的 (a |= b)。
        PowerAssign	            71	对某个数字进行幂运算的复合赋值运算，如 Visual Basic 中的(a ^= b)。
        RightShiftAssign	    72	按位右移复合赋值运算，如 (a >>= b)。
        SubtractAssign	        73	减法复合赋值运算，如 (a -= b)，针对数值操作数，不进行溢出检查。
        AddAssignChecked	    74	加法复合赋值运算，如 (a += b)，针对数值操作数，并进行溢出检查。
        MultiplyAssignChecked	75	乘法复合赋值运算，如 (a *= b)，针对数值操作数，进行溢出检查。
        SubtractAssignChecked	76	减法复合赋值运算，如 (a -= b)，针对数值操作数，进行溢出检查。
        PreIncrementAssign	    77	一元前缀递增，如 (++a)。 应就地修改 a 对象。
        PreDecrementAssign	    78	一元前缀递减，如 (--a)。 应就地修改 a 对象。
        PostIncrementAssign	    79	一元后缀递增，如 (a++)。 应就地修改 a 对象。
        PostDecrementAssign	    80	一元后缀递减，如 (a--)。 应就地修改 a 对象。
        TypeEqual	            81	确切类型测试。
        OnesComplement	        82	二进制反码运算，如 C# 中的 (~a)。
        IsTrue	                83	true 条件值。
        IsFalse	                84	false 条件值。
        */
        /// <summary>
        /// 设置节点连接类型
        /// </summary>
        /// <param name="type">节点类型</param>
        /// <returns></returns>
        public virtual string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return " + ";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return " - ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return " * ";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 设置节点连接类型
        /// </summary>
        /// <param name="type">节点类型</param>
        /// <returns></returns>
        public virtual string ExpressionTypeCasts(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " <> ";
                case ExpressionType.GreaterThan:
                    return " <= ";
                case ExpressionType.GreaterThanOrEqual:
                    return " < ";
                case ExpressionType.LessThan:
                    return " >= ";
                case ExpressionType.LessThanOrEqual:
                    return " > ";
                case ExpressionType.NotEqual:
                    return " = ";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return " + ";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return " - ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return " * ";
                default:
                    return null;
            }
        }
        #endregion

        #region 解析IQueryableX
        /// <summary>
        /// 解析IQueryableX
        /// </summary>
        /// <param name="queryableX">IQueryableX</param>
        /// <returns></returns>
        public string GetIQueryableXSQL(IQueryableX queryableX)
        {
            PropertyInfo info = queryableX.GetType().GetProperty("DataSQL");
            if (info == null) return "";
            if (!(info.GetValue(queryableX) is DataSQL dataSQL)) return "";
            var SQL = dataSQL.GetSQLString().RemovePattern(@"\s*;\s*$");
            if (dataSQL.Parameters.Count > 0)
            {
                var prefix = "@Sub_";
                var id = this.GetParameters().Keys.Where(a => a.StartsWith(@"@Sub_")).OrderByDescending(a => a).Take(1);
                if (id.Any())
                {
                    var _id = id.FirstOrDefault();
                    if (_id.IsMatch(@"@Sub_\d+"))
                    {
                        var num = _id.GetMatch(@"@Sub_(?<a>\d+)").ToInt32();
                        prefix += (++num) + "_";
                    }
                    else
                        prefix += "0_";
                }
                else prefix += "0_";
                dataSQL.Parameters.Each(a =>
                {
                    var key = prefix + a.Key.TrimStart('@');
                    SQL = SQL.ReplacePattern(a.Key + @"([^\d])", key + "$1");
                    this.AddParam(key, a.Value);
                });
            }
            return SQL;
        }
        #endregion

        #region 执行Lambda表达式
        /// <summary>
        /// 执行Lambda表达式
        /// </summary>
        /// <param name="expression">Lambda表达式</param>
        /// <returns></returns>
        public virtual object Eval(Expression expression)
        {
            UnaryExpression cast = Expression.Convert(expression, typeof(object));
            object val = null;
            if (cast != null)
                try { val = Expression.Lambda<Func<object>>(cast).Compile().Invoke(); }
                catch { }
            return val;
            /*
             * 2022-11-19 14:29
             * IQueryableX专一再作处理
             */
            /*if (val?.GetType().Name == "DataHelperX`1")
            {
                PropertyInfo info = val.GetType().GetProperty("DataSQL");
                if (info == null) return "";
                if (!(info.GetValue(val) is DataSQL dataSQL)) return "";
                var SQL = dataSQL.GetSQLString().RemovePattern(@"\s*;\s*$");
                if (dataSQL.Parameters.Count > 0)
                {
                    var prefix = "@Sub_";
                    var id = this.GetParameters().Keys.Where(a => a.StartsWith(@"@Sub_")).OrderByDescending(a => a).Take(1);
                    if (id.Any())
                    {
                        var _id = id.FirstOrDefault();
                        if (_id.IsMatch(@"@Sub_\d+"))
                        {
                            var num = _id.GetMatch(@"@Sub_(?<a>\d+)").ToInt32();
                            prefix += (++num) + "_";
                        }
                        else
                            prefix += "0_";
                    }
                    else prefix += "0_";
                    dataSQL.Parameters.Each(a =>
                    {
                        var key = prefix + a.Key.TrimStart('@');
                        SQL = SQL.ReplacePattern(a.Key + @"([^\d])", key + "$1");
                        this.AddParam(key, a.Value);
                    });
                }
                return SQL;
            }
            return val;*/
        }
        #endregion

        #region 匹配输入标识与数据库表对象
        /// <summary>
        /// 匹配输入标识与数据库表对象
        /// </summary>
        /// <param name="Tags">标签</param>
        /// <param name="SqlString">Sql语句</param>
        /// <returns></returns>
        public string ReplaceMatchTag(Dictionary<string, string> Tags, string SqlString)
        {
            var _ = SqlString;
            Tags.Each(k =>
            {
                _ = _.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            return _;
        }
        #endregion

        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~QueryableProvider() { }
        #endregion
    }
    #endregion

    #region T1,T2
    /// <summary>
    /// Queryable驱动
    /// </summary>
    /// <typeparam name="T">第一个类型</typeparam>
    /// <typeparam name="T2">第二个类型</typeparam>
    public class QueryableProvider<T, T2> : QueryableProvider<T>
    {
        #region 属性
        /// <summary>
        /// 配置数据
        /// </summary>
        public new virtual DataSQL2 DataSQL { get; set; }
        #endregion

        #region 设置存储过程参数
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public override string GetParamName(object value)
        {
            if (value == null) return "null";
            if (value is string str && str.IsNullOrEmpty()) return "''";
            if (value.ToString().IsMatch(@"@Sub_(\d+_)?ParamName\d+"))
                return value.ToString();
            if (value is bool) value = Convert.ToInt32(value);
            else if (value is Guid guid) value = guid;
            //var ParaName = "@ParamName" + (this.DataSQL.Parameters.Count + 1);
            //var ParamIndex = 1;
            if (this.DataSQL.Parameters == null) this.DataSQL.Parameters = new Dictionary<string, object>();
            /*var ParamItems = this.DataSQL.Parameters.Where(a => a.Key.StartsWith("@ParamName")).Select(a => a.Key.Replace("@ParamName", "").ToCast<int>());
            if (ParamItems.Count() > 0)
                ParamIndex = ParamItems.Max() + 1;
            var ParaName = $"@ParamName{(this.DataSQL.Parameters.Count == 0 ? 1 : ParamIndex)}";*/
            var ParaName = $"@ParamName{this.DataSQL.Parameters.Count + 1}";
            this.AddParam(ParaName, value);
            return ParaName;
            //return (this.DataHelper.ProviderType == DbProviderType.Dameng || this.DataHelper.ProviderType == DbProviderType.MySql) ? "?" : ParaName;
        }
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">值</param>
        public override void AddParam(string name, object value)
        {
            if (value is Guid || value is DateTime) value = value.GetValue();
            if (value is bool) value = value.ToCast<int>();
            if (this.DataSQL.Parameters == null || !this.DataSQL.Parameters.Any())
            {
                this.DataSQL.Parameters = new Dictionary<string, object>()
                {
                    { name , value }
                };
                return;
            }
            if (!this.DataSQL.Parameters.ContainsKey(name))
                this.DataSQL.Parameters.Add(name, value);
            else
                this.DataSQL.Parameters[name] = value;
        }
        /// <summary>
        /// 获取存储过程参数值
        /// </summary>
        /// <returns></returns>
        public override DbParameter[] GetDbParameters()
        {
            var Params = new DbParameter[this.DataSQL.Parameters.Count];
            if (this.DataSQL.Parameters == null) return Params;
            var i = 0;
            this.DataSQL.Parameters.Each(a =>
            {
                Params[i++] = this.DataHelper.MakeParam(@"" + a.Key, a.Value);
            });
            return Params;
        }
        /// <summary>
        /// 获取Param值
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <returns></returns>
        public override object GetParamValue(string ParamName)
        {
            if (ParamName.IsNullOrEmpty() || this.DataSQL.Parameters == null) return "";
            return this.DataSQL.Parameters.TryGetValue((ParamName.IsMatch(@"^@") ? "" : "@") + ParamName, out var val) ? val : "";
        }
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="name">参数错</param>
        public override void RemoveParam(string name)
        {
            if (name.IsNullOrEmpty()) return;
            if (this.DataSQL.Parameters == null || !this.DataSQL.Parameters.Any()) return;
            if (this.DataSQL.Parameters.ContainsKey(name))
                this.DataSQL.Parameters.Remove(name);
        }
        /// <summary>
        /// 获取所有的参数集
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetParameters()
        {
            return this.DataSQL.Parameters;
        }
        #endregion

        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="func">返回结果Lambda</param>
        /// <param name="orderType">排序类型 asc,desc</param>
        /// <returns></returns>
        public virtual string OrderByString<TResult>(Expression<Func<T, T2, TResult>> func, string orderType = "asc")
        {
            if (func == null) return "";
            var OrderByString = ExpressionRouterModel(func.Body);
            if (OrderByString.IsNotNullOrEmpty())
                return OrderByString.ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            return "";
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="func">Expression</param>
        /// <returns></returns>
        public virtual string SetColumns<TResult>(Expression<Func<T, T2, TResult>> func)
        {
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression lex)
            {
                lex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = this.ExpressionRouterModel(b.Expression);
                    Columns += ",{0} as {1}".format(value, FieldFormat(b.Member.Name));
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = this.ExpressionRouterModel(nex.Arguments[i]);
                    Columns += ",{0} as {1}".format(value, FieldFormat(nex.Members[i].Name));
                }
            }
            else if (func.Body is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(fex.IfTrue), ExpressionRouterModel(fex.IfFalse));
                    /*if (_.IsMatch(@"@ParamName\d+"))
                    {
                        _.getPatterns(@"@ParamName\d+").Each(a =>
                        {
                            var _val = this.DataSQL.Parameters[a].ToString();
                            _ = _.ReplacePattern(@"" + a, (_val.isFloat() ? "{0}" : "'{0}'").format(_val));
                        });
                    }*/
                    return _;
                }
            }
            else if (func.Body is ParameterExpression pex)
            {
                Columns = "[{0}].*".format(pex.Name);
            }
            else if (func.Body is BinaryExpression bex)
            {
                if (bex.NodeType == ExpressionType.Coalesce)
                {
                    string FieldName = ExpressionRouterModel(bex.Left);
                    var mex = bex.Left as MemberExpression;
                    string FieldValue = ExpressionRouterModel(bex.Right);
                    /*if (FieldValue.IsMatch(@"@ParamName\d+"))
                    {
                        FieldValue = this.DataSQL.Parameters[FieldValue].GetValue(mex.Type).GetValue();
                        FieldValue = (FieldValue.isFloat() ? "{0}" : "'{0}'").format(FieldValue);
                    }*/
                    return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
                }
            }
            else
            {
                Columns = this.ExpressionRouterModel(func.Body);
            }
            return Columns.Trim(',');
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3
    /// <summary>
    /// Queryable驱动
    /// </summary>
    /// <typeparam name="T">第一个类型</typeparam>
    /// <typeparam name="T2">第二个类型</typeparam>
    /// <typeparam name="T3">第三个类型</typeparam>
    public class QueryableProvider<T, T2, T3> : QueryableProvider<T, T2>
    {
        #region 属性
        /// <summary>
        /// 配置数据
        /// </summary>
        public new virtual DataSQL3 DataSQL { get; set; }
        #endregion

        #region 设置存储过程参数
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public override string GetParamName(object value)
        {
            if (value == null) return "null";
            if (value is string str && str.IsNullOrEmpty()) return "''";
            if (value.ToString().IsMatch(@"@Sub_(\d+_)?ParamName\d+"))
                return value.ToString();
            if (value is bool) value = Convert.ToInt32(value);
            else if (value is Guid guid) value = guid;
            //var ParaName = "@ParamName" + (this.DataSQL.Parameters.Count + 1);
            /*var ParamIndex = 1;
            var ParamItems = this.DataSQL.Parameters.Where(a => a.Key.StartsWith("@ParamName")).Select(a => a.Key.Replace("@ParamName", "").ToCast<int>());
            if (ParamItems.Count() > 0)
                ParamIndex = ParamItems.Max() + 1;
            var ParaName = $"@ParamName{(this.DataSQL.Parameters.Count == 0 ? 1 : ParamIndex)}";*/
            var ParaName = $"@ParamName{this.DataSQL.Parameters.Count + 1}";
            this.AddParam(ParaName, value);
            return ParaName;
            //return (this.DataHelper.ProviderType == DbProviderType.Dameng || this.DataHelper.ProviderType == DbProviderType.MySql) ? "?" : ParaName;
        }
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">值</param>
        public override void AddParam(string name, object value)
        {
            if (value is Guid || value is DateTime) value = value.GetValue();
            if (value is bool) value = value.ToCast<int>();
            if (this.DataSQL.Parameters == null || !this.DataSQL.Parameters.Any())
            {
                this.DataSQL.Parameters = new Dictionary<string, object>()
                {
                    { name , value }
                };
                return;
            }
            if (!this.DataSQL.Parameters.ContainsKey(name))
                this.DataSQL.Parameters.Add(name, value);
            else
                this.DataSQL.Parameters[name] = value;
        }
        /// <summary>
        /// 获取存储过程参数值
        /// </summary>
        /// <returns></returns>
        public override DbParameter[] GetDbParameters()
        {
            var Params = new DbParameter[this.DataSQL.Parameters.Count];
            var i = 0;
            this.DataSQL.Parameters.Each(a =>
            {
                Params[i++] = this.DataHelper.MakeParam(@"" + a.Key, a.Value);
            });
            return Params;
        }
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="name">参数错</param>
        public override void RemoveParam(string name)
        {
            if (name.IsNullOrEmpty()) return;
            if (this.DataSQL.Parameters == null || !this.DataSQL.Parameters.Any()) return;
            if (this.DataSQL.Parameters.ContainsKey(name))
                this.DataSQL.Parameters.Remove(name);
        }
        /// <summary>
        /// 获取Param值
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <returns></returns>
        public override object GetParamValue(string ParamName)
        {
            if (ParamName.IsNullOrEmpty()) return "";
            return this.DataSQL.Parameters.TryGetValue((ParamName.IsMatch(@"^@") ? "" : "@") + ParamName, out var val) ? val : "";
        }
        /// <summary>
        /// 获取所有的参数集
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetParameters()
        {
            return this.DataSQL.Parameters;
        }
        #endregion

        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="func">返回结果Lambda</param>
        /// <param name="orderType">排序类型 asc,desc</param>
        /// <returns></returns>
        public virtual string OrderByString<TResult>(Expression<Func<T, T2, T3, TResult>> func, string orderType = "asc")
        {
            if (func == null) return "";
            string OrderByString = ExpressionRouterModel(func.Body);
            if (OrderByString.IsNotNullOrEmpty())
                return OrderByString.ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            return "";
            /*if (func.Body is NewExpression ex)
            {
                ex.Members.Each(a =>
                {
                    OrderByString += a.Name + " {0},".format(orderType);
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                OrderByString += mex.Member.Name + " {0}".format(orderType);
            }
            else if (func.Body is BinaryExpression bex)
            {
                OrderByString += this.ExpressionRouter(bex).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            else if (func.Body is NewArrayExpression naex)
            {
                OrderByString += this.ExpressionRouter(naex).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            else if (func.Body is MethodCallExpression mce)
            {
                OrderByString += this.ExpressionRouter(mce).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            if (OrderByString.IsNullOrEmpty()) return "";
            return OrderByString;*/
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="func">结果Lambda</param>
        /// <returns></returns>
        public virtual string SetColumns<TResult>(Expression<Func<T, T2, T3, TResult>> func)
        {
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression mie)
            {
                mie.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = this.ExpressionRouterModel(b.Expression);
                    Columns += ",{0} as {1}".format(value, FieldFormat(b.Member.Name));
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = this.ExpressionRouterModel(nex.Arguments[i]);
                    Columns += ",{0} as {1}".format(value, FieldFormat(nex.Members[i].Name));
                }
            }
            else if (func.Body is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(fex.IfTrue), ExpressionRouterModel(fex.IfFalse));
                    /*if (_.IsMatch(@"@ParamName\d+"))
                    {
                        _.getPatterns(@"@ParamName\d+").Each(a =>
                        {
                            var _val = this.DataSQL.Parameters[a].ToString();
                            _ = _.ReplacePattern(@"" + a, (_val.isFloat() ? "{0}" : "'{0}'").format(_val));
                        });
                    }*/
                    return _;
                }
            }
            else if (func.Body is ParameterExpression pex)
            {
                Columns = "[{0}].*".format(pex.Name);
            }
            else if (func.Body is BinaryExpression bex)
            {
                if (bex.NodeType == ExpressionType.Coalesce)
                {
                    string FieldName = ExpressionRouterModel(bex.Left);
                    var mex = bex.Left as MemberExpression;
                    string FieldValue = ExpressionRouterModel(bex.Right);
                    /*if (FieldValue.IsMatch(@"@ParamName\d+"))
                    {
                        FieldValue = this.DataSQL.Parameters[FieldValue].GetValue(mex.Type).GetValue();
                        FieldValue = (FieldValue.isFloat() ? "{0}" : "'{0}'").format(FieldValue);
                    }*/
                    return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
                }
            }
            else
            {
                Columns = ExpressionRouterModel(func.Body);
            }
            return Columns.Trim(',');
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3,T4
    /// <summary>
    /// Queryable驱动
    /// </summary>
    /// <typeparam name="T">第一个类型</typeparam>
    /// <typeparam name="T2">第二个类型</typeparam>
    /// <typeparam name="T3">第三个类型</typeparam>
    /// <typeparam name="T4">第四个类型</typeparam>
    public class QueryableProvider<T, T2, T3, T4> : QueryableProvider<T, T2, T3>
    {
        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="func">返回结果Lambda</param>
        /// <param name="orderType">排序类型 asc,desc</param>
        /// <returns></returns>
        public virtual string OrderByString<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func, string orderType = "asc")
        {
            if (func == null) return "";
            string OrderByString = ExpressionRouterModel(func.Body);
            if (OrderByString.IsNotNullOrEmpty())
                return OrderByString.ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            return "";
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="func">结果Lambda</param>
        /// <returns></returns>
        public virtual string SetColumns<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func)
        {
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression lex)
            {
                lex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = this.ExpressionRouterModel(b.Expression);
                    Columns += ",{0} as {1}".format(value, FieldFormat(b.Member.Name));
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = this.ExpressionRouterModel(nex.Arguments[i]);
                    Columns += ",{0} as {1}".format(value, FieldFormat(nex.Members[i].Name));
                }
            }
            else if (func.Body is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(fex.IfTrue), ExpressionRouterModel(fex.IfFalse));
                    /*if (_.IsMatch(@"@ParamName\d+"))
                    {
                        _.getPatterns(@"@ParamName\d+").Each(a =>
                        {
                            var _val = this.DataSQL.Parameters[a].ToString();
                            _ = _.ReplacePattern(@"" + a, (_val.isFloat() ? "{0}" : "'{0}'").format(_val));
                        });
                    }*/
                    return _;
                }
            }
            else if (func.Body is ParameterExpression pex)
            {
                Columns = "[{0}].*".format(pex.Name);
            }
            else if (func.Body is BinaryExpression bex)
            {
                if (bex.NodeType == ExpressionType.Coalesce)
                {
                    string FieldName = ExpressionRouterModel(bex.Left);
                    var mex = bex.Left as MemberExpression;
                    string FieldValue = ExpressionRouterModel(bex.Right);
                    /*if (FieldValue.IsMatch(@"@ParamName\d+"))
                    {
                        FieldValue = this.DataSQL.Parameters[FieldValue].GetValue(mex.Type).GetValue();
                        FieldValue = (FieldValue.isFloat() ? "{0}" : "'{0}'").format(FieldValue);
                    }*/
                    return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
                }
            }
            else
            {
                Columns = ExpressionRouterModel(func.Body);
            }
            return Columns.Trim(',');
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3,T4,T5
    /// <summary>
    /// Queryable驱动
    /// </summary>
    /// <typeparam name="T">第一个类型</typeparam>
    /// <typeparam name="T2">第二个类型</typeparam>
    /// <typeparam name="T3">第三个类型</typeparam>
    /// <typeparam name="T4">第四个类型</typeparam>
    /// <typeparam name="T5">第五个类型</typeparam>
    public class QueryableProvider<T, T2, T3, T4, T5> : QueryableProvider<T, T2, T3, T4>
    {
        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="func">返回结果Lambda</param>
        /// <param name="orderType">排序类型 asc,desc</param>
        /// <returns></returns>
        public virtual string OrderByString<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func, string orderType = "asc")
        {
            if (func == null) return "";
            string OrderByString = ExpressionRouterModel(func.Body);
            if (OrderByString.IsNotNullOrEmpty())
                return OrderByString.ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            return "";
            /*
            if (func.Body is NewExpression ex)
            {
                ex.Members.Each(a =>
                {
                    OrderByString += a.Name + " {0},".format(orderType);
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                OrderByString += mex.Member.Name + " {0}".format(orderType);
            }
            else if (func.Body is BinaryExpression bex)
            {
                OrderByString += this.ExpressionRouter(bex).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            else if (func.Body is NewArrayExpression naex)
            {
                OrderByString += this.ExpressionRouter(naex).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            else if (func.Body is MethodCallExpression mce)
            {
                OrderByString += this.ExpressionRouter(mce).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            if (OrderByString.IsNullOrEmpty()) return "";
            return OrderByString;*/
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="func">结果Lambda</param>
        /// <returns></returns>
        public virtual string SetColumns<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func)
        {
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression miex)
            {
                miex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = this.ExpressionRouterModel(b.Expression);
                    Columns += ",{0} as {1}".format(value, FieldFormat(b.Member.Name));
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = this.ExpressionRouterModel(nex.Arguments[i]);
                    Columns += ",{0} as {1}".format(value, FieldFormat(nex.Members[i].Name));
                }
            }
            else if (func.Body is ConditionalExpression cex)
            {
                if (cex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(cex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(cex.IfTrue), ExpressionRouterModel(cex.IfFalse));
                    /*if (_.IsMatch(@"@ParamName\d+"))
                    {
                        _.getPatterns(@"@ParamName\d+").Each(a =>
                        {
                            var _val = this.DataSQL.Parameters[a].ToString();
                            _ = _.ReplacePattern(@"" + a, (_val.isFloat() ? "{0}" : "'{0}'").format(_val));
                        });
                    }*/
                    return _;
                }
            }
            else if (func.Body is ParameterExpression pex)
            {
                Columns = "[{0}].*".format(pex.Name);
            }
            else if (func.Body is BinaryExpression bex)
            {
                if (bex.NodeType == ExpressionType.Coalesce)
                {
                    string FieldName = ExpressionRouterModel(bex.Left);
                    var mex = bex.Left as MemberExpression;
                    string FieldValue = ExpressionRouterModel(bex.Right);
                    /*if (FieldValue.IsMatch(@"@ParamName\d+"))
                    {
                        FieldValue = this.DataSQL.Parameters[FieldValue].GetValue(mex.Type).GetValue();
                        FieldValue = (FieldValue.isFloat() ? "{0}" : "'{0}'").format(FieldValue);
                    }*/
                    return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
                }
            }
            else
            {
                Columns = ExpressionRouterModel(func.Body);
            }
            return Columns.Trim(',');
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3,T4,T5,T6
    /// <summary>
    /// Queryable驱动
    /// </summary>
    /// <typeparam name="T">第一个类型</typeparam>
    /// <typeparam name="T2">第二个类型</typeparam>
    /// <typeparam name="T3">第三个类型</typeparam>
    /// <typeparam name="T4">第四个类型</typeparam>
    /// <typeparam name="T5">第五个类型</typeparam>
    /// <typeparam name="T6">第六个类型</typeparam>
    public class QueryableProvider<T, T2, T3, T4, T5, T6> : QueryableProvider<T, T2, T3, T4, T5>
    {
        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TResult">返回结果类型</typeparam>
        /// <param name="func">返回结果Lambda</param>
        /// <param name="orderType">排序类型 asc,desc</param>
        /// <returns></returns>
        public virtual string OrderByString<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func, string orderType = "asc")
        {
            if (func == null) return "";
            string OrderByString = ExpressionRouterModel(func.Body);
            if (OrderByString.IsNotNullOrEmpty())
                return OrderByString.ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            return "";
            /*
            if (func.Body is NewExpression ex)
            {
                ex.Members.Each(a =>
                {
                    OrderByString += a.Name + " {0},".format(orderType);
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                OrderByString += mex.Member.Name + " {0}".format(orderType);
            }
            else if (func.Body is BinaryExpression bex)
            {
                OrderByString += this.ExpressionRouter(bex).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            else if (func.Body is NewArrayExpression naex)
            {
                OrderByString += this.ExpressionRouter(naex).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            else if (func.Body is MethodCallExpression mce)
            {
                OrderByString += this.ExpressionRouter(mce).ReplacePattern(@"(,|$)", " {0}$1".format(orderType));
            }
            if (OrderByString.IsNullOrEmpty()) return "";
            return OrderByString;*/
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="func">结果Lambda</param>
        /// <returns></returns>
        public virtual string SetColumns<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func)
        {
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression lex)
            {
                lex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = this.ExpressionRouterModel(b.Expression);
                    Columns += ",{0} as {1}".format(value, FieldFormat(b.Member.Name));
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = this.ExpressionRouterModel(nex.Arguments[i]);
                    Columns += ",{0} as {1}".format(value, FieldFormat(nex.Members[i].Name));
                }
            }
            else if (func.Body is ConditionalExpression fex)
            {
                if (fex.NodeType == ExpressionType.Conditional)
                {
                    var _ = "";
                    var _case = ExpressionRouterModel(fex.Test);
                    if (!_case.IsMatch(@"(=|is|<>|>|>=|<|<=)")) _case += " = true";
                    _ = "(case when {0} then {1} else {2} end)".format(_case, ExpressionRouterModel(fex.IfTrue), ExpressionRouterModel(fex.IfFalse));
                    /*if (_.IsMatch(@"@ParamName\d+"))
                    {
                        _.getPatterns(@"@ParamName\d+").Each(a =>
                        {
                            var _val = this.DataSQL.Parameters[a].ToString();
                            _ = _.ReplacePattern(@"" + a, (_val.isFloat() ? "{0}" : "'{0}'").format(_val));
                        });
                    }*/
                    return _;
                }
            }
            else if (func.Body is ParameterExpression pex)
            {
                Columns = "[{0}].*".format(pex.Name);
            }
            else if (func.Body is BinaryExpression bex)
            {
                if (bex.NodeType == ExpressionType.Coalesce)
                {
                    string FieldName = ExpressionRouterModel(bex.Left);
                    var mex = bex.Left as MemberExpression;
                    string FieldValue = ExpressionRouterModel(bex.Right);
                    /*if (FieldValue.IsMatch(@"@ParamName\d+"))
                    {
                        FieldValue = this.DataSQL.Parameters[FieldValue].GetValue(mex.Type).GetValue();
                        FieldValue = (FieldValue.isFloat() ? "{0}" : "'{0}'").format(FieldValue);
                    }*/
                    return "(case when {0} is null then {1} else {2} end)".format(FieldName, FieldValue, FieldName);
                }
            }
            else
            {
                Columns = ExpressionRouterModel(func.Body);
            }
            return Columns.Trim(',');
        }
        #endregion
    }
    #endregion
}