using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

using Cheng.DataStructure;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Trees;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure.Streams;
using Cheng.Streams;


namespace Cheng.Algorithm.Compressions.ZipDataParser
{

    #region 基类

    /// <summary>
    /// zip项数据解压缩器的公共接口
    /// </summary>
    public interface IZipItemDecompress
    {

        /// <summary>
        /// 查看解压缩器是否支持指定的压缩类型
        /// </summary>
        /// <param name="compressionMethodCode">zip包存储项的压缩方法类型标准码，0表示仅存储</param>
        /// <returns>是否支持<paramref name="compressionMethodCode"/>类型的压缩项；当<paramref name="compressionMethodCode"/>是0时，必须返回true</returns>
        bool CanDecompressType(int compressionMethodCode);

        /// <summary>
        /// 查看解压缩器在解压缩指定类型的项时基础流数据是否必须要随机读取权限
        /// </summary>
        /// <param name="compressionMethodCode">zip包存储项的压缩方法类型标准码，0表示仅存储</param>
        /// <returns>
        /// 仅当<see cref="CanDecompressType(int)"/>返回true时，该函数返回值才有效
        /// 当<paramref name="compressionMethodCode"/>是0时，必须返回false<br/>
        /// </returns>
        bool DecompressOnSeek(int compressionMethodCode);

        /// <summary>
        /// 为流数据创建一个解压缩的读取流
        /// </summary>
        /// <param name="stream">
        /// <para>要封装的基础流数据对象，用于读取需要解压缩或解码的zip项数据</para>
        /// </param>
        /// <param name="compressionMethodCode">
        /// <para>表示zip包存储项的压缩方法类型标准码，例如0表示仅存储，可直接返回<paramref name="stream"/></para>
        /// </param>
        /// <returns>
        /// <para>从<paramref name="stream"/>解压缩数据的包装流对象，该流数据在释放时需要释放基础流<paramref name="stream"/></para>
        /// </returns>
        /// <exception cref="NotSupportedException">对象<paramref name="stream"/>的读取权限不足以支持解压缩流的创建</exception>
        /// <exception cref="InvalidDataException">不支持该类型的压缩数据</exception>
        Stream CreateDecompressStream(Stream stream, int compressionMethodCode);

    }

    /// <summary>
    /// zip项数据解压缩实现基类
    /// </summary>
    public abstract class BaseZipItemDecompress : IZipItemDecompress
    {

        /// <summary>
        /// 查看解压缩器是否支持指定的压缩类型
        /// </summary>
        /// <param name="compressionMethodCode">zip包存储项的压缩方法类型标准码，0表示仅存储</param>
        /// <returns>是否支持<paramref name="compressionMethodCode"/>类型的压缩项；当<paramref name="compressionMethodCode"/>是0时，必须返回true</returns>
        public virtual bool CanDecompressType(int compressionMethodCode)
        {
            return compressionMethodCode == 0;
        }

        /// <summary>
        /// 查看解压缩器在解压缩指定类型的项时基础流数据是否必须要随机读取权限
        /// </summary>
        /// <param name="compressionMethodCode">zip包存储项的压缩方法类型标准码，0表示仅存储</param>
        /// <returns>
        /// 仅当<see cref="CanDecompressType(int)"/>返回true时，该函数返回值才有效
        /// 当<paramref name="compressionMethodCode"/>是0时，必须返回false<br/>
        /// </returns>
        public virtual bool DecompressOnSeek(int compressionMethodCode)
        {
            return compressionMethodCode != 0;
        }

        /// <summary>
        /// 为流数据创建一个解压缩的读取流
        /// </summary>
        /// <param name="stream">
        /// <para>要封装的基础流数据对象，用于读取需要解压缩或解码的zip项数据</para>
        /// </param>
        /// <param name="compressionMethodCode">
        /// <para>表示zip包存储项的压缩方法类型标准码，例如0表示仅存储，可直接返回<paramref name="stream"/></para>
        /// </param>
        /// <returns>
        /// <para>从<paramref name="stream"/>解压缩数据的包装流对象，该流数据在释放时需要释放基础流<paramref name="stream"/></para>
        /// </returns>
        /// <exception cref="NotSupportedException">对象<paramref name="stream"/>的读取权限不足以支持解压缩流的创建</exception>
        /// <exception cref="InvalidDataException">不支持该类型的压缩数据</exception>
        public abstract Stream CreateDecompressStream(Stream stream, int compressionMethodCode);
       
    }

    #endregion

    #region 默认实现

    internal sealed class ZipItemDecompressDefault : BaseZipItemDecompress
    {

        #region 创建

        internal static BaseZipItemDecompress CreateDef()
        {
            return new ZipItemDecompressDefault();
        }

        #endregion

        #region 实现

        internal ZipItemDecompressDefault()
        {
        }

        public override bool CanDecompressType(int compressionMethodCode)
        {
            switch (compressionMethodCode)
            {
                case 0:
                case 8:
                case 9:
                    return true;
                default:
                    return false;
            }
        }

        public override bool DecompressOnSeek(int compressionMethodCode) => false;

        public override Stream CreateDecompressStream(Stream stream, int compressionMethodCode)
        {

            switch (compressionMethodCode)
            {
                case 0:
                    // 仅存储
                    return stream;
                case 8:
                    // deflate
                    return new DeflateStream(stream, CompressionMode.Decompress, false);
                case 9:
                    // deflate64
                    return new Deflate64DecompressStream(stream, true);
            }

            throw new InvalidDataException(ZipDataCompressionParser.cp_exceptionUnsupportedCompression);
        }

        #endregion

    }

    #endregion

}
