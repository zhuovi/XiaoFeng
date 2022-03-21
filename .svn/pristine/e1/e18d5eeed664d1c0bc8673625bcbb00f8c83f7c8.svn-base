using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Config
{
    /// <summary>
    /// 上传配置接口
    /// </summary>
    public interface IUpload
    {
        #region 属性
        /// <summary>
        /// 是否开启上传
        /// </summary>
        bool IsOpen { get; set; }
        /// <summary>
        /// 是否检测文件木马
        /// </summary>
        bool IsCheckTrojan { get; set; }
        /// <summary>
        /// 木马特征
        /// </summary>
        string TrojanFeature { get; set; }
        /// <summary>
        /// 最大允许上传大小 单位为B 默认为10M
        /// </summary>
       long MaxLength { get; set; }
        /// <summary>
        /// 生成文件名格式
        /// </summary>
        string FileNameFormat { get; set; }
        /// <summary>
        /// 上传目录
        /// </summary>
        string UploadPath { get; set; }
        /// <summary>
        /// 默认上传类型配置
        /// </summary>
        UploadData Default { get; set; }
        /// <summary>
        /// 上传类型配置
        /// </summary>
        Dictionary<string, UploadData> Data { get; set; }
        /// <summary>
        /// 上传路径是否带域名
        /// </summary>
        Boolean IsDomain { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        string Domain { get; set; }
        #endregion

        #region 方法

        #region 获取带域名路径
        /// <summary>
        /// 获取带域名路径
        /// </summary>
        /// <param name="basePath">路径</param>
        /// <returns></returns>
        string GetDomainPath(string basePath);
        #endregion

        #region 获取不带域名路径
        /// <summary>
        /// 获取不带域名路径
        /// </summary>
        /// <param name="domainPath">带域名路径</param>
        /// <returns></returns>
        string GetNotDomainPath(string domainPath);
        #endregion

        #endregion
    }
}