using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Data;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-16 14:20:32                            *
*  Version : v 1.0.1                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcached 连接配置
    /// </summary>
    public class MemcachedConfig : ConnectionConfig
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedConfig()
        {
            this.ProviderType = DbProviderType.Redis;
        }
        /// <summary>
        /// 配置数据库连接串
        /// </summary>
        /// <param name="config">配置</param>
        public MemcachedConfig(ConnectionConfig config) : this(config.ConnectionString)
        {
            this.IsPool = config.IsPool;
            this.MaxPool = config.MaxPool;
        }
        /// <summary>
        /// 配置数据库连接串 redis://7092734@127.0.0.1:6379/0?配置
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        public MemcachedConfig(string connectionString) : this()
        {
            if (connectionString.IsNullOrEmpty()) return;
            if (connectionString.IsMatch(@"^memcached://([a-z0-9]+@)?[^:/]+(:\d+)?(\/\d+)?"))
            {
                var dict = connectionString.GetMatchs(@"^memcached://((?<user>[a-z0-9]+):(?<pwd>[a-z0-9]+)@)?(?<host>[^:/]+)(:(?<port>\d+))?(\/(?<db>\d+))?(\/?\?(?<option>[\s\S]*))?$");
                this.Host = dict["host"];
                this.Port = dict["port"].ToCast(11211);
                this.DbNum = dict["db"].ToCast<int>();
                this.User = dict["user"];
                this.Password = dict["pwd"];
                var option = dict["option"];
                if (option.IsNotNullOrEmpty())
                {
                    var opts = option.GetMatches(@"(?<a>[a-z]+)=(?<b>[^&]+)(&|$)");
                    var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    (from a in opts select new KeyValuePair<string, string>(a["a"], a["b"])).Each(k => d.Add(k.Key, k.Value));
                    if (d.ContainsKey("ConnectionTimeout"))
                        this.ConnectionTimeout = d["ConnectionTimeout"].ToCast<int>();
                    if (d.ContainsKey("ReadTimeout"))
                        this.ReadTimeout = d["ReadTimeout"].ToCast<int>();
                    if (d.ContainsKey("SendTimeout"))
                        this.CommandTimeOut = d["SendTimeout"].ToCast<int>();
                    if (d.ContainsKey("Pool"))
                    {
                        this.MaxPool = d["Pool"].ToCast<int>();
                        if (this.MaxPool > 0) this.IsPool = true;
                    }
                }
            }
            else if (connectionString.IsMatch("(server|host|port|user|account|uid|pwd|password|db|database|pool|connectiontimeout|readtimeout|commandtimeout)="))
            {
                this.Host = connectionString.GetMatch(@"(^|;)\s*(server|host)\s*=\s*(?<a>[^;]+)\s*(;|$)");
                this.Port = connectionString.GetMatch(@"(^|;)\s*port\s*=\s*(?<a>\d+)\s*(;|$)").ToCast<int>(11211);
                this.User = connectionString.GetMatch(@"(^|;)\s*(uid|user|account)\s*=\s*(?<a>[^;]+)\s*(;|$)");
                this.Password = connectionString.GetMatch(@"(^|;)\s*(pwd|password)\s*=\s*(?<a>[^;]+)\s*(;|$)");
                this.DbNum = connectionString.GetMatch(@"(^|;)\s*(db|database)\s*=\s*(?<a>[^;]+)\s*(;|$)").ToCast<int>();
                this.MaxPool = connectionString.GetMatch(@"(^|;)\s*(pool)\s*=\s*(?<a>[^;]+)\s*(;|$)").ToCast<int>();
                if (this.MaxPool > 0) this.IsPool = true;
                this.ConnectionTimeout = connectionString.GetMatch(@"(^|;)\s*(connectiontimeout)\s*=\s*(?<a>[^;]+)\s*(;|$)").ToCast<int>();
                this.ReadTimeout = connectionString.GetMatch(@"(^|;)\s*(readtimeout)\s*=\s*(?<a>[^;]+)\s*(;|$)").ToCast<int>(10000);
                this.CommandTimeOut = connectionString.GetMatch(@"(^|;)\s*(commandtimeout)\s*=\s*(?<a>[^;]+)\s*(;|$)").ToCast<int>(10000);
            }
            else
            {
                var db = XiaoFeng.Config.DataBase.Current.Data;
                if (db == null) return;
                var conns = db[connectionString];
                if (conns == null || !conns.Any()) return;
                new MemcachedConfig(conns.FirstOrDefault()).CopyTo(this);
            }
            this.ConnectionString = this.ToString();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 主机
        /// </summary>
        private string _Host = "127.0.0.1";
        /// <summary>
        /// 主机
        /// </summary>
        [Description("主机")]
        public string Host
        {
            get
            {
                if (_Host.IsNullOrEmpty()) _Host = "127.0.0.1";
                return _Host;
            }
            set => _Host = value;
        }
        /// <summary>
        /// 端口
        /// </summary>
        private int _Port = 11211;
        /// <summary>
        /// 端口
        /// </summary>
        [Description("端口")]
        public int Port
        {
            get
            {
                if (_Port <= 0) _Port = 11211;
                return _Port;
            }
            set => _Port = value;
        }
        /// <summary>
        /// 帐号
        /// </summary>
        [Description("帐号")]
        public string User { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")] 
        public string Password { get; set; }
        /// <summary>
        /// 数据库
        /// </summary>
        [Description("数据库")]
        public int DbNum { get; set; } = 0;
        /// <summary>
        /// 连接超时时间
        /// </summary>
        [Description("连接超时时间")]
        public int ConnectionTimeout { get; set; } = 10000;
        /// <summary>
        /// 读取数据超时时间
        /// </summary>
        [Description("读取数据超时时间")] 
        public int ReadTimeout { get; set; } = 10000;
        /// <summary>
        /// 是否是SSL
        /// </summary>
        public Boolean IsSsl { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"memcached://{this.User}:{this.Password + (this.Password.IsNotNullOrEmpty() ? "@" : "")}{this.Host}:{this.Port}/{this.DbNum}?connectiontimout={this.ConnectionTimeout}&readtimeout={this.ReadTimeout}&pool={this.MaxPool}";
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public string ToStringX()
        {
            return $"host={this.Host};port={this.Port};db={this.DbNum};uid={this.User};password={this.Password};connectiontimeout={this.ConnectionTimeout};readtimeout={this.ReadTimeout};";
        }
        /// <summary>
        /// 转换公共连接串
        /// </summary>
        /// <returns></returns>
        public ConnectionConfig ToConnetionConfig()
        {
            this.ConnectionString = this.ToString();
            return (ConnectionConfig)this;
        }
        #endregion
    }
}