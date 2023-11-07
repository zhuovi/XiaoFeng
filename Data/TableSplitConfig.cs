using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XiaoFeng.Config;
using XiaoFeng.Json;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-03-24 18:15:28                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 分表配置
    /// </summary>
    [ConfigFile("/Config/TableSplit.json", 0, "FAYELF-CONFIG-TABLESPLIT", ConfigFormat.Json)]
    public class TableSplitConfig : ConfigSets<TableSplitConfig>, ITableSplit
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TableSplitConfig()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建表SQL
        /// </summary>
        public string CreateTableSQL { get; set; }
        /// <summary>
        /// 新表名格式
        /// </summary>
        public string NewTableNameFormat { get; set; }
        /// <summary>
        /// 分表字段
        /// </summary>
        public string SplitField { get; set; }
        /// <summary>
        /// 所有分表
        /// </summary>
        public List<TableSplitData> TableNames { get; set; }
        /// <summary>
        /// 分表类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TableSplitType SplitType { get; set; }
        /// <summary>
        /// 间隔
        /// </summary>
        public uint Peroid { get; set; } = 1;
        #endregion

        #region 方法
        /// <summary>
        /// 是否存在表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public Boolean Exists(string tableName) => this.TableNames.Exists(a => a.Name.EqualsIgnoreCase(tableName));
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            var now = DateTime.Now;
            var startNow = new DateTime(now.Year, 1, 1);
            int Q;
            if (now.Month <= 3) Q = 1;
            else if (now.Month <= 6) Q = 2;
            else if (now.Month <= 9) Q = 3;
            else Q = 4;
            var wNow = new DateTime(now.Year, 1, 1);
            var wWeek = (int)wNow.DayOfWeek;
            var W = (int)Math.Ceiling(((now - startNow).TotalDays + wWeek) / 7.0);

            var lastTableName = this.TableNames.Last().Name;
            var nowTableName = lastTableName.SubString(this.Name.Length + "_FB_".Length);
            if (this.SplitType == TableSplitType.Year)
            {
                var years = nowTableName.ToCast<int>() + this.Peroid - 1;
                if (years >= now.Year) return lastTableName;
            }
            else if (this.SplitType == TableSplitType.Quarter)
            {
                var qyear = nowTableName.Substring(1, 4).ToCast<int>();
                var qdate = new DateTime(qyear, 1, 1);

                var qvalue = nowTableName.Substring(5, 1).ToCast<int>();
                qdate.AddMonths((int)((qvalue + this.Peroid - 1) * 3));
                if (qdate >= new DateTime(now.Year, now.Month, 1)) return lastTableName;
            }
            else if (this.SplitType == TableSplitType.Month)
            {
                var myear = nowTableName.Substring(0, 4).ToCast<int>();
                var mmonth = nowTableName.Substring(4, 2).ToCast<int>();
                var mdate = new DateTime(myear, mmonth, 1);
                mdate.AddMonths((int)(this.Peroid - 1));
                if (mdate >= new DateTime(now.Year, now.Month, 1)) return lastTableName;
            }
            else if (this.SplitType == TableSplitType.Week)
            {
                var wyear = nowTableName.Substring(1, 4).ToCast<int>();
                var wweek = nowTableName.Substring(5).ToCast<int>();
                var wdate = new DateTime(wyear, 1, 1);
                wdate.AddDays((double)((wweek + (int)this.Peroid - 1) * 7 - wdate.DayOfWeek));
                if (wdate >= new DateTime(now.Year, now.Month, now.Day)) return lastTableName;
            }
            else if (this.SplitType == TableSplitType.Day)
            {
                var dyear = nowTableName.Substring(4).ToCast<int>();
                var dmonth = nowTableName.Substring(4, 2).ToCast<int>();
                var dday = nowTableName.Substring(6).ToCast<int>();
                var ddate = new DateTime(dyear, dmonth, dday);
                ddate.AddDays(this.Peroid - 1);
                if (ddate >= new DateTime(now.Year, now.Month, now.Day)) return lastTableName;
            }
            else if (this.SplitType == TableSplitType.Hour)
            {
                var hyear = nowTableName.Substring(4).ToCast<int>();
                var hmonth = nowTableName.Substring(4, 2).ToCast<int>();
                var hday = nowTableName.Substring(6, 2).ToCast<int>();
                var hhour = nowTableName.Substring(8).ToCast<int>();
                var hdate = new DateTime(hyear, hmonth, hday, hhour, 0, 0);
                hdate.AddHours(this.Peroid - 1);
                if (hdate >= new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)) return lastTableName;
            }
            else if (this.SplitType == TableSplitType.AutoID)
            {

            }
            var dict = new Dictionary<string, string>
            {
                {"yyyy",now.ToString("yyyy") },
                {"MM", now.ToString("yyyyMM") },
                {"dd", now.ToString("yyyyMMdd") },
                {"Q", "Q"+ now.ToString("yyyy") + Q },
                {"W", "W"+ now.ToString("yyyy") + W },
                {"HH", now.ToString("yyyyMMddHH") },
                {"ID", "ID"+this.Peroid }
            };
            return this.NewTableNameFormat.format(dict);
        }
        /// <summary>
        /// 添加分表
        /// </summary>
        /// <param name="tableName">表名</param>
        public void AddTableSplit(string tableName)
        {
            if (tableName.IsNullOrEmpty() || tableName.IndexOf("_FB_") == -1) return;
            var tblName = tableName.SubString(this.Name.Length + "_FB_".Length);
            var data = new TableSplitData();
            data.Name = tableName;
            if (this.SplitType == TableSplitType.Year)
            {
                var years = tblName.ToCast<int>();
                var ydate = new DateTime(years, 1, 1);
                data.Begin = ydate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                ydate.AddYears((int)this.Peroid);
                data.End = ydate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else if (this.SplitType == TableSplitType.Quarter)
            {
                var qyear = tblName.Substring(1, 4).ToCast<int>();
                var qdate = new DateTime(qyear, 1, 1);
                var qvalue = tblName.Substring(5, 1).ToCast<int>();
                qdate.AddMonths(qvalue * 3);
                data.Begin = qdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                qdate.AddMonths((int)(this.Peroid * 3));
                data.End = qdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else if (this.SplitType == TableSplitType.Month)
            {
                var myear = tblName.Substring(0, 4).ToCast<int>();
                var mmonth = tblName.Substring(4, 2).ToCast<int>();
                var mdate = new DateTime(myear, mmonth, 1);
                data.Begin = mdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                mdate.AddMonths((int)this.Peroid);
                data.End = mdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else if (this.SplitType == TableSplitType.Week)
            {
                var wyear = tblName.Substring(1, 4).ToCast<int>();
                var wweek = tblName.Substring(5).ToCast<int>();
                var wdate = new DateTime(wyear, 1, 1);
                wdate.AddDays((double)(wweek * 7 - wdate.DayOfWeek));
                data.Begin = wdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                wdate.AddDays((int)this.Peroid * 7);
                data.End = wdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else if (this.SplitType == TableSplitType.Day)
            {
                var dyear = tblName.Substring(4).ToCast<int>();
                var dmonth = tblName.Substring(4, 2).ToCast<int>();
                var dday = tblName.Substring(6).ToCast<int>();
                var ddate = new DateTime(dyear, dmonth, dday);
                data.Begin = ddate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                ddate.AddDays(this.Peroid);
                data.End = ddate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else if (this.SplitType == TableSplitType.Hour)
            {
                var hyear = tblName.Substring(4).ToCast<int>();
                var hmonth = tblName.Substring(4, 2).ToCast<int>();
                var hday = tblName.Substring(6, 2).ToCast<int>();
                var hhour = tblName.Substring(8).ToCast<int>();
                var hdate = new DateTime(hyear, hmonth, hday, hhour, 0, 0);
                data.Begin = hdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                hdate.AddHours(this.Peroid);
                data.End = hdate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else if (this.SplitType == TableSplitType.AutoID)
            {

            }
            this.TableNames.Add(data);
        }
        #endregion
    }
    /// <summary>
    /// 分表类型
    /// </summary>
    public enum TableSplitType
    {
        /// <summary>
        /// 按自增长ID分
        /// </summary>
        [Description("按自增长ID分")]
        AutoID = 0,
        /// <summary>
        /// 按年分
        /// </summary>
        [Description("按年分")]
        Year = 1,
        /// <summary>
        /// 按季度分
        /// </summary>
        [Description("按季度分")]
        Quarter = 2,
        /// <summary>
        /// 按月分
        /// </summary>
        [Description("按年分")]
        Month = 3,
        /// <summary>
        /// 按周分
        /// </summary>
        [Description("按周分")]
        Week = 4,
        /// <summary>
        /// 按天分
        /// </summary>
        [Description("按天分")]
        Day = 5,
        /// <summary>
        /// 按小时分
        /// </summary>
        [Description("按小时分")]
        Hour = 6
    }
    /// <summary>
    /// 接口
    /// </summary>
    public interface ITableSplit
    {
        /// <summary>
        /// 表名
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 创建表SQL
        /// </summary>
        string CreateTableSQL { get; set; }
        /// <summary>
        /// 新表名格式
        /// </summary>
        string NewTableNameFormat { get; set; }
        /// <summary>
        /// 分表字段
        /// </summary>
        string SplitField { get; set; }
        /// <summary>
        /// 所有分表
        /// </summary>
        List<TableSplitData> TableNames { get; set; }
        /// <summary>
        /// 分表类型
        /// </summary>
        TableSplitType SplitType { get; set; }
        /// <summary>
        /// 间隔
        /// </summary>
        uint Peroid { get; set; }
        /// <summary>
        /// 是否存在表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        Boolean Exists(string tableName);
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        string GetTableName();
        /// <summary>
        /// 添加分表
        /// </summary>
        /// <param name="tableName">表名</param>
        void AddTableSplit(string tableName);
    }
    /// <summary>
    /// 分表数据
    /// </summary>
    public class TableSplitData
    {
        /// <summary>
        /// 分表名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分表开始
        /// </summary>
        public string Begin { get; set; }
        /// <summary>
        /// 分表结束
        /// </summary>
        public string End { get; set; }
    }
}