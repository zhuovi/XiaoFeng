using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using XiaoFeng.Threading;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-26 08:36:46                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志消息队列
    /// </summary>
    public class LogTaskQueue : TaskServiceQueue<LogData>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public LogTaskQueue()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 日志记录器
        /// </summary>
        private Logger Logger { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="workItem">数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public override Task ExecuteAsync(LogData workItem, CancellationToken cancellationToken)
        {
            if (Logger == null) Logger = new Logger();
            Logger.Write(workItem);
            return Task.CompletedTask;
        }
        #endregion
    }
}