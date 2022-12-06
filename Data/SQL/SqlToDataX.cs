using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-22 09:32:07                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region 项目实例
    /// <summary>
    /// 项目实例
    /// </summary>
    public class ProgramNameDataX
    {
        /// <summary>
        /// 回调事件
        /// </summary>
        public static RunSQLEventHandler CallBack;
        /// <summary>
        /// 操作对象
        /// </summary>
        private static DataHelperX _X;
        /// <summary>
        /// 操作对象
        /// </summary>
        public static DataHelperX X
        {
            get
            {
                if (_X == null) _X = new DataHelperX(new ConnectionConfig
                {
                    ConnectionString = "",
                    ProviderType = DbProviderType.SqlServer,
                    IsTransaction = true
                }, e =>
                 {
                     if (CallBack != null) CallBack.Invoke(e);
                 });
                return _X;
            }
        }
    }
    #endregion
}