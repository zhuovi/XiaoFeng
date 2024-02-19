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
        private static string[] UpperUnits = new string[] { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟", "京", "拾", "佰", "仟", "垓", "拾", "佰", "仟", "秭", "拾", "佰", "仟", "穣", "拾", "佰", "仟", "沟", "拾", "佰", "仟", "涧", "拾", "佰", "仟", "正", "拾", "佰", "仟", "载", "拾", "佰", "仟", "极", "拾", "佰", "仟", "恒河沙", "拾", "佰", "仟", "阿僧只", "拾", "佰", "仟", "那由他", "拾", "佰", "仟", "不可思议", "拾", "佰", "仟", "无量大数", "拾", "佰", "仟" };
        /// <summary>
        /// 人民币基本单位
        /// </summary>
        private static string[] MoneyRadice = new string[] { "", "拾", "佰", "仟" };
        /// <summary>
        /// 大写基本单位
        /// </summary>
        private static string[] UpperRadice = new string[] { "", "拾", "佰", "仟" };
        /// <summary>
        /// 整型单位
        /// </summary>
        private static string[] IntUnits = new string[] { "", "万", "亿", "兆", "京", "垓", "秭", "穣", "沟", "涧", "正", "载", "极", "恒河沙", "阿僧只", "那由他", "不可思议", "无量大数" };
        /// <summary>
        /// 整
        /// </summary>
        private static string Integer = "整";
        /// <summary>
        /// 圆
        /// </summary>
        private static string IntLast = "元";
        /// <summary>
        /// 金额单位
        /// </summary>
        private static string[] UpperMoney = new string[] { "角", "分", "厘", "毫", "微", "纳", "皮" };
        /// <summary>
        /// 最大数字
        /// </summary>
        private static double Max = 8999999999999999999.9999;
        /// <summary>
        /// 数字字典
        /// </summary>
        private static Dictionary<string, long> UpperNumberDict = new Dictionary<string, long>
        {
            {"零",0 },
            {"壹",1 },
            {"贰",2 },
            {"叁",3 },
            {"肆",4 },
            {"伍",5 },
            {"陆",6 },
            {"柒",7 },
            {"捌",8 },
            {"玖",9 },
            {"拾",10 },
            {"十",10 },
            {"百",100 },
            {"佰",100 },
            {"千",1000 },
            {"仟",1000 },
            {"万",10000 },
            {"亿",100000000 },
            {"兆",1000000000000 },
            {"京",10000000000000000 }
        };
        #endregion

        #region 方法
        /// <summary>
        /// 阿拉伯数字转换大写数字
        /// </summary>
        /// <param name="number">阿拉伯数字</param>
        /// <param name="upperType">数字类型</param>
        /// <returns>大写数字</returns>
        public static string NumberToChinese(this string number, UpperType upperType = UpperType.Money)
        {
            var val = number.ToCast<double>();
            if (val > Max) return "";
            if (val == 0) return "零" + (upperType == UpperType.Money ? "元整" : "");
            var IntegerNum = "";
            var DecimalNum = "";
            var _ = new StringBuilder();
            if (number.IndexOf(".") == -1)
            {
                IntegerNum = number;
            }
            else
            {
                var parts = number.Split('.');
                IntegerNum = parts[0];
                DecimalNum = parts[1];
            }
            var Radice = upperType == UpperType.Money ? MoneyRadice : UpperRadice;
            //整数部分
            if (IntegerNum.ToCast<long>() > 0)
            {
                var ZeroCount = 0;
                var IntLength = IntegerNum.Length;
                for (var i = 0; i < IntLength; i++)
                {
                    var n = IntegerNum[i];
                    var p = IntLength - i - 1;
                    var q = p / 4;
                    var m = p % 4;
                    if (n == '0')
                        ZeroCount++;
                    else
                    {
                        if (ZeroCount > 0)
                            _.Append(UpperChars[0]);
                        ZeroCount = 0;
                        _.Append(UpperChars[n.ToString().ToCast<int>()] + Radice[m]);
                    }
                    if (m == 0 && ZeroCount < 4)
                        _.Append(IntUnits[q]);
                }
                if (upperType == UpperType.Money) _.Append(IntLast);
            }
            else
            {
                if (upperType == UpperType.Number) _.Append("零");
            }
            if (upperType == UpperType.Number) _.Append("点");
            //小数部分
            if (DecimalNum.IsNotNullOrEmpty())
            {
                var decLength = DecimalNum.Length;
                if (decLength > 7 && upperType == UpperType.Money) decLength = 7;
                for (var i = 0; i < decLength; i++)
                {
                    var n = DecimalNum[i];
                    //if (n != '0')
                    //{
                        _.Append(UpperChars[n.ToString().ToCast<int>()]);
                        if (upperType == UpperType.Money)
                            _.Append(UpperMoney[i]);
                   // }
                }
            }
            if (_.Length == 0)
                return "零" + (upperType == UpperType.Money ? "元整" : "");
            if (upperType == UpperType.Money && DecimalNum.IsNullOrEmpty())
                _.Append(Integer);
            return _.ToString().TrimEnd('点');
        }
        /// <summary>
        /// 大写数字转换阿拉伯数字或数字转换带逗号格式
        /// </summary>
        /// <param name="chineseNumber">大写数字</param>
        /// <param name="isComma">是否加逗号</param>
        /// <returns>阿拉伯数字</returns>
        public static string ChineseToNumber(this string chineseNumber, Boolean isComma = false)
        {
            if (chineseNumber.IsNullOrEmpty()) return "0";
            var _ = 0D;
            //先把圆角分厘去掉
            chineseNumber = chineseNumber.Replace("圆", "点").Replace("元", "点");
            chineseNumber = chineseNumber.RemovePattern(@"[整零" + UpperMoney.Join() + "@]").TrimEnd('点');
            chineseNumber = chineseNumber.Replace("拾", "十").Replace("佰", "百").Replace("仟", "千");

            var IntegerNum = "";
            var DecimalNum = "";
            if (chineseNumber.IndexOf("点") == -1)
            {
                IntegerNum = chineseNumber;
            }
            else
            {
                var parts = chineseNumber.Split('点');
                IntegerNum = parts[0];
                DecimalNum = parts[1];
            }
            if (IntegerNum.IsNotNullOrEmpty())
            {
                //整数部分
                var units = new char[] { '京', '兆', '亿', '万' };
                //先把上边单位拆出来计算
                units.Each(u =>
                {
                    if (IntegerNum.IndexOf(u) == -1) return;
                    var _parts = IntegerNum.Split(new char[] { u }, StringSplitOptions.RemoveEmptyEntries);
                    var tempNumber = _parts[0];
                    _ += J(tempNumber) * UpperNumberDict[u.ToString()];
                    if (_parts.Length > 1)
                        IntegerNum = _parts[1];
                    else IntegerNum = "";
                });
            }
            if (IntegerNum.IsNotNullOrEmpty())
                _ += J(IntegerNum);
            //计算小数部分
            if (DecimalNum.IsNotNullOrEmpty())
            {
                var d = "0.";
                DecimalNum.Each(a =>
                {
                    d += UpperNumberDict[a.ToString()];
                });
                _ += d.ToCast<double>();
            }
            return isComma ? _.ToString("N2") : _.ToString();
        }
        /// <summary>
        /// 转换千百十
        /// </summary>
        /// <param name="part">数据</param>
        /// <returns></returns>
        private static double J(string part)
        {
            var _ = 0D;
            var units = new char[] { '千', '百', '十' };
            units.Each(u =>
            {
                if (part.IndexOf(u) == -1) return;
                var _parts = part.Split(new char[] { u }, StringSplitOptions.RemoveEmptyEntries);
                var temp = _parts[0];
                _ += UpperNumberDict[temp].ToCast<long>() * UpperNumberDict[u.ToString()];
                if (_parts.Length > 1)
                {
                    part = _parts[1];
                }
            });
            if (part.IsNotNullOrEmpty())
                _ += UpperNumberDict[part];
            return _;
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
        Money = 1
    }
}