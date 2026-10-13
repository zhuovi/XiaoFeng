using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Cryptography;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:50:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// 证书验证结果
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:50:00
    /// </summary>
    public class CertificateVerifyResult
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public CertificateVerifyResult()
        {
            this.Errors = new List<string>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否有效
        /// </summary>
        public Boolean IsValid { get; set; }
        /// <summary>
        /// 证书状态
        /// </summary>
        public CertificateStatus Status { get; set; } = CertificateStatus.Unknown;
        /// <summary>
        /// 验证消息
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// 错误明细
        /// </summary>
        public List<string> Errors { get; set; }
        /// <summary>
        /// 证书序列号
        /// </summary>
        public string SerialNumber { get; set; } = "";
        /// <summary>
        /// 证书主题
        /// </summary>
        public string Subject { get; set; } = "";
        /// <summary>
        /// 证书颁发者
        /// </summary>
        public string Issuer { get; set; } = "";
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime NotBefore { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime NotAfter { get; set; }
        /// <summary>
        /// 密钥算法
        /// </summary>
        public CertificateAlgorithmType AlgorithmType { get; set; } = CertificateAlgorithmType.RSA;
        #endregion

        #region 方法
        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        public void AddError(string message)
        {
            if (message.IsNullOrEmpty()) return;
            this.Errors.Add(message);
            this.IsValid = false;
        }
        #endregion
    }

    /// <summary>
    /// X.509证书验证器
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:50:00
    /// Description : 验证证书有效期、签名链、密钥用法与吊销状态。
    /// </summary>
    public static class X509CertificateValidator
    {
        #region 方法

        #region 获取证书状态
        /// <summary>
        /// 获取证书状态
        /// </summary>
        /// <param name="certificate">证书</param>
        /// <param name="revoked">是否已注销</param>
        /// <param name="time">验证时间 默认为当前时间</param>
        /// <param name="expiringDays">即将过期提醒天数</param>
        /// <returns>证书状态</returns>
        public static CertificateStatus GetStatus(X509CertificateInfo certificate, Boolean revoked = false, DateTime? time = null, int expiringDays = 30)
        {
            if (certificate == null) return CertificateStatus.Unknown;
            if (revoked) return CertificateStatus.Revoked;
            var now = time ?? DateTime.Now;
            if (now < certificate.NotBefore) return CertificateStatus.Inactive;
            if (now > certificate.NotAfter) return CertificateStatus.Expired;
            if ((certificate.NotAfter - now).TotalDays <= expiringDays) return CertificateStatus.Expiring;
            return CertificateStatus.Valid;
        }
        #endregion

        #region 验证证书
        /// <summary>
        /// 验证证书 验证有效期与签名
        /// </summary>
        /// <param name="certificate">证书</param>
        /// <param name="issuerCertificate">颁发者证书 自签名证书可为空</param>
        /// <param name="revoked">是否已注销</param>
        /// <param name="time">验证时间 默认为当前时间</param>
        /// <returns>验证结果</returns>
        public static CertificateVerifyResult Verify(X509CertificateInfo certificate, X509CertificateInfo issuerCertificate = null, Boolean revoked = false, DateTime? time = null)
        {
            var result = new CertificateVerifyResult();
            if (certificate == null)
            {
                result.AddError("证书不存在.");
                result.Message = "证书不存在.";
                result.Status = CertificateStatus.Unknown;
                return result;
            }
            result.SerialNumber = certificate.SerialNumber;
            result.Subject = certificate.Subject.ToString();
            result.Issuer = certificate.Issuer.ToString();
            result.NotBefore = certificate.NotBefore;
            result.NotAfter = certificate.NotAfter;
            result.AlgorithmType = certificate.PublicKeyAlgorithm;
            result.Status = GetStatus(certificate, revoked, time);
            result.IsValid = true;

            if (revoked) result.AddError("证书已注销.");
            var now = time ?? DateTime.Now;
            if (now < certificate.NotBefore) result.AddError("证书未生效.");
            if (now > certificate.NotAfter) result.AddError("证书已过期.");

            var issuer = issuerCertificate;
            if (issuer == null && certificate.Subject.ToString() == certificate.Issuer.ToString())
                issuer = certificate;

            if (issuer == null)
            {
                result.AddError("未找到证书的颁发者证书，无法验证签名.");
            }
            else
            {
                if (issuer.Subject.ToString() != certificate.Issuer.ToString())
                    result.AddError("证书颁发者与签发证书主题不一致.");
                try
                {
                    var key = issuer.GetPublicKey();
                    var digest = GetDigestAlgorithm(certificate.SignatureAlgorithmOid);
                    if (!key.Verify(certificate.TbsData, certificate.SignatureValue, digest))
                        result.AddError("证书签名验证失败，证书内容可能被篡改.");
                }
                catch (Exception ex)
                {
                    result.AddError("证书签名验证出错:" + ex.Message);
                }
            }
            if (result.Errors.Count > 0)
                result.Message = string.Join(" ", result.Errors.ToArray());
            else
                result.Message = "证书有效.";
            return result;
        }
        #endregion

        #region 获取摘要算法
        /// <summary>
        /// 根据签名算法OID获取摘要算法
        /// </summary>
        /// <param name="oid">签名算法OID</param>
        /// <returns>摘要算法</returns>
        public static DigestAlgorithmType GetDigestAlgorithm(string oid)
        {
            switch (oid)
            {
                case CertificateKey.OidSm2WithSm3:
                    return DigestAlgorithmType.SM3;
                case CertificateKey.OidSha1WithRsaEncryption:
                    return DigestAlgorithmType.SHA1;
                case CertificateKey.OidSha384WithRsaEncryption:
                    return DigestAlgorithmType.SHA384;
                case CertificateKey.OidSha512WithRsaEncryption:
                    return DigestAlgorithmType.SHA512;
                default:
                    return DigestAlgorithmType.SHA256;
            }
        }
        #endregion

        #endregion
    }
}
