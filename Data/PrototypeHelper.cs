﻿using System;
using System.Data.Common;
using XiaoFeng.Data;
using XiaoFeng.Model;
using XiaoFeng.Redis;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-08-10 11:45:08                            *
*  Version : v 1.1.6                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 数据库扩展类
    /// Version : 1.1.6
    /// </summary>
    public static partial class PrototypeHelper
    {
        #region 生成当前数据库数据库表Model
        /// <summary>
        /// 生成当前数据库数据库表Model
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="path">保存路径</param>
        /// <param name="tableName">表名</param>
        /// <param name="namespace">命名空间</param>
        /// <param name="connName">数据库连接名</param>
        /// <param name="connIndex">数据库索引</param>
        public static void CreateModel(this IDbHelper data, string path, string tableName = "", string @namespace = "XiaoFeng.Models", string connName = "", int connIndex = 0)
        {
            using (var make = new Model.MakeModel() { DataHelper = data, Namespace = @namespace }) make.CreateModel(path, tableName, connName.Multivariate(data.ConnConfig.AppKey), connIndex);
        }
        #endregion

        #region 获取存储过程带参数的SQL语句
        /// <summary>
        /// 获取存储过程带参数的SQL语句
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="isNull">参数是否是?号</param>
        /// <returns></returns>
        public static string GetParameterCommandText(this DbCommand cmd, Boolean isNull = false)
        {
            var paramSQLString = cmd.CommandText;
            if (paramSQLString.IsNullOrEmpty()) return "";
            if (cmd.Parameters == null || cmd.Parameters.Count == 0) return paramSQLString;
            var paramString = "";
            if (isNull)
            {
                var i = 0;
                paramSQLString = paramSQLString.ReplacePattern(@"\?(?<a>(\s|,|\)|;|$))", m =>
                {
                    if (i > cmd.Parameters.Count - 1) return m.Groups[0].Value;
                    var p = cmd.Parameters[i++];
                    var c = p.DbType.ToString().IsMatch(@"(Byte|Boolean|Currency|Decimal|Double|U?Int(16|32|64)|SByte|VarNumeric)") ? "" : "'";
                    return c + p.Value.GetValue() + c + m.Groups["a"].Value;
                });
            }
            else
                cmd.Parameters.Each<DbParameter>(p =>
                {
                    paramString += $"{p.ParameterName}[p.DbType]={p.Value.GetValue()},";
                    var c = p.DbType.ToString().IsMatch(@"(Byte|Boolean|Currency|Decimal|Double|U?Int(16|32|64)|SByte|VarNumeric)") ? "" : "'";
                    paramSQLString = paramSQLString.ReplacePattern(@"[@:]" + p.ParameterName.TrimStart(new char[] { '@', ':' }) + @"(\s*[,=\);\s]|$)", c + p.Value.GetValue() + c + "$1");
                });
            return @"存储过程:" + cmd.CommandText + "\r\n代入参数的存储过程:" + paramSQLString + "\r\n存储参数:" + paramString.TrimEnd(',');
        }
        #endregion

        #region 转换到Redis配置连接串
        /// <summary>
        /// 转换到Redis配置连接串
        /// </summary>
        /// <param name="config">配置</param>
        /// <returns></returns>
        public static RedisConfig ToRedisConfig(this ConnectionConfig config) => new Redis.RedisConfig(config);
        #endregion

        #region 模型生成表
        /// <summary>
        /// 模型生成表
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="modelType">模型类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="connName">数据库配置key</param>
        /// <param name="index">数据库索引</param>
        /// <returns></returns>
        public static string CreateTable(this ConnectionConfig config, Type modelType, string tableName = "", string connName = "", int index = -1)
        {
            return new Table.MakeTable(config).Create(modelType, tableName, connName, index);
            /* if (modelType == null) return false;
             var tableAttr = modelType.GetTableAttribute();
             if (tableAttr == null) tableAttr = new TableAttribute();
             if (connName.IsNotNullOrEmpty()) tableAttr.ConnName = connName;
             if (index != -1) tableAttr.ConnIndex = index;
             modelType.GetPropertiesAndFields().Each(c =>
             {
                 var columnAttr = c.GetColumnAttribute();
                 if (columnAttr == null) columnAttr = new ColumnAttribute();
             });
             return true;*/
        }
        /// <summary>
        /// 模型生成表
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="config">数据库配置</param>
        /// <param name="tableName">表名</param>
        /// <param name="connName">数据库配置key</param>
        /// <param name="index">数据库索引</param>
        /// <returns></returns>
        public static string CreateTable<T>(this ConnectionConfig config, string tableName = "", string connName = "", int index = -1) where T : Entity<T>, new() => config.CreateTable(typeof(T), tableName, connName, index);
        #endregion

        #region 屏蔽数据库连接串中的密码
        /// <summary>
        /// 屏蔽数据库连接串中的密码
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <returns></returns>
        public static string BlockPassword(this string connectionString)
        {
            var _ = connectionString.ReplacePattern(@"(^|;)(\s*)(pwd|password)=[\s\S]*(;|$)", "$1$2$3=******$4");
            _ = _.ReplacePattern(@":([^@]+)@", ":******@");
            return _;
        }
        #endregion
    }
}