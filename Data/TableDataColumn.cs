using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-11-17 15:27:43                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// Version : 1.0.0
    /// Create Time : 2017/11/17 15:27:43
    /// Update Time : 2017/11/17 15:27:43
    /// </summary>
    public class TableDataColumn
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        TableDataColumn() { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~TableDataColumn() { }
        #endregion
    }

    #region 字段数据
        /// <summary>
        /// 字段数据
        /// </summary>
    public class DataField
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataField() { this.IsWhere = false; this.Encrypt = 0; this.Format = ""; }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="Name">字段名</param>
        /// <param name="Value">字段值</param>
        public DataField(string Name, object Value) : this() { this.Name = Name; this.Value = Value; }
        #endregion

        #region 属性
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 模式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 是否是条件
        /// </summary>
        public Boolean IsWhere { get; set; }
        /// <summary>
        /// 是否加密 0 不加密 1可逆加密 2不可逆加密
        /// </summary>
        public int Encrypt { get; set; }
        #endregion
    }
    #endregion

    #region 结构体
    /// <summary>
    /// 列属性
    /// </summary>
    public struct DataColumns
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public Int64 SortID;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 是否是自增长
        /// </summary>
        public Boolean IsIdentity;
        /// <summary>
        /// 是否主键
        /// </summary>
        public Boolean PrimaryKey;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type;
        /// <summary>
        /// 长度
        /// </summary>
        public Int64 Length;
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Digits;
        /// <summary>
        /// 是否为空
        /// </summary>
        public Boolean IsNull;
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue;
        /// <summary>
        /// 说明
        /// </summary>
        public string Description;
        /// <summary>
        /// 是否索引
        /// </summary>
        public Boolean IsIndex;
        /// <summary>
        /// 是否唯一
        /// </summary>
        public Boolean IsUnique;
        /// <summary>
        /// 自增长步长
        /// </summary>
        public int AutoIncrementStep;
        /// <summary>
        /// 自增长种子
        /// </summary>
        public long AutoIncrementSeed;
    }
    #endregion
}