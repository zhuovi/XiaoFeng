using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-19 01:00:11                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class Extentions
    {
        #region 方法
        /// <summary>
        /// 放弃任务安全
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="func">错误回调</param>
        public static void ForgetTaskSafe(this Task task, Action<Exception> func)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    LogHelper.Error(t.Exception.InnerException, "任务出错:");
                    func?.Invoke(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(false);
        }
        #endregion
    }
}