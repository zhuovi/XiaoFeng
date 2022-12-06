using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using XiaoFeng.Collections;
using XiaoFeng.Data.SQL;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 11:05:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.Pool
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class DataHelper : DbBase
    {
        #region 构造器
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="config">配置</param>
        public DataHelper(ConnectionConfig config) : base(config) { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}