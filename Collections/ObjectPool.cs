using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Threading;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Collections
{
    /// <summary>
    /// 资源池 支持空闲释放
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class ObjectPool<T> : Disposable, IPool<T>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ObjectPool()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 最大数量 0为不限
        /// </summary>
        public virtual int Max { get; set; } = 10;
        /// <summary>
        /// 最小数量
        /// </summary>
        public virtual int Min { get; set; } = 1;
        /// <summary>
        /// 空闲多长时间关闭资源 单位为秒 0为不清除
        /// </summary>
        public virtual int IdleTime { get; set; } = 3 * 60;
        /// <summary>
        /// 多长时间检查一次 单位为秒 0为不定时检查
        /// </summary>
        public virtual int TimeOut { get; set; } = 5 * 60;
        /// <summary>
        /// 空闲池
        /// </summary>
        private readonly ConcurrentQueue<PoolItem<T>> FreeItems = new ConcurrentQueue<PoolItem<T>>();
        /// <summary>
        /// 工作池
        /// </summary>
        private readonly ConcurrentDictionary<string, PoolItem<T>> BusyItems = new ConcurrentDictionary<string, PoolItem<T>>();
        /// <summary>
        /// 总请求数
        /// </summary>
        public int _TotalCount = 0;
        /// <summary>
        /// 总请求数
        /// </summary>
        public int TotalCount => this._TotalCount;
        /// <summary>
        /// 空闲数
        /// </summary>
        public int _FreeCount = 0;
        /// <summary>
        /// 空闲数
        /// </summary>
        public int FreeCount => this._FreeCount;
        /// <summary>
        /// 工作数
        /// </summary>
        public int _BusyCount = 0;
        /// <summary>
        /// 工作数
        /// </summary>
        public int BusyCount => this._BusyCount;
        /// <summary>借出时是否可用</summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
        protected virtual bool OnGet(PoolItem<T> value) => value != null && value.Value != null && value.IsWork;
        /// <summary>归还时是否可用</summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
        protected virtual bool OnPut(PoolItem<T> value) => value != null && value.Value != null && value.IsWork;
        /// <summary>
        /// 定时作业
        /// </summary>
        public IJob Job { get; set; } = null;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            if (this.Name.IsNullOrEmpty())
            {
                var str = this.GetType().Name;
                if (str.Contains("`")) str = str.Substring(null, "`");
                if (str != "Pool")
                    this.Name = str;
                else
                    this.Name = $"Pool<{typeof(T).Name}>";
            }
            /*先初如化最小数*/
            for (var i = 0; i < this.Min; i++)
            {
                Task.Run(() =>
                {
                    this.FreeItems.Enqueue(new PoolItem<T>(OnCreate()));
                    Interlocked.Increment(ref _FreeCount);
                });
            }
        }
        #endregion

        #region 创建对象
        /// <summary>
        /// 创建对象
        /// </summary>
        protected virtual T OnCreate() => Activator.CreateInstance<T>();
        #endregion

        #region 清空对象
        /// <summary>
        /// 清空对象
        /// </summary>
        /// <returns></returns>
        public virtual void Clear()
        {
            while (this.FreeItems.TryDequeue(out PoolItem<T> item)) OnDispose(item.Value);
            this.BusyItems.Values.Each(item => OnDispose(item.Value));
            this.BusyItems.Clear();
            this._BusyCount = 0;
            this._FreeCount = 0;
        }
        #endregion

        #region 释放对象
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="value">对象</param>
        public virtual void OnDispose(T value) => value.TryDispose();
        #endregion

        #region 借出资源
        /// <summary>
        /// 借出资源
        /// </summary>
        /// <returns>返回一个对象</returns>
        public virtual PoolItem<T> Get()
        {
            Interlocked.Increment(ref this._TotalCount);
            PoolItem<T> pi = null;
            int i = 0;
            do
            {
                //LogHelper.Debug($"空闲数:{this.FreeItems.Count},繁忙数:{this.BusyItems.Count}");
                Interlocked.Increment(ref i);
                //从空闲集合借一个
                if (this.FreeItems.TryDequeue(out pi))
                {
                    Interlocked.Decrement(ref this._FreeCount);
                    break;
                }
                else
                {
                    if (Synchronized.Run(() =>
                    {
                        //超出最大值后,等待一会继续借直到借到为止
                        if (this.Max > 0 && this.Max <= this.FreeCount + this.BusyCount)
                        {
                            var msg = $"申请失败,已有 {this.FreeCount + this.BusyCount:n0} 个资源,达到或超过最大值 {this.Max:n0} .";
                            LogHelper.Info(this.Name + " " + msg);
                            if (i < 4) return false;
                        }
                        pi = new PoolItem<T>(OnCreate());
                        return true;
                    })) break;
                }
                //如果超过最大连接数 则等100毫秒后再去取
                Task.Delay(50).Wait();
            } while (!OnGet(pi));
            //最后时间
            pi.LastTime = DateTime.Now;
            //加入繁忙集合
            this.BusyItems.TryAdd(pi.ID, pi);
            Interlocked.Increment(ref this._BusyCount);
            return pi;
        }
        #endregion

        #region 归还资源
        /// <summary>
        /// 归还资源
        /// </summary>
        /// <param name="value">资源</param>
        /// <returns></returns>
        public virtual bool Put(PoolItem<T> value)
        {
            if (value == null || value.Value == null) return false;
            //关闭资源
            //this.Close(value.Value);
            try
            {
                //从繁忙队列找到并移除缓存项
                this.BusyItems.TryRemove(value.ID, out var _);
                Interlocked.Decrement(ref _BusyCount);
                //是否可用
                if (!OnPut(value)) return false;
                //最后时间
                value.LastTime = DateTime.Now;
                this.FreeItems.Enqueue(value);
                Interlocked.Increment(ref _FreeCount);
            }
            catch (Exception ex)
            {
                this.OnDispose(value.Value);
                LogHelper.Error(ex, "归还资源出错:");
            }
            return true;
        }
        #endregion

        #region 关闭资源
        /// <summary>
        /// 关闭资源
        /// </summary>
        /// <param name="item">资源</param>
        public abstract void Close(T item);
        #endregion

        #region 定时清理过期连接
        /// <summary>
        /// 定时清理过期连接
        /// </summary>
        /// <param name="job">作业</param>
        protected virtual void Work(IJob job)
        {
            if (this.IdleTime == 0 || this.FreeCount + this.BusyCount == 0) return;
            var expire = DateTime.Now.AddSeconds(-this.IdleTime);
            var FreeQueue = new ConcurrentQueue<PoolItem<T>>();
            while (this.FreeItems.TryDequeue(out var item))
            {
                if (item.LastTime <= expire)
                {
                    Interlocked.Decrement(ref this._FreeCount);
                    OnDispose(item.Value);
                }
                else
                {
                    item.IsWork = false;
                    FreeQueue.Enqueue(item);
                }
            }
            FreeQueue.Each(item => this.FreeItems.Enqueue(item));
            if (!this.BusyItems.IsEmpty)
            {
                this.BusyItems.Each(item =>
                {
                    if (!item.Value.IsWork || item.Value.LastTime <= expire)
                    {
                        if (this.BusyItems.TryRemove(item.Key, out PoolItem<T> _item))
                        {
                            Interlocked.Decrement(ref this._BusyCount);
                            OnDispose(_item.Value);
                        }
                    }
                });
            }
            LogHelper.Trace($"空闲数:{this.FreeItems.Count} 个资源,繁忙数:{this.BusyItems.Count} 个资源.");
            if (this.FreeItems.IsEmpty && this.BusyItems.IsEmpty) job?.Stop();
        }
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
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing, () =>
            {
                this.BusyItems.Clear();
                if (Job.Status == JobStatus.Waiting) Job.Stop();
                Job.TryDispose();
            });
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~ObjectPool()
        {
            this.Dispose(false);
        }
        #endregion
    }
}