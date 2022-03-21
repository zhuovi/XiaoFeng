using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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