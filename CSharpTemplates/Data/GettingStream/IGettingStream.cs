using Cheng.IO;
using System;
using System.IO;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 数据IO来源类型
    /// </summary>
    public enum OpenStreamIOSourceType
    {

        /// <summary>
        /// 错误的数据
        /// </summary>
        /// <remarks>
        /// <para>打开的对象是一个无法读取的，错误的数据</para>
        /// </remarks>
        Error = -1,

        /// <summary>
        /// 未知来源或来源不可追踪
        /// </summary>
        /// <remarks>
        /// <para>此类型表示打开的<see cref="Stream"/>读取或写入的数据最终指向的位置不确定；例如一些对象解析器的基类</para>
        /// </remarks>
        Unknown = 0,

        /// <summary>
        /// 数据来源于本地文件系统
        /// </summary>
        /// <remarks>
        /// <para>表示从文件系统访问的数据，例如<see cref="FileStream"/></para>
        /// </remarks>
        FileIOStream,

        /// <summary>
        /// 数据来源于系统内存
        /// </summary>
        /// <remarks>
        /// <para>数据来源于进程或其它进程的内存，例如<see cref="MemoryStream"/></para>
        /// </remarks>
        MemoryData,

        /// <summary>
        /// 数据来源于远程网络
        /// </summary>
        /// <remarks>
        /// <para>任何来源非本地设备的数据都属于该类型，例如<see cref="System.Net.Sockets.NetworkStream"/></para>
        /// </remarks>
        WebStream

    }

    /// <summary>
    /// 提供一个可打开<see cref="Stream"/>对象的获取接口
    /// </summary>
    public interface IGettingStream
    {

        /// <summary>
        /// 该数据流<see cref="Stream"/>的IO来源
        /// </summary>
        OpenStreamIOSourceType SourceType { get; }

        /// <summary>
        /// 当前对象所指向的<see cref="Stream"/>的长度
        /// </summary>
        /// <returns><see cref="Stream"/>的长度，如果返回-1表示长度无法确认</returns>
        long StreamLength { get; }

        /// <summary>
        /// 打开当前对象所指向的<see cref="Stream"/>
        /// </summary>
        /// <returns>一个<see cref="Stream"/>对象，不再使用后需要关闭；如果返回null则表示因为各种原因无法打开流对象</returns>
        Stream OpenStream();

    }

    public static class GettingStreamExtend
    {

        #region 封装

        private sealed class GettingStreamError : IGettingStream
        {
            public GettingStreamError()
            {
            }

            public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.Error;

            public long StreamLength => -1;

            public Stream OpenStream()
            {
                return null;
            }

            internal static GettingStreamError error = new GettingStreamError();
        }

        #endregion

        /// <summary>
        /// 表示一个错误的对象获取器
        /// </summary>
        public static IGettingStream ErrorGetter
        {
            get => GettingStreamError.error;
        }

    }

    /// <summary>
    /// 使用委托函数实现一个<see cref="IGettingStream"/>
    /// </summary>
    public sealed class GettingStreamFunction : IGettingStream
    {

        /// <summary>
        /// 实例化委托实现封装数据获取接口
        /// </summary>
        /// <param name="openStream">提供一个获取<see cref="Stream"/>对象的函数，参数是null表示<see cref="OpenStream"/>函数永远返回null</param>
        /// <param name="getLength">提供一个获取流长度的函数，参数是null表示无法获取长度</param>
        public GettingStreamFunction(Func<Stream> openStream, Func<long> getLength)
        {
            p_openStream = openStream;
            p_getLength = getLength;
        }

        private readonly Func<Stream> p_openStream;
        private readonly Func<long> p_getLength;

        public long StreamLength
        {
            get
            {
                try
                {
                    return (p_getLength?.Invoke()).GetValueOrDefault(-1);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public Stream OpenStream()
        {
            try
            {
                return p_openStream?.Invoke();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.Unknown;

    }

    /// <summary>
    /// 提供一个打开指定路径的文件流对象接口
    /// </summary>
    public sealed class GettingFileStream : IGettingStream
    {

        #region

        /// <summary>
        /// 实例化一个打开指定路径的文件流接口
        /// </summary>
        /// <param name="filePath"></param>
        public GettingFileStream(string filePath)
        {
            fileInfo = new FileInfo(Path.GetFullPath(filePath));
        }

        #endregion

        #region

        private readonly FileInfo fileInfo;

        #endregion

        #region 派生

        public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.FileIOStream;

        public long StreamLength
        {
            get
            {
                try
                {
                    fileInfo.Refresh();
                    return fileInfo.Length;
                }
                catch (Exception)
                {
                    return -1;
                }
                
            }
        }

        public Stream OpenStream()
        {
            try
            {
                return fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

    }

    /// <summary>
    /// 使用一个字节数组实现流对象获取接口
    /// </summary>
    public sealed class GettingMemoryStream : IGettingStream
    {

        /// <summary>
        /// 使用一个字节数组实例化流对象获取接口
        /// </summary>
        /// <param name="buffer">要使用的字节数组</param>
        public GettingMemoryStream(byte[] buffer)
        {
            p_buffer = buffer ?? throw new ArgumentNullException();
        }

        private readonly byte[] p_buffer;

        public long StreamLength => p_buffer.Length;

        public Stream OpenStream()
        {
            return new MemoryStream(p_buffer);
        }

        public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.MemoryData;

    }

}
