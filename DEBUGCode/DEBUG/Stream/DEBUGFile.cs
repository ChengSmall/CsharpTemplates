using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

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

    }

}
