using System;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-03-15 11:39:48                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 枚举名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
    public class EnumNameAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public EnumNameAttribute()
        {

        }
        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        public EnumNameAttribute(string name)
        {
            this.Name = name;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}