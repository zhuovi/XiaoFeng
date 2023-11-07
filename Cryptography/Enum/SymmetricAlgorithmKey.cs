using System.ComponentModel;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021/12/7 18:50:48                             *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 加密类型
    /// </summary>
    public enum SymmetricAlgorithmType
    {
        /// <summary>
        /// AES
        /// </summary>
        [Description("AES")]
        AES = 0,
        /// <summary>
        /// DES
        /// </summary>
        [Description("DES")]
        DES = 1,
        /// <summary>
        /// 3DES
        /// </summary>
        [Description("3DES")]
        DES3 = 2
    }
}