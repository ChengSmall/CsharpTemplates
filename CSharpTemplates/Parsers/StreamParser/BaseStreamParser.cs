using System;
using System.IO;

namespace Cheng.Streams.Parsers
{

    /// <summary>
    /// 流数据解析器公共接口
    /// </summary>
    public interface IStreamParser
    {

        /// <summary>
        /// 读取流数据转化为对象
        /// </summary>
        /// <param name="stream">要读取的流数据</param>
        /// <returns>转化到的对象</returns>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        object ConverToObject(Stream stream);

        /// <summary>
        /// 将给定对象转化并写入流数据
        /// </summary>
        /// <param name="obj">要转化的对象</param>
        /// <param name="stream">要写入的数据流对象</param>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        void ConverToStream(object obj, Stream stream);
    }

    /// <summary>
    /// 实现流数据解析器的基类
    /// </summary>
    public abstract class StreamParser : IStreamParser
    {

        #region 派生

        /// <summary>
        /// 读取流数据转化为对象
        /// </summary>
        /// <param name="stream">要读取的流数据</param>
        /// <returns>转化到的对象</returns>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        /// <exception cref="StreamParserException">无法解析</exception>
        public abstract object ConverToObject(Stream stream);

        /// <summary>
        /// 将给定对象转化并写入流数据
        /// </summary>
        /// <param name="obj">要转化的对象</param>
        /// <param name="stream">要写入的数据流对象</param>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        /// <exception cref="StreamParserException">无法解析</exception>
        public abstract void ConverToStream(object obj, Stream stream);

        #endregion

        #region 功能

        /// <summary>
        /// 线程安全封装
        /// </summary>
        /// <param name="streamParser">要封装为线程安全的解析器</param>
        /// <returns>一个线程安全的流数据解析器</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static StreamParser AsnyhSafe(IStreamParser streamParser)
        {
            return new ThreadSafe(streamParser);
        }

        /// <summary>
        /// 读取流数据转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">要读取的流数据</param>
        /// <returns>转化到的对象</returns>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        /// <exception cref="StreamParserException">无法解析</exception>
        public virtual T ConverToObject<T>(Stream stream)
        {
            return (T)ConverToObject(stream);
        }

        #endregion

        #region 结构

        class ThreadSafe : StreamParser
        {
            public ThreadSafe(IStreamParser sp)
            {
                this.sp = sp ?? throw new ArgumentNullException();
            }

            IStreamParser sp;

            public override object ConverToObject(Stream stream)
            {
                lock (sp) return sp.ConverToObject(stream);
            }

            public override void ConverToStream(object obj, Stream stream)
            {
                lock (sp) sp.ConverToStream(obj, stream);
            }
        }

        #endregion
    }

}
