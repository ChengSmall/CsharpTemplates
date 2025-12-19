using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using Cheng.Algorithm;

using StateType = Cheng.ButtonTemplates.Joysticks.JoystickByButton4State.StateType;

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

    #region
#if UNITY_EDITOR
    /// <summary>
    /// 使用4个<see cref="KeyCode"/>按键作为4个方向键的摇杆
    /// </summary>
    /// <remarks>
    /// <para>
    /// 使用4个<see cref="UnityEngine.KeyCode"/>作为4个方向按钮的映射摇杆，当一个方向参数被<see cref="Input.GetKey(KeyCode)"/>函数检测为true时，摇杆会偏向与之对应的方向<br/>
    /// 当没有方向按下时摇杆向量的长度是0<br/>
    /// 当一条方向线上的2个按键状态都是true或false时，该方向上的数据表示为0<br/>
    /// 当两个不同方向线的按钮状态为true时，摇杆数据的向量长度为1，角度表示一个斜线；例如left和up两个按钮状态为true且另两个为false时，摇杆向量长度为1，角度为135；注意，此时的两个向量分量分别是Sqrt(0.5)
    /// </para>
    /// <para>可从 Unity Inspector 中修改4个按键参数和两轴反转开关</para>
    /// </remarks>
#else
    /// <summary>
    /// 使用4个<see cref="KeyCode"/>按键作为4个方向键的摇杆
    /// </summary>
    /// <remarks>
    /// <para>
    /// 使用4个<see cref="UnityEngine.KeyCode"/>作为4个方向按钮的映射摇杆，当一个方向参数被<see cref="Input.GetKey(KeyCode)"/>函数检测为true时，摇杆会偏向与之对应的方向<br/>
    /// 当没有方向按下时摇杆向量的长度是0<br/>
    /// 当一条方向线上的2个按键状态都是true或false时，该方向上的数据表示为0<br/>
    /// 当两个不同方向线的按钮状态为true时，摇杆数据的向量长度为1，角度表示一个斜线；例如left和up两个按钮状态为true且另两个为false时，摇杆向量长度为1，角度为135；注意，此时的两个向量分量分别是Sqrt(0.5)
    /// </para>
    /// </remarks>
#endif
    #endregion
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

        #endregion

        #endregion

        #region 参数

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
        public const string FieldName_left = nameof(p_left);

        /// <summary>
        /// 右移按键字段名称
        /// </summary>
        public const string FieldName_right = nameof(p_right);

        /// <summary>
        /// 下移按键字段名称
        /// </summary>
        public const string FieldName_down = nameof(p_down);

        /// <summary>
        /// 上移按键字段名称
        /// </summary>
        public const string FieldName_up = nameof(p_up);

        /// <summary>
        /// 横轴反转开关字段名称
        /// </summary>
        public const string FieldName_HorizontalReverse = nameof(p_horRev);

        /// <summary>
        /// 纵轴反转开关字段名称
        /// </summary>
        public const string FieldName_VerticalReverse = nameof(p_verRev);

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

        public override bool CanGetVector => true;

        public override bool CanGetHorizontalComponent => true;

        public override bool CanGetVerticalComponent => true;

        public override bool CanGetHorizontalReverse => true;

        public override bool CanGetVerticalReverse => true;

        public override bool CanSetHorizontalReverse => true;

        public override bool CanSetVerticalReverse => true;

        #endregion

        #region 封装

        private static bool f_isDown(KeyCode key)
        {
            return Input.GetKey(key);
        }

        private void f_getState(out bool left, out bool right, out bool up, out bool down)
        {
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
        }

        #endregion

        #region 功能实现

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

        public sealed override float Horizontal
        {
            get
            {
                GetAxis(out float h, out _);
                return h;
            }
        }

        public sealed override float Vertical
        {
            get
            {
                GetAxis(out _, out float v);
                return v;
            }
        }

        public sealed override void GetAxis(out float horizontal, out float vertical)
        {
            f_getState(out var left, out var right, out var up, out var down);

            const float cp_s = Maths.FSqrt0p5;
            const float cp_ns = -cp_s;

            var state = JoystickByButton4State.GetState(left, right, up, down);
            switch (state)
            {
                case StateType.Right:
                    horizontal = 1;
                    vertical = 0;
                    break;
                case StateType.Up:
                    horizontal = 0;
                    vertical = 1;
                    break;
                case StateType.Left:
                    horizontal = -1;
                    vertical = 0;
                    break;
                case StateType.Down:
                    horizontal = 0;
                    vertical = -1;
                    break;
                case StateType.RigitUp:
                    horizontal = cp_s;
                    vertical = cp_s;
                    break;
                case StateType.LeftUp:
                    horizontal = cp_ns;
                    vertical = cp_s;
                    break;
                case StateType.LeftDown:
                    horizontal = cp_ns;
                    vertical = cp_ns;
                    break;
                case StateType.RightDown:
                    horizontal = cp_s;
                    vertical = cp_ns;
                    break;
                default:
                    horizontal = 0;
                    vertical = 0;
                    break;
            }

        }

        public sealed override void GetVector(out float radian, out float length)
        {
            bool left, right, up, down;

            f_getState(out left, out right, out up, out down);

            const double onceRadian = Maths.OneRadian;
            length = 1;
            var state = JoystickByButton4State.GetState(left, right, up, down);
            switch (state)
            {
                case StateType.Right:
                    radian = 0;
                    break;
                case StateType.Up:
                    radian = (float)(onceRadian * 90);
                    break;
                case StateType.Left:
                    radian = (float)(onceRadian * 180);
                    break;
                case StateType.Down:
                    radian = (float)(onceRadian * (90 * 3));
                    break;
                case StateType.RigitUp:
                    radian = (float)(onceRadian * (45));
                    break;
                case StateType.LeftUp:
                    radian = (float)(onceRadian * (45 + 90));
                    break;
                case StateType.LeftDown:
                    radian = (float)(onceRadian * (180 + 45));
                    break;
                case StateType.RightDown:
                    radian = (float)(onceRadian * (360 - 45));
                    break;
                default:
                    radian = 0;
                    length = 0;
                    break;
            }

        }

        public sealed override void GetVectorAngle(out float angle, out float length)
        {
            f_getState(out var left, out var right, out var up, out var down);

            length = 1;
            var state = JoystickByButton4State.GetState(left, right, up, down);
            switch (state)
            {
                case StateType.Right:
                    angle = 0;
                    break;
                case StateType.Up:
                    angle = (float)(90);
                    break;
                case StateType.Left:
                    angle = (float)(180);
                    break;
                case StateType.Down:
                    angle = (float)((90 * 3));
                    break;
                case StateType.RigitUp:
                    angle = (float)((45));
                    break;
                case StateType.LeftUp:
                    angle = (float)((45 + 90));
                    break;
                case StateType.LeftDown:
                    angle = (float)((180 + 45));
                    break;
                case StateType.RightDown:
                    angle = (float)((360 - 45));
                    break;
                default:
                    angle = 0;
                    length = 0;
                    break;
            }

        }

        #region

        /// <summary>
        /// 无效函数
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <exception cref="NotSupportedException">引发异常</exception>
        public sealed override void SetAxis(float horizontal, float vertical)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 无效函数
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="length"></param>
        /// <exception cref="NotSupportedException">引发异常</exception>
        public sealed override void SetVector(float radian, float length)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 无效函数
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="length"></param>
        /// <exception cref="NotSupportedException">引发异常</exception>
        public sealed override void SetVectorAngle(float angle, float length)
        {
            ThrowNotSupportedException();
        }

        #endregion

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
