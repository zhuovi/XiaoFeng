using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-10 10:26:30                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Excel
{
    /// <summary>
    /// 值
    /// </summary>
    public class CellValue
    {
        #region 构造器
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="numberFormat">值格式</param>
        public CellValue(string value, NumberFormat numberFormat = NumberFormat.General)
        {
            this.Value = value;
            this.Format = numberFormat;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="baseYear">基础年</param>
        /// <param name="numberFormat">值格式</param>
        public CellValue(string value, int baseYear, NumberFormat numberFormat = NumberFormat.Date)
        {
            this.Value = value;
            this.Format = numberFormat;
            this.BaseYear = BaseYear;
        }
        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="dateTime">日期</param>
        public CellValue(DateTime dateTime) : this(ToCellFormat(dateTime)) { }
        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="dateTimeOffset">日期</param>
        public CellValue(DateTimeOffset dateTimeOffset) : this(ToCellFormat(dateTimeOffset)) { }
        
        #endregion

        #region 属性
        /// <summary>
        /// 日期格式
        /// </summary>
        private const string DateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";
        /// <summary>
        /// 日期格式
        /// </summary>
        private const string DateTimeOffsetFormatString = DateTimeFormatString + "zzz";
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        public NumberFormat Format { get; set; } = NumberFormat.General;
        /// <summary>
        /// 基础年
        /// </summary>
        private int BaseYear { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 日期转换成字符串
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        private static string ToCellFormat(DateTime dateTime)
            => dateTime.ToString(DateTimeFormatString, CultureInfo.InvariantCulture);
        /// <summary>
        /// 日期转换成字符串
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        private static string ToCellFormat(DateTimeOffset dateTime)
            => dateTime.ToString(DateTimeOffsetFormatString, CultureInfo.InvariantCulture);
        /// <summary>
        /// 重写转换字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Format == NumberFormat.Date)
            {
                const int MYSTERIOUS_CONSTANT = 2;
                var convertedDate = DateTime.Parse(string.Format("{0}-01-01", this.BaseYear));
                double daysToAdd = this.Value.ToCast<double>() - MYSTERIOUS_CONSTANT;
                convertedDate = convertedDate.AddDays(daysToAdd);
                return convertedDate.ToShortDateString();
            }
            else if (this.Format == NumberFormat.Time)
            {
                var time = double.Parse((string)this.Value, CultureInfo.GetCultureInfo("en-us"));
                var second = 1 / 86400d;
                var seconds = time / second;
                var span = TimeSpan.FromSeconds(seconds);
                return span.ToString();
            }
            return base.ToString();
        }
        #endregion
    }
}