using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.DataStructure
{

    /// <summary>
    /// 附条件的对象转换委托
    /// </summary>
    /// <typeparam name="TIn">要输入的对象类型</typeparam>
    /// <typeparam name="TOut">转换后的对象类型</typeparam>
    /// <param name="obj">要转换的对象</param>
    /// <param name="result">转换后的对象，仅在返回值是true时有效</param>
    /// <returns>如果成功转换，返回true并将转换后的结果赋值到<paramref name="result"/>；如果无法转换返回false</returns>
    public delegate bool ConvertByCondition<in TIn, TOut>(TIn obj, out TOut result);

}