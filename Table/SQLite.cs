using System;
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
    /// SQLite
    /// </summary>
    public class SQLite : BaseTable, ITable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SQLite() { this.Config = new ConnectionConfig { ProviderType = DbProviderType.SQLite }; }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="config"></param>
        public SQLite(ConnectionConfig config)
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
                    var count = data.ExecuteScalar($@"SELECT COUNT(0) FROM sqlite_master WHERE type='view' and name='{table.Name}';").ToCast<int>();
                    if (count > 0) return table.Name + "$0";
                    else data.ExecuteNonQuery($@"CREATE VIEW {table.Name} AS
    {view.Definition};");
                    return table.Name + "$1";
                }
            }
            var SqlFormat = @"
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- 表：{0}
DROP TABLE IF EXISTS {0};
-- 创建表
CREATE TABLE {0} (
    {1}
);
-- 创建索引
{2}

SELECT 1;
COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
";
            table = base.GetTableAttribute(type, tableName, connName, connIndex);
            var dbType = new DataType(this.Config.ProviderType);
            var Fields = "";
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
                //Fields += new FieldPacket(table, this.Config.ProviderType, columnAttr).ToString();
                if (columnAttr.IsIndex) Indexs.Add(columnAttr.Name);
                if (columnAttr.PrimaryKey)
                {
                    //PrimaryKey = $"{columnAttr.Name} PRIMARY KEY ASC,";
                    if (!Indexs.Contains(columnAttr.Name)) Indexs.Add(columnAttr.Name);
                }
                if (columnAttr.IsUnique) Unique += columnAttr.Name + ",";
            });
            SqlFormat = SqlFormat.format(table.Name, Fields.TrimEnd(','), Indexs.Join(","), Description);
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
            return base.GetField(column, DbProviderType.SQLite);
        }
        #endregion
    }
}