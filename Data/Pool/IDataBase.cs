using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Data.Pool
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    public interface IDataBase
    {
        #region 方法
        /// <summary>
        /// 返回数据表
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string SqlString);
        /// <summary>
        /// 异步返回数据表
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        Task<DataTable> ExecuteDataTableAsync(string SqlString);
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        List<T> QueryList<T>(string SqlString);
        /// <summary>
        /// 异步获取数据列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        Task<List<T>> QueryListAsync<T>(string SqlString);
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        T Query<T>(string SqlString);
        /// <summary>
        /// 异步获取数据表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        Task<T> QueryAsync<T>(string SqlString);
        /// <summary>
        /// 返回执行行数
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        int ExecuteNonQuery(string SqlString);
        /// <summary>
        /// 异步返回执行行数
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string SqlString);
        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        object ExecuteScalar(string SqlString);
        /// <summary>
        /// 异步返回首行首列
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string SqlString);
        /// <summary>
        /// 返回一个DbDataReader
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <param name="callback">回调方法</param>
        void ExecuteReader(string SqlString, Action<DbDataReader> callback);
        /// <summary>
        /// 返回一个DbDataReader
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <param name="callback">回调方法</param>
        void ExecuteReaderAsync(string SqlString, Action<DbDataReader> callback);
        /// <summary>
        /// 异步返回一个DbDataReader
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        Task<DbDataReader> ExecuteReaderAsync(string SqlString);
        #endregion

        #region 储存过程方法

        #region 用输入的数据替换到储存过程中
        /// <summary>
        /// 用输入的数据替换到储存过程中
        /// </summary>
        /// <param name="commandText">储存过程</param>
        /// <param name="parameter">SQLParamerter</param>
        /// <returns></returns>
        string GetParamInfo(string commandText, DbParameter[] parameter);
        #endregion

        #region 创建储存过程参数
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">所属类型</param>
        /// <returns></returns>
        DbParameter MakeParam(string name, object value, ParameterDirection direction = ParameterDirection.Input);
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="direction">参数类型</param>
        /// <returns></returns>
        DbParameter MakeParam(string name, object value, DbType type, int size = 0, ParameterDirection direction = ParameterDirection.Input);
        #endregion

        #region 返回一个带储存过程的Command
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="param">Parameter数组</param>
        /// <param name="db">数据库连接对象</param>
        /// <returns>返回一个Command</returns>
        DbCommand MakeParamCommand(string commandText, DbParameter[] param, DbConnection db);
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">储存过程类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="db">数据库连接对象</param>
        /// <returns>返回一个Command</returns>
        DbCommand MakeParamCommand(string commandText, CommandType commandType, DbParameter[] parameter, DbConnection db);
        #endregion

        #region 执行储存过程返回执行行数
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回执行行数</returns>
        int ExecuteNonQuery(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回执行行数</returns>
        int ExecuteNonQuery(string commandText, CommandType commandType, DbParameter[] parameter);
        #endregion

        #region 执行储存过程返回首行首列
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回首行首列</returns>
        object ExecuteScalar(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回首行首列</returns>
        object ExecuteScalar(string commandText, CommandType commandType, DbParameter[] parameter);
        #endregion

        #region 执行储存过程返回一个DataTable
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataTable</returns>
        DataTable ExecuteDataTable(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataTable</returns>
        DataTable ExecuteDataTable(string commandText, CommandType commandType, DbParameter[] parameter);
        #endregion

        #region 执行储存过程返回一个DataSet
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataSet</returns>
        DataSet ExecuteDataSet(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataSet</returns>
        DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] parameter);
        #endregion

        #region 执行储存过程返回一个DataReader
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns>返回一个DataReader</returns>
        void ExecuteReader(string commandText, DbParameter[] parameter, Action<DbDataReader> callback);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns>返回一个DataReader</returns>
        void ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter, Action<DbDataReader> callback);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        DbDataReader ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个List
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个List</returns>
        List<T> QueryList<T>(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个List
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个List</returns>
        List<T> QueryList<T>(string commandText, CommandType commandType, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个T
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个T</returns>
        T Query<T>(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 执行储存过程返回一个T
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个T</returns>
        T Query<T>(string commandText, CommandType commandType, DbParameter[] parameter);
        #endregion

        #region 异步执行储存过程返回一个DataReader
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        void ExecuteReaderAsync(string commandText, DbParameter[] parameter, Action<DbDataReader> callback);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        void ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DbDataReader> callback);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, DbParameter[] parameter);
        /// <summary>
        /// 异步执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter);
        #endregion
        #endregion
    }
}