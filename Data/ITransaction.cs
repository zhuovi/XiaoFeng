using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-06-29 09:11:53                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库操作事务接口
    /// </summary>
    public interface IDbTransactionX
    {
        /// <summary>
        /// 事务级别
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }
        /// <summary>
        /// 事务
        /// </summary>
        DbTransaction Transaction { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        Boolean IsOpen { get; set; }
        /// <summary>
        /// 创建事务
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();
        /// <summary>
        /// 回流事务
        /// </summary>
        void Rollback();
        /// <summary>
        /// 结束事务
        /// </summary>
        void EndTransaction();
        /// <summary>
        /// 创建命令
        /// </summary>
        /// <returns></returns>
        DbCommand CreateCommand();
    }
}