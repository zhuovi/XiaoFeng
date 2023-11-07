using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XiaoFeng;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-15 15:47:46                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis 值
    /// </summary>
    public class RedisValue : IComparable, IFormattable, IEquatable<RedisValue>, IComparable<RedisValue>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RedisValue() : this(RedisType.Null, null) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="redisType">类型</param>
        /// <param name="value">值</param>
        public RedisValue(RedisType redisType, object value)
        {
            this.RedisType = redisType;
            this.Value = value;
        }
        /// <summary>
        /// 整型
        /// </summary>
        /// <param name="val">值</param>
        public RedisValue(int val) : this(RedisType.Int, val) { }
        /// <summary>
        /// 长整型
        /// </summary>
        /// <param name="val">值</param>
        public RedisValue(long val) : this(RedisType.Int, val) { }
        /// <summary>
        /// 长整型
        /// </summary>
        /// <param name="val">值</param>
        public RedisValue(float val) : this(RedisType.Int, val) { }
        /// <summary>
        /// 长整型
        /// </summary>
        /// <param name="val">值</param>
        public RedisValue(double val) : this(RedisType.Int, val) { }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="val">值</param>
        public RedisValue(string val) : this(RedisType.String, val) { }
        /// <summary>
        /// 布尔值
        /// </summary>
        /// <param name="val">值</param>
        public RedisValue(Boolean val) : this(RedisType.Boolean, val) { }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="list">数据</param>
        public RedisValue(List<RedisValue> list) : this(RedisType.Array, list) { }
        /// <summary>
        /// 数组
        /// </summary>
        /// <param name="vals">数据</param>
        public RedisValue(RedisValue[] vals) : this(RedisType.Array, vals.ToList<RedisValue>()) { }
        /// <summary>
        /// 字典
        /// </summary>
        /// <param name="dic">数据</param>
        public RedisValue(Dictionary<string, RedisValue> dic)
        {
            this.RedisType = RedisType.Dictionary;
            var vals = new Dictionary<RedisValue, RedisValue>();
            dic.Each(a => vals.Add(new RedisValue(a.Key), a.Value));
        }
        /// <summary>
        /// 字典
        /// </summary>
        /// <param name="dic">数据</param>
        public RedisValue(Dictionary<RedisValue, RedisValue> dic) : this(RedisType.Dictionary, dic) { }
        /// <summary>
        /// 结构类型
        /// </summary>
        /// <param name="type">类型</param>
        public RedisValue(RedisKeyType type) : this(RedisType.KeyType, type) { }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        public RedisValue(RedisValue value)
        {
            this.RedisType = value.RedisType;
            this.Value = value.Value;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        public RedisValue(object value)
        {
            this.Value = value;
            if (value == null)
            {
                this.RedisType = RedisType.Null;
                return;
            }
            if (value is String)
                this.RedisType = RedisType.String;
            else if (value is Boolean)
                this.RedisType = RedisType.Boolean;
            else if (value is IList)
                this.RedisType = RedisType.Array;
            else if (value is IDictionary)
                this.RedisType = RedisType.Dictionary;
            else if (value is RedisKeyType)
                this.RedisType = RedisType.KeyType;
            else if (value.ToString().IsNumberic())
                this.RedisType = RedisType.Int;
            else this.RedisType = RedisType.Model;
        }
        /// <summary>
        /// 获取数组中数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>RedisValue</returns>
        public RedisValue this[int index]
        {
            get
            {
                if (index < 0 || this.Value == null) return new RedisValue();

                if (this.RedisType == RedisType.Array)
                {
                    var val = this.Value as List<RedisValue>;
                    return val[index];
                }
                return new RedisValue();
            }
        }
        /// <summary>
        /// 获取字典中数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>RedisValue</returns>
        public RedisValue this[string key]
        {
            get
            {
                if (key.IsNullOrEmpty() || this.Value == null) return new RedisValue();
                if (this.RedisType == RedisType.Dictionary)
                {
                    var val = this.Value as Dictionary<RedisValue, RedisValue>;
                    if (val.TryGetValue(new RedisValue(key), out var _val))
                        return _val;
                }
                return new RedisValue();
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; private set; }
        /// <summary>
        /// 类型
        /// </summary>
        public RedisType RedisType { get; set; }
        /// <summary>
        /// 值长度
        /// </summary>
        public int Length
        {
            get
            {
                if (this.RedisType == RedisType.String)
                    return this.ToString().Length;
                if (this.RedisType == RedisType.Array)
                    return this.ToList().Count;
                if (this.RedisType == RedisType.Dictionary)
                    return this.ToDictionary().Count;
                return -1;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator int(RedisValue v) => v.ToInt();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(int v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator float(RedisValue v) => v.Value.ToCast<float>();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(double v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator double(RedisValue v) => v.Value.ToCast<double>();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(float v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator long(RedisValue v) => v.Value.ToCast<long>();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(long v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator String(RedisValue v) => v.ToString();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(String v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Boolean(RedisValue v) => v.RedisType == RedisType.Boolean && (Boolean)v;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(Boolean v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Dictionary<string, RedisValue>(RedisValue v)
        => v.ToDictionary<string, RedisValue>();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(Dictionary<string, RedisValue> v)
        => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Dictionary<RedisValue, RedisValue>(RedisValue v)
        => v.RedisType == RedisType.Dictionary ? v.Value as Dictionary<RedisValue, RedisValue> : null;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(Dictionary<RedisValue, RedisValue> v)
        => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator List<RedisValue>(RedisValue v)
        => (v.RedisType == RedisType.Array) ? v.ToList() : null;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(List<RedisValue> v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator RedisValue[](RedisValue v) => v.RedisType == RedisType.Array ? v.ToArray() : null;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(RedisValue[] v) => new RedisValue(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator RedisKeyType(RedisValue v) => v.RedisType == RedisType.KeyType ? v.Value.ToCast<RedisKeyType>() : RedisKeyType.Error;
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RedisValue(RedisKeyType v) => new RedisValue(v);
        /// <summary>
        /// 转换为List
        /// </summary>
        /// <returns></returns>
        public List<RedisValue> ToList() => this.RedisType == RedisType.Array ? this.Value as List<RedisValue> : null;
        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public RedisValue[] ToArray() => this.ToList()?.ToArray();
        /// <summary>
        /// 转换为字典
        /// </summary>
        /// <typeparam name="T1">KEY类型</typeparam>
        /// <typeparam name="T2">Value类型</typeparam>
        /// <returns></returns>
        public Dictionary<T1, T2> ToDictionary<T1, T2>()
        {
            var result = new Dictionary<T1, T2>();
            if (this.RedisType == RedisType.Dictionary)
            {
                (this.Value as Dictionary<RedisValue, RedisValue>).Each(a =>
                {
                    if (a.Key.RedisType == RedisType.Null || a.Value.RedisType == RedisType.Null) return;
                    result.Add((T1)a.Key.ToObject(typeof(T1)), (T2)a.Value.ToObject(typeof(T2)));
                });
                return result;
            }
            if (this.RedisType == RedisType.Array)
            {
                var list = this.ToList();
                if (list == null || !list.Any() || list.Count % 2 != 0) return null;
                for (var i = 0; i < list.Count; i += 2)
                    result.Add((T1)list[i].ToObject(typeof(T1)), (T2)list[i + 1].ToObject(typeof(T2)));
                return result;
            }
            return null;
        }
        /// <summary>
        /// 转换为字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, RedisValue> ToDictionary()
        {
            return this.ToDictionary<string, RedisValue>();
        }
        /// <summary>
        /// 转换为字典
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public Dictionary<string, T> ToDictionary<T>()
        {
            return this.ToDictionary<string, T>();
        }
        /// <summary>
        /// 转换为布尔值
        /// </summary>
        /// <returns></returns>
        public Boolean ToBoolean() => this.RedisType == RedisType.Boolean ? (bool)this.Value : false;
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.Value?.ToString();
        /// <summary>
        /// 转换成数字
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            if (this.RedisType == RedisType.Int)
                return this.Value.ToCast<int>();
            if (this.RedisType == RedisType.String)
                return this.Value.ToCast(-1);
            if (this.RedisType == RedisType.Boolean)
                return (bool)this.Value == true ? 1 : 0;
            return -1;
        }
        /// <summary>
        /// 转换成数字
        /// </summary>
        /// <returns></returns>
        public long ToLong() => this.Value.ToCast<long>();
        /// <summary>
        /// 转换成Double
        /// </summary>
        /// <returns></returns>
        public double ToDouble() => this.Value.ToCast<Double>();
        /// <summary>
        /// 转换成模型
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <returns></returns>
        public T ToModel<T>()
        {
            if (this.RedisType == RedisType.Model) return (T)this.Value;
            if (this.RedisType == RedisType.String) return this.ToString().JsonToObject<T>();
            return default(T);
        }
        /// <summary>
        /// 转换成模型列表
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <returns></returns>
        public List<T> ToList<T>()
        {
            if (this.RedisType != RedisType.Array) return null;
            return (from a in this.ToList() select a.Value.ToCast<T>()).ToList();
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() => (T)this.GetValue(typeof(T));
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public object GetValue(Type type)
        {
            return this.ToObject(type);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public object ToObject(Type type, object target = null)
        {
            if ((type == typeof(RedisValue) || type == null) && target == null) return this;
            if (type == null && target != null) type = target.GetType();
            if (this.Value == null) return null;
            if (type == this.Value.GetType()) return this.Value;
            ValueTypes valueTypes = type.GetValueType();
            if (this.RedisType == RedisType.Array)
            {
                if (valueTypes == ValueTypes.Array)
                    return (from a in this.ToList() select a.Value.GetValue(type)).ToArray();
                else if (valueTypes == ValueTypes.List || valueTypes == ValueTypes.ArrayList)
                    return (from a in this.ToList() select a.Value.GetValue(type)).ToList();
                else if (valueTypes == ValueTypes.Dictionary || valueTypes == ValueTypes.IDictionary)
                    return this.ToDictionary<RedisValue, RedisValue>();
                else
                    return null;
            }
            else if (this.RedisType == RedisType.Boolean || this.RedisType == RedisType.Int || this.RedisType == RedisType.String)
                return this.Value.GetValue(type);
            else if (this.RedisType == RedisType.Null)
                return null;
            else
                return this.Value.GetValue(type);
        }
        /// <summary>
        /// 转换成模型数组
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <returns></returns>
        public T[] ToArray<T>() => this.ToList<T>()?.ToArray<T>();
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="formatProvider">驱动</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider) => this.ToString();
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <returns></returns>
        public bool Equals(RedisValue other) => this == other;
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <returns></returns>
        public int CompareTo(RedisValue other) => (this.Value == other.Value && this.RedisType == other.RedisType) ? 0 : -1;
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (RedisType == RedisType.Null || Value == null) return -1;
            if (obj is RedisValue v) return this.CompareTo(v);
            return -1;
        }
        /// <summary>
        /// 两类型相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator ==(RedisValue a, RedisValue b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.RedisType.Equals(b.RedisType) && a.Value.Equals(b.Value);
        }
        /// <summary>
        /// 两类型不相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator !=(RedisValue a, RedisValue b) => !(a == b);
        /// <summary>
        /// 获取Hash码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => base.GetHashCode();
        /// <summary>
        /// 重写相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj) => this.CompareTo(obj) == 0;
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="val">数据</param>
        public void Add(RedisValue val)
        {
            if (this.RedisType == RedisType.Array)
            {
                if (this.Value == null) this.Value = new List<RedisValue>();
                if (this.Value is List<RedisValue> list) list.Add(val);
                else
                {
                    this.RedisType = val.RedisType;
                    this.Value = val.Value;
                }
            }
        }
        #endregion
    }
}