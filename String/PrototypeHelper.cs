using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Data.SQL;

namespace XiaoFeng
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public partial class PrototypeHelper
    {
        /// <summary>
        /// 扩展String IndexOf
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="value">子字符串</param>
        /// <returns></returns>
        public static int IndexOfX(this string _, string value) =>
#if NETSTANDARD2_1
            _.AsSpan().IndexOf(value.AsSpan());
#else
            _.IndexOf(value);
#endif
        /// <summary>
        /// 扩展String LastIndexOf
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="value">子字符串</param>
        /// <returns></returns>
        public static int LastIndexOfX(this string _, string value) =>
#if NETSTANDARD2_1
            _.AsSpan().LastIndexOf(value.AsSpan());
#else
            _.LastIndexOf(value);
#endif
        /// <summary>
        /// 扩展String Substring
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">结束位置</param>
        /// <returns></returns>
        public static string SubstringX(this string _, int start, int length)
        {
            if (length <= 0 || length < start) return string.Empty;
            return
#if NETSTANDARD2_1
            _.AsSpan().Slice(start, length).ToString()
#else
            _.Substring(start, length)
#endif
                ;
        }
        /// <summary>
        /// 扩展String Substring
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="start">开始位置</param>
        /// <returns></returns>
        public static string SubstringX(this string _, int start) => _.SubstringX(start, _.Length - start);
        /// <summary>
        /// 扩展String Replace
        /// </summary>
        /// <param name="_">字符串</param>
        /// <param name="oldValue">子字符串</param>
        /// <param name="newValue">替换后子符串</param>
        /// <returns></returns>
        public static string ReplaceX(this string _, string oldValue, string newValue)
        {
#if NETSTANDARD2_1
            var list = new List<string>();
            var strSpan = _.AsSpan();
            var subSpan = oldValue.AsSpan();
            var n = strSpan.IndexOf(subSpan);
            while (n > -1)
            {
                list.Add(strSpan.Slice(0, n).ToString());
                strSpan = strSpan.Slice(n + subSpan.Length);
                n = strSpan.IndexOf(subSpan);
            }
            return list.Join(newValue);
#else
            return _.Replace(oldValue, newValue);
#endif
        }
        /// <summary>
        /// 字符串是否闭合
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        public static Boolean IsClosure(this string _)
        {
            var index = -1;
            Pair Current = null;
            int fBreak = 0;
            for (var i = 0; i < _.Length; i++)
            {
                var c = _[i];
                switch (c)
                {
                    case '{':
                    case '[':
                    case '(':
                    case '<':
                        if (Current == null)
                            Current = new Pair(c);
                        else
                        {
                            var cp = new Pair(c);
                            cp.ParentPair = Current;
                            if (Current.ChildPair == null)
                                Current.ChildPair = new List<Pair> { cp };
                            else
                                Current.ChildPair.Add(cp);
                            Current = cp;
                        }
                        break;
                    case '}':
                    case ']':
                    case ')':
                    case '>':
                        if (Current == null)
                        {
                            fBreak = -1;
                            break;
                        }
                        Current.IsPair = true;
                        if (Current.ParentPair == null)
                        {
                            fBreak = 1;
                            index = i;
                            break;
                        }
                        Current = Current.ParentPair;
                        break;
                    default:

                        break;
                }
                if (fBreak != 0) break;
            }
            if (Current == null) return false;
            return index > -1 ? Current.IsPair && _.Substring(index + 1).IsNullOrEmpty() : Current.IsPair;
        }
    }
}