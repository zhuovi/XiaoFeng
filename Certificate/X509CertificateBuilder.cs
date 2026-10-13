using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using XiaoFeng.Asn1;
using XiaoFeng.Cryptography;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:30:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// X.509证书生成器
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:30:00
    /// Description : 用于生成X.509 V3标准数字证书，支持RSA与国密SM2算法，支持自签名根证书与CA签发证书。
    /// </summary>
    public class X509CertificateBuilder
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public X509CertificateBuilder()
        {
            this.SerialNumber = GenerateSerialNumber();
            this.NotBefore = DateTime.Now.AddMinutes(-10);
            this.NotAfter = DateTime.Now.AddYears(1);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 证书版本号 V3
        /// </summary>
        public int Version { get; set; } = 3;
        /// <summary>
        /// 证书序列号 大端字节
        /// </summary>
        public byte[] SerialNumber { get; set; }
        /// <summary>
        /// 证书主题
        /// </summary>
        public X509Name Subject { get; set; }
        /// <summary>
        /// 证书颁发者
        /// </summary>
        public X509Name Issuer { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime NotBefore { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime NotAfter { get; set; }
        /// <summary>
        /// 证书主体密钥 包含公钥
        /// </summary>
        public CertificateKey SubjectKey { get; set; }
        /// <summary>
        /// 签发者密钥 必须包含私钥
        /// </summary>
        public CertificateKey IssuerKey { get; set; }
        /// <summary>
        /// 证书类型
        /// </summary>
        public CertificateType CertificateType { get; set; } = CertificateType.EndEntity;
        /// <summary>
        /// 是否为CA证书
        /// </summary>
        public Boolean IsCA { get; set; }
        /// <summary>
        /// 证书路径长度限制 小于0表示不限制
        /// </summary>
        public int PathLengthConstraint { get; set; } = -1;
        /// <summary>
        /// 密钥用法
        /// </summary>
        public KeyUsageFlag KeyUsage { get; set; } = KeyUsageFlag.DigitalSignature | KeyUsageFlag.KeyEncipherment;
        /// <summary>
        /// 摘要算法
        /// </summary>
        public DigestAlgorithmType DigestAlgorithm { get; set; } = DigestAlgorithmType.SHA256;
        /// <summary>
        /// 颁发者密钥标识 为空时自动从签发者公钥计算
        /// </summary>
        public byte[] AuthorityKeyIdentifier { get; set; }
        /// <summary>
        /// 扩展密钥用法OID集合 如服务器认证1.3.6.1.5.5.7.3.1 客户端认证1.3.6.1.5.5.7.3.2
        /// </summary>
        public List<string> ExtendedKeyUsage { get; set; } = new List<string>();
        #endregion

        #region 方法

        #region 生成证书
        /// <summary>
        /// 生成证书DER数据
        /// </summary>
        /// <returns>证书DER字节</returns>
        public byte[] Build()
        {
            if (this.Subject == null) throw new Exception("证书主题不能为空.");
            if (this.SubjectKey == null) throw new Exception("证书密钥不能为空.");
            if (this.IssuerKey == null) this.IssuerKey = this.SubjectKey;
            if (this.Issuer == null) this.Issuer = this.Subject;
            if (this.SerialNumber == null || this.SerialNumber.Length == 0) this.SerialNumber = GenerateSerialNumber();
            if (this.SubjectKey.AlgorithmType == CertificateAlgorithmType.SM2) this.DigestAlgorithm = DigestAlgorithmType.SM3;
            if (this.IsCA) this.KeyUsage |= KeyUsageFlag.KeyCertSign | KeyUsageFlag.CRLSign;

            var signatureOid = CertificateKey.GetSignatureAlgorithmOid(this.IssuerKey.AlgorithmType, this.DigestAlgorithm);
            var spki = this.SubjectKey.ToSubjectPublicKeyInfo();
            var subjectKeyIdentifier = ComputeKeyIdentifier(spki);
            if (this.AuthorityKeyIdentifier == null || this.AuthorityKeyIdentifier.Length == 0)
                this.AuthorityKeyIdentifier = this.IsCA ? subjectKeyIdentifier : ComputeKeyIdentifier(this.IssuerKey.ToSubjectPublicKeyInfo());

            var items = new List<byte[]>
            {
                DerWriter.Context(0, DerWriter.Integer(this.Version - 1)),
                DerWriter.Integer(this.SerialNumber),
                DerWriter.AlgorithmIdentifier(signatureOid, DerWriter.Null()),
                this.Issuer.Encode(),
                DerWriter.Sequence(DerWriter.Time(this.NotBefore), DerWriter.Time(this.NotAfter)),
                this.Subject.Encode(),
                spki
            };
            items.Add(DerWriter.Context(3, DerWriter.Sequence(BuildExtensions(subjectKeyIdentifier).ToArray())));
            var tbsCertificate = DerWriter.Sequence(items.ToArray());
            var signature = this.IssuerKey.Sign(tbsCertificate, this.DigestAlgorithm);
            return DerWriter.Sequence(tbsCertificate, DerWriter.AlgorithmIdentifier(signatureOid, DerWriter.Null()), DerWriter.BitString(signature));
        }
        /// <summary>
        /// 生成证书PEM
        /// </summary>
        /// <returns>证书PEM字符串</returns>
        public string BuildPem() => DerWriter.ToPem(this.Build(), "CERTIFICATE");
        #endregion

        #region 生成扩展
        /// <summary>
        /// 生成扩展节点集
        /// </summary>
        /// <param name="subjectKeyIdentifier">主体密钥标识</param>
        /// <returns>扩展节点集</returns>
        private List<byte[]> BuildExtensions(byte[] subjectKeyIdentifier)
        {
            var list = new List<byte[]>();
            /*密钥用法*/
            if (this.KeyUsage != KeyUsageFlag.None)
                list.Add(BuildExtension("2.5.29.15", true, EncodeKeyUsage(this.KeyUsage)));
            /*基本约束*/
            var basicConstraints = this.PathLengthConstraint >= 0
                ? DerWriter.Sequence(DerWriter.Boolean(this.IsCA), DerWriter.Integer(this.PathLengthConstraint))
                : DerWriter.Sequence(DerWriter.Boolean(this.IsCA));
            list.Add(BuildExtension("2.5.29.19", true, basicConstraints));
            /*主体密钥标识*/
            list.Add(BuildExtension("2.5.29.14", false, DerWriter.OctetString(subjectKeyIdentifier)));
            /*颁发者密钥标识*/
            if (this.AuthorityKeyIdentifier != null && this.AuthorityKeyIdentifier.Length > 0)
                list.Add(BuildExtension("2.5.29.35", false, DerWriter.Sequence(DerWriter.Context(0, DerWriter.OctetString(this.AuthorityKeyIdentifier), false))));
            /*扩展密钥用法*/
            if (this.ExtendedKeyUsage != null && this.ExtendedKeyUsage.Count > 0)
            {
                var oids = new List<byte[]>();
                foreach (var oid in this.ExtendedKeyUsage) oids.Add(DerWriter.Oid(oid));
                list.Add(BuildExtension("2.5.29.37", false, DerWriter.Sequence(oids.ToArray())));
            }
            return list;
        }
        /// <summary>
        /// 生成扩展节点
        /// </summary>
        /// <param name="oid">扩展OID</param>
        /// <param name="critical">是否关键扩展</param>
        /// <param name="value">扩展值DER</param>
        /// <returns>扩展节点</returns>
        private byte[] BuildExtension(string oid, Boolean critical, byte[] value)
        {
            if (critical)
                return DerWriter.Sequence(DerWriter.Oid(oid), DerWriter.Boolean(true), DerWriter.OctetString(value));
            return DerWriter.Sequence(DerWriter.Oid(oid), DerWriter.OctetString(value));
        }
        #endregion

        #region 编码密钥用法
        /// <summary>
        /// 编码密钥用法
        /// </summary>
        /// <param name="usage">密钥用法</param>
        /// <returns>DER字节</returns>
        private static byte[] EncodeKeyUsage(KeyUsageFlag usage)
        {
            var flags = (int)usage;
            var bits = 0;
            for (var i = 0; i < 9; i++)
                if ((flags & (1 << i)) != 0) bits = i + 1;
            if (bits == 0) return DerWriter.BitString(new byte[] { 0x00 });
            var bytes = new byte[(bits + 7) / 8];
            for (var i = 0; i < bits; i++)
                if ((flags & (1 << i)) != 0) bytes[i / 8] |= (byte)(0x80 >> (i % 8));
            return DerWriter.BitString(bytes, (byte)(bytes.Length * 8 - bits));
        }
        #endregion

        #region 生成序列号
        /// <summary>
        /// 生成证书序列号 16字节正数
        /// </summary>
        /// <returns>序列号字节</returns>
        public static byte[] GenerateSerialNumber()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            bytes[0] = (byte)(bytes[0] & 0x7F);
            if (bytes[0] == 0) bytes[0] = 0x01;
            return bytes;
        }
        /// <summary>
        /// 计算密钥标识 公钥BIT STRING内容的SHA1值
        /// </summary>
        /// <param name="subjectPublicKeyInfo">公钥信息DER</param>
        /// <returns>密钥标识</returns>
        public static byte[] ComputeKeyIdentifier(byte[] subjectPublicKeyInfo)
        {
            var node = DerReader.Parse(subjectPublicKeyInfo);
            var publicKey = node.Count > 1 ? node[1].GetBitStringBytes() : subjectPublicKeyInfo;
            using (var sha1 = SHA1.Create())
                return sha1.ComputeHash(publicKey);
        }
        #endregion

        #endregion
    }
}
