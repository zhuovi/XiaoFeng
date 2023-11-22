using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 程序集扩展类
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 是否是系统程序集
        /// <summary>
        /// 是否是系统程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        public static bool IsSystemAssembly(this Assembly assembly)
        {
            if (assembly == null) return false;
            var name = assembly.FullName;
            return name.IsMatch(@"PublicKeyToken\s*=\s*(b77a5c561934e089|b03f5f7f11d50a3a|89845dcd8080cc91|31bf3856ad364e35)$");
        }
        #endregion

        #region 获取当前类的所有子类
        /// <summary>
        /// 获取当前类的所有子类
        /// </summary>
        /// <param name="baseType">基类</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetChildrenClass(this Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => baseType.Name == t.BaseType?.Name));
        }
        #endregion

        #region 判断属性是否是索引
        /// <summary>
        /// 判断属性是否是索引 this[]
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns></returns>
        public static Boolean IsIndexer(this PropertyInfo property) => property.GetIndexParameters().Length > 0;
        #endregion

        #region 获取类型属性及字段
        /// <summary>
        /// 获取类型属性及字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="func">委托</param>
        /// <returns>类型的属性及字段</returns>
        public static IEnumerable<MemberInfo> GetPropertiesAndFields(this Type type, Func<MemberInfo, Boolean> func)
        {
            if (type.IsNullOrEmpty()) return null;
            var keys = new List<string>();
            var list = new List<MemberInfo>(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            list.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            var _list = new List<MemberInfo>();
            list.Each(m =>
              {
                  /*如果是 索引器 则跳过*/
                  if (m is PropertyInfo _p && _p.GetIndexParameters().Length > 0) return true;
                  /*如果是 被重写的属性 则跳过*/
                  if (keys.Contains(m.Name) && m.DeclaringType != type) return true;

                  /*如果有忽略属性 则跳过*/
                  if (m.IsDefined(typeof(FieldIgnoreAttribute), false)) return true;

                  string name = m.Name;
                  keys.Add(name);
                  _list.Add(m);
                  if (func != null)
                      return func.Invoke(m);
                  return true;
              });
            keys.Clear();
            return _list;
        }
        /// <summary>
        /// 获取类型属性及字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="action">委托</param>
        /// <returns>类型的属性及字段</returns>
        public static IEnumerable<MemberInfo> GetPropertiesAndFields(this Type type, Action<MemberInfo> action = null)
        {
            if (type.IsNullOrEmpty()) return null;
            var keys = new List<string>();
            var list = new List<MemberInfo>(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            list.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            var _list = new List<MemberInfo>();
            list.Each(m =>
            {
                /*如果是 索引器 则跳过*/
                if (m is PropertyInfo _p && _p.GetIndexParameters().Length > 0) return;
                /*如果是 被重写的属性 则跳过*/
                if (keys.Contains(m.Name) && m.DeclaringType != type) return;

                /*如果有忽略属性 则跳过*/
                if (m.IsDefined(typeof(FieldIgnoreAttribute), false)) return;

                string name = m.Name;
                keys.Add(name);
                _list.Add(m);
                action?.Invoke(m);
            });
            keys.Clear();
            return _list;
        }
        #endregion
    }
}