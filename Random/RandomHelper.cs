using System;
using System.Collections.Generic;
/****************************************************************
*  Copyright © (2018) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2018-01-23 11:01:07                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    #region 随机生成字符串
    /// <summary>
    /// 随机生成字符串
    /// Verstion : 1.1.0
    /// Create Time : 2018/01/23 11:01:07
    /// Update Time : 2018/04/23 10:31:15
    /// </summary>
    public class RandomHelper
    {
        #region 构造器

        #region 无参构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RandomHelper()
        {
            this.RandomType = RandomType.Number;
            this.IsRepeat = true;
            this.Min = 0;
            this.Max = 100;
            this.Count = 1;
            this.Length = 4;
            long num = Guid.NewGuid().GetHashCode() + DateTime.Now.Ticks;
            this.Ran = new Random(((int)(((ulong)num) & 0xffffffffL)) | ((int)(num >> 32)));
        }
        #endregion

        #endregion

        #region 属性
        /// <summary>
        /// 特殊字符
        /// </summary>
        private const string SpecialChars = @"`~!@#$%^&*()_+-=;':"",./?\|{}[]";
        /// <summary>
        /// 字母
        /// </summary>
        private const string LetterChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// 数字
        /// </summary>
        private const string NumberChars = "0123456789";
        /// <summary>
        /// 最小值
        /// </summary>
        public int Min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public int Max { get; set; }
        /// <summary>
        /// 随机类型
        /// </summary>
        public RandomType RandomType { get; set; }
        /// <summary>
        /// 字符串长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 是否重复
        /// </summary>
        public Boolean IsRepeat { get; set; }
        /// <summary>
        /// 生成个数
        /// </summary>
        public int Count { get; set; }
        private Random _Ran;
        /// <summary>
        /// 随机种子
        /// </summary>
        private Random Ran
        {
            get
            {
                if (this._Ran == null)
                {
                    long num = Guid.NewGuid().GetHashCode() + DateTime.Now.Ticks;
                    this._Ran = new Random(((int)(((ulong)num) & 0xffffffffL)) | ((int)(num >> 32)));
                }
                return _Ran;
            }
            set => this._Ran = value;
        }
        #endregion

        #region 方法

        #region 单例模式
        /// <summary>
        /// 实例化类
        /// </summary>
        private static RandomHelper _Helper = null;
        /// <summary>
        /// 实例化类
        /// </summary>
        private static RandomHelper Helper
        {
            get
            {
                if (_Helper == null) _Helper = new RandomHelper();
                return _Helper;
            }
        }
        #endregion

        #region 静态方法

        #region 获取区间数字
        /// <summary>
        /// 获取区间数字
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max) { return Helper.GetRandom(min, max); }
        #endregion

        #region 获取一组随机数字
        /// <summary>
        /// 获取一组随机数字
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="count">数组长度</param>
        /// <param name="isRepeat">是否重复</param>
        /// <returns></returns>
        public static List<int> GetRandomInts(int min, int max, int count, Boolean isRepeat = true)
        {
            return Helper.GetRandoms(min, max, count, isRepeat);
        }
        #endregion

        #region 获取字符串
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="ranType">类型</param>
        /// <param name="isRepeat">是否重复</param>
        /// <returns></returns>
        public static string GetRandomString(int length = 4, RandomType ranType = RandomType.Number | RandomType.Letter, Boolean isRepeat = true)
        {
            return Helper.GetRandom(length, ranType, isRepeat);
        }
        #endregion

        #region 获取一组随机字符串
        /// <summary>
        /// 获取一组随机字符串
        /// </summary>
        /// <param name="count">数组长度</param>
        /// <param name="length">字符串长度</param>
        /// <param name="ranType">类型</param>
        /// <param name="isRepeat">是否重复</param>
        /// <returns></returns>
        public static List<string> GetRandomStrings(int count, int length = 4, RandomType ranType = RandomType.Number | RandomType.Letter, Boolean isRepeat = true)
        {
            return Helper.GetRandoms(count, length, ranType, isRepeat);
        }
        #endregion

        #region 随机生成一个字节数组
        /// <summary>
        /// 随机生成一个字节数组
        /// </summary>
        /// <param name="length">数组长度</param>
        /// <returns></returns>
        public static byte[] GetRandomBytes(int length) => Helper.GetBytes(length);
        #endregion

        #endregion

        #region 动态方法

        #region 获取区间数字
        /// <summary>
        /// 获取区间数字
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public int GetRandom(int min, int max)
        {
            if (min > max) return 0;
            else if (min == max) return min;
            return this.Ran.Next(min, max);
        }
        #endregion

        #region 获取一组随机数字
        /// <summary>
        /// 获取一组随机数字
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="count">数组长度</param>
        /// <param name="isRepeat">是否重复</param>
        /// <returns></returns>
        public List<int> GetRandoms(int min, int max, int count, Boolean isRepeat = true)
        {
            List<int> list = new List<int>();
            if (count <= 0) return list;
            if (min > max) return list;
            else if (min == max) { list.Add(min); return list; }
            if (!isRepeat && max - min < count) count = max - min;
            for (int i = 0; i < count; i++)
            {
                int Rand = this.GetRandom(min, max);
                while (!isRepeat && list.Contains(Rand))
                {
                    Rand = this.GetRandom(min, max);
                }
                list.Add(Rand);
            }
            return list;
        }
        #endregion

        #region 获取字符串
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="ranType">类型</param>
        /// <param name="isRepeat">是否重复</param>
        /// <returns></returns>
        public string GetRandom(int length = 4, RandomType ranType = RandomType.Number | RandomType.Letter, Boolean isRepeat = true)
        {
            if (length <= 0) return "";
            string _ = string.Empty, _Chars = string.Empty;
            switch ((int)ranType)
            {
                case 1: _Chars = NumberChars; break;
                case 2: _Chars = LetterChars; break;
                case 3: _Chars = NumberChars + LetterChars; break;
                case 4: _Chars = SpecialChars; break;
                case 5: _Chars = NumberChars + SpecialChars; break;
                case 6: _Chars = LetterChars + SpecialChars; break;
                case 7: _Chars = NumberChars + LetterChars + SpecialChars; break;
            }
            int _Length = _Chars.Length;
            for (int i = 0; i < length; i++)
            {
                string _Char = _Chars[this.Ran.Next(0, _Length - 1)].ToString();
                if (!isRepeat)
                {
                    _Chars = _Chars.Replace(_Char, ""); _Length--;
                    if (_Length == 0) break;
                }
                _ += _Char;
            }
            return _;
        }
        #endregion

        #region 获取一组随机字符串
        /// <summary>
        /// 获取一组随机字符串
        /// </summary>
        /// <param name="count">数组长度</param>
        /// <param name="length">字符串长度</param>
        /// <param name="ranType">类型</param>
        /// <param name="isRepeat">是否重复</param>
        /// <returns></returns>
        public List<string> GetRandoms(int count, int length = 4, RandomType ranType = RandomType.Number | RandomType.Letter, Boolean isRepeat = true)
        {
            List<string> list = new List<string>();
            if (count <= 0 || length <= 0) return list;
            for (int i = 0; i < count; i++)
            {
                string Rand = this.GetRandom(length, ranType, isRepeat);
                while (!isRepeat && list.Contains(Rand))
                {
                    Rand = this.GetRandom(length, ranType, isRepeat);
                }
                list.Add(Rand);
            }
            return list;
        }
        #endregion

        #region 随机生成一个字节数组
        /// <summary>
        /// 随机生成一个字节数组
        /// </summary>
        /// <param name="length">数组长度</param>
        /// <returns></returns>
        public byte[] GetBytes(int length)
        {
            var bytes = new byte[length];
            this.Ran.NextBytes(bytes);
            return bytes;
        }
        #endregion

        #endregion

        #endregion
    }
    #endregion

    #region 生成字符类型
    /// <summary>
    /// 生成字符类型
    /// </summary>
    [Flags]
    public enum RandomType
    {
        /// <summary>
        /// 数字
        /// </summary>
        Number = 1,
        /// <summary>
        /// 字母
        /// </summary>
        Letter = 2,
        /// <summary>
        /// 特殊字符
        /// </summary>
        Special = 4,
        /// <summary>
        /// 所有
        /// </summary>
        All = 7
    }
    #endregion
}