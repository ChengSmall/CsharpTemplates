using System;
using UnityEngine;

namespace Cheng.DataStructure.Receptacles
{

    /// <summary>
    /// Receptacle容器的 Unity 转存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    [Serializable]
    public struct ReceptacleValue<T> : IEquatable<ReceptacleValue<T>> where T : unmanaged, IEquatable<T>
    {

        #region 构造

        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="value">值</param>
        public ReceptacleValue(T value)
        {
            this.value = value;
            maxValue = value;
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        public ReceptacleValue(T value, T maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 值
        /// </summary>
        public T value;

        /// <summary>
        /// 最大值
        /// </summary>
        public T maxValue;

#if UNITY_EDITOR
        
        public const string guiName_value = nameof(value);

        public const string guiName_maxValue = nameof(maxValue);

#endif

        #endregion

        #region 功能

        public override bool Equals(object obj)
        {
            if(obj is ReceptacleValue<T> r)
            {
                return value.Equals(r.value) && maxValue.Equals(r.maxValue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() ^ maxValue.GetHashCode();
        }

        /// <summary>
        /// 以字符串格式返回容器值
        /// </summary>
        /// <returns>value/maxValue</returns>
        public override string ToString()
        {
            return value.ToString() + "/" + maxValue.ToString();
        }

        public bool Equals(ReceptacleValue<T> other)
        {
            return value.Equals(this.value) && maxValue.Equals(this.maxValue);
        }

        #endregion

    }

}
