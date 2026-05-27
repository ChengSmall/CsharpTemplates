using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Reflection;

using Cheng.Algorithm.Compressions;
using Cheng.Memorys;
using Cheng.Streams;
using Cheng.IO;

namespace Cheng.Algorithm.Compressions.ResourcePackages
{

    /// <summary>
    /// 资源包只读文件解析器
    /// </summary>
    /// <remarks>
    /// 对本地硬盘上的文件提供资源包解析器，使其每次打开某项数据时都会创建一个独立的文件截取流读取资源
    /// </remarks>
    public sealed class FileResourcePackageReader : ResourcePackageReader
    {

        #region

        #region 构造

        const int defBufferSize = 1024 * 4;

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="filePath">要访问的文件所在路径</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(string filePath) : this(new FileInfo(filePath), 1024 * 4, 1024 * 4, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="filePath">要访问的文件所在路径</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小，默认为4096</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(string filePath, int bufferSize) : this(new FileInfo(filePath), bufferSize, 1024 * 4, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="filePath">要访问的文件所在路径</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小，默认为4096</param>
        /// <param name="fileBufferSize">文件对象<see cref="FileStream"/>内的缓冲区大小</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(string filePath, int bufferSize, int fileBufferSize) : this(new FileInfo(filePath), bufferSize, fileBufferSize, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小，默认为4096</param>
        /// <param name="fileBufferSize">文件对象<see cref="FileStream"/>内的缓冲区大小</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize, int fileBufferSize) : this(fileInfo, bufferSize, fileBufferSize, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小，默认为4096</param>
        /// <param name="fileBufferSize">文件对象<see cref="FileStream"/>内的缓冲区大小</param>
        /// <param name="createStreamBufferSize">在打开数据创建新的流时的数据缓冲区大小，如果该参数为0，则使用无内置缓冲区流，值默认为0</param>
        /// <param name="checkHeader">读取数据索引前是否验证文件头；默认为true</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        /// <exception cref="InvalidDataException">文件不属于资源包格式；如果参数<paramref name="checkHeader"/>是true，会先验证数据头，无法通过验证会抛出此错误</exception>
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize, int fileBufferSize, int createStreamBufferSize, bool checkHeader) : base()
        {
            if (fileInfo is null) throw new ArgumentNullException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            FileStream file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, fileBufferSize);
            p_stream = file;
            try
            {
                p_onDispose = true;
                if (checkHeader)
                {
                    if (!ResourcePackageReader.EqualDataHeader(file))
                    {
                        throw new InvalidDataException(Cheng.Properties.Resources.StreamParserDef_NotConver);
                    }
                }

                CreateStreamBufferSize = createStreamBufferSize;
                f_init(file, bufferSize);
                p_fileInfo = fileInfo;
                f_fileInit();
            }
            catch (Exception)
            {
                p_stream?.Close();
                throw;
            }
            
            
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小，默认为4096</param>
        /// <param name="fileBufferSize">文件对象<see cref="FileStream"/>内的缓冲区大小</param>
        /// <param name="createStreamBufferSize">在打开数据创建新的流时的数据缓冲区大小，如果该参数为0，则使用无内置缓冲区流，值默认为0</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        /// <exception cref="InvalidDataException">文件不属于资源包格式；如果参数<paramref name="checkHeader"/>是true，会先验证数据头，无法通过验证会抛出此错误</exception>
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize, int fileBufferSize, int createStreamBufferSize) : this(fileInfo, bufferSize, fileBufferSize, createStreamBufferSize, true)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize) : this(fileInfo, bufferSize, 1024 * 4, defBufferSize, true)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo) : this(fileInfo, 1024 * 8, 1024 * 4, 1024 * 8)
        {
        }

        /// <summary>
        /// 初始化文件参数
        /// </summary>
        private void f_fileInit()
        {
            p_fullFilePath = p_fileInfo.FullName;
        }

        #endregion

        #region 回收

        #region 封装

        #endregion

        /// <summary>
        /// 需要释放文件资源
        /// </summary>
        public override bool IsNeedToReleaseResources => true;

        #endregion

        #region 参数

        private FileInfo p_fileInfo;

        private string p_fullFilePath;

        #endregion

        #region 功能

        /// <summary>
        /// 当前数据包文件所在的绝对路径
        /// </summary>
        public string FullPath
        {
            get => p_fullFilePath;
        }

        protected override Stream CreateGetBaseStream(long position, long length)
        {
            if (length == 0) return Stream.Null;

            FileStream file = new FileStream(p_fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            try
            {
                if (CreateStreamBufferSize == 0)
                {
                    return new NotBufferTruncateStream(file, position, length, true);
                }

                return new TruncateStream(file, position, length, true, CreateStreamBufferSize);
            }
            catch (Exception)
            {
                file?.Close();
                throw;
            }
           
        }

        public override bool CanCreateGetBaseStreamIsIndependent => true;

        #endregion

        #endregion

    }

}
