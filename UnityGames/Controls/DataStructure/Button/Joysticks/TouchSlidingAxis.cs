using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

    /// <summary>
    /// 触摸滑动轴
    /// </summary>
    /// <remarks>
    /// <para>能够访问用手指在屏幕上滑动的速度</para>
    /// <para>使用<see cref="Speed"/>参数设置或访问滑动速率值，速率值决定最终访问到的摇杆参数的大小；当<see cref="Speed"/>设置为（1，1）时，摇杆速度表示为 像素/秒 </para>
    /// <para>
    /// 访问的总是第一个触控参数，也就是<see cref="Input.GetTouch(int)"/>函数参数为0的返回值
    /// </para>
    /// <para>可在 Unity 编辑器中设置速率值</para>
    /// </remarks>
    [Serializable]
    public sealed class TouchSlidingAxis : BaseJoystick
    {

        #region 构造

        /// <summary>
        /// 实例化一个触摸滑动轴
        /// </summary>
        public TouchSlidingAxis()
        {
            p_speed = new Vector2(0, 0);
        }

        /// <summary>
        /// 实例化一个触摸滑动轴
        /// </summary>
        /// <param name="speed">滑动速率值</param>
        public TouchSlidingAxis(Vector2 speed)
        {
            p_speed = speed;
        }

        /// <summary>
        /// 实例化一个触摸滑动轴
        /// </summary>
        /// <param name="horizontalSpeed">横轴滑动速率值</param>
        /// <param name="verticalSpeed">纵轴滑动速率值</param>
        public TouchSlidingAxis(float horizontalSpeed, float verticalSpeed)
        {
            p_speed = new Vector2(horizontalSpeed, verticalSpeed);
        }

        #endregion

        #region 参数

        [SerializeField] private Vector2 p_speed;

        #region 编辑器参数

#if UNITY_EDITOR

        public const string fieldName_speed = nameof(p_speed);

#endif

        #endregion

        #endregion

        #region 功能

        #region 权限重写

        public override bool CanGetHorizontalComponent
        {
            get
            {
                return Input.touchSupported;
            }
        }

        public override bool CanGetVerticalComponent
        {
            get
            {
                return Input.touchSupported;
            }
        }

        public override bool CanGetVector
        {
            get
            {
                return Input.touchSupported;
            }
        }


        public override bool CanGetHorizontalReverse => true;

        public override bool CanSetHorizontalReverse => true;

        public override bool CanGetVerticalReverse => true;

        public override bool CanSetVerticalReverse => true;

        #endregion

        #region 派生

        public sealed override float Horizontal
        { 
            get
            {
                var tc = Input.touchCount;

                if (tc == 0) return 0;

                //获取
                var touch = Input.GetTouch(0);

                //计算每秒移速值

                var delPos = touch.deltaPosition;
                var delTime = touch.deltaTime;
                
                return (delPos.x * delTime) * p_speed.x;
            }
        }

        public sealed override float Vertical
        {
            get
            {
                var tc = Input.touchCount;

                if (tc == 0) return 0;

                //获取
                var touch = Input.GetTouch(0);

                //计算每秒移速值

                var delPos = touch.deltaPosition;
                var delTime = touch.deltaTime;

                return (delPos.y * delTime) * p_speed.y;
            }
        }

        public sealed override void GetAxis(out float horizontal, out float vertical)
        {
            var tc = Input.touchCount;

            if (tc == 0)
            {
                horizontal = 0;
                vertical = 0;
                return;
            }

            //获取
            var touch = Input.GetTouch(0);

            //计算每秒移速值

            var delPos = (touch.deltaPosition * touch.deltaTime) * p_speed;

            horizontal = delPos.x;
            vertical = delPos.y;
        }

        public sealed override bool IsHorizontalReverse 
        {
            get => p_speed.x >= 0;
            set
            {
                float v = p_speed.x;

                if (value)
                {
                    //取反
                    if(v > 0)
                    {
                        v = -v;
                    }
                }
                else
                {
                    //不取反
                    if(v < 0)
                    {
                        v = -v;
                    }
                }

                p_speed.x = v;
            }
        }

        public sealed override bool IsVerticalReverse 
        {
            get => p_speed.y >= 0;
            set
            {
                float v = p_speed.y;

                if (value)
                {
                    //取反
                    if (v > 0)
                    {
                        v = -v;
                    }
                }
                else
                {
                    //不取反
                    if (v < 0)
                    {
                        v = -v;
                    }
                }

                p_speed.y = v;
            }
        }

        public override void GetVector(out float radian, out float length)
        {
            GetAxis(out float x, out float y);
            BaseJoystick.GetVectorRadionAndLength(x, y, out radian, out length);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 获取触控移动速率
        /// </summary>
        /// <returns>
        /// <para>第一个触控数据的移动速率；由此次移动的像素距离和<see cref="Speed"/>参数相乘所得</para>
        /// </returns>
        public Vector2 TouchAxis
        {
            get
            {
                var tc = Input.touchCount;

                if (tc == 0)
                {
                    return new Vector2(0, 0);
                }

                //获取
                var touch = Input.GetTouch(0);

                //计算每秒移速值

                return (touch.deltaPosition * touch.deltaTime) * p_speed;
            }
        }

        /// <summary>
        /// 触摸滑动速率
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <value>
        /// <para>
        /// 一个<see cref="Vector2"/>结构，x表示横向滑动速率，y表示竖向滑动速率；
        /// </para>
        /// <para>
        /// 如果x或y值小于0，则此值所在的滑动轴参数将会取反；<br/>
        /// 也可以设置<see cref="IsHorizontalReverse"/>和<see cref="IsVerticalReverse"/>这两个参数控制是否取反，但其内部也是以修改该参数来呈现效果；
        /// </para>
        /// <para>默认值为（1，1）</para>
        /// </value>
        public Vector2 Speed
        {
            get => p_speed;
            set
            {
                p_speed = value;
            }
        }

        #endregion

        #endregion

    }

}

