using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Cheng.Memorys;
using Cheng.Algorithm;
using Cheng.DataStructure;

using Cheng.Algorithm.Randoms;
using System.Security;

namespace Cheng.Algorithm.Randoms.Extends
{


    static unsafe partial class RandomExtends
    {

        #region 随机器测试封装

        /// <summary>
        /// 随机器生成指定长度的字节数组
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="length">生成的字节长度</param>
        /// <returns>生成的随机值字节数组</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字节长度小于0</exception>
        public static byte[] NextByteArray(this IRandomGenerateBytes random, int length)
        {
            if (random is null) throw new ArgumentNullException();
            if (length < 0) throw new ArgumentOutOfRangeException();
            if (length == 0) return Array.Empty<byte>();
            byte[] buf = new byte[length];
            fixed (byte* bptr = buf)
            {
                random.NextPtr(bptr, length);
            }
            return buf;
        }

        #endregion

        #region IO

        /// <summary>
        /// 将随机字节序列写入指定对象
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="stream">要写入的流对象</param>
        /// <param name="length">要写入的字节长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字节长度小于0</exception>
        /// <exception cref="NotSupportedException">流对象无法写入数据</exception>
        /// <exception cref="Exception">其它流对象错误</exception>
        public static void NextToStream(this IRandomGenerateBytes random, Stream stream, long length)
        {
            if (random is null || stream is null) throw new ArgumentNullException();
            if (!stream.CanWrite) throw new NotSupportedException();
            if (length < 0) throw new ArgumentOutOfRangeException();

            if (length == 0) return;
            const int len = 1024 * 4;
            byte[] buffer = new byte[len];
            fixed (byte* bptr = buffer)
            {
                long wrc = 0;
                while (wrc < length)
                {
                    int wrlen = (int)Math.Min(length, len);
                    random.NextPtr(bptr, wrlen);
                    stream.Write(buffer, 0, wrlen);
                    wrc += wrlen;
                }
            }

        }

        /// <summary>
        /// 将随机二进制字节序列写入指定文件
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="filePath">要写入的文件路径</param>
        /// <param name="length">要写入的字节长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字节长度小于0</exception>
        /// <exception cref="DirectoryNotFoundException">路径无效</exception>
        /// <exception cref="PathTooLongException">路径错误</exception>
        /// <exception cref="SecurityException">没有权限</exception>
        /// <exception cref="Exception">其它文件错误</exception>
        public static void NextToFile(this IRandomGenerateBytes random, string filePath, long length)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 1024 * 4, FileOptions.SequentialScan))
            {
                NextToStream(random, file, length);
            }
        }

        #endregion

    }

}
