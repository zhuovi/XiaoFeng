﻿using System;
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
    /// Xml属性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class XmlConverterAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 设置转换类型
        /// </summary>
        /// <param name="converterType">转换类型</param>
        public XmlConverterAttribute(Type converterType)
        {
            this.ConverterType = converterType;
        }
        /// <summary>
        /// 设置属性配置
        /// </summary>
        /// <param name="converterType">转换类型</param>
        /// <param name="converterParameters">属性</param>
        public XmlConverterAttribute(Type converterType, params object[] converterParameters)
        {
            this.ConverterType = converterType;
            this.ConverterParameters = converterParameters;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 转换类型
        /// </summary>
        public Type ConverterType { get; set; }
        /// <summary>
        /// 属性配置
        /// </summary>
        public object[] ConverterParameters { get; set; }
        #endregion
    }
}
