using System.Text;
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
namespace XiaoFeng.IO
{
    /// <summary>
    /// 自定义文件操作类
    /// </summary>
    public class FayFile
    {
        #region 构造器
        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path">路径</param>
        public FayFile(string path)
        {
            if (path.IsNotNullOrEmpty())
                this.Path = path.GetBasePath();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool Exists() => FileHelper.Exists(this.Path);
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public bool CreateFile(string content, Encoding encoding = null) => FileHelper.WriteText(this.Path, content, encoding);
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <param name="offset">位置</param>
        /// <returns></returns>
        public bool WriteBytes(byte[] bytes, int offset = 0) => FileHelper.WriteBytes(this.Path, bytes, offset);
        /// <summary>
        /// 附加内容到文件
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public bool AppendFile(string content, Encoding encoding = null) => FileHelper.AppendText(this.Path, content, encoding);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public bool DeleteFile() => FileHelper.DeleteFile(this.Path);
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <returns></returns>
        public void DeleteDirectory() => this.Path.ToDirectoryInfo().Delete(true);
        /// <summary>
        /// 获取文件字节
        /// </summary>
        /// <returns>文件字节</returns>
        public byte[] GetBytes() => FileHelper.OpenBytes(this.Path);
        /// <summary>
        /// 获取文件内容文本
        /// </summary>
        /// <returns>文件内容文本</returns>
        public string GetText() => FileHelper.OpenText(this.Path);
        #endregion
    }
}