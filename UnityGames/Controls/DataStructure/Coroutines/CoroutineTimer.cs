using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cheng.Unitys
{

    /// <summary>
    /// Unity协程计时器参数
    /// </summary>
    /// <remarks>
    /// <para>一个简易的Unity协程计时器，作为函数<see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>的参数使用</para>
    /// </remarks>
    public sealed class CoroutineEasyTimer : IEnumerator
    {

        #region

        /// <summary>
        /// 实例化一个协程等待枚举器
        /// </summary>
        /// <param name="waitForSeconds">枚举器每次推进后的等待秒数，使用<see cref="UnityEngine.WaitForSeconds"/>作为返回值</param>
        public CoroutineEasyTimer(float waitForSeconds) : this(new WaitForSeconds(waitForSeconds))
        {
        }

        /// <summary>
        /// 实例化一个协程等待枚举器
        /// </summary>
        /// <param name="yieldValue">枚举器每次推进后的返回值</param>
        public CoroutineEasyTimer(object yieldValue)
        {
            p_thread = new object();
            p_yield = yieldValue;
            p_action = null;
            p_start = true;
        }

        #endregion

        #region 参数

        private readonly object p_thread;

        private readonly object p_yield;

        private Action p_action;

        private bool p_start;

        #endregion

        #region

        /// <summary>
        /// 每次推进枚举器时的事件
        /// </summary>
        public event Action CoroutineEvent
        {
            add
            {
                lock (p_thread) p_action += value;
            }
            remove
            {
                lock (p_thread) p_action -= value;
            }
        }

        /// <summary>
        /// 停止枚举器继续推进
        /// </summary>
        public void StopCoroutine()
        {
            p_start = false;
        }

        public object Current => p_yield;

        public bool MoveNext()
        {
            if (p_start)
            {
                p_action?.Invoke();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            p_start = true;
        }

        #endregion

    }

}
