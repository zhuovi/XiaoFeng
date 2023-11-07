using System;
using System.IO;
using System.Reflection;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-14 11:21:51                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Resource
{
    /// <summary>
    /// 资源文件接口
    /// </summary>
    public interface IResourceFileInfo
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists { get; }
        /// <summary>
        /// 文件大小
        /// </summary>
        long Length { get; }
        /// <summary>
        /// 物理路径
        /// </summary>
        string PhysicalPath { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTimeOffset LastModified { get; }
        /// <summary>
        /// 是否是目录
        /// </summary>
        bool IsDirectory { get; }
        /// <summary>
        /// key
        /// </summary>
		string Key { get; }
        /// <summary>
        /// 程序集
        /// </summary>
		Assembly Assembly { get; }
        /// <summary>
        /// 资源路径
        /// </summary>
        string ResourcePath { get; }
        /// <summary>
        /// 读取文件字节
        /// </summary>
        /// <returns></returns>
        byte[] CreateReadBytes();
        /// <summary>
        /// 创建文件流
        /// </summary>
        /// <returns></returns>
        Stream CreateReadStream();
    }
}