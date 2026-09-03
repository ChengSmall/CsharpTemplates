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
        /// <exception cref="Exception">其他错误</exception>
        public abstract object ConverToObject(Stream stream);

        /// <summary>
        /// 将给定对象转化并写入流数据
        /// </summary>
        /// <param name="obj">要转化的对象</param>
        /// <param name="stream">要写入的数据流对象</param>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        /// <exception cref="Exception">其他错误</exception>
        public abstract void ConverToStream(object obj, Stream stream);

        #endregion

        #region 功能

        /// <summary>
        /// 读取流数据转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">要读取的流数据</param>
        /// <returns>转化到的对象</returns>
        /// <exception cref="ArgumentNullException">流对象为null</exception>
        public virtual T ConverToObject<T>(Stream stream)
        {
            return (T)ConverToObject(stream);
        }

        #endregion

    }

}
