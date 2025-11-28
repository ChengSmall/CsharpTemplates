using Cheng.Texts;
using System;
using System.IO;
using System.Text;


namespace Cheng.Algorithm.Encryptions.SSH
{

    static partial class RsaSSH
    {

        /// <summary>
        /// 从文本读取rsa公钥并转化为字节数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte[] GetPubSHH(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            var sb = new CMStringBuilder(1024);
            CMStringBuilderWriter strwr;
            strwr = new CMStringBuilderWriter(sb);
            PubSSHToBase64Str(reader, strwr);
            strwr.Close();
            var buf = sb.GetCharBuffer();
            return Convert.FromBase64CharArray(buf.Array, buf.Offset, buf.Count);
        }

        /// <summary>
        /// 从文件读取rsa公钥并转化为字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetPubSSH(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                using (StreamReader sr = new StreamReader(file, Encoding.ASCII, false, 1024, true))
                {
                    return GetPubSHH(sr);
                }
            }
        }

        /// <summary>
        /// 从文本读取私钥并转化为字节数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte[] GetSSH(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            var sb = new CMStringBuilder(1024);
            CMStringBuilderWriter strwr;
            strwr = new CMStringBuilderWriter(sb);
            SSHToBase64Str(reader, strwr);
            strwr.Close();
            var buf = sb.GetCharBuffer();
            return Convert.FromBase64CharArray(buf.Array, buf.Offset, buf.Count);
        }

        /// <summary>
        /// 从文件读取私钥并转化为字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetSSH(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                using (StreamReader sr = new StreamReader(file, Encoding.ASCII, false, 1024, true))
                {
                    return GetSSH(sr);
                }
            }
        }

    }

}