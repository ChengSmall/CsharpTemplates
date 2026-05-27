using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UObj = UnityEngine.Object;
using GObj = UnityEngine.GameObject;

namespace Cheng.Unitys.DataStructure
{

    /// <summary>
    /// 可在 Inspector 禁用的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct Disabledable<T> : IEquatable<Disabledable<T>> where T : UObj
    {

        #region 构造

        public Disabledable(T obj, bool active)
        {
            if (obj == null) throw new ArgumentNullException();
            p_obj = obj;
            p_active = active;
        }

        public Disabledable(T obj)
        {
            if (obj == null) throw new ArgumentNullException();
            p_obj = obj;
            p_active = true;
        }

        #endregion

        #region 参数

        [SerializeField] private T p_obj;
        [SerializeField] private bool p_active;

#if UNITY_EDITOR

        /// <summary>
        /// 字段名称 - Unity对象
        /// </summary>
        public const string FieldName_object = nameof(p_obj);

        /// <summary>
        /// 字段名称 - 真值开关
        /// </summary>
        public const string FieldName_activeBoolean = nameof(p_active);

#endif

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 是否启用对象
        /// </summary>
        public bool Active
        {
            get => p_active;
            set
            {
                p_active = value;
            }
        }

        /// <summary>
        /// 获取启用的对象
        /// </summary>
        /// <returns>如果<see cref="Active"/>参数是true则返回对象，参数是false返回null</returns>
        public T Value
        {
            get
            {
                if(p_active) return p_obj;
                return null;
            }
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转为对象，根据启用参数返回对象或null
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator T(Disabledable<T> value)
        {
            if (value.p_active) return value.p_obj;
            return null;
        }

        /// <summary>
        /// 判断对象实例是否存在并转换为可禁用对象
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Disabledable<T>(T value)
        {
            if (value)
            {
                return new Disabledable<T>(value, true);
            }
            return new Disabledable<T>(value, false);
        }

        #endregion

        #region 派生

        public static bool operator ==(Disabledable<T> v1, Disabledable<T> v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Disabledable<T> v1, Disabledable<T> v2)
        {
            return !(v1.Equals(v2));
        }

        public override string ToString()
        {
            if (p_active && p_obj != null) p_obj.ToString();
            return string.Empty;
        }

        public bool Equals(Disabledable<T> other)
        {
            if (p_active)
            {
                if (other.p_active)
                {
                    return p_obj == other.p_obj;
                }
                return false;
            }
            else
            {
                if (other.p_active) return false;
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is Disabledable<T> uobj)
            {
                return Equals(uobj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if(p_active) return (p_obj == null) ? 0 : p_obj.GetHashCode();
            return 0;
        }

        #endregion

        #endregion

    }


}
