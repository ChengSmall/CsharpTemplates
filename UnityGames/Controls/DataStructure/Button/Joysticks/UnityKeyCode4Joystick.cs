using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using Cheng.Algorithm;

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

    /// <summary>
    /// 使用4个<see cref="KeyCode"/>按键作为4个方向键的摇杆
    /// </summary>
    /// <remarks>
    /// <para>可从 Unity Inspector 中修改4个按键参数和两轴反转开关</para>
    /// </remarks>
    [Serializable]
    public sealed class UnityKeyCode4Joystick : BaseJoystick
    {

        #region 构造
        /// <summary>
        /// 实例化四向按键摇杆
        /// </summary>
        public UnityKeyCode4Joystick()
        {
            p_left = default;
            p_right = default;
            p_up = default;
            p_down = default;
            p_horRev = false;
            p_verRev = false;
        }

        /// <summary>
        /// 实例化四向按键摇杆
        /// </summary>
        /// <param name="left">表示向左的按键</param>
        /// <param name="right">表示向右的按键</param>
        /// <param name="down">表示向下的按键</param>
        /// <param name="up">表示向上的按键</param>
        public UnityKeyCode4Joystick(KeyCode left, KeyCode right, KeyCode down, KeyCode up)
        {
            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
            p_horRev = false;
            p_verRev = false;
        }

        /// <summary>
        /// 实例化四向按键摇杆
        /// </summary>
        /// <param name="left">表示向左的按键</param>
        /// <param name="right">表示向右的按键</param>
        /// <param name="down">表示向下的按键</param>
        /// <param name="up">表示向上的按键</param>
        /// <param name="isHorizontalReverse">横轴是否反转操作</param>
        /// <param name="isVerticalReverse">纵轴是否反转操作</param>
        public UnityKeyCode4Joystick(KeyCode left, KeyCode right, KeyCode down, KeyCode up, bool isHorizontalReverse, bool isVerticalReverse)
        {
            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
            p_horRev = isHorizontalReverse;
            p_verRev = isVerticalReverse;
        }

        #region 静态
        static float f_getSqrt0_5()
        {
            return (float)System.Math.Sqrt(0.5);
        }
        #endregion

        #endregion

        #region 参数

        private static readonly float cp_sqrt0_5 = f_getSqrt0_5();

        [SerializeField] private KeyCode p_left;

        [SerializeField] private KeyCode p_right;

        [SerializeField] private KeyCode p_down;

        [SerializeField] private KeyCode p_up;
        
        [SerializeField] private bool p_horRev;
      
        [SerializeField] private bool p_verRev;  

        #region Editor

#if UNITY_EDITOR

        /// <summary>
        /// 左移按键字段名称
        /// </summary>
        public const string cp_leftFieldName = nameof(p_left);

        /// <summary>
        /// 右移按键字段名称
        /// </summary>
        public const string cp_rightFieldName = nameof(p_right);

        /// <summary>
        /// 下移按键字段名称
        /// </summary>
        public const string cp_downFieldName = nameof(p_down);

        /// <summary>
        /// 上移按键字段名称
        /// </summary>
        public const string cp_upFieldName = nameof(p_up);

        /// <summary>
        /// 横轴反转开关字段名称
        /// </summary>
        public const string cp_horRevFieldName = nameof(p_horRev);

        /// <summary>
        /// 纵轴反转开关字段名称
        /// </summary>
        public const string cp_verRevFieldName = nameof(p_verRev);

#endif

        #endregion

        #endregion

        #region 参数访问

        /// <summary>
        /// 访问或设置表示向左的按键
        /// </summary>
        public KeyCode LeftKey
        {
            get => p_left;
            set => p_left = value;
        }

        /// <summary>
        /// 访问或设置表示向右的按键
        /// </summary>
        public KeyCode RightKey
        {
            get => p_left;
            set => p_left = value;
        }

        /// <summary>
        /// 访问或设置表示向下的按键
        /// </summary>
        public KeyCode DownKey
        {
            get => p_left;
            set => p_left = value;
        }

        /// <summary>
        /// 访问或设置表示向上的按键
        /// </summary>
        public KeyCode UpKey
        {
            get => p_left;
            set => p_left = value;
        }

        #endregion

        #region 派生

        #region 权限重写

        public sealed override bool CanGetVector => true;
        public sealed override bool CanGetHorizontalComponent => true;
        public sealed override bool CanGetVerticalComponent => true;
        public sealed override bool CanGetHorizontalReverse => true;
        public sealed override bool CanGetVerticalReverse => true;

        public sealed override bool CanSetHorizontalReverse => true;
        public sealed override bool CanSetVerticalReverse => true;

        /// <summary>
        /// 是否反转横轴
        /// </summary>
        public sealed override bool IsHorizontalReverse
        {
            get => p_horRev;
            set => p_horRev = value;
        }

        /// <summary>
        /// 是否反转纵轴
        /// </summary>
        public sealed override bool IsVerticalReverse
        {
            get => p_verRev;
            set => p_verRev = value;
        }

        #endregion

        #region 封装

        private static bool f_isDown(KeyCode key)
        {
            return Input.GetKey(key);
        }

        #endregion

        #region 功能实现

        public sealed override float Horizontal
        {
            get
            {
                bool left, right;

                left = f_isDown(p_left);
                right = f_isDown(p_right);

                //左右一致
                if (left == right) return 0;

                //左右不一致

                //获取上下
                bool up, down;
                up = f_isDown(p_up);
                down = f_isDown(p_down);

                //上下一致
                if(up == down)
                {
                    if(p_horRev) return right ? -1 : 1;
                    return left ? -1 : 1;
                }

                //上下不一致
                if (p_horRev) return right ? -cp_sqrt0_5 : cp_sqrt0_5;
                return left ? -cp_sqrt0_5 : cp_sqrt0_5;

            }
            set => ThrowNotSupportedException();
        }

        public sealed override float Vertical
        {
            get
            {
                bool up, down;

                up = f_isDown(p_up);
                down = f_isDown(p_down);

                //上下一致
                if (up == down) return 0;

                //上下不一致

                //获取左右
                bool right, left;
                right = f_isDown(p_right);
                left = f_isDown(p_left);

                //左右一致
                if (right == left)
                {
                    if(p_verRev) return up ? -1 : 1;
                    return down ? -1 : 1;
                }

                //左右不一致
                if (p_verRev) return up ? -cp_sqrt0_5 : cp_sqrt0_5;
                return down ? -cp_sqrt0_5 : cp_sqrt0_5;
            }
            set => ThrowNotSupportedException();
        }

        public sealed override void GetAxis(out float horizontal, out float vertical)
        {
            bool left, right, up, down;

            #region 获取参数

            if (p_horRev)
            {
                right = f_isDown(p_left);
                left = f_isDown(p_right);
            }
            else
            {
                left = f_isDown(p_left);
                right = f_isDown(p_right);
            }
            if (p_verRev)
            {
                down = f_isDown(p_up);
                up = f_isDown(p_down);
            }
            else
            {
                up = f_isDown(p_up);
                down = f_isDown(p_down);
            }

            #endregion

            #region 判断条件并赋值

            if (left)
            {
                //左

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //全安
                            horizontal = 0;
                            vertical = 0;
                        }
                        else
                        {
                            //左右上
                            horizontal = 0;
                            vertical = 1;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            horizontal = 0;
                            vertical = -1;
                        }
                        else
                        {
                            //左右
                            horizontal = 0;
                            vertical = 0;
                        }
                    }

                }
                else
                {

                    if (up)
                    {

                        if (down)
                        {
                            //左上下
                            horizontal = -1;
                            vertical = 0;
                        }
                        else
                        {
                            //左上
                            horizontal = -cp_sqrt0_5;
                            vertical = cp_sqrt0_5;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左下
                            horizontal = -cp_sqrt0_5;
                            vertical = horizontal;
                        }
                        else
                        {
                            //左
                            horizontal = -1;
                            vertical = 0;
                        }
                    }


                }

            }
            else
            {

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //右上下
                            horizontal = 1;
                            vertical = 0;
                        }
                        else
                        {
                            //右上
                            horizontal = cp_sqrt0_5;
                            vertical = cp_sqrt0_5;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //右下
                            horizontal = cp_sqrt0_5;
                            vertical = -cp_sqrt0_5;
                        }
                        else
                        {
                            //右
                            horizontal = 1;
                            vertical = 0;
                        }
                    }

                }
                else
                {

                    if (up)
                    {

                        if (down)
                        {
                            //上下
                            horizontal = 0;
                            vertical = 0;
                        }
                        else
                        {
                            //上
                            horizontal = 0;
                            vertical = 1;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            horizontal = 0;
                            vertical = -1;
                        }
                        else
                        {
                            //无
                            horizontal = 0;
                            vertical = 0;
                        }
                    }


                }

            }

            #endregion

        }

        public sealed override void GetVector(out float radian, out float length)
        {

            bool left, right, up, down;

            #region 获取参数

            if (p_horRev)
            {
                right = f_isDown(p_left);
                left = f_isDown(p_right);
            }
            else
            {
                left = f_isDown(p_left);
                right = f_isDown(p_right);
            }

            if (p_verRev)
            {
                down = f_isDown(p_up);
                up = f_isDown(p_down);
            }
            else
            {
                up = f_isDown(p_up);
                down = f_isDown(p_down);
            }

            #endregion

            #region 判断条件并赋值

            if (left)
            {
                //左

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //全
                            length = 0;
                            radian = 0;
                        }
                        else
                        {
                            //左右上
                            length = 1;
                            radian = (float)(Maths.OneRadian * (90));
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            length = 1;
                            radian = (float)(Maths.OneRadian * (90 * 3));
                        }
                        else
                        {
                            //左右
                            length = 0;
                            radian = 0;
                        }
                    }

                }
                else
                {

                    if (up)
                    {

                        if (down)
                        {
                            //左上下
                            length = 1;
                            radian = (float)(Maths.OneRadian * (90 * 2));

                        }
                        else
                        {
                            //左上
                            length = 1;
                            radian = (float)(Maths.OneRadian * (135));
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左下
                            length = 1;
                            radian = (float)(Maths.OneRadian * (225));
                        }
                        else
                        {
                            //左
                            length = 1;
                            radian = (float)(Maths.OneRadian * (180));
                        }
                    }


                }

            }
            else
            {

                if (right)
                {
                    
                    if (up)
                    {
                        
                        if (down)
                        {
                            //右上下
                            length = 1;
                            radian = 0;
                        }
                        else
                        {
                            //右上
                            length = 1;
                            radian = (float)(Maths.OneRadian * (45));
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //右下
                            length = 1;
                            radian = (float)(Maths.OneRadian * (315));
                        }
                        else
                        {
                            //右
                            length = 1;
                            radian = 0;
                        }
                    }

                }
                else
                {

                    if (up)
                    {

                        if (down)
                        {
                            //上下
                            length = 0;
                            radian = 0;
                        }
                        else
                        {
                            //上
                            length = 1;
                            radian = (float)(Maths.OneRadian * (90));
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            length = 1;
                            radian = (float)(Maths.OneRadian * (90 * 3));
                        }
                        else
                        {
                            //无
                            length = 0;
                            radian = 0;
                        }
                    }

                }

            }

            #endregion
        }
        
        public sealed override void GetVectorAngle(out float angle, out float length)
        {

            bool left, right, up, down;

            #region 获取参数

            if (p_horRev)
            {
                right = f_isDown(p_left);
                left = f_isDown(p_right);
            }
            else
            {
                left = f_isDown(p_left);
                right = f_isDown(p_right);
            }

            if (p_verRev)
            {
                down = f_isDown(p_up);
                up = f_isDown(p_down);
            }
            else
            {
                up = f_isDown(p_up);
                down = f_isDown(p_down);
            }

            #endregion

            #region 判断条件并赋值

            if (left)
            {
                //左

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //全安
                            length = 0;
                            angle = 0;
                        }
                        else
                        {
                            //左右上
                            length = 1;
                            angle = (90);
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            length = 1;
                            angle = (90 * 3);
                        }
                        else
                        {
                            //左右
                            length = 0;
                            angle = 0;
                        }
                    }

                }
                else
                {

                    if (up)
                    {

                        if (down)
                        {
                            //左上下
                            length = 1;
                            angle = (float)(90 * 2);

                        }
                        else
                        {
                            //左上
                            length = 1;
                            angle = (float)((135));
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左下
                            length = 1;
                            angle = (float)((225));
                        }
                        else
                        {
                            //左
                            length = 1;
                            angle = (float)((180));
                        }
                    }


                }

            }
            else
            {

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //右上下
                            length = 1;
                            angle = 0;
                        }
                        else
                        {
                            //右上
                            length = 1;
                            angle = (float)((45));
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //右下
                            length = 1;
                            angle = (float)((315));
                        }
                        else
                        {
                            //右
                            length = 1;
                            angle = 0;
                        }
                    }

                }
                else
                {

                    if (up)
                    {

                        if (down)
                        {
                            //上下
                            length = 0;
                            angle = 0;
                        }
                        else
                        {
                            //上
                            length = 1;
                            angle = (float)(90);
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            length = 1;
                            angle = (float)((90 * 3));
                        }
                        else
                        {
                            //无
                            length = 0;
                            angle = 0;
                        }
                    }


                }

            }

            #endregion
        }

        public sealed override void SetAxis(float horizontal, float vertical)
        {
            ThrowNotSupportedException();
        }

        public sealed override void SetVector(float radian, float length)
        {
            ThrowNotSupportedException();
        }

        public sealed override void SetVectorAngle(float angle, float length)
        {
            ThrowNotSupportedException();
        }

        #endregion

        #region 派生

        /// <summary>
        /// 返回当前四向按键映射摇杆的各个参数
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);

            sb.Append('[');
            
            sb.Append("Left:");
            sb.Append(LeftKey.ToString());
            sb.Append(' ');

            sb.Append("Right:");
            sb.Append(RightKey.ToString());
            sb.Append(' ');

            sb.Append("Down:");
            sb.Append(DownKey.ToString());
            sb.Append(' ');

            sb.Append("Up:");
            sb.Append(UpKey.ToString());

            bool hor, ver;
            hor = IsHorizontalReverse;
            ver = IsVerticalReverse;

            if(hor || ver)
            {
                sb.Append(' ');
                if (hor)
                {
                    sb.Append(nameof(IsHorizontalReverse));
                }

                if (ver && hor) sb.Append(' ');

                if (ver)
                {
                    sb.Append(nameof(IsVerticalReverse));
                }

            }
            
            sb.Append(']');

            return sb.ToString();
        }

        #endregion

        #endregion

    }

}
