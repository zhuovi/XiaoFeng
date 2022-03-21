using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/19 9:10:24          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 同步类
    /// </summary>
    public class Synchronized
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Synchronized()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 排它锁
        /// </summary>
        private static readonly Mutex Mutex = new Mutex();
        #endregion

        #region 方法
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="action">方法</param>
        public static void Run(Action action)
        {
            Mutex.WaitOne();
            action.Invoke();
            Mutex.ReleaseMutex();
        }
        /// <summary>
        /// 同步执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="func">方法</param>
        /// <returns></returns>
        public static T Run<T>(Func<T> func)
        {
            Mutex.WaitOne();
            var t = func.Invoke();
            Mutex.ReleaseMutex();
            return t;
        }
        #endregion
    }
}