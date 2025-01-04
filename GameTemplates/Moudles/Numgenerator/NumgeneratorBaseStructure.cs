using Cheng.Algorithm.Randoms;
using System;

namespace Cheng.NumberGenerators
{

    /// <summary>
    /// 一个使用随机数生成器生成数的实例
    /// </summary>
    public class RandomNumgenerator : Numgenerator
    {
        /// <summary>
        /// 随机数生成器
        /// </summary>
        protected BaseRandom pr_random;
        /// <summary>
        /// 生成一个浮点数
        /// </summary>
        public override double Value => pr_random.NextDouble();
        /// <summary>
        /// 生成一个整数
        /// </summary>
        public override long ValueInt64 => pr_random.NextLong();
        /// <summary>
        /// 生成一个整数
        /// </summary>
        public override int ValueInt32 => pr_random.Next();
    }

    /// <summary>
    /// 常量生成器
    /// </summary>
    public class ConstNumgenerator : Numgenerator, IEquatable<ConstNumgenerator>
    {
        /// <summary>
        /// 实例化一个常量生成器
        /// </summary>
        /// <param name="value">常量</param>
        public ConstNumgenerator(double value)
        {
            p_value = value;
            p_intValue = (long)p_value;
        }
        /// <summary>
        /// 实例化一个常量生成器
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intValue"></param>
        public ConstNumgenerator(double value, long intValue)
        {
            p_value = value;
            p_intValue = intValue;
        }
        /// <summary>
        /// 实例化一个常量生成器
        /// </summary>
        /// <param name="intValue">常量</param>
        public ConstNumgenerator(long intValue)
        {
            p_intValue = intValue;
            p_value = intValue;
        }
        private double p_value;
        private long p_intValue;
        /// <summary>
        /// 返回此实例的常量
        /// </summary>
        public override double Value => p_value;

        public override long ValueInt64 => p_intValue;
        public override int ValueInt32 => (int)p_intValue;

        public static explicit operator ConstNumgenerator(double value)
        {
            return new ConstNumgenerator(value);
        }
        public static implicit operator double(ConstNumgenerator cn)
        {
            return cn.p_value;
        }

        public static bool operator ==(ConstNumgenerator c1, ConstNumgenerator c2)
        {
            if (c1 == (object)c2) return true;
            if (c1 is null || c2 is null) return false;
            return c1.Equals(c2);
        }
        public static bool operator !=(ConstNumgenerator c1, ConstNumgenerator c2)
        {
            if (c1 == (object)c2) return false;
            if (c1 is null || c2 is null) return true;
            return !c1.Equals(c2);
        }
        /// <summary>
        /// 返回此实例的常量
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_value.ToString();
        }
        public virtual bool Equals(ConstNumgenerator other)
        {
            if (other is null) return false;
            return this.p_value == other.p_value && this.p_intValue == other.p_intValue;
        }
        public override bool Equals(object obj)
        {
            if(obj is ConstNumgenerator)
            {
                return Equals((ConstNumgenerator)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return p_value.GetHashCode() ^ p_intValue.GetHashCode();
        }
    }

    /// <summary>
    /// 指定范围生成器，返回一个指定范围内的随机数
    /// </summary>
    public class RangeNumgenerator : RandomNumgenerator
    {
        /// <summary>
        /// 实例化一个指定范围生成器
        /// </summary>
        /// <param name="min">指定最小值常量</param>
        /// <param name="max">指定最大值常量</param>
        /// <param name="random">指定用于生成数的随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public RangeNumgenerator(double min, double max, BaseRandom random)
        {
            pr_random = random ?? throw new ArgumentNullException();
            p_min = new ConstNumgenerator(min);
            p_max = new ConstNumgenerator(max);
        }
        /// <summary>
        /// 实例化一个指定范围生成器
        /// </summary>
        /// <param name="min">指定最小值数生成器</param>
        /// <param name="max">指定最大值数生成器</param>
        /// <param name="random">指定用于生成数的随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public RangeNumgenerator(Numgenerator min, Numgenerator max, BaseRandom random)
        {
            if(min is null || max is null || random is null) throw new ArgumentNullException();
            pr_random = random;
            p_min = min;
            p_max = max;
        }

        /// <summary>
        /// 最小值生成器
        /// </summary>
        protected Numgenerator p_min;

        /// <summary>
        /// 最大值生成器
        /// </summary>
        protected Numgenerator p_max;

        /// <summary>
        /// 生成一个最小值
        /// </summary>
        /// <remarks>
        /// 注意：获取该属性的返回值可能会造成内部生成器的变动或修改
        /// </remarks>
        protected virtual double Min
        {
            get => p_min.Value;
        }

        /// <summary>
        /// 生成一个最大值
        /// </summary>
        /// <remarks>
        /// 注意：获取该属性的返回值可能会造成内部生成器的变动或修改
        /// </remarks>
        protected virtual double Max
        {
            get => p_max.Value;
        }

        /// <summary>
        /// 生成并返回一个随机数，范围在[<see cref="Min"/>,<see cref="Max"/>]
        /// </summary>
        public override double Value
        {
            get
            {
                var min = Min;
                var max = Max;
                return pr_random.NextDouble(min, max + 1e-5d);
            }
        }

        /// <summary>
        /// 生成并返回一个随机数
        /// </summary>
        public override int ValueInt32
        {
            get
            {
                int min = p_min.ValueInt32;
                int max = p_max.ValueInt32;
                return pr_random.Next(min, max + 1);
            }
        }

        /// <summary>
        /// 生成并返回一个随机数
        /// </summary>
        public override long ValueInt64
        {
            get
            {
                long min = p_min.ValueInt64;
                long max = p_max.ValueInt64;
                return pr_random.NextLong(min, max + 1);
            }
        }

        /// <summary>
        /// 生成并返回一个数，范围在[<paramref name="min"/>,<paramref name="max"/>]
        /// </summary>
        /// <param name="min">此次生成时的最小值</param>
        /// <param name="max">此次生成时的最大值</param>
        /// <returns>范围在[<paramref name="min"/>,<paramref name="max"/>]的数</returns>
        public virtual double Generator(out double min, out double max)
        {
            min = Min;
            max = Max;
            return pr_random.NextDouble(min, max + 1e-5d);
        }

    }

    /// <summary>
    /// 伯努利概率数生成器
    /// </summary>
    public class BernoulliNumgenerator : RandomNumgenerator
    {

        #region 构造

        /// <summary>
        /// 实例化一个伯努利概率数生成器
        /// </summary>
        /// <param name="p">指定每次实验的成功概率，范围在[0,1]，常量</param>
        /// <param name="n">指定每次实验的次数，常量</param>
        /// <param name="random">指定计算概率时需要的随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BernoulliNumgenerator(double p, long n, BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            BernoullInit(new ConstNumgenerator(p), new ConstNumgenerator(n), random);
        }

        /// <summary>
        /// 实例化一个伯努利概率数生成器
        /// </summary>
        /// <param name="p">指定每次实验的成功概率，范围在[0,1]</param>
        /// <param name="n">指定每次实验的次数</param>
        /// <param name="random">指定计算概率时需要的随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BernoulliNumgenerator(Numgenerator p, Numgenerator n, BaseRandom random)
        {
            if (p is null || n is null || random is null) throw new ArgumentNullException();
            BernoullInit(p, n, random);
        }

        /// <summary>
        /// 在构造方法例调用以初始化参数，参数不得为null。没有安全检查
        /// </summary>
        /// <param name="p">指定每次实验的成功概率，范围在[0,1]</param>
        /// <param name="n">指定每次实验的次数</param>
        /// <param name="random">指定计算概率时需要的随机数生成器</param>
        protected void BernoullInit(Numgenerator p, Numgenerator n, BaseRandom random)
        {
            pr_random = random;
            p_p = p;
            p_n = n;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 实验的成功概率参数
        /// </summary>
        protected Numgenerator p_p;

        /// <summary>
        /// 每次实验的次数参数
        /// </summary>
        protected Numgenerator p_n;

        #endregion

        #region 派生

        /// <summary>
        /// 使用伯努利数值计算并返回伯努利概率
        /// </summary>
        public override double Value
        {
            get => GeneratorProbablily;
        }

        /// <summary>
        /// 使用伯努利数值计算并返回伯努利成功数
        /// </summary>
        public override long ValueInt64
        {
            get => GeneratorCount;
        }

        /// <summary>
        /// 使用伯努利数值计算并返回伯努利成功数
        /// </summary>
        public override int ValueInt32 => (int)ValueInt64;

        /// <summary>
        /// 使用伯努利数值计算并返回伯努利成功数
        /// </summary>
        protected long GeneratorCount
        {
            get
            {
                double p = GetBernoulliPar(out long n);
                long count = 0;
                long i;

                for(i = 0; i < n; i++)
                {
                    if (pr_random.NextDouble(p)) count++;
                }

                return count;
            }
        }

        /// <summary>
        /// 使用伯努利数值计算并返回伯努利概率
        /// </summary>
        public double GeneratorProbablily
        {
            get
            {
                double p = GetBernoulliPar(out long n);
                long count = 0;
                long i;

                for (i = 0; i < n; i++)
                {
                    if (pr_random.NextDouble(p)) count++;
                }

                return (count / (double)n);
            }
        }

        #endregion

        #region 功能

        /// <summary>
        /// 调用该函数获取此次要计算的伯努利公式的参数
        /// </summary>
        /// <param name="n">获取此次的实验次数</param>
        /// <returns>获取此次的实验成功概率</returns>
        protected virtual double GetBernoulliPar(out long n)
        {
            n = p_n.ValueInt64;
            return p_p.Value;
        }

        /// <summary>
        /// 使用伯努利数值计算并返回结果
        /// </summary>
        /// <param name="count">伯努利成功数</param>
        /// <returns>伯努利概率</returns>
        public double Generator(out long count)
        {
            double p = GetBernoulliPar(out long n);
            count = 0;
            long i;

            for (i = 0; i < n; i++)
            {
                if (pr_random.NextDouble(p)) count++;
            }

            return (count / (double)n);
        }

        #endregion

    }

    /// <summary>
    /// 强制单类型生成器
    /// </summary>
    public class OnceNumGenerator : Numgenerator
    {
        public enum NumType
        {
            Int32,
            Int64,
            Double
        }

        #region 构造
        /// <summary>
        /// 实例化单数值生成器
        /// </summary>
        /// <param name="num">数生成器</param>
        /// <param name="type">指定此生成器封装的数生成类型</param>
        /// <exception cref="ArgumentNullException">生成器不能为null</exception>
        public OnceNumGenerator(Numgenerator num, NumType type)
        {
            p_num = num ?? throw new ArgumentNullException();
            p_type = type;
        }
        #endregion

        #region 参数

        private Numgenerator p_num;
        private NumType p_type;

        #endregion

        #region 派生

        public override double Value
        {
            get
            {
                switch (p_type)
                {
                    case NumType.Int32:
                        return p_num.ValueInt32;
                    case NumType.Int64:
                        return p_num.ValueInt64;
                    case NumType.Double:
                        return p_num.Value;
                }
                throw new ArgumentException();
            }
        }

        public override int ValueInt32
        {
            get
            {
                switch (p_type)
                {
                    case NumType.Int32:
                        return p_num.ValueInt32;
                    case NumType.Int64:
                        return (int)p_num.ValueInt64;
                    case NumType.Double:
                        return (int)p_num.Value;
                }
                throw new ArgumentException();
            }
        }

        public override long ValueInt64
        {
            get
            {
                switch (p_type)
                {
                    case NumType.Int32:
                        return p_num.ValueInt32;
                    case NumType.Int64:
                        return p_num.ValueInt64;
                    case NumType.Double:
                        return (long)p_num.Value;
                }
                throw new ArgumentException();
            }
        }

        public override string ToString()
        {
            switch (p_type)
            {
                case NumType.Int32:
                    return p_num.ValueInt32.ToString();
                case NumType.Int64:
                    return p_num.ValueInt64.ToString();
                case NumType.Double:
                    return p_num.Value.ToString();
            }
            return base.ToString();
        }

        #endregion

    }

    /// <summary>
    /// 计算两个值的生成器
    /// </summary>
    public class ArithmeticNumGenerator : Numgenerator
    {

        /// <summary>
        /// 计算类型
        /// </summary>
        public enum ALCType : byte
        {
            /// <summary>
            /// 加法
            /// </summary>
            Add,
            /// <summary>
            /// 减法
            /// </summary>
            Sub,
            /// <summary>
            /// 乘法
            /// </summary>
            Mult,
            /// <summary>
            /// 除法
            /// </summary>
            Div,
            /// <summary>
            /// 取模
            /// </summary>
            Mod,
            /// <summary>
            /// 按位与（仅整数运算）
            /// </summary>
            AND,
            /// <summary>
            /// 按位或（仅整数运算）
            /// </summary>
            OR,
            /// <summary>
            /// 按位异或（仅整数运算）
            /// </summary>
            XOR,
            /// <summary>
            /// 按位同或（仅整数运算）
            /// </summary>
            NXOR,
            /// <summary>
            /// 按位取反（仅整数运算，取x）
            /// </summary>
            NOT,
        }

        #region 构造

        public ArithmeticNumGenerator(Numgenerator x, Numgenerator y, ALCType type)
        {
            if (x is null || y is null) throw new ArgumentNullException();
            p_x = x;
            p_y = y;
            p_type = type;
        }

        #endregion

        #region 参数
        private Numgenerator p_x;
        private Numgenerator p_y;
        private ALCType p_type;
        #endregion

        #region

        /// <summary>
        /// 访问或设置运算类型
        /// </summary>
        public ALCType Type
        {
            get => p_type;
            set
            {
                p_type = value;
            }
        }

        #endregion

        #region 派生

        /// <summary>
        /// 使用两个值计算一个数
        /// </summary>
        /// <exception cref="ArgumentException">不是有效的计算类型</exception>
        public override double Value
        {
            get
            {
                switch (p_type)
                {
                    case ALCType.Add:
                        return p_x.Value + p_y.Value;
                    case ALCType.Sub:
                        return p_x.Value - p_y.Value;
                    case ALCType.Mult:
                        return p_x.Value * p_y.Value;
                    case ALCType.Div:
                        return p_x.Value / p_y.Value;
                    case ALCType.Mod:
                        return p_x.Value % p_y.Value;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// 使用两个值计算一个数
        /// </summary>
        /// <exception cref="ArgumentException">不是有效的计算类型</exception>
        public override long ValueInt64
        {
            get
            {
                switch (p_type)
                {
                    case ALCType.Add:
                        return p_x.ValueInt64 + p_y.ValueInt64;
                    case ALCType.Sub:
                        return p_x.ValueInt64 - p_y.ValueInt64;
                    case ALCType.Mult:
                        return p_x.ValueInt64 * p_y.ValueInt64;
                    case ALCType.Div:
                        return p_x.ValueInt64 / p_y.ValueInt64;
                    case ALCType.Mod:
                        return p_x.ValueInt64 % p_y.ValueInt64;
                    case ALCType.AND:
                        return p_x.ValueInt64 & p_y.ValueInt64;
                    case ALCType.OR:
                        return p_x.ValueInt64 | p_y.ValueInt64;
                    case ALCType.XOR:
                        return p_x.ValueInt64 ^ p_y.ValueInt64;
                    case ALCType.NXOR:
                        return ~(p_x.ValueInt64 ^ p_y.ValueInt64);
                    case ALCType.NOT:
                        return ~(p_x.ValueInt64);
                    default:
                        throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// 使用两个值计算一个数
        /// </summary>
        /// <exception cref="ArgumentException">不是有效的计算类型</exception>
        public override int ValueInt32
        {
            get
            {
                switch (p_type)
                {
                    case ALCType.Add:
                        return p_x.ValueInt32 + p_y.ValueInt32;
                    case ALCType.Sub:
                        return p_x.ValueInt32 - p_y.ValueInt32;
                    case ALCType.Mult:
                        return p_x.ValueInt32 * p_y.ValueInt32;
                    case ALCType.Div:
                        return p_x.ValueInt32 / p_y.ValueInt32;
                    case ALCType.Mod:
                        return p_x.ValueInt32 % p_y.ValueInt32;
                    case ALCType.AND:
                        return p_x.ValueInt32 & p_y.ValueInt32;
                    case ALCType.OR:
                        return p_x.ValueInt32 | p_y.ValueInt32;
                    case ALCType.XOR:
                        return p_x.ValueInt32 ^ p_y.ValueInt32;
                    case ALCType.NXOR:
                        return ~(p_x.ValueInt32 ^ p_y.ValueInt32);
                    case ALCType.NOT:
                        return ~(p_x.ValueInt32);
                    default:
                        throw new ArgumentException();
                }
            }
        }

        #endregion
    }

}
