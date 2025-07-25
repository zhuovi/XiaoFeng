using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-07-15 16:20:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 只读库配置
    /// </summary>
    public class ReadOnlyDbConfig
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public ReadOnlyDbConfig()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="enabled">是否启用</param>
        /// <param name="backup">是否备用</param>
        /// <param name="weight">权重</param>
        public ReadOnlyDbConfig(string connectionString, bool enabled, bool backup, int weight)
        {
            ConnectionString = connectionString;
            Enabled = enabled;
            Backup = backup;
            Weight = weight;
        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="data">数据</param>
        public ReadOnlyDbConfig(string data)
        {
            if (data.IsNullOrEmpty() || !data.IsJson()) return;
            var json = data.JsonToObject<ReadOnlyDbConfig>();
            json.CopyTo(this);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 数据库连接串
        /// </summary>
        [Description("数据库连接串")]
        public string ConnectionString { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Description("是否启用")]
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// 是否备用
        /// </summary>
        [Description("是否备用")]
        public bool Backup { get; set; } = false;
        /// <summary>
        /// 权重
        /// </summary>
        [Description("权重")]
        public int Weight { get; set; } = 1;
        #endregion

        #region 方法
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return this.ToJson();
        }

        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~ReadOnlyDbConfig()
        {

        }
        #endregion
        #endregion
    }
}