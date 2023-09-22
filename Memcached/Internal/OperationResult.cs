using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 18:21:54                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// 操作结果类
    /// </summary>
    public class OperationResult : IOperationResult
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public OperationResult()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 操作结果状态
        /// </summary>
        public OperationStatus Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /*
        /// <summary>
        /// 响应数据
        /// </summary>
        public byte[] PayLoad { get; set; }
        */
        #endregion

        #region 方法

        #endregion
    }
    /// <summary>
    /// 操作结果类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class OperationResult<T> : OperationResult, IOperationResult<T>
    {
        /// <summary>
        /// 结果数据
        /// </summary>
        public T Value { get; set; }
    }
}