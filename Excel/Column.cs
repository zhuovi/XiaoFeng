using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 21:44:14                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 数据列
    /// </summary>
    public class Column
    {
        #region 构造器
        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="index">列索引</param>
        /// <param name="hidden">是否隐藏</param>
        public Column(int index, Boolean hidden = false)
        {
            this.Index=index;
            this.Hidden=hidden;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 数据表
        /// </summary>
        public Sheet Sheet { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public Boolean Hidden { get; set; }
        /// <summary>
        /// 单元格
        /// </summary>
        private List<Cell> CurrentColumnCells { get; set; } = new List<Cell>();
        #endregion

        #region 方法
        /// <summary>
        /// 当前列所有单元格
        /// </summary>
        public IEnumerable<Cell> Cells => this.CurrentColumnCells.Where(c => this.Sheet.Workbook.Option.IncludeHiddenRow || c.Row.Hidden == false).OrderBy(c => c.Row.Index);
        /// <summary>
        /// 添加单元格
        /// </summary>
        /// <param name="cell">单元格</param>
        public void AddCell(Cell cell)
        {
            var Cell = (from c in this.CurrentColumnCells where c.Row.Index == cell.Row.Index select c).SingleOrDefault();
            if (Cell == null)
            {
                cell.Column = this;
                this.CurrentColumnCells.Add(cell);
            }
        }
        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <returns></returns>
        public Cell Cell(int rowIndex) => this.CurrentColumnCells.SingleOrDefault(c => c.Row.Index == rowIndex && (this.Sheet.Workbook.Option.IncludeHiddenRow ||c.Row.Hidden == false));
        #endregion
    }
}