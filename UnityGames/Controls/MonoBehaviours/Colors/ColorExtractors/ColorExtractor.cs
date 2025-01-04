using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Cheng.Unitys.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Cheng.Unitys.ColorExtractors
{

    /// <summary>
    /// Unity颜色提取器
    /// </summary>
    /// <remarks>
    /// </remarks>
    [DisallowMultipleComponent]
    public class ColorExtractor : MonoBehaviour
    {

        #region 构造

        #endregion

        #region 参数

        #region 外部参数

#if UNITY_EDITOR
        [Tooltip("当前提取器内的颜色")]
#endif
        [SerializeField] protected Color color;

#if UNITY_EDITOR
        [Tooltip("色相提取器")]
#endif
        [SerializeField] private ColorHueExtraclor hueExtraclor;


#if UNITY_EDITOR
        [Tooltip("饱和度与亮度提取器")]
#endif
        [SerializeField] private ColorSVExtraclor svExtraclor;

#if UNITY_EDITOR
        [Tooltip("颜色发生改变时引发的事件")]
#endif
        [SerializeField] private UnityEvent<Color> changeColorEvent;

#if UNITY_EDITOR
        [Tooltip("是否开启Unity事件引发\r\n若该参数为true，则在引发事件时执行Unity持久注册事件，参数为false则不会引发Unity 编辑器内注册的持久性对象事件；\r\n关闭该参数能够提高性能")]
#endif
        [SerializeField] protected bool startUnityEvent;

        #endregion

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置当前提取到的颜色
        /// </summary>
        public virtual Color Color
        {
            get => color;
            set
            {
                color = value;
                Color.RGBToHSV(color, out float h, out float s, out float v);
                hueExtraclor.OnlySetHue(h);
                svExtraclor.OnlySetSV(s, v);
                InvokeChangeColorEvent(color);
            }
        }

        /// <summary>
        /// 是否开启Unity事件引发
        /// </summary>
        /// <value>
        /// <para>若该参数为true，则在引发事件时执行Unity持久注册事件，参数为false则不会引发Unity 编辑器内注册的持久性对象事件</para>
        /// <para>设为false能够提高性能</para>
        /// </value>
        public virtual bool StartUnityEvent
        {
            get => startUnityEvent;
            set
            {
                startUnityEvent = value;
            }
        }

        /// <summary>
        /// 访问或设置色相提取器
        /// </summary>
        public ColorHueExtraclor HueExtraclor
        {
            get => hueExtraclor;
            set
            {
                if(hueExtraclor is object) hueExtraclor.ChangeHueEvent -= fe_changeHueEvent;
                hueExtraclor = value;
                if (hueExtraclor is object) hueExtraclor.ChangeHueEvent += fe_changeHueEvent;
            }
        }

        /// <summary>
        /// 访问或设置饱和度和亮度提取器
        /// </summary>
        public ColorSVExtraclor SVExtraclor
        {
            get => svExtraclor;
            set
            {
                if(svExtraclor is object) svExtraclor.ChangeValueEvent -= fe_changeValueEvent;
                svExtraclor = value;
                if (hueExtraclor is object) svExtraclor.ChangeValueEvent += fe_changeValueEvent;
            }
        }

        /// <summary>
        /// 仅设置颜色参数
        /// </summary>
        /// <param name="color"></param>
        public void OnlySetColor(Color color)
        {
            this.color = color;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 颜色发生改变时引发的Unity事件实例
        /// </summary>
        public UnityEvent<Color> ChangeColorUnityEvent
        {
            get => changeColorEvent;
        }

        /// <summary>
        /// 颜色发生改变时引发的事件
        /// </summary>
        public event UnityAction<Color> ChangeColorEvent;

        /// <summary>
        /// 调用该函数以引发颜色改变事件
        /// </summary>
        /// <param name="color">改变后的颜色</param>
        protected void InvokeChangeColorEvent(Color color)
        {
            ChangeColorEvent?.Invoke(color);
            if (startUnityEvent) changeColorEvent.Invoke(color);
        }

        #endregion

        #endregion

        #region 运行

        #region 事件注册

        private void fe_changeValueEvent(float s, float v)
        {
            color = UnityEngine.Color.HSVToRGB(hueExtraclor.Hue, s, v);
            InvokeChangeColorEvent(color);
        }

        private void fe_changeHueEvent(float hue)
        {
            color = UnityEngine.Color.HSVToRGB(hue, svExtraclor.Saturation, svExtraclor.Value);
            InvokeChangeColorEvent(color);
        }
        #endregion

        /// <summary>
        /// 在派生类重写该函数时需调用基实现
        /// </summary>
        protected virtual void Awake()
        {

            if(hueExtraclor) hueExtraclor.ChangeHueEvent += fe_changeHueEvent;
            if (svExtraclor) svExtraclor.ChangeValueEvent += fe_changeValueEvent;

        }

        /// <summary>
        /// 在派生类重写该函数时需调用基实现
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (hueExtraclor) hueExtraclor.ChangeHueEvent -= fe_changeHueEvent;
            if (svExtraclor) svExtraclor.ChangeValueEvent -= fe_changeValueEvent;
            hueExtraclor = null;
            svExtraclor = null;
            //changeColorEvent.RemoveAllListeners();
            changeColorEvent = null;
        }

        #endregion

    }


}
