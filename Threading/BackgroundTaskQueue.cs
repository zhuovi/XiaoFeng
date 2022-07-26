using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Config;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-26 15:35:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 任务队列
    /// </summary>
    public class BackgroundTaskQueue : TaskServiceQueue<Func<CancellationToken, Task>>, IBackgroundTaskQueue
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BackgroundTaskQueue() : base(nameof(BackgroundTaskQueue))
        {
            this.Setting = XiaoFeng.Config.Setting.Current;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        private ISetting Setting { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public Task AddWorkItem(Action action)
        {
            return this.AddWorkItem(action, CancellationToken.None);
        }
        ///<inheritdoc/>
        public Task AddWorkItem(Action action, CancellationToken cancel, TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return base.AddWorkItem(c => new Task(action, cancel == CancellationToken.None ? c : CancellationTokenSource.CreateLinkedTokenSource(c, cancel).Token, creationOptions));
        }
        ///<inheritdoc/>
        public Task AddWorkItem(Action<object> action, object state)
        {
            return this.AddWorkItem(action, state, CancellationToken.None);
        }
        ///<inheritdoc/>
        public Task AddWorkItem(Action<object> action, object state, CancellationToken cancel, TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
           return base.AddWorkItem(c => new Task(action, state, cancel == CancellationToken.None ? c : CancellationTokenSource.CreateLinkedTokenSource(c, cancel).Token, creationOptions));
        }
        ///<inheritdoc/>
        public override Task ExecuteAsync(Func<CancellationToken, Task> workItem, CancellationToken cancellationToken)
        {
            var task = workItem.Invoke(cancellationToken);
            //Console.WriteLine($"任务-{task.Id}-开始-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}");
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
                task.ContinueWith(t =>
                {
                    //Console.WriteLine($"任务-{task.Id}-已完成-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}");
                }).Wait(TimeSpan.FromSeconds(Setting.TaskWaitTimeout));
            }
            else if (task.Status == TaskStatus.WaitingToRun || task.Status == TaskStatus.WaitingForActivation || task.Status == TaskStatus.Running)
            {
                task.ContinueWith(t =>
                {
                    //Console.WriteLine($"任务-{task.Id}-已完成-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}");
                }).Wait(TimeSpan.FromSeconds(Setting.TaskWaitTimeout));
            }
            return task;
        }
        ///<inheritdoc/>
        Task IBackgroundTaskQueue.Stop()
        {
            return base.StopAsync();
        }
        #endregion
    }
}