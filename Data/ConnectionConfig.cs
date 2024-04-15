using System;
using System.ComponentModel;
using System.Linq;
using XiaoFeng.IO;
using XiaoFeng.Json;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-14 09:32:07                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库配置 Version : 1.0.0
    /// </summary>
    public class ConnectionConfig : EntityBase
    {
        #region 构造器
        /// <summary>
        /// 无参构造器 默认设置为SqlServer数据库驱动
        /// </summary>
        public ConnectionConfig() { this.ProviderType = DbProviderType.SqlServer; this.CacheType = CacheType.No; this.CacheTimeOut = 10 * 60; }
        /// <summary>
        /// 设置连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串或config名称或索引</param>
        /// <param name="providerType">数据库类型</param>
        /// <param name="isTransaction">是否启用事务处理</param>
        /// <param name="commandTimeOut">执行超时时间</param>
        public ConnectionConfig(string connectionString, DbProviderType providerType = DbProviderType.SqlServer, Boolean isTransaction = true, int commandTimeOut = 0)
        {
            this.ProviderType = providerType;
            this.ConnectionString = connectionString;
            this.IsTransaction = isTransaction;
            this.CommandTimeOut = commandTimeOut;
            this.CacheType = CacheType.No;
            this.CacheTimeOut = 10 * 60;
        }
        /// <summary>
        /// 设置连接数据库
        /// </summary>
        /// <param name="connectionStringKey">key</param>
        public ConnectionConfig(string connectionStringKey)
        {
            if (connectionStringKey.IsNullOrEmpty()) return;
            var db = XiaoFeng.Config.DataBase.Current.Data;
            if (db == null) return;
            var conns = db[connectionStringKey];
            if (conns == null || !conns.Any()) return;
            conns.FirstOrDefault().CopyTo(this);
            this.AppKey = connectionStringKey;
        }
        #endregion

        #region 属性
        /// <summary>
        ///驱动类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Description("驱动类型")]
        public DbProviderType ProviderType { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _ConnectionString = string.Empty;
        /// <summary>
        ///数据库连接字符串
        /// </summary>
        [Description("数据库连接字符串")]
        public string ConnectionString
        {
            get
            {
                if (this._ConnectionString.IsNullOrEmpty()) return string.Empty;
                this._ConnectionString = Encrypt.get(this._ConnectionString);
                return this._ConnectionString.IsMatch(@"(\{|\[)\s*(Root|RootPath|/)\s*(\}|\])") ?
                    this._ConnectionString.ReplacePattern(@"(\{|\[)\s*(Root|RootPath|/)\s*(\}|\])", FileHelper.GetCurrentDirectory()) :
                    this._ConnectionString;
            }
            set { this._ConnectionString = value; }
        }
        /// <summary>
        /// 是否启用事务处理
        /// </summary>
        [Description("是否启用事务处理")]
        public Boolean IsTransaction { get; set; }
        /// <summary>
        /// 获取或设置在终止尝试执行命令并生成错误之前的等待时间
        /// </summary>
        [Description("执行超时时间")]
        public int CommandTimeOut { get; set; }
        /// <summary>
        /// 缓存时长 单位为秒 0为永久缓存
        /// </summary>
        [Description("缓存时长 单位为秒 0为永久缓存")]
        public int CacheTimeOut { get; set; }
        /// <summary>
        /// 缓存类型 0不缓存
        /// </summary>
        [Description("缓存类型 Default 默认,No 不缓存,Memory 内存,Disk 磁盘,Redis,Memcache,MongoDB")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CacheType CacheType { get; set; } = 0;
        /// <summary>
        /// 事务级别
        /// </summary>
        [Description("事务级别 DbNull 空级别,Serializable 串行读,RepeatableRead 可重复读,ReadCommitted 提交读,ReadUncommitted 未提交读,Snapshot 隔离未提交读,Chaos 混乱读,Unspecified 未指定")]
        [JsonConverter(typeof(StringEnumConverter))]
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Unspecified;
        /// <summary>
        /// 是否启用连接池
        /// </summary>
        [Description("是否启用连接池")]
        public Boolean IsPool { get; set; } = false;
        /// <summary>
        /// 应用池最大数量
        /// </summary>
        [Description("连接池最大数量")]
        public int MaxPool { get; set; } = 100;
        /// <summary>
        /// 字段名大小写敏感
        /// </summary>
        [Description("字段名大小写敏感")]
        public Boolean IgnoreCase { get; set; } = true;
        /// <summary>
        /// 配置Key
        /// </summary>
        [Description("配置Key")]
        public string AppKey { get; set; }
        /// <summary>
        /// 是否格式化字段
        /// </summary>
        [Description("是否格式化字段")]
        public Boolean IsFormatField { get; set; }
        /// <summary>
        /// 格式化字段字符串
        /// </summary>
        [Description("格式化字段字符串")]
        public string FormatField { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取当前配置的通用数据库操作对象
        /// </summary>
        /// <returns></returns>
        public IDataHelper DataHelper() => new DataHelper(this);
        /// <summary>
        /// 获取当前配置的数据库操作对象
        /// </summary>
        /// <returns></returns>
        public IDbHelper DbHelper()
        {
            switch (this.ProviderType)
            {
                case DbProviderType.SqlServer:
                    return new SqlHelper(this);
                case DbProviderType.SQLite:
                    return new SQLiteHelper(this);
                case DbProviderType.MySql:
                    return new MySqlHelper(this);
                case DbProviderType.Dameng:
                    return new DamengHelper(this);
                default:
                    return null;
            }
        }
        #endregion
    }
}