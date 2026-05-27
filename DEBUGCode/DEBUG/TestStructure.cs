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

        /// <summary>
        /// 返回true的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool RetrunTrueFunc<T>(T value) => true;

        /// <summary>
        /// 返回false的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool RetrunFalseFunc<T>(T value) => true;

        /// <summary>
        /// 判断参数是否不为null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>如果参数不为null返回true</returns>
        public static bool IsNotNullFunc<T>(T obj) where T : class
        {
            return obj != null;
        }

    }

}
