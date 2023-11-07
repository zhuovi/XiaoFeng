using System;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-01-13 13:55:26                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 数据库视图模型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class ViewAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ViewAttribute() { }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="definition">内容</param>
        public ViewAttribute(string name, string definition)
        {
            this.Name = name;
            this.Definition = definition;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 视图名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 视图内容
        /// </summary>
        public string Definition { get; set; }
        #endregion
    }
}