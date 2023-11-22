using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.Event;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-26 10:35:34                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 任务服务队列
    /// </summary>
    public abstract class TaskServiceQueue<T> : Disposable, ITaskServiceQueue<T>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TaskServiceQueue() { this.QueueName = $"TaskServiceQueue<{typeof(T).Name}>"; }
        /// <summary>
        /// 设置取消指令
        /// </summary>
        /// <param name="cancellationToken">取消指令</param>
        public TaskServiceQueue(CancellationToken cancellationToken)
        {
            this.CancelToken = cancellationToken;
        }
        /// <summary>
        /// 设置队列名称
        /// </summary>
        /// <param name="queueName">队列名称</param>
        public TaskServiceQueue(string queueName)
        {
            this.QueueName = queueName;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="cancellationToken">取消指令</param>
        public TaskServiceQueue(string queueName, CancellationToken cancellationToken)
        {
            this.QueueName = queueName;
            this.CancelToken = cancellationToken;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="maxCount"></param>
        /// <param name="cancellationToken"></param>
        public TaskServiceQueue(string queueName, int maxCount, CancellationToken cancellationToken)
        {
            this.QueueName = queueName;
            this.MaxCount = maxCount;
            this.CancelToken = cancellationToken;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 最大运行线程数
        /// </summary>
        private int _MaxCount = 1;
        /// <summary>
        /// 最大运行线程数
        /// </summary>
        private int MaxCount
        {
            get
            {
                if (this._MaxCount <= 0 || this._MaxCount >= 1000)
                    this._MaxCount = 1;
                return this._MaxCount;
            }
            set => this._MaxCount = value;
        }
        /// <summary>
        /// 队列名称
        /// </summary>
        public virtual string QueueName { get; private set; }
        /// <summary>
        /// 日志队列
        /// </summary>
        private ConcurrentQueue<T> QueueData = new ConcurrentQueue<T>();
        /// <summary>
        /// 线程同步信号
        /// </summary>
        private ManualResetEventSlim Manual = new ManualResetEventSlim(false);
        /// <summary>
        /// 线程数锁
        /// </summary>
        private SemaphoreSlim Slim { get; set; } = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 传播取消通知
        /// </summary>
        private CancellationToken CancelToken { get; set; } = CancellationToken.None;
        /// <summary>
        /// 取消事件
        /// </summary>
        private CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
        /// <summary>
        /// 消费运行状态
        /// </summary>
        public int ConsumeState = 0;
        /// <summary>
        /// 队列是否为空
        /// </summary>
        public Boolean IsEmpty => this.QueueData.IsEmpty;
        /// <summary>
        /// 队列包含元素数量
        /// </summary>
        public int Count => this.QueueData.Count;
        /// <summary>
        /// 任务错误事件
        /// </summary>
        public event TaskQueueError<T> TaskQueueError;
        /// <summary>
        /// 任务成功事件
        /// </summary>
        public event TaskQueueOk<T> TaskQueueOk;
        /// <summary>
        /// 任务队列空事件
        /// </summary>
        public event TaskQueueEmpty<T> TaskQueueEmpty;
        /// <summary>
        /// 配置
        /// </summary>
        private ISetting _Setting;
        /// <summary>
        /// 配置
        /// </summary>
        private ISetting Setting
        {
            get => _Setting ?? (_Setting = XiaoFeng.Config.Setting.Current);
            set => _Setting = value;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 是否存在于队列
        /// </summary>
        /// <param name="t">对象</param>
        /// <returns></returns>
        public virtual Boolean Contains(T t) => QueueData.Contains(t);
        /// <summary>
        /// 加入到队列前边
        /// </summary>
        /// <param name="t">对象</param>
        /// <returns></returns>
        public virtual async Task PrependWorkItem(T t)
        {
            QueueData.Prepend(t);
            Wake();
            await Task.CompletedTask;
        }
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="t">对象</param>
        public virtual async Task AddWorkItem(T t)
        {
            QueueData.Enqueue(t);
            Wake();
            await Task.CompletedTask;
        }
        /// <summary>
        /// 加入到队列前边
        /// </summary>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public virtual async Task PrependWorkItem(Func<T> func)
        {
            if (func != null) await PrependWorkItem(func.Invoke());
        }
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="func">委托</param>
        public virtual async Task AddWorkItem(Func<T> func)
        {
            if (func != null) await AddWorkItem(func.Invoke());
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="workItem">数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(T workItem, CancellationToken cancellationToken);
        /// <summary>
        /// 取出队列中数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T> DequeueAsync()
        {
            Manual.Wait(TimeSpan.FromSeconds(Setting.IdleSeconds <= 0 ? 0 : Setting.IdleSeconds));
            if (QueueData.TryDequeue(out var workItem))
            {
                return await Task.FromResult(workItem);
            }
            else
            {
                Manual.Reset();
                return default;
            }
        }
        /// <summary>
        /// 唤醒消费
        /// </summary>
        void Wake()
        {
            Manual.Set();
            if (Interlocked.CompareExchange(ref ConsumeState, 1, 0) == 0)
            {
                this.Slim = new SemaphoreSlim(this.MaxCount, this.MaxCount);
                this.Manual = new ManualResetEventSlim(false);
                if (this.CancelTokenSource == null || this.CancelTokenSource.IsCancellationRequested) this.CancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.CancelToken);
                this.CancelTokenSource.Token.Register(() =>
                {
                    Interlocked.CompareExchange(ref ConsumeState, 0, 1);
                });
                this.ConsoleWrite($"-- 有新任务,启动消费任务[{this.QueueName}]. --");
                this.ConsumeTask();
            }
        }
        /// <summary>
        /// 消费任务线程
        /// </summary>
        void ConsumeTask()
        {
            new Task(async () =>
            {
                if (this.QueueName.IsNotNullOrEmpty())
                    Thread.CurrentThread.Name = this.QueueName;
                while (!this.CancelTokenSource.Token.IsCancellationRequested)
                {
                    if (this.MaxCount > 1)
                        await Slim.WaitAsync(TimeSpan.FromSeconds(Setting.IdleSeconds <= 0 ? 0 : Setting.IdleSeconds), this.CancelTokenSource.Token);
                    if (QueueData.TryDequeue(out var workItem))
                    {
                        try
                        {
                            var task = this.ExecuteAsync(workItem, this.CancelTokenSource.Token);
                            if (task.Status == TaskStatus.Created)
                                task.Start();
                            var taska = task.ContinueWith(t =>
                              {
                                  if (this.MaxCount > 1)
                                      this.Slim.Release();
                                  this.TaskQueueOk?.Invoke(workItem);
                              }).ConfigureAwait(false);
                            if (MaxCount == 1) await taska;
                        }
                        catch (Exception ex)
                        {
                            this.ConsoleWrite($"执行任务队列出错.{Environment.NewLine}{ex.Message}");
                            LogHelper.Error(ex, "执行任务队列出错.");
                            this.TaskQueueError?.Invoke(workItem, ex);
                        }
                    }
                    else
                    {
                        TaskQueueEmpty?.Invoke(this);

                        Manual.Reset();
                        Manual.Wait(TimeSpan.FromSeconds(Setting.IdleSeconds <= 0 ? 0 : Setting.IdleSeconds), this.CancelTokenSource.Token);

                        if (QueueData.IsEmpty)
                        {
                            Synchronized.Run(() =>
                            {
                                //等待时长超过消费等待时长限制 终止当前消费任务.
                                this.ConsoleWrite($"等待时长超过消费等待时长 {Setting.IdleSeconds}S 限制,终止当前消费任务.");
                                this.CancelTokenSource.Cancel();
                                GC.Collect();
                            });
                        }

                    }
                }
            }, this.CancelTokenSource.Token, TaskCreationOptions.LongRunning).Start();
        }
        /// <summary>
        /// 停止消费任务
        /// </summary>
        public virtual async Task StopAsync()
        {
            //取消任务
            this.CancelTokenSource.Cancel();
            //清空队列
            this.QueueData = new ConcurrentQueue<T>();
            GC.Collect();
            await Task.CompletedTask;
        }
        /// <summary>
        /// 停止消费任务
        /// </summary>
        public void Stop()
        {
            this.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 输出控制台信息
        /// </summary>
        /// <param name="msg">信息</param>
        public virtual void ConsoleWrite(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("".PadLeft(70, '='));
            Console.WriteLine($"-- {msg} - {this.QueueName} - {DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff} --");
            Console.WriteLine("".PadLeft(70, '='));
            Console.ResetColor();
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public virtual Task Clear()
        {
            return Task.Run(() =>
            {
#if NETSTANDARD2_1
                this.QueueData.Clear();
#else
            while (this.QueueData.TryDequeue(out var _)) { }
#endif
            });
        }
        #endregion
    }
}