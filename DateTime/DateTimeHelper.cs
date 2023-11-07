using System;
using System.Globalization;
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
    /// 日期操作类
    /// Version : 1.0.0
    /// Create Time : 2017/9/18 0:51:57
    /// Update Time : 2017/9/18 0:51:57
    /// </summary>
    public class DateTimeHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DateTimeHelper() { }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 获取某一年有多少周
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="dayOfWeek">一星期中第一天是星期几</param>
        /// <returns>该年周数</returns>
        public static int GetWeekAmount(int year, DayOfWeek dayOfWeek = DayOfWeek.Monday)
        {
            var end = new DateTime(year, 12, 31);
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, dayOfWeek); //该年星期数
        }
        /// <summary>
        /// 返回年度第几个星期   默认星期日是第一天
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="dayOfWeek">那天为一周的第一天</param>
        /// <returns></returns>
        public static int WeekOfYear(DateTime date, DayOfWeek dayOfWeek = DayOfWeek.Sunday)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, dayOfWeek);
        }
        /// <summary>
        /// 返回年度第几个星期
        /// </summary>
        /// <returns></returns>
        public static int WeekOfYear() { return WeekOfYear(DateTime.Now); }
        /// <summary>
        /// 得到一年中的某周的起始日和截止日
        /// 年 nYear
        /// 周数 nNumWeek
        /// 周始 out dtWeekStart
        /// 周终 out dtWeekeEnd
        /// </summary>
        /// <param name="Year">年份</param>
        /// <param name="NumWeek">第几周</param>
        /// <param name="dtWeekStart">开始日期</param>
        /// <param name="dtWeekeEnd">结束日期</param>
        public static void GetWeekTime(int Year, int NumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
        {
            DateTime dt = new DateTime(Year, 1, 1);
            dt += new TimeSpan((NumWeek - 1) * 7, 0, 0, 0);
            dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday);
            dtWeekeEnd = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1);
        }

        #region 时间戳转为C#格式时间
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="TimeStamp">时间戳</param>
        /// <returns></returns>
        public static DateTime GetDateTime(string TimeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return TimeStamp.Length == 13 ? dtStart.AddMilliseconds(TimeStamp.ToLong()) : dtStart.AddSeconds(TimeStamp.ToLong());
        }
        #endregion

        #region 获取时间戳
        /// <summary>
        /// 获取时间戳 秒
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static int GetTimeStamp(DateTime time)
        {
            return (int)(GetTimeStamps(time) / 1000);
        }
        /// <summary>
        /// 获取时间戳 秒
        /// </summary>
        /// <returns></returns>
        public static int GetTimeStamp() { return GetTimeStamp(DateTime.Now); }
        /// <summary>
        /// 获取时间戳 毫秒
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static long GetTimeStamps(DateTime time)
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (time.Ticks - startTime.Ticks) / 10000;
        }
        /// <summary>
        /// 获取时间戳 毫秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamps() { return GetTimeStamps(DateTime.Now); }
        #endregion

        #endregion
    }
}