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
    /// <summary>
    /// 区间结构
    /// </summary>
    public class BetweenValue
    {
        /// <summary>
        /// 设置最大最小值
        /// </summary>
        /// <param name="MinValue">最小值</param>
        /// <param name="MaxValue">最大值</param>
        public BetweenValue(double MinValue, double MaxValue)
        {
            this.MaxValue = MaxValue;this.MinValue = MinValue;
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue { get; set; }
    }
}