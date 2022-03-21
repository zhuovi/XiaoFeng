using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Data
{
    /// <summary>
    /// 命令列表类
    /// </summary>
    public class CommandList
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CommandList() { }
        #endregion

        #region 属性
        /// <summary>
        /// CommandText
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 储存参数
        /// </summary>
        public DbParameter[] Parameters { get; set; }
        /// <summary>
        /// 数据库连接字符
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbProviderType ProviderType { get; set; }
        #endregion
    }
}