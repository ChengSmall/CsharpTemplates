using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 可映射多个 <see cref="KeyCode"/> 枚举值的按钮
    /// </summary>
    [Serializable]
    public sealed class MultKeyCodeButton : UnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个空的多键按钮
        /// </summary>
        public MultKeyCodeButton()
        {
            p_keys = new List<KeyCode>();
            p_isAnd = false;
        }

        /// <summary>
        /// 实例化一个多键按钮
        /// </summary>
        /// <param name="keys">表示按钮的集合</param>
        public MultKeyCodeButton(IEnumerable<KeyCode> keys)
        {
            p_keys = new List<KeyCode>(keys);
            p_isAnd = false;
        }

        #endregion

        #region 参数

        [SerializeField]
        private List<KeyCode> p_keys;
        [SerializeField]
        private bool p_isAnd;

#if UNITY_EDITOR
        /// <summary>
        /// 按键集合的字段名称
        /// </summary>
        public const string cp_keyArrText = nameof(p_keys);
        /// <summary>
        /// 映射方法的布尔值字段名称
        /// </summary>
        public const string cp_isAndText = nameof(p_isAnd);
#endif

        #endregion

        #region 功能

        #region 数组功能

        /// <summary>
        /// 使用索引访问或设置映射的键码
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public KeyCode this[int index]
        {
            get => p_keys[index];
            set
            {
                p_keys[index] = value;
            }
        }

        /// <summary>
        /// 当前映射的键码数量
        /// </summary>
        public int Count => p_keys.Count;

        /// <summary>
        /// 获取要映射的键码集合
        /// </summary>
        public List<KeyCode> Keys
        {
            get => p_keys;
        }

        #endregion

        #region 按钮功能

        #region 派生

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                return UnityButtonAvailablePromissions |
                 ButtonAvailablePermissions.CanGetState |
                 ButtonAvailablePermissions.CanGetChangeFrameButtonUp |
                 ButtonAvailablePermissions.CanGetChangeFrameButtonDown |
                 ButtonAvailablePermissions.AllGetPowerPermissions;
            }
        }

        /// <summary>
        /// 使用<see cref="Input.GetKey(KeyCode)"/>映射多个键码
        /// </summary>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override bool ButtonState
        {
            get
            {
                int i;
                int length = p_keys.Count;

                if (p_isAnd)
                {

                    for (i = 0; i < length; i++)
                    {
                        //无则为false
                        if (!Input.GetKey(p_keys[i])) return false;
                    }
                    //全有则true
                    return true;
                }

                for (i = 0; i < length; i++)
                {
                    //有则为true
                    if (Input.GetKey(p_keys[i])) return true;
                }
                //全无则false
                return false;
            }
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 使用<see cref="Input.GetKeyDown(KeyCode)"/>映射多个键码
        /// </summary>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override bool ButtonDown
        {
            get
            {
                int i;
                int length = p_keys.Count;

                if (p_isAnd)
                {

                    for (i = 0; i < length; i++)
                    {
                        //无则为false
                        if (!Input.GetKeyDown(p_keys[i])) return false;
                    }
                    //全有则true
                    return true;
                }

                for (i = 0; i < length; i++)
                {
                    //有则为true
                    if (Input.GetKeyDown(p_keys[i])) return true;
                }
                //全无则false
                return false;
            }
        }

        /// <summary>
        /// 使用<see cref="Input.GetKeyUp(KeyCode)"/>映射多个键码
        /// </summary>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override bool ButtonUp
        {
            get
            {
                int i;
                int length = p_keys.Count;

                if (p_isAnd)
                {

                    for (i = 0; i < length; i++)
                    {
                        //无则为false
                        if (!Input.GetKeyUp(p_keys[i])) return false;
                    }
                    //全有则true
                    return true;
                }

                for (i = 0; i < length; i++)
                {
                    //有则为true
                    if (Input.GetKeyUp(p_keys[i])) return true;
                }
                //全无则false
                return false;
            }
        }

        /// <summary>
        /// 使用按钮状态映射力度值，false表示0，true表示1
        /// </summary>
        public override float Power
        {
            get
            {
                return ButtonState ? 1f : 0;
            }
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 最大值为1
        /// </summary>
        public override float MaxPower 
        {
            get => 1; 
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 最小值为0
        /// </summary>
        public override float MinPower
        {
            get => 0;
            set => ThrowSupportedException();
        }

        #endregion

        /// <summary>
        /// 访问或设置按钮状态的映射模式
        /// </summary>
        /// <value>
        /// 值设置为false时，任意一个键码为true则返回true；设置为true时，仅当所有键码都为true时返回true
        /// </value>
        public bool IsAndState
        {
            get => p_isAnd;
            set => p_isAnd = value;
        }

        #endregion

        #endregion

    }

}
