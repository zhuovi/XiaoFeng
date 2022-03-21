using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.Data;
using XiaoFeng.Json;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-07 10:18:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    [ConfigFile("/Config/Cache.json", 0, "FAYELF-CONFIG-CACHE", ConfigFormat.Json)]
    public class CacheConfig : ConfigSet<CacheConfig>
    {
        #region 属性
        /// <summary>
        /// 缓存Key
        /// </summary>
        [Description("缓存Key")]
        public string CacheKey { get; set; } = "FAYELF_CACHE";
        /// <summary>
        /// 缓存类型
        /// </summary>
        [Description("缓存类型 不缓存:No,内存:Memory,磁盘:Disk,Redis:Redis,Memcache:Memcache,MongoDB:MongoDB")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CacheType CacheType { get; set; } = CacheType.Memory;
        /// <summary>
        /// 缓存路径
        /// </summary>
        [Description("缓存路径")]
        public string CachePath { get; set; } = "Cache";
        /// <summary>
        /// 数据库配置
        /// </summary>
        [Description("数据库配置")]
        public ConnectionConfig ConnectionConfig { get; set; } = new ConnectionConfig
        {
            ProviderType = DbProviderType.Redis,
            ConnectionString = "server=127.0.0.1;port=6379;password=;"
        };
        #endregion
    }
}