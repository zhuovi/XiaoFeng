using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-16 10:12:20                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel.Base
{
    /// <summary>
    /// 内容类型
    /// </summary>
    [XmlRoot("Types")]
    public class ContentTypes
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ContentTypes()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 命名空间
        /// </summary>
        [XmlAttribute]
        public string Xmlns { get; set; }
        /// <summary>
        /// 后缀类型
        /// </summary>
        [XmlArrayItem("Default")]
        public List<DefaultModel> Default { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        [XmlArrayItem("Override")]
        public List<OverrideModel> Override { get; set; }
        #endregion

        #region 方法

        #endregion
    }
    /// <summary>
    /// 默认类型
    /// </summary>
    public class DefaultModel
    {
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }
    }
    /// <summary>
    /// 文件路径
    /// </summary>
    public class OverrideModel
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string PartName { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }
    }
}