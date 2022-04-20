using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using System.Threading;
using XiaoFeng.Threading;
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
            var str = this.GetType().Name;
            if (str.Contains("`")) str = str.Substring(null, "`");
            if (str != "Pool")
                this.Name = str;
            else
                this.Name = $"Pool<{typeof(T).Name}>";
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
        public virtual int Min { get; set; } = 0;
        /// <summary>
        /// 空闲多长时间关闭资源 单位为秒 0为不清除
        /// </summary>
        public virtual int IdleTime { get; set; } = 180;
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
            foreach (var item in this.BusyItems.Values)
            {
                OnDispose(item.Value);
            }
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
        /// <returns></returns>
        public virtual PoolItem<T> Get()
        {
            LogHelper.Debug($"空闲数:{this.FreeItems.Count},繁忙数:{this.BusyItems.Count}");
            Interlocked.Increment(ref this._TotalCount);
            PoolItem<T> pi = null;
            do
            {
                // 从空闲集合借一个
                if (this.FreeItems.TryDequeue(out pi))
                {
                    Interlocked.Decrement(ref this._FreeCount);
                }
                else
                {
                    var flag = false;
                    Synchronized.Run(() =>
                    {
                        //超出最大值后,等待一会继续借直到借到为止
                        if (this.Max > 0 && this.Max <= this.FreeCount + this.BusyCount)
                        {
                            var msg = $"申请失败,已有 {this.FreeCount + this.BusyCount:n0} 达到或超过最大值 {this.Max:n0}";
                            LogHelper.Info(this.Name + " " + msg);
                            flag = true;
                        }
                        else
                            pi = new PoolItem<T>(OnCreate());
                    });
                    if (flag)
                    {
                        //如果超过最大连接数 则等100毫秒后再去取
                        Task.Delay(100).Wait();
                    }
                }
            } while (!OnGet(pi));
            // 最后时间
            pi.LastTime = DateTime.Now;
            // 加入繁忙集合
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
            if (value == null) return false;
            //关闭资源
            this.Close(value.Value);
            // 从繁忙队列找到并移除缓存项
            this.BusyItems.TryRemove(value.ID, out var item);
            Interlocked.Decrement(ref _BusyCount);
            // 是否可用
            if (!OnPut(value)) return false;
            // 最后时间
            item.LastTime = DateTime.Now;
            this.FreeItems.Enqueue(item);
            Interlocked.Increment(ref _FreeCount);
            return true;
        }
        #endregion
       
        #region 关闭资源
        /// <summary>
        /// 关闭资源
        /// </summary>
        /// <param name="obj">资源</param>
        public abstract void Close(T obj);
        #endregion

        #region 定时清理过期连接
        /// <summary>
        /// 定时清理过期连接
        /// </summary>
        /// <param name="job">作业</param>
        protected virtual void Work(IJob job)
        {
            if (this.IdleTime == 0 || this.FreeCount + this.BusyCount == 0) return;
            var exp = DateTime.Now.AddSeconds(-this.IdleTime);
            var FreeQueue = new ConcurrentQueue<PoolItem<T>>();
            while (this.FreeItems.TryDequeue(out var item))
            {
                if (item.LastTime <= exp)
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
            FreeQueue.Each(item =>
            {
                this.FreeItems.Enqueue(item);
            });
            if (!this.BusyItems.IsEmpty)
            {
                this.BusyItems.Each(item =>
                {
                    if (!item.Value.IsWork || item.Value.LastTime <= exp)
                    {
                        if (this.BusyItems.TryRemove(item.Key, out PoolItem<T> _item))
                        {
                            Interlocked.Decrement(ref this._BusyCount);
                            OnDispose(_item.Value);
                        }
                    }
                });
            }
            if(this.FreeItems.IsEmpty && this.BusyItems.IsEmpty)
            {
                if (job != null)
                    job.Stop();
            }
        }
        #endregion

        #region 释放资源
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (Job.Status == JobStatus.Wait) Job.Stop();
            Job.TryDispose();
        }
        #endregion
    }
}