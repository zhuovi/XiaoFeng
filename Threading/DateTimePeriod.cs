using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-11-20 17:30:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 作业时间段
    /// </summary>
    public class DateTimePeriod : ITimePeriod
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public DateTimePeriod()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        public DateTimePeriod(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }
        #endregion

        #region 属性
        private DateTime _Begin;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Begin { get; set; }
        private DateTime _End;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public Boolean IsBetween(DateTime time, long deviation = 1000)
        {
            var cBegin = (time - this.Begin).TotalSeconds;
            if (cBegin < 0) return false;
            var cEnd = (this.End - time).TotalSeconds;
            if (cEnd < 0) return false;
            return true;
        }
        #endregion
    }
}