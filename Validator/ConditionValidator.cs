using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using XiaoFeng;
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
    /// Version : 1.0.2
    /// Description:
    /// </summary>
    public class ConditionValidator
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConditionValidator() {  }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="format">规则</param>
        public ConditionValidator(string name, object value, ValidateFormat format = null) : this()
        {
            this.Name = name; this.Value = value; 
            if (format == null) return;
            this.ValidateFormat(format);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 验证结果
        /// </summary>
        public List<string> Result { get; set; } = new List<string>();
        /// <summary>
        /// 是否验证
        /// </summary>
        public Boolean IsValid { get { return this.Result.Count == 0; } }
        /// <summary>
        /// 是否必填 true为必填  false为非必填
        /// </summary>
        private Boolean IsMust { get; set; } = false;
        #endregion

        #region 方法

        #region 验证规则
        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="format">格式</param>
        private void ValidateFormat(ValidateFormat format)
        {
            this.IsMust = format.IsMust;
            if (this.IsMust) format.IsNull = false;
            if (format.IsNull != null)
                if (format.IsNull.Value) this.IsNullOrEmpty(); else this.IsNotNullOrEmpty();
            if (format.IsNullOrWhiteSpace != null)
                if (format.IsNullOrWhiteSpace.Value) this.IsNullOrWhiteSpace(); else this.IsNotNullOrWhiteSpace();
            if (format.Contains != null && format.Contains.IsNotNullOrEmpty()) this.Contains(format.Contains);
            if (format.NotContains != null && format.NotContains.IsNotNullOrEmpty()) this.NotContains(format.NotContains);
            if (format.StartsWith != null && format.StartsWith.IsNotNullOrEmpty()) this.StartsWith(format.StartsWith);
            if (format.EndsWith != null && format.EndsWith.IsNotNullOrEmpty()) this.EndsWith(format.EndsWith);
            if (format.HasLength != null) this.HasLength(format.HasLength.Value);
            if (format.HasNotLength != null) this.HasNotLength(format.HasNotLength.Value);
            if (format.MinLength != null) this.IsShorter(format.MinLength.Value);
            if (format.MaxLength != null) this.IsLonger(format.MaxLength.Value);
            if (format.Min != null && format.Max != null && format.Max >= format.Min) this.IsBetween(format.Min.Value, format.Max.Value);
            else
            {
                if (format.Min != null) this.IsLessThan(format.Min.Value);
                if (format.Max != null) this.IsMoreThan(format.Max.Value);
            }
            if (format.Between != null)
            {
                if (format.Between.MaxValue >= format.Between.MinValue) this.IsBetween(format.Between.MinValue, format.Between.MaxValue);
            }
            if (format.EqualsTo != null) this.IsEqualsTo(format.EqualsTo);
            if (format.NotEqualsTo != null) this.IsNotEqualsTo(format.NotEqualsTo);
            if (format.IsPattern != null && format.IsPattern.IsNotNullOrEmpty()) this.IsPattern(format.IsPattern);
            if (format.IsPhone != null) this.IsPhone();
            if (format.IsTel != null) this.IsTel();
            if (format.IsSite != null) this.IsSite();
            if (format.IsNumberic != null) this.IsNumberic();
            if (format.IsIp != null) this.IsIp();
            if (format.IsFloat != null) this.IsFloat();
            if (format.IsGuid != null) this.IsGuid();
            if (format.IsEmail != null) this.IsEmail();
            if (format.IsDate != null) this.IsDate();
            if (format.IsTime != null) this.IsTime();
            if (format.IsDateTime != null) this.IsDateTime();
            if (format.IsDateOrTime != null) this.IsDateOrTime();
            if (format.IsBoolean != null) this.IsBoolean();
        }
        #endregion

        #region 验证参数
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public ConditionValidator Requires(string name, object value)
        {
            var validator = new ConditionValidator(name, value);
            validator.Result.AddRange(this.Result);
            return validator;
        }
        #endregion

        #region 验证参数是否为空
        /// <summary>
        /// 验证参数是否为空
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsNullOrEmpty()
        {
            if (!(this.Value is Guid) && !this.Value.IsNullOrEmpty())
            {
                this.Result.Add("参数[{0}]的值不为空.".format(this.Name));
            }
            return this;
        }
        /// <summary>
        /// 验证参数是否不为空
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsNotNullOrEmpty()
        {
            if (!(this.Value is Guid) && this.Value.IsNullOrEmpty())
            {
                this.Result.Add("参数[{0}]的值为空.".format(this.Name));
            }
            return this;
        }
        /// <summary>
        /// 验证参数是否为空
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsNullOrWhiteSpace()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (this.Value.ToString().IsNullOrWhiteSpace())
            {
                this.Result.Add("参数[{0}]的值不为空或不为空格.".format(this.Name));
            }
            return this;
        }
        /// <summary>
        /// 验证参数是否不为空
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsNotNullOrWhiteSpace()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (this.Value.ToString().IsNullOrWhiteSpace())
            {
                this.Result.Add("参数[{0}]的值为空或空格.".format(this.Name));
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
        public ConditionValidator IsPattern(string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if ((!this.IsMust && this.Value.IsNullOrEmpty()) || pattern.IsNullOrEmpty()) return this;
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
        public ConditionValidator IsEqualsTo(object _)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (this.Value.ToString() != _.ToString())
            {
                this.Result.Add("参数[{0}]的值为 ' {1} ' 不等于 ' {2} '.".format(this.Name, this.Value, _));
            }
            return this;
        }
        #endregion

        #region 验证是否不相等
        /// <summary>
        /// 验证是否不相等
        /// </summary>
        /// <param name="_">对比值</param>
        /// <returns></returns>
        public ConditionValidator IsNotEqualsTo(object _)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (this.Value.ToString() == _.ToString())
            {
                this.Result.Add("参数[{0}]的值为 ' {1} ' 等于 ' {2} '.".format(this.Name, this.Value, _));
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
        public ConditionValidator IsBetween(double min, double max)
        {
            if ((!this.IsMust && this.Value.IsNullOrEmpty()) || min > max) return this;
            if (!this.Value.ToString().IsFloat()) return this.IsFloat();
            double val = this.Value.ToCast<double>();
            if (val < min || val > max)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不在 ' {2} ' 与 ' {3} ' 之间.".format(this.Name, this.Value, min, max));
            }
            return this;
        }
        #endregion

        #region 验证是否少于
        /// <summary>
        /// 验证是否少于
        /// </summary>
        /// <param name="min">最小值</param>
        /// <returns></returns>
        public ConditionValidator IsLessThan(double min)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsFloat()) return this.IsFloat();
            double val = this.Value.ToCast<double>();
            if (val < min)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不少于 ' {2} '.".format(this.Name, this.Value, min));
            }
            return this;
        }
        #endregion

        #region 验证是否大于
        /// <summary>
        /// 验证是否大于
        /// </summary>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public ConditionValidator IsMoreThan(double max)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsFloat()) return this.IsFloat();
            double val = this.Value.ToCast<double>();
            if (val > max)
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不大于 ' {2} '.".format(this.Name, this.Value, max));
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
        public ConditionValidator StartsWith(string _)
        {
            if ((!this.IsMust && this.Value.IsNullOrEmpty()) || _.IsNullOrEmpty()) return this;
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
        public ConditionValidator EndsWith(string _)
        {
            if ((!this.IsMust && this.Value.IsNullOrEmpty()) || _.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().EndsWith(_))
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
        public ConditionValidator HasLength(int length)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
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
        public ConditionValidator HasNotLength(int length)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
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
        public ConditionValidator IsShorter(int maxLength)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            int valueLength = 0;
            if (this.Value.IsNotNullOrEmpty()) valueLength = this.Value.ToString().Length;
            if (valueLength < maxLength)
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
        public ConditionValidator IsLonger(int minLength)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            int valueLength = 0;
            if (this.Value.IsNotNullOrEmpty()) valueLength = this.Value.ToString().Length;
            if (valueLength > minLength)
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
        public ConditionValidator Contains(string _)
        {
            if ((!this.IsMust && this.Value.IsNullOrEmpty()) || _.IsNullOrEmpty()) return this;
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
        public ConditionValidator NotContains(string _)
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (this.Value.ToString().Contains(_))
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 包含 ' {2} '.".format(this.Name, this.Value, _));
            }
            return this;
        }
        #endregion

        #region 验证是否是数字类型
        /// <summary>
        /// 验证是否是数字
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsNumberic()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsNumberic())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[数字]类型.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是浮点类型
        /// <summary>
        /// 验证是否是数字
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsFloat()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsFloat())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[浮点]类型.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是手机号格式
        /// <summary>
        /// 验证是否是手机号格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsPhone()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsPhone())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[手机号码]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是座机号码格式
        /// <summary>
        /// 验证是否是座机号码格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsTel()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsTel())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[座机号码]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是网址格式
        /// <summary>
        /// 验证是否是网址格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsSite()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsSite())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[网址]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是Email格式
        /// <summary>
        /// 验证是否是Email格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsEmail()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsEmail())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[Email]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是日期格式
        /// <summary>
        /// 验证是否是日期格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsDate()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsDate())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[日期]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是时间格式
        /// <summary>
        /// 验证是否是时间格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsTime()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsTime())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[时间]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是全日期时间格式
        /// <summary>
        /// 验证是否是全日期时间格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsDateTime()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsDateTime())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[全时间]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是日期格式
        /// <summary>
        /// 验证是否是全日期时间格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsDateOrTime()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().Trim().IsDateOrTime())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[日期]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是Guid格式
        /// <summary>
        /// 验证是否是Guid格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsGuid()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsGUID())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[Guid]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是IP格式
        /// <summary>
        /// 验证是否是IP格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsIp()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsIP())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[IP]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #region 验证是否是布尔格式
        /// <summary>
        /// 验证是否是布尔格式
        /// </summary>
        /// <returns></returns>
        public ConditionValidator IsBoolean()
        {
            if (!this.IsMust && this.Value.IsNullOrEmpty()) return this;
            if (!this.Value.ToString().IsBoolean())
            {
                this.Result.Add("参数[{0}]的值 ' {1} ' 不是[Boolean]格式.".format(this.Name, this.Value));
            }
            return this;
        }
        #endregion

        #endregion
    }
}