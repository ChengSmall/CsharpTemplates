using Cheng.DataStructure;
using System;
using System.IO;

namespace Cheng.Algorithm.Compressions
{

    /// <summary>
    /// 使用索引打开数据以实现<see cref="IGettingStream"/>接口
    /// </summary>
    public sealed class CompressionParserOpenStreamByIndex : IGettingStream
    {

        /// <summary>
        /// 实例化流访问对象
        /// </summary>
        /// <param name="compressionParser">要获取数据的数据读取器</param>
        /// <param name="index">要获取的数据路径</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public CompressionParserOpenStreamByIndex(BaseCompressionParser compressionParser, int index)
        {
            p_par = compressionParser ?? throw new ArgumentNullException(nameof(compressionParser));
            if ((!compressionParser.CanOpenCompressedStreamByIndex) || (!compressionParser.CanIndexOf))
            {
                throw new NotSupportedException();
            }
            p_i = index;
        }

        private BaseCompressionParser p_par;
        private int p_i;

        public long StreamLength
        {
            get
            {
                if (p_i < 0 || p_i >= p_par.Count) return -1;
                var inf = p_par[p_i];
                return inf.BeforeSize;
            }
        }

        public Stream OpenStream()
        {
            try
            {
                return p_par.OpenCompressedStream(p_i);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    /// <summary>
    /// 使用路径打开数据以<see cref="IGettingStream"/>接口
    /// </summary>
    public sealed class CompressionParserOpenStreamByPath : IGettingStream
    {

        /// <summary>
        /// 实例化流访问对象
        /// </summary>
        /// <param name="compressionParser">要获取数据的数据读取器</param>
        /// <param name="path">要获取的数据路径</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public CompressionParserOpenStreamByPath(BaseCompressionParser compressionParser, string path)
        {
            p_par = compressionParser ?? throw new ArgumentNullException(nameof(compressionParser));
            if (!compressionParser.CanOpenCompressedStreamByPath)
            {
                throw new NotSupportedException();
            }

            p_path = path ?? throw new ArgumentNullException(nameof(path));
        }

        private BaseCompressionParser p_par;
        private string p_path;

        public long StreamLength
        {
            get
            {
                var inf = p_par[p_path];
                return inf.BeforeSize;
            }
        }

        public Stream OpenStream()
        {
            try
            {
                return p_par.OpenCompressedStream(p_path);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}
