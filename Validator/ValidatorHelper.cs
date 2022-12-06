using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
namespace XiaoFeng.Validator
{
    /// <summary>
    /// 验证操作类
    /// </summary>
    public class ValidatorHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ValidatorHelper() { this.Result = new List<string>(); }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        public ValidatorHelper(string name, object value):this()
        {
            this.Name = name;this.Value = value;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 参数值
        /// </summary>
        private object Value { get; set; } 
        /// <summary>
        /// 参数名
        /// </summary>
        private string Name { get; set; }
        /// <summary>
        /// 验证结果
        /// </summary>
        public List<string> Result { get; set; }
        /// <summary>
        /// 是否验证
        /// </summary>
        public Boolean IsValid { get { return this.Result.Count > 0; }  }
        #endregion

        #region 方法

        #region 验证参数
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public ValidatorHelper Requires(string name,object value)
        {
            var validator = new ValidatorHelper(name, value);
            validator.Result.AddRange(this.Result);
            return validator;
        }
        #endregion

        #region 验证参数是否为空
        /// <summary>
        /// 验证参数是否为空
        /// </summary>
        /// <returns></returns>
        public ValidatorHelper IsNullOrEmpty()
        {
            if (!this.Value.IsNullOrEmpty())
            {
                this.Result.Add("参数[{0}]的值为空.".format(this.Name));
            }
            return this;
        }
        /// <summary>
        /// 验证参数是否不为空
        /// </summary>
        /// <returns></returns>
        public ValidatorHelper IsNotNullOrEmpty()
        {
            if (!this.Value.IsNotNullOrEmpty())
            {
                this.Result.Add("参数[{0}]的值为空.".format(this.Name));
            }
            return this;
        }
        /// <summary>
        /// 验证参数是否为空
        /// </summary>
        /// <returns></returns>
        public ValidatorHelper IsNullOrWhiteSpace()
        {
            if (!this.Value.ToString().IsNullOrWhiteSpace())
            {
                this.Result.Add("参数[{0}]的值为空.".format(this.Name));
            }
            return this;
        }
        /// <summary>
        /// 验证参数是否不为空
        /// </summary>
        /// <returns></returns>
        public ValidatorHelper IsNotNullOrWhiteSpace()
        {
            if (this.Value.ToString().IsNotNullOrWhiteSpace())
            {
                this.Result.Add("参数[{0}]的值为空.".format(this.Name));
            }
            return this;
        }
        #endregion

        #region 验证规则
        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <param name="options">表达式选项</param>
        /// <returns></returns>
        public ValidatorHelper IsPattern(string pattern,RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (this.Value.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsMatch(pattern, options))
            {
                this.Result.Add("参数[{0}]的值不符合规则 ' {1} '.".format(this.Name, pattern));
            }
            return this;
        }
        #endregion

        #region 验证是否相等
        /// <summary>
        /// 验证是否相等
        /// </summary>
        /// <param name="_">对比值</param>
        /// <returns></returns>
        public ValidatorHelper IsEquals(object _)
        {
            if (!this.Value.Equals(_))
            {
                this.Result.Add("参数[{0}]的值为 ' {1} ' 不等于 ' {2} '.".format(this.Name, this.Value, _));
            }
            
            return this;
        }
        #endregion

        #region 验证是否在某个区间
        /// <summary>
        /// 验证是否在某个区间
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public ValidatorHelper IsBetween(int min, int max)
        {
            if (min == max) return this;
            int val = this.Value.ToCast<int>();
            if (!(val >= min && val <= max))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不在 ' {2} ' 与 ' {3} ' 之间.".format(this.Name, this.Value, min, max));
            }
            return this;
        }
        /// <summary>
        /// 验证是否在某个区间
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public ValidatorHelper IsBetween(Int64 min,Int64 max)
        {
            if (min == max) return this;
            Int64 val = this.Value.ToCast<Int64>();
            if(val<min || val > max)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不在 ' {2} ' 与 ' {3} ' 之间.".format(this.Name, this.Value, min, max));
            }
            return this;
        }
        /// <summary>
        /// 验证是否在某个区间
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public ValidatorHelper IsBetween(float min, float max)
        {
            if (min == max) return this;
            float val = this.Value.ToCast<float>();
            if (val < min || val > max)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不在 ' {2} ' 与 ' {3} ' 之间.".format(this.Name, this.Value, min, max));
            }
            return this;
        }
        /// <summary>
        /// 验证是否在某个区间
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public ValidatorHelper IsBetween(double min, double max)
        {
            if (min == max) return this;
            double val = this.Value.ToCast<double>();
            if (val < min || val > max)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不在 ' {2} ' 与 ' {3} ' 之间.".format(this.Name, this.Value, min, max));
            }
            return this;
        }
        #endregion

        #region 是否开始存在某个字符
        /// <summary>
        /// 是否开始存在某个字符
        /// </summary>
        /// <param name="_">字符</param>
        /// <returns></returns>
        public ValidatorHelper StartsWith(string _)
        {
            if (_.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().StartsWith(_))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 开头不存在 ' {2} '.".format(this.Name, this.Value, _));
            }
            return this;
        }
        #endregion

        #region 是否结束存在某个字符
        /// <summary>
        /// 是否结束存在某个字符
        /// </summary>
        /// <param name="_">字符</param>
        /// <returns></returns>
        public ValidatorHelper EndsWith(string _)
        {
            if (_.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().StartsWith(_))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 结尾不存在 ' {2} '.".format(this.Name, this.Value, _));
            }
            return this;
        }
        #endregion

        #region 验证长度
        /// <summary>
        /// 验证长度
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public ValidatorHelper HasLength(int length)
        {
            int valueLength = 0;
            if (this.Value.IsNotNullOrEmpty()) valueLength = this.Value.ToString().Length;
            if (valueLength != length)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 长度不为 ' {2} '.".format(this.Name, this.Value, length));
            }
            return this;
        }
        /// <summary>
        /// 验证长度
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public ValidatorHelper HasNotLength(int length)
        {
            int valueLength = 0;
            if (this.Value.IsNotNullOrEmpty()) valueLength = this.Value.ToString().Length;
            if (valueLength == length)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 长度为 ' {2} '.".format(this.Name, this.Value, length));
            }
            return this;
        }
        #endregion

        #region 验证长度是否短于
        /// <summary>
        /// 验证长度是否短于
        /// </summary>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public ValidatorHelper IsShorter(int maxLength)
        {
            int valueLength = 0;
            if (this.Value.IsNotNullOrEmpty()) valueLength = this.Value.ToString().Length;
            if (!(valueLength < maxLength))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 的长度为 ' {2} ' 短于 ' {3} '.".format(this.Name, this.Value, valueLength, maxLength));
            }
            return this;
        }
        #endregion

        #region 验证长度是否长于
        /// <summary>
        /// 验证长度是否长于
        /// </summary>
        /// <param name="minLength">最短长度</param>
        /// <returns></returns>
        public ValidatorHelper IsLonger(int minLength)
        {
            int valueLength = 0;
            if (this.Value.IsNotNullOrEmpty()) valueLength = this.Value.ToString().Length;
            if (!(valueLength > minLength))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 的长度为 ' {2} ' 长于 ' {3} '.".format(this.Name, this.Value, valueLength, minLength));
            }
            return this;
        }
        #endregion

        #region 是否包含
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public ValidatorHelper Contains(string _)
        {
            if (_.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().Contains(_))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不包含 ' {2} '.".format(this.Name, this.Value, _));
            }
            return this;
        }
        #endregion

        #region 是否不包含
        /// <summary>
        /// 是否不包含
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public ValidatorHelper NotContains(string _)
        {
            if (this.Value.IsNullOrEmpty() || _.IsNotNullOrEmpty()) return this;
            if (this.Value.ToString().Contains(_))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 包含 ' {2} '.".format(this.Name, this.Value, _));
            }
            return this;
        }
        #endregion

        #endregion
    }
}