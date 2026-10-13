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
    /// 摘要算法类型
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:00:00
    /// Description : SM2算法必须使用SM3摘要，RSA建议使用SHA256。
    /// </summary>
    public enum DigestAlgorithmType
    {
        /// <summary>
        /// SHA1
        /// </summary>
        [Description("SHA1")]
        SHA1 = 0,
        /// <summary>
        /// SHA256
        /// </summary>
        [Description("SHA256")]
        SHA256 = 1,
        /// <summary>
        /// SHA384
        /// </summary>
        [Description("SHA384")]
        SHA384 = 2,
        /// <summary>
        /// SHA512
        /// </summary>
        [Description("SHA512")]
        SHA512 = 3,
        /// <summary>
        /// 国密SM3
        /// </summary>
        [Description("SM3")]
        SM3 = 4
    }
}
