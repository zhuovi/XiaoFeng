using System;
using System.Collections.Generic;
/****************************************************************
*  Copyright © (2015) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 15:04:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库对应C#类型
    /// Verstion : 2.0.0
    /// Description:
    /// v 2.0.0
    /// 增加了 MySql Oracle对应类型
    /// </summary>
    public class DataType
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataType() { this.ProviderType = DbProviderType.SqlServer; }
        /// <summary>
        /// 设置数据库驱动
        /// </summary>
        /// <param name="providerType">数据库驱动</param>
        public DataType(DbProviderType providerType) { this.ProviderType = providerType; }
        #endregion

        #region 属性
        /// <summary>
        /// 数据库驱动类型
        /// </summary>
        public DbProviderType ProviderType { get; set; }
        /// <summary>
        /// 获取相关数据库类型
        /// </summary>
        /// <param name="dataType">C#数据类型</param>
        /// <returns></returns>
        public object this[Type dataType]
        {
            get { return this[dataType.Name]; }
        }
        /// <summary>
        /// 获取相关数据库类型
        /// </summary>
        /// <param name="dataType">C#数据类型</param>
        /// <returns></returns>
        public object this[string dataType]
        {
            get
            {
                if (dataType == null) return "varchar";
                if (dataType.IsMatch(@"\[(?<a>[\s\S]*?)\]"))
                    dataType = dataType.GetMatch(@"\[(?<a>[\s\S]*?)\]");
                dataType = dataType.ReplacePattern(@"^System\.", "");
                object _ = null;
                switch (this.ProviderType)
                {
                    case DbProviderType.SqlServer:
                        _ = this.SQLServer.ContainsKey(dataType) ? this.SQLServer[dataType] : "varchar";
                        break;
                    case DbProviderType.SQLite:
                        _ = this.SQLite.ContainsKey(dataType) ? this.SQLite[dataType] : "VARCHAR";
                        break;
                    case DbProviderType.MySql:
                        _ = this.MySql.ContainsKey(dataType) ? this.MySql[dataType] : "VARCHAR";
                        break;
                    case DbProviderType.Oracle:
                        _ = this.Oracle.ContainsKey(dataType) ? this.Oracle[dataType] : "VARCHAR";
                        break;
                    case DbProviderType.Dameng:
                        _ = this.Dameng.ContainsKey(dataType) ? this.Dameng[dataType] : "varchar";
                        break;
                    default:
                        _ = "varchar";
                        break;
                }
                return _;
            }
        }

        #region SQLServer 类型
        /// <summary>
        /// SQLServer 类型
        /// </summary>
        private Dictionary<string, string> _SQLServer;
        /// <summary>
        /// SQLServer 类型
        /// </summary>
        public Dictionary<string, string> SQLServer
        {
            get
            {
                if (this._SQLServer == null)
                {
                    this._SQLServer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        {"Guid","uniqueidentifier"},
                        {"string","varchar"},
                        {"Int16","smallint"},
                        {"int","int"},
                        {"Int32","int"},
                        {"Int64","bigint"},
                        {"UInt16","smallint"},
                        {"uint","int"},
                        {"UInt32","int"},
                        {"UInt64","bigint"},
                        {"Binary","varbinary"},
                        {"Boolean","bit"},
                        {"Bool","bit"},
                        {"DateTime","datetime"},
                        {"DateOnly","date" },
                        {"TimeOnly","time" },
                        {"Decimal","decimal"},
                        {"Double","float"},
                        {"Single","real"},
                        {"Object","sql_variant"},
                        {"Byte","tinyint"},
                        {"SByte","tinyint"}
                    };
                }
                return this._SQLServer;
            }
        }
        /// <summary>
        /// SQLServer 类型
        /// </summary>
        private Dictionary<string, string> _SQLServerToDotNet;
        /// <summary>
        /// SQLServer 类型
        /// </summary>
        public Dictionary<string, string> SQLServerToDotNet
        {
            get
            {
                if (this._SQLServerToDotNet == null)
                {
                    this._SQLServerToDotNet = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        {"uniqueidentifier","Guid"},
                        {"varchar","string"},
                        {"smallint","Int16"},
                        {"int","int"},
                        {"bigint","Int64"},
                        {"varbinary","Binary"},
                        {"bit","Boolean"},
                        {"datetime","DateTime"},
                        {"date","DateOnly"},
                        {"time","TimeOnly"},
                        {"decimal","Decimal"},
                        {"float","Double"},
                        {"real","Single"},
                        {"sql_variant","Object"},
                        {"tinyint","Byte"}
                    };
                }
                return this._SQLServerToDotNet;
            }
        }
        #endregion

        #region SQLite 类型
        /// <summary>
        /// SQLite 类型
        /// </summary>
        private Dictionary<string, string> _SQLite;
        /// <summary>
        /// SQLite 类型
        /// </summary>
        public Dictionary<string, string> SQLite
        {
            get
            {
                if (this._SQLite == null)
                {
                    this._SQLite = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"int","INTEGER"},
                        {"Int32","INT"},
                        {"Int64","BIGINT"},
                        {"uint","INTEGER"},
                        {"UInt32","INT"},
                        {"UInt64","BIGINT"},
                        {"Byte[]","BLOB"},
                        {"Boolean","BOOLEAN"},
                        {"String","VARCHAR"},
                        {"DateTime","DATETIME"},
                        {"DateOnly","DATE" },
                        {"TimeOnly","TIME" },
                        {"Decimal","DECIMAL"},
                        {"Double","DOUBLE"},
                        {"Float","DOUBLE"},
                        {"Object","NONE"}
                    };
                }
                return this._SQLite;
            }
        }
        /// <summary>
        /// SQLite 类型
        /// </summary>
        private Dictionary<string, string> _SQLiteToDotNet;
        /// <summary>
        /// SQLite 类型
        /// </summary>
        public Dictionary<string, string> SQLiteToDotNet
        {
            get
            {
                if (this._SQLiteToDotNet == null)
                {
                    this._SQLiteToDotNet = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"INTEGER","int"},
                        {"INT","Int32"},
                        {"BIGINT","Int64"},
                        {"BLOB","Byte[]"},
                        {"BOOLEAN","Boolean"},
                        {"VARCHAR","String"},
                        {"STRING","String"},
                        {"TEXT","String"},
                        {"DATETIME","DateTime"},
                        {"DATE","DateOnly"},
                        {"TIME","TimeOnly"},
                        {"DECIMAL","Decimal"},
                        {"DOUBLE","Double"},
                        {"NUMERIC","Double"},
                        {"NONE","Object"},
                        {"CHAR","Char"},
                    };
                }
                return this._SQLiteToDotNet;
            }
        }
        #endregion

        #region MySql 类型
        /// <summary>
        /// MySql
        /// </summary>
        private Dictionary<string, string> _MySql = null;
        /// <summary>
        /// MySql
        /// </summary>
        public Dictionary<string, string> MySql
        {
            get
            {
                if (this._MySql == null)
                {
                    this._MySql = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        { "Boolean","BIT"},
                        { "bit","BIT"},
                        { "sbyte","INT" },
                        { "byte","INT" },
                        { "Int16","SMALLINT" },
                        { "smallint","SMALLINT" },
                        { "Int32","INT" },
                        { "Int64","BIGINT" },
                        { "Single","FLOAT" },
                        { "Double","DOUBLE" },
                        { "Decimal","DECIMAL" },
                        { "String","VARCHAR" },
                        { "DateTime","DATETIME" },
                        { "DateOnly","DATE" },
                        { "TimeOnly","TIME" }
                    };
                }
                return this._MySql;
            }
        }
        /// <summary>
        /// MySql
        /// </summary>
        private Dictionary<string, string> _MySqlToDotNet = null;
        /// <summary>
        /// MySql
        /// </summary>
        public Dictionary<string, string> MySqlToDotNet
        {
            get
            {
                if (this._MySqlToDotNet == null)
                {
                    this._MySqlToDotNet = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        { "BOOL","Boolean"},
                        { "BOOLEAN","Boolean"},
                        { "Bit","Boolean"},
                        { "TINYINT","byte" },
                        { "SMALLINT","Int16" },
                        { "INT","Int32" },
                        { "BIGINT","Int64" },
                        { "FLOAT","Single" },
                        { "DOUBLE","Double" },
                        { "NUMERIC","Decimal" },
                        { "DECIMAL","Decimal" },
                        { "VARCHAR","String" },
                        { "TEXT","String" },
                        { "JSON","String" },
                        { "LONGTEXT","String" },
                        { "DATETIME","DateTime" },
                        { "DATE","DateOnly"},
                        { "TIME","TimeOnly"},
                        { "TimeStamp","DateTime" }
                    };
                }
                return this._MySqlToDotNet;
            }
        }
        #endregion

        #region Oracle 类型
        /// <summary>
        /// Oracle
        /// </summary>
        private Dictionary<string, string> _Oracle = null;
        /// <summary>
        /// Oracle
        /// </summary>
        public Dictionary<string, string> Oracle
        {
            get
            {
                if (this._Oracle == null)
                {
                    this._Oracle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        { "byte[]","BFILE"},
                        { "Decimal","FLOAT" },
                        { "Double","FLOAT" },
                        { "Float","FLOAT" },
                        { "Int16","INTERVAL" },
                        { "Int","INTERVAL" },
                        { "Int32","INTERVAL" },
                        { "Int64","INTERVAL" },
                        { "TimeSpan","INTERVAL" },
                        { "String","VARCHAR" },
                        { "DateTime","TIMESTAMP" },
                        { "DateOnly","DATE" },
                        { "TimeOnly","TIME" }
                    };
                }
                return this._Oracle;
            }
        }
        /// <summary>
        /// Oracle
        /// </summary>
        private Dictionary<string, string> _OracleToDotNet = null;
        /// <summary>
        /// Oracle
        /// </summary>
        public Dictionary<string, string> OracleToDotNet
        {
            get
            {
                if (this._OracleToDotNet == null)
                {
                    this._OracleToDotNet = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        { "BFILE","byte[]"},
                        { "FLOAT","Double" },
                        { "INTERVAL","Int64" },
                        { "VARCHAR","String" },
                        { "NUMBER","Decimal" },
                        { "TIMESTAMP","DateTime" },
                        { "DATE","DateOnly"},
                        { "TIME","TimeOnly"},
                    };
                }
                return this._OracleToDotNet;
            }
        }
        #endregion

        #region 达梦 类型
        /// <summary>
        /// 达梦
        /// </summary>
        private Dictionary<string, string> _Dameng = null;
        /// <summary>
        /// 达梦
        /// </summary>
        public Dictionary<string, string> Dameng
        {
            get
            {
                if (this._Dameng == null)
                {
                    this._Dameng = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        { "byte[]","BFILE"},
                        { "Decimal","DECIMAL" },
                        { "Double","DOUBLE" },
                        { "Float","FLOAT" },
                        { "Int16","INT" },
                        { "Int","INT" },
                        { "Int32","INT" },
                        { "Int64","BIGINT" },
                        { "TimeSpan","BIGINT" },
                        { "String","VARCHAR" },
                        { "DateTime","TIMESTAMP" },
                        { "DateOnly","DATE" },
                        { "TimeOnly","TIME" }
                    };
                }
                return this._Dameng;
            }
        }
        /// <summary>
        /// 达梦
        /// </summary>
        private Dictionary<string, string> _DamengToDotNet = null;
        /// <summary>
        /// 达梦
        /// </summary>
        public Dictionary<string, string> DamengToDotNet
        {
            get
            {
                if (this._DamengToDotNet == null)
                {
                    this._DamengToDotNet = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                        { "CHAR","char" },
                        { "CHARCTER","char" },
                        { "VARCHAR","String" },
                        { "VARCHAR2","String" },
                        { "NUMBERIC","Decimal" },
                        { "DECIMAL","Decimal" },
                        { "DEC","Decimal" },
                        { "INTEGER","int" },
                        { "INT","int" },
                        { "BIGINT","long" },
                        { "TINYINT","byte" },
                        { "BYTE","byte" },
                        { "SAMLLINT","int" },
                        { "BINARY","byte" },
                        { "VARBINARY","byte" },
                        { "FLOAT","Double" },
                        { "DOUBLE","Double" },
                        { "REAL","int" },
                        { "DOUBLE PRECISION","Double" },
                        { "BIT","Boolean"},
                        { "DATE","DateTime"},
                        { "TIME","DateTime"},
                        { "TIMESTAMP","DateTime"},
                        { "DATETIME","DateTime"},
                        { "TIME WITH TIME ZONE","DateTime"},
                        { "DATETIME WITH TIME ZONE","DateTime"},
                        { "TIMESTAMP WITH LOCAL TIME ZONE","DateTime"},
                        { "INTERVAL YEAR","String"},
                        { "INTERVAL YEAR TO MONTH","String"},
                        { "INTERVAL MONTH","String"},
                        { "INTERVAL DAY","String"},
                        { "INTERVAL DAY TO HOUR","String"},
                        { "INTERVAL DAY TO MINUTE","String"},
                        { "INTERVAL DAY TO SECOND","String"},
                        { "INTERVAL HOUR","String"},
                        { "INTERVAL HOUR TO MINUTE","String"},
                        { "INTERVAL HOUR TO SECOND","String"},
                        { "INTERVAL MINUTE","String"},
                        { "INTERVAL MINUTE TO SECOND","String"},
                        { "TEXT","String"},
                        { "LONG","String"},
                        { "LONGVARCHAR","String"},
                        { "IMAGE","byte[]"},
                        { "LONGVARBINARY","byte[]"},
                        { "BLOB","byte[]"},
                        { "CLOB","String"},
                        { "BFILE","byte[]"}
                    };
                }
                return this._DamengToDotNet;
            }
        }
        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 获取DotNet类型
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public string GetDotNetType(string dbType)
        {
            if (dbType.IsNullOrEmpty()) return "string";
            if (dbType.IsMatch(@"\[(?<a>[\s\S]*?)\]"))
                dbType = dbType.GetMatch(@"\[(?<a>[\s\S]*?)\]");
            string _ = "";
            switch (this.ProviderType)
            {
                case DbProviderType.SqlServer:
                    _ = this.SQLServerToDotNet.ContainsKey(dbType) ? this.SQLServerToDotNet[dbType] : "string";
                    break;
                case DbProviderType.SQLite:
                    _ = this.SQLiteToDotNet.ContainsKey(dbType) ? this.SQLiteToDotNet[dbType] : "string";
                    break;
                case DbProviderType.MySql:
                    _ = this.MySqlToDotNet.ContainsKey(dbType) ? this.MySqlToDotNet[dbType] : "string";
                    break;
                case DbProviderType.Oracle:
                    _ = this.OracleToDotNet.ContainsKey(dbType) ? this.OracleToDotNet[dbType] : "string";
                    break;
                case DbProviderType.Dameng:
                    _ = this.DamengToDotNet.ContainsKey(dbType) ? this.DamengToDotNet[dbType] : "string";
                    break;
                default:
                    _ = "string";
                    break;
            }
            return _;
        }
        #endregion

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~DataType() { }
        #endregion
    }
}