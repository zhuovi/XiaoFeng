using System;
namespace XiaoFeng
{
    /****************************************************************
    *  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
    *  Author : jacky                                               *
    *  QQ : 7092734                                                 *
    *  Email : jacky@fayelf.com                                     *
    *  Site : www.fayelf.com                                        *
    *  Create Time : 2017-12-08 10:17:43                            *
    *  Version : v 1.0.0                                            *
    *  CLR Version : 4.0.30319.42000                                *
    *****************************************************************/
    /// <summary>
    /// 数据库表属性
    /// Verstion : 1.1.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public sealed class TableAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TableAttribute() { }
        /// <summary>
        /// 设置表属性
        /// </summary>
        /// <param name="name">表名</param>
        /// <param name="description">表说明</param>
        /// <param name="primaryKey">表主键</param>
        /// <param name="modelType">model类型</param>
        /// <param name="connName">数据库连接串</param>
        /// <param name="connIndex">数据库索引</param>
        public TableAttribute(string name, string description = "", string primaryKey = "", ModelType modelType = 0, string connName = "", int connIndex = 0)
        {
            this.Name = name;
            this.Description = description;
            this.PrimaryKey = primaryKey;
            this.ConnName = connName;
            this.ModelType = modelType == 0 ? name.IsMatch(@"^VIEW_") ? ModelType.View : ModelType.Table : modelType;
            this.ConnIndex = connIndex;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; } = "ID";
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnName { get; set; }
        /// <summary>
        /// 连接索引 默认 为0
        /// </summary>
        public int ConnIndex { get; set; } = 0;
        /// <summary>
        /// Model类型
        /// </summary>
        public ModelType ModelType { get; set; } = ModelType.Table;
        #endregion
    }
}