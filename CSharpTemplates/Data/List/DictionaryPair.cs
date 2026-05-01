using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 字典的键值对
    /// </summary>
    /// <typeparam name="TK">键的类型</typeparam>
    /// <typeparam name="TV">值的类型</typeparam>
    public readonly struct DictionaryPair<TK, TV>
    {

        /// <summary>
        /// 初始化键值对结构
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public DictionaryPair(in TK key, in TV value)
        {
            this.key = key; this.value = value;
        }

        /// <summary>
        /// 初始化键值对结构
        /// </summary>
        /// <param name="pair">官方的键值对结构</param>
        public DictionaryPair(in KeyValuePair<TK, TV> pair)
        {
            key = pair.Key; value = pair.Value;
        }

        /// <summary>
        /// 键
        /// </summary>
        public readonly TK key;

        /// <summary>
        /// 值
        /// </summary>
        public readonly TV value;

        /// <summary>
        /// 使用的字符串表示形式返回键和值
        /// </summary>
        /// <returns>字符串表示形式的键值对</returns>
        public override string ToString()
        {
            return "[" + key?.ToString() + ", " + value?.ToString() + "]";
        }

        /// <summary>
        /// 显式转化到键值对结构
        /// </summary>
        /// <param name="pair"></param>
        public static explicit operator DictionaryPair<TK, TV>(in KeyValuePair<TK, TV> pair)
        {
            return new DictionaryPair<TK, TV>(in pair);
        }

        /// <summary>
        /// 隐式转换到官方键值对结构
        /// </summary>
        /// <param name="pair"></param>
        public static implicit operator KeyValuePair<TK, TV>(in DictionaryPair<TK, TV> pair)
        {
            return new KeyValuePair<TK, TV>(pair.key, pair.value);
        }

    }

}