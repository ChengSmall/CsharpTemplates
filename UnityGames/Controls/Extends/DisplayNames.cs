using Cheng.DataStructure;
using Cheng.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cheng.Unitys
{

    /// <summary>
    /// 自定义 <see cref="KeyCode"/> 枚举名称的json映射表扩展
    /// </summary>
    public static class DisplayNames
    {

        #region json扩展

        /// <summary>
        /// 将json对象转化到名称映射表
        /// </summary>
        /// <param name="json">
        /// 要转化的json对象；
        /// <para>
        /// json对象必须是一个键值对集合，且每一个key对应一个名称，key为<see cref="KeyCode"/>枚举值中的常量标识符，value表示每一个枚举值对应的自定义名称
        /// </para>
        /// </param>
        /// <returns>转化到的键值对集合，key表示对象，value为对象名称</returns>
        /// <exception cref="ArgumentException">json参数格式不正确</exception>
        public static DisplayNames<KeyCode> JsonToKeyCodeDisplayName(this JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));

            if (json.DataType != JsonType.Dictionary) throw new ArgumentException("json对象类型不是Dictionary");

            return new DisplayNames<KeyCode>(fs_toJsonDict<KeyCode>(json.JsonObject));
        }

        /// <summary>
        /// 将json对象转化到枚举类型名称映射表
        /// </summary>
        /// <typeparam name="T">对象的枚举类型</typeparam>
        /// <param name="json">
        /// 要转化的json对象；
        /// <para>
        /// json对象必须是一个键值对集合，且每一个key对应一个名称，key为<see cref="T"/>枚举值中的常量标识符，value表示每一个枚举值对应的自定义名称
        /// </para>
        /// </param>
        /// <returns>转化到的键值对集合，key表示对象，value为对象名称</returns>
        /// <exception cref="ArgumentException">json参数格式不正确</exception>
        public static DisplayNames<T> JsonToDisplayName<T>(this JsonVariable json) where T : struct, global::System.Enum
        {
            
            if (json is null) throw new ArgumentNullException(nameof(json));

            if (json.DataType != JsonType.Dictionary) throw new ArgumentException("json对象类型不是Dictionary");

            return new DisplayNames<T>(fs_toJsonDict<T>((JsonDictionary)json));
        }


        static Dictionary<T, string> fs_toJsonDict<T>(JsonDictionary json) where T : struct, global::System.Enum
        {
            
            T key;
            Dictionary<T, string> d = new Dictionary<T, string>(32);

            foreach (var item in json)
            {

                if (Enum.TryParse(item.Key, out key) && (item.Value.DataType == JsonType.String))
                {
                    d[key] = item.Value.String;
                }

            }

            return d;
        }

        #endregion

    }
}
