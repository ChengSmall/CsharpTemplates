using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Cheng.Unitys.Cameras;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Cheng.Unitys.UI
{

    /// <summary>
    /// UI脚本-画布事件捕获
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasEvent : MonoBehaviour
    {

        #region 构造
        public CanvasEvent()
        {
            p_buffer4 = new Vector3[4];
        }
        #endregion

        #region 参数

        #region 外部参数

        #endregion

        #region 内部参数
        private Canvas p_canvas;
        private RectTransform p_canvasTrans;
        private Vector3[] p_buffer4;

        /// <summary>
        /// 鼠标位置
        /// </summary>
        private Vector2 p_mousePosition;
        /// <summary>
        /// 鼠标上一帧位置
        /// </summary>
        private Vector2 p_lateFrameMousePosition;
        /// <summary>
        /// 当前帧鼠标移动增量
        /// </summary>
        private Vector3 p_frameMouseMove;
        #endregion

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 当前帧鼠标所在的画布坐标位置
        /// </summary>
        /// <returns>以左下角为原点，画布的2D变换坐标</returns>
        public Vector2 MousePosition
        {
            get => p_mousePosition;
        }
        /// <summary>
        /// 上一帧鼠标所在位置
        /// </summary>
        /// <returns>以左下角为原点，画布的2D变换坐标</returns>
        public Vector2 PreviousMousePosition
        {
            get => p_lateFrameMousePosition;
        }
        /// <summary>
        /// 当前帧鼠标的移动路程
        /// </summary>
        public Vector2 MouseDistance
        {
            get => p_frameMouseMove;
        }

        #endregion

        #endregion

        #region 封装

        private Camera f_getEventCamera()
        {
            var mode = p_canvas.renderMode;

            switch (mode)
            {
                case RenderMode.ScreenSpaceOverlay:
                case RenderMode.ScreenSpaceCamera:
                    return p_canvas.worldCamera;
            }
            return Camera.current;
        }

        #endregion

        #region 运行

        private void Awake()
        {
            p_canvas = GetComponent<Canvas>();
            p_canvasTrans = p_canvas.GetComponent<RectTransform>();
        }

        private void Start()
        {
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {

            f_mousePosUpdate();

        }

        private void f_mousePosUpdate()
        {

            var mPixelPos = Input.mousePosition;
            var camera = f_getEventCamera();
            var rpos = camera.PixelToTransform(mPixelPos, p_canvasTrans, p_buffer4);
            p_lateFrameMousePosition = p_mousePosition;
            p_mousePosition = rpos;
            p_frameMouseMove = p_mousePosition - p_lateFrameMousePosition;

        }

        #endregion

    }

}
