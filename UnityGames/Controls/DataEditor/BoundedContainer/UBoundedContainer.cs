using System;
using System.Text;
using UnityEngine;

namespace Cheng.DataStructure.BoundedContainers
{

    /// <summary>
    /// BoundedContainer 结构的 Unity GUI 值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    [Serializable]
    public struct UBoundedContainer<T> : IEquatable<UBoundedContainer<T>> where T : unmanaged, IEquatable<T>
    {

        #region 初始化

        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public UBoundedContainer(T value, T min, T max)
        {
            p_value = value;
            p_min = min;
            p_max = max;
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public UBoundedContainer(T min, T max)
        {
            p_value = min;
            p_min = min;
            p_max = max;
        }

        #endregion

        #region 参数

        [SerializeField] private T p_value;

        [SerializeField] private T p_min;

        [SerializeField] private T p_max;

#if UNITY_EDITOR

        /// <summary>
        /// 值字段名
        /// </summary>
        public const string cp_valueFieldName = nameof(p_value);

        /// <summary>
        /// 最小值字段名
        /// </summary>
        public const string cp_minFieldName = nameof(p_min);

        /// <summary>
        /// 最大值字段名
        /// </summary>
        public const string cp_maxFieldName = nameof(p_max);

#endif

        #endregion

        #region 功能

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public void GetValue(out T value, out T min, out T max)
        {
            value = p_value;
            min = p_min;
            max = p_max;
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public T Value => p_value;

        /// <summary>
        /// 最大值
        /// </summary>
        public T Max => p_max;

        /// <summary>
        /// 最小值
        /// </summary>
        public T Min => p_min;

        public override bool Equals(object obj)
        {
            if(obj is UBoundedContainer<T> other)
            {
                return p_value.Equals(other.p_value) && p_min.Equals(other.p_min) && p_max.Equals(other.p_max);

            }
            return false;
        }

        public bool Equals(UBoundedContainer<T> other)
        {
            return p_value.Equals(other.p_value) && p_min.Equals(other.p_min) && p_max.Equals(other.p_max);
        }

        public override int GetHashCode()
        {
            return p_value.GetHashCode() ^ p_min.GetHashCode() ^ p_max.GetHashCode();
        }

        public override string ToString()
        {
            return p_min.ToString() + " <- " + p_value.ToString() + " -> " + p_max.ToString();
        }

        #endregion

    }

}
