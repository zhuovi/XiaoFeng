using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-11-20 19:24:04                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 天间隔
    /// </summary>
    public class DayPeriod:ITimePeriod
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public DayPeriod()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="begin">开始日 比如一号、二号</param>
        /// <param name="end">结束日 比如一号、二号</param>
        public DayPeriod(int begin,int end)
        {
            this.Begin = begin;
            this.End = end;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始日  比如一号、二号
        /// </summary>
        public int Begin { get; set; }
        /// <summary>
        /// 结束日 比如一号、二号
        /// </summary>
        public int End { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public bool IsBetween(DateTime time, long deviation = 1000)
        {
            var day = time.Day;
            if (day >= this.Begin && day <= this.End) return true;
            return false;
        }
        #endregion
    }
}