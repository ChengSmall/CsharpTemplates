using System.Collections.Generic;
using System;
namespace Cheng.DataStructure
{

    /// <summary>
    /// 可以对指定类型自定义名称的映射表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisplayNames<T>
    {
        /// <summary>
        /// 实例化一个自定义类型名称映射表
        /// </summary>
        /// <param name="displayNames">映射表</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public DisplayNames(IDictionary<T, string> displayNames)
        {
            p_displayNames = displayNames ?? throw new ArgumentNullException();
        }
        /// <summary>
        /// 无参空构造
        /// </summary>
        protected DisplayNames()
        {
        }

        /// <summary>
        /// 一个自定义名称映射字典
        /// </summary>
        protected IDictionary<T, string> p_displayNames;

        /// <summary>
        /// 返回给定对象的自定义名称
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns>给定对象的自定义名称，若字典内没有映射名称则返回默认字符串</returns>
        public virtual string ToName(T value)
        {
            if (p_displayNames.TryGetValue(value, out string str)) return str;
            return value?.ToString();
        }

    }

}
