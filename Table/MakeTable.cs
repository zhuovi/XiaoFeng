using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Data;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Table
{
    /// <summary>
    /// 通过Model创建表
    /// Verstion : 1.0.0
    /// Create Time : 2019/03/18 16:32:26
    /// Update Time : 2019/03/18 16:32:26
    /// </summary>
    public class MakeTable : Disposable, ITable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MakeTable() { this.Config = XiaoFeng.Config.DataBase.Current.First(); }
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="config">数据库配置</param>
        public MakeTable(ConnectionConfig config)
        {
            this.Config = config;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据库配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串</param>
        /// <param name="connIndex">第几个连接</param>
        /// <returns></returns>
        public string Create<T>(string tableName = "", string connName = "", int connIndex = -1) => this.Create(typeof(T), tableName, connName, connIndex);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="modelType">Model类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串</param>
        /// <param name="connIndex">第几个连接</param>
        /// <returns></returns>
        public string Create(Type modelType, string tableName = "", string connName = "", int connIndex = -1)
        {
            if (this.Config.ProviderType == DbProviderType.SqlServer)
            {
                return new SqlServer(this.Config).Create(modelType, tableName, connName, connIndex);
            }
            return String.Empty;
        }
        #endregion
    }
}