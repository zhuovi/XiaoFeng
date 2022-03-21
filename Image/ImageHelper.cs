using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Image
{
    /// <summary>
    /// 图片常规操作类
    /// Version : v 1.0.0
    /// Create Date : 2016-12-02
    /// Author : jacky
    /// QQ : 7092734
    /// Email : jacky@zhuovi.com
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 图片拼接布局枚举
        /// </summary>
        public enum ImageMergeOrientation
        {
            /// <summary>
            /// 水平
            /// </summary>
            Horizontal,
            /// <summary>
            /// 纵向
            /// </summary>
            Vertical
        }
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="files">文件流</param>
        /// <param name="mergeType">合并方向</param>
        /// <returns></returns>
        public static System.Drawing.Bitmap CombineImages(List<FileInfo> files, ImageMergeOrientation mergeType = ImageMergeOrientation.Vertical)
        {
           var imgs = files.Select(f => System.Drawing.Image.FromFile(f.FullName));

            var finalWidth = mergeType == ImageMergeOrientation.Horizontal ?
                imgs.Sum(img => img.Width) :
                imgs.Max(img => img.Width);

            var finalHeight = mergeType == ImageMergeOrientation.Vertical ?
                imgs.Sum(img => img.Height) :
                imgs.Max(img => img.Height);

            var finalImg = new Bitmap(finalWidth, finalHeight + 300);
            Graphics g = Graphics.FromImage(finalImg);
            g.Clear(SystemColors.AppWorkspace);

            var width = finalWidth;
            var height = finalHeight;
            var nIndex = 0;
            foreach (FileInfo file in files)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(file.FullName);
                if (nIndex == 0)
                {
                    g.DrawImage(img, new Point(0, 0));
                    nIndex++;
                    width = img.Width;
                    height = img.Height;
                }
                else
                {
                    switch (mergeType)
                    {
                        case ImageMergeOrientation.Horizontal:
                            g.DrawImage(img, new Point(width, 0));
                            width += img.Width;
                            break;
                        case ImageMergeOrientation.Vertical:
                            g.DrawImage(img, new Point(0, height));
                            height += img.Height;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("MergeType");
                    }
                }
                img.Dispose();
            }
            g.Dispose();
            return finalImg;
        }
    }
}