using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using XiaoFeng.Xml;
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
    /// 扩展XML方法
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 对象转XML
        /// <summary>
        /// 对象转XML
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="t">对象</param>
        /// <param name="encode">编码</param>
        /// <param name="removeNamespaces">是否移除命名空间</param>
        /// <param name="removeXmlDeclaration">是否移除XML声明</param>
        /// <returns></returns>
        [Obsolete("已过期，请使用EntityToXml")]
        public static string ToXml<T>(this T t, string encode = "UTF-8", Boolean removeNamespaces = false, Boolean removeXmlDeclaration = false)
        {
            if (t == null) return String.Empty;
            return XmlConvert.SerializerObject(t, encode, removeNamespaces, removeXmlDeclaration);
        }
        /// <summary>
        /// 对象转XML
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="t">对象</param>
        /// <param name="encoding">编码</param>
        /// <param name="OmitXmlDeclaration">是否忽略XML声明</param>
        /// <param name="OmitEmptyNode">是否忽略空节点</param>
        /// <param name="OmitNamespace">是否忽略命名空间</param>
        /// <param name="OmitComment">是否忽略注释</param>
        /// <param name="Indented">是否格式化</param>
        /// <returns></returns>
        public static string EntityToXml<T>(this T t, Encoding encoding = null, Boolean OmitXmlDeclaration = false, Boolean OmitEmptyNode = true, Boolean OmitNamespace = true, Boolean OmitComment = false, Boolean Indented = true) => XmlSerializer.Serializer(t, encoding, OmitXmlDeclaration, OmitEmptyNode, OmitNamespace, OmitComment, Indented);
        #endregion

        #region XML转对象
        /// <summary>
        /// XML转对象
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="xml">XML数据</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        [Obsolete("已过期，请使用XmlToEntity")]
        public static T XmlToObject<T>(this String xml, string encode = "UTF-8")
        {
            if (xml.IsNullOrEmpty()) return Activator.CreateInstance<T>();
            return XmlConvert.DeserializeObject<T>(xml, encode);
        }
        /// <summary>
        /// XML转对象
        /// </summary>
        /// <param name="xml">XML数据</param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete("已过期，请使用XmlToEntity")]
        public static object XmlToObject(this String xml, Type type)
        {
            if (xml.IsNullOrEmpty()) return Activator.CreateInstance(type);
            return XmlConvert.DeserializeObject(type, xml);
        }
        /// <summary>
        /// XML转对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="xml">XML数据</param>
        /// <returns></returns>
        public static T XmlToEntity<T>(this String xml) => XmlSerializer.Deserialize<T>(xml);
        /// <summary>
        /// XML转对象
        /// </summary>
        /// <param name="xml">XML数据</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object XmlToEntity(this String xml, Type type = null) => XmlSerializer.Deserialize(xml, type);
        #endregion

        #region 获取节点
        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="xElement">节点</param>
        /// <param name="elementName">节点名称</param>
        /// <returns></returns>
        public static XElement GetXElement(this XElement xElement, string elementName)
        {
            if (!xElement.HasElements) return null;
            var name = xElement.GetDefaultNamespace();
            return xElement.Element((name.IsNullOrEmpty() ? "" : name) + elementName);
        }
        /// <summary>
        /// 获取节点下子节点集
        /// </summary>
        /// <param name="xElement">节点</param>
        /// <param name="elementName">节点名称</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetXElements(this XElement xElement, string elementName)
        {
            if (!xElement.HasElements) return null;
            var name = xElement.GetDefaultNamespace();
            return xElement.Elements((name.IsNullOrEmpty() ? "" : name) + elementName);
        }
        /// <summary>
        /// 获取节点下所有节点
        /// </summary>
        /// <param name="xElement">节点</param>
        /// <param name="elementName">节点名称</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetXDescendants(this XElement xElement, string elementName)
        {
            if (!xElement.HasElements) return null;
            var name = xElement.GetDefaultNamespace();
            return xElement.Descendants((name.IsNullOrEmpty() ? "" : name) + elementName);
        }
        /// <summary>
        /// 获取节点属性
        /// </summary>
        /// <param name="xElement">节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns></returns>
        public static XAttribute GetXAttribute(this XElement xElement, string attributeName)
        {
            if (!xElement.HasAttributes) return null;
            if (attributeName.IndexOf(":") > -1)
            {
                var rs = attributeName.Split(':');
                var rName = rs[0];
                var rValue = rs[1];
                if (rName.IsNotNullOrEmpty())
                {
                    XNamespace xRName = xElement.GetNamespaceOfPrefix(rName);
                    if (xRName.NamespaceName.IsNullOrEmpty()) return null;
                    return xElement.Attribute(xRName + rValue);
                }
            }
            return xElement.Attribute(attributeName);
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="xElement">节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>属性值</returns>
        public static T GetXAttribute<T>(this XElement xElement, string attributeName, T defaultValue = default(T))
        {
            var attr = xElement.GetXAttribute(attributeName);
            return attr == null ? defaultValue : attr.Value.ToCast<T>();
        }
        #endregion
    }
}