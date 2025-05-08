using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Cheng.Algorithm.HashCodes;

using DNum = Cheng.DataStructure.NumGenerators.DynamicNumber;

namespace Cheng.DataStructure.NumGenerators
{

    /// <summary>
    /// 数值生成器基类
    /// </summary>
    public abstract class NumGenerator
    {

        #region 功能

        /// <summary>
        /// 生成一个值
        /// </summary>
        /// <returns>生成的值</returns>
        /// <exception cref="NotImplementedException">生成值的过程中出现未知错误</exception>
        public abstract DynamicNumber Generate();

        /// <summary>
        /// 创建一个定值生成器
        /// </summary>
        /// <param name="num">要生产的定值</param>
        /// <returns>定值生成器</returns>
        public static NumGenerator CreateValue(DynamicNumber num)
        {
            return new NumGeneratorValue(num);
        }

        #endregion

    }

    /// <summary>
    /// 数值生成器 - 生成值
    /// </summary>
    public sealed class NumGeneratorValue : NumGenerator
    {

        /// <summary>
        /// 实例化常量值生成器
        /// </summary>
        /// <param name="value">要生成的值</param>
        public NumGeneratorValue(long value)
        {
            number = new DNum(value);
        }

        /// <summary>
        /// 实例化常量值生成器
        /// </summary>
        /// <param name="value">要生成的值</param>
        public NumGeneratorValue(double value)
        {
            number = new DNum(value);
        }

        /// <summary>
        /// 实例化常量值生成器
        /// </summary>
        /// <param name="num">要生成的值</param>
        public NumGeneratorValue(DNum num)
        {
            number = num;
        }

        /// <summary>
        /// 生成器每次生成的值
        /// </summary>
        public DNum number;

        public override DNum Generate()
        {
            return number;
        }

    }

}
