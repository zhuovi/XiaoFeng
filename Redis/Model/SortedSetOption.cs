using System.Collections.Generic;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-18 14:33:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 有序集合合并选项
    /// </summary>
    public class SortedSetOptions
    {
        /// <summary>
        /// 有序集合key和权重
        /// </summary>
        public Dictionary<string, int> KeyAndWeights { get; set; }
        /// <summary>
        /// 聚合联合类型
        /// </summary>
        public AggregateType Aggregate { get; set; } = AggregateType.SUM;
        /// <summary>
        /// 生成参数
        /// </summary>
        /// <returns></returns>
        public object[] ToArgments()
        {
            var list = new List<object>();
            list.Add(this.KeyAndWeights.Count);
            this.KeyAndWeights.Keys.Each(k => list.Add(k));
            list.Add("WEIGHTS");
            this.KeyAndWeights.Values.Each(v => list.Add(v));
            list.Add("AGGREGATE");
            list.Add(this.Aggregate.ToString());
            return list.ToArray();
        }
    }
}