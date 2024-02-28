using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-02-28 10:11:26                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 异步锁等待者
    /// </summary>
    sealed class AsyncLockWaiter : IDisposable
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public AsyncLockWaiter()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="cancellationToken">取消标识</param>
        public AsyncLockWaiter(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (cancellationToken.CanBeCanceled)
            {
                CancellationTokenRegistration = cancellationToken.Register(Cancel);
                HasCancellationTokenRegistration = true;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 取消标识注册器
        /// </summary>
        readonly CancellationTokenRegistration CancellationTokenRegistration;
        /// <summary>
        /// 是否有取消标识注册器
        /// </summary>
        readonly bool HasCancellationTokenRegistration;
        /// <summary>
        /// 异步任务完成器
        /// </summary>
        readonly AsyncTaskCompletionSource<IDisposable> Promise = new AsyncTaskCompletionSource<IDisposable>();
        /// <summary>
        /// 任务
        /// </summary>
        public Task<IDisposable> Task => Promise.Task;
        #endregion

        #region 方法
        /// <summary>
        /// 同意释放
        /// </summary>
        /// <param name="disposable">释放对象</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数空异常</exception>
        public bool Approve(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            if (Promise.Task.IsCompleted) return false;
            return Promise.TrySetResult(disposable);
        }
        /// <summary>
        /// 取消
        /// </summary>
        void Cancel() => Promise.TrySetCanceled();
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (HasCancellationTokenRegistration)
                CancellationTokenRegistration.Dispose();
            Promise.TrySetException(new ObjectDisposedException(nameof(AsyncLockWaiter)));
        }
        #endregion
    }
}