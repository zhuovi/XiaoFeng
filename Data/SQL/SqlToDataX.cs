using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng;
namespace XiaoFeng.Data.SQL
{
    /*
    ===================================================================
       Author : jacky
       Email : jacky@zhuovi.com
       QQ : 7092734
       Site : www.zhuovi.com
       Create Time : 2017/12/22 9:34:53
       Update Time : 2017/12/22 9:34:53
    ===================================================================
    */
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