using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/22 14:23:26                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// 转化
    /// </summary>
    public static class XmlParser
    {
        #region 构造器
        

        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object Deserialize(String xml,Type type)
        {
            if (xml.IsNullOrEmpty()) return null;
            var doc = new XMLDocument();
            doc.LoadXml(xml);
            if (!doc.HasChildNodes) return null;
            var root = doc.DocumentElement;
            if (!root.HasChildNodes && !root.HasAttributes) return Activator.CreateInstance(type);
            return new XmlValueX(root).ParserValue(type);
        }
        
        #endregion
    }
}