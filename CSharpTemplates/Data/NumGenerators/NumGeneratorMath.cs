using System;

namespace Cheng.DataStructure.NumGenerators
{

    /// <summary>
    /// 数值生成器 - 算术运算
    /// </summary>
    public sealed class NumGeneratorMath : NumGenerator
    {

        #region 结构

        /// <summary>
        /// 算数运算类型
        /// </summary>
        public enum OperationType : byte
        {
            /// <summary>
            /// 加法运算
            /// </summary>
            Add,

            /// <summary>
            /// 减法运算 x作为被减数 y作为减数
            /// </summary>
            Sub,

            /// <summary>
            /// 乘法运算
            /// </summary>
            Mult,

            /// <summary>
            /// 除法运算 x作为被除数 y作为除数
            /// </summary>
            Dev,

            /// <summary>
            /// 求余数 x作为被除数 y作为除数
            /// </summary>
            Mod,

            /// <summary>
            /// 将x值取反（忽略y值）
            /// </summary>
            Neg,

            /// <summary>
            /// 幂运算 x的y次方
            /// </summary>
            Pow,

            /// <summary>
            /// 将 x 开根（忽略y值）
            /// </summary>
            Sqrt

        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化算术运算数值生成器
        /// </summary>
        /// <param name="type">指定运算类型</param>
        /// <param name="x">x值生成器</param>
        /// <param name="y">y值生成器</param>
        /// <exception cref="ArgumentNullException">生成器是null</exception>
        public NumGeneratorMath(OperationType type, NumGenerator x, NumGenerator y)
        {
            if (x is null || y is null) throw new ArgumentNullException();
            p_x = x;
            p_y = y;
            this.operationType = type;
        }

        /// <summary>
        /// 实例化算术运算数值生成器，默认使用加法运算
        /// </summary>
        /// <param name="x">x值生成器</param>
        /// <param name="y">y值生成器</param>
        /// <exception cref="ArgumentNullException">生成器是null</exception>
        public NumGeneratorMath(NumGenerator x, NumGenerator y) : this(OperationType.Add, x, y)
        {
        }

        #endregion

        #region 参数

        private NumGenerator p_x;

        private NumGenerator p_y;

        private OperationType operationType;

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置算术运算的 x 值
        /// </summary>
        /// <exception cref="ArgumentNullException">参数设为null</exception>
        public NumGenerator X
        {
            get => p_x;
            set
            {
                p_x = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 访问或设置算术运算的 y 值
        /// </summary>
        /// <exception cref="ArgumentNullException">参数设为null</exception>
        public NumGenerator Y
        {
            get => p_x;
            set
            {
                p_x = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 访问或设置算术运算类型
        /// </summary>
        public OperationType ArcOperationType
        {
            get => operationType;
            set
            {
                operationType = value;
            }
        }

        public override DynamicNumber Generate()
        {
            var x = p_x.Generate();

            switch (operationType)
            {
                case OperationType.Neg:
                    return -x;
                case OperationType.Sqrt:
                    return Math.Sqrt((double)x);
                default:
                    break;
            }
            if (operationType == OperationType.Sqrt)
            {
                return Math.Sqrt((double)x);
            }


            var y = p_y.Generate();

            switch (operationType)
            {
                case OperationType.Add:
                    return x + y;
                case OperationType.Sub:
                    return x - y;
                case OperationType.Mult:
                    return x * y;
                case OperationType.Dev:
                    return x / y;
                case OperationType.Mod:
                    return x % y;
                case OperationType.Pow:
                    return Math.Pow((double)x, (double)y);
                default:
                    throw new NotImplementedException();
            }




        }

        #endregion

    }

}
