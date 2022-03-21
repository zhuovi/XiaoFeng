using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-07 18:36:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 缓存 基类
    /// </summary>
    public class BaseCache
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseCache()
        {
            this.Config = CacheConfig.Get();
            if (this.Config.CacheKey.IsNullOrEmpty()) this.Config.CacheKey = "FAYELF_CACHE";
        }
        #endregion

        #region 属性
        /// <summary>
        /// 缓存配置
        /// </summary>
        public CacheConfig Config { get; set; }
        /// <summary>
        /// 缓存KEY
        /// </summary>
        public const string CacheKey = "FAYELF_CACHE";
        #endregion

        #region 方法
        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <param name="isValue">是否是值类型</param>
        /// <returns></returns>
        protected string GetValue<T>(T value, out Boolean isValue)
        {
            var type = value.GetType();
            var valueType = type.GetValueType();
            isValue = false;
            if (valueType == ValueTypes.Null || valueType == ValueTypes.String || valueType == ValueTypes.Value || valueType == ValueTypes.Other)
            {
                isValue = true;
                return CacheKey + "->" + ((type == typeof(DateTime) || type == typeof(DateTime?)) ? value.ToCast<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ffffff") : value.ToString());
            }
            else return CacheKey + "->" + value.ToJson();
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        protected string GetValue<T>(T value) => this.GetValue(value, out _);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="typeName">类型名称</param>
        /// <returns></returns>
        protected object SetValue(string value, string typeName)
        {
            Type BaseType = typeName.IsNullOrEmpty() ? typeof(string) : Type.GetType(typeName);
            value = value.RemovePattern($@"^[\s\S]*?{CacheKey}->");
            var type = BaseType.GetValueType();
            return (type == ValueTypes.Null || type == ValueTypes.String || type == ValueTypes.Value || type == ValueTypes.Other) ? value.GetValue(BaseType) : value.ToString().JsonToObject(BaseType);
        }
        #endregion
        #endregion
    }
}