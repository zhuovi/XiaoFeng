using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using XiaoFeng.Json;
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
    /// 作业操作类
    /// Version : 2.1
    /// </summary>
    [Serializable]
    public class Job : Disposable, IJob
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Job()
        {
            this.Name = this.ID.ToString("N");
        }
        /// <summary>
        /// 设置调度
        /// </summary>
        /// <param name="scheduler">调度</param>
        public Job(IJobScheduler scheduler) : this()
        {
            this._Scheduler = scheduler;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 当前调度
        /// </summary>
        private IJobScheduler _Scheduler = null;
        /// <summary>
        /// 当前调度
        /// </summary>
        public IJobScheduler Scheduler
        {
            get
            {
                if (this._Scheduler == null)
                    this._Scheduler = JobScheduler.Default ?? JobScheduler.CreateScheduler();
                return this._Scheduler;
            }
            set { this._Scheduler = value; }
        }
        /// <summary>
        /// 取消信号
        /// </summary>
        public CancellationTokenSource CancelToken { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// 作业数据
        /// </summary>
        public object State { get; set; }
        /// <summary>
        /// 是否是异步
        /// </summary>
        public bool Async { get; set; } = true;
        /// <summary>
        /// 运行次数
        /// </summary>
        public int Count { get { return this.SuccessCount + this.FailureCount; } }
        /// <summary>
        /// 已成功运行次数
        /// </summary>
        public int SuccessCount { get; set; } = 0;
        /// <summary>
        /// 失败运行次数
        /// </summary>
        public int FailureCount { get; set; } = 0;
        /// <summary>
        /// 运行日志
        /// </summary>
        public List<string> Message { get; set; } = new List<string>();
        /// <summary>
        /// 作业ID
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();
        /// <summary>
        /// 作业名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 成功回调
        /// </summary>
        [JsonIgnore] 
        public Action<IJob> SuccessCallBack { get; set; } = null;
        /// <summary>
        /// 当前任务执行完成后再进入计时队列
        /// </summary>
        [JsonIgnore] 
        public Action<IJob> CompleteCallBack { get; set; } = null;
        /// <summary>
        /// 失败回调
        /// </summary>
        [JsonIgnore] 
        public Action<IJob, Exception> FailureCallBack { get; set; } = null;
        /// <summary>
        /// 停止作业回调
        /// </summary>
        [JsonIgnore]
        public Action<IJob> StopCallBack { get; set; } = null;
        /// <summary>
        /// 最后一次运行时间
        /// </summary>
        public DateTime? LastTime { get; set; }
        /// <summary>
        /// 下次运行时间
        /// </summary>
        public DateTime? NextTime { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public JobStatus Status { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 最大运行次数
        /// </summary>
        public int? MaxCount { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 运行完是否销毁
        /// </summary>
        public bool IsDestroy { get; set; } = false;
        /// <summary>
        /// 定时器类型
        /// </summary>
        [JsonConverter(typeof(DescriptionConverter))]
        public TimerType TimerType { get; set; } = TimerType.UnKnow;
        /// <summary>
        /// 时间
        /// </summary>
        private Time _Time = null;
        /// <summary>
        /// 时间
        /// </summary>
        public Time Time
        {
            get { return this._Time; }
            set
            {
                this._Time = value;
                //if (this.Times == null) this.Times = new List<Times>();
                //this.Times.Add(new Times(value));
            }
        }
        /// <summary>
        /// 几点，几号，周几（周日为一周的第一天）,可用负数，-1代表一天中最后一小时即23点，一周内最后一天即周六，一月内最后一天
        /// </summary>
        private int[] _DayOrWeekOrHour;
        /// <summary>
        /// 几点，几号，周几（周日为一周的第一天）,可用负数，-1代表一天中最后一小时即23点，一周内最后一天即周六，一月内最后一天
        /// </summary>
        public int[] DayOrWeekOrHour
        {
            get { return this._DayOrWeekOrHour; }
            set
            {
                this._DayOrWeekOrHour = value;
                Array.Sort(this._DayOrWeekOrHour);
                if (this.Times == null) this.Times = new List<Times>();
                if (!(TimerType.Hour| TimerType.Day| TimerType.Week| TimerType.Month| TimerType.Year).HasFlag(this.TimerType)) return;
            }
        }
        /// <summary>
        /// 时间集 几点，几号，周几（周日为一周的第一天）, 可用负数，-1代表一天中最后一小时即23点，一周内最后一天即周六，一月内最后一天 代替 Time+DayOrWeekOrHour ;
        /// </summary>
        private List<Times> _Times;
        /// <summary>
        /// 时间集 几点，几号，周几（周日为一周的第一天）, 可用负数，-1代表一天中最后一小时即23点，一周内最后一天即周六，一月内最后一天 代替 Time+DayOrWeekOrHour;
        /// </summary>
        public List<Times> Times
        {
            get { return this._Times; }
            set
            {
                this._Times = value;
            }
        }
        /// <summary>
        /// 执行任务时间偏差 单位为毫秒 默认是1s 建议不要超过10s;
        /// </summary>
        private long _Deviation = 1000;
        /// <summary>
        /// 执行任务时间偏差 单位为毫秒 默认是1s 建议不要超过10s;
        /// </summary>
        public long Deviation
        {
            get { return this._Deviation; }
            set
            {
                this._Deviation = value;
                if (this._Deviation <= 0)
                    this._Deviation = 0;
                else if (this._Deviation >= 60 * 1000)
                    this._Deviation = 1000;
            }
        }
        /// <summary>
        /// 间隔 单位毫秒
        /// </summary>
        public long Period { get; set; }
        #endregion

        #region 方法

        #region 启动作业
        /// <summary>
        /// 启动作业
        /// </summary>
        public void Start() => this.Start(null);
        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="scheduler">调度</param>
        public void Start(IJobScheduler scheduler)
        {
            (scheduler ?? this.Scheduler).Add(this);
        }
        #endregion

        #region 停止作业
        /// <summary>
        /// 停止作业
        /// </summary>
        public void Stop() => this.Stop(null);
        /// <summary>
        /// 停止作业
        /// </summary>
        /// <param name="scheduler">调度</param>
        public void Stop(IJobScheduler scheduler)
        {
            (scheduler ?? this.Scheduler).Remove(this);
            this.StopCallBack?.Invoke(this);
            this.CancelToken.Cancel();
        }
        #endregion

        #region 转换为字符串
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToJson();
        }
        #endregion

        #region 释放资源
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #endregion
    }
}