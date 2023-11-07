using System.Collections.Generic;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 10:18:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
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