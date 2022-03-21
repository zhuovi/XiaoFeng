using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace XiaoFeng.Json
{
    /// <summary>
    /// Json读取器
    /// </summary>
    public class JsonReader : Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonReader() { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region 反序列化Json成对象
        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public T Read<T>(string json)
        {
            return (T)this.Read(json, typeof(T));
        }
        /// <summary>
        /// 反序列化Json成对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public object Read(string json, Type type)
        {
            var jsonValue = JsonParser.ParseValue(new JsonData(json));
            if (jsonValue.IsNullOrEmpty()) return null;
            return jsonValue.ToObject(type);
            //return this.ToObject(jsonValue, type, null);
        }
        #endregion

        #region Json字典或列表转为具体类型对象
        /// <summary>Json字典或列表转为具体类型对象</summary>
        /// <param name="jsonValue">Json对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        [Obsolete("已过期")]
        public object ToObjectBak(JsonValue jsonValue, Type type, object target)
        {
            return null;/*
            if (type == typeof(JsonValue)) return jsonValue;
            if (type == null && target != null) type = target.GetType();
            ValueTypes valueTypes = type.GetValueType();
            if (jsonValue.Type == JsonType.Array)
            {
                if (valueTypes == ValueTypes.Array)
                    return this.ParseArray(jsonValue, type, target);
                else if (valueTypes == ValueTypes.List || valueTypes == ValueTypes.ArrayList)
                    return this.ParseList(jsonValue, type, target);
                else
                    return null;
            }
            else if (jsonValue.Type == JsonType.Bool)
                return jsonValue.AsBool();
            else if (jsonValue.Type == JsonType.DateTime)
                return jsonValue.AsDateTime();
            else if (jsonValue.Type == JsonType.Float)
                return jsonValue.AsFloat();
            else if (jsonValue.Type == JsonType.Guid)
                return jsonValue.AsGuid();
            else if (jsonValue.Type == JsonType.Null)
                return null;
            else if (jsonValue.Type == JsonType.Number)
                return jsonValue.AsLong();
            else if (jsonValue.Type == JsonType.Float)
                return jsonValue.AsFloat();
            else if (jsonValue.Type == JsonType.String)
                return jsonValue.AsString();
            else if (jsonValue.Type == JsonType.Object)
                return ParseObject(jsonValue, type, target);
            else if (jsonValue.Type == JsonType.Type)
                return jsonValue.value;
            else
                return jsonValue.value;*/
        }
        #endregion

        #region Json字典或列表转为具体类型对象
        /// <summary>Json字典或列表转为具体类型对象</summary>
        /// <param name="jsonValue">Json对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public object ToObject(JsonValue jsonValue, Type type, object target)
        {
            if (type == typeof(JsonValue)) return jsonValue;
            if (type == null && target != null) type = target.GetType();
            ValueTypes valueTypes = type.GetValueType();
            if (jsonValue.Type == JsonType.Array)
            {
                if (valueTypes == ValueTypes.Array)
                    return this.ParseArray(jsonValue, type, target);
                else if (valueTypes == ValueTypes.List || valueTypes == ValueTypes.ArrayList)
                    return this.ParseList(jsonValue, type, target);
                else
                    return null;
            }
            else if (jsonValue.Type == JsonType.Bool)
                return jsonValue.ToBoolean();
            else if (jsonValue.Type == JsonType.DateTime)
                return jsonValue.ToDateTime();
            else if (jsonValue.Type == JsonType.Float)
                return jsonValue.ToFloat();
            else if (jsonValue.Type == JsonType.Guid)
                return jsonValue.ToGuid();
            else if (jsonValue.Type == JsonType.Null)
                return null;
            else if (jsonValue.Type == JsonType.Number)
                return jsonValue.ToLong();
            else if (jsonValue.Type == JsonType.String)
                return jsonValue.ToString();
            else if (jsonValue.Type == JsonType.Object)
                return ParseObject(jsonValue, type, target);
            else if (jsonValue.Type == JsonType.Type)
                return jsonValue.value;
            else
                return jsonValue.value;
        }
        #endregion

        #region 转换成数组
        /// <summary>
        /// 转换成数组
        /// </summary>
        /// <param name="jsonValue">json对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        private Array ParseArray(JsonValue jsonValue, Type type, object target)
        {
            if (jsonValue == null) return null;
            var list = jsonValue.AsArray();
            var elmType = type?.GetElementXType();
            if (elmType == null) elmType = typeof(object);
            if (!(target is Array arr)) arr = Array.CreateInstance(elmType, list.Count);
            list.For(0, list.Count, i =>
            {
                var item = list[i];
                object val = null;
                if (item != null)
                {
                    val = this.ToObject(item, elmType, arr.GetValue(i));
                }
                arr.SetValue(val.GetValue(elmType), i);
                i++;
            });
            return arr;
        }
        #endregion

        #region 转换成List
        /// <summary>
        /// 转换成List
        /// </summary>
        /// <param name="jsonValue">json对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        private IList ParseList(JsonValue jsonValue, Type type, object target)
        {
            if (jsonValue == null) return null;
            var vlist = jsonValue.AsArray();
            var elmType = type.GetGenericArguments().FirstOrDefault();
            if (elmType == null) elmType = typeof(object);
            // 处理一下type是IList<>的情况
            if (type.IsInterface) type = typeof(List<>).MakeGenericType(elmType);
            // 创建列表
            var list = (target ?? Activator.CreateInstance(type)) as IList;
            foreach (var item in vlist)
            {
                object val = null;
                if (item != null) val = this.ToObject(item, elmType, null);
                list.Add(val.GetValue(elmType));
            }
            return list;
        }
        #endregion

        #region 转换成对象
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="jsonValue">Json对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns> 
        private object ParseObject(JsonValue jsonValue, Type type, object target)
        {
            ValueTypes types = type.GetValueType();
            if (type == typeof(object)) return jsonValue.value;
            Dictionary<string, JsonValue> list = jsonValue.AsObject();
            try
            {
                if (target == null) target = Activator.CreateInstance(type);
            }
            catch 
            {
                return null;
            }
            if (types == ValueTypes.Dictionary || types == ValueTypes.IDictionary)
            {
                return this.ParseDictionary(jsonValue, type, target as IDictionary);
            }
            else if (types == ValueTypes.Class || types == ValueTypes.Struct)
            {
                if (list == null || list.Count == 0 || list.FirstOrDefault().Value.Type == JsonType.Null || list.FirstOrDefault().Value.value == null) return null;
                type.GetMembers().Each(m =>
                {
                    if (m is FieldInfo || m is PropertyInfo)
                    {
                        object val = null;
                        var name = m.Name;
                        if (m.IsDefined(typeof(JsonElement), false))
                        {
                            var element = m.GetCustomAttribute<JsonElement>();
                            if (element.Name.IsNotNullOrEmpty()) name = element.Name;
                        }
                        var _f = list.ContainsKey(name);
                        if (m is FieldInfo f)
                        {
                            val = _f ? this.ToObject(list[name], f.FieldType, null) : null;
                            if (val != null && _f && list[name].Type != JsonType.Type)
                                val = val.GetValue(f.FieldType);
                            f.SetValue(target, val);
                        }
                        else if (m is PropertyInfo p)
                        {
                            if (!p.CanRead || !p.CanWrite || p.IsIndexer()) return;
                            val = _f ? this.ToObject(list[name], p.PropertyType, null) : null;
                            if (val != null && _f && list[name].Type != JsonType.Type)
                                val = val.GetValue(p.PropertyType);
                            p.SetValue(target, val, null);
                        }
                    }
                });
            }
            return target;
        }
        #endregion

        #region 转换成字典
        /// <summary>
        /// 转换成字典
        /// </summary>
        /// <param name="jsonValue">Json对象</param>
        /// <param name="type">模板类型</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        private IDictionary ParseDictionary(JsonValue jsonValue,Type type, IDictionary target)
        {
            ValueTypes types = type.GetValueType();
            Dictionary<string, JsonValue> list = jsonValue.AsObject();
            if (types == ValueTypes.Dictionary)
            {
                var _types = type.GetGenericArguments();
                if (target == null)
                {
                    // 处理一下type是Dictionary<,>的情况
                    if (type.IsInterface) type = typeof(Dictionary<,>).MakeGenericType(_types[0], _types[1]);
                    target = Activator.CreateInstance(type) as IDictionary;
                }
                list.Each(a =>
                {
                    var key = this.ToObject(new JsonValue(a.Key), _types[0], null);
                    var val = this.ToObject(a.Value, _types[1], null);
                    target.Add(key.GetValue(_types[0]), val.GetValue(_types[1]));
                });
                return target;
            }
            return null;
        }
        #endregion

        #endregion
    }
}