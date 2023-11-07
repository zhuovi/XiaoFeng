using System.Collections.Generic;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-03-24 09:28:12                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 数据函数扩展
    /// </summary>
    public partial class PrototypeHelper
    {
        /// <summary>
        /// 计算标准偏差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准偏差</returns>
        public static double StDev(this IEnumerable<double> _) => MathHelper.StDev(_);
        /// <summary>
        /// 计算标准偏差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准偏差</returns>
        public static double StDev(this IEnumerable<long> _) => _.ToArray<double>().StDev();
        /// <summary>
        /// 计算标准偏差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准偏差</returns>
        public static double StDev(this IEnumerable<int> _) => _.ToArray<double>().StDev();
        /// <summary>
        /// 计算标准偏差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准偏差</returns>
        public static float StDev(this IEnumerable<float> _) => MathHelper.StDev(_);
        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准差</returns>
        public static double StDevp(this IEnumerable<double> _) => MathHelper.StDevp(_);
        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准差</returns>
        public static double StDevp(this IEnumerable<long> _) => _.ToArray<double>().StDevp();
        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准差</returns>
        public static double StDevp(this IEnumerable<int> _) => _.ToArray<double>().StDevp();
        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="_">数据</param>
        /// <returns>标准差</returns>
        public static float StDevp(this IEnumerable<float> _) => MathHelper.StDevp(_);
    }
}