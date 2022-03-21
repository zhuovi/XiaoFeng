using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Threading
{
    /// <summary>
    /// 作业调度中心接口
    /// </summary>
    public interface IJobScheduler
    {
        #region 添加作业
        /// <summary>
        /// 添加多个作业
        /// </summary>
        /// <param name="jobs">作业集</param>
        Task Add(params IJob[] jobs);
        /// <summary>
        /// 添加作业
        /// </summary>
        /// <param name="job">作业</param>
        Task Add(IJob job);
        /// <summary>
        /// 批量添加作业
        /// </summary>
        /// <param name="jobs">作业集</param>
        Task AddRange(IEnumerable<IJob> jobs);
        #endregion

        #region 移除作业
        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="name">作业名称</param>
        void Remove(string name);
        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="ID">ID</param>
        void Remove(Guid ID);
        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="job">作业</param>
        void Remove(IJob job);
        #endregion

        #region 唤醒处理
        /// <summary>唤醒处理</summary>
        void Wake();
        #endregion

        #region 停止调度器
        /// <summary>
        /// 停止调度器
        /// </summary>
        void Stop();
        #endregion

        #region 作业列表
        /// <summary>
        /// 作业列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<IJob> GetJobs();
        #endregion
    }
}