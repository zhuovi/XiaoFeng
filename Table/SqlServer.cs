﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// SqlServer
    /// </summary>
    public class SqlServer : BaseTable, ITable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SqlServer() { this.Config = new ConnectionConfig { ProviderType = DbProviderType.SqlServer }; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="config"></param>
        public SqlServer(ConnectionConfig config)
        {
            this.Config = config;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public ConnectionConfig Config { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="modelType">表类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串</param>
        /// <param name="connIndex">索引</param>
        /// <returns></returns>
        public string Create(Type modelType, string tableName = "", string connName = "", int connIndex = -1)
        {
            var type = modelType;
            var table = modelType.GetTableAttribute(false);
            var view = modelType.GetViewAttribute(false);
            if (table == null) return String.Empty;
            var data = new DataHelper(this.Config);
            if (view != null && table.ModelType == ModelType.View)
            {
                if (view.Definition.IsNullOrEmpty()) return table.Name + "$2";
                else
                {
                    var count = data.ExecuteScalar($@"SELECT count(0) FROM sys.sql_modules AS m INNER JOIN sys.all_objects AS o ON m.object_id = o.object_id WHERE o.[type] = 'v' and o.Name = '{table.Name}'").ToCast<int>();
                    if (count > 0) return table.Name + "$0";
                    else data.ExecuteNonQuery($@"CREATE VIEW [dbo].[{table.Name}] AS
    {view.Definition.ReplacePattern(@"ifnull", "ISNULL")};");
                    return table.Name + "$1";
                }
            }
            var SqlFormat = @"
IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE ID = OBJECT_ID(N'[dbo].[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN 
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='PK_{0}')
    DROP INDEX {0}.PK_{0};
IF EXISTS (SELECT * FROM SYSINDEXES WHERE NAME='IX_{0}')
    DROP INDEX {0}.IX_{0};

CREATE TABLE [dbo].[{0}](
{1}
) ON [PRIMARY];

CREATE NONCLUSTERED INDEX IX_{0}
ON {0}({2});

{3}

SELECT 1;
END;
";
            table = base.GetTableAttribute(type, tableName, connName, connIndex);
            var dbType = new DataType(DbProviderType.SqlServer);
            var Fields = "";
            //var PrimaryKey = "";
            var Indexs = new List<string>();
            var Description = "";
            var Unique = "";
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).Each(p =>
            {
                if (p.GetCustomAttribute<FieldIgnoreAttribute>() != null) return;
                var columnAttr = p.GetColumnAttribute() ?? new ColumnAttribute();
                if (columnAttr.Name.IsNullOrEmpty()) columnAttr.Name = p.Name;
                if (columnAttr.DataType.IsNullOrEmpty()) columnAttr.DataType = dbType[p.PropertyType];
                if (columnAttr.Description.IsNullOrEmpty()) columnAttr.Description = p.Name;
                Fields += this.GetField(columnAttr);
                if (columnAttr.IsIndex) Indexs.Add(columnAttr.Name);
                if (columnAttr.PrimaryKey)
                {
                    //PrimaryKey = $"[{columnAttr.Name}] ASC,";
                    if (!Indexs.Contains(columnAttr.Name)) Indexs.Add(columnAttr.Name);
                }
                if (columnAttr.IsUnique) Unique += columnAttr.Name + ",";
                Description += @"
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{0}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{1}', @level2type=N'COLUMN',@level2name=N'{2}';{3}".format(columnAttr.Description, table.Name, columnAttr.Name, Environment.NewLine);
            });
            //PrimaryKey = PrimaryKey.TrimEnd(',');
            Unique = Unique.TrimEnd(',');
            if (Unique.IsNotNullOrEmpty()) Fields += "CONSTRAINT [UN_{0}] UNIQUE ({1}),{2}".format(table.Name, Unique, Environment.NewLine);
            //if (PrimaryKey.IsNullOrEmpty()) PrimaryKey = "ID";
            //if (PrimaryKey.IsNotNullOrEmpty()) Fields += @"CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED({1})
            //WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]".format(table.Name, PrimaryKey);
            SqlFormat = SqlFormat.format(table.Name, Fields, Indexs.Join(","), Description);
            if (table.ConnName.IsMatch(@"(^ZW:|;)"))
            {
                this.Config.ConnectionString = table.ConnName;
            }
            else
            {
                var configs = XiaoFeng.Config.DataBase.Current[table.ConnName];
                if (configs.Count > table.ConnIndex)
                    this.Config = configs[table.ConnIndex];
                else if (this.Config.ConnectionString.IsNullOrEmpty())
                    this.Config = configs.First();
            }
            var result = data.ExecuteScalar(SqlFormat);
            return table.Name + "$" + result;
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串</param>
        /// <param name="connIndex">索引</param>
        /// <returns></returns>
        public string Create<T>(string tableName = "", string connName = "", int connIndex = -1) => this.Create(typeof(T), tableName, connName, connIndex);
        /// <summary>
        /// 获取字段定义
        /// </summary>
        /// <param name="column">字段配置</param>
        /// <returns></returns>
        public string GetField(ColumnAttribute column)
        {
            return base.GetField(column, DbProviderType.SqlServer);
        }
        #endregion
    }
}