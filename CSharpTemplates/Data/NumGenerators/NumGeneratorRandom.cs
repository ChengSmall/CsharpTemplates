using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Cheng.Algorithm.HashCodes;
using Cheng.Algorithm.Randoms;

namespace Cheng.DataStructure.NumGenerators
{

    /// <summary>
    /// 数值生成器 - 随机数生成
    /// </summary>
    public class NumGeneratorRandom : NumGenerator
    {

        /// <summary>
        /// 实例化一个随机数生成器
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public NumGeneratorRandom(BaseRandom random)
        {
            this.random = random ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 随机数生成器
        /// </summary>
        protected BaseRandom random;

        /// <summary>
        /// 生成一个随机整数
        /// </summary>
        /// <returns>一个随机长整型值，范围在[0,9223372036854775807)</returns>
        public override DynamicNumber Generate()
        {
            return random.NextLong();
        }

    }

    /// <summary>
    /// 数值生成器 - 动态调整随机器方法的随机数生成
    /// </summary>
    public sealed class NumGeneratorDRandom : NumGeneratorRandom
    {

        /// <summary>
        /// 实例化一个随机数生成器
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="type">随机数生成器的生成数值类型</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public NumGeneratorDRandom(BaseRandom random, NumType type) : base(random)
        {
            p_type = type;
        }

        /// <summary>
        /// 实例化一个随机数生成器，采用整数随机数返回结果
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public NumGeneratorDRandom(BaseRandom random) : this(random, NumType.Integer)
        {
        }

        private NumType p_type;

        /// <summary>
        /// 访问或设置随机数生成器要生成的数值类型
        /// </summary>
        public NumType RandomNumType
        {
            get => p_type;
            set
            {
                p_type = value;
            }
        }

        /// <summary>
        /// 生成一个随机数
        /// </summary>
        /// <returns>如果参数<see cref="RandomNumType"/>是<see cref="NumType.Integer"/>则调用<see cref="BaseRandom.NextLong"/>函数返回结果；如果是<see cref="NumType.RealNumber"/>调用<see cref="BaseRandom.NextDouble"/>返回结果</returns>
        /// <exception cref="NotImplementedException">随机数值类型参数错误</exception>
        public override DynamicNumber Generate()
        {
            switch (p_type)
            {
                case NumType.Integer:
                    return new DynamicNumber(random.NextLong());
                case NumType.RealNumber:
                    return new DynamicNumber(random.NextDouble());
                default:
                    throw new NotImplementedException();
            }
        }

    }

    /// <summary>
    /// 数值生成器 - 范围随机数生成
    /// </summary>
    public sealed class NumGeneratorRandomRange : NumGeneratorRandom
    {

        #region 构造

        /// <summary>
        /// 实例化一个范围随机数生成器
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="min">最小值生成器</param>
        /// <param name="max">最大值生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public NumGeneratorRandomRange(BaseRandom random, NumGenerator min, NumGenerator max) : base(random)
        {
            if (min is null || max is null) throw new ArgumentNullException();
            p_max = max;
            p_min = min;
        }

        /// <summary>
        /// 实例化一个范围随机数生成器
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public NumGeneratorRandomRange(BaseRandom random, long min, long max) : base(random)
        {
            p_min = new NumGeneratorValue(min);
            p_max = new NumGeneratorValue(max);
        }

        /// <summary>
        /// 实例化一个范围随机数生成器
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public NumGeneratorRandomRange(BaseRandom random, double min, double max) : base(random)
        {
            p_min = new NumGeneratorValue(min);
            p_max = new NumGeneratorValue(max);
        }

        #endregion

        #region 参数

        private NumGenerator p_min;

        private NumGenerator p_max;

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置最大值生成器
        /// </summary>
        /// <exception cref="ArgumentNullException">设置为null</exception>
        public NumGenerator Min
        {
            get => p_min;
            set
            {
                p_min = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 访问或设置最小值生成器
        /// </summary>
        /// <exception cref="ArgumentNullException">设置为null</exception>
        public NumGenerator Max
        {
            get => p_max;
            set
            {
                p_max = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 生成一个指定范围的随机数
        /// </summary>
        /// <returns>范围在[<see cref="Min"/>,<see cref="Max"/>)之间的随机数；如果数生成器出现<see cref="Min"/>大于或等于<see cref="Max"/>则返回0</returns>
        public override DynamicNumber Generate()
        {
            var min = p_min.Generate();
            var max = p_max.Generate();

            if (min >= max) return 0;

            bool real = min.type == NumType.RealNumber || max.type == NumType.RealNumber;

            if (real)
            {
                
                return random.NextDouble((double)min, (double)max);
            }
            else
            {
                return random.NextLong(min.valueInteger, max.valueInteger);
            }

        }

        #endregion

    }

    /// <summary>
    /// 数值生成器 - 伯努利随机试验
    /// </summary>
    public sealed class NumGeneratorBernoulliRandom : NumGeneratorRandom
    {

        #region 构造

        /// <summary>
        /// 实例化一个伯努利随机试验概率值生成器
        /// </summary>
        /// <param name="n">一次试验的次数生成器</param>
        /// <param name="p">每次试验的成功率生成器</param>
        /// <param name="random">随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NumGeneratorBernoulliRandom(NumGenerator n, NumGenerator p, BaseRandom random) : base(random)
        {
            if (n is null || p is null) throw new ArgumentNullException();
            p_n = n;
            p_p = p;
        }

        /// <summary>
        /// 实例化一个伯努利随机试验概率值生成器
        /// </summary>
        /// <param name="n">一次试验的次数</param>
        /// <param name="p">每次试验的成功率，范围在[0,1]</param>
        /// <param name="random">随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NumGeneratorBernoulliRandom(long n, double p, BaseRandom random) : base(random)
        {
            p_n = new NumGeneratorValue(n);
            p_p = new NumGeneratorValue(p);
        }

        #endregion

        #region 参数

        private NumGenerator p_n;

        private NumGenerator p_p;

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置试验次数数值生成器
        /// </summary>
        /// <exception cref="ArgumentNullException">设置为null</exception>
        public NumGenerator N
        {
            get => p_n;
            set
            {
                p_n = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 访问或设置每次试验的成功率数值生成器
        /// </summary>
        /// <exception cref="ArgumentNullException">设置为null</exception>
        public NumGenerator P
        {
            get => p_p;
            set
            {
                p_p = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 进行一次伯努利试验并返回实验结果
        /// </summary>
        /// <remarks>
        /// <para>使用<see cref="N"/>获取此次试验次数，再使用随机器模拟n次后返回试验结果</para>
        /// </remarks>
        /// <param name="n">此次试验的总次数</param>
        /// <param name="count">成功的次数</param>
        /// <returns>此次试验的成功率</returns>
        /// <exception cref="NotImplementedException">数值生成器出现未知错误</exception>
        public double GenerateBernoulli(out long n, out long count)
        {

            n = (long)p_n.Generate();
            count = 0;

            for (int i = 0; i < n; i++)
            {
                if (random.NextDouble((double)p_p.Generate())) count++;
            }

            return (double)count / (double)n;
        }

        /// <summary>
        /// 采用伯努利试验返回随机值概率
        /// </summary>
        /// <returns>使用<see cref="N"/>获取此次试验次数，再使用随机器模拟n次后返回试验成功率</returns>
        public override DynamicNumber Generate()
        {
            return new DynamicNumber(GenerateBernoulli(out _, out _));
        }

        #endregion

    }

}
