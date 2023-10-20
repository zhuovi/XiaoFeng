using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using XiaoFeng;
/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-20 11:06:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// SimpleHash加密类
    /// </summary>
    public class SimpleHashEncryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SimpleHashEncryption()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7) => this.Encrypt(data, slatKey, 2, SHAType.MD5);
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="hashIterations">要加密的次数</param>
        /// <param name="type">类型</param>
        /// <returns>加密后的数据</returns>
        public byte[] Encrypt(byte[] data, int hashIterations, SHAType type = SHAType.MD5) => this.Encrypt(data, Array.Empty<byte>(), hashIterations, type);
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="salt">盐</param>
        /// <param name="hashIterations">要加密的次数</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data, byte[] salt, int hashIterations, SHAType type = SHAType.MD5)
        {
            if (hashIterations <= 0) hashIterations = 1;
            var hash = HashAlgorithm.Create(type.GetDescription());
            if (salt != null && salt.Length > 0)
                data = salt.Concat(data).ToArray();
            for (var i = 0; i < hashIterations; i++)
            {
                data = hash.ComputeHash(data);
            }
            hash.Dispose();
            return data;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="salt">盐</param>
        /// <param name="hashIterations">要加密的次数</param>
        /// <param name="type">类型</param>
        /// <param name="mode">输出编码</param>
        /// <returns></returns>
        public string Encrypt(string data, string salt, int hashIterations = 2, SHAType type = SHAType.MD5, OutputMode mode = OutputMode.Hex)
        {
            var encryptdata = this.Encrypt(data.GetBytes(), salt.GetBytes(), hashIterations, type);
            return this.OutputString(encryptdata, mode);
        }
        #endregion
    }
}