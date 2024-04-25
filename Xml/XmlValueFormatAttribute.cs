using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-24 14:40:34                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// Xml节点格式
    /// </summary>
    public class XmlValueFormatAttribute:Attribute
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public XmlValueFormatAttribute() { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="format">节点格式</param>
        public XmlValueFormatAttribute(string format) => this.Format = format;
        #endregion

        #region 属性
        /// <summary>
        /// 节点格式
        /// </summary>
        public string Format { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}