using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved. *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-12-23 上午 01:52:56          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// MD5加密方法
    /// Version : 1.0.0
    /// CrateTime : 2020-12-23 上午 01:52:56
    /// 更新说明
    /// </summary>
    public class MD5Encryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MD5Encryption()
        {
        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7)
        {
            if (data == null) return Array.Empty<byte>();
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(data);
            }
        }
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="slatKey">16和32</param>
        /// <returns></returns>
        public string Encrypts(byte[] data,string slatKey = "32")
        {
            var _data = this.Encode(data, null, null);
            return (slatKey.IsNullOrEmpty() || slatKey == "32") ? BitConverter.ToString(_data, 0, 16).Replace("-", "") : BitConverter.ToString(_data, 4, 8);
        }
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64)
        {
            var _data = this.Encode(this.GetBytes(data), null, null);
            return (slatKey.IsNullOrEmpty() || slatKey == "32") ? BitConverter.ToString(_data, 0, 16).Replace("-", "") : BitConverter.ToString(_data, 4, 8);
        }
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="slatKey">16是16位长度 32是32位长度</param>
        /// <returns></returns>
        public string Encrypt(string data, string slatKey) => this.Encrypt(data, slatKey, "");
        #endregion
    }
}