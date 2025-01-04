using System;
using System.IO;
using System.Text;

namespace Cheng.Algorithm.Encryptions.SymmetricEncryption
{
    /// <summary>
    /// Base64算法实现
    /// </summary>
    public unsafe class Base64 : BaseStringEncryption
    {

        #region 构造
        public Base64()
        {
            //p_buffer = new byte[16];
            //p_charBuffer = new byte[16];
            p_encoding = Encoding.UTF8;
        }
        /// <summary>
        /// 实例化一个base64加密算法
        /// </summary>
        /// <param name="encoding">要加密到base64的字符编码</param>
        public Base64(Encoding encoding)
        {
            p_encoding = encoding ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数


        private Encoding p_encoding;

        //private byte[] p_buffer;
        //private byte[] p_charBuffer;
        #endregion

        #region 参数访问
        public override Encoding Encoding => p_encoding;

        #endregion

        #region 封装

        public override void Encry(TextReader reader, TextWriter writer)
        {
            string str = Convert.ToBase64String(p_encoding.GetBytes(reader.ReadToEnd()));
            writer.Write(str);
        }

        public override void Dncry(TextReader reader, TextWriter writer)
        {
            var bs = Convert.FromBase64String(reader.ReadToEnd());
            writer.Write(p_encoding.GetString(bs));
        }

        public override string Dncry(string str, int startIndex, int count)
        {
            var bs = Convert.FromBase64String(str?.Substring(startIndex, count));
            return p_encoding.GetString(bs);
        }

        public override string Encry(string str, int startIndex, int count)
        {
            string strs = Convert.ToBase64String(p_encoding.GetBytes(str?.Substring(startIndex, count)));
            return strs;
        }

        public override string Dncry(string str)
        {
            var bs = Convert.FromBase64String(str);
            return p_encoding.GetString(bs);
        }

        public override string Encry(string str)
        {
            string strs = Convert.ToBase64String(p_encoding.GetBytes(str));
            return strs;
        }

        #endregion


    }

}
