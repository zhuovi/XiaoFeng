using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-10-12 18:00:55                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 大写操作类
    /// </summary>
    public static class UpperNumber
    {
        #region 属性
        /// <summary>
        /// 数字
        /// </summary>
        private static string[] UpperChars = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        /// <summary>
        /// 数字单位
        /// </summary>
        private static string[] UpperUnits = new string[] { "","拾", "佰", "仟", "万","拾","佰","仟", "亿" ,"拾","佰","仟", "兆", "拾", "佰", "仟", "京", "拾", "佰", "仟", "垓", "拾", "佰", "仟", "秭", "拾", "佰", "仟", "穣", "拾", "佰", "仟", "沟", "拾", "佰", "仟", "涧", "拾", "佰", "仟", "正", "拾", "佰", "仟", "载", "拾", "佰", "仟", "极", "拾", "佰", "仟", "恒河沙", "拾", "佰", "仟", "阿僧只", "拾", "佰", "仟", "那由他", "拾", "佰", "仟", "不可思议", "拾", "佰", "仟", "无量大数", "拾", "佰", "仟" };
        /// <summary>
        /// 金额单位
        /// </summary>
        private static string[] UpperMoney = new string[] { "角", "分", "厘", "毫", "微", "纳", "皮" };
        #endregion

        #region 方法
        /// <summary>
        /// 阿拉伯数字转换大写数字
        /// </summary>
        /// <param name="num">阿拉伯数字</param>
        /// <param name="upperType">数字类型</param>
        /// <returns>大写数字</returns>
        public static string ToChineseNumber(this string num, UpperType upperType = UpperType.Number)
        {
            if (num.IsNullOrEmpty()) return String.Empty;
            var value = num;
            value = value.RemovePattern(",");
            if (!value.IsFloat()) return String.Empty;
            if (num.IsNotNullOrEmpty()) value = num;
            value = value.ReplacePattern(@"(.\d*?)(0+)$", m => m.Groups[1].Value).RemovePattern(@"(^0+|,|\.$)");
            string Integral = "", Spot = "", Digits = "";
            if (value.IndexOf(".") > -1)
            {
                var vals = value.Split('.');
                Integral = vals[0];
                Spot = ".";
                Digits = vals[1];
            }
            else Integral = value;
            var sbr = new StringBuilder();
            if (Integral.StartsWith("-")) {
                Integral = Integral.TrimStart('-');
                sbr.Append("负");
            }
            /*整数*/
            for (var i = 0; i < Integral.Length; i++)
            {
                var v = Integral[Integral.Length - i - 1].ToString();
                var unit = UpperUnits[i];
                if (upperType == UpperType.Money && unit.IsNullOrEmpty()) unit = "圆";
                sbr.Insert(0, UpperChars[v.ToCast<int>()] + unit);
            }
            /*点*/
            if (Spot.IsNotNullOrEmpty())
                sbr.Append(upperType == UpperType.Number ? "点" : "");
            else
                if (upperType == UpperType.Money) sbr.Append("整");
            /*小数*/
            for (var i = 0; i < Digits.Length; i++)
            {
                sbr.Append(UpperChars[i]);
                if (upperType == UpperType.Money) sbr.Append(UpperMoney[i]);
            }
            return sbr.ToString();
        }
        /// <summary>
        /// 大写数字转换阿拉伯数字
        /// </summary>
        /// <param name="chineseNumber">大写数字</param>
        /// <param name="isComma">是否加逗号</param>
        /// <returns>阿拉伯数字</returns>
        public static string ToNumber(this string chineseNumber, Boolean isComma = false)
        {
            if (chineseNumber.IsNullOrEmpty()) return String.Empty;
            var sbr = new StringBuilder();
            chineseNumber = chineseNumber.RemovePattern($@"({UpperUnits.Join("|").Trim('|')}|整)");
            chineseNumber = chineseNumber.Replace("点", ".");
            if (chineseNumber.IsMatch(@"(圆|元)"))
                chineseNumber = chineseNumber.ReplacePattern(@"(圆|元)", ".").TrimEnd('.');
            chineseNumber = chineseNumber.RemovePattern($@"({UpperMoney.Join("|")})");
            for (var i = 0; i < chineseNumber.Length; i++)
            {
                var v = chineseNumber[i].ToString();
                if (v == ".")
                {
                    sbr.Append(".");
                    continue;
                }
                sbr.Append(UpperChars.IndexOf(v));
            }
            var Sbr = new StringBuilder();
            if (isComma)
            {
                var _ = sbr.ToString();
                var vs = _.Split('.');
                for (var i = 0; i < vs[0].Length; i++)
                {
                    var v = vs[0].Length - i - 1;
                    if (i > 0 && i % 3 == 0)
                        Sbr.Insert(0, ",");
                    Sbr.Insert(0, vs[0][v].ToString());
                }
                if (vs.Length > 1)
                {
                    Sbr.Append(".");
                    Sbr.Append(vs[1]);
                }
                sbr.Clear();
            }
            else Sbr = sbr;
            return Sbr.ToString();
        }
        #endregion
    }
    /// <summary>
    /// 大写类型
    /// </summary>
    public enum UpperType
    {
        /// <summary>
        /// 数字
        /// </summary>
        [Description("数字")]
        Number = 0,
        /// <summary>
        /// 金额
        /// </summary>
        [Description("金额")] 
        Money =1
    }
}