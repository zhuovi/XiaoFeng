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
    /// 证书类型
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:00:00
    /// </summary>
    public enum CertificateType
    {
        /// <summary>
        /// 根证书 自签名证书
        /// </summary>
        [Description("根证书")]
        Root = 0,
        /// <summary>
        /// 中级证书 由根证书签发
        /// </summary>
        [Description("中级证书")]
        Intermediate = 1,
        /// <summary>
        /// 终端证书 用于签名或加密的叶子证书
        /// </summary>
        [Description("终端证书")]
        EndEntity = 2
    }
}
