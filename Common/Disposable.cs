using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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