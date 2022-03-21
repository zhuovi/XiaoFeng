using System;
using System.Collections.Generic;
using System.IO;
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
 *  Create Time : 2020-12-23 下午 10:51:40          *
 *  Version : v 1.0.0                               *
 ***************************************************/

namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// RSA加密算法
    /// Version : 1.0.0
    /// CrateTime : 2020-12-23 下午 10:51:40
    /// Author : Jacky
    /// 更新说明
    /// </summary>
    public class RSAEncryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RSAEncryption()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7) => null;

        #region 产生 密钥
        /// <summary>
        /// RSA产生密钥
        /// </summary>
        public virtual Tuple<string, string> CreateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                var PrivateKey = rsa.ToXmlString(true);
                var PublicKey = rsa.ToXmlString(false);
                return new Tuple<string, string>(PrivateKey, PublicKey);
            }
        }
        /// <summary>
        /// RSA产生密钥
        /// </summary>
        public virtual Tuple<RSAParameters, RSAParameters> CreateKey()
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                var PrivateKey = rsa.ExportParameters(true);
                var PublicKey = rsa.ExportParameters(false);
                return new Tuple<RSAParameters, RSAParameters>(PrivateKey, PublicKey);
            }
        }
        #endregion

        #region 密码方法
        /// <summary>
        /// 密码方法
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">密钥</param>
        /// <param name="cryptographyType">密码类型</param>
        /// <returns></returns>
        public byte[] Encode(byte[] data, string key, CryptographyType cryptographyType)
        {
            if (data == null) return Array.Empty<byte>();
            using (var rsa = new RSACryptoServiceProvider())
            {
                if (key.IsNotNullOrEmpty())
                {
                    if (key.StartsWith("<RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
                        rsa.FromXmlString(key);
                    else
                        rsa.FromXmlString(this.GetString(key.FromBase64StringToBytes()));
                }
                var bytes = data;
                int bufferSize = (rsa.KeySize / 8);
                var buffer = new byte[bufferSize];
                using (MemoryStream input = new MemoryStream(bytes),
                     output = new MemoryStream())
                {
                    int readSize;
                    while ((readSize = input.Read(buffer, 0, bufferSize)) > 0)
                    {
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encrypt = cryptographyType == CryptographyType.Encrypt ? rsa.Encrypt(temp, false) : rsa.Decrypt(temp, false);
                        output.Write(encrypt, 0, encrypt.Length);
                    }
                    return output.ToArray();
                }
            }
        }
        #endregion

        #region 加密
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey, OutputMode outputMode) => this.OutputString(this.Encode(this.GetBytes(data), slatKey, CryptographyType.Encrypt), outputMode);
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public string Encrypt(string data, string publicKey) => this.Encrypt(data, publicKey, OutputMode.Base64);
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Encrypt(data, slatKey, outputMode);
        ///<inheritdoc/>
        [Obsolete("此加密无此方法")]
        public new byte[] Encrypt(byte[] data, string slatKey = "", string vectorKey = "") => null;
        #endregion

        #region 解密
        ///<inheritdoc/>
        public override string Decrypt(string data, string slatKey, OutputMode outputMode) => this.OutputString(this.Encode(this.InputBytes(data, outputMode), slatKey, CryptographyType.Decrypt));
        ///<inheritdoc/>
        public override string Decrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Base64) => this.Decrypt(data, slatKey, outputMode);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public string Decrypt(string data, string privateKey) => this.Decrypt(data, privateKey, OutputMode.Base64);
        ///<inheritdoc/>
        [Obsolete("此加密无此方法")]
        public new byte[] Decrypt(byte[] data, string slatKey = "", string vectorKey = "") => null;
        #endregion

        #endregion
    }
}