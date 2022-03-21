using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-12-28 11:51:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 分库分表模型
    /// </summary>
    public class SubDataBaseTable
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 数据库索引
        /// </summary>
        public uint? Num { get; set; }
        /// <summary>
        /// 分表后缀
        /// </summary>
        public string Suffix { get; set; }
    }
}