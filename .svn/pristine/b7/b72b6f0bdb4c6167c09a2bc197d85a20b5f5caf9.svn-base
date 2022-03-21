using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/4/29 16:05:45                            *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Always)]
    public static class LogFactory
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        static LogFactory()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, WeakReference<ILog>> Data = new ConcurrentDictionary<string, WeakReference<ILog>>();
        #endregion

        #region 方法
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">日志名称</param>
        /// <returns></returns>
        public static ILog Create(Type type, string name = "")
        {
            var key = name + type.AssemblyQualifiedName;
            if (Data.ContainsKey(key))
            {
                var log = Data[key];
                if (log == null)
                    log = new WeakReference<ILog>(Activator.CreateInstance(type) as ILog);
                if (log.IsAlive)
                    return log.Target;
                else
                    return log.Target = Activator.CreateInstance(type) as ILog;
            }
            else
            {
                var log = new WeakReference<ILog>(Activator.CreateInstance(type) as ILog);
                Data.TryAdd(key, log);
                return log.Target;
            }
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static ILog Create<T>(string name = "") where T : new() => Create(typeof(T), name);
        #endregion
    }
}