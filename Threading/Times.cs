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
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 时间
    /// </summary>
    public class Times : IComparable, IComparable<Times>, IFormattable, IEqualityComparer<Times>, IEquatable<Times>
    {
        #region 构造器  设置时间
        /// <summary>
        /// 设置时间
        /// </summary>
        public Times() : this(DateTime.Now) { }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="timeString">时间串 格式必须为 1:2:3或01:02:03</param>
        public Times(string timeString)
        {
            if (timeString.IsMatch(@"^\d{1,2}:\d{1,2}:\d{1,2}$"))
            {
                var time = timeString.GetMatchs(@"^(?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2})$");
                if (time.Count <= 3) return;
                this.Hour = time["hour"].ToCast<int>();
                this.Minute = time["minute"].ToCast<int>();
                this.Second = time["second"].ToCast<int>();
            }
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="hour">小时</param>
        /// <param name="minute">分钟</param>
        /// <param name="second">秒</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="week">星期</param>
        public Times(int? hour, int minute, int second, int? month = null, int? day = null, int? week = null)
        {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            if (month.HasValue)
                this.Month = month;
            if (day.HasValue)
                this.Day = day;
            if (week.HasValue)
                this.Week = week;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="dateTime">全时间</param>
        public Times(DateTime dateTime)
        {
            this.Month = dateTime.Month;
            this.Day = dateTime.Day;
            this.Week = (int)dateTime.DayOfWeek;
            this.Hour = dateTime.Hour;
            this.Minute = dateTime.Minute;
            this.Second = dateTime.Second;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time">时间</param>
        public Times(Times time)
        {
            this.Month = time.Month;
            this.Day = time.Day;
            this.Week = time.Week;
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time">时间</param>
        public Times(Time time)
        {
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
        }
        #endregion;

        #region 属性
        /// <summary>
        /// 每年月天数
        /// </summary>
        private readonly int[] Days = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        /// <summary>
        /// 月份
        /// </summary>
        private int? _Month;
        /// <summary>
        /// 月份
        /// </summary>
        public int? Month
        {
            get { return this._Month; }
            set
            {
                this._Month = value;
                if (this._Month > 12)
                    this._Month = (this._Month - 1) % 12 + 1;
                else if (this._Month < -12)
                    this._Month = this._Month % 12 + 12 + 1;
                else if (this._Month < 0)
                    this._Month += 12 + 1;
            }
        }
        /// <summary>
        /// 日
        /// </summary>
        public int? _Day;
        /// <summary>
        /// 日
        /// </summary>
        public int? Day
        {
            get { return this._Day; }
            set
            {
                var now = DateTime.Now;
                this._Day = value;
                if (this.Month.HasValue)
                {
                    this.Days[1] = DateTime.DaysInMonth(now.Year, 2);
                    if (this._Day > 0)
                    {
                        while (this._Day > this.Days[this.Month.Value - 1])
                        {
                            this.Month += 1;
                            this._Day -= this.Days[this.Month.Value - 1];
                        }
                    }
                    else if (this._Day < -this.Days[this.Month.Value - 1])
                        this._Day = this._Day % this.Days[this.Month.Value - 1] + 1 + this.Days[this.Month.Value - 1];
                    else if (this._Day < 0)
                        this._Day += this.Days[this.Month.Value - 1] + 1;
                }
                else
                {
                    var days = this.Days[now.Month - 1];
                    if (this._Day > days)
                        this._Day = (this._Day - 1) % days + 1;
                    else if (this._Day < -days)
                        this._Day = this._Day % days + 1 + days;
                    else if (this._Day < 0)
                        this._Day += days + 1;
                }
            }
        }
        /// <summary>
        /// 周几
        /// </summary>
        public int? _Week;
        /// <summary>
        /// 周几
        /// </summary>
        public int? Week
        {
            get { return this._Week; }
            set
            {
                this._Week = value;
                if (this._Week > 6)
                    this._Week %= 7;
                else if (this._Week < -7)
                    this._Week = this._Week % 7 + 7;
                else if (this._Week < 0)
                    this._Week += 7;
            }
        }
        /// <summary>
        /// 时
        /// </summary>
        private int? _Hour;
        /// <summary>
        /// 时
        /// </summary>
        public int? Hour {
            get { return this._Hour; }
            set
            {
                this._Hour = value;
                if (this._Hour >= 24)
                {
                    if (this.Day.HasValue) this.Day += this._Hour / 24;
                    this._Hour %= 24;
                }
                else if (this._Hour < -24)
                {
                    this._Hour = this._Hour % 24 + 24;
                }
                else if (this._Hour < 0)
                    this._Hour += 24;
            }
        }
        /// <summary>
        /// 分
        /// </summary>
        private int _Minute = 0;
        /// <summary>
        /// 分
        /// </summary>
        public int Minute {
            get { return this._Minute; }
            set
            {
                this._Minute = value;
                if (this._Minute >= 60)
                {
                    if (this.Hour.HasValue) this.Hour += this._Minute / 60;
                    this._Minute %= 60;
                }
                else if (this._Minute < -60)
                {
                    this._Minute = this._Second % 60 + 60;
                }
                else if (this._Minute < 0)
                    this._Minute += 60;
            }
        }
        /// <summary>
        /// 秒
        /// </summary>
        private int _Second = 0;
        /// <summary>
        /// 秒
        /// </summary>
        public int Second {
            get { return this._Second; }
            set
            {
                this._Second = value;
                if (this._Second >= 60)
                {
                    this.Minute += this._Second / 60;
                    this._Second %= 60;
                }
                else if (this._Second < -60)
                {
                    this._Second = this._Second % 60 + 60;
                }
                else if (this._Second < 0)
                    this._Second += 60;
            }
        }
        /// <summary>
        /// 总秒数
        /// </summary>
        public int TotalSeconds { get { return this.Hour.GetValueOrDefault() * 60 * 60 + this.Minute * 60 + this.Second; } }
        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public int CompareTo(object time)
        {
            if (time == null || !(time is Times)) return -1;
            return this.CompareTo(time as Times);
        }
        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public int CompareTo(Times time)
        {
            if (time == null) return -1;
            var mc = this.Month.GetValueOrDefault().CompareTo(time.Month.GetValueOrDefault());
            if (mc != 0) return mc;
            var dc = this.Day.GetValueOrDefault().CompareTo(time.Day.GetValueOrDefault());
            if (dc != 0) return dc;
            var wc = this.Week.GetValueOrDefault().CompareTo(time.Week.GetValueOrDefault());
            if (wc != 0) return wc;
            return this.TotalSeconds.CompareTo(time.TotalSeconds);
        }
        /// <summary>
        /// 比较两个值
        /// </summary>
        /// <param name="x">第一个时间</param>
        /// <param name="y">第二个时间</param>
        /// <returns></returns>
        public bool Equals(Times x, Times y)
        {
            if (x == null)
                return y == null;
            else
                return y != null &&
                    x.Month == y.Month &&
                    x.Day == y.Day &&
                    x.Week == y.Week &&
                    x.TotalSeconds.Equals(y.TotalSeconds);
        }
        /// <summary>
        /// 比较两个值
        /// </summary>
        /// <param name="time">第二个时间</param>
        /// <returns></returns>
        public bool Equals(Times time)
        {
            return this.Equals(this, time);
        }
        /// <summary>
        /// 当前实例的哈希代码
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public int GetHashCode(Times time)
        {
            return time.GetHashCode();
        }

        #region 转换字符串
        /// <summary>
        /// 转换字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{0}:{1}:{2}".format(this.Hour.GetValueOrDefault().ToString().PadLeft(2, '0'),
                this.Minute.ToString().PadLeft(2, '0'),
                this.Second.ToString().PadLeft(2, '0'));
        }
        /// <summary>
        /// 转换字符串
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="formatProvider">格式驱动</param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var now = DateTime.Now;
            var that = new DateTime(now.Year, this.Month ?? now.Month, this.Day ?? now.Day, this.Hour.GetValueOrDefault(), this.Minute, this.Second);
            return that.ToString(format, formatProvider);
        }
        #endregion

        #endregion

        #region 添加月
        /// <summary>
        /// 添加月
        /// </summary>
        /// <param name="month">几个月</param>
        public void AddMonth(int month)
        {
            this.Month += month;
        }
        #endregion

        #region 添加日
        /// <summary>
        /// 添加日
        /// </summary>
        /// <param name="day">几日</param>
        public void AddDay(int day)
        {
            this.Day += day;
        }
        #endregion

        #region 添加周
        /// <summary>
        /// 添加周
        /// </summary>
        /// <param name="week">几周</param>
        public void AddWeek(int week)
        {
            this.Week += week;
        }
        #endregion

        #region 添加小时
        /// <summary>
        /// 添加小时
        /// </summary>
        /// <param name="Hours">几小时</param>
        public void AddHours(int Hours)
        {
            this.Hour += Hours;
        }
        #endregion

        #region 添加分钟
        /// <summary>
        /// 添加分钟
        /// </summary>
        /// <param name="Minutes">几分</param>
        public void AddMinutes(int Minutes)
        {
            this.Minute += Minutes;
        }
        #endregion

        #region 添加秒
        /// <summary>
        /// 添加秒
        /// </summary>
        /// <param name="Seconds">几秒</param>
        public void AddSeconds (int Seconds)
        {
            this.Second += Seconds;
        }
        #endregion


        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Time(Times v) => new Time(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Times(string v) => new Times(v);
    }
}