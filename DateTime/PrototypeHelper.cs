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
*  Create Time : 2017-09-18 00:51:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 附加属性方法操作类 日期操作
    /// Version : 1.0.0
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 时间转时间戳(秒)
        /// <summary>
        /// 时间转时间戳(秒)
        /// </summary>
        /// <param name="_">时间</param>
        /// <param name="startDateTime">起始时间</param>
        /// <returns></returns>
        public static int ToTimeStamp(this DateTime _, DateTime startDateTime)
        {
            return (int)(_.ToTimeStamps(startDateTime)/1000);
        }
        /// <summary>
        /// 时间转时间戳(秒)
        /// </summary>
        /// <param name="_">时间</param>
        /// <returns></returns>
        public static int ToTimeStamp(this DateTime _)
        {
            return _.ToTimeStamp(new DateTime(1970, 1, 1));
        }
        #endregion

        #region 时间转时间戳(毫秒)
        /// <summary>
        /// 时间转时间戳(毫秒)
        /// </summary>
        /// <param name="_">时间</param>
        /// <param name="startDateTime">起始时间</param>
        /// <returns></returns>
        public static long ToTimeStamps(this DateTime _, DateTime startDateTime)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTimeFromUtc(startDateTime, TimeZoneInfo.Local);
            return (long)(_ - StartTime).TotalMilliseconds;
        }
        /// <summary>
        /// 时间转时间戳(毫秒)
        /// </summary>
        /// <param name="_">时间</param>
        /// <returns></returns>
        public static long ToTimeStamps(this DateTime _)
        {
            return _.ToTimeStamps(new DateTime(1970, 1, 1));
        }
        #endregion

        #region 时间戳转时间(秒)
        /// <summary>
        /// 时间戳转时间(秒)
        /// </summary>
        /// <param name="_">时间戳</param>
        /// <param name="startDateTime">起始时间</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this int _, DateTime startDateTime)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTimeFromUtc(startDateTime, TimeZoneInfo.Local);
            return StartTime.AddSeconds(_);
        }
        /// <summary>
        /// 时间戳转时间(秒)
        /// </summary>
        /// <param name="_">时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this int _)
        {
            return _.ToDateTime(new DateTime(1970, 1, 1));
        }
        #endregion

        #region 时间戳转时间(毫秒)
        /// <summary>
        /// 时间戳转时间(毫秒)
        /// </summary>
        /// <param name="_">时间戳</param>
        /// <param name="startDateTime">起始时间</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long _, DateTime startDateTime)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTimeFromUtc(startDateTime, TimeZoneInfo.Local);
            return StartTime.AddMilliseconds(_);
        }
        /// <summary>
        /// 时间戳转时间(毫秒)
        /// </summary>
        /// <param name="_">时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long _)
        {
            return _.ToDateTime(new DateTime(1970, 1, 1));
        }
        #endregion

        #region 时间差防SQL中DateDiff
        /// <summary>
        /// 时间差防SQL中DateDiff
        /// </summary>
        /// <param name="nowTime">结束时间</param>
        /// <param name="subtractTime">开始时间</param>
        /// <param name="diffType">时间差类型</param>
        /// <returns></returns>
        public static double DateDiff(this DateTime nowTime, DateTime subtractTime, DateDiffType diffType = 0)
        {
            TimeSpan ts = nowTime.Subtract(subtractTime);
            switch (diffType)
            {
                case DateDiffType.Milliseconds:
                    return ts.TotalMilliseconds;
                case DateDiffType.Seconds:
                    return ts.TotalSeconds;
                case DateDiffType.Minutes:
                    return ts.TotalMinutes;
                case DateDiffType.Hours:
                    return ts.TotalHours;
                case DateDiffType.Days:
                    return ts.TotalDays;
                case DateDiffType.Weeks:
                    int week = 6 - (int)subtractTime.DayOfWeek;
                    return Math.Ceiling(ts.TotalDays / 7) + (((int)ts.TotalDays % 7) > week ? 1 : 0) + (ts.TotalDays < 7 ? -1 : 0);
                case DateDiffType.Months:
                    return (nowTime.Year - subtractTime.Year) * 12 + nowTime.Month - subtractTime.Month;
                case DateDiffType.Years:
                    return nowTime.Year - subtractTime.Year;
                default:
                    return ts.TotalMilliseconds;
            }
        }
        #endregion
    }
}