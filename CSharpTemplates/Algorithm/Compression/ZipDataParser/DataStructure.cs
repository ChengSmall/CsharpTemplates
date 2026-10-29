using System;
using System.IO;

using Cheng.DataStructure;

namespace Cheng.Algorithm.Compressions.ZipDataParser
{

    #region IGettingStream实现

    /// <summary>
    /// 从<see cref="ZipDataCompressionParser"/>获取原始数据流的<see cref="Stream"/>获取方法
    /// </summary>
    public sealed class GettingZipRawDataStream : IGettingStream
    {

        /// <summary>
        /// 实例化流获取器
        /// </summary>
        /// <param name="zipParser">zip解析器</param>
        /// <param name="zipInformation">zip解析器对应的项</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public GettingZipRawDataStream(ZipDataCompressionParser zipParser, ZipDataInformation zipInformation) : this(zipParser, zipInformation, -1) {}

        /// <summary>
        /// 实例化流获取器
        /// </summary>
        /// <param name="zipParser">zip解析器</param>
        /// <param name="zipInformation">zip解析器对应的项</param>
        /// <param name="size">指定由<see cref="StreamLength"/>返回的值，如果小于0会从<paramref name="zipInformation"/>获取压缩后大小</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public GettingZipRawDataStream(ZipDataCompressionParser zipParser, ZipDataInformation zipInformation, long size)
        {
            zip = zipParser ?? throw new ArgumentNullException();
            inf = zipInformation ?? throw new ArgumentNullException();
            s = size;
        }

        private readonly ZipDataCompressionParser zip;
        private readonly ZipDataInformation inf;
        private readonly long s;

        public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.Unknown;

        public long StreamLength
        {
            get
            {
                if(s < 0) return inf.CompressedSize;
                return s;
            }
        }

        public Stream OpenStream()
        {
            try
            {
                return zip.OpenRawStreamByInformation(inf);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    #endregion

}
