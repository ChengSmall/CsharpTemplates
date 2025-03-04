using System;

namespace Cheng.Algorithm.Encryptions.Base64Encryption
{

    /// <summary>
    /// Base64编码错误异常
    /// </summary>
    public class Base64EncoderException : Exception
    {

        public Base64EncoderException() : base(Cheng.Properties.Resources.Exception_Base64EncoderError)
        {
        }

        public Base64EncoderException(string message) : base(message)
        {
        }

        public Base64EncoderException(string message, Exception exception) : base(message, exception)
        {
        }
    }


}
