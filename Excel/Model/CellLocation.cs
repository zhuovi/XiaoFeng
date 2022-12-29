using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-10 15:54:17                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel.Model
{
    /// <summary>
    /// 位置
    /// </summary>
    public class CellLocation
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CellLocation() { }
        /// <summary>
        /// 位置名称
        /// </summary>
        /// <param name="location">名称 如 A1</param>
        public CellLocation(string location)
        {
            if (location.IsNotMatch(@"^[a-z]+\d+$")) return;
            var values = location.GetMatchs(@"^(?<a>[a-z]+)(?<b>\d+)$");
            if (values == null || !values.ContainsKey("a") || !values.ContainsKey("b")) return;
            this.RowIndex = values["b"].ToCast<int>();
            this.ColumnIndex = Common.GetColumnIndex(values["a"]);
        }
        /// <summary>
        /// 位置索引
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        public CellLocation(int rowIndex, int columnIndex)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 位置
        /// </summary>
        private string _Location;
        /// <summary>
        /// 位置
        /// </summary>
        public string Location
        {
            get
            {

                if (this._Location.IsNullOrEmpty())
                {
                    this._Location = Common.GetColumnName(this.ColumnIndex) + this.RowIndex.ToString();
                }
                return this._Location;
            }
        }
        /// <summary>
        /// 行索引
        /// </summary>
        public int RowIndex { get; private set; }
        /// <summary>
        /// 列索引
        /// </summary>
        public int ColumnIndex { get; private set; }
        #endregion

        #region 方法

        #endregion
    }
}