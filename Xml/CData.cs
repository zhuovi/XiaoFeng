using System;
using System.Xml.Schema;
using System.Xml.Serialization;
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
namespace XiaoFeng.Xml
{
    /// <summary>
    /// CData扩展
    /// </summary>
    public class CData : IXmlSerializable
    {
        /// <summary>
        /// 值
        /// </summary>
        private string _Value;
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CData() { }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        public CData(string value) => this._Value = value;
        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get
            {
                return this._Value;
            }
        }
        /// <summary>
        /// 是否有值
        /// </summary>
        public Boolean HasValue => this._Value.IsNotNullOrEmpty();
        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="reader">读</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            //this._Value = reader.ReadElementContentAsString();
            reader.MoveToContent();
            var isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                this._Value = reader.ReadString();
                reader.ReadEndElement();
            }
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="writer">写</param>
        public void WriteXml(System.Xml.XmlWriter writer) => writer.WriteCData(this.Value);
        /// <summary>
        /// 获取Schema
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() => null;
        /// <summary>
        /// 重写转字符
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this._Value;
        /// <summary>
        /// 强转字符串
        /// </summary>
        /// <param name="element">节点</param>
        public static implicit operator string(CData element) => element?.Value;
        /// <summary>
        /// 强转 int
        /// </summary>
        /// <param name="element">节点</param>
        public static implicit operator int(CData element) => (int)element?.Value.ToInt32();
        /// <summary>
        /// 强转 long
        /// </summary>
        /// <param name="element">节点</param>
        public static implicit operator long(CData element) => (long)element?.Value.ToLong();
        /// <summary>
        /// 强转 long
        /// </summary>
        /// <param name="element">节点</param>
        public static implicit operator Guid(CData element) => (Guid)element?.Value.ToGuid();
        /// <summary>
        /// 强转CDATA
        /// </summary>
        /// <param name="val">字符串</param>
        public static implicit operator CData(string val) => new CData(val);
    }
}