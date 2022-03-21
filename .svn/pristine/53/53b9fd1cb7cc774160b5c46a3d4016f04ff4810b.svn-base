using System;
using System.Collections.Generic;
using System.Text;

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
        public void DeleteDirectory() => FileHelper.DeleteDirectory(this.Path);
        #endregion
    }
}