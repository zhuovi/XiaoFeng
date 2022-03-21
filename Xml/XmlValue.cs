using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/20 11:13:00                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// Xml值对象
    /// </summary>
    public class XmlValue : IComparable, IFormattable, IConvertible, IComparable<XmlValue>, IEquatable<XmlValue>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XmlValue()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public XmlType ElementType { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public List<XmlValue> Attributes { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<XmlValue> ChildNodes { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 深度
        /// </summary>
        public int Depth { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        [XmlIgnore]
        [Json.JsonIgnore]
        public XmlValue ParentElement { get; set; }
        /// <summary>
        /// 是否有属性
        /// </summary>
        public Boolean HasAttributes
        {
            get
            {
                return this.Attributes != null && this.Attributes.Count > 0;
            }
        }
        /// <summary>
        /// 是否有子节点
        /// </summary>
        public Boolean HasChildNodes
        {
            get
            {
                return this != null && this.ChildNodes != null && this.ChildNodes.Count > 0;
            }
        }
        /// <summary>
        /// 是否是空
        /// </summary>
        public Boolean IsEmpty
        {
            get
            {
                return this != null && this.Value.IsNullOrEmpty() && !this.HasAttributes && !this.HasChildNodes;
            }
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public XmlValue this[string name]
        {
            get
            {
                var value = this.GetElement(name);
                if (value == null)
                    return this.GetAttribute(name);
                return null;
            }
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public XmlValue this[int index]
        {
            get
            {
                if (index < 0 || (!this.HasChildNodes && !this.HasAttributes)) return null;
                if (this.HasChildNodes && index < this.ChildNodes.Count)
                    return this.ChildNodes[index];
                if (this.HasAttributes && index < this.Attributes.Count)
                    return this.Attributes[index];
                return null;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public object GetValue(Type type) => this.ToObject(type);
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public XmlValue GetAttribute(string name)
        {
            if (!this.HasAttributes || this.Attributes.Count == 0) return null;
            return this.Attributes.Find(a => a.Name.EqualsIgnoreCase(name));
        }
        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public XmlValue GetElement(string name)
        {
            if (!this.HasChildNodes || this.ChildNodes.Count == 0) return null;
            return this.ChildNodes.Find(a => a.Name.EqualsIgnoreCase(name) && a.ElementType == XmlType.Element);
        }
        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public XmlValue GetElements(string name)
        {
            if (!this.HasChildNodes || this.ChildNodes.Count == 0) return null;
            return new XmlValue()
            {
                Name = name,
                ElementType = XmlType.Array,
                ParentElement = this, 
                Depth = this.Depth + 1,
                ChildNodes = this.ChildNodes.Where(a => a.Name.EqualsIgnoreCase(name) && a.ElementType == XmlType.Element).ToList()
            };
        }
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="value">子节点</param>
        public void Append(XmlValue value)
        {
            if (this.ChildNodes == null) this.ChildNodes = new List<XmlValue>();
            this.ChildNodes.Add(value);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public object ToObject(Type type, object target = null)
        {
            if (this == null) return null;
            if ((type == this.GetType() || type == null) && target == null) return this;
            if (type == null && target != null)
                type = target.GetType();
            var ValueType = type.GetValueType();
            if (ValueType == ValueTypes.String || ValueType == ValueTypes.Value)
            {
                return this.Value.GetValue(type);
            }
            else if (ValueType == ValueTypes.Class || ValueType == ValueTypes.Struct)
            {
                return ParseObject(this, type, target);
            }
            else if (ValueType == ValueTypes.Array)
            {
                return ParseArray(this, type, target);
            }
            else if (ValueType == ValueTypes.ArrayList || ValueType == ValueTypes.IEnumerable || ValueType == ValueTypes.List)
            {
                return ParseList(this, type, target);
            }
            else if (ValueType == ValueTypes.Enum)
            {
                return this.Value.ToEnum(type);
            }
            else
                return this.Value.GetValue(type);
        }
        /// <summary>
        /// 转成对象
        /// </summary>
        /// <param name="xmlValue">Xml对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public object ParseObject(XmlValue xmlValue, Type type, object target)
        {
            if (xmlValue == null) return null;
            if (type == typeof(object) || type == typeof(XmlValue)) return xmlValue;
            if (target == null)
                target = Activator.CreateInstance(type);
            type.GetPropertiesAndFields(m =>
            {
                var PropertyType = m.MemberType == MemberTypes.Property ? (m as PropertyInfo).PropertyType : (m as FieldInfo).FieldType;
                var ValueType = PropertyType.GetValueType();
                var Name = m.Name;
                var ArrayName = "";
                var ItemName = "";
                XmlValue xValue = xmlValue?.GetElement(Name);
                if (m.IsDefined(typeof(XmlElementAttribute), false))
                {
                    Name = m.GetCustomAttribute<XmlElementAttribute>().ElementName;
                    xValue = xmlValue.GetElement(Name.IfEmpty(m.Name));
                }
                else if (m.IsDefined(typeof(XmlArrayAttribute), false) || m.IsDefined(typeof(XmlArrayItemAttribute), false))
                {
                    XmlValue _xmlValue = null;
                    if (m.IsDefined(typeof(XmlArrayAttribute), false))
                    {
                        ArrayName = m.GetCustomAttribute<XmlArrayAttribute>().ElementName;
                    }
                    else
                    {
                        //ArrayName = Name;
                    }
                    if (ArrayName.IsNotNullOrEmpty())
                        _xmlValue = xmlValue?.GetElement(ArrayName);
                    if (m.IsDefined(typeof(XmlArrayItemAttribute), false))
                    {
                        ItemName = m.GetCustomAttribute<XmlArrayItemAttribute>().ElementName;
                        xValue = (_xmlValue ?? xmlValue)?.GetElements(ItemName);
                    }
                }
                else if (m.IsDefined(typeof(XmlAttributeAttribute), false))
                {
                    Name = m.GetCustomAttribute<XmlAttributeAttribute>().AttributeName;
                    xValue = xmlValue.GetAttribute(Name.IfEmpty(m.Name));
                }
                else if (m.IsDefined(typeof(XmlTextAttribute), false))
                {
                    Name = "";
                    xValue = xmlValue;
                }
                if (m.IsDefined(typeof(XmlElementPathAttribute), false))
                {
                    var Path = m.GetCustomAttribute<XmlElementPathAttribute>();
                    if (Path.IsNotNullOrEmpty() && xmlValue != null)
                    {
                        xValue = xmlValue;
                        for (var i = 0; i < Path.Paths.Length; i++)
                        {
                            if (xValue.HasChildNodes)
                            {
                                xValue = xValue.GetElement(Path.Paths[i]);
                                if (xValue == null) break;
                            }
                        }
                    }
                }
                if (xValue != null)
                {
                    if (m is PropertyInfo p)
                        p.SetValue(target, xValue?.ToObject(p.PropertyType));
                    else if (m is FieldInfo f)
                        f.SetValue(target, xValue?.ToObject(f.FieldType));
                }
            });
            return target;
        }
        /// <summary>
        /// 转数组
        /// </summary>
        /// <param name="xmlValue">数据</param>
        /// <param name="type">类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public Array ParseArray(XmlValue xmlValue, Type type, object target)
        {
            if (xmlValue == null) return null;
            var list = new List<XmlValue>();
            if (xmlValue.HasChildNodes)
                list = xmlValue.ChildNodes;
            else
            {
                list = xmlValue.ParentElement.ChildNodes.Where(a => a.Name.EqualsIgnoreCase(xmlValue.Name)).ToList();
            }
            var length = list.Count;
            var elmType = type?.GetElementXType();
            if (elmType == null) elmType = typeof(object);
            var arr = Array.CreateInstance(elmType, length);
            list.For(0, length, i =>
             {
                 var item = list[i];
                 object val = null;
                 if (item != null)
                     val = item.ToObject(elmType, arr.GetValue(i));
                 arr.SetValue(val.GetValue(elmType), i);
                 i++;
             });
            return arr;
        }
        /// <summary>
        /// 转列表
        /// </summary>
        /// <param name="xmlValue">数据</param>
        /// <param name="type">类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public IList ParseList(XmlValue xmlValue, Type type, object target)
        {
            if (xmlValue == null) return null;
            var lists = new List<XmlValue>();
            if (xmlValue.HasChildNodes)
                lists = xmlValue.ChildNodes;
            else
            {
                lists = xmlValue.ParentElement.ChildNodes.Where(a => a.Name.EqualsIgnoreCase(xmlValue.Name)).ToList();
            }
            var elmType = type.GetGenericArguments().FirstOrDefault();
            if (elmType == null) elmType = typeof(object);
            // 处理一下type是IList<>的情况
            if (type.IsInterface) type = typeof(List<>).MakeGenericType(elmType);
            // 创建列表
            var list = (target ?? Activator.CreateInstance(type)) as IList;
            foreach (var item in lists)
            {
                object val = null;
                if (item != null) val = item.ToObject(elmType, null);
                list.Add(val.GetValue(elmType));
            }
            return list;
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">其它对象</param>
        /// <returns></returns>
        public int CompareTo(XmlValue other)
        {
            return this.ElementType == other.ElementType && this.Value == other.Value ? 0 : -1;
        }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other">其它对象</param>
        /// <returns></returns>
        public bool Equals(XmlValue other)
        {
            return this.ElementType.Equals(other.ElementType) && this.Value.Equals(other.Value);
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (this.ElementType == XmlType.None || this.Value == null) return -1;
            if (obj is XmlValue other) return this.CompareTo(other);
            return obj.Equals(this.Value) ? 0 : -1;
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator String(XmlValue v) => v.ToString();
        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="formatProvider">驱动</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString();
        }
        /// <summary>
        /// 类型编码
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return ((IConvertible)this.ElementType).GetTypeCode();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Boolean(XmlValue v) => v.ToBoolean();
        /// <summary>
        /// 转boolean
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public bool ToBoolean(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Boolean>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Char(XmlValue v) => v.ToChar();
        /// <summary>
        /// 转字符
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public char ToChar(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Char>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator SByte(XmlValue v) => v.ToSByte();
        /// <summary>
        /// 转无符号字节
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public sbyte ToSByte(IFormatProvider provider = null)
        {
            return this.Value.ToCast<sbyte>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Byte(XmlValue v) => v.ToByte();
        /// <summary>
        /// 转有符号字节
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public byte ToByte(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Byte>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Int16(XmlValue v) => v.ToInt16();
        /// <summary>
        /// 转Int16
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public short ToInt16(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Int16>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator UInt16(XmlValue v) => v.ToUInt16();
        /// <summary>
        /// 转无符号Int16
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public ushort ToUInt16(IFormatProvider provider = null)
        {
            return this.Value.ToCast<UInt16>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Int32(XmlValue v) => v.ToInt32();
        /// <summary>
        /// 转int
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public int ToInt32(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Int32>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator UInt32(XmlValue v) => v.ToUInt32();
        /// <summary>
        /// 转UInt32
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public uint ToUInt32(IFormatProvider provider = null)
        {
            return this.Value.ToCast<UInt32>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Int64(XmlValue v) => v.ToInt64();
        /// <summary>
        /// 转有符号长整型
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public long ToInt64(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Int64>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator UInt64(XmlValue v) => v.ToUInt64();
        /// <summary>
        /// 转无符号长整型
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public ulong ToUInt64(IFormatProvider provider = null)
        {
            return this.Value.ToCast<UInt64>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Single(XmlValue v) => v.ToSingle();
        /// <summary>
        /// 转单精度浮点型
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public float ToSingle(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Single>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Double(XmlValue v) => v.ToDouble();
        /// <summary>
        /// 转双精度浮点型
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public double ToDouble(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Double>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Decimal(XmlValue v) => v.ToDecimal();
        /// <summary>
        /// 转十进制浮点型
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public decimal ToDecimal(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Decimal>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator DateTime(XmlValue v) => v.ToDateTime();
        /// <summary>
        /// 转时间
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public DateTime ToDateTime(IFormatProvider provider = null)
        {
            return this.Value.ToCast<DateTime>();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Guid(XmlValue v) => v.ToGuid();
        /// <summary>
        /// 转时间
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public Guid ToGuid(IFormatProvider provider = null)
        {
            return this.Value.ToCast<Guid>();
        }
        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider = null)
        {
            return this.Value.ToCast<String>();
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="conversionType">转换类型</param>
        /// <param name="provider">驱动</param>
        /// <returns></returns>
        public object ToType(Type conversionType, IFormatProvider provider = null)
        {
            return this.Value.GetValue(conversionType);
        }
        /// <summary>
        /// 两类型相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator ==(XmlValue a, XmlValue b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            return a.ElementType.Equals(b.ElementType) && a.Value.Equals(b.Value);
        }
        /// <summary>
        /// 两类型不相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator !=(XmlValue a, XmlValue b)
        {
            return !(a == b);
        }
        /// <summary>
        /// HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }
        #endregion
    }
}