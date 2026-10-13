using System;
using System.Security.Cryptography;
using XiaoFeng.Cryptography;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 12:00:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// 电子签章数据
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 12:00:00
    /// Description : 电子文件签章结果，包含文件摘要、签名值、签名证书等信息，可序列化成JSON独立存储或随文件分发。
    /// </summary>
    public class DigitalSignatureData
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DigitalSignatureData()
        {
            this.SignatureId = Guid.NewGuid().ToString("N").ToUpper();
            this.SignTime = DateTime.Now;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 签章版本号
        /// </summary>
        public string Version { get; set; } = "1.0";
        /// <summary>
        /// 签章编号
        /// </summary>
        public string SignatureId { get; set; } = "";
        /// <summary>
        /// 签名证书序列号
        /// </summary>
        public string SerialNumber { get; set; } = "";
        /// <summary>
        /// 密钥算法
        /// </summary>
        public CertificateAlgorithmType AlgorithmType { get; set; } = CertificateAlgorithmType.RSA;
        /// <summary>
        /// 摘要算法
        /// </summary>
        public DigestAlgorithmType DigestAlgorithm { get; set; } = DigestAlgorithmType.SHA256;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = "";
        /// <summary>
        /// 文件大小(字节)
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件摘要值 十六进制
        /// </summary>
        public string DigestValue { get; set; } = "";
        /// <summary>
        /// 签名值 Base64
        /// </summary>
        public string SignatureValue { get; set; } = "";
        /// <summary>
        /// 签名证书数据 Base64 DER
        /// </summary>
        public string CertificateData { get; set; } = "";
        /// <summary>
        /// 签章人
        /// </summary>
        public string Signer { get; set; } = "";
        /// <summary>
        /// 签章时间
        /// </summary>
        public DateTime SignTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";
        #endregion

        #region 方法
        /// <summary>
        /// 解析JSON字符串
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>签章数据</returns>
        public static DigitalSignatureData Parse(string json) => json.JsonToObject<DigitalSignatureData>();
        #endregion
    }

    /// <summary>
    /// 电子签章器
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 12:00:00
    /// Description : 使用数字证书私钥对电子文件进行签章，并使用证书公钥验证签章。
    /// </summary>
    public static class DigitalSigner
    {
        #region 方法

        #region 签章
        /// <summary>
        /// 对电子文件签章
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="key">签名密钥 必须包含私钥</param>
        /// <param name="certificateDer">签名证书DER数据</param>
        /// <param name="serialNumber">证书序列号</param>
        /// <param name="signer">签章人</param>
        /// <param name="remark">备注</param>
        /// <returns>签章数据</returns>
        public static DigitalSignatureData Sign(byte[] data, string fileName, CertificateKey key, byte[] certificateDer, string serialNumber = "", string signer = "", string remark = "")
        {
            if (data == null) throw new Exception("签章数据不能为空.");
            if (key == null || !key.HasPrivateKey) throw new Exception("签名密钥无效，必须使用包含私钥的证书密钥.");
            var digest = key.DefaultDigest;
            var signature = new DigitalSignatureData
            {
                SerialNumber = serialNumber ?? "",
                AlgorithmType = key.AlgorithmType,
                DigestAlgorithm = digest,
                FileName = fileName ?? "",
                FileSize = data.Length,
                DigestValue = CertificateKey.ComputeDigestHex(data, digest),
                SignatureValue = key.SignBase64(data, digest),
                CertificateData = certificateDer == null ? "" : Convert.ToBase64String(certificateDer),
                Signer = signer ?? "",
                Remark = remark ?? ""
            };
            return signature;
        }
        #endregion

        #region 验签
        /// <summary>
        /// 验证电子文件签章
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <param name="signature">签章数据</param>
        /// <param name="revoked">签名证书是否已注销</param>
        /// <param name="message">验证消息</param>
        /// <returns>是否有效</returns>
        public static Boolean Verify(byte[] data, DigitalSignatureData signature, Boolean revoked, out string message)
        {
            message = "";
            if (data == null) { message = "文件数据不能为空."; return false; }
            if (signature == null) { message = "签章数据不能为空."; return false; }
            if (signature.SignatureValue.IsNullOrEmpty()) { message = "签名值不能为空."; return false; }
            var digestValue = CertificateKey.ComputeDigestHex(data, signature.DigestAlgorithm);
            if (!digestValue.EqualsIgnoreCase(signature.DigestValue))
            {
                message = "文件摘要不一致，文件已被篡改.";
                return false;
            }
            if (revoked)
            {
                message = "签名证书已注销.";
                return false;
            }
            var certificate = X509CertificateInfo.ParsePem(signature.CertificateData);
            if (certificate == null)
            {
                message = "签名证书无效.";
                return false;
            }
            var key = certificate.GetPublicKey();
            if (key == null)
            {
                message = "签名证书公钥无效.";
                return false;
            }
            if (!key.VerifyBase64(data, signature.SignatureValue, signature.DigestAlgorithm))
            {
                message = "签名验证失败，签章无效.";
                return false;
            }
            message = "签章验证通过.";
            return true;
        }
        /// <summary>
        /// 验证电子文件签章
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <param name="signature">签章数据</param>
        /// <returns>是否有效</returns>
        public static Boolean Verify(byte[] data, DigitalSignatureData signature)
        {
            string message;
            return Verify(data, signature, false, out message);
        }
        #endregion

        #endregion
    }
}
