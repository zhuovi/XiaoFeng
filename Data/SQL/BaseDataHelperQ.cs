using System.Collections.Generic;
using XiaoFeng.Cache;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-18 11:05:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    /// <summary>
    /// 数据拼接基类
    /// </summary>
    public abstract class BaseDataHelperQ : Disposable
    {
        #region 执行回调
        /// <summary>
        /// 执行回调
        /// </summary>
        public event RunSQLEventHandler SQLCallBack;
        #endregion

        #region 缓存Key
        /// <summary>
        /// 缓存key
        /// </summary>
        protected internal string CacheKey { get; set; }
        #endregion

        #region 数据
        /// <summary>
        /// 数据
        /// </summary>
        public virtual DataSqlQ DataSql { get; set; } = new DataSqlQ();
        #endregion

        #region 抽象方法
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存时长 单位为秒</param>
        /// <returns></returns>
        public abstract IQueryableQ SetCache(int TimeOut);
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public abstract IQueryableQ NoCache();
        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="query">IQueryableX</param>
        /// <returns></returns>
        public abstract IQueryableQ If<T>(IQueryableX<T> query);
        /// <summary>
        /// 符合条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        public abstract IQueryableQ Then<T>(params IQueryableX<T>[] querys);
        /// <summary>
        /// 不符合条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        public abstract IQueryableQ Else<T>(params IQueryableX<T>[] querys);
        /// <summary>
        /// 其它条件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="query">IQueryableX</param>
        /// <returns></returns>
        public abstract IQueryableQ ElseIf<T>(IQueryableX<T> query);
        /// <summary>
        /// 其它条件执行
        /// </summary>
        /// <typeparam name="T">IQueryableX</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        public abstract IQueryableQ ElseIfThen<T>(params IQueryableX<T>[] querys);
        #endregion

        #region 获取SQL
        /// <summary>
        /// 获取SQL
        /// </summary>
        public virtual string SQL
        {
            get
            {
                string _SQL = string.Empty;
                if (this.DataSql.If.IsNullOrEmpty())
                    return _SQL;
                else
                {
                    if (this.DataSql.Then.Count == 0) return _SQL;
                    _SQL += @"
                    if exists({0})
                    begin
                    {1}
                    end".format(this.DataSql.If, this.DataSql.Then.Join(";"));
                    if (this.DataSql.ElseIf.Count > 0)
                    {
                        for (int i = 0; i < this.DataSql.ElseIf.Count; i++)
                        {
                            _SQL += @"
                            else if(exists({0}))
                            begin
                            {1}
                            end".format(this.DataSql.ElseIf[0], this.DataSql.ElseIfThen[0].Join(";"));
                        }
                    }
                    if (this.DataSql.Else.Count > 0)
                    {
                        _SQL += @"
                        else
                        begin
                        {0}
                        end".format(this.DataSql.Else.Join(";"));
                    }
                    return _SQL;
                }
            }
        }
        #endregion

        #region 执行
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public virtual bool End()
        {
            string SQL = this.SQL;
            if (SQL.IsNullOrEmpty()) return false;
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            using (var db = new DataHelper(this.DataSql.Config))
            {
                int M = db.ExecuteNonQuery(SQL);
                s.Stop();
                this.DataSql.RunSQLTime = s.ElapsedMilliseconds;
                this.SQLCallBack?.Invoke(this.DataSql);
                this.DataSql = new DataSqlQ();
                return M > 0;
            }
        }
        #endregion

        #region 获取对象
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public virtual T ToEntity<T>()
        {
            string SQL = this.SQL;
            if (SQL.IsNullOrEmpty()) return default(T);
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            T model = default(T);
            if (this.DataSql.Config.CacheType != CacheType.No)
            {
                this.DataSql.CacheState = CacheState.Yes;
                this.DataSql.CacheTimeOut = this.DataSql.Config.CacheTimeOut;
            }
            if (this.DataSql.CacheState == CacheState.Yes)
            {
                var val = this.GetCacheData();
                if (val == null)
                {
                    using (var db = new DataHelper(this.DataSql.Config))
                        model = db.Query<T>(SQL);
                    this.SetCacheData(model, this.DataSql.CacheTimeOut ?? 0);
                }
                else
                    model = (T)val;
            }
            else
                using (var db = new DataHelper(this.DataSql.Config))
                    model = db.Query<T>(SQL);
            s.Stop();
            this.DataSql.RunSQLTime = s.ElapsedMilliseconds;
            this.DataSql.ModelType = typeof(T);
            this.SQLCallBack?.Invoke(this.DataSql);
            this.DataSql = new DataSqlQ();
            return model;
        }
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public virtual List<T> ToList<T>()
        {
            string SQL = this.SQL;
            if (SQL.IsNullOrEmpty()) return new List<T>();
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            if (this.DataSql.Config.CacheType != CacheType.No)
            {
                this.DataSql.CacheState = CacheState.Yes;
                this.DataSql.CacheTimeOut = this.DataSql.Config.CacheTimeOut;
            }
            using (var data = new DataHelper(this.DataSql.Config))
            {
                var list = new List<T>();
                if (this.DataSql.CacheState == CacheState.Yes)
                {
                    var val = this.GetCacheData();
                    if (val == null)
                    {
                        list = data.QueryList<T>(SQL);
                        this.SetCacheData(list, this.DataSql.CacheTimeOut ?? 0);
                    }
                    else
                        list = val as List<T>;
                }
                else
                    list = data.QueryList<T>(SQL);
                s.Stop();
                this.DataSql.RunSQLTime = s.ElapsedMilliseconds;
                this.DataSql.ModelType = typeof(List<T>);
                this.SQLCallBack?.Invoke(this.DataSql);
                this.DataSql = new DataSqlQ();
                return list;
            }
        }
        #endregion

        #region 缓存操作
        /// <summary>
        /// 创建CacheKey
        /// </summary>
        /// <returns></returns>
        public virtual string CreateCacheKey()
        {
            return this.CacheKey = ("SpliceSQL-" + this.SQL).MD5();
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        public virtual object GetCacheData()
        {
            if (this.CacheKey.IsNullOrEmpty()) this.CreateCacheKey();
            object data = CacheFactory.Create(this.DataSql.Config.CacheType).Get(this.CacheKey);
            /*
            if (this.DataSql.Config.CacheType == CacheType.Memory)
                data = Cache.CacheHelper.Get(this.CacheKey);
            else if(this.DataSql.Config.CacheType == CacheType.Disk)
            {
                var setting = Config.Setting.Current;
                //string CachePath = setting.GetCachePath;
                //CachePath = (setting.GetCachePath.Replace("\\", "/").TrimStart('/') + @"/" + this.CacheKey + ".temp").GetBasePath();
                if (FileHelper.Exists(CachePath,FileAttribute.File))
                {
                    string content = "";
                    using (StreamReader sr = File.OpenText(CachePath))
                    {
                        content = sr.ReadToEnd();
                    }
                    data = "Cache:" + content.RemovePattern(@"/\*CacheTime:(.*?)\*");
                    DateTime CacheTime = content.GetMatch(@"/\*CacheTime:(.*?)\*").ToCast<DateTime>();
                    if (CacheTime <= DateTime.Now)
                        FileHelper.Delete(CachePath, FileAttribute.File);
                }
            }*/
            this.DataSql.IsHitCache = data != null;
            if (this.DataSql.IsHitCache)
            {
                object count = Cache.CacheHelper.Get("HitCount-" + this.CacheKey);
                this.DataSql.HitCacheCount = count == null ? 1 : (count.ToCast<long>() + 1);
                if (this.DataSql.HitCacheCount > 0)
                    Cache.CacheHelper.Set("HitCount-" + this.CacheKey, this.DataSql.HitCacheCount);
            }
            return data;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="data">缓存值</param>
        /// <param name="timeOut">过期时间 单位为秒 0为永久</param>
        public virtual void SetCacheData(object data, int timeOut)
        {
            if (this.CacheKey.IsNullOrEmpty()) this.CreateCacheKey();
            var cache = CacheFactory.Create(this.DataSql.Config.CacheType);
            if (timeOut < 0)
            {
                cache.Remove(this.CacheKey);
                CacheHelper.Remove("HitCount-" + this.CacheKey);
            }
            else if (timeOut == 0)
            {
                cache.Set(this.CacheKey, data);
                CacheHelper.Set("HitCount-" + this.CacheKey, 0);
            }
            else
            {
                cache.Set(this.CacheKey, data, timeOut * 1000);
                CacheHelper.Set("HitCount-" + this.CacheKey, 0, timeOut * 1000);
            }
            /*DateTime dateTime;
            if (timeOut < 0)
                dateTime = DateTime.Now.AddMinutes(5);
            else if (timeOut == 0)
                dateTime = DateTime.Now.AddYears(1);
            else
                dateTime = DateTime.Now.AddSeconds(timeOut);
            
            if (this.DataSql.Config.CacheType == CacheType.Memory)
                Cache.CacheHelper.Set(this.CacheKey, data, dateTime);
            else if(this.DataSql.Config.CacheType == CacheType.Disk)
            {
                string CachePath = XiaoFeng.Config.Setting.Current.GetCachePath;
                CachePath = (CachePath + @"/" + this.CacheKey + ".temp").GetBasePath();
                //FileHelper.Delete(CachePath);
                string content = "/*CacheTime:{0}{1}{2}".format(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), Environment.NewLine, data.ToJson());
                FileHelper.WriteText(CachePath, content, Encoding.UTF8);
                /*byte[] bs = content.GetBytes(Encoding.UTF8);
                using (FileStream f = File.OpenWrite(CachePath))
                {
                    f.Write(bs, 0, bs.Length);
                }*
            }
            Cache.CacheHelper.Set("HitCount-" + this.CacheKey, 0, dateTime);*/
        }
        #endregion
    }
}