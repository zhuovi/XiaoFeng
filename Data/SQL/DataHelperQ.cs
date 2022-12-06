using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.Json;
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
    /// 拼接SQL类
    /// </summary>
    public class DataHelperQ : BaseDataHelperQ, IQueryableQ
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataHelperQ() { }
        /// <summary>
        /// 设置数据库配置
        /// </summary>
        /// <param name="config">数据库配置</param>
        public DataHelperQ(ConnectionConfig config)
        {
            this.DataSql.Config = config;
        }
        /// <summary>
        /// 设置数据库配置及事件
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="e">事件</param>
        public DataHelperQ(ConnectionConfig config, RunSQLEventHandler e) : this(config)
        {
            if (e != null)
                this.SQLCallBack += e;
            if (Setting.Current.Debug)
                this.SQLCallBack += a =>
                {
                    LogHelper.SQL("DataSQL:\r\n" + a.ToJson(new JsonSerializerSetting() { Indented = true }));
                };
        }
        #endregion

        #region 实现方法
        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="query">IQueryableX</param>
        /// <returns></returns>
        public override IQueryableQ If<T>(IQueryableX<T> query)
        {
            this.DataSql.If = query.SQL().RemovePattern(@"[\s;]+$");
            return this;
        }
        /// <summary>
        /// 符合条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        public override IQueryableQ Then<T>(params IQueryableX<T>[] querys)
        {
            querys.Each(q =>
            {
                this.DataSql.Then.Add(q.SQL().RemovePattern(@"[\s;]+$"));
            });
            return this;
        }
        /// <summary>
        /// 不符合条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        public override IQueryableQ Else<T>(params IQueryableX<T>[] querys)
        {
            querys.Each(q =>
            {
                this.DataSql.Else.Add(q.SQL().RemovePattern(@"[\s;]+$"));
            });
            return this;
        }
        /// <summary>
        /// 其它条件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="query">IQueryableX</param>
        /// <returns></returns>
        public override IQueryableQ ElseIf<T>(IQueryableX<T> query)
        {
            this.DataSql.ElseIf.Add(query.SQL().RemovePattern(@"[\s;]+$"));
            return this;
        }
        /// <summary>
        /// 其它条件执行
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="querys">IQueryableX集合</param>
        /// <returns></returns>
        public override IQueryableQ ElseIfThen<T>(params IQueryableX<T>[] querys)
        {
            var list = new List<string>();
            querys.Each(q =>
            {
                list.Add(q.SQL().RemovePattern(@"[\s;]+$"));
            });
            this.DataSql.ElseIfThen.Add(list);
            return this;
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public override T ToEntity<T>()
        {
            return base.ToEntity<T>();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public override List<T> ToList<T>()
        {
            return base.ToList<T>();
        }
        /// <summary>
        /// 获取SQL
        /// </summary>
        public override string SQL => base.SQL;
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="TimeOut">缓存时长 单位为秒</param>
        /// <returns></returns>
        public override IQueryableQ SetCache(int TimeOut)
        {
            this.DataSql.CacheState = CacheState.Yes;
            this.DataSql.CacheTimeOut = TimeOut;
            return this;
        }
        /// <summary>
        /// 不缓存
        /// </summary>
        /// <returns></returns>
        public override IQueryableQ NoCache()
        {
            this.DataSql.CacheState = CacheState.No;
            return this;
        }
        #endregion
    }
}