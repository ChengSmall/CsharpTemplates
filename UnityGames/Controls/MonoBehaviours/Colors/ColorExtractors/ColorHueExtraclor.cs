using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using Cheng.Unitys.Editors;
#endif

namespace Cheng.Unitys.ColorExtractors
{

    /// <summary>
    /// Unity颜色提取器-色相提取器
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ColorHueExtraclor : MonoBehaviour
    {

        #region 构造

        #endregion

        #region 参数

        #region 外部参数

#if UNITY_EDITOR
        [Tooltip("色相值")]
        #endif
        [Range(0, 1)] [SerializeField] protected float hue;

        #if UNITY_EDITOR
        [Tooltip("色相值发生变动时引发的事件")]
        #endif
        [SerializeField] private UnityEvent<float> changeHueEvent;

        #if UNITY_EDITOR
        [Tooltip("是否开启Unity事件引发\r\n若该参数为true，则在引发事件时执行Unity持久注册事件，参数为false则不会引发Unity 编辑器内注册的持久性对象事件；\r\n关闭该参数能够提高性能")]
        #endif
        [SerializeField] protected bool startUnityEvent;

        #endregion

        #endregion

        #region 功能

        #region 参数访问

        internal void OnlySetHue(float hue)
        {
            this.hue = hue;
        }

        /// <summary>
        /// 访问或设置该色相提取器的色相值
        /// </summary>
        /// <value>范围在[0,1]</value>
        public virtual float Hue
        {
            get => hue;
            set
            {
                if (value == hue) return;

                if (value > 1) hue = 1;
                else if (value < 0) hue = 0;
                else hue = value;

                InvokeChangeHueEvent(hue);
            }
        }

        /// <summary>
        /// 是否开启Unity事件引发
        /// </summary>
        /// <value>
        /// <para>若该参数为true，则在引发事件时执行Unity持久注册事件，参数为false则不会引发Unity 编辑器内注册的持久性对象事件</para>
        /// <para>关闭该参数能够提高性能</para>
        /// </value>
        public virtual bool StartUnityEvent
        {
            get => startUnityEvent;
            set
            {
                startUnityEvent = value;
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 获取色相值发生变动时引发的Unity事件实例
        /// </summary>
        public UnityEvent<float> ChangeHueUnityEvent
        {
            get => changeHueEvent;
        }

        /// <summary>
        /// 色相值发生变动时引发的事件
        /// </summary>
        public event UnityAction<float> ChangeHueEvent;

        /// <summary>
        /// 在色相值变动时调用的事件发生函数
        /// </summary>
        /// <param name="Hue">变动后的色相值</param>
        protected void InvokeChangeHueEvent(float Hue)
        {
            ChangeHueEvent?.Invoke(Hue);
            if (startUnityEvent) changeHueEvent.Invoke(Hue);
        }

        #endregion

        #endregion

        #region 运行

        protected virtual void OnDestroy()
        {
            ChangeHueEvent = null;
            //changeHueEvent.RemoveAllListeners();
            changeHueEvent = null;
        }

        #endregion


    }


}
