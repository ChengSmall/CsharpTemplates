using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.DEBUG
{

    public static class DEBUGNet
    {

        #region DownLoadString

        /// <summary>
        /// 从指定uri下载字符串数据，使用utf-8编码解码字符串
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uri">uri</param>
        /// <returns>获取的字符串</returns>
        public static string DownloadStringByUTF8(this WebClient web, string uri)
        {
            StringBuilder sb = new StringBuilder(64);
            DownloadString(web, new Uri(uri), Encoding.UTF8, sb, new char[1024 * 8]);
            return sb.ToString();
        }

        /// <summary>
        /// 从指定uri下载字符串数据
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uri">uri</param>
        /// <param name="encoding">使用字符编码</param>
        /// <returns>获取的字符串</returns>
        public static string DownloadString(this WebClient web, string uri, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder(64);
            DownloadString(web, new Uri(uri), encoding, sb, new char[1024 * 8]);
            return sb.ToString();
        }

        /// <summary>
        /// 从指定uri下载字符串数据
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uri">uri</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>获取的字符串</returns>
        public static string DownloadString(this WebClient web, Uri uri, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder(64);
            DownloadString(web, uri, encoding, sb, new char[1024 * 8]);
            return sb.ToString();
        }

        /// <summary>
        /// 从指定uri下载字符串数据
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uri">uri</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="getStr">从此实例向后添加获取的数据</param>
        public static void DownloadString(this WebClient web, Uri uri, Encoding encoding, StringBuilder getStr)
        {
            DownloadString(web, uri, encoding, getStr, new char[1024 * 8]);
        }

        /// <summary>
        /// 从指定uri下载字符串数据
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uri">uri</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="getStr">从此实例向后添加获取的数据</param>
        /// <param name="buffer">拷贝数据的缓冲区</param>
        public static void DownloadString(this WebClient web, string uri, Encoding encoding, StringBuilder getStr, char[] buffer)
        {
            DownloadString(web, new Uri(uri), encoding, getStr, buffer);
        }

        /// <summary>
        /// 从指定uri下载字符串数据
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uri">uri</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="getStr">从此实例向后添加获取的数据</param>
        /// <param name="buffer">拷贝数据的缓冲区</param>
        public static void DownloadString(this WebClient web, Uri uri, Encoding encoding, StringBuilder getStr, char[] buffer)
        {

            if (web is null || uri is null || encoding is null || getStr is null || buffer is null) throw new ArgumentNullException();

            if (buffer.Length == 0) throw new ArgumentException();

            TextReader reader;

            using (var open = web.OpenRead(uri))
            {
                reader = new StreamReader(open, encoding, true, 1024 * 8, true);

                int length = buffer.Length;

                Loop:
                var ri = reader.Read(buffer, 0, length);
                if (ri == 0) goto End;

                getStr.Append(buffer, 0, ri);
                goto Loop;
                End:

                reader.Close();
            }


        }

        #endregion

    }

}
