using System;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-09 17:05:26                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 区间值
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class RangeAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 设置区间值
        /// </summary>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        public RangeAttribute(double start, double end)
        {
            Start = start;
            End = end;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始值
        /// </summary>
        public double Start { get; set; }
        /// <summary>
        /// 结束值
        /// </summary>
        public double End { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}