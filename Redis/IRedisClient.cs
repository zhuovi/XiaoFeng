using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-05 17:25:27                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis接口
    /// </summary>
    public interface IRedisClient
    {
        #region GEO
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        int GeoAdd(string key, int? dbNum, params GeoModel[] geos);
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        Task<int> GeoAddAsync(string key, int? dbNum, params GeoModel[] geos);
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        int GeoAdd(string key, params GeoModel[] geos) => this.GeoAdd(key, null, geos);
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        Task<int> GeoAddAsync(string key, params GeoModel[] geos);
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        List<GeoModel> GetGeoPos(string key, int? dbNum, params object[] members);
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        Task<List<GeoModel>> GetGeoPosAsync(string key, int? dbNum, params object[] members);
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        List<GeoModel> GetGeoPos(string key, params object[] members);
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
       Task<List<GeoModel>> GetGeoPosAsync(string key, params object[] members);
        /// <summary>
        /// 返回两个给定位置之间的距离
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="firstMember">第一个位置</param>
        /// <param name="secondMember">第二个位置</param>
        /// <param name="unitType">单位类型</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置之间的距离</returns>
        double GetGeoDist(string key, string firstMember, string secondMember, UnitType unitType = UnitType.M, int? dbNum = null);
        /// <summary>
        /// 返回两个给定位置之间的距离 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="firstMember">第一个位置</param>
        /// <param name="secondMember">第二个位置</param>
        /// <param name="unitType">单位类型</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置之间的距离</returns>
        Task<double> GetGeoDistAsync(string key, string firstMember, string secondMember, UnitType unitType = UnitType.M, int? dbNum = null);
        /// <summary>
        /// 获取一个或多个位置元素的 geohash 值
        /// GeoHash 编码长度与精度
        /// 长度  Lat位数   Lng位数   Lat误差           Lng误差               KM误差
        ///   1     2           3      ±23               ±23                 ±2500
        ///   2     5           5      ±2.8              ±5.6                ±630
        ///   3     7           8      ±0.70             ±0.76               ±78
        ///   4     10          10     ±0.087            ±0.18               ±20
        ///   5     12          13     ±0.022            ±0.022              ±2.4
        ///   6     15          15     ±0.0027           ±0.0055             ±0.61
        ///   7     17          18     ±0.00068          ±0.00068            ±0.0761
        ///   8     20          20     ±0.000086         ±0.000172           ±0.01911
        ///   9     22          23     ±0.000021         ±0.000021           ±0.00478
        ///   10    25          25     ±0.00000268       ±0.00000536         ±0.0005971
        ///   11    27          28     ±0.00000067       ±0.00000067         ±0.0001492
        ///   12    30          30     ±0.00000008       ±0.00000017         ±0.0000186
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">元素</param>
        /// <returns>HASH列表</returns>
        List<string> GetGeoHash(string key, int? dbNum, params object[] members);
        /// <summary>
        /// 获取一个或多个位置元素的 geohash 值 异步
        /// GeoHash 编码长度与精度
        /// 长度  Lat位数   Lng位数   Lat误差           Lng误差               KM误差
        ///   1     2           3      ±23               ±23                 ±2500
        ///   2     5           5      ±2.8              ±5.6                ±630
        ///   3     7           8      ±0.70             ±0.76               ±78
        ///   4     10          10     ±0.087            ±0.18               ±20
        ///   5     12          13     ±0.022            ±0.022              ±2.4
        ///   6     15          15     ±0.0027           ±0.0055             ±0.61
        ///   7     17          18     ±0.00068          ±0.00068            ±0.0761
        ///   8     20          20     ±0.000086         ±0.000172           ±0.01911
        ///   9     22          23     ±0.000021         ±0.000021           ±0.00478
        ///   10    25          25     ±0.00000268       ±0.00000536         ±0.0005971
        ///   11    27          28     ±0.00000067       ±0.00000067         ±0.0001492
        ///   12    30          30     ±0.00000008       ±0.00000017         ±0.0000186
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">元素</param>
        /// <returns>HASH列表</returns>
        Task<List<string>> GetGeoHashAsync(string key, int? dbNum, params object[] members);
        /// <summary>
        /// 获取一个或多个位置元素的 geohash 值
        /// GeoHash 编码长度与精度
        /// 长度  Lat位数   Lng位数   Lat误差           Lng误差               KM误差
        ///   1     2           3      ±23               ±23                 ±2500
        ///   2     5           5      ±2.8              ±5.6                ±630
        ///   3     7           8      ±0.70             ±0.76               ±78
        ///   4     10          10     ±0.087            ±0.18               ±20
        ///   5     12          13     ±0.022            ±0.022              ±2.4
        ///   6     15          15     ±0.0027           ±0.0055             ±0.61
        ///   7     17          18     ±0.00068          ±0.00068            ±0.0761
        ///   8     20          20     ±0.000086         ±0.000172           ±0.01911
        ///   9     22          23     ±0.000021         ±0.000021           ±0.00478
        ///   10    25          25     ±0.00000268       ±0.00000536         ±0.0005971
        ///   11    27          28     ±0.00000067       ±0.00000067         ±0.0001492
        ///   12    30          30     ±0.00000008       ±0.00000017         ±0.0000186
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">元素</param>
        /// <returns>HASH列表</returns>
        List<string> GetGeoHash(string key, params object[] members) => this.GetGeoHash(key, null, members);
        /// <summary>
        /// 获取一个或多个位置元素的 geohash 值 异步
        /// GeoHash 编码长度与精度
        /// 长度  Lat位数   Lng位数   Lat误差           Lng误差               KM误差
        ///   1     2           3      ±23               ±23                 ±2500
        ///   2     5           5      ±2.8              ±5.6                ±630
        ///   3     7           8      ±0.70             ±0.76               ±78
        ///   4     10          10     ±0.087            ±0.18               ±20
        ///   5     12          13     ±0.022            ±0.022              ±2.4
        ///   6     15          15     ±0.0027           ±0.0055             ±0.61
        ///   7     17          18     ±0.00068          ±0.00068            ±0.0761
        ///   8     20          20     ±0.000086         ±0.000172           ±0.01911
        ///   9     22          23     ±0.000021         ±0.000021           ±0.00478
        ///   10    25          25     ±0.00000268       ±0.00000536         ±0.0005971
        ///   11    27          28     ±0.00000067       ±0.00000067         ±0.0001492
        ///   12    30          30     ±0.00000008       ±0.00000017         ±0.0000186
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">元素</param>
        /// <returns>HASH列表</returns>
        Task<List<string>> GetGeoHashAsync(string key, params object[] members);
        /// <summary>
        /// 以给定的经纬度为中心,返回键包含的位置元素当中,与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        List<GeoRadiusModel> GetGeoRadius(string key, GeoRadiusOptions options, int? dbNum = null);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        Task<List<GeoRadiusModel>> GetGeoRadiusAsync(string key, GeoRadiusOptions options, int? dbNum = null);
        /// <summary>
        /// 以给定的位置元素为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        List<GeoRadiusModel> GetGeoRadiusByMember(string key, GeoRadiusOptions options, int? dbNum = null);
        /// <summary>
        /// 以给定的位置元素为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        Task<List<GeoRadiusModel>> GetGeoRadiusByMemberAsync(string key, GeoRadiusOptions options, int? dbNum = null);
        /// <summary>
        /// 搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。 6.2版本以后使用
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        List<GeoRadiusModel> SearchGeoRadius(string key, GeoRadiusSearchOptions options, int? dbNum = null);
        /// <summary>
        /// 搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。6.2版本以后使用 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        Task<List<GeoRadiusModel>> SearchGeoRadiusAsync(string key, GeoRadiusSearchOptions options, int? dbNum = null);
        #endregion

        #region 哈希(Hash)

        #region 设置Hash
        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Boolean SetHash<T>(string key, string fieldName, T value, int? dbNum = null);
        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Task<Boolean> SetHashAsync<T>(string key, string fieldName, T value, int? dbNum = null);
        /// <summary>
        /// 只有在字段 field 不存在时，设置哈希表字段的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean SetHashNoExists<T>(string key, string fieldName, T value, int? dbNum = null);
        /// <summary>
        /// 只有在字段 field 不存在时，设置哈希表字段的值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Task<Boolean> SetHashNoExistsAsync<T>(string key, string fieldName, T value, int? dbNum = null);
        /// <summary>
        /// 批量设置Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">字段值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean SetHash(string key, Dictionary<string, object> values, int? dbNum = null);
        /// <summary>
        /// 批量设置Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">字段值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> SetHashAsync(string key, Dictionary<string, object> values, int? dbNum = null);
        #endregion

        #region 获取Hash
        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        T GetHash<T>(string key, string field, int? dbNum = null);
        /// <summary>
        /// 获取Hash值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<T> GetHashAsync<T>(string key, string field, int? dbNum = null);
        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>Hash值</returns>
        string GetHash(string key, string field, int? dbNum = null);
        /// <summary>
        /// 获取Hash值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>Hash值</returns>
        Task<string> GetHashAsync(string key, string field, int? dbNum = null);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, string> GetHash(string key, int? dbNum = null);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetHashAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, T> GetHash<T>(string key, int? dbNum = null);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, T>> GetHashAsync<T>(string key, int? dbNum = null);
        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        List<string> GetHashKeys(string key, int? dbNum = null);
        /// <summary>
        /// 获取所有哈希表中的字段 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        Task<List<string>> GetHashKeysAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        List<string> GetHashValues(string key, int? dbNum = null);
        /// <summary>
        /// 获取所有哈希表中的字段 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>所有哈希表中的字段</returns>
        Task<List<string>> GetHashValuesAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>哈希表中字段的数量</returns>
        int GetHashKeysCount(string key, int? dbNum = null);
        /// <summary>
        /// 获取哈希表中字段的数量 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>哈希表中字段的数量</returns>
        Task<int> GetHashKeysCountAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取所有给定字段的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns>返回所有给定字段的值</returns>
        List<string> GetHashValues(string key, int? dbNum, params object[] fields);
        /// <summary>
        /// 获取所有给定字段的值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns>返回所有给定字段的值</returns>
        Task<List<string>> GetHashValuesAsync(string key, int? dbNum, params object[] fields);
        /// <summary>
        /// 获取所有给定字段的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns>返回字段值</returns>
        List<string> GetHashValues(string key, params object[] fields);
        /// <summary>
        /// 获取所有给定字段的值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns>返回字段值</returns>
        Task<List<string>> GetHashValuesAsync(string key, params object[] fields);
        /// <summary>
        /// 查找Hash中字段名
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        Dictionary<string, string> SearchHashMember(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Hash中字段名
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        Dictionary<string, T> SearchHashMember<T>(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Hash中字段名 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        Task<Dictionary<string, string>> SearchHashMemberAsync(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Hash中字段名 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        Task<Dictionary<string, T>> SearchHashMemberAsync<T>(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        #endregion

        #region 删除Hash
        /// <summary>
        /// 删除Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        int DelHash(string key, int? dbNum, params object[] fields);
        /// <summary>
        /// 删除Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        Task<int> DelHashAsync(string key, int? dbNum, params object[] fields);
        /// <summary>
        /// 删除Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        int DelHash(string key, params object[] fields);
        /// <summary>
        /// 删除Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        Task<int> DelHashAsync(string key, params object[] fields);
        #endregion

        #region 是否存在Hash
        /// <summary>
        /// 是否存在Hash
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean ExistsHash(string key, string field, int? dbNum = null);
        /// <summary>
        /// 是否存在Hash 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> ExistsHashAsync(string key, string field, int? dbNum = null);
        #endregion

        #region 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        long HashIncrement(string key, string field, long increment, int? dbNum = null);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        Task<long> HashIncrementAsync(string key, string field, long increment, int? dbNum = null);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        float HashIncrement(string key, string field, float increment, int? dbNum = null);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        Task<float> HashIncrementAsync(string key, string field, float increment, int? dbNum = null);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        double HashIncrement(string key, string field, double increment, int? dbNum = null);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="field">字段名</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>增加后的值</returns>
        Task<double> HashIncrementAsync(string key, string field, double increment, int? dbNum = null);
        #endregion

        #endregion

        #region HyperLogLog
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="elements">元素</param>
        /// <returns>成功状态</returns>
        Boolean SetHyperLogLog(string key, int? dbNum, params object[] elements);
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="elements">元素</param>
        /// <returns>成功状态</returns>
        Task<Boolean> SetHyperLogLogAsync(string key, int? dbNum, params object[] elements);
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="elements">元素</param>
        /// <returns>成功状态</returns>
        Boolean SetHyperLogLog(string key, params object[] elements);
        /// <summary>
        /// 添加指定元素到 HyperLogLog 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="elements">元素</param>
        /// <returns>成功状态</returns>
        Task<Boolean> SetHyperLogLogAsync(string key, params object[] elements);
        /// <summary>
        /// 获取基数估算值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>估算值</returns>
        int GetHyperLogLog(string key, int? dbNum = null);
        /// <summary>
        /// 获取基数估算值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>估算值</returns>
        Task<int> GetHyperLogLogAsync(string key, int? dbNum = null);
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="sourceKey">源key</param>
        /// <returns>成功状态</returns>
        Boolean MergeHyperLogLog(string destKey, int? dbNum, params object[] sourceKey);
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog 异步
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="sourceKey">源key</param>
        /// <returns>成功状态</returns>
        Task<Boolean> MergeHyperLogLogAsync(string destKey, int? dbNum, params object[] sourceKey);
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="sourceKey">源key</param>
        /// <returns>成功状态</returns>
        Boolean MergeHyperLogLog(string destKey, params object[] sourceKey);
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog 异步
        /// </summary>
        /// <param name="destKey">目的key</param>
        /// <param name="sourceKey">源key</param>
        /// <returns>成功状态</returns>
        Task<Boolean> MergeHyperLogLogAsync(string destKey, params object[] sourceKey);
        #endregion

        #region KEY

        #region 删除key
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        int DelKey(int? dbNum, params string[] keys);
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        int DelKey(params string[] keys);
        /// <summary>
        /// 删除key 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        Task<int> DelKeyAsync(int? dbNum, params string[] keys);
        /// <summary>
        /// 删除key 异步
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns>删除成功的数量</returns>
        Task<int> DelKeyAsync(params string[] keys);
        /// <summary>
        /// 获取key值 并删除 6.2.0后可用
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>删除key的值</returns>
        string GetDelKey(string key, int? dbNum = null);
        /// <summary>
        /// 获取key值 并删除 6.2.0后可用 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>删除key的值</returns>
        Task<string> GetDelKeyAsync(string key, int? dbNum = null);
        #endregion

        #region 序列化key
        /// <summary>
        /// 序列化key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        String DumpKey(string key, int? dbNum = null);
        /// <summary>
        /// 序列化key 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<String> DumpKeyAsync(string key, int? dbNum = null);
        #endregion

        #region 是否存在key
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean ExistsKey(string key, int? dbNum = null);
        /// <summary>
        /// 是否存在key 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> ExistsKeyAsync(string key, int? dbNum = null);
        #endregion

        #region 设置过期时间
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="seconds">过期时长 单位为秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        int SetKeyExpireSeconds(string key, int seconds, int? dbNum = null);
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="seconds">过期时长 单位为秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        Task<int> SetKeyExpireSecondsAsync(string key, int seconds, int? dbNum = null);
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="milliseconds">过期时长 单位为毫秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        int SetKeyExpireMilliseconds(string key, long milliseconds, int? dbNum = null);
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="milliseconds">过期时长 单位为毫秒</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        Task<int> SetKeyExpireMillisecondsAsync(string key, long milliseconds, int? dbNum = null);
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamp">过期时长 秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        int SetKeyExpireSecondsTimestamp(string key, int timestamp, int? dbNum = null);
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamp">过期时长 秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        Task<int> SetKeyExpireSecondsTimestampAsync(string key, int timestamp, int? dbNum = null);
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamps">过期时长 毫秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        int SetKeyExpireMillisecondsTimestamp(string key, long timestamps, int? dbNum = null);
        /// <summary>
        /// 设置过期时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="timestamps">过期时长 毫秒时间戳</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>设置成功数量 0是不存在 1是设置成功</returns>
        Task<int> SetKeyExpireMillisecondsTimestampAsync(string key, long timestamps, int? dbNum = null);
        #endregion

        #region 重命名key
        /// <summary>
        /// 重命名key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Boolean RenameKey(string key, string newKey, int? dbNum = null);
        /// <summary>
        /// 重命名key 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Task<Boolean> RenameKeyAsync(string key, string newKey, int? dbNum = null);
        /// <summary>
        /// 重命名key 当新key不存在时
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Boolean RenameKeyNoExists(string key, string newKey, int? dbNum = null);
        /// <summary>
        /// 重命名key 当新key不存在时 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="newKey">新key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Task<Boolean> RenameKeyNoExistsAsync(string key, string newKey, int? dbNum = null);
        #endregion

        #region 移动key
        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Boolean MoveKey(string key, int destDbNum, int? dbNum = null);
        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Task<Boolean> MoveKeyAsync(string key, int destDbNum, int? dbNum = null);
        #endregion

        #region 移除过期时间
        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Boolean RemoveKeyExpire(string key, int? dbNum = null);
        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回成功状态</returns>
        Task<Boolean> RemoveKeyExpireAsync(string key, int? dbNum = null);
        #endregion

        #region 获取key剩余过期时间
        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        int GetKeyExpireSeconds(string key, int? dbNum = null);
        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        Task<int> GetKeyExpireSecondsAsync(string key, int? dbNum = null);
        /// <summary>
        /// 以毫秒为单位，返回给定 key 的剩余生存时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        int GetKeyExpireMilliseconds(string key, int? dbNum = null);
        /// <summary>
        /// 以毫秒为单位，返回给定 key 的剩余生存时间 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回剩余时间</returns>
        Task<int> GetKeyExpireMillisecondsAsync(string key, int? dbNum = null);
        #endregion

        #region 从当前数据库中随机返回一个 key
        /// <summary>
        /// 从当前数据库中随机返回一个 key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回KEY</returns>
        string GetKeyRandom(int? dbNum = null);
        /// <summary>
        /// 从当前数据库中随机返回一个 key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回KEY</returns>
        Task<string> GetKeyRandomAsync(int? dbNum);
        #endregion

        #region 返回 key 所储存的值的类型
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回数据类型</returns>
        RedisKeyType GetKeyType(string key, int? dbNum = null);
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回数据类型</returns>
        Task<RedisKeyType> GetKeyTypeAsync(string key, int? dbNum = null);
        #endregion

        #region 查找key
        /// <summary>
        /// 查找数据库中的数据库键
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">返回条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> GetKeys(string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找数据库中的数据库键 异步
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> GetKeysAsync(string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找所有符合给定模式(pattern)的 key
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> SearchKeys(string pattern, int? dbNum = null);
        /// <summary>
        /// 查找所有符合给定模式(pattern)的 key 异步
        /// </summary>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> SearchKeysAsync(string pattern, int? dbNum = null);
        #endregion

        #region 复制 Key
        /// <summary>
        /// 复制 Key 6.2.0版本
        /// </summary>
        /// <param name="key">源 key</param>
        /// <param name="destKey">目标 key</param>
        /// <param name="isReplace">存在是否替换</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean CopyKey(string key, string destKey, Boolean isReplace = false, int? destDbNum = null, int? dbNum = null);
        /// <summary>
        /// 复制 Key 异步 6.2.0版本
        /// </summary>
        /// <param name="key">源 key</param>
        /// <param name="destKey">目标 key</param>
        /// <param name="isReplace">存在是否替换</param>
        /// <param name="destDbNum">目标库索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> CopyKeyAsync(string key, string destKey, Boolean isReplace = false, int? destDbNum = null, int? dbNum = null);
        #endregion

        #endregion

        #region 字符串(String)

        #region 设置字符串
        /// <summary>
        /// 设置字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <param name="dbNum">数据库</param>
        /// <returns>是否设置成功</returns>
        Boolean SetString<T>(string key, T value, TimeSpan? timeSpan = null, int? dbNum = null);
        /// <summary>
        /// 设置字符串 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Task<Boolean> SetStringAsync<T>(string key, T value, TimeSpan? timeSpan = null, int? dbNum = null);
        /// <summary>
        /// 批量设置值
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean SetString(Dictionary<string, object> values, int? dbNum = null);
        /// <summary>
        /// 批量设置值 异步
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> SetStringAsync(Dictionary<string, object> values, int? dbNum = null);
        /// <summary>
        /// 设置字符串 key不存在时
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Boolean SetStringNoExists<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 设置字符串 key不存在时 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Task<Boolean> SetStringNoExistsAsync<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 批量设置值 key不存在时
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean SetStringNoExists(Dictionary<string, object> values, int? dbNum = null);
        /// <summary>
        /// 批量设置值 key不存在时 异步
        /// </summary>
        /// <param name="values">key值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> SetStringNoExistsAsync(Dictionary<string, object> values, int? dbNum = null);
        /// <summary>
        /// 设置字符串 覆盖给定 key 所储存的字符串值，覆盖的位置从偏移量 offset 开始
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="offset">偏移量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Boolean SetString(string key, string value, int offset, int? dbNum = null);
        /// <summary>
        /// 设置字符串 覆盖给定 key 所储存的字符串值，覆盖的位置从偏移量 offset 开始 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="offset">偏移量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否设置成功</returns>
        Task<Boolean> SetStringAsync(string key, string value, int offset, int? dbNum = null);
        /// <summary>
        /// 给指定的key值附加到原来值的尾部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean AppendString(string key, string value, int? dbNum = null);
        /// <summary>
        /// 给指定的key值附加到原来值的尾部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> AppendStringAsync(string key, string value, int? dbNum = null);
        #endregion

        #region 获取字符串
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        T GetString<T>(string key, int? dbNum = null);
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        string GetString(string key, int? dbNum = null);
        /// <summary>
        /// 获取字符串 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        Task<T> GetStringAsync<T>(string key, int? dbNum = null);
        /// <summary>
        /// 获取字符串 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值</returns>
        Task<string> GetStringAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">终止位置</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值的子字符串</returns>
        string GetString(string key, int start, int end, int? dbNum = null);
        /// <summary>
        /// 获取字符串 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">终止位置</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的值的子字符串</returns>
        Task<string> GetStringAsync(string key, int start, int end, int? dbNum = null);
        /// <summary>
        /// 获取 key 值的长度
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        int GetStringLength(string key, int? dbNum = null);
        /// <summary>
        /// 获取 key 值的长度 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<int> GetStringLengthAsync(string key, int? dbNum = null);
        #endregion

        #region 设置key的新值并返回key旧值
        /// <summary>
        /// 设置key的新值并返回key旧值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">key的新值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的旧值</returns>
        T GetSetString<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 设置key的新值并返回key旧值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">key的新值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>key的旧值</returns>
        Task<T> GetSetStringAsync<T>(string key, T value, int? dbNum = null);
        #endregion

        #region 获取所有(一个或多个)给定key的值
        /// <summary>
        /// 获取所有(一个或多个)给定key的值
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        List<string> GetString(int? dbNum, params object[] args);
        /// <summary>
        /// 获取所有(一个或多个)给定key的值 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        Task<List<string>> GetStringAsync(int? dbNum, params object[] args);
        /// <summary>
        /// 获取所有(一个或多个)给定key的值
        /// </summary>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        List<string> GetString(params object[] args);
        /// <summary>
        /// 获取所有(一个或多个)给定key的值 异步
        /// </summary>
        /// <param name="args">key</param>
        /// <returns>按顺序返回key值</returns>
        Task<List<string>> GetStringAsync(params object[] args);
        #endregion

        #region 设置自增长
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        double StringIncrement(string key, double increment, int? dbNum = null);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<double> StringIncrementAsync(string key, double increment, int? dbNum = null);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        float StringIncrement(string key, float increment, int? dbNum = null);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<float> StringIncrementAsync(string key, float increment, int? dbNum = null);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        long StringIncrement(string key, long increment, int? dbNum = null);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="increment">增量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<long> StringIncrementAsync(string key, long increment, int? dbNum = null);
        /// <summary>
        /// 将 key 中储存的数字值增一
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        int StringIncrement(string key, int? dbNum = null);
        /// <summary>
        /// 将 key 中储存的数字值增一 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<int> StringIncrementAsync(string key, int? dbNum = null);
        /// <summary>
        /// key 所储存的值减去给定的减量值（decrement）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        long StringDecrement(string key, long decrement, int? dbNum = null);
        /// <summary>
        /// key 所储存的值减去给定的减量值（decrement） 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="decrement">减量值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<long> StringDecrementAsync(string key, long decrement, int? dbNum = null);
        /// <summary>
        /// 将 key 中储存的数字值减一
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        int StringDecrement(string key, int? dbNum = null);
        /// <summary>
        /// 将 key 中储存的数字值减一 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<int> StringDecrementAsync(string key, int? dbNum = null);
        #endregion

        #endregion

        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">排序选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> Sort(string key, SortOptions options, int? dbNum = null);
        /// <summary>
        /// 排序 异步 List,Set,SortedSet
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">排序选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> SortAsync(string key, SortOptions options, int? dbNum = null);
        #endregion

        #region 列表(List)

        #region 设置列表
        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Boolean SetListItem<T>(string key, int index, T value, int? dbNum = null);
        /// <summary>
        /// 通过索引设置列表元素的值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Task<Boolean> SetListItemAsync<T>(string key, int index, T value, int? dbNum = null);
        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Boolean SetListItem(string key, int index, string value, int? dbNum = null);
        /// <summary>
        /// 通过索引设置列表元素的值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Task<Boolean> SetListItemAsync(string key, int index, string value, int? dbNum = null);
        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        int SetListItemBefore(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 将一个或多个值插入到列表头部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemBeforeAsync(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        int SetListItemBefore(string key, params object[] values);
        /// <summary>
        /// 将一个或多个值插入到列表头部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemBeforeAsync(string key, params object[] values);
        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        int SetListItem(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 在列表中添加一个或多个值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemAsync(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        int SetListItem(string key, params object[] values);
        /// <summary>
        /// 在列表中添加一个或多个值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemAsync(string key, params object[] values);
        /// <summary>
        /// 在列表的元素前插入元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int InsertListItemBefore<T>(string key, string item, T value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素前插入元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> InsertListItemBeforeAsync<T>(string key, string item, T value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素前插入元素
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int InsertListItemBefore(string key, string item, string value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素前插入元素 异步
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> InsertListItemBeforeAsync(string key, string item, string value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素后插入元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int InsertListItemAfter<T>(string key, string item, T value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素后插入元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> InsertListItemAfterAsync<T>(string key, string item, T value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素后插入元素
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int InsertListItemAfter(string key, string item, string value, int? dbNum = null);
        /// <summary>
        /// 在列表的元素后插入元素 异步
        /// </summary>
        /// <param name="key">列表key</param>
        /// <param name="item">元素</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> InsertListItemAfterAsync(string key, string item, string value, int? dbNum = null);
        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int GetListCount(string key, int? dbNum = null);
        /// <summary>
        /// 获取列表长度 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> GetListCountAsync(string key, int? dbNum = null);
        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int SetListItemBeforeExists<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 将一个值插入到已存在的列表头部 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemBeforeExistsAsync<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int SetListItemBeforeExists(string key, string value, int? dbNum = null);
        /// <summary>
        /// 将一个值插入到已存在的列表头部 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemBeforeExistsAsync(string key, string value, int? dbNum = null);
        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int SetListItemExists<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 为已存在的列表添加值 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemExistsAsync<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        int SetListItemExists(string key, string value, int? dbNum = null);
        /// <summary>
        /// 为已存在的列表添加值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表长度</returns>
        Task<int> SetListItemExistsAsync(string key, string value, int? dbNum = null);
        #endregion

        #region 获取并移除
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        Dictionary<string, T> GetListFirstItem<T>(int? dbNum, int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        Task<Dictionary<string, T>> GetListFirstItemAsync<T>(int? dbNum, int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        Dictionary<string, string> GetListFirstItem(int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        Task<Dictionary<string, string>> GetListFirstItemAsync(int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        Dictionary<string, string> GetListFirstItem(params object[] keys);
        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的第一个元素</returns>
        Task<Dictionary<string, string>> GetListFirstItemAsync(params object[] keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        Dictionary<string, T> GetListLastItem<T>(int? dbNum, int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        Task<Dictionary<string, T>> GetListLastItemAsync<T>(int? dbNum, int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        Dictionary<string, string> GetListLastItem(int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        Task<Dictionary<string, string>> GetListLastItemAsync(int? timeout, params object[] keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        Dictionary<string, string> GetListLastItem(params object[] keys);
        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>列表的最后一个元素</returns>
        Task<Dictionary<string, string>> GetListLastItemAsync(params object[] keys);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        T GetListLastItemToOtherListFirst<T>(string key, string otherKey, int? timeout = 0, int? dbNum = null);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        Task<T> GetListLastItemToOtherListFirstAsync<T>(string key, string otherKey, int? timeout = 0, int? dbNum = null);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止
        /// </summary>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        string GetListLastItemToOtherListFirst(string key, string otherKey, int? timeout = 0, int? dbNum = null);
        /// <summary>
        /// 从列表中取出最后一个元素，并插入到另外一个列表的头部； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止 异步
        /// </summary>
        /// <param name="key">源列表key</param>
        /// <param name="otherKey">目标列表key</param>
        /// <param name="timeout">超时时间 单位为秒 0一直等待</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        Task<string> GetListLastItemToOtherListFirstAsync(string key, string otherKey, int? timeout = 0, int? dbNum = null);
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        T GetListItem<T>(string key, int index, int? dbNum = null);
        /// <summary>
        /// 通过索引获取列表中的元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        Task<T> GetListItemAsync<T>(string key, int index, int? dbNum = null);
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        string GetListItem(string key, int index, int? dbNum = null);
        /// <summary>
        /// 通过索引获取列表中的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>列表中的元素</returns>
        Task<string> GetListItemAsync(string key, int index, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        T GetListFirstItem<T>(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的第一个元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        Task<T> GetListFirstItemAsync<T>(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        string GetListFirstItem(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的第一个元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>第一个元素</returns>
        Task<string> GetListFirstItemAsync(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的最后一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        T GetListLastItem<T>(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的最后一个元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        Task<T> GetListLastItemAsync<T>(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的最后一个元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        string GetListLastItem(string key, int? dbNum = null);
        /// <summary>
        /// 移出并获取列表的最后一个元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>最后一个元素</returns>
        Task<string> GetListLastItemAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始索引 可以用负数 如 -1代表最后一个</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>数据列表</returns>
        List<T> GetListItems<T>(string key, int start, int stop, int? dbNum = null);
        /// <summary>
        /// 获取列表中指定区间内的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引 可以用负数 如 -1代表最后一个</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>数据列表</returns>
        Task<List<T>> GetListItemsAsync<T>(string key, int start, int stop, int? dbNum = null);
        /// <summary>
        /// 获取列表中指定区间内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引 可以用负数 如 -1代表最后一个</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>数据列表</returns>
        List<string> GetListItems(string key, int start, int stop, int? dbNum = null);
        /// <summary>
        /// 获取列表中指定区间内的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引 可以用负数 如 -1代表最后一个</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>数据列表</returns>
        Task<List<string>> GetListItemsAsync(string key, int start, int stop, int? dbNum = null);
        #endregion

        #region 移除
        /// <summary>
        /// 根据参数 COUNT 的值，移除列表中与参数 VALUE 相等的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="count">移除数量 取绝对值 负数从表头开始向表尾搜索，正数从表尾开始向表头搜索 0移除所有</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除元素数量</returns>
        int DelListItem(string key, string value, int count, int? dbNum = null);
        /// <summary>
        /// 根据参数 COUNT 的值，移除列表中与参数 VALUE 相等的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="count">移除数量 取绝对值 负数从表头开始向表尾搜索，正数从表尾开始向表头搜索 0移除所有</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除元素数量</returns>
        Task<int> DelListItemAsync(string key, string value, int count, int? dbNum = null);
        /// <summary>
        /// 删除不在区间内的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除元素数量</returns>
        Boolean DelListItem(string key, int start, int stop, int? dbNum = null);
        /// <summary>
        /// 删除不在区间内的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除元素数量</returns>
        Task<Boolean> DelListItemAsync(string key, int start, int stop, int? dbNum = null);
        #endregion

        #endregion

        #region 脚本

        #region 执行 Lua 脚本
        /// <summary>
        /// 执行 Lua 脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        RedisValue EvalScript(string script, string[] parameters = null, string[] keys = null, int? dbNum = null);
        /// <summary>
        /// 异步执行 Lua 脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        Task<RedisValue> EvalScriptAsync(string script, string[] parameters = null, string[] keys = null, int? dbNum = null);
        #endregion

        #region 执行 Lua 脚本 根据SHA1码
        /// <summary>
        /// 执行 Lua 脚本
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        RedisValue EvalSHA(string shacode, string[] parameters = null, string[] keys = null, int? dbNum = null);
        /// <summary>
        /// 异步执行 Lua 脚本 根据SHA1码
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <param name="parameters">参数</param>
        /// <param name="keys">Keys</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>执行结果</returns>
        Task<RedisValue> EvalSHAAsync(string shacode, string[] parameters = null, string[] keys = null, int? dbNum = null);
        #endregion

        #region 查看指定的脚本是否已经被保存在缓存当中
        /// <summary>
        /// 查看指定的脚本是否已经被保存在缓存当中
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <returns></returns>
        RedisValue ScriptExists(params string[] shacode);
        /// <summary>
        /// 查看指定的脚本是否已经被保存在缓存当中
        /// </summary>
        /// <param name="shacode">Lua 脚本 sha1码</param>
        /// <returns></returns>
        Task<RedisValue> ScriptExistsAsync(params string[] shacode);
        #endregion

        #region 从脚本缓存中移除所有脚本
        /// <summary>
        /// 从脚本缓存中移除所有脚本
        /// </summary>
        /// <returns></returns>
        Boolean ScriptFlush();
        /// <summary>
        /// 从脚本缓存中移除所有脚本
        /// </summary>
        /// <returns></returns>
        Task<Boolean> ScriptFlushAsync();
        #endregion

        #region 杀死当前正在运行的 Lua 脚本
        /// <summary>
        /// 杀死当前正在运行的 Lua 脚本
        /// </summary>
        /// <returns></returns>
        Boolean ScriptKill();
        /// <summary>
        /// 杀死当前正在运行的 Lua 脚本
        /// </summary>
        /// <returns></returns>
        Task<Boolean> ScriptKillAsync();
        #endregion

        #region 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本
        /// <summary>
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <returns>返回sha1编码</returns>
        string ScriptLoad(string script);
        /// <summary>
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <returns>返回sha1编码</returns>
        Task<string> ScriptLoadAsync(string script);
        #endregion

        #endregion

        #region 服务器
        /// <summary>
        /// 异步执行一个 AOF（AppendOnly File） 文件重写操作。
        /// </summary>
        Boolean BackgroundRewriteAOF();
        /// <summary>
        /// 后台异步保存当前数据库的数据到磁盘。
        /// </summary>
        /// <param name="dbNum">库索引</param>
        Boolean BackgroundSave(int? dbNum = null);
        /// <summary>
        /// 客户端命令
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        RedisReader Client(string key, params object[] values);
        /// <summary>
        /// 客户端命令 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        Task<RedisReader> ClientAsync(string key, params object[] values);
        /// <summary>
        /// 杀死客户端
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Boolean ClientKill(string host, int port);
        /// <summary>
        /// 杀死客户端 异步
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Task<Boolean> ClientKillAsync(string host, int port);
        /// <summary>
        /// 在指定时间内终止运行来自客户端的命令
        /// </summary>
        /// <param name="timeout">时间 单位为毫秒</param>
        /// <returns></returns>
        Boolean ClientPause(long timeout);
        /// <summary>
        /// 在指定时间内终止运行来自客户端的命令 异步
        /// </summary>
        /// <param name="timeout">时间 单位为毫秒</param>
        /// <returns></returns>
        Task<Boolean> ClientPauseAsync(long timeout);
        /// <summary>
        /// 客户端列表
        /// </summary>
        /// <returns></returns>
        List<ClientInfo> ClientList();
        /// <summary>
        /// 客户端列表 异步
        /// </summary>
        /// <returns></returns>
        Task<List<ClientInfo>> ClientListAsync();
        /// <summary>
        /// 设置客户端名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Boolean ClientSetName(string name);
        /// <summary>
        /// 设置客户端名称 异步
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task<Boolean> ClientSetNameAsync(string name);
        /// <summary>
        /// 获取客户端名称
        /// </summary>
        /// <returns></returns>
        string ClientGetName();
        /// <summary>
        /// 获取客户端名称 异步
        /// </summary>
        /// <returns></returns>
        Task<string> ClientGetNameAsync();
        /// <summary>
        /// 将当前服务器转变为指定服务器的从属服务器(slave server)
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Boolean SlavEOF(string host, int port);
        /// <summary>
        /// 将当前服务器转变为指定服务器的从属服务器(slave server) 异步
        /// </summary>
        /// <param name="host">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Task<Boolean> SlavEOFAsync(string host, int port);
        /// <summary>
        /// 异步保存数据到硬盘，并关闭服务器
        /// </summary>
        /// <param name="isSave">是否保存数据</param>
        /// <returns></returns>
        Boolean ShutDown(Boolean isSave = true);
        /// <summary>
        /// 异步保存数据到硬盘，并关闭服务器 异步
        /// </summary>
        /// <param name="isSave">是否保存数据</param>
        /// <returns></returns>
        Task<Boolean> ShutDownAsync(Boolean isSave = true);
        /// <summary>
        /// 一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。
        /// </summary>
        /// <returns></returns>
        Boolean Save();
        /// <summary>
        /// 一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。 异步
        /// </summary>
        /// <returns></returns>
        Task<Boolean> SaveAsync();
        /// <summary>
        /// 查看主从实例所属的角色，角色有master, slave, sentinel。
        /// </summary>
        /// <returns></returns>
        RedisValue Role();
        /// <summary>
        /// 查看主从实例所属的角色，角色有master, slave, sentinel。 异步
        /// </summary>
        /// <returns></returns>
        Task<RedisValue> RolesAsync();
        /// <summary>
        /// 返回当前数据库的 key 的数量
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        int GetDbKeySize(int? dbNum = null);
        /// <summary>
        /// 返回当前数据库的 key 的数量 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<int> GetDbKeySizeAsync(int? dbNum = null);
        /// <summary>
        /// 删除当前数据库的所有key
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Boolean DelDbKeys(int? dbNum = null);
        /// <summary>
        /// 删除当前数据库的所有key 异步
        /// </summary>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Boolean> DelDbKeysAsync(int? dbNum = null);
        /// <summary>
        /// 删除所有数据库的所有key
        /// </summary>
        /// <returns></returns>
        Boolean DelAllKeys();
        /// <summary>
        /// 删除所有数据库的所有key 异步
        /// </summary>
        /// <returns></returns>
        Task<Boolean> DelAllKeysAsync();
        /// <summary>
        /// 服务器配置命令
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        RedisReader Config(string key, params object[] values);
        /// <summary>
        /// 服务器配置命令 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        Task<RedisReader> ConfigAsync(string key, params object[] values);
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns></returns>
        Dictionary<string, string> GetConfig(string key = "*");
        /// <summary>
        /// 获取配置 异步
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetConfigAsync(string key = "*");
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Boolean SetConfig(string key, string value);
        /// <summary>
        /// 设置配置 异步
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<Boolean> SetConfigAsync(string key, string value);
        /// <summary>
        /// 对启动 Redis 服务器时所指定的 redis.conf 配置文件进行改写
        /// </summary>
        /// <returns></returns>
        Boolean ConfigRewrite();
        /// <summary>
        /// 对启动 Redis 服务器时所指定的 redis.conf 配置文件进行改写 异步
        /// </summary>
        /// <returns></returns>
        Task<Boolean> ConfigRewriteAsync();
        #endregion

        #region 集合(Set)

        #region 设置
        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        int SetSetMember(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 向集合添加一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        Task<int> SetSetMemberAsync(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        int SetSetMember(string key, params object[] values);
        /// <summary>
        /// 向集合添加一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>添加数量</returns>
        Task<int> SetSetMemberAsync(string key, params object[] values);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        Boolean MoveSetMember<T>(string key, string destKey, T value, int? dbNum = null);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        Task<Boolean> MoveSetMemberAsync<T>(string key, string destKey, T value, int? dbNum = null);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        Boolean MoveSetMember(string key, string destKey, string value, int? dbNum = null);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合 异步
        /// </summary>
        /// <param name="key">源key</param>
        /// <param name="destKey">目标key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否成功</returns>
        Task<Boolean> MoveSetMemberAsync(string key, string destKey, string value, int? dbNum = null);
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        int DelSetMember(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 移除集合中一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        Task<int> DelSetMemberAsync(string key, int? dbNum, params object[] values);
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        int DelSetMember(string key, params object[] values);
        /// <summary>
        /// 移除集合中一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <returns>移除数量</returns>
        Task<int> DelSetMemberAsync(string key, params object[] values);
        #endregion

        #region 获取
        /// <summary>
        /// 获取集合的成员数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员数</returns>
        int GetSetCount(string key, int? dbNum = null);
        /// <summary>
        /// 获取集合的成员数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员数</returns>
        Task<int> GetSetCountAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        List<T> GetSetDiff<T>(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        List<string> GetSetDiff(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        List<string> GetSetDiff(string key, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<List<T>> GetSetDiffAsync<T>(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<List<string>> GetSetDiffAsync(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的差异 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<List<string>> GetSetDiffAsync(string key, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        int GetSetDiffStore(string key, string storeKey, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        int GetSetDiffStore(string key, string storeKey, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<int> GetSetDiffStoreAsync(string key, string storeKey, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<int> GetSetDiffStoreAsync(string key, string storeKey, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        List<T> GetSetInter<T>(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        List<string> GetSetInter(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        List<string> GetSetInter(string key, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        Task<List<T>> GetSetInterAsync<T>(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        Task<List<string>> GetSetInterAsync(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 获取第一个集合与其他集合之间的交集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的交集</returns>
        Task<List<string>> GetSetInterAsync(string key, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        int GetSetInterStore(string key, string storeKey, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        int GetSetInterStore(string key, string storeKey, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        Task<int> GetSetInterStoreAsync(string key, string storeKey, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回差异数量</returns>
        Task<int> GetSetInterStoreAsync(string key, string storeKey, params object[] otherKey);
        /// <summary>
        /// 所有给定集合的并集
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        List<T> GetSetUnion<T>(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 所有给定集合的并集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        List<string> GetSetUnion(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 所有给定集合的并集
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        List<string> GetSetUnion(string key, params object[] otherKey);
        /// <summary>
        /// 所有给定集合的并集 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<List<T>> GetSetUnionAsync<T>(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 所有给定集合的并集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<List<string>> GetSetUnionAsync(string key, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 所有给定集合的并集 异步
        /// </summary>
        /// <param name="key">第一个集合</param>
        /// <param name="otherKey">其他集合</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<List<string>> GetSetUnionAsync(string key, params object[] otherKey);
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        int GetSetUnionStore(string key, string storeKey, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        int GetSetUnionStore(string key, string storeKey, params object[] otherKey);
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<int> GetSetUnionStoreAsync(string key, string storeKey, int? dbNum, params object[] otherKey);
        /// <summary>
        /// 返回所有给定集合的并集并存储在 destination 中 异步
        /// </summary>
        /// <param name="key">第一个集合key</param>
        /// <param name="storeKey">存储集合key</param>
        /// <param name="otherKey">其他集合key</param>
        /// <returns>返回第一个集合与其他集合之间的差异</returns>
        Task<int> GetSetUnionStoreAsync(string key, string storeKey, params object[] otherKey);
        /// <summary>
        /// 判断成员元素是否是集合的成员
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否存在</returns>
        Boolean ExistsSetMember<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 判断成员元素是否是集合的成员 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>是否存在</returns>
        Task<Boolean> ExistsSetMemberAsync<T>(string key, T value, int? dbNum = null);
        /// <summary>
        /// 获取集合中的所有成员
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        List<T> GetSetMemberList<T>(string key, int? dbNum = null);
        /// <summary>
        /// 获取集合中的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        List<string> GetSetMemberList(string key, int? dbNum = null);
        /// <summary>
        /// 获取集合中的所有成员 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        Task<List<T>> GetSetMemberListAsync<T>(string key, int? dbNum = null);
        /// <summary>
        /// 获取集合中的所有成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>集合中的所有成员</returns>
        Task<List<string>> GetSetMemberListAsync(string key, int? dbNum = null);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        List<T> GetSetPop<T>(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        List<string> GetSetPop(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        Task<List<T>> GetSetPopAsync<T>(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 移除并返回集合中的一个或多个随机元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">移除位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>移除的元素</returns>
        Task<List<string>> GetSetPopAsync(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 获取集合中一个或多个随机数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        List<T> GetSetRandomMember<T>(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 获取集合中一个或多个随机数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        List<string> GetSetRandomMember(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 获取集合中一个或多个随机数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        Task<List<T>> GetSetRandomMemberAsync<T>(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 获取集合中一个或多个随机数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">随机位数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>随机的元素</returns>
        Task<List<string>> GetSetRandomMemberAsync(string key, int count = 1, int? dbNum = null);
        /// <summary>
        /// 查找Set中的元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        List<T> SearchSetMember<T>(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Set中的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        List<string> SearchSetMember(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Set中的元素 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        Task<List<T>> SearchSetMemberAsync<T>(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Set中的元素 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>元素</returns>
        Task<List<string>> SearchSetMemberAsync(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        #endregion

        #endregion

        #region 有序集合(ZSet)

        #region 设置
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功添加数量</returns>
        int SetSortedSetMember(string key, Dictionary<object, float> values, int? dbNum = null);
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="values">值</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功添加数量</returns>
        Task<int> SetSortedSetMemberAsync(string key, Dictionary<object, float> values, int? dbNum = null);
        #endregion

        #region 获取
        /// <summary>
        /// 获取有序集合的成员数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>有序集合的成员数</returns>
        int GetSortedSetCount(string key, int? dbNum = null);
        /// <summary>
        /// 获取有序集合的成员数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>有序集合的成员数</returns>
        Task<int> GetSortedSetCountAsync(string key, int? dbNum = null);
        /// <summary>
        /// 获取有序集合指定区间分数的成员数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>有序集合的成员数</returns>
        int GetSortedSetCount(string key, object min, object max, int? dbNum = null);
        /// <summary>
        /// 获取有序集合指定区间分数的成员数 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>有序集合的成员数</returns>
        Task<int> GetSortedSetCountAsync(string key, object min, object max, int? dbNum = null);
        /// <summary>
        /// 计算给定的一个或多个有序集的交集并将结果集存储在新的有序集合 destination 中
        /// </summary>
        /// <param name="destKey">存储key</param>
        /// <param name="options">计算项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>存储成员数</returns>
        int GetSortedSetInterStore(string destKey, SortedSetOptions options, int? dbNum = null);
        /// <summary>
        /// 计算给定的一个或多个有序集的交集并将结果集存储在新的有序集合 destination 中 异步
        /// </summary>
        /// <param name="destKey">存储key</param>
        /// <param name="options">计算项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>存储成员数</returns>
        Task<int> GetSortedSetInterStoreAsync(string destKey, SortedSetOptions options, int? dbNum = null);
        /// <summary>
        /// 计算给定的一个或多个有序集的并集并将结果集存储在新的有序集合 destination 中
        /// </summary>
        /// <param name="destKey">存储key</param>
        /// <param name="options">计算项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>存储成员数</returns>
        int GetSortedSetUnionStore(string destKey, SortedSetOptions options, int? dbNum = null);
        /// <summary>
        /// 计算给定的一个或多个有序集的并集并将结果集存储在新的有序集合 destination 中 异步
        /// </summary>
        /// <param name="destKey">存储key</param>
        /// <param name="options">计算项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>存储成员数</returns>
        Task<int> GetSortedSetUnionStoreAsync(string destKey, SortedSetOptions options, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> GetSortedSetRangeAsync(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<T>> GetSortedSetRangeAsync<T>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, float>> GetSortedSetRangeWithScoresAsync(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<TKey, TValue>> GetSortedSetRangeWithScoresAsync<TKey, TValue>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> GetSortedSetRange(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<T> GetSortedSetRange<T>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, float> GetSortedSetRangeWithScores(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递增排序
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<TKey, TValue> GetSortedSetRangeWithScores<TKey, TValue>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<T> GetSortedSetRevRange<T>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<T>> GetSortedSetRevRangeAsync<T>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> GetSortedSetRevRange(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> GetSortedSetRevRangeAsync(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<TKey, TValue> GetSortedSetRevRangeWithScores<TKey, TValue>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序 异步
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<TKey, TValue>> GetSortedSetRevRangeWithScoresAsync<TKey, TValue>(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, double> GetSortedSetRevRangeWithScores(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过索引区间返回有序集合指定区间内的成员 分数递减排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, double>> GetSortedSetRevRangeWithScoresAsync(string key, int start = 0, int stop = -1, int? dbNum = null);
        /// <summary>
        /// 通过字典区间返回有序集合的成员
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<T> GetSortedSetRangeByLex<T>(string key, string min, string max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过字典区间返回有序集合的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> GetSortedSetRangeByLex(string key, string min, string max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过字典区间返回有序集合的成员 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<T>> GetSortedSetRangeByLexAsync<T>(string key, string min, string max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过字典区间返回有序集合的成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> GetSortedSetRangeByLexAsync(string key, string min, string max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<T> GetSortedSetRangeByScore<T>(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> GetSortedSetRangeByScore(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<T>> GetSortedSetRangeByScoreAsync<T>(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> GetSortedSetRangeByScoreAsync(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<TKey, TValue> GetSortedSetRangeByScoreWithScores<TKey, TValue>(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, double> GetSortedSetRangeByScoreWithScoresWithScores(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员 异步
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<TKey, TValue>> GetSortedSetRangeByScoreWithScoresAsync<TKey, TValue>(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过分数区间返回有序集合的成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, double>> GetSortedSetRangeByScoreWithScoresAsync(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 获取有序集合中指定成员的索引
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员索引</returns>
        int GetSortedSetRank(string key, string member, int? dbNum = null);
        /// <summary>
        /// 获取有序集合中指定成员的索引 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员索引</returns>
        Task<int> GetSortedSetRankAsync(string key, string member, int? dbNum = null);
        /// <summary>
        /// 获取有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员索引</returns>
        int GetSortedSetRevRank(string key, string member, int? dbNum = null);
        /// <summary>
        /// 获取有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员索引</returns>
        Task<int> GetSortedSetRevRankAsync(string key, string member, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="max">最大分数</param>
        /// <param name="min">最小分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<T> GetSortedSetRevRangeByScore<T>(string key, float max, float min, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="max">最大分数</param>
        /// <param name="min">最小分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<string> GetSortedSetRevRangeByScore(string key, float max, float min, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序 异步
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<T>> GetSortedSetRevRangeByScoreAsync<T>(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<string>> GetSortedSetRevRangeByScoreAsync(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="max">最大分数</param>
        /// <param name="min">最小分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<TKey, TValue> GetSortedSetRevRangeByScoreWithScores<TKey, TValue>(string key, float max, float min, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="max">最大分数</param>
        /// <param name="min">最小分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, double> GetSortedSetRevRangeByScoreWithScores(string key, float max, float min, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序 异步
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        /// <typeparam name="TValue">Value类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<TKey, TValue>> GetSortedSetRevRangeByScoreWithScoresAsync<TKey, TValue>(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 通过有序集中指定分数区间内的成员，分数从高到低排序 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小分数</param>
        /// <param name="max">最大分数</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, double>> GetSortedSetRevRangeByScoreWithScoresAsync(string key, float min, float max, int start = 0, int end = -1, int? dbNum = null);
        /// <summary>
        /// 获取有序集中，成员的分数值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员索引</returns>
        float GetSortedSetScore(string key, string member, int? dbNum = null);
        /// <summary>
        /// 获取有序集中，成员的分数值 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="member">成员</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成员索引</returns>
        Task<float> GetSortedSetScoreAsync(string key, string member, int? dbNum = null);
        /// <summary>
        /// 查找Hash中字段名
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        Dictionary<string, float> SearchSortedSetMember(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        /// <summary>
        /// 查找Hash中字段名 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="pattern">模式 支持*和?</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">遍历条数</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>字段名和值</returns>
        Task<Dictionary<string, float>> SearchSortedSetMemberAsync(string key, string pattern, int start = 0, int count = 10, int? dbNum = null);
        #endregion

        #region 有序集合中对指定成员的分数加上增量 increment
        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="increment">递增量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        float SetSortedSetIncrement(string key, object value, int increment = 1, int? dbNum = null);
        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">元素</param>
        /// <param name="increment">递增量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<float> SetSortedSetIncrementAsync(string key, object value, int increment = 1, int? dbNum = null);
        #endregion

        #region 移除
        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">成员</param>
        /// <returns>成功移除个数</returns>
        int DelSortedSetMember(string key, int? dbNum, params object[] members);
        /// <summary>
        /// 移除有序集合中的一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">成员</param>
        /// <returns>成功移除个数</returns>
        Task<int> DelSortedSetMemberAsync(string key, int? dbNum, params object[] members);
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">成员</param>
        /// <returns>成功移除个数</returns>
        int DelSortedSetMember(string key, params object[] members);
        /// <summary>
        /// 移除有序集合中的一个或多个成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">成员</param>
        /// <returns>成功移除个数</returns>
        Task<int> DelSortedSetMemberAsync(string key, params object[] members);
        /// <summary>
        /// 移除有序集合中给定的字典区间的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功移除个数</returns>
        int DelSortedSetMemberByLex(string key, string min, string max, int? dbNum = null);
        /// <summary>
        /// 移除有序集合中给定的字典区间的所有成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功移除个数</returns>
        Task<int> DelSortedSetMemberByLexAsync(string key, string min, string max, int? dbNum = null);
        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功移除个数</returns>
        int DelSortedSetMemberByScore(string key, float min, float max, int? dbNum = null);
        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功移除个数</returns>
        Task<int> DelSortedSetMemberByScoreAsync(string key, float min, float max, int? dbNum = null);
        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功移除个数</returns>
        int DelSortedSetMemberByRank(string key, int min, int max, int? dbNum = null);
        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功移除个数</returns>
        Task<int> DelSortedSetMemberByRankAsync(string key, int min, int max, int? dbNum = null);
        #endregion

        #endregion

        #region 消息队列(Redis Stream)

        #region 添加消息到末尾
        /// <summary>
        /// 添加消息到末尾
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="id">消息 id，我们使用 * 表示由 redis 生成，可以自定义，但是要自己保证递增性。</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">记录值</param>
        /// <returns>返回添加的消息ID</returns>
        String AddMessageEnd(string key, string id, Dictionary<string, string> values, int? dbNum = null);
        /// <summary>
        /// 添加消息到末尾
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="id">消息 id，我们使用 * 表示由 redis 生成，可以自定义，但是要自己保证递增性。</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="values">记录值</param>
        /// <returns>返回添加的消息ID</returns>
        Task<String> AddMessageEndAsync(string key, string id, Dictionary<string, string> values, int? dbNum = null);
        #endregion

        #region 获取流包含的元素数量，即消息长度
        /// <summary>
        /// 获取流包含的元素数量
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息队列的数量</returns>
        int GetMessageLength(string key, int? dbNum = null);
        /// <summary>
        /// 获取流包含的元素数量
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息队列的数量</returns>
        Task<int> GetMessageLengthAsync(string key, int? dbNum = null);
        #endregion

        #region 对流进行修剪，限制长度
        /// <summary>
        /// 对流进行修剪，限制长度
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="length">长度</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回移除的数量</returns>
        int SetMessageLength(string key, int length, int? dbNum = null);
        /// <summary>
        /// 对流进行修剪，限制长度
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="length">长度</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回移除的数量</returns>
        Task<int> SetMessageLengthAsync(string key, int length, int? dbNum = null);
        #endregion

        #region 删除消息
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="id">消息ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回删的数量</returns>
        int DeleteMessage(string key, string id, int? dbNum = null);
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="id">消息ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回删的数量</returns>
        Task<int> DeleteMessageAsync(string key, string id, int? dbNum = null);
        #endregion

        #region  获取消息列表，会自动过滤已经删除的消息
        /// <summary>
        /// 获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        Dictionary<string, Dictionary<string, string>> GetMessageList(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null);
        /// <summary>
        /// 获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        Task<Dictionary<string, Dictionary<string, string>>> GetMessageListAsync(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null);
        #endregion

        #region  反向获取消息列表，会自动过滤已经删除的消息
        /// <summary>
        /// 反向获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        Dictionary<string, Dictionary<string, string>> GetReverseMessageList(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null);
        /// <summary>
        /// 反向获取消息列表，会自动过滤已经删除的消息
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息列队数据</returns>
        Task<Dictionary<string, Dictionary<string, string>>> GetReverseMessageListAsync(string key, int? start = null, int? end = null, int? count = null, int? dbNum = null);
        #endregion

        #region 以阻塞或非阻塞方式获取消息列表
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        /// <param name="keyIds">队列名 消息ID 集合</param>
        /// <param name="count">数量</param>
        /// <param name="milliseconds">可选，阻塞毫秒数，没有设置就是非阻塞模式</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息数据</returns>
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetMessageList(Dictionary<string, string> keyIds, int? count = null, long? milliseconds = null, int? dbNum = null);
        /// <summary>
        /// 以阻塞或非阻塞方式获取消息列表
        /// </summary>
        /// <param name="keyIds">队列名 消息ID 集合</param>
        /// <param name="count">数量</param>
        /// <param name="milliseconds">可选，阻塞毫秒数，没有设置就是非阻塞模式</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>返回消息数据</returns>
        Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> GetMessageListAsync(Dictionary<string, string> keyIds, int? count = null, long? milliseconds = null, int? dbNum = null);
        #endregion

        #region 创建消费者组
        /// <summary>
        /// 创建消费者组
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="groupName">组名</param>
        /// <param name="position">消费位置 TOP 是表示从头开始消费 END是表示从尾部开始消费，只接受新消息，当前 Stream 消息会全部忽略。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>创建成功状态</returns>
        Boolean CreateConsumerGroup(string key, string groupName, ConsumerGroupPosition position = ConsumerGroupPosition.TOP, int? dbNum = null);
        /// <summary>
        /// 创建消费者组
        /// </summary>
        /// <param name="key">队列名称，如果不存在就创建</param>
        /// <param name="groupName">组名</param>
        /// <param name="position">消费位置 TOP 是表示从头开始消费 END是表示从尾部开始消费，只接受新消息，当前 Stream 消息会全部忽略。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>创建成功状态</returns>
        Task<Boolean> CreateConsumerGroupAsync(string key, string groupName, ConsumerGroupPosition position = ConsumerGroupPosition.TOP, int? dbNum = null);
        #endregion

        #region 为消费者组设置新的最后递送消息ID
        /// <summary>
        /// 为消费者组设置新的最后递送消息ID
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="id">消息队列ID $为最后一个ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Boolean ConsumerGroupSetID(string key, string groupName, string id = "$", int? dbNum = null);
        /// <summary>
        /// 创建消费者组
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="id">消息队列ID $为最后一个ID</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Task<Boolean> ConsumerGroupSetIDAsync(string key, string groupName, string id = "$", int? dbNum = null);
        #endregion

        #region 删除消费者
        /// <summary>
        /// 删除消费者
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="consumerName">消费者名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Boolean ConsumerGroupDeleteConsumer(string key, string groupName, string consumerName, int? dbNum = null);
        /// <summary>
        /// 删除消费者
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="consumerName">消费者名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Task<Boolean> ConsumerGroupDeleteConsumerAsync(string key, string groupName, string consumerName, int? dbNum = null);
        #endregion

        #region 删除消费者组
        /// <summary>
        /// 删除消费者组
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Boolean DeleteConsumerGroup(string key, string groupName, int? dbNum = null);
        /// <summary>
        /// 删除消费者组
        /// </summary>
        /// <param name="key">队列名称</param>
        /// <param name="groupName">组名</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>成功状态</returns>
        Task<Boolean> DeleteConsumerGroupAsync(string key, string groupName, int? dbNum = null);
        #endregion

        #region 读取消费者组中的消息
        /// <summary>
        /// 读取消费者组中的消息
        /// </summary>
        /// <param name="groupName">消费组名称</param>
        /// <param name="consumerName">消费者名称，如果消费者不存在，会自动创建一个消费者</param>
        /// <param name="keyIds">指定队列名称 及ID  0-0从第一个开始 > 从下一个未消费的消息开始</param>
        /// <param name="count">本次查询的最大数量</param>
        /// <param name="ack">true 无需手动ACK，获取到消息后自动确认</param>
        /// <param name="milliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> ReadCounsumerGroupMessage(string groupName, string consumerName, Dictionary<string, string> keyIds, int count = 1, Boolean ack = false, long? milliseconds = null, int? dbNum = null);
        /// <summary>
        /// 读取消费者组中的消息
        /// </summary>
        /// <param name="groupName">消费组名称</param>
        /// <param name="consumerName">消费者名称，如果消费者不存在，会自动创建一个消费者</param>
        /// <param name="keyIds">指定队列名称 及ID  0-0从第一个开始 > 从下一个未消费的消息开始</param>
        /// <param name="count">本次查询的最大数量</param>
        /// <param name="ack">无需手动ACK，获取到消息后自动确认</param>
        /// <param name="milliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> ReadCounsumerGroupMessageAsync(string groupName, string consumerName, Dictionary<string, string> keyIds, int count = 1, Boolean ack = false, long? milliseconds = null, int? dbNum = null);
        #endregion

        #region 将消息标记为"已处理"
        /// <summary>
        /// 将消息标记为"已处理"
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="id">消息id</param>
        /// <returns>执行条数</returns>
        int ConsumerGroupAck(string key, string groupName, int? dbNum = null, params string[] id);
        /// <summary>
        /// 将消息标记为"已处理"
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="id">消息id</param>
        /// <returns>执行条数</returns>
        Task<int> ConsumerGroupAckAsync(string key, string groupName, int? dbNum = null, params string[] id);
        #endregion

        #region 显示待处理消息的相关信息
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        ConsumerGroupXPendingModel ConsumerGroupPending(string key, string groupname, int? dbNum = null);
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<ConsumerGroupXPendingModel> ConsumerGroupPendingAsync(string key, string groupname, int? dbNum = null);
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="count">数量</param>
        /// <param name="consumer">消费者</param>
        /// <param name="milliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<ConsumerGroupXPendingConsumerModel> ConsumerGroupPending(string key, string groupname, string start, string end, int? count = null, string consumer = "", long? milliseconds = null, int? dbNum = null);
        /// <summary>
        /// 显示待处理消息的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupname">组名称</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="count">数量</param>
        /// <param name="consumer">消费者</param>
        /// <param name="idleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<ConsumerGroupXPendingConsumerModel>> ConsumerGroupPendingAsync(string key, string groupname, string start, string end, int? count = null, string consumer = "", long? idleMilliseconds = null, int? dbNum = null);
        #endregion

        #region 转移消息的归属权
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, Dictionary<string, string>> ConsumerGroupXClaim(string key, string groupName, string consumer, string id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null);
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, Dictionary<string, string>>> ConsumerGroupXClaimAsync(string key, string groupName, string consumer, string id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null);
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Dictionary<string, Dictionary<string, string>> ConsumerGroupXClaim(string key, string groupName, string consumer, string[] id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null);
        /// <summary>
        /// 转移消息的归属权
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="consumer">消费者</param>
        /// <param name="id">消息ID</param>
        /// <param name="minIdleMilliseconds">当没有消息时最长等待时间</param>
        /// <param name="count">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会增加。通常XCLAIM不会更改此计数器，该计数器仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常，例如在大量传递尝试后由于某种原因从未处理过的消息。</param>
        /// <param name="idleMilliseconds">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为该消息现在有一个新的所有者尝试处理它。</param>
        /// <param name="timeMilliseconds">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。XCLAIM这对于重写 AOF 文件生成命令很有用。</param>
        /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建待处理的消息条目。但是消息必须存在于流中，否则不存在的消息的 ID 将被忽略。</param>
        /// <param name="isJustID">仅返回成功声明的消息 ID 数组，不返回实际消息。使用此选项意味着重试计数器不会增加。</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<Dictionary<string, Dictionary<string, string>>> ConsumerGroupXClaimAsync(string key, string groupName, string consumer, string[] id, long? minIdleMilliseconds, int count = 1, long? idleMilliseconds = null, long? timeMilliseconds = null, Boolean? isForce = false, Boolean? isJustID = false, int? dbNum = null);
        #endregion

        #region 查看流和消费者组的相关信息
        /// <summary>
        /// 查看流和消费者组的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<ConsumerGroupXInfoConsumerModel> ConsumerGroupXInfoConsumer(string key, string groupName, int? dbNum = null);
        /// <summary>
        /// 查看流和消费者组的相关信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="groupName">组名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<ConsumerGroupXInfoConsumerModel>> ConsumerGroupXInfoConsumerAsync(string key, string groupName, int? dbNum = null);
        #endregion

        #region 打印消费者组的信息
        /// <summary>
        /// 打印消费者组的信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        List<ConsumerGroupXInfoGroupsModel> ConsumerGroupXInfoGroups(string key, int? dbNum = null);
        /// <summary>
        /// 打印消费者组的信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<List<ConsumerGroupXInfoGroupsModel>> ConsumerGroupXInfoGroupsAsync(string key, int? dbNum = null);
        #endregion

        #region 打印流信息
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        ConsumerGroupXInfoStreamModel ConsumerGroupXInfoStream(string key, int? dbNum = null);
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<ConsumerGroupXInfoStreamModel> ConsumerGroupXInfoStreamAsync(string key, int? dbNum = null);
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="count">显示数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        ConsumerGroupXInfoStreamFullModel ConsumerGroupXInfoStream(string key, int? count, int? dbNum = null);
        /// <summary>
        /// 打印流信息
        /// </summary>
        /// <param name="key">消息队列名称</param>
        /// <param name="count">显示数量</param>
        /// <param name="dbNum">库索引</param>
        /// <returns></returns>
        Task<ConsumerGroupXInfoStreamFullModel> ConsumerGroupXInfoStreamAsync(string key, int? count, int? dbNum = null);
        #endregion

        #endregion
    }
}