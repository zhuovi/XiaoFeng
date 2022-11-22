using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Cache;

namespace XiaoFeng.Config
{
    /// <summary>
    /// 上传配置
    /// </summary>
    [ConfigFile("Config/Upload.json", 0, "FAYELF-CONFIG-UPLOAD", ConfigFormat.Json)]
    public class Upload : ConfigSet<Upload>, IUpload
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Upload() : base() { }
        /// <summary>
        /// 设置配置文件路径
        /// </summary>
        /// <param name="dataPath">配置文件路径</param>
        public Upload(string dataPath) : base(dataPath) { }
        #endregion

        #region 属性
        /// <summary>
        /// 是否开启上传
        /// </summary>
        [Description("是否开启上传")]
        public bool IsOpen { get; set; } = false;
        /// <summary>
        /// 是否检测文件木马
        /// </summary>
        [Description("是否检测文件木马")]
        public bool IsCheckTrojan { get; set; } = true;
        /// <summary>
        /// 木马特征
        /// </summary>
        [Description("木马特征")]
        public string TrojanFeature { get; set; } = @"request[\[\(\s]|eval|alert|document\.|response\.|system|excute|redirect|delete |create |exec |select |update |\.asp|\.php|\.aspx|\.ashx|\.jsp|\.asmx|\.js| table | file|for\(|foreach|drop |alter |dbo\.|sys\.|<script|\.getfolder|\.createfolder|\.deletefolder|\.createdirectory|\.deletedirectory|\.saveas|wscript\.shell|script\.encode|server\.|\.createobject|execute|activexobject|language=|echo|<\?php|iframe";
        /// <summary>
        /// 最大允许上传大小 单位为B 默认为10M
        /// </summary>
        [Description("最大允许上传大小 单位为B 默认为10M")]
        public long MaxLength { get; set; } = 10 * 1024 * 1024;
        /// <summary>
        /// 生成文件名格式
        /// </summary>
        [Description("生成文件名格式")]
        public string FileNameFormat { get; set; } = "ZW_{yyyy}{MM}{dd}{HH}{mm}{ss}{fff}_{rnd:4}.{ext}";
        /// <summary>
        /// 上传目录
        /// </summary>
        [Description("上传目录")]
        public string UploadPath { get; set; } = "UploadFiles";
        /// <summary>
        /// 默认上传类型配置
        /// </summary>
        [Description("默认上传类型配置")]
        public UploadData Default { get; set; } = new UploadData { Path = "Other", Ext = "html,htm,txt,xml,jpg,jpeg,gif,bmp,png,rar,zip,7z,doc,xls,ppt,pdf,wmv,wma,flv,mp3,mp4,wpeg,swf" };
        /// <summary>
        /// 上传类型配置
        /// </summary>
        [Description("上传类型配置")]
        public Dictionary<string, UploadData> Data { get; set; } = new Dictionary<string, UploadData>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 是否启用图片压缩
        /// </summary>
        [Description("是否启用图片压缩")]
        public Boolean ImageCompressEnable { get; set; } = true;
        /// <summary>
        /// 图片压缩最长边限制
        /// </summary>
        [Description("图片压缩最长边限制")] 
        public int ImageCompressLength { get; set; } = 1000;
        /// <summary>
        /// 图片压缩质量
        /// </summary>
        [Description("图片压缩质量")] 
        public int ImageCompressQuality { get; set; } = 100;
        /// <summary>
        /// 上传路径是否带域名
        /// </summary>
        [Description("上传路径是否带域名")]
        public Boolean IsDomain { get; set; } = false;
        /// <summary>
        /// 域名
        /// </summary>
        [Description("域名")]
        public string Domain { get; set; } = "";
        #endregion

        #region 方法

        #region 获取带域名路径
        /// <summary>
        /// 获取带域名路径
        /// </summary>
        /// <param name="basePath">路径</param>
        /// <returns></returns>
        public string GetDomainPath(string basePath)
        {
            if (basePath.IsNullOrEmpty()) return basePath;
            if (this.Domain.IsNotNullOrEmpty() && !basePath.StartsWith("http") && !basePath.IsMatch(@"^" + this.Domain.ToRegexEscape()))
            {
                basePath = this.Domain.RemovePattern(@"[\\/]+$") + "/" + basePath.RemovePattern(@"^[\\/]+");
            }
            return basePath;
        }
        #endregion

        #region 获取不带域名路径
        /// <summary>
        /// 获取不带域名路径
        /// </summary>
        /// <param name="domainPath">带域名路径</param>
        /// <returns></returns>
        public string GetNotDomainPath(string domainPath)
        {
            if (domainPath.IsNullOrEmpty()) return domainPath;
            if (this.Domain.IsNotNullOrEmpty() && domainPath.IsMatch(@"^" + this.Domain.ToRegexEscape())) domainPath = domainPath.RemovePattern(@"^" + this.Domain.ToRegexEscape());
            if (!domainPath.IsMatch(@"/")) domainPath = "/" + domainPath;
            return domainPath;
        }
        #endregion

        #endregion
    }

    #region 上传类型配置
    /// <summary>
    /// 上传类型配置
    /// </summary>
    public class UploadData
    {
        /// <summary>
        /// 存放文件夹名
        /// </summary>
        [Description("存放文件夹名")]
        public string Path { get; set; } = "Other";
        /// <summary>
        /// 后缀名 多个用逗号隔开
        /// </summary>
        [Description("后缀名 多个用逗号隔开")]
        public string Ext { get; set; } = "jpg,jpeg,gif,bmp,png,txt";
    }
    #endregion
}