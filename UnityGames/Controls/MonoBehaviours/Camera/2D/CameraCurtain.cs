using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Cheng.Algorithm;

using UObj = UnityEngine.Object;
using GObj = UnityEngine.GameObject;

namespace Cheng.Unitys.Cameras
{

    /// <summary>
    /// 2D摄像机幕布渐变遮罩
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/2D/渲染/幕布遮罩")]
#endif
    [DisallowMultipleComponent]
    public sealed class CameraCurtain : MonoBehaviour
    {

        #region 构造

        public CameraCurtain()
        {
        }

        #endregion

        #region 参数

        #region 外部参数

//#if UNITY_EDITOR
//        [Tooltip("要管理的摄像机")]
//#endif
//        [SerializeField] private Camera nowCamera = null; 我tm是傻逼，放了个没用的参数在这儿

#if UNITY_EDITOR
        [Tooltip("摄像机幕布渲染器")]
#endif
        [SerializeField] private SpriteRenderer curtainRenderer = null;

        #endregion

        #region 内部参数

        private Coroutine p_nowColoutime = null;

        #endregion

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 摄像机幕布渲染器
        /// </summary>
        /// <value>
        /// <para>该参数需要是一个能够完全遮盖摄像机画面的图像渲染器，用于在启动遮罩时使用该参数遮盖摄像机</para>
        /// </value>
        public SpriteRenderer CurtainRenderer
        {
            get => curtainRenderer;
            set
            {
                if (value == null) curtainRenderer = null;
                else curtainRenderer = value;
            }
        }

        /// <summary>
        /// 幕布是否正在罩住摄像机
        /// </summary>
        /// <returns>如果幕布正在工作，或者u3d没有在运行，则为null</returns>
        public bool? IsCloseCurtain
        {
            get
            {
                if(p_nowColoutime == null) return curtainRenderer?.enabled;
                return null;
            }
        }

        #endregion

        #region 摄像机幕布

        /// <summary>
        /// 启动幕布逐渐罩拢摄像机前盖
        /// </summary>
        /// <param name="sec">逐步罩拢的时间间隔，单位秒</param>
        /// <param name="closeColor">罩拢后的颜色</param>
        public void StartCloseCamera(float sec, Color closeColor)
        {
            if(p_nowColoutime != null)
            {
                StopCoroutine(p_nowColoutime);
            }
            p_nowColoutime = StartCoroutine(f_startClose(sec, closeColor).GetEnumerator());
        }

        /// <summary>
        /// 启动纯黑色幕布逐渐罩拢摄像机前盖
        /// </summary>
        /// <param name="sec">逐步罩拢的时间间隔，单位秒</param>
        public void StartCloseCamera(float sec)
        {
            if (p_nowColoutime != null)
            {
                StopCoroutine(p_nowColoutime);
            }
            p_nowColoutime = StartCoroutine(f_startClose(sec, Color.black).GetEnumerator());
        }

        /// <summary>
        /// 逐渐打开摄像机幕布
        /// </summary>
        /// <param name="sec">逐步打开的间隔</param>
        public void StartOpenCamera(float sec)
        {
            if (p_nowColoutime != null)
            {
                StopCoroutine(p_nowColoutime);
            }
            p_nowColoutime = StartCoroutine(f_startOpen(sec).GetEnumerator());
        }

        private IEnumerable f_startClose(float sec, Color closeColor)
        {
            //当前时间
            float nowTime = Time.unscaledTime;
            //结束时间
            //float overTime = nowTime + sec;
            //将颜色完全透明
            Color tmc = closeColor;
            //打开幕布对象
            //curtain.SetActive(true);
            curtainRenderer.enabled = true;
            tmc.a = 0;
            curtainRenderer.color = tmc;

            while (true)
            {
                //时间差
                yield return null;
                var tc = Time.unscaledTime - nowTime;
                if (tc >= sec) break;
                //间隔累加
                //var dt = Time.unscaledDeltaTime;
                //count += dt;
                
                //根据时间间隔累加设置透明度
                var a = Maths.Clamp((tc / sec), 0, 1);
                tmc = curtainRenderer.color;
                tmc.a = a;
                curtainRenderer.color = tmc;

            }
            //完毕
            tmc = closeColor;
            tmc.a = 1;
            curtainRenderer.color = tmc;
            p_nowColoutime = null;
        }

        private IEnumerable f_startOpen(float sec)
        {
            //当前时间
            float nowTime = Time.unscaledTime;
            //结束时间
            //float overTime = nowTime + sec;
            //设置颜色完全不透明
            Color tmc = curtainRenderer.color;
            tmc.a = 1;
            //curtainRenderer.color = tmc;
            //打开幕布对象
            //curtain.SetActive(true);
            //float count = 0;

            while (true)
            {
                yield return null;
                var tc = Time.unscaledTime - nowTime;
                if (tc >= sec) break;

                //间隔累加
                //var dt = Time.unscaledDeltaTime;
                //count += dt;
                //根据时间间隔累加设置透明度
               
                var a = Maths.Clamp(1f - (tc / sec), 0, 1);
                tmc = curtainRenderer.color;
                tmc.a = a;
                curtainRenderer.color = tmc;
                

            }
            //完毕
            //tmc = openColor;
            //tmc.a = 0;
            //curtainRenderer.color = tmc;
            //关闭幕布对象
            curtainRenderer.enabled = false;
            p_nowColoutime = null;
        }

        #endregion

        #endregion

        #region 运行

        private void Awake()
        {
            p_nowColoutime = null;
        }

        private void OnDestroy()
        {
            curtainRenderer = null;
            //nowCamera = null;
        }

        private void OnValidate()
        {
            if (curtainRenderer == null)
            {
                curtainRenderer = null;
            }
            //if (nowCamera == null)
            //{
            //    nowCamera = null;
            //}
        }

        #endregion

    }
}
#if UNITY_EDITOR

#endif