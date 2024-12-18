using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-12-18 09:19:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public static class OptionsHelper
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        static OptionsHelper()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 配置KEY
        /// </summary>
        public const string OPTIONS_CONFIG = "OPTIONS-KEYS-CONFIG";
        /// <summary>
        /// 全局配置
        /// </summary>
        private static ConcurrentDictionary<string, IOptions> Collections { get; set; }
        #endregion

        #region 方法

        #region 尝试将指定的键和值添加到集合中
        /// <summary>
        /// 尝试将指定的键和值添加到集合中
        /// </summary>
        /// <typeparam name="T"><see cref="IOptions"/> 类型</typeparam>
        /// <param name="key">要添加的元素的键</param>
        /// <param name="value">要添加的元素的值。 对于引用类型，该值可以为 null</param>
        /// <returns>
        /// <para><term>true</term> 如果成功地将键/值对添加到 true；</para>
        /// <para><term>false</term> 如果该键已存在，则为 false；</para>
        /// </returns>
        public static bool TryAdd<T>(string key, T value) where T : IOptions
        {
            if (Collections == null) Collections = new ConcurrentDictionary<string, IOptions>();
            return Collections.TryAdd(key, value);
        }
        #endregion

        #region 获取与指定的键关联的值
        /// <summary>
        /// 获取与指定的键关联的值
        /// </summary>
        /// <typeparam name="T"><see cref="IOptions"/> 类型</typeparam>
        /// <param name="key">要获取的值的键</param>
        /// <param name="value">当此方法返回时，将包含具有指定键的对象；如果操作失败，则包含类型的默认值。</param>
        /// <returns>如果找到该键，则为 true；否则为 false。</returns>
        public static bool TryGetValue<T>(string key, out T value) where T : IOptions
        {
            value = default(T);
            if (Collections == null) return false;
            if (Collections.TryGetValue(key, out var val))
            {
                value = (T)val;
                return true;
            }
            return false;
        }
        #endregion

        #region 尝试移除并返回具有指定键的值
        /// <summary>
        /// 尝试移除并返回具有指定键的值
        /// </summary>
        /// <typeparam name="T"><see cref="IOptions"/> 类型</typeparam>
        /// <param name="key">要移除并返回的元素的键</param>
        /// <param name="value">包含删除的对象，或类型的默认值 <typeparamref name="T"/> （如果 key 不存在）。</param>
        /// <returns>如果成功地移除了对象，则为 true；否则为 false。</returns>
        public static bool TryRemove<T>(string key, out T value) where T : IOptions
        {
            value = default(T);
            if (Collections == null) return false;
            if (Collections.TryRemove(key, out var val))
            {
                value = (T)val;
                return true;
            }
            return false;
        }
        #endregion

        #region 如果具有 key 的现有值等于 comparisonValue，则将与 key 关联的值更新为 newValue。
        /// <summary>
        /// 如果具有 key 的现有值等于 <paramref name="comparisonValue"/>，则将与 key 关联的值更新为 newValue。
        /// </summary>
        /// <typeparam name="T"><see cref="IOptions"/> 类型</typeparam>
        /// <param name="key">与 comparisonValue 进行比较并且可能被替换的值键</param>
        /// <param name="newValue">一个值，当比较结果相等时，将替换具有指定 key 的元素的值</param>
        /// <param name="comparisonValue">与具有指定 key 的元素的值进行比较的值</param>
        /// <returns>如果具有 true 的值与 key 相等且被替换为 <paramref name="comparisonValue"/>，则为 newValue；否则为 false</returns>
        public static bool TryUpdate<T>(string key, T newValue, T comparisonValue) where T : IOptions
        {
            if (Collections == null) Collections = new ConcurrentDictionary<string, IOptions>();
            if (Collections.TryUpdate(key, newValue, comparisonValue))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 确定是否包含指定键
        /// <summary>
        /// 确定是否包含指定键。
        /// </summary>
        /// <param name="key">定位的键</param>
        /// <returns>如果 true 包含具有指定键的元素，则为 true；否则为 false。</returns>
        public static bool ContainsKey(string key)
        {
            if (key.IsNullOrEmpty()) return false;
            return Collections.ContainsKey(key);
        }
        #endregion

        #region 获取配置
        /// <summary>
        /// 获取配置
        /// </summary>
        public static ConfigOptions ConfigOptions
        {
            get
            {
                if (TryGetValue<ConfigOptions>(OPTIONS_CONFIG, out var value))
                {
                    return value;
                }
                else TryAdd(OPTIONS_CONFIG, new ConfigOptions());
                return null;
            }
            set => TryAdd<ConfigOptions>(OPTIONS_CONFIG, value);
        }
        #endregion

        #endregion
    }
}