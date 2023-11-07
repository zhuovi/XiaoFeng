using System.ComponentModel;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-11-17 15:29:59                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    #region 密码类型
    /// <summary>
    /// 密码类型
    /// </summary>
    public enum CryptographyType
    {
        /// <summary>
        /// 加密
        /// </summary>
        [Description("加密")]
        Encrypt = 0,
        /// <summary>
        /// 解密
        /// </summary>
        [Description("解密")]
        Decrypt = 1
    }
    #endregion
}