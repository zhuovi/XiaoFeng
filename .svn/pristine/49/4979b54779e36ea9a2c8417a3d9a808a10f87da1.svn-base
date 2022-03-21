using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 错误信息是否展示到页面 2抛出异常 1展示 0转向
        /// </summary>
        ExceptionType Error { get; set; }
        /// <summary>
        /// 最大线程数量
        /// </summary>
        int MaxWorkerThreads { get; set; }
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