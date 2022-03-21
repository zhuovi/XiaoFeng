using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Data;
namespace XiaoFeng.Table
{
    /// <summary>
    /// 基础表
    /// </summary>
    public abstract class BaseTable : Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        protected BaseTable()
        {

        }
        #endregion

        #region 方法

        #region 获取表配置属性
        /// <summary>
        /// 获取表配置属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串</param>
        /// <param name="connIndex">索引</param>
        /// <returns></returns>
        public virtual TableAttribute GetTableAttribute(Type type, string tableName = "", string connName = "", int connIndex = -1)
        {
            var table = new TableAttribute();
            if (tableName.IsNotNullOrEmpty()) table.Name = tableName;
            if (connName.IsNotNullOrEmpty()) table.ConnName = connName;
            if (connIndex > -1) table.ConnIndex = connIndex;

            if (table.Name.IsNullOrEmpty())
            {
                var TableAttr = type.GetTableAttribute(false);
                if (TableAttr == null) table.Name = type.Name;
                else
                {
                    table.Name = TableAttr.Name;
                    if (table.ConnName.IsNullOrEmpty() && TableAttr.ConnName.IsNotNullOrEmpty()) table.ConnName = TableAttr.ConnName;
                    if (table.ConnIndex < 0 && TableAttr.ConnIndex >= 0) table.ConnIndex = TableAttr.ConnIndex;
                }
            }
            return table;
        }
        #endregion

        /// <summary>
        /// 获取字段定义
        /// </summary>
        /// <param name="column">字段配置</param>
        /// <param name="providerType">驱动类型</param>
        /// <returns></returns>
        public virtual string GetField(ColumnAttribute column,DbProviderType providerType)
        {
            if (column == null) return "";
            var FieldFormat = "[{0}] [{1}]{2} {3} {4}," + Environment.NewLine;
            var dbType = column.DataType.ToString();
            var tLength = "";
            string DefaultValue;
            /*
             * [ID] [bigint] IDENTITY(1,1) NOT NULL,
             * [Column1] [varchar](50) NULL,
             * [Column2] [nvarchar](50) NOT NULL,
             * [Column3] [int] NULL DEFAULT(0),
             * [Column4] [datetime] NULL,
             * [Column5] [decimal](18, 4) NULL,
             * [Column6] [uniqueidentifier] NULL,
             * [Column7] [bit] NULL,
             * [Column8] [smallint] NULL,
             * [Column9] [timestamp] NULL,
             * [Column10] [varchar](max) NULL,
             * [Column11] [datetime] NULL,
             */
            if (",varchar,nvarchar,".IndexOf("," + dbType + ",",StringComparison.OrdinalIgnoreCase) > -1)
            {
                tLength = "({0})".format(column.Length == 0 ? "50" : column.Length == -1 ? "max" : column.Length.ToString());
            }
            else if ("decimal".Equals(dbType, StringComparison.CurrentCultureIgnoreCase))
            {
                tLength = "({0},{1})".format(column.Length == 0 ? 18 : column.Length, column.Digit);
            }else if ("boolean".EqualsIgnoreCase(dbType))
            {
                dbType = "BIT";
            }
            if (column.AutoIncrement)
            {
                var defaultValue = column.DefaultValue.ToString().Trim('\'');
                switch (providerType)
                {
                    case DbProviderType.SqlServer:
                        dbType = "BIGINT";
                        tLength = " IDENTITY({0},1)".format(defaultValue.IsNumberic() ? defaultValue : "1");
                        break;
                    case DbProviderType.SQLite:
                        dbType = "INTEGER"; break;
                    default: dbType = "INTEGER"; break;
                }
                DefaultValue = "";
            }
            else
            {
                DefaultValue = "DEFAULT ({0})";
                string defaultValue;
                if (column.DefaultValue.IsNullOrEmpty() || column.DefaultValue.ToString().ToUpper() == "NULL")
                    defaultValue = "NULL";
                else if (column.DefaultValue.ToString().IsNumberic())
                    defaultValue = column.DefaultValue.ToString();
                else if (column.DefaultValue.ToString().IsMatch(@"(getdate|newid)"))
                {
                    defaultValue = column.DefaultValue.ToString();
                }
                else if (column.DefaultValue.ToString() == "''")
                    defaultValue = "''";
                else
                    defaultValue = GetDefaultValue(column.DefaultValue.ToString().Trim('\''), providerType);
                DefaultValue = DefaultValue.format(defaultValue);
            }
            return FieldFormat.format(column.Name, dbType, tLength, "{0} NULL".format(column.IsNullable ? "" : "NOT"), DefaultValue);
        }
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="providerType">驱动类型</param>
        /// <returns></returns>
        public string GetDefaultValue(string defaultValue,DbProviderType providerType)
        {
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
            return $"'{defaultValue}'";
        }
        #endregion
    }
}