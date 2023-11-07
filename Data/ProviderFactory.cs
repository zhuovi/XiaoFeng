using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-08-10 11:45:08                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary> 
    /// DbProviderFactory工厂类
    /// </summary> 
    public static class ProviderFactory
    {
        /// <summary>
        /// 驱动集
        /// </summary>
        private static Dictionary<DbProviderType, string> ProviderInvariantNames
        {
            get
            {
                return new Dictionary<DbProviderType, string>()
                {
                    { DbProviderType.SqlServer, "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient" },
                    { DbProviderType.OleDb, "System.Data.OleDb.OleDbFactory,System.Data.OleDb" },
                    { DbProviderType.ODBC, "System.Data.Odbc.OdbcFactory,System.Data.ODBC" },
                    { DbProviderType.Oracle, "Oracle.ManagedDataAccess.Client.OracleClientFactory,Oracle.ManagedDataAccess" },
                    { DbProviderType.MySql, "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data" },/*要装MySql Connector 不然连接不上*/
                    { DbProviderType.SQLite, "System.Data.SQLite.SQLiteFactory,System.Data.SQLite" },
                    { DbProviderType.Firebird, "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory,FirebirdSql.Data.FirebirdClient" },
                    { DbProviderType.PostgreSql, "Npgsql.NpgsqlFactory,Npgsql" },
                    { DbProviderType.DB2, "IBM.Data.DB2.Core.DB2Factory,IBM.Data.DB2.Core" },
                    { DbProviderType.Informix, "IBM.Data.Informix.IfxFactory,IBM.Data.Informix" },
                    { DbProviderType.SqlServerCe, "System.Data.SqlServerCe.SqlCeProviderFactory,System.Data.SqlServerCe" },
                    { DbProviderType.Dameng, "Dm.DmClientFactory,DmProvider" }
                };
            }
        }
        /// <summary>
        /// 驱动集
        /// </summary>
        private readonly static Dictionary<DbProviderType, DbProviderFactory> providerFactoies = new Dictionary<DbProviderType, DbProviderFactory>(20);
        /// <summary>
        /// 独占锁
        /// </summary>
        private readonly static System.Threading.ReaderWriterLockSlim Lock = new System.Threading.ReaderWriterLockSlim();
        /// <summary> 
        /// 获取指定数据库类型对应的程序集名称 
        /// </summary> 
        /// <param name="providerType">数据库类型枚举</param> 
        /// <returns></returns> 
        public static string GetProviderInvariantName(DbProviderType providerType)
        {
            return ProviderInvariantNames[providerType];
        }
        /// <summary> 
        /// 获取指定类型的数据库对应的DbProviderFactory 
        /// </summary> 
        /// <param name="providerType">数据库类型枚举</param> 
        /// <returns></returns> 
        public static DbProviderFactory GetDbProviderFactory(DbProviderType providerType)
        {
            Lock.EnterWriteLock();
            try
            {
                /*如果还没有加载，则加载该DbProviderFactory*/
                if (!providerFactoies.ContainsKey(providerType))
                {
                    providerFactoies.Add(providerType, ImportDbProviderFactory(providerType));
                }
            }
            finally { Lock.ExitWriteLock(); }
            return providerFactoies[providerType];
        }
        /// <summary> 
        /// 加载指定数据库类型的DbProviderFactory 
        /// </summary> 
        /// <param name="providerType">数据库类型枚举</param> 
        /// <returns></returns> 
        private static DbProviderFactory ImportDbProviderFactory(DbProviderType providerType)
        {
            string providerName = ProviderInvariantNames[providerType];
            DbProviderFactory factory = null;
            try
            {
                var type = Type.GetType(providerName);
                if (type == null)
                {
                    if (providerType == DbProviderType.SqlServer)
                    {
                        /*最新驱动*/
                        providerName = "Microsoft.Data.SqlClient.SqlClientFactory,Microsoft.Data.SqlClient";
                        type = Type.GetType(providerName);
                    }
                    else if (providerType == DbProviderType.SQLite)
                    {
                        providerName = "Microsoft.Data.Sqlite.SqliteFactory,Microsoft.Data.Sqlite";
                        type = Type.GetType(providerName);
                    }
                    else if (providerType == DbProviderType.MySql)
                    {
                        providerName = "MySqlConnector.MySqlConnectorFactory,MySqlConnector";
                        type = Type.GetType(providerName);
                    }
                }
                if (type == null)
                {
                    /*没有DLL自动下载*/
                    var dllPath = "http://dll.fayelf.com";
                    dllPath += "/" + providerName.Split(',')[1].Trim() + ".rar";
                    return null;
#if NETSTANDARD

#elif NETCOREAPP3_1

#elif NET5_0

#endif
                }
                FieldInfo field = type.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);

#if NETSTANDARD1_3
                if (field != null && field.FieldType.GetTypeInfo().IsSubclassOf(typeof(DbProviderFactory)))
#elif NETSTANDARD2_0
                    if (field != null && field.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
#endif
                {
                    object value = field.GetValue(null);
                    if (value != null)
                    {
                        factory = (DbProviderFactory)value;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                factory = null;
                LogHelper.Error(ex);
            }
            return factory;
        }
    }
}