using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Data;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-08 15:17:06                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql
{
    /// <summary>
    /// 基础驱动
    /// </summary>
    public abstract class BaseProvider
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public BaseProvider()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="config">数据库连接配置</param>
        public BaseProvider(ConnectionConfig config)
        {
            Config = config;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 错误SQL语句
        /// </summary>
        public string ErrorSqlString { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 表字段格式化
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>加格式后的字段字符串</returns>
        public virtual string FormattingField(string field)
        {
            if (field.IsMatch(@"(\(|\)| as |\]|\[|`|\"")")) return field;
            var first = "";
            
            if (field.IsMatch(@"(DISTINCT)\s+"))
            {
                first = "DISTINCT ";
                _ = field.RemovePattern(@"\s*DISTINCT\s*");
                var _list = new List<string>();
                field.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(a =>
                {
                    _list.Add(this.FormattingField(a));
                });
                return first + _list.Join(",");
            }
            var format = this.GetFormatFieldString();
            if (format.IsNullOrEmpty()) return field;
            return format.format(field);
        }
        /// <summary>
        /// 获取格式化字符串
        /// </summary>
        /// <returns></returns>
        public abstract string GetFormatFieldString();
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <param name="builder">SQL创建器</param>
        /// <returns></returns>
        public abstract string GetSqlString(ISqlBuilder builder);
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="sqlString">错误SQL语句</param>
        /// <param name="errorMessage">错误消息</param>
        public void Error(string sqlString, string errorMessage)
        {
            this.ErrorSqlString = sqlString;
            this.ErrorMessage = errorMessage;
        }
        #endregion
    }
}