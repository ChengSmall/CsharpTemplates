using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Json
{

    /// <summary>
    /// Json数据不安全扩展
    /// </summary>
    public static class JsonUnsafeExtend
    {

        /// <summary>
        /// 获取json对象内部用于储存数据的字典实例
        /// </summary>
        /// <remarks>
        /// <para>提示：修改内部实例可能会导致未知的异常出现</para>
        /// </remarks>
        /// <param name="json">json对象</param>
        /// <returns>对象内部用于储存数据的字典</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static Dictionary<string, JsonVariable> GetBaseDictionary(this JsonDictionary json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));
            return json.p_dict;
        }

        /// <summary>
        /// 获取json对象内部用于储存数据的集合实例
        /// </summary>
        /// <remarks>
        /// <para>提示：修改内部实例可能会导致未知的异常出现</para>
        /// </remarks>
        /// <param name="json">json对象</param>
        /// <returns>对象内部用于储存数据的集合</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static List<JsonVariable> GetBaseList(this JsonList json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));
            return json.p_list;
        }

    }

}
