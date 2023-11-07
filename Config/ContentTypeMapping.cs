using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using XiaoFeng.Xml;
/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020/6/17 18:32:20                *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 文件MIME
    /// </summary>
    [ConfigFile("Config/ContentTypeMapping.xml", 0, "FAYELF-CONFIG-CONTENTTYPE-MAPPING", ConfigFormat.Xml)]
    [XmlRoot("Root")]
    public class ContentTypeMapping : ConfigSet<ContentTypeMapping>
    {
        /// <summary>
        /// 内容类型
        /// </summary>
        [XmlArrayItem("Mime")]
        public List<Mime> Mimes { get; set; }
    }
    /// <summary>
    /// 内容类型
    /// </summary>
    public class Mime
    {
        /// <summary>
        /// 后缀名
        /// </summary>
        [XmlAttribute("Ext")]
        public string Ext { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [XmlCData]
        [XmlText]
        public string Value { get; set; }
    }
}