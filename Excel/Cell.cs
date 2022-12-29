using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Excel.Model;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 11:48:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 数据列
    /// </summary>
    public class Cell
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Cell()
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="format">数值格式</param>
        public Cell(string value, NumberFormat format = NumberFormat.General)
        {
            Value = new CellValue(value,format);
            Format = format;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 值
        /// </summary>
        public CellValue Value { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        public NumberFormat Format { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        public Row Row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public Column Column { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public CellLocation Location { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}