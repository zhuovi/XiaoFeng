using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Threading;

namespace XiaoFeng.Collections
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 对象池最大
        /// </summary>
        int Max { get; set; }
        /// <summary>
        /// 对象池最小
        /// </summary>
        int Min { get; set; }
        /// <summary>
        /// 空闲多长时间关闭资源 单位为秒 0为不清除
        /// </summary>
       int IdleTime { get; set; }
        /// <summary>
        /// 多长时间检查一次 单位为秒 0为不定时检查
        /// </summary>
        int TimeOut { get; set; }
        /// <summary>
        /// 总请求数
        /// </summary>
        int TotalCount { get; }
        /// <summary>
        /// 空闲数
        /// </summary>
        int FreeCount { get; }
        /// <summary>
        /// 工作数
        /// </summary>
        int BusyCount { get; }
        /// <summary>
        /// 定时作业
        /// </summary>
        IJob Job { get; set; }
        /// <summary>借出</summary>
        /// <returns></returns>
        PoolItem<T> Get();
        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="value">对象</param>
        bool Put(PoolItem<T> value);
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="value">对象</param>
        void OnDispose(T value);
        /// <summary>
        /// 关闭资源
        /// </summary>
        /// <param name="obj">资源</param>
        void Close(T obj);
    }
}