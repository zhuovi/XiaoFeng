using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using XiaoFeng.Expressions;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-09-18 00:51:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 扩展拼接Lambda表达式树
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 扩展与条件表达式树
        /// <summary>
        /// 扩展与条件表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">当前表达式树</param>
        /// <param name="second">与的表达式树</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            if (second == null) return first;
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(first.Body, second.Body), first.Parameters);
        }
        /// <summary>
        /// 扩展与条件表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="expressionType">表达式类型</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, ExpressionType expressionType, string propertyName, object value)
        {
            if (propertyName.IsNullOrEmpty()) return first;
            ParameterExpression parameter;
            if (first == null)
                parameter = Expression.Parameter(typeof(T), "a");
            else
                parameter = first.Parameters[0];
            var member = Expression.PropertyOrField(parameter, propertyName);

            var val = Expression.Constant(value, member.Type);

            var body = Expression.MakeBinary(expressionType, member, val);
            if (first != null)
                body = Expression.AndAlso(first.Body, body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        /// <summary>
        /// 扩展与条件表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="method">方法</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, MethodInfo method, string propertyName, params object[] value)
        {
            if (method == null || propertyName.IsNullOrEmpty()) return first;
            ParameterExpression parameter;
            if (first == null)
                parameter = Expression.Parameter(typeof(T), "a");
            else
                parameter = first.Parameters[0];
            Expression body;
            if (first != null)
                body = Expression.AndAlso(first.Body, first.Method(method, propertyName, value));
            else
            {
                var member = Expression.PropertyOrField(parameter, propertyName);
                var val = Expression.Constant(value, member.Type);
                body = Expression.Call(member, method, val);
            }
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        #endregion

        #region 扩展或条件表达式树
        /// <summary>
        /// 扩展或条件表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">当前表达式树</param>
        /// <param name="second">或的表达式树</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            if (second == null) return first;
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(first.Body, second.Body), first.Parameters);
        }
        /// <summary>
        /// 扩展与条件表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="method">表达式类型</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, MethodInfo method, string propertyName, params object[] value)
        {
            if (first == null || method == null || propertyName.IsNullOrEmpty()) return first;
            var body = Expression.OrElse(first.Body, first.Method(method, propertyName, value));
            return Expression.Lambda<Func<T, bool>>(body, first.Parameters);
        }
        #endregion

        #region 扩展包含表达式树
        /// <summary>
        /// 扩展包含表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Contains<T>(this Expression<Func<T, bool>> first, string propertyName, object value)
        {
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            return first.Method(method, propertyName, value);
        }
        #endregion

        #region 扩展不包含表达式树
        /// <summary>
        /// 扩展包含表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> NotContains<T>(this Expression<Func<T, bool>> first, string propertyName, object value)
        {
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            return first.NotMethod(method, propertyName, value);
        }
        #endregion

        #region 扩展方法表达式树
        /// <summary>
        /// 扩展方法表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="method">方法</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Method<T>(this Expression<Func<T, bool>> first, MethodInfo method, string propertyName, object value)
        {
            ParameterExpression parameter;
            if (first == null)
                parameter = Expression.Parameter(typeof(T), "a");
            else
                parameter = first.Parameters[0];
            var member = Expression.PropertyOrField(parameter, propertyName);
            var val = Expression.Constant(value, member.Type);
            var body = Expression.Call(member, method, val);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        #endregion

        #region 扩展非方法表达式树
        /// <summary>
        /// 扩展非方法表达式树
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="first">对象</param>
        /// <param name="method">方法</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> NotMethod<T>(this Expression<Func<T, bool>> first, MethodInfo method, string propertyName, object value)
        {
            var member = Expression.PropertyOrField(first.Parameters[0], propertyName);
            var val = Expression.Constant(value, member.Type);
            var body = Expression.Not(Expression.Call(member, method, val));
            return Expression.Lambda<Func<T, bool>>(body, first.Parameters);
        }
        #endregion

        #region 设置对象属性值
        /// <summary>
        /// 设置对象属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="TProperty">值类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="prop">属性表达式</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T SetValue<T, TProperty>(this T t, Expression<Func<T, TProperty>> prop, TProperty value)
        {
            var type = typeof(T);
            var mi = ((MemberExpression)prop.Body).Member;
            if (type.Name.StartsWith("<>f__AnonymousType"))
            {
                var field = type.GetField($"<{mi.Name}>i__Field", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null) return t;
                field.SetValue(t, value);
            }
            else if (t is IDictionary d)
            {
                if (d.Contains(mi.Name))
                    d[mi.Name] = value;
            }
            else
            {
                var field = type.GetPropertiesAndFields().First(a => a.Name.EqualsIgnoreCase(mi.Name));
                if (field == null) return t;
                if (field is PropertyInfo pi)
                    pi.SetValue(t, value);
                else if (field is FieldInfo fi)
                    fi.SetValue(t, value);
            }
            return t;
        }
        #endregion

        #region 替换表达式
        /// <summary>
        /// 替换表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="searchEx">搜索表达式</param>
        /// <param name="replaceEx">替换表达式</param>
        /// <returns></returns>
        public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }
        #endregion

        #region 转换表达式参数
        /// <summary>
        /// 转换表达式参数
        /// </summary>
        /// <typeparam name="NewParam">新参数</typeparam>
        /// <typeparam name="OldParam">老参数</typeparam>
        /// <typeparam name="TResult">结果</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static Expression<Func<NewParam, TResult>> To<NewParam, OldParam, TResult>(this
    Expression<Func<OldParam, TResult>> expression)
    where NewParam : OldParam
        {
            var param = Expression.Parameter(typeof(NewParam));
            return Expression.Lambda<Func<NewParam, TResult>>(
                expression.Body.Replace(expression.Parameters[0], param), param);
        }
        #endregion

        #region 获取编译数据
        /// <summary>
        /// 获取编译数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="paramName">参数名</param>
        /// <returns></returns>
        public static Func<T, bool> CompileGetValueExpression<T>(this Expression<Func<T, bool>> expression, string paramName = "a")
        {
            var propertyInfo = typeof(T);
            var instance = Expression.Parameter(propertyInfo, paramName);

            var lambda = Expression.Lambda<Func<T, bool>>(expression, instance);
            return lambda.Compile();
        }
        #endregion
    }
}