using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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