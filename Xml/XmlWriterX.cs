using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/16 8:33:12                             *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// XML写入器
    /// </summary>
    public class XmlWriterX
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XmlWriterX()
        {

        }
        /// <summary>
        /// 写对象
        /// </summary>
        /// <param name="o">对象</param>
        public XmlWriterX(object o)
        {
            this.Data = o;
            this.ObjectType = o?.GetType();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 类型
        /// </summary>
        public Type ObjectType { get; set; }
        /// <summary>
        /// 对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 写对象
        /// </summary>
        private XmlWriter XmlWriter { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        private byte[] Bytes { get; set; }
        /// <summary>
        /// 序列化配置
        /// </summary>
        public XmlSerializerSetting SerializerSetting { get; set; } = new XmlSerializerSetting();
        #endregion

        #region 方法
        /// <summary>
        /// 写入到流
        /// </summary>
        /// <param name="stream">流</param>
        public void WriteTo(MemoryStream stream) => stream.Write(this.Bytes, 0, this.Bytes.Length);
        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串编码</returns>
        private string EncodeString(string str) => str.ReplacePattern(@"<", "&lt;").ReplacePattern(@">", "&gt;").ReplacePattern(@"'", "&apos;").ReplacePattern(@"""", "&quot;");
        /// <summary>
        /// 写XML
        /// </summary>
        public void Write()
        {
            var type = this.ObjectType;
            var rootName = this.SerializerSetting.DefaultRootName;
            if (type != null && type.IsDefined(typeof(XmlRootAttribute), false))
            {
                var root = type.GetCustomAttribute<XmlRootAttribute>();
                rootName = root.ElementName.IfEmpty(type.Name);
            }
            using (var ms = new MemoryStream())
            {
                using (XmlWriter = XmlWriter.Create(ms, new XmlWriterSettings
                {
                    Indent = this.SerializerSetting.Indented,
                    Encoding = this.SerializerSetting.DefaultEncoding.WebName == "UTF-8" ? new UTF8Encoding(false) : this.SerializerSetting.DefaultEncoding,
                    OmitXmlDeclaration = this.SerializerSetting.OmitXmlDeclaration,
                    NewLineChars = this.SerializerSetting.NewLineChars,
                    NamespaceHandling = this.SerializerSetting.NamespaceHandling
                }))
                {
                    if (rootName.IsNotNullOrEmpty()) XmlWriter.WriteStartElement(rootName);
                    if (!this.SerializerSetting.OmitNamespace)
                    {
                        XmlWriter.WriteAttributeString("xmlns", "xsi", "", "http://www.w3.org/2001/XMLSchema-instance");
                        XmlWriter.WriteAttributeString("xmlns", "xsd", "", "http://www.w3.org/2001/XMLSchema");
                    }
                    this.WriteValue(this.Data);
                    if (rootName.IsNotNullOrEmpty()) XmlWriter.WriteEndDocument();
                }
                this.Bytes = ms.ToArray();
                XmlWriter.Close();
            }
        }
        /// <summary>
        /// 写数据
        /// </summary>
        public void WriteValue(object data, Boolean isCData = false, string tagName = "")
        {
            if (data.IsNullOrEmpty())
            {
                if (this.SerializerSetting.OmitEmptyNode) return;
                if (tagName.IsNullOrEmpty()) return;
                XmlWriter.WriteStartElement(tagName);
                XmlWriter.WriteEndElement();
                return;
            }
            var type = data.GetType();
            var BaseType = type.GetValueType();
            if (data is String str)
            {
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteStartElement(tagName);
                if (isCData)
                    XmlWriter.WriteCData(str);
                else
                    XmlWriter.WriteString(EncodeString(str));
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteEndElement();
                return;
            }
            else if (data is DateTime date)
            {
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteStartElement(tagName);
                if (isCData)
                    XmlWriter.WriteCData(date.ToString(this.SerializerSetting.DateTimeFormat));
                else
                    XmlWriter.WriteString(date.ToString(this.SerializerSetting.DateTimeFormat));
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteEndElement();
                return;
            }
            else if (data is Guid guid)
            {
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteStartElement(tagName);
                if (isCData)
                    XmlWriter.WriteCData(guid.ToString(this.SerializerSetting.GuidFormat));
                else
                    XmlWriter.WriteString(guid.ToString(this.SerializerSetting.GuidFormat));
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteEndElement();
                return;
            }
            else if (data is IEnumerable ie)
            {
                foreach (var item in ie)
                {
                    WriteValue(item, isCData, tagName);
                }
            }
            else if (data is Enum)
            {
                if (SerializerSetting.EnumValueType == EnumValueType.Name)
                    data = data.ToString();
                else if (SerializerSetting.EnumValueType == EnumValueType.Description)
                    data = data.GetType().GetField(data.ToString()).GetDescription(false);
                else data = data.GetValue(Enum.GetUnderlyingType(data.GetType()) ?? typeof(Int32));

                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteStartElement(tagName);
                if (isCData)
                    XmlWriter.WriteCData(data.ToString());
                else
                    XmlWriter.WriteString(EncodeString(data.ToString()));
                if (tagName.IsNotNullOrEmpty()) XmlWriter.WriteEndElement();
                return;
            }
            else if (data is XmlValue xvalue)
            {
                if (xvalue.IsEmpty && this.SerializerSetting.OmitEmptyNode) return;

                if (tagName.IsNullOrEmpty()) tagName = xvalue.Name;
                XmlWriter.WriteStartElement(tagName);
                if (xvalue.HasAttributes)
                {
                    xvalue.Attributes.Each(a =>
                    {
                        XmlWriter.WriteAttributeString(a.Name, a.Value.getValue());
                    });
                }
                if (xvalue.HasChildNodes)
                {
                    xvalue.ChildNodes.Each(c =>
                    {
                        this.WriteValue(c, c.ElementType == XmlType.CDATA, c.Name);
                    });
                }
                else
                {
                    if (xvalue.Value.IsNotNullOrEmpty())
                    {
                        if (xvalue.ElementType == XmlType.CDATA)
                            XmlWriter.WriteCData(xvalue.Value.getValue());
                        else
                            XmlWriter.WriteString(xvalue.Value.getValue());
                    }
                }
                XmlWriter.WriteEndElement();
                return;
            }
            else
            {
                if (BaseType == ValueTypes.Value || BaseType == ValueTypes.String || BaseType == ValueTypes.Null)
                {
                    XmlWriter.WriteStartElement(tagName);
                    if (isCData)
                        XmlWriter.WriteCData(data.ToString());
                    else
                        XmlWriter.WriteString(EncodeString(data.ToString()));
                    XmlWriter.WriteEndElement();
                }
                else if (BaseType == ValueTypes.Class || BaseType == ValueTypes.Struct || BaseType == ValueTypes.Anonymous)
                {
                    if (tagName.IsNotNullOrEmpty())
                        XmlWriter.WriteStartElement(tagName);
                    /*类是否有忽略空节点*/
                    var ClassOmitEmptyNode = type.IsDefined(typeof(OmitEmptyNodeAttribute), false);
                    //var fields = type.GetPropertiesAndFields();
                    var fields = new List<MemberInfo>(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
                    fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
                    fields.Where(a => a.IsDefined(typeof(XmlAttributeAttribute), false)).Each(a =>
                    {
                        var val = (a.MemberType == MemberTypes.Property ? (a as PropertyInfo).GetValue(data) : (a as FieldInfo).GetValue(data));
                        if (val.IsNullOrEmpty()) return;
                        string value = string.Empty;
                        if (val is String _data)
                            value = EncodeString(_data);
                        else if (val is DateTime _date)
                            value = _date.ToString(this.SerializerSetting.DateTimeFormat);
                        else if (val is Guid _guid)
                            value = _guid.ToString(this.SerializerSetting.GuidFormat);
                        else if (val is Boolean _val)
                            value = _val.ToString().ToLower();
                        else if (val is Enum _enum)
                        {
                            if (a.IsDefined(typeof(XmlConverterAttribute), false))
                            {
                                var converter = a.GetCustomAttribute<XmlConverterAttribute>();
                                if (converter.ConverterType == typeof(DescriptionConverter))
                                {
                                    value = _enum.GetDescription().IfEmpty(((int)val).ToString());
                                }
                                else if (converter.ConverterType == typeof(StringEnumConverter))
                                {
                                    value = _enum.ToString();
                                }
                            }
                            else
                            {
                                value = ((int)val).ToString();
                            }
                        }
                        else value = val.ToString();
                        /*属性是否有忽略属性*/
                        var FieldOmitEmptyNode = a.IsDefined(typeof(OmitEmptyNodeAttribute), false);
                        if (value.IsNullOrEmpty() && (this.SerializerSetting.OmitEmptyNode || ClassOmitEmptyNode || FieldOmitEmptyNode)) return;
                        XmlWriter.WriteAttributeString(a.GetCustomAttribute<XmlAttributeAttribute>().AttributeName.IfEmpty(a.Name), value);
                    });
                    fields.Where(a => !a.IsDefined(typeof(XmlAttributeAttribute), false)).Each(m =>
                      {
                          var FieldName = m.Name;
                          var ItemName = "";
                          var ArrayName = "";
                          if (m.IsDefined(typeof(XmlIgnoreAttribute), false) || !(m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)) return;
                          var value = m.MemberType == MemberTypes.Property ? (m as PropertyInfo).GetValue(data) : (m as FieldInfo).GetValue(data);
                          /*属性是否有忽略属性*/
                          var FieldOmitEmptyNode = m.IsDefined(typeof(OmitEmptyNodeAttribute), false);
                          if (value.IsNullOrEmpty() && (this.SerializerSetting.OmitEmptyNode || ClassOmitEmptyNode || FieldOmitEmptyNode)) return;
                          /*写注释*/
                          if (!this.SerializerSetting.OmitComment && m.IsDefined(typeof(DescriptionAttribute), false))
                          {
                              var description = m.GetDescription();
                              if (description.IsNotNullOrEmpty())
                                  XmlWriter.WriteComment(description);
                          }
                          /*写节点名称*/
                          if (m.IsDefined(typeof(XmlElementAttribute), false))
                              FieldName = m.GetCustomAttribute<XmlElementAttribute>().ElementName.IfEmpty(FieldName);
                          var _type = m.MemberType == MemberTypes.Property ? (m as PropertyInfo).PropertyType : (m as FieldInfo).FieldType;
                          var _BaseType = _type.GetValueType();
                          if (m.IsDefined(typeof(XmlArrayAttribute), false))
                          {
                              ArrayName = m.GetCustomAttribute<XmlArrayAttribute>().ElementName;
                          }
                          /*字节名称*/
                          if (m.IsDefined(typeof(XmlArrayItemAttribute), false))
                          {
                              ItemName = m.GetCustomAttribute<XmlArrayItemAttribute>().ElementName.IfEmpty(FieldName);
                              //if (ArrayName.IsNullOrEmpty()) ArrayName = FieldName;
                          }

                          if (_BaseType == ValueTypes.Anonymous || _BaseType == ValueTypes.Dictionary || _BaseType == ValueTypes.IDictionary)
                          {
                              XmlWriter.WriteStartElement(FieldName);
                              foreach (DictionaryEntry item in (IEnumerable)value)
                              {
                                  WriteValue(item.Value, false, ItemName.IfEmpty(item.Key.ToString()));
                              }
                              XmlWriter.WriteEndElement();
                          }
                          else if (_BaseType == ValueTypes.Array || _BaseType == ValueTypes.ArrayList || _BaseType == ValueTypes.IEnumerable || _BaseType == ValueTypes.List)
                          {
                              if (ArrayName.IsNullOrEmpty())
                              {
                                  if (!this.SerializerSetting.OmitArrayItemName && ItemName.IsNotNullOrEmpty())
                                      XmlWriter.WriteStartElement(ItemName);
                              }
                              else
                                  XmlWriter.WriteStartElement(ArrayName);
                              //if (!this.SerializerSetting.OmitArrayItemName && ItemName.IsNotNullOrEmpty())
                              //    XmlWriter.WriteStartElement(FieldName);
                              if (value != null)
                              {
                                  foreach (var item in (IEnumerable)value)
                                  {
                                      WriteValue(item, false, ItemName.IfEmpty(this.SerializerSetting.OmitArrayItemName ? FieldName : item.GetType().Name));
                                  }
                              }
                              //else
                              //XmlWriter.WriteValue(null);
                              if (ArrayName.IsNullOrEmpty())
                              {
                                  if (!this.SerializerSetting.OmitArrayItemName && ItemName.IsNotNullOrEmpty())
                                      XmlWriter.WriteEndElement();
                              }
                              else
                                  XmlWriter.WriteEndElement();
                          }
                          else
                          {
                              if (_BaseType == ValueTypes.Enum)
                              {
                                  if (m.IsDefined(typeof(XmlConverterAttribute), false))
                                  {
                                      var converter = m.GetCustomAttribute<XmlConverterAttribute>();
                                      if (converter.ConverterType == typeof(DescriptionConverter))
                                      {
                                          if ((m.MemberType == MemberTypes.Field ? (m as FieldInfo).FieldType : (m as PropertyInfo).PropertyType).GetValueType() == ValueTypes.Enum)
                                          {
                                              value = value.GetType().GetBaseType().GetField(value.ToString()).GetDescription().IfEmpty(((int)value).ToString());
                                          }
                                          else
                                              value = m.GetDescription().IfEmpty(((int)value).ToString());
                                      }
                                      else if (converter.ConverterType == typeof(StringEnumConverter))
                                      {
                                          value = value.ToString();
                                      }
                                  }
                                  else
                                  {
                                      value = (int)value;
                                  }
                              }
                              else
                              {

                              }
                              WriteValue(value, m.IsDefined(typeof(XmlCDataAttribute), false), m.IsDefined(typeof(XmlTextAttribute), false) ? "" : ItemName.IfEmpty(FieldName));
                          }
                      });
                    if (tagName.IsNotNullOrEmpty())
                        XmlWriter.WriteEndElement();
                }
            }
        }
        #endregion
    }
}