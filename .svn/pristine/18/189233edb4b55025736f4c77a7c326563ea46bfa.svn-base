using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-07-07 16:30:15                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cache
{
    /// <summary>
    /// 文件缓存
    /// </summary>
    public class FileCache :BaseCache, IMemoryCacheX
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FileCache() : base()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Clear()
        {
            FileHelper.DeleteDirectory(this.Config.CachePath.IfEmpty("Cache"));
        }
        ///<inheritdoc/>
        public bool Contains(string key)
        {
            if (key.IsNullOrEmpty()) return false;
            return FileHelper.Exists(this.GetKey(key), FileAttribute.File);
        }
        ///<inheritdoc/>
        public object Get(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            var path = this.GetKey(key);
            if (FileHelper.Exists(path, FileAttribute.File))
            {
                string content = FileHelper.OpenText(path);
                /*var Tags = content.GetMatch(@"\/\*[\s\S]+\*\/");
                if (Tags.IsNullOrEmpty()) return null;
                var dict = Tags.GetMatchs(@"^\/\*
\*Cache->CreateTime:(?<CreateTime>[^\r\n]*)
\*Cache->ExpireTime:(?<ExpireTime>[^\r\n]*)
\*Cache->ObjectType:(?<ObjectType>[^\r\n]*)
\*Cache->Path:(?<Path>[^\r\n]*)
\*\/");
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
                if (model.ExpireTime.HasValue)
                {
                    if (model.ExpireTime.Value < DateTime.Now)
                        this.Remove(key);
                    else
                    {
                        if (model.IsSliding && model.ExpiresIn.HasValue && model.ExpiresIn > 0)
                        {
                            model.ExpireTime = DateTime.Now.AddMilliseconds(model.ExpiresIn.Value);
                            FileHelper.WriteText(path, model.ToString());
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
            return null;
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
            FileHelper.DeleteFile(this.GetKey(key));
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key.IsNullOrEmpty() || value == null || expiresIn.TotalMilliseconds <= 1000) return false;
            //var val = @"/**Cache->CreateTime:{0}*Cache->ExpireTime:{1}*Cache->ObjectType:{2}*Cache->Path:{3}*/{4}".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.AddMilliseconds(expiresIn.TotalMilliseconds).ToString("yyyy-MM-dd HH:mm:ss.fff"), value.GetType().AssemblyQualifiedName, "", this.GetValue(value));
                var val = new CacheModel
                {
                    ExpireTime = DateTime.Now.AddMilliseconds(expiresIn.TotalMilliseconds),
                    Value = value,
                    ExpiresIn = expiresIn.TotalMilliseconds,
                    IsSliding = isSliding
                };
            return FileHelper.WriteText(this.GetKey(key), val.ToString());
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, string path)
        {
            if (key.IsNullOrEmpty() || value.IsNullOrEmpty() || path.IsNullOrEmpty()) return false;
            //var val = @"/**Cache->CreateTime:{0}*Cache->ExpireTime:{1}*Cache->ObjectType:{2}*Cache->Path:{3}*/{4}".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", value.GetType().AssemblyQualifiedName, path.GetBasePath(), this.GetValue(value));
                var val = new CacheModel
                {
                    Value = value,
                    Path = path
                };
            return FileHelper.WriteText(this.GetKey(key), val.ToString());
        }
        ///<inheritdoc/>
        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key.IsNullOrEmpty() || value == null) return false;
            return this.Set(key, value, (int)Math.Max(expiresSliding.TotalSeconds, expiressAbsoulte.TotalSeconds));
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
            if (key.IsNullOrEmpty() || value.IsNullOrEmpty()) return false;
            //var val = @"/**Cache->CreateTime:{0}*Cache->ExpireTime:{1}*Cache->ObjectType:{2}*Cache->Path:{3}*/{4}".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", value.GetType().AssemblyQualifiedName, "", this.GetValue(value));
            var val = new CacheModel
            {
                Value = value
            };
            return FileHelper.WriteText(this.GetKey(key), val.ToString());
        }
        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        private string GetKey(string key) => (FileHelper.AltDirectorySeparatorChar + this.Config.CachePath.Trim(new char[] { '/', '\\' }) + FileHelper.AltDirectorySeparatorChar + this.Config.CacheKey + "_" + key + ".cache").GetBasePath();
        #endregion
    }
}