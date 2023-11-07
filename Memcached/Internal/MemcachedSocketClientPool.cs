using System;
using System.Net;
using XiaoFeng.Collections;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-19 14:33:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// Memcached 线程池
    /// </summary>
    public class MemcachedSocketClientPool : ObjectPool<IMemcachedSocketClient>
    {
        #region 构造器
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="memcachedConfig">配置</param>
        /// <param name="endPoint">网络终结点</param>
        public MemcachedSocketClientPool(MemcachedConfig memcachedConfig, IPEndPoint endPoint)
        {
            this.MemcachedConfig = memcachedConfig;
            this.EndPoint = endPoint;
            this.Max = this.MemcachedConfig.PoolSize;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 连接配置
        /// </summary>
        private MemcachedConfig MemcachedConfig { get; set; }
        /// <summary>
        /// 网络端结点
        /// </summary>
        private IPEndPoint EndPoint { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        protected override IMemcachedSocketClient OnCreate()
        {
            return new MemcachedSocketClient(this.MemcachedConfig, this.EndPoint);
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="item">项</param>
        public override void Close(IMemcachedSocketClient item)
        {
            item.Close();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="value">项</param>
        public override void OnDispose(IMemcachedSocketClient value)
        {
            this.Close(value);
            base.OnDispose(value);
            GC.Collect();
        }
        #endregion
    }
}