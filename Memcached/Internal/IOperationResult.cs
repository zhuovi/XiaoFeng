/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 16:26:03                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// 操作结果接口
    /// </summary>
    public interface IOperationResult<T> : IOperationResult
    {
        #region 属性
        /// <summary>
        /// 数据
        /// </summary>
        T Value { get; set; }
        #endregion

        #region 方法

        #endregion
    }
    /// <summary>
    /// 操作结果接口
    /// </summary>
    public interface IOperationResult
    {
        /// <summary>
        /// 操作结果状态
        /// </summary>
        OperationStatus Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        string Message { get; set; }
        /*
        /// <summary>
        /// 响应数据
        /// </summary>
        byte[] PayLoad { get; set; }
        */
    }
}