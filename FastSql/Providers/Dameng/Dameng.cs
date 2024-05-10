﻿using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Data;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-08 14:58:04                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FastSql.Providers
{
    /// <summary>
    /// Dameng 操作类
    /// </summary>
    public class Dameng : BaseProvider, IFastSql
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public Dameng()
        {

        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="connConfig">数据库连接配置</param>
        public Dameng(ConnectionConfig connConfig) : base(connConfig)
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// SQL 函数集
        /// </summary>
        private Dictionary<string, string> _SqlFunction;
        /// <summary>
        /// SQL 函数集
        /// </summary>
        public Dictionary<string, string> SqlFunction
        {
            get
            {
                if (this._SqlFunction == null)
                {
                    this._SqlFunction = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","INSTR({0},{1}) > 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LCASE({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} LIKE {1}"},
                        {"LikeSQLX","{0} LIKE {1}"},
                        {"NotLikeSQL","{0} NOT LIKE {1}"},
                        {"NotLikeSQLX","{0} NOT LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","INSTR({0},{1})"},
                        {"CharIndexSQL","INSTR({0},{1})"},
                        {"PatindexSQL","{0} REGEXP {1}"},
                        {"DateAddSQL","DATE_ADD({0},INTERVAL {1} {2})"},
                        {"DateDiffSQL","TIMESTAMPDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATE_FORMAT({0},{1})"},
                        {"DateFormatSQL","DATE_FORMAT({0}, {1})" },
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LCASE({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","IFNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" },
                        {"CastSQL","CAST({0} as {1})" },
                        {"StDevSQL","STDEV({0})" },
                        {"StDevpSQL","STDEVP({0})" }
                    };
                }
                return this._SqlFunction;
            }
            set => this._SqlFunction = value;
        }
        /// <summary>
        /// 取反 SQL 函数集
        /// </summary>
        private Dictionary<string, string> _SqlUnFunction;
        /// <summary>
        /// 取反 SQL 函数集
        /// </summary>
        public Dictionary<string, string> SqlUnFunction
        {
            get
            {
                if (this._SqlUnFunction == null)
                {
                    this._SqlUnFunction = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} NOT LIKE {1}"},
                        {"Contains","INSTR({0},{1}) <= 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LCASE({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} NOT IN ({1})"},
                        {"NotInSQL","{0} IN ({1})"},
                        {"IndexOf","INSTR({0},{1})"},
                        {"CharIndexSQL","INSTR({0},{1})"},
                        {"PatindexSQL","{0} REGEXP {1}"},
                        {"DateAddSQL","DATE_ADD({0},INTERVAL {1} {2})"},
                        {"DateDiffSQL","TIMESTAMPDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATE_FORMAT({0},{1})"},
                        {"DateFormatSQL","DATE_FORMAT({0}, {1})" },
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LCASE({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","IFNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" },
                        {"CastSQL","CAST({0} as {1})" },
                        {"StDevSQL","STDEV({0})" },
                        {"StDevpSQL","STDEVP({0})" }
                    };
                }
                return this._SqlUnFunction;
            }
            set => this._SqlUnFunction = value;
        }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override string GetFormatFieldString()
        {
            if (!this.Config.IsFormatField) return string.Empty;
            return this.Config.FormatField.IsNullOrEmpty() ? "\"{0}\"" : this.Config.FormatField;
        }
        ///<inheritdoc/>
        public override string GetSqlString(ISqlBuilder builder)
        {
            return string.Empty;
        }
        #endregion
    }
}