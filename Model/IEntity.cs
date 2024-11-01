﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XiaoFeng.Data;
using XiaoFeng.Data.SQL;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IEntity<T> : IEntity
    {
        #region 创建分表
        /// <summary>
        /// 创建分表
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns>返回是否创建成功</returns>
        Task<Boolean> CreateSubTable(string suffix);
        /// <summary>
        /// 创建分表
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns>返回是否创建成功</returns>
        Task<Boolean> CreateSubTableSuffix(string suffix);
        #endregion

        #region 使用分库
        /// <summary>
        /// 使用分库分表
        /// </summary>
        /// <returns></returns>
        T Sub();
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <returns></returns>
        T SubDataBase();
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="key">数据库连接串</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        T SubDataBase(string key, bool isGlobal = false);
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="num">库索引</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        T SubDataBase(uint num, bool isGlobal = false);
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="key">分库数据库连接串</param>
        /// <param name="num">库索引</param>
        /// <param name="suffix">分表后缀</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        T SubDataBase(string key, uint num, string suffix = "", bool isGlobal = false);
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="suffix">分表后缀</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        T SubDataBase(ConnectionConfig config, string suffix = "", bool isGlobal = false);
        #endregion

        #region 使用分表
        /// <summary>
        /// 使用分表
        /// </summary>
        /// <returns></returns>
        T SubTable();
        /// <summary>
        /// 使用分表默认带 _FB_
        /// </summary>
        /// <param name="suffix">分表后缀</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        T SubTable(string suffix, bool isGlobal = false);
        /// <summary>
        /// 使用分表不带_FB_
        /// </summary>
        /// <param name="suffix">分表后缀</param>
        /// <returns></returns>
        T SubTableSuffix(string suffix);
        #endregion

        #region 增删改查
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        Boolean Delete(Expression<Func<T, bool>> func);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        Boolean Delete(Expression<Func<T, bool>> func, string whereString);
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="func">Lambda表达式</param>
        /// <returns></returns>
        IQueryableX<T> Where(Expression<Func<T, bool>> func);
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        T Find(Expression<Func<T, bool>> func);
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        object Find(Expression<Func<T, bool>> func, Type type);
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        T Find(Expression<Func<T, bool>> func, string whereString);
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        object Find(Expression<Func<T, bool>> func, string whereString, Type type);
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        List<T> ToList(Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="func">条件</param>
        /// <returns></returns>
        List<object> ToList(Type type, Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        List<T> ToList(Expression<Func<T, bool>> func, string whereString);
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        List<object> ToList(Expression<Func<T, bool>> func, string whereString, Type type);
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        List<object> ToObjectList(Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        List<object> ToObjectList(Expression<Func<T, bool>> func, string whereString);
        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="models">model集合</param>
        /// <returns></returns>
        bool Inserts(IEnumerable<T> models);
        #endregion

        #region 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        Boolean Exists(Expression<Func<T, bool>> where);
        #endregion

        #region 条数
        /// <summary>
        /// 条数
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> where = null);
        #endregion

        #region 当前模型的数据库对象
        /// <summary>
        /// 当前模型的数据库对象
        /// </summary>
        IDataHelper DataHelper { get; }
        #endregion
    }
    /// <summary>
    /// 实体接口
    /// </summary>
    public interface IEntity : IEntityBase
    {
        #region 属性
        /// <summary>
        /// 数据库名
        /// </summary>
        string DataBaseName { get; set; }
        /// <summary>
        /// 分库索引 
        /// </summary>
        uint DataBaseNum { get; set; }
        /*/// <summary>
        /// 分表名称
        /// </summary>
        string TableName { get; set; }*/
        #endregion

        #region 实体类型
        /// <summary>
        /// 实体类型
        /// </summary>
        ModelType TableType { get; set; }
        #endregion

        #region 增删改查
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="isBackAutoID">是否返回自增长ID</param>
        /// <returns></returns>
        bool Insert(Boolean isBackAutoID = false);
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        bool Update();
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        bool Delete();
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        bool Delete(string whereString);
        /// <summary>
        /// 列
        /// </summary>
        List<ColumnAttribute> Fields { get; set; }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        List<object> ToObjectList(string whereString);

        #endregion

        #region 查找主键
        /// <summary>
        /// 查找主键
        /// </summary>
        /// <returns></returns>
        string GetPrimaryKey();
        #endregion

        #region 获取唯一键字段
        /// <summary>
        /// 获取唯一键字段
        /// </summary>
        /// <returns></returns>
        List<string> GetUniqueKey();
        #endregion

        #region 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        Boolean Exists(string whereString);
        #endregion

        #region 条数
        /// <summary>
        /// 条数
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        int Count(string whereString);
        #endregion
    }
}