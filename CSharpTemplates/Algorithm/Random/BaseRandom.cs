using Cheng.Memorys;
using System;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 表示一个生成随机数列的随机数生成器基类
    /// </summary>
    public abstract class BaseRandom
    {

        #region 结构

        /// <summary>
        /// 随机数生成器的种子值类型
        /// </summary>
        [Flags]
        public enum SeedType : byte
        {
            /// <summary>
            /// 没有种子值类型
            /// </summary>
            None = 0,
            /// <summary>
            /// 32位整型
            /// </summary>
            Int32 = 1,
            /// <summary>
            /// 32位无符号整型
            /// </summary>
            UInt32 = 0b10,
            /// <summary>
            /// 64位整型
            /// </summary>
            Int64 = 0b100,
            /// <summary>
            /// 64位无符号整型
            /// </summary>
            UInt64 = 0b1000
        }

        #endregion

        #region 权限

        /// <summary>
        /// 是否允许获取当前随机数生成器的随机种子
        /// </summary>
        public virtual bool CanGetSeed => false;

        /// <summary>
        /// 是否允许设置当前随机数生成器的随机种子
        /// </summary>
        public virtual bool CanSetSeed => false;

        /// <summary>
        /// 获取当前随机数生成器内的种子值类型，无法获取则为<see cref="SeedType.None"/>
        /// </summary>
        public virtual SeedType SeedValueType => SeedType.None;

        #endregion

        #region 参数

        /// <summary>
        /// 获取或设置当前随机数种子
        /// </summary>
        /// <exception cref="NotSupportedException">没有权限或者种子不包含此类型</exception>
        public virtual int SeedInt32
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置当前随机数种子
        /// </summary>
        /// <exception cref="NotSupportedException">没有权限或者种子不包含此类型</exception>
        public virtual long SeedInt64
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置当前随机数种子
        /// </summary>
        /// <exception cref="NotSupportedException">没有权限或者种子不包含此类型</exception>
        public virtual int SeedUInt32
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置当前随机数种子
        /// </summary>
        /// <exception cref="NotSupportedException">没有权限或者种子不包含此类型</exception>
        public virtual ulong SeedUInt64
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        #endregion

        #region API

        /// <summary>
        /// 获取一个随机整数
        /// </summary>
        /// <returns>一个随机整数，范围在[0, 2147483647)</returns>
        public abstract int Next();

        /// <summary>
        /// 获取一个范围在[<paramref name="min"/>,<paramref name="max"/>)的随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">不超过或等于此值</param>
        /// <returns>一个指定范围的随机数</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数min大于或等于max</exception>
        public virtual int Next(int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + (Next() % (max - min));
        }

        /// <summary>
        /// 获取一个随机长整型值
        /// </summary>
        /// <returns>一个随机长整型值，范围在[0,9223372036854775807)</returns>
        public virtual long NextLong()
        {
            /*
            00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000
            00000000_00000000 00000000_00000000_00000000 00000000_00000000_00000000
            */
            ulong n1 = (ulong)(Next() % 0b1_00000000_00000000_00000000);
            ulong n2 = (ulong)(Next() % 0b1_00000000_00000000_00000000);
            ulong n3 = (ulong)(Next() % 0b1_00000000_00000000);

            n1 = ((n3 << (8 * 6)) | (n2 << (8 * 3)) | (n1));
            return (long)(n1 % long.MaxValue);
        }

        /// <summary>
        /// 获取一个范围在[<paramref name="min"/>,<paramref name="max"/>)的随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">不超过或等于此值</param>
        /// <returns>一个指定范围的随机数</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数min大于或等于max</exception>
        public virtual long NextLong(long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + (NextLong() % (max - min));
        }

        /// <summary>
        /// 获取一个范围在[0,1)的双浮点随机数
        /// </summary>
        /// <returns>范围在[0,1)的双浮点随机数</returns>
        public virtual double NextDouble()
        {
            return Next() / 2147483647d;
        }

        /// <summary>
        /// 获取一个范围在[0,1)的单浮点随机数
        /// </summary>
        /// <returns>范围在[0,1)的单浮点随机数</returns>
        public virtual float NextFloat()
        {
            return Next(0, 8388607) / 8388607F;
        }

        /// <summary>
        /// 获取一个指定范围的单浮点数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">不超过该值</param>
        /// <returns>一个范围在[<paramref name="min"/>,<paramref name="max"/>)的单浮点数</returns>
        /// <exception cref="ArgumentException">参数min大于max</exception>
        public virtual float NextFloat(float min, float max)
        {
            if (min > max) throw new ArgumentOutOfRangeException();
            return (float)(min + ((max - min) * NextDouble()));
        }

        /// <summary>
        /// 获取一个指定范围的双浮点数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">不超过该值</param>
        /// <returns>一个范围在[<paramref name="min"/>,<paramref name="max"/>)的双浮点数</returns>
        /// <exception cref="ArgumentException">参数min大于max</exception>
        public virtual double NextDouble(double min, double max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + ((max - min) * (NextDouble()));
        }

        /// <summary>
        /// 用随机数填充指定字节数组的元素。
        /// </summary>
        /// <param name="buffer">待填充的字节数组</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual unsafe void NextBytes(byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();
            fixed (byte* ptr = buffer)
            {
                NextPtr(new IntPtr(ptr), buffer.Length);
            }
        }

        /// <summary>
        /// 用随机数填充指定字节数组的元素。
        /// </summary>
        /// <param name="buffer">待填充的字节数组</param>
        /// <param name="offset">待填充字节数组的起始位置</param>
        /// <param name="count">要填充的字节数量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual unsafe void NextBytes(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (count + offset > buffer.Length)) throw new ArgumentOutOfRangeException();
            fixed (byte* ptr = buffer)
            {
                NextPtr(new IntPtr(ptr + offset), count);
            }
        }

        /// <summary>
        /// 指定一个概率参数，判断并返回此次生成器是否小于该参数
        /// </summary>
        /// <param name="probability">一个概率参数</param>
        /// <returns>
        /// 生成器生成的参数是否小于概率参数，参数越接近1返回true的概率越大；当参数大于或等于1时，返回值永远为true；参数小于0时，返回值永远为false；
        /// </returns>
        public virtual bool NextFloat(float probability)
        {
            if (probability < 0) return false;
            if (probability >= 1) return true;
            return NextFloat() < probability;
        }

        /// <summary>
        /// 指定一个概率参数，判断并返回此次生成器是否小于该参数
        /// </summary>
        /// <param name="probability">一个概率参数</param>
        /// <returns>
        /// 生成器生成的参数是否小于概率参数，参数越接近1返回true的概率越大；当参数大于或等于1时，返回值永远为true；参数小于0时，返回值永远为false；
        /// </returns>
        public virtual bool NextDouble(double probability)
        {
            if (probability < 0) return false;
            if (probability >= 1) return true;
            return NextDouble() < probability;
        }

        /// <summary>
        /// 用随机数填充指定内存中的每一个字节
        /// </summary>
        /// <param name="ptr">指向字节数据的指针</param>
        /// <param name="length">要填充的字节长度</param>
        public virtual unsafe void NextPtr(IntPtr ptr, int length)
        {
            if (length == 0) return;
            int len = length / 2;
            ushort* buffer = (ushort*)ptr;
            int i;
            for (i = 0; i < len; i++)
            {
                buffer[i] = (ushort)Next(0, ushort.MaxValue + 1);
            }
            if((length % 2) != 0)
            {
                *((byte*)(buffer + i)) = (byte)Next(0, byte.MaxValue + 1);
            }
        }

        #endregion

    }

}