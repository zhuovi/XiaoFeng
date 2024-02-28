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
*  Create Time : 2024-02-28 09:40:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 异步任务完成源类库
    /// </summary>
    sealed class AsyncTaskCompletionSource<TResult>
    {
        #region 构造器
        /// <summary>
        /// 实始化一个新实例
        /// </summary>
        public AsyncTaskCompletionSource()
        {
            TaskCompletionSource =
#if NETSTANDARD2_0
            new TaskCompletionSource<TResult>()
#else
            new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously)
#endif
            ;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 任务完成源
        /// </summary>
        readonly TaskCompletionSource<TResult> TaskCompletionSource;
        /// <summary>
        /// 任务
        /// </summary>
        public Task<TResult> Task => TaskCompletionSource.Task;
        #endregion

        #region 方法
        /// <summary>
        /// 尝试将基础 System.Threading.Tasks.Task`1 转换为 <see cref="TaskStatus.Canceled"/> 状态并启用要存储在取消的任务中的取消标记。
        /// </summary>
        /// <param name="cancellationToken">取消标记。</param>
        public void TrySetCanceled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;
#if NETSTANDARD2_0
            System.Threading.Tasks.Task.Run(() => TaskCompletionSource.TrySetCanceled(cancellationToken));
            SpinWait.SpinUntil(() => TaskCompletionSource.Task.IsCompleted);
#else
            TaskCompletionSource.TrySetCanceled(cancellationToken);
#endif
        }
        /// <summary>
        /// 尝试将基础 System.Threading.Tasks.Task`1 转换为 <see cref="TaskStatus.Canceled"/> 状态。
        /// </summary>
        public void TrySetCanceled()
        {
            if (TaskCompletionSource.Task.IsCanceled || TaskCompletionSource.Task.IsCompleted) return;
#if NETSTANDARD2_0
            System.Threading.Tasks.Task.Run(() => TaskCompletionSource.TrySetCanceled());
            SpinWait.SpinUntil(() => TaskCompletionSource.Task.IsCompleted);
#else
            TaskCompletionSource.TrySetCanceled();
#endif
        }
        /// <summary>
        /// 尝试将基础 System.Threading.Tasks.Task`1 转换为 <see cref="TaskStatus.Faulted"/> 状态，并对其绑定一些异常对象。
        /// </summary>
        /// <param name="exceptions">要绑定到此 System.Threading.Tasks.Task`1 的异常的集合。</param>
        public void TrySetException(IEnumerable<Exception> exceptions)
        {
#if NETSTANDARD2_0
            System.Threading.Tasks.Task.Run(() => TaskCompletionSource.TrySetException(exceptions));
            SpinWait.SpinUntil(() => TaskCompletionSource.Task.IsCompleted);
#else
            TaskCompletionSource.TrySetException(exceptions);
#endif
        }
        /// <summary>
        /// 尝试将基础 System.Threading.Tasks.Task`1 转换为 <see cref="TaskStatus.Faulted"/> 状态，并将其绑定到一个指定异常上。
        /// </summary>
        /// <param name="exception">要绑定到此 System.Threading.Tasks.Task`1 的异常。</param>
        public void TrySetException(Exception exception)
        {
#if NETSTANDARD2_0
            System.Threading.Tasks.Task.Run(() => TaskCompletionSource.TrySetException(exception));
            SpinWait.SpinUntil(() => TaskCompletionSource.Task.IsCompleted);
#else
            TaskCompletionSource.TrySetException(exception);
#endif
        }
        /// <summary>
        /// 尝试将基础 System.Threading.Tasks.Task`1 转换为 <see cref="TaskStatus.RanToCompletion"/> 状态。
        /// </summary>
        /// <param name="result">要绑定到此 System.Threading.Tasks.Task`1 的结果值。</param>
        /// <returns></returns>
        public bool TrySetResult(TResult result)
        {
#if NETSTANDARD2_0
            if (TaskCompletionSource.Task.IsCompleted) return false;
            System.Threading.Tasks.Task.Run(() => TaskCompletionSource.TrySetResult(result));
            SpinWait.SpinUntil(() => TaskCompletionSource.Task.IsCompleted);
            return true;
#else
            return TaskCompletionSource.TrySetResult(result);
#endif
        }
        #endregion
    }
}