/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-15 15:37:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 监控文件类型
    /// </summary>
    public enum WatcherChangeType
    {
        /// <summary>
        /// 出错
        /// </summary>
        Error = 0,
        /// <summary>
        /// 创建
        /// </summary>
        Created = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Deleted = 2,
        /// <summary>
        /// 改变
        /// </summary>
        Changed = 4,
        /// <summary>
        /// 重命名
        /// </summary>
        Renamed = 8,
        /// <summary>
        /// 所有
        /// </summary>
        All = 15
    }
}