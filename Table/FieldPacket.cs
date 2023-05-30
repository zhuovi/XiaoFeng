using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.Memcached;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-05-18 16:52:58                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Table
{
    /// <summary>
    /// 表字段包
    /// </summary>
    public class FieldPacket
    {
        #region 构造器
        /// <summary>
        /// 设置包数据
        /// </summary>
        /// <param name="providerType">驱动类型</param>
        /// <param name="tableAttribute">表属性</param>
        /// <param name="columnAttributes">字段属性集</param>
        public FieldPacket(DbProviderType providerType, TableAttribute tableAttribute, List<ColumnAttribute> columnAttributes)
        {
            this.ProviderType = providerType;
            this.Table = tableAttribute;
            this.Columns = columnAttributes;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表属性
        /// </summary>
        public TableAttribute Table { get; set; }
        /// <summary>
        /// 字段属性
        /// </summary>
        public List<ColumnAttribute> Columns { get; set; }
        /// <summary>
        /// 驱动类型
        /// </summary>
        public DbProviderType ProviderType { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        private StringBuilder Data { get; set; } = new StringBuilder();
        private Dictionary<DbProviderType, string> CreateTableTemplates = new Dictionary<DbProviderType, string> {
            { DbProviderType.SqlServer, @"
IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE ID = OBJECT_ID(N'[dbo].[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN 
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='PK_{0}')
    DROP INDEX {0}.PK_{0};
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='IX_{0}')
    DROP INDEX {0}.IX_{0};

CREATE TABLE [dbo].[{0}](
{1}
)
{2}
END
" },
            { DbProviderType.MySql, @"" },
            { DbProviderType.SQLite, @"" },
            { DbProviderType.Oracle, @"" },
        };
        #endregion

        #region 方法
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Table == null || this.Columns == null || this.Columns.Count == 0) return string.Empty;

            //生成表

            this.Data.Append("CREATE TABLE ");
            var IsFirst = true;
            //生成表字段
            this.Columns.Each(c =>
            {
                if (IsFirst)
                    Data.AppendLine();
                else IsFirst = false;
                //字段名称
                Data.Append(c.Name + "\t");
                //字段类型
                var ftype = c.DataType.ToString();
                Data.Append(ftype);
                if ("NVARCHAR".Contains(ftype.ToUpper()))
                {
                    if (c.Length == 0) c.Length = 50;
                    Data.Append($"({c.Length})");
                }
                else if (ftype.EqualsIgnoreCase("decimal"))
                {
                    Data.Append($"({c.Length},{c.Digit})");
                }
                Data.Append("\t");
                //是否为空
                Data.Append((c.IsNullable || c.PrimaryKey ? "NOT " : "") + "NULL\t");

                var defaultValue = c.DefaultValue.ToString();
                //字段约束及主键
                if (c.PrimaryKey)
                {
                    Data.Append("PRIMARY KEY ASC\t");
                    if (c.AutoIncrement)
                    {
                        switch (this.ProviderType)
                        {
                            case DbProviderType.SqlServer:
                                Data.Append(" IDENTITY({0},{1})".format(defaultValue.IsNumberic() ? defaultValue : "1", c.AutoIncrementStep));
                                break;
                            case DbProviderType.SQLite:
                                Data.Append(" AUTOINCREMENT");
                                break;
                            case DbProviderType.MySql:
                                Data.Append(" AUTO_INCREMENT");
                                break;
                        }
                        Data.Append("\t");
                    }
                }
                //字段默认值
                if (c.DefaultValue.IsNotNullOrEmpty())
                {
                    Data.Append($"Default({this.GetDefaultValue(c)})");
                }
            });
            //生成索引
            var Indexs = this.Columns.Where(a => a.IsUnique || a.PrimaryKey);
            if (Indexs.Any())
            {

            }
            return Data.Append(",").ToString();
        }

        #region 获取默认值
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="column">字段属性</param>
        /// <returns></returns>
        public string GetDefaultValue(ColumnAttribute column)
        {
            var defaultValue = column.DefaultValue.ToString();
            var providerType = this.ProviderType;
            if (defaultValue == null) return string.Empty;
            else if (defaultValue.EqualsIgnoreCase("UUID"))
            {
                switch (providerType)
                {
                    case DbProviderType.SqlServer:
                        return "newid()";
                    case DbProviderType.SQLite:
                        return "'' || hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4), 1) || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)) || ''";
                    default: return "";
                }
            }
            else if (defaultValue.EqualsIgnoreCase("NOW"))
            {
                switch (providerType)
                {
                    case DbProviderType.SqlServer:
                        return "getdate()";
                    case DbProviderType.SQLite:
                        return "datetime('now', 'localtime')";
                    case DbProviderType.OleDb:
                        return "now()";
                    default: return "CURRENT_TIMESTAMP()";
                }
            }
            else if (defaultValue.EqualsIgnoreCase("TIMESTAMP"))
            {
                var date = defaultValue.GetMatch(@"\d{4}-\d{2}-\d{2}");
                if (date.IsNullOrEmpty()) date = "1970-01-01";
                switch (providerType)
                {
                    case DbProviderType.SqlServer:
                        return $"datediff(second,getdate(),'{date}')";
                    case DbProviderType.SQLite:
                        return $"strftime('%s', 'now') - strftime('%s', '{date}')";
                    case DbProviderType.OleDb:
                        return $"datediff('s', now(),'{date}')";
                    default: return "CURRENT_TIMESTAMP()";
                }
            }

            return "NVARCHAR".Contains(column.DataType.ToString().ToUpper()) ? $"'{defaultValue}'" : defaultValue;
        }
        #endregion
        #endregion


    }
}