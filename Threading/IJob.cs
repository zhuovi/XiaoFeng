using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 作业接口
    /// </summary>
    public interface IJob
    {
        #region 属性
        /// <summary>
        /// 当前调度
        /// </summary>
        IJobScheduler Scheduler { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        JobStatus Status { get; set; }
        /// <summary>
        /// 作业数据
        /// </summary>
        object State { get; set; }
        /// <summary>
        /// 是否是异步
        /// </summary>
        bool Async { get; set; }
        /// <summary>
        /// 已成功运行次数
        /// </summary>
        int SuccessCount { get; set; }
        /// <summary>
        /// 失败运行次数
        /// </summary>
        int FailureCount { get; set; }
        /// <summary>
        /// 运行日志
        /// </summary>
        List<string> Message { get; set; }
        /// <summary>
        /// 取消信号
        /// </summary>
        CancellationTokenSource CancelToken { get; set; }
        /// <summary>
        /// 作业ID
        /// </summary>
        Guid ID { get; }
        /// <summary>
        /// 作业名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 运行次数
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 成功回调
        /// </summary>
        Action<IJob> SuccessCallBack { get; set; }
        /// <summary>
        /// 当前任务执行完成后再进入计时队列
        /// </summary>
        Action<IJob> CompleteCallBack { get; set; }
        /// <summary>
        /// 失败回调
        /// </summary>
        Action<IJob, Exception> FailureCallBack { get; set; }
        /// <summary>
        /// 停止作业回调
        /// </summary>
        Action<IJob> StopCallBack { get; set; }
        /// <summary>
        /// 最后一次运行时间
        /// </summary>
        DateTime? LastTime { get; set; }
        /// <summary>
        /// 下次运行时间
        /// </summary>
        DateTime? NextTime { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        DateTime? StartTime { get; set; }
        /// <summary>
        /// 最大运行次数
        /// </summary>
        int? MaxCount { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 运行完是否销毁
        /// </summary>
        bool IsDestroy { get; set; }
        /// <summary>
        /// 定时器类型
        /// </summary>
        TimerType TimerType { get; set; }
         /// <summary>
        /// 间隔 单位毫秒
        /// </summary>
        long Period { get; set; }
        /// <summary>
        /// 时间集 几点，几号，周几（周日为一周的第一天）,可用负数，-1代表一天中最后一小时即23点，一周内最后一天即周六，一月内最后一天 代替 Time+DayOrWeekOrHour;
        /// </summary>
        List<Times> Times { get; set; }
        /// <summary>
        /// 执行任务时间偏差 单位为毫秒 默认是1s 建议不要超过10s;
        /// </summary>
        long Deviation { get; set; }
        #endregion

        #region 启动作业
        /// <summary>
        /// 启动作业
        /// </summary>
        void Start();
        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="scheduler">调度</param>
        void Start(IJobScheduler scheduler);
        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="token">取消指令 <see cref="CancellationToken"/></param>
        void Start(CancellationToken token);
        #endregion

        #region 停止作业
        /// <summary>
        /// 停止作业
        /// </summary>
        void Stop();
        /// <summary>
        /// 停止作业
        /// </summary>
        /// <param name="scheduler">调度</param>
        void Stop(IJobScheduler scheduler);
        #endregion

        #region 扩展属性
        /// <summary>
        /// 设置作业名称
        /// </summary>
        /// <param name="name">作业名称</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetName(string name);
        /// <summary>
        /// 设置作业ID
        /// </summary>
        /// <param name="id">作业ID</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetID(Guid id);
        /// <summary>
        /// 间隔多长时间执行
        /// </summary>
        /// <param name="period">间隔时长 单位为毫秒</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob Interval(long period);
        /// <summary>
        /// 间隔多长时间执行
        /// </summary>
        /// <param name="period">间隔时长 单位为毫秒</param>
        /// <param name="action">执行函数</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob Interval(long period, Action<IJob> action);
        /// <summary>
        /// 设置开始运行时间
        /// </summary>
        /// <param name="startTime">开始运行时间</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetStartTime(DateTime startTime);
        /// <summary>
        /// 每小时几分运行
        /// </summary>
        /// <param name="times">几分运行</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryHour(List<Time> times);
        /// <summary>
        /// 每小时几分运行
        /// </summary>
        /// <param name="times">几分运行</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryHour(List<Times> times);
        /// <summary>
        /// 每天几点几分运行
        /// </summary>
        /// <param name="times">几点几分</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryDay(List<Time> times);
        /// <summary>
        /// 每天几点几分运行
        /// </summary>
        /// <param name="times">几点几分</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryDay(List<Times> times);
        /// <summary>
        /// 每周周几几点几分运行
        /// </summary>
        /// <param name="times">周几几点几分</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryWeek(List<Times> times);
        /// <summary>
        /// 每月几号几点几分运行
        /// </summary>
        /// <param name="times">几号几点几分</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryMonth(List<Times> times);
        /// <summary>
        /// 每年几月几号几点几分运行
        /// </summary>
        /// <param name="times">几月几号几点几分</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob EveryYear(List<Times> times);
        /// <summary>
        /// 设置作业执行完成再执行调度
        /// </summary>
        /// <param name="job">作业内容</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetCompleteCallBack(Action<IJob> job);
        /// <summary>
        /// 设置执行成功回调
        /// </summary>
        /// <param name="job">作业内容</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetSuccessCallBack(Action<IJob> job);
        /// <summary>
        /// 设置作业停止回调
        /// </summary>
        /// <param name="job">作业内容</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetStopCallBack(Action<IJob> job);
        /// <summary>
        /// 设置失败回调
        /// </summary>
        /// <param name="job">作业内容</param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetFailureCallBack(Action<IJob, Exception> job);
        /// <summary>
        /// 设置取消指令
        /// </summary>
        /// <param name="token">取消指令 <see cref="CancellationToken"/></param>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SetCancelToken(CancellationToken token);
        /// <summary>
        /// 添加工作作业
        /// </summary>
        /// <typeparam name="T">作业</typeparam>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob Worker<T>() where T : IJobWoker;
        /// <summary>
        /// 添加工作作业
        /// </summary>
        /// <typeparam name="T">作业</typeparam>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob SuccessWorker<T>() where T : IJobWoker;
        /// <summary>
        /// 添加工作作业
        /// </summary>
        /// <typeparam name="T">作业</typeparam>
        /// <returns>一个作业 <see cref="IJob"/></returns>
        IJob CompleteWorker<T>() where T : IJobWoker;
        #endregion
    }
}