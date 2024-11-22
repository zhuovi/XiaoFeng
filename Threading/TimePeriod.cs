using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-11-20 16:32:02                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 作业时间段
    /// </summary>
    public class TimePeriod:ITimePeriod
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public TimePeriod()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        public TimePeriod(Time begin,Time end)
        {
            this.Begin = begin;
            this.End = end;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始时间
        /// </summary>
        public Time Begin { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public Time End { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public Boolean IsBetween(DateTime time, long deviation = 1000)
        {
            var cTime = new Time(time);
            var cBegin = cTime.TotalSeconds - this.Begin.TotalSeconds;
            if (cBegin < 0) return false;
            var cEnd = this.End.TotalSeconds - cTime.TotalSeconds;
            if (cEnd < 0) return false;
            return true;
        }
        #endregion
    }
}