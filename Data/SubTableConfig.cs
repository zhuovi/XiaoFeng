using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-30 10:17:43                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 分表配置
    /// </summary>
    [ConfigFile("Config/SubTables.json", 0, "FAYELF-CONFIG-SUBTABLES", ConfigFormat.Json)]
    public class SubTableConfig : ConfigSets<SubTableConfig>
    {
        #region 属性
        /// <summary>
        /// 表类型
        /// </summary>
        public Type TableType { get; set; }
        /// <summary>
        /// 分表字段
        /// </summary>
        public string FieldName { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取分表名称
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <param name="defaultName">默认表名</param>
        /// <returns></returns>
        public string GetTableName(string suffix, string defaultName)
        {
            if (this.TableType == null || suffix.IsNullOrEmpty()) return defaultName;
            var tableAttr = this.TableType.GetTableAttribute();
            var tableName = tableAttr == null ? this.TableType.Name : tableAttr.Name;
            return $"{tableName}_FB_{suffix}";
        }
        #endregion
    }
}