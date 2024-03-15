using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Cache;
using XiaoFeng.Collections;
using XiaoFeng.Data.SQL;
using XiaoFeng.Model;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-30 10:17:43                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 通用数据库操作类
    /// 增加 加密连接字符功能
    /// 增加 异步操作数据库功能
    /// Version : V 6.0.1
    /// 增加是否启用连接池
    /// 6.0.1
    /// 增加清空当前缓存
    /// </summary>
    public class DataHelper : EntityBase, IDataHelper
    {
        #region 构造函数

        #region 无参构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelper()
        {
            this.ConnConfig = new ConnectionConfig { ProviderType = DbProviderType.SqlServer, CacheType = CacheType.No, CacheTimeOut = 10 * 60, CommandTimeOut = 0, IsTransaction = true };
        }
        #endregion

#if NETFRAMEWORK
        #region 配置数据库
        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public DataHelper(string connectionString)
            : this()
        {
            /*在这里处理连接串问题 分析出各种字符串*/
            if (connectionString.IsNotNullOrEmpty())
            {
                if (connectionString.IndexOfX("=") > -1)
                    this.ConnConfig.ProviderType = DbProviderType.SqlServer;
                else
                {
                    var app = ConfigurationManager.ConnectionStrings[connectionString];
                    if (app == null)
                    {
                        var apps = ConfigurationManager.AppSettings[connectionString];
                        if (apps.IsNotNullOrEmpty())
                        {
                            this.ConnConfig.ConnectionString = apps;
                            this.ConnConfig.ProviderType = DbProviderType.SqlServer;
                        }
                    }
                    else
                    {
                        if (app.ProviderName.IsNullOrEmpty())
                            this.ConnConfig.ProviderType = DbProviderType.SqlServer;
                        else
                        {
                            var ProviderType = app.ProviderName.GetMatch(@"(SqlServer|MySql|Oracle|Oledb|SQLite|DB2|ODBC|Firebird|PostgreSql|Informix|SqlServerCe|Null)");
                            var pType = ProviderType.ToEnum<DbProviderType>();
                            this.ConnConfig.ProviderType = pType;
                        }
                        if (this.ConnConfig.ConnectionString.IsNotNullOrEmpty())
                            this.ConnConfig.ConnectionString = app.ConnectionString;
                    }
                }
            }
        }
        #endregion
#else
        #region 配置数据库
        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="provider">数据库驱动</param>
        /// <param name="connectionString">数据库连接串</param>
        public DataHelper(DbProviderType provider, string connectionString) : this()
        {
            this.ConnConfig.ProviderType = provider;
            this.ConnConfig.ConnectionString = connectionString;
        }
        #endregion
#endif

        #region 设置数据库配置
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="connectionConfig">数据库配置</param>
        public DataHelper(ConnectionConfig connectionConfig) : this()
        {
            this.ConnConfig = connectionConfig ?? throw new DataHelperException("数据库配置出错.");
        }
        #endregion

        #endregion

        #region 属性
        /// <summary>
        /// 锁
        /// </summary>
        public readonly object Lock = new object();
        /// <summary>
        /// 连接池
        /// </summary>
        private readonly static ConcurrentDictionary<string, ConnectionPool> DataHelperPools = new ConcurrentDictionary<string, ConnectionPool>();
        /// <summary>
        /// 连接池
        /// </summary>
        public ConnectionPool Pool
        {
            get
            {
                return XiaoFeng.Threading.Synchronized.Run(() =>
                {
                    if (DataHelperPools.TryGetValue(this.ConnectionString, out var _Pool))
                        return _Pool;
                    else
                    {
                        var pool = new ConnectionPool(this.ConnConfig);
                        DataHelperPools.TryAdd(this.ConnectionString, pool);
                        return pool;
                    }
                });
            }
        }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public Boolean IsCache
        {
            get { return this.ConnConfig.CacheType != CacheType.No; }
        }
        /// <summary>
        /// 缓存时长
        /// </summary>
        public int CacheTimeOut
        {
            get { return this.ConnConfig.CacheTimeOut; }
            set { this.ConnConfig.CacheTimeOut = value; }
        }
        /// <summary>
        /// 出错信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            get { return this.ConnConfig.ConnectionString; }
            set
            {
                if (value.IsMatch(@"^[a-z0-9_-]+$"))
                {
                    var db = XiaoFeng.Config.DataBase.Current;
                    if (db.Data != null && db.Data.TryGetValue(value, out var val))
                    {
                        this.ConnConfig = val[0];
                    }
                    else
                    {
#if NETFRAMEWORK
                        var config = ConfigurationManager.ConnectionStrings[value];
                        if (config != null)
                            this.ConnConfig.ConnectionString = config.ConnectionString;
                        else
                        {
                            var connString = ConfigurationManager.AppSettings.Get(value);
                            if (connString.IsNotNullOrEmpty()) this.ConnConfig.ConnectionString = connString;
                        }
#endif
                    }
                }
                else
                    this.ConnConfig.ConnectionString = value;
            }
        }
        /// <summary>
        /// 执行命令时超时间
        /// </summary>
        public int CommandTimeOut
        {
            get { return this.ConnConfig.CommandTimeOut; }
            set { this.ConnConfig.CommandTimeOut = value; }
        }
        /// <summary>
        /// 数据驱动
        /// </summary>
        public DbProviderType ProviderType
        {
            get { return this.ConnConfig.ProviderType; }
            set { this.ConnConfig.ProviderType = value; }
        }
        /// <summary>
        /// 是否使用事务处理
        /// </summary>
        public Boolean IsTransaction
        {
            get { return this.ConnConfig.IsTransaction; }
            set { this.ConnConfig.IsTransaction = value; }
        }
        /// <summary>
        /// 驱动工厂
        /// </summary>
        private DbProviderFactory _Provider;
        /// <summary>
        /// 驱动工厂
        /// </summary>
        private DbProviderFactory ProviderFactory
        {
            get
            {
                if (this._Provider == null)
                    this._Provider = Data.ProviderFactory.GetDbProviderFactory(this.ProviderType);
                return this._Provider;
            }
            set { this._Provider = value; }
        }
        /// <summary>
        /// 数据库连接串配置
        /// </summary>
        public ConnectionConfig ConnConfig { get; set; } = new ConnectionConfig();
        #endregion

        #region 创建数据库连接Conn
        /// <summary>
        /// 创建Data数据库连接
        /// </summary>
        /// <returns></returns>
        public virtual DbConnection CreateConn()
        {
            try
            {
                if (this.ConnectionString.IsNullOrEmpty())
                {
                    this.ErrorMessage = "没有设置数据库连接配置.";
                    throw new DataHelperException("请设置数据库连接串.");
                }
                if (this.ProviderFactory == null) return null;
                DbConnection Conn = this.ProviderFactory.CreateConnection();
                Conn.ConnectionString = this.ConnectionString;
                return Conn;
            }
            catch (DbException e)
            {
                this.ErrorMessage = "创建数据库连接失败:" + e.Message;
                LogHelper.Error(e, "\r\n数据库连接字符串:" + this.ConnectionString + "[" + Data.ProviderFactory.GetProviderInvariantName(this.ProviderType) + "]");
                return null;
            }
        }
        #endregion

        #region 获取连接对象
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="fun">方法</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        public T GetConn<T>(Func<DbConnection, DbProviderFactory, T> fun, Boolean isCloseConn = true)
        {
            if (isCloseConn)
            {
                if (this.ConnConfig.IsPool)
                    return this.Pool.Execute(fun);
                else
                {
                    using (var conn = this.CreateConn())
                    {
                        if (conn == null) throw new Exception("驱动或数据库连接串有问题.");
                        var t = fun.Invoke(conn, ProviderFactory);
                        if (conn.State != ConnectionState.Closed) conn.Close();
                        return t;
                    }
                }
            }
            else
                return fun.Invoke(this.CreateConn(), ProviderFactory);
        }
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
        public T Execute<T>(Func<DbCommand, DbProviderFactory, T> fun, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null, Boolean isCloseConn = true)
        {
            return this.GetConn((conn, factory) =>
            {
                var t = default(T);
                if (conn == null) return t;
                using (var tran = new DbTransactionX(conn)
                {
                    IsolationLevel = isolationLevel,
                    IsOpen = isCloseConn && this.ConnConfig.IsTransaction
                })
                {
                    var Cmd = tran.CreateCommand();
                    if (this.CommandTimeOut > 0) Cmd.CommandTimeout = this.CommandTimeOut;
                    try
                    {
                        t = fun.Invoke(Cmd, this.ProviderFactory);
                        tran.Commit();
                    }
                    catch (DbException ex)
                    {
                        string SQL;
                        if (Cmd.Parameters != null && Cmd.Parameters.Count > 0)
                            SQL = Cmd.GetParameterCommandText((DbProviderType.Dameng | DbProviderType.Oracle).HasFlag(this.ProviderType));
                        else
                            SQL = "SQL语句:" + Cmd.CommandText;
                        LogHelper.Error(ex, "\r\n" + SQL);
                        tran.Rollback();
                        error?.Invoke(ex, SQL);
                    }
                    tran.EndTransaction();
                }
                return t;
            }, isCloseConn);
        }
        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="fun">方法体</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        public T Execute<T>(Func<DbCommand, DbProviderFactory, T> fun, Action<Exception, string> error, IsolationLevel isolationLevel, Boolean isCloseConn) => this.Execute(fun, isolationLevel, error, isCloseConn);
        /// <summary>
        /// 执行Command
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fun">方法体</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        public T Execute<T>(Func<DbCommand, T> fun, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null, Boolean isCloseConn = true) => this.Execute((cmd, factory) => fun.Invoke(cmd), isolationLevel, error, isCloseConn);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="fun">方法体</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="isCloseConn">是否关闭连接</param>
        /// <returns></returns>
        public T Execute<T>(Func<DbCommand, T> fun, Action<Exception, string> error, IsolationLevel isolationLevel, Boolean isCloseConn) => this.Execute(fun, isolationLevel, error, isCloseConn);
        #endregion

        #region 执行SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行行数</returns>
        public virtual int ExecuteNonQuery(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return 0;
            var flag = true;
            var M = this.Execute(cmd =>
              {
                  if (cmd == null) return 0;
                  cmd.CommandText = commandText;
                  return cmd.ExecuteNonQuery();
              }, isolationLevel, (e, msg) =>
              {
                  flag = false;
                  error?.Invoke(e, msg);
              });
            return flag ? M : -1;
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回执行行数</returns>
        public virtual int ExecuteNonQuery(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => ExecuteNonQuery(commandText, isolationLevel, error);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行SQL语句以及执行状态</returns>
        public virtual Dictionary<string, int> ExecuteNonQuery(List<string> commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute(cmd =>
            {
                if (cmd == null) return null;
                Dictionary<string, int> dict = new Dictionary<string, int>();
                commandText.Each(sql =>
                {
                    cmd.CommandText = sql;
                    try
                    {
                        var M = cmd.ExecuteNonQuery();
                        dict.Add(sql, M);
                    }
                    catch
                    {
                        dict.Add(sql, 0);
                    }
                });
                return dict;
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回执行SQL语句以及执行状态</returns>
        public virtual Dictionary<string, int> ExecuteNonQuery(List<string> commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteNonQuery(commandText, isolationLevel, error);
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
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<int> callback = null, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return 0;
            var flag = true;
            var M = await this.Execute(async cmd =>
             {
                 if (cmd == null) return 0;
                 cmd.CommandText = commandText;
                 return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

             }, isolationLevel, (e, msg) =>
             {
                 flag = false;
                 error?.Invoke(e, msg);
             });
            return flag ? M : -1;
        }
        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText, Action<int> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteNonQueryAsync(commandText, isolationLevel, callback, error);
        #endregion

        #region 执行SQL语句返回首行首列
        /// <summary>
        /// 执行SQL语句返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回首行首列数据</returns>
        public virtual object ExecuteScalar(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            return this.Execute(cmd =>
            {
                var CacheValue = this.GetCacheValue(ref commandText);
                cmd.CommandText = commandText;
                return cmd.ExecuteScalar();
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行SQL语句返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回首行首列数据</returns>
        public virtual object ExecuteScalar(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteScalar(commandText, isolationLevel, error);
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
        public virtual async Task<object> ExecuteScalarAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<object> callback = null, Action<Exception, string> error = null)
        {
            return await this.Execute(async cmd =>
            {
                var CacheValue = this.GetCacheValue(ref commandText);
                cmd.CommandText = commandText;
                return await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            }, isolationLevel, error);
        }
        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        public virtual async Task<object> ExecuteScalarAsync(string commandText, Action<object> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteScalarAsync(commandText, isolationLevel, callback, error);
        #endregion

        #region 执行SQL语句返回一个DataTable
        /// <summary>
        /// 执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>DataTable</returns>
        public virtual DataTable ExecuteDataTable(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute((cmd, factory) =>
            {
                string DataName = commandText.GetMatch(@" from (?<a>[^\s]+)(\s|$)").Trim().TrimStart('[').TrimEnd(']');
                DataTable Dt = new DataTable
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture,
                    TableName = DataName.IsNullOrEmpty() ? commandText : DataName
                };
                var CacheValue = this.GetCacheValue(ref commandText);
                var flag = false;
                var CacheKey = "";
                if (!CacheValue.IsMatch(@"^(no|not)cache") && (CacheValue.IsMatch(@"^(clear)?cache") || (int)this.ConnConfig.CacheType > 0))
                {
                    CacheKey = this.GetCacheKey(commandText);
                    var val = this.GetCacheData(CacheKey);
                    if (CacheValue.EqualsIgnoreCase("clearcache")) val = null;
                    if (val != null) return (DataTable)val;
                    flag = true;
                }
                cmd.CommandText = commandText;
#if NETSTANDARD2_0
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
                if (flag) this.SetCacheData(CacheKey, Dt);
                return Dt;
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>DataTable</returns>
        public virtual DataTable ExecuteDataTable(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteDataTable(commandText, isolationLevel, error);
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
        public virtual async Task<DataTable> ExecuteDataTableAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<DataTable> callback = null, Action<Exception, string> error = null)
        {
            return await Task.Run(() =>
            {
                var dt = this.ExecuteDataTable(commandText, isolationLevel, error);
                callback?.Invoke(dt);
                return dt;
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 异步执行SQL语句返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        public virtual async Task<DataTable> ExecuteDataTableAsync(string commandText, Action<DataTable> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteDataTableAsync(commandText, isolationLevel, callback, error);
        #endregion

        #region 执行SQL语句返回一个DataSet
        /// <summary>
        /// 执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>DataSet</returns>
        public virtual DataSet ExecuteDataSet(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute((cmd, factory) =>
            {
                DataSet Ds = new DataSet
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture
                };
                var CacheValue = this.GetCacheValue(ref commandText);
                if (CacheValue.ToUpper() == "CACHE" || this.IsCache)
                {
                    var val = this.GetCacheData(commandText);
                    if (val != null) return (DataSet)val;
                }
                cmd.CommandText = commandText;
                var sda = factory.CreateDataAdapter();
                sda.SelectCommand = cmd;
                sda.Fill(Ds);
                if (CacheValue.ToUpper() == "CACHE" || this.IsCache)
                    this.SetCacheData(commandText, Ds);
                return Ds;
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>DataSet</returns>
        public virtual DataSet ExecuteDataSet(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteDataSet(commandText, isolationLevel, error);
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
        public virtual async Task<DataSet> ExecuteDataSetAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<DataSet> callback = null, Action<Exception, string> error = null)
        {
            return await Task.Run(() =>
            {
                var dt = this.ExecuteDataSet(commandText, isolationLevel, error);
                callback?.Invoke(dt);
                return dt;
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 异步执行SQL语句返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        public virtual async Task<DataSet> ExecuteDataSetAsync(string commandText, Action<DataSet> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteDataSetAsync(commandText, isolationLevel, callback, error);
        #endregion

        #region 执行SQL语句返回一个DataReader
        /// <summary>
        /// 执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        public virtual DbDataReader ExecuteReader(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute(cmd =>
            {
                cmd.CommandText = commandText;
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }, isolationLevel, error, false);
        }
        /// <summary>
        /// 执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回一个DataReader</returns>
        public virtual DbDataReader ExecuteReader(string commandText, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteReader(commandText, isolationLevel, error);
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
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string commandText, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<DbDataReader> callback = null, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return await this.Execute(async cmd =>
            {
                cmd.CommandText = commandText;
                return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }, isolationLevel, error, false);
        }
        /// <summary>
        /// 异步执行SQL返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="callback">回调方法</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns></returns>
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string commandText, Action<DbDataReader> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteReaderAsync(commandText, isolationLevel, callback, error);
        #endregion

        /********************************************************************
         *                                                                  *
         *                        下边是调用储存过程                        *
         *                                                                  *
         ********************************************************************/

        #region 用输入的数据替换到储存过程中
        /// <summary>
        /// 用输入的数据替换到储存过程中
        /// </summary>
        /// <param name="parameter">SQLParamerter</param>
        /// <param name="commandText">储存过程</param>
        /// <returns></returns>
        public virtual string GetParamInfo(DbParameter[] parameter, string commandText)
        {
            StringBuilder paramString = new StringBuilder();
            string paramSQLString = commandText;
            if (parameter != null && parameter.Length > 0)
                parameter.Each(p =>
                {
                    paramString.AppendLine($"{p.ParameterName}[{p.DbType}]={p.Value}");
                    string c = (p.DbType == DbType.String || p.DbType == DbType.Date || p.DbType == DbType.DateTime || p.DbType == DbType.DateTime2 || p.DbType == DbType.DateTimeOffset || p.DbType == DbType.Time || p.DbType == DbType.Guid || p.DbType == DbType.StringFixedLength) ? "'" : "";
                    paramSQLString = paramSQLString.ReplacePattern(@"[@:]" + p.ParameterName.TrimStart(new char[] { '@', ':' }) + @"(\s*[,=\);\s]|$)", c + p.Value.GetValue() + c + "$1");
                });
            return $"\r\n储存过程:{commandText}\r\n代入参数的存储过程:{paramSQLString}\r\n存储参数:{paramString}";
        }
        #endregion

        #region 创建储存过程参数
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="parameterDirection">所属类型</param>
        /// <returns></returns>
        public virtual DbParameter MakeParam(string paramName, object paramValue, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            DbParameter Param = this.ProviderFactory?.CreateParameter();
            switch (this.ProviderType)
            {
                case DbProviderType.SqlServer:
                case DbProviderType.SQLite:
                case DbProviderType.MySql:
                    if (!paramName.StartsWith("@"))
                        paramName = "@" + paramName.TrimStart(new char[] { ':' });
                    break;
                case DbProviderType.Oracle:
                    if (!paramName.StartsWith(":"))
                        paramName = ":" + paramName.TrimStart(new char[] { '@' });
                    break;
                case DbProviderType.Dameng:
                    if (paramName.StartsWith(":") || paramName.StartsWith("@"))
                        paramName = paramName.TrimStart(new char[] { ':', '@' });
                    break;
            }
            if (Param == null) return null;
            Param.ParameterName = paramName;
            Param.Value = paramValue ?? "";
            Param.Direction = parameterDirection;
            return Param;
        }
        /// <summary>
        /// 创建储存过程参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="paramType">参数类型</param>
        /// <param name="paramSize">参数大小</param>
        /// <param name="paramDirection">参数类型</param>
        /// <returns></returns>
        public virtual DbParameter MakeParam(string paramName, object paramValue, DbType paramType, int paramSize = 0, ParameterDirection paramDirection = ParameterDirection.Input)
        {
            try
            {
                DbParameter Param = this.ProviderFactory.CreateParameter();
                Param.ParameterName = paramName;
                Param.DbType = paramType;
                if (paramSize != 0) Param.Size = paramSize;
                Param.Direction = paramDirection;
                if (paramDirection == ParameterDirection.Input || paramDirection == ParameterDirection.InputOutput)
                    Param.Value = paramValue ?? "";
                return Param;
            }
            catch (DbException e)
            {
                this.ErrorMessage = "创建储存过程参数出错:" + e.Message;
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
        /// <param name="conn">数据连接对象</param>
        /// <returns>返回一个Command</returns>
        public virtual DbCommand MakeParamCommand(string commandText, DbParameter[] param, DbConnection conn)
        {
            return this.MakeParamCommand(commandText, CommandType.StoredProcedure, param, conn);
        }
        /// <summary>
        /// 返回一个带储存过程的Command
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">储存过程类型</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="conn">数据连接对象</param>
        /// <returns>返回一个Command</returns>
        public virtual DbCommand MakeParamCommand(string commandText, CommandType commandType, DbParameter[] parameter, DbConnection conn)
        {
            DbCommand Cmd = this.ProviderFactory.CreateCommand();
            Cmd.Connection = conn;
            Cmd.CommandText = commandText;
            if (this.CommandTimeOut > 0) Cmd.CommandTimeout = this.CommandTimeOut;
            Cmd.CommandType = commandType;
            try
            {
                //parameter.Each(p => Cmd.Parameters.Add(p));
                Cmd.Parameters.AddRange(parameter);
            }
            catch (DbException e)
            {
                this.ErrorMessage = "把储存过程参数添加到Command中出错:" + e.Message;
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
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回执行行数</returns>
        public virtual int ExecuteNonQuery(string commandText, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return this.ExecuteNonQuery(commandText, CommandType.StoredProcedure, parameter);
        }
        /// <summary>
        /// 执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">解析命令字符串方式</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回执行行数</returns>
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            var flag = true;
            var M = this.Execute(cmd =>
            {
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                var N = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return N;
            }, isolationLevel, (e, msg) =>
            {
                flag = false;
                error?.Invoke(e, msg);
            });
            return flag ? M : -1;
        }
        #endregion

        #region 异步执行储存过程返回执行行数
        /// <summary>
        /// 异步执行储存过程返回执行行数
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调方法</param>
        /// <returns></returns>
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText, DbParameter[] parameter, Action<int> callback = null) => await this.ExecuteNonQueryAsync(commandText, CommandType.StoredProcedure, parameter, callback);
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
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<int> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteNonQueryAsync(commandText, commandType, parameter, isolationLevel, callback, error);
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
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<int> callback = null, Action<Exception, string> error = null)
        {
            var flag = true;
            var M = await this.Execute(async cmd =>
             {
                 cmd.CommandText = commandText;
                 cmd.CommandType = commandType;
                 if (parameter != null && parameter.Length > 0)
                     cmd.Parameters.AddRange(parameter);
                 var N = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                 cmd.Parameters.Clear();
                 return N;
             }, isolationLevel, (e, msg) =>
             {
                 flag = false;
                 error?.Invoke(e, msg);
             });
            return flag ? M : -1;
        }
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
        public virtual object ExecuteScalar(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            return this.Execute(cmd =>
            {
                var CacheValue = this.GetCacheValue(ref commandText);
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                object data = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return data;
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数值</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回首行首列数据</returns>
        public virtual object ExecuteScalar(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteScalar(commandText, commandType, parameter, isolationLevel, error);
        #endregion

        #region 异步执行储存过程返回首行首列
        /// <summary>
        /// 异步执行储存过程返回首行首列
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调方法</param>
        /// <returns></returns>
        public virtual async Task<object> ExecuteScalarAsync(string commandText, DbParameter[] parameter, Action<object> callback = null) => await this.ExecuteScalarAsync(commandText, CommandType.StoredProcedure, parameter, callback);
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
        public virtual async Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<object> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            return await this.Execute(async cmd =>
            {
                var CacheValue = this.GetCacheValue(ref commandText);
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                object data = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                cmd.Parameters.Clear();
                return data;
            }, isolationLevel, error);
        }
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
        public virtual Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<object> callback = null, Action<Exception, string> error = null) => this.ExecuteScalarAsync(commandText, commandType, parameter, callback, isolationLevel, error);
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
        public virtual DataTable ExecuteDataTable(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute((cmd, factory) =>
            {
                string DataName = commandText.GetMatch(@" from (?<a>[^\s]+)(\s|$)").Trim().TrimStart('[').TrimEnd(']');
                if (DataName.IsNullOrEmpty())
                {
                    DataName = commandText.GetMatch(@"^(exec\s+)?(?<a>[a-z0-9_]+)");
                }
                DataTable Dt = new DataTable
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture,
                    TableName = DataName.IsNullOrEmpty() ? commandText : DataName
                };
                var CacheValue = this.GetCacheValue(ref commandText);
                var flag = false;
                var CacheKey = "";
                if (!CacheValue.IsMatch(@"^(no|not)cache") && (CacheValue.IsMatch(@"^(clear)?cache") || (int)this.ConnConfig.CacheType > 0))
                {
                    CacheKey = this.GetCacheKey(commandText, parameter);
                    var val = this.GetCacheData(CacheKey);
                    if (CacheValue.EqualsIgnoreCase("clearcache")) val = null;
                    if (val != null) return (DataTable)val;
                    flag = true;
                }
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);

#if NETSTANDARD2_0
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
                if (flag) this.SetCacheData(CacheKey, Dt);
                cmd.Parameters.Clear();
                return Dt;
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数数组</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回一个DataTable</returns>
        public virtual DataTable ExecuteDataTable(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteDataTable(commandText, commandType, parameter, isolationLevel, error);
        /// <summary>
        /// 执行储存过程返回一个DataTable
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataTable</returns>
        public virtual DataTable ExecuteDataTable(string commandText, DbParameter[] parameter, Action<Exception, string> error = null) => this.ExecuteDataTable(commandText, CommandType.StoredProcedure, parameter, error);
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
        public virtual async Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DataTable> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            return await Task.Run(() =>
            {
                var dt = this.ExecuteDataTable(commandText, commandType, parameter, isolationLevel, error);
                callback?.Invoke(dt);
                return dt;
            }).ConfigureAwait(false);
        }
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
        public virtual async Task<DataTable> ExecuteDataTableAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<DataTable> callback = null, Action<Exception, string> error = null) => await this.ExecuteDataTableAsync(commandText, commandType, parameter, callback, isolationLevel, error);
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
        public virtual DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute((cmd, factory) =>
            {
                DataSet Ds = new DataSet
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture
                };
                var CacheValue = this.GetCacheValue(ref commandText);
                if (CacheValue.ToUpper() == "CACHE" || this.IsCache)
                {
                    var val = this.GetCacheData(commandText);
                    if (val != null) return (DataSet)val;
                }
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                var sda = factory.CreateDataAdapter();
                sda.SelectCommand = cmd;
                sda.Fill(Ds);
                if (CacheValue.ToUpper() == "CACHE" || this.IsCache)
                    this.SetCacheData(commandText, Ds);
                cmd.Parameters.Clear();
                return Ds;
            }, isolationLevel, error);
        }
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数集合</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>DataSet</returns>
        public virtual DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteDataSet(commandText, commandType, parameter, isolationLevel, error);
        /// <summary>
        /// 执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataSet</returns>
        public virtual DataSet ExecuteDataSet(string commandText, DbParameter[] parameter, Action<Exception, string> error = null) => this.ExecuteDataSet(commandText, CommandType.StoredProcedure, parameter, error);
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
        public virtual async Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DataSet> callback = null, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            return await Task.Run(() =>
            {
                var ds = this.ExecuteDataSet(commandText, commandType, parameter, isolationLevel, error);
                callback?.Invoke(ds);
                return ds;
            }).ConfigureAwait(false);
        }
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
        public virtual async Task<DataSet> ExecuteDataSetAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<DataSet> callback = null, Action<Exception, string> error = null) => await this.ExecuteDataSetAsync(commandText, commandType, parameter, callback, isolationLevel, error);
        /// <summary>
        /// 异步执行储存过程返回一个DataSet
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callbak">回调</param>
        /// <returns>返回一个DataSet</returns>
        public virtual async Task<DataSet> ExecuteDataSetAsync(string commandText, DbParameter[] parameter, Action<DataSet> callbak = null) => await this.ExecuteDataSetAsync(commandText, CommandType.StoredProcedure, parameter, callbak);
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
        public virtual DbDataReader ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return this.Execute(cmd =>
            {
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                var sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return sdr;
            }, isolationLevel, error, false);
        }
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameter">参数组</param>
        /// <param name="error">错误回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <returns>返回一个DataReader</returns>
        public virtual DbDataReader ExecuteReader(string commandText, CommandType commandType, DbParameter[] parameter, Action<Exception, string> error, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => this.ExecuteReader(commandText, commandType, parameter, isolationLevel, error);
        /// <summary>
        /// 执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        public virtual DbDataReader ExecuteReader(string commandText, DbParameter[] parameter, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => this.ExecuteReader(commandText, CommandType.StoredProcedure, parameter, isolationLevel, error);
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
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter, Action<DbDataReader> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null)
        {
            if (commandText.IsNullOrEmpty()) return null;
            return await this.Execute(async cmd =>
            {
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                var sdr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);
                cmd.Parameters.Clear();
                return sdr;
            }, isolationLevel, error, false);
        }
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
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType, DbParameter[] parameter, IsolationLevel isolationLevel, Action<DbDataReader> callback = null, Action<Exception, string> error = null) => await this.ExecuteReaderAsync(commandText, commandType, parameter, callback, isolationLevel, error);
        /// <summary>
        /// 异步执行储存过程返回一个DataReader
        /// </summary>
        /// <param name="commandText">SQL语句或储存过程名称</param>
        /// <param name="parameter">Parameter数组</param>
        /// <param name="callback">回调</param>
        /// <param name="isolationLevel">事务级别</param>
        /// <param name="error">错误回调</param>
        /// <returns>返回一个DataReader</returns>
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string commandText, DbParameter[] parameter, Action<DbDataReader> callback, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, Action<Exception, string> error = null) => await this.ExecuteReaderAsync(commandText, CommandType.StoredProcedure, parameter, callback, isolationLevel, error);
        #endregion

        /********************************************************************
         *                                                                  *
         *                       下边是数据库相关操作                       *
         *                                                                  *
         ********************************************************************/

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
        public virtual DataTable Select(string tableName, string Columns, string Condition, string OrderColumnName, string OrderType, int PageIndex, int PageSize, out int PageCount, out int Counts, string PrimaryKey = "")
        {
            IDbHelper data = this.ConnConfig.DbHelper();
            PageCount = 0; Counts = 0;
            if (data == null) return null;
            return data.Select(tableName, Columns, Condition, OrderColumnName, OrderType, PageIndex, PageSize, out PageCount, out Counts, PrimaryKey);
        }
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
        public virtual List<T> Select<T>(string tableName, string Columns, string Condition, string OrderColumnName, string OrderType, int PageIndex, int PageSize, out int PageCount, out int Counts, string PrimaryKey = "")
        {
            return this.Select(tableName, Columns, Condition, OrderColumnName, OrderType, PageIndex, PageSize, out PageCount, out Counts, PrimaryKey).ToList<T>();
        }
        #endregion

        #region 获取数据库操作对象
        /// <summary>
        /// 获取数据库操作对象
        /// </summary>
        /// <returns></returns>
        public IDbHelper GetData() => this.ConnConfig.DbHelper();
        #endregion

        #region 缓存操作
        /// <summary>
        /// 获取CacheKey
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">参数集</param>
        /// <returns></returns>
        public virtual string GetCacheKey(string commandText, DbParameter[] parameter = null)
        {
            var Paramers = "";
            if (parameter != null && parameter.Length > 0)
                parameter.Each(p => Paramers += p.ParameterName + "=" + p.Value + ",");
            return new { Config = this.ConnectionString, SQLString = commandText, Parameter = Paramers.TrimEnd(',') }.ToJson().MD5();
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="data">数据</param>
        public virtual void SetCacheData(string key, object data)
        {
            var cache = CacheFactory.Create(this.ConnConfig.CacheType);
            if (this.CacheTimeOut < 0)
            {
                cache.Remove(key);
                CacheHelper.Remove("HitCount-" + key);
            }
            else if (this.CacheTimeOut == 0)
            {
                cache.Set(key, data);
                CacheHelper.Set("HitCount-" + key, 0);
            }
            else
            {
                cache.Set(key, data, this.CacheTimeOut * 1000);
                CacheHelper.Set("HitCount-" + key, 0, this.CacheTimeOut * 1000);
            }
            /*
            DateTime dateTime;
            if (this.CacheTimeOut == 0)
                dateTime = DateTime.MaxValue;
            else if (this.CacheTimeOut > 0)
                dateTime = DateTime.UtcNow.AddSeconds(this.CacheTimeOut);
            else
                dateTime = DateTime.UtcNow.AddSeconds(10 * 60);
            switch (this.ConnConfig.CacheType)
            {
                case CacheType.Memory:
                    CacheHelper.Set(key, data, dateTime);
                    break;
                case CacheType.Disk:
                    var setting = Config.Setting.Current;
                    var CachePath = (setting.GetCachePath.Replace("\\", "/").TrimStart('/') + @"/" + key + ".fay").GetBasePath();
                    //FileHelper.DeleteFile(CachePath);
                    string content = "/*{1}*CacheTime:{0}{1}*CacheObjectType:{2}{1}*\/{1}{3}".format(dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), Environment.NewLine, data.GetType().AssemblyQualifiedName, data.ToJson());
                    FileHelper.WriteText(CachePath, content, Encoding.UTF8);
                    break;
                default: return;
            }
            CacheHelper.Set("HitCount-" + key, 0, dateTime);*/
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public virtual object GetCacheData(string key)
        {
            object data = CacheFactory.Create(this.ConnConfig.CacheType).Get(key);
            /*switch (this.ConnConfig.CacheType)
            {
                case CacheType.Memory:
                    data = CacheHelper.Get(key);
                    break;
                case CacheType.Disk:
                    var setting = Config.Setting.Current;
                    var CachePath = (setting.GetCachePath.Replace("\\", "/").TrimStart('/') + @"/" + key + ".fay").GetBasePath();
                    if (FileHelper.Exists(CachePath))
                    {
                        string content = FileHelper.OpenText(CachePath, Encoding.UTF8);
                        var CacheHeader = content.GetMatch(@"^/\*[\s\S]+?\*\/\r\n");
                        var CacheBody = content.RemovePattern(@"^\/\*[\s\S]+?\*\/\r\n");
                        var CacheTime = CacheHeader.GetMatch(@"\*CacheTime:(.*?)\r\n").ToCast<DateTime>();
                        if (CacheTime <= DateTime.Now)
                            FileHelper.DeleteFile(CachePath);
                        else
                        {
                            var CacheObjectType = CacheHeader.GetMatch(@"CacheObjectType:(.*?)\r\n");
                            var ObjectType = Type.GetType(CacheObjectType);
                            data = CacheBody.JsonToObject(ObjectType);
                        }
                    }
                    break;
            }*/
            if (data != null)
            {
                object count = CacheHelper.Get("HitCount-" + key);
                long HitCacheCount = count == null ? 1 : (count.ToCast<long>() + 1);
                if (HitCacheCount > 0)
                    CacheHelper.Set("HitCount-" + key, HitCacheCount);
            }
            else
                CacheHelper.Remove("HitCount-" + key);
            return data;
        }
        /// <summary>
        /// 获取SQL语句中的缓存关键字
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <returns></returns>
        public virtual string GetCacheValue(ref string commandText)
        {
            string CacheValue = "";
            if (commandText.IsMatch(@";?\s*(no|not|clear)?\s*cache(\[\d+\])?\s*;?\s*$"))
            {
                CacheValue = commandText.GetMatch(@";?\s*(?<a>(no|not|clear)?\s*cache(\[\d+\])?)\s*;?\s*$");
                commandText = commandText.ReplacePattern(@"(;?)\s*(no|not|clear)?\s*cache(\[\d+\])?\s*;?\s*$", "$1");
                CacheValue = CacheValue.Trim();
            }
            return CacheValue;
        }
        #endregion

        /********************************************************************
         *                                                                  *
         *                              公用对象                            *
         *                                                                  *
         ********************************************************************/

        #region 获取对象
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="SQL">SQL语句</param>
        /// <returns></returns>
        public T Query<T>(string SQL)
        {
            if (SQL.IsNullOrEmpty()) return default(T);
            ValueTypes types = typeof(T).GetValueType();
            T model = default(T);
            if (types == ValueTypes.Enum || types == ValueTypes.String || types == ValueTypes.Value)
            {
                object val = this.ExecuteScalar(SQL);
                model = val.ToCast<T>();
            }
            else if (types == ValueTypes.Class || types == ValueTypes.Struct)
            {
                model = this.ExecuteDataTable(SQL).ToEntity<T>();
            }
            return model;
        }
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="SQL">SQL语句</param>
        /// <returns></returns>
        public List<T> QueryList<T>(string SQL)
        {
            if (SQL.IsNullOrEmpty()) return new List<T>();
            return this.ExecuteDataTable(SQL).ToList<T>();
        }
        #endregion

        #region 转换成 IQueryableX
        /// <summary>
        /// 转换成 IQueryableX
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <returns></returns>
        public IQueryableX<T> New<T>() where T : Entity<T>, new() => new DataHelperX<T>(this.ConnConfig);
        #endregion

        #region 回收资源
        /// <summary>
        /// 回收资源
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="disposing">是否释放</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing, () =>
            {

            });
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        ~DataHelper()
        {
            this.Dispose(false);
        }
        #endregion
    }
}