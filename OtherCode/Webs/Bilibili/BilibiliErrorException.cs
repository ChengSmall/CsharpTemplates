using System;

namespace Cheng.Bilibili
{

    /// <summary>
    /// Bilibili错误代码
    /// </summary>
    public class BilibiliErrorException : Exception
    {

        public BilibiliErrorException() : base()
        {
        }

        /// <summary>
        /// 实例化Bilibili数据异常
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="id">code</param>
        /// <param name="ex">引发异常的异常</param>
        public BilibiliErrorException(string message, int id, Exception ex) : base(message, ex)
        {
            messageID = id;
        }

        /// <summary>
        /// 实例化Bilibili数据异常
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="id">code</param>
        public BilibiliErrorException(string message, int id) : base(message)
        {
            messageID = id;
        }

        private int messageID;

        /// <summary>
        /// 错误代码
        /// </summary>
        public int MessageID
        {
            get => messageID;
        }

    }

}
