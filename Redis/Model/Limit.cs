/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-19 下午 09:32:16                       *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 索引位置
    /// </summary>
    public class Limit
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Limit() { }
        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        public Limit(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 开始索引
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 结束索引
        /// </summary>
        public int End { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}