using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/16 8:33:33                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// 类说明
    /// </summary>
    public class XmlReaderX
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XmlReaderX()
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="xml">数据</param>
        /// <param name="type">类型</param>
        public XmlReaderX(string xml, Type type)
        {
            this.Xml = xml;
            this.ObjectType = type;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据
        /// </summary>
        public string Xml { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type ObjectType { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public XmlSerializerSetting SerializerSetting { get; set; } = new XmlSerializerSetting();
        #endregion

        #region 方法
        /// <summary>
        /// 字符串解码
        /// </summary>
        /// <param name="str">编码字符串</param>
        /// <returns></returns>
        public string DecodeString(string str) => str.ReplacePattern("&lt;", "<").ReplacePattern("&gt;", ">").ReplacePattern("&apos;", "'").ReplacePattern("&quot;", @"""");
        /// <summary>
        /// 读取XML
        /// </summary>
        public XmlValue ParseXml()
        {
            using (var reader = XmlReader.Create(new MemoryStream(this.Xml.GetBytes()), new XmlReaderSettings
            {
                Async = true,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                MaxCharactersInDocument = 0
            }))
            {
                XmlValue value = new XmlValue()
                {
                    Name = "Document",
                    ElementType = XmlType.Document,
                    Depth = -1,
                    ParentElement = null
                };
                //ParseValue(reader, value);
                var parentElement = value;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.None || parentElement == null) break;
                    parentElement = ParseValues(reader, parentElement);
                }
                return value;
            }
        }
        /// <summary>
        /// 转换XML
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <param name="parentElement">父节点</param>
        private XmlValue ParseValues(XmlReader reader, XmlValue parentElement)
        {
            if (reader.Depth > this.SerializerSetting.MaxDepth) return parentElement;
            var pElement = parentElement;
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    var element = new XmlValue
                    {
                        Name = reader.Name,
                        ElementType = (XmlType)(int)reader.NodeType,
                        Depth = reader.Depth,
                        ParentElement = parentElement
                    };
                    parentElement.Append(element);
                    if (!reader.IsEmptyElement)
                        pElement = element;
                    if (reader.HasAttributes)
                    {
                        element.Attributes = new List<XmlValue>();
                        while (reader.MoveToNextAttribute())
                        {
                            element.Attributes.Add(new XmlValue
                            {
                                Name = reader.Name,
                                Value = DecodeString(reader.Value),
                                ElementType = (XmlType)(int)XmlNodeType.Attribute,
                                Depth = reader.Depth,
                                ParentElement = element
                            });
                        }
                    }
                    break;
                case XmlNodeType.Text:
                    parentElement.Value = DecodeString(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    parentElement.Value = reader.Value;
                    break;
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.Comment:
                case XmlNodeType.EntityReference:
                    parentElement.Append(new XmlValue
                    {
                        Name = reader.Name,
                        Value = reader.Value,
                        ElementType = (XmlType)(int)reader.NodeType,
                        Depth = reader.Depth,
                        ParentElement = parentElement
                    });
                    break;
                case XmlNodeType.DocumentType:
                case XmlNodeType.Document:
                case XmlNodeType.XmlDeclaration:
                    parentElement.Append(new XmlValue
                    {
                        Name = reader.Name,
                        Value = reader.Value,
                        ElementType = (XmlType)(int)reader.NodeType,
                        Depth = reader.Depth,
                        ParentElement = parentElement
                    });
                    break;
                case XmlNodeType.EndElement:
                    pElement = parentElement.ParentElement;
                    break;
            }
            return pElement;
            //ParseValue(reader, pElement);
        }
        /// <summary>
        /// 转换XML
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <param name="parentElement">父节点</param>
        private void ParseValue(XmlReader reader, XmlValue parentElement)
        {
            if (!reader.Read() || reader.NodeType == XmlNodeType.None || parentElement == null) return;
            if (reader.Depth > this.SerializerSetting.MaxDepth) return;
            var pElement = parentElement;
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    var element = new XmlValue
                    {
                        Name = reader.Name,
                        ElementType = (XmlType)(int)reader.NodeType,
                        Depth = reader.Depth,
                        ParentElement = parentElement
                    };
                    if (reader.HasAttributes)
                    {
                        element.Attributes = new List<XmlValue>();
                        while (reader.MoveToNextAttribute())
                        {
                            element.Attributes.Add(new XmlValue
                            {
                                Name = reader.Name,
                                Value = DecodeString(reader.Value),
                                ElementType = (XmlType)(int)XmlNodeType.Attribute,
                                Depth = reader.Depth,
                                ParentElement = element
                            });
                        }
                    }
                    parentElement.Append(element);
                    if (!reader.IsEmptyElement)
                        pElement = element;
                    break;
                case XmlNodeType.Text:
                    parentElement.Value = DecodeString(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    parentElement.Value = reader.Value;
                    break;
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.Comment:
                case XmlNodeType.EntityReference:
                    parentElement.Append(new XmlValue
                    {
                        Name = reader.Name,
                        Value = reader.Value,
                        ElementType = (XmlType)(int)reader.NodeType,
                        Depth = reader.Depth,
                        ParentElement = parentElement
                    });
                    break;
                case XmlNodeType.DocumentType:
                case XmlNodeType.Document:
                case XmlNodeType.XmlDeclaration:
                    parentElement.Append(new XmlValue
                    {
                        Name = reader.Name,
                        Value = reader.Value,
                        ElementType = (XmlType)(int)reader.NodeType,
                        Depth = reader.Depth,
                        ParentElement = parentElement
                    });
                    break;
                case XmlNodeType.EndElement:
                    pElement = parentElement.ParentElement;
                    break;
            }
            ParseValue(reader, pElement);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public object Read(Type type = null)
        {
            var value = this.ParseXml();
            if (type == null && this.ObjectType == null) return value;
            if (type == null) type = this.ObjectType;
            if (!value.HasChildNodes) return null;
            var element = value.ChildNodes.Find(a => a.ElementType == XmlType.Element);
            if (element == null) return null;
            var model = Activator.CreateInstance(type);
            if (!element.HasChildNodes && !element.HasAttributes) return model;
            return element.ToObject(type, model);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public T Read<T>() => (T)this.Read(typeof(T));
        #endregion
    }
}