using System.ComponentModel;
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
    public class ContentTypeMapping : ConfigSets<ContentTypeMapping>
    {
        /// <summary>
        /// 后缀名
        /// </summary>
        [XmlCData]
        [XmlElement("Ext")]
        [Description("后缀名")]
        public string Ext { get; set; }
        /// <summary>
        /// Mime类型
        /// </summary>
        [XmlCData]
        [XmlElement("Mime")]
        [Description("Mime类型")]
        public string Mime { get; set; }
    }
}