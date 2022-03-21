using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XiaoFeng
{
    /// <summary>
    /// 订单号生成类
    /// </summary>
    public class OrderNumber : EntityBase
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public OrderNumber() { }
        #endregion

        #region 属性
        /// <summary>
        /// 订单号共享锁
        /// </summary>
        private static readonly object OrderNumberLock = new object();
        /// <summary>
        /// 订单号累加器
        /// </summary>
        private static Int64 Counter = 1;
        /// <summary>
        /// 原来秒数
        /// </summary>
        private static Int64 OldTimer = 0;
        /// <summary>
        /// 老时间
        /// </summary>
        private static string OldTime = "";
        /// <summary>
        /// 订单号最大并发数
        /// </summary>
        public static int MaxCounter = 100;
        #endregion

        #region 方法
        /// <summary>
        /// 获取订单号
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            Monitor.Enter(OrderNumberLock);
            Int64 NewTimer = DateTime.Now.ToTimeStamps();

            if (NewTimer == OldTimer) { Counter++; if (Counter >= MaxCounter) Counter = 1; } else { OldTimer = NewTimer; Counter = 1; }
            string OrderNumber = NewTimer.ToString() + RandomHelper.GetRandomInt(100,1000) + Counter.ToString().PadLeft(MaxCounter.ToString().Length, '0');
            Monitor.Exit(OrderNumberLock);
            return OrderNumber;
        }
        /// <summary>
        /// 获取订单号
        /// </summary>
        /// <param name="format">时间格式 如yyyyMMdd</param>
        /// <returns></returns>
        public static string GetDateTime(string format= "yyyyMMddHHmmssfff")
        {
            Monitor.Enter(OrderNumberLock);
            string NewTime = DateTime.Now.ToString(format);

            if (NewTime == OldTime) { Counter++; if (Counter >= MaxCounter) Counter = 1; } else { OldTime = NewTime; Counter = 1; }
            string OrderNumber = NewTime + RandomHelper.GetRandomInt(100, 1000) + Counter.ToString().PadLeft(MaxCounter.ToString().Length, '0');
            Monitor.Exit(OrderNumberLock);

            return OrderNumber;
        }
        #endregion
    }
}