using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-02-28 10:31:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Threading
{
    /// <summary>
    /// 释放者
    /// </summary>
    public readonly struct Releaser : IDisposable
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="asyncLock">异步锁</param>
        public Releaser(AsyncLock asyncLock)
        {
            if (asyncLock == null) throw new ArgumentNullException(nameof(asyncLock));
            AsyncLock = asyncLock;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 异步锁
        /// </summary>
        readonly AsyncLock AsyncLock;
        #endregion

        #region 方法
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            AsyncLock.Release();
        }
        #endregion
    }
}