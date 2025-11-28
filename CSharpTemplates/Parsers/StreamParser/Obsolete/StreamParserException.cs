using System;

namespace Cheng.Streams.Parsers
{

    /// <summary>
    /// 数据流解析器异常基类
    /// </summary>
    public class StreamParserException : Exception
    {

        #region
        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        public StreamParserException() : base("引发流数据解析器异常")
        {
        }
        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="message">指定错误消息</param>
        public StreamParserException(string message) : base(message)
        {
        }
        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="message">指定错误消息</param>
        /// <param name="exception">引发该异常的异常</param>
        public StreamParserException(string message, Exception exception) : base(message, exception)
        {
        }
        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="type">引发异常的解析类型</param>
        public StreamParserException(Type type)
        {
            this.type = type;
        }
        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="type">引发异常的解析类型</param>
        /// <param name="message">指定错误消息</param>
        public StreamParserException(Type type, string message) : base(message)
        {
            this.type = type;
        }
        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="type">引发异常的解析类型</param>
        /// <param name="message">指定错误消息</param>
        /// <param name="exception">引发该异常的异常</param>
        public StreamParserException(Type type, string message, Exception exception) : base(message, exception)
        {
            this.type = type;
        }

        #endregion

        private Type type;

        /// <summary>
        /// 获取或设置引发异常的解析类型
        /// </summary>
        /// <value>引发异常的解析类型，若没有异常的解析类型则是一个null</value>
        public Type ExceptionType
        {
            get => type;
            set
            {
                type = value;
            }
        }

    }

}
