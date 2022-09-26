using System;
using System.Collections.Concurrent;
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
            this.CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
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
            this.CancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }
        #endregion

        #region 属性
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
        /// 取消事件
        /// </summary>
        private CancellationTokenSource CancelToken = new CancellationTokenSource();
        /// <summary>
        /// 消费运行状态
        /// </summary>
        public Boolean ConsumeState { get; private set; } = false;
        /// <summary>
        /// 配置
        /// </summary>
        private ISetting _Setting;
        /// <summary>
        /// 配置
        /// </summary>
        private ISetting Setting
        {
            get
            {
                if (_Setting == null)
                    _Setting = XiaoFeng.Config.Setting.Current;
                return _Setting;
            }
            set => _Setting = value;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="t">对象</param>
        public virtual async Task AddWorkItem(T t)
        {
            QueueData.Enqueue(t);
            Manual.Set();
            Synchronized.Run(() =>
            {
                if (this.CancelToken.IsCancellationRequested || !this.ConsumeState)
                {
                    this.CancelToken = new CancellationTokenSource();
                    this.CancelToken.Token.Register(() =>
                    {
                        Synchronized.Run(() =>
                        {
                            this.ConsumeState = false;
                        });
                    });
                    this.ConsumeState = true;
                    //有新任务,重新启动任务.
                    this.ConsoleWrite("有新任务,启动消费任务.");
                    this.ConsumeTask();
                }
            });
            await Task.CompletedTask;
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
                return default(T);
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
                while (!this.CancelToken.Token.IsCancellationRequested)
                {
                    if (QueueData.TryDequeue(out var workItem))
                    {
                        try
                        {
                            var task = this.ExecuteAsync(workItem, this.CancelToken.Token);
                            if (task.Status == TaskStatus.Created)
                                task.Start();
                            await Task.WhenAny(task).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            this.ConsoleWrite($"执行任务队列出错.{Environment.NewLine}{ex.Message}");
                            LogHelper.Error(ex, "执行任务队列出错.");
                        }
                    }
                    else
                    {
                        Manual.Reset();
                        Manual.Wait(TimeSpan.FromSeconds(Setting.IdleSeconds <= 0 ? 0 : Setting.IdleSeconds));
                        if (QueueData.IsEmpty)
                        {
                            Synchronized.Run(() =>
                            {
                                //等待时长超过消费等待时长限制 终止当前消费任务.
                                this.ConsoleWrite($"等待时长超过消费等待时长 {Setting.IdleSeconds}S 限制,终止当前消费任务.");
                                this.CancelToken.Cancel();
                            });
                        }
                    }
                }
            }, this.CancelToken.Token, TaskCreationOptions.LongRunning).Start();
        }
        /// <summary>
        /// 停止消费任务
        /// </summary>
        public virtual async Task StopAsync()
        {
            //取消任务
            this.CancelToken.Cancel();
            //清空队列
            this.QueueData = new ConcurrentQueue<T>();
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
        #endregion
    }
}