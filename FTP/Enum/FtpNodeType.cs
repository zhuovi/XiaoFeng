/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-09-18 00:51:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum FtpNodeType
    {
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// 目录
        /// </summary>
        Directory,
        /// <summary>
        /// 快捷方式
        /// </summary>
        SymbolicLink
    }
}