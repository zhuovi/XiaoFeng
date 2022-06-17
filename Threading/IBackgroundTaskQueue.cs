using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-08-19 15:54:08                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 队列任务 接口
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 加入任务
        /// </summary>
        /// <param name="action">事件</param>
        Task AddWorkItem(Action action);
        /// <summary>
        /// 加入任务
        /// </summary>
        /// <param name="action">事件</param>
        /// <param name="cancel">取消通知</param>
        /// <param name="creationOptions">任务配置</param>
        Task AddWorkItem(Action action, CancellationToken cancel, TaskCreationOptions creationOptions = TaskCreationOptions.None);
        /// <summary>
        /// 加入任务
        /// </summary>
        /// <param name="action">事件</param>
        /// <param name="state">数据对象</param>
        Task AddWorkItem(Action<object> action, object state);
        /// <summary>
        /// 加入任务
        /// </summary>
        /// <param name="action">事件</param>
        /// <param name="state">数据对象</param>
        /// <param name="cancel">取消通知</param>
        /// <param name="creationOptions">任务配置</param>
        Task AddWorkItem(Action<object> action, object state, CancellationToken cancel, TaskCreationOptions creationOptions = TaskCreationOptions.None);
        /// <summary>
        /// 加入任务
        /// </summary>
        /// <param name="workItem">任务</param>
        Task AddWorkItem(Func<CancellationToken, Task> workItem);
        /// <summary>
        /// 停止消费任务
        /// </summary>
        Task Stop();
        #endregion
    }
}