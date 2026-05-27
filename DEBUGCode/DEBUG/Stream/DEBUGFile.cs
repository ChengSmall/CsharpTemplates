using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security;

using Cheng.Streams;

namespace Cheng.DEBUG
{

    public static class DEBUGFileExd
    {

        /// <summary>
        /// 创建指定路径下的文件并返回文件流
        /// </summary>
        /// <param name="filePath">要创建的文件路径</param>
        /// <returns></returns>
        /// <exception cref="Exception">各种异常</exception>
        public static FileStream CreateFile(this string filePath)
        {
            return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        }

        /// <summary>
        /// 创建指定路径后创建文件并返回文件流
        /// </summary>
        /// <param name="filePath">要创建的文件路径</param>
        /// <returns></returns>
        /// <exception cref="Exception">各种异常</exception>
        public static FileStream CreateDireAndFile(this string filePath)
        {
            var dire = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(dire);

            return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        }

        /// <summary>
        /// 打开一个只读文件流，使用只读共享权限
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public static FileStream OpenFile(this string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }


        /// <summary>
        /// 打开指定路径的文件读取所有数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内包含的所有数据</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException">非文件设备或无读取对应权限</exception>
        /// <exception cref="FileNotFoundException">找不到该文件</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效，例如位于未映射的驱动器上</exception>
        /// <exception cref="PathTooLongException">文件路径超过了系统定义的最大长度</exception>
        /// <exception cref="UnauthorizedAccessException">操作系统不允许所请求的 access</exception>
        /// <exception cref="InternalBufferOverflowException">文件大小超出byte[]对象的常规范围</exception>
        public static byte[] FileReadAll(this string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                if (file.Length > int.MaxValue)
                {
                    throw new InternalBufferOverflowException();
                }
                return file.ReadAll();
            }
        }

        /// <summary>
        /// 打开指定路径的文件读取所有文本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">指定读取文件文本的字符编解码器</param>
        /// <returns>读取后的数据</returns>
        /// <returns>文件内包含的所有数据</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException">非文件设备或无读取对应权限</exception>
        /// <exception cref="FileNotFoundException">找不到该文件</exception>
        /// <exception cref="SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="DirectoryNotFoundException">指定的路径无效，例如位于未映射的驱动器上</exception>
        /// <exception cref="PathTooLongException">文件路径超过了系统定义的最大长度</exception>
        /// <exception cref="UnauthorizedAccessException">操作系统不允许所请求的 access</exception>
        /// <exception cref="OutOfMemoryException">内存不够</exception>
        /// <exception cref="Exception">其他错误</exception>
        public static string FileReadAllText(this string filePath, Encoding encoding)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                using (StreamReader sr = new StreamReader(file, encoding, false, 1024 * 2, true))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }

}
