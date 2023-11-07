using System;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 15:44:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// MemcachedValue
    /// </summary>
    public class MemcachedValue
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedValue() { }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="valueType">值类型</param>
        /// <param name="value">值</param>
        /// <param name="unique">唯一标识</param>
        public MemcachedValue(string key, ValueType valueType, object value, UInt64 unique)
        {
            this.Key = key;
            this.Type = valueType;
            this.Value = value;
            this.Unique = unique;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 类型
        /// </summary>
        public ValueType Type { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public UInt64 Unique { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}