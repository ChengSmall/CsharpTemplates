using System;
using System.Collections.Generic;
using System.Collections;

using Cheng.DataStructure.Collections;

namespace Cheng.Json
{

    /// <summary>
    /// 将json对象转化为.net对象的公共接口
    /// </summary>
    public interface IJsonToObject
    {
        /// <summary>
        /// 获取指定索引的可转化类型
        /// </summary>
        /// <param name="index">范围是[0,<see cref="TypeCount"/>)的索引</param>
        /// <returns>可转化类型</returns>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        Type this[int index] { get; }

        /// <summary>
        /// 获取可转化类型的数量，若无法确定转化的类型数量则返回-1
        /// </summary>
        int TypeCount { get; }

        /// <summary>
        /// 将json对象转化为object
        /// </summary>
        /// <param name="json">json对象</param>
        /// <returns>转化后的实例</returns>
        /// <exception cref="InvalidOperationException">转化时出错</exception>
        object ToObject(JsonVariable json);

        /// <summary>
        /// 将对象转化为json对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json对象</returns>
        /// <exception cref="NotImplementedException">转化时出错</exception>
        JsonVariable ToJson(object obj);

        /// <summary>
        /// 将json对象转化为object
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="obj">转化后的实例</param>
        /// <returns>是否转化成功</returns>
        bool ToObject(JsonVariable json, out object obj);

    }

    /// <summary>
    /// 将json对象转化为.net对象的公共基类
    /// </summary>
    public abstract class JsonToObject : IJsonToObject, IReadOnlyList<Type>
    {

        /// <summary>
        /// 获取指定索引的可转化类型
        /// </summary>
        /// <param name="index">范围是[0,<see cref="TypeCount"/>)的索引</param>
        /// <returns>可转化类型</returns>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public abstract Type this[int index] { get; }

        /// <summary>
        /// 获取可转化类型的数量，若无法确定转化的类型数量则返回-1
        /// </summary>
        public abstract int TypeCount { get; }

        int IReadOnlyCollection<Type>.Count => TypeCount;

        /// <summary>
        /// 将对象转化为json对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json对象</returns>
        /// <exception cref="NotImplementedException">转化时出错</exception>
        public abstract JsonVariable ToJson(object obj);

        /// <summary>
        /// 将json对象转化为object
        /// </summary>
        /// <param name="json">json对象</param>
        /// <returns>转化后的实例</returns>
        /// <exception cref="InvalidOperationException">转化时出错</exception>
        public abstract object ToObject(JsonVariable json);

        /// <summary>
        /// 将json对象转化为object
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="obj">转化后的实例</param>
        /// <returns>是否转化成功</returns>
        public virtual bool ToObject(JsonVariable json, out object obj)
        {
            try
            {
                obj = ToObject(json);
                return true;
            }
            catch (Exception)
            {
                obj = null;
                return false;
            }
        }

        /// <summary>
        /// 访问该对象所有可转化的类型枚举器
        /// </summary>
        /// <returns>一个枚举器，包含该对象所有可转化的类型</returns>
        public virtual IEnumerator<Type> GetEnumerator()
        {
            for (int i = 0; i < TypeCount; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
