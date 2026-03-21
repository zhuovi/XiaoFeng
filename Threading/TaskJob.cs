using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-03-19 15:17:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 任务作业
    /// </summary>
    public class TaskJob : ITaskJob
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public TaskJob()
        {
            this.WriteLog($"初始化任务作业[{this.Id}].");
        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="function">函数</param>
        public TaskJob(Action function)
        {
            this.Callback = j => function.Invoke();
            this.WriteLog($"初始化带回调函数的任务作业[{this.Id}].");
        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="function">函数</param>
        public TaskJob(Action<ITaskJob> function)
        {
            this.Callback = function;
            this.WriteLog($"初始化带回调函数的任务作业[{this.Id}].");
        }
        #endregion

        #region 属性
        /// <summary>
        /// 作业ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 取消指令
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }
        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEventSlim PauseSignal = new ManualResetEventSlim(true);
        /// <summary>
        /// 状态
        /// </summary>
        public JobStatus Status { get; set; }
        /// <summary>
        /// 任务回调
        /// </summary>
        public Action<ITaskJob> Callback { get; set; }
        /// <summary>
        /// 是否输出调试日志
        /// </summary>
        public bool IsConsoleLog { get; set; } = true;
        #endregion

        #region 方法

        #region 运行
        /// <summary>
        /// 运行
        /// </summary>
        public Task Run()
        {
            return this.Run(this.Callback, CancellationToken.None);
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="cancellationToken">取消标识</param>
        /// <returns></returns>
        public Task Run(CancellationToken cancellationToken) => this.Run(this.Callback, cancellationToken);
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <returns></returns>
        public Task Run(Action function) => this.Run(function, CancellationToken.None);
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <param name="cancellationToken">取消标识</param>
        /// <returns></returns>
        public Task Run(Action function, CancellationToken cancellationToken)
        {
            if (function != null)
                this.Callback = j => function.Invoke();

            return this.Run(this.Callback, cancellationToken);
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <returns></returns>
        public Task Run(Action<ITaskJob> function)
        {
            return this.Run(function,CancellationToken.None);
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <param name="cancellationToken">取消标识</param>
        /// <returns></returns>
        public Task Run(Action<ITaskJob> function, CancellationToken cancellationToken)
        {
            if (function != null)
                this.Callback = function;
            else
            {
                if (this.Callback != null)
                    function = this.Callback;
                else
                    return default;
            }
            this.WriteLog($"任务作业[{this.Id}]开始运行.");
            this.CancellationTokenSource = new CancellationTokenSource();
            var cancel = CancellationTokenSource.CreateLinkedTokenSource(this.CancellationTokenSource.Token, cancellationToken);
            this.Status = JobStatus.Runing;
            var task = Task.Factory.StartNew(() =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    this.PauseSignal.Wait(cancel.Token);
                    function.Invoke(this);
                }
            }, cancel.Token);
            return task;
        }
        #endregion

        #region 暂停
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            this.WriteLog($"任务作业[{this.Id}]已暂停.");
            this.Status = JobStatus.Paused;
            this.PauseSignal.Reset();
        }
        #endregion

        #region 恢复
        /// <summary>
        /// 恢复
        /// </summary>
        public void Resume()
        {
            this.WriteLog($"任务作业[{this.Id}]已恢复正常.");
            this.Status = JobStatus.Runing;
            this.PauseSignal.Set();
        }
        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.WriteLog($"任务作业[{this.Id}]已停止.");
            this.Status = JobStatus.Stoping;
            this.CancellationTokenSource.Cancel();
        }
        #endregion

        #region 输出调试日志
        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="message">消息</param>
        private void WriteLog(string message)
        {
            if (this.IsConsoleLog)
                LogHelper.Debug(message);
        }
        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~TaskJob()
        {
            this.PauseSignal.Dispose();
        }
        #endregion

        #endregion
    }
}