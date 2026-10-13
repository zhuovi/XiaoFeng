using System;
using System.Collections.Generic;
using System.Text;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 10:20:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Asn1
{
    /// <summary>
    /// ASN.1 DER 节点
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 10:20:00
    /// </summary>
    public class DerNode
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DerNode()
        {
            this.Children = new List<DerNode>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 标签
        /// </summary>
        public byte Tag { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// 完整编码数据
        /// </summary>
        public byte[] Encoded { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// 子节点集
        /// </summary>
        public List<DerNode> Children { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>子节点</returns>
        public DerNode this[int index]
        {
            get
            {
                if (this.Children == null || index < 0 || index >= this.Children.Count) return null;
                return this.Children[index];
            }
        }
        /// <summary>
        /// 子节点数量
        /// </summary>
        public int Count => this.Children == null ? 0 : this.Children.Count;
        #endregion

        #region 方法
        /// <summary>
        /// 获取指定标签的子节点集
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>子节点集</returns>
        public List<DerNode> Where(byte tag)
        {
            var list = new List<DerNode>();
            if (this.Children == null) return list;
            foreach (var node in this.Children)
                if (node.Tag == tag) list.Add(node);
            return list;
        }
        /// <summary>
        /// 获取字符串内容
        /// </summary>
        /// <returns>字符串</returns>
        public string GetString() => Encoding.UTF8.GetString(this.Content);
        /// <summary>
        /// 获取OID
        /// </summary>
        /// <returns>OID字符串</returns>
        public string GetOid()
        {
            if (this.Content == null || this.Content.Length == 0) return string.Empty;
            var sb = new StringBuilder();
            var first = this.Content[0];
            sb.Append(first / 40).Append(".").Append(first % 40);
            ulong value = 0;
            for (var i = 1; i < this.Content.Length; i++)
            {
                value = (value << 7) | (byte)(this.Content[i] & 0x7F);
                if ((this.Content[i] & 0x80) == 0)
                {
                    sb.Append(".").Append(value);
                    value = 0;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取整型内容的大端字节
        /// </summary>
        /// <returns>无符号大端字节</returns>
        public byte[] GetIntegerBytes()
        {
            if (this.Content == null) return Array.Empty<byte>();
            var list = new List<byte>(this.Content);
            while (list.Count > 1 && list[0] == 0x00) list.RemoveAt(0);
            return list.ToArray();
        }
        /// <summary>
        /// 获取比特串内容
        /// </summary>
        /// <returns>数据</returns>
        public byte[] GetBitStringBytes()
        {
            if (this.Content == null || this.Content.Length < 2) return Array.Empty<byte>();
            var result = new byte[this.Content.Length - 1];
            Array.Copy(this.Content, 1, result, 0, result.Length);
            return result;
        }
        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns>时间</returns>
        public DateTime GetTime()
        {
            var value = Encoding.ASCII.GetString(this.Content).Trim();
            var format = value.Length == 13 ? "yyMMddHHmmssZ" : "yyyyMMddHHmmssZ";
            DateTime time;
            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal, out time))
                return time.ToLocalTime();
            return DateTime.MinValue;
        }
        #endregion
    }

    /// <summary>
    /// ASN.1 DER 解析器
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 10:20:00
    /// Description : 用于解析X.509数字证书、PKCS#8私钥、SubjectPublicKeyInfo公钥。
    /// </summary>
    public static class DerReader
    {
        #region 方法
        /// <summary>
        /// 解析DER数据
        /// </summary>
        /// <param name="data">DER数据</param>
        /// <returns>根节点</returns>
        public static DerNode Parse(byte[] data) => Parse(data, 0, data == null ? 0 : data.Length);
        /// <summary>
        /// 解析DER数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>根节点</returns>
        public static DerNode Parse(byte[] data, int offset, int length)
        {
            var node = new DerNode();
            if (data == null || length < 2) return node;
            node.Tag = data[offset];
            var pos = offset + 1;
            var contentLength = 0;
            var lenByte = data[pos++];
            if (lenByte < 0x80)
            {
                contentLength = lenByte;
            }
            else
            {
                var count = lenByte & 0x7F;
                for (var i = 0; i < count; i++)
                    contentLength = (contentLength << 8) | data[pos++];
            }
            if (contentLength < 0 || pos + contentLength > offset + length) contentLength = offset + length - pos;
            node.Content = new byte[contentLength];
            Array.Copy(data, pos, node.Content, 0, contentLength);
            node.Encoded = new byte[pos - offset + contentLength];
            Array.Copy(data, offset, node.Encoded, 0, node.Encoded.Length);
            if (IsConstructed(node.Tag))
            {
                var childOffset = 0;
                while (childOffset < contentLength)
                {
                    var child = Parse(node.Content, childOffset, contentLength - childOffset);
                    if (child.Encoded.Length == 0) break;
                    node.Children.Add(child);
                    childOffset += child.Encoded.Length;
                }
            }
            return node;
        }
        /// <summary>
        /// 是否为结构类型
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否为结构类型</returns>
        private static Boolean IsConstructed(byte tag)
        {
            if ((tag & 0x20) == 0x20) return true;
            switch (tag)
            {
                case DerWriter.TagBitString:
                case DerWriter.TagOctetString:
                    return false;
                default:
                    return false;
            }
        }
        #endregion
    }
}
