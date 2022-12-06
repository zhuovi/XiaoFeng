using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-25 11:59:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Json
{
    /// <summary>
    /// 定义类型
    /// </summary>
    public class XValue : IComparable, IFormattable, IConvertible, IComparable<XValue>, IEquatable<XValue>
    {
        #region 构造器
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">类型</param>
        internal XValue(object value, JsonType type)
        {
            Value = value; ValueType = type;
        }
        /// <summary>
        /// 设置Null
        /// </summary>
        public XValue() : this(null, JsonType.Null) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(int value) : this(value, JsonType.Number) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(uint value) : this(value, JsonType.Number) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(short value) : this(value, JsonType.Number) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(ushort value) : this(value, JsonType.Number) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(long value) : this(value, JsonType.Number) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(ulong value) : this(value, JsonType.Number) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(double value) : this(value, JsonType.Float) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(decimal value) : this(value, JsonType.Float) { }
        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="value">值</param>
        public XValue(float value) : this(value, JsonType.Float) { }
        /// <summary>
        /// 设置字符
        /// </summary>
        /// <param name="value">值</param>
        public XValue(string value) : this(value, JsonType.Float) { }
        /// <summary>
        /// 设置字符
        /// </summary>
        /// <param name="value">值</param>
        public XValue(char value) : this(value, JsonType.Char) { }
        /// <summary>
        /// 设置字节
        /// </summary>
        /// <param name="value">值</param>
        public XValue(byte value) : this(value, JsonType.Byte) { }
        /// <summary>
        /// 设置字节
        /// </summary>
        /// <param name="value">值</param>
        public XValue(sbyte value) : this(value, JsonType.Byte) { }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="value">值</param>
        public XValue(DateTime value) : this(value, JsonType.DateTime) { }
        /// <summary>
        /// 设置Guid
        /// </summary>
        /// <param name="value">值</param>
        public XValue(Guid value) : this(value, JsonType.Guid) { }
        /// <summary>
        /// 设置Bool
        /// </summary>
        /// <param name="value">值</param>
        public XValue(bool value) : this(value, JsonType.Bool) { }
        /// <summary>
        /// 设置Type
        /// </summary>
        /// <param name="value">值</param>
        public XValue(Type value) : this(value, JsonType.Type) { }
        /// <summary>
        /// 设置数组
        /// </summary>
        /// <param name="value">值</param>
        public XValue(Array value) : this(value, JsonType.Array) { }
        /// <summary>
        /// 设置数组
        /// </summary>
        /// <param name="value">值</param>
        public XValue(IEnumerable<object> value) : this(value, JsonType.Array) { }
        /// <summary>
        /// 设置数组
        /// </summary>
        /// <param name="value">值</param>
        public XValue(ArrayList value) : this(value, JsonType.Array) { }
        /// <summary>
        /// 设置对象
        /// </summary>
        /// <param name="value">值</param>
        public XValue(XValue value) : this(value.Value, value.ValueType) { }
        /// <summary>
        /// 设置对象
        /// </summary>
        /// <param name="value">值</param>
        public XValue(object value) : this(value, JsonType.Object) { }
        #endregion

        #region 属性
        /// <summary>
        /// 类型
        /// </summary>
        private JsonType ValueType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        private object Value { get; set; }
        #endregion

        #region 比较
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (ValueType == JsonType.Null || Value == null) return -1;
            if (obj is XValue) return this.CompareTo((XValue)obj);
            if (obj is int || obj is uint || obj is long || obj is ulong ||
                obj is short || obj is ushort || obj is float || obj is double ||
                obj is decimal || obj is char || obj is byte || obj is sbyte ||
                obj is bool)
                if (ValueType == JsonType.Number || ValueType == JsonType.Float ||
                    ValueType == JsonType.Byte)
                {
                    var val = this.Value.GetValue(obj.GetType());
                    return val == obj ? 0 : 1;
                }
                else
                    return -1;
            if ((obj is Guid && ValueType == JsonType.Guid) ||
                (obj is DateTime && ValueType == JsonType.DateTime) ||
                (obj is Type && ValueType == JsonType.Type) ||
                ((obj is Array || obj is ArrayList || obj is IEnumerable) && ValueType == JsonType.Array))
                if (ValueType == JsonType.Guid)
                    return obj == Value ? 0 : -1;
                else
                    return -1;
            if (obj is object && ValueType == JsonType.Object)
                return obj == Value ? 0 : -1;
            return -1;
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">其它值</param>
        /// <returns></returns>
        public int CompareTo(XValue other)
        {
            return (this.Value == other.Value && this.ValueType == other.ValueType) ? 0 : -1;
        }
        #endregion

        #region 转换

        #region int
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator int(XValue v)
        {
            return v.ToInt32();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(int v)
        {
            return new XValue(v, JsonType.Number);
        }
        #endregion

        #region uint
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator uint(XValue v)
        {
            return v.ToUInt32();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(uint v)
        {
            return new XValue(v, JsonType.Number);
        }
        #endregion

        #region short
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator short(XValue v)
        {
            return v.ToInt16();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(short v)
        {
            return new XValue(v, JsonType.Number);
        }
        #endregion

        #region ushort
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator ushort(XValue v)
        {
            return v.ToUInt16();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(ushort v)
        {
            return new XValue(v, JsonType.Number);
        }
        #endregion

        #region long
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator long(XValue v)
        {
            return v.ToInt64();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(long v)
        {
            return new XValue(v, JsonType.Number);
        }
        #endregion

        #region ulong
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator ulong(XValue v)
        {
            return v.ToUInt64();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(ulong v)
        {
            return new XValue(v, JsonType.Number);
        }
        #endregion

        #region byte
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator byte(XValue v)
        {
            return v.ToByte();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(byte v)
        {
            return new XValue(v, JsonType.Byte);
        }
        #endregion

        #region sbyte
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator sbyte(XValue v)
        {
            return v.ToSByte();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(sbyte v)
        {
            return new XValue(v, JsonType.Byte);
        }
        #endregion

        #region double
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator double(XValue v)
        {
            return v.ToDouble();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(double v)
        {
            return new XValue(v, JsonType.Float);
        }
        #endregion

        #region float
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator float(XValue v)
        {
            return v.ToFloat();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(float v)
        {
            return new XValue(v, JsonType.Float);
        }
        #endregion

        #region decimal
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator decimal(XValue v)
        {
            return v.ToDecimal();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(decimal v)
        {
            return new XValue(v, JsonType.Float);
        }
        #endregion

        #region bool
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator bool(XValue v)
        {
            return v.ToBoolean();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(bool v)
        {
            return new XValue(v, JsonType.Bool);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator DateTime(XValue v)
        {
            return v.ToDateTime();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(DateTime v)
        {
            return new XValue(v, JsonType.DateTime);
        }
        #endregion

        #region Guid
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Guid(XValue v)
        {
            return v.ToGuid();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(Guid v)
        {
            return new XValue(v, JsonType.Guid);
        }
        #endregion

        #region string
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator string(XValue v)
        {
            return v.ToString();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XValue(string v)
        {
            return new XValue(v, JsonType.String);
        }
        #endregion

        #endregion

        #region 相等
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="other">其它值</param>
        /// <returns></returns>
        public bool Equals(XValue other)
        {
            return this.Value == other.Value && this.ValueType == other.ValueType;
        }
        /// <summary>
        /// 获取 HashCode
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
            if (obj is XValue value)
                return this.Equals(value);
            return false;
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="x1">第一个对象</param>
        /// <param name="x2">第二个对象</param>
        /// <returns></returns>
        public static bool operator ==(XValue x1, XValue x2)
        {
            if (x1.IsNullOrEmpty() || x2.IsNullOrEmpty())
            {
                return false;
            }
            return x1.Equals(x2);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="x1">第一个对象</param>
        /// <param name="x2">第二个对象</param>
        /// <returns></returns>
        public static bool operator !=(XValue x1, XValue x2)
        {
            return !(x1 == x2);
        }
        #endregion

        #region 获取类型码
        /// <summary>
        /// 获取类型码
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return ((IConvertible)ValueType).GetTypeCode();
        }
        #endregion

        #region 转类型
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public bool ToBoolean(IFormatProvider provider = null)
        {
            return this.ValueType == JsonType.Bool ? (bool)Value : false;
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public byte ToByte(IFormatProvider provider = null)
        {
            return this.ValueType == JsonType.Byte ? (byte)Value : default(byte);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public char ToChar(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Char ? (char)Value : default(char);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public DateTime ToDateTime(IFormatProvider provider = null)
        {
            return ValueType == JsonType.DateTime ? (DateTime)Value : default(DateTime);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public Guid ToGuid(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Guid ? (Guid)Value : default(Guid);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public decimal ToDecimal(IFormatProvider provider = null)
        {
            return (ValueType == JsonType.Float || ValueType == JsonType.Number) ? (decimal)Value : default(decimal);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public double ToDouble(IFormatProvider provider = null)
        {
            return (ValueType == JsonType.Float || ValueType == JsonType.Number) ? (double)Value : default(double);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public float ToFloat(IFormatProvider provider = null)
        {
            return (ValueType == JsonType.Float || ValueType == JsonType.Number) ? (float)Value : default(float);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public short ToInt16(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Number ? (short)Value : ValueType == JsonType.Float ? (short)Math.Round((double)Value, 0) : default(short);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public int ToInt32(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Number ? (int)Value : ValueType == JsonType.Float ? (int)Math.Round((double)Value, 0) : default(int);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public long ToInt64(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Number ? (long)Value : ValueType == JsonType.Float ? (long)Math.Round((double)Value, 0) : default(long);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public sbyte ToSByte(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Byte ? (sbyte)Value : ValueType == JsonType.Float ? (sbyte)Math.Round((double)Value, 0) : default(sbyte);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public float ToSingle(IFormatProvider provider = null)
        {
            return (ValueType == JsonType.Float || ValueType== JsonType.Number) ? (float)Value :  default(float);
        }
        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (ValueType == JsonType.String)
                return Value.ToString();
            else if (ValueType == JsonType.Char || ValueType == JsonType.Byte ||
                ValueType == JsonType.Float || ValueType == JsonType.Number ||
                ValueType == JsonType.Object)
                return Value.ToString();
            else if (ValueType == JsonType.Guid)
                return Value.ToString();
            else if (ValueType == JsonType.DateTime)
                return Value.ToString();
            else if (ValueType == JsonType.Type)
                return Value.GetType().AssemblyQualifiedName;
            return string.Empty;
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider = null)
        {
            return ToString(null, provider);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <param name="conversionType">类型</param>
        /// <returns></returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ValueType == JsonType.Type ? (Type)Value : default(Type);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public ushort ToUInt16(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Number ? (ushort)Value : ValueType == JsonType.Float ? (ushort)Math.Round((double)Value, 0) : default(ushort);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public uint ToUInt32(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Number ? (uint)Value : ValueType == JsonType.Float ? (uint)Math.Round((double)Value, 0) : default(uint);
        }
        /// <summary>
        /// 转类型
        /// </summary>
        /// <param name="provider">格式</param>
        /// <returns></returns>
        public ulong ToUInt64(IFormatProvider provider = null)
        {
            return ValueType == JsonType.Number ? (ulong)Value : ValueType == JsonType.Float ? (ulong)Math.Round((double)Value, 0) : default(ulong);
        }
        #endregion
    }
}