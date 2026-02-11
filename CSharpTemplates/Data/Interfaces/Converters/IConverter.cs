using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 转换对象的公共接口
    /// </summary>
    /// <typeparam name="TIn">要输入的对象类型</typeparam>
    /// <typeparam name="TOut">转换后的对象类型</typeparam>
    public interface IConverter<in TIn, out TOut>
    {

        /// <summary>
        /// 将<typeparamref name="TIn"/>对象转换为<typeparamref name="TOut"/>对象
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的对象</returns>
        TOut Convert(TIn obj);
    }

    /// <summary>
    /// 附条件的对象转换公共接口
    /// </summary>
    /// <typeparam name="TIn">要输入的对象类型</typeparam>
    /// <typeparam name="TOut">转换后的对象类型</typeparam>
    public interface IConverterByCondition<in TIn, TOut>
    {

        /// <summary>
        /// 判断<typeparamref name="TIn"/>对象并尝试转换为<typeparamref name="TOut"/>对象
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="result">转换后的对象，仅在返回值是true时有效</param>
        /// <returns>如果成功转换，返回true并将结果赋值到<paramref name="result"/>；如果无法转换返回false</returns>
        bool Convert(TIn obj, out TOut result);
    }

    /// <summary>
    /// 将派生对象转换为另一种类型的公共接口
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface IConverToOther<out TOut>
    {

        /// <summary>
        /// 返回将当前对象转换后的新对象
        /// </summary>
        /// <returns>转换后的新对象</returns>
        TOut ConvertTo();
    }


}