using System;
using System.ComponentModel;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:00:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// 证书状态
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:00:00
    /// </summary>
    public enum CertificateStatus
    {
        /// <summary>
        /// 未知 证书不存在
        /// </summary>
        [Description("未知")]
        Unknown = -1,
        /// <summary>
        /// 有效
        /// </summary>
        [Description("有效")]
        Valid = 0,
        /// <summary>
        /// 已过期
        /// </summary>
        [Description("已过期")]
        Expired = 1,
        /// <summary>
        /// 已注销 已吊销
        /// </summary>
        [Description("已注销")]
        Revoked = 2,
        /// <summary>
        /// 已到期需续期 即将过期
        /// </summary>
        [Description("即将过期")]
        Expiring = 3,
        /// <summary>
        /// 已被新证书替换 续期后旧证书状态
        /// </summary>
        [Description("已被替换")]
        Superseded = 4,
        /// <summary>
        /// 未生效
        /// </summary>
        [Description("未生效")]
        Inactive = 5
    }
}
