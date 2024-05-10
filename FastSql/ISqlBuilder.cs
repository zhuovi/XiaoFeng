﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.Data.SQL;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-10 16:53:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// Sql创建器
    /// </summary>
    public interface ISqlBuilder
    {
        #region 属性
        /// <summary>
        /// 是否缓存
        /// </summary>
        CacheState CacheState { get; set; }
        /// <summary>
        /// 缓存时长 单位为秒
        /// </summary>
        int? CacheTimeOut { get; set; }
        /// <summary>
        /// 是否命中缓存
        /// </summary>
        Boolean IsHitCache { get; set; }
        /// <summary>
        /// 命中缓存次数
        /// </summary>
        long HitCacheCount { get; set; }
        /// <summary>
        /// 缓存Key
        /// </summary>
        string CacheKey { get; set; }
        /// <summary>
        /// Model类型
        /// </summary>
        Type ModelType { get; set; }
        /// <summary>
        /// 分表配置
        /// </summary>
        ITableSplit TableSplit { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; set; }
        /// <summary>
        /// SQL语句
        /// </summary>
        string SQLString { get; set; }
        /// <summary>
        /// SQL语句带入存储过程参数
        /// </summary>
        string SQLParameter { get; set; }
        /// <summary>
        /// 上一次执行SQL语句
        /// </summary>
        string LastSQLString { get; set; }
        /// <summary>
        /// 上一次执行SQL语句带入存储过程参数
        /// </summary>
        string LastSQLParameter { get; set; }
        /// <summary>
        /// 显示字段
        /// </summary>
        List<string> Columns { get; set; }
        /// <summary>
        /// 设置字段
        /// </summary>
        List<object> UpdateColumns { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        Dictionary<string, object> Fields { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        StringBuilder WhereString { get; set; }
        /// <summary>
        /// 一页多少条
        /// </summary>
        int? PageSize { get; set; }
        /// <summary>
        /// 共有多少页
        /// </summary>
        int? PageCount { get; set; }
        /// <summary>
        /// 共有多少条
        /// </summary>
        int? Counts { get; set; }
        /// <summary>
        /// 前多少条
        /// </summary>
        int? Top { get; set; }
        /// <summary>
        /// 跳过多少条
        /// </summary>
        int? Skip { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        List<string> OrderBy { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        List<string> GroupBy { get; set; }
        /// <summary>
        /// 处理类型
        /// </summary>
        SQLType SQLType { get; set; }
        /// <summary>
        /// 数据库配置
        /// </summary>
        ConnectionConfig Config { get; set; }
        /// <summary>
        /// 执行SQL时长 单位为毫秒
        /// </summary>
        Int64 RunSQLTime { get; set; }
        /// <summary>
        /// 拼接SQL时长 单位为毫秒
        /// </summary>
        Int64 SpliceSQLTime { get; set; }
        /// <summary>
        /// LinqToSql时长
        /// </summary>
        double LinqToSQLTime { get; set; }
        /// <summary>
        /// 存储过程参数集
        /// </summary>
        Dictionary<string, object> Parameters { get; set; }
        #endregion

        #region 方法

        #region 设置条件
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="where">条件字符串</param>
        void AddWhere(string where);
        #endregion

        #region 设置排序
        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        void SetOrderBy(string orderString);
        #endregion

        #region 设置前几条
        /// <summary>
        /// 设置前几条
        /// </summary>
        /// <param name="top">获取数据条数</param>
        void SetTop(int top);
        #endregion

        #region 获取前几条
        /// <summary>
        /// 获取前几条
        /// </summary>
        /// <returns></returns>
        int? GetTop();
        #endregion

        #region 设置跳过多少条
        /// <summary>
        /// 设置跳过多少条
        /// </summary>
        /// <param name="skip">跳过的条数</param>
        void SetSkip(int skip);
        #endregion

        #region  获取跳过多少条
        /// <summary>
        /// 获取跳过多少条
        /// </summary>
        /// <returns></returns>

        int? GetSkip();
        #endregion

        #region 获取条件
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns></returns>
        string GetWhereString();
        #endregion

        #region 获取分组
        /// <summary>
        /// 获取分组
        /// </summary>
        /// <returns></returns>
        string GetGroupBy();
        #endregion

        #region 获取排序
        /// <summary>
        /// 获取排序
        /// </summary>
        /// <returns></returns>
        string GetOrderBy();
        #endregion

        #region 设置字段
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="field">字段名称</param>
        /// <param name="value">字段值</param>
        void SetField(string field, string value);
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="field">字段名称</param>
        void SetField(string field);
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="fields">字段集</param>
        void SetField(IEnumerable<KeyValuePair<string, string>> fields);
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="fields">字段名称集</param>
        void SetField(IEnumerable<string> fields);
        #endregion

        #region 获取字段集
        /// <summary>
        /// 获取字段名称集
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetFieldKeys();
        /// <summary>
        /// 获取字段集
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetFields();
        #endregion

        #region 获取字段值集
        /// <summary>
        /// 获取字段值集
        /// </summary>
        /// <returns></returns>
        ICollection<object> GetFieldValues();
        #endregion

        #region 清空数据
        /// <summary>
        /// 清空数据
        /// </summary>
        void Clear();
        #endregion

        #region 硬复制
        /// <summary>
        /// 硬复制
        /// </summary>
        /// <param name="sqlBuilder">SqlBuilder</param>
        void Clone(SqlBuilder sqlBuilder);
        #endregion

        #region 软复制数据
        /// <summary>
        /// 复制数据
        /// </summary>
        /// <returns></returns>
        object Clone();
        #endregion

        #region 获取SQL语句
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        string GetSqlString();
        #endregion

        #endregion
    }
    public interface ISqlBuilder<T> : ISqlBuilder
    {

    }
    public interface ISqlBuilder<T1, T2> : ISqlBuilder<T1>
    {

    }
    public interface ISqlBuilder<T1, T2, T3> : ISqlBuilder<T1, T2>
    {

    }
    public interface ISqlBuilder<T1, T2, T3,T4> : ISqlBuilder<T1, T2,T3>
    {

    }
    public interface ISqlBuilder<T1, T2, T3,T4,T5> : ISqlBuilder<T1, T2,T3,T4>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5,T6> : ISqlBuilder<T1, T2, T3, T4,T5>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5,T6,T7> : ISqlBuilder<T1, T2, T3, T4,T5,T6>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7,T8> : ISqlBuilder<T1, T2, T3, T4, T5, T6,T7>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8,T9> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7,T8>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9,T10> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8,T9>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9,T10>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>: ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13,T14,T15,T16,T17,T18,T19,T20> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>
    {

    }
    public interface ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25,T26> : ISqlBuilder<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19,T20,T21,T22,T23,T24,T25>
    {

    }
}