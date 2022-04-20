using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.IO;
using XiaoFeng.Threading;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-06 09:42:39                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// Redis缓存操作类
    /// </summary>
    public class RedisCache : BaseCache, IMemoryCacheX
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public RedisCache() : base()
        {
            this.Redis = new Redis.RedisClient(this.Config.ConnectionStringKey);

            /*Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(2000);
                    Console.WriteLine("检查一次");
                    var now = DateTime.Now;
                    this.Redis.GetHash(this.Config.CacheKey).Each(kv =>
                    {
                        var value = kv.Value;
                        var model = CacheModel.Parse(value);
                        if (model.ExpireTime.HasValue && model.ExpireTime <= now)
                            this.Redis.DelHash(this.Config.CacheKey, kv.Key);
                    });
                }
            }, TaskCreationOptions.LongRunning);*/
        }
        #endregion

        #region 属性
        /// <summary>
        /// Redis
        /// </summary>
        public Redis.RedisClient Redis { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public void Clear()
        {
            this.Redis.DelKey(this.Config.CacheKey);
        }
        /// <inheritdoc/>
        public bool Contains(string key)
        {
            if (key.IsNullOrEmpty()) return false;
            return this.Redis.ExistsHash(this.Config.CacheKey, key);
        }
        ///<inheritdoc/>
        public object Get(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            string content = this.Redis.GetHash(this.Config.CacheKey, key);
            /*var Tags = content.GetMatch(@"\/\*[\s\S]+\*\/");
            if (Tags.IsNullOrEmpty()) return null;
            var dict = Tags.GetMatchs($@"^\/\*{FileHelper.NewLine}\*Cache->CreateTime:(?<CreateTime>[^\r\n]*){FileHelper.NewLine}\*Cache->ExpireTime:(?<ExpireTime>[^\r\n]*){FileHelper.NewLine}\*Cache->ObjectType:(?<ObjectType>[^\r\n]*){FileHelper.NewLine}\*Cache->Path:(?<Path>[^\r\n]*){FileHelper.NewLine}\*\/");
            var CreateTime = dict["CreateTime"];
            var ExpireTime = dict["ExpireTime"];
            var ObjectType = dict["ObjectType"];
            var Path = dict["Path"];

            if (ExpireTime.IsDateTime())
            {
                if (ExpireTime.ToCast<DateTime>() <= DateTime.Now)
                    this.Remove(key);
            }
            else if (Path.IsNotNullOrEmpty())
            {
                if (!FileHelper.Exists(Path))
                    this.Remove(key);
            }*/
            var model = CacheModel.Parse(content);
            if (model == null) return null;
            if (model.ExpireTime.HasValue)
            {
                if (model.ExpireTime.Value < DateTime.Now)
                    this.Remove(key);
                else
                {
                    if (model.IsSliding && model.ExpiresIn.HasValue && model.ExpiresIn > 0)
                    {
                        model.ExpireTime = DateTime.Now.AddMilliseconds(model.ExpiresIn.Value);
                        this.Redis.SetHash(this.Config.CacheKey, key, model.ToString());
                    }
                }
            }
            if (model.Path.IsNotNullOrEmpty())
            {
                if (!FileHelper.Exists(model.Path))
                    this.Remove(key);
            }
            return model.Value;
            //return this.SetValue(content, ObjectType);
        }
        ///<inheritdoc/>
        public T Get<T>(string key) where T : class
        {
            if (key.IsNullOrEmpty()) return default(T);
            var val = this.Get(key);
            return (T)val;
        }
        ///<inheritdoc/>
        public void Remove(string key)
        {
            if (key.IsNullOrEmpty()) return;
            this.Redis.DelHash(this.Config.CacheKey, key);
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key.IsNullOrEmpty() || value == null || expiresIn.TotalMilliseconds <= 1000) return false;

            //var val = @"/*{5}*Cache->CreateTime:{0}{5}*Cache->ExpireTime:{1}{5}*Cache->ObjectType:{2}{5}*Cache->Path:{3}{5}*/{5}{4}".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.AddMilliseconds(expiresIn.TotalMilliseconds).ToString("yyyy-MM-dd HH:mm:ss.fff"), value.GetType().AssemblyQualifiedName, "", this.GetValue(value), FileHelper.NewLine);
            var val = new CacheModel
            {
                ExpireTime = DateTime.Now.AddMilliseconds(expiresIn.TotalMilliseconds),
                Value = value,
                ExpiresIn = expiresIn.TotalMilliseconds,
                IsSliding = isSliding
            };
            return this.Redis.SetHash(this.Config.CacheKey, key, val.ToString());
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, string path)
        {
            if (key.IsNullOrEmpty() || value.IsNullOrEmpty() || path.IsNullOrEmpty()) return false;
            //var val = @"/*{5}*Cache->CreateTime:{0}{5}*Cache->ExpireTime:{1}{5}*Cache->ObjectType:{2}{5}*Cache->Path:{3}{5}*/{5}{4}".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", value.GetType().AssemblyQualifiedName, path.GetBasePath(), this.GetValue(value), FileHelper.NewLine);
            var val = new CacheModel
            {
                Value = value,
                Path = path
            };
            return this.Redis.SetHash(this.Config.CacheKey, key, val.ToString());
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key.IsNullOrEmpty() || value == null) return false;
            return this.Set(key, value, TimeSpan.FromMilliseconds(Math.Max(expiresSliding.TotalMilliseconds, expiressAbsoulte.TotalMilliseconds)), TimeSpan.Zero != expiressAbsoulte);
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, long cacheTime)
        {
            if (key.IsNullOrEmpty() || value == null) return false;
            return this.Set(key, value, TimeSpan.FromMilliseconds(cacheTime));
        }
        ///<inheritdoc/>
        public bool Set(string key, object value)
        {
            if (key.IsNullOrEmpty() || value == null) return false;
            //var val = @"/*{5}*Cache->CreateTime:{0}{5}*Cache->ExpireTime:{1}{5}*Cache->ObjectType:{2}{5}*Cache->Path:{3}{5}*/{5}{4}".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", value.GetType().AssemblyQualifiedName, "", this.GetValue(value), FileHelper.NewLine);
            var val = new CacheModel
            {
                Value = value
            };
            return this.Redis.SetHash(this.Config.CacheKey, key, val.ToString());
        }
        #endregion
    }
}