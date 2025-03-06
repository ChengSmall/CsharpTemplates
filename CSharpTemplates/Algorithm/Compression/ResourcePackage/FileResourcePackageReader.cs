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
    /// 只读资源文件包读取解析器
    /// </summary>
    /// <remarks>
    /// 对本地硬盘上的文件特供的资源包解析器
    /// </remarks>
    public class FileResourcePackageReader : ResourcePackageReader
    {

        #region

        #region 构造

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
        public FileResourcePackageReader(string filePath) : this(new FileInfo(filePath), 1024 * 4, 1024 * 4, 0)
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
        public FileResourcePackageReader(string filePath, int bufferSize) : this(new FileInfo(filePath), bufferSize, 1024 * 4, 0)
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
        public FileResourcePackageReader(string filePath, int bufferSize, int fileBufferSize) : this(new FileInfo(filePath), bufferSize, fileBufferSize, 0)
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
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize, int fileBufferSize) : this(fileInfo, bufferSize, fileBufferSize, 0)
        {
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
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize, int fileBufferSize, int createStreamBufferSize) : base()
        {
            if (fileInfo is null) throw new ArgumentNullException();
            if (bufferSize <= 0 || createStreamBufferSize < 0) throw new ArgumentOutOfRangeException();

            FileStream file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, fileBufferSize);
            p_stream = file;
            p_createStreamBufferSize = createStreamBufferSize;
            f_init(file, bufferSize);
            p_fileInfo = fileInfo;
            p_disposeBasefile = true;
            f_fileInit();
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
        public FileResourcePackageReader(FileInfo fileInfo, int bufferSize) : this(fileInfo, bufferSize, 1024 * 4, 0)
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
        public FileResourcePackageReader(FileInfo fileInfo) : this(fileInfo, 1024 * 8, 1024 * 4, 0)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件路径</param>
        /// <param name="file">与<paramref name="fileInfo"/>相匹配的文件流，初始化索引时会从当前位置开始</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小</param>
        /// <param name="isDisposeFileStream">在释放资源时是否释放给定的流，默认为true</param>
        /// <param name="createStreamBufferSize">在创建截取流时的数据缓冲区大小，如果该参数为0，则使用无内置缓冲区流，值默认为0</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">给定的流没有读取和查找权限</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo, FileStream file, int bufferSize, bool isDisposeFileStream, int createStreamBufferSize)
        {
            if (fileInfo is null || file is null) throw new ArgumentNullException();
            if (bufferSize <= 0 || createStreamBufferSize < 0) throw new ArgumentOutOfRangeException();
            if(!(file.CanRead && file.CanSeek))
            {
                throw new NotSupportedException();
            }
            //FileStream file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            p_stream = file;
            p_createStreamBufferSize = createStreamBufferSize;
            f_init(file, bufferSize);
            p_fileInfo = fileInfo;
            p_disposeBasefile = isDisposeFileStream;
            f_fileInit();
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件路径</param>
        /// <param name="file">与<paramref name="fileInfo"/>相匹配的文件流，初始化索引时会从当前位置开始</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小</param>
        /// <param name="isDisposeFileStream">在释放资源时是否释放给定的流，默认为true</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">给定的流没有读取和查找权限</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo, FileStream file, int bufferSize, bool isDisposeFileStream) : this(fileInfo, file, bufferSize, isDisposeFileStream, 0)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件路径</param>
        /// <param name="file">与<paramref name="fileInfo"/>相匹配的文件流，初始化索引时会从当前位置开始</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区大小</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">给定的流没有读取和查找权限</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo, FileStream file, int bufferSize) : this(fileInfo, file, bufferSize, true)
        {
        }

        /// <summary>
        /// 实例化只读资源文件包读取解析器
        /// </summary>
        /// <param name="fileInfo">要访问的文件路径</param>
        /// <param name="file">与<paramref name="fileInfo"/>相匹配的文件流，初始化索引时会从当前位置开始</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="DirectoryNotFoundException">路径不存在</exception>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">给定的流没有读取和查找权限</exception>
        /// <exception cref="System.Security.SecurityException">没有指定权限</exception>
        /// <exception cref="PathTooLongException">路径格式不正确</exception>
        /// <exception cref="Exception">异常</exception>
        public FileResourcePackageReader(FileInfo fileInfo, FileStream file) : this(fileInfo, file, 1024 * 8, true, 0)
        {
        }

        /// <summary>
        /// 初始化文件参数
        /// </summary>
        protected void f_fileInit()
        {
            p_fullFilePath = p_fileInfo.FullName;
        }

        #endregion

        #region 回收

        #region 封装

        #endregion

        /// <summary>
        /// 在派生类重写此方法，用于释放非托管资源和托管对象
        /// </summary>
        /// <remarks>该方法在首次调用<see cref="SafreleaseUnmanagedResources.Dispose(bool)"/>方法时被调用，<paramref name="disposeing"/>参数由<see cref="SafreleaseUnmanagedResources.Dispose(bool)"/>的参数传递</remarks>
        /// <param name="disposeing">是否清理托管资源对象</param>
        /// <returns>
        /// <para>是否关闭该对象的析构方法</para>
        /// <para>返回为true</para>
        /// </returns>
        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing && p_disposeBasefile)
            {
                p_stream?.Close();
            }
            p_stream = null;

            return true;
        }

        /// <summary>
        /// 需要释放文件资源
        /// </summary>
        public override bool IsNeedToReleaseResources => true;

        #endregion

        #region 参数

        private FileInfo p_fileInfo;

        private string p_fullFilePath;

        private int p_createStreamBufferSize;

        private bool p_disposeBasefile;

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置打开数据时流的缓冲区大小
        /// </summary>
        /// <value>
        /// <para>使用<see cref="ResourcePackageReader.OpenCompressedStream(int)"/>或<see cref="ResourcePackageReader.OpenCompressedStream(string)"/>打开数据时，返回的流内部的缓冲区大小，设置为 0 则表示不需要缓冲区</para>
        /// <para>默认值为 0</para>
        /// </value>
        public int CreateStreamBufferSize
        {
            get => p_createStreamBufferSize;
            set
            {
                if (value < 0) value = 0;
                p_createStreamBufferSize = value;
            }
        }

        protected override Stream CreateGetBaseStream(long position, long length)
        {
            if (length == 0) return Stream.Null;

            FileStream file = new FileStream(p_fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            if (p_createStreamBufferSize == 0)
            {
                return new NotBufferTruncateStream(file, position, length, true);
            }

            return new TruncateStream(file, position, length, true, p_createStreamBufferSize);
        }

        public override bool CanCreateGetBaseStreamIsIndependent => true;

        #endregion

        #endregion

    }

}
