using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-01 16:59:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 任务队列池
    /// </summary>
    public class TaskPool
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TaskPool()
        {

        }
        /// <summary>
        /// 设置最大线程数
        /// </summary>
        /// <param name="MaxCount">最大线程数</param>
        public TaskPool(int MaxCount)
        {
            MaxCount = MaxCount <= 0 ? 1 : MaxCount;
            this.MaxTaskCount = MaxCount;
            this.Manual = new SemaphoreSlim(MaxCount, MaxCount);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前排队等待处理的工作项的数量。
        /// </summary>
        public int PendingWorkItemCount { get { return this.TaskQueue.Count; } }
        /// <summary>
        /// 完成工作项数
        /// </summary>
        public int CompletedWorkItemCount { get { return this._CompletedWorkItemCount; } }
        /// <summary>
        /// 完成工作项数
        /// </summary>
        private int _CompletedWorkItemCount = 0;
        /// <summary>
        /// 最大任务数
        /// </summary>
        public int MaxTaskCount { get; set; } = 1;
        /// <summary>
        /// 正在运行的任务数
        /// </summary>
        public int TaskingCount { get { return this.Manual.CurrentCount; } }
        /// <summary>
        /// 任务队列
        /// </summary>
        public ConcurrentQueue<Func<CancellationToken, Task>> TaskQueue { get; set; } = new ConcurrentQueue<Func<CancellationToken, Task>>();
        /// <summary>
        /// 执行任务列表
        /// </summary>
        public List<Task> TaskList { get; set; } = new List<Task>();
        /// <summary>
        /// 消费运行状态
        /// </summary>
        private Boolean ConsumeState { get; set; } = false;
        /// <summary>
        /// 取消信号
        /// </summary>
        public CancellationTokenSource CancelToken { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// 线程同步信号
        /// </summary>
        private SemaphoreSlim Manual = new SemaphoreSlim(1, 3);
        #endregion

        #region 方法

        #region 添加任务
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task">任务</param>
        public Task QueueUserWorkItem(Func<CancellationToken, Task> task)
        {
            this.TaskQueue.Enqueue(task);
            return this.Wake();
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="action">委托</param>
        /// <returns></returns>
        public Task QueueUserWorkItem(Action action)
        {
            this.TaskQueue.Enqueue(c => Task.Run(action, c));
            return this.Wake();
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="action">委托</param>
        /// <param name="state">数据对象</param>
        /// <returns></returns>
        public Task QueueUserWorkItem(Action<object> action, object state)
        {
            this.TaskQueue.Enqueue(c => Task.Factory.StartNew(action, state, c));
            return this.Wake();
        }
        #endregion

        #region 唤醒线程去执行任务
        /// <summary>
        /// 唤醒线程去执行任务
        /// </summary>
        public Task Wake()
        {
            while (true)
            {
                if (this.TaskQueue.TryDequeue(out var task))
                {
                    Task.Run(() =>
                    {
                        this.Execute(task).ConfigureAwait(false);
                    });
                }
                else break;
            }
            return Task.CompletedTask;
        }
        #endregion

        #region 执行任务
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task">任务</param>
        public async Task Execute(Func<CancellationToken, Task> task)
        {
            Manual.Wait(this.CancelToken.Token);
            var _task = task.Invoke(this.CancelToken.Token);
            Task.WaitAll(_task);
            Interlocked.Increment(ref this._CompletedWorkItemCount);
            Manual.Release();
            await Task.CompletedTask;
        }
        #endregion

        #region 取消
        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            this.CancelToken.Cancel();
        }
        #endregion

        #endregion
    }
}