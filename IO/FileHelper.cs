using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace XiaoFeng.IO
{
    /// <summary>
    /// IO操作类
    /// Version : 1.0.2
    /// 2020-11-25
    /// 增加文件编码判断
    /// V 1.0.2
    /// 2020-12-02
    /// 增加目录是否存在
    /// </summary>
    public static class FileHelper
    {
        #region 文件是否存在
        /// <summary>
        /// 文件或目录是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="attribute">文件类型</param>
        /// <returns></returns>
        public static Boolean Exists(string path, FileAttribute attribute = FileAttribute.UnKnown)
        {
            if (path.IsNullOrEmpty()) return false;
            path = GetBasePath(path);
            if (attribute == FileAttribute.UnKnown)
                attribute = path.HasExtension() ? FileAttribute.File : FileAttribute.Directory;
            return attribute == FileAttribute.File ? File.Exists(path) : Directory.Exists(path);
        }
        #endregion

        #region 创建目录或文件
        /// <summary>
        /// 创建目录或文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="attribute">文件类型</param>
        public static void Create(string path, FileAttribute attribute = FileAttribute.UnKnown)
        {
            path = GetBasePath(path);
            if (attribute == FileAttribute.UnKnown)
                attribute = path.HasExtension() ? FileAttribute.File : FileAttribute.Directory;
            if (attribute == FileAttribute.Directory)
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
            else if (attribute == FileAttribute.File)
            {
                Create(path.GetDirectoryName(), FileAttribute.Directory);
                if (!File.Exists(path)) using (var fs = File.Create(path)) { fs.Close();fs.Dispose(); }
            }
        }
        /// <summary>
        /// 写文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <param name="encoding">文件编码</param>
        /// <returns></returns>
        public static Boolean Create(string path, string content, Encoding encoding) => WriteText(path, content, encoding);
        #endregion

        #region 删除文件或目录
        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="attribute">属性</param>
        public static void Delete(string path, FileAttribute attribute = FileAttribute.UnKnown)
        {
            if (path.IsNullOrEmpty()) return;
            path = path.GetBasePath();
            var file = new FileInfo(path);
            if ((FileAttributes.ReadOnly | FileAttributes.System | FileAttributes.Offline  | FileAttributes.Device).HasFlag(file.Attributes)) return;
            if (attribute == FileAttribute.UnKnown)
            {
                if (file.Attributes == FileAttributes.Directory)
                    attribute = FileAttribute.Directory;
                else
                    attribute = FileAttribute.File;
            }
            if (attribute == FileAttribute.File)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            else if (attribute == FileAttribute.Directory)
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }
        #endregion

        #region 读取文件内容
        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文件编码</param>
        /// <returns></returns>
        public static string OpenText(string path, Encoding encoding = null)
        {
            if (path.IsNullOrEmpty()) return string.Empty;
            path = GetBasePath(path);
            if (!File.Exists(path)) return string.Empty;
            var bytes = OpenBytes(path);
            if (bytes.IsNullOrEmpty() || bytes.Length == 0) return string.Empty;
            return bytes.GetString(encoding);
        }
        /// <summary>
        /// 读取文件字节
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static byte[] OpenBytes(string path)
        {
            if (path.IsNullOrEmpty()) return null;
            path = GetBasePath(path);
            if (!File.Exists(path)) return null;
            byte[] bytes = Array.Empty<byte>();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length == 0) return Array.Empty<byte>();
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                fs.Dispose();
            }
            return bytes;
        }
        /// <summary>
        /// 读取文件字节
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static sbyte[] OpenSBytes(string path)
        {
            var bytes = OpenBytes(path);
            var bs = new sbyte[bytes.Length];
            bytes.Each((b, index) => bs[index] = (sbyte)b);
            return bs;
        }
        #endregion

        #region 写文件内容
        /// <summary>
        /// 写文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <param name="encoding">文件编码</param>
        /// <returns></returns>
        public static Boolean WriteText(string path, string content, Encoding encoding = null)
        {
            if (path.IsNullOrEmpty()) return false;
            return WriteBytes(path, content.GetBytes(encoding), 0);
        }
        /// <summary>
        /// 写文件字节
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="bytes">文件字节</param>
        /// <param name="offset">开始写位置 0重写 -1 附加 其它为从指定位置写</param>
        /// <returns></returns>
        public static Boolean WriteBytes(string path, byte[] bytes, int offset = 0)
        {
            if (path.IsNullOrEmpty()) return false;
            path = GetBasePath(path);
            Create(path.GetDirectoryName(), FileAttribute.Directory);
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (offset == 0)
                    {
                        fs.SetLength(0);
                        fs.Seek(0, SeekOrigin.Begin);
                    }
                    else if (offset == -1)
                        fs.Seek(0, SeekOrigin.End);
                    else
                    {
                        var length = fs.Length;
                        if (offset >= length)
                            fs.Seek(0, SeekOrigin.End);
                        else
                            fs.Seek(offset, SeekOrigin.Begin);
                    }
                    if (bytes != null && bytes.Length > 0)
                        fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
                return false;
            }
        }
        #endregion

        #region 附加文件内容
        /// <summary>
        /// 附加文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <param name="encoding">文件编码</param>
        /// <returns></returns>
        public static Boolean AppendText(string path, string content, Encoding encoding)
        {
            if (path.IsNullOrEmpty() || content.IsNullOrEmpty()) return false;
            return WriteBytes(path, content.GetBytes(encoding), -1);
        }
        /// <summary>
        /// 附加文件字节
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="bytes">字节</param>
        /// <returns></returns>
        public static Boolean AppendBytes(string path, byte[] bytes)
        {
            if (path.IsNullOrEmpty() || bytes == null || bytes.Length == 0) return false;
            return WriteBytes(path, bytes, -1);
        }
        #endregion

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static Boolean DeleteFile(string path)
        {
            try
            {
                Delete(path, FileAttribute.File);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
        #endregion

        #region 删除目录
        /// <summary>
        /// 删除目录如果空目录继续向上删除
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="root">根目录</param>
        public static void DeleteDirectoryEmpty(string path, string root)
        {
            if (path.IsNullOrEmpty()) return;
            path = path.GetBasePath();
            if (!Directory.Exists(path)) return;
            if (root.IsNullOrEmpty())
                Directory.Delete(path, true);
            else
            {
                root = root.GetBasePath();
                if (!Directory.Exists(root))
                    Directory.Delete(path, true);
                else
                    DeleteDirectoryEmpty(new DirectoryInfo(path), root);
            }
        }
        /// <summary>
        /// 删除目录如果空目录继续向上删除
        /// </summary>
        /// <param name="directory">目录</param>
        /// <param name="root">根目录</param>
        public static void DeleteDirectoryEmpty(DirectoryInfo directory, string root)
        {
            if (directory.FullName.EqualsIgnoreCase(root)) return;
            if (!directory.Exists) { DeleteDirectoryEmpty(directory.Parent, root); return; }
            if (directory.GetFiles().Length > 0 || directory.GetDirectories().Length > 0) return;
            directory.Delete(true);
            DeleteDirectoryEmpty(directory.Parent, root);
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path">目录路径</param>
        public static void DeleteDirectory(string path)
        {
            try
            {
                Delete(path, FileAttribute.Directory);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
        #endregion

        #region 重命名
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="source">文件路径</param>
        /// <param name="newName">新文件名</param>
        /// <returns></returns>
        public static Boolean Rename(string source, string newName)
        {
            if (source.IsNullOrEmpty()) return false;
            if (newName.IsNullOrEmpty()) return false;
            source = GetBasePath(source);
            if (!File.Exists(source)) return false;
            return MoveFile(source, source.ReplacePattern(DirectorySeparatorChar + @"[\s\S]+$", DirectorySeparatorChar + newName));
        }
        #endregion

        #region 移动文件
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="source">源文件</param>
        /// <param name="dest">目录文件</param>
        /// <returns></returns>
        public static Boolean MoveFile(string source, string dest)
        {
            if (source.IsNullOrEmpty() || dest.IsNullOrEmpty()) return false;
            source = GetBasePath(source);
            if (!File.Exists(source)) return false;
            dest = GetBasePath(dest);
            Create(Path.GetDirectoryName(dest), FileAttribute.Directory);
            try
            {
                if (File.Exists(dest)) File.Delete(dest);
                File.Move(source, dest);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
                return false;
            }
        }
        #endregion

        #region 移动目录及目录下所有文件
        /// <summary>
        /// 移动目录及目录下所有文件
        /// </summary>
        /// <param name="source">源路径</param>
        /// <param name="dest">目的路径</param>
        public static void MoveDirectory(string source, string dest)
        {
            if (source.IsNullOrEmpty()) return;
            source = GetBasePath(source);
            if (!Directory.Exists(source)) return;
            if (dest.IsNullOrEmpty()) return;
            dest = GetBasePath(dest);
            if (!Directory.Exists(dest)) Directory.CreateDirectory(Path.GetDirectoryName(dest));
            MoveDirectory(new DirectoryInfo(source), new DirectoryInfo(dest));
        }
        /// <summary>
        /// 移动目录及目录下所有文件
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="dest">目的目录</param>
        public static void MoveDirectory(DirectoryInfo source, DirectoryInfo dest)
        {
            source.MoveTo(dest.FullName);
            /*
            source.GetFiles().Each(f =>
            {
                f.MoveTo(dest.FullName + "\\" + f.Name);
            });
            source.GetDirectories().Each(d =>
            {
                MoveDirectory(d, new DirectoryInfo(dest.FullName + "\\" + d.Name));
                d.MoveTo(dest.FullName + "\\" + d.Name);
            });*/
        }
        #endregion

        #region 复制文件
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="dest">文件路径</param>
        /// <returns></returns>
        public static Boolean CopyFile(string source, string dest)
        {
            if (source.IsNullOrEmpty() || dest.IsNullOrEmpty()) return false;
            source = GetBasePath(source);
            if (!FileHelper.Exists(source)) return false;
            dest = GetBasePath(dest);
            if (!Directory.Exists(Path.GetDirectoryName(dest))) Directory.CreateDirectory(Path.GetDirectoryName(dest));
            try
            {
                if (File.Exists(dest)) File.Delete(dest);
                File.Copy(source, dest);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
                return false;
            }
        }
        #endregion

        #region 复制目录及目录下所有文件
        /// <summary>
        /// 复制目录及目录下所有文件
        /// </summary>
        /// <param name="source">源目录路径</param>
        /// <param name="dest">目的目录路径</param>
        public static void CopyDirectory(string source, string dest)
        {
            if (source.IsNullOrEmpty()) return;
            source = GetBasePath(source);
            if (!Directory.Exists(source)) return;
            if (dest.IsNullOrEmpty()) return;
            dest = GetBasePath(dest);
            if (!Directory.Exists(dest)) return;
            CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(dest));
        }
        /// <summary>
        /// 复制目录及目录下所有文件
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="dest">目的目录</param>
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo dest)
        {
            if (!dest.Exists) dest.Create();
            source.GetFiles().Each(f =>
            {
                var path = dest.FullName + FileHelper.DirectorySeparatorChar + f.Name;
                FileHelper.DeleteFile(path);
                f.CopyTo(path);
            });
            source.GetDirectories().Each(d =>
            {
                var dir = dest.FullName + FileHelper.DirectorySeparatorChar + d.Name;
                FileHelper.Create(dir, FileAttribute.Directory);
                d.CopyTo(dir);
            });
        }
        #endregion

        #region 计算文件夹大小
        /// <summary>
        /// 计算文件夹大小
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static long GetFolderSize(string path)
        {
            //double _ = 0;
            path = GetBasePath(path);
            var dir = new DirectoryInfo(path);
            return dir.GetFiles("*.*", SearchOption.AllDirectories).Select(a => a.Length).Sum();
            /*dir.GetFiles().Each(f =>
            {
                _ += f.Length;
            });
            dir.GetDirectories().Each(d =>
            {
                _ += GetFolderSize(d.FullName);
            });
            return _;*/
        }
        #endregion

        #region 获取文件路径
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetBasePath(string path)
        {
            if (path.IsNullOrEmpty()) return OS.Platform.CurrentDirectory;
            
            path = path.TrimEnd(new char[] { '\\', '/' });
            if (!path.IsBasePath())
            {
                DirectoryInfo directoryInfo = OS.Platform.CurrentDirectory.ToDirectoryInfo();
                path = path.RemovePattern(@"^\.\/");
                while (path.IsMatch(@"^\.\.\/"))
                {
                    path = path.RemovePattern(@"^\.\.\/");
                    directoryInfo = directoryInfo.Parent;
                }
                path = directoryInfo.FullName + DirectorySeparatorChar + path;
            }
            return path.ReplacePattern(@"[\/\\]+", DirectorySeparatorChar.ToString());
            /*
            var os = OS.Platform.GetOSPlatform();
            if (!path.IsBasePath())
            {
                if (os == PlatformOS.Linux || os == PlatformOS.OSX)
                    path = (OS.Platform.CurrentDirectory + "/" + path.ReplacePattern(@"[\\]+", "/")).Trim('/');
                else
                    path = (OS.Platform.CurrentDirectory + "\\" + path.ReplacePattern(@"[/]+", "\\")).Trim('\\');
            }
            else
            {
                if (os == PlatformOS.Linux || os == PlatformOS.OSX)
                    path = path.ReplacePattern(@"[\\]+", "/").TrimEnd('/');
                else
                    path = path.ReplacePattern(@"[/]+", "\\").TrimEnd('\\');
            }
            return path.ReplacePattern(@"[/]+", "/").ReplacePattern(@"[\\]+", "\\");
            */
        }
        #endregion

        #region 字节转相应单位
        /// <summary>
        /// 字节转相应单位
        /// </summary>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public static string ConvertByte(double size)
        {
            var Units = new string[] { "B", "K", "M", "G", "T", "P" };
            double mod = 1024.0000;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size, 4) + Units[i];
        }
        #endregion

        #region 获取根目录
        /// <summary>
        /// 获取根目录
        /// </summary>
        public static string GetRootStaticPath()
        {
            return "/";
            /*
#if NETFRAMEWORK
            return "/";
#else
            return "wwwroot/";
#endif*/
        }
        #endregion

        #region 获取项目目录
        /// <summary>
        /// 获取项目目录
        /// </summary>
        public static string CurrentDirectory => OS.Platform.CurrentDirectory;
        #endregion

        #region 文件编码
        /// <summary>
        /// 通过分析其字节顺序标记（BOM）确定文本文件的编码。
        ///当文本文件的字节序检测失败时，默认为ASCII。
        /// </summary>
        /// <param name="data">文件字节</param>
        /// <returns>文件编码</returns>
        public static Encoding GetEncoding(byte[] data)
        {
            if (data == null || data.Length < 3) return Encoding.ASCII;
            /*读取BOM*/
            var bom = data;
            //data.CopyTo(bom,3)
            /*UTF7*/
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            /*UTF8 带BOM的UTF8*/
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            /*UTF-16LE*/
            if (bom[0] == 0xff && bom[1] == 0xfe && (bom.Length < 4 || bom[2] != 0 || bom[3] != 0)) return Encoding.Unicode;
            /*UTF-16BE*/
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode;
            /*UTF-32*/
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            /*不带BOM的UTF8*/
            if (IsUTF8Bytes(bom)) return Encoding.UTF8;
            return Encoding.Default;
        }
        /// <summary> 
        /// 判断是否是不带BOM的UTF8 格式 
        /// </summary> 
        /// <param name="data"></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; /*计算当前正分析的字符应还有的字节数*/
            byte curByte; /*当前分析的字节.*/
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        /*判断当前*/
                        while (((curByte <<= 1) & 0x80) != 0) charByteCounter++;
                        /*标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X*/
                        if (charByteCounter == 1 || charByteCounter > 6) return false;
                    }
                }
                else
                {
                    /*若是UTF-8 此时第一位必须为1 */
                    if ((curByte & 0xC0) != 0x80) return false;
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
                throw new Exception("非预期的byte格式");
            return true;
        }
        #endregion

        #region 分隔符
        /// <summary>
        /// 用于在环境变量中分隔路径字符串的平台特定的分隔符 如;
        /// </summary>
        public static char PathSeparator => Path.PathSeparator;
        /// <summary>
        /// 提供平台特定的字符，该字符用于在反映分层文件系统组织的路径字符串中分隔目录级别。如:\
        /// </summary>
        public static char DirectorySeparatorChar => Path.DirectorySeparatorChar;
        /// <summary>
        /// 提供平台特定的替换字符，该替换字符用于在反映分层文件系统组织的路径字符串中分隔目录级别。如/
        /// </summary>
        public static char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
        /// <summary>
        /// 提供平台特定的卷分隔符。如:
        /// </summary>
        public static char VolumeSeparatorChar => Path.VolumeSeparatorChar;
        /// <summary>
        /// 获取包含不允许在文件名中使用的字符的数组
        /// </summary>
        /// <returns>包含不允许在文件名中使用的字符的数组</returns>
        public static char[] GetInvalidFileNameChars() => Path.GetInvalidFileNameChars();
        /// <summary>
        /// 获取包含不允许在路径名中使用的字符的数组
        /// </summary>
        /// <returns>包含不允许在路径名中使用的字符的数组</returns>
        public static char[] GetInvalidPathChars() => Path.GetInvalidPathChars();
        #endregion

        #region 新行
        /// <summary>
        /// 新行
        /// </summary>
        public static string NewLine => Environment.NewLine;
        #endregion

        #region 文件扩展名
        /// <summary>
        /// 返回指定的路径字符串的扩展名
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>扩展名</returns>
        public static string GetExtension(string path) => path.GetExtension();
        #endregion

        #region 文件名和扩展名
        /// <summary>
        /// 返回指定的路径字符串的文件名和扩展名
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文件名和扩展名</returns>
        public static string GetFileName(string path) => path.GetFileName();
        #endregion

        #region 目录信息
        /// <summary>
        /// 返回指定的路径字符串的目录信息
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>目录信息</returns>
        public static string GetDirectoryName(string path) => path.GetDirectoryName();
        #endregion
    }
}