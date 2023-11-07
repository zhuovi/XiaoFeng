using System;
using System.Collections.Generic;
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
    public class Time : IComparable, IComparable<Time>, IFormattable, IEqualityComparer<Time>, IEquatable<Time>
    {
        #region 构造器  设置时间
        /// <summary>
        /// 设置时间
        /// </summary>
        public Time() : this(DateTime.Now) { }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="timeString">时间串 格式必须为 1:2:3或01:02:03</param>
        public Time(string timeString)
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
        public Time(int? hour, int minute, int second)
        {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="dateTime">全时间</param>
        public Time(DateTime dateTime)
        {
            this.Hour = dateTime.Hour;
            this.Minute = dateTime.Minute;
            this.Second = dateTime.Second;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time">时间</param>
        public Time(Time time)
        {
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time">时间</param>
        public Time(Times time)
        {
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
        }
        #endregion;

        #region 属性
        /// <summary>
        /// 时
        /// </summary>
        private int? _Hour = 0;
        /// <summary>
        /// 时
        /// </summary>
        public int? Hour
        {
            get { return this._Hour; }
            set
            {
                this._Hour = value;
                if (this._Hour >= 24)
                    this._Hour %= 24;
            }
        }
        /// <summary>
        /// 分
        /// </summary>
        private int _Minute = 0;
        /// <summary>
        /// 分
        /// </summary>
        public int Minute
        {
            get { return this._Minute; }
            set
            {
                this._Minute = value;
                if (this._Minute >= 60)
                {
                    int Hours = this._Minute / 60;
                    this._Minute %= 60;
                    this.Hour += Hours;
                }
            }
        }
        /// <summary>
        /// 秒
        /// </summary>
        private int _Second = 0;
        /// <summary>
        /// 秒
        /// </summary>
        public int Second
        {
            get { return this._Second; }
            set
            {
                this._Second = value;
                if (this._Second >= 60)
                {
                    int Minutes = this._Second / 60;
                    this._Second %= 60;
                    this.Minute += Minutes;
                }
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
            if (time == null || !(time is Time)) return -1;
            return this.TotalSeconds.CompareTo((time as Time).TotalSeconds);
        }
        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public int CompareTo(Time time)
        {
            if (time == null) return -1;
            return this.TotalSeconds.CompareTo(time.TotalSeconds);
        }
        /// <summary>
        /// 比较两个值
        /// </summary>
        /// <param name="x">第一个时间</param>
        /// <param name="y">第二个时间</param>
        /// <returns></returns>
        public bool Equals(Time x, Time y)
        {
            if (x == null)
                return y == null;
            else
                return y != null && x.TotalSeconds.Equals(y.TotalSeconds);
        }
        /// <summary>
        /// 比较两个值
        /// </summary>
        /// <param name="time">第二个时间</param>
        /// <returns></returns>
        public bool Equals(Time time)
        {
            return this.Equals(this, time);
        }
        /// <summary>
        /// 当前实例的哈希代码
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public int GetHashCode(Time time)
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
            return "{0}:{1}:{2}".format(this.Hour.ToString().PadLeft(2, '0'),
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
            var that = new DateTime(now.Year, now.Month, now.Day, this.Hour.GetValueOrDefault(), this.Minute, this.Second);
            return that.ToString(format, formatProvider);
        }
        #endregion

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
        public void AddSeconds(int Seconds)
        {
            this.Second += Seconds;
        }
        #endregion

        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Times(Time v) => new Times(v);
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator Time(string v) => new Time(v);
    }
}