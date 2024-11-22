using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-11-20 19:27:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 周时间段
    /// </summary>
    public class WeekPeriod:ITimePeriod
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public WeekPeriod()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="begin">开始星期 比如周一、周二 周日为0依次类推</param>
        /// <param name="end">结束星期 比如周一、周二 周日为0依次类推</param>
        public WeekPeriod(int begin, int end)
        {
            this.Begin = begin;
            this.End = end;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始日 比如周一、周二 周日为0依次类推
        /// </summary>
        public int Begin { get; set; }
        /// <summary>
        /// 结束日 比如周一、周二 周日为0依次类推
        /// </summary>
        public int End { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public bool IsBetween(DateTime time, long deviation = 1000)
        {
            var day = (int)time.DayOfWeek;
            if (day >= this.Begin && day <= this.End) return true;
            return false;
        }
        #endregion
    }
}