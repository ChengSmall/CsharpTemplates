using System;
using Cheng.Algorithm.Randoms;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cheng.Json;

namespace Cheng.NumberGenerators
{
    /// <summary>
    /// 表示一个数生成器基类，派生此类定义数生成器
    /// </summary>
    public abstract class Numgenerator
    {
        protected Numgenerator() { }

        #region 参数访问
        /// <summary>
        /// 生成并返回一个数
        /// </summary>
        public abstract double Value { get; }
        /// <summary>
        /// 生成并返回一个64位整数
        /// </summary>
        public virtual long ValueInt64 => (long)Value;
        /// <summary>
        /// 并返回一个32位整数
        /// </summary>
        public virtual int ValueInt32 => (int)Value;
        #endregion

        #region 派生
        /// <summary>
        /// 生成一个数，并以字符串的格式返回
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

    }


}
