﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
    /// IO操作类
    /// Version : 1.0.3
    /// 2020-11-25
    /// 增加文件编码判断
    /// V 1.0.2
    /// 2020-12-02
    /// 增加目录是否存在
    /// V 1.0.3
    /// 2022-02-14
    /// i 
    /// </summary>
    public static class FileHelper
    {
        #region 路径是文件还是目录
        /// <summary>
        /// 路径是文件还是目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static FileAttribute GetFileAttributes(string path)
        {
            if (path.IsNullOrEmpty()) return FileAttribute.UnKnown;
            var _path = GetFullPathFunction(path);
            var dir = new DirectoryInfo(_path);
            if (dir.Attributes.HasFlag(FileAttributes.Directory)) return FileAttribute.Directory;
            else if (dir.Attributes.HasFlag(FileAttribute.Device)) return FileAttribute.Device;
            return FileAttribute.File;
        }
        #endregion

        #region 文件或目录是否存在
        /// <summary>
        /// 文件或目录是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="attribute">文件类型</param>
        /// <returns></returns>
        public static Boolean Exists(string path, FileAttribute attribute = FileAttribute.UnKnown)
        {
            if (path.IsNullOrEmpty()) return false;
            var _path = GetFullPathFunction(path);
            if (attribute == FileAttribute.UnKnown)
            {
                var dir = new DirectoryInfo(_path);
                return dir.Exists || File.Exists(_path);
            }
            return attribute == FileAttribute.File ? File.Exists(_path) : Directory.Exists(_path);
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
            var _path = GetFullPathFunction(path);
            if (attribute == FileAttribute.UnKnown)
            {
                var _dir = _path.ToDirectoryInfo();
                if (_dir.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (!_dir.Exists) _dir.Create();
                    return;
                }
                var _fileInfo = _path.ToFileInfo();
                _dir = _fileInfo.Directory;
                if (!_dir.Exists) _dir.Create();
                if (!_fileInfo.Exists)
                {
                    var fs = _fileInfo.Create();
                    fs.Close();
                    fs.Dispose();
                }
                return;
            }
            if (attribute == FileAttribute.Directory)
            {
                var _dir = _path.ToDirectoryInfo();
                if (!_dir.Exists) _dir.Create();
                return;
            }
            var fileInfo = _path.ToFileInfo();
            var dir = fileInfo.Directory;
            if (!dir.Exists) dir.Create();
            if (!fileInfo.Exists)
            {
                var fs = fileInfo.Create();
                fs.Close();
                fs.Dispose();
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
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static DirectoryInfo CreateDirectory(string path)
        {
            if (path.IsNullOrEmpty()) return null;
            var d = new DirectoryInfo(path.GetBasePath());
            if (d.Exists) return d;
            d.Create();
            return d;
        }
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
            var _path = GetFullPathFunction(path);

            if (attribute == FileAttribute.UnKnown)
            {
                var dir = _path.ToDirectoryInfo();
                if (dir.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (dir.Exists) dir.Delete(true);
                    return;
                }
                var fileInfo = _path.ToFileInfo();
                if (fileInfo.Exists) fileInfo.Delete();
                return;
            }
            if (attribute == FileAttribute.File)
            {
                var fileInfo = _path.ToFileInfo();
                if (fileInfo.Exists) fileInfo.Delete();
                return;
            }
            if (attribute == FileAttribute.Directory)
            {
                var _dir = _path.ToDirectoryInfo();
                if (_dir.Exists) _dir.Delete(true);
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
            var bytes = OpenBytes(path);
            if (bytes.IsNullOrEmpty() || bytes.Length == 0) return string.Empty;
            return bytes.GetString(encoding);
        }
        /// <summary>
        /// 读取文件字节
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="offset">开始读取位置</param>
        /// <param name="length">读取长度</param>
        /// <returns></returns>
        public static byte[] OpenBytes(string path, int offset = 0, long length = 0)
        {
            if (path.IsNullOrEmpty()) return null;
            var _path = GetFullPathFunction(path);
            if (!File.Exists(_path)) return null;
            byte[] bytes = Array.Empty<byte>();
            if (offset < 0) offset = 0;
            if (length < 0) length = 0;
            using (var fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (fs.Length == 0) return Array.Empty<byte>();
                if (length > 0)
                    length = Math.Min(length, fs.Length - offset);
                else
                    length = fs.Length - offset;
                bytes = new byte[length];
                fs.Read(bytes, offset, (int)length);
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

        #region 读取文件头类型
        /// <summary>
        /// 读取文件头类型
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] OpenReadMime(string path, int length = 4)
        {
            if (path.IsNullOrEmpty()) return null;
            var _path = GetFullPathFunction(path);
            if (!File.Exists(_path)) return null;
            byte[] bytes = Array.Empty<byte>();
            if (length < 0) length = 4;
            /*
 * MIME(Multipurpose Internet Mail Extensions)多用途互联网邮件扩展类型。
 * 一般来说，某种类型的文件的起始几个字节都是固定的。
 * 根据这几个起始字节的内容就可以确定文件类型，因此这几个字节的内容被称为魔数 (magic number)。
 * 常用文件的文件头如下：(以前六位为准)
JPEG (jpg)，文件头：FFD8FF 
PNG (png)，文件头：89504E47 
GIF (gif)，文件头：47494638 
TIFF (tif)，文件头：49492A00 
Windows Bitmap (bmp)，文件头：424D 
CAD (dwg)，文件头：41433130 
Adobe Photoshop (psd)，文件头：38425053 
Rich Text Format (rtf)，文件头：7B5C727466 
XML (xml)，文件头：3C3F786D6C 
HTML (html)，文件头：68746D6C3E 
Email [thorough only] (eml)，文件头：44656C69766572792D646174653A 
Outlook Express (dbx)，文件头：CFAD12FEC5FD746F 
Outlook (pst)，文件头：2142444E 
MS Word/Excel (xls.or.doc)，文件头：D0CF11E0 
MS Access (mdb)，文件头：5374616E64617264204A 
WordPerfect (wpd)，文件头：FF575043 
Postscript (eps.or.ps)，文件头：252150532D41646F6265 
Adobe Acrobat (pdf)，文件头：255044462D312E 
Quicken (qdf)，文件头：AC9EBD8F 
Windows Password (pwl)，文件头：E3828596 
ZIP Archive (zip)，文件头：504B0304 
RAR Archive (rar)，文件头：52617221 
Wave (wav)，文件头：57415645 
AVI (avi)，文件头：41564920 
Real Audio (ram)，文件头：2E7261FD 
Real Media (rm)，文件头：2E524D46 
MPEG (mpg)，文件头：000001BA 
MPEG (mpg)，文件头：000001B3 
Quicktime (mov)，文件头：6D6F6F76 
Windows Media (asf)，文件头：3026B2758E66CF11 
MIDI (mid)，文件头：4D546864 
*/
            using (var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                BinaryReader reader = new BinaryReader(fs, Encoding.Default);
                bytes = reader.ReadBytes(length);
                reader.Close();
                reader.Dispose();
                fs.Close();
                fs.Dispose();
            }
            return bytes;
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
            var _path = GetFullPathFunction(path);
            Create(_path.GetDirectoryName(), FileAttribute.Directory);
            try
            {
                using (var fs = new FileStream(_path, offset == 0 ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read, 4096))
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
        public static Boolean AppendText(string path, string content, Encoding encoding = null)
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
        public static Boolean DeleteFile(params string[] path)
        {
            if (path.Length == 0) return false;
            try
            {
                path.Each(p => Delete(p, FileAttribute.File));
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
            var _path = GetFullPathFunction(path);
            var _root = GetFullPathFunction(root);
            if (!_path.StartsWith(_root))
            {
                DeleteDirectory(_path);
                return;
            }
            var dir = _path.ToDirectoryInfo();
            do
            {
                if (dir.Exists) dir.Delete(true);
                dir = dir.Parent;
            } while (dir.Parent != null && dir.FullName != _root);
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path">目录路径</param>
        public static void DeleteDirectory(params string[] path)
        {
            if (path.Length == 0) return;
            try
            {
                path.Each(p => Delete(p, FileAttribute.Directory));
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
            var _source = GetFullPathFunction(source);
            if (!File.Exists(_source)) return false;
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
            var _source = GetFullPathFunction(source);
            if (!File.Exists(_source)) return false;
            var _dest = GetFullPathFunction(dest);
            Create(Path.GetDirectoryName(_dest), FileAttribute.Directory);
            try
            {
                if (File.Exists(_dest)) File.Delete(_dest);
                File.Move(_source, _dest);
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
            var _source = GetFullPathFunction(source);
            if (!Directory.Exists(_source)) return;
            if (dest.IsNullOrEmpty()) return;
            var _dest = GetFullPathFunction(dest);
            if (!Directory.Exists(_dest)) Directory.CreateDirectory(Path.GetDirectoryName(_dest));
            MoveDirectory(new DirectoryInfo(_source), new DirectoryInfo(_dest));
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
            var _source = GetFullPathFunction(source);
            if (!File.Exists(_source)) return false;
            var _dest = GetFullPathFunction(dest);
            if (!Directory.Exists(Path.GetDirectoryName(_dest))) Directory.CreateDirectory(Path.GetDirectoryName(_dest));
            try
            {
                if (File.Exists(_dest)) File.Delete(_dest);
                File.Copy(_source, _dest);
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
            var _source = GetFullPathFunction(source);
            if (!Directory.Exists(_source)) return;
            if (dest.IsNullOrEmpty()) return;
            var _dest = GetFullPathFunction(dest);
            if (!Directory.Exists(_dest)) return;
            CopyDirectory(new DirectoryInfo(_source), new DirectoryInfo(_dest));
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
                DeleteFile(path);
                f.CopyTo(path);
            });
            source.GetDirectories().Each(d =>
            {
                var dir = dest.FullName + FileHelper.DirectorySeparatorChar + d.Name;
                Create(dir, FileAttribute.Directory);
                d.CopyTo(dir);
            });
        }
        /// <summary>
        /// 复制目录及目录下所有文件
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="dest">目的目录</param>
        public static void CopyDirectoryLinux(DirectoryInfo source, DirectoryInfo dest)
        {
            if (!dest.Exists) dest.Create();
            source.GetFiles().Each(f =>
            {
                var path = dest.FullName + FileHelper.DirectorySeparatorChar + f.Name;
                DeleteFile("{*}/" + path);
                f.CopyTo(path);
            });
            source.GetDirectories().Each(d =>
            {
                var dir = dest.FullName + FileHelper.DirectorySeparatorChar + d.Name;
                Create("{*}/" + dir, FileAttribute.Directory);
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
        public static long GetFolderSize(string path) => GetFullPathFunction(path).ToDirectoryInfo().GetFiles("*.*", SearchOption.AllDirectories).Select(a => a.Length).Sum();
        #endregion

        #region 是否是包含根目录
        /// <summary>
        /// 是否是包含根目录
        /// </summary>
        /// <param name="path">目录</param>
        /// <returns></returns>
        public static Boolean IsPathRoot(string path)
        {
            if (path.IsNullOrEmpty()) return false;
            return OS.Platform.GetOSPlatform() == PlatformOS.Windows ? path.IsMatch(@"^[a-z]+:(\\|\/)") : path.StartsWith(DirectorySeparatorChar.ToString());
        }
        #endregion

        #region 获取文件全路径
        /// <summary>
        /// 获取文件全路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetFullPath(string path) => path.GetBasePath();
        /// <summary>
        /// 文件全路径函数
        /// </summary>
        private static Func<string, string> _GetFullPathFunction = a => GetBasePath(a);
        /// <summary>
        /// 获取文件全路径函数
        /// </summary>
        public static Func<string, string> GetFullPathFunction
        {
            get => _GetFullPathFunction;
            set => _GetFullPathFunction = value;
        }
        /// <summary>
        /// 获取文件全路径 如果是用绝对路径则前边可以加{*}或/则表示是根目录路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetBasePath(string path = "")
        {
            if (path.IsNullOrEmpty()) return GetCurrentDirectory();
            if (path.IsBasePath())
            {
                var _path = Path.GetFullPath(path.StartsWith("{*}") ? ("/" + path.RemovePattern(@"\{\*\}").TrimStart(new char[] { '/', '\\' })) : path);
                return OS.Platform.GetOSPlatform() == PlatformOS.Windows ? _path : _path.ReplacePattern(@"(\\|\/)+", DirectorySeparatorChar.ToString());
            }
            /*
             * 磁盘根目录路径
             */
            if (path.StartsWith("{*}") || path.StartsWith("/"))
            {
                path = "/" + path.RemovePattern(@"\{\*\}").TrimStart(new char[] { '/', '\\' });

                if (path == "/") return Path.GetPathRoot(GetCurrentDirectory());
                var _Path = Path.GetFullPath(path);
                return OS.Platform.GetOSPlatform() == PlatformOS.Windows ? _Path : _Path.ReplacePattern(@"(\\|\/)+", DirectorySeparatorChar.ToString());
            }
            /*
             * 家目录路径
             */
            if (path.IsMatch(@"^(\{(Root|RootPath|\/)\})?[\\/]*") || path.StartsWith("{Root}", StringComparison.OrdinalIgnoreCase) || path.StartsWith("{/}") || path.StartsWith("{RootPath}", StringComparison.OrdinalIgnoreCase) || path.StartsWith("\\"))
            {
                path = path.RemovePattern(@"^(\{(Root|RootPath|\/)\})?[\\/]*");
                if (path.IsNullOrEmpty() || path.Trim(new char[] { '/', '\\' }).IsNullOrEmpty()) return GetCurrentDirectory();
            }
            var _ = Path.GetFullPath(path);
            return OS.Platform.GetOSPlatform() == PlatformOS.Windows ? _ : _.ReplacePattern(@"(\\|\/)+", DirectorySeparatorChar.ToString());
        }
        #endregion

        #region 字节转相应单位
        /// <summary>
        /// 字节转相应单位
        /// </summary>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public static string ConvertByte(double size) => size.ByteToKMGTP(4);
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
        public static string CurrentDirectory => GetCurrentDirectory();
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
        /// 用于在环境变量中分隔路径字符串的平台特定的分隔符 如 windows是';' linux是':'
        /// </summary>
        public static char PathSeparator => Path.PathSeparator;
        /// <summary>
        /// 提供平台特定的字符，该字符用于在反映分层文件系统组织的路径字符串中分隔目录级别。如:windows是'\' linux是'/'
        /// </summary>
        public static char DirectorySeparatorChar => Path.DirectorySeparatorChar;
        /// <summary>
        /// 提供平台特定的替换字符，该替换字符用于在反映分层文件系统组织的路径字符串中分隔目录级别。如 windows是'/' linux是'/'
        /// </summary>
        public static char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
        /// <summary>
        /// 提供平台特定的卷分隔符。如 windows是':' linux是'/'
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

        #region 设置当前目录
        /// <summary>
        /// 设置当前目录
        /// </summary>
        /// <param name="path">目录</param>
        public static void SetCurrentDirectory(string path) => OS.Platform.CurrentDirectory = path;
        #endregion

        #region 获取当前目录
        /// <summary>
        /// 获取当前目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory() => OS.Platform.CurrentDirectory;
        #endregion

        #region 合并目录
        /// <summary>
        /// 将字符串数组组合成一个路径。
        /// </summary>
        /// <param name="path">由路径的各部分构成的数组。</param>
        /// <returns>已组合的路径。</returns>
        public static string Combine(params string[] path) => path.Any() ? Path.Combine(path) : string.Empty;
        /// <summary>
        /// 将字符串数组组合成一个路径。
        /// </summary>
        /// <param name="paths">由路径的各部分构成的数组。</param>
        /// <returns>已组合的路径。</returns>
        public static string Combine(IEnumerable<string> paths) => paths == null || !paths.Any() ? string.Empty : Combine(paths.ToArray());
        /// <summary>
        /// 将字符串数组组合成一个相对路径。
        /// </summary>
        /// <param name="paths">由路径的各部分构成的数组。</param>
        /// <returns>已组合的路径。</returns>
        public static string CombineRelativePath(IEnumerable<string> paths) => paths == null || !paths.Any() ? string.Empty : CombineRelativePath(paths.ToArray());
        /// <summary>
        /// 将字符串数组组合成一个相对路径。
        /// </summary>
        /// <param name="paths">由路径的各部分构成的数组。</param>
        /// <returns>已组合的路径。</returns>
        public static string CombineRelativePath(params string[] paths)
        {
            if (paths == null || !paths.Any()) return string.Empty;
            var list = new List<string>();
            if (paths.Length > 0)
                paths.Each(s =>
                {
                    if (s.IsNotNullOrEmpty())
                        list.AddRange(s.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries));
                });
            if (list.Count == 0) return string.Empty;
            return string.Join(AltDirectorySeparatorChar.ToString(), list);
        }
        #endregion

        #region 创建 zip 存档，该存档包含指定目录的文件和目录
        /// <summary>
        /// 创建 zip 存档，该存档包含指定目录的文件和目录。
        /// </summary>
        /// <param name="sourceDirectoryName">到要存档的目录的路径，指定为相对路径或绝对路径。 相对路径被解释为相对于当前工作目录。</param>
        /// <param name="destination">要存储 zip 存档的流。</param>
        /// <param name="compressionLevel">指示创建项时是否强调速度或压缩有效性的枚举值之一。</param>
        /// <param name="includeBaseDirectory">包括从在存档的根的 sourceDirectoryName 的目录名称，则为 true；仅包含目录中的内容，则为 false 。</param>
        /// <param name="entryNameEncoding">在存档中读取或写入项名时使用的编码。 仅当需要针对具有不支持条目名称的 UTF-8 编码的 zip 归档工具和库的互操作性进行编码时，为此参数指定值。</param>
        public static Boolean CreateZipArchiveFromDirectory(string sourceDirectoryName, Stream destination, CompressionLevel compressionLevel = CompressionLevel.Fastest, bool includeBaseDirectory = true, Encoding entryNameEncoding = null)
        {
            var dir = sourceDirectoryName.GetBasePath().ToDirectoryInfo();
            if (!dir.Exists) return false;
            using (var zip = new ZipArchive(destination, ZipArchiveMode.Create, false, entryNameEncoding == null ? Encoding.UTF8 : entryNameEncoding))
            {
                CreateZipArchiveFromDirectory(zip, dir, includeBaseDirectory ? (dir.Name + "/") : "", compressionLevel);
                zip.Dispose();
            }
            return true;
        }
        /// <summary>
        /// 打包文件夹下的文件到文档流
        /// </summary>
        /// <param name="zip">压缩包流</param>
        /// <param name="dir">目录</param>
        /// <param name="path">相对路径</param>
        /// <param name="compressionLevel">指示创建项时是否强调速度或压缩有效性的枚举值之一。</param>
        public static void CreateZipArchiveFromDirectory(ZipArchive zip, DirectoryInfo dir, string path, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            if (path.IsNotNullOrEmpty())
            {
                path = path.Trim(new char[] { '/', '\\' }).ReplacePattern(@"\\+", "/") + "/";
            }
            dir.GetDirectories().Each(d =>
            {
                CreateZipArchiveFromDirectory(zip, d, path + d.Name + "/", compressionLevel);
            });
            dir.GetFiles().Each(f =>
            {
                var entry = zip.CreateEntryFromFile(f.FullName, path + f.Name, compressionLevel);
                entry.TryDispose();
            });
        }
        /// <summary>
        /// 创建 zip 存档，该存档包含指定目录的文件和目录。
        /// </summary>
        /// <param name="sourceDirectoryName">到要存档的目录的路径，指定为相对路径或绝对路径。 相对路径被解释为相对于当前工作目录。</param>
        /// <param name="destinationArchiveFileName">要生成的存档路径，指定为相对路径或绝对路径。 相对路径被解释为相对于当前工作目录。</param>
        /// <param name="compressionLevel">指示创建项时是否强调速度或压缩有效性的枚举值之一。</param>
        /// <param name="includeBaseDirectory">包括从在存档的根的 sourceDirectoryName 的目录名称，则为 true；仅包含目录中的内容，则为 false 。</param>
        /// <param name="entryNameEncoding">在存档中读取或写入项名时使用的编码。 仅当需要针对具有不支持条目名称的 UTF-8 编码的 zip 归档工具和库的互操作性进行编码时，为此参数指定值。</param>
        public static void CreateZipArchiveFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel = CompressionLevel.Fastest, bool includeBaseDirectory = true, Encoding entryNameEncoding = null)
        {
            try
            {
                using (var fs = new FileStream(destinationArchiveFileName.GetBasePath(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 4096, true))
                {
                    CreateZipArchiveFromDirectory(sourceDirectoryName, fs, compressionLevel, includeBaseDirectory, entryNameEncoding);
                }
            }
            catch (IOException ex)
            {
                LogHelper.Error(ex);
            }
        }
        #endregion
    }
}