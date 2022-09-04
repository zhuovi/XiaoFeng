using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Data;
using System.ComponentModel;
using XiaoFeng.IO;
namespace XiaoFeng
{
    /// <summary>
    /// 扩展属性方法操作类
    /// Version : 1.1.9
    /// Author : jacky
    /// Site : www.zhuovi.com
    /// QQ : 7092734
    /// Email : jacky@zhuovi.com
    /// Description:
    /// v 1.1.4
    /// 1.扩展 ToEntity 更新转换实体时 默认为null 而不是实例化
    /// v 1.1.5 2018-07-12
    /// 1.扩展 获取对象基础类型
    /// 2.增加数据行转对象方法
    /// v 1.1.6 2018-08-20
    /// 1.把MD5扩展方法移到加密文件夹中
    /// v 1.1.7 2018-09-05
    /// 1.优化ToCast方法
    /// 2.优化 数字 溢出 返回 -2的问题
    /// v 1.1.8 2019-02-28
    /// 1.增加获取当前类型的所有基类
    /// v 1.1.9 2020-05-13
    /// 1.优化Enum转换
    /// 2.增加Enum 复合枚举转换
    /// v 1.2.0 2022-07-11
    /// 1.增加数组扩展方法 IndexOf Find FindAll FindIndex LastIndexOf FindLast FindLastIndex
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 获取对象基础类型
        /// <summary>
        /// 获取对象基础类型
        /// </summary>
        /// <param name="_">类型</param>
        /// <returns></returns>
        public static ValueTypes GetValueType(this Type _)
        {
            if (_ == null) return ValueTypes.Null;
            if (_.IsGenericType && _.GetGenericTypeDefinition() == typeof(Nullable<>))
                _ = _.GetGenericArguments()[0];
            if (_.IsValueType)
            {
                if (_.IsEnum)
                    return ValueTypes.Enum;
                else if (_ == typeof(short) || _ == typeof(int) || _ == typeof(long) || _ == typeof(double) || _ == typeof(float) || _ == typeof(decimal) || _ == typeof(byte) || _ == typeof(bool) || _ == typeof(char) || _ == typeof(DateTime) || _ == typeof(Guid) || _ == typeof(uint) || _ == typeof(ulong) || _ == typeof(ushort) || _ == typeof(sbyte))
                    return ValueTypes.Value;
                else if (_.IsPrimitive || (_.IsPublic && _.Namespace.IsMatch(@"^System")))
                    return ValueTypes.Value;
                else
                    return ValueTypes.Struct;
            }
            else if (_ == typeof(string))
                return ValueTypes.String;
            else if (_.IsClass)
            {
                if (_.Name == "DataTable")
                {
                    return ValueTypes.DataTable;
                }
                else if (_.Name.IsMatch(@"^<>f__AnonymousType\d+`\d+$"))
                    return ValueTypes.Anonymous;
                else if (!_.Namespace.IsMatch(@"^System") && !_.IsPrimitive && !_.IsArray)
                    return ValueTypes.Class;
                else
                {
                    if (_.IsArray)
                        return ValueTypes.Array;
                    else if (_.Name.IsMatch("Dictionary"))
                        return ValueTypes.Dictionary;
                    else if (_.Name == "List`1")
                        return ValueTypes.List;
                    else if (_ == typeof(ArrayList))
                        return ValueTypes.ArrayList;
                    else if (_.GetInterface("IDictionary") != null)
                        return ValueTypes.IDictionary;
                    else if (_.GetInterface("IEnumerable") != null)
                        return ValueTypes.IEnumerable;
                    else
                        return ValueTypes.Class;
                }
            }
            else if (_.GetInterface("IDictionary") != null)
                return ValueTypes.IDictionary;
            else if (_.GetInterface("IEnumerable") != null)
                return ValueTypes.IEnumerable;
            else return ValueTypes.Other;
        }
        #endregion

        #region 数据行转对象列表
        /// <summary>
        /// 数据行转对象列表
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="_">数据行</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataRow[] _)
        {
            List<T> list = new List<T>();
            if (_ == null || _.Length == 0) return list;
            DataTable table = _[0].Table.Clone();
            _.Each(dr => table.ImportRow(dr));
            return table.ToList<T>();
        }
        #endregion

        #region 数据表转换对象列表
        /// <summary>
        /// 数据表转换对象列表
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="_">数据表</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable _)
        {
            var list = new List<T>();
            if (_ == null || _.Rows.Count == 0) return list;
            T model = default(T);
            Type t = typeof(T);
            ValueTypes valueTypes = t.GetValueType();
            _.Rows.Each<DataRow>(dr =>
            {
                if (valueTypes == ValueTypes.Enum)
                    list.Add(dr[0].ToEnum<T>());
                else if (valueTypes == ValueTypes.Value || valueTypes == ValueTypes.String)
                    list.Add(dr[0].ToCast<T>());
                else if (valueTypes == ValueTypes.Class || valueTypes == ValueTypes.Struct)
                {
                    model = Activator.CreateInstance<T>();
                    var foreignKeys = 
                    dr.Table.Columns.Each<DataColumn>(dc =>
                    {
                        object drValue = dr[dc.ColumnName];
                        if (drValue == null || Convert.IsDBNull(drValue)) return;
                        PropertyInfo pi = t.GetProperty(dc.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        bool IsGeneric = false;
                        if (pi == null)
                        {
                            object m = model;
                            FieldInfo fi = t.GetField(dc.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (fi != null && fi.IsPublic)
                            {
                                Type _Field = fi.FieldType;
                                drValue = drValue.GetValue(_Field, out IsGeneric);
                                if (drValue != null || (drValue == null && IsGeneric))
                                    fi.SetValue(m, drValue);
                                model = (T)m;
                            }
                        }
                        else
                        {
                            Type _Field = pi.PropertyType;

                            drValue = drValue.GetValue(_Field, out IsGeneric);
                            if (drValue != null || (drValue == null && IsGeneric))
                                if (pi.CanWrite && !pi.IsIndexer()) pi.SetValue(model, drValue, null);
                        }
                    });
                    list.Add(model);
                }
                else if (valueTypes == ValueTypes.Anonymous)
                {
                    var constructor = t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .OrderBy(c => c.GetParameters().Length).First();
                    var parameters = constructor.GetParameters();
                    var values = new object[parameters.Length];
                    int index = 0;
                    parameters.Each(item => values[index++] = dr[item.Name].GetValue(item.ParameterType));
                    list.Add((T)constructor.Invoke(values));
                }
            });
            return list;
        }
        /// <summary>
        /// 转换成列表
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<object> ToList(this DataTable dataTable, Type type)
        {
            if (dataTable == null || dataTable.Rows.Count == 0) return null;
            var list = new List<object>();
            dataTable.Rows.Each<DataRow>(dr =>
            {
                list.Add(dr.ToEntity(type));
            });
            return list;
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object ToEntity(this DataTable dataTable,Type type)
        {
            if(dataTable==null|| dataTable.Rows.Count == 0) return null;
            return dataTable.Rows[0].ToEntity(type);
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="dataRow">行数据</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object ToEntity(this DataRow dataRow, Type type)
        {
            if (dataRow == null || dataRow.ItemArray == null || dataRow.ItemArray.Length == 0) return null;

            var valType = type.GetValueType();
            if (valType == ValueTypes.DataTable)
            {
                var dt = new DataTable();
                dt.Rows.Add(dataRow);
                return dt;
            }
            if (valType == ValueTypes.Dictionary || valType == ValueTypes.IDictionary)
            {
                var dic = Activator.CreateInstance(type) as IDictionary<object, object>;
                var items = dataRow.ItemArray;
                if (items.Length == 1)
                    dic.Add(items[0], null);
                else
                    dic.Add(items[0], items[1]);
                return dic;
            }
            if (valType == ValueTypes.ArrayList || valType == ValueTypes.Array || valType == ValueTypes.IEnumerable || valType == ValueTypes.List)
            {
                return dataRow.ItemArray;
            }
            if (valType == ValueTypes.Enum)
            {
                return dataRow.ItemArray[0].ToEnum(type);
            }
            if (valType == ValueTypes.String || valType == ValueTypes.Value)
                return dataRow.ItemArray[0].ToCast(type);
            if (valType == ValueTypes.Anonymous)
            {
                var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .OrderBy(c => c.GetParameters().Length).First();
                var parameters = constructor.GetParameters();
                var values = new object[parameters.Length];
                int index = 0;
                parameters.Each(item => values[index++] = dataRow[item.Name].GetValue(item.ParameterType));
                return constructor.Invoke(values);
            }
            if (valType == ValueTypes.Class)
            {
                var model = Activator.CreateInstance(type);
                dataRow.Table.Columns.Each<DataColumn>(c =>
                {
                    var ColumnName = c.ColumnName;
                    object drValue = dataRow[ColumnName];
                    if (drValue.IsNullOrEmpty()) return;
                    var p = type.GetProperty(ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (p == null || p.IsIndexer()) return;
                    var vType = p.PropertyType.GetValueType();
                    var val = drValue;
                    var Foreign = p.GetCustomAttribute<ForeignAttribute>(false);

                    if (Foreign != null || vType == ValueTypes.IEnumerable || vType == ValueTypes.List || vType == ValueTypes.ArrayList)
                    {
                        return;
                    }
                    else
                        val = val.GetValue(p.PropertyType);
                    p.SetValue(model, val);
                });
                return model;
            }
            if (valType == ValueTypes.Struct)
            {
                var model = Activator.CreateInstance(type);
                dataRow.Table.Columns.Each<DataColumn>(c =>
                {
                    var ColumnName = c.ColumnName;
                    object drValue = dataRow[ColumnName];
                    if (drValue.IsNullOrEmpty()) return;
                    var f = type.GetField(ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (f == null) return;
                    f.SetValue(model, drValue.GetValue(f.FieldType));
                });
                return model;
            }
            return null;
        }
        #endregion

        #region 数据表转换对象
        /// <summary>
        /// 数据表转换对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="_">数据表</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataTable _)
        {
            if (_ == null || _.Rows.Count == 0) return default(T);
            DataTable table = _.Clone();
            table.ImportRow(_.Rows[0]);
            var list = table.ToList<T>();
            if (list == null || list.Count == 0) return default(T);
            return list[0];
        }
        /// <summary>
        /// 数据行转换对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="_">数据行</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow _)
        {
            if (_ == null) return default(T);
            DataTable table = _.Table.Clone();
            table.ImportRow(_);
            var list = table.ToList<T>();
            if (list == null || list.Count == 0) return default(T);
            return list[0];
        }
        #endregion

        #region 数组转换成字符串
        /// <summary>
        /// 数组转换成字符串
        /// </summary>
        /// <param name="values">数组对象</param>
        /// <param name="separator">连接字符串中间的字符</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">个数</param>
        /// <returns></returns>
        public static String Join(this String[] values, String separator, int startIndex, int count) => string.Join(separator, values, startIndex, count);
        /// <summary>
        /// 数组转换成字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="values">数组对象</param>
        /// <param name="separator">连接字符串中间的字符</param>
        /// <returns></returns>
        public static String Join<T>(this IEnumerable<T> values, String separator = "") => string.Join(separator, values);
        #endregion

        #region 键值对转换成对象
        /// <summary>
        /// 键值对转换成对象
        /// </summary>
        /// <typeparam name="T">对象 可以是class 也可以是struct</typeparam>
        /// <param name="d">键值对</param>
        /// <returns></returns>
        public static T DictionaryToObject<T>(this IDictionary<string, string> d)
        {
            Type t = typeof(T);
            T model = Activator.CreateInstance<T>();
            if (d == null || d.Count == 0) return model;
            var Types = t.GetValueType();
            if (Types == ValueTypes.Class || Types == ValueTypes.Struct)
            {
                object o = model;
                t.GetMembers().Each(m =>
                {
                    string _value = d.ContainsKey(m.Name) ? d[m.Name] : "";
                    if (m.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo p = m as PropertyInfo;
                        if (p == null || !p.CanWrite || p.IsIndexer()) return;
                        object value = _value.GetValue(p.PropertyType, out bool IsGeneric);
                        if (value != null || (value == null && IsGeneric))
                            p.SetValue(o, value, null);
                    }
                    else if (m.MemberType == MemberTypes.Field)
                    {
                        FieldInfo f = m as FieldInfo;
                        if (f == null) return;
                        object value = _value.GetValue(f.FieldType, out bool IsGeneric);
                        if (value != null || (value == null && IsGeneric))
                            f.SetValue(o, value);
                    }
                });
                model = (T)o;
            }
            return model;
        }
        #endregion

        #region 对象转键值对
        /// <summary>
        /// 对象转键值对
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="_">对象</param>
        /// <returns></returns>
        public static IDictionary<string, string> ObjectToDictionary<T>(this T _)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (_.IsNullOrEmpty()) return dic;
            Type type = typeof(T);
            if (type == typeof(string))
            {
                var str = _.ToString();
                if (str.IsMatch(@"^([^=&]+=[^&]*&?)+$"))
                {
                    str.GetMatches(@"(?<a>[^=&]+)=(?<b>[^&]*)").Each(a =>
                    {
                        var Key = a["a"];
                        var Value = a["b"];
                        if (!dic.ContainsKey(Key))
                            dic.Add(Key, Value);
                    });
                }
                else if (str.IsMatch(@"^(({[\s\S]+})|(\[[\s\S]+\]))$"))
                {
                    dic = str.JsonToObject<Dictionary<string, string>>();
                }
                return dic;
            }
            PropertyInfo[] infos = type.GetProperties();
            if (infos.Length == 0)
            {
                FieldInfo[] fields = type.GetFields();
                if (fields.Length == 0) return dic;
                else
                {
                    fields.Each(f =>
                    {
                        if (!dic.ContainsKey(f.Name)) dic.Add(f.Name, f.GetValue(_).getValue());
                    });
                }
            }
            else
            {
                infos.Each(i =>
                {
                    if (!dic.ContainsKey(i.Name)) dic.Add(i.Name, i.GetValue(_).getValue());
                });
            }
            return dic;
        }
        #endregion

        #region 转换成Enum类型
        /// <summary>
        /// 转换成Enum类型
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="o">常量，整型，无符号整型,长整型，无符号长整型，短整型，无符号短整型，字节</param>
        /// <param name="ignoreCase">True 忽略大小写 False考虑大小写</param>
        /// <returns></returns>
        public static T ToEnum<T>(this object o, Boolean ignoreCase = true)
        {
            if (!typeof(T).IsEnum || o.IsNullOrEmpty()) return default(T);
            return (T)o.ToEnum(typeof(T), ignoreCase);
        }
        /// <summary>
        /// 转换成Enum类型
        /// </summary>
        /// <param name="o">常量，整型，无符号整型,长整型，无符号长整型，短整型，无符号短整型，字节</param>
        /// <param name="type">类型</param>
        /// <param name="ignoreCase">True 忽略大小写 False考虑大小写</param>
        /// <returns></returns>
        public static object ToEnum(this object o, Type type, Boolean ignoreCase = true)
        {
            if (!type.IsEnum || o.IsNullOrEmpty()) return null;
            Type t = o.GetType();
            if (t == type) return o;
            if (o.ToString().IsNumberic())
            {
                var val = o.ToCast<int>();
                if (Enum.IsDefined(type, val))
                    return Enum.Parse(type, o.ToString(), ignoreCase);
                if (type.IsDefined(typeof(FlagsAttribute), false))
                {
                    var _ = "";
                    Enum.GetNames(type).Each(v =>
                    {
                        var _val = (int)Enum.Parse(type, v);
                        if ((val & _val) == _val) _ += "," + v;
                    });
                    return _.IsNullOrEmpty() ? type.GetEnumNames().First().ToEnum(type, ignoreCase) : Enum.Parse(type, _.Trim(','), ignoreCase);
                }
                else return type.GetEnumNames().First().ToEnum(type, ignoreCase);
            }
            if (t == typeof(String))
            {
                /*复合枚举*/
                if (o.ToString().IsMatch(@","))
                {
                    if (type.IsDefined(typeof(FlagsAttribute), false))
                    {
                        var names = Enum.GetNames(type);
                        var _names = o.ToString().RemovePattern(@"\s+");
                        var _ = "";
                        names.Each(n =>
                        {
                            if (_names.IsMatch(@"(^|,)" + n + @"(,|$)", ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)) _ += n + ",";
                        });
                        return _.IsNullOrEmpty() ? type.GetEnumNames().First().ToEnum(type, ignoreCase) : Enum.Parse(type, _.Trim(','), ignoreCase);
                    }
                    else return type.GetEnumNames().First().ToEnum(type, ignoreCase);
                }
                if (Enum.IsDefined(type, o))
                    return Enum.Parse(type, o.ToString(), ignoreCase);
                else
                {
                    var Name = type.GetFields().Where(b => b.Name.EqualsIgnoreCase(o.ToString()) || (b.IsDefined(typeof(EnumNameAttribute), false) && b.GetEnumName(false) == o.ToString()) || (b.IsDefined(typeof(DescriptionAttribute), false) && b.GetDescription(false) == o.ToString())).FirstOrDefault()?.Name;
                    return Name.IsNullOrEmpty() ? type.GetEnumNames().First().ToEnum(type, ignoreCase) : Enum.Parse(type, Name, ignoreCase);
                }
            }
            return type.GetEnumNames().First().ToEnum(type, ignoreCase);
        }
        #endregion

        #region 设置节点属性值
        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="elm">节点</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        /// <returns></returns>
        public static XmlElement SetAttributeValue<T>(this XmlElement elm, string attributeName, T attributeValue)
        {
            if (elm != null)
            {
                if (attributeValue == null || attributeValue.ToString().IsNullOrEmpty()) { if (elm.HasAttribute(attributeName)) elm.RemoveAttribute(attributeName); return elm; }
                string value;
                if (attributeValue.GetType() == typeof(int))
                    value = attributeValue.ToString();
                else if (attributeValue.GetType() == typeof(Boolean))
                    value = attributeValue.ToString();
                else if (attributeValue.GetType().IsEnum)
                    value = attributeValue.ToString();
                else if (attributeValue.GetType() == typeof(DateTime))
                    value = attributeValue.ToString().ToDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                else if (attributeValue.GetType() == typeof(Guid))
                    value = attributeValue.ToString().ToGUID().ToString("N");
                else
                    value = attributeValue.ToString();
                elm.SetAttribute(attributeName, value);
            }
            return elm;
        }
        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="elm">节点</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="attributeValue">属性值</param>
        /// <returns></returns>
        public static XmlNode SetAttributeValue<T>(this XmlNode elm, string attributeName, T attributeValue)
        {
            return ((XmlElement)elm).SetAttributeValue<T>(attributeName, attributeValue) as XmlNode;
        }
        #endregion

        #region 根据xpath获取节点
        /// <summary>
        /// 根据xpath获取节点
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="xpath">xpath</param>
        /// <param name="attrName">属性名</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public static XmlNode SelectSingleNode(this XmlNode node, string xpath = "", string attrName = "", string attrValue = "")
        {
            if (xpath.IsNullOrEmpty()) return node;
            else
            {
                if (attrName.IsNullOrEmpty())
                    return node.SelectSingleNode(xpath);
                else
                    return node.SelectSingleNode(xpath + "[translate(@" + attrName + ", 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='" + attrValue.ToLower() + "']");
            }
        }
        /// <summary>
        /// 根据xpath获取节点
        /// </summary>
        /// <param name="element">结点</param>
        /// <param name="xpath">xpath</param>
        /// <param name="attrName">属性名</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public static XmlNode SelectSingleNode(this XmlElement element, string xpath = "", string attrName = "", string attrValue = "") => (element as XmlNode).SelectSingleNode(xpath, attrName, attrValue);
        #endregion

        #region 循环遍历数据 For
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <param name="action">委托事件</param>
        /// <returns>返回对象</returns>
        public static IEnumerable<T> For<T>(this IEnumerable<T> enumerable, int start, int end, Action<int> action)
        {
            return enumerable.For(action, start, end);
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="action">委托事件</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <returns>返回对象</returns>
        public static IEnumerable<T> For<T>(this IEnumerable<T> enumerable, Action<int> action, int start = 0, int end = 0)
        {
            if (enumerable == null || action == null) return enumerable;
            int Count = enumerable.Count();
            if (Count > 0)
            {
                if (start < 0) start = 0;
                if (end == 0 || end > Count) end = Count;
                for (int i = start; i < end; i++)
                    action(i);
            }
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <param name="action">委托事件</param>
        /// <returns>返回对象</returns>
        public static T[] For<T>(this T[] enumerable, int start, int end, Action<int, T[]> action)
        {
            return enumerable.For(action, start, end);
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="action">委托事件</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <returns>返回对象</returns>
        public static T[] For<T>(this T[] enumerable, Action<int, T[]> action, int start = 0, int end = 0)
        {
            if (enumerable == null || action == null) return enumerable;
            int Count = enumerable.Length;
            if (Count > 0)
            {
                if (start < 0) start = 0;
                if (end == 0 || end > Count) end = Count;
                for (int i = start; i < end; i++)
                    action(i, enumerable);
            }
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <param name="func">委托事件</param>
        /// <returns>返回对象</returns>
        public static IEnumerable<T> For<T>(this IEnumerable<T> enumerable, int start, int end, Func<int, Boolean> func)
        {
            return enumerable.For(func, start, end);
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="func">委托事件</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <returns>返回对象</returns>
        public static IEnumerable<T> For<T>(this IEnumerable<T> enumerable, Func<int, Boolean> func, int start = 0, int end = 0)
        {
            if (enumerable == null || func == null) return enumerable;
            int Count = enumerable.Count();
            if (Count > 0)
            {
                if (start < 0) start = 0;
                if (end == 0 || end > Count) end = Count;
                for (int i = start; i < end; i++)
                    if (!func(i)) break;
            }
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <param name="func">委托事件</param>
        /// <returns>返回对象</returns>
        public static T[] For<T>(this T[] enumerable, int start, int end, Func<int, T[], Boolean> func)
        {
            return enumerable.For(func, start, end);
        }
        /// <summary>
        /// 循环遍历数据 For
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">对象</param>
        /// <param name="func">委托事件</param>
        /// <param name="start">数组开始</param>
        /// <param name="end">数组结束</param>
        /// <returns>返回对象</returns>
        public static T[] For<T>(this T[] enumerable, Func<int, T[], Boolean> func, int start = 0, int end = 0)
        {
            if (enumerable == null || func == null) return enumerable;
            int Count = enumerable.Length;
            if (Count > 0)
            {
                if (start < 0) start = 0;
                if (end == 0 || end > Count) end = Count;
                for (int i = start; i < end; i++)
                    if (!func(i, enumerable)) break;
            }
            return enumerable;
        }
        #endregion

        #region 循环遍历数据 ForEach
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">类型对象</param>
        /// <param name="action">无返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null || action == null) return enumerable;
            foreach (T t in enumerable) action(t);
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">类型对象</param>
        /// <param name="action">无返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            if (enumerable == null || enumerable.Count() == 0 || action == null) return enumerable;
            int i = 0;
            foreach (T t in enumerable) action(t, i++);
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">类型对象</param>
        /// <param name="func">有返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Func<T, Boolean> func)
        {
            if (enumerable == null || !enumerable.Any() || func == null) return enumerable;
            foreach (T t in enumerable) if (!func(t)) break;
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">类型对象</param>
        /// <param name="func">有返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Func<T, int, Boolean> func)
        {
            if (enumerable == null || !enumerable.Any() || func == null) return enumerable;
            int i = 0;
            foreach (T t in enumerable) if (!func(t, i++)) break;
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <param name="enumerable">对象</param>
        /// <param name="action">无返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable Each<T>(this IEnumerable enumerable, Action<T> action)
        {
            if (enumerable == null || action == null) return enumerable;
            foreach (T o in enumerable) action(o);
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <param name="enumerable">对象</param>
        /// <param name="action">无返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable Each<T>(this IEnumerable enumerable, Action<T, int> action)
        {
            if (enumerable == null || action == null) return enumerable;
            int i = 0;
            foreach (T o in enumerable) action(o, i++);
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <param name="enumerable">对象</param>
        /// <param name="func">有返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable Each<T>(this IEnumerable enumerable, Func<T, Boolean> func)
        {
            if (enumerable == null || func == null) return enumerable;
            foreach (T o in enumerable) if (!func(o)) break;
            return enumerable;
        }
        /// <summary>
        /// 循环遍历数据
        /// </summary>
        /// <param name="enumerable">对象</param>
        /// <param name="func">有返回值Lambda</param>
        /// <returns></returns>
        public static IEnumerable Each<T>(this IEnumerable enumerable, Func<T, int, Boolean> func)
        {
            if (enumerable == null || func == null) return enumerable;
            int i = 0;
            foreach (T o in enumerable) if (!func(o, i++)) break;
            return enumerable;
        }
        #endregion

        #region 获取IDictionary值
        /// <summary>
        /// 值是否在键值对中
        /// </summary>
        /// <typeparam name="TKey">类型</typeparam>
        /// <typeparam name="TValue">类型</typeparam>
        /// <param name="_">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Boolean ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> _, TValue value = default(TValue)) => _.Values.Contains(value);
        /// <summary>
        /// 获取Dictionary值 Value
        /// </summary>
        /// <typeparam name="TKey">Key 类型</typeparam>
        /// <typeparam name="TValue">Value 类型</typeparam>
        /// <param name="_">Dictionary对象</param>
        /// <param name="key">Key 值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static TValue Value<TKey, TValue>(this IDictionary<TKey, TValue> _, TKey key, TValue defaultValue = default(TValue)) => _.TryGetValue(key, out var val) ? val : defaultValue;
        /// <summary>
        /// 获取Dictionary值 Key
        /// </summary>
        /// <typeparam name="TKey">Key 类型</typeparam>
        /// <typeparam name="TValue">Value 类型</typeparam>
        /// <param name="_">Dictionary对象</param>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static TKey GetKey<TKey, TValue>(this IDictionary<TKey, TValue> _, TValue value, TKey defaultValue = default(TKey)) => _.ContainsValue(value) ? _.FindByValue(value).Key : defaultValue;
        /// <summary>
        /// 获取Dictionary值 Key
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">Dictionary对象</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static T GetKey<T>(this IDictionary<T, T> _, T value) => _.GetKey(value, value);
        /// <summary>
        /// 获取Dictionary值 KeyValuePair
        /// </summary>
        /// <typeparam name="TKey">Key 类型</typeparam>
        /// <typeparam name="TValue">Value 类型</typeparam>
        /// <param name="_">Dictionary对象</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static KeyValuePair<TKey, TValue> FindByValue<TKey, TValue>(this IDictionary<TKey, TValue> _, TValue value)
        {
            if (_ == null) return new KeyValuePair<TKey, TValue>();
            if (_.ContainsValue(value))
                foreach (KeyValuePair<TKey, TValue> item in _)
                    if (item.Value.Equals(value)) return item;
            return new KeyValuePair<TKey, TValue>();
        }
        #endregion

        #region IDictionary 转换成参数写法
        /// <summary>
        /// IDictionary 转换成参数写法
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="_">IDictionary对象</param>
        /// <returns></returns>
        public static string ToQuery<TKey, TValue>(this IDictionary<TKey, TValue> _)
        {
            if (_ == null || _.Count == 0) return String.Empty;
            var list = new List<string>();
            _.Each(KValue => list.Add($"{KValue.Key}={KValue.Value}"));
            return list.Join("&");
        }
        #endregion

        #region 转换首字母大小
        /// <summary>
        /// 转换首字母大小
        /// </summary>
        /// <param name="_">字符串对象</param>
        /// <returns></returns>
        public static string ToUpperFirst(this string _)
        {
            return _.ReplacePattern(@"(?<a>(\r|\t|\n|\s|\b))(?<b>[a-z])", m => m.Groups["a"].Value + m.Groups["b"].Value.ToUpper(), RegexOptions.Multiline);
        }
        #endregion

        #region 转义字符串
        /// <summary>
        /// 通过替换为转义码来转义最小的字符集（\、*、+、?、|、{、[、(、)、^、$、.、# 和空白）。 这将指示正则表达式引擎按原义解释这些字符而不是解释为元字符。
        /// </summary>
        /// <param name="_">包含要转换的文本的输入字符串</param>
        /// <returns>由转换为转义形式的元字符组成的字符串</returns>
        public static String ToRegexEscape(this String _) { return Regex.Escape(_); }
        /// <summary>
        /// 转换输入字符串中的任何转义字符
        /// </summary>
        /// <param name="_">包含要转换的文本的输入字符串</param>
        /// <returns>包含任何转换为非转义形式的转义字符的字符串</returns>
        public static String ToRegexUnescape(this String _) { return Regex.Unescape(_); }
        #endregion

        #region 字符串匹配模式
        /// <summary>
        /// 字符串匹配模式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static Boolean IsMatch(this string _, string pattern, int startIndex, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (pattern.IsNullOrEmpty()) return true;
            if (_.IsNullOrEmpty()) return false;
            var regex = new Regex(pattern, options);
            return startIndex <= -1 ? regex.IsMatch(_) : regex.IsMatch(_, startIndex);
        }
        /// <summary>
        /// 字符串匹配模式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">格式</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static Boolean IsMatch(this string _, string pattern, RegexOptions options = RegexOptions.IgnoreCase) => _.IsMatch(pattern, -1, options);
        /// <summary>
        /// 字符串不匹配
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static Boolean IsNotMatch(this string _, string pattern, RegexOptions options = RegexOptions.IgnoreCase) => !_.IsMatch(pattern, options);
        #endregion

        #region 提取符合模式的数据
        /// <summary>
        /// 提取符合模式的数据
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static List<string> GetPatterns(this string _, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            List<string> list = new List<string>();
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return list;
            _.GetMatches(pattern, options).Each(d => list.Add(d.Count > 0 ? d.ContainsKey("a") ? d["a"] : d.ContainsKey("1") ? d["1"] : d.FirstOrDefault().Value : ""));
            return list;
        }
        /// <summary>
        /// 提取符合模式的数据
        /// </summary>
        /// <param name="_">字串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> GetMatches(this String _, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return list;
            Regex regex = new Regex(pattern, options);
            MatchCollection mc = regex.Matches(_);
            if (mc.Count == 0) return list;
            mc.Each<Match>(m =>
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                regex.GetGroupNames().Each(o => d.Add(o, m.Groups[o].Value));
                list.Add(d);
            });
            return list;
        }
        /// <summary>
        /// 提取符合模式的数据
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMatchs(this String _, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            List<Dictionary<string, string>> list = _.GetMatches(pattern, options);
            return list.Count == 0 ? new Dictionary<string, string>() : list[0];
        }
        /// <summary>
        /// 提取符合模式的数据
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">模式</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public static string GetMatch(this String _, String pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            Dictionary<string, string> d = _.GetMatchs(pattern, options);
            return d.Count > 0 ? d.ContainsKey("a") ? d["a"] : d.ContainsKey("1") ? d["1"] : d.FirstOrDefault().Value : "";
        }
        /// <summary>
        /// 使用指定的匹配选项在输入字符串中搜索指定的正则表达式的第一个匹配项。
        /// </summary>
        /// <param name="_">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">长度</param>
        /// <param name="options">枚举值的一个按位组合，这些枚举值提供匹配选项</param>
        /// <returns></returns>
        public static Match Match(this String _, String pattern, int startIndex, int length, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return null;
            var regex = new Regex(pattern, options);
            return regex.Match(_, startIndex, length);
        }
        /// <summary>
        /// 使用指定的匹配选项在输入字符串中搜索指定的正则表达式的第一个匹配项。
        /// </summary>
        /// <param name="_">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="options">枚举值的一个按位组合，这些枚举值提供匹配选项</param>
        /// <returns></returns>
        public static Match Match(this String _, String pattern, int startIndex, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return null;
            var regex = new Regex(pattern, options);
            return regex.Match(_, startIndex);
        }
        /// <summary>
        /// 使用指定的匹配选项在输入字符串中搜索指定的正则表达式的第一个匹配项。
        /// </summary>
        /// <param name="_">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="options">枚举值的一个按位组合，这些枚举值提供匹配选项</param>
        /// <returns>一个包含有关匹配的信息的对象</returns>
        public static Match Match(this String _, String pattern, RegexOptions options = RegexOptions.IgnoreCase) => Regex.Match(_, pattern, options);
        /// <summary>
        /// 使用指定的匹配选项在指定的输入字符串中搜索指定的正则表达式的所有匹配项
        /// </summary>
        /// <param name="_">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="options">枚举值的一个按位组合，这些枚举值提供匹配选项</param>
        /// <returns>搜索操作找到的 System.Text.RegularExpressions.Match 对象的集合。 如果未找到匹配项，则此方法将返回一个空集合对象</returns>
        public static MatchCollection Matches(this String _, String pattern, RegexOptions options = RegexOptions.IgnoreCase) => Regex.Matches(_, pattern, options);
        #endregion

        #region 获取参数值
        /// <summary>
        /// 获取参数值 正则
        /// </summary>
        /// <param name="_">网址或参数集</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static string GetQuery(this string _, string key)
        {
            if (_.IsNullOrEmpty() || key.IsNullOrEmpty()) return "";
            return _.GetMatch(@"(^|[?&#])" + key + @"=(?<a>[^?&#]*)($|[&#])");
        }
        /// <summary>
        /// 获取参数值 参数组
        /// </summary>
        /// <param name="_">网址或参数集</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static string GetParam(this string _, string key)
        {
            if (_.IsNullOrEmpty() || key.IsNullOrEmpty()) return "";
            SortedDictionary<string, string> d = _.GetQuerys();
            return d.ContainsKey(key) ? d[key] : "";
        }
        /// <summary>
        /// 获取参数键值对
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetQuerys(this string _)
        {
            if (_.IsNullOrEmpty()) return new SortedDictionary<string, string>();
            using (QueryHelper queryHelper = new QueryHelper(_))
            {
                return queryHelper.data;
            }
        }
        /// <summary>
        /// 获取参数键值对
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetParams(this string _)
        {
            if (_.IsNullOrEmpty()) return new SortedDictionary<string, string>();
            using (ParamHelper queryHelper = new ParamHelper(_))
            {
                return queryHelper.querys;
            }
        }
        #endregion

        #region 替换移除模式
        /// <summary>
        /// 在指定输入子字符串内，使用指定替换字符串替换与某个正则表达式模式匹配的字符串（其数目为指定的最大数目）。
        /// </summary>
        /// <param name="_">要搜索匹配项的字符串。</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="replacement">替换字符串。</param>
        /// <param name="count">可进行替换的最大次数。</param>
        /// <param name="startIndex">输入字符串中开始执行搜索的字符位置。</param>
        /// <param name="options">模式选项</param>
        /// <returns>一个与输入字符串基本相同的新字符串，唯一的差别在于，其中的每个匹配字符串已被替换字符串代替。 如果正则表达式模式与当前实例不匹配，则此方法返回未更改的当前实例。</returns>
        public static string ReplacePattern(this string _, string pattern, string replacement, int count, int startIndex, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return _;
            var regex = new Regex(pattern, options);
            return regex.Replace(_, replacement, count, startIndex);
        }
        /// <summary>
        /// 替换模式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="replacement">替换字符串。</param>
        /// <param name="options">模式选项</param>
        /// <returns></returns>
        public static string ReplacePattern(this string _, string pattern, string replacement = "", RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return _;
            return Regex.Replace(_, pattern, replacement, options);
        }
        /// <summary>
        /// 在指定的输入子字符串内，使用 System.Text.RegularExpressions.MatchEvaluator 委托返回的字符串替换与某个正则表达式模式匹配的字符串（其数目为指定的最大数目）。
        /// </summary>
        /// <param name="_">要搜索匹配项的字符串。</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="m">一个自定义方法，该方法检查每个匹配项，然后返回原始的匹配字符串或替换字符串。</param>
        /// <param name="count">进行替换的最大次数。</param>
        /// <param name="startIndex">输入字符串中开始执行搜索的字符位置。</param>
        /// <param name="options">模式选项</param>
        /// <returns></returns>
        public static string ReplacePattern(this string _, string pattern, MatchEvaluator m, int count, int startIndex, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return _;
            var regex = new Regex(pattern, options);
            return regex.Replace(_, m, count, startIndex);
        }
        /// <summary>
        /// 替换模式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="m">方法操作过程中每当找到正则表达式匹配时都调用的方法</param>
        /// <param name="options">模式选项</param>
        /// <returns></returns>
        public static string ReplacePattern(this string _, string pattern, MatchEvaluator m, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (_.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return _;
            return Regex.Replace(_, pattern, m, options);
        }
        /// <summary>
        /// 移除模式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="options">模式选项</param>
        /// <returns></returns>
        public static string RemovePattern(this string _, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            return _.ReplacePattern(pattern, "", options);
        }
        #endregion

        #region 拆分字符串为数组
        /// <summary>
        /// 拆分字符串为数组
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">模式</param>
        /// <param name="options">模式选项</param>
        /// <returns></returns>
        public static string[] SplitPattern(this String _, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            return Regex.Split(_, pattern, options);
        }
        /// <summary>
        /// 拆分字符串为数组
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="pattern">模式</param>
        /// <param name="options">模式选项</param>
        /// <param name="matchTimeOut">超时间隔，或 System.Text.RegularExpressions.Regex.InfiniteMatchTimeout 指示该方法不应超时</param>
        /// <returns></returns>
        public static string[] SplitPattern(this String _, string pattern, RegexOptions options, TimeSpan matchTimeOut)
        {
            return Regex.Split(_, pattern, options, matchTimeOut);
        }
        #endregion

        #region 格式化数据
        /// <summary>
        /// 格式化数据[显示用]
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        public static string getValue(this object o)
        {
            if (o == null) return "";
            Type t = o.GetType();
            if (t.IsEnum) return t.IsDefined(typeof(FlagsAttribute)) ? o.ToString() : ((int)o).ToString();
            if (t == typeof(string)) return o.ToString();
            if (t == typeof(Guid)) return new Guid(o.ToString()).ToString("N");
            if (t == typeof(DateTime)) return ((DateTime)o).ToString("yyyy-MM-dd HH:mm:ss.fff");
            if (t == typeof(bool)) return o.ToString().ToLower();
            return o.ToString();
        }
        /// <summary>
        /// 格式化数据[数据库用]
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns></returns>
        public static string GetValue(this object o)
        {
            //return o.GetSqlValue();
            if (o == null) return "";
            Type t = o.GetType();
            if (t.IsEnum) return t.IsDefined(typeof(FlagsAttribute)) ? o.ToString() : ((int)o).ToString();
            if (t == typeof(string)) return o.ToString().ReplacePattern(@"'", "''");
            if (t == typeof(Guid) || t == typeof(Guid?)) return new Guid(o.ToString()).ToString("D");
            if (t == typeof(DateTime) || t == typeof(DateTime?)) return ((DateTime)o).ToString("yyyy-MM-dd HH:mm:ss.fff");
            if (t == typeof(bool) || t == typeof(bool?)) return Convert.ToInt32(o).ToString();
            return o.ToString();
        }
        /// <summary>
        /// 格式化数据[数据库用]
        /// </summary>
        /// <param name="_">对象</param>
        /// <returns></returns>
        public static string GetSqlValue(this object _)
        {
            if (_ is string && _.ToString().IsMatch(@"@ParamName\d+")) return _.ToString();
            if (_ == null) return "null";
            Type t = _.GetType();
            if (t == typeof(Guid) || t == typeof(Guid?)) return "'{0}'".format(new Guid(_.ToString()).ToString("D"));
            if (_.IsNullOrEmpty()) return "''";
            if (t.IsEnum) return t.IsDefined(typeof(FlagsAttribute)) ? _.ToString() : ((int)_).ToString();
            if (t == typeof(char)) return "'" + _.ToString() + "'";
            if (t == typeof(string)) return "'{0}'".format(_.ToString().ReplacePattern(@"'", "''"));
            if (t == typeof(DateTime) || t == typeof(DateTime?)) return "'{0}'".format(((DateTime)_).ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (t == typeof(bool) || t == typeof(bool?)) return Convert.ToInt32(_).ToString();
            return _.ToString();
        }
        #endregion

        #region 字符从一个编码转换成另一种编码
        /// <summary>
        /// 字符从一个编码转换成另一种编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="from">编码</param>
        /// <param name="to">目标编码</param>
        /// <returns></returns>
        public static string ToEncode(this String _, String from, String to) => _.ToEncode(Encoding.GetEncoding(from), Encoding.GetEncoding(to));
        /// <summary>
        /// 字符从一个编码转换成另一种编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="from">编码</param>
        /// <param name="to">目标编码</param>
        /// <returns></returns>
        public static string ToEncode(this String _, Encoding from, Encoding to) => to.GetString(Encoding.Convert(from, to, from.GetBytes(_)));
        #endregion

        #region 实现String.Format的扩展
        /// <summary>
        /// 实现String.Format的扩展
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="args">参数集</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        public static String format(this String _, params object[] args)
        {
            var len = _.GetMatches(@"\{\d+\}").Count;
            object[] arg = new object[Math.Max(len, args.Length)];
            args.CopyTo(arg, 0);
            if (len > args.Length) for (int i = args.Length; i < len; i++) arg[i] = "";
            return String.Format(_, arg);
        }
        /// <summary>
        /// 实现String.Format的扩展
        /// </summary>
        /// <param name="_">字符串 自定义变量转换 {key}或${key}</param>
        /// <param name="d">Dictionary集</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        public static String format(this String _, IDictionary<String, String> d)
        {
            var _d = new Dictionary<String, String>(d);
            var RemoveKeys = new List<string>();
            _d.Each(a =>
            {
                if (!_.IsMatch(@"\$?{{{0}}}".format(a.Key))) RemoveKeys.Add(a.Key);
            });
            RemoveKeys.Each(a => _d.Remove(a));
            RemoveKeys.Clear();
            string[] keys = _d.Keys.ToArray();
            string[] values = _d.Values.ToArray();
            for (int i = 0; i < keys.Length; i++)
                _ = _.ReplacePattern(@"\$?{{{0}}}".format(keys[i]), "{" + i.ToString() + "}");
            return String.Format(_, values);
            /*d.Each(KValue => _ = _.ReplacePattern(@"\$?{{{0}}}".format(KValue.Key), KValue.Value));
            return _;*/
        }
        /// <summary>
        /// 实现String.Format的扩展
        /// </summary>
        /// <param name="_">字符串 自定义变量转换 {key}或${key}</param>
        /// <param name="d">Dictionary集</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "<挂起>")]
        public static String format(this String _, IDictionary<string, object> d)
        {
            var _d = new Dictionary<string,object>(d);
            var RemoveKeys = new List<string>();
            _d.Each(a =>
            {
                if (!_.IsMatch(@"\$?{{{0}}}".format(a.Key))) RemoveKeys.Add(a.Key);
            });
            RemoveKeys.Each(a => _d.Remove(a));
            RemoveKeys.Clear();
            string[] keys = _d.Keys.ToArray<string>();
            string[] values = _d.Values.ToArray<string>();
            for (int i = 0; i < keys.Length; i++)
                _ = _.ReplacePattern(@"\$?{{{0}}}".format(keys[i]), "{" + i.ToString() + "}");
            return String.Format(_, values);
        }
        #endregion

        #region 字符串转换成对象

        #region 字符串转换成Int16
        /// <summary>
        /// 字符串转换成Int16
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static short ToInt16(this string _, short defaultValue = default(short))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                if (!_.IsNumberic()) _ = Convert.ChangeType(_, typeof(double)).ToString();
                return short.TryParse(_, out short _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        /// <summary>
        /// 字符串转换成UInt16
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static ushort ToUInt16(this string _, ushort defaultValue = default(ushort))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                if (!_.IsNumberic()) _ = Math.Abs((double)Convert.ChangeType(_, typeof(double))).ToString();
                return ushort.TryParse(_, out ushort _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成Int32
        /// <summary>
        /// 字符串转换成Int32
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ToInt32(this string _, int defaultValue = default(int))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                if (!_.IsNumberic()) _ = Convert.ChangeType(_, typeof(double)).ToString();
                return int.TryParse(_, out int _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        /// <summary>
        /// 字符串转换成UInt32
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static uint ToUInt32(this string _, uint defaultValue = default(uint))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                if (!_.IsNumberic()) _ = Math.Abs((double)Convert.ChangeType(_, typeof(double))).ToString();
                return uint.TryParse(_, out uint _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成Int64
        /// <summary>
        /// 字符串转换成Int64
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long ToInt64(this string _, long defaultValue = default(long))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                if (!_.IsNumberic()) _ = Convert.ChangeType(_, typeof(double)).ToString();
                return long.TryParse(_, out long _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        /// <summary>
        /// 字符串转换成UInt64
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static ulong ToUInt64(this string _, ulong defaultValue = default(ulong))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                if (!_.IsNumberic()) _ = Math.Abs((double)Convert.ChangeType(_, typeof(double))).ToString();
                return ulong.TryParse(_, out ulong _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成Double
        /// <summary>
        /// 字符串转换成Double
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double ToDouble(this string _, double defaultValue = default(double))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                return double.TryParse(_, out double _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成Decimal
        /// <summary>
        /// 字符串转换成Decimal
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string _, decimal defaultValue = default(decimal))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                return decimal.TryParse(_, out decimal _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成float
        /// <summary>
        /// 字符串转换成float
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static float ToFloat(this string _, float defaultValue = default(float))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                return float.TryParse(_, out float _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成long
        /// <summary>
        /// 字符串转换成long
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long ToLong(this string _, long defaultValue = default(long))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsFloat()) return defaultValue;
                return long.TryParse(_, out long _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成Byte
        /// <summary>
        /// 字符串转换成Byte
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static byte ToByte(this string _, byte defaultValue = default(byte))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsNumberic()) return defaultValue;
                return byte.TryParse(_, out byte _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成SByte
        /// <summary>
        /// 字符串转换成SByte
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static sbyte ToSByte(this string _, sbyte defaultValue = default(sbyte))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                if (!_.IsNumberic()) return defaultValue;
                return sbyte.TryParse(_, out sbyte _result) ? _result : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 字符串转换成Guid
        /// <summary>
        /// 字符串转换成Guid
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static Guid ToGUID(this string _, Guid defaultValue = default(Guid)) => _.ToGuid(defaultValue);
        /// <summary>
        /// 字符串转换成Guid
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static Guid ToGuid(this string _, Guid defaultValue = default(Guid))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            _ = _.Trim();
            return _.IsGUID() ? new Guid(_) : defaultValue;
        }
        #endregion

        #region 字符串转换成Boolean
        /// <summary>
        /// 字符串转换成Boolean
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool ToBoolean(this string _, bool defaultValue = false)
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            _ = _.Trim();
            if (_.IsFloat()) return _.RemovePattern(@"[0\.\-]+") != "";
            return _.IsMatch(@"^(true|false)$") ?
                (bool.TryParse(_, out bool f) ? f : defaultValue) :
                _.IsNotNullOrWhiteSpace();
        }
        #endregion

        #region 字符串转换成DateTime
        /// <summary>
        /// 字符串转换成DateTime
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string _, DateTime defaultValue = default(DateTime))
        {
            if (_.IsNullOrWhiteSpace()) return defaultValue;
            try
            {
                _ = _.Trim();
                DateTime dt = defaultValue;
                return DateTime.TryParse(_, out dt) ? dt : defaultValue;
            }
            catch { return defaultValue; }
        }
        #endregion

        #region 类型相互转换
        /// <summary>
        /// 类型相互转换
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">原对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ToCast<T>(this object _, T defaultValue = default(T))
        {
            var val = _.GetValue(typeof(T));
            return val == null ? defaultValue : (T)val;
        }
        /// <summary>
        /// 类型相互转换
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object GetValue(this object o, Type type) => o.GetValue(type, out _);
        /// <summary>
        /// 类型相互转换
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="targetType">类型</param>
        /// <param name="isGeneric">是否是泛类型</param>
        /// <returns></returns>
        public static object GetValue(this object o, Type targetType, out bool isGeneric)
        {
            isGeneric = false;
            ValueTypes BaseTargetType = targetType.GetValueType();
            if (o.IsNullOrEmpty())
            {
                if (BaseTargetType == ValueTypes.Value) return Activator.CreateInstance(targetType);
                else if (BaseTargetType == ValueTypes.String) return string.Empty;
                else return null;
            }
#if false
            if (o is Microsoft.Extensions.Primitives.StringValues _o)
            {
                if (_o.Count == 0) o = "";
                else if (_o.Count == 1) o = _o.ToString();
                else o = _o.ToArray();
            }
#endif
            else if (o is Json.JsonValue JValue)
            {
                return JValue.ToObject(targetType);
            }
            else if(o is Redis.RedisValue RValue)
            {
                return RValue.ToObject(targetType);
            }
            else if (o is Xml.XmlValue XValue)
            {
                return XValue.ToObject(targetType);
            }
            if (targetType == null || targetType == typeof(object)) return o;

            Type sourceType = o.GetType();
            /*类型相同*/
            if (sourceType == targetType) return o;
            /*枚举类型*/
            if (sourceType.IsEnum)
            {
                if (targetType == typeof(string))
                    return o.ToString();
                else return Convert.ChangeType(o, targetType);
            }
            if (targetType.IsEnum) return o.ToEnum(targetType);
            if (targetType == typeof(string)) return o.IsNullOrEmpty() ? String.Empty : o.ToString();
            if (o.IsNullOrEmpty()) return null;
            /*判断是否是基础类型值类型*/
            ValueTypes BaseSourceType = sourceType.GetValueType();
            if ((BaseSourceType == ValueTypes.Struct ||
                BaseSourceType == ValueTypes.Class)
                && (
                BaseTargetType == ValueTypes.Struct ||
                BaseTargetType == ValueTypes.Class))
            {
                if (targetType.IsAbstract) return null;
                var model = Activator.CreateInstance(targetType);
                var _model = model;
                o.CopyTo(_model);
                return _model;
            }
            else if ((BaseSourceType == ValueTypes.Dictionary || BaseSourceType == ValueTypes.IDictionary) && (BaseTargetType == ValueTypes.Struct || BaseTargetType == ValueTypes.Class))
            {
                var _ = o as IDictionary;
                var _model = Activator.CreateInstance(targetType);
                targetType.GetMembers().Each(m =>
                {
                    if (m.MemberType == MemberTypes.Field)
                    {
                        var f = m as FieldInfo;
                        if (_.Contains(f.Name))
                            f.SetValue(_model, _[f.Name].GetValue(f.FieldType));
                    }
                    else if (m.MemberType == MemberTypes.Property)
                    {
                        var p = m as PropertyInfo;
                        if (!p.CanRead || !p.CanWrite || p.IsIndexer()) return;
                        if (_.Contains(p.Name))
                            p.SetValue(_model, _[p.Name].GetValue(p.PropertyType));
                    }
                });
                return _model;
            }
            else if ((BaseSourceType == ValueTypes.Struct || BaseSourceType == ValueTypes.Class) && (BaseTargetType == ValueTypes.Dictionary || BaseTargetType == ValueTypes.IDictionary))
            {
                return o.ObjectToDictionary();
            }
            else if ((BaseTargetType == ValueTypes.Struct || BaseTargetType == ValueTypes.Class) && BaseSourceType == ValueTypes.String)
            {
                var _ = o.ToString();
                if ((_.IsQuery() && !_.IsJson()) || _.IsJson())
                {
                    return _.JsonToObject(targetType);
                }
                else if (_.IsXml())
                    return _.XmlToObject(targetType);
                return null;
            }
            else if (BaseSourceType == ValueTypes.String && (BaseTargetType == ValueTypes.Dictionary || BaseTargetType == ValueTypes.IDictionary))
            {
                var _ = o.ToString();
                if ((_.IsQuery() && !_.IsJson()) || _.IsJson())
                {
                    return _.JsonToObject(targetType);
                }
            }
            if ((BaseSourceType != ValueTypes.Value && BaseSourceType != ValueTypes.String) || (BaseTargetType != ValueTypes.Value && BaseTargetType != ValueTypes.String)) return null;
            if (sourceType.BaseType == typeof(Array)) return null;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
                isGeneric = true;
            }
            /*类型相同*/
            if (sourceType == targetType) return o;
            /*是否继承了类型转换类型*/
            if ((!(o is IConvertible) && !(o is Guid)) || o is ICollection) return null;
            string _val = o.ToString().Trim();
            if (targetType == typeof(DateTime))
            {
                if (sourceType == typeof(int)) return ((int)o).ToDateTime();
                else if (sourceType == typeof(long)) return ((long)o).ToDateTime();
                if (!_val.IsDateOrTime()) return isGeneric ? default(DateTime?) : default(DateTime);
                else o = _val.Replace("T", " ").Replace("点", "时");
            }
            else if (targetType == typeof(Guid))
            {
                if (!_val.IsGUID()) return isGeneric ? default(Guid?) : default(Guid);
                o = new Guid(_val);
            }
            else if (targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(double) || targetType == typeof(decimal) || targetType == typeof(float) || targetType == typeof(short) || targetType == typeof(ushort) || targetType == typeof(uint) || targetType == typeof(ulong) || targetType == typeof(byte) || targetType == typeof(sbyte))
            {
                if (sourceType == typeof(DateTime))
                {
                    if (targetType == typeof(int)) return ((DateTime)o).ToTimeStamp();
                    else if (targetType == typeof(long)) return ((DateTime)o).ToTimeStamps();
                }
                else if (sourceType == typeof(bool))
                {
                    o = ((bool)o) ? 1 : 0;
                    return Convert.ChangeType(o, targetType);
                }
                else if (sourceType == typeof(byte) && targetType == typeof(sbyte))
                {
                    /*if (targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(double) || targetType == typeof(decimal) || targetType == typeof(float) || targetType == typeof(short))
                    {
                        return Convert.ToInt16(_val, 8).GetValue(targetType, out isGeneric);
                    }
                    else if (targetType == typeof(sbyte))*/
                    return (sbyte)o;
                }
                else if (sourceType == typeof(sbyte) && targetType == typeof(byte))
                    return (byte)o;
                if (_val.IsMatch(@"^(true|false)$"))
                {
                    return Convert.ChangeType(_val.Trim().EqualsIgnoreCase("true") ? 1 : 0, targetType);
                }
                //if (!_val.IsFloat()) return isGeneric ? null : Activator.CreateInstance(targetType);
                if (sourceType == typeof(string))
                {
                    if (_val.IsMatch(@"^[A-F0-9]+$") && !_val.IsNumberic())
                    {
                        return Convert.ToInt64(_val, 16).GetValue(targetType, out isGeneric);
                    }
                    else if (_val.IsMatch(@"^(\+|-)?\d+[.]\d+$"))
                    {
                        o = _val.ToDecimal().GetValue(targetType, out isGeneric);
                    }
                    else
                        o = _val;
                    try { o = Convert.ChangeType(o, targetType); } catch { o = 0; }
                }
            }
            else if (targetType == typeof(bool))
            {
                if (_val.IsFloat()) return _val.ToBoolean();
                if (!_val.IsBoolean()) return isGeneric ? default(bool?) : default(bool);
            }
            else if (targetType == typeof(char))
            {
                if (_val.IsNullOrEmpty()) return isGeneric ? default(char?) : default(char);
                if (sourceType == typeof(int)) return Convert.ChangeType(o, targetType);
                if (_val.Length > 1) return _val[0];
            }
            try { return Convert.ChangeType(o, targetType); }
            catch { return Activator.CreateInstance(targetType); }
        }
        #endregion

        #endregion

        #region 将集合数据复制到数组中
        /// <summary>
        /// 将集合数据复制到List中
        /// </summary>
        /// <typeparam name="T">基础值类型</typeparam>
        /// <param name="enumerable">集合数据</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerable enumerable)
        {
            Type type = typeof(T);
            ValueTypes baseType = type.GetValueType();
            List<T> list = null;
            if (baseType == ValueTypes.Value || baseType == ValueTypes.String || baseType == ValueTypes.Enum || baseType == ValueTypes.Class || baseType == ValueTypes.Struct)
            {
                list = new List<T>();
                foreach (var i in enumerable) list.Add(i.ToCast<T>());
            }
            return list;
        }
        /// <summary>
        /// 将集合数据复制到数组中
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">集合数据</param>
        /// <returns></returns>
        public static T[] ToArray<T>(this IEnumerable enumerable)
        {
            List<T> list = enumerable.ToList<T>();
            return list?.ToArray();
        }
        /// <summary>
        /// 数组类型转换
        /// </summary>
        /// <typeparam name="TInput">原数组类型</typeparam>
        /// <typeparam name="TOutput">新数组类型</typeparam>
        /// <param name="array">原数组</param>
        /// <param name="converter">转换方法</param>
        /// <returns></returns>
        public static TOutput[] ToArray<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(array, converter);
        }
        /// <summary>
        /// 原数组
        /// </summary>
        /// <typeparam name="TInput">原数组类型</typeparam>
        /// <typeparam name="TOutput">新数组类型</typeparam>
        /// <param name="array">原数组</param>
        /// <returns></returns>
        public static TOutput[] ToArray<TInput, TOutput>(this TInput[] array)
        {
            return array.ToArray(s => s.ToCast<TOutput>());
        }
        #endregion

        #region 字符串与Base64互转
        /// <summary>
        /// 字节转base64字符串
        /// </summary>
        /// <param name="_">字节</param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] _) => Convert.ToBase64String(_);
        /// <summary>
        /// 字节转base64字符串
        /// </summary>
        /// <param name="_">字节</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] _, int start, int length) => Convert.ToBase64String(_, start, length);
        /// <summary>
        /// 字节转base64字符串
        /// </summary>
        /// <param name="_">字节</param>
        /// <param name="options">是否在其输出中插入换行符</param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] _, Base64FormattingOptions options) => Convert.ToBase64String(_, options);
        /// <summary>
        /// 字节转base64字符串
        /// </summary>
        /// <param name="_">字节</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">长度</param>
        /// <param name="options">是否在其输出中插入换行符</param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] _, int start, int length, Base64FormattingOptions options) => Convert.ToBase64String(_, start, length, options);
        /// <summary>
        /// 字符串转base64
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToBase64String(this String _, string encoding) => Convert.ToBase64String(_.GetBytes(encoding ?? "UTF-8"));
        /// <summary>
        /// 字符串转base64
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToBase64String(this String _, Encoding encoding = null) => Convert.ToBase64String(_.GetBytes(encoding));
        /// <summary>
        /// base64字符串转字符串
        /// </summary>
        /// <param name="_">Base64字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string FromBase64String(this String _, string encoding) => _.FromBase64String(Encoding.GetEncoding(encoding));
        /// <summary>
        /// base64字符串转字符串
        /// </summary>
        /// <param name="_">Base64字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string FromBase64String(this String _, Encoding encoding = null) => _.IsNullOrWhiteSpace() ? "" : _.FromBase64StringToBytes().GetString(encoding ?? Encoding.Default);
        /// <summary>
        /// Base64字符串转Byte[]
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static byte[] FromBase64StringToBytes(this string _)
        {
            if (_.IsNullOrWhiteSpace()) return Array.Empty<byte>();
            _ = _.TrimEnd('=');
            double len = _.Length;
            _ = _.Replace(" ", "+");
            var mod = (int)len % 4;
            if (mod == 1) return Array.Empty<Byte>();
            if (mod > 0)
            {
                _ += new string('=', 4 - mod);
            }
            /*var _len = Math.Ceiling(len / 4) * 4;
            if (_len > len) _ = _.PadRight((int)_len, '=');*/
            return Convert.FromBase64String(_);
        }
        #endregion

        #region 字符串字节互转
        /// <summary>
        /// 获取字节编码
        /// </summary>
        /// <param name="_">字节</param>
        /// <returns>字节编码</returns>
        public static Encoding GetEncoding(this byte[] _) => FileHelper.GetEncoding(_);
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] GetBytes(this String _, Encoding encoding = null) => _.IsNullOrEmpty() ? null : (encoding ?? Encoding.UTF8).GetBytes(_);
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] GetBytes(this String _, string encoding) => Encoding.GetEncoding(encoding ?? "UTF-8").GetBytes(_);
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static sbyte[] GetSBytes(this String _, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(_);
            var bs = new sbyte[bytes.Length];
            bytes.Each((b, index) =>
            {
                bs[index] = (sbyte)b;
            });
            return bs;
        }
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static sbyte[] GetSBytes(this String _, string encoding) => _.GetSBytes(Encoding.GetEncoding(encoding));
        /// <summary>
        /// 字节转字符串
        /// </summary>
        /// <param name="_">字节</param>
        /// <param name="encoding">编码</param>
        /// <param name="index">开始位置</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static string GetString(this byte[] _, Encoding encoding, int index = 0, int count = 0)
        {
            var result = string.Empty;
            if (_ == null || _.Length == 0) return result;
            var _encoding = encoding ?? _.GetEncoding();
            if (_encoding.WebName == Encoding.UTF8.WebName && _.Length >= 3 && _[0] == 0xEF && _[1] == 0xBB && _[2] == 0xBF)
                index = 3;
            result = _encoding.GetString(_, index, count == 0 ? _.Length - index : count + index > _.Length ? _.Length - index : count);
            return result;
        }
        /// <summary>
        /// 字节转字符串
        /// </summary>
        /// <param name="_">字节</param>
        /// <param name="encoding">编码</param>
        /// <param name="index">开始位置</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static string GetString(this byte[] _, string encoding = "", int index = 0, int count = 0)
        {
            var result = string.Empty;
            if (_ == null || _.Length == 0) return result;
            var _encoding = encoding.IsNullOrEmpty() ? _.GetEncoding() : Encoding.GetEncoding(encoding);
            return _.GetString(_encoding, index, count);
        }
        #endregion

        #region 获取HTML文本内容
        /// <summary>
        /// 获取HTML文本内容
        /// </summary>
        /// <param name="_">HTML内容</param>
        /// <returns></returns>
        public static String InnerText(this String _)
        {
            if (_.IsNullOrEmpty()) return "";
            _ = _.RemovePattern(@"<!--[\s\S]*?-->");
            _ = _.RemovePattern(@"<script[^>]*>[\s\S]*?</\s*script>");
            _ = _.RemovePattern(@"<(script|style|textarea)[^>]*>[\s\S]*?</\s*(script|style|textarea)>");
            _ = _.RemovePattern(@"<\s*(!doctype|table|thead|tbody|tr|td|th|div|blockquote|fieldset|legend|font|i|u|h[1-9]|s|b|m|p|strong|meta|iframe|frame|span|layer|link|html|head|body|title|a|ul|ol|li|dl|dt|dd|img|form|select|input|button|canvas|header|nav|footer|select|option|textarea|em|noscript|section|svg|use|label)(\s*[^>]*)?>|<\/\s*(table|thead|tbody|tr|td|th|div|blockquote|fieldset|legend|font|i|u|h[1-9]|s|b|m|p|strong|meta|iframe|frame|span|layer|link|html|head|body|title|a|ul|ol|li|dl|dt|dd|img|form|select|input|button|canvas|header|nav|footer|select|option|textarea|em|noscript|section|svg|use|label)\s*>");
            _ = _.RemovePattern(@"[\r\n\t]");
            _ = _.ReplacePattern(@"&gt;", ">").ReplacePattern(@"&lt;", "<").ReplacePattern(@"&amp;", "&").ReplacePattern(@"&quot;", "\"")
                .ReplacePattern("&nbsp;", " ").ReplacePattern(@"\s+", " ")
                .ReplacePattern(@"&copy;", "©").ReplacePattern(@"&reg;", "®");
            return _;
        }
        #endregion

        #region 截取字符串 一个汉字为两个字符
        /// <summary>
        /// 截取字符串 一个汉字为两个字符
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="length">长度</param>
        /// <param name="endString">结束串 如...</param>
        /// <returns></returns>
        public static string SubString(this string _, int length = 0, string endString = "...")
        {
            if (_.IsNullOrEmpty()) return "";
            _ = _.RemovePattern(@"[\s\r\t\n]+", RegexOptions.IgnoreCase);
            if (length <= 0) return _;
            if (_.GetBytes().Length <= length) return _;
            int num = 0;
            string str = "";
            for (int i = 0; i < _.Length; i++)
            {
                num += _[i].ToString().GetBytes().Length;
                str += _[i].ToString();
                if (num >= length) break;
            }
            return str + endString;
        }
        #endregion

        #region 16进制转字符串
        /// <summary>
        /// 16进制转字符串
        /// </summary>
        /// <param name="_">16进制字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string HexToString(this string _, Encoding encoding = null)
        {
            if (_.IsNullOrEmpty()) return "";
            _ = _.RemovePattern(@"0x");
            _ = _.RemovePattern(@"\s+");
            int len = _.Length;
            if (len % 2 != 0) return "";
            byte[] bytes = new byte[len / 2];
            for (int i = 0; i < len / 2; i++)
                bytes[i] = byte.Parse(_.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return bytes.GetString(encoding ?? Encoding.UTF8);
        }
        #endregion

        #region 字符串转16进制
        /// <summary>
        /// 字符串转16进制
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string StringToHex(this string _, Encoding encoding = null)
        {
            if (_.IsNullOrEmpty()) return "";
            byte[] bytes = _.GetBytes(encoding ?? Encoding.UTF8);
            string str = string.Empty;
            bytes.Each(a =>
            {
                str += "{0:X}".format(a).PadLeft(2, '0');
                //str += Convert.ToString(a, 16);
            });
            return str;
        }
        #endregion

        #region 字节数组转16进制字符串
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <param name="isSpace">是否有空格隔开 默认为有空格</param>
        /// <returns></returns>
        public static string ByteToHexString(this byte[] bytes, bool isSpace = true)
        {
            if (bytes?.Length == 0) return string.Empty;
            string _ = "";
            string space = isSpace ? " " : "";
            for (int i = 0; i < bytes.Length; i++)
                _ += bytes[i].ToString("X2") + space;
            return _.TrimEnd(' ');
        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">长度</param>
        /// <param name="isSpace">是否有空格隔开 默认为有空格</param>
        /// <returns></returns>
        public static string ByteToHexString(this byte[] bytes, int start, int length, bool isSpace = true)
        {
            if (bytes != null)
            {
                if (length == 0) length = bytes.Length;
                if (start + length > bytes.Length) length = bytes.Length - start;
                string _ = "";
                string space = isSpace ? " " : "";
                for (int i = start; i < length; i++)
                    _ += bytes[i].ToString("X2") + space;
                return _.TrimEnd(' ');
            }
            return string.Empty;
        }
        #endregion

        #region 16进制字符串转字节数组
        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="hexString">16进制字符串</param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(this string hexString)
        {
            try
            {
                hexString = hexString.RemovePattern(@"[\r\n\s]+");
                if (hexString.IsNullOrEmpty()) return null;
                if (hexString.Length % 2 != 0)
                    hexString += "0";
                byte[] _ = new byte[hexString.Length / 2];
                for (int i = 0; i < _.Length; i++)
                    _[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                return _;
            }
            catch
            {
                return hexString.GetBytes();
            }
        }
        #endregion

        #region 复制对象
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">对象</param>
        /// <returns></returns>
        public static T Copy<T>(this T _) where T : new()
        {
            if (_ == null) return default(T);
            var t = _.GetType();
            var o = Activator.CreateInstance<T>();
            PropertyInfo[] pd = t.GetProperties();
            PropertyInfo[] po = t.GetProperties();
            for (int i = 0; i < pd.Length; i++)
            {
                if (po[i].HasFieldIgnore()) continue;
                if (po[i].CanWrite && po[i].CanRead && !po[i].IsIndexer())
                {
                    po[i].SetValue(o, pd[i].GetValue(_, null), null);
                }
            }
            return o;
        }
        /// <summary>
        /// 复制数据到对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        public static void CopyTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            if (source == null) { target = default(TTarget); return; }
            var sourceType = source.GetType();
            var targetType = target.GetType();
            sourceType.GetMembers().Each(m =>
            {
                bool IsGeneric = false;
                object val = null;
                if (m.MemberType == MemberTypes.Property)
                {
                    var _p = m as PropertyInfo;
                    if (_p.IsIndexer()) return;
                    val = _p.GetValue(source, null);
                }
                else if (m.MemberType == MemberTypes.Field)
                {
                    val = ((FieldInfo)m).GetValue(source);
                }
                else return;
                MemberInfo[] mis = targetType.GetMember(m.Name);
                if (mis == null || mis.Length == 0) return;
                var mi = mis[0];
                if (m.HasFieldIgnore() || mi.HasFieldIgnore()) return;
                if (mi.MemberType == MemberTypes.Property)
                {
                    var pi = mi as PropertyInfo;
                    if ((pi.GetMethod != null && pi.GetMethod.IsStatic) || (pi.SetMethod != null && pi.SetMethod.IsStatic) || pi.IsIndexer()) return;
                    Type _Field = pi.PropertyType;
                    val = val.GetValue(_Field, out IsGeneric);
                    if (val != null || (val == null && IsGeneric))
                        if (!pi.IsIndexer() && pi.CanWrite) pi.SetValue(target, val, null);
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    var fi = mi as FieldInfo;
                    Type _Field = fi.FieldType;
                    val = val.GetValue(_Field, out IsGeneric);
                    if (val != null || (val == null && IsGeneric))
                        fi.SetValue(target, val);
                }

                /*var tm = targetType.GetMember(m.Name);
                if (tm == null || tm.Length == 0) return;
                var ttm = tm[0];
                if (ttm.MemberType != MemberTypes.Property && ttm.MemberType != MemberTypes.Field) return;
                if (m.HasFieldIgnore() || ttm.HasFieldIgnore()) return;
                if (m.MemberType == MemberTypes.Property)
                {
                    var p = m as PropertyInfo;
                    if ((p.GetMethod != null && p.GetMethod.IsStatic) || (p.SetMethod != null && p.SetMethod.IsStatic) || p.IsIndexer()) return;
                    var val = p.GetValue(source, null);
                    if (ttm.MemberType == MemberTypes.Property)
                    {
                        var _p = ttm as PropertyInfo;
                        if (!_p.IsIndexer() && _p.CanWrite) _p.SetValue(target, val);
                    }
                    else
                    {
                        var _f = ttm as FieldInfo;
                        _f.SetValue(target, val);
                    }
                }
                else if (m.MemberType == MemberTypes.Field)
                {
                    var f = m as FieldInfo;
                    if (f.IsStatic) return;
                    var val = f.GetValue(source);
                    if (ttm.MemberType == MemberTypes.Property)
                    {
                        var _p = ttm as PropertyInfo;
                        if (!_p.IsIndexer() && _p.CanWrite) _p.SetValue(target, val);
                    }
                    else
                    {
                        var _f = ttm as FieldInfo;
                        _f.SetValue(target, val);
                    }
                }*/
            });
        }
        #endregion

        #region 当前类型是否是某个类型
        /// <summary>
        /// 当前类型是否是某个类型
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">基础类型</param>
        /// <returns></returns>
        public static bool Is(this Type type, Type baseType)
        {
            if (type == null || baseType == null) return false;
            return type == baseType || type.Equals(baseType);
        }
        /// <summary>
        /// 当前类型是否是某个类型
        /// </summary>
        /// <typeparam name="T">基础类型</typeparam>
        /// <param name="type">当前类型</param>
        /// <returns></returns>
        public static bool Is<T>(this Type type)
        {
            return type.Is(typeof(T));
        }
        /// <summary>
        /// 当前类型是否可以作为某个类型
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">基础类型</param>
        /// <returns></returns>
        public static bool As(this Type type, Type baseType)
        {
            if (type == null || baseType == null) return false;
            if (baseType.IsGenericTypeDefinition && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) type = type.GetGenericArguments()[0];
            return baseType == type || baseType.IsAssignableFrom(type);
        }
        /// <summary>
        /// 当前类型是否可以作为某个类型
        /// </summary>
        /// <typeparam name="T">基础类型</typeparam>
        /// <param name="type">当前类型</param>
        /// <returns></returns>
        public static bool As<T>(this Type type)
        {
            return type.As(typeof(T));
        }
        #endregion

        #region 获取当前类型的所有基类
        /// <summary>
        /// 获取当前类型的所有基类
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns></returns>
        public static List<Type> GetBaseTypes(this Type t)
        {
            var list = new List<Type>();
            var _t = t.BaseType;
            while (_t != null)
            {
                list.Add(_t);
                _t = _t.BaseType;
            }
            return list;
        }
        /// <summary>
        /// 获取当前类型的所有基类的名称
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns></returns>
        public static List<string> GetBaseTypeNames(this Type t)
        {
            return t.GetBaseTypes().Select(a => a.Name).ToList();
        }
        /// <summary>
        /// 获取当前类型的基础类型
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns></returns>
        public static Type GetBaseType(this Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = t.GetGenericArguments()[0];
            }
            return t;
        }
        #endregion

        #region IP编码转换
        /// <summary>
        /// IP转换编码
        /// </summary>
        /// <param name="_">IP串</param>
        /// <returns></returns>
        public static ulong IpToCode(this string _)
        {
            if (_.IsNullOrWhiteSpace()) return 0;
            string[] ip = _.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (ip.Length != 4) return 0;
            return ip[0].ToCast<ulong>() * 256 * 256 * 256 + ip[1].ToCast<ulong>() * 256 * 256 + ip[2].ToCast<ulong>() * 256 + ip[3].ToCast<ulong>();
        }
        /// <summary>
        /// 编码转IP串
        /// </summary>
        /// <param name="_">编码</param>
        /// <returns></returns>
        public static string CodeToIp(this ulong _)
        {
            if (_ == 0) return string.Empty;
            ulong ip1 = _ / (256 * 256 * 256);
            ulong ip2 = _ % (256 * 256 * 256) / (256 * 256);
            ulong ip3 = _ % (256 * 256) / 256;
            ulong ip4 = _ % 256;
            return "{0}.{1}.{2}.{3}".format(ip1, ip2, ip3, ip4);
        }
        #endregion

        #region 从字符串中检索子字符串，在指定头部字符串之后，指定尾部字符串之前
        /// <summary>从字符串中检索子字符串，在指定头部字符串之后，指定尾部字符串之前</summary>
        /// <remarks>常用于截取xml某一个元素等操作</remarks>
        /// <param name="str">目标字符串</param>
        /// <param name="after">头部字符串，在它之后</param>
        /// <param name="before">尾部字符串，在它之前</param>
        /// <param name="startIndex">搜索的开始位置</param>
        /// <returns></returns>
        public static string Substring(this string str, string after, string before = null, int startIndex = 0)
        {
            if (str.IsNullOrEmpty()) return str;
            if (after.IsNullOrEmpty() && before.IsNullOrEmpty()) return str;
            var p = -1;
            if (after.IsNotNullOrEmpty())
            {
                p = str.IndexOf(after, startIndex);
                if (p < 0) return null;
                p += after.Length;
            }
            if (before.IsNullOrEmpty()) return str.Substring(p);
            var f = str.IndexOf(before, p >= 0 ? p : startIndex);
            if (f < 0) return null;
            if (p >= 0 && f - p > 0)
                return str.Substring(p, f - p);
            else
                return str.Substring(0, f);
        }
        #endregion

        #region 不区分大小写的比较
        /// <summary>
        /// 不区分大小写的比较
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="val">原值</param>
        /// <param name="other">其它值</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase<T>(this T val, params T[] other)
        {
            if (val.IsNullOrEmpty())
            {
                foreach (var o in other)
                    if (o.IsNullOrEmpty()) return true;
                return false;
            }
            foreach (var o in other)
            {
                if (typeof(T) == typeof(String))
                {
                    if (val.ToString().Equals(o?.ToString(), StringComparison.OrdinalIgnoreCase)) return true;
                }
                else
                {
                    if (val.Equals(o)) return true;
                }
            }
            return false;
        }
        #endregion

        #region 字符串排序
        /// <summary>
        /// 字符串正排
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>字符串</returns>
        public static string OrderBy(this string _)
        {
            char[] str = _.ToArray();
            Array.Sort(str);
            return new string(str);
        }
        /// <summary>
        /// 字符串倒排
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>字符串</returns>
        public static string OrderByDescending(this string _)
        {
            char[] str = _.ToArray();
            Array.Sort(str);
            Array.Reverse(str);
            return new string(str);
        }
        #endregion

        #region 多元符表达式
        /// <summary>
        /// 多元符表达式
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">数据</param>
        /// <param name="vs">其它数据</param>
        /// <returns></returns>
        public static T Multivariate<T>(this T _, params T[] vs)
        {
            foreach (var o in new T[] { _ }.Concat(vs))
                if (o.IsNotNullOrEmpty()) return o;
            return default(T);
        }
        /// <summary>
        /// 多元符表达式
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">数组</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Multivariate<T>(this IEnumerable<T> _, T defaultValue = default(T))
        {
            var er = _.GetEnumerator();
            while (er.MoveNext())
            {
                var o = er.Current;
                if (o.IsNotNullOrEmpty()) return o;
            }
            return defaultValue;
        }
        /// <summary>
        /// 多元符表达式
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">数组</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Multivariate<T>(this T[] _, T defaultValue = default(T))
        {
            for (int i = 0; i < _.Length; i++)
                if (_[i].IsNotNullOrEmpty()) return _[i];
            return defaultValue;
        }
        #endregion

        #region Url编码
        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static string UrlEncode(this string _) => System.Net.WebUtility.UrlEncode(_);
        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlEncode(this string _, Encoding encoding) => System.Web.HttpUtility.UrlEncode(_, encoding);
        #endregion

        #region Url解码
        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static string UrlDecode(this string _) => System.Net.WebUtility.UrlDecode(_);
        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlDecode(this string _, Encoding encoding) => System.Web.HttpUtility.UrlDecode(_, encoding);
        #endregion

        #region Html编码 
        /// <summary>
        /// Html编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static string HtmlEncode(this string _) => System.Net.WebUtility.HtmlEncode(_);
        #endregion

        #region Html解码
        /// <summary>
        /// Html解码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static string HtmlDecode(this string _) => System.Net.WebUtility.HtmlDecode(_);
        #endregion

        #region Javascript编码
        /// <summary>
        /// Javascript编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static string JavaScriptStringEncode(this string _) => System.Web.HttpUtility.JavaScriptStringEncode(_);
        /// <summary>
        /// Javascript编码
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="addDoubleQuotes">是否包含双引号</param>
        /// <returns></returns>
        public static string JavaScriptStringEncode(this string _, bool addDoubleQuotes) => System.Web.HttpUtility.JavaScriptStringEncode(_, addDoubleQuotes);
        #endregion

        #region 是否是值类型
        /// <summary>
        /// 是否是值类型
        /// </summary>
        /// <param name="_">类型</param>
        /// <returns></returns>
        public static Boolean IsValueType(this Type _) => _.IsValueType || _.Name.IsMatch(@"^(Int16|UInt16|Int32|UInt32|Int64|UInt64|Double|Decimal|String|Single|Guid|DateTime|Boolean|Byte|Char|SByte)$");
        #endregion

        #region 设置空对象默认值
        /// <summary>
        /// 返回不为空的对象值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>不为空的对象值</returns>
        public static T IfEmpty<T>(this T _, T defaultValue) => new T[] { _, defaultValue }.Multivariate();
        /// <summary>
        /// 返回不为空的对象值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">对象</param>
        /// <param name="func">委托</param>
        /// <returns>不为空的对象值</returns>
        public static T IfEmpty<T>(this T _, Func<T> func) => _.IfEmpty(func.Invoke());
        /// <summary>
        /// 设置对象值,如果为空
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">对象</param>
        /// <param name="value">值</param>
        /// <returns>对象</returns>
        public static T IfEmptyValue<T>(this T _, T value) => _ = new T[] { _, value }.Multivariate();
        /// <summary>
        /// 设置对象值,如果为空
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">对象</param>
        /// <param name="func">委托</param>
        /// <returns>对象</returns>
        public static T IfEmptyValue<T>(this T _, Func<T> func) => _.IfEmptyValue(func.Invoke());
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_">对象</param>
        /// <returns></returns>
        public static T Empty<T>(this T _)
        {
            if (_.IsNullOrEmpty()) return _;
            var t = typeof(T).GetValueType();
            switch (t)
            {
                case ValueTypes.ArrayList:
                    (_ as ArrayList).Clear(); break;
                case ValueTypes.Anonymous:
                case ValueTypes.Array:
                case ValueTypes.Value:
                case ValueTypes.IEnumerable:
                case ValueTypes.Enum:
                    return _ = default(T);
                case ValueTypes.Class:
                case ValueTypes.Struct:
                    return _ = Activator.CreateInstance<T>();
                case ValueTypes.DataTable:
                    (_ as DataTable).Clear(); break;
                case ValueTypes.Dictionary:
                case ValueTypes.IDictionary:
                    (_ as IDictionary).Clear();
                    break;
                case ValueTypes.List:
                    (_ as IList).Clear(); break;
                case ValueTypes.String:
                    (_ as String).Remove(0); break;
            }
            return _;
        }
        #endregion

        #region 赋值数组
        /// <summary>
        /// 赋值数组
        /// </summary>
        /// <param name="data">目标</param>
        /// <param name="destStart">目标起始位置</param>
        /// <param name="source">源</param>
        /// <param name="sourceStart">源开始</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] Write(this byte[] data, int destStart, byte[] source, int sourceStart = 0, int length = -1)
        {
            if (length == -1) length = source.Length - sourceStart;
            if (data.IsNullOrEmpty()) data = new byte[destStart + length];
            if (source.IsNullOrEmpty()) return data;
            if (source.Length <= sourceStart + length) length = source.Length - sourceStart;
            if (data.Length <= destStart + length) length = data.Length - destStart;
            Array.Copy(source, sourceStart, data, destStart, length);
            return data;
        }
        /// <summary>
        /// 赋值数组
        /// </summary>
        /// <param name="data">目标</param>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static byte[] Write(this byte[] data, byte[] source) => data.Write(0, source);
        #endregion

        #region 加密函数[对应javascript里面的escape]
        /// <summary>
        /// 加密函数[对应javascript里面的escape]
        /// </summary>
        /// <param name="s">要加密的字符串</param>
        /// <returns>返回加过密的字符串</returns>
        public static string Escape(this string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ba = Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));
                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion

        #region 字节转相应单位
        /// <summary>
        /// 字节转相应单位
        /// </summary>
        /// <param name="size">字节数</param>
        /// <param name="digits">小数点位数</param>
        /// <returns></returns>
        public static string ByteToKMGTP(this long size, int digits = 2) => size.ToCast<double>().ByteToKMGTP(digits);
        /// <summary>
        /// 字节转相应单位
        /// </summary>
        /// <param name="size">字节数</param>
        /// <param name="digits">小数点位数</param>
        /// <returns></returns>
        public static string ByteToKMGTP(this double size, int digits = 2)
        {
            var Units = new string[] { "B", "K", "M", "G", "T", "P" };
            var mod = 1024;
            var i = 0;
            while (size >= mod)
            {
                size %= mod;
                i++;
            }
            return Math.Round(size, digits) + Units[i];
        }
        #endregion

        #region 相应单位转换字节
        /// <summary>
        /// 相应单位转换字节
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static double KMGTPToByte(this string str)
        {
            if (str.IsNullOrEmpty()) return 0;
            var Units = new string[] { "B", "K", "M", "G", "T", "P" };
            var matchs = str.GetMatchs($@"^(?<a>\d+)\s*(?<b>({Units.Join("|")}))\s*$");
            if (!matchs.ContainsKey("a") || !matchs.ContainsKey("b")) return 0;
            var unit = matchs["b"];
            var val = matchs["a"].ToDouble();
            var index = Array.IndexOf(Units, unit);
            return val * Math.Pow(1024, index);
        }
        #endregion

        #region 扩展数组属性
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">值</param>
        /// <param name="startIndex">查找索引</param>
        /// <param name="count">查找元素个数</param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] array, T value, int startIndex = 0, int count = 0)
        {
            var length = array.Length;
            var _count = count == 0 ? (length - startIndex) : (count + startIndex) > length ? (length - startIndex) : count;
            return Array.IndexOf(array, value, startIndex, _count);
        }
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">值</param>
        /// <param name="startIndex">查找索引</param>
        /// <param name="count">查找元素个数</param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this T[] array,T value,int startIndex=0,int count = 0)
        {
            var length = array.Length;
            var _count = count == 0 ? (length - startIndex) : (count + startIndex) > length ? (length - startIndex) : count;
            return Array.LastIndexOf(array, value, startIndex, _count);
        }
        /// <summary>
        /// 查找数组中数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static T Find<T>(this T[] array, Predicate<T> match) => Array.Find(array, match);
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static int FindIndex<T>(this T[] array, Predicate<T> match) => Array.FindIndex(array, match);
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">开始查找位置</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static int FindIndex<T>(this T[] array, int startIndex, Predicate<T> match) => Array.FindIndex(array, startIndex, match);
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">开始查找位置</param>
        /// <param name="count">查找元素个数</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static int FindIndex<T>(this T[] array, int startIndex,int count, Predicate<T> match) => Array.FindIndex(array, startIndex,count, match);
        /// <summary>
        /// 查找数组中数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static T FindLast<T>(this T[] array, Predicate<T> match) => Array.FindLast(array, match);
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static int FindLastIndex<T>(this T[] array, Predicate<T> match) => Array.FindLastIndex(array, match);
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">开始查找位置</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static int FindLastIndex<T>(this T[] array, int startIndex, Predicate<T> match) => Array.FindLastIndex(array, startIndex, match);
        /// <summary>
        /// 查找数组中的位置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">开始查找位置</param>
        /// <param name="count">查找元素个数</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static int FindLastIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match) => Array.FindLastIndex(array, startIndex, count, match);
        /// <summary>
        /// 查找元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="match">要搜索的元素的条件</param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Predicate<T> match) => Array.FindAll(array, match);
        #endregion
    }
}