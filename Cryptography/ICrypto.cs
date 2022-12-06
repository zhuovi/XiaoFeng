/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// 加密解密接口
    /// </summary>
    interface ICrypto
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        string Encrypt(string data, string key = "");
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        string Decrypt(string data, string key = "");
    }
}