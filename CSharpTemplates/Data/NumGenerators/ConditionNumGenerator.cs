using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.DataStructure.NumGenerators
{

    /// <summary>
    /// 条件判断类型；0表示false，非0表示true
    /// </summary>
    public enum ConditionNumGeneratorType : byte
    {

        /// <summary>
        /// 判断相等 x == y
        /// </summary>
        Equal = 1,

        /// <summary>
        /// 判断不相等 x != y
        /// </summary>
        NotEqual,

        /// <summary>
        /// 判断大于 x &gt; y
        /// </summary>
        Greater,

        /// <summary>
        /// 判断小于 x &lt; y
        /// </summary>
        Less,

        /// <summary>
        /// 判断大于等于 x &gt;= y
        /// </summary>
        GreaterEqual,

        /// <summary>
        /// 判断小于等于 x &lt;= y
        /// </summary>
        LessEqual,

        /// <summary>
        /// 判断结果取反
        /// </summary>
        Neg,

        /// <summary>
        /// 判断所有的值全是true（非0）
        /// </summary>
        And,

        /// <summary>
        /// 判断任意一个是true（非0）
        /// </summary>
        Or,

    }

    /// <summary>
    /// 条件判断数值生成器；按指定类型判断条件并根据条件生成值
    /// </summary>
    public sealed class ConditionNumGenerator : NumGenerator
    {

        #region 构造

        /// <summary>
        /// 实例化条件判断数值生成器
        /// </summary>
        /// <param name="type">条件判断类型</param>
        /// <param name="x">左值 x</param>
        /// <param name="y">右值 y</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConditionNumGenerator(ConditionNumGeneratorType type, NumGenerator x, NumGenerator y)
        {
            if (x is null || y is null) throw new ArgumentNullException();
            p_x = x; p_y = y; p_type = type;
        }

        #endregion

        #region 参数

        private NumGenerator p_x;

        private NumGenerator p_y;

        private ConditionNumGeneratorType p_type;

        #endregion

        #region 功能

        public override DynamicNumber Generate()
        {
            var ct = p_type;
            var x = p_x.Generate();

            switch (ct)
            {
                case ConditionNumGeneratorType.Neg:
                    return x == 0 ? 1 : 0;
                default:
                    break;
            }

            var y = p_y.Generate();

            switch (ct)
            {
                case ConditionNumGeneratorType.Equal:
                    return x == y ? 1 : 0;
                case ConditionNumGeneratorType.NotEqual:
                    return x != y ? 1 : 0;
                case ConditionNumGeneratorType.Greater:
                    return x > y ? 1 : 0;
                case ConditionNumGeneratorType.Less:
                    return x < y ? 1 : 0;
                case ConditionNumGeneratorType.GreaterEqual:
                    return x >= y ? 1 : 0;
                case ConditionNumGeneratorType.LessEqual:
                    return x <= y ? 1 : 0;
                case ConditionNumGeneratorType.And:
                    return (x != 0) && (y != 0) ? 1 : 0;
                case ConditionNumGeneratorType.Or:
                    return (x != 0) || (y != 0) ? 1 : 0;
            }

            throw new NotImplementedException();
        }

        #endregion

    }

    /// <summary>
    /// 多项条件判断数值生成器
    /// </summary>
    /// <remarks>
    /// <para>判断多项集合参数，仅适合<see cref="ConditionNumGeneratorType.And"/>, <see cref="ConditionNumGeneratorType.Or"/></para>
    /// </remarks>
    public sealed class ConditionListNumGenerator : NumGenerator
    {

        #region 构造

        /// <summary>
        /// 实例化多项条件判断数值生成器
        /// </summary>
        /// <param name="type">判断类型</param>
        /// <param name="nums">值生成器列表</param>
        public ConditionListNumGenerator(ConditionNumGeneratorType type, IEnumerable<NumGenerator> nums)
        {
            if (nums is null) throw new ArgumentNullException();
            p_type = type;
            p_list = nums;
        }

        /// <summary>
        /// 实例化多项条件判断数值生成器
        /// </summary>
        /// <param name="type">判断类型</param>
        /// <param name="nums">值生成器数组</param>
        public ConditionListNumGenerator(ConditionNumGeneratorType type, params NumGenerator[] nums)
        {
            if (nums is null) throw new ArgumentNullException();
            p_type = type;
            p_list = nums;
        }

        #endregion

        #region 参数

        private IEnumerable<NumGenerator> p_list;

        private ConditionNumGeneratorType p_type;

        #endregion

        #region 功能

        public override DynamicNumber Generate()
        {
            DynamicNumber num;
            switch (p_type)
            {
                case ConditionNumGeneratorType.And:
                    goto AndList;
                case ConditionNumGeneratorType.Or:
                    goto OrList;
                default:
                    throw new NotImplementedException();
            }

            AndList:
            foreach (var ng in p_list)
            {
                if (ng is null) continue;
                num = ng.Generate();
                if(num == 0)
                {
                    // 有一个是false
                    return 0;
                }
            }

            return 1;

            OrList:
            foreach (var ng in p_list)
            {
                if (ng is null) continue;
                num = ng.Generate();
                if (num == 1)
                {
                    // 有一个是true
                    return 1;
                }
            }

            return 0;
        }

        #endregion

    }

}
