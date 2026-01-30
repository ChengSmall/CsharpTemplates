using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Cheng.Memorys;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 用于测试的一组函数和占位符
    /// </summary>
    public static class TestFunc
    {

        /// <summary>
        /// 返回值自身
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T RethisFunc<T>(T value) => value;

    }

}
