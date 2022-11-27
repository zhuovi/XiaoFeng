using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-14 16:31:02                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存实体
    /// </summary>
    public class CacheEntity:IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CacheEntity()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 值类型
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        private object _Value;
        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
                this.EntityType = value?.GetType();
            }
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        public Boolean IsEmpty { get { return this.Value.IsNullOrEmpty(); } }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpiresTime { get; set; }
        /// <summary>
        /// 缓存时长
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }
        /// <summary>
        /// 是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）
        /// </summary>
        public Boolean IsSliding { get; set; } = false;
        /// <summary>
        /// 回调
        /// </summary>
        public Action<WatcherChangeType, string, string> CallBack { get; set; }
        /// <summary>
        /// 关联文件
        /// </summary>
        private string _Path = string.Empty;
        /// <summary>
        /// value
        /// </summary>
        private bool disposedValue;
        /// <summary>
        /// 关联文件
        /// </summary>
        public string Path
        {
            get { return this._Path; }
            set
            {
                this._Path = value; 
                if (value.IsNotNullOrEmpty())
                {
                    //因为当前组件出现死锁，暂注释掉，后边单独在写一个方法监听文件目录
                    //this.Provider = new FileProvider(this.Name);
                    //this.Provider.Watch(value, this.CallBack).ConfigureAwait(false);
                }
            }
        }
        /// <summary>
        /// Key
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 监控
        /// </summary>
        private FileProvider Provider { get; set; }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">值</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Provider?.Dispose();
                    this.Provider = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        /// <summary>
        /// TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        /// </summary>
        ~CacheEntity()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }
        /// <summary>
        /// 释放未托管资源
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region 方法

        #endregion
    }
}