using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Data.SQL;

namespace XiaoFeng.Expressions
{
    /// <summary>
    /// 处理Lambda表达式
    /// </summary>
    public class QueryableHelper
    {
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
            if (leftSQL == "1") leftSQL = "1 = 1";
            else if (leftSQL == "0") leftSQL = "1 = 0";
            SubSQL += leftSQL;
            if (left.Type == typeof(bool) && right.Type == typeof(bool) && right.NodeType == ExpressionType.Constant)
                SubSQL = SubSQL.RemovePattern(@" = [01]");
            SubSQL += ExpressionTypeCast(type);
            /*再处理右边*/
            string tmpStr = ExpressionRouter(right);
            
            if (tmpStr == "null")
            {
                if (SubSQL.Trim().EndsWith("="))
                    SubSQL = SubSQL.ReplacePattern(@"=\s*", " is null ");
                else if (SubSQL.Trim().EndsWith("<>"))
                    SubSQL = SubSQL.ReplacePattern(@"<>\s*", " is not null ");
            }
            else
                SubSQL += tmpStr;
            return SubSQL += ")";
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
            Type type = val.GetType();
            ValueTypes types = type.GetValueType();
            if (types == ValueTypes.Value || types == ValueTypes.String)
                return val.GetSqlValue();
            else if (types == ValueTypes.Array || types == ValueTypes.ArrayList || types == ValueTypes.List || types == ValueTypes.IEnumerable)
            {
                string _ = string.Empty;
                foreach (var o in val as IEnumerable)
                {
                    _ += "'{0}',".format(o.GetValue());
                }
                return _.TrimEnd(',');
            }
            return val.GetSqlValue();
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
            if (exp is BinaryExpression be)
            {
                return BinaryExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression me)
            {
                if (me.ToString().IsMatch(@"\s*value\([\s\S]+?\)"))
                {
                    object val = this.Eval(me);
                    return this.GetLambdaValue(val);
                }
                else if (me.Expression != null && me.Expression.GetType().Name == "TypedParameterExpression")
                {
                    if (me.Member.Name == "Now")
                        return DateTime.Now.GetSqlValue();
                    else
                        return me.Member.Name/* + (me.Type == typeof(bool) ? " = 1" : "")*/;
                }
                else if (me.Expression is MemberExpression mex)
                {
                    return mex.Member.Name;
                }
                else
                {
                    object val = this.Eval(me);
                    return this.GetLambdaValue(val);
                }
            }
            else if (exp is NewArrayExpression ae)
            {
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                if (mce.Type.Name == "Guid" && mce.Method.Name == "NewGuid")
                    return "'" + Guid.NewGuid().ToString("D") + "'";
                else if (mce.Method.Name == "Parse")
                {
                    object val = this.Eval(mce);
                    return this.GetLambdaValue(val);
                }
                else if (mce.Type.Name == "String" && mce.Method.Name == "ToString")
                {
                    return "Convert(nvarchar(40)," + ExpressionRouter(mce.Arguments.Count == 0 ? mce.Object : mce.Arguments[0]) + ")";
                }
            }
            else if (exp is ConstantExpression ce)
            {
                if (ce.Value == null)
                    return "null";
                else if (ce.Value.ToString() == "")
                    return "''";
                else if (ce.Value is bool)
                    return Convert.ToInt32(ce.Value).ToString();
                else if (ce.Value is DateTime || ce.Value.ToString().IsDate() || ce.Value.ToString().IsDateOrTime() || ce.Value is Guid || ce.Value is string || ce.Value is char)
                    return "'{0}'".format(ce.Value.GetValue());
                else if (ce.Value is ValueType)
                    return ce.Value.ToString();
                else
                    return ce.Value.GetValue();
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
        #endregion

        #region 设置节点连接类型
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
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
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
        }
        #endregion
    }
#if NETCOREAPP
    /// <summary>
    /// 动态创建表达式
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class QueryableHelper<T>
    {
        /// <summary>
        /// 键值对
        /// </summary>
        private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new ConcurrentDictionary<string, LambdaExpression>();
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="source">IQueryable</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        internal static IOrderedQueryable<T> OrderBy(IQueryable<T> source, string propertyName, SortDirection sortDirection)
        {
            dynamic keySelector = GetLambdaExpression(propertyName);
            return sortDirection == SortDirection.Ascending
                ? Queryable.OrderBy(source, keySelector)
                : Queryable.OrderByDescending(source, keySelector);
        }
        /// <summary>
        /// 附加排序
        /// </summary>
        /// <param name="source">源IQueryable</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        internal static IOrderedQueryable<T> ThenBy(IOrderedQueryable<T> source, string propertyName, SortDirection sortDirection)
        {
            dynamic keySelector = GetLambdaExpression(propertyName);
            return sortDirection == SortDirection.Ascending
                ? Queryable.ThenBy(source, keySelector)
                : Queryable.ThenByDescending(source, keySelector);
        }
        /// <summary>
        /// 获取表达式
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        private static LambdaExpression GetLambdaExpression(string propertyName)
        {
            if (Cache.ContainsKey(propertyName))
            {
                return Cache[propertyName];
            }
            ParameterExpression param = Expression.Parameter(typeof(T));
            MemberExpression body = Expression.Property(param, propertyName);
            LambdaExpression keySelector = Expression.Lambda(body, param);
            Cache[propertyName] = keySelector;
            return keySelector;
        }
    }
#endif
}