using System;
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
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 执行时间类
    /// </summary>
    public class StopWatch
    {
        /// <summary>
        /// 获取运行时长
        /// </summary>
        /// <param name="action">方法</param>
        /// <returns></returns>
        public static long GetTime(Action action)
        {
            var s = System.Diagnostics.Stopwatch.StartNew();
            action.Invoke();
            s.Stop();
            return s.ElapsedMilliseconds;
        }
        /// <summary>
        /// 异步获取运行时长
        /// </summary>
        /// <param name="action">方法</param>
        /// <returns></returns>
        public static async Task<long> GetTimeAsync(Action action) => await Task.Run<long>(() => GetTime(action));
    }
}