using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Collections;
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
    /// 类说明
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
        /// <returns></returns>
        public static string Serializer(object o, Encoding encoding = null, Boolean OmitXmlDeclaration = false, Boolean OmitEmptyNode = true, Boolean OmitNamespace = true, Boolean OmitComment = false, Boolean Indented = true)
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
            writer.Write();
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
        /// <returns></returns>
        public static T Deserialize<T>(String xml) => (T)Deserialize(xml, typeof(T));
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xml">xml内容</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object Deserialize(String xml, Type type)
        {
            if (xml.IsNullOrEmpty()) return null;
            var reader = new XmlReaderX(xml, type)
            {
                SerializerSetting = SerializerSetting
            };
            return reader.Read();
        }
        #endregion
    }
}