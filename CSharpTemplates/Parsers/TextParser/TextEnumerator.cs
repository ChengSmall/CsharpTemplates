using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 按序枚举文本的枚举器接口
    /// </summary>
    public interface ITextEnumerator
    {

        /// <summary>
        /// 枚举器当前枚举的文本参数，超过范围为null
        /// </summary>
        string CurructText { get; }

        /// <summary>
        /// 向下推进并迭代此次文本
        /// </summary>
        /// <returns>true表示仍然可以推进，false表示已到达结尾无法推进；获取第一个文本前需要调用该函数进行第一次推进</returns>
        bool MoveNext();

        /// <summary>
        /// 该枚举器是否可以重置计数
        /// </summary>
        bool CanReset { get; }

        /// <summary>
        /// 重置计数器到最初
        /// </summary>
        /// <param name="maxCount">指示重置后再次推进时最大推进量，该参数可能影响<see cref="MoveNext"/>函数的返回值；若参数小于0表示无要求，使用枚举器默认设置</param>
        /// <exception cref="NotSupportedException">该枚举器不支持重置计数器功能（参数<see cref="CanReset"/>是false）</exception>
        void Reset(int maxCount);

    }

}
