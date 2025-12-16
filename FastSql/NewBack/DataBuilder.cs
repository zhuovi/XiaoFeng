using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.Data.SQL;
using XiaoFeng.FastSql.NewBack.Interface;
using XiaoFeng.Json;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-01-03 11:20:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql.NewBack
{
    /// <summary>
    /// 数据仓创建器
    /// </summary>
    public class DataBuilder : IDataBuilder
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public DataBuilder()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否缓存
        /// </summary>
        [JsonConverter(typeof(Json.StringEnumConverter))]
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
        /// Model类型
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// 表类型配置 代替 ModelType
        /// </summary>
        public Dictionary<TableType, TableData> TableData { get; set; }
        /// <summary>
        /// 分表配置
        /// </summary>
        public ITableSplit TableSplit { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        private TableSplitConfig TableSplitConfig { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        private string _TableName;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                if (this._TableName.IsNullOrEmpty())
                {
                    var Table = this.ModelType.GetTableAttribute();
                    if (Table == null)
                        this._TableName = this.ModelType.Name;
                    else
                        this._TableName = Table.Name ?? this.ModelType.Name;
                    var tables = this.TableSplitConfig.List;
                    if (tables.Any())
                    {
                        var table = tables.FirstOrDefault(a => a.Name.EqualsIgnoreCase(this._TableName));
                        if (table != null)
                        {
                            this.TableSplit = table;
                        }
                    }
                }
                return this._TableName;
            }
            set
            {
                var val = value;
                if (val.IndexOf(" ") == -1)
                {
                    var tables = this.TableSplitConfig?.List;
                    if (tables != null && tables.Any())
                    {
                        var table = tables.FirstOrDefault(a => a.Name.EqualsIgnoreCase(val));
                        if (table != null)
                        {
                            this.TableSplit = table;
                        }
                    }
                }
                this._TableName = val;
            }
        }
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQLString { get; set; }
        /// <summary>
        /// SQL语句带入存储过程参数
        /// </summary>
        public string SQLParameter { get; set; }
        /// <summary>
        /// 上一次执行SQL语句
        /// </summary>
        public string LastSQLString { get; set; }
        /// <summary>
        /// 上一次执行SQL语句带入存储过程参数
        /// </summary>
        public string LastSQLParameter { get; set; }
        /// <summary>
        /// 显示字段
        /// </summary>
        public List<string> Columns { get; set; }
        /// <summary>
        /// 设置字段
        /// </summary>
        public List<object> UpdateColumns { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public Dictionary<string, object> Fields { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public StringBuilder WhereString { get; set; }
        /// <summary>
        /// 一页多少条
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// 共有多少页
        /// </summary>
        public int? PageCount { get; set; }
        /// <summary>
        /// 共有多少条
        /// </summary>
        public int? Counts { get; set; }
        /// <summary>
        /// 前多少条
        /// </summary>
        public int? Top { get; set; }
        /// <summary>
        /// 跳过多少条
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public List<string> OrderBy { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public List<string> GroupBy { get; set; }
        /// <summary>
        /// 处理类型
        /// </summary>
        [JsonConverter(typeof(Json.StringEnumConverter))]
        public SQLType SQLType { get; set; }
        /// <summary>
        /// 数据库配置
        /// </summary>
        private ConnectionConfig _Config;
        /// <summary>
        /// 数据库配置
        /// </summary>
        public ConnectionConfig Config
        {
            get => this._Config;
            set
            {
                this._Config = value;
            }
        }
        /// <summary>
        /// 执行SQL时长 单位为毫秒
        /// </summary>
        public Int64 RunSQLTime { get; set; }
        /// <summary>
        /// 拼接SQL时长 单位为毫秒
        /// </summary>
        public Int64 SpliceSQLTime { get; set; }
        /// <summary>
        /// LinqToSql时长
        /// </summary>
        public double LinqToSQLTime { get; set; }
        /// <summary>
        /// 初始化时间
        /// </summary>
        private DateTime BeginTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 存储过程参数集
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        #endregion

        #region 方法

        #endregion
    }
}