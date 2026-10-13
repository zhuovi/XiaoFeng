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
    /// 证书密钥算法类型
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:00:00
    /// </summary>
    public enum CertificateAlgorithmType
    {
        /// <summary>
        /// RSA算法
        /// </summary>
        [Description("RSA")]
        RSA = 0,
        /// <summary>
        /// 国密SM2算法
        /// </summary>
        [Description("SM2")]
        SM2 = 1
    }
}
