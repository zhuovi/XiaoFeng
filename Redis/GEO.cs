using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-07 11:13:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// GEO
    /// </summary>
    public partial class RedisClient : Disposable, IRedisClient
    {
        #region GEO
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        public int GeoAdd(string key, int? dbNum, params GeoModel[] geos)
        {
            if (key.IsNullOrEmpty() || geos.Length == 0) return -1;
            var list = new List<object>() { key };
            geos.Each(g =>
            {
                list.Add(g.Longitude);
                list.Add(g.Latitude);
                list.Add(g.Address);
            });
            return this.Execute(CommandType.GEOADD, dbNum, result => result.OK ? (int)result.Value : -1, list.ToArray());
        }
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        public async Task<int> GeoAddAsync(string key, int? dbNum, params GeoModel[] geos)
        {
            if (key.IsNullOrEmpty() || geos.Length == 0) return -1;
            var list = new List<object>() { key };
            geos.Each(g =>
            {
                list.Add(g.Longitude);
                list.Add(g.Latitude);
                list.Add(g.Address);
            });
            return await this.ExecuteAsync(CommandType.GEOADD, dbNum, async result => await Task.FromResult(result.OK ? (int)result.Value : -1), list.ToArray());
        }
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        public int GeoAdd(string key, params GeoModel[] geos) => this.GeoAdd(key, null, geos);
        /// <summary>
        /// 存储指定的地理空间位置，可以将一个或多个经度(longitude)、纬度(latitude)、位置名称(member)添加到指定的 key 中 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="geos">经纬度集</param>
        /// <returns>添加成功数量</returns>
        public async Task<int> GeoAddAsync(string key, params GeoModel[] geos) => await this.GeoAddAsync(key, null, geos);
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        public List<GeoModel> GetGeoPos(string key, int? dbNum, params object[] members)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GEOPOS, dbNum, result => result.OK ? (List<GeoModel>)result.Value.Value : null, new object[] { key }.Concat(members).ToArray());
        }
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dbNum">库索引</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        public async Task<List<GeoModel>> GetGeoPosAsync(string key, int? dbNum, params object[] members)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GEOPOS, dbNum, async result => await Task.FromResult(result.OK ? (List<GeoModel>)result.Value.Value : null), new object[] { key }.Concat(members).ToArray());
        }
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        public List<GeoModel> GetGeoPos(string key, params object[] members) => this.GetGeoPos(key, null, members);
        /// <summary>
        /// 用于从给定的 key 里返回所有指定名称(member)的位置（经度和纬度），不存在的返回 nil。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="members">元素</param>
        /// <returns>位置列表</returns>
        public async Task<List<GeoModel>> GetGeoPosAsync(string key, params object[] members) => await this.GetGeoPosAsync(key, null, members);
        /// <summary>
        /// 返回两个给定位置之间的距离
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="firstMember">第一个位置</param>
        /// <param name="secondMember">第二个位置</param>
        /// <param name="unitType">单位类型</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置之间的距离</returns>
        public double GetGeoDist(string key, string firstMember, string secondMember, UnitType unitType = UnitType.M, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || firstMember.IsNullOrEmpty() || secondMember.IsNullOrEmpty()) return -1;
            return this.Execute(CommandType.GEODIST, dbNum, result => result.OK ? result.Value.ToDouble() : -1, key, firstMember, secondMember, unitType.ToString().ToLower());
        }
        /// <summary>
        /// 返回两个给定位置之间的距离 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="firstMember">第一个位置</param>
        /// <param name="secondMember">第二个位置</param>
        /// <param name="unitType">单位类型</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置之间的距离</returns>
        public async Task<double> GetGeoDistAsync(string key, string firstMember, string secondMember, UnitType unitType = UnitType.M, int? dbNum = null)
        {
            if (key.IsNullOrEmpty() || firstMember.IsNullOrEmpty() || secondMember.IsNullOrEmpty()) return -1;
            return await this.ExecuteAsync(CommandType.GEODIST, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToDouble() : -1), key, firstMember, secondMember, unitType.ToString().ToLower());
        }
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
        public List<string> GetGeoHash(string key, int? dbNum, params object[] members)
        {
            if (key.IsNullOrEmpty() || members.Length == 0) return null;
            return this.Execute(CommandType.GEOHASH, dbNum, result => result.OK ? result.Value.ToList<string>() : null, new object[] { key }.Concat(members).ToArray());
        }
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
        public async Task<List<string>> GetGeoHashAsync(string key, int? dbNum, params object[] members)
        {
            if (key.IsNullOrEmpty() || members.Length == 0) return null;
            return await this.ExecuteAsync(CommandType.GEOHASH, dbNum, async result => await Task.FromResult(result.OK ? result.Value.ToList<string>() : null), new object[] { key }.Concat(members).ToArray());
        }
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
        public List<string> GetGeoHash(string key, params object[] members) => this.GetGeoHash(key, null, members);
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
        public async Task<List<string>> GetGeoHashAsync(string key, params object[] members) => await this.GetGeoHashAsync(key, null, members);
        /// <summary>
        /// 以给定的经纬度为中心,返回键包含的位置元素当中,与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        public List<GeoRadiusModel> GetGeoRadius(string key, GeoRadiusOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GEORADIUS, dbNum, result => result.OK ? (List<GeoRadiusModel>)result.Value.Value : null, new object[] { key }.Concat(options.ToArgments(CommandType.GEORADIUS)).ToArray());
        }
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        public async Task<List<GeoRadiusModel>> GetGeoRadiusAsync(string key, GeoRadiusOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GEORADIUS, dbNum, async result => await Task.FromResult(result.OK ? (List<GeoRadiusModel>)result.Value.Value : null), new object[] { key }.Concat(options.ToArgments(CommandType.GEORADIUS)).ToArray());
        }
        /// <summary>
        /// 以给定的位置元素为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        public List<GeoRadiusModel> GetGeoRadiusByMember(string key, GeoRadiusOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(CommandType.GEORADIUSBYMEMBER, dbNum, result => result.OK ? (List<GeoRadiusModel>)result.Value.Value : null, new object[] { key }.Concat(options.ToArgments(CommandType.GEORADIUSBYMEMBER)).ToArray());
        }
        /// <summary>
        /// 以给定的位置元素为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        public async Task<List<GeoRadiusModel>> GetGeoRadiusByMemberAsync(string key, GeoRadiusOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(CommandType.GEORADIUSBYMEMBER, dbNum, async result => await Task.FromResult(result.OK ? (List<GeoRadiusModel>)result.Value.Value : null), new object[] { key }.Concat(options.ToArgments(CommandType.GEORADIUSBYMEMBER)).ToArray());
        }
        /// <summary>
        /// 搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。 6.2版本以后使用
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        public List<GeoRadiusModel> SearchGeoRadius(string key, GeoRadiusSearchOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return this.Execute(options.StoreDistKey.IsNotNullOrEmpty() ? CommandType.GEOSEARCH : CommandType.GEOSEARCHSTORE, dbNum, result => result.OK ? (List<GeoRadiusModel>)result.Value.Value : null, new object[] { key }.Concat(options.ToArgments()).ToArray());
        }
        /// <summary>
        /// 搜索以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。6.2版本以后使用 异步
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="options">选项</param>
        /// <param name="dbNum">库索引</param>
        /// <returns>位置信息</returns>
        public async Task<List<GeoRadiusModel>> SearchGeoRadiusAsync(string key, GeoRadiusSearchOptions options, int? dbNum = null)
        {
            if (key.IsNullOrEmpty()) return null;
            return await this.ExecuteAsync(options.StoreDistKey.IsNotNullOrEmpty() ? CommandType.GEOSEARCH : CommandType.GEOSEARCHSTORE, dbNum, async result => await Task.FromResult(result.OK ? (List<GeoRadiusModel>)result.Value.Value : null), new object[] { key }.Concat(options.ToArgments()).ToArray());
        }
        #endregion
    }
}