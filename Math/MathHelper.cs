using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
       Create Time : 2017/10/31 14:18:38
       Update Time : 2017/10/31 14:18:38
    ===================================================================
    */
    /// <summary>
    /// 数字扩展方法
    /// Verstion : 1.0.0
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2017/10/31 14:18:38
    /// Update Time : 2017/10/31 14:18:38
    /// </summary>
    public static class MathHelper
    {
        #region 方法

        #region 无符号右移
        /// <summary>
        /// 无符号右移
        /// </summary>
        /// <param name="value">整型</param>
        /// <param name="pos">移动位数 默认2位</param>
        /// <returns></returns>
        public static int RightMove(this int value, int pos = 2)
        {
            int _value = value;
            if (pos != 0)  //移动 0 位时直接返回原值
            {
                int mask = int.MaxValue;     // int.MaxValue = 0x7FFFFFFF 整数最大值
                _value >>= 1;               //无符号整数最高位不表示正负但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
                _value &= mask;     //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                _value >>= pos - 1;     //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
            }
            return _value;
        }
        #endregion
        
        #region 10进制转任意进制
        /// <summary>
        /// 生成编码字符
        /// </summary>
        private const String keys = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ+_=-~!@#$%^&*()`,./<>?|:;}{}[]'";
        /// <summary>
        /// 10进制转任意进制
        /// </summary>
        /// <param name="value">10进制数据</param>
        /// <param name="NumberFormat">进制数</param>
        /// <returns>返回一个转换后的进制数据</returns>
        public static string DecimalToString(decimal value, int NumberFormat = 64)
        {
            string result = string.Empty;
            int Exponent = NumberFormat > keys.Length ? keys.Length : NumberFormat;
            do
            {
                decimal index = value % Exponent;
                result = keys[(int)index] + result;
                value = (value - index) / Exponent;
            }
            while (value > 0);
            return result;
        }
        #endregion

        #region 任意进制转换原数据
        /// <summary>
        /// 任意进制转换原数据
        /// </summary>
        /// <param name="value">任意进制</param>
        /// <param name="NumberFormat">转换回原进制</param>
        /// <returns>返回任意进制转换原数据</returns>
        public static decimal StringToDecimal(string value, int NumberFormat = 64)
        {
            decimal result = 0;
            int Exponent = NumberFormat > keys.Length ? keys.Length : NumberFormat;
            for (int i = 0; i < value.Length; i++)
            {
                int x = value.Length - i - 1;
                result += keys.IndexOf(value[i]) * Pow(Exponent, x);// Math.Pow(exponent, x);
            }
            return result;
        }
        #endregion

        #region 一个数据的N次方
        /// <summary>
        /// 一个数据的N次方
        /// </summary>
        /// <param name="baseNo">数据</param>
        /// <param name="x">次幂数</param>
        /// <returns></returns>
        private static decimal Pow(decimal baseNo, decimal x)
        {
            decimal value = 1;
            while (x > 0)
            {
                value *= baseNo;
                x--;
            }
            return value;
        }
        #endregion

        #endregion
    }

    #region 定义 8位 有符号 无符号数组  仿Javascript Uint8Array Int8Array

    #region 表示 8 位无符号整数。
    /// <summary>
    /// 表示 8 位无符号整数。
    /// </summary>
    public class UInt8Array
    {
        /// <summary>
        /// 最大值
        /// </summary>
        public const Byte MaxValue = Byte.MaxValue;
        /// <summary>
        /// 最小值
        /// </summary>
        public const Byte MinValue = Byte.MinValue;
        /// <summary>
        /// 数据
        /// </summary>
        private Byte[] Data { get; set; }
        /// <summary>
        /// 设置数组长度
        /// </summary>
        /// <param name="Length">长度</param>
        public UInt8Array(int Length)
        {
            this.Data = new Byte[Length];
        }
        /// <summary>
        /// 赋值数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="v">值</param>
        private void Set(int index, Int64 v)
        {
            Int64 m = v % (MaxValue + 1);
            this.Data[index] = (Byte)(m > 0 ? m : (m + MaxValue + 1));
        }
        /// <summary>
        /// 复制数组到当前数组
        /// </summary>
        /// <param name="a">要复制的数组</param>
        /// <param name="start">复制到数组的开始位置</param>
        public void Set(object[] a, int start = 0)
        {
            if (a.Length > this.Length + start) return;
            for (int i = start; i < this.Length; i++)
                this[i] = (Int16)a[i - start];
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public Byte Get(int index) { return index >= this.Length ? (Byte)0 : this.Data[index]; }
        /// <summary>
        /// 设置获取值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public Int64 this[int index]
        {
            get { return this.Get(index); }
            set { this.Set(index, value); }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        public void Delete() { this.Data = null; }
        /// <summary>
        /// 数组长度
        /// </summary>
        public int Length { get { return this.Data.Length; } }
        /// <summary>
        /// 将此实例的数值转换为其等效的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
    #endregion

    #region 表示 8 位有符号整数。
    /// <summary>
    /// 表示 8 位有符号整数。
    /// </summary>
    public class Int8Array
    {
        /// <summary>
        /// 最大值
        /// </summary>
        public const SByte MaxValue = SByte.MaxValue;
        /// <summary>
        /// 最小值
        /// </summary>
        public const SByte MinValue = SByte.MinValue;
        /// <summary>
        /// 数据
        /// </summary>
        private SByte[] Data { get; set; }
        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <param name="Length">长度</param>
        public Int8Array(int Length)
        {
            this.Data = new SByte[Length];
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public SByte Get(int index) { return index >= this.Length ? (SByte)0 : this.Data[index]; }
        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void Set(int index, Int64 value)
        {
            Int64 TotalValue = MaxValue - MinValue + 1;
            value %= TotalValue;
            if (value > MaxValue && value < TotalValue)
                value -= TotalValue;
            else if (value > -TotalValue && value < MinValue)
                value += TotalValue;
            this.Data[index] = (SByte)value;
        }
        /// <summary>
        /// 获取或设置数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public Int64 this[int index]
        {
            get { return this.Get(index); }
            set { this.Set(index, value); }
        }
        /// <summary>
        /// 复制数组到当前数组
        /// </summary>
        /// <param name="a">要复制的数组</param>
        /// <param name="start">复制到数组的开始位置</param>
        public void Set(object[] a, int start = 0)
        {
            if (a.Length > this.Length + start) return;
            for (int i = start; i < this.Length; i++)
                this[i] = (Int16)a[i - start];
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        public void Delete() { this.Data = null; }
        /// <summary>
        /// 获取数据长度
        /// </summary>
        public int Length { get { return this.Data.Length; } }
        /// <summary>
        /// 将此实例的数值转换为其等效的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
    #endregion

    #endregion
}