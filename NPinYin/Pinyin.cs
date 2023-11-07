using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
namespace XiaoFeng
{
    /// <summary>
    /// 汉字转拼音类
    /// </summary>
    public static class PinYin
    {
        /// <summary>
        /// 取中文文本的拼音首字母
        /// </summary>
        /// <param name="text">编码为UTF8的文本</param>
        /// <returns>返回中文对应的拼音首字母</returns>
        public static string GetFirstLetter(string text)
        {
            text = text.Trim();
            StringBuilder chars = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                string py = GetPinyin(text[i]);
                if (py != "") chars.Append(py[0]);
            }
            return chars.ToString().ToUpper();
        }
        /// <summary>
        /// 取中文文本的拼音首字母
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="encoding">源文本的编码</param>
        /// <returns>返回encoding编码类型中文对应的拼音首字母</returns>
        public static string GetFirstLetter(string text, Encoding encoding)
        {
            //return text.ToEncode(Encoding.UTF8, encoding);
            string temp = ConvertEncoding(text, encoding, Encoding.UTF8);
            return ConvertEncoding(GetFirstLetter(temp), Encoding.UTF8, encoding);
        }
        /// <summary>
        /// 取中文文本的拼音
        /// </summary>
        /// <param name="text">编码为UTF8的文本</param>
        /// <param name="IsFirstUpper">是否首字母大写</param>
        /// <returns>返回中文文本的拼音</returns>
        public static string GetPinyin(string text, Boolean IsFirstUpper = false)
        {
            PhraseSpecial.Each<KeyValuePair<string, string>>(a =>
            {
                text = text.Replace(a.Key.ToString(), a.Value.ToString());
            });
            StringBuilder pinyin = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                string py = GetPinyin(text[i]);
                if (py != "") pinyin.Append(py);
                pinyin.Append(" ");
            }
            PhraseSpecial.Each<KeyValuePair<string, string>>(a =>
            {
                pinyin = pinyin.Replace(a.Value.ToString().ToCharArray().Join(" "), a.Value.ToString());
            });
            var _ = pinyin.ToString().Trim();
            if (IsFirstUpper) _ = _.ToUpperFirst();
            return _;
        }
        /// <summary>
        /// 取中文文本的拼音
        /// </summary>
        /// <param name="text">编码为UTF8的文本</param>
        /// <param name="encoding">源文本的编码</param>
        /// <returns>返回encoding编码类型的中文文本的拼音</returns>
        public static string GetPinyin(string text, Encoding encoding)
        {
            string temp = ConvertEncoding(text.Trim(), encoding, Encoding.UTF8);
            return ConvertEncoding(GetPinyin(temp), Encoding.UTF8, encoding);
        }
        /// <summary>
        /// 取和拼音相同的汉字列表
        /// </summary>
        /// <param name="pinyin">编码为UTF8的拼音</param>
        /// <returns>取拼音相同的汉字列表，如拼音“ai”将会返回“唉爱……”等</returns>
        public static string GetChineseText(string pinyin)
        {
            string key = pinyin.Trim().ToLower();

            foreach (string str in PyCode.codes)
            {
                if (str.StartsWith(key + " ") || str.StartsWith(key + ":"))
                    return str.Substring(7);
            }
            return "";
        }
        /// <summary>
        /// 取和拼音相同的汉字列表，编码同参数encoding
        /// </summary>
        /// <param name="pinyin">编码为encoding的拼音</param>
        /// <param name="encoding">编码</param>
        /// <returns>返回编码为encoding的拼音为pinyin的汉字列表，如拼音“ai”将会返回“唉爱……”等</returns>
        public static string GetChineseText(string pinyin, Encoding encoding)
        {
            string text = ConvertEncoding(pinyin, encoding, Encoding.UTF8);
            return ConvertEncoding(GetChineseText(text), Encoding.UTF8, encoding);
        }
        /// <summary>
        /// 返回单个字符的汉字拼音
        /// </summary>
        /// <param name="ch">编码为UTF8的中文字符</param>
        /// <returns>ch对应的拼音</returns>
        public static string GetPinyin(char ch)
        {
            short hash = GetHashIndex(ch);
            for (var i = 0; i < PyHash.hashes[hash].Length; ++i)
            {
                short index = PyHash.hashes[hash][i];
                var pos = PyCode.codes[index].IndexOf(ch, 7);
                if (pos != -1)
                    return PyCode.codes[index].Substring(0, 6).Trim();
            }
            return ch.ToString();
        }
        /// <summary>
        /// 返回单个字符的汉字拼音
        /// </summary>
        /// <param name="ch">编码为encoding的中文字符</param>
        /// <param name="encoding">编码</param>
        /// <returns>编码为encoding的ch对应的拼音</returns>
        public static string GetPinyin(char ch, Encoding encoding)
        {
            ch = ConvertEncoding(ch.ToString(), encoding, Encoding.UTF8)[0];
            return ConvertEncoding(GetPinyin(ch), Encoding.UTF8, encoding);
        }
        /// <summary>
        /// 转换编码 
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="srcEncoding">源编码</param>
        /// <param name="dstEncoding">目标编码</param>
        /// <returns>目标编码文本</returns>
        public static string ConvertEncoding(string text, Encoding srcEncoding, Encoding dstEncoding)
        {
            byte[] srcBytes = srcEncoding.GetBytes(text);
            byte[] dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }
        /// <summary>
        /// 取文本索引值
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns>文本索引值</returns>
        private static short GetHashIndex(char ch)
        {
            return (short)((uint)ch % PyCode.codes.Length);
        }
        /// <summary>
        /// 设置或获取包含例外词组读音的键/值对的组合
        /// </summary>
        private static IDictionary _PhraseSpecial = new Dictionary<string, string>();
        /// <summary>
        /// 设置或获取包含例外词组读音的键/值对的组合
        /// </summary>
        public static IDictionary PhraseSpecial
        {
            get
            {
                if (_PhraseSpecial == null || _PhraseSpecial.Count == 0)
                {
                    _PhraseSpecial = new Dictionary<string, string>
                    {
                        { "重庆", "chong qing" },
                        { "银行", "yin hang" },
                        { "了解", "liao jie" },
                        { "行家","hang jia" },
                        { "一行","yi hang" },
                        { "两行","liang hang" },
                        { "三行","san hang" },
                        { "四行","si hang" },
                        { "行行","hang hang" },
                        { "便宜", "pian yi" },
                        { "提防", "di fang" },
                        { "人参", "ren shen" },
                        { "朝夕", "zhao xi" },
                        { "省亲", "xing qin" },
                        { "西藏", "xi zang" },
                        { "宝藏", "bao zang" },
                        { "藏府", "zang fu" },
                        { "藏族", "zang zu" },
                        { "藏獒", "zang ao" },
                        { "弹射", "tan she" },
                        { "弹跳", "tan tiao" },
                        { "弹拨", "tan bo" },
                        { "弹指", "tan zhi" },
                        { "弹琴", "tan qin" },
                        { "弹奏", "tan zhou" },
                        { "弹冠相庆", "tan guan xiang qing" },
                        { "伎俩", "ji liang" },
                        { "堡垒", "bao lei" },
                        { "城堡", "cheng bao" },
                        { "桥头堡", "qiao tou bao" },
                        { "星宿", "xing xiu" },
                        { "都不", "dou bu" },
                        { "都是", "dou shi" },
                        { "都行", "dou xing" },
                        { "都中", "dou zhong" },
                        { "都好", "dou hao" },
                    };
                    /*
                     
                     */
                }
                return _PhraseSpecial;
            }
            set { _PhraseSpecial = value; }
        }
    }
}