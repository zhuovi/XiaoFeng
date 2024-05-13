﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using XiaoFeng.Cache;
using XiaoFeng.Config;
using XiaoFeng.Json;
using XiaoFeng.Model;
/*
===================================================================
Author : jacky
Email : jacky@zhuovi.com
QQ : 7092734
Site : www.zhuovi.com
Verstion : 1.1.11
Create Time : 2017/12/18 10:18:41
Update Time : 2020/11/20 08:54:31
Description : LinqToSQL
Log :
1.增加了拼接SQL，执行SQL计时器 
2.优化了实时回调事件
2018-06-27
1.优化了Update Set字段后有括号的问题
2018-07-05
1.增加了缓存操作
2.增加可以个别设置缓存和不缓存
2018-07-17  v1.1.9
1.修复了复制DataSQL对象 引用对象复制不了的问题
2020-11-20  v1.1.10
1.修复了批量插入，批量更新偶尔出错的bug
2020-12-12
1.取消当前缓存,当前缓存用基类DataHelper的缓存
2022-02-0=22 v1.1.11
1.更新基类中的相关bug
===================================================================
*/
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 11:05:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region 转换数据类型
    /// <summary>
    /// 转换数据类型
    /// </summary>
    public class DataHelperX : IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX() { }
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            this.Config = config;
            if (e != null) this.SQLCallBack += e;
        }
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerType">数据库驱动</param>
        /// <param name="isTransaction">是否启用事务处理</param>
        /// <param name="commandTimeOut">数据库操作超时时间</param>
        public DataHelperX(string connectionString, DbProviderType providerType = DbProviderType.SqlServer, Boolean isTransaction = true, int commandTimeOut = 0)
        {
            this.Config = new ConnectionConfig()
            {
                ConnectionString = connectionString,
                ProviderType = providerType,
                IsTransaction = isTransaction,
                CommandTimeOut = commandTimeOut
            };
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据库相关配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        /// <summary>
        /// 数据库操作基础类
        /// </summary>
        public DataHelper DataHelper { get { return new DataHelper(this.Config); } }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL 回调方法
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 方法
        /// <summary>
        /// 转换成QueryableX
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public virtual IQueryableX<T> QueryableX<T>(RunSQLEventHandler e = null)
        {
            if (e != null) SQLCallBack += e;
            return new DataHelperX<T>(this.Config, SQLCallBack);
        }
        /// <summary>
        /// 转换成QueryableX
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public virtual IQueryableX<T, T2> QueryableX<T, T2>(RunSQLEventHandler e = null)
        {
            if (e != null) SQLCallBack += e;
            return new DataHelperX<T, T2>(this.Config, SQLCallBack);
        }
        /// <summary>
        /// 转换成QueryableX
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public virtual IQueryableX<T, T2, T3> QueryableX<T, T2, T3>(RunSQLEventHandler e = null)
        {
            if (e != null) SQLCallBack += e;
            return new DataHelperX<T, T2, T3>(this.Config, SQLCallBack);
        }
        /// <summary>
        /// 转换成QueryableX
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public virtual IQueryableX<T, T2, T3, T4> QueryableX<T, T2, T3, T4>(RunSQLEventHandler e = null)
        {
            if (e != null) SQLCallBack += e;
            return new DataHelperX<T, T2, T3, T4>(this.Config, SQLCallBack);
        }
        /// <summary>
        /// 转换成QueryableX
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="T5">T5类型</typeparam>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public virtual IQueryableX<T, T2, T3, T4, T5> QueryableX<T, T2, T3, T4, T5>(RunSQLEventHandler e = null)
        {
            if (e != null) SQLCallBack += e;
            return new DataHelperX<T, T2, T3, T4, T5>(this.Config, SQLCallBack);
        }
        /// <summary>
        /// 转换成QueryableX
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="T5">T5类型</typeparam>
        /// <typeparam name="T6">T6类型</typeparam>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public virtual IQueryableX<T, T2, T3, T4, T5, T6> QueryableX<T, T2, T3, T4, T5, T6>(RunSQLEventHandler e = null)
        {
            if (e != null) SQLCallBack += e;
            return new DataHelperX<T, T2, T3, T4, T5, T6>(this.Config, SQLCallBack);
        }
        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion

    #region T1
    /// <summary>
    /// DataSQL 操作类
    /// Create Time : 2017/12/18 10:18:41
    /// Update Time : 2018/02/23 16:27:13
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class DataHelperX<T> : QueryableProvider<T>, IQueryableX<T>, IDisposable, ICloneable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX()
        {
            this.DataSQL = new DataSQL
            {
                ModelType = typeof(T)
            };
        }
        /// <summary>
        /// 设置数据库相关配置
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            this.Config = config;
            this.DataHelper = new DataHelper(config);
            this.DataSQL = new DataSQL
            {
                ModelType = typeof(T),
                Config = this.Config
            };
            if (e != null) this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID = Guid.NewGuid();
        /// <summary>
        /// 相关配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public override DataSQL DataSQL { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 方法

        #region 转换关联表
        /// <summary>
        /// SQL语句转换成IQueryableX
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <returns></returns>
        public IQueryableX<T> AsQueryableX(string SqlString)
        {
            if (SqlString.IsNullOrEmpty()) return this;
            var dataX = new DataHelperX<T>(this.Config, this.SQLCallBack);
            dataX.DataSQL.TableName = SqlString;
            dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
            dataX.DataSQL.LastSQLString = this.DataSQL.LastSQLString;
            dataX.DataSQL.LastSQLParameter = this.DataSQL.LastSQLParameter;
            dataX.DataSQL.Parameters = this.DataSQL.Parameters;
            return dataX;
        }
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <param name="SqlString">第二个SQL语句</param>
        /// <returns></returns>
        public IQueryableX<T, T2> AsQueryableX<T2>(string SqlString) where T2 : class, new()
        {
            var dataX = new DataHelperX<T, T2>(this.Config, this.SQLCallBack);
            dataX.DataSQL.TableName = new Dictionary<TableType, string>() {
                { TableType.T1, DataSQL.GetSQLString() }
            };
            dataX.DataSQL.Parameters = this.DataSQL.Parameters;
            if (SqlString.IsNotNullOrEmpty()) dataX.DataSQL.TableName.Add(TableType.T2, SqlString);
            dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
            dataX.DataSQL.LastSQLString = this.DataSQL.LastSQLString;
            dataX.DataSQL.LastSQLParameter = this.DataSQL.LastSQLParameter;
            return dataX;
        }
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <typeparam name="T3">类型</typeparam>
        /// <param name="SqlString2">第二个SQL语句</param>
        /// <param name="SqlString3">第三个SQL语句</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> AsQueryableX<T2, T3>(string SqlString2, string SqlString3)
            where T2 : class, new()
            where T3 : class, new()
        {
            var dataX = new DataHelperX<T, T2, T3>(this.Config, this.SQLCallBack);
            dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString() } };
            dataX.DataSQL.Parameters = this.DataSQL.Parameters;
            if (SqlString2.IsNotNullOrEmpty()) dataX.DataSQL.TableName.Add(TableType.T2, SqlString2);
            if (SqlString3.IsNotNullOrEmpty()) dataX.DataSQL.TableName.Add(TableType.T3, SqlString3);
            dataX.DataSQL.LastSQLString = this.DataSQL.LastSQLString;
            dataX.DataSQL.LastSQLParameter = this.DataSQL.LastSQLParameter;
            dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
            return dataX;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T> AS()
        {
            var dataX = new DataHelperX<T>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <returns></returns>
        public IQueryableX<T, T2> AS<T2>() where T2 : class, new()
        {
            var dataX = new DataHelperX<T, T2>(this.Config, this.SQLCallBack);
            dataX.DataSQL.TableName = new Dictionary<TableType, string>() {
                { TableType.T1, DataSQL.GetSQLString() }
            };
            dataX.DataSQL.Parameters = this.DataSQL.Parameters;
            dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
            dataX.DataSQL.LastSQLString = this.DataSQL.LastSQLString;
            dataX.DataSQL.LastSQLParameter = this.DataSQL.LastSQLParameter;
            return dataX;
        }
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <typeparam name="T3">类型</typeparam>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> AS<T2, T3>()
            where T2 : class, new()
            where T3 : class, new()
        {
            var dataX = new DataHelperX<T, T2, T3>(this.Config, this.SQLCallBack);
            dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString() } };
            dataX.DataSQL.Parameters = this.DataSQL.Parameters;
            dataX.DataSQL.LastSQLString = this.DataSQL.LastSQLString;
            dataX.DataSQL.LastSQLParameter = this.DataSQL.LastSQLParameter;
            dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
            return dataX;
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="T2">目标对象</typeparam>
        /// <returns></returns>
        public IQueryableX<T2> To<T2>() where T2 : class, new()
        {
            var dataX = new DataHelperX<T2>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        #endregion

        #region 扩展SQL Join
        /// <summary>
        /// 扩展SQL Join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Join<T2, TResult>(Expression<Func<T, T2, TResult>> funcOn) => this.Join(null, null, funcOn);
        /// <summary>
        /// 扩展SQL Join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Join<T2, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T, T2, TResult>> funcOn) => this.Join(null, func, funcOn);
        /// <summary>
        /// 左连接关联表（left join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null) => this.Join(JoinType.Left, funcOn, func);
        /// <summary>
        /// 右连接关联表（right join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null) => this.Join(JoinType.Right, funcOn, func);
        /// <summary>
        /// 全连接关联表（full join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null) => this.Join(JoinType.Full, funcOn, func);
        /// <summary>
        /// 内连接关联表（inner join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null) => this.Join(JoinType.Inner, funcOn, func);
        /// <summary>
        /// 合并连接关联表（union join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> UnionJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null) => this.Join(JoinType.Union, funcOn, func);
        /// <summary>
        /// 关联表
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="joinType">关联类型</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Join<T2>(JoinType joinType, Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null)
        {
            using (var dataX = new DataHelperX<T, T2>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, this.DataSQL.TableName } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
                dataX.DataSQL.JoinType = joinType;
                dataX.On(funcOn);
                dataX.DataSQL.SetWhere(this.DataSQL.GetWhere(), TableType.T1);
                if (func != null)
                    dataX.Where(func, TableType.T2);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Join<T2, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T, T2, TResult>> funcOn)
        {
            /*改造SQL关联优化*/
            using (var dataX = new DataHelperX<T, T2>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, this.DataSQL.TableName } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func, TableType.T1);
                dataX.DataSQL.SetWhere(this.DataSQL.GetWhere(), TableType.T1);
                dataX.Where(func2, TableType.T2);
                return dataX;
            }
            /*
             * this.Where(func);
            using (var dataX = new DataHelperX<T, T2>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, this.DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func2, TableType.T2);
                return dataX;
            }*/
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Join<T2, T3, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, TResult>> funcOn)
        {
            using (var dataX = new DataHelperX<T, T2, T3>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func, TableType.T2);
                dataX.Where(func3, TableType.T3);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="func3">T3条件</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Join<T2, T3, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, TResult>> funcOn)
        {
            this.Where(func);
            using (var dataX = new DataHelperX<T, T2, T3>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func2, TableType.T2);
                dataX.Where(func3, TableType.T3);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Join<T2, T3, T4, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, TResult>> funcOn)
        {
            using (var dataX = new DataHelperX<T, T2, T3, T4>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func, TableType.T2);
                dataX.Where(func3, TableType.T3);
                dataX.Where(func4, TableType.T4);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Join<T2, T3, T4, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, TResult>> funcOn)
        {
            this.Where(func);
            using (var dataX = new DataHelperX<T, T2, T3, T4>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func2, TableType.T2);
                dataX.Where(func3, TableType.T3);
                dataX.Where(func4, TableType.T4);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="T5">T5类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Join<T2, T3, T4, T5, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, TResult>> funcOn)
        {
            using (var dataX = new DataHelperX<T, T2, T3, T4, T5>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func, TableType.T2);
                dataX.Where(func3, TableType.T3);
                dataX.Where(func4, TableType.T4);
                dataX.Where(func5, TableType.T5);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="T5">T5类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Join<T2, T3, T4, T5, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, TResult>> funcOn)
        {
            this.Where(func);
            using (var dataX = new DataHelperX<T, T2, T3, T4, T5>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func2, TableType.T2);
                dataX.Where(func3, TableType.T3);
                dataX.Where(func4, TableType.T4);
                dataX.Where(func5, TableType.T5);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="T5">T5类型</typeparam>
        /// <typeparam name="T6">T6类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="func6">T6条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Join<T2, T3, T4, T5, T6, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> funcOn)
        {
            using (var dataX = new DataHelperX<T, T2, T3, T4, T5, T6>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func, TableType.T2);
                dataX.Where(func3, TableType.T3);
                dataX.Where(func4, TableType.T4);
                dataX.Where(func5, TableType.T5);
                dataX.Where(func6, TableType.T6);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="T4">T4类型</typeparam>
        /// <typeparam name="T5">T5类型</typeparam>
        /// <typeparam name="T6">T6类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="func6">T6条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Join<T2, T3, T4, T5, T6, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> funcOn)
        {
            this.Where(func);
            using (var dataX = new DataHelperX<T, T2, T3, T4, T5, T6>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() { { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") } };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func2, TableType.T2);
                dataX.Where(func3, TableType.T3);
                dataX.Where(func4, TableType.T4);
                dataX.Where(func5, TableType.T5);
                dataX.Where(func6, TableType.T6);
                return dataX;
            }
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TThird">On返回类型</typeparam>
        /// <typeparam name="TResult">结果实体类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="fResult">返回实体条件Lambda</param>
        /// <returns></returns>
        public List<TResult> Join<T2, TThird, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T, T2, TThird>> funcOn, Expression<Func<T, T2, TResult>> fResult)
            where T2 : class, new()
            where TResult : class, new()
        {
            this.Where(func);
            using (var dataX = new DataHelperX<T, T2>(this.Config, this.SQLCallBack))
            {
                dataX.DataSQL.TableName = new Dictionary<TableType, string>() {
                    { TableType.T1, DataSQL.GetSQLString().RemovePattern(@"\s*;\s*$") }
                };
                dataX.DataSQL.Parameters = this.DataSQL.Parameters;
                dataX.DataSQL.SpliceSQLTime += DataSQL.SpliceSQLTime;
                dataX.On(funcOn);
                dataX.Where(func2, TableType.T2);
                return dataX.ToList(fResult);
            }
        }
        #endregion

        #region 扩展SQL Top
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T> Take(int topCount)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount == 0) return this;
            this.DataSQL.Top = topCount;
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> Take(int topCount, Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount > 0) this.DataSQL.Top = topCount;
            this.Where(func);
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> TakeWhile(int topCount, Expression<Func<T, bool>> func)
        {
            return this.Take(topCount, func);
        }
        #endregion

        #region 扩展SQL Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T> OrderBy(string orderString)
        {
            if (orderString.IsNullOrEmpty()) return this;
            List<string> list = orderString.GetPatterns(@"\s+(?<a>asc|desc)\s*(,|$)");
            if (list.Contains(@"asc") && list.Contains("desc"))
            {
                this.DataSQL.SetOrderBy(orderString);
                return this;
            }

            if (orderString.IsMatch(@"\s+desc$")) return this.OrderByDescending(orderString);
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@",").IsNullOrEmpty()) return this;
            orderString = orderString.RemovePattern(@"(\s+|^)order\s+by\s+").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " asc,") + " asc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> ThenBy<TResult>(Expression<Func<T, TResult>> func)
        {
            return this.OrderBy(func);
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> OrderBy<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            string OrderByString = this.OrderByString(func, "asc");
            this.DataSQL.SetOrderBy(OrderByString);
            if (OrderByString.IsNullOrEmpty()) return this;
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T> OrderByDescending(string orderString)
        {
            if (orderString.IsNullOrEmpty()) return this;
            List<string> list = orderString.GetPatterns(@"\s+(?<a>asc|desc)\s*(,|$)");
            if (list.Contains(@"asc") && list.Contains("desc"))
            {
                this.DataSQL.SetOrderBy(orderString);
                return this;
            }

            if (orderString.IsMatch(@"\s+asc$")) return this.OrderBy(orderString);
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@",").IsNullOrEmpty()) return this;
            orderString = orderString.RemovePattern(@"(\s+|^)order\s+by\s+").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " desc,") + " desc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> ThenByDescending<TResult>(Expression<Func<T, TResult>> func)
        {
            return this.OrderByDescending(func);
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            string OrderByString = this.OrderByString(func, "desc");
            this.DataSQL.SetOrderBy(OrderByString);
            if (OrderByString == "") return this;
            return this;
        }
        #endregion

        #region 扩展SQL Group By 分组
        /// <summary>
        /// 扩展group by
        /// </summary>
        /// <param name="groupString">分组串</param>
        /// <returns></returns>
        public IQueryableX<T> GroupBy(string groupString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (groupString.IsNullOrEmpty() || groupString.ReplacePattern(@"[\s,]", "") == "") return this;
            groupString = groupString.ReplacePattern(@"\s+group\s+by\s+", "").ReplacePattern(@"\s+", "").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',');
            this.DataSQL.GroupByString.AddRange(groupString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            return this;
        }
        /// <summary>
        /// 扩展group by
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">分组Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> GroupBy<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    this.DataSQL.GroupByString.Add(a.Name);
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.GroupByString.Add(mex.Member.Name);
            }
            return this;
        }
        #endregion

        #region 扩展SQL DISTINCT
        /// <summary>
        /// 扩展SQL DISTINCT
        /// </summary>
        /// <param name="distinctString">Distinct列</param>
        /// <returns></returns>
        public IQueryableX<T> Distinct(string distinctString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (distinctString.IsNullOrEmpty() || distinctString.ReplacePattern(@"[\s,]", "") == "") return this;
            /*distinctString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(c =>
            {
                this.DataSQL.SetColumns("distinct({0})".format(c));
            });*/

            this.DataSQL.SetColumns("DISTINCT {0}".format(distinctString.RemovePattern(@"^distinct\s+")));
            return this;
        }
        /// <summary>
        /// 扩展SQL DISTINCT
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Distinct Lmabda</param>
        /// <returns></returns>
        public IQueryableX<T> Distinct<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            var Columns = "";
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    Columns += a.Name + ",";
                    //this.DataSQL.SetColumns("distinct({0})".format(a.Name));
                });
                this.DataSQL.SetColumns("DISTINCT " + Columns.Trim(','));
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.SetColumns("DISTINCT {0}".format(mex.Member.Name));
            }
            return this;
        }
        #endregion

        #region 扩展SQL SUM
        /// <summary>
        /// 扩展SQL SUM
        /// </summary>
        /// <param name="sumString">SUM列</param>
        /// <returns></returns>
        public IQueryableX<T> Sum(string sumString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (sumString.IsNullOrEmpty() || sumString.ReplacePattern(@"[\s,]", "") == "") return this;
            sumString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(c =>
            {
                this.DataSQL.SetColumns("SUM({0})".format(c));
            });
            return this;
        }
        /// <summary>
        /// 扩展SQL SUM
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Sum Lmabda</param>
        /// <returns></returns>
        public IQueryableX<T> Sum<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    this.DataSQL.SetColumns("SUM({0})".format(a.Name));
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.SetColumns("SUM({0})".format(mex.Member.Name));
            }
            return this;
        }
        #endregion

        #region 扩展SQL COUNT
        /// <summary>
        /// 条数 运用 explain来读取总条数
        /// </summary>
        /// <returns></returns>
        public int CountX()
        {
            if (this.DataSQL.Columns != null) this.DataSQL.Columns.Clear();
            string SQLString = String.Empty;
            if (this.DataSQL.Columns != null && this.DataSQL.Columns.Count == 0)
            {
                this.DataSQL.SetColumns("count(0)");
                SQLString = this.DataSQL.GetSQLString();
            }
            else
            {
                SQLString = $"select count(0) from ({this.DataSQL.GetSQLString()}) as AAAA";
            }
            int _CountX = 0;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            if ((DbProviderType.MySql | DbProviderType.Oracle | DbProviderType.Dameng).HasFlag(this.DataHelper.ProviderType))
            {
                SQLString = "EXPLAIN " + SQLString;
                var dt = this.DataHelper.ExecuteDataTable(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString));
                if (dt.Rows.Count > 0)
                {
                    _CountX = dt.Rows[0]["Rows"].ToCast<int>();
                }
            }
            else
            {
                _CountX = this.DataHelper.ExecuteScalar(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToCast<int>();
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return _CountX;
        }
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            if (this.DataSQL.Columns != null) this.DataSQL.Columns.Clear();
            //this.DataSQL.ClearColumns("count(0)");
            //string SQLString = this.DataSQL.GetSQLString();

            string SQLString = String.Empty;
            if (this.DataSQL.Columns != null && this.DataSQL.Columns.Count == 0)
            {
                this.DataSQL.SetColumns("count(0)");
                SQLString = this.DataSQL.GetSQLString();
            }
            else
            {
                SQLString = $"select count(0) from ({this.DataSQL.GetSQLString()}) as AAAA";
            }
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            int _Count = this.DataHelper.ExecuteScalar(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToCast<int>();
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return _Count;
        }
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, Boolean>> func)
        {
            this.Where(func);
            return this.Count();
        }
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <param name="countString">COUNT列</param>
        /// <returns></returns>
        public IQueryableX<T> Count(string countString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (countString.IsNullOrEmpty() || countString.ReplacePattern(@"[\s,]", "") == "") return this;
            countString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(c =>
            {
                this.DataSQL.SetColumns("COUNT({0})".format(c));
            });
            return this;
        }
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Count Lmabda</param>
        /// <returns></returns>
        public IQueryableX<T> Count<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    this.DataSQL.SetColumns("COUNT({0})".format(a.Name));
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.SetColumns("COUNT({0})".format(mex.Member.Name));
            }
            return this;
        }
        #endregion

        #region 扩展SQL AVG
        /// <summary>
        /// 扩展SQL AVG
        /// </summary>
        /// <param name="avgString">Avg列</param>
        /// <returns></returns>
        public IQueryableX<T> Avg(string avgString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (avgString.IsNullOrEmpty() || avgString.ReplacePattern(@"[\s,]", "") == "") return this;
            avgString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(c =>
            {
                this.DataSQL.SetColumns("AVG({0})".format(c));
            });
            return this;
        }
        /// <summary>
        /// 扩展SQL AVG
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Avg Lmabda</param>
        /// <returns></returns>
        public IQueryableX<T> Avg<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    this.DataSQL.SetColumns("AVG({0})".format(a.Name));
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.SetColumns("AVG({0})".format(mex.Member.Name));
            }
            return this;
        }
        #endregion

        #region 扩展SQL MAX
        /// <summary>
        /// 扩展SQL MAX
        /// </summary>
        /// <param name="maxString">Max列</param>
        /// <returns></returns>
        public IQueryableX<T> Max(string maxString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (maxString.IsNullOrEmpty() || maxString.ReplacePattern(@"[\s,]", "") == "") return this;
            maxString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(c =>
            {
                this.DataSQL.SetColumns("MAX({0})".format(c));
            });
            return this;
        }
        /// <summary>
        /// 扩展SQL MAX
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Max Lmabda</param>
        /// <returns></returns>
        public IQueryableX<T> Max<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    this.DataSQL.SetColumns("MAX({0})".format(a.Name));
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.SetColumns("MAX({0})".format(mex.Member.Name));
            }
            return this;
        }
        #endregion

        #region 扩展SQL MIN
        /// <summary>
        /// 扩展SQL MIN
        /// </summary>
        /// <param name="minString">Min列</param>
        /// <returns></returns>
        public IQueryableX<T> Min(string minString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (minString.IsNullOrEmpty() || minString.ReplacePattern(@"[\s,]", "") == "") return this;
            minString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(c =>
            {
                this.DataSQL.SetColumns("MIN({0})".format(c));
            });
            return this;
        }
        /// <summary>
        /// 扩展SQL MIN
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Min Lmabda</param>
        /// <returns></returns>
        public IQueryableX<T> Min<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (func.Body is NewExpression nex)
            {
                nex.Members.Each(a =>
                {
                    this.DataSQL.SetColumns("MIN({0})".format(a.Name));
                });
            }
            else if (func.Body is MemberExpression mex)
            {
                this.DataSQL.SetColumns("MIN({0})".format(mex.Member.Name));
            }
            return this;
        }
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity();
        }
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <returns></returns>
        [Obsolete("请使用First方法")]
        public T FirstOrDefault()
        {
            return this.First();
        }
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        [Obsolete("请使用First方法")]
        public T FirstOrDefault(Expression<Func<T, bool>> func)
        {
            return this.First(func);
        }
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public T First(Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            this.Where(func);
            return this.ToEntity();
        }
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        public TResult First<TResult>()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity<TResult>();
        }
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public TResult First<TResult>(Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            this.Where(func);
            return this.ToEntity<TResult>();
        }
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity();
        }
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public T Last(Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            this.Where(func);
            return this.ToEntity();
        }
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        public TResult Last<TResult>()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity<TResult>();
        }
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public TResult Last<TResult>(Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            this.Where(func);
            return this.ToEntity<TResult>();
        }
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        public IQueryableX<T> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            this.DataSQL.SQLType = SQLType.limit;
            this.DataSQL.Limit = skipCount;
            return this;
        }
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> Skip(int skipCount, Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = SQLType.limit;
            if (skipCount > 0) this.DataSQL.Limit = skipCount;
            this.Where(func);
            return this;
        }
        /// <summary>
        /// 跳过几条数据 遇到条件跳过
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> SkipWhile(int skipCount, Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = SQLType.limit;
            if (skipCount > 0) this.DataSQL.Limit = skipCount;
            if (func == null) return this;
            if (func.Body is BinaryExpression be)
            {
                this.DataSQL.SetWhere(UnBinaryExpressionProvider(be.Left, be.Right, be.NodeType));
            }
            else
                this.DataSQL.SetWhere(UnExpressionRouter(func.Body));
            return this;
        }
        #endregion

        #region 扩展Any
        /// <summary>
        /// 扩展 Any
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public Boolean Any(Expression<Func<T, bool>> func)
        {
            this.DataSQL.SQLType = SQLType.exists;
            if (func != null) this.Where(func);
            this.DataSQL.Top = 1;
            string SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            object val = this.DataHelper.ExecuteScalar(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString));
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return (val ?? "0").ToString().ToInt32() == 1;
        }
        #endregion

        #region 返回数据实体
        /// <summary>
        /// 返回数据实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        public TResult ToEntity<TResult>()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            TResult data;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();

            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<TResult>();
            if (data != null)
            {
                /*设置分表*/
                var p = typeof(TResult).GetProperty("TableName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (data != null) p?.SetValue(data, this.DataSQL.TableName);
            }
            SetForeignKey<TResult>(data);
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToEntity<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut.Value);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = (TResult)val;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<TResult>();
                    }
                }
            }
            else
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToEntity<TResult>();
            */
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        /// <summary>
        /// 返回数据实体
        /// </summary>
        ///<param name="type">返回类型</param>
        /// <returns></returns>
        public object ToEntity(Type type)
        {
            type = type ?? this.DataSQL.ModelType;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = type.GetCustomAttribute<CacheDataAttribute>();
            object data;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();

            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToEntity(type);
            var baseType = type.GetValueType();
            if (data != null && (baseType == ValueTypes.Class || baseType == ValueTypes.Class || baseType == ValueTypes.Anonymous))
            {
                /*设置分表*/
                var p = type.GetProperty("TableName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                p?.SetValue(data, this.DataSQL.TableName);
            }
            SetForeignKey(data);

            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        /// <summary>
        /// 设置对象有外键数据
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="data">数据</param>
        public void SetForeignKey<TResult>(TResult data)
        {
            if (data == null) return;
            var ForeignList = new Dictionary<Type, List<PropertyInfo>>();
            data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Instance).Each(p =>
            {
                var foreign = p.GetCustomAttribute<ForeignAttribute>(false);
                if (foreign != null)
                {
                    if (ForeignList.ContainsKey(foreign.Type))
                        ForeignList[foreign.Type].Add(p);
                    else
                        ForeignList.Add(foreign.Type, new List<PropertyInfo> { p });
                }
            });
            if (ForeignList.Any())
            {
                ForeignList.Each(d =>
                {
                    var tblType = d.Key;
                    var values = d.Value;
                    var tblAttr = tblType.GetTableAttribute();
                    var tblName = tblAttr == null ? tblType.Name : tblAttr.Name;
                    var forList = from v in values select v.GetCustomAttribute<ForeignAttribute>(false);
                    var foreignAttr = forList.FirstOrDefault(a => a.Key.IsNotNullOrEmpty() && a.Field.IsNotNullOrEmpty() && a.Format.IsNotNullOrEmpty() && a.Type != null);
                    if (foreignAttr == null) return;
                    var fProperty = typeof(T).GetProperty(foreignAttr.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn);
                    if (fProperty == null) return;
                    var val = fProperty.GetValue(data).GetSqlValue();
                    if (val.IndexOf(",") > -1) val = "'" + val.Trim('\'').ReplacePattern(@",", "','") + "'";
                    /*
                     * 设置缓存
                     */
                    var SQL = $"select * from {tblName} where {foreignAttr.Format.ReplacePattern(@"\$?\{" + foreignAttr.Key + @"\}", val)}";
                    var cacheKey = "FOREIGN-" + SQL.MD5();
                    var cacheValue = CacheHelper.Get(cacheKey);
                    List<object> dt;
                    if (cacheValue != null)
                        dt = cacheValue as List<object>;
                    else
                    {
                        dt = this.DataHelper.ExecuteDataTable(SQL).ToList(tblType);
                        if (dt == null) return;
                        CacheHelper.Set(cacheKey, dt, TimeSpan.FromMinutes(10));
                    }

                    values.Each(f =>
                    {
                        var valueType = f.PropertyType.GetValueType();
                        var foreighKey = f.GetCustomAttribute<ForeignAttribute>();
                        if (valueType == ValueTypes.IEnumerable || valueType == ValueTypes.List)
                        {
                            var list = Activator.CreateInstance(f.PropertyType) as System.Collections.IList;
                            dt.Each(a =>
                                                {
                                                    var model = Activator.CreateInstance(f.PropertyType.GenericTypeArguments[0]);
                                                    model = a;
                                                    list.Add(model);
                                                });
                            f.SetValue(data, list);
                        }
                        else if (valueType == ValueTypes.Class || valueType == ValueTypes.Struct)
                            f.SetValue(data, dt.FirstOrDefault());
                        else
                            f.SetValue(data, ((IEntity)dt.FirstOrDefault())[f.Name]);
                    });
                });
            }
        }
        /// <summary>
        /// 返回数据实体
        /// </summary>
        /// <returns></returns>
        public T ToEntity()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(T).GetCustomAttribute<CacheDataAttribute>();
            T data = default(T);
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            ValueTypes type = typeof(T).GetValueType();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            _SQLString = _SQLString.SQLFormat(this.DataHelper.ProviderType);
            if (type == ValueTypes.Class || type == ValueTypes.Struct || type == ValueTypes.Anonymous)
            {
                if (this.DataSQL.TableName.IndexOf("_FB_") > -1)
                {
                    data = this.DataHelper.ExecuteDataTable(_SQLString, CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                }
                else
                {
                    /*查询分表*/
                    var tbls = TableSplitConfig.Current.List?.Find(a => a.Name.EqualsIgnoreCase(this.DataSQL.TableName));
                    if (tbls != null)
                    {
                        var a = _SQLString.GetMatch(@"(\s|\()" + this.DataSQL.FieldFormat(tbls.SplitField).ToRegexEscape() + @"\s*=\s*(?<a>@[a-z_0-9]+)");
                        if (a.IsNullOrEmpty())
                        {
                            tbls.TableNames.Reverse();
                            tbls.TableNames.Each(b =>
                            {
                                data = this.DataHelper.ExecuteDataTable(_SQLString.Replace(tbls.Name, b.Name), CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                if (data != null)
                                {
                                    this.DataSQL.TableName = b.Name;
                                    return false;
                                }

                                return true;
                            });
                        }
                        else
                        {
                            if (this.DataSQL.Parameters.TryGetValue(a, out var val))
                            {
                                var _ = val.ToString();
                                if (_.IndexOf(":") > -1)
                                {
                                    var _val = _.ToDateTime();
                                    var tb = tbls.TableNames.Find(b => b.Begin.ToCast<DateTime>() <= _val && b.End.ToCast<DateTime>() > _val);
                                    if (tb == null)
                                        data = this.DataHelper.ExecuteDataTable(_SQLString, CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                    else
                                    {
                                        data = this.DataHelper.ExecuteDataTable(_SQLString.Replace(tbls.Name, tb.Name), CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                        this.DataSQL.TableName = tb.Name;
                                    }
                                }
                                else if (_.IsNumberic())
                                {
                                    if (tbls.SplitType == TableSplitType.AutoID)
                                    {
                                        var _val = _.ToLong();
                                        var tb = tbls.TableNames.Find(b => b.Begin.ToLong() <= _val && b.End.ToString().ToLong() > _val);
                                        if (tb == null)
                                            data = this.DataHelper.ExecuteDataTable(_SQLString, CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                        else
                                        {
                                            data = this.DataHelper.ExecuteDataTable(_SQLString.Replace(tbls.Name, tb.Name), CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                            this.DataSQL.TableName = tb.Name;
                                        }
                                    }
                                    else
                                    {
                                        var _val = _.ToDateTime().ToTimeStamp();
                                        var tb = tbls.TableNames.Find(b => b.Begin.ToDateTime().ToTimeStamp() <= _val && b.End.ToString().ToDateTime().ToTimeStamp() > _val);
                                        if (tb == null)
                                            data = this.DataHelper.ExecuteDataTable(_SQLString, CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                        else
                                        {
                                            data = this.DataHelper.ExecuteDataTable(_SQLString.Replace(tbls.Name, tb.Name), CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                                            this.DataSQL.TableName = tb.Name;
                                        }
                                    }
                                }
                            }
                            else
                                data = default(T);
                        }
                    }
                    else
                    {
                        data = this.DataHelper.ExecuteDataTable(_SQLString, CommandType.Text, this.GetDbParameters(SQLString)).ToEntity<T>();
                    }
                }
                if (data != null)
                {
                    /*设置分表*/
                    var p = typeof(T).GetProperty("TableName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    p?.SetValue(data, this.DataSQL.TableName);
                }
            }
            else if (type == ValueTypes.Enum || type == ValueTypes.String || type == ValueTypes.Value)
            {
                data = this.DataHelper.ExecuteScalar(_SQLString, CommandType.Text, this.GetDbParameters(SQLString)).ToCast<T>();
            }
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    if (type == ValueTypes.Class || type == ValueTypes.Struct || type == ValueTypes.Anonymous)
                    {
                        DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                        data = _data.ToEntity<T>();
                    }
                    else if (type == ValueTypes.Enum || type == ValueTypes.String || type == ValueTypes.Value)
                    {
                        var _val = this.DataHelper.ExecuteScalar(SQLString, CommandType.Text, this.GetDbParameters());
                        data = _val.ToCast<T>();
                    }
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = (T)val;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<T>();
                    }
                }
            }
            else
            {
                if (type == ValueTypes.Class || type == ValueTypes.Struct || type == ValueTypes.Anonymous)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToEntity<T>();
                }
                else if (type == ValueTypes.Enum || type == ValueTypes.String || type == ValueTypes.Value)
                {
                    var _val = this.DataHelper.ExecuteScalar(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _val.ToCast<T>();
                }
            }*/
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 返回数据实体集合
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if ((this.DataSQL.Columns == null || this.DataSQL.Columns.Count == 0 || (this.DataSQL.Columns.Count == 1 && this.DataSQL.Columns[0] == "*")) && (this.DataSQL.GroupByString != null && this.DataSQL.GroupByString.Count > 0))
            {
                this.DataSQL.SetColumns(this.DataSQL.GroupByString[0]);
            }
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(T).GetCustomAttribute<CacheDataAttribute>();
            List<T> data;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();

            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<T>();
            var type = typeof(T);
            var baseType = type.GetValueType();
            if (data != null && (baseType == ValueTypes.Class || baseType == ValueTypes.Class || baseType == ValueTypes.Anonymous))
            {
                /*设置分表*/
                var p = typeof(T).GetProperty("TableName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                data.Each(a =>
                {
                    p?.SetValue(a, this.DataSQL.TableName);
                    SetForeignKey(a);
                });
            }
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
             * if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<T>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<T>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<T>>();
                    }
                }
            }
            else
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<T>();
            */
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<T> ToList(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            this.Skip((page - 1) * pageSize).Take(pageSize);
            return this.ToList();
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <param name="pageCount">共有多少页</param>
        /// <param name="counts">共有多少条</param>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public List<T> ToList(int page, int pageSize, out int pageCount, out int counts, string primaryKey = "")
        {
            this.DataSQL.SQLType = SQLType.select;
            pageCount = 0; counts = 0;
            Type t = typeof(T);
            t = this.DataSQL.ModelType ?? t;
            string TableName = this.DataSQL.TableName;
            if (TableName.IsNullOrEmpty())
            {
                TableAttribute Table = t.GetTableAttribute();
                TableName = Table == null ? t.Name : Table.Name ?? t.Name;
            }
            string OrderColumn = "", OrderType = "";
            if (this.DataSQL.OrderByString.IsNullOrEmpty())
            {
                t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
                {
                    if (p.IsIndexer()) return true;
                    ColumnAttribute Column = p.GetCustomAttribute<ColumnAttribute>();
                    if (Column == null || !Column.PrimaryKey)
                        return true;
                    else
                    {
                        OrderColumn = Column.Name ?? p.Name;
                        OrderType = "asc";
                        return false;
                    }
                });
                if (OrderColumn.IsNullOrEmpty()) return new List<T>();
            }
            else
            {
                Dictionary<string, string> list = this.DataSQL.GetOrderBy().GetMatchs(@"\s*order\s+by\s+(?<JoinKey>[^,\s]+)[\s\S]*?(?<b>(asc|desc))\s*$");
                if (list.ContainsKey("JoinKey") && list.ContainsKey("b"))
                {
                    OrderColumn = list["JoinKey"]; OrderType = list["b"];
                }
                else return new List<T>();
            }
            if ((this.DataSQL.Columns == null || this.DataSQL.Columns.Count == 0) && (this.DataSQL.GroupByString != null && this.DataSQL.GroupByString.Count > 0))
            {
                this.DataSQL.SetColumns(this.DataSQL.GroupByString);
            }
            string Columns = this.DataSQL.GetColumns(), WhereString = this.DataSQL.GetWhere() ?? "";
            if (Columns.IsNullOrEmpty()) Columns = "*";
            this.DataSQL.SQLString = "exec proc_ReadData('{0}','{1}','{2}','{3}','{4}',{5},{6},{7} out,{8} out,'{9}');".format(TableName, Columns, WhereString, OrderColumn, OrderType, page, pageSize, pageCount, counts, primaryKey);
            this.DataSQL.CreateCacheKey();
            CacheDataAttribute cacheData = t.GetCustomAttribute<CacheDataAttribute>();
            List<T> data = new List<T>();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            if (this.DataSQL.IsCache(cacheData))
            {
                var val = this.DataSQL.GetCacheData<List<T>>();
                if (val == null)
                {
                    data = this.DataHelper.Select<T>(TableName, Columns, WhereString, OrderColumn, OrderType, page, pageSize, out pageCount, out counts, primaryKey);
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<T>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<T>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.Select<T>(TableName, Columns, WhereString, OrderColumn, OrderType, page, pageSize, out pageCount, out counts, primaryKey);
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        public List<TResult> ToList<TResult>()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if ((this.DataSQL.Columns == null || this.DataSQL.Columns.Count == 0 || (this.DataSQL.Columns.Count == 1 && this.DataSQL.Columns[0] == "*")) && (this.DataSQL.GroupByString != null && this.DataSQL.GroupByString.Count > 0))
            {
                this.DataSQL.SetColumns(this.DataSQL.GroupByString[0]);
            }
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<TResult>();
            var type = typeof(TResult);
            var baseType = type.GetValueType();
            if (data != null && (baseType == ValueTypes.Class || baseType == ValueTypes.Class || baseType == ValueTypes.Anonymous))
            {
                /*设置分表*/
                var p = type.GetProperty("TableName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                data.Each(a =>
                {
                    p?.SetValue(a, this.DataSQL.TableName);
                    SetForeignKey(a);
                });
            }
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    var _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<TResult>();
            }
            */
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public List<object> ToList(Type type)
        {
            type = type ?? this.DataSQL.ModelType;
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if ((this.DataSQL.Columns == null || this.DataSQL.Columns.Count == 0 || (this.DataSQL.Columns.Count == 1 && this.DataSQL.Columns[0] == "*")) && (this.DataSQL.GroupByString != null && this.DataSQL.GroupByString.Count > 0))
            {
                this.DataSQL.SetColumns(this.DataSQL.GroupByString[0]);
            }
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = type.GetCustomAttribute<CacheDataAttribute>();
            List<object> data;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList(type);
            var baseType = type.GetValueType();
            if (data != null && (baseType == ValueTypes.Class || baseType == ValueTypes.Class || baseType == ValueTypes.Anonymous))
            {
                /*设置分表*/
                var p = type.GetProperty("TableName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                data.Each(a =>
                {
                    p?.SetValue(a, this.DataSQL.TableName);
                    SetForeignKey(a);
                });
            }

            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            this.Skip((page - 1) * pageSize).Take(pageSize);
            return this.ToList<TResult>();
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <param name="pageCount">共有多少页</param>
        /// <param name="counts">共有多少条</param>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(int page, int pageSize, out int pageCount, out int counts, string primaryKey = "")
        {
            this.DataSQL.SQLType = SQLType.select;
            pageCount = 0; counts = 0;
            Type t = typeof(TResult);
            t = this.DataSQL.ModelType ?? t;
            string TableName = this.DataSQL.TableName;
            if (TableName.IsNullOrEmpty())
            {
                TableAttribute Table = t.GetTableAttribute();
                TableName = Table == null ? t.Name : Table.Name ?? t.Name;
            }
            string OrderColumn = "", OrderType = "";
            if (this.DataSQL.OrderByString.IsNullOrEmpty())
            {
                t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
                {
                    if (p.IsIndexer()) return true;
                    ColumnAttribute Column = p.GetCustomAttribute<ColumnAttribute>();
                    if (Column == null || !Column.PrimaryKey)
                        return true;
                    else
                    {
                        OrderColumn = Column.Name ?? p.Name;
                        OrderType = "asc";
                        return false;
                    }
                });
                if (OrderColumn.IsNullOrEmpty()) return new List<TResult>();
            }
            else
            {
                Dictionary<string, string> list = this.DataSQL.GetOrderBy().GetMatchs(@"\s*order\s+by\s+(?<JoinKey>[^,\s]+)[\s\S]*?(?<b>(asc|desc))\s*$");
                if (list.ContainsKey("JoinKey") && list.ContainsKey("b"))
                {
                    OrderColumn = list["JoinKey"]; OrderType = list["b"];
                }
                else return new List<TResult>();
            }
            if ((this.DataSQL.Columns == null || this.DataSQL.Columns.Count == 0) && (this.DataSQL.GroupByString != null && this.DataSQL.GroupByString.Count > 0))
            {
                this.DataSQL.SetColumns(this.DataSQL.GroupByString);
            }
            string Columns = this.DataSQL.GetColumns(), WhereString = this.DataSQL.GetWhere() ?? "";
            if (Columns.IsNullOrEmpty()) Columns = "*";
            this.DataSQL.SQLString = "exec proc_ReadData('{0}','{1}','{2}','{3}','{4}',{5},{6},out {7},out {8},'{9}');".format(TableName, Columns, WhereString, OrderColumn, OrderType, page, pageSize, pageCount, counts, primaryKey);
            this.DataSQL.CreateCacheKey();
            CacheDataAttribute cacheData = t.GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data = new List<TResult>();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData<List<TResult>>();
                if (val == null)
                {
                    data = this.DataHelper.Select<TResult>(TableName, Columns, WhereString, OrderColumn, OrderType, page, pageSize, out pageCount, out counts, primaryKey);
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.Select<TResult>(TableName, Columns, WhereString, OrderColumn, OrderType, page, pageSize, out pageCount, out counts, primaryKey);
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 插入数据

        #region 批量插入数据
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">对象组</param>
        /// <returns></returns>
        public Boolean Inserts<TOther>(IEnumerable<TOther> models) where TOther : class
        {
            if (models == null || !models.Any()) return false;
            this.DataSQL.SQLType = SQLType.insert;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            string SQLTemplate = "insert into ${TableName} (${columns}) values(${Values});";
            Type t = typeof(TOther);
            this.DataSQL.ModelType = t;
            var i = 0;
            var f = true;
            models.Each(model =>
            {
                if (model == null) return;
                i++;
                string Columns = "", Values = "";
                Type _t = model.GetType();
                if (_t.GetBaseTypeNames().Contains(typeof(Model.Entity<>).Name))
                {
                    var GetDirty = _t.GetMethod("GetDirty");
                    if (GetDirty != null)
                    {
                        var Dirtys = GetDirty.Invoke(model, null) as Model.DirtyCollection;
                        if (Dirtys.Count > 0)
                        {
                            f = false;
                            Dirtys.ToArray().Each(k =>
                            {
                                var p = _t.GetProperty(k, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                                if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                                object value = p.GetValue(model, null);
                                ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();
                                if (column != null && column.AutoIncrement) return;

                                Type pType = p.PropertyType;
                                if (!pType.Name.IsMatch(@"^List`") && pType.ToString().IsMatch(@"^System\."))
                                {
                                    string d = Columns.IsNullOrEmpty() ? "" : ",";
                                    Columns += d + FieldFormat(column == null ? p.Name : column.Name.IsNullOrEmpty() ? p.Name : column.Name);
                                    //Values += d + "'" + value.GetSqlValue() + "'";
                                    Values += d + value.GetSqlValue();
                                }
                            });
                        }
                    }
                }
                if (f)
                {
                    _t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
                    {
                        if (!p.CanRead || !p.CanWrite || p.IsIndexer()) return;
                        if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                        object value = p.GetValue(model, null);
                        if (value != null)
                        {
                            ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();
                            if (column != null && column.AutoIncrement) return;

                            Type pType = p.PropertyType;
                            if (pType.IsValueType())
                            {
                                string d = Columns.IsNullOrEmpty() ? "" : ",";
                                Columns += d + FieldFormat(column == null ? p.Name : column.Name.IsNullOrEmpty() ? p.Name : column.Name);
                                //Values += d + "'" + value.GetSqlValue() + "'";
                                Values += d + value.GetSqlValue();
                            }
                        }
                    });
                }
                TableAttribute Table = _t.GetTableAttribute();
                string TableName = Table == null ? _t.Name : Table.Name ?? _t.Name;
                this.DataSQL.SQLString += SQLTemplate.format(new Dictionary<string, string>
                {
                    {"TableName",TableName },
                    {"columns",Columns },
                    {"Values",Values }
                });
            });
            this.DataSQL.SQLParameter = this.DataSQL.GetSQLParameter();
            sTime.Stop();
            this.DataSQL.SpliceSQLTime += sTime.ElapsedMilliseconds;
            sTime.Restart();
            int M = this.DataHelper.ExecuteNonQuery(this.DataSQL.SQLString, CommandType.Text, this.GetDbParameters(this.DataSQL.SQLString));
            if (M == -1 && !this.DataHelper.ErrorMessage.IsNullOrEmpty() && this.DataHelper.ErrorMessage.IsMatch(@"对象名 '[^']+' 无效"))
            {
                if (this.CreateTable<TOther>()) return this.Inserts(models);
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return M > 0;
        }
        #endregion

        #region 插入数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <returns></returns>
        public IQueryableX<T> InsertQ<TOther>(TOther model) where TOther : class
        {
            if (model == null) return this;
            this.DataSQL.SQLType = SQLType.insert;
            this.DataSQL.Columns = new List<string>();
            this.DataSQL.UpdateColumns = new List<object>();
            Type t = model.GetType();
            this.DataSQL.ModelType = t;
            bool f = true;
            if (t.GetBaseTypeNames().Contains(typeof(Model.Entity<>).Name))
            {
                var GetDirty = t.GetMethod("GetDirty");
                if (GetDirty != null)
                {
                    if (GetDirty.Invoke(model, null) is Model.DirtyCollection Dirtys && Dirtys.Count > 0)
                    {
                        f = false;
                        Dirtys.ToArray().Each(k =>
                        {
                            var p = t.GetProperty(k, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            if (p == null) return;
                            if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                            object value = p.GetValue(model, null);
                            var Column = p.GetCustomAttribute<ColumnAttribute>();
                            if (Column != null && Column.AutoIncrement) return;
                            Type _t = p.PropertyType;
                            if (!_t.Name.IsMatch(@"^List`") && _t.ToString().IsMatch(@"^System\."))
                            {
                                if (Column == null || !Column.AutoIncrement)
                                {
                                    var columnName = Column == null ? p.Name : Column.Name.IsNullOrEmpty() ? p.Name : Column.Name;
                                    this.AddParam($"@{columnName}", value);
                                    this.DataSQL.SetColumns(FieldFormat(columnName));
                                    this.DataSQL.SetUpdateColumns(value == null ? "null" : $"@{columnName}");
                                }
                            }
                        });
                    }
                }
            }
            if (f)
            {
                var ValueType = t.GetValueType();
                t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
                {
                    if (ValueType != ValueTypes.Anonymous && (!p.CanRead || !p.CanWrite || p.IsIndexer())) return;
                    if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                    if (!p.CanRead || !p.CanWrite || p.IsIndexer()) return;
                    object value = p.GetValue(model, null);
                    if (value != null)
                    {
                        ColumnAttribute Column = p.GetCustomAttribute<ColumnAttribute>();
                        if (Column != null && Column.AutoIncrement) return;

                        Type _t = p.PropertyType;
                        if (_t.IsValueType())
                        {
                            if (Column == null || !Column.AutoIncrement)
                            {
                                var columnName = Column == null ? p.Name : Column.Name.IsNullOrEmpty() ? p.Name : Column.Name;
                                this.AddParam($"@{columnName}", value);
                                this.DataSQL.SetColumns(FieldFormat(columnName));
                                this.DataSQL.SetUpdateColumns(value == null ? "null" : $"@{columnName}");
                            }
                        }
                    }
                });
            }
            return this;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <returns></returns>
        public Boolean Insert<TOther>(TOther model) where TOther : class
        {
            if (model == null) return false;
            this.InsertQ(model);
            /*判断当前表有没有分表*/
            var tblsplit = TableSplitConfig.Current;
            var tbls = tblsplit.List?.Find(a => a.Name.EqualsIgnoreCase(this.DataSQL.TableName));
            if (tbls != null)
            {
                /*创建分表*/
                var tblName = tbls.GetTableName();
                if (!tbls.Exists(tblName))
                {
                    this.DataHelper.ExecuteNonQuery(tbls.CreateTableSQL.format(tblName));
                    tbls.AddTableSplit(tblName);
                    tblsplit.Save();
                }
                this.DataSQL.TableName = tblName;
            }
            string SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            int Non = 0;
            var Params = this.GetDbParameters();
            if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
            {
                //SQLString = SQLString.ReplacePattern(@"@[a-z_0-9]+(\s|,|\)|;|$)", "?$1");
                /*
                 * 用SQL语句参数偶尔会提示格式不正确.
                 */
                SQLString = SQLString.ReplacePattern(@"(?<a>@[a-z_0-9]+)(?<b>(\s|,|\)|;|$))", m =>
                 {
                     var a = m.Groups["a"].Value.Trim(new char[] { '@', ':' });
                     var b = m.Groups["b"].Value;
                     var val = Params.Where(c => c.ParameterName.IsMatch(@"^[@:]?" + a + "$")).FirstOrDefault();
                     return (val == null ? a : val.Value.GetSqlValue()) + b;
                 });
                Non = this.DataHelper.ExecuteNonQuery(SQLString);
            }
            else
            {
                Non = this.DataHelper.ExecuteNonQuery(SQLString, CommandType.Text, Params);
            }
            if (Non == 0 && !this.DataHelper.ErrorMessage.IsNullOrEmpty() && this.DataHelper.ErrorMessage.IsMatch(@"对象名 '[^']+' 无效"))
            {
                if (this.CreateTable<TOther>()) return this.Insert(model);
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return Non > 0;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <returns>返回自增长ID</returns>
        public Int64 Add<TOther>(TOther model) where TOther : class
        {
            return this.Insert(model, out Int64 ID) ? ID : 0;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <param name="ID">自增长ID</param>
        /// <returns></returns>
        public Boolean Insert<TOther>(TOther model, out Int64 ID) where TOther : class
        {
            ID = 0;
            if (model == null) return false;
            this.InsertQ(model);
            this.DataSQL.SQLType = SQLType.AutoIncrement;
            string SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var Params = this.GetDbParameters();
            if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
            {
                //SQLString = SQLString.ReplacePattern(@"@[a-z_0-9]+(\s|,|\)|;|$)", "?$1");
                /*
                 * 用SQL语句参数偶尔会提示格式不正确.
                 */
                SQLString = SQLString.ReplacePattern(@"(?<a>@[a-z_0-9]+)(?<b>(\s|,|\)|;|$))", m =>
                {
                    var a = m.Groups["a"].Value.Trim(new char[] { '@', ':' });
                    var b = m.Groups["b"].Value;
                    var val = Params.Where(c => c.ParameterName.IsMatch(@"^[@:]?" + a + "$")).FirstOrDefault();
                    return (val == null ? a : val.Value.GetSqlValue()) + b;
                });
                ID = this.DataHelper.ExecuteScalar(SQLString).ToCast<Int64>();
            }
            else
            {
                ID = this.DataHelper.ExecuteScalar(SQLString, CommandType.Text, Params).ToCast<Int64>();
            }
            if (ID == 0 && !this.DataHelper.ErrorMessage.IsNullOrEmpty() && this.DataHelper.ErrorMessage.IsMatch(@"对象名 '[^']+' 无效"))
            {
                if (this.CreateTable<TOther>()) return this.Insert(model, out ID);
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return ID > 0;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <returns></returns>
        public IQueryableX<T> InsertQ<TOther>(Expression<Func<T, TOther>> fResult)
        {
            this.DataSQL.SQLType = SQLType.insert;
            this.ExpressionRouter(fResult.Body).GetMatches(@"\s*(?<columnName>\[[^\s]+\])\s*=\s*(?<columnValue>@ParamName\d+)").Each(a =>
            {
                this.DataSQL.SetColumns(a["columnName"]);
                this.DataSQL.SetUpdateColumns(a["columnValue"]);
            });
            return this;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <returns></returns>
        public Boolean Insert<TOther>(Expression<Func<T, TOther>> fResult)
        {
            this.InsertQ(fResult);
            if (this.DataSQL.UpdateColumns == null || this.DataSQL.UpdateColumns.Count == 0 || this.DataSQL.Columns == null || this.DataSQL.Columns.Count == 0) return false;
            string SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            int Non = 0;
            var Params = this.GetDbParameters();
            if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
            {
                //SQLString = SQLString.ReplacePattern(@"@[a-z_0-9]+(\s|,|\)|;|$)", "?$1");
                /*
                 * 用SQL语句参数偶尔会提示格式不正确.
                 */
                SQLString = SQLString.ReplacePattern(@"(?<a>@[a-z_0-9]+)(?<b>(\s|,|\)|;|$))", m =>
                {
                    var a = m.Groups["a"].Value.Trim(new char[] { '@', ':' });
                    var b = m.Groups["b"].Value;
                    var val = Params.Where(c => c.ParameterName.IsMatch(@"^[@:]?" + a + "$")).FirstOrDefault();
                    return (val == null ? a : val.Value.GetSqlValue()) + b;
                });
                Non = this.DataHelper.ExecuteNonQuery(SQLString);
            }
            else
            {
                Non = this.DataHelper.ExecuteNonQuery(SQLString, CommandType.Text, Params);
            }
            //int Non = this.DataHelper.ExecuteNonQuery(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters());
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return Non > 0;
        }
        #endregion

        #endregion

        #region 更新数据
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> UpdateQ<TOther>(Expression<Func<T, TOther>> fResult, Expression<Func<T, bool>> func = null)
        {
            this.DataSQL.SQLType = SQLType.update;
            if (fResult.Body is MemberInitExpression miex)
            {
                miex.Bindings.Each<MemberAssignment>(b =>
                {
                    object value = null;
                    var ParamName = FieldFormat("@" + b.Member.Name);
                    if (b.Expression is ConstantExpression cex)
                    {
                        value = cex.Value.GetSqlValue();
                    }
                    else if (b.Expression is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression cexn)
                        {
                            value = cexn.Value;
                        }
                        else
                            value = this.ExpressionRouter(uex.Operand).Trim('\'');
                        value = value.GetSqlValue();
                    }
                    else
                    {
                        value = this.ExpressionRouter(b.Expression);
                    }
                    this.DataSQL.SetUpdateColumns(
                        ("{0} = {1}").format(FieldFormat(b.Member.Name), value));
                });
            }
            else
            {
                var updateColumns = this.ExpressionRouter(fResult.Body);
                //updateColumns = updateColumns.RemovePattern(@"(^\(|\)$)");
                var Columns = updateColumns.SplitPattern(@"\)\s+AND\s+\(");
                /*var leftChar = "";
                if (this.Config.ProviderType == DbProviderType.MySql || this.Config.ProviderType == DbProviderType.Oracle)
                    leftChar = "`";
                else if(this.Config.ProviderType== DbProviderType.Dameng)
                    leftChar = "";
                else leftChar = "[";
                var Columns = this.ExpressionRouter(fResult.Body).ReplacePattern(@"\s+(AND|OR)\s+", ",").ReplacePattern(@"\s+is\s+null\s+", " = null").SplitPattern(@"\),\(" + leftChar.ToRegexEscape());
                var len = Columns.Length - 1;
                for (var i = 0; i < Columns.Length; i++)
                {
                    if (i == 0)
                    {
                        Columns[i] = Columns[i].RemovePattern(@"^\({" + (len < 0 ? 0 : len) + @"}") + (i == len ? "" : ")");
                    }
                    else if (i == len)
                    {
                        Columns[i] = "(" + leftChar + Columns[i].RemovePattern(@"\)$");
                    }
                    else
                    {
                        Columns[i] = "(" + leftChar + Columns[i];
                    }
                    Columns[i] = Columns[i].ReplacePattern(@"[^']\s*1\s*=\s*1\s*[^']", "1").ReplacePattern(@"[^']\s*1\s*=\s*0\s*[^']", "0");
                    while (Columns[i].IsMatch(@"^\(.*\)$"))
                        Columns[i] = Columns[i].ReplacePattern(@"^\((.*)\)$", "$1");
                }*/
                this.DataSQL.SetUpdateColumns(Columns);
            }
            if (func != null) this.Where(func);
            return this;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public Boolean Update<TOther>(Expression<Func<T, TOther>> fResult, Expression<Func<T, bool>> func = null)
        {
            this.UpdateQ(fResult, func);
            if (this.DataSQL.UpdateColumns == null || this.DataSQL.UpdateColumns.Count == 0 || this.DataSQL.GetWhere().IsNullOrEmpty()) return false;
            string SQLString = this.DataSQL.GetSQLString();
            var sTime = new Stopwatch();

            sTime.Start();
            int Non = this.DataHelper.ExecuteNonQuery(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString));
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return Non > 0;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">model</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> UpdateQ<TOther>(TOther model, Expression<Func<T, bool>> func = null)
        {
            if (model == null) return this;
            this.DataSQL.SQLType = SQLType.update;
            string Where = "";
            Type t = model.GetType();
            if (t.GetBaseTypeNames().Contains(typeof(Model.Entity<>).Name))
            {
                var GetDirty = t.GetMethod("GetDirty");
                if (GetDirty != null)
                {
                    if (GetDirty.Invoke(model, null) is Model.DirtyCollection Dirtys && Dirtys.Count > 0)
                    {
                        //f = false;
                        Dirtys.ToArray().Each(k =>
                        {
                            var p = t.GetProperty(k, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (p == null) return;
                            if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                            object value = p.GetValue(model, null);
                            //if (value != null)
                            {
                                Type _t = p.PropertyType;
                                if (!_t.Name.IsMatch(@"^List`") && _t.ToString().IsMatch(@"^System\."))
                                {
                                    ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();
                                    var ParamName = "@" + p.Name;
                                    if ((column != null && (column.PrimaryKey || column.AutoIncrement)) || p.Name.EqualsIgnoreCase("ID"))
                                        Where = "{0} = {1}".format(FieldFormat(column.Name), value == null ? "null" : ParamName/*(DbProviderType.Dameng | DbProviderType.MySql).HasFlag(this.DataHelper.ProviderType) ? "?" : ParamName*/);
                                    else
                                        this.DataSQL.SetUpdateColumns("{0} = {1}".format(FieldFormat(p.Name), value == null ? "null" : ParamName));
                                    this.AddParam(ParamName, value);
                                }
                            }
                        });
                    }
                }
            }
            /*
             * 模型必有脏数据，所以不用下边
             * if (f)
            {
                var ValueType = t.GetValueType();
                t.GetProperties().Each(p =>
                 {
                     if (ValueType != ValueTypes.Anonymous && (!p.CanRead || !p.CanWrite || p.GetMethod.IsVirtual || p.IsIndexer())) return;
                     if (!p.CanRead || !p.CanWrite || p.GetMethod.IsVirtual || p.IsIndexer()) return;
                     object value = p.GetValue(model, null);
                     if (value == null && p.PropertyType.GetBaseType() == typeof(string)) value = "";
                     if (value != null)
                     {
                         Type _t = p.PropertyType;
                         if (_t.IsValueType())
                         {
                             ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();
                             var ParamName = "@" + p.Name;
                             var columns = "";
                             if (column == null)
                             {
                                 columns = "{0} = {1}".format(FieldFormat(p.Name), ParamName);
                                 this.DataSQL.SetUpdateColumns(columns);
                             }
                             else
                             {
                                 columns = "{0} = {1}".format(FieldFormat(column.Name.IsNullOrEmpty() ? p.Name : column.Name), value == null ? "null" : ParamName/*(DbProviderType.Dameng | DbProviderType.MySql).HasFlag(this.DataHelper.ProviderType) ? "?" : ParamName*);
                                 if (column.PrimaryKey || column.AutoIncrement)
                                     Where += " and " + columns;
                                 else
                                     this.DataSQL.SetUpdateColumns(columns);
                             }
                             this.AddParam(ParamName, value);
                         }
                     }
                 });
            }*/
            if (this.DataSQL.GetUpdateColumns().IsNullOrEmpty()) return this;
            if (func != null) this.Where(func);
            if (this.DataSQL.GetWhere().IsNullOrEmpty())
            {
                if (Where.IsNullOrEmpty())
                {
                    t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
                    {
                        if (!p.CanRead || !p.CanWrite || p.IsIndexer() || !p.PropertyType.IsValueType()) return;
                        if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                        object value = p.GetValue(model, null);
                        if (value != null)
                        {
                            Type _t = p.PropertyType;
                            ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();

                            if (column != null && (column.PrimaryKey || column.AutoIncrement))
                            {
                                var ParamName = "@" + column.Name;
                                Where += " and {0} = {1}".format(FieldFormat(column.Name.IsNullOrEmpty() ? p.Name : column.Name), ParamName/*(DbProviderType.Dameng | DbProviderType.MySql).HasFlag(this.DataHelper.ProviderType) ? "?" : ParamName*/);
                                this.AddParam(ParamName, value);
                            }
                        }
                    });
                    if (Where.IsNullOrEmpty()) return this;
                }
                this.DataSQL.SetWhere(Where);
            }
            return this;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">model</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public Boolean Update<TOther>(TOther model, Expression<Func<T, bool>> func = null) where TOther : class
        {
            if (model == null) return false;
            this.UpdateQ(model, func);
            if (this.DataSQL.UpdateColumns == null || this.DataSQL.UpdateColumns.Count == 0 || this.DataSQL.GetWhere().IsNullOrEmpty()) return true;
            string SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            int Non = 0;
            var Params = this.GetDbParameters();
            if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
            {
                //SQLString = SQLString.ReplacePattern(@"@[a-z_0-9]+(\s|,|\)|;|$)", "?$1");
                /*
                 * 用SQL语句参数偶尔会提示格式不正确.
                 */
                SQLString = SQLString.ReplacePattern(@"(?<a>@[a-z_0-9]+)(?<b>(\s|,|\)|;|$))", m =>
                {
                    var a = m.Groups["a"].Value.Trim(new char[] { '@', ':' });
                    var b = m.Groups["b"].Value;
                    var val = Params.Where(c => c.ParameterName.IsMatch(@"^[@:]?" + a + "$")).FirstOrDefault();
                    return (val == null ? a : val.Value.GetSqlValue()) + b;
                });
                Non = this.DataHelper.ExecuteNonQuery(SQLString);
            }
            else
            {
                Non = this.DataHelper.ExecuteNonQuery(SQLString, CommandType.Text, Params);
            }
            //int Non = this.DataHelper.ExecuteNonQuery(SQLString, CommandType.Text, this.GetDbParameters());
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return Non > 0;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">对象</param>
        /// <param name="whereString">条件字符串 如果更新所有则输入'1=1'</param>
        /// <returns></returns>
        public Boolean Update<TOther>(TOther model, string whereString) where TOther : class
        {
            this.DataSQL.SQLType = SQLType.update;
            if (whereString.IsNullOrEmpty()) whereString = "1=1";
            this.DataSQL.SetWhere(whereString);
            return this.Update(model);
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">集合</param>
        /// <returns></returns>
        public Boolean Update<TOther>(IEnumerable<TOther> models) where TOther : class
        {
            this.DataSQL.SQLType = SQLType.update;
            if (models == null || !models.Any()) return false;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            string SqlTemplate = "update ${TableName} set ${Columns} where ${Where};";
            Type t = typeof(TOther);
            this.DataSQL.ModelType = t;
            var i = 0;
            var f = true;
            models.Each(model =>
            {
                i++;
                if (model == null) return;
                string Where = "", Columns = "";
                Type _t = model.GetType();
                if (_t.GetBaseTypeNames().Contains(typeof(Model.Entity<>).Name))
                {
                    var GetDirty = _t.GetMethod("GetDirty");
                    if (GetDirty != null)
                    {
                        var Dirtys = GetDirty.Invoke(model, null) as Model.DirtyCollection;
                        if (Dirtys.Count > 0)
                        {
                            f = false;
                            Dirtys.ToArray().Each(k =>
                            {
                                var p = t.GetProperty(k, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                                if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                                object value = p.GetValue(model, null);
                                //if (value != null)
                                {
                                    Type pType = p.PropertyType;
                                    if (!pType.Name.IsMatch(@"^List`") && pType.ToString().IsMatch(@"^System\."))
                                    {
                                        ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();
                                        if (column != null && (column.PrimaryKey || column.AutoIncrement))
                                        {
                                            Where += " and {0} = {1}".format(FieldFormat(column.Name), value == null ? "null" : value.GetSqlValue());
                                        }
                                        else
                                            Columns += ",{0} = {1}".format(FieldFormat(p.Name), value == null ? "null" : value.GetSqlValue());
                                    }
                                }
                            });
                            Columns = Columns.Trim(',');
                            if (Where.IsNullOrEmpty())
                            {
                                var GetPrimaryKey = _t.GetMethod("GetPrimaryKey");
                                if (GetPrimaryKey != null)
                                {
                                    string PrimaryKey = GetPrimaryKey.Invoke(model, null).ToString();
                                    if (PrimaryKey.IsNotNullOrEmpty())
                                    {
                                        var p = _t.GetProperty(PrimaryKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                        if (p != null && p.CanRead && p.CanWrite)
                                        {
                                            var val = p.GetValue(model, null);
                                            Where = "{0} = {1}".format(FieldFormat(PrimaryKey), val == null ? "null" : val.GetSqlValue());
                                        }
                                    }
                                }
                            }
                            if (Where.IsNotNullOrEmpty())
                            {
                                TableAttribute Table = t.GetTableAttribute();
                                string TableName = Table == null ? t.Name : Table.Name ?? t.Name;
                                this.DataSQL.SQLString += SqlTemplate.format(new Dictionary<string, string>
                        {
                            {"TableName",TableName },
                            {"Columns",Columns },
                            {"Where",Where.RemovePattern(@"^\s+and\s+") }
                        });
                            }
                        }
                    }
                }
                if (f)
                {
                    var ValueType = _t.GetValueType();
                    _t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
                    {
                        if (ValueType != ValueTypes.Anonymous && (!p.CanRead || !p.CanWrite || p.IsIndexer()) && !p.PropertyType.IsValueType()) return;
                        if (p.IsDefined(typeof(FieldIgnoreAttribute))) return;
                        object value = p.GetValue(model, null);
                        if (value != null)
                        {
                            Type pType = p.PropertyType;
                            ColumnAttribute column = p.GetCustomAttribute<ColumnAttribute>();
                            if (column != null && (column.PrimaryKey || column.AutoIncrement))
                            {
                                Where += " and {0} = '{1}'".format(FieldFormat(column.Name), value.GetSqlValue());
                            }
                            else
                                Columns += ",{0} = '{1}'".format(FieldFormat(p.Name), value.GetSqlValue());
                        }
                    });
                    Columns = Columns.Trim(',');
                    if (Where.IsNotNullOrEmpty())
                    {
                        TableAttribute Table = t.GetTableAttribute();
                        string TableName = Table == null ? t.Name : Table.Name ?? t.Name;
                        this.DataSQL.SQLString += SqlTemplate.format(new Dictionary<string, string>
                        {
                            {"TableName",TableName },
                            {"Columns",Columns },
                            {"Where",Where.RemovePattern(@"^\s*and\s+") }
                        });
                    }
                }
            });
            this.DataSQL.SQLParameter = this.DataSQL.GetSQLParameter();
            sTime.Stop();
            this.DataSQL.SpliceSQLTime += sTime.ElapsedMilliseconds;
            sTime.Restart();
            if (this.DataSQL.SQLString.IsNullOrEmpty()) return false;
            var SQLString = this.DataSQL.SQLString;
            int Non = 0;
            var Params = this.GetDbParameters();
            if ((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.DataHelper.ProviderType))
            {
                //int M = this.DataHelper.ExecuteNonQuery(this.DataSQL.SQLString, CommandType.Text, this.GetDbParameters());
                /*
                 * 用SQL语句参数偶尔会提示格式不正确.
                 */
                SQLString = SQLString.ReplacePattern(@"(?<a>@[a-z_0-9]+)(?<b>(\s|,|\)|;|$))", m =>
                {
                    var a = m.Groups["a"].Value.Trim(new char[] { '@', ':' });
                    var b = m.Groups["b"].Value;
                    var val = Params.Where(c => c.ParameterName.IsMatch(@"^[@:]?" + a + "$")).FirstOrDefault();
                    return (val == null ? a : val.Value.GetSqlValue()) + b;
                });
                Non = this.DataHelper.ExecuteNonQuery(SQLString);
            }
            else
            {
                Non = this.DataHelper.ExecuteNonQuery(SQLString, CommandType.Text, Params);
            }
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();

            return Non > 0;
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">集合</param>
        /// <returns></returns>
        public Boolean Update<TOther>(List<TOther> models) where TOther : class
        {
            return this.Update(models as IEnumerable<TOther>);
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">集合</param>
        /// <returns></returns>
        public Boolean Updates<TOther>(IEnumerable<TOther> models) where TOther : class
        {
            return this.Update(models);
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> DeleteQ(Expression<Func<T, bool>> func = null)
        {
            if (func != null)
                this.Where(func);
            this.DataSQL.SQLType = SQLType.delete;
            return this;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="whereString">空则为无效,1=1删除所有,drop删除表,truncate初始化表</param>
        /// <returns></returns>
        public Boolean Delete(string whereString)
        {
            if (whereString.IsNullOrEmpty()) return false;
            if (whereString == "drop")
                this.DataSQL.SQLType = SQLType.drop;
            else if (whereString == "truncate")
                this.DataSQL.SQLType = SQLType.truncate;
            else
            {
                this.DataSQL.SQLType = SQLType.delete;
                this.DataSQL.SetWhere(whereString);
            }
            var SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            int M = this.DataHelper.ExecuteNonQuery(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString));
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return M > 0;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public Boolean Delete(Expression<Func<T, bool>> func = null)
        {
            this.DeleteQ(func);
            string SQLString = this.DataSQL.GetSQLString();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            int M = this.DataHelper.ExecuteNonQuery(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString));
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return M > 0;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <param name="Columns">显示字段</param>
        /// <returns></returns>
        public IQueryableX<T> Select(string Columns) => this.Select(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <param name="columns">字段集合</param>
        /// <returns></returns>
        public IQueryableX<T> Select(IEnumerable<string> columns)
        {
            if (columns.IsNullOrEmpty() || !columns.Any()) return this;
            this.DataSQL.SetColumns(columns);
            return this;
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="func">显示字段Lambda</param>
        /// <returns></returns>
        public IQueryableX<TResult> Select<TResult>(Expression<Func<T, TResult>> func)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            var list = this.SetColumns(func);
            var key = @"" + FieldFormat(func.Parameters[0].Name).ToRegexEscape() + @"[^\.\s]*?\.";
            list.For(0, list.Count, i =>
            {
                list[i] = list[i].RemovePattern(key, System.Text.RegularExpressions.RegexOptions.None);
            });
            this.DataSQL.SetColumns(list);
            var dataX = new DataHelperX<TResult>(this.Config, this.SQLCallBack);
            dataX.DataSQL.SQLType = this.DataSQL.SQLType;
            dataX.DataSQL.Parameters = this.DataSQL.Parameters;
            dataX.DataSQL.ModelType = this.DataSQL.ModelType;
            dataX.DataSQL.TableName = this.DataSQL.GetSQLString().ReplacePattern(@"\s*;\s*", "");
            dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
            return dataX;
        }
        #endregion 

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <param name="func">显示字段Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> SelectX<TResult>(Expression<Func<T, TResult>> func)
        {
            if (func == null) return this;
            this.DataSQL.SQLType = SQLType.select;
            var list = this.SetColumns(func);
            var key = @"" + FieldFormat(func.Parameters[0].Name).ToRegexEscape() + @"[^\.\s]*?\.";
            list.For(0, list.Count, i =>
            {
                list[i] = list[i].RemovePattern(key, System.Text.RegularExpressions.RegexOptions.None);
            });
            this.DataSQL.SetColumns(list);
            return this;
        }
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public IQueryableX<T> SetTable(string tableName)
        {
            if (tableName.IsNotNullOrEmpty()) this.DataSQL.TableName = tableName;
            return this;
        }
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">对象</param>
        /// <returns></returns>
        public IQueryableX<T> SetTable(Dictionary<TableType, string> tableName)
        {
            if (tableName != null && tableName.Count > 0 && tableName.ContainsKey(TableType.T1)) this.DataSQL.TableName = tableName[TableType.T1];
            return this;
        }
        #endregion

        #region 扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T> Where(Expression<Func<T, bool>> func)
        {
            if (func == null) return this;
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool val)
                    this.DataSQL.SetWhere(val ? "1 = 1" : "1 = 0");
            }
            else
            {
                var where = "";
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            where = val ? "1 = 1" : "1 = 0";
                            where += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouter(bex.Right);
                        }
                    }
                    else
                        where = this.ExpressionRouter(func.Body);
                }
                else
                    where = this.ExpressionRouter(func.Body);
                /*where = where.ReplacePattern(@"(?<a>[^=\s])\s*(?<b>@ParamName\d+)", m =>
                {
                    var m1 = m.Groups["a"].Value;
                    var m2 = m.Groups["b"].Value;
                    var val = this.DataSQL.Parameters[m2].ToString().ToLower();
                    if (val == "1") val = "1 = 1"; else val = "1 = 0";
                    return m1 + val;
                });
                where = where.ReplacePattern(@"(?<a>\[[a-z\d_]+\])(?<b>[^=\s])", m =>
                {
                    var a = m.Groups["a"].Value;
                    var b = m.Groups["b"].Value;
                    return "{0} = 1{1}".format(a, b);
                }).ReplacePattern(@"(?<a>\[[a-z\d_]+\])(?<b>\s+(AND|OR))", m =>
                {
                    var a = m.Groups["a"].Value;
                    var b = m.Groups["b"].Value;
                    return "{0} = 1{1}".format(a, b);
                });*/
                this.DataSQL.SetWhere(where);
            }
            return this;
        }
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="whereString">条件字符串</param>
        /// <returns></returns>
        public IQueryableX<T> Where(string whereString)
        {
            this.DataSQL.SetWhere(whereString);
            return this;
        }
        /// <summary>
        /// 根据Model设置条件
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">对象</param>
        /// <returns></returns>
        public IQueryableX<T> Where<TOther>(TOther model) where TOther : class, new()
        {
            if (model == null) return this;
            model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).Each(p =>
            {
                if (p.IsIndexer()) return;
                object value = p.GetValue(model, null);
                this.DataSQL.SetWhere((" (" + FieldFormat("{0}") + " = {1})").format(p.Name, value.GetSqlValue()));
            });
            return this;
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return ((ICloneable)DataSQL).Clone();
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T> Clone()
        {
            return this.MemberwiseClone() as IQueryableX<T>;
        }
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        public IQueryableX<T> Cache(uint? TimeOut = null)
        {
            this.DataSQL.CacheState = CacheState.Yes;
            this.DataSQL.CacheTimeOut = (int?)TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T> NoCache()
        {
            this.DataSQL.CacheState = CacheState.No;
            this.DataSQL.CacheTimeOut = null;
            return this;
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T> ClearCache()
        {
            this.DataSQL.CacheState = CacheState.Clear;
            return this;
        }

        #endregion

        #region 设置SQL语句类型
        /// <summary>
        /// 设置SQL语句类型
        /// </summary>
        /// <param name="sqlType">SQL语句类型</param>
        /// <returns></returns>
        public IQueryableX<T> SetSqlType(SQLType sqlType = SQLType.NULL)
        {
            if (sqlType != SQLType.NULL)
                this.DataSQL.SQLType = sqlType;
            return this;
        }
        #endregion

        #region 获取SQL语句
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <param name="sqlType">SQL语句类型</param>
        /// <returns></returns>
        public string SQL(SQLType sqlType = SQLType.NULL)
        {
            if (sqlType != SQLType.NULL)
                this.DataSQL.SQLType = sqlType;
            return this.DataSQL.GetSQLString();
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion

    #region T1,T2
    /// <summary>
    /// DataSQL 操作类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    public class DataHelperX<T, T2> : QueryableProvider<T, T2>, IQueryableX<T, T2>, IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX()
        {
            this.DataSQL = new DataSQL2
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) }
                }
            };
        }
        /// <summary>
        /// 设置数据库相关配置
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            this.Config = config;
            this.DataHelper = new DataHelper(config);
            this.DataSQL = new DataSQL2
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) }
                },
                Config = this.Config
            };
            if (e != null) this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 属性
        /// <summary>
        /// 相关配置
        /// </summary>
        private ConnectionConfig Config { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public override DataSQL2 DataSQL { get; set; }
        #endregion

        #region 方法

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Take(int topCount)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount == 0) return this;
            this.DataSQL.Top = topCount;
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2> TakeWhile(int topCount)
        {
            return this.Take(topCount);
        }
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult First<TResult>(Expression<Func<T, T2, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult Last<TResult>(Expression<Func<T, T2, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            this.DataSQL.SQLType = SQLType.limit;
            this.DataSQL.Limit = skipCount;
            return this;
        }
        #endregion

        #region 扩展SQL On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> On<TResult>(Expression<Func<T, T2, TResult>> func)
        {
            if (func == null) return this;
            string OnString = "";
            Dictionary<TableType, string> Columns = new Dictionary<TableType, string>();
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            Dictionary<string, TableType> dTable = new Dictionary<string, TableType>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                TableType tableType = (i + 1).ToEnum<TableType>();
                string Name = func.Parameters[i].Name;
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(Name), (65 + i).ToCast<char>() + "");
            }
            if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression cex)
                        {
                            this.DataSQL.JoinType = cex.Value.ToEnum<JoinType>();
                        }
                        else if (uex.Operand is BinaryExpression bex)
                        {
                            OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                        }
                        else if (uex.Operand is MemberExpression mex)
                        {
                            string pName = "";
                            if (mex.Expression is ParameterExpression pex)
                            {
                                pName = pex.Name;
                            }
                            else
                                pName = mex.ToString().GetMatch(@"([a-z]+)\.");
                            if (!dTable.ContainsKey(pName)) return;
                            var tableType = dTable[pName];
                            if (Columns.ContainsKey(tableType))
                            {
                                Columns[tableType] += "," + mex.Member.Name;
                            }
                            else
                                Columns.Add(tableType, mex.Member.Name);
                        }
                        else if (uex.Operand is MethodCallExpression mcex)
                        {
                            if (mcex.Method.Name.ToLower() == "as")
                            {
                                string pName = "", value = "";
                                if (mcex.Arguments[0] is MemberExpression mexs)
                                {
                                    if (mexs.Expression is ParameterExpression pex)
                                    {
                                        pName = pex.Name;
                                    }
                                    value = mexs.Member.Name;
                                }
                                if (mcex.Arguments[1] is ConstantExpression _cex)
                                {
                                    value += " as " + _cex.Value;
                                }
                                var tableType = dTable[pName];
                                if (Columns.ContainsKey(tableType))
                                {
                                    Columns[tableType] += "," + value;
                                }
                                else
                                    Columns.Add(tableType, value);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^\s*AND ", "");
            }
            else if (func.Body is NewExpression)
            {
                //NewExpression nex = func.Body as NewExpression;
            }
            else if (func.Body is BinaryExpression bex)
            {
                OnString += (OnString.IsNullOrEmpty() ? "" : " AND ") + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
            }
            else if (func.Body is UnaryExpression ue)
            {
                OnString += (OnString.IsNullOrEmpty() ? "" : " AND ") + this.ExpressionRouterModel(ue.Operand);
            }
            if (OnString.IsNullOrEmpty() && this.DataSQL.JoinType != JoinType.Union) return this;
            if (Columns != null && Columns.Count > 0)
            {
                Columns.Each(a =>
                {
                    if (!this.DataSQL.TableName.ContainsKey(a.Key))
                    {
                        TableAttribute Table = this.DataSQL.ModelType[a.Key].GetTableAttribute();
                        this.DataSQL.TableName.Add(a.Key,
                            Table == null ?
                            this.DataSQL.ModelType[a.Key].Name :
                            Table.Name ?? this.DataSQL.ModelType[a.Key].Name);
                    }
                    var tableName = this.DataSQL.TableName[a.Key];
                    if (tableName.IsMatch(@"select ")) tableName = "(" + tableName + ") as " + a.Key.ToString() + "A";
                    this.DataSQL.TableName[a.Key] = "select {0} from {1}".format(a.Value, tableName);
                });
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOn(OnString);
            return this;
        }
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> On(Expression<Func<T, T2, bool>> func)
        {
            if (func == null) return this;
            string OnString = "";
            if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression cex)
                        {
                            this.DataSQL.JoinType = cex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression)
            {
                //NewExpression nex = func.Body as NewExpression;
            }
            else if (func.Body is BinaryExpression bex)
            {
                OnString += this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
            }
            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                string Name = func.Parameters[i].Name, /*TypeName = func.Parameters[i].Type.Name,*/
               //Prefix = this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                Prefix = (65 + i).ToCast<char>() + "";
                //dFunc.Add(Name, Prefix);
                dFunc.Add(FieldFormat(Name), Prefix);
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });*/
            this.DataSQL.SetOn(OnString);
            return this;
        }
        #endregion

        #region 扩展SQL COUNT
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <param name="func">第一个表条件</param>
        /// <param name="func2">第二个表条件</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, Boolean>> func, Expression<Func<T2, Boolean>> func2)
        {
            this.Where(func, func2);
            return this.Count();
        }
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            string SQLString = this.DataSQL.GetSQLString(@"
select COUNT(0) as COUNT from {TableNameA} {JoinType} {TableNameB} {WhereString} {WhereString}
");
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            object val = this.DataHelper.ExecuteScalar(SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString));
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return val.ToString().ToInt32();
        }
        #endregion

        #region 扩展SQL Join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Join(Expression<Func<T2, bool>> func, Expression<Func<T, T2, bool>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.On(funcOn);
            return this;
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T, T2, TResult>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.On(funcOn);
            return this;
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Join<T3>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, bool>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            var dataX = new DataHelperX<T, T2, T3>(this.Config, this.SQLCallBack)
            {
                DataSQL = new DataSQL3
                {
                    Columns = this.DataSQL.Columns,
                    Counts = this.DataSQL.Counts,
                    SQLType = this.DataSQL.SQLType,
                    GroupByString = this.DataSQL.GroupByString,
                    //joinType = this.dataSQL.joinType,
                    Limit = this.DataSQL.Limit,
                    Top = this.DataSQL.Top,
                    //OnString = this.dataSQL.OnString,
                    OrderByString = this.DataSQL.OrderByString,
                    TableName = this.DataSQL.TableName,
                    WhereString = this.DataSQL.WhereString,
                    ModelType = new Dictionary<TableType, Type> {
                        {TableType.T1,typeof(T)},
                        {TableType.T2,typeof(T2)},
                        {TableType.T3,typeof(T3)}
                    },
                    Config = this.Config,
                    Parameters = this.DataSQL.Parameters
                }
            };
            dataX.On(funcOn);
            return dataX;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> func)
        {
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            using (var dataX = new DataHelperX<TResult>(this.Config, this.SQLCallBack)
            {
                DataSQL = new DataSQL
                {
                    Limit = this.DataSQL.Limit,
                    Top = this.DataSQL.Top,
                    ModelType = typeof(TResult),
                    SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                    Parameters = this.DataSQL.Parameters,
                    TableName = this.DataSQL.GetSQLTable(),
                    Config = this.Config
                }
            })
            {
                dataX.DataSQL.SpliceSQLTime += this.DataSQL.SpliceSQLTime;
                return dataX;
            }
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<T, T2> SelectX<TResult>(Expression<Func<T, T2, TResult>> func)
        {
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            /*dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });*/
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return this;
        }
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public IQueryableX<T, T2> SetTable(Dictionary<TableType, string> tableName)
        {
            if (tableName != null && tableName.Count > 0) this.DataSQL.TableName = tableName;
            return this;
        }
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2> OrderBy(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.ReplacePattern(@"[\s,]", "").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " asc,") + " asc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> OrderBy<TResult>(Expression<Func<T, T2, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2> OrderByDescending(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.ReplacePattern(@"[\s,]", "").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " desc,") + " desc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> OrderByDescending<TResult>(Expression<Func<T, T2, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        #endregion

        #region 返回实体集合
        /// <summary>
        /// 返回实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(Expression<Func<T, T2, TResult>> func, int page = 0, int pageSize = 0) where TResult : class, new()
        {
            if (func == null) return new List<TResult>();
            if (pageSize > 0)
            {
                page = page <= 0 ? 1 : page;
                this.Skip((page - 1) * pageSize).Take(pageSize);
            }
            /*处理显示列*/
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data;
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<TResult>();
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<TResult>();
            }*/

            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        public TResult ToEntity<TResult>(Expression<Func<T, T2, TResult>> func) where TResult : class, new()
        {
            List<TResult> list = this.ToList(func);
            return (list == null || list.Count == 0) ? null : list[0];
        }
        #endregion

        #region 扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Where(Expression<Func<T, T2, Boolean>> func)
        {
            if (func == null) return this;
            string WhereString = "";
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) WhereString = f ? "1 = 1" : "1 = 0";
            }
            else
            {
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            WhereString = val ? "1 = 1" : "1 = 0";
                            WhereString += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouterModel(bex.Right);
                        }
                    }
                    else
                        WhereString = this.ExpressionRouterModel(func.Body);
                }
                else
                    WhereString = this.ExpressionRouterModel(func.Body);
            }
            /*WhereString = WhereString.ReplacePattern(@"(?<a>[^=\s])\s*(?<b>@ParamName\d+)", m =>
            {
                var m1 = m.Groups["a"].Value;
                var m2 = m.Groups["b"].Value;
                var val = this.DataSQL.Parameters[m2].ToString().ToLower();
                if (val == "1") val = "1 = 1"; else val = "1 = 0";
                return m1 + val;
            });*/
            if (WhereString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            WhereString = ReplaceMatchTag(dFunc, WhereString);
            /*
            dFunc.Each(k =>
            {
                WhereString = WhereString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });*/
            this.DataSQL.SetWhere(WhereString, TableType.TResult);
            return this;
        }
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第一张表条件Lambda</param>
        /// <param name="func2">第二张表条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2)
        {
            if (func != null) this.Where(func, TableType.T1);
            if (func2 != null) this.Where(func2, TableType.T2);
            return this;
        }
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="tableType">表类型</param>
        public void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType)
        {
            if (func == null) return;
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) this.DataSQL.SetWhere(f ? "1=1" : "1=0", tableType);
            }
            else
            {
                var WhereString = "";
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            WhereString = val ? "1 = 1" : "1 = 0";
                            WhereString += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouter(bex.Right);
                        }
                    }
                    else
                        WhereString = this.ExpressionRouter(func.Body);
                }
                else
                    WhereString = this.ExpressionRouter(func.Body);
                this.DataSQL.SetWhere(WhereString, tableType);
            }
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2> AS()
        {
            var dataX = new DataHelperX<T, T2>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2> Clone()
        {
            return this.MemberwiseClone() as IQueryableX<T, T2>;
        }
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        public IQueryableX<T, T2> Cache(uint? TimeOut = null)
        {
            this.DataSQL.CacheState = CacheState.Yes;
            this.DataSQL.CacheTimeOut = (int?)TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2> NoCache()
        {
            this.DataSQL.CacheState = CacheState.No;
            this.DataSQL.CacheTimeOut = null;
            return this;
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3
    /// <summary>
    /// DataSQL 操作类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    /// <typeparam name="T3">T3类型</typeparam>
    public class DataHelperX<T, T2, T3> : QueryableProvider<T, T2, T3>, IQueryableX<T, T2, T3>, IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX()
        {
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) }
                }
            };
        }
        /// <summary>
        /// 设置数据库相关配置
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            if (config == null) return;
            this.Config = config;
            this.DataHelper = new DataHelper(config);
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) }
                },
                Config = this.Config
            };
            if (e != null) this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 属性
        /// <summary>
        /// 相关配置
        /// </summary>
        private ConnectionConfig Config { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public override DataSQL3 DataSQL { get; set; }
        #endregion

        #region 方法

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Take(int topCount)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount == 0) return this;
            this.DataSQL.Top = topCount;
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> TakeWhile(int topCount)
        {
            return this.Take(topCount);
        }
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult First<TResult>(Expression<Func<T, T2, T3, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult Last<TResult>(Expression<Func<T, T2, T3, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            this.DataSQL.SQLType = SQLType.limit;
            this.DataSQL.Limit = skipCount;
            return this;
        }
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> On<TResult>(Expression<Func<T, T2, T3, TResult>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression ubex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(ubex.Left, ubex.Right, ubex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression)
            {
                NewExpression nex = func.Body as NewExpression;
            }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                string Name = func.Parameters[i].Name;
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(Name), (65 + i).ToCast<char>() + "");
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });*/
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> On(Expression<Func<T, T2, T3, bool>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            {

            }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });*/
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        #endregion

        #region 扩展SQL Join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, bool>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.On(funcOn);
            return this;
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, TResult>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.On(funcOn);
            return this;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> func)
        {
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return new DataHelperX<TResult>(this.Config, this.SQLCallBack)
            {
                DataSQL = new DataSQL
                {
                    Limit = this.DataSQL.Limit,
                    Top = this.DataSQL.Top,
                    ModelType = typeof(TResult),
                    SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                    TableName = this.DataSQL.GetSQLTable(),
                    Parameters = this.DataSQL.Parameters,
                    Config = this.Config
                }
            };
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> SelectX<TResult>(Expression<Func<T, T2, T3, TResult>> func)
        {
            if (func == null) return this;
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            /*
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });*/
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return this;
        }
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> SetTable(Dictionary<TableType, string> tableName)
        {
            if (tableName != null && tableName.Count > 0) this.DataSQL.TableName = tableName;
            return this;
        }
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            if (OrderByString == "") return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> OrderBy(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.ReplacePattern(@"[\s,]", "").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " asc,") + " asc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> OrderBy<TResult>(Expression<Func<T, T2, T3, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> OrderByDescending(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.ReplacePattern(@"[\s,]", "").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " desc,") + " desc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> OrderByDescending<TResult>(Expression<Func<T, T2, T3, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        #endregion

        #region 返回实体集合
        /// <summary>
        /// 返回实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, TResult>> func, int page = 0, int pageSize = 0) where TResult : class, new()
        {
            if (func == null) return new List<TResult>();
            if (pageSize > 0)
            {
                page = page <= 0 ? 1 : page;
                this.Skip((page - 1) * pageSize).Take(pageSize);
            }
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression lex)
            {
                lex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = b.Expression.ToString();
                    Columns += ",{0} as {1}".format(value, b.Member.Name);
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = nex.Arguments[i].ToString();
                    Columns += ",{0} as {1}".format(value, nex.Members[i].Name);
                }
            }
            Columns = Columns.Trim(',');
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(func.Parameters[i].Name, (65 + i).ToCast<char>() + "");
            }
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(@"(^|\(|\s+|,)" + k.Key + @"\.", "$1" + k.Value + ".");
            });
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data = new List<TResult>();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<TResult>();
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<TResult>();
            }*/
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        public TResult ToEntity<TResult>(Expression<Func<T, T2, T3, TResult>> func) where TResult : class, new()
        {
            List<TResult> list = this.ToList(func);
            return (list == null || list.Count == 0) ? null : list[0];
        }
        #endregion

        #region 扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Where(Expression<Func<T, T2, T3, Boolean>> func)
        {
            if (func == null) return this;
            string WhereString = "";
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) WhereString = f ? "1=1" : "1=0";
            }
            else
            {
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            WhereString = val ? "1 = 1" : "1 = 0";
                            WhereString += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouterModel(bex.Right);
                        }
                    }
                    else
                        WhereString = this.ExpressionRouterModel(func.Body);
                }
                else
                    WhereString = this.ExpressionRouterModel(func.Body);
            }

            if (WhereString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            WhereString = ReplaceMatchTag(dFunc, WhereString);
            this.DataSQL.SetWhere(WhereString, TableType.TResult);
            return this;
        }
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3)
        {
            if (func != null) this.Where(func, TableType.T1);
            if (func2 != null) this.Where(func2, TableType.T2);
            if (func3 != null) this.Where(func3, TableType.T3);
            return this;
        }
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="tableType">表类型</param>
        public void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType)
        {
            if (func == null) return;
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) this.DataSQL.SetWhere(f ? "1=1" : "1=0", tableType);
            }
            else
                this.DataSQL.SetWhere(this.ExpressionRouter(func.Body), tableType);
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> AS()
        {
            var dataX = new DataHelperX<T, T2, T3>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Clone()
        {
            return this.MemberwiseClone() as IQueryableX<T, T2, T3>;
        }
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> Cache(uint? TimeOut = null)
        {
            this.DataSQL.CacheState = CacheState.Yes;
            this.DataSQL.CacheTimeOut = (int?)TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3> NoCache()
        {
            this.DataSQL.CacheState = CacheState.No;
            this.DataSQL.CacheTimeOut = null;
            return this;
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3,T4
    /// <summary>
    /// DataSQL 操作类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    /// <typeparam name="T3">T3类型</typeparam>
    /// <typeparam name="T4">T4类型</typeparam>
    public class DataHelperX<T, T2, T3, T4> : QueryableProvider<T, T2, T3, T4>, IQueryableX<T, T2, T3, T4>, IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX()
        {
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) },
                    { TableType.T4, typeof(T4) } }
            };
        }
        /// <summary>
        /// 设置数据库相关配置
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            if (config == null) return;
            this.Config = config;
            this.DataHelper = new DataHelper(config);
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) },
                    { TableType.T4, typeof(T4) } },
                Config = this.Config
            };
            if (e != null) this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 属性
        /// <summary>
        /// 相关配置
        /// </summary>
        private ConnectionConfig Config { get; set; }
        #endregion

        #region 方法

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Take(int topCount)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount == 0) return this;
            this.DataSQL.Top = topCount;
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> TakeWhile(int topCount)
        {
            return this.Take(topCount);
        }
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult First<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult Last<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            this.DataSQL.SQLType = SQLType.limit;
            this.DataSQL.Limit = skipCount;
            return this;
        }
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> On<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            {

            }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> On(Expression<Func<T, T2, T3, T4, bool>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            {

            }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        #endregion

        #region 扩展SQL Join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, bool>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.Where(func4, TableType.T4);
            this.On(funcOn);
            return this;
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, TResult>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.Where(func4, TableType.T4);
            this.On(funcOn);
            return this;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func)
        {
            if (func == null)
            {
                var _dataX = new DataHelperX<TResult>(this.Config, this.SQLCallBack)
                {
                    DataSQL = new DataSQL
                    {
                        Limit = this.DataSQL.Limit,
                        Top = this.DataSQL.Top,
                        ModelType = typeof(TResult),
                        SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                        TableName = this.DataSQL.GetSQLTable(),
                        Parameters = this.DataSQL.Parameters,
                        Config = this.Config
                    }
                };
                return _dataX;
            }
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return new DataHelperX<TResult>(this.Config, this.SQLCallBack)
            {
                DataSQL = new DataSQL
                {
                    Limit = this.DataSQL.Limit,
                    Top = this.DataSQL.Top,
                    ModelType = typeof(TResult),
                    SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                    TableName = this.DataSQL.GetSQLTable(),
                    Parameters = this.DataSQL.Parameters,
                    Config = this.Config
                }
            };
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> SelectX<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func)
        {
            if (func == null) return this;
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            /*
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return this;
        }
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> SetTable(Dictionary<TableType, string> tableName)
        {
            if (tableName != null && tableName.Count > 0) this.DataSQL.TableName = tableName;
            return this;
        }
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> OrderBy(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@"[\s,]").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " asc,") + " asc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> OrderByDescending(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@"[\s,]").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " desc,") + " desc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> OrderByDescending<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            if (func == null) return this;
            string OrderByString = this.OrderByString(func, "desc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        #endregion

        #region 返回实体集合
        /// <summary>
        /// 返回实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func, int page = 0, int pageSize = 0) where TResult : class, new()
        {
            if (func == null) return new List<TResult>();
            if (pageSize > 0)
            {
                page = page <= 0 ? 1 : page;
                this.Skip((page - 1) * pageSize).Take(pageSize);
            }
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression lex)
            {
                lex.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = b.Expression.ToString();
                    Columns += ",{0} as {1}".format(value, b.Member.Name);
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = nex.Arguments[i].ToString();
                    Columns += ",{0} as {1}".format(value, nex.Members[i].Name);
                }
            }
            Columns = Columns.Trim(',');
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(func.Parameters[i].Name, (65 + i).ToCast<char>() + "");
            }
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(@"(^|\(|\s+|,)" + k.Key + @"\.", "$1" + k.Value + ".");
            });
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data = new List<TResult>();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<TResult>();
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<TResult>();
            }*/
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        public TResult ToEntity<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func) where TResult : class, new()
        {
            List<TResult> list = this.ToList(func);
            return (list == null || list.Count == 0) ? null : list[0];
        }
        #endregion

        #region 扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, Boolean>> func)
        {
            if (func == null) return this;
            string WhereString = "";
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) WhereString = f ? "1=1" : "1=0";
            }
            else
            {
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            WhereString = val ? "1 = 1" : "1 = 0";
                            WhereString += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouterModel(bex.Right);
                        }
                    }
                    else
                        WhereString = this.ExpressionRouterModel(func.Body);
                }
                else
                    WhereString = this.ExpressionRouterModel(func.Body);
            }
            if (WhereString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            WhereString = ReplaceMatchTag(dFunc, WhereString);
            this.DataSQL.SetWhere(WhereString, TableType.TResult);
            return this;
        }
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <param name="func4">第4张表条件lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4)
        {
            if (func != null) this.Where(func, TableType.T1);
            if (func2 != null) this.Where(func2, TableType.T2);
            if (func3 != null) this.Where(func3, TableType.T3);
            if (func4 != null) this.Where(func4, TableType.T4);
            return this;
        }
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="tableType">表类型</param>
        public void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType)
        {
            if (func == null) return;
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) this.DataSQL.SetWhere(f ? "1=1" : "1=0", tableType);
            }
            else
                this.DataSQL.SetWhere(this.ExpressionRouter(func.Body), tableType);
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> AS()
        {
            var dataX = new DataHelperX<T, T2, T3, T4>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Clone()
        {
            return this.MemberwiseClone() as IQueryableX<T, T2, T3, T4>;
        }
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> Cache(uint? TimeOut = null)
        {
            this.DataSQL.CacheState = CacheState.Yes;
            this.DataSQL.CacheTimeOut = (int?)TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4> NoCache()
        {
            this.DataSQL.CacheState = CacheState.No;
            this.DataSQL.CacheTimeOut = null;
            return this;
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3,T4,T5
    /// <summary>
    /// DataSQL 操作类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    /// <typeparam name="T3">T3类型</typeparam>
    /// <typeparam name="T4">T4类型</typeparam>
    /// <typeparam name="T5">T5类型</typeparam>
    public class DataHelperX<T, T2, T3, T4, T5> : QueryableProvider<T, T2, T3, T4, T5>, IQueryableX<T, T2, T3, T4, T5>, IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX()
        {
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) },
                    { TableType.T4, typeof(T4) },
                    { TableType.T5, typeof(T5) }
                }
            };
        }
        /// <summary>
        /// 设置数据库相关配置
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            if (config == null) return;
            this.Config = config;
            this.DataHelper = new DataHelper(config);
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) },
                    { TableType.T4, typeof(T4) },
                    { TableType.T5, typeof(T5) }
                },
                Config = this.Config
            };
            if (e != null) this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 属性
        /// <summary>
        /// 相关配置
        /// </summary>
        private ConnectionConfig Config { get; set; }
        #endregion

        #region 方法

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Take(int topCount)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount == 0) return this;
            this.DataSQL.Top = topCount;
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> TakeWhile(int topCount)
        {
            return this.Take(topCount);
        }
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult First<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult Last<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            this.DataSQL.SQLType = SQLType.limit;
            this.DataSQL.Limit = skipCount;
            return this;
        }
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> On<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            { }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                string Name = func.Parameters[i].Name;
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(Name), (65 + i).ToCast<char>() + "");
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> On(Expression<Func<T, T2, T3, T4, T5, bool>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            {

            }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                string Name = func.Parameters[i].Name, /*TypeName = func.Parameters[i].Type.Name,*/ 
                Prefix = "";//this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                Prefix = (65 + i).ToCast<char>() + "";
                dFunc.Add(FieldFormat(Name), Prefix);
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        #endregion

        #region 扩展SQL Join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, bool>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.Where(func4, TableType.T4);
            this.Where(func5, TableType.T5);
            this.On(funcOn);
            return this;
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, TResult>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.Where(func4, TableType.T4);
            this.Where(func5, TableType.T5);
            this.On(funcOn);
            return this;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func)
        {
            if (func == null)
            {
                return new DataHelperX<TResult>(this.Config, this.SQLCallBack)
                {
                    DataSQL = new DataSQL
                    {
                        Limit = this.DataSQL.Limit,
                        Top = this.DataSQL.Top,
                        ModelType = typeof(TResult),
                        SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                        TableName = this.DataSQL.GetSQLTable(),
                        Parameters = this.DataSQL.Parameters,
                        Config = this.Config
                    }
                };
            }
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return new DataHelperX<TResult>(this.Config, this.SQLCallBack)
            {
                DataSQL = new DataSQL
                {
                    Limit = this.DataSQL.Limit,
                    Top = this.DataSQL.Top,
                    ModelType = typeof(TResult),
                    SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                    TableName = this.DataSQL.GetSQLTable(),
                    Parameters = this.DataSQL.Parameters,
                    Config = this.Config
                }
            };
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> SelectX<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func)
        {
            if (func == null) return this;
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            /*
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return this;
        }
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> SetTable(Dictionary<TableType, string> tableName)
        {
            if (tableName != null && tableName.Count > 0) this.DataSQL.TableName = tableName;
            return this;
        }
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> OrderBy(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@"[\s,]").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " asc,") + " asc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> OrderByDescending(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@"[\s,]").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " desc,") + " desc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> OrderByDescending<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        #endregion

        #region 返回实体集合
        /// <summary>
        /// 返回实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func, int page = 0, int pageSize = 0) where TResult : class, new()
        {
            if (func == null) return new List<TResult>();
            if (pageSize > 0)
            {
                page = page <= 0 ? 1 : page;
                this.Skip((page - 1) * pageSize).Take(pageSize);
            }
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression mie)
            {
                mie.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = b.Expression.ToString();
                    Columns += ",{0} as {1}".format(value, b.Member.Name);
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = nex.Arguments[i].ToString();
                    Columns += ",{0} as {1}".format(value, nex.Members[i].Name);
                }
            }
            Columns = Columns.Trim(',');
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(func.Parameters[i].Name, (65 + i).ToCast<char>() + "");
            }
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(@"(^|\(|\s+|,)" + k.Key + @"\.", "$1" + k.Value + ".");
            });
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data = new List<TResult>();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<TResult>();
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
             if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<TResult>();
            }*/
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        public TResult ToEntity<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func) where TResult : class, new()
        {
            List<TResult> list = this.ToList(func);
            return (list == null || list.Count == 0) ? null : list[0];
        }
        #endregion

        #region 扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> func)
        {
            if (func == null) return this;
            string WhereString = "";
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) WhereString = f ? "1=1" : "1=0";
            }
            else
            {
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            WhereString = val ? "1 = 1" : "1 = 0";
                            WhereString += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouterModel(bex.Right);
                        }
                    }
                    else
                        WhereString = this.ExpressionRouterModel(func.Body);
                }
                else
                    WhereString = this.ExpressionRouterModel(func.Body);
            }
            if (WhereString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            WhereString = ReplaceMatchTag(dFunc, WhereString);

            this.DataSQL.SetWhere(WhereString, TableType.TResult);
            return this;
        }
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <param name="func4">第4张表条件lambda</param>
        /// <param name="func5">第5张表条件lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5)
        {
            if (func != null) this.Where(func, TableType.T1);
            if (func2 != null) this.Where(func2, TableType.T2);
            if (func3 != null) this.Where(func3, TableType.T3);
            if (func4 != null) this.Where(func4, TableType.T4);
            if (func5 != null) this.Where(func5, TableType.T5);
            return this;
        }
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="tableType">表类型</param>
        public void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType)
        {
            if (func == null) return;
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) this.DataSQL.SetWhere(f ? "1=1" : "1=0", tableType);
            }
            else
                this.DataSQL.SetWhere(this.ExpressionRouter(func.Body), tableType);
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> AS()
        {
            var dataX = new DataHelperX<T, T2, T3, T4, T5>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Clone()
        {
            return this.MemberwiseClone() as IQueryableX<T, T2, T3, T4, T5>;
        }
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> Cache(uint? TimeOut = null)
        {
            this.DataSQL.CacheState = CacheState.Yes;
            this.DataSQL.CacheTimeOut = (int?)TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5> NoCache()
        {
            this.DataSQL.CacheState = CacheState.No;
            this.DataSQL.CacheTimeOut = null;
            return this;
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion

    #region T1,T2,T3,T4,T5,T6
    /// <summary>
    /// DataSQL 操作类
    /// </summary>
    /// <typeparam name="T">T1类型</typeparam>
    /// <typeparam name="T2">T2类型</typeparam>
    /// <typeparam name="T3">T3类型</typeparam>
    /// <typeparam name="T4">T4类型</typeparam>
    /// <typeparam name="T5">T5类型</typeparam>
    /// <typeparam name="T6">T6类型</typeparam>
    public class DataHelperX<T, T2, T3, T4, T5, T6> : QueryableProvider<T, T2, T3, T4, T5, T6>, IQueryableX<T, T2, T3, T4, T5, T6>, IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperX()
        {
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) },
                    { TableType.T4, typeof(T4) },
                    { TableType.T5, typeof(T5) },
                    { TableType.T6, typeof(T6) }
                }
            };
        }
        /// <summary>
        /// 设置数据库相关配置
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="e">事件</param>
        public DataHelperX(ConnectionConfig config, RunSQLEventHandler e = null)
        {
            if (config == null) return;
            this.Config = config;
            this.DataHelper = new DataHelper(config);
            this.DataSQL = new DataSQL3
            {
                ModelType = new Dictionary<TableType, Type>() {
                    { TableType.T1, typeof(T) },
                    { TableType.T2, typeof(T2) },
                    { TableType.T3, typeof(T3) },
                    { TableType.T4, typeof(T4) },
                    { TableType.T5, typeof(T5) },
                    { TableType.T6, typeof(T6) }
                },
                Config = this.Config
            };
            if (e != null) this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 属性
        /// <summary>
        /// 相关配置
        /// </summary>
        private ConnectionConfig Config { get; set; }
        #endregion

        #region 方法

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Take(int topCount)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (topCount == 0) return this;
            this.DataSQL.Top = topCount;
            return this;
        }
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> TakeWhile(int topCount)
        {
            return this.Take(topCount);
        }
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult First<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = 1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public TResult Last<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func) where TResult : class, new()
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            this.DataSQL.Top = -1;
            return this.ToEntity(func);
        }
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Skip(int skipCount)
        {
            if (skipCount == 0) return this;
            this.DataSQL.SQLType = SQLType.limit;
            this.DataSQL.Limit = skipCount;
            return this;
        }
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> On<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            { }
            else
                OnString = this.ExpressionRouterModel(func.Body);
            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> On(Expression<Func<T, T2, T3, T4, T5, T6, bool>> func)
        {
            if (func == null) return this;
            string OnString = "";
            JoinType jType = JoinType.Left;
            if (func.Body is BinaryExpression be)
            {
                OnString = this.BinaryExpressionProviderModel(be.Left, be.Right, be.NodeType);
            }
            else if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) OnString = f ? "1=1" : "1=0";
            }
            else if (func.Body is NewArrayExpression aex)
            {
                aex.Expressions.Each(ex =>
                {
                    if (ex is UnaryExpression uex)
                    {
                        if (uex.Operand is ConstantExpression ucex)
                        {
                            jType = ucex.Value.ToEnum<JoinType>();
                        }
                        else
                        {
                            if (uex.Operand is BinaryExpression bex)
                            {
                                OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                            }
                        }
                    }
                    else if (ex is BinaryExpression bex)
                    {
                        OnString += " AND " + this.BinaryExpressionProviderModel(bex.Left, bex.Right, bex.NodeType);
                    }
                });
                OnString = OnString.ReplacePattern(@"^ AND ", "");
            }
            else if (func.Body is NewExpression nex)
            {

            }
            else
                OnString = this.ExpressionRouterModel(func.Body);

            if (OnString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            string _Bodys = func.Body.ToString();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                string Name = func.Parameters[i].Name, /*TypeName = func.Parameters[i].Type.Name,*/
                Prefix = "";//this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                Prefix = (65 + i).ToCast<char>() + "";
                dFunc.Add(FieldFormat(Name), Prefix);
            }
            OnString = ReplaceMatchTag(dFunc, OnString);
            /*
            dFunc.Each(k =>
            {
                OnString = OnString.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetOnAndJoinType(OnString, jType);
            return this;
        }
        #endregion

        #region 扩展SQL join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="func6">T6条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, bool>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.Where(func4, TableType.T4);
            this.Where(func5, TableType.T5);
            this.Where(func6, TableType.T6);
            this.On(funcOn);
            return this;
        }
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="func6">T6条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> funcOn)
        {
            this.Where(func, TableType.T2);
            this.Where(func3, TableType.T3);
            this.Where(func4, TableType.T4);
            this.Where(func5, TableType.T5);
            this.Where(func6, TableType.T6);
            this.On(funcOn);
            return this;
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func)
        {
            if (func == null)
            {
                return new DataHelperX<TResult>(this.Config, this.SQLCallBack)
                {
                    DataSQL = new DataSQL
                    {
                        Limit = this.DataSQL.Limit,
                        Top = this.DataSQL.Top,
                        ModelType = typeof(TResult),
                        SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                        TableName = this.DataSQL.GetSQLTable(),
                        Parameters = this.DataSQL.Parameters,
                        Config = this.Config
                    }
                };
            }
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return new DataHelperX<TResult>(this.Config, this.SQLCallBack)
            {
                DataSQL = new DataSQL
                {
                    Limit = this.DataSQL.Limit,
                    Top = this.DataSQL.Top,
                    ModelType = typeof(TResult),
                    SQLType = this.DataSQL.Limit > 0 ? SQLType.limit : SQLType.select,
                    TableName = this.DataSQL.GetSQLTable(),
                    Parameters = this.DataSQL.Parameters,
                    Config = this.Config
                }
            };
        }
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> SelectX<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func)
        {
            if (func == null) return this;
            string Columns = this.SetColumns(func);
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            Columns = ReplaceMatchTag(dFunc, Columns);
            /*
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(RegexString.MatchInputTag + k.Key.ToRegexEscape() + @"[^\.]*?\.", "$1" + k.Value + ".");
            });
            */
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            return this;
        }
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> SetTable(Dictionary<TableType, string> tableName)
        {
            if (tableName != null && tableName.Count > 0) this.DataSQL.TableName = tableName;
            return this;
        }
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> OrderBy(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@"[\s,]").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " asc,") + " asc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "asc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> OrderByDescending(string orderString)
        {
            this.DataSQL.SQLType = this.DataSQL.SQLType == SQLType.NULL ? SQLType.select : this.DataSQL.SQLType;
            if (orderString.IsNullOrEmpty() || orderString.RemovePattern(@"[\s,]").IsNullOrEmpty()) return this;
            orderString = orderString.ReplacePattern(@"\s+order\s+by\s+", "").ReplacePattern(@"\s+(asc|desc)(\s*,|\s*$)", "$2").ReplacePattern(@"\s+", "").ReplacePattern("[,]{2,}", ",").Trim(',').ReplacePattern(",", " desc,") + " desc";
            this.DataSQL.SetOrderBy(orderString);
            return this;
        }
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> OrderByDescending<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            OrderByString = ReplaceMatchTag(dFunc, OrderByString);
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            string OrderByString = this.OrderByString(func, "desc");
            if (OrderByString.IsNullOrEmpty()) return this;
            this.DataSQL.SetOrderBy(OrderByString);
            return this;
        }
        #endregion

        #region 返回实体集合
        /// <summary>
        /// 返回实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <param name="page">当前页</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func, int page = 0, int pageSize = 0) where TResult : class, new()
        {
            if (func == null) return new List<TResult>();
            if (pageSize > 0)
            {
                page = page <= 0 ? 1 : page;
                this.Skip((page - 1) * pageSize).Take(pageSize);
            }
            /*处理显示列*/
            string Columns = "";
            if (func.Body is MemberInitExpression mie)
            {
                mie.Bindings.Each<MemberAssignment>(b =>
                {
                    string value = b.Expression.ToString();
                    Columns += ",{0} as {1}".format(value, b.Member.Name);
                });
            }
            else if (func.Body is NewExpression nex)
            {
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    string value = nex.Arguments[i].ToString();
                    Columns += ",{0} as {1}".format(value, nex.Members[i].Name);
                }
            }
            Columns = Columns.Trim(',');
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(func.Parameters[i].Name, (65 + i).ToCast<char>() + "");
            }
            dFunc.Each(k =>
            {
                Columns = Columns.ReplacePattern(@"(^|\(|\s+|,)" + k.Key + @"\.", "$1" + k.Value + ".");
            });
            this.DataSQL.SetColumns(Columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            string SQLString = this.DataSQL.GetSQLString();
            CacheDataAttribute cacheData = typeof(TResult).GetCustomAttribute<CacheDataAttribute>();
            List<TResult> data = new List<TResult>();
            Stopwatch sTime = new Stopwatch();
            sTime.Start();
            var _SQLString = SQLString.Trim(';');
            if (this.DataSQL.CacheState == CacheState.Yes)
            {
                _SQLString += ";Cache";
                if (this.DataSQL.CacheTimeOut.HasValue)
                    _SQLString += "[" + this.DataSQL.CacheTimeOut.Value + "]";
                _SQLString += ";";
            }
            else if (this.DataSQL.CacheState == CacheState.No)
            {
                _SQLString += ";NoCache;";
            }
            else if (this.DataSQL.CacheState == CacheState.Clear)
            {
                _SQLString += ";ClearCache;";
            }
            else
            {
                if (cacheData != null)
                {
                    if (cacheData.CacheType == CacheType.No)
                        _SQLString += ";NoCache;";
                    else if (cacheData.CacheType != CacheType.Default)
                        _SQLString += ";Cache[" + cacheData.TimeOut + "];";
                }
            }
            data = this.DataHelper.ExecuteDataTable(_SQLString.SQLFormat(this.DataHelper.ProviderType), CommandType.Text, this.GetDbParameters(SQLString)).ToList<TResult>();
            /*
             * 移除QueryableX中的缓存 统一用基类DataHelper中的缓存
            if (this.DataSQL.IsCache(cacheData))
            {
                object val = this.DataSQL.GetCacheData();
                if (val == null)
                {
                    DataTable _data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters());
                    data = _data.ToList<TResult>();
                    this.DataSQL.SetCacheData(data, this.DataSQL.CacheTimeOut ?? this.Config.CacheTimeOut);
                }
                else
                {
                    if (this.Config.CacheType == 0)
                        data = val as List<TResult>;
                    else
                    {
                        string content = val.ToString().RemovePattern(@"^Cache:");
                        data = content.JsonToObject<List<TResult>>();
                    }
                }
            }
            else
            {
                data = this.DataHelper.ExecuteDataTable(SQLString, CommandType.Text, this.GetDbParameters()).ToList<TResult>();
            }*/
            sTime.Stop();
            this.DataSQL.RunSQLTime += sTime.ElapsedMilliseconds;
            if (this.SQLCallBack != null) this.SQLCallBack.Invoke(this.DataSQL);
            this.DataSQL.Clear();
            return data;
        }
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        public TResult ToEntity<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func) where TResult : class, new()
        {
            List<TResult> list = this.ToList(func);
            return (list == null || list.Count == 0) ? null : list[0];
        }
        #endregion

        #region 扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, Boolean>> func)
        {
            if (func == null) return this;
            string WhereString = "";
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) WhereString = f ? "1=1" : "1=0";
            }
            else
            {
                if (func.Body is BinaryExpression bex)
                {
                    if (bex.Left is ConstantExpression lcex)
                    {
                        if (lcex.Value is bool val)
                        {
                            WhereString = val ? "1 = 1" : "1 = 0";
                            WhereString += " " + ExpressionTypeCast(bex.NodeType) + " " + this.ExpressionRouterModel(bex.Right);
                        }
                    }
                    else
                        WhereString = this.ExpressionRouterModel(func.Body);
                }
                else
                    WhereString = this.ExpressionRouterModel(func.Body);
            }

            WhereString = WhereString.ReplacePattern(@"(?<a>[^=\s])\s*(?<b>@ParamName\d+)", m =>
            {
                var m1 = m.Groups["a"].Value;
                var m2 = m.Groups["b"].Value;
                var val = this.DataSQL.Parameters[m2].ToString().ToLower();
                if (val == "1") val = "1 = 1"; else val = "1 = 0";
                return m1 + val;
            });

            if (WhereString.IsNullOrEmpty()) return this;
            /*匹配两个输入值*/
            Dictionary<string, string> dFunc = new Dictionary<string, string>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                //this.DataSQL.Prefix[(i + 1).ToEnum<TableType>()];
                dFunc.Add(FieldFormat(func.Parameters[i].Name), (65 + i).ToCast<char>() + "");
            }
            WhereString = ReplaceMatchTag(dFunc, WhereString);

            this.DataSQL.SetWhere(WhereString, TableType.TResult);
            return this;
        }
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <param name="func4">第4张表条件lambda</param>
        /// <param name="func5">第5张表条件lambda</param>
        /// <param name="func6">第6张表条件lambda</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6)
        {
            if (func != null) this.Where(func, TableType.T1);
            if (func2 != null) this.Where(func2, TableType.T2);
            if (func3 != null) this.Where(func3, TableType.T3);
            if (func4 != null) this.Where(func4, TableType.T4);
            if (func5 != null) this.Where(func5, TableType.T5);
            if (func6 != null) this.Where(func6, TableType.T6);
            return this;
        }
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="tableType">表类型</param>
        public void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType)
        {
            if (func == null) return;
            if (func.Body is ConstantExpression cex)
            {
                if (cex.Value is bool f) this.DataSQL.SetWhere(f ? "1=1" : "1=0", tableType);
            }
            else
                this.DataSQL.SetWhere(this.ExpressionRouter(func.Body), tableType);
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> AS()
        {
            var dataX = new DataHelperX<T, T2, T3, T4, T5, T6>
            {
                Config = this.Config,
                DataHelper = this.DataHelper
            };
            dataX.DataSQL.Done(this.DataSQL);
            dataX.SQLCallBack += this.SQLCallBack;
            return dataX;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Clone()
        {
            return this.MemberwiseClone() as IQueryableX<T, T2, T3, T4, T5, T6>;
        }
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> Cache(uint? TimeOut = null)
        {
            this.DataSQL.CacheState = CacheState.Yes;
            this.DataSQL.CacheTimeOut = (int?)TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public IQueryableX<T, T2, T3, T4, T5, T6> NoCache()
        {
            this.DataSQL.CacheState = CacheState.No;
            this.DataSQL.CacheTimeOut = null;
            return this;
        }
        #endregion

        #endregion

        #region 释放资源
        /// <summary>
        /// 要检测冗余调用
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">要检测冗余调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataHelperX()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }
        /// <summary>
        /// 添加此代码以正确实现可处置模式。
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion
}