using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Collections;
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
namespace XiaoFeng.Data.Pool
{
    /// <summary>
    /// 数据库基础类
    /// </summary>
    public abstract class DbBase : Disposable, IDataBase
    {
        #region 构造器
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="config">配置</param>
        public DbBase(ConnectionConfig config)
        {
            this.Config = config;
        }
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="ConnectionString">连接串</param>
        public DbBase(string ConnectionString)
        {
            this.Config = new ConnectionConfig { ConnectionString = ConnectionString };
        }
        #endregion

        #region 属性
        /// <summary>
        /// 对象
        /// </summary>
        public static object obj = new object();
        /// <summary>
        /// 数据库配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        /// <summary>
        /// 连接池
        /// </summary>
        private ConnectionPool _Pool = null;
        /// <summary>
        /// 连接池
        /// </summary>
        public ConnectionPool Pool
        {
            get
            {
                if (_Pool != null) return _Pool;
                //lock (obj)
                {
                    return _Pool = new ConnectionPool(this.Config, $"Pool<ConnectionPool>[{this.Config.ConnectionString.ReplacePattern(@"((password|pwd)=)([^;]*)(;|$)", "$1******$4")}]");
                }
            }
        }
        #endregion

        #region 方法

        #region 返回数据表
        /// <summary>
        /// 返回数据表
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual DataTable ExecuteDataTable(string SqlString)
        {
            return this.Pool.Execute((db, factory) =>
            {
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                var Dt = new DataTable();
#if NETSTANDARD2_0 || NETFRAMEWORK
                var sda = factory.CreateDataAdapter();
                sda.SelectCommand = cmd;
                sda.Fill(Dt);
#else

                if (factory.CanCreateDataAdapter)
                {
                    var sda = factory.CreateDataAdapter();
                    sda.SelectCommand = cmd;
                    sda.Fill(Dt);
                }
                else
                {
                    var sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    sdr.GetColumnSchema().Each(c =>
                    {
                        var dc = new DataColumn(c.ColumnName, c.DataType);
                        if (c.IsAutoIncrement.HasValue && c.IsAutoIncrement.Value)
                        {
                            dc.AutoIncrementStep = 1;
                            dc.AutoIncrement = true;
                            dc.AutoIncrementSeed = 1;
                        }
                        //if (c.IsUnique.HasValue)
                        //    dc.Unique = c.IsUnique.Value;
                        if (c.AllowDBNull.HasValue)
                            dc.AllowDBNull = c.AllowDBNull.Value;

                        Dt.Columns.Add(dc);
                    });
                    while (sdr.Read())
                    {
                        var dr = Dt.NewRow();
                        for (var i = 0; i < sdr.FieldCount; i++)
                        {
                            dr[i] = sdr[i].GetValue(Dt.Columns[i].DataType);
                        }
                        Dt.Rows.Add(dr);
                    }
                    sdr.Close();
                    sdr = null;
                }
#endif     
                return Dt;
            });
        }
        /// <summary>
        /// 异步返回数据表
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual Task<DataTable> ExecuteDataTableAsync(string SqlString)
        {
            return Task.Factory.StartNew(() =>
            {
                var Dt = new DataTable();
                var Dr = this.ExecuteReaderAsync(SqlString).Result;
                Dt.Load(Dr);
                return Dt;
            });
        }
        #endregion

        #region 获取数据列表
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual List<T> QueryList<T>(string SqlString)
        {
            return this.ExecuteDataTable(SqlString).ToList<T>();
        }
        /// <summary>
        /// 异步获取数据列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual Task<List<T>> QueryListAsync<T>(string SqlString)
        {
            return Task.Factory.StartNew(() =>
            {
                return this.ExecuteDataTableAsync(SqlString).Result.ToList<T>();
            });
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual T Query<T>(string SqlString)
        {
            return this.ExecuteDataTable(SqlString).ToEntity<T>();
        }
        /// <summary>
        /// 异步获取数据列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual Task<T> QueryAsync<T>(string SqlString)
        {
            return Task.Factory.StartNew(() =>
            {
                return this.ExecuteDataTableAsync(SqlString).Result.ToEntity<T>();
            });
        }
        #endregion

        #region 返回执行行数
        /// <summary>
        /// 返回执行行数
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(string SqlString)
        {
            return this.Pool.Execute(db =>
            {
                if (db.State != ConnectionState.Open) db.Open();
                var M = -1;
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                if (this.Config.IsTransaction)
                {
                    using (var tran = db.BeginTransaction())
                    {
                        cmd.Transaction = tran;
                        try
                        {
                            M = cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (DbException ex)
                        {
                            LogHelper.Error(ex);
                            tran.Rollback();
                        }
                    }
                }
                else
                {
                    try
                    {
                        M = cmd.ExecuteNonQuery();
                    }
                    catch (DbException ex)
                    {
                        LogHelper.Error(ex);
                    }
                }
                return M;
            });
        }
        /// <summary>
        /// 异步返回执行行数
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual Task<int> ExecuteNonQueryAsync(string SqlString)
        {
            return this.Pool.Execute(db =>
            {
                if (db.State != ConnectionState.Open) db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                return cmd.ExecuteNonQueryAsync();
            });
        }
        #endregion

        #region 返回首行首列
        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual object ExecuteScalar(string SqlString)
        {
            return this.Pool.Execute(db =>
            {
                if (db.State != ConnectionState.Open) db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                return cmd.ExecuteScalar();
            });
        }
        /// <summary>
        /// 异步返回首行首列
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual Task<object> ExecuteScalarAsync(string SqlString)
        {
            return this.Pool.Execute(db =>
            {
                if (db.State != ConnectionState.Open) db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                return cmd.ExecuteScalarAsync();
            });
        }
        #endregion

        #region 返回一个DbDataReader
        /// <summary>
        /// 返回一个DbDataReader
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <param name="callback">回调方法</param>
        public virtual void ExecuteReader(string SqlString, Action<DbDataReader> callback)
        {
            this.Pool.Execute(db =>
            {
                if (db.State != ConnectionState.Open) db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                callback?.Invoke(cmd.ExecuteReader(CommandBehavior.CloseConnection));
                return true;
            });
        }
        /// <summary>
        /// 返回一个DbDataReader
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <param name="callback">回调方法</param>
        public virtual void ExecuteReaderAsync(string SqlString, Action<DbDataReader> callback)
        {
            callback?.Invoke(this.ExecuteReaderAsync(SqlString).Result);
        }
        /// <summary>
        /// 异步返回一个DbDataReader
        /// </summary>
        /// <param name="SqlString">SQL字符串</param>
        /// <returns></returns>
        public virtual Task<DbDataReader> ExecuteReaderAsync(string SqlString)
        {
            return this.Pool.Execute(db =>
            {
                if (db.State != ConnectionState.Open) db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = SqlString;
                if (this.Config.CommandTimeOut > 0)
                    cmd.CommandTimeout = this.Config.CommandTimeOut;
                return cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            });
        }
        #endregion

        #endregion

        /********************************************************************/
        /*                                                                  */
        /*                        下边是调用储存过程                        */
        /*                                                                  */
        /********************************************************************/

        #region 储存过程方法

        #region 用输入的数据替换到储存过程中
        /// <summary>
        /// 用输入的数据替换到储存过程中
        /// </summary>
        /// <param name="commandText">储存过程</param>
        /// <param name="parameter">SQLParamerter</param>
        /// <returns></returns>
        public string GetParamInfo(string commandText, DbParameter[] parameter)
        {
            StringBuilder paramString = new StringBuilder("");
            string paramSQLString = commandText;
            if (parameter != null && parameter.Length > 0)
                parameter.Each(p =>
                {
                    paramString.AppendLine(p.ParameterName + "[" + p.DbType.ToString() + "," + p.DbType.ToString() + "]" + "=" + p.Value);
                    string c = p.DbType == DbType.String ? "'" : "";
                    paramSQLString = paramSQLString.Replace(p.ParameterName.ToString(), c + p.Value.GetValue() + c);
                });
            return "\r\n储存过程：" + commandText + "\r\n" + paramSQLString + "\r\n" + paramString.ToString();
        }
        #endregion

        #region 创建储存过程参数
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">所属类型</param>
        /// <returns></returns>
        public DbParameter MakeParam(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            DbParameter Param = this.Pool.Factory.CreateParameter();
            Param.ParameterName = name;
            Param.Value = value;
            Param.Direction = direction;
            return Param;
        }
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="direction">参数类型</param>
        /// <returns></returns>
        public DbParameter MakeParam(string name, object value, DbType type, int size = 0, ParameterDirection direction = ParameterDirection.Input)
        {
            try
            {
                DbParameter Param = this.Pool.Factory.CreateParameter();
                Param.ParameterName = name;
                Param.DbType = type;
                if (size != 0) Param.Size = size;
                Param.Direction = direction;
                if (direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput)
                    Param.Value = value;
                return Param;
            }
            catch (DbException e)
            {
                LogHelper.Error(e);
                return null;
            }
        }
        #endregion

        #region 返回一个带储存过程的Command
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="param">Parameter数组</param>
        /// <param name="db">数据库连接</param>
        /// <returns>返回一个Command</returns>
        public DbCommand MakeParamCommand(string commandText, DbParameter[] param, DbConnection db)
        {
            return this.MakeParamCommand(commandText, CommandType.StoredProcedure, param, db);
        }
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">储存过程类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="db">数据库连接</param>
        /// <returns>返回一个Command</returns>
        public DbCommand MakeParamCommand(string commandText, CommandType commandType, DbParameter[] parameter, DbConnection db)
        {
            DbCommand Cmd = this.Pool.Factory.CreateCommand();
            Cmd.Connection = db;
            Cmd.CommandText = commandText;
            if (this.Config.CacheTimeOut > 0) Cmd.CommandTimeout = this.Config.CacheTimeOut;
            Cmd.CommandType = commandType;
            try
            {
                Cmd.Parameters.AddRange(parameter);
                //parameter.Each(p => Cmd.Parameters.Add(p));
            }
            catch (DbException e)
            {
                LogHelper.Error(e);
                return null;
            }
            return Cmd;
        }
        #endregion

        #region 执行储存过程返回执行行数
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回执行行数</returns>
        public virtual int ExecuteNonQuery(string commandText, DbParameter[] parameter)
        {
            return this.ExecuteNonQuery(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回执行行数</returns>
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute(db =>
            {
                int M = -1;
                if (db.State != ConnectionState.Open) db.Open();
                var Cmd = this.MakeParamCommand(commandText, commandType, parameter, db);
                if (this.Config.CommandTimeOut > 0)
                    Cmd.CommandTimeout = this.Config.CommandTimeOut;
                if (this.Config.IsTransaction)
                {
                    using (var Trans = db.BeginTransaction())
                    {
                        try
                        {
                            Cmd.Transaction = Trans;
                            M = Cmd.ExecuteNonQuery();
                            Trans.Commit();
                        }
                        catch (DbException e)
                        {
                            LogHelper.WriteLog(e, this.GetParamInfo(commandText, parameter));
                            Trans.Rollback();
                        }
                    }
                }
                else
                {
                    try { M = Cmd.ExecuteNonQuery(); }
                    catch (DbException e)
                    {
                        LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                    }
                }
                Cmd.Parameters.Clear();
                return M;
            });
        }
        #endregion

        #region 异步执行储存过程返回执行行数
        /// <summary>
        /// 异步执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回执行行数</returns>
        public virtual Task<int> ExecuteNonQueryAsync(string commandText, DbParameter[] parameter)
        {
            return this.ExecuteNonQueryAsync(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 异步执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回执行行数</returns>
        public virtual Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute(db =>
            {
                Task<int> M = null;
                if (db.State != ConnectionState.Open) db.Open();
                var Cmd = this.MakeParamCommand(commandText, commandType, parameter, db);
                if (this.Config.CommandTimeOut > 0)
                    Cmd.CommandTimeout = this.Config.CommandTimeOut;
                if (this.Config.IsTransaction)
                {
                    using (var Trans = db.BeginTransaction())
                    {
                        try
                        {
                            Cmd.Transaction = Trans;
                            M = Cmd.ExecuteNonQueryAsync();
                            Trans.Commit();
                        }
                        catch (DbException e)
                        {
                            LogHelper.WriteLog(e, this.GetParamInfo(commandText, parameter));
                            Trans.Rollback();
                        }
                    }
                }
                else
                {
                    try { M = Cmd.ExecuteNonQueryAsync(); }
                    catch (DbException e)
                    {
                        LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                    }
                }
                Cmd.Parameters.Clear();
                return M;
            });
        }
        #endregion

        #region 执行储存过程返回首行首列
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回首行首列</returns>
        public virtual object ExecuteScalar(string commandText, DbParameter[] parameter)
        {
            return ExecuteScalar(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回首行首列</returns>
        public object ExecuteScalar(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute(db =>
            {
                object data = null;
                try
                {
                    if (db.State != ConnectionState.Open) db.Open();
                    var cmd = this.MakeParamCommand(commandText, commandType, parameter, db);
                    if (this.Config.CommandTimeOut > 0)
                        cmd.CommandTimeout = this.Config.CommandTimeOut;
                    data = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                }
                catch (DbException e)
                {
                    LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                }
                return data;
            });
        }
        #endregion

        #region 执行储存过程返回一个DataTable
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataTable</returns>
        public virtual DataTable ExecuteDataTable(string commandText, DbParameter[] parameter)
        {
            return ExecuteDataTable(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute((db, factory) =>
            {
                DataTable Dt = new DataTable
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture
                };
                if (commandText.IsNullOrEmpty()) return Dt;
                string DataName = commandText.GetMatch(@" from (?<a>[^\s]+)(\s|$)").Trim().TrimStart('[').TrimEnd(']');
                Dt.TableName = DataName.IsNullOrEmpty() ? commandText : DataName;
                try
                {
                    var Cmd = this.MakeParamCommand(commandText, commandType, parameter, db);
                    if (this.Config.CommandTimeOut > 0) Cmd.CommandTimeout = this.Config.CommandTimeOut;
#if NETSTANDARD2_0 || NETFRAMEWORK
                    var sda = factory.CreateDataAdapter();
                    sda.SelectCommand = Cmd;
                    sda.Fill(Dt);
#else

                    if (factory.CanCreateDataAdapter)
                    {
                        var sda = factory.CreateDataAdapter();
                        sda.SelectCommand = Cmd;
                        sda.Fill(Dt);
                    }
                    else
                    {
                        var sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        sdr.GetColumnSchema().Each(c =>
                        {
                            var dc = new DataColumn(c.ColumnName, c.DataType);
                            if (c.IsAutoIncrement.HasValue && c.IsAutoIncrement.Value)
                            {
                                dc.AutoIncrementStep = 1;
                                dc.AutoIncrement = true;
                                dc.AutoIncrementSeed = 1;
                            }
                            if (c.IsUnique.HasValue)
                                dc.Unique = c.IsUnique.Value;
                            if (c.AllowDBNull.HasValue)
                                dc.AllowDBNull = c.AllowDBNull.Value;

                            Dt.Columns.Add(dc);
                        });
                        while (sdr.Read())
                        {
                            var dr = Dt.NewRow();
                            for (var i = 0; i < sdr.FieldCount; i++)
                            {
                                dr[i] = sdr[i].GetValue(Dt.Columns[i].DataType);
                            }
                            Dt.Rows.Add(dr);
                        }
                        sdr.Close();
                        sdr = null;
                    }
#endif     
                    Cmd.Parameters.Clear();
                }
                catch (DbException e)
                {
                    LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                }
                return Dt;
            });
        }
        #endregion

        #region 执行储存过程返回一个List
        /// <summary>
        /// 执行储存过程返回一个List
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个List</returns>
        public virtual List<T> QueryList<T>(string commandText, DbParameter[] parameter)
        {
            return this.QueryList<T>(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回一个List
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个List</returns>
        public virtual List<T> QueryList<T>(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return ExecuteDataTable(commandText, commandType, parameter).ToList<T>();
        }
        /// <summary>
        /// 执行储存过程返回一个T
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个T</returns>
        public virtual T Query<T>(string commandText, DbParameter[] parameter)
        {
            return this.Query<T>(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回一个T
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个T</returns>
        public virtual T Query<T>(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.ExecuteDataTable(commandText, commandType, parameter).ToEntity<T>();
        }
        #endregion

        #region 执行储存过程返回一个DataSet
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataSet</returns>
        public virtual DataSet ExecuteDataSet(string commandText, DbParameter[] parameter)
        {
            return ExecuteDataSet(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute((db, factory) =>
            {
                DataSet Ds = new DataSet
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture
                };
                try
                {
                    var Cmd = MakeParamCommand(commandText, commandType, parameter, db);
                    if (this.Config.CommandTimeOut > 0)
                        Cmd.CommandTimeout = this.Config.CommandTimeOut;
                    var Sda = factory.CreateDataAdapter();
                    Sda.SelectCommand = Cmd;
                    Sda.Fill(Ds);
                    Cmd.Parameters.Clear();
                }
                catch (DbException e)
                {
                    LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                }
                return Ds;
            });
        }
        #endregion

        #region 执行储存过程返回一个DataReader
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns>返回一个DataReader</returns>
        public virtual void ExecuteReader(string commandText, DbParameter[] parameter, Action<DbDataReader> callback)
        {
            this.ExecuteReader(commandText, CommandType.StoredProcedure, parameter, callback);
        }
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns>返回一个DataReader</returns>
        public virtual void ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter, Action<DbDataReader> callback)
        {
            try
            {
                callback?.Invoke(this.ExecuteReader(commandText, commandType, parameter));
            }
            catch (DbException ex)
            {
                LogHelper.Error(ex);
            }
        }
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        public virtual DbDataReader ExecuteReader(string commandText, DbParameter[] parameter)
        {
            return this.ExecuteReader(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        public virtual DbDataReader ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute(db =>
            {
                try
                {
                    if (db.State != ConnectionState.Open) db.Open();
                    var Cmd = this.MakeParamCommand(commandText, commandType, parameter, db);
                    if (this.Config.CommandTimeOut > 0)
                        Cmd.CommandTimeout = this.Config.CommandTimeOut;
                    return Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (DbException e)
                {
                    LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                    return null;
                }
            });
        }
        #endregion

        #region 异步执行储存过程返回一个DataReader
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public virtual void ExecuteReaderAsync(string commandText, DbParameter[] parameter, Action<DbDataReader> callback)
        {
            this.ExecuteReaderAsync(commandText, CommandType.StoredProcedure, parameter, callback);
        }
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public virtual void ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DbDataReader> callback)
        {
            try
            {
                callback?.Invoke(this.ExecuteReaderAsync(commandText, commandType, parameter).Result);
            }
            catch (DbException ex)
            {
                LogHelper.Error(ex);
            }
        }
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, DbParameter[] parameter)
        {
            return this.ExecuteReaderAsync(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 异步执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">CommandType类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <returns>返回一个DataReader</returns>
        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter)
        {
            return this.Pool.Execute(db =>
            {
                try
                {
                    if (db.State != ConnectionState.Open) db.Open();
                    var Cmd = this.MakeParamCommand(commandText, commandType, parameter, db);
                    if (this.Config.CommandTimeOut > 0)
                        Cmd.CommandTimeout = this.Config.CommandTimeOut;
                    return Cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                }
                catch (DbException e)
                {
                    LogHelper.Error(e, this.GetParamInfo(commandText, parameter));
                    return null;
                }
            });
        }
        #endregion

        #endregion
    }
}