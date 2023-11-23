using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using XiaoFeng.Net;

namespace XiaoFeng
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
    ===================================================================
    */
    /// <summary>
    /// 自定义属性扩展
    /// Verstion : 1.2.0
    /// Create Time : 2017/11/15 09:41:58
    /// Update Time : 2018/04/09 16:01:29
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 方法
        /// <summary>
        /// 获取表索引值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static TableIndexAttribute[] GetTableIndexAttributes<T>(this T t, bool inherit = true)
        {
            Type type = t is Type ? t as Type : typeof(T);
            return type.GetCustomAttributes<TableIndexAttribute>(inherit)?.ToArray();
        }
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static TableAttribute GetTableAttribute<T>(this T t, bool inherit = true)
        {
            Type type = t is Type ? t as Type : typeof(T);
            return type.GetCustomAttribute<TableAttribute>(inherit);
        }
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static ViewAttribute GetViewAttribute<T>(this T t, bool inherit = true)
        {
            Type type = t is Type ? t as Type : typeof(T);
            return type.GetCustomAttribute<ViewAttribute>(inherit);
        }
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static ColumnAttribute GetColumnAttribute<T>(this T t, bool inherit = true) where T : MemberInfo
        {
            return (t as MemberInfo).GetCustomAttribute<ColumnAttribute>(inherit);
        }
        /// <summary>
        /// 获取自定义属性值
        /// </summary>
        /// <typeparam name="T">自定义属性类型</typeparam>
        /// <param name="t">类型</param>
        /// <param name="func">Lambda表达式</param>
        /// <param name="name">类型方法或属性</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static object GetCustomAttributeValue<T>(this Type t, Func<T, object> func, string name = "", bool inherit = true) where T : Attribute
        {
            object val = new object();
            T tc = default;
            if (t == null) return tc;
            if (name.IsNullOrEmpty())
            {
                tc = t.GetCustomAttribute<T>(inherit);
                if (tc == null) return val;
            }
            else
            {
                MemberInfo mInfo = t.GetMember(name).FirstOrDefault();
                if (mInfo == null) return null;
                tc = mInfo.GetCustomAttribute(typeof(T), false) as T;
            }
            val = tc == null ? null : func(tc);
            return val;
        }
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T">自定义属性类型</typeparam>
        /// <param name="t">自定义属性类型</param>
        /// <param name="inherit">是否往父类找</param>
        /// <returns></returns>
        [Obsolete("被系统替换")]
        public static T GetCustomAttributeBAK<T>(this Type t, bool inherit = true) where T : Attribute
        {
            T tc = default;
            if (t == null) return tc;
            object _t = t.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
            return (_t ?? tc) as T;
        }
        /// <summary>
        /// 获取自定义属性值
        /// </summary>
        /// <typeparam name="T">自定义属性类型</typeparam>
        /// <param name="m">属性对象</param>
        /// <param name="func">Lambda表达式</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static object GetCustomAttributeValue<T>(this MemberInfo m, Func<T, object> func, bool inherit = true) where T : Attribute
        {
            if (func == null) return null;
            T val = m.GetCustomAttribute<T>(inherit);
            return val == default(T) ? default(T) : func(val);
        }
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T">自定义属性类型</typeparam>
        /// <param name="m">属性对象</param>
        /// <returns></returns>
        public static T GetCustomAttributeX<T>(this MemberInfo m) where T : Attribute
        {
            T tc = default;
            if (m == null) return tc;
            object _t = m.GetCustomAttributes(typeof(T), true).FirstOrDefault();
            return (_t ?? tc) as T;
        }
        /// <summary>
        /// 获取指定属性或事件的描述
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDescription(this Type t, bool inherit = true)
        {
            return t.GetCustomAttributeValue<DescriptionAttribute>(a => a.Description, inherit).ToString();
        }
        /// <summary>
        /// 获取枚举Description
        /// </summary>
        /// <param name="_">枚举</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDescription(this Enum _, bool inherit = true)
        {
            var d = _.GetType().GetMember(_.ToString());
            if (d.Length == 0) return String.Empty;
            return d[0].GetDescription(inherit);
        }
        /// <summary>
        /// 获取指定属性的DefaultValue
        /// </summary>
        /// <param name="m">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDefaultValue(this MemberInfo m, bool inherit = true)
        {
            if (m == null || !m.IsDefined(typeof(DefaultValueAttribute), inherit)) return String.Empty;
            object val = m.GetCustomAttributeValue<DefaultValueAttribute>(a => a.Value, inherit);
            return val == null ? String.Empty : val.ToString();
        }
        /// <summary>
        /// 获取指定属性的DefaultValue
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDefaultValue(this Type t, bool inherit = true)
        {
            return t.GetCustomAttributeValue<DefaultValueAttribute>(a => a.Value, inherit).ToString();
        }
        /// <summary>
        /// 获取枚举DefaultValue
        /// </summary>
        /// <param name="_">枚举</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDefaultValue(this Enum _, bool inherit = true)
        {
            return _.GetType().GetMember(_.ToString())[0].GetDefaultValue(inherit);
        }
        /// <summary>
        /// 是否包含指定特性
        /// </summary>
        /// <typeparam name="T">特性</typeparam>
        /// <param name="_">枚举</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static bool IsDefined<T>(this Enum _, bool inherit = true) where T : Attribute
        {
            return _.GetType().GetMember(_.ToString())[0].IsDefined(typeof(T), inherit);
        }
        /// <summary>
        /// 获取指定属性或事件的描述
        /// </summary>
        /// <param name="m">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo m, bool inherit = true)
        {
            if (m == null || !m.IsDefined(typeof(DescriptionAttribute))) return String.Empty;
            object val = m.GetCustomAttributeValue<DescriptionAttribute>(a => a.Description, inherit);
            return val == null ? String.Empty : val.ToString();
        }
        /// <summary>
        /// 获取指定属性或事件的枚举名称
        /// </summary>
        /// <param name="m">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetEnumName(this MemberInfo m, bool inherit = true)
        {
            object val = m.GetCustomAttributeValue<EnumNameAttribute>(a => a.Name, inherit);
            return val == null ? "" : val.ToString();
        }
        /// <summary>
        /// 获取指定属性或事件的端口
        /// </summary>
        /// <param name="m">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static int GetPort(this MemberInfo m, bool inherit = true)
        {
            object val = m.GetCustomAttributeValue<PortAttribute>(a => a.Port, inherit);
            return val == null ? 0 : (int)val;
        }
        /// <summary>
        /// 获取指定属性或事件的描述
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDisplayName(this Type t, bool inherit = true)
        {
            return t.GetCustomAttributeValue<DisplayNameAttribute>(a => a.DisplayName, inherit).ToString();
        }
        /// <summary>
        /// 获取指定属性或事件的描述
        /// </summary>
        /// <param name="m">类型</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static string GetDisplayName(this MemberInfo m, bool inherit = true)
        {
            return m.GetCustomAttributeValue<DisplayNameAttribute>(a => a.DisplayName, inherit).ToString();
        }
        /// <summary>
        /// 获取元素类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Type GetElementXType(this Type type)
        {
            if (type.HasElementType) return type.GetElementType();
            if (type.As<IEnumerable>())
            {
                // 如果实现了IEnumerable<>接口，那么取泛型参数
                foreach (var item in type.GetInterfaces())
                {
                    if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return item.GetGenericArguments()[0];
                }
            }
            return null;
        }
        /// <summary>
        /// 是否包括字段忽略属性
        /// </summary>
        /// <param name="member">成员元数据</param>
        /// <param name="inherit">是否向父类查找</param>
        /// <returns></returns>
        public static Boolean HasFieldIgnore(this MemberInfo member, bool inherit = true) => member.GetCustomAttribute<FieldIgnoreAttribute>(inherit) != null;
        #endregion
    }
}