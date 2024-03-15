using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Config;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 2.1.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 作业调度器
    /// Version : 2.1
    /// </summary>
    public class JobScheduler : Disposable, IJobScheduler
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public JobScheduler()
        {
            this.Setting = XiaoFeng.Config.Setting.Current;
        }
        /// <summary>
        /// 设置带名称的调度
        /// </summary>
        /// <param name="name">名称</param>
        public JobScheduler(string name) : this()
        {
            this.Name = name;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 调度器集合
        /// </summary>
        public static readonly ConcurrentDictionary<string, JobScheduler> Schedulers = new ConcurrentDictionary<string, JobScheduler>();
        /// <summary>
        /// 调度名称
        /// </summary>
        public string Name { get; set; } = "Default";
        /// <summary>
        /// 间隔时长 单位为毫秒
        /// </summary>
        private long Period { get; set; } = 1 * 60 * 60 * 1000;
        /// <summary>
        /// 调度作业列表
        /// </summary>
        private readonly ConcurrentDictionary<Guid, IJob> SchedulerJobs = new ConcurrentDictionary<Guid, IJob>();
        /// <summary>
        /// 消费运行状态
        /// </summary>
        private int ConsumeState = 0;
        /// <summary>
        /// 取消信号
        /// </summary>
        public CancellationTokenSource CancelTokenSource { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// 线程同步信号
        /// </summary>
        private ManualResetEventSlim Manual = new ManualResetEventSlim(false);
        /// <summary>默认调度器</summary>
        public static IJobScheduler Default { get; } = CreateScheduler("Default");
        /// <summary>
        /// 当前调度器
        /// </summary>
        [ThreadStatic]
        private IJobScheduler _Current = null;
        /// <summary>当前调度器</summary>
        public IJobScheduler Current { get { return _Current ?? CreateScheduler(); } private set { _Current = value; } }
        /// <summary>
        /// 全局配置
        /// </summary>
        private ISetting Setting { get; set; }
        #endregion

        #region 方法

        #region 创建调度
        /// <summary>
        /// 创建调度
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static JobScheduler CreateScheduler(string name = "Default")
        {
            if (Schedulers.TryGetValue(name, out var ts)) return ts;
            return Synchronized.Run(() =>
            {
                ts = new JobScheduler(name);
                Schedulers.TryAdd(name, ts);
                ts.Log("创建调度器:" + name);
                return ts;
            });
        }
        #endregion

        #region 添加作业
        /// <summary>
        /// 添加多个作业
        /// </summary>
        /// <param name="jobs">作业集</param>
        public async Task Add(params IJob[] jobs)
        {
            await this.AddRange(jobs);
        }
        /// <summary>
        /// 添加作业
        /// </summary>
        /// <param name="job">作业</param>
        public async Task Add(IJob job)
        {
            if (job == null || this.SchedulerJobs.ContainsKey(job.ID)) return;
            var j = this.SchedulerJobs.Values.Where(a => a.Name == job.Name);
            //如果作业中有正在等待的作业并且名和要加入的作业名称一样的情况下则不再添加作业
            if (j.Any() && j != null && j.First().Status == JobStatus.Waiting)
            {
                return;
            }
            if (job.StartTime.HasValue)
            {
                var startTime = job.StartTime.Value;
                job.StartTime = null;
                if ((startTime - DateTime.Now).TotalMilliseconds > 100)
                {
                    job.NextTime = startTime;
                    job.StartTime = null;
                }
            }
            /*检查作业定时执行时间*/
            if ((TimerType.Hour | TimerType.Day | TimerType.Week | TimerType.Month | TimerType.Year).HasFlag(job.TimerType))
            {
#if NETSTANDARD2_0
                if (job.Times == null) job.Times =
#else
                job.Times ??=
#endif
                new List<Times>();
                if (job.TimerType == TimerType.Hour)
                {
                    if (job.Times.Count == 0) job.Times.Add(new Times(0, 0, 0));
                }
                else if (job.TimerType == TimerType.Day)
                {
                    if (job.Times.Count == 0) job.Times.Add(new Times(0, 0, 0));
                }
                else if (job.TimerType == TimerType.Week)
                {
                    if (job.Times.Count == 0) job.Times.Add(new Times(0, 0, 0, week: 1));
                }
                else if (job.TimerType == TimerType.Month)
                {
                    if (job.Times.Count == 0) job.Times.Add(new Times(0, 0, 0, day: 1));
                    var list = new List<Times>();
                    job.Times.Each(a =>
                    {
                        if (!a.Day.HasValue) a.Day = 1;
                        if (!list.Any(b => b.Day == a.Day && b.TotalSeconds == a.TotalSeconds)) list.Add(a);
                    });
                    job.Times = list;
                }
                else if (job.TimerType == TimerType.Year)
                {
                    if (job.Times.Count == 0) job.Times.Add(new Times(0, 0, 0, 1, 1));
                    var list = new List<Times>();
                    job.Times.Each(a =>
                    {
                        if (!a.Month.HasValue) a.Month = 1;
                        if (!a.Day.HasValue) a.Day = 1;
                        if (!list.Any(b => b.Month == a.Month && b.Day == a.Day && b.TotalSeconds == a.TotalSeconds)) list.Add(a);
                    });
                    job.Times = list;
                }
                if (job.Times.Count == 0)
                {
                    this.Log($"-- 作业[{job.Name}]未加入调度，因为作业任务类型为:{job.TimerType.GetDescription()} 未设置执行时间 --");
                    return;
                }
                job.Times = job.Times.OrderBy(a => a.Month).ThenBy(a => a.Day).ThenBy(a => a.Week).ThenBy(a => a.TotalSeconds).ToList();
            }
            job.Status = JobStatus.Waiting;
            if (job.CancelToken == null || job.CancelToken.IsCancellationRequested) job.CancelToken = new CancellationTokenSource();
            job.CancelToken.Token.Register(() =>
            {
                if (this.SchedulerJobs.TryRemove(job.ID, out var joba))
                {
                    joba.StopCallBack?.Invoke(joba);
                    joba.TryDispose();
                    joba = null;
                    GC.Collect();
                }
            });
            this.SchedulerJobs.TryAdd(job.ID, job);
            IntervalJob(job);
            OnceJob(job);
            this.Wake();
            await Task.CompletedTask;
        }
        /// <summary>
        /// 批量添加作业
        /// </summary>
        /// <param name="jobs">作业集</param>
        public async Task AddRange(IEnumerable<IJob> jobs)
        {
            if (jobs.Count() == 0) return;
            foreach (var j in jobs)
                await this.Add(j);
        }
        #endregion

        #region 移除作业
        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="name">作业名称</param>
        public void Remove(string name)
        {
            if (name.IsNullOrEmpty()) return;
            var job = this.SchedulerJobs.Values.First(a => a.Name.EqualsIgnoreCase(name));
            if (job == null) return;
            this.Remove(job);
        }
        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="ID">ID</param>
        public void Remove(Guid ID)
        {
            if (ID == Guid.Empty) return;
            if (this.SchedulerJobs.TryRemove(ID, out var job))
            {
                if (!job.CancelToken.IsCancellationRequested) job.CancelToken.Cancel();
            }
            this.Wake();
        }
        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="job">作业</param>
        public void Remove(IJob job)
        {
            if (job == null) return;
            this.Remove(job.ID);
        }
        #endregion

        #region 唤醒处理
        /// <summary>唤醒处理</summary>
        [SecuritySafeCritical]
        public void Wake()
        {
            Manual.Set();
            if (Interlocked.CompareExchange(ref ConsumeState, 1, 0) == 0)
            {
                this.Manual = new ManualResetEventSlim(false);
                if (this.CancelTokenSource == null || this.CancelTokenSource.IsCancellationRequested) this.CancelTokenSource = new CancellationTokenSource();
                this.CancelTokenSource.Token.Register(() =>
                {
                    Interlocked.CompareExchange(ref ConsumeState, 0, 1);
                    this.SchedulerJobs.Clear();
                    GC.Collect();
                });
                this.Log($"-- 有新作业任务,启动调度器[{this.Name}]. --");
                this.Process();
            }
        }
        #endregion

        #region 入口
        /// <summary>
        /// 入口
        /// </summary>
        public void Process()
        {
            Current = this;
            new Task(() =>
            {
                if (this.Name.IsNotNullOrEmpty())
                    Thread.CurrentThread.Name = this.Name;
                while (!this.CancelTokenSource.IsCancellationRequested)
                {
                    if (this.SchedulerJobs.Count == 0)
                    {
                        this.Period = this.Setting.JobSchedulerWaitTimeout * 1000;
                        this.CancelTokenSource.Cancel();
                        this.Log($"-- 暂无作业任务,终止调度器[{this.Name}]. --");
                        break;
                    }
                    else
                        this.Period = this.Setting.JobSchedulerWaitTimeout * 1000;
                    var now = DateTime.Now;
                    this.SchedulerJobs.Values.Each(job =>
                    {
                        if (job.TimerType == TimerType.Interval || job.TimerType == TimerType.Once) return;
                        long period = 0;
                        if (job.Status == JobStatus.Waiting && this.CheckTimes(job, now, out period))
                        {
                            if (job.TimerType != TimerType.Once)
                                Synchronized.Run(() =>
                                {
                                    this.Period = Math.Min(this.Period, job.Period);
                                });
                            job.Status = JobStatus.Runing;
                            job.LastTime = now;
                            this.Log($"开始运行作业 [{job.Name}] - {this.SchedulerJobs.Count}");
                            if (job.CompleteCallBack == null) job.Status = JobStatus.Waiting;
                            if (!job.Async)
                                Execute(job);
                            else
                            {
                                /*
                                 * Date:2022-04-01
                                 * 优化调度执行完成后再间隔时间
                                 */
                                var cancel = CancellationTokenSource.CreateLinkedTokenSource(this.CancelTokenSource.Token, job.CancelToken.Token);
                                Task.Factory.StartNew(this.Execute, job, cancel.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ContinueWith((t, j) =>
                                {
                                    var _job = (IJob)j;
                                    if (_job.CompleteCallBack != null)
                                        _job.Status = JobStatus.Waiting;
                                }, job, cancel.Token);
                            }
                            /*计算下次运行时长*/
                            this.CheckTimes(job, DateTime.Now.AddMilliseconds(job.Deviation * 2), out period);
                        }
                        //this.Log($"计算出间隔:{period}");
                        if (period > 0)
                            Synchronized.Run(() =>
                            {
                                this.Period = Math.Min(this.Period, period);
                            });
                        this.Log($"-- 下一次检测作业时间为:{now.AddMilliseconds(this.Period):yyyy-MM-dd HH:mm:ss.ffff} --");
                    });
                    //this.Log($"等待间隔:{this.Period}");
#if NETSTANDARD2_0
                    if (this.Manual == null)this.Manual =
#else
                    this.Manual ??=
#endif
                    new ManualResetEventSlim(false);
                    Manual.Reset();
                    Manual.Wait(TimeSpan.FromMilliseconds(this.Period < 0 ? 10 : this.Period));
                }
                GC.Collect();
            }, CancelTokenSource.Token, TaskCreationOptions.LongRunning).Start();
        }
        /// <summary>
        /// 只运行一次的作业
        /// </summary>
        /// <param name="job"></param>
        private void OnceJob(IJob job)
        {
            if (job.TimerType == TimerType.Once)
            {
                var cancel = CancellationTokenSource.CreateLinkedTokenSource(this.CancelTokenSource.Token, job.CancelToken.Token);
                Task.Run(async () =>
                {
                    if (job.NextTime.HasValue)
                        await Task.Delay(job.NextTime.Value.Subtract(DateTime.Now)).ConfigureAwait(false);
                    job.Status = JobStatus.Runing;
                    job.LastTime = DateTime.Now;
                    this.Log($"开始运行作业 [{job.Name}] - {this.SchedulerJobs.Count}");
                    if (job.CompleteCallBack == null) job.Status = JobStatus.Waiting;
                    Execute(job);
                    job.Stop();
                }, cancel.Token);
            }
        }
        /// <summary>
        /// 专一处理定时间隔作业
        /// </summary>
        /// <param name="job">作业</param>
        private void IntervalJob(IJob job)
        {
            if (job.TimerType == TimerType.Interval)
            {
                var cancel = CancellationTokenSource.CreateLinkedTokenSource(this.CancelTokenSource.Token, job.CancelToken.Token);
                Task.Run(async () =>
                {
                    if (job.ExpireTime.HasValue)
                    {
                        if (job.ExpireTime.Value > DateTime.Now)
                            cancel.CancelAfter(job.ExpireTime.Value.Subtract(DateTime.Now));

                        await Task.Delay(job.NextTime.Value.Subtract(DateTime.Now)).ConfigureAwait(false);
                    }
                    while (!cancel.IsCancellationRequested)
                    {
                        job.Status = JobStatus.Runing;
                        job.LastTime = DateTime.Now;
                        this.Log($"开始运行作业 [{job.Name}] - {this.SchedulerJobs.Count}");
                        if (job.CompleteCallBack == null) job.Status = JobStatus.Waiting;
                        if (!job.Async)
                            Execute(job);
                        else
                        {
                            /*
                             * Date:2022-04-01
                             * 优化调度执行完成后再间隔时间
                             */
                            var task = Task.Factory.StartNew(this.Execute, job, cancel.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ContinueWith((t, j) =>
                             {
                                 var _job = (IJob)j;
                                 if (_job.CompleteCallBack != null)
                                     _job.Status = JobStatus.Waiting;
                             }, job, cancel.Token);
                            if (job.CompleteCallBack != null) await task;
                        }
                        await Task.Delay(TimeSpan.FromMilliseconds(job.Period)).ConfigureAwait(false);
                    }
                }, cancel.Token);
            }
        }
        #endregion

        #region 执行作业
        /// <summary>
        /// 执行作业
        /// </summary>
        /// <param name="state">作业</param>
        [SecuritySafeCritical]
        private void Execute(object state)
        {
            var job = state as IJob;
            try
            {
                if (job.SuccessCallBack == null)
                {
                    if (job.CompleteCallBack == null)
                    {
                        this.Remove(job);
                        return;
                    }
                    else
                        job.SuccessCallBack = j => j.CompleteCallBack?.Invoke(j);
                }
                job.SuccessCallBack?.Invoke(job);
                this.Success(job);
            }
            catch (ThreadAbortException ex)
            {
                LogHelper.Error(ex, "任务终止");
                job.FailureCallBack?.Invoke(job, ex);
                this.Failure(job);
            }
            catch (ThreadInterruptedException ex)
            {
                LogHelper.Error(ex, "任务中断");
                job.FailureCallBack?.Invoke(job, ex);
                this.Failure(job);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                job.FailureCallBack?.Invoke(job, ex);
                this.Failure(job);
            }
            finally
            {
                //job.Status = JobStatus.Wait;
            }
        }
        #endregion

        #region 执行成功后执行
        /// <summary>
        /// 执行成功后执行
        /// </summary>
        /// <param name="job">作业</param>
        private void Success(IJob job)
        {
            if (job.Message.Count >= this.Setting.JobMessageCount)
                job.Message.Clear();
            job.Message.Add("执行作业[{0}]成功.[{1}]".format(job.Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            this.Log($"执行作业 [{job.Name}] 完成.");
            job.SuccessCount++;
            if (job.TimerType == TimerType.Once || job.IsDestroy || (job.MaxCount.HasValue && job.SuccessCount + job.FailureCount >= job.MaxCount))
            {
                this.Remove(job.ID);
            }
        }
        #endregion

        #region 执行失败后执行
        /// <summary>
        /// 执行失败后执行
        /// </summary>
        /// <param name="job">作业</param>
        private void Failure(IJob job)
        {
            if (job.Message.Count >= this.Setting.JobMessageCount)
                job.Message.Clear();
            job.Message.Add("执行作业[{0}]失败.[{1}]".format(job.Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            job.FailureCount++;
            if (job.TimerType == TimerType.Once || job.IsDestroy || (job.MaxCount.HasValue && job.SuccessCount + job.FailureCount >= job.MaxCount))
            {
                this.Remove(job.ID);
            }
        }
        #endregion

        #region 检查定时器是否到期 升级
        /// <summary>检查定时器是否到期</summary>
        /// <param name="job">任务</param>
        /// <param name="now">现在时间</param>
        /// <param name="period">返回间隔</param>
        /// <returns></returns>
        private Boolean CheckTimes(IJob job, DateTime now, out long period)
        {
            period = -1;
            //if (job.Status == JobStatus.Runing) return false;
            if ((job.MaxCount.HasValue && job.Count >= job.MaxCount) ||
            (job.ExpireTime.HasValue && job.ExpireTime < now))
            {
                this.Remove(job);
                return false;
            }
            if (job.NextTime.HasValue && job.NextTime.Value >= now)
            {
                var ts = (long)(job.NextTime.Value - now).TotalSeconds * 1000;
                if (ts <= job.Deviation)
                {
                    return true;
                }
                else
                {
                    period = ts;
                    return false;
                }
            }
            var MatchFlag = false;
            var SetFlag = false;
            var i = 0;
            var _now = new Time(now);
            switch (job.TimerType)
            {
                case TimerType.UnKnow:
                    this.Remove(job);
                    return false;
                case TimerType.Interval:
                    if (job.NextTime.HasValue)
                    {
                        if (job.NextTime <= now)
                        {
                            job.NextTime = now.AddMilliseconds(job.Period);
                            return true;
                        }
                        period = (long)(job.NextTime.Value - now).TotalSeconds * 1000;
                        if (period <= job.Deviation) return true;
                        return false;
                    }
                    else
                    {
                        job.Period = job.Period > 0 ? job.Period : job.Deviation * 1000;
                        job.NextTime = now.AddMilliseconds(job.Period);
                        return true;
                    }
                case TimerType.Once:
                    if (job.NextTime.HasValue)
                    {
                        period = (long)(job.NextTime.Value - now).TotalSeconds * 1000;
                        job.Period = period;
                        return period <= job.Deviation;
                    }
                    return true;
                case TimerType.Hour:
                    if (job.Times == null || job.Times.Count == 0)
                    {
                        this.Remove(job);
                        return false;
                    }
                    var totalSeconds = now.Minute * 60 + now.Second;
                    var hts = job.Times;
                    hts.Each(a =>
                    {
                        i++;
                        var t = (long)(a.TotalSeconds - totalSeconds) * 1000;
                        if (t < 0)
                            return true;
                        else if (t <= job.Deviation)
                        {
                            if (i == hts.Count)
                                i = 0;
                            job.NextTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, hts[i].Minute, hts[i].Second);
                            if (i == 0) job.NextTime = job.NextTime.Value.AddHours(1);
                            job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                            MatchFlag = true;
                            return false;
                        }
                        else
                        {
                            job.Period = t;
                            job.NextTime = now.AddMilliseconds(job.Period);
                            SetFlag = true;
                            return false;
                        }
                    });
                    if (!MatchFlag && !SetFlag && i >= hts.Count)
                    {
                        job.NextTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, hts[0].Minute, hts[0].Second).AddHours(1);
                        job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                    }
                    period = job.Period;
                    return MatchFlag;
                case TimerType.Day:
                    if (job.Times == null || job.Times.Count == 0)
                    {
                        this.Remove(job);
                        return false;
                    }
                    var dts = job.Times;
                    dts.Each(a =>
                    {
                        i++;
                        var t = (long)(a.TotalSeconds - _now.TotalSeconds) * 1000;
                        if (t < 0)
                            return true;
                        else if (t <= job.Deviation)
                        {
                            if (i == dts.Count)
                                i = 0;
                            job.NextTime = new DateTime(now.Year, now.Month, now.Day, dts[i].Hour.GetValueOrDefault(), dts[i].Minute, dts[i].Second);
                            if (i == 0) job.NextTime = job.NextTime.Value.AddHours(1);
                            job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                            MatchFlag = true;
                            return false;
                        }
                        else
                        {
                            job.Period = t;
                            job.NextTime = now.AddMilliseconds(job.Period);
                            SetFlag = true;
                            return false;
                        }
                    });
                    if (!MatchFlag && !SetFlag && i >= dts.Count)
                    {
                        job.NextTime = new DateTime(now.Year, now.Month, now.Day, dts[0].Hour.GetValueOrDefault(), dts[0].Minute, dts[0].Second).AddDays(1);
                        job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                    }
                    period = job.Period;
                    //this.Log($"now={now:yyyy-MM-dd HH:mm:ss.fff}-NOW={DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                    return MatchFlag;
                case TimerType.Week:
                    if (job.Times == null || job.Times.Count == 0)
                    {
                        this.Remove(job);
                        return false;
                    }
                    var CurrentWeek = (int)now.DayOfWeek;
                    var wts = job.Times;
                    wts.Each(a =>
                    {
                        i++;
                        if (a.Week == CurrentWeek)
                        {
                            var m = (Time)a;
                            var t = (long)(m.TotalSeconds - _now.TotalSeconds) * 1000;
                            if (t < 0)
                                return true;
                            else if (t <= job.Deviation)
                            {
                                if (i == wts.Count)
                                    i = 0;
                                job.NextTime = new DateTime(now.Year, now.Month, now.Day, wts[i].Hour.GetValueOrDefault(), wts[i].Minute, wts[i].Second);
                                if (i == 0)
                                {
                                    job.NextTime = job.NextTime.Value.AddDays(7 - CurrentWeek + wts[i].Week.GetValueOrDefault());
                                }
                                else
                                {
                                    job.NextTime = job.NextTime.Value.AddDays(wts[i].Week.GetValueOrDefault() - CurrentWeek);
                                }
                                job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                                MatchFlag = true;
                                return false;
                            }
                            else
                            {
                                job.Period = t;
                                job.NextTime = now.AddMilliseconds(job.Period);
                                SetFlag = true;
                                return false;
                            }
                        }
                        else if (a.Week > CurrentWeek)
                        {
                            job.NextTime = new DateTime(now.Year, now.Month, now.Day, wts[i].Hour.GetValueOrDefault(), wts[i].Minute, wts[i].Second).AddDays(a.Week.Value - CurrentWeek);
                            job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                            SetFlag = true;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    });
                    if (!MatchFlag && !SetFlag && i >= wts.Count)
                    {
                        job.NextTime = new DateTime(now.Year, now.Month, now.Day, wts[0].Hour.GetValueOrDefault(), wts[0].Minute, wts[0].Second).AddDays(7 - CurrentWeek + wts.First().Week.Value);
                        job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                    }
                    period = job.Period;
                    return MatchFlag;
                case TimerType.Month:
                    if (job.Times == null || job.Times.Count == 0)
                    {
                        this.Remove(job);
                        return false;
                    }
                    var CurrentDay = now.Day;
                    var mts = job.Times;
                    mts.Each(a =>
                    {
                        i++;
                        if (a.Day == CurrentDay)
                        {
                            var t = (long)(a.TotalSeconds - _now.TotalSeconds) * 1000;
                            if (t < 0)
                                return true;
                            else if (t <= job.Deviation)
                            {
                                if (i == mts.Count)
                                    i = 0;
                                job.NextTime = new DateTime(now.Year, now.Month, mts[i].Day.Value, mts[i].Hour.GetValueOrDefault(), mts[i].Minute, mts[i].Second);
                                if (i == 0) job.NextTime = job.NextTime.Value.AddMonths(1);
                                job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                                MatchFlag = true;
                                return false;
                            }
                            else
                            {
                                job.Period = t;
                                job.NextTime = now.AddMilliseconds(job.Period);
                                SetFlag = true;
                                return false;
                            }
                        }
                        else if (a.Day > CurrentDay)
                        {
                            job.NextTime = new DateTime(now.Year, now.Month, a.Day.Value, mts[i].Hour.GetValueOrDefault(), mts[i].Minute, mts[i].Second);
                            job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                            SetFlag = true;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    });
                    if (!MatchFlag && !SetFlag && i >= mts.Count)
                    {
                        job.NextTime = new DateTime(now.Year, now.Month, mts[0].Day.Value, mts[0].Hour.GetValueOrDefault(), mts[0].Minute, mts[0].Second);
                        job.NextTime = job.NextTime.Value.AddMonths(1);
                        job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                    }
                    period = job.Period;
                    return MatchFlag;
                case TimerType.Year:
                    if (job.Times == null || job.Times.Count == 0)
                    {
                        this.Remove(job);
                        return false;
                    }
                    var CurrentMonth = now.Month;
                    CurrentDay = now.Day;
                    var yts = job.Times;
                    yts.Each(a =>
                    {
                        var _f = false;
                        i++;
                        if (a.Month == CurrentMonth)
                        {
                            if (a.Day == CurrentDay)
                            {
                                var t = (long)(a.TotalSeconds - _now.TotalSeconds) * 1000;
                                if (t < 0)
                                    return true;
                                else if (t <= job.Deviation)
                                {
                                    if (i == yts.Count)
                                        i = 0;
                                    job.NextTime = new DateTime(now.Year, yts[i].Month.Value, yts[i].Day.Value, yts[i].Hour.GetValueOrDefault(), yts[i].Minute, yts[i].Second);
                                    if (i == 0) job.NextTime = job.NextTime.Value.AddYears(1);
                                    job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                                    MatchFlag = true;
                                    return false;
                                }
                                else
                                    _f = true;
                            }
                            else if (a.Day > CurrentDay)
                                _f = true;
                            else return true;
                        }
                        else if (a.Month > CurrentMonth)
                            _f = true;
                        else
                        {
                            return true;
                        }
                        if (_f)
                        {
                            job.NextTime = new DateTime(now.Year, a.Month.Value, a.Day.Value, a.Hour.Value, a.Minute, a.Second);
                            job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                            SetFlag = true;
                            return false;
                        }
                        return true;
                    });
                    if (!MatchFlag && !SetFlag && i >= yts.Count)
                    {
                        job.NextTime = new DateTime(now.Year, yts[0].Month.Value, yts[0].Day.Value, yts[0].Hour.GetValueOrDefault(), yts[0].Minute, yts[0].Second);
                        job.NextTime = job.NextTime.Value.AddYears(1);
                        job.Period = (long)Math.Abs((job.NextTime.Value - now).TotalMilliseconds);
                    }
                    period = job.Period;
                    return MatchFlag;
                default: return false;
            }
        }
        #endregion

        #region 获取离当前值最接近的索引
        /*/// <summary>
        /// 获取离当前值最接近的索引
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="val">值</param>
        /// <param name="timerType">类型</param>
        /// <returns></returns>
        private int GetIndex(int[] arr, int val, TimerType timerType)
        {
            int Index = -1, Val = -1;
            if (arr == null || arr.Length == 0) return Index;
            for (int i = 0; i < arr.Length; i++)
            {
                var ArrValue = arr[i];
                if (ArrValue < 0)
                    ArrValue = GetMaxValue(timerType, ArrValue);
                var v = ArrValue - val;
                if (v >= 0 && v <= Val)
                {
                    Index = i; Val = v;
                }
            }
            return Index == -1 ? 0 : Index;
        }*/
        #endregion

        #region 获取最大值
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="timerType">定时器类型</param>
        /// <param name="addValue">添加值</param>
        /// <returns></returns>
        public int GetMaxValue(TimerType timerType, int addValue)
        {
            if (addValue > -1) return addValue;
            addValue++;
            if (timerType == TimerType.Day)
                return 23 + addValue % 24;
            else if (timerType == TimerType.Week)
                return 6 + addValue % 7;
            else if (timerType == TimerType.Month)
            {
                var now = DateTime.Now;
                var MaxDays = DateTime.DaysInMonth(now.Year, now.Month);
                return MaxDays + addValue % MaxDays;
            }
            else return Math.Abs(--addValue);
        }
        #endregion

        #region 停止调度器
        /// <summary>
        /// 停止调度器
        /// </summary>
        public void Stop()
        {
            if (this.CancelTokenSource != null && !this.CancelTokenSource.IsCancellationRequested)
                this.CancelTokenSource.Cancel();
        }
        #endregion

        #region 作业列表
        /// <summary>
        /// 作业列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IJob> GetJobs() => this.SchedulerJobs.Values;
        #endregion

        #region 扩展属性
        /// <summary>
        /// 添加作业调度
        /// </summary>
        /// <typeparam name="T">作业</typeparam>
        /// <returns></returns>
        public IJob Worker<T>() where T : IJobWoker
        {
            return new Job().Worker<T>();
        }
        #endregion

        #region 日志
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="message">消息</param>
        private void Log(string message)
        {
            var level = this.Setting.JobLogLevel;
            if (level.HasValue)
            {
                LogHelper.WriteLog(this.Setting.JobLogLevel.Value, message);
            }
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放状态</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing, () =>
            {
                if (this.SchedulerJobs != null)
                {
                    this.SchedulerJobs.Clear();
                }
            });
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~JobScheduler() => this.Dispose(false);
        #endregion
    }
}