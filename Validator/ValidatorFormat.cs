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
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Validator
{
    #region 验证规则
    /// <summary>
    /// 验证规则
    /// </summary>
    public class ValidateFormat
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ValidateFormat() { this.IsMust = false; }
        #endregion

        #region 属性
        /// <summary>
        /// 是否必填
        /// </summary>
        public Boolean IsMust { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public Boolean? IsNull { get; set; }
        /// <summary>
        /// 是否为空或空格组成
        /// </summary>
        public Boolean? IsNullOrWhiteSpace { get; set; }
        /// <summary>
        /// 最小长度
        /// </summary>
        public int? MinLength { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int? MaxLength { get; set; }
        /// <summary>
        /// 是否指定长度
        /// </summary>
        public int? HasLength { get; set; }
        /// <summary>
        /// 是否不是指定长度
        /// </summary>
        public int? HasNotLength { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double? Min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public double? Max { get; set; }
        /// <summary>
        /// 是否包含
        /// </summary>
        public string Contains { get; set; }
        /// <summary>
        /// 是否不包含
        /// </summary>
        public string NotContains { get; set; }
        /// <summary>
        /// 是否匹配规则
        /// </summary>
        public string IsPattern { get; set; }
        /// <summary>
        /// 开始包含
        /// </summary>
        public string StartsWith { get; set; }
        /// <summary>
        /// 结尾包含
        /// </summary>
        public string EndsWith { get; set; }
        /// <summary>
        /// 是否等于
        /// </summary>
        public object EqualsTo { get; set; }
        /// <summary>
        /// 是否不等于
        /// </summary>
        public object NotEqualsTo { get; set; }
        /// <summary>
        /// 是否在指定区间
        /// </summary>
        public BetweenValue Between { get; set; }
        /// <summary>
        /// 是否是数字
        /// </summary>
        public Boolean? IsNumberic { get; set; }
        /// <summary>
        /// 是否是浮点
        /// </summary>
        public Boolean? IsFloat { get; set; }
        /// <summary>
        /// 是否是手机
        /// </summary>
        public Boolean? IsPhone { get; set; }
        /// <summary>
        /// 是否是固话
        /// </summary>
        public Boolean? IsTel { get; set; }
        /// <summary>
        /// 是否是网址
        /// </summary>
        public Boolean? IsSite { get; set; }
        /// <summary>
        /// 是否是Email
        /// </summary>
        public Boolean? IsEmail { get; set; }
        /// <summary>
        /// 是否是日期
        /// </summary>
        public Boolean? IsDate { get; set; }
        /// <summary>
        /// 是否是时间
        /// </summary>
        public Boolean? IsTime { get; set; }
        /// <summary>
        /// 是否是全日期
        /// </summary>
        public Boolean? IsDateTime { get; set; }
        /// <summary>
        /// 是否是日期
        /// </summary>
        public Boolean? IsDateOrTime { get; set; }
        /// <summary>
        /// 是否是Guid
        /// </summary>
        public Boolean? IsGuid { get; set; }
        /// <summary>
        /// 是否是IP
        /// </summary>
        public Boolean? IsIp { get; set; }
        /// <summary>
        /// 是否是Boolean
        /// </summary>
        public Boolean? IsBoolean { get; set; }
        #endregion
    }
    /// <summary>
    /// 验证规则
    /// </summary>
    public class ValidatorFormat : ValidateFormat
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ValidatorFormat() { }
        #endregion

        #region 属性
        /// <summary>
        /// 是否为空
        /// </summary>
        public new Boolean IsNull { get; set; }
        /// <summary>
        /// 是否为空或空格组成
        /// </summary>
        public new Boolean IsNullOrWhiteSpace { get; set; }
        /// <summary>
        /// 最小长度
        /// </summary>
        public new int MinLength { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public new int MaxLength { get; set; }
        /// <summary>
        /// 是否指定长度
        /// </summary>
        public new int HasLength { get; set; }
        /// <summary>
        /// 是否不是指定长度
        /// </summary>
        public new int HasNotLength { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public new double Min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public new double Max { get; set; }
        /// <summary>
        /// 是否是数字
        /// </summary>
        public new Boolean IsNumberic { get; set; }
        /// <summary>
        /// 是否是浮点
        /// </summary>
        public new Boolean IsFloat { get; set; }
        /// <summary>
        /// 是否是手机
        /// </summary>
        public new Boolean IsPhone { get; set; }
        /// <summary>
        /// 是否是固话
        /// </summary>
        public new Boolean IsTel { get; set; }
        /// <summary>
        /// 是否是网址
        /// </summary>
        public new Boolean IsSite { get; set; }
        /// <summary>
        /// 是否是Email
        /// </summary>
        public new Boolean IsEmail { get; set; }
        /// <summary>
        /// 是否是日期
        /// </summary>
        public new Boolean IsDate { get; set; }
        /// <summary>
        /// 是否是时间
        /// </summary>
        public new Boolean IsTime { get; set; }
        /// <summary>
        /// 是否是全日期
        /// </summary>
        public new Boolean IsDateTime { get; set; }
        /// <summary>
        /// 是否是日期
        /// </summary>
        public new Boolean IsDateOrTime { get; set; }
        /// <summary>
        /// 是否是Guid
        /// </summary>
        public new Boolean IsGuid { get; set; }
        /// <summary>
        /// 是否是IP
        /// </summary>
        public new Boolean IsIp { get; set; }
        /// <summary>
        /// 是否是Boolean
        /// </summary>
        public new Boolean IsBoolean { get; set; }
        #endregion
    }
    #endregion
}