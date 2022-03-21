using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using XiaoFeng;
/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020/6/9 10:40:14                 *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 表示程序集的版本号
    /// </summary>
    public sealed class XVersion : IComparable, IComparable<XVersion>, IEqualityComparer<XVersion>, IEquatable<XVersion>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public XVersion() { }
        /// <summary>
        /// 设置版本号
        /// </summary>
        /// <param name="version">版本号</param>
        public XVersion(string version)
        {
            if (version.IsMatch(Rule))
            {
                var m = version.Match(Rule);
                this.Major = m.Groups["a"].Value.ToCast<int>();
                var c = m.Groups["c"];
                var count = c.Captures.Count;
                if (count == 1)
                    this.Minor = c.Captures[0].Value.ToCast<int>();
                else if (count == 2)
                {
                    this.Minor = c.Captures[0].Value.ToCast<int>();
                    this.Build = c.Captures[1].Value.ToCast<int>();
                }
                else if (count == 3)
                {
                    this.Minor = c.Captures[0].Value.ToCast<int>();
                    this.Build = c.Captures[1].Value.ToCast<int>();
                    this.Revision = c.Captures[2].Value.ToCast<int>();
                }
            }
        }
        /// <summary>
        /// 设置版本号
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="build">内部版本号</param>
        public XVersion(int major, int minor, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
        }
        /// <summary>
        /// 设置版本号
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="build">内部版本号</param>
        /// <param name="revision">修订号</param>
        public XVersion(int major, int minor, int build, int revision) : this(major, minor, build)
        {
            this.Revision = revision;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 格式
        /// </summary>
        private const string Rule = @"^(?<a>\d+)(\.(?<c>\d+)){1,3}$";
        /// <summary>
        /// 主版本号
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// 次版本号
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// 内部版本号
        /// </summary>
        public int Build { get; set; }
        /// <summary>
        /// 修订号
        /// </summary>
        public int Revision { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 转换版本号
        /// </summary>
        /// <param name="v">版本号</param>
        /// <returns></returns>
        public static XVersion Parse(string v) => v;
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="_">版本</param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool TryParse(string _, out XVersion version)
        {
            version = null;
            if (_.IsNullOrEmpty()) return false;
            if (_.IsMatch(Rule))
            {
                version = _;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public int CompareTo(object obj) => this.CompareTo((XVersion)obj);
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">对象</param>
        /// <returns></returns>
        public int CompareTo(XVersion other)
        {
            if (this.Major > other.Major ||
                (this.Major == other.Major && this.Minor > other.Minor) ||
                (this.Major == other.Major && this.Minor == other.Minor && this.Build > other.Build) ||
                (this.Major == other.Major && this.Minor == other.Minor && this.Build == other.Build && this.Revision > other.Revision))
                return 1;
            else if (this.Major == other.Major && this.Minor == other.Minor && this.Build == other.Build && this.Revision == other.Revision)
                return 0;
            else
                return -1;
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public bool Equals(XVersion v1, XVersion v2) => v1.CompareTo(v2) == 0;
        /// <summary>
        /// 获取 HashCode
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public int GetHashCode(XVersion obj) => obj.GetHashCode();
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public static bool operator ==(XVersion v1, XVersion v2) => !v1.IsNullOrEmpty() && !v2.IsNullOrEmpty() && v1.Equals(v2);
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public static bool operator !=(XVersion v1, XVersion v2) => !(v1 == v2);
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public static bool operator >(XVersion v1, XVersion v2) => v1.CompareTo(v2) == 1;
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public static bool operator >=(XVersion v1, XVersion v2) => v1.CompareTo(v2) != -1;
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public static bool operator <=(XVersion v1, XVersion v2) => v1.CompareTo(v2) != 1;
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="v1">第一个对象</param>
        /// <param name="v2">第二个对象</param>
        /// <returns></returns>
        public static bool operator <(XVersion v1, XVersion v2) => v1.CompareTo(v2) == -1;
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator string(XVersion v) => v.ToString();
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator XVersion(string v) => new XVersion(v);
        /// <summary>
        /// 重写转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.Major + "." + this.Minor + "." + this.Build + "." + this.Revision;
        /// <summary>
        /// 重写相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj) => this.CompareTo(obj) == 0;
        /// <summary>
        /// 获取 HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => ToString().GetHashCode();
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="other">对象</param>
        /// <returns></returns>
        public bool Equals(XVersion other) => this.CompareTo(other) == 0;
        #endregion
    }
}