using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/*
===================================================================
    Author : jacky
    Email : jacky@zhuovi.com
    QQ : 7092734
    Site : www.zhuovi.com
    Verstion : 1.1.7
    Create Time : 2017/12/18 10:18:41
    Update Time : 2018/07/12 16:07:16
    Description : 
    v 1.1.6   2018/07/12 16:07:16
    1.增加设置表名方法 SetTable
===================================================================
*/
namespace XiaoFeng.Data.SQL
{
    #region T1
    /// <summary>
    /// 操作SQL
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface IQueryableX<T>
    {
        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 插入数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <returns>返回自增长ID</returns>
        Int64 Add<TOther>(TOther model) where TOther : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">实例集合</param>
        /// <returns></returns>
        Boolean Inserts<TOther>(IEnumerable<TOther> models) where TOther : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <returns></returns>
        IQueryableX<T> InsertQ<TOther>(TOther model) where TOther : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">实例</param>
        /// <returns></returns>
        Boolean Insert<TOther>(TOther model) where TOther : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">数据Model</param>
        /// <param name="ID">自增长ID</param>
        /// <returns></returns>
        Boolean Insert<TOther>(TOther model, out Int64 ID) where TOther : class;
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <returns></returns>
        IQueryableX<T> InsertQ<TOther>(Expression<Func<T, TOther>> fResult);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <returns></returns>
        Boolean Insert<TOther>(Expression<Func<T, TOther>> fResult);
        #endregion

        #region 更新数据
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> UpdateQ<TOther>(Expression<Func<T, TOther>> fResult, Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">model</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> UpdateQ<TOther>(TOther model, Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="fResult">结果对象</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        Boolean Update<TOther>(Expression<Func<T, TOther>> fResult, Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">实例</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        Boolean Update<TOther>(TOther model, Expression<Func<T, bool>> func = null) where TOther : class;
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">实例</param>
        /// <param name="whereString">条件字符串 如果更新所有则输入'1=1'</param>
        /// <returns></returns>
        Boolean Update<TOther>(TOther model, string whereString) where TOther : class;
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">集合</param>
        /// <returns></returns>
        Boolean Update<TOther>(IEnumerable<TOther> models) where TOther : class;
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">集合</param>
        /// <returns></returns>
        Boolean Update<TOther>(List<TOther> models) where TOther : class;
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="models">集合</param>
        /// <returns></returns>
        Boolean Updates<TOther>(IEnumerable<TOther> models) where TOther : class;
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> DeleteQ(Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        Boolean Delete(Expression<Func<T, bool>> func = null);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="whereString">空则为无效,1=1删除所有,drop删除表,truncate初始化表</param>
        /// <returns></returns>
        Boolean Delete(string whereString);
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="Columns">显示字段</param>
        /// <returns></returns>
        IQueryableX<T> Select(string Columns);
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="func">显示字段Lambda</param>
        /// <returns></returns>
        IQueryableX<TResult> Select<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <param name="func">显示字段Lambda</param>
        /// <returns></returns>
        IQueryableX<T> SelectX<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryableX<T> SetTable(string tableName);
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">对象</param>
        /// <returns></returns>
        IQueryableX<T> SetTable(Dictionary<TableType, string> tableName);
        #endregion

        #region  扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="whereString">条件字符串</param>
        /// <returns></returns>
        IQueryableX<T> Where(string whereString);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> Where(Expression<Func<T, bool>> func);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="model">条件字符串</param>
        /// <returns></returns>
        IQueryableX<T> Where<TOther>(TOther model) where TOther : class,new();
        #endregion

        #region 返回数据实体
        /// <summary>
        /// 返回数据实体
        /// </summary>
        /// <returns></returns>
        T ToEntity();
        /// <summary>
        /// 返回数据实体
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        object ToEntity(Type type);
        /// <summary>
        /// 返回数据实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        TResult ToEntity<TResult>();
        #endregion

        #region 返回数据实体集合
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <returns></returns>
        List<T> ToList();
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        List<T> ToList(int page, int pageSize);
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <param name="pageCount">共有多少页</param>
        /// <param name="counts">共有多少条</param>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        List<T> ToList(int page, int pageSize, out int pageCount, out int counts, string primaryKey = "");
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        List<TResult> ToList<TResult>();
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        List<object> ToList(Type type);
        /// <summary>
        /// 返回数据实体集合
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        List<TResult> ToList<TResult>(int page, int pageSize);
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
        List<TResult> ToList<TResult>(int page, int pageSize, out int pageCount, out int counts, string primaryKey = "");
        #endregion

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T> Take(int topCount);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> Take(int topCount, Expression<Func<T, bool>> func);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> TakeWhile(int topCount, Expression<Func<T, bool>> func);
        #endregion

        #region 扩展SQL Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T> OrderBy(string orderString);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T> ThenBy<TResult>(Expression<Func<T, TResult>> func);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T> OrderBy<TResult>(Expression<Func<T, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T> OrderByDescending(string orderString);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T> ThenByDescending<TResult>(Expression<Func<T, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL Group By
        /// <summary>
        /// 扩展group by
        /// </summary>
        /// <param name="groupString">分组串</param>
        /// <returns></returns>
        IQueryableX<T> GroupBy(string groupString);
        /// <summary>
        /// 扩展group by
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">分组Lambda</param>
        /// <returns></returns>
        IQueryableX<T> GroupBy<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL DISTINCT
        /// <summary>
        /// 扩展SQL DISTINCT
        /// </summary>
        /// <param name="distinctString">Distinct列</param>
        /// <returns></returns>
        IQueryableX<T> Distinct(string distinctString);
        /// <summary>
        /// 扩展SQL DISTINCT
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Distinct Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Distinct<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL SUM
        /// <summary>
        /// 扩展SQL SUM
        /// </summary>
        /// <param name="sumString">Sum Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Sum(string sumString);
        /// <summary>
        /// 扩展SQL SUM
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Sum Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Sum<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL Count
        /// <summary>
        /// 条数
        /// </summary>
        /// <returns></returns>
        int CountX();
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 扩展SQL COUNT
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        int Count(Expression<Func<T, Boolean>> func);
        /// <summary>
        /// 扩展SQL Count
        /// </summary>
        /// <param name="countString">Count 字段字符串</param>
        /// <returns></returns>
        IQueryableX<T> Count(string countString);
        /// <summary>
        /// 扩展SQL Count
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Count Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Count<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL Avg
        /// <summary>
        /// 扩展SQL Avg
        /// </summary>
        /// <param name="avgString">Avg Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Avg(string avgString);
        /// <summary>
        /// 扩展SQL Avg
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Avg Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Avg<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL Max
        /// <summary>
        /// 扩展SQL Max
        /// </summary>
        /// <param name="maxString">Max Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Max(string maxString);
        /// <summary>
        /// 扩展SQL Max
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Max Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Max<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展SQL Min
        /// <summary>
        /// 扩展SQL Min
        /// </summary>
        /// <param name="minString">Min Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Min(string minString);
        /// <summary>
        /// 扩展SQL Min
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">Min Lmabda</param>
        /// <returns></returns>
        IQueryableX<T> Min<TResult>(Expression<Func<T, TResult>> func);
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <returns></returns>
        [Obsolete("请使用First方法")]
        T FirstOrDefault();
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        [Obsolete("请使用First方法")]
        T FirstOrDefault(Expression<Func<T, bool>> func);
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <returns></returns>
        T First();
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        T First(Expression<Func<T, bool>> func);
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        TResult First<TResult>();
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        TResult First<TResult>(Expression<Func<T, bool>> func);
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <returns></returns>
        T Last();
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        T Last(Expression<Func<T, bool>> func);
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <returns></returns>
        TResult Last<TResult>();
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        TResult Last<TResult>(Expression<Func<T, bool>> func);
        #endregion

        #region 跳过几条数据
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        IQueryableX<T> Skip(int skipCount);
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> Skip(int skipCount, Expression<Func<T, bool>> func);
        /// <summary>
        /// 跳过几条数据 遇到条件跳过
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T> SkipWhile(int skipCount, Expression<Func<T, bool>> func);
        #endregion

        #region 扩展Any
        /// <summary>
        /// 扩展Any
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        Boolean Any(Expression<Func<T, bool>> func = null);
        #endregion

        #region 转换关联表
        /// <summary>
        /// SQL语句转换成IQueryableX
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        IQueryableX<T> AsQueryableX(string SQLString);
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <param name="SQLString">第二个SQL语句</param>
        /// <returns></returns>
        IQueryableX<T, T2> AsQueryableX<T2>(string SQLString) where T2 : class, new();
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <typeparam name="T3">类型</typeparam>
        /// <param name="SQLString2">第二个SQL语句</param>
        /// <param name="SQLString3">第三个SQL语句</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> AsQueryableX<T2, T3>(string SQLString2, string SQLString3)
            where T2 : class, new()
            where T3 : class, new();
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <typeparam name="T3">类型</typeparam>
        /// <returns></returns>
        IQueryableX<T, T2, T3> AS<T2, T3>()
            where T2 : class,new()
            where T3 : class,new();
        /// <summary>
        /// 转换关联表
        /// </summary>
        /// <typeparam name="T2">类型</typeparam>
        /// <returns></returns>
        IQueryableX<T, T2> AS<T2>() where T2 : class,new();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T> AS();
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="T2">目标对象</typeparam>
        /// <returns></returns>
        IQueryableX<T2> To<T2>() where T2 : class,new();
        #endregion

        #region 扩展Join
        /// <summary>
        /// 扩展Join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Join<T2, TResult>(Expression<Func<T, T2, TResult>> funcOn);
        /// <summary>
        /// 扩展Join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Join<T2, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T, T2, TResult>> funcOn);
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Join<T2, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T, T2, TResult>> funcOn);
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
        IQueryableX<T, T2, T3> Join<T2, T3, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, TResult>> funcOn);
        /// <summary>
        /// 扩展join 请用Join方法
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="T3">T3类型</typeparam>
        /// <typeparam name="TResult">On返回类型</typeparam>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Join<T2, T3, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, TResult>> funcOn);
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
        IQueryableX<T, T2, T3, T4> Join<T2, T3, T4, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, TResult>> funcOn);
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
        IQueryableX<T, T2, T3, T4> Join<T2, T3, T4, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, TResult>> funcOn);
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
        IQueryableX<T, T2, T3, T4, T5> Join<T2, T3, T4, T5, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, TResult>> funcOn);
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
        IQueryableX<T, T2, T3, T4, T5> Join<T2, T3, T4, T5, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, TResult>> funcOn);
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
        IQueryableX<T, T2, T3, T4, T5, T6> Join<T2, T3, T4, T5, T6, TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> funcOn);
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
        IQueryableX<T, T2, T3, T4, T5, T6> Join<T2, T3, T4, T5, T6, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> funcOn);
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
        List<TResult> Join<T2, TThird, TResult>(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T, T2, TThird>> funcOn, Expression<Func<T, T2, TResult>> fResult)
            where T2 : class, new()
            where TResult : class, new();
        /// <summary>
        /// 关联表
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="joinType">关联类型</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Join<T2>(JoinType joinType, Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null);
        /// <summary>
        /// 左连接关联表（left join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null);
        /// <summary>
        /// 右连接关联表（right join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null);
        /// <summary>
        /// 内连接关联表（inner join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null);
        /// <summary>
        /// 合并连接关联表（union join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> UnionJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null);
        /// <summary>
        /// 全连接关联表（full join）
        /// </summary>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <param name="funcOn">On条件Lambda</param>
        /// <param name="func">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> funcOn, Expression<Func<T2, bool>> func = null);
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        IQueryableX<T> Clone();
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        IQueryableX<T> Cache(uint? TimeOut = null);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T> NoCache();
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T> ClearCache();
        #endregion

        #region 设置SQL语句类型
        /// <summary>
        /// 设置SQL语句类型
        /// </summary>
        /// <param name="sqlType">SQL语句类型</param>
        /// <returns></returns>
        IQueryableX<T> SetSqlType(SQLType sqlType = SQLType.NULL);
        #endregion

        #region 获取SQL语句
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <param name="sqlType">SQL语句类型</param>
        /// <returns></returns>
        string SQL(SQLType sqlType = SQLType.NULL);
        #endregion
    }
    #endregion

    #region T,T2
    /// <summary>
    /// 操作两张表数据
    /// </summary>
    /// <typeparam name="T">第一张表类型</typeparam>
    /// <typeparam name="T2">第二张表类型</typeparam>
    public interface IQueryableX<T, T2>
    {
        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2> Take(int topCount);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2> TakeWhile(int topCount);
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        IQueryableX<T, T2> Skip(int skipCount);
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult First<TResult>(Expression<Func<T, T2, TResult>> func) where TResult : class, new();
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult Last<TResult>(Expression<Func<T, T2, TResult>> func) where TResult : class, new();
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> On<TResult>(Expression<Func<T, T2, TResult>> func);
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> On(Expression<Func<T, T2, bool>> func);
        #endregion

        #region 扩展join 
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T, T2, TResult>> funcOn);
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Join(Expression<Func<T2, bool>> func, Expression<Func<T, T2, bool>> funcOn);
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Join<T3>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, bool>> funcOn);
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> func);
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<T, T2> SelectX<TResult>(Expression<Func<T, T2, TResult>> func);
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryableX<T, T2> SetTable(Dictionary<TableType, string> tableName);
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2> OrderBy(string orderString);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> OrderBy<TResult>(Expression<Func<T, T2, TResult>> func);
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2> OrderByDescending(string orderString);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> OrderByDescending<TResult>(Expression<Func<T, T2, TResult>> func);
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func);
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
        List<TResult> ToList<TResult>(Expression<Func<T, T2, TResult>> func, int page = 0, int pageSize = 0) where TResult : class, new();
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        TResult ToEntity<TResult>(Expression<Func<T, T2, TResult>> func) where TResult : class, new();
        #endregion

        #region  扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Where(Expression<Func<T, T2, Boolean>> func);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <param name="tableType">表类型</param>
        /// <returns></returns>
        void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">T条件Lambda</param>
        /// <param name="func2">T2条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2);
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2> AS();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2> Clone();
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        IQueryableX<T, T2> Cache(uint? TimeOut = null);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2> NoCache();
        #endregion
    }
    #endregion

    #region T,T2,T3
    /// <summary>
    /// 操作三张表数据
    /// </summary>
    /// <typeparam name="T">第一张表类型</typeparam>
    /// <typeparam name="T2">第二张表类型</typeparam>
    /// <typeparam name="T3">第三张表类型</typeparam>"
    public interface IQueryableX<T, T2, T3>
    {
        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Take(int topCount);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> TakeWhile(int topCount);
        #endregion

        #region 扩展Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Skip(int skipCount);
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult First<TResult>(Expression<Func<T, T2, T3, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult Last<TResult>(Expression<Func<T, T2, T3, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> On<TResult>(Expression<Func<T, T2, T3, TResult>> func);
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> On(Expression<Func<T, T2, T3, bool>> func);
        #endregion

        #region 扩展join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, bool>> funcOn);
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T, T2, T3, TResult>> funcOn);
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> func);
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> SelectX<TResult>(Expression<Func<T, T2, T3, TResult>> func);
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> SetTable(Dictionary<TableType, string> tableName);
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> OrderBy(string orderString);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> OrderBy<TResult>(Expression<Func<T, T2, T3, TResult>> func);
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> OrderByDescending(string orderString);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> OrderByDescending<TResult>(Expression<Func<T, T2, T3, TResult>> func);
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func);
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
        List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, TResult>> func, int page = 0, int pageSize = 0) where TResult : class,new();
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        TResult ToEntity<TResult>(Expression<Func<T, T2, T3, TResult>> func) where TResult : class,new();
        #endregion

        #region  扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Where(Expression<Func<T, T2, T3, Boolean>> func);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <param name="tableType">表类型</param>
        /// <returns></returns>
        void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3);
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3> AS();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Clone();
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3> Cache(uint? TimeOut = null);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3> NoCache();
        #endregion
    }
    #endregion

    #region T,T2,T3,T4
    /// <summary>
    /// 操作四张表数据
    /// </summary>
    /// <typeparam name="T">第一张表类型</typeparam>
    /// <typeparam name="T2">第二张表类型</typeparam>
    /// <typeparam name="T3">第三张表类型</typeparam>
    /// <typeparam name="T4">第四张表类型</typeparam>
    public interface IQueryableX<T, T2, T3, T4>
    {
        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Take(int topCount);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> TakeWhile(int topCount);
        #endregion

        #region 扩展 Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Skip(int skipCount);
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult First<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult Last<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> On<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func);
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> On(Expression<Func<T, T2, T3, T4, bool>> func);
        #endregion

        #region 扩展join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, bool>> funcOn);
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <typeparam name="TResult">On类型</typeparam>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T, T2, T3, T4, TResult>> funcOn);
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func);
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> SelectX<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func);
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> SetTable(Dictionary<TableType, string> tableName);
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> OrderBy(string orderString);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func);
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> OrderByDescending(string orderString);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> OrderByDescending<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func);
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func);
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
        List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func, int page = 0, int pageSize = 0) where TResult : class,new();
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        TResult ToEntity<TResult>(Expression<Func<T, T2, T3, T4, TResult>> func) where TResult : class,new();
        #endregion

        #region  扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, Boolean>> func);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <param name="tableType">表类型</param>
        /// <returns></returns>
        void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <param name="func4">第4张表条件lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4);
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> AS();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Clone();
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> Cache(uint? TimeOut = null);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4> NoCache();
        #endregion

    }
    #endregion

    #region T,T2,T3,T4,T5
    /// <summary>
    /// 操作四张表数据
    /// </summary>
    /// <typeparam name="T">第一张表类型</typeparam>
    /// <typeparam name="T2">第二张表类型</typeparam>
    /// <typeparam name="T3">第三张表类型</typeparam>
    /// <typeparam name="T4">第四张表类型</typeparam>
    /// <typeparam name="T5">第五张表类型</typeparam>
    public interface IQueryableX<T, T2, T3, T4, T5>
    {
        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Take(int topCount);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> TakeWhile(int topCount);
        #endregion

        #region 扩展 Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Skip(int skipCount);
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult First<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult Last<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> On<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func);
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> On(Expression<Func<T, T2, T3, T4, T5, bool>> func);
        #endregion

        #region 扩展join
        /// <summary>
        /// 扩展join
        /// </summary>
        /// <param name="func">T2条件Lambda</param>
        /// <param name="func3">T3条件Lambda</param>
        /// <param name="func4">T4条件Lambda</param>
        /// <param name="func5">T5条件Lambda</param>
        /// <param name="funcOn">On条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, bool>> funcOn);
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
        IQueryableX<T, T2, T3, T4, T5> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T, T2, T3, T4, T5, TResult>> funcOn);
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func);
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> SelectX<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func);
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> SetTable(Dictionary<TableType, string> tableName);
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> OrderBy(string orderString);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func);
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> OrderByDescending(string orderString);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> OrderByDescending<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func);
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func);
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
        List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func, int page = 0, int pageSize = 0) where TResult : class,new();
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        TResult ToEntity<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> func) where TResult : class,new();
        #endregion

        #region  扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, Boolean>> func);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <param name="tableType">表类型</param>
        /// <returns></returns>
        void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">第1张表条件Lambda</param>
        /// <param name="func2">第2张表条件Lambda</param>
        /// <param name="func3">第3张表条件lambda</param>
        /// <param name="func4">第4张表条件lambda</param>
        /// <param name="func5">第5张表条件lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5);
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> AS();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Clone();
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> Cache(uint? TimeOut = null);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5> NoCache();
        #endregion
    }
    #endregion

    #region T,T2,T3,T4,T5,T6
    /// <summary>
    /// 操作四张表数据
    /// </summary>
    /// <typeparam name="T">第一张表类型</typeparam>
    /// <typeparam name="T2">第二张表类型</typeparam>
    /// <typeparam name="T3">第三张表类型</typeparam>
    /// <typeparam name="T4">第四张表类型</typeparam>
    /// <typeparam name="T5">第五张表类型</typeparam>
    /// <typeparam name="T6">第六张表类型</typeparam>
    public interface IQueryableX<T, T2, T3, T4, T5, T6>
    {
        #region 事件
        /// <summary>
        /// 执行完SQL回调
        /// </summary>
        event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 前几条数据
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> Take(int topCount);
        /// <summary>
        /// 前几条数据
        /// </summary>
        /// <param name="topCount">前多少条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> TakeWhile(int topCount);
        #endregion

        #region 扩展 Skip
        /// <summary>
        /// 跳过几条数据
        /// </summary>
        /// <param name="skipCount">跳几条</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> Skip(int skipCount);
        #endregion

        #region 扩展First
        /// <summary>
        /// 扩展First
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult First<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展Last
        /// <summary>
        /// 扩展Last
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        TResult Last<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func) where TResult : class,new();
        #endregion

        #region 扩展On条件
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> On<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func);
        /// <summary>
        /// 扩展On条件
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> On(Expression<Func<T, T2, T3, T4, T5, T6, bool>> func);
        #endregion

        #region 扩展join
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
        IQueryableX<T, T2, T3, T4, T5, T6> Join(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, bool>> funcOn);
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
        IQueryableX<T, T2, T3, T4, T5, T6> Join<TResult>(Expression<Func<T2, bool>> func, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> funcOn);
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func);
        #endregion

        #region 设置显示字段
        /// <summary>
        /// 设置显示字段
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">条件</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> SelectX<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func);
        #endregion

        #region 设置表名
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> SetTable(Dictionary<TableType, string> tableName);
        #endregion

        #region 排序 Order By
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> OrderBy(string orderString);
        /// <summary>
        /// 设置正序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">正序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func);
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> OrderBy<TModel, TResult>(Expression<Func<TModel, TResult>> func);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <param name="orderString">排序字符串</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> OrderByDescending(string orderString);
        /// <summary>
        /// 设置倒序排序
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="func">倒序Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> OrderByDescending<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func);
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <typeparam name="TModel">结果集类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> OrderByDescending<TModel, TResult>(Expression<Func<TModel, TResult>> func);
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
        List<TResult> ToList<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func, int page = 0, int pageSize = 0) where TResult : class,new();
        #endregion

        #region 返回实体
        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">返回实例结构Lambda</param>
        /// <returns></returns>
        TResult ToEntity<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> func) where TResult : class,new();
        #endregion

        #region  扩展SQL 条件算法
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <param name="func">条件Lambda</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, Boolean>> func);
        /// <summary>
        /// 扩展SQL 条件算法
        /// </summary>
        /// <typeparam name="TOther">类型</typeparam>
        /// <param name="func">条件Lambda</param>
        /// <param name="tableType">表类型</param>
        /// <returns></returns>
        void Where<TOther>(Expression<Func<TOther, bool>> func, TableType tableType);
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
        IQueryableX<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> func, Expression<Func<T2, bool>> func2, Expression<Func<T3, bool>> func3, Expression<Func<T4, bool>> func4, Expression<Func<T5, bool>> func5, Expression<Func<T6, bool>> func6);
        #endregion

        #region 复制
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> AS();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> Clone();
        #endregion

        #region 设置缓存状态
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存过期时长 单位为秒</param>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> Cache(uint? TimeOut = null);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        IQueryableX<T, T2, T3, T4, T5, T6> NoCache();
        #endregion
    }
    #endregion
}