using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-02-28 10:25:33                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 异步锁
    /// </summary>
    public class AsyncLock : IDisposable
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public AsyncLock()
        {
            Releaser = new Releaser(this);
            CompletedTask = Task.FromResult(Releaser);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 完成任务
        /// </summary>
        readonly Task<IDisposable> CompletedTask;
        /// <summary>
        /// 释放器
        /// </summary>
        readonly IDisposable Releaser;
        /// <summary>
        /// 锁
        /// </summary>
        readonly object SyncRoot = new object();
        /// <summary>
        /// 异步锁等待队列
        /// </summary>
        readonly Queue<AsyncLockWaiter> Waiters = new Queue<AsyncLockWaiter>(64);
        /// <summary>
        /// 是否释放
        /// </summary>
        volatile bool _IsDisposed;
        /// <summary>
        /// 是否锁定
        /// </summary>
        bool IsLocked;
        #endregion

        #region 方法
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            lock (SyncRoot)
            {
                _IsDisposed = true;
                while (Waiters.Any())
                {
                    Waiters.Dequeue().Dispose();
                }
            }
        }
        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">空对象异常</exception>
        public Task<IDisposable> EnterAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_IsDisposed) throw new ObjectDisposedException(nameof(AsyncLock));

            lock (SyncRoot)
            {
                if (!IsLocked)
                {
                    IsLocked = true;
                    return CompletedTask;
                }
                var waiter = new AsyncLockWaiter(cancellationToken);
                Waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            lock (SyncRoot)
            {
                if (_IsDisposed) return;

                IsLocked = false;

                while (Waiters.Any())
                {
                    var waiter = Waiters.Dequeue();
                    var isApproved = waiter.Approve(Releaser);
                    waiter.Dispose();
                    if (isApproved)
                    {
                        IsLocked = true;
                        return;
                    }
                }
            }
        }
        #endregion
    }
}