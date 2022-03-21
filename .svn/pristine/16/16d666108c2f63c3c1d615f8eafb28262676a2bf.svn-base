using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/23 17:33:52                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Xml
{
    /// <summary>
    /// 节点路径属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XmlElementPathAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XmlElementPathAttribute() { }
        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="path">路径</param>
        public XmlElementPathAttribute(string path)
        {
            this.Path = path;
            if (path.IsNotNullOrEmpty())
            {
                this.Paths = path.Split(new char[] { '/', ':', '>' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 路径集合
        /// </summary>
        public string[] Paths { get; private set; }
        #endregion

        #region 方法

        #endregion
    }
}