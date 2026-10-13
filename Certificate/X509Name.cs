using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Asn1;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 11:10:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Certificate
{
    /// <summary>
    /// X.509证书主题名称
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 11:10:00
    /// Description : 证书主题或颁发者名称，对应X.501 Distinguished Name。
    /// </summary>
    public class X509Name
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public X509Name()
        {
        }
        /// <summary>
        /// 设置通用名称
        /// </summary>
        /// <param name="commonName">通用名称</param>
        public X509Name(string commonName)
        {
            this.CommonName = commonName;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 国家 C 2.5.4.6
        /// </summary>
        public string Country { get; set; } = "";
        /// <summary>
        /// 省份 ST 2.5.4.8
        /// </summary>
        public string StateOrProvince { get; set; } = "";
        /// <summary>
        /// 城市 L 2.5.4.7
        /// </summary>
        public string Locality { get; set; } = "";
        /// <summary>
        /// 组织 O 2.5.4.10
        /// </summary>
        public string Organization { get; set; } = "";
        /// <summary>
        /// 组织单位 OU 2.5.4.11
        /// </summary>
        public string OrganizationalUnit { get; set; } = "";
        /// <summary>
        /// 通用名称 CN 2.5.4.3
        /// </summary>
        public string CommonName { get; set; } = "";
        /// <summary>
        /// 邮箱 E 1.2.840.113549.1.9.1
        /// </summary>
        public string Email { get; set; } = "";
        /// <summary>
        /// 序列号 serialNumber 2.5.4.5
        /// </summary>
        public string SerialNumber { get; set; } = "";
        #endregion

        #region 方法

        #region 编码
        /// <summary>
        /// 编码成RDNSequence
        /// </summary>
        /// <returns>DER字节</returns>
        public byte[] Encode()
        {
            var list = new List<byte[]>();
            AddRdn(list, "2.5.4.6", this.Country);
            AddRdn(list, "2.5.4.8", this.StateOrProvince);
            AddRdn(list, "2.5.4.7", this.Locality);
            AddRdn(list, "2.5.4.10", this.Organization);
            AddRdn(list, "2.5.4.11", this.OrganizationalUnit);
            AddRdn(list, "2.5.4.3", this.CommonName);
            AddRdn(list, "2.5.4.5", this.SerialNumber);
            AddRdn(list, "1.2.840.113549.1.9.1", this.Email);
            return DerWriter.Sequence(list.ToArray());
        }
        /// <summary>
        /// 添加RDN
        /// </summary>
        /// <param name="list">节点集</param>
        /// <param name="oid">对象标识符</param>
        /// <param name="value">值</param>
        private void AddRdn(List<byte[]> list, string oid, string value)
        {
            if (value.IsNullOrEmpty()) return;
            list.Add(DerWriter.Set(DerWriter.Sequence(DerWriter.Oid(oid), DerWriter.Utf8String(value))));
        }
        #endregion

        #region 转字符串
        /// <summary>
        /// 转成字符串 如 CN=XiaoFeng,O=ELF,C=CN
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            var list = new List<string>();
            if (this.CommonName.IsNotNullOrEmpty()) list.Add("CN=" + this.CommonName);
            if (this.Organization.IsNotNullOrEmpty()) list.Add("O=" + this.Organization);
            if (this.OrganizationalUnit.IsNotNullOrEmpty()) list.Add("OU=" + this.OrganizationalUnit);
            if (this.Locality.IsNotNullOrEmpty()) list.Add("L=" + this.Locality);
            if (this.StateOrProvince.IsNotNullOrEmpty()) list.Add("ST=" + this.StateOrProvince);
            if (this.Country.IsNotNullOrEmpty()) list.Add("C=" + this.Country);
            if (this.SerialNumber.IsNotNullOrEmpty()) list.Add("SERIALNUMBER=" + this.SerialNumber);
            if (this.Email.IsNotNullOrEmpty()) list.Add("E=" + this.Email);
            return list.ToArray().Join(",");
        }
        #endregion

        #region 解析字符串
        /// <summary>
        /// 解析字符串 如 CN=XiaoFeng,O=ELF,C=CN
        /// </summary>
        /// <param name="text">名称字符串</param>
        /// <returns>证书名称</returns>
        public static X509Name Parse(string text)
        {
            var name = new X509Name();
            if (text.IsNullOrEmpty()) return name;
            foreach (var item in text.Split(','))
            {
                var index = item.IndexOf('=');
                if (index < 0) continue;
                var key = item.Substring(0, index).Trim().ToUpper();
                var value = item.Substring(index + 1).Trim();
                switch (key)
                {
                    case "CN": name.CommonName = value; break;
                    case "O": name.Organization = value; break;
                    case "OU": name.OrganizationalUnit = value; break;
                    case "L": name.Locality = value; break;
                    case "ST": name.StateOrProvince = value; break;
                    case "C": name.Country = value; break;
                    case "E": case "EMAIL": name.Email = value; break;
                    case "SERIALNUMBER": name.SerialNumber = value; break;
                }
            }
            return name;
        }
        #endregion

        #region 解析DER
        /// <summary>
        /// 解析RDNSequence
        /// </summary>
        /// <param name="der">DER数据</param>
        /// <returns>证书名称</returns>
        public static X509Name FromDer(byte[] der)
        {
            var name = new X509Name();
            if (der == null || der.Length == 0) return name;
            var node = DerReader.Parse(der);
            foreach (var rdn in node.Children)
            {
                if (rdn.Count == 0) continue;
                var atv = rdn[0];
                if (atv.Count < 2) continue;
                var oid = atv[0].GetOid();
                var value = atv[1].GetString();
                switch (oid)
                {
                    case "2.5.4.6": name.Country = value; break;
                    case "2.5.4.8": name.StateOrProvince = value; break;
                    case "2.5.4.7": name.Locality = value; break;
                    case "2.5.4.10": name.Organization = value; break;
                    case "2.5.4.11": name.OrganizationalUnit = value; break;
                    case "2.5.4.3": name.CommonName = value; break;
                    case "2.5.4.5": name.SerialNumber = value; break;
                    case "1.2.840.113549.1.9.1": name.Email = value; break;
                }
            }
            return name;
        }
        #endregion

        #endregion
    }
}
