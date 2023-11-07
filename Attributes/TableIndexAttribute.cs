using System;
using System.Collections.Generic;
using XiaoFeng.Model;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-05-19 15:21:02                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 索引属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class TableIndexAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TableIndexAttribute()
        {

        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="tbName">表名</param>
        /// <param name="name">索引名称</param>
        /// <param name="tableIndexType">索引类型</param>
        /// <param name="primary">是否是主建</param>
        /// <param name="keys">包含KEY</param>
        public TableIndexAttribute(string tbName, string name, TableIndexType tableIndexType, Boolean primary, params string[] keys)
        {
            this.TableName = tbName;
            Name = name;
            TableIndexType = tableIndexType;
            this.Primary = primary;
            Keys = keys.ToList<string>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 索引名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表索引类型
        /// </summary>
        public TableIndexType TableIndexType { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public Boolean Primary { get; set; }
        /// <summary>
        /// 包含Key  字段名称,字段排序，索引类型
        /// </summary>
        public List<string> Keys { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}