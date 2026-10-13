using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 10:10:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Asn1
{
    /// <summary>
    /// ASN.1 DER 编码写入器
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 10:10:00
    /// Description : 用于生成X.509数字证书、PKCS#8私钥、SubjectPublicKeyInfo公钥等标准ASN.1结构。
    /// </summary>
    public static class DerWriter
    {
        #region 标签常量
        /// <summary>BOOLEAN</summary>
        public const byte TagBoolean = 0x01;
        /// <summary>INTEGER</summary>
        public const byte TagInteger = 0x02;
        /// <summary>BIT STRING</summary>
        public const byte TagBitString = 0x03;
        /// <summary>OCTET STRING</summary>
        public const byte TagOctetString = 0x04;
        /// <summary>NULL</summary>
        public const byte TagNull = 0x05;
        /// <summary>OBJECT IDENTIFIER</summary>
        public const byte TagOid = 0x06;
        /// <summary>UTF8String</summary>
        public const byte TagUtf8String = 0x0C;
        /// <summary>SEQUENCE</summary>
        public const byte TagSequence = 0x30;
        /// <summary>SET</summary>
        public const byte TagSet = 0x31;
        /// <summary>PrintableString</summary>
        public const byte TagPrintableString = 0x13;
        /// <summary>UTCTime</summary>
        public const byte TagUtcTime = 0x17;
        /// <summary>GeneralizedTime</summary>
        public const byte TagGeneralizedTime = 0x18;
        #endregion

        #region 基础编码
        /// <summary>
        /// 编码长度
        /// </summary>
        /// <param name="length">内容长度</param>
        /// <returns>长度字节</returns>
        public static byte[] Length(int length)
        {
            if (length < 0x80) return new byte[] { (byte)length };
            var bytes = new List<byte>();
            var value = length;
            while (value > 0)
            {
                bytes.Insert(0, (byte)(value & 0xFF));
                value >>= 8;
            }
            var result = new byte[bytes.Count + 1];
            result[0] = (byte)(0x80 | bytes.Count);
            bytes.CopyTo(result, 1);
            return result;
        }
        /// <summary>
        /// 编码TLV结构
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="content">内容</param>
        /// <returns>TLV字节</returns>
        public static byte[] Tlv(byte tag, byte[] content)
        {
            if (content == null) content = Array.Empty<byte>();
            var length = Length(content.Length);
            var result = new byte[1 + length.Length + content.Length];
            result[0] = tag;
            Array.Copy(length, 0, result, 1, length.Length);
            Array.Copy(content, 0, result, 1 + length.Length, content.Length);
            return result;
        }
        /// <summary>
        /// 拼接字节数组
        /// </summary>
        /// <param name="items">字节数组集</param>
        /// <returns>拼接后的字节数组</returns>
        public static byte[] Concat(params byte[][] items)
        {
            var ms = new System.IO.MemoryStream();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item == null || item.Length == 0) continue;
                    ms.Write(item, 0, item.Length);
                }
            }
            return ms.ToArray();
        }
        /// <summary>
        /// 编码SEQUENCE
        /// </summary>
        /// <param name="items">子节点集</param>
        /// <returns>DER字节</returns>
        public static byte[] Sequence(params byte[][] items) => Tlv(TagSequence, Concat(items));
        /// <summary>
        /// 编码SET
        /// </summary>
        /// <param name="items">子节点集</param>
        /// <returns>DER字节</returns>
        public static byte[] Set(params byte[][] items) => Tlv(TagSet, Concat(items));
        #endregion

        #region 基本类型
        /// <summary>
        /// 编码整型
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>DER字节</returns>
        public static byte[] Integer(BigInteger value)
        {
            if (value.Sign == 0) return Tlv(TagInteger, new byte[] { 0x00 });
            var bytes = new List<byte>(value.ToByteArray());
            while (bytes.Count > 1 && bytes[bytes.Count - 1] == 0) bytes.RemoveAt(bytes.Count - 1);
            bytes.Reverse();
            if (bytes[0] >= 0x80) bytes.Insert(0, 0x00);
            return Tlv(TagInteger, bytes.ToArray());
        }
        /// <summary>
        /// 编码无符号大端整型
        /// </summary>
        /// <param name="value">大端字节</param>
        /// <returns>DER字节</returns>
        public static byte[] Integer(byte[] value)
        {
            if (value == null || value.Length == 0) value = new byte[] { 0x00 };
            var bytes = new List<byte>(value);
            while (bytes.Count > 1 && bytes[0] == 0x00) bytes.RemoveAt(0);
            if (bytes[0] >= 0x80) bytes.Insert(0, 0x00);
            return Tlv(TagInteger, bytes.ToArray());
        }
        /// <summary>
        /// 编码比特串
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="unusedBits">未使用位数</param>
        /// <returns>DER字节</returns>
        public static byte[] BitString(byte[] data, byte unusedBits = 0)
        {
            if (data == null) data = Array.Empty<byte>();
            var content = new byte[data.Length + 1];
            content[0] = unusedBits;
            Array.Copy(data, 0, content, 1, data.Length);
            return Tlv(TagBitString, content);
        }
        /// <summary>
        /// 编码八位字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>DER字节</returns>
        public static byte[] OctetString(byte[] data) => Tlv(TagOctetString, data ?? Array.Empty<byte>());
        /// <summary>
        /// 编码NULL
        /// </summary>
        /// <returns>DER字节</returns>
        public static byte[] Null() => new byte[] { TagNull, 0x00 };
        /// <summary>
        /// 编码布尔值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>DER字节</returns>
        public static byte[] Boolean(Boolean value) => Tlv(TagBoolean, new byte[] { value ? (byte)0xFF : (byte)0x00 });
        /// <summary>
        /// 编码UTF8字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>DER字节</returns>
        public static byte[] Utf8String(string value) => Tlv(TagUtf8String, Encoding.UTF8.GetBytes(value ?? ""));
        /// <summary>
        /// 编码可打印字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>DER字节</returns>
        public static byte[] PrintableString(string value) => Tlv(TagPrintableString, Encoding.ASCII.GetBytes(value ?? ""));
        #endregion

        #region 对象标识符
        /// <summary>
        /// 编码OID
        /// </summary>
        /// <param name="oid">对象标识符 如 1.2.840.113549.1.1.11</param>
        /// <returns>DER字节</returns>
        public static byte[] Oid(string oid)
        {
            var parts = (oid ?? "").Split('.');
            var bytes = new List<byte>();
            var first = int.Parse(parts[0]);
            var second = int.Parse(parts[1]);
            bytes.Add((byte)(first * 40 + second));
            for (var i = 2; i < parts.Length; i++)
            {
                var value = ulong.Parse(parts[i]);
                var stack = new Stack<byte>();
                stack.Push((byte)(value & 0x7F));
                value >>= 7;
                while (value > 0)
                {
                    stack.Push((byte)((value & 0x7F) | 0x80));
                    value >>= 7;
                }
                while (stack.Count > 0) bytes.Add(stack.Pop());
            }
            return Tlv(TagOid, bytes.ToArray());
        }
        /// <summary>
        /// 编码算法标识符
        /// </summary>
        /// <param name="oid">算法OID</param>
        /// <param name="parameters">算法参数</param>
        /// <returns>DER字节</returns>
        public static byte[] AlgorithmIdentifier(string oid, byte[] parameters = null) => Sequence(Oid(oid), parameters ?? Null());
        #endregion

        #region 时间
        /// <summary>
        /// 编码UTCTime
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>DER字节</returns>
        public static byte[] UtcTime(DateTime time) => Tlv(TagUtcTime, Encoding.ASCII.GetBytes(time.ToUniversalTime().ToString("yyMMddHHmmss") + "Z"));
        /// <summary>
        /// 编码GeneralizedTime
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>DER字节</returns>
        public static byte[] GeneralizedTime(DateTime time) => Tlv(TagGeneralizedTime, Encoding.ASCII.GetBytes(time.ToUniversalTime().ToString("yyyyMMddHHmmss") + "Z"));
        /// <summary>
        /// 编码时间 1950-2049之间使用UTCTime 超出使用GeneralizedTime
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>DER字节</returns>
        public static byte[] Time(DateTime time) => time.ToUniversalTime().Year >= 1950 && time.ToUniversalTime().Year <= 2049 ? UtcTime(time) : GeneralizedTime(time);
        #endregion

        #region 上下文标签
        /// <summary>
        /// 编码上下文指定标签
        /// </summary>
        /// <param name="number">标签号</param>
        /// <param name="content">内容</param>
        /// <param name="constructed">是否为结构类型</param>
        /// <returns>DER字节</returns>
        public static byte[] Context(int number, byte[] content, Boolean constructed = true)
        {
            var tag = (byte)((constructed ? 0xA0 : 0x80) | (number & 0x1F));
            return Tlv(tag, content);
        }
        #endregion

        #region PEM
        /// <summary>
        /// DER转PEM
        /// </summary>
        /// <param name="der">DER数据</param>
        /// <param name="type">类型 如 CERTIFICATE、PUBLIC KEY、PRIVATE KEY</param>
        /// <returns>PEM字符串</returns>
        public static string ToPem(byte[] der, string type)
        {
            if (der == null || der.Length == 0) return string.Empty;
            var base64 = Convert.ToBase64String(der);
            var sb = new StringBuilder();
            sb.Append("-----BEGIN ").Append(type).Append("-----\n");
            for (var i = 0; i < base64.Length; i += 64)
            {
                var len = Math.Min(64, base64.Length - i);
                sb.Append(base64, i, len).Append("\n");
            }
            sb.Append("-----END ").Append(type).Append("-----\n");
            return sb.ToString();
        }
        /// <summary>
        /// PEM转DER
        /// </summary>
        /// <param name="pem">PEM字符串</param>
        /// <returns>DER数据</returns>
        public static byte[] FromPem(string pem)
        {
            if (pem.IsNullOrEmpty()) return Array.Empty<byte>();
            var sb = new StringBuilder();
            foreach (var line in pem.Replace("\r", "").Split('\n'))
            {
                var val = line.Trim();
                if (val.StartsWith("-----", StringComparison.OrdinalIgnoreCase)) continue;
                sb.Append(val);
            }
            var data = sb.ToString();
            if (data.IsNullOrEmpty()) return Array.Empty<byte>();
            return Convert.FromBase64String(data);
        }
        #endregion
    }
}
