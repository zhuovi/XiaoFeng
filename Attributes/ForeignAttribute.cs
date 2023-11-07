using System;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-04 10:44:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 外键特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ForeignAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ForeignAttribute()
        {

        }
        /// <summary>
        /// 设置类型
        /// </summary>
        /// <param name="entityType"></param>
        public ForeignAttribute(Type entityType)
        {
            this.Type = entityType;
        }
        /// <summary>
        /// 设置外键
        /// </summary>
        /// <param name="key">KEY</param>
        /// <param name="field">当前字段</param>
        /// <param name="format">格式 key='${field}'</param>
        /// <param name="type">外键表类型</param>
        public ForeignAttribute(string key, string field, string format, Type type = null)
        {
            this.Key = key;
            this.Field = field;
            this.Format = format.IsNullOrEmpty() ? $"{field}='${{{key}}}'" : format;
            this.Type = type;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 外键KEY
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 当前字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 条件格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 表类型
        /// </summary>
        public Type Type { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}