using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-04 11:48:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 数据行
    /// </summary>
    public class Row
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Row() { }
        /// <summary>
        /// 设置行数据
        /// </summary>
        /// <param name="index">行索引</param>
        /// <param name="height">行高</param>
        /// <param name="minColumnIndex">最小列索引</param>
        /// <param name="maxColumnIndex">最大列索引</param>
        /// <param name="hidden">是否隐藏</param>
        public Row(int index,double height,int minColumnIndex,int maxColumnIndex, Boolean hidden)
        {
            this.Index = index;
            this.Height = height;
            this.MinColumnIndex = minColumnIndex;
            this.MaxColumnIndex = maxColumnIndex;
            this.Hidden = hidden;
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
        /// 行高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 最小列
        /// </summary>
        public int MinColumnIndex { get; set; }
        /// <summary>
        /// 最大列
        /// </summary>
        public int MaxColumnIndex { get; set; }
        /// <summary>
        /// 当前行内所有单元格
        /// </summary>
        private List<Cell> CurrentRowCells { get; set; } = new List<Cell>();
        #endregion

        #region 方法
        /// <summary>
        /// 当前行单元格集
        /// </summary>
        public IEnumerable<Cell> Cells
        {
            get
            {
                if (this.CurrentRowCells == null || this.CurrentRowCells.Count == 0) return null;
                return this.Sheet.Workbook.Option.IncludeHiddenColumn?this.CurrentRowCells.OrderBy(c => c.Column.Index): this.CurrentRowCells.Where(a => a.Column.Hidden == false).OrderBy(c => c.Column.Index);
            }
        }

        /// <summary>
        /// 添加单元格
        /// </summary>
        /// <param name="cell">单元格</param>
        public void AddCell(Cell cell)
        {
            var Cell = (from c in this.CurrentRowCells where c.Column.Index == cell.Column.Index select c).SingleOrDefault();
            if (Cell == null)
            {
                cell.Row = this;
                this.CurrentRowCells.Add(cell);
            }
        }
        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="columnIndex">列索引</param>
        /// <returns></returns>
        public Cell Cell(int columnIndex) => this.CurrentRowCells.SingleOrDefault(c => c.Column.Index == columnIndex && (this.Sheet.Workbook.Option.IncludeHiddenColumn || c.Column.Hidden == false));
        #endregion
    }
}