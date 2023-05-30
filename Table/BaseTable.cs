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

        #region 获取字段定义
        /// <summary>
        /// 获取字段定义
        /// </summary>
        /// <param name="column">字段配置</param>
        /// <param name="providerType">驱动类型</param>
        /// <returns></returns>
        public virtual string GetField(ColumnAttribute column,DbProviderType providerType)
        {
            if (column == null) return "";
            var FieldFormat = "{0}  {1}{2}  {3} {4}," + Environment.NewLine;
            var dbType = column.DataType.ToString();
            var tLength = "";
            var NotNull = "{0} NULL".format(column.IsNullable ? "" : "NOT");
            string DefaultValue;
            /*
             * SQLSERVER 
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
             * 
             * SQLite
             * 
             * ID           INTEGER       CONSTRAINT ID PRIMARY KEY ASC ON CONFLICT ROLLBACK AUTOINCREMENT
             *                            CONSTRAINT ID UNIQUE ON CONFLICT ROLLBACK
             *                            NOT NULL ON CONFLICT ROLLBACK,
             * Name         VARCHAR (50)  NOT NULL ON CONFLICT ROLLBACK,
             * ParentID     INTEGER       DEFAULT (0),
             * Icon         VARCHAR (200),
             * Url          VARCHAR (200),
             * AuthID       VARCHAR (50),
             * SortID       INTEGER       DEFAULT (100000),
             * Pass         BOOLEAN       DEFAULT (1),
             * IsDelete     BOOLEAN       DEFAULT (0),
             * AddDate      DATETIME      DEFAULT (datetime('now', 'localtime') ),
             * AddTimeStamp INTEGER       DEFAULT (strftime('%s', 'now') - strftime('%s', '2023-01-01') ) 
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
            if (column.PrimaryKey)
            {
                var defaultValue = column.DefaultValue.ToString().Trim('\'');
                switch (providerType)
                {
                    case DbProviderType.SqlServer:
                        dbType = "BIGINT";
                        if (column.AutoIncrement)
                            tLength = " IDENTITY({0},{1})".format(defaultValue.IsNumberic() ? defaultValue : "1", column.AutoIncrementStep);
                        NotNull = " Primary key ";
                        break;
                    case DbProviderType.SQLite:
                        dbType = "INTEGER";
                        if (column.AutoIncrement)
                            NotNull = @"CONSTRAINT ID PRIMARY KEY ASC ON CONFLICT ROLLBACK AUTOINCREMENT
                CONSTRAINT ID UNIQUE ON CONFLICT ROLLBACK
                NOT NULL ON CONFLICT ROLLBACK";
                        else
                            NotNull = "AUTOINCREMENT";
                        break;
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
                //else
                    //defaultValue = GetDefaultValue(column.DefaultValue.ToString().Trim('\''), providerType);
                //DefaultValue = DefaultValue.format(defaultValue);
            }
            return FieldFormat.format(column.Name, dbType, tLength, NotNull, DefaultValue);
        }
        #endregion

        

        #endregion
    }
}