using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
*  Author : jacky                                              *
*  QQ : 7092734                                                *
*  Email : jacky@fayelf.com                                    *
*  Site : www.fayelf.com                                       *
*  Create Time : 2021/4/16 8:33:52                             *
*  Version : v 1.0.0                                           *
*  CLR Version : 4.0.30319.42000                               *
****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// Xml序列化类
    /// </summary>
    public static class XmlSerializer
    {

        #region 属性
        /// <summary>
        /// 序列化配置
        /// </summary>
        public static XmlSerializerSetting SerializerSetting { get; set; } = new XmlSerializerSetting();
        #endregion

        #region 方法
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="encoding">编码</param>
        /// <param name="OmitXmlDeclaration">是否忽略头部声明</param>
        /// <param name="OmitEmptyNode">是否忽略空节点</param>
        /// <param name="OmitNamespace">是否忽略命名空间</param>
        /// <param name="OmitComment">是否忽略注释</param>
        /// <param name="Indented">是否格式化</param>
        /// <param name="namespaces">命名空间</param>
        /// <returns></returns>
        public static string Serializer(object o, Encoding encoding = null, Boolean OmitXmlDeclaration = false, Boolean OmitEmptyNode = true, Boolean OmitNamespace = true, Boolean OmitComment = false, Boolean Indented = true, XmlSerializerNamespaces namespaces = null)
        {
            if (o.IsNullOrEmpty()) return string.Empty;
            var writer = new XmlWriterX(o)
            {
                SerializerSetting = SerializerSetting
            };
            writer.SerializerSetting.OmitXmlDeclaration = OmitXmlDeclaration;
            writer.SerializerSetting.OmitEmptyNode = OmitEmptyNode;
            writer.SerializerSetting.OmitNamespace = OmitNamespace;
            writer.SerializerSetting.OmitComment = OmitComment;
            writer.SerializerSetting.Indented = Indented;
            writer.Write(namespaces);
            using (var ms = new MemoryStream())
            {
                writer.WriteTo(ms);
                return ms.ToArray().GetString(encoding.IfEmpty(SerializerSetting.DefaultEncoding));
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="xml">xml内容</param>
        /// <param name="serializerSetting">反序列化配置</param>
        /// <returns></returns>
        public static T Deserialize<T>(String xml, XmlSerializerSetting serializerSetting = null) => (T)Deserialize(xml, typeof(T), serializerSetting);
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xml">xml内容</param>
        /// <param name="type">类型</param>
        /// <param name="serializerSetting">反序列化配置</param>
        /// <returns></returns>
        public static object Deserialize(String xml, Type type, XmlSerializerSetting serializerSetting = null)
        {
            if (xml.IsNullOrEmpty()) return null;
            var reader = new XmlReaderX(xml, type)
            {
                SerializerSetting = serializerSetting ?? SerializerSetting
            };
            if (reader == null) return null;
            return reader.Read();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="stream">Xml文件流</param>
        /// <param name="serializerSetting">序列化配置</param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream, XmlSerializerSetting serializerSetting = null)
        {
            return (T)Deserialize(stream, typeof(T), serializerSetting);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="stream">Xml文件流</param>
        /// <param name="type">类型</param>
        /// <param name="serializerSetting">序列化配置</param>
        /// <returns></returns>
        public static object Deserialize(Stream stream, Type type, XmlSerializerSetting serializerSetting = null)
        {
            if (stream == null) return null;
            var reader = new XmlReaderX(stream, type)
            {
                SerializerSetting = serializerSetting ?? SerializerSetting
            };
            if (reader == null) return null;
            return reader.Read();
        }
        #endregion
    }
}