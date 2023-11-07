using System;
using System.Collections.Generic;
using XiaoFeng.Json;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 10:18:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 拼接SQL记录器
    /// </summary>
    public class DataSqlQ
    {
        /// <summary>
        /// 是否缓存
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CacheState CacheState { get; set; }
        /// <summary>
        /// 缓存时长 单位为秒
        /// </summary>
        public int? CacheTimeOut { get; set; }
        /// <summary>
        /// 是否命中缓存
        /// </summary>
        public Boolean IsHitCache { get; set; }
        /// <summary>
        /// 命中缓存次数
        /// </summary>
        public long HitCacheCount { get; set; }
        /// <summary>
        /// 缓存Key
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public string If { get; set; }
        /// <summary>
        /// 符合条件 执行
        /// </summary>
        public List<string> Then { get; set; } = new List<string>();
        /// <summary>
        /// 不符合条件执行
        /// </summary>
        public List<string> Else { get; set; } = new List<string>();
        /// <summary>
        /// 其它的条件
        /// </summary>
        public List<string> ElseIf { get; set; } = new List<string>();
        /// <summary>
        /// 其它的条件执行
        /// </summary>
        public List<List<string>> ElseIfThen { get; set; } = new List<List<string>>();
        /// <summary>
        /// 数据库配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        /// <summary>
        /// 运行时长
        /// </summary>
        public long RunSQLTime { get; set; } = 0;
    }
}