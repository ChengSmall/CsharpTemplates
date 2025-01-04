using System;
using System.IO;
using System.Text;

namespace Cheng.Algorithm.Encryptions
{

    /// <summary>
    /// 一个字符串加密算法的基类
    /// </summary>
    public abstract class BaseStringEncryption
    {

        #region 构造
        protected BaseStringEncryption() { }
        #endregion

        #region 参数访问
        /// <summary>
        /// 获取当前字符串加密时的字符串编码
        /// </summary>
        public abstract Encoding Encoding { get; }

        #endregion
        
        #region 方法

        /// <summary>
        /// 将给定的数据加密到另一个数据当中
        /// </summary>
        /// <param name="reader">要加密的读取字符序列</param>
        /// <param name="writer">加密到的写入字符序列</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public abstract void Encry(TextReader reader, TextWriter writer);      

        /// <summary>
        /// 将给定的字符数组加密
        /// </summary>
        /// <param name="buffer">要加密的字符数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">要加密的字符数量</param>
        /// <returns>加密后的字符数组</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public virtual char[] Encry(char[] buffer, int index, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            return Encry(new string(buffer, index, count), 0, count).ToCharArray();
        }

        /// <summary>
        /// 将给定的字符串加密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">字符数</param>
        /// <returns>加密后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public virtual string Encry(string str, int startIndex, int count)
        {
            if (str is null) throw new ArgumentNullException();

            StringReader sr = new StringReader(str.Substring(startIndex, count));
            StringWriter sw = new StringWriter();
            Encry(sr, sw);
            return sw.ToString();
        }

        /// <summary>
        /// 将给定的字符串加密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>加密后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual string Encry(string str)
        {
            if (str is null) throw new ArgumentNullException();
            return Encry(str, 0, str.Length);
        }

        /// <summary>
        /// 将给定的数据解密到另一个数据当中
        /// </summary>
        /// <param name="reader">要解密的读取字符序列</param>
        /// <param name="writer">解密到的写入字符序列</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public abstract void Dncry(TextReader reader, TextWriter writer);

        /// <summary>
        /// 将给定的字符数组解密
        /// </summary>
        /// <param name="buffer">要解密的字符数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">要解密的字符数量</param>
        /// <returns>解密后的字符数组</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public virtual char[] Dncry(char[] buffer, int index, int count)
        {
            return Dncry(new string(buffer, index, count), 0, count).ToCharArray();
        }

        /// <summary>
        /// 将给定的字符串解密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">字符数</param>
        /// <returns>解密后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public virtual string Dncry(string str, int startIndex, int count)
        {
            if (str is null) throw new ArgumentNullException();
            if (str.Length < startIndex + count) throw new ArgumentOutOfRangeException();

            StringReader sr = new StringReader(str.Substring(startIndex, count));
            StringWriter sw = new StringWriter();
            Dncry(sr, sw);
            return sw.ToString();
        }

        /// <summary>
        /// 将给定的字符串解密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解密后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual string Dncry(string str)
        {
            return Dncry(str, 0, str.Length);
        }

        #endregion

    }
}
