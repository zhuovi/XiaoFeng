using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.Json;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-08 14:48:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// 数据接口
    /// </summary>
    public interface IDbFastSql
    {
        #region 属性
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        ConnectionConfig Config { get; set; }
        /// <summary>
        /// SQL 函数集
        /// </summary>
        [JsonIgnore]
        Dictionary<string, string> SqlFunction { get; set; }
        /// <summary>
        /// 取反 SQL 函数集
        /// </summary>
        [JsonIgnore] 
        Dictionary<string, string> SqlUnFunction { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrorMessage { get; set; }
        /// <summary>
        /// 错误SQL语句
        /// </summary>
        string ErrorSqlString { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 表字段格式化
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>格式化后的字段字符串</returns>
        string FormattingField(string field);
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        string GetSqlString(ISqlBuilder builder);
        #endregion
    }
}