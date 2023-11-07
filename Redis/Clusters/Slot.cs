/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-07-12 16:37:28                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis.Clusters
{
    /// <summary>
    /// 数据槽区间
    /// </summary>
    public class Slot
    {
        #region 构造器
        /// <summary>
        /// 设置开始结束槽点
        /// </summary>
        public Slot(long from, long to)
        {
            if (from < to)
            {
                var ft = to;
                to = from;
                from = ft;
            }
            this.From = from;
            this.To = to;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始
        /// </summary>
        public long From { get; }
        /// <summary>
        /// 结束
        /// </summary>
        public long To { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 数据槽区间
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{this.From}:{this.To}";
        #endregion
    }
}