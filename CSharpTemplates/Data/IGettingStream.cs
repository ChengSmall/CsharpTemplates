using Cheng.IO;
using System;
using System.IO;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 提供一个可打开<see cref="Stream"/>对象的获取接口
    /// </summary>
    public interface IGettingStream
    {

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
    }

    /// <summary>
    /// 提供一个打开指定路径的文件流对象接口
    /// </summary>
    public class GettingFileStream : IGettingStream
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

}
