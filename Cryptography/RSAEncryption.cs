using System;
using System.IO;
using System.Security.Cryptography;
using XiaoFeng.IO;
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
    /// 更新说明
    /// 加密用公钥   解密用私钥
    /// 签名用私钥   验证用公钥
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
        /// <summary>
        /// oid
        /// </summary>
        private readonly byte[] SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        /// <summary>
        /// 版本
        /// </summary>
        private readonly byte[] Ver = new byte[] { 0x02, 0x01, 0x00 };
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingModel = PaddingMode.PKCS7) => null;

        #region 产生 密钥
        /// <summary>
        /// RSA产生密钥
        /// </summary>
        /// <param name="keySize">密钥大小</param>
        public virtual Tuple<string, string> CreateKeys(int keySize = 1024)
        {
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                var PublicAndPrivateKey = rsa.ToXmlString(true);
                var PublicKey = rsa.ToXmlString(false);
                return new Tuple<string, string>(PublicAndPrivateKey, PublicKey);
            }
        }
        /// <summary>
        /// RSA产生密钥
        /// </summary>
        /// <param name="keySize">密钥大小</param>
        public virtual Tuple<string, string> CreateKey(int keySize = 1024)
        {
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                var PrivateKey = rsa.ExportParameters(true);
                var PublicKey = rsa.ExportParameters(false);
                return new Tuple<string, string>(ToBase64String(PrivateKey), ToBase64String(PublicKey));
            }
        }
        /// <summary>
        /// 创建KeyPEM
        /// </summary>
        /// <param name="keySize">密钥大小</param>
        /// <param name="pKCSType">PKCS类型</param>
        /// <returns></returns>
        public virtual Tuple<string, string> CreateKeyPEM(int keySize = 1024, PKCSType pKCSType = PKCSType.PKCS1)
        {
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
#if NETSTANDARD2_0
                var parameters = rsa.ExportParameters(true);
                return ToPEM(parameters, pKCSType);
#else
                var flag = pKCSType == PKCSType.PKCS1 ? " RSA" : "";
                var publicKey = $"-----BEGIN{flag} PUBLIC KEY-----\n{rsa.ExportRSAPublicKey().ToBase64String()}\n-----END{flag} PUBLIC KEY-----\n";
                var privateKey = $"-----BEGIN{flag} PRIVATE KEY-----\n{(pKCSType == PKCSType.PKCS1 ? rsa.ExportRSAPrivateKey() : rsa.ExportPkcs8PrivateKey()).ToBase64String()}\n-----END{flag} PRIVATE KEY-----\n";
                return new Tuple<string, string>(publicKey, privateKey);
#endif
            }
        }

        #region 把RSAParameters转成base64
        /// <summary>
        /// RSAParameters转Base64字符串
        /// </summary>
        /// <param name="parameters">RSAParameters</param>
        /// <returns></returns>
        public string ToBase64String(RSAParameters parameters)
        {
            var ms = new MemoryStream();
            ms.Write(parameters.Modulus);
            ms.Write(parameters.Exponent);

            if (parameters.D != null && parameters.D.Length > 0)
            {
                ms.Write(parameters.D);
                ms.Write(parameters.P);
                ms.Write(parameters.Q);
                ms.Write(parameters.DP);
                ms.Write(parameters.DQ);
                ms.Write(parameters.InverseQ);
            }

            return ms.ToArray().ToBase64String();
        }
        #endregion

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

            var service = new RSACryptoServiceProvider();
            //加密块最大长度
            int MaxBlockSize;
            if (key.StartsWith("<RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
                service.FromXmlString(key);
            else
                service.ImportParameters(FromPEM(key));

            if (cryptographyType == CryptographyType.Encrypt)
            {
                service.PersistKeyInCsp = false;
                MaxBlockSize = service.KeySize / 8 - 11;
                //如果明文长度小于等于单个加密块最大长度，直接单次调用加密接口完成加密
                if (data.Length <= MaxBlockSize)
                    return service.Encrypt(data, false);
            }
            else
            {
                MaxBlockSize = service.KeySize / 8;
                //如果明文长度小于等于单个加密块最大长度，直接单次调用加密接口完成加密
                if (data.Length <= MaxBlockSize)
                    return service.Decrypt(data, false);
            }
            using (var cipherStream = new MemoryStream())
            {
                using (var plainStream = new MemoryStream(data))
                {
                    var buffer = new byte[MaxBlockSize];
                    int readSize = plainStream.Read(buffer, 0, MaxBlockSize);
                    while (readSize > 0)
                    {
                        var plainBlock = new byte[readSize];
                        Array.Copy(buffer, 0, plainBlock, 0, readSize);
                        var cipherBlock = cryptographyType == CryptographyType.Encrypt ? service.Encrypt(plainBlock, false) : service.Decrypt(data, false);
                        cipherStream.Write(cipherBlock, 0, cipherBlock.Length);
                        readSize = plainStream.Read(buffer, 0, MaxBlockSize);
                    }
                }
                return cipherStream.ToArray();
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

        #region 读取Pem数据
        /// <summary>
        /// 获取RSAParameters
        /// </summary>
        /// <param name="content">PEM中的数据</param>
        /// <returns></returns>
        /// <exception cref="Exception">抛出异常</exception>
        public RSAParameters FromPEM(string content)
        {
            if (content.IsNullOrEmpty())
                throw new Exception("数据内容不能为空.");
            content = content.RemovePattern(@"[\r\n]+");
            if (content.StartsWith("-----BEGIN( RSA)? PUBLIC KEY-----", StringComparison.OrdinalIgnoreCase))
            {
                content = content.RemovePattern(@"-----(BEGIN|END)( RSA)? PUBLIC KEY-----");
                return FromPEMPublicKey(content);
            }
            else
            {
                content = content.RemovePattern(@"-----(BEGIN|END)( RSA)? PRIVATE KEY-----");
                return FromPEMPrivateKey(content);
            }
        }
        /// <summary>
        /// 转换成PEM
        /// </summary>
        /// <param name="parameters">赋值算法标准参数</param>
        /// <param name="pKCSType">类型</param>
        /// <returns></returns>
        public Tuple<string, string> ToPEM(RSAParameters parameters, PKCSType pKCSType)
        {
            var rsa = new RSAPEM(parameters);
            rsa.ToPEM(pKCSType);
            return new Tuple<string, string>(rsa.PublicKey.ToString(), rsa.PrivateKey.ToString());
        }
        /// <summary>
        /// 从PEM中公钥读取RSAParameters
        /// </summary>
        /// <param name="publicKey">PEM中的公钥</param>
        /// <returns></returns>
        private RSAParameters FromPEMPublicKey(string publicKey)
        {
            //移除干扰文本
            publicKey = publicKey.RemovePattern(@"-----(BEGIN|END)( RSA)? PUBLIC KEY-----").RemovePattern(@"[\r\n]+");

            byte[] keyData = publicKey.FromBase64StringToBytes();
            bool keySize1024 = keyData.Length == 162;
            bool keySize2048 = keyData.Length == 294;
            if (!(keySize1024 || keySize2048))
                throw new Exception("公钥长度只支持1024和2048。");
            byte[] pemModulus = keySize1024 ? new byte[128] : new byte[256];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, keySize1024 ? 29 : 33, pemModulus, 0, keySize1024 ? 128 : 256);
            Array.Copy(keyData, keySize1024 ? 159 : 291, pemPublicExponent, 0, 3);
            return new RSAParameters
            {
                Modulus = pemModulus,
                Exponent = pemPublicExponent
            };
        }
        /// <summary>
        /// 从PEM中私钥读取RSAParameters
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        private RSAParameters FromPEMPrivateKey(string privateKey)
        {
            var parameters = new RSAParameters();
            var key = privateKey.GetBytes();
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;

            //设置流以解码asn.1个编码的RSA私钥
            //使用BinaryReader包装内存流，便于阅读
            using (var binaryReader = new BinaryReader(new MemoryStream(key)))
            {
                twobytes = binaryReader.ReadUInt16();
                //以小端顺序读取的数据（序列的实际数据顺序为30 81）
                if (twobytes == 0x8130)
                {
                    binaryReader.ReadByte();
                }
                else if (twobytes == 0x8230)
                {
                    binaryReader.ReadInt16();
                }
                else
                {
                    return parameters;
                }

                twobytes = binaryReader.ReadUInt16();
                //版本号
                if (twobytes != 0x0102)
                {
                    return parameters;
                }
                bt = binaryReader.ReadByte();
                if (bt != 0x00)
                {
                    return parameters;
                }

                //所有私钥组件都是整数序列
                elems = GetIntegerSize(binaryReader);
                MODULUS = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                E = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                D = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                P = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                Q = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                DP = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                DQ = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                IQ = binaryReader.ReadBytes(elems);

                return new RSAParameters
                {
                    Modulus = MODULUS,
                    Exponent = E,
                    D = D,
                    P = P,
                    Q = Q,
                    DP = DP,
                    DQ = DQ,
                    InverseQ = IQ
                };
            }
        }
        /// <summary>
        /// 获取整型长度
        /// </summary>
        /// <param name="binaryReader">二进制流</param>
        /// <returns></returns>
        private int GetIntegerSize(BinaryReader binaryReader)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;

            bt = binaryReader.ReadByte();

            //expect integer
            if (bt != 0x02)
            {
                return 0;
            }
            bt = binaryReader.ReadByte();

            if (bt == 0x81)
            {
                //data size in next byte
                count = binaryReader.ReadByte();
            }
            else if (bt == 0x82)
            {
                //data size in next 2 bytes
                highbyte = binaryReader.ReadByte();
                lowbyte = binaryReader.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                //we already have the data size
                count = bt;
            }
            while (binaryReader.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            //last ReadByte wasn't a removed zero, so back up a byte
            binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
        #endregion

        #region 签名
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <returns>签名数据</returns>
        public byte[] SignData(byte[] data, string privateKey, HashAlgorithmName hashAlgorithmName)
        {
            if (data.IsNullOrEmpty()) return Array.Empty<byte>();
            var service = new RSACryptoServiceProvider();
            if (privateKey.StartsWith("<RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
                service.FromXmlString(privateKey);
            else
                service.ImportParameters(FromPEM(privateKey));

            return service.SignData(data, HashAlgorithm.Create(hashAlgorithmName.Name));
        }
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data">要进行哈希处理和签名的输入数据。</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <returns>签名数据</returns>
        public byte[] SignData(byte[] data, byte[] privateKey, HashAlgorithmName hashAlgorithmName)
        {
            if (data.IsNullOrEmpty()) return Array.Empty<byte>();
            var service = new RSACryptoServiceProvider();
            service.ImportCspBlob(privateKey);

            return service.SignData(data, HashAlgorithm.Create(hashAlgorithmName.Name));
        }
        #endregion

        #region 验证
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="data">已签名的数据。</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="signature">要验证的签名数据</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <returns>是否一致</returns>
        public Boolean VerifyData(byte[] data, string publicKey, byte[] signature, HashAlgorithmName hashAlgorithmName)
        {
            if (data.IsNullOrEmpty()) return false;
            var service = new RSACryptoServiceProvider();
            if (publicKey.StartsWith("<RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
                service.FromXmlString(publicKey);
            else
                service.ImportParameters(FromPEM(publicKey));

            return service.VerifyData(data, HashAlgorithm.Create(hashAlgorithmName.Name), signature);
        }
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="data">已签名数据</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="signature">要验证的签名数据</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <returns>是否一致</returns>
        public Boolean VerifyData(byte[] data, byte[] publicKey, byte[] signature, HashAlgorithmName hashAlgorithmName)
        {
            if (data.IsNullOrEmpty()) return false;
            var service = new RSACryptoServiceProvider();
            service.ImportCspBlob(publicKey);
            return service.VerifyData(data, HashAlgorithm.Create(hashAlgorithmName.Name), signature);
        }
        #endregion

        #region Hash签名
        /// <summary>
        /// 使用指定的填充计算指定的哈希值的签名。
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <param name="signaturePadding">指定要用于 RSA 签名创建或验证操作的填充模式和参数。</param>
        /// <returns>是否一致</returns>
        public byte[] SignHash(byte[] data, string publicKey, HashAlgorithmName hashAlgorithmName, RSASignaturePadding signaturePadding)
        {
            if (data.IsNullOrEmpty()) return Array.Empty<byte>();
            var service = new RSACryptoServiceProvider();
            if (publicKey.StartsWith("<RSAKeyValue>", StringComparison.OrdinalIgnoreCase))
                service.FromXmlString(publicKey);
            else
                service.ImportParameters(FromPEM(publicKey));

            return service.SignHash(data, hashAlgorithmName, signaturePadding);
        }
        #endregion

        #region Hash签名验证
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="data">已签名数据</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="signature">要验证的签名数据</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <param name="signaturePadding">指定要用于 RSA 签名创建或验证操作的填充模式和参数。</param>
        /// <returns>是否一致</returns>
        public Boolean VerifyData(byte[] data, byte[] publicKey, byte[] signature, HashAlgorithmName hashAlgorithmName, RSASignaturePadding signaturePadding)
        {
            if (data.IsNullOrEmpty()) return false;
            var service = new RSACryptoServiceProvider();
            service.ImportCspBlob(publicKey);
            return service.VerifyHash(data, signature, hashAlgorithmName, signaturePadding);
        }
        #endregion

        #endregion
    }
}