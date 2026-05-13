using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-05-13 14:49:10                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 枚举扩展属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All,AllowMultiple =true,Inherited =true)]
    public class EnumAttribute:Attribute
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public EnumAttribute() { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="description">说明</param>
        public EnumAttribute(string name, object value, string description)
        {
            this.Name = name;
            this.Value = value;
            this.Description = description;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}