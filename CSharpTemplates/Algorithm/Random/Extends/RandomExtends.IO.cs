using System;
using System.IO;
using System.Security.Cryptography;

using Cheng.Algorithm;
using Cheng.DataStructure;
using Cheng.Memorys;

using Cheng.Algorithm.Collections;

using RangenIO = System.Security.Cryptography.RandomNumberGenerator;

namespace Cheng.Algorithm.Randoms.Extends
{

    unsafe partial class RandomExtends
    {

        /// <summary>
        /// 从<see cref="System.Security.Cryptography.RandomNumberGenerator.Create"/>获取随机器并生成随机数的字节数组
        /// </summary>
        /// <param name="size">要生成的字节数量</param>
        /// <returns>包含随机数的字节数组</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public static byte[] CryptographyRandomBytes(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException();
            if (size == 0) return Array.Empty<byte>();
            byte[] buf = new byte[size];
            using (var rangen = RangenIO.Create())
            {
                rangen.GetBytes(buf);
            }
            return buf;
        }

    }

}