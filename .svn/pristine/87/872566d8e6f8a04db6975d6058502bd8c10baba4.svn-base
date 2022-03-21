using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 数据拼接接口
    /// </summary>
    public interface IQueryableQ
    {
        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="query">IQueryableX</param>
        /// <returns></returns>
        IQueryableQ If<T>(IQueryableX<T> query);
        /// <summary>
        /// 符合条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        IQueryableQ Then<T>(params IQueryableX<T>[] querys);
        /// <summary>
        /// 不符合条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        IQueryableQ Else<T>(params IQueryableX<T>[] querys);
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        bool End();
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        T ToEntity<T>();
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        List<T> ToList<T>();
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存时长 单位为秒</param>
        /// <returns></returns>
        IQueryableQ SetCache(int TimeOut);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableQ NoCache();
    }
}