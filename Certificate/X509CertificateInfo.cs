using System;
using System.Collections.Generic;
using XiaoFeng.Asn1;
using XiaoFeng.Cryptography;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:40:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// X.509证书信息
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:40:00
    /// Description : 解析后的X.509证书结构，包含版本、序列号、颁发者、主题、有效期、公钥、扩展与签名值。
    /// </summary>
    public class X509CertificateInfo
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public X509CertificateInfo()
        {
        }
        #endregion

        #region 属性
        /// <summary>
        /// 证书版本号
        /// </summary>
        public int Version { get; set; } = 3;
        /// <summary>
        /// 证书序列号 十六进制字符串
        /// </summary>
        public string SerialNumber { get; set; } = "";
        /// <summary>
        /// 签名算法OID
        /// </summary>
        public string SignatureAlgorithmOid { get; set; } = "";
        /// <summary>
        /// 颁发者
        /// </summary>
        public X509Name Issuer { get; set; } = new X509Name();
        /// <summary>
        /// 主题
        /// </summary>
        public X509Name Subject { get; set; } = new X509Name();
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime NotBefore { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime NotAfter { get; set; }
        /// <summary>
        /// 公钥算法类型
        /// </summary>
        public CertificateAlgorithmType PublicKeyAlgorithm { get; set; } = CertificateAlgorithmType.RSA;
        /// <summary>
        /// 密钥长度
        /// </summary>
        public int KeySize { get; set; }
        /// <summary>
        /// 公钥信息DER SubjectPublicKeyInfo
        /// </summary>
        public byte[] SubjectPublicKeyInfo { get; set; }
        /// <summary>
        /// 签名值
        /// </summary>
        public byte[] SignatureValue { get; set; }
        /// <summary>
        /// 待签名数据DER
        /// </summary>
        public byte[] TbsData { get; set; }
        /// <summary>
        /// 证书原始数据DER
        /// </summary>
        public byte[] RawData { get; set; }
        /// <summary>
        /// 是否为CA证书
        /// </summary>
        public Boolean IsCA { get; set; }
        /// <summary>
        /// 证书路径长度限制
        /// </summary>
        public int PathLengthConstraint { get; set; } = -1;
        /// <summary>
        /// 密钥用法
        /// </summary>
        public KeyUsageFlag KeyUsage { get; set; } = KeyUsageFlag.None;
        /// <summary>
        /// 主体密钥标识
        /// </summary>
        public string SubjectKeyIdentifier { get; set; } = "";
        /// <summary>
        /// 颁发者密钥标识
        /// </summary>
        public string AuthorityKeyIdentifier { get; set; } = "";
        #endregion

        #region 方法

        #region 获取公钥
        /// <summary>
        /// 获取证书公钥
        /// </summary>
        /// <returns>证书密钥 仅包含公钥</returns>
        public CertificateKey GetPublicKey() => CertificateKey.FromPublicKeyPem(DerWriter.ToPem(this.SubjectPublicKeyInfo, "PUBLIC KEY"));
        #endregion

        #region 获取PEM
        /// <summary>
        /// 获取证书PEM
        /// </summary>
        /// <returns>PEM字符串</returns>
        public string ToPem() => DerWriter.ToPem(this.RawData, "CERTIFICATE");
        /// <summary>
        /// 获取证书Base64
        /// </summary>
        /// <returns>Base64字符串</returns>
        public string ToBase64() => Convert.ToBase64String(this.RawData ?? new byte[0]);
        #endregion

        #region 解析证书
        /// <summary>
        /// 解析证书DER数据
        /// </summary>
        /// <param name="der">证书DER数据</param>
        /// <returns>证书信息</returns>
        public static X509CertificateInfo Parse(byte[] der)
        {
            if (der == null || der.Length == 0) return null;
            var root = DerReader.Parse(der);
            if (root.Count < 3) return null;
            var info = new X509CertificateInfo { RawData = der };
            var tbs = root[0];
            info.TbsData = tbs.Encoded;
            if (root[1].Count > 0) info.SignatureAlgorithmOid = root[1][0].GetOid();
            info.SignatureValue = root[2].GetBitStringBytes();

            var index = 0;
            if (tbs[index].Tag == 0xA0)
            {
                if (tbs[index].Count > 0 && tbs[index][0].Content.Length > 0)
                    info.Version = tbs[index][0].Content[0] + 1;
                index++;
            }
            /*序列号*/
            if (index < tbs.Count)
            {
                info.SerialNumber = SM2Encryption.ToHex(tbs[index].GetIntegerBytes());
                index++;
            }
            /*签名算法*/
            index++;
            /*颁发者*/
            if (index < tbs.Count)
            {
                info.Issuer = X509Name.FromDer(tbs[index].Encoded);
                index++;
            }
            /*有效期*/
            if (index < tbs.Count && tbs[index].Count >= 2)
            {
                info.NotBefore = tbs[index][0].GetTime();
                info.NotAfter = tbs[index][1].GetTime();
                index++;
            }
            /*主题*/
            if (index < tbs.Count)
            {
                info.Subject = X509Name.FromDer(tbs[index].Encoded);
                index++;
            }
            /*公钥*/
            if (index < tbs.Count)
            {
                info.SubjectPublicKeyInfo = tbs[index].Encoded;
                var key = CertificateKey.FromPublicKeyPem(DerWriter.ToPem(info.SubjectPublicKeyInfo, "PUBLIC KEY"));
                if (key != null)
                {
                    info.PublicKeyAlgorithm = key.AlgorithmType;
                    info.KeySize = key.KeySize;
                }
                index++;
            }
            /*扩展*/
            if (index < tbs.Count && tbs[index].Tag == 0xA3)
                ParseExtensions(info, tbs[index]);
            return info;
        }
        /// <summary>
        /// 解析证书PEM
        /// </summary>
        /// <param name="pem">证书PEM或Base64数据</param>
        /// <returns>证书信息</returns>
        public static X509CertificateInfo ParsePem(string pem)
        {
            if (pem.IsNullOrEmpty()) return null;
            byte[] der;
            if (pem.IndexOf("-----BEGIN", StringComparison.OrdinalIgnoreCase) > -1)
                der = DerWriter.FromPem(pem);
            else
                der = Convert.FromBase64String(pem.Replace("\r", "").Replace("\n", "").Trim());
            return Parse(der);
        }
        #endregion

        #region 解析扩展
        /// <summary>
        /// 解析证书扩展
        /// </summary>
        /// <param name="info">证书信息</param>
        /// <param name="extensions">扩展节点</param>
        private static void ParseExtensions(X509CertificateInfo info, DerNode extensions)
        {
            /*扩展节点可能是[3]下的一层SEQUENCE容器*/
            var items = extensions.Count == 1 && extensions[0].Tag == DerWriter.TagSequence ? extensions[0].Children : extensions.Children;
            foreach (var extension in items)
            {
                if (extension.Count < 2) continue;
                var oid = extension[0].GetOid();
                var valueNode = extension[extension.Count - 1];
                var value = DerReader.Parse(valueNode.Content);
                switch (oid)
                {
                    case "2.5.29.14":
                        /*扩展值为OCTET STRING 其内容才是密钥标识*/
                        info.SubjectKeyIdentifier = SM2Encryption.ToHex(value.Content.Length > 0 ? value.Content : valueNode.Content);
                        break;
                    case "2.5.29.35":
                        if (value.Count > 0)
                        {
                            /*keyIdentifier为[0]隐式标签 兼容显式标签的旧数据*/
                            var keyIdentifier = value[0].Content;
                            if (keyIdentifier.Length > 2 && keyIdentifier[0] == DerWriter.TagOctetString)
                                keyIdentifier = DerReader.Parse(keyIdentifier).Content;
                            info.AuthorityKeyIdentifier = SM2Encryption.ToHex(keyIdentifier);
                        }
                        break;
                    case "2.5.29.15":
                        info.KeyUsage = ParseKeyUsage(DerReader.Parse(valueNode.Content));
                        break;
                    case "2.5.29.19":
                        if (value.Count > 0 && value[0].Tag == DerWriter.TagBoolean)
                            info.IsCA = value[0].Content.Length > 0 && value[0].Content[0] != 0x00;
                        if (value.Count > 1 && value[1].Tag == DerWriter.TagInteger)
                            info.PathLengthConstraint = value[1].Content.Length > 0 ? value[1].Content[0] : 0;
                        break;
                }
            }
        }
        /// <summary>
        /// 解析密钥用法
        /// </summary>
        /// <param name="node">比特串节点</param>
        /// <returns>密钥用法</returns>
        private static KeyUsageFlag ParseKeyUsage(DerNode node)
        {
            var usage = KeyUsageFlag.None;
            if (node == null || node.Content == null || node.Content.Length < 2) return usage;
            var data = node.GetBitStringBytes();
            var unusedBits = node.Content[0];
            var totalBits = data.Length * 8 - unusedBits;
            for (var i = 0; i < totalBits && i < 9; i++)
            {
                var bit = (data[i / 8] >> (7 - (i % 8))) & 0x01;
                if (bit == 1) usage |= (KeyUsageFlag)(1 << i);
            }
            return usage;
        }
        #endregion

        #endregion
    }
}
