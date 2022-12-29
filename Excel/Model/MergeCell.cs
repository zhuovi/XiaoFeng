using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-10 18:06:35                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel.Model
{
    /// <summary>
    /// 合并单元格
    /// </summary>
    public class MergeCell
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MergeCell() { }
        /// <summary>
        /// 设置开始结束位置
        /// </summary>
        /// <param name="begin">开始位置</param>
        /// <param name="end">结束位置</param>
        public MergeCell(CellLocation begin, CellLocation end)
        {
            Begin = begin;
            End = end;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始位置
        /// </summary>
        public CellLocation Begin { get;private set; }
        /// <summary>
        /// 结束位置
        /// </summary>
        public CellLocation End { get; private set; }
        /// <summary>
        /// 总行数
        /// </summary>
        public int Rows => this.End.RowIndex - this.Begin.RowIndex + 1;
        /// <summary>
        /// 总列数
        /// </summary>
        public int Columns=>this.End.ColumnIndex - this.Begin.ColumnIndex + 1;
        #endregion

        #region 方法

        #endregion
    }
}