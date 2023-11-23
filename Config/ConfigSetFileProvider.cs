using System;
using System.Collections.Concurrent;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-12-07 09:09:54                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 配置文件目录监控
    /// </summary>
    public static class ConfigSetFileProvider
    {
        #region 属性
        /// <summary>
        /// 配置文件目录
        /// </summary>
        public static ConcurrentDictionary<string, Item> ConfigFiles { get; set; }
        /// <summary>
        /// 目录监控器
        /// </summary>
        public static FileProvider FileProvider { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 添加监控
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="path">路径</param>
        /// <param name="callback">回调</param>
        public static void Add(string key, string path, Action<WatcherChangeType, string, string> callback = null)
        {
#if NETSTANDARD2_0
            if (ConfigFiles == null) ConfigFiles = 
#else
            ConfigFiles ??=
#endif
            new ConcurrentDictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
            var item = new Item
            {
                Key = key,
                Path = path,
                CallBack = callback
            };
            if (ConfigFiles.ContainsKey(path))
                ConfigFiles[path] = item;
            else
                ConfigFiles.TryAdd(path, item);
            XiaoFeng.Threading.Synchronized.Run(() =>
            {
#if NETSTANDARD2_0
                if (FileProvider == null) FileProvider = 
#else
                FileProvider ??=
#endif
                new FileProvider(key);
                if (!FileProvider.IsRun) FileProvider.Watch(FileHelper.Combine(FileHelper.GetCurrentDirectory(), "Config"), "", (t, p, k) =>
                {
                    if (ConfigFiles.TryGetValue(p, out var _item))
                    {
                        if (t == WatcherChangeType.Deleted)
                            ConfigFiles.TryRemove(p, out var _);
                        _item.CallBack?.Invoke(t, p, _item.Key);
                    }
                }).ConfigureAwait(false);
            });
        }
#endregion

        #region 子类
        /// <summary>
        /// 子项
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Key
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// 路径
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// 回调
            /// </summary>
            public Action<WatcherChangeType, string, string> CallBack { get; set; }
        }
        #endregion
    }
}