using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cheng.Unitys
{

    /// <summary>
    /// 根据平台获取触控或鼠标的状态参数
    /// </summary>
    /// <remarks>
    /// <para>根据平台环境不同，采取不同的方式获取相同参数；在存在鼠标的平台下获取的是鼠标参数，在触控平台下获取的是触控参数</para>
    /// </remarks>
    public static class MouseORTouchState
    {

        #region 功能

        #region 参数访问

        /// <summary>
        /// 触控是否处于屏幕上
        /// </summary>
        /// <returns>
        /// <para>如果屏幕没有任何触控则返回false，有至少一个触控处于屏幕上时返回true</para>
        /// <para>如果平台处于鼠标控制，则永久返回true</para>
        /// </returns>
        public static bool OnScreen
        {
            get
            {
#if HAVE_MOUSE
                return true;
#elif HAVE_TOUCH
                return Input.touchCount > 0;
#endif
            }
        }

        /// <summary>
        /// 触控或鼠标当前所在的屏幕像素坐标
        /// </summary>
        /// <returns>
        /// <para>触控或鼠标所在的屏幕像素坐标；以左下角作为(0,0)</para>
        /// <para>如果是鼠标所在平台，当鼠标处于播放器屏幕外时，返回值范围会超出屏幕</para>
        /// <para>当处于触控所在平台时，如果触控不处于屏幕上，该参数无效；可使用<see cref="OnScreen"/>参数判断触控是否处于屏幕上</para>
        /// </returns>
        public static Vector2 Position
        {
            get
            {
#if HAVE_MOUSE

                return Input.mousePosition;

#elif HAVE_TOUCH

                var tc = Input.touchCount;
                if(tc > 0)
                {
                    return Input.GetTouch(0).position;
                }
                return default;
#endif

            }
        }

        /// <summary>
        /// 鼠标是否在当前帧点击或是否在当前帧触摸
        /// </summary>
        /// <returns>
        /// <para>如果处于鼠标所在平台，返回当前帧是否按下鼠标左键</para>
        /// <para>如果处于触控平台，则返回当前帧是否第一次将触控放置在屏幕上</para>
        /// </returns>
        public static bool ButtonDown
        {
            get
            {

#if HAVE_MOUSE
                return Input.GetMouseButtonDown(0);

#elif HAVE_TOUCH

                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
#endif
            }
        }

        /// <summary>
        /// 鼠标或触控的状态
        /// </summary>
        /// <returns>
        /// <para>当处于鼠标所在平台下，返回当前帧的鼠标左键状态</para>
        /// <para>当处于触控平台下时，只要触控处于屏幕上，就会返回true；在触控离开后返回false</para>
        /// </returns>
        public static bool ButtonState
        {
            get
            {
#if HAVE_MOUSE
                return Input.GetMouseButton(0);
#elif HAVE_TOUCH
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
#endif
            }
        }

        /// <summary>
        /// 鼠标或触控是否在当前帧抬起
        /// </summary>
        /// <returns>
        /// <para>当处于鼠标所在平台下，返回当前帧是否松开鼠标左键</para>
        /// <para>当处于触控平台下时，当前帧如果最后一个触控离开屏幕，返回true；否则返回false</para>
        /// </returns>
        public static bool ButtonUp
        {
            get
            {
#if HAVE_MOUSE
                return Input.GetMouseButtonUp(0);
#elif HAVE_TOUCH
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Ended:
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
#endif

            }
        }

        #endregion

        #endregion

    }

}
