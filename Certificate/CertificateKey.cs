using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using XiaoFeng.Asn1;
using XiaoFeng.Cryptography;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:20:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// 数字证书密钥
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:20:00
    /// Description : 支持RSA与国密SM2算法的密钥对，提供签名、验签、加密、解密、PEM导入导出、证书公钥信息编码等功能。
    /// 私钥存储格式为内部文本格式，私钥对外导出为标准PKCS#8 PEM格式。
    /// </summary>
    public class CertificateKey
    {
        #region 常量
        /// <summary>
        /// RSA加密算法OID
        /// </summary>
        public const string OidRsaEncryption = "1.2.840.113549.1.1.1";
        /// <summary>
        /// SHA1WithRSA签名算法OID
        /// </summary>
        public const string OidSha1WithRsaEncryption = "1.2.840.113549.1.1.5";
        /// <summary>
        /// SHA256WithRSA签名算法OID
        /// </summary>
        public const string OidSha256WithRsaEncryption = "1.2.840.113549.1.1.11";
        /// <summary>
        /// SHA384WithRSA签名算法OID
        /// </summary>
        public const string OidSha384WithRsaEncryption = "1.2.840.113549.1.1.12";
        /// <summary>
        /// SHA512WithRSA签名算法OID
        /// </summary>
        public const string OidSha512WithRsaEncryption = "1.2.840.113549.1.1.13";
        /// <summary>
        /// 椭圆曲线公钥算法OID
        /// </summary>
        public const string OidEcPublicKey = "1.2.840.10045.2.1";
        /// <summary>
        /// SM2椭圆曲线OID sm2p256v1
        /// </summary>
        public const string OidSm2Curve = "1.2.156.10197.1.301";
        /// <summary>
        /// SM2签名算法OID sm2-with-sm3
        /// </summary>
        public const string OidSm2WithSm3 = "1.2.156.10197.1.501";
        #endregion

        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CertificateKey()
        {
        }
        /// <summary>
        /// 设置密钥
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="publicKey">公钥</param>
        public CertificateKey(string privateKey, string publicKey)
        {
            this.PrivateKey = privateKey ?? "";
            this.PublicKey = publicKey ?? "";
            var type = (this.PrivateKey.IsNotNullOrEmpty() ? this.PrivateKey : this.PublicKey).Split('|')[0];
            this.AlgorithmType = type.EqualsIgnoreCase("SM2") ? CertificateAlgorithmType.SM2 : CertificateAlgorithmType.RSA;
            var parts = (this.PrivateKey.IsNotNullOrEmpty() ? this.PrivateKey : this.PublicKey).Split('|');
            if (parts.Length > 1) this.KeySize = parts[1].ToCast(0);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 密钥算法类型
        /// </summary>
        public CertificateAlgorithmType AlgorithmType { get; set; } = CertificateAlgorithmType.RSA;
        /// <summary>
        /// 密钥长度
        /// </summary>
        public int KeySize { get; set; } = 2048;
        /// <summary>
        /// 私钥 内部格式
        /// </summary>
        public string PrivateKey { get; set; } = "";
        /// <summary>
        /// 公钥 内部格式
        /// </summary>
        public string PublicKey { get; set; } = "";
        /// <summary>
        /// SM2用户ID 签名时参与摘要计算
        /// </summary>
        public string UserId { get; set; } = SM2Encryption.DefaultUserId;
        /// <summary>
        /// 是否包含私钥
        /// </summary>
        public Boolean HasPrivateKey => this.PrivateKey.IsNotNullOrEmpty() && this.PrivateKey.Split('|').Length >= 5;
        /// <summary>
        /// 默认摘要算法
        /// </summary>
        public DigestAlgorithmType DefaultDigest => GetDefaultDigest(this.AlgorithmType);
        #endregion

        #region 方法

        #region 创建密钥
        /// <summary>
        /// 创建密钥
        /// </summary>
        /// <param name="algorithmType">算法类型</param>
        /// <param name="keySize">密钥长度 RSA默认2048 SM2默认256</param>
        /// <returns>证书密钥</returns>
        public static CertificateKey Create(CertificateAlgorithmType algorithmType, int keySize = 0)
        {
            var key = new CertificateKey { AlgorithmType = algorithmType };
            if (algorithmType == CertificateAlgorithmType.SM2)
            {
                var pair = SM2Encryption.GenerateKeyPair();
                key.KeySize = 256;
                var x = pair.PublicKey.Substring(2, 64);
                var y = pair.PublicKey.Substring(66, 64);
                key.PrivateKey = "SM2|256|" + pair.PrivateKey + "|" + x + "|" + y;
                key.PublicKey = "SM2|256|" + x + "|" + y;
            }
            else
            {
                if (keySize <= 0) keySize = 2048;
                using (var rsa = RSA.Create())
                {
                    rsa.KeySize = keySize;
                    var parameters = rsa.ExportParameters(true);
                    key.KeySize = rsa.KeySize;
                    key.PrivateKey = string.Join("|", new string[]
                    {
                        "RSA", key.KeySize.ToString(),
                        Convert.ToBase64String(parameters.Modulus),
                        Convert.ToBase64String(parameters.Exponent),
                        Convert.ToBase64String(parameters.D),
                        Convert.ToBase64String(parameters.P),
                        Convert.ToBase64String(parameters.Q),
                        Convert.ToBase64String(parameters.DP),
                        Convert.ToBase64String(parameters.DQ),
                        Convert.ToBase64String(parameters.InverseQ)
                    });
                    key.PublicKey = "RSA|" + key.KeySize + "|" + Convert.ToBase64String(parameters.Modulus) + "|" + Convert.ToBase64String(parameters.Exponent);
                }
            }
            return key;
        }
        #endregion

        #region 签名
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>签名数据 RSA为PKCS#1签名 SM2为DER格式签名</returns>
        public byte[] Sign(byte[] data, DigestAlgorithmType digest = DigestAlgorithmType.SHA256)
        {
            if (data == null) return Array.Empty<byte>();
            if (!this.HasPrivateKey) throw new Exception("当前密钥不包含私钥，无法进行签名操作.");
            if (this.AlgorithmType == CertificateAlgorithmType.SM2)
            {
                var parts = this.PrivateKey.Split('|');
                return SM2Encryption.SignToDer(data, parts[2], this.GetSM2PublicKey(), this.UserId);
            }
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(this.GetRsaParameters(true));
                return rsa.SignData(data, GetHashAlgorithmName(digest), RSASignaturePadding.Pkcs1);
            }
        }
        /// <summary>
        /// 签名并转成Base64字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>Base64签名值</returns>
        public string SignBase64(byte[] data, DigestAlgorithmType digest = DigestAlgorithmType.SHA256) => Convert.ToBase64String(this.Sign(data, digest));
        #endregion

        #region 验签
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">签名数据</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>是否一致</returns>
        public Boolean Verify(byte[] data, byte[] signature, DigestAlgorithmType digest = DigestAlgorithmType.SHA256)
        {
            if (data == null || signature == null || signature.Length == 0) return false;
            if (this.AlgorithmType == CertificateAlgorithmType.SM2)
                return SM2Encryption.VerifyDer(data, signature, this.GetSM2PublicKey(), this.UserId);
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(this.GetRsaParameters(false));
                return rsa.VerifyData(data, signature, GetHashAlgorithmName(digest), RSASignaturePadding.Pkcs1);
            }
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">Base64签名值</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>是否一致</returns>
        public Boolean VerifyBase64(byte[] data, string signature, DigestAlgorithmType digest = DigestAlgorithmType.SHA256)
        {
            if (signature.IsNullOrEmpty()) return false;
            return this.Verify(data, Convert.FromBase64String(signature), digest);
        }
        #endregion

        #region 加密
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <returns>密文</returns>
        public byte[] Encrypt(byte[] data)
        {
            if (data == null || data.Length == 0) return Array.Empty<byte>();
            if (this.AlgorithmType == CertificateAlgorithmType.SM2)
                return SM2Encryption.Encrypt(data, this.GetSM2PublicKey());
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(this.GetRsaParameters(false));
                return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }
        #endregion

        #region 解密
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <returns>明文</returns>
        public byte[] Decrypt(byte[] data)
        {
            if (data == null || data.Length == 0) return Array.Empty<byte>();
            if (!this.HasPrivateKey) throw new Exception("当前密钥不包含私钥，无法进行解密操作.");
            if (this.AlgorithmType == CertificateAlgorithmType.SM2)
                return SM2Encryption.Decrypt(data, this.PrivateKey.Split('|')[2]);
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(this.GetRsaParameters(true));
                return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }
        #endregion

        #region 摘要
        /// <summary>
        /// 计算摘要值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>摘要字节</returns>
        public static byte[] ComputeDigest(byte[] data, DigestAlgorithmType digest)
        {
            if (data == null) data = Array.Empty<byte>();
            switch (digest)
            {
                case DigestAlgorithmType.SM3:
                    return SM3Encryption.Hash(data);
                case DigestAlgorithmType.SHA1:
                    using (var sha1 = SHA1.Create()) return sha1.ComputeHash(data);
                case DigestAlgorithmType.SHA384:
                    using (var sha384 = SHA384.Create()) return sha384.ComputeHash(data);
                case DigestAlgorithmType.SHA512:
                    using (var sha512 = SHA512.Create()) return sha512.ComputeHash(data);
                default:
                    using (var sha256 = SHA256.Create()) return sha256.ComputeHash(data);
            }
        }
        /// <summary>
        /// 计算摘要值并转成十六进制字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>摘要十六进制字符串</returns>
        public static string ComputeDigestHex(byte[] data, DigestAlgorithmType digest) => SM2Encryption.ToHex(ComputeDigest(data, digest));
        /// <summary>
        /// 获取默认摘要算法
        /// </summary>
        /// <param name="algorithmType">密钥算法类型</param>
        /// <returns>摘要算法</returns>
        public static DigestAlgorithmType GetDefaultDigest(CertificateAlgorithmType algorithmType) => algorithmType == CertificateAlgorithmType.SM2 ? DigestAlgorithmType.SM3 : DigestAlgorithmType.SHA256;
        /// <summary>
        /// 获取摘要算法名称
        /// </summary>
        /// <param name="digest">摘要算法</param>
        /// <returns>摘要算法名称</returns>
        public static HashAlgorithmName GetHashAlgorithmName(DigestAlgorithmType digest)
        {
            switch (digest)
            {
                case DigestAlgorithmType.SHA1: return HashAlgorithmName.SHA1;
                case DigestAlgorithmType.SHA384: return HashAlgorithmName.SHA384;
                case DigestAlgorithmType.SHA512: return HashAlgorithmName.SHA512;
                default: return HashAlgorithmName.SHA256;
            }
        }
        /// <summary>
        /// 获取签名算法OID
        /// </summary>
        /// <param name="algorithmType">密钥算法</param>
        /// <param name="digest">摘要算法</param>
        /// <returns>签名算法OID</returns>
        public static string GetSignatureAlgorithmOid(CertificateAlgorithmType algorithmType, DigestAlgorithmType digest)
        {
            if (algorithmType == CertificateAlgorithmType.SM2) return OidSm2WithSm3;
            switch (digest)
            {
                case DigestAlgorithmType.SHA1: return OidSha1WithRsaEncryption;
                case DigestAlgorithmType.SHA384: return OidSha384WithRsaEncryption;
                case DigestAlgorithmType.SHA512: return OidSha512WithRsaEncryption;
                default: return OidSha256WithRsaEncryption;
            }
        }
        #endregion

        #region 密钥参数
        /// <summary>
        /// 获取RSA密钥参数
        /// </summary>
        /// <param name="includePrivate">是否包含私钥</param>
        /// <returns>RSA参数</returns>
        public RSAParameters GetRsaParameters(Boolean includePrivate)
        {
            var text = includePrivate ? this.PrivateKey : this.PublicKey;
            if (text.IsNullOrEmpty()) throw new Exception("密钥不存在.");
            var parts = text.Split('|');
            var modulusLength = this.KeySize / 8;
            var halfLength = this.KeySize / 16;
            var parameters = new RSAParameters
            {
                Modulus = FixLength(Convert.FromBase64String(parts[2]), modulusLength),
                Exponent = Convert.FromBase64String(parts[3])
            };
            if (includePrivate)
            {
                parameters.D = FixLength(Convert.FromBase64String(parts[4]), modulusLength);
                parameters.P = FixLength(Convert.FromBase64String(parts[5]), halfLength);
                parameters.Q = FixLength(Convert.FromBase64String(parts[6]), halfLength);
                parameters.DP = FixLength(Convert.FromBase64String(parts[7]), halfLength);
                parameters.DQ = FixLength(Convert.FromBase64String(parts[8]), halfLength);
                parameters.InverseQ = FixLength(Convert.FromBase64String(parts[9]), halfLength);
            }
            return parameters;
        }
        /// <summary>
        /// 获取SM2密钥对
        /// </summary>
        /// <returns>SM2密钥对</returns>
        public SM2KeyPair GetSM2KeyPair()
        {
            var parts = this.HasPrivateKey ? this.PrivateKey.Split('|') : this.PublicKey.Split('|');
            var privateKey = this.HasPrivateKey ? parts[2] : "";
            return new SM2KeyPair(privateKey, "04" + parts[parts.Length - 2] + parts[parts.Length - 1]);
        }
        /// <summary>
        /// 获取SM2公钥 130个十六进制字符
        /// </summary>
        /// <returns>SM2公钥</returns>
        public string GetSM2PublicKey()
        {
            var parts = this.HasPrivateKey ? this.PrivateKey.Split('|') : this.PublicKey.Split('|');
            return "04" + parts[parts.Length - 2] + parts[parts.Length - 1];
        }
        /// <summary>
        /// 补齐或截断字节到指定长度
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">长度</param>
        /// <returns>字节</returns>
        private static byte[] FixLength(byte[] data, int length)
        {
            if (data == null) data = Array.Empty<byte>();
            if (data.Length == length) return data;
            var result = new byte[length];
            if (data.Length > length) Array.Copy(data, data.Length - length, result, 0, length);
            else Array.Copy(data, 0, result, length - data.Length, data.Length);
            return result;
        }
        #endregion

        #region 公钥信息编码
        /// <summary>
        /// 获取SubjectPublicKeyInfo的DER编码
        /// </summary>
        /// <returns>DER字节</returns>
        public byte[] ToSubjectPublicKeyInfo()
        {
            if (this.AlgorithmType == CertificateAlgorithmType.SM2)
            {
                var parts = this.HasPrivateKey ? this.PrivateKey.Split('|') : this.PublicKey.Split('|');
                var x = SM2Encryption.FromHex(parts[parts.Length - 2]);
                var y = SM2Encryption.FromHex(parts[parts.Length - 1]);
                var publicKey = SM2Encryption.ConcatBytes(new byte[] { 0x04 }, x, y);
                return DerWriter.Sequence(DerWriter.AlgorithmIdentifier(OidEcPublicKey, DerWriter.Oid(OidSm2Curve)), DerWriter.BitString(publicKey));
            }
            var parameters = this.GetRsaParameters(false);
            var rsaPublicKey = DerWriter.Sequence(DerWriter.Integer(parameters.Modulus), DerWriter.Integer(parameters.Exponent));
            return DerWriter.Sequence(DerWriter.AlgorithmIdentifier(OidRsaEncryption, DerWriter.Null()), DerWriter.BitString(rsaPublicKey));
        }
        /// <summary>
        /// 获取PKCS#8私钥的DER编码
        /// </summary>
        /// <returns>DER字节</returns>
        public byte[] ToPrivateKeyInfo()
        {
            if (!this.HasPrivateKey) return Array.Empty<byte>();
            if (this.AlgorithmType == CertificateAlgorithmType.SM2)
            {
                var parts = this.PrivateKey.Split('|');
                var privateKey = SM2Encryption.FromHex(parts[2]);
                var publicKey = SM2Encryption.FromHex(this.GetSM2PublicKey());
                var ecPrivateKey = DerWriter.Sequence(
                    DerWriter.Integer(1),
                    DerWriter.OctetString(privateKey),
                    DerWriter.Context(1, DerWriter.BitString(publicKey)));
                return DerWriter.Sequence(
                    DerWriter.Integer(0),
                    DerWriter.AlgorithmIdentifier(OidEcPublicKey, DerWriter.Oid(OidSm2Curve)),
                    DerWriter.OctetString(ecPrivateKey));
            }
            var parameters = this.GetRsaParameters(true);
            var modulusLength = this.KeySize / 8;
            var halfLength = this.KeySize / 16;
            var pkcs1 = DerWriter.Sequence(
                DerWriter.Integer(0),
                DerWriter.Integer(FixLength(parameters.Modulus, modulusLength)),
                DerWriter.Integer(parameters.Exponent),
                DerWriter.Integer(FixLength(parameters.D, modulusLength)),
                DerWriter.Integer(FixLength(parameters.P, halfLength)),
                DerWriter.Integer(FixLength(parameters.Q, halfLength)),
                DerWriter.Integer(FixLength(parameters.DP, halfLength)),
                DerWriter.Integer(FixLength(parameters.DQ, halfLength)),
                DerWriter.Integer(FixLength(parameters.InverseQ, halfLength)));
            return DerWriter.Sequence(
                DerWriter.Integer(0),
                DerWriter.AlgorithmIdentifier(OidRsaEncryption, DerWriter.Null()),
                DerWriter.OctetString(pkcs1));
        }
        /// <summary>
        /// 获取公钥PEM
        /// </summary>
        /// <returns>PEM字符串</returns>
        public string ToPublicKeyPem() => DerWriter.ToPem(this.ToSubjectPublicKeyInfo(), "PUBLIC KEY");
        /// <summary>
        /// 获取PKCS#8私钥PEM
        /// </summary>
        /// <returns>PEM字符串</returns>
        public string ToPrivateKeyPem() => DerWriter.ToPem(this.ToPrivateKeyInfo(), "PRIVATE KEY");
        /// <summary>
        /// 获取密钥指纹 公钥的SHA1值
        /// </summary>
        /// <returns>十六进制指纹</returns>
        public string GetThumbprint()
        {
            using (var sha1 = SHA1.Create())
                return SM2Encryption.ToHex(sha1.ComputeHash(this.ToSubjectPublicKeyInfo()));
        }
        #endregion

        #region 导入导出
        /// <summary>
        /// 从公钥PEM导入
        /// </summary>
        /// <param name="pem">公钥PEM</param>
        /// <returns>证书密钥</returns>
        public static CertificateKey FromPublicKeyPem(string pem)
        {
            var der = DerWriter.FromPem(pem);
            if (der.Length == 0) return null;
            var node = DerReader.Parse(der);
            if (node.Count < 2) return null;
            var oid = node[0][0].GetOid();
            var key = new CertificateKey();
            if (oid == OidEcPublicKey)
            {
                var publicKey = node[1].GetBitStringBytes();
                if (publicKey.Length != 65) return null;
                key.AlgorithmType = CertificateAlgorithmType.SM2;
                key.KeySize = 256;
                key.PublicKey = "SM2|256|" + SM2Encryption.ToHex(SM2Encryption.SubBytes(publicKey, 1, 32)) + "|" + SM2Encryption.ToHex(SM2Encryption.SubBytes(publicKey, 33, 32));
            }
            else
            {
                var spki = DerReader.Parse(node[1].GetBitStringBytes());
                var modulus = spki[0].GetIntegerBytes();
                var exponent = spki[1].GetIntegerBytes();
                key.AlgorithmType = CertificateAlgorithmType.RSA;
                key.KeySize = modulus.Length * 8;
                key.PublicKey = "RSA|" + key.KeySize + "|" + Convert.ToBase64String(modulus) + "|" + Convert.ToBase64String(exponent);
            }
            return key;
        }
        /// <summary>
        /// 从PKCS#8私钥PEM导入
        /// </summary>
        /// <param name="pem">私钥PEM</param>
        /// <returns>证书密钥</returns>
        public static CertificateKey FromPrivateKeyPem(string pem)
        {
            var der = DerWriter.FromPem(pem);
            if (der.Length == 0) return null;
            var node = DerReader.Parse(der);
            if (node.Count < 3) return null;
            var oid = node[1][0].GetOid();
            var key = new CertificateKey();
            var privateKeyDer = node[2].Content;
            if (oid == OidEcPublicKey)
            {
                var ec = DerReader.Parse(privateKeyDer);
                if (ec.Count < 2) return null;
                var d = ec[1].Content;
                var publicKey = ec.Count > 2 ? ec[2].GetBitStringBytes() : null;
                string x, y;
                if (publicKey != null && publicKey.Length == 65)
                {
                    x = SM2Encryption.ToHex(SM2Encryption.SubBytes(publicKey, 1, 32));
                    y = SM2Encryption.ToHex(SM2Encryption.SubBytes(publicKey, 33, 32));
                }
                else
                {
                    var point = SM2Encryption.PointMultiply(SM2Encryption.G, SM2Encryption.FromUnsignedBigEndian(d));
                    x = SM2Encryption.ToHex(SM2Encryption.ToFixedBytes(point[0], 32));
                    y = SM2Encryption.ToHex(SM2Encryption.ToFixedBytes(point[1], 32));
                }
                key.AlgorithmType = CertificateAlgorithmType.SM2;
                key.KeySize = 256;
                key.PrivateKey = "SM2|256|" + SM2Encryption.ToHex(SM2Encryption.ToFixedBytesBigEndian(d, 32)) + "|" + x + "|" + y;
                key.PublicKey = "SM2|256|" + x + "|" + y;
            }
            else
            {
                var pkcs1 = DerReader.Parse(privateKeyDer);
                if (pkcs1.Count < 9) return null;
                var modulus = pkcs1[1].GetIntegerBytes();
                var exponent = pkcs1[2].GetIntegerBytes();
                key.AlgorithmType = CertificateAlgorithmType.RSA;
                key.KeySize = modulus.Length * 8;
                key.PrivateKey = "RSA|" + key.KeySize + "|" + Convert.ToBase64String(modulus) + "|" + Convert.ToBase64String(exponent) +
                    "|" + Convert.ToBase64String(pkcs1[3].GetIntegerBytes()) +
                    "|" + Convert.ToBase64String(pkcs1[4].GetIntegerBytes()) +
                    "|" + Convert.ToBase64String(pkcs1[5].GetIntegerBytes()) +
                    "|" + Convert.ToBase64String(pkcs1[6].GetIntegerBytes()) +
                    "|" + Convert.ToBase64String(pkcs1[7].GetIntegerBytes()) +
                    "|" + Convert.ToBase64String(pkcs1[8].GetIntegerBytes());
                key.PublicKey = "RSA|" + key.KeySize + "|" + Convert.ToBase64String(modulus) + "|" + Convert.ToBase64String(exponent);
            }
            return key;
        }
        /// <summary>
        /// 解析内部格式的密钥
        /// </summary>
        /// <param name="value">密钥文本</param>
        /// <returns>证书密钥</returns>
        public static CertificateKey Parse(string value)
        {
            if (value.IsNullOrEmpty()) return null;
            var key = new CertificateKey();
            var parts = value.Split('|');
            key.AlgorithmType = parts[0].EqualsIgnoreCase("SM2") ? CertificateAlgorithmType.SM2 : CertificateAlgorithmType.RSA;
            if (parts.Length > 1) key.KeySize = parts[1].ToCast(0);
            if (key.AlgorithmType == CertificateAlgorithmType.SM2)
            {
                if (parts.Length >= 5)
                {
                    key.PrivateKey = value;
                    key.PublicKey = "SM2|256|" + parts[3] + "|" + parts[4];
                }
                else if (parts.Length >= 4)
                    key.PublicKey = value;
            }
            else
            {
                if (parts.Length >= 10)
                {
                    key.PrivateKey = value;
                    key.PublicKey = "RSA|" + parts[1] + "|" + parts[2] + "|" + parts[3];
                }
                else if (parts.Length >= 4)
                    key.PublicKey = value;
            }
            return key;
        }
        #endregion

        #endregion
    }
}
