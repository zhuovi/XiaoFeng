using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Collections;

namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    public interface IDataHelper
    {
        #region 属性
        /// <summary>
        /// 数据库连接串配置
        /// </summary>
        ConnectionConfig ConnConfig { get; set; }
        /// <summary>
        /// 缓存时长
        /// </summary>
        int CacheTimeOut { get; set; }
        /// <summary>
        /// 出错信息
        /// </summary>
        string ErrorMessage { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
         string ConnectionString { get; set; }
        /// <summary>
        /// 执行命令时超时间
        /// </summary>
        int CommandTimeOut { get; set; }
        /// <summary>
        /// 数据驱动
        /// </summary>
        DbProviderType ProviderType { get; set; }
        /// <summary>
        /// 是否使用事务处理
        /// </summary>
        Boolean IsTransaction { get; set; }
        /// <summary>
        /// 连接池
        /// </summary>
        ConnectionPool Pool { get; }
        #endregion

        #region 创建数据库连接Conn
        /// <summary>
        /// 创建Data数据库连接
        /// </summary>
        /// <returns></returns>
        DbConnection CreateConn();
        #endregion

        #region 获取连接对象
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="fun">方法</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        T GetConn<T>(Func<DbConnection, DbProviderFactory, T> fun, Boolean isCloseConn = true);
        #endregion

        #region 执行Command
        /// <summary>
        /// 执行Command
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fun">方法体</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        T Execute<T>(Func<DbCommand, DbProviderFactory, T> fun, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null, Boolean isCloseConn = true);
        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="fun">方法体</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        T Execute<T>(Func<DbCommand, DbProviderFactory, T> fun, Action<Exception, string> error, IsolationLevel isolationLevel, Boolean isCloseConn);
        /// <summary>
        /// 执行Command
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fun">方法体</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        T Execute<T>(Func<DbCommand, T> fun, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null, Boolean isCloseConn = true);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="fun">方法体</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        T Execute<T>(Func<DbCommand, T> fun, Action<Exception, string> error, IsolationLevel isolationLevel, Boolean isCloseConn);
        #endregion

        #region 执行SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行行数</returns>
        int ExecuteNonQuery(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回执行行数</returns>
        int ExecuteNonQuery(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行SQL语句以及执行状态</returns>
        Dictionary<string, int> ExecuteNonQuery(List<string> commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回执行SQL语句以及执行状态</returns>
        Dictionary<string, int> ExecuteNonQuery(List<string> commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        #region 异步执行SQL语句
        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<int> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandText, Action<int> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        #region 执行SQL语句返回首行首列
        /// <summary>
        /// 执行SQL语句返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回首行首列数据</returns>
        object ExecuteScalar(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行SQL语句返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回首行首列数据</returns>
        object ExecuteScalar(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        #region 异步执行SQL语句返回首行首列
        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<object> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string commandText, Action<object> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        #region 执行SQL语句返回一个DataTable
        /// <summary>
        /// 执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        #region 异步执行SQL语句返回一个DataTable
        /// <summary>
        /// 异步执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<DataTable> ExecuteDataTableAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<DataTable> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<DataTable> ExecuteDataTableAsync(string commandText, Action<DataTable> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        #region 执行SQL语句返回一个DataSet
        /// <summary>
        /// 执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>DataSet</returns>
        DataSet ExecuteDataSet(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>DataSet</returns>
        DataSet ExecuteDataSet(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        #region 异步执行SQL语句返回一个DataSet
        /// <summary>
        /// 异步执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<DataSet> ExecuteDataSetAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<DataSet> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<DataSet> ExecuteDataSetAsync(string commandText, Action<DataSet> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        #region 执行SQL语句返回一个DataReader
        /// <summary>
        /// 执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        #region 异步执行SQL语句返回一个DataReader
        /// <summary>
        /// 异步执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调方法</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<DbDataReader> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调方法</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, Action<DbDataReader> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        /********************************************************************/
        /*                                                                  */
        /*                        下边是调用储存过程                        */
        /*                                                                  */
        /********************************************************************/

        #region 用输入的数据替换到储存过程中
        /// <summary>
        /// 用输入的数据替换到储存过程中
        /// </summary>
        /// <param name="parameter">SQLParamerter</param>
        /// <param name="commandText">储存过程</param>
        /// <returns></returns>
        string GetParamInfo(DbParameter[] parameter, string commandText);
        #endregion

        #region 创建储存过程参数
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="parameterDirection">所属类型</param>
        /// <returns></returns>
        DbParameter MakeParam(string paramName, object paramValue, ParameterDirection parameterDirection = ParameterDirection.Input);
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="paramType">参数类型</param>
        /// <param name="paramSize">参数大小</param>
        /// <param name="paramDirection">参数类型</param>
        /// <returns></returns>
        DbParameter MakeParam(string paramName, object paramValue, DbType paramType, int paramSize = 0, ParameterDirection paramDirection = ParameterDirection.Input);
        #endregion

        #region 返回一个带储存过程的Command
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="param">Parameter数组</param>
        /// <param name="conn">数据连接对象</param>
        /// <returns>返回一个Command</returns>
        DbCommand MakeParamCommand(string commandText, DbParameter[] param, DbConnection conn);
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">储存过程类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="conn">数据连接对象</param>
        /// <returns>返回一个Command</returns>
        DbCommand MakeParamCommand(string commandText, CommandType commandType, DbParameter[] parameter, DbConnection conn);
        #endregion

        #region 执行储存过程返回执行行数
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回执行行数</returns>
        int ExecuteNonQuery(string commandText, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">解析命令字符串方式</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        #region 异步执行储存过程返回执行行数
        /// <summary>
        /// 异步执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调方法</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandText, DbParameter[] parameter, Action<int> callback = null);
        /// <summary>
        /// 异步执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">解析命令字符串方式</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调方法</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行行数</returns>
        Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<int> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">解析命令字符串方式</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调方法</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行行数</returns>
        Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<int> callback = null, Action<Exception, string> error = null);
        #endregion

        #region 执行储存过程返回首行首列
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数值</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回首行首列数据</returns>
        object ExecuteScalar(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数值</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回首行首列数据</returns>
        object ExecuteScalar(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        #region 异步执行储存过程返回首行首列
        /// <summary>
        /// 异步执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调方法</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string commandText, DbParameter[] parameter, Action<object> callback = null);
        /// <summary>
        /// 异步执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数值</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回首行首列数据</returns>
        Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<object> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数值</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回首行首列数据</returns>
        Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<object> callback = null, Action<Exception, string> error = null);
        #endregion

        #region 执行储存过程返回一个DataTable
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataTable</returns>
        DataTable ExecuteDataTable(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数数组</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回一个DataTable</returns>
        DataTable ExecuteDataTable(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataTable</returns>
        DataTable ExecuteDataTable(string commandText, DbParameter[] parameter, Action<Exception, string> error = null);
        #endregion

        #region 异步执行储存过程返回一个DataTable
        /// <summary>
        /// 异步执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数数组</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataTable</returns>
        Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DataTable> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataTable</returns>
        Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<DataTable> callback = null, Action<Exception, string> error = null);
        #endregion

        #region 执行储存过程返回一个DataSet
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数集合</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>DataSet</returns>
        DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数集合</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>DataSet</returns>
        DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataSet</returns>
        DataSet ExecuteDataSet(string commandText, DbParameter[] parameter, Action<Exception, string> error = null);
        #endregion

        #region 异步执行储存过程返回一个DataSet
        /// <summary>
        /// 异步执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数集合</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>DataSet</returns>
        Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DataSet> callback = null, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数集合</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="callback">回调</param>
        /// <param name="error">错误回调</param>
        /// <returns>DataSet</returns>
        Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<DataSet> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callbak">回调</param>
        /// <returns>返回一个DataSet</returns>
        Task<DataSet> ExecuteDataSetAsync(string commandText, DbParameter[] parameter, Action<DataSet> callbak = null);
        #endregion

        #region 执行储存过程返回一个DataReader
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数组</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        #region 异步执行储存过程返回一个DataReader
        /// <summary>
        /// 异步执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数组</param>
        /// <param name="callback">正确回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DbDataReader> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数组</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<DbDataReader> callback = null, Action<Exception, string> error = null);
        /// <summary>
        /// 异步执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, DbParameter[] parameter, Action<DbDataReader> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null);
        #endregion

        /********************************************************************/
        /*                                                                  */
        /*                       下边是数据库相关操作                       */
        /*                                                                  */
        /********************************************************************/

        #region 分页查询数据
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="Columns">显示列</param>
        /// <param name="Condition">条件</param>
        /// <param name="OrderColumnName">排序字段</param>
        /// <param name="OrderType">排序类型ASC或DESC</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">一页多少条</param>
        /// <param name="PageCount">共多少页</param>
        /// <param name="Counts">共多少条</param>
        /// <param name="PrimaryKey">主键</param>
        /// <returns></returns>
        DataTable Select(string tableName, string Columns, string Condition, string OrderColumnName, string OrderType, int PageIndex, int PageSize, out int PageCount, out int Counts, string PrimaryKey = "");
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="Columns">显示列</param>
        /// <param name="Condition">条件</param>
        /// <param name="OrderColumnName">排序字段</param>
        /// <param name="OrderType">排序类型ASC或DESC</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">一页多少条</param>
        /// <param name="PageCount">共多少页</param>
        /// <param name="Counts">共多少条</param>
        /// <param name="PrimaryKey">主键</param>
        /// <returns></returns>
        List<T> Select<T>(string tableName, string Columns, string Condition, string OrderColumnName, string OrderType, int PageIndex, int PageSize, out int PageCount, out int Counts, string PrimaryKey = "");
        #endregion

        #region 获取数据库操作对象
        /// <summary>
        /// 获取数据库操作对象
        /// </summary>
        /// <returns></returns>
        IDbHelper GetData();
        #endregion

        #region 缓存操作
        /// <summary>
        /// 获取CacheKey
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">参数集</param>
        /// <returns></returns>
        string GetCacheKey(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="data">数据</param>
        void SetCacheData(string commandText, object data);
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <returns></returns>
        object GetCacheData(string commandText);
        /// <summary>
        /// 获取SQL语句中的缓存关键字
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <returns></returns>
        string GetCacheValue(ref string commandText);
        #endregion

        /********************************************************************/
        /*                                                                  */
        /*                              公用对象                            */
        /*                                                                  */
        /********************************************************************/

        #region 获取对象
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="SQL">SQL语句</param>
        /// <returns></returns>
        T Query<T>(string SQL);
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="SQL">SQL语句</param>
        /// <returns></returns>
        List<T> QueryList<T>(string SQL);
        #endregion
    }
}