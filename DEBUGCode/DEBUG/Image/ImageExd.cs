using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;
using Cheng.Consoles;
using System.Drawing.Imaging;

namespace Cheng.DEBUG
{

    public static unsafe class ImageExd
    {

        #region 图像加载

        /// <summary>
        /// 创建指定图像文件的位图
        /// </summary>
        /// <param name="imageFile">图像文件路径</param>
        /// <returns></returns>
        public static Bitmap CreateImage(this string imageFile)
        {

            Bitmap image = null;

            try
            {

                using (FileStream file = new FileStream(imageFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    image = new Bitmap(file);
                }

            }
            catch (Exception)
            {
                image?.Dispose();
                throw;
            }

            return image;

        }

        /// <summary>
        /// 创建指定图像文件的位图
        /// </summary>
        /// <param name="imageFile">图像文件路径</param>
        /// <param name="width">图像长度</param>
        /// <param name="height">图像高度</param>
        /// <returns></returns>
        public static Bitmap CreateImage(this string imageFile, int width, int height)
        {
            Bitmap image = null;
            Bitmap tm = null;
            try
            {
                image = CreateImage(imageFile);

                if(width != image.Width || height != image.Height)
                {
                    tm = new Bitmap(image, width, height);
                    image.Dispose();
                    image = tm;
                    tm = null;
                }

            }
            catch (Exception)
            {
                image?.Dispose();
                tm?.Dispose();
                throw;
            }

            return image;
        }

        #endregion

        #region 打印控制台

        /// <summary>
        /// 将图像打印到控制台，需开启虚拟终端
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void printImage(this Bitmap image, int width, int height)
        {

            if(width != image.Width || height != image.Height)
            {
                using (Bitmap pri = new Bitmap(image, width, height))
                {
                    printImage(pri);
                }
            }
            else
            {
                printImage(image);
            }

        }

        /// <summary>
        /// 将图像打印到控制台，需开启虚拟终端
        /// </summary>
        /// <param name="image"></param>
        public static void printImage(this Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            int x, y;

            StringBuilder sb = new StringBuilder(128);

            byte r, g, b;

            for (y = 0; y < height; y++)
            {

                for (x = 0; x < width; x++)
                {
                    var color = image.GetPixel(x, y);
                    r = color.R;
                    g = color.G;
                    b = color.B;

                    sb.AppendANSIColorText(r, g, b, true);
                    sb.AppendANSIColorText(r, g, b, false);

                    sb.Append('■');
                }

                sb.AppendANSIResetColorText();
                sb.AppendLine();
            }

            Console.Write(sb.ToString());
        }

        #endregion

        #region 图像修改

        /// <summary>
        /// 两张图片上下拼接为新的图像
        /// </summary>
        /// <param name="imageUp">上图</param>
        /// <param name="imageDown">下图</param>
        /// <param name="width">指定图像宽度，若小于两图宽度则会截断</param>
        /// <returns>新的拼接图像</returns>
        public static Bitmap StitchingUpDown(this Bitmap imageUp, Bitmap imageDown, int width) 
        {
            if(imageUp is null || imageDown is null)
            {
                throw new ArgumentNullException();
            }
            if (width <= 0) throw new ArgumentOutOfRangeException("width", "指定图像宽度没有大于0");

            Bitmap re = null;

            int outWidth, outHeight;

            try
            {

                int upWidth = imageUp.Width, upHeight = imageUp.Height;
                int downWidth = imageDown.Width, downHeight = imageDown.Height;

                outHeight = upHeight + downHeight;
                outWidth = width;
                re = new Bitmap(outWidth, outHeight);

                int x, y;
                int reX, reY;
                Color t_c;

                //上图赋值
                reX = 0;
                reY = 0;

                for (y = 0; y < upHeight; y++, reY++)
                {
                    for (x = 0, reX = 0; x < upWidth; x++, reX++)
                    {
                        if (reX >= 0 && reY >= 0 && reX < outWidth && reY < outHeight)
                        {
                            t_c = imageUp.GetPixel(x, y);
                            re.SetPixel(reX, reY, t_c);
                        }
                    }
                }

                for (y = 0; y < downHeight; y++, reY++)
                {
                    for (x = 0, reX = 0; x < downWidth; x++, reX++)
                    {
                        if (reX >= 0 && reY >= 0 && reX < outWidth && reY < outHeight)
                        {
                            t_c = imageDown.GetPixel(x, y);
                            re.SetPixel(reX, reY, t_c);
                        }
                    }
                }


            }
            catch (Exception)
            {
                re?.Dispose();
                throw;
            }

            return re;

        }

        /// <summary>
        /// 两张图左右拼接为新图像
        /// </summary>
        /// <param name="imageLeft">左图</param>
        /// <param name="imageRight">右图</param>
        /// <param name="height">指定新的图像高，若小于两图高度则截断</param>
        /// <returns>新的拼接图像</returns>
        public static Bitmap StitchingLeftRight(this Bitmap imageLeft, Bitmap imageRight, int height) 
        {

            if (imageLeft is null || imageRight is null)
            {
                throw new ArgumentNullException();
            }
            if (height <= 0) throw new ArgumentOutOfRangeException("width", "指定图像宽度没有大于0");

            Bitmap re = null;

            int outWidth, outHeight;

            try
            {
                int leftWidth = imageLeft.Width, leftHeight = imageLeft.Height;
                int rWidth = imageRight.Width, rHeight = imageRight.Height;

                outHeight = height;
                outWidth = leftWidth + rWidth;
                re = new Bitmap(outWidth, outHeight);

                int x, y;
                int reX, reY;
                Color t_c;

                reX = 0;
                reY = 0;

                for(x = 0; x < leftWidth; x++, reX++)
                {
                    for(y = 0, reY = 0; y < leftHeight; y++, reY++)
                    {
                        if (reX >= 0 && reY >= 0 && reX < outWidth && reY < outHeight)
                        {
                            t_c = imageLeft.GetPixel(x, y);
                            re.SetPixel(reX, reY, t_c);
                        }
                    }
                }


                for (x = 0; x < rWidth; x++, reX++)
                {
                    for (y = 0, reY = 0; y < rHeight; y++, reY++)
                    {
                        if (reX >= 0 && reY >= 0 && reX < outWidth && reY < outHeight)
                        {
                            t_c = imageRight.GetPixel(x, y);
                            re.SetPixel(reX, reY, t_c);
                        }
                    }
                }

            }
            catch (Exception)
            {
                re?.Dispose();
                throw;
            }

            return re;

        }

        /// <summary>
        /// 两张图片上下拼接为新的图像
        /// </summary>
        /// <param name="imageUpPath">上图路径</param>
        /// <param name="imageDownPath">下图路径</param>
        /// <param name="newImagePath">要写入到的图片文件路径</param>
        /// <param name="format">保存时的图像格式</param>
        public static void StitchingUpDown(this string imageUpPath, string imageDownPath, string newImagePath, ImageFormat format)
        {
            using (Bitmap imageUp = new Bitmap(imageUpPath))
            {
                using (Bitmap imageDown = new Bitmap(imageDownPath))
                {

                    using (var re = StitchingUpDown(imageUp, imageDown, Math.Max(imageUp.Width, imageDown.Width)))
                    {
                        re.Save(newImagePath, format);
                    }

                }
            }

        }

        /// <summary>
        /// 两张图左右拼接为新图像
        /// </summary>
        /// <param name="imageLeftPath">左图路径</param>
        /// <param name="imageRightPath">右图路径</param>
        /// <param name="newImagePath">要写入到的图片文件路径</param>
        /// <param name="format">保存时的图像格式</param>
        public static void StitchingLeftRight(this string imageLeftPath, string imageRightPath, string newImagePath, ImageFormat format)
        {
            using (Bitmap imageL = new Bitmap(imageLeftPath))
            {
                using (Bitmap imageR = new Bitmap(imageRightPath))
                {

                    using (var re = StitchingLeftRight(imageL, imageR, Math.Max(imageL.Height, imageR.Height)))
                    {
                        re.Save(newImagePath, format);
                    }

                }
            }
        }

        #endregion

    }
}
