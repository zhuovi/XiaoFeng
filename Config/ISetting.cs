using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
namespace XiaoFeng.Config
{
    /// <summary>
    /// XiaoFeng配置接口
    /// </summary>
    public interface ISetting
    {
        #region 属性
        /// <summary>
        /// 是否启用调试
        /// </summary>
        bool Debug { get; set; }
        /// <summary>
        /// 最大线程数量
        /// </summary>
        int MaxWorkerThreads { get; set; }
        /// <summary>
        /// 消费日志空闲时长
        /// </summary>
        int IdleSeconds { get; set; }
        /// <summary>
        /// 任务队列执行任务超时时间 单位为秒
        /// </summary>
        int TaskWaitTimeout { get; set; }
        /// <summary>
        /// 调度最小等待时长 单位为秒
        /// </summary>
        int JobSchedulerWaitTimeout { get; set; }
        /// <summary>
        /// 是否启用数据加密
        /// </summary>
        bool DataEncrypt { get; set; }
        /// <summary>
        /// 加密数据key
        /// </summary>
        string DataKey { get; set; }
        /// <summary>
        /// 是否开启IIS请求日志
        /// </summary>
        bool ServerLogging { get; set; }
        /// <summary>
        /// 是否拦截
        /// </summary>
        bool IsIntercept { get; set; }
        /// <summary>
        /// SQL注入串
        /// </summary>
        string SQLInjection { get; set; }
        #endregion
    }
}