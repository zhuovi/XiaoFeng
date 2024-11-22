using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-11-20 17:58:50                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 时间段集合
    /// </summary>
    public class TimePeriodCollection
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public TimePeriodCollection()
        {
            
        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="timePeriodType">时间段类型</param>
        public TimePeriodCollection(TimePeriodType timePeriodType)
        {
            TimePeriodType = timePeriodType;
        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="timePeriods">时间段</param>
        /// <param name="timePeriodType">时间段类型</param>
        public TimePeriodCollection(IEnumerable<ITimePeriod> timePeriods, TimePeriodType timePeriodType = TimePeriodType.OR) : this(timePeriodType)
        {
            if (this.Collections == null) this.Collections = new List<ITimePeriod>();
            if (timePeriods != null && !timePeriods.Any())
                this.Collections.AddRange(timePeriods);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 时间段集合
        /// </summary>
        private List<ITimePeriod> Collections { get; set; } = new List<ITimePeriod>();
        /// <summary>
        /// 长度
        /// </summary>
        public int Count => this.Collections == null ? 0 : this.Collections.Count;
        /// <summary>
        /// 时间段类型
        /// </summary>
        public TimePeriodType TimePeriodType { get; set; } = TimePeriodType.OR;
        #endregion

        #region 方法
        /// <summary>
        /// 添加时间段
        /// </summary>
        /// <param name="timePeriod">时间段</param>
        public void Add(ITimePeriod timePeriod)
        {
            if (this.Collections == null) this.Collections = new List<ITimePeriod>();
            this.Collections.Add(timePeriod);
        }
        /// <summary>
        /// 添加时间段
        /// </summary>
        /// <param name="timePeriods">时间段集合</param>
        public void AddRange(IEnumerable<ITimePeriod> timePeriods)
        {
            if (this.Collections == null) this.Collections = new List<ITimePeriod>();
            if (timePeriods == null || !timePeriods.Any()) return;
            this.Collections.AddRange(timePeriods);
        }
        /// <summary>
        /// 移除时间段
        /// </summary>
        /// <param name="timePeriod"></param>
        public Boolean Remove(ITimePeriod timePeriod)
        {
            if (this.Collections == null || this.Collections.Count == 0) return false;
            return this.Collections.Remove(timePeriod);
        }
        /// <summary>
        /// 移除时间段
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveAt(int index)
        {
            if (this.Collections == null || this.Collections.Count == 0) return;
            this.Collections.RemoveAt(index);
        }
        /// <summary>
        /// 清空时间段集合
        /// </summary>
        public void Clear()
        {
            if (this.Collections == null || this.Collections.Count == 0) return;
            this.Collections.Clear();
        }
        /// <summary>
        /// 是否在时间段内
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public Boolean IsBetween(DateTime time)
        {
            if (this.Collections == null || this.Collections.Count == 0) return true;
            if (this.TimePeriodType == TimePeriodType.OR)
            {
                foreach (var a in this.Collections)
                {
                    if (a.IsBetween(time)) return true;
                }
                return false;
            }
            else
            {
                foreach (var a in this.Collections)
                {
                    if (!a.IsBetween(time)) return false;
                }
                return true;
            }
        }
        #endregion
    }
    /// <summary>
    /// 时间段类型
    /// </summary>
    public enum TimePeriodType
    {
        /// <summary>
        /// 或 有一个时间段满足即为满足
        /// </summary>
        OR = 0,
        /// <summary>
        /// 与 所有时间段都满足即为满足
        /// </summary>
        AND = 1
    }
}