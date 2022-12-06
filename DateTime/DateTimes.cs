using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-09-18 00:51:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// DateTime区间操作类
    /// </summary>
    public class DateTimes : IFormattable, IComparable<DateTimes>, IEquatable<DateTimes>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DateTimes() { }
        /// <summary>
        /// 设置数据 两个时间中间用' - '隔开
        /// </summary>
        /// <param name="v">字符串格式</param>
        public DateTimes(string v)
        {
            if (v.IsNullOrEmpty() || !v.IsMatch(Separator.ToRegexEscape())) return;
            var vals = v.SplitPattern(Separator.ToRegexEscape());
            if (vals.Length != 2) return;
            if (!vals[0].IsDateOrTime() || !vals[1].IsDateOrTime()) return;
            this.Start = vals[0].ToCast<DateTime>();
            this.End = vals[1].ToCast<DateTime>();
            if (this.Start > this.End)
            {
                var _ = this.Start;
                this.Start = this.End;
                this.End = this.Start;
            }
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="separator">分隔符</param>
        public DateTimes(DateTime start, DateTime end, string separator = " - ")
        {
            this.Start = start; this.End = end; this.Separator = separator;
            if (this.Start > this.End)
            {
                var _ = this.Start;
                this.Start = this.End;
                this.End = this.Start;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 分隔符
        /// </summary>
        private readonly string Separator = " - ";
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// 相差时间
        /// </summary>
        public TimeSpan? TimeSpan { get { return this.IsEmpty() ? new TimeSpan(0) : (this.End - this.Start); } }
        /// <summary>
        /// 总月数
        /// </summary>
        public double TotalMonths
        {
            get
            {
                if (this.IsEmpty()) return 0;
                if (this.Start.IsNullOrEmpty() || this.End.IsNullOrEmpty()) return 0;
                var EndMonth = this.End.Value.Year * 12 + this.End.Value.Month;
                var StartMonth = this.Start.Value.Year * 12 + this.Start.Value.Month;
                if (this.End.IsNullOrEmpty())
                    return -StartMonth;
                else if (this.Start.IsNullOrEmpty())
                    return EndMonth;
                else
                    return StartMonth;
            }
        }
        /// <summary>
        /// 总天数
        /// </summary>
        public double TotalDays { get { return this.TimeSpan.Value.TotalDays; } }
        /// <summary>
        /// 总小时
        /// </summary>
        public double TotalHours { get { return this.TimeSpan.Value.TotalHours; } }
        /// <summary>
        /// 总分钟
        /// </summary>
        public double TotalMinutes { get { return this.TimeSpan.Value.TotalMinutes; } }
        /// <summary>
        /// 总秒
        /// </summary>
        public double TotalSeconds { get { return this.TimeSpan.Value.TotalSeconds; } }
        /// <summary>
        /// 总毫秒
        /// </summary>
        public double TotalMilliseconds { get { return this.TimeSpan.Value.TotalMilliseconds; } }
        #endregion

        #region 方法

        #region 验证值格式
        /// <summary>
        /// 验证值格式
        /// </summary>
        /// <param name="v">值</param>
        /// <returns></returns>
        public static Boolean IsDateTimes(string v)
        {
            var Separator = new DateTimes().Separator;
            if (v.IsNullOrEmpty() || !v.IsMatch(Separator.ToRegexEscape())) return false;
            var vals = v.SplitPattern(Separator.ToRegexEscape());
            if (vals.Length != 2) return false;
            if (!vals[0].IsDateOrTime() || !vals[1].IsDateOrTime()) return false;
            return true;
        }
        #endregion

        #region 转换
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="v">字符串</param>
        /// <returns></returns>
        public static DateTimes Parse(string v)
        {
            if (!IsDateTimes(v)) return null;   
            return new DateTimes(v);
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="v">字符串格式</param>
        public static explicit operator DateTimes(string v)
        {
            if (!IsDateTimes(v)) return null;
            return new DateTimes(v);
        }
        #endregion

        #region 转字符串
        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="formatProvider">格式机制</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.Start?.ToString(format, formatProvider) + Separator + this.End?.ToString(format, formatProvider);
        }
        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return this.Start?.ToString(format) + Separator + this.End?.ToString(format);
        }
        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">对象</param>
        /// <returns></returns>
        public int CompareTo(DateTimes other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other">对象</param>
        /// <returns></returns>
        public bool Equals(DateTimes other)
        {
            return this.CompareTo(other) == 0;
        }
        #endregion

        #region 强制转换
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator double(DateTimes v)
        {
            if (!v.End.HasValue)
            {
                if (!v.Start.HasValue) return 0;
                else return v.Start.Value.ToTimeStamps();
            }
            else
            {
                if (!v.Start.HasValue)
                    return v.End.Value.ToTimeStamps();
                else
                    return (v.End - v.Start).Value.TotalMilliseconds;
            }
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator int(DateTimes v)
        {
            if (!v.End.HasValue)
            {
                if (!v.Start.HasValue) return 0;
                else return v.Start.Value.ToTimeStamp();
            }
            else
            {
                if (!v.Start.HasValue)
                    return v.End.Value.ToTimeStamp();
                else
                    return (int)(v.End - v.Start).Value.TotalSeconds;
            }
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator String(DateTimes v)
        {
            return v.ToString();
        }
        #endregion

        #region 是否为空
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty() => this.Start == null || this.End == null;
        #endregion
        #endregion
    }
}