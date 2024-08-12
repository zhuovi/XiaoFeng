using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XiaoFeng.Config;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-11-16 14:19:03                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 判断字符串是否为某种格式
    /// Verstion : 1.0.1
    /// Create Time : 2017/11/16 14:19:03
    /// Update Time : 2018/04/11 11:43:18
    /// </summary>
    public static partial class PrototypeHelper
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        private static RegexExpression Pattern => RegexExpression.Current;

        #region 判断字符串是否是某种格式

        #region 是否是物理路径
        /// <summary>
        /// 是否是物理路径 如果路径前边加{*}一定是物理路径
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsBasePath(this String _)
        {
            if (_.StartsWith("{*}")) return true;
            var os = OS.Platform.GetOSPlatform();
            if (os == PlatformOS.Linux)
                return _.StartsWith("/");
            if (os == PlatformOS.OSX)
                return _.StartsWith("/");
            return _.IsMatch(RegexPattern.BasePath);
        }
        #endregion

        #region 是否是汉字格式
        /// <summary>
        /// 是否是汉字格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsChinese(this String _) { return _.IsMatch(RegexPattern.Chinese); }
        #endregion

        #region 是否是字母格式
        /// <summary>
        /// 是否是字母格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsLetter(this String _) { return _.IsMatch(RegexPattern.Letter); }
        #endregion

        #region 是否是网址格式
        /// <summary>
        /// 是否是网址格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsSite(this String _) { return _.IsMatch(RegexPattern.Site); }
        #endregion

        #region 是否是FTP格式
        /// <summary>
        /// 是否是FTP格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsFTP(this String _) { return _.IsMatch(RegexPattern.Ftp); }
        #endregion

        #region 是否是GUID格式
        /// <summary>
        /// 是否是GUID
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsGuid(this String _) => _.Trim(new char[] { '{', '}', '(', ')' }).IsMatch(RegexPattern.Guid);
        /// <summary>
        /// 是否是UUID
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsUUID(this String _) => _.IsGuid();
        #endregion

        #region 是否是Email格式
        /// <summary>
        /// 是否是Email
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsEmail(this String _) { return _.IsMatch(RegexPattern.Email); }
        #endregion

        #region 是否是数字格式
        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNumberic(this String _) { return _.IsMatch(RegexPattern.Numberic); }
        #endregion

        #region 是否是浮点格式
        /// <summary>
        /// 是否是浮点
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsFloat(this String _) { return _.IsMatch(RegexPattern.Float); }
        #endregion

        #region 是否是固话格式
        /// <summary>
        /// 是否是固话
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsTel(this String _) => _.IsMatch(RegexPattern.Tel);
        #endregion

        #region 是否是手机格式
        /// <summary>
        /// 是否是手机
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsPhone(this String _) => _.IsMatch(RegexPattern.Phone);
        #endregion

        #region 是否是日期格式
        /// <summary>
        /// 是否是日期
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsDate(this String _) { return _.IsMatch(RegexPattern.Date); }
        #endregion

        #region 是否是时间格式
        /// <summary>
        /// 是否是时间
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsTime(this String _) { return _.IsMatch(RegexPattern.Time); }
        #endregion

        #region 是否是日期时间格式
        /// <summary>
        /// 是否是日期时间
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsDateTime(this String _) { return _.IsMatch(RegexPattern.DateTime); }
        #endregion

        #region 是否是日期时间格式
        /// <summary>
        /// 是否是日期时间 日期或日期+时间格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsDateOrTime(this String _) { return _.IsMatch(RegexPattern.DateOrTime); }
        #endregion

        #region 是否是IP格式
        /// <summary>
        /// 是否是IP格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsIP(this String _) { return _.IsMatch(RegexPattern.IP); }
        /// <summary>
        /// 是否是内网IP或本地IP
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsIntranetIp(this String _) => _.IsMatch(RegexPattern.IntranetIp);
        /// <summary>
        /// 是否是本地IP
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsLocalIp(this String _) => _.IsMatch(RegexPattern.LocalIp);
        #endregion

        #region 是否是bool格式
        /// <summary>
        /// 是否是bool格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsBoolean(this String _) { return _.IsMatch(RegexPattern.Boolean); }
        #endregion

        #region 指定字符串是否为 null 或 System.String.Empty 字符串
        /// <summary>
        /// Guid是否为空
        /// </summary>
        /// <param name="_">guid</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNullOrEmpty(this Guid? _) => !_.HasValue || _ == Guid.Empty || _ == null;
        /// <summary>
        /// Guid是否不为空
        /// </summary>
        /// <param name="_">guid</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNotNullOrEmpty(this Guid? _) => !_.IsNullOrEmpty();
        /// <summary>
        /// Guid是否为空
        /// </summary>
        /// <param name="_">guid</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNullOrEmpty(this Guid _) => _ == Guid.Empty;
        /// <summary>
        /// Guid是否不为空
        /// </summary>
        /// <param name="_">guid</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNotNullOrEmpty(this Guid _) => !_.IsNullOrEmpty();
        /// <summary>
        /// 指定字符串是否为 null 或 System.String.Empty 字符串
        /// </summary>
        /// <param name="_">对象</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNullOrEmpty<T>(this T _)
        {
            if (_ == null) return true;
            var t = _.GetType();
            if (t == typeof(DBNull)) return true;
            //if (t == typeof(Guid)) return _.ToCast<Guid>() == Guid.Empty;
            if (t == typeof(String)) return _.ToString().IsNullOrEmpty();

            return false;
        }
        /// <summary>
        /// 指定字符串是否不为 null 或 非System.String.Empty 字符串
        /// </summary>
        /// <param name="_">对象</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNotNullOrEmpty<T>(this T _) => !_.IsNullOrEmpty();
        /// <summary>
        /// 指定字符串是否为 null 或 System.String.Empty 字符串
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNullOrEmpty(this String _)
        {
            if (_ != null) return _.Length == 0;
            return true;
        }
        /// <summary>
        /// 指定字符串是否不为 null 或 非System.String.Empty 字符串
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNotNullOrEmpty(this String _) => !_.IsNullOrEmpty();
        #endregion

        #region 指定字符串是否为 null、空还是仅由空白字符组成
        /// <summary>
        /// 指定字符串是否为 null、空还是仅由空白字符组成
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNullOrWhiteSpace(this String _)
        {
            if (_ != null)
                for (int i = 0; i < _.Length; i++)
                    if (!char.IsWhiteSpace(_[i])) return false;
            return true;
        }
        /// <summary>
        /// 指定字符串是否为不为 null、空还是仅由非空白字符组成
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsNotNullOrWhiteSpace(this String _)
        {
            return !_.IsNullOrWhiteSpace();
        }
        #endregion

        #region 是否是身份证号
        /// <summary>
        /// 是否是身份证号
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsIdentityCard(this String _) => new CardInfo().Valid(_);
        #endregion

        #region 防止SQL注入
        /// <summary>
        /// 防止SQL注入.
        /// </summary>
        /// <param name="value">参数值</param>
        public static string ReplaceSQL(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return "";
            var config = Setting.Current;
            string RSQL = config.SQLInjection.IsNullOrEmpty() ? "" : config.SQLInjection;
            if (RSQL.IsNullOrEmpty())
                RSQL = @"insert\s+into |update |delete |select | union | join |exec |execute | exists|'|=|truncate |create |drop |alter |column |table |dbo\.|sys\.|alert\(|<scr|ipt>|<script|confirm\(|console\.|\.js|<\/\s*script>|now\(\)|getdate\(\)|time\(\)| Directory\.| File\.|FileStream |\.Write\(|\.Connect\(|<\?php|show tables |echo | outfile |Request[\.\(]|Response[\.\(]|eval\s*\(|\$_GET|\$_POST|cast\(|Server\.CreateObject|VBScript\.Encode|replace\(";
            while (value.IsMatch(@"[\r\n\t\s]*(" + RSQL + ")"))
                value = value.RemovePattern(@"[\r\n\t\s]*(" + RSQL + ")", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return value;
        }
        #endregion

        #region 是否是参数
        /// <summary>
        /// 是否是参数
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsQuery(this String _) => _.IsMatch(RegexPattern.Query);
        #endregion

        #region 是否是Json
        /// <summary>
        /// 是否是Json
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsJson(this String _) => _.IsMatch(RegexPattern.Json);
        #endregion

        #region 是否是Xml
        /// <summary>
        /// 是否是Xml
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsXml(this String _) => _.IsMatch(RegexPattern.Xml);
        #endregion

        #region 是否是属性索引器
        /// <summary>
        /// 是否是属性索引器
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsIndexer(this String _) => _.IsMatch(RegexPattern.Indexer);
        #endregion

        #region 是否是银行卡号
        /// <summary>
        /// 是否是银行卡号
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns>返回结果
        /// <para><term>true</term> 符合正则表达式</para>
        /// <para><term>false</term> 不符合正则表达式</para>
        /// </returns>
        public static Boolean IsBankCardNo(this String _) => BankInfo.CheckBankCardNO(_);
        #endregion

        #region 获取字符串强度
        /// <summary>
        /// 获取字符串强度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="strengthType">验证强度类型</param>
        /// <returns>字符串强度值
        /// <para><term>0</term> 长度少于6位。</para>
        /// <para><term>1</term> 包含数字，小写字母，大写字母，半角字符，汉字，全角字符中的一种。</para>
        /// <para><term>2</term> 包含两种。</para>
        /// <para><term>3</term> 包含三种</para>
        /// <para><term>4</term> 包含四种</para>
        /// <para><term>5</term> 包含五种</para>
        /// <para><term>6</term> 包含六种</para>
        /// </returns>
        public static int GetStringStrength(this string str, StrengthType strengthType = StrengthType.Number | StrengthType.LowwerLetter | StrengthType.UpperLetter | StrengthType.HalfSymbol | StrengthType.FullSymbol | StrengthType.Chinese)
        {
            if (str.Length <= 5 || (int)strengthType < 1) return 0;
            var matchs = new List<string>();
            strengthType.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Each(s =>
            {
                var regex = s.Trim().ToEnum<StrengthType>().GetDefaultValue();
                if (regex.IsNotNullOrEmpty()) matchs.Add(regex);
            });
            var i = 0;
            matchs.Each(m => { if (new Regex(m).IsMatch(str)) i++; });
            if (i >= 4 && str.Length < 13) i--;
            return i;
        }
        #endregion

        #endregion
    }
}