using System;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-10-27 10:55:46                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Json
{
    /// <summary>
    /// json 节点
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class JsonElementAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JsonElementAttribute()
        {

        }
        /// <summary>
        /// 设置节点名称
        /// </summary>
        /// <param name="name">节点名称</param>
        public JsonElementAttribute(string name) => this.Name = name;
        #endregion

        #region 属性
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}