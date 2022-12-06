using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
namespace XiaoFeng.Xml
{
    /// <summary>
    /// Xml管理
    /// </summary>
    public class XMLDocument : XmlDocument
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XMLDocument() : base() { }
        #endregion

        #region 方法
        /// <summary>
        /// 加载XML文档
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public override void Load(string fileName)
        {
            if (fileName.IsNullOrEmpty()) return;
           
            if (!FileHelper.Exists(fileName, FileAttribute.File)) return;
            //2022-07-12 为了兼容linux中{*}根目录，改到下面 赋值
            fileName = fileName.GetBasePath();
            base.Load(fileName);
        }
        /// <summary>
        /// 根据xpath获取节点
        /// </summary>
        /// <param name="xpath">xpath</param>
        /// <param name="attrName">属性名</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public virtual XmlNode SelectSingleNode(string xpath = "", string attrName = "", string attrValue = "")
        {
            if (xpath.IsNullOrEmpty()) return this.DocumentElement as XmlNode;
            else
            {
                if (attrName.IsNullOrEmpty())
                    return base.SelectSingleNode(xpath);
                else
                    return base.SelectSingleNode(xpath + "[translate(@" + attrName + ", 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='" + attrValue.ToLower() + "']");
            }
        }
        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        public virtual XmlAttribute CreateAttribute(string name, object value)
        {
            var attr = base.CreateAttribute(name);
            attr.Value = value.ToString();
            return attr;
        }
        /// <summary>
        /// 创建结点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="text">内容</param>
        /// <returns></returns>
        public virtual XmlElement CreateElement(string name, object text)
        {
            var element = base.CreateElement(name);
            element.InnerText = text?.ToString();
            return element;
        }
        #endregion
    }
}