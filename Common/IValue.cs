using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-04-23 18:46:28                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 值类型
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        object Parse(string value);
    }
    /// <summary>
    /// 值类型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface IValue<T> : IValue
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="result">转换值</param>
        /// <returns></returns>
        bool TryParse(string value, out T result);
    }
}