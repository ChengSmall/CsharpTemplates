using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cheng.Unitys.ColorExtractors
{

    /// <summary>
    /// Unity颜色提取器-饱和度与亮度提取器
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ColorSVExtraclor : MonoBehaviour
    {

        #region 构造

        #endregion

        #region 参数

        #region 外部参数

#if UNITY_EDITOR
        [Tooltip("饱和度")]
#endif
        [SerializeField, Range(0, 1)] protected float saturation;

#if UNITY_EDITOR
        [Tooltip("亮度")]
#endif
        [SerializeField, Range(0, 1)] protected float value;


#if UNITY_EDITOR
        [Tooltip("饱和度和亮度改变时引发的事件")]
#endif
        [SerializeField] private UnityEvent<float, float> changeValueEvent;


#if UNITY_EDITOR
        [Tooltip("是否开启Unity事件引发\r\n若该参数为true，则在引发事件时执行Unity持久注册事件，参数为false则不会引发Unity 编辑器内注册的持久性对象事件；\r\n关闭该参数能够提高性能")]
#endif
        [SerializeField] protected bool startUnityEvent;


        #endregion

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置饱和度
        /// </summary>
        /// <value>值范围在[0,1]</value>
        public virtual float Saturation
        {
            get => saturation;
            set
            {
                if (value == saturation) return;

                if (value < 0) saturation = 0;
                else if (value > 1) saturation = 1;
                else saturation = value;

                InvokeChangeValueEvent(saturation, this.value);
            }
        }

        /// <summary>
        /// 访问或设置亮度值
        /// </summary>
        /// <value>值范围在[0,1]</value>
        public virtual float Value
        {
            get => value;

            set
            {
                if (value == this.value) return;

                if (value < 0) this.value = 0;
                else if (value > 1) this.value = 1;
                else this.value = value;

                InvokeChangeValueEvent(saturation, this.value);
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

        internal void OnlySetSV(float s, float v)
        {
            this.saturation = s;
            this.value = v;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 获取饱和度和亮度改变时引发的Unity事件
        /// </summary>
        /// <remarks>事件函数，参数1表示饱和度，参数2表示亮度</remarks>
        public UnityEvent<float, float> ChangeValueUnityEvent
        {
            get => changeValueEvent;
        }

        /// <summary>
        /// 饱和度和亮度改变时引发的事件
        /// </summary>
        /// <remarks>参数1表示饱和度，参数2表示亮度</remarks>
        public event UnityAction<float, float> ChangeValueEvent;

        /// <summary>
        /// 调用该函数引发饱和度和亮度改变时的事件
        /// </summary>
        /// <param name="saturation">改变后的饱和度</param>
        /// <param name="value">改变后的亮度</param>
        protected void InvokeChangeValueEvent(float saturation, float value)
        {
            //Vector2 v = new Vector2(saturation, value);

            ChangeValueEvent?.Invoke(saturation, value);
            if(startUnityEvent) changeValueEvent.Invoke(saturation, value);
        }

        #endregion

        #endregion

        #region 运行

        protected virtual void OnDestroy()
        {
            ChangeValueEvent = null;
            changeValueEvent = null;
        }

        #endregion


    }

}
