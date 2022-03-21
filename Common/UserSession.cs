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
*  Create Time : 2021-12-29 10:21:53                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 会话对象
    /// </summary>
    public class UserSession
    {
        #region 构造器

        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        private static string ID { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        private static string TokenName => "FAYELF_USER_SESSIONID";
        #endregion

        #region 方法
        /// <summary>
        /// 获取用户会话ID
        /// </summary>
        public static string Id
        {
            get
            {
                if (OS.Platform.IsWebForm)
                {
                    var UserID = Guid.NewGuid().ToString("N");
                    if (Web.HttpContext.Current != null)
                    {
                        var _TokenID = Web.HttpCookie.Get(TokenName);
                        if (_TokenID.IsNotNullOrEmpty()) return _TokenID;
                        var session = Web.HttpContext.Current.Session;
                        if (session != null) UserID = session.
#if NETCORE
                                    Id
#else
                                    SessionID
#endif
                                ;
                        Web.HttpCookie.Set(TokenName, UserID, false);
                        return UserID;
                    }
                    else return string.Empty;
                }
                else
                {
                    if (ID.IsNullOrEmpty()) ID = Guid.NewGuid().ToString("N");
                    return ID;
                }
            }
        }
#endregion
    }
}