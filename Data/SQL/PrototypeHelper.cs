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
*  Create Time : 2017-12-20 09:40:00                            *
*  Version : v 2.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 扩展SQL语法
    /// Verstion : 2.0.0
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
        /// <param name="field">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T AddSQL<T>(this T field, T value) => field;
        #endregion

        #region 扩展SQL 字段减值-
        /// <summary>
        /// 字段减值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T SubtractSQL<T>(this T field, T value) => field;
        #endregion

        #region 扩展SQL 字段乘值*
        /// <summary>
        /// 字段乘值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>        
        public static T MultiplySQL<T>(this T field, T value) => field;
        #endregion

        #region 扩展SQL 字段除值/
        /// <summary>
        /// 字段除值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>         
        public static T DivideSQL<T>(this T field, T value) => field;
        #endregion

        #region 扩展SQL Between
        /// <summary>
        /// 扩展SQL Between
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>         
        public static Boolean BetweenSQL<T>(this T field, T minValue, T maxValue) => true;
        #endregion

        #region 扩展SQL ISNULL
        /// <summary>
        /// 扩展SQL ISNULL
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>         
        public static T IsNullSQL<T>(this T field, T defaultValue) => field;
        #endregion

        #region 扩展SQL in 语法
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="array">数组织</param>
        /// <returns></returns>         
        public static Boolean InSQL<T>(this T field, IEnumerable<T> array) => true;
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="array">数组织</param>
        /// <returns></returns>         
        public static Boolean InSQL<T>(this T field, T[] array) => true;
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="T1">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="queryableX">IQueryableX对象</param>
        /// <returns></returns>         
        public static Boolean InSQL<T, T1>(this T field, IQueryableX<T1> queryableX) => true;
        /// <summary>
        /// 扩展SQL in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>         
        public static Boolean InSQL<T>(this T field, object[] array) => true;
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>         
        public static Boolean NotInSQL<T>(this T field, IEnumerable<T> array) => true;
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>         
        public static Boolean NotInSQL<T>(this T field, T[] array) => true;
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="T1">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="queryableX">IQueryableX对象</param>
        /// <returns></returns>         
        public static Boolean NotInSQL<T, T1>(this T field, IQueryableX<T1> queryableX) => true;
        /// <summary>
        /// 扩展SQL not in语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="array">数组</param>
        /// <returns></returns>         
        public static Boolean NotInSQL<T>(this T field, object[] array) => true;
        #endregion

        #region 扩展SQL like 语法
        /// <summary>
        /// 扩展SQL like 语法 自动加%
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="likeString">子串</param>
        /// <returns></returns>         
        public static Boolean LikeSQL<T>(this T field, string likeString) { return true; }
        /// <summary>
        /// 扩展SQL not like 语法 自动加%
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="likeString">子串</param>
        /// <returns></returns>         
        public static Boolean NotLikeSQL<T>(this T field, string likeString) { return true; }
        /// <summary>
        /// 扩展SQL like 语法 不自动增加%
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="likeString">子串</param>
        /// <returns></returns>        
        public static Boolean LikeSQLX<T>(this T field, string likeString) { return true; }
        /// <summary>
        /// 扩展SQL not like 语法 不自动增加%
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="likeString">子串</param>
        /// <returns></returns>        
        public static Boolean NotLikeSQLX<T>(this T field, string likeString) { return true; }
        #endregion

        #region 扩展SQL DateAdd 语法
        /// <summary>
        /// 扩展SQL DateAdd 语法
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="val">值</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static DateTime DateAddSQL(this DateTime field, long val, string format) => field;
        /// <summary>
        /// 扩展SQL DateAdd 语法
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="val">值</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static DateTime DateAddSQL(this DateTime? field, long val, string format) => field.GetValueOrDefault();
        #endregion

        #region 扩展SQL DateDiff 语法
        /// <summary>
        /// 扩展SQL DateDiff 语法
        /// </summary>
        /// <param name="field">第一个时间</param>
        /// <param name="secondDate">第二个时间</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static int DateDiffSQL(this DateTime? field, DateTime? secondDate, string format) { return 1; }
        /// <summary>
        /// 扩展SQL DateDiff 语法
        /// </summary>
        /// <param name="field">第一个时间</param>
        /// <param name="secondDate">第二个时间</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>
        public static int DateDiffSQL(this DateTime field, DateTime secondDate, string format) { return 1; }
        #endregion

        #region 扩展SQL DatePart 语法
        /// <summary>
        /// 扩展SQL DatePart 语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="format">格式 年yy,yyyy 季度qq,q 月mm,m 年中的日dy,y 日dd,d 周wk,ww 星期dw,w 小时hh 分钟mi,n 秒ss,s 毫秒ms 微秒mcs 纳秒ns</param>
        /// <returns></returns>         
        public static int DatePartSQL<T>(this T field, string format) { return 0; }
        #endregion

        #region 扩展SQL DateFormat 语法
        /// <summary>
        /// 扩展SQL DateFormat 语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string DateFormatSQL<T>(this T field, string format) { return string.Empty; }
        #endregion

        #region 扩展SQL Charindex 语法
        /// <summary>
        /// 扩展SQL Charindex 语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="childString">子串</param>
        /// <returns></returns>         
        public static int CharIndexSQL<T>(this T field, string childString) { return 1; }
        #endregion

        #region 扩展SQL Patindex语法
        /// <summary>
        /// 扩展SQL Patindex语法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="childString">子串</param>
        /// <returns></returns>         
        public static int PatindexSQL<T>(this T field, string childString) { return 1; }
        #endregion

        #region 扩展SQL Abs 算法
        /// <summary>
        /// 扩展SQL Abs 算法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static T AbsSQL<T>(this T field) { return field; }
        #endregion

        #region 扩展SQL Floor 算法
        /// <summary>
        /// 扩展SQL Floor 算法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static T FloorSQL<T>(this T field) { return field; }
        #endregion

        #region 扩展SQL Ceiling 算法
        /// <summary>
        /// 扩展SQL Ceiling 算法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static T CeilingSQL<T>(this T field) { return field; }
        #endregion

        #region 扩展SQL Round 算法
        /// <summary>
        /// 扩展SQL Round 算法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="len">小数点保留长度</param>
        /// <returns></returns>         
        public static T RoundSQL<T>(this T field, int len) { return field; }
        #endregion

        #region 扩展SQL Left
        /// <summary>
        /// 扩展SQL Left
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="num">第几位</param>
        /// <returns></returns>         
        public static string LeftSQL(this string field, int num) => string.Empty;
        #endregion

        #region 扩展SQL Right
        /// <summary>
        /// 扩展SQL Right
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="num">第几位</param>
        /// <returns></returns>         
        public static string RightSQL(this string field, int num) => string.Empty;
        #endregion

        #region 扩展SQL Len
        /// <summary>
        /// 扩展SQL Len
        /// </summary>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static int LengthSQL(this string field) => 1;
        #endregion

        #region 扩展SQL Replace
        /// <summary>
        /// 扩展SQL Replace
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="oldString">原字符串</param>
        /// <param name="replaceString">新字符串</param>
        /// <returns></returns>         
        public static string ReplaceSQL(this string field, string oldString, string replaceString) => string.Empty;
        #endregion

        #region 扩展SQL Replicate
        /// <summary>
        /// 扩展SQL Replicate
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="num">重复次数</param>
        /// <returns></returns>         
        public static string ReplicateSQL(this string field, int num)=> string.Empty;
        #endregion

        #region 扩展SQL Reverse
        /// <summary>
        /// 扩展SQL Reverse
        /// </summary>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static string ReverseSQL(this string field) => string.Empty;
        #endregion

        #region 扩展SQL Stuff
        /// <summary>
        /// 扩展SQL Stuff
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="strat">开始位置</param>
        /// <param name="length">长度</param>
        /// <param name="replaceString">替换字符串</param>
        /// <returns></returns>         
        public static string StuffSQL(this string field, int strat, int length, string replaceString) => string.Empty;
        #endregion

        #region 扩展SQL Substring
        /// <summary>
        /// 扩展SQL Substring
        /// </summary>
        /// <param name="field">对象</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>         
        public static string SubstringSQL(this string field, int start, int length) => string.Empty;
        #endregion

        #region 扩展SQL Trim
        /// <summary>
        /// 扩展SQL Ltrim
        /// </summary>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static string LTrimSQL(this string field) => string.Empty;
        /// <summary>
        /// 扩展SQL Rtrim
        /// </summary>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static string RTrimSQL(this string field) => string.Empty;
        /// <summary>
        /// 扩展SQL Trim
        /// </summary>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static string TrimSQL(this string field) => string.Empty;
        #endregion

        #region 扩展SQL Lower
        /// <summary>
        /// 扩展SQL Lower
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static T LowerSQL<T>(this T field) => field;
        #endregion

        #region 扩展SQL Upper
        /// <summary>
        /// 扩展SQL Upper
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static T UpperSQL<T>(this T field) => field;
        #endregion

        #region 扩展SQL Count
        /// <summary>
        /// 扩展SQL Count
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>         
        public static int CountSQL<T>(this T field) => 0;
        #endregion

        #region 扩展SQL MAX
        /// <summary>
        /// 扩展SQL MAX
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>
        public static T MaxSQL<T>(this T field) => field;
        #endregion

        #region 扩展SQL MIN
        /// <summary>
        /// 扩展SQL MIN
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>
        public static T MinSQL<T>(this T field) => field;
        #endregion

        #region 扩展SQL SUM
        /// <summary>
        /// 扩展SQL SUM
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>
        public static T SumSQL<T>(this T field) => field;
        #endregion

        #region 扩展SQL AVG
        /// <summary>
        /// 扩展SQL AVG
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <returns></returns>
        public static T AvgSQL<T>(this T field) => field;
        #endregion

        #region 扩展SQL STDEV
        /// <summary>
        /// 扩展SQL STDEV
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static float StDevSQL<T>(this T field) => 0;
        #endregion

        #region 扩展SQL STDEVP
        /// <summary>
        /// 扩展SQL STDEVP
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static float StDevpSQL<T>(this T field) => 0;
        #endregion

        #region 设置字段别名
        /// <summary>
        /// 设置字段别名
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="fieldName">别名</param>
        /// <returns></returns>         
        public static T As<T>(this T field, string fieldName) => field;
        #endregion

        #region 转换类型
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="field">对象</param>
        /// <param name="dataType">目标类型</param>
        /// <returns></returns>
        public static T CastSQL<T>(this T field, string dataType) => field;
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
                default: 
                    return _;
            }
        }
        #endregion

        #endregion
    }
}