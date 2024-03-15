using System;
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
namespace XiaoFeng
{
    /// <summary>
    /// 释放资源
    /// </summary>
    public class Disposable : IDisposable
    {
        #region 要检测冗余调用
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放托管
        /// </summary>
        /// <param name="disposing">状态</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    GC.SuppressFinalize(this);
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 释放托管
        /// </summary>
        /// <param name="disposing">释放状态</param>
        /// <param name="managed">托管对象</param>
        /// <param name="unmanaged">未托管对象</param>
        protected virtual void Dispose(bool disposing, Action managed, Action unmanaged = null)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    managed?.Invoke();
                    GC.SuppressFinalize(this);
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                unmanaged?.Invoke();
                disposedValue = true;
            }
        }
        /// <summary>
        ///  添加此代码以正确实现可处置模式
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~Disposable() { this.Dispose(false); }
        #endregion
    }
}