using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using XiaoFeng.IO;
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
    /// XML操作类
    /// Version : v 1.0
    /// Author : Jacky
    /// Site : www.zhuovi.com
    /// QQ : 7092734
    /// Email : jacky@zhuovi.com
    /// </summary>
    public class XmlConvert
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XmlConvert() { }
        /// <summary>
        /// 设置数据路径
        /// </summary>
        /// <param name="xmlPath">XML 路径</param>
        public XmlConvert(string xmlPath)
        {
            this.GetDom(xmlPath);
        }
        /// <summary>
        /// 设置数据对象
        /// </summary>
        /// <param name="XML">XML 对象</param>
        public XmlConvert(XmlDocument XML) { this.Xml = XML; }
        #endregion

        #region 属性
        /// <summary>
        /// XML 对象
        /// </summary>
        private XmlDocument _Xml = null;
        /// <summary>
        /// XML 对象
        /// </summary>
        public XmlDocument Xml { get { return this._Xml; } set { this._Xml = value; } }
        /// <summary>
        /// XML 路径
        /// </summary>
        private string _XmlPath = "";
        /// <summary>
        /// XML 路径
        /// </summary>
        public string XmlPath { get { return this._XmlPath; } set { this._XmlPath = value; } }
        /// <summary>
        /// 消息
        /// </summary>
        private string _Message = "";
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get { return this._Message; } set { this._Message = value; } }
        #endregion

        #region 方法

        #region 获取 XML 对象
        /// <summary>
        /// 获取 XML 对象
        /// </summary>
        /// <param name="xmlPath">XML 路径</param>
        public XmlDocument GetDom(string xmlPath = "")
        {
            if (xmlPath.IsNullOrEmpty())
            {
                if (this.XmlPath.IsNullOrEmpty())
                {
                    if (this.Xml == null)
                    {
                        this.Message = "XML 对象为空."; return null;
                    }
                    else
                        return this.Xml;
                }
            }
            else
                this.XmlPath = xmlPath;
            this.XmlPath = xmlPath.GetBasePath();
            if (!FileHelper.Exists(this.XmlPath, FileAttribute.File))
            {
                this.Message = "XML 文件不存在.";
                return null;
            }
            else
            {
                this.Xml = new XmlDocument();
                this.Xml.Load(this.XmlPath);
                this.Message = "success";
                return this.Xml;
            }
        }
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="nodePath">结点路径 比如：
        /// <para>@"/Root/Node"</para>
        /// <para>@"/Root/Node[ChildNodeName='ChildNodeValue']</para>
        /// <para>@"/Root/Node[@ChildNodeName='ChildNodeValue']</para>
        /// <para>@"/Root/Node[ChildNodeName='ChildNodeValue']/node</para>
        /// <para>@"/Root/Node[ChildNodeName='ChildNodeValue']/node/@ID</para>
        /// </param>
        /// <param name="valueType">结点类型 0 InnerText 1 InnerXml 2 Value</param>
        /// <returns></returns>
        public string ReadValue(string nodePath, int valueType = 0)
        {
            XmlNode node = this.Xml.SelectSingleNode(@"" + nodePath);
            if (node == null) return "";
            else
            {
                if (valueType == 0) return node.InnerText;
                else if (valueType == 1) return node.InnerXml;
                else if (valueType == 2) return node.Value;
                else return node.InnerText;
            }
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="model">对象</param>
        /// <param name="encode">编码</param>
        /// <param name="removeNamespaces">是否移除命名空间</param>
        /// <param name="removeXmlDeclaration">是否移除XML声明</param>
        /// <returns></returns>
        public static string SerializerObject<T>(T model, string encode = "UTF-8", Boolean removeNamespaces = false, Boolean removeXmlDeclaration = false)
        {
            if (model == null) return "";
            string XmlString = "";
            XmlWriterSettings settings = new XmlWriterSettings
            {
                /*移除xml声明*/
                OmitXmlDeclaration = removeXmlDeclaration,
                Indent = true
            };
            Encoding _Encode = encode.IsNullOrEmpty() ? Encoding.Default : Encoding.GetEncoding(encode);
            if (_Encode == Encoding.UTF8)
                settings.Encoding = new UTF8Encoding();
            else if (_Encode == Encoding.ASCII)
                settings.Encoding = new ASCIIEncoding();
            else if (_Encode == Encoding.UTF7)
                settings.Encoding = new UTF7Encoding();
            else if (_Encode == Encoding.Unicode)
                settings.Encoding = new UnicodeEncoding();
            else if (_Encode == Encoding.UTF32)
                settings.Encoding = new UTF32Encoding();
            else if (_Encode == Encoding.Default)
                settings.Encoding = new UTF8Encoding();
            else
                settings.Encoding = _Encode;
            using (MemoryStream mem = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(mem, settings))
                {
                    /*去除默认命名空间xmlns:xsd和xmlns:xsi*/
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    if (removeNamespaces) ns.Add("", "");
                    XmlSerializer xml = new XmlSerializer(model.GetType());
                    try { xml.Serialize(writer, model, ns); }
                    catch (XmlException ex)
                    {
                        LogHelper.WriteLog(ex); return "";
                    }
                }
                XmlString = _Encode.GetString(mem.ToArray());
            }
            return XmlString;
        }
        #endregion

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="xml">XML数据</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string xml, string encode = "UTF-8")
        {
            if (xml.IsNullOrEmpty()) return default;
            Encoding _Encode = encode.IsNullOrEmpty() ? Encoding.Default : Encoding.GetEncoding(encode);
#if NETSTANDARD2_0
            using (var mem = new MemoryStream(_Encode.GetBytes(xml)))
#else
            using var mem = new MemoryStream(_Encode.GetBytes(xml));
#endif
            {
#if NETSTANDARD2_0
                using (var reader = XmlReader.Create(mem))
#else
                using var reader = XmlReader.Create(mem);
#endif
                {
                    try
                    {
                        XmlSerializer _xml = new XmlSerializer(typeof(T));
                        return (T)_xml.Deserialize(reader);
                    }
                    catch (XmlException ex)
                    {
                        LogHelper.WriteLog(ex);
                        return default;
                    }
                }
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object DeserializeObject(Type type, string xml)
        {
            try
            {
#if NETSTANDARD2_0
                using (var sr = new StringReader(xml))
#else
                using StringReader sr = new StringReader(xml);
#endif
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (XmlException ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="stream">数据流</param>
        /// <returns></returns>
        public static object DeserializeObject(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
#endregion

#endregion
    }
}