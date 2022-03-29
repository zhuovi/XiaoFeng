using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Data
{
    /// <summary>
    /// SQL 函数
    /// Verstion : 1.0.0
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// </summary>
    public class DataSQLFun
    {
        #region SQLServer
        /// <summary>
        /// SQLServer 对象集
        /// </summary>
        private Dictionary<string, string> _MsSqlFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// SQLServer 对象集
        /// </summary>
        public Dictionary<string, string> MsSqlFun
        {
            get
            {
                if (this._MsSqlFun == null || this._MsSqlFun.Count == 0)
                {
                    this._MsSqlFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","CHARINDEX({1},{0}) > 0"},
                        {"Length","LEN({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} LIKE {1}"},
                        {"LikeSQLX","{0} LIKE {1}"},
                        {"NotLikeSQL","{0} NOT LIKE {1}"},
                        {"NotLikeSQLX","{0} NOT LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","CHARINDEX({1},{0})"},
                        {"CharIndexSQL","CHARINDEX({1},{0})"},
                        {"PatindexSQL","PATINDEX({0},{1})"},
                        {"DateAddSQL","DATEADD({2},{1},{0})"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","CONVERT(VARCHAR, {0}, {1})" },
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"AbsSQL","ABS({0})"},
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LEN({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._MsSqlFun;
            }
        }
        /// <summary>
        /// SQLServer 对象集
        /// </summary>
        private Dictionary<string, string> _MsSqlUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// SQLServer 对象集
        /// </summary>
        public Dictionary<string, string> MsSqlUnFun
        {
            get
            {
                if (this._MsSqlUnFun == null || this._MsSqlUnFun.Count == 0)
                {
                    this._MsSqlUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","CHARINDEX({1},{0}) <= 0"},
                        {"Length","LEN({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} NOT IN ({1})"},
                        {"NotInSQL","{0} IN ({1})"},
                        {"IndexOf","CHARINDEX({1},{0})"},
                        {"CharIndexSQL","CHARINDEX({1},{0})"},
                        {"PatindexSQL","PATINDEX({0},{1})"},
                        {"DateAddSQL","DATEADD({2},{1},{0})"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","CONVERT(VARCHAR, {0}, {1})" },
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"AbsSQL","ABS({0})"},
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LEN({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._MsSqlUnFun;
            }
        }
        #endregion

        #region SQLite
        /// <summary>
        /// SQLite 对象集
        /// </summary>
        private Dictionary<string, string> _SQLiteFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// SQLite 对象集
        /// </summary>
        public Dictionary<string, string> SQLiteFun
        {
            get
            {
                if (this._SQLiteFun == null || this._SQLiteFun.Count == 0)
                {
                    this._SQLiteFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","INSTR({1},{0}) > 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} LIKE {1}"},
                        {"LikeSQLX","{0} LIKE {1}"},
                        {"NotLikeSQL","{0} NOT LIKE {1}"},
                        {"NotLikeSQLX","{0} NOT LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","INSTR({1},{0})"},
                        {"CharIndexSQL","INSTR({1},{0})"},
                        {"PatindexSQL","PATINDEX({0},{1})"},
                        {"DateAddSQL","DATETIME({0},'localtime','{1} {2}')"},
                        {"DateDiffSQL","ROUND((julianday({1}) - julianday({0})){2})"},
                        {"DatePartSQL","STRFTIME({1},{0})"},
                        {"DateFormatSQL","STRFTIME({1},{0},'localtime')"},
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._SQLiteFun;
            }
        }
        /// <summary>
        /// SQLite 对象集
        /// </summary>
        private Dictionary<string, string> _SQLiteUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// SQLite 对象集
        /// </summary>
        public Dictionary<string, string> SQLiteUnFun
        {
            get
            {
                if (this._SQLiteUnFun == null || this._SQLiteUnFun.Count == 0)
                {
                    this._SQLiteUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","INSTR({1},{0}) <= 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} NOT IN ({1})"},
                        {"NotInSQL","{0} IN ({1})"},
                        {"IndexOf","INSTR({1},{0})"},
                        {"CharIndexSQL","INSTR({1},{0})"},
                        {"PatindexSQL","PATINDEX({0},{1})"},
                        {"DateAddSQL","DATETIME({0},'localtime','{1} {2}')"},
                        {"DateDiffSQL","ROUND((julianday({1}) - julianday({0})){2})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","STRFTIME({1},{0},'localtime')"},
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._SQLiteUnFun;
            }
        }
        #endregion

        #region MySQL
        /// <summary>
        /// MySQL对象集
        /// </summary>
        private Dictionary<string, string> _MySqlFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// MySQL对象集
        /// </summary>
        public Dictionary<string, string> MySqlFun
        {
            get
            {
                if (this._MySqlFun == null || this._MySqlFun.Count == 0)
                {
                    this._MySqlFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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
                        {"IndexOf","INSTR({1},{0})"},
                        {"CharIndexSQL","INSTR({1},{0})"},
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
                        //{"ReplicateSQL","REPLICATE({0},{1})"},
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
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._MySqlFun;
            }
        }
        /// <summary>
        /// MySQL对象集
        /// </summary>
        private Dictionary<string, string> _MySqlUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// MySQL对象集
        /// </summary>
        public Dictionary<string, string> MySqlUnFun
        {
            get
            {
                if (this._MySqlUnFun == null || this._MySqlUnFun.Count == 0)
                {
                    this._MySqlUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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
                        {"IndexOf","INSTR({1},{0})"},
                        {"CharIndexSQL","INSTR({1},{0})"},
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
                        //{"ReplicateSQL","REPLICATE({0},{1})"},
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
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._MySqlUnFun;
            }
        }
        #endregion

        #region Oracle
        /// <summary>
        /// Oracle 对象集
        /// </summary>
        private Dictionary<string, string> _OracleFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Oracle 对象集
        /// </summary>
        public Dictionary<string, string> OracleFun
        {
            get
            {
                if (this._OracleFun == null || this._OracleFun.Count == 0)
                {
                    this._OracleFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","CHARINDEX({1},{0}) > 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} LIKE {1}"},
                        {"LikeSQLX","{0} LIKE {1}"},
                        {"NotLikeSQL","{0} NOT LIKE {1}"},
                        {"NotLikeSQLX","{0} NOT LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","CHARINDEX({1},{0})"},
                        {"CharIndexSQL","CHARINDEX({1},{0})"},
                        {"PatindexSQL","PATINDEX({0},{1})"},
                        {"DateAddSQL","{0} + INTERVAL {1} {2}"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({0},{1})"},
                        {"DateFormatSQL","DATE_FORMAT({0},{1})" },
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","NVL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._OracleFun;
            }
        }
        /// <summary>
        /// Oracle 对象集
        /// </summary>
        private Dictionary<string, string> _OracleUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Oracle 对象集
        /// </summary>
        public Dictionary<string, string> OracleUnFun
        {
            get
            {
                if (this._OracleUnFun == null || this._OracleUnFun.Count == 0)
                {
                    this._OracleUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","CHARINDEX({1},{0}) <= 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} NOT IN ({1})"},
                        {"NotInSQL","{0} IN ({1})"},
                        {"IndexOf","CHARINDEX({1},{0})"},
                        {"CharIndexSQL","CHARINDEX({1},{0})"},
                        {"PatindexSQL","PATINDEX({0},{1})"},
                        {"DateAddSQL","{0} + INTERVAL {1} {2}"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","DATE_FORMAT({0}, {1})" },
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","NVL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._OracleUnFun;
            }
        }
        #endregion

        #region Oledb
        /// <summary>
        /// Oledb 对象集
        /// </summary>
        private Dictionary<string, string> _OledbFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Oledb 对象集
        /// </summary>
        public Dictionary<string, string> OledbFun
        {
            get
            {
                if (this._OledbFun == null || this._OledbFun.Count == 0)
                {
                    this._OledbFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","INSTR({0},{1}) > 0"},
                        {"Length","LEN({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","MID({0},{1},{2})"},
                        {"Trim","TRIM({0})"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UCASE({0})"},
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
                        {"PatindexSQL","INSTR({0},{1})"},
                        {"DateAddSQL","DATEADD('{2}',{1},{0})"},
                        {"DateDiffSQL","DATEDIFF('{2}',{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","FORMAT({0},{1})"},
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","INT({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FIX({0})"},
                        {"LengthSQL","LEN({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","StrReverse({0})"},
                        {"StuffSQL","(LEFT({0}, {1} - 1) + {3} + MID({0}, {1} + {2}))"},
                        {"SubstringSQL","MID({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UCASE({0})"},
                        {"LowerSQL","LCASE({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._OledbFun;
            }
        }
        /// <summary>
        /// Oledb 对象集
        /// </summary>
        private Dictionary<string, string> _OledbUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Oledb 对象集
        /// </summary>
        public Dictionary<string, string> OledbUnFun
        {
            get
            {
                if (this._OledbUnFun == null || this._OledbUnFun.Count == 0)
                {
                    this._OledbUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} NOT LIKE {1}"},
                        {"Contains","INSTR({0},{1}) <= 0"},
                        {"Length","LEN({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","MID({0},{1},{2})"},
                        {"Trim","TRIM({0})"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UCASE({0})"},
                        {"ToLower","LCASE({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","INSTR({0},{1})"},
                        {"CharIndexSQL","INSTR({0},{1})"},
                        {"PatindexSQL","INSTR({0},{1})"},
                        {"DateAddSQL","DATEADD('{2}',{1},{0})"},
                        {"DateDiffSQL","DATEDIFF('{2}',{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","FORMAT({0},{1})"},
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","INT({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FIX({0})"},
                        {"LengthSQL","LEN({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","StrReverse({0})"},
                        {"StuffSQL","(LEFT({0}, {1} - 1) + {3} + MID({0}, {1} + {2}))"},
                        {"SubstringSQL","MID({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UCASE({0})"},
                        {"LowerSQL","LCASE({0})"},
                        {"CountSQL","COUNT({0})"},
                        {"MaxSQL","MAX({0})"},
                        {"MinSQL","MIN({0})"},
                        {"SumSQL","SUM({0})"},
                        {"IsNullSQL","ISNULL({0},{1})"},
                        {"AddSQL","{0} + {1}"},
                        {"SubtractSQL","{0} - {1}"},
                        {"MultiplySQL","{0} * {1}"},
                        {"DivideSQL","{0} / {1}"},
                        {"BetweenSQL","{0} BETWEEN {1} AND {2}"},
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._OledbUnFun;
            }
        }
        #endregion

        #region 达梦
        /// <summary>
        /// 达梦 对象集
        /// </summary>
        private Dictionary<string, string> _DamengFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 达梦 对象集
        /// </summary>
        public Dictionary<string, string> DamengFun
        {
            get
            {
                if (this._DamengFun == null || this._DamengFun.Count == 0)
                {
                    this._DamengFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","POSITION({1},{0}) > 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} LIKE {1}"},
                        {"LikeSQLX","{0} LIKE {1}"},
                        {"NotLikeSQL","{0} NOT LIKE {1}"},
                        {"NotLikeSQLX","{0} NOT LIKE {1}"},
                        {"InSQL","{0} IN ({1})"},
                        {"NotInSQL","{0} NOT IN ({1})"},
                        {"IndexOf","POSITION({1},{0})"},
                        {"CharIndexSQL","POSITION({1},{0})"},
                        {"PatindexSQL","POSITION({0},{1})"},
                        {"DateAddSQL","DATEADD({2},{1},{0})"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({0},{1})"},
                        {"DateFormatSQL","DATE_FORMAT({0},{1})" },
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
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
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._DamengFun;
            }
        }
        /// <summary>
        /// 达梦 对象集
        /// </summary>
        private Dictionary<string, string> _DamengUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 达梦 对象集
        /// </summary>
        public Dictionary<string, string> DamengUnFun
        {
            get
            {
                if (this._DamengUnFun == null || this._DamengUnFun.Count == 0)
                {
                    this._DamengUnFun = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"StartsWith","{0} NOT LIKE {1}"},
                        {"EndsWith","{0} LIKE {1}"},
                        {"Contains","POSITION({1},{0}) <= 0"},
                        {"Length","LENGTH({0})"},
                        {"Replace","REPLACE({0},{1},{2})"},
                        {"Substring","SUBSTRING({0},{1},{2})"},
                        {"Trim","LTRIM(RTRIM({0}))"},
                        {"TrimStart","LTRIM({0})"},
                        {"TrimEnd","RTRIM({0})"},
                        {"ToUpper","UPPER({0})"},
                        {"ToLower","LOWER({0})"},
                        {"Count","COUNT({0})"},
                        {"Sum","SUM({0})"},
                        {"LikeSQL","{0} NOT LIKE {1}"},
                        {"LikeSQLX","{0} NOT LIKE {1}"},
                        {"NotLikeSQL","{0} LIKE {1}"},
                        {"NotLikeSQLX","{0} LIKE {1}"},
                        {"InSQL","{0} NOT IN ({1})"},
                        {"NotInSQL","{0} IN ({1})"},
                        {"IndexOf","POSITION({1},{0})"},
                        {"CharIndexSQL","POSITION({1},{0})"},
                        {"PatindexSQL","POSITION({0},{1})"},
                        {"DateAddSQL","DATEADD({2},{1},{0})"},
                        {"DateDiffSQL","DATEDIFF({2},{0},{1})"},
                        {"DatePartSQL","DATEPART({1},{0})"},
                        {"DateFormatSQL","DATE_FORMAT({0}, {1})" },
                        {"AbsSQL","ABS({0})"},
                        {"CeilingSQL","CEILING({0})" },
                        {"RoundSQL","ROUND({0},{1})" },
                        {"FloorSQL","FLOOR({0})"},
                        {"LengthSQL","LENGTH({0})"},
                        {"LeftSQL","LEFT({0},{1})"},
                        {"RightSQL","RIGHT({0},{1})"},
                        {"ReplaceSQL","REPLACE({0},{1},{2})"},
                        {"ReplicateSQL","REPLICATE({0},{1})"},
                        {"ReverseSQL","REVERSE({0})"},
                        {"StuffSQL","STUFF({0},{1},{2},{3})"},
                        {"SubstringSQL","SUBSTRING({0},{1},{2})"},
                        {"TrimSQL","LTRIM(RTRIM({0}))"},
                        {"LTrimSQL","LTRIM({0})"},
                        {"RTrimSQL","RTRIM({0})"},
                        {"UpperSQL","UPPER({0})"},
                        {"LowerSQL","LOWER({0})"},
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
                        {"AvgSQL","AVG({0})" }
                    };
                }
                return this._DamengUnFun;
            }
        }
        #endregion

        #region 字段表格式
        /// <summary>
        /// 字段表格式
        /// </summary>
        /// <param name="dbProviderType">驱动类型</param>
        /// <param name="_">字段名</param>
        /// <returns></returns>
        public static string FieldFormat(DbProviderType dbProviderType, string _)
        {
            if (_.IsMatch(@"(\(|\)| as |\]|\[|`|\"")")) return _;
            var first = "";
            if (_.IsMatch(@"DISTINCT\s+"))
            {
                first = "DISTINCT ";
                _ = _.RemovePattern(@"\s*DISTINCT\s*");
                var _list = new List<string>();
                _.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Each(a =>
                  {
                      _list.Add(FieldFormat(dbProviderType, a));
                  });
                return first + _list.Join(",");
            }
            switch (dbProviderType)
            {
                case DbProviderType.SqlServer:
                case DbProviderType.OleDb:
                case DbProviderType.SQLite:
                    _ = "[" + _ + "]"; break;
                case DbProviderType.Oracle:
                case DbProviderType.MySql:
                    _ = "`" + _ + "`"; break;
                case DbProviderType.Dameng:
                    _ = "\"" + _ + "\""; break;
                default:
                    break;
            }
            return first + _;
        }
        #endregion

        #region SQL 函数
        /// <summary>
        /// SQL 函数
        /// </summary>
        public static string[] SQLFun = new string[] { "Upper","UCASE", "Lower","LCASE","MID", "Trim", "Substring", "Stuff", "Reverse","StrReverse", "Replicate", "Replace", "Len", "Right", "Left", "Round", "Ceiling", "Floor","FIX", "Abs","INT", "Charindex", "Date_Format","FORMAT", "INSTR", "DatePart", "DateDiff", "DATE_ADD","DATEADD", "Patindex", "CONVERT", "RTRIM", "LTRIM","ISNULL", "TIMESTAMPDIFF" };
        #endregion
    }
}