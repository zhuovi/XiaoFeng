using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    /// 写Json
    /// </summary>
    public class JsonWriter : Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonWriter() { this.SerializerSetting = JsonParser.DefaultSettings ?? new JsonSerializerSetting(); }
        /// <summary>
        /// 设置日期格式Json格式设置        
        /// </summary>
        /// <param name="formatterSetting">Json格式设置</param>
        public JsonWriter(JsonSerializerSetting formatterSetting)
        {
            this.SerializerSetting = formatterSetting;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 解析对象时长
        /// </summary>
        public long ObjectTimes { get; set; } = 0;
        /// <summary>
        /// 数据
        /// </summary>
        public StringBuilder Builder = new StringBuilder();
        /// <summary>
        /// 深度
        /// </summary>
        private int _Depth = 0;
        /// <summary>
        /// 字典集
        /// </summary>
        private Dictionary<object, int> _DepthDict = new Dictionary<object, int>();
        /// <summary>
        /// Json格式
        /// </summary>
        public JsonSerializerSetting SerializerSetting { get; set; }
        #endregion

        #region 方法

        #region 写数据
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
        public void WriteValue(object value)
        {
            /*if (value.IsNullOrEmpty())
            {
                Builder.Append("null");
                return;
            }
            var t = value.GetType();
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Empty:
                    Builder.Append("null");break;
                case TypeCode.String:
                case TypeCode.Char:
                    WriteString(value + "");break;
                case TypeCode.Boolean:
                    Builder.Append((value + "").ToLower());break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    Builder.Append(value.ToString());break;
                case TypeCode.DateTime:
                    WriteDateTime((DateTime)value);break;
                default:
                    if (typeof(IEnumerable).IsAssignableFrom(t))
                        WriteArray((IEnumerable)value);
                    else
                    {
                        var ots = System.Diagnostics.Stopwatch.StartNew();
                        ots.Start();
                        WriteObject(value);
                        ots.Stop();
                        this.ObjectTimes += ots.ElapsedMilliseconds;
                    }
                    break;
            }
            return;*/
            if (value is Xml.XmlValue xmlValue)
            {
                value = xmlValue.Value;
            }
            if (value is string || value is char)
            {
                WriteString(value + "");
                return;
            }
            if (value == null)
            {
                Builder.Append("null");
                return;
            }
            if (value is bool)
            {
                Builder.Append((value + "").ToLower());
                return;
            }
            if (
                value is int || value is double || value is decimal || 
                value is Single || value is float || value is byte || 
                value is short || value is sbyte || value is ushort ||
                value is uint
            )
            {
                Builder.Append(((IConvertible)value).ToString(NumberFormatInfo.InvariantInfo));
                return;
            }
            if(value is long l)
            {
                if (this.SerializerSetting.LongSerializeString && (l < -9007199254740992 || l > 9007199254740992))
                    Builder.Append("\"" + value + "\"");
                else
                    Builder.Append(((IConvertible)value).ToString(NumberFormatInfo.InvariantInfo));
                return;
            }
            if(value is ulong ul)
            {
                if (this.SerializerSetting.LongSerializeString && ul > 9007199254740992)
                    Builder.Append("\"" + value + "\"");
                else
                    Builder.Append(((IConvertible)value).ToString(NumberFormatInfo.InvariantInfo));
                return;
            }
            if (value is DateTime dateTime)
            {
                WriteDateTime(dateTime);
                return;
            }
            if (value is Guid guid)
            {
                WriteGuid(guid);
                return;
            }
            if (value is IDictionary<string, object> dic)
            {
                WriteStringDictionary(dic);
                return;
            }
            if (value is System.Dynamic.ExpandoObject)
            {
                WriteStringDictionary((IDictionary<string, object>)value);
                return;
            }
            if (value is IDictionary dict)
            {
                WriteDictionary(dict);
                return;
            }
            if (value is byte[] buf)
            {
                WriteStringFast(Convert.ToBase64String(buf, 0, buf.Length, Base64FormattingOptions.None));
                return;
            }
            if (value is StringDictionary sdict)
            {
                WriteSD(sdict);
                return;
            }
            if (value is NameValueCollection nvc)
            {
                WriteNV(nvc);
                return;
            }
            if (value is IEnumerable ea)
            {
                WriteArray(ea);
                return;
            }
            if (value is Enum)
            {
                if (SerializerSetting.EnumValueType == EnumValueType.Name)
                    value = value.ToString();
                else if (SerializerSetting.EnumValueType == EnumValueType.Description)
                    value = value.GetType().GetField(value.ToString()).GetDescription(false);
                else value = value.GetValue(Enum.GetUnderlyingType(value.GetType()) ?? typeof(Int32));
                WriteValue(value);
                return;
            }
            if (value is DataTable dt)
            {
                WriteDataTable(dt);
                return;
            }
            if (value is DataRow dr)
            {
                WriteDataRow(dr);
                return;
            }
            if (value is Type _t)
            {
                WriteType(_t);
                return;
            }
            if (value is Delegate)
            {
                WriteString(value.ToString());
                return;
            }
            if (value is JsonValue jsonValue)
            {
                WriteJsonValue(jsonValue);
                return;
            }

            //var ot = System.Diagnostics.Stopwatch.StartNew();
            //ot.Start();
            WriteObject(value);
            //ot.Stop();
            //this.ObjectTimes += ot.ElapsedMilliseconds;
        }
        #endregion

        #region 写时间
        /// <summary>
        /// 写时间
        /// </summary>
        /// <param name="dateTime">时间</param>
        private void WriteDateTime(DateTime dateTime)
        {
            Builder.AppendFormat("\"{0}\"", dateTime.ToString(this.SerializerSetting.DateTimeFormat));
        }
        #endregion

        #region 写Guid
        /// <summary>
        /// 写Guid
        /// </summary>
        /// <param name="guid">guid</param>
        private void WriteGuid(Guid guid)
        {
            Builder.AppendFormat("\"{0}\"", guid.ToString(SerializerSetting.GuidFormat));
        }
        #endregion

        #region 写对象
        /// <summary>
        /// 写对象
        /// </summary>
        /// <param name="obj">对象</param>
        private void WriteObject(object obj)
        {
            //if (!_DepthDict.TryGetValue(obj, out var i)) _DepthDict.Add(obj, _DepthDict.Count + 1);
            Builder.Append('{');
            _Depth++;
            if (_Depth > SerializerSetting.MaxDepth) throw new JsonException("超过了序列化最大深度 " + SerializerSetting.MaxDepth);
            var t = obj.GetType();
            var first = true;
            var keys = new List<string>();
            var list = new List<MemberInfo>(t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            list.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            /*类是否有忽略空节点*/
            var IClassOmitEmptyNode = t.IsDefined(typeof(OmitEmptyNodeAttribute), false);
            list.Each(m =>
            {
                /*如果是 索引器 则跳过*/
                if (m is PropertyInfo _p && _p.GetIndexParameters().Length > 0) return;
                /*如果是 被重写的属性 则跳过*/
                if (keys.Contains(m.Name) && m.DeclaringType != t) return;
                /*如果有忽略属性 则跳过*/
                if (m.IsDefined(typeof(JsonIgnoreAttribute), false)) return;
                
                string name = m.Name;
                keys.Add(name);

                if (!this.SerializerSetting.IgnoreJsonElement)
                {
                    var element = m.GetCustomAttribute<JsonElementAttribute>(false);
                    if (element != null && element.Name.IsNotNullOrEmpty()) name = element.Name;
                }
                object value = null;
                Type mType = m.DeclaringType;
                try
                {
                    if (m.MemberType == MemberTypes.Property)
                    {
                        var p = m as PropertyInfo;
                        mType = p.PropertyType;
                        value = p.GetValue(obj, null);
                    }
                    else
                    {
                        var f = m as FieldInfo;
                        mType = f.FieldType;
                        value = f.GetValue(obj);
                    }
                }
                catch (Exception ex)
                {
                    mType = typeof(string);
                    value = ex.Message;
                }
                var comment = string.Empty;
                if (this.SerializerSetting.IsComment)
                {
                    comment = m.GetDescription(false);
                    if (comment.IsNotNullOrEmpty()) comment = comment.RemovePattern(@"[\r\n\t]+");
                    //if (desc.IsNotNullOrEmpty()) name = "/*{0}*/{1}".format(desc, name);
                }

                JsonConverterAttribute jsonConverter = m.GetCustomAttribute<JsonConverterAttribute>(false);
                if (jsonConverter != null)
                {
                    if (jsonConverter.ConverterType == typeof(StringEnumConverter) && mType.IsEnum)
                        value = value.ToString();
                    else if (jsonConverter.ConverterType == typeof(DescriptionConverter))
                    {
                        if (mType.IsEnum)
                        {
                            value = value.GetType().GetField(value.ToString()).GetDescription(false);
                        }
                        else
                            value = m.GetDescription(false);
                    }
                }
                if (value.IsNullOrEmpty())
                {
                    /*属性是否有忽略空节点*/
                    var FieldOmitEmptyNode = m.IsDefined(typeof(OmitEmptyNodeAttribute), false);
                    if (this.SerializerSetting.OmitEmptyNode || IClassOmitEmptyNode || FieldOmitEmptyNode) return;
                }
                var fw = WritePair(name, value, first ? "" : ",", comment);
                if (first && fw) first = false;
            });
            keys.Clear();
            Builder.Append('}');
            _Depth--;
        }
        #endregion

        #region 写DataTable
        /// <summary>
        /// 写DataTable
        /// </summary>
        /// <param name="data">数据表</param>
        private void WriteDataTable(DataTable data)
        {
            Builder.Append('[');
            var first = true;
            data.Rows.Each<DataRow>(dr =>
            {
                if (!first) Builder.Append(',');
                Builder.Append('{');
                first = false;
                var _first = true;
                data.Columns.Each<DataColumn>(c =>
                {
                    var fw = WritePair(c.ColumnName, dr[c.ColumnName], _first ? "" : ",");
                    if (_first && fw) _first = false;
                });
                Builder.Append('}');
            });
            Builder.Append(']');
        }
        #endregion

        #region 写DataRow
        /// <summary>
        /// 写DataRow
        /// </summary>
        /// <param name="dr">数据</param>
        private void WriteDataRow(DataRow dr)
        {
            var first = true;
            dr.Table.Columns.Each<DataColumn>(c =>
            {
                //if (!first) Builder.Append(',');
                var fw = WritePair(c.ColumnName, dr[c.ColumnName], first ? "" : ",");
                if (first && fw) first = false;
                //Builder.Append("\"" + c.ColumnName + "\":");
                //WriteValue(dr[c.ColumnName]);
            });
        }
        #endregion

        #region 写键值对
        /// <summary>
        /// 写键值对
        /// </summary>
        /// <param name="nvs">键值对</param>
        private void WriteNV(NameValueCollection nvs)
        {
            Builder.Append('{');
            var first = true;
            foreach (string item in nvs)
            {
                if (nvs[item] != null)
                {
                    var fw = WritePair(item, nvs[item], first ? "" : ",");
                    if (first && fw) first = false;
                }
            }
            Builder.Append('}');
        }
        /// <summary>
        /// 写键值对
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <param name="prefix">前缀字符</param>
        /// <param name="comment">注释</param>
        private Boolean WritePair(string name, object value, string prefix = "", string comment = "")
        {
            if (this.SerializerSetting.OmitEmptyNode)
            {
                if (value.IsNullOrEmpty()) return false;
                if (value.GetType().GetInterface("ICollection", true) != null && ((ICollection)value).Count == 0) return false;
                if (value is DataTable dt && dt.Rows.Count == 0) return false;
            }
            if (prefix.IsNotNullOrEmpty()) Builder.Append(prefix);
            WriteStringFast(this.SerializerSetting.IgnoreCase ? name.ToLower() : name);
            if (comment.IsNotNullOrEmpty())
            {
                Builder.Append($"/*{comment}*/");
            }
            Builder.Append(':');
            WriteValue(value);
            return true;
        }
        #endregion

        #region 写数组
        /// <summary>
        /// 写数组
        /// </summary>
        /// <param name="arr">数组</param>
        private void WriteArray(IEnumerable arr)
        {
            Builder.Append('[');
            foreach (var obj in arr)
            {
                this.WriteValue(obj);
                Builder.Append(',');
            }
            Builder = Builder.Replace(",", "", Builder.Length - 1, 1);
            Builder.Append(']');
        }
        #endregion

        #region 写字典
        /// <summary>
        /// 写字典
        /// </summary>
        /// <param name="dic">字典</param>
        private void WriteSD(StringDictionary dic)
        {
            Builder.Append('{');
            var first = true;
            foreach (DictionaryEntry item in dic)
            {
                var fw = WritePair((string)item.Key, item.Value, first ? "" : ",");
                if (first && fw) first = false;
            }
            Builder.Append('}');
        }
        /// <summary>
        /// 写字典
        /// </summary>
        /// <param name="dic">字典</param>
        private void WriteStringDictionary(IDictionary<string, object> dic)
        {
            Builder.Append('{');
            var first = true;
            foreach (var item in dic)
            {
                var fw = WritePair(item.Key, item.Value, first ? "" : ",");
                if (first && fw) first = false;
            }
            Builder.Append('}');
        }
        /// <summary>
        /// 写字典
        /// </summary>
        /// <param name="dic">字典</param>
        private void WriteDictionary(IDictionary dic)
        {
            Builder.Append('{');
            var first = true;
            foreach(DictionaryEntry a in dic)
            {
                var fw = WritePair(a.Key.ToString(), a.Value, first ? "" : ",");
                if (first && fw) first = false;
            };
            Builder.Append('}');
        }
        #endregion

        #region 写字符串
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="str">字符串</param>
        private void WriteStringFast(string str)
        {
            //var comment = "";
            //if (str.IndexOfX("/*") > -1 && str.IndexOfX("*/") > -1 && str.IsMatch(@"^\/\*[\s\S]*?\*\/"))
            //{
            //    var _ = str.GetMatchs(@"^(?<a>\/\*[\s\S]*?\*\/)(?<b>[\s\S]*)$");
            //    str = _["b"];
            //    comment = _["a"];
            //}
            Builder.Append($"\"{str}\"");
            //if (comment.IsNotNullOrEmpty())
            //    Builder.Append(" " + comment);
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="str">字符串</param>
        private void WriteString(string str)
        {
            Builder.Append('\"');
            var idx = -1;
            var len = str.Length;
            for (var index = 0; index < len; ++index)
            {
                var c = str[index];
                if (c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\')// && c != ':' && c!=',')
                {
                    if (idx == -1) idx = index;
                    continue;
                }
                if (idx != -1)
                {
                    Builder.Append(str, idx, index - idx);
                    idx = -1;
                }
                switch (c)
                {
                    case '\t': Builder.Append("\\t"); break;
                    case '\r': Builder.Append("\\r"); break;
                    case '\n': Builder.Append("\\n"); break;
                    case '"':
                    case '\\': Builder.Append('\\'); Builder.Append(c); break;
                    default:
                        Builder.Append(c);

                        break;
                }
            }
            if (idx != -1) Builder.Append(str, idx, str.Length - idx);
            Builder.Append('\"');
        }
        #endregion

        #region 写类型
        /// <summary>
        /// 写类型
        /// </summary>
        /// <param name="type">类型</param>
        private void WriteType(Type type)
        {
            Builder.Append('\"');
            Builder.Append(type.AssemblyQualifiedName);
            Builder.Append('\"');
        }
        #endregion

        #region 写JsonValue
        /// <summary>
        /// 写JsonValue
        /// </summary>
        /// <param name="jsonValue">jsonvalue</param>
        private void WriteJsonValue(JsonValue jsonValue)
        {
            switch (jsonValue.Type)
            {
                case JsonType.Array:
                    WriteValue(jsonValue.AsArray()); break;
                case JsonType.Object:
                    WriteValue(jsonValue.AsObject()); break;
                case JsonType.Bool:
                case JsonType.Byte:
                case JsonType.DateTime:
                case JsonType.Float:
                case JsonType.Char:
                case JsonType.Guid:
                case JsonType.Null:
                case JsonType.String:
                case JsonType.Type:
                case JsonType.Number:
                    WriteValue(jsonValue.value); break;
            }
        }
        #endregion

        #region 格式化Json文本
        /// <summary>格式化Json文本</summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string Format(string json)
        {
            var sb = new StringBuilder();
            var escaping = false;
            var inQuotes = false;
            var indentation = 0;
            foreach (var ch in json)
            {
                if (escaping)
                {
                    escaping = false;
                    sb.Append(ch);
                }
                else
                {
                    if (ch == '\\')
                    {
                        escaping = true;
                        sb.Append(ch);
                    }
                    else if (ch == '\"')
                    {
                        inQuotes = !inQuotes;
                        sb.Append(ch);
                    }
                    else if (!inQuotes)
                    {
                        if (ch == ',')
                        {
                            sb.Append(ch);
                            sb.Append("\r\n");
                            sb.Append(' ', indentation * 2);
                        }
                        else if (ch == '[' || ch == '{')
                        {
                            sb.Append(ch);
                            sb.Append("\r\n");
                            sb.Append(' ', ++indentation * 2);
                        }
                        else if (ch == ']' || ch == '}')
                        {
                            sb.Append("\r\n");
                            sb.Append(' ', --indentation * 2);
                            sb.Append(ch);
                        }
                        else if (ch == ':')
                        {
                            sb.Append(ch);
                            sb.Append(' ', 1);
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
            }
            var _ = sb.ToString();
            if (this.SerializerSetting.IsComment)
            {
                return _.ReplacePattern(@"/\*[\s\S]+?\*/", m =>
                {
                    return m.Groups[0].Value.RemovePattern(@"[\r\n]+").ReplacePattern(@"\s{2,}"," ");
                });
            }
            return _;
        }
        #endregion

        #endregion
    }
}