using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Cheng.DataStructure
{


    /// <summary>
    /// 条件判断委托
    /// </summary>
    /// <typeparam name="T">类型参数</typeparam>
    /// <param name="t">要判断的对象</param>
    /// <returns>判断结果</returns>
    public delegate bool CostomCondition<in T>(T t);

    /// <summary>
    /// 参数条件判断委托
    /// </summary>
    /// <typeparam name="T">类型参数</typeparam>
    /// <typeparam name="P">判断参数</typeparam>
    /// <param name="t">要判断的对象</param>
    /// <param name="obj">使用的判断参数</param>
    /// <returns>判断结果</returns>
    public delegate bool CostomCondition<in T, in P>(T t, P obj);

}
