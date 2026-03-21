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
*  Create Time : 2026-03-19 16:39:11                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 任务作业接口
    /// </summary>
    public interface ITaskJob
    {
        #region 属性
        /// <summary>
        /// 作业ID
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 取消指令
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        JobStatus Status { get; set; }
        /// <summary>
        /// 回调
        /// </summary>
        Action<ITaskJob> Callback { get; set; }

        /// <summary>
        /// 是否输出调试日志
        /// </summary>
        bool IsConsoleLog { get; set; }
        #endregion

        #region 方法

        #region 运行
        /// <summary>
        /// 运行
        /// </summary>
        Task Run();
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="cancellationToken">取消标识</param>
        /// <returns></returns>
        Task Run(CancellationToken cancellationToken);
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <returns></returns>
        Task Run(Action function);
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <param name="cancellationToken">取消标识</param>
        /// <returns></returns>
        Task Run(Action function, CancellationToken cancellationToken);
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <returns></returns>
        Task Run(Action<ITaskJob> function);
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="function">回调函数</param>
        /// <param name="cancellationToken">取消标识</param>
        /// <returns></returns>
        Task Run(Action<ITaskJob> function, CancellationToken cancellationToken);
        #endregion

        #region 暂停
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        #endregion

        #region 恢复
        /// <summary>
        /// 恢复
        /// </summary>
        void Resume();
        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        #endregion

        #endregion
    }
}