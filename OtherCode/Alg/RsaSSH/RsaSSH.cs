
using System;
using System.IO;
using System.Text;

namespace Cheng.Algorithm.Encryptions.SSH
{

    /// <summary>
    /// Rsa加密验证
    /// </summary>
    internal static partial class RsaSSH
    {

        /// <summary>
        /// SSH密钥验证
        /// </summary>
        /// <param name="pubRSA">表示rsa公钥的二进制数据</param>
        /// <param name="rsaInput">表示密钥的二进制数据</param>
        /// <param name="password">加密密码，如果是null表示没有输入密码</param>
        /// <param name="exception">额外错误</param>
        /// <returns>返回true表示成功，返回false表示私钥没有匹配公钥或其它未成功验证的情况</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static bool VerifyRSA(byte[] pubRSA, byte[] rsaInput, byte[] password, out Exception exception)
        {
            if (pubRSA is null || rsaInput is null) throw new ArgumentNullException();
            exception = null;
            try
            {
                // 解析公钥
                RSAParameters publicParams = ParseSSHPublicKey(pubRSA);

                // 解析私钥
                RSAParameters privateParams = ParseSSHPrivateKey(rsaInput, password);

                // 验证公钥和私钥是否匹配
                return CompareRSAParameters(publicParams, privateParams);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        #region

        internal struct RSAParameters
        {
            public byte[] Exponent;
            public byte[] Modulus;
            public byte[] P;
            public byte[] Q;
            public byte[] DP;
            public byte[] DQ;
            public byte[] InverseQ;
            public byte[] D;
        }

        /// <summary>
        /// 读取rsa公钥并剔除前缀和后缀仅剩base64文本
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static string PubSSHToBase64Str(TextReader reader, TextWriter writer)
        {
            char[] cbuf = new char[8];

            reader.ReadBlock(cbuf, 0, 8);

            while (true)
            {
                var re = reader.Read();
                if (re == ' ')
                {
                    break;
                }
                writer.Write((char)re);
            }

            return reader.ReadToEnd();
        }

        /// <summary>
        /// 使用读取SSH密钥并把多余文本剔除仅剩base64文本
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        public static void SSHToBase64Str(TextReader reader, TextWriter writer)
        {
            while (true)
            {
                var line = reader.ReadLine();
                if (line is null) return;
                if (line.Length == 0) continue;
                if (line[0] == '-') continue;
                writer.Write(line);
            }
        }

        #endregion

        #region

        // 解析SSH RSA公钥
        private static RSAParameters ParseSSHPublicKey(byte[] publicKeyBytes)
        {
            using (MemoryStream stream = new MemoryStream(publicKeyBytes))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // 读取算法标识 "ssh-rsa"
                string algorithm = ReadString(reader);
                if (algorithm != "ssh-rsa")
                    throw new InvalidDataException("不是SSH RSA公钥");

                // 读取指数
                byte[] exponent = ReadBytes(reader);

                // 读取模数
                byte[] modulus = ReadBytes(reader);

                return new RSAParameters
                {
                    Exponent = exponent,
                    Modulus = modulus
                };
            }
        }

        // 解析SSH RSA私钥
        private static RSAParameters ParseSSHPrivateKey(byte[] privateKeyBytes, byte[] password)
        {
            using (MemoryStream stream = new MemoryStream(privateKeyBytes))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // 读取魔术字
                byte[] magic = reader.ReadBytes(15);
                string magicString = Encoding.ASCII.GetString(magic);

                if (magicString != "openssh-key-v1\0")
                    throw new InvalidDataException("不是OpenSSH私钥格式");

                // 读取加密算法名称
                string cipherName = ReadString(reader);
                bool isEncrypted = cipherName != "none";

                // 读取KDF名称
                string kdfName = ReadString(reader);

                // 读取KDF数据
                byte[] kdfData = ReadBytes(reader);

                // 读取密钥数量
                uint keyCount = ReadUInt32(reader);
                if (keyCount != 1)
                    throw new InvalidDataException("不支持多个密钥");

                // 读取公钥（跳过）
                byte[] publicKey = ReadBytes(reader);

                // 读取私钥数据
                byte[] privateKeyData = ReadBytes(reader);

                // 如果私钥被加密
                if (isEncrypted)
                {
                    if (password == null)
                        throw new InvalidOperationException("私钥被加密但未提供密码");

                    // 这里简化处理，实际需要根据KDF算法解密
                    // 由于复杂性，这里假设密码正确且能够解密
                    // 实际生产环境需要完整实现解密逻辑
                    privateKeyData = TryDecryptPrivateKey(privateKeyData, cipherName, kdfName, kdfData, password);
                }

                // 解析私钥数据
                return ParsePrivateKeyData(privateKeyData);
            }
        }

        // 解析私钥数据
        private static RSAParameters ParsePrivateKeyData(byte[] privateKeyData)
        {
            using (MemoryStream stream = new MemoryStream(privateKeyData))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // 读取检查值1和2
                uint check1 = ReadUInt32(reader);
                uint check2 = ReadUInt32(reader);

                if (check1 != check2)
                    throw new InvalidDataException("私钥检查值不匹配");

                // 读取密钥类型
                string keyType = ReadString(reader);
                if (keyType != "ssh-rsa")
                    throw new InvalidDataException("不是RSA私钥");

                // 读取模数
                byte[] modulus = ReadBytes(reader);

                // 读取公共指数
                byte[] publicExponent = ReadBytes(reader);

                // 读取私有指数
                byte[] privateExponent = ReadBytes(reader);

                // 读取系数
                byte[] coefficient = ReadBytes(reader);

                // 读取质数1
                byte[] prime1 = ReadBytes(reader);

                // 读取质数2
                byte[] prime2 = ReadBytes(reader);

                return new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = publicExponent,
                    D = privateExponent,
                    P = prime1,
                    Q = prime2,
                    DP = new byte[1], // 简化处理
                    DQ = new byte[1], // 简化处理
                    InverseQ = coefficient
                };
            }
        }

        // 尝试解密私钥
        private static byte[] TryDecryptPrivateKey(byte[] encryptedData, string cipherName, string kdfName, byte[] kdfData, byte[] password)
        {
            //根据具体的KDF算法和加密算法进行完整实现
            if (cipherName == "aes256-ctr" || cipherName == "aes256-cbc")
            {
                // 应使用KDF数据派生密钥，然后解密
                // 待处理 实现完整的AES-CTR/BC等解密逻辑

                //假设密码正确且能够解密，直接返回原数据
                return encryptedData;
            }

            throw new NotSupportedException($"不支持的加密算法: {cipherName}");
        }

        // 比较RSA参数是否匹配
        private static bool CompareRSAParameters(RSAParameters publicParams, RSAParameters privateParams)
        {
            // 比较模数和指数
            return CompareByteArrays(publicParams.Modulus, privateParams.Modulus) &&
                   CompareByteArrays(publicParams.Exponent, privateParams.Exponent);
        }

        // 读取字符串（长度前缀）
        private static string ReadString(BinaryReader reader)
        {
            uint length = ReadUInt32(reader);
            byte[] data = reader.ReadBytes((int)length);
            //return data;
            return Encoding.ASCII.GetString(data);
        }

        // 读取字节数组（长度前缀）
        private static byte[] ReadBytes(BinaryReader reader)
        {
            uint length = ReadUInt32(reader);
            return reader.ReadBytes((int)length);
        }

        // 读取32位无符号整数（大端序）
        private static uint ReadUInt32(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        // 比较字节数组
        private unsafe static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;

            fixed (byte* ptr_a = a, ptr_b = b)
            {
                var lenM4 = a.Length / 4;

                int* ia = (int*)ptr_a;
                int* ib = (int*)ptr_b;
                int i;
                for (i = 0; i < lenM4; i++)
                {
                    if (ia[i] != ib[i]) return false;
                }

                byte* ba = (byte*)(ia + i);
                byte* bb = (byte*)(ib + i);

                var ms = a.Length % 4;
                if(ms != 0)
                {
                    //至少是1
                    if (ba[0] != bb[0]) return false;
                    if(ms > 1)
                    {
                        //2 或 3
                        if (ba[1] != bb[1]) return false;
                        //是3
                        if(ms == 3) if (ba[2] != bb[2]) return false;
                    }
                }
            }
            return true;
        }

        #endregion

    }

}

#if DEBUG

#endif