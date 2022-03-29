using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Data
{
    #region 数据库类型枚举
    /// <summary> 
    /// 数据库类型枚举 
    /// </summary> 
    [Flags]
    public enum DbProviderType : int
    {
        /// <summary>
        /// SqlClient Data Provider
        /// </summary>
        [Description("SqlServer")]
        SqlServer = 1 << 0,
        /// <summary>
        /// OleDb Data Provider
        /// </summary>
        [Description("Oledb")]
        OleDb = 1 << 1,
        /// <summary>
        /// MySql Data Provider
        /// </summary>
        [Description("MySql")]
        MySql = 1 << 2,
        /// <summary>
        /// SQLite Data Provider
        /// </summary>
        [Description("SQLite")]
        SQLite = 1 << 3,
        /// <summary>
        /// OracleClient Data Provider
        /// </summary>
        [Description("Oracle")]
        Oracle = 1 << 4,
        /// <summary>
        /// Odbc Data Provider
        /// </summary>
        [Description("ODBC")]
        ODBC = 1 << 5,
        /// <summary>
        /// Firebird Data Provider
        /// </summary>
        [Description("Firebird")]
        Firebird = 1 << 6,
        /// <summary>
        /// PostgreSql Data Provider
        /// </summary>
        [Description("PostgreSql")]
        PostgreSql = 1 << 7,
        /// <summary>
        /// IBM DB2 Data Provider
        /// </summary>
        [Description("DB2")]
        DB2 = 1 << 8,
        /// <summary>
        /// IBM Informix Data Provider
        /// </summary>
        [Description("Informix")]
        Informix = 1 << 9,
        /// <summary>
        /// Microsoft SQL Server Compact Data Provider 4.0
        /// </summary>
        [Description("SqlServerCe")]
        SqlServerCe = 1 << 10,
        /// <summary>
        /// Redis
        /// </summary>
        [Description("Redis")]
        Redis = 1 << 11,
        /// <summary>
        /// 达梦
        /// </summary>
        [Description("达梦")]
        Dameng = 1 << 12,
        /// <summary>
        /// 人大金仓
        /// </summary>
        [Description("人大金仓")]
        Kingbase = 1 << 13,
        /// <summary>
        /// 神州通用
        /// </summary>
        [Description("神州通用")] 
        Shentong = 1<<14,
        /// <summary>
        /// 瀚高
        /// </summary>
        [Description("瀚高")] 
        Highgo = 1<<15,
    }
    #endregion
}