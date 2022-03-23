using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Data.SQL
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
       Create Time : 2017/12/20 9:40:00
       Update Time : 2017/12/20 9:40:00
    ===================================================================
    */
    /// <summary>
    /// 扩展SQL语法
    /// Verstion : 2.0.0
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2017/12/20 9:40:00
    /// Update Time : 2018/01/19 13:40:00
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 扩展SQL 语法

        #region 扩展SQL 字段加值+
        /// <summary>
        /// 字段加值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T AddSQL<T>(this T obj, T value) { return obj; }
        #endregion

        #region 扩展SQL 字段减值-
        /// <summary>
        /// 字段减值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T SubtractSQL<T>(this T obj, T value) { return obj; }
        #endregion

        #region 扩展SQL 字段乘值*
        /// <summary>
        /// 字段乘值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static T MultiplySQL<T>(this T obj, T value) { return obj; }
        #endregion

        #region 扩展SQL 字段除值/
        /// <summary>
        /// 字段除值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static T DivideSQL<T>(this T obj, T value) { return obj; }
        #endregion

        #region 扩展SQL Between
        /// <summary>
        /// 扩展SQL Between
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="startValue">开始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Boolean BetweenSQL(this object obj, object startValue, object endValue) { return true; }
        #endregion

        #region 扩展SQL ISNULL
        /// <summary>
        /// 扩展SQL ISNULL
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static T IsNullSQL<T>(this T obj, T defaultValue) { return defaultValue; }
        #endregion

        #region 扩展SQL in 语法
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="array">数组织</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool InSQL<T>(this T obj, IEnumerable<T> array) { return true; }
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="array">数组织</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool InSQL<T>(this T obj, T[] array) { return true; }
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="T1">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="queryableX">IQueryableX对象</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Boolean InSQL<T, T1>(this T obj, IQueryableX<T1> queryableX) { return true; }
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool InSQL<T>(this T obj, object[] array) { return true; }
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool NotInSQL<T>(this T obj, IEnumerable<T> array) { return true; }
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool NotInSQL<T>(this T obj, T[] array) { return true; }
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="T1">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="queryableX">IQueryableX对象</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Boolean NotInSQL<T, T1>(this T obj, IQueryableX<T1> queryableX) { return true; }
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool NotInSQL<T>(this T obj, object[] array) { return true; }
        #endregion

        #region 扩展SQL like 语法
        /// <summary>
        /// 扩展SQL like 语法
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="likeStr">子串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool LikeSQL(this object str, string likeStr) { return true; }
        /// <summary>
        /// 扩展SQL not like 语法
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="likeStr">子串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static bool NotLikeSQL(this string str, string likeStr) { return true; }
        /// <summary>
        /// 扩展SQL like 语法
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="likeStr">子串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool LikeSQLX(this object str, string likeStr) { return true; }
        /// <summary>
        /// 扩展SQL not like 语法
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="likeStr">子串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotLikeSQLX(this string str, string likeStr) { return true; }
        #endregion

        #region 扩展SQL DateAdd 语法
        /// <summary>
        /// 扩展SQL DateAdd 语法
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="val">值</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static DateTime DateAddSQL(this DateTime date, long val, string format)
        {
            return date;
        }
        /// <summary>
        /// 扩展SQL DateAdd 语法
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="val">值</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static DateTime DateAddSQL(this DateTime? date, long val, string format)
        {
            return date.GetValueOrDefault();
        }
        #endregion

        #region 扩展SQL DateDiff 语法
        /// <summary>
        /// 扩展SQL DateDiff 语法
        /// </summary>
        /// <param name="firstDate">第一个时间</param>
        /// <param name="SecondDate">第二个时间</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static int DateDiffSQL(this DateTime? firstDate, DateTime? SecondDate, string format) { return 1; }
        /// <summary>
        /// 扩展SQL DateDiff 语法
        /// </summary>
        /// <param name="firstDate">第一个时间</param>
        /// <param name="SecondDate">第二个时间</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static int DateDiffSQL(this DateTime firstDate, DateTime SecondDate, string format) { return 1; }
        #endregion

        #region 扩展SQL DatePart 语法
        /// <summary>
        /// 扩展SQL DatePart 语法
        /// </summary>
        /// <param name="Date">当前时间</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int DatePartSQL(this DateTime? Date, string format) { return 0; }
        /// <summary>
        /// 扩展SQL DatePart 语法
        /// </summary>
        /// <param name="Date">当前时间</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int DatePartSQL(this DateTime Date, string format) { return 0; }
        #endregion

        #region 扩展SQL DateFormat 语法
        /// <summary>
        /// 扩展SQL DateFormat 语法
        /// </summary>
        /// <param name="Date">当前时间</param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DateFormatSQL(this DateTime? Date, string format) { return string.Empty; }
        /// <summary>
        /// 扩展SQL DateFormat 语法
        /// </summary>
        /// <param name="Date">当前时间</param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DateFormatSQL(this DateTime Date, string format) { return string.Empty; }
        #endregion

        #region 扩展SQL Charindex 语法
        /// <summary>
        /// 扩展SQL Charindex 语法
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="IndexStr">子串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int CharIndexSQL(this string str, string IndexStr) { return 1; }
        #endregion

        #region 扩展SQL Patindex语法
        /// <summary>
        /// 扩展SQL Patindex语法
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="IndexStr">子串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int PatindexSQL(this string str, string IndexStr) { return 1; }
        #endregion

        #region 扩展SQL Abs 算法
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int AbsSQL(this int value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int AbsSQL(this int? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Int16 AbsSQL(this Int16? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Int16 AbsSQL(this Int16 value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Int64 AbsSQL(this Int64? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Int64 AbsSQL(this Int64 value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double AbsSQL(this Double? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double AbsSQL(this Double value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal AbsSQL(this decimal? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal AbsSQL(this decimal value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float AbsSQL(this float? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float AbsSQL(this float value) { return 1; }
        #endregion

        #region 扩展SQL Floor 算法
        /// <summary>
        /// 扩展SQL Floor 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double FloorSQL(this Double? value) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double FloorSQL(this Double value) { return 1; }
        /// <summary>
        /// 扩展SQL Floor 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal FloorSQL(this decimal? value) { return 1; }
        /// <summary>
        /// 扩展SQL Floor 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal FloorSQL(this decimal value) { return 1; }
        /// <summary>
        /// 扩展SQL Floor 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float FloorSQL(this float? value) { return 1; }
        /// <summary>
        /// 扩展SQL Floor 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float FloorSQL(this float value) { return 1; }
        #endregion

        #region 扩展SQL Ceiling 算法
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double CeilingSQL(this Double? value) { return 1; }
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double CeilingSQL(this Double value) { return 1; }
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal CeilingSQL(this decimal? value) { return 1; }
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal CeilingSQL(this decimal value) { return 1; }
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float CeilingSQL(this float? value) { return 1; }
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float CeilingSQL(this float value) { return 1; }
        #endregion

        #region 扩展SQL Round 算法
        /// <summary>
        /// 扩展SQL Round 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double RoundSQL(this Double? value, int len) { return 1; }
        /// <summary>
        /// 扩展SQL Round 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static Double RoundSQL(this Double value, int len) { return 1; }
        /// <summary>
        /// 扩展SQL Round 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal RoundSQL(this decimal? value, int len) { return 1; }
        /// <summary>
        /// 扩展SQL Round 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static decimal RoundSQL(this decimal value, int len) { return 1; }
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float RoundSQL(this float? value, int len) { return 1; }
        /// <summary>
        /// 扩展SQL Round 算法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static float RoundSQL(this float value, int len) { return 1; }
        #endregion

        #region 扩展SQL Left
        /// <summary>
        /// 扩展SQL Left
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="num">第几位</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string LeftSQL(this string str, int num) { return ""; }
        #endregion

        #region 扩展SQL Right
        /// <summary>
        /// 扩展SQL Right
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="num">第几位</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string RightSQL(this string str, int num) { return ""; }
        #endregion

        #region 扩展SQL Len
        /// <summary>
        /// 扩展SQL Len
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int LengthSQL(this string str) { return 1; }
        #endregion

        #region 扩展SQL Replace
        /// <summary>
        /// 扩展SQL Replace
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="oldString">原字符串</param>
        /// <param name="replaceString">新字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string ReplaceSQL(this string str, string oldString, string replaceString) { return ""; }
        #endregion

        #region 扩展SQL Replicate
        /// <summary>
        /// 扩展SQL Replicate
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="num">重复次数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string ReplicateSQL(this string str, int num) { return ""; }
        #endregion

        #region 扩展SQL Reverse
        /// <summary>
        /// 扩展SQL Reverse
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string ReverseSQL(this string str) { return ""; }
        #endregion

        #region 扩展SQL Stuff
        /// <summary>
        /// 扩展SQL Stuff
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="strat">开始位置</param>
        /// <param name="length">长度</param>
        /// <param name="replaceString">替换字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string StuffSQL(this string str, int strat, int length, string replaceString) { return ""; }
        #endregion

        #region 扩展SQL Substring
        /// <summary>
        /// 扩展SQL Substring
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string SubstringSQL(this string str, int start, int length) { return ""; }
        #endregion

        #region 扩展SQL Trim
        /// <summary>
        /// 扩展SQL Ltrim
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string LTrimSQL(this string str) { return ""; }
        /// <summary>
        /// 扩展SQL Rtrim
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string RTrimSQL(this string str) { return ""; }
        /// <summary>
        /// 扩展SQL Trim
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string TrimSQL(this string str) { return ""; }
        #endregion

        #region 扩展SQL Lower
        /// <summary>
        /// 扩展SQL Lower
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string LowerSQL(this string str) { return ""; }
        #endregion

        #region 扩展SQL Upper
        /// <summary>
        /// 扩展SQL Upper
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static string UpperSQL(this string str) { return ""; }
        #endregion

        #region 扩展SQL Count
        /// <summary>
        /// 扩展SQL Count
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static int CountSQL(this object str) { return 0; }
        #endregion

        #region 扩展SQL MAX
        /// <summary>
        /// 扩展SQL MAX
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">字符串</param>
        /// <returns></returns>
        public static T MaxSQL<T>(this T obj) { return obj; }
        #endregion

        #region 扩展SQL MIN
        /// <summary>
        /// 扩展SQL MIN
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">字符串</param>
        /// <returns></returns>
        public static T MinSQL<T>(this T obj) { return obj; }
        #endregion

        #region 扩展SQL SUM
        /// <summary>
        /// 扩展SQL SUM
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">字符串</param>
        /// <returns></returns>
        public static T SumSQL<T>(this T obj) { return obj; }
        #endregion

        #region 扩展SQL AVG
        /// <summary>
        /// 扩展SQL AVG
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">字符串</param>
        /// <returns></returns>
        public static T AvgSQL<T>(this T obj) { return obj; }
        #endregion

        #region 设置字段别名
        /// <summary>
        /// 设置字段别名
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="ColumnName">别名</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")] 
        public static T As<T>(this T obj, string ColumnName) { return obj; }
        #endregion

        #region SQL参数化调整
        /// <summary>
        /// SQL参数化调整
        /// </summary>
        /// <param name="_">SQL语句</param>
        /// <param name="dbProvider">数据库驱动</param>
        /// <returns></returns>
        public static string SQLFormat(this string _, DbProviderType dbProvider)
        {
            switch (dbProvider)
            {
                case DbProviderType.Dameng:
                    return _.ReplacePattern(@"@((Sub_(\d+_)?)?ParamName\d+)", "?");
                case DbProviderType.Oracle:
                    return _.ReplacePattern(@"@((Sub_(\d+_)?)?ParamName\d+)", ":$1");
            }
            return _;
        }
        #endregion

        #endregion
    }
}