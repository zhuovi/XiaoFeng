using System.Collections.Generic;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-19 下午 02:58:52                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class PrototypeHelper
    {
        #region 方法
        /// <summary>
        /// 字典类型转换
        /// </summary>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="d">数据</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<string, string> d)
        {
            var ParamsTypes = d.GetType().GetGenericArguments();
            if (typeof(TKey) == ParamsTypes[0] && typeof(TValue) == ParamsTypes[1]) return (Dictionary<TKey, TValue>)d;
            var dict = new Dictionary<TKey, TValue>();
            d.Each(a => dict.Add(a.Key.ToCast<TKey>(), a.Value.ToCast<TValue>()));
            return dict;
        }
        #endregion
    }
}