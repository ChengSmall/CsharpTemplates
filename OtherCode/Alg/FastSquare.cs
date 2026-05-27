using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode
{

    /// <summary>
    /// 快速平方根倒数
    /// </summary>
    public unsafe class FastSquare
    {
        /// <summary>
        /// 快速平方根倒数之魔数常量
        /// </summary>
        public const int MagicNumber = 0x5F375A86;
        /// <summary>
        /// 快速平方根倒数之魔数常量-双精度版
        /// </summary>
        public const long MagicNumberDouble = 0x5FE6EB50C7AA19F9;

        /// <summary>
        /// 快速平方根倒数
        /// </summary>
        /// <param name="num">要计算的值</param>
        public static float Rsqrt(float num)
        {
            float x = num * 0.5f;

            int i = *(int*)&num;
            i = MagicNumber - (i >> 1);
            num = *(float*)&i;

            num = num * (1.5f - (x * num * num));

            return num;
        }

        /// <summary>
        /// 快速平方根倒数
        /// </summary>
        /// <param name="num">要计算的值</param>
        /// <returns>求出的得数</returns>
        public static double Rsqrt(double num)
        {
            double x = num * 0.5f;

            long i = *(long*)&num;
            i = MagicNumberDouble - (i >> 1);
            num = *(double*)&i;

            num = num * (1.5 - (x * num * num));
            num = num * (1.5 - (x * num * num));

            return num;
        }

    }


}
