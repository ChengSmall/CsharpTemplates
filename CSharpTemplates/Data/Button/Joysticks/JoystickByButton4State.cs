using Cheng.Algorithm;
using System;

namespace Cheng.ButtonTemplates.Joysticks
{

    /// <summary>
    /// 使用表示四个方向的按钮状态封装一个摇杆
    /// </summary>
    /// <remarks>
    /// 该摇杆使用表示4个方向的按钮封装，根据按钮状态映射摇杆；<br/>
    /// 当某个方向的按钮状态为true时，向量的角度会偏向按钮所在角度，长度固定为1；<br/>
    /// 当没有按钮状态为true时，向量的长度和角度都为0；<br/>
    /// 当一条方向线上的2个按钮状态相同时，该方向上的数据作废，表示0；<br/>
    /// 当两个不同方向线的按钮状态为true时，摇杆数据的向量长度为1，角度表示一个斜线；例如left和up两个按钮状态为true且另两个为false时，摇杆向量长度为1，角度为135；注意，此时的两个向量分量是Sqrt(0.5)
    /// </remarks>
    public sealed class JoystickByButton4State : BaseJoystick
    {

        #region 结构

        /// <summary>
        /// 8向摇杆方向类别
        /// </summary>
        public enum StateType : byte
        {

            /// <summary>
            /// 无方向
            /// </summary>
            None = 0,

            /// <summary>
            /// 右侧
            /// </summary>
            Right = 1,

            /// <summary>
            /// 右上
            /// </summary>
            RigitUp,

            /// <summary>
            /// 上
            /// </summary>
            Up,

            /// <summary>
            /// 左上
            /// </summary>
            LeftUp,

            /// <summary>
            /// 左
            /// </summary>
            Left,

            /// <summary>
            /// 左下
            /// </summary>
            LeftDown,

            /// <summary>
            /// 下
            /// </summary>
            Down,

            /// <summary>
            /// 右下
            /// </summary>
            RightDown
            
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化摇杆
        /// </summary>
        /// <param name="left">表示向左的按钮</param>
        /// <param name="right">表示向右的按钮</param>
        /// <param name="up">表示向上的按钮</param>
        /// <param name="down">表示向下的按钮</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JoystickByButton4State(BaseButton left, BaseButton right, BaseButton up, BaseButton down)
        {
            if (left is null || right is null || up is null || down is null) throw new ArgumentNullException();

            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
            p_hRev = false;
            p_vRev = false;
        }

        /// <summary>
        /// 实例化摇杆
        /// </summary>
        /// <param name="left">表示向左的按钮</param>
        /// <param name="right">表示向右的按钮</param>
        /// <param name="up">表示向上的按钮</param>
        /// <param name="down">表示向下的按钮</param>
        /// <param name="horizontalReverse">水平摇杆是否反转</param>
        /// <param name="verticalReverse">垂直摇杆是否反转</param>
        public JoystickByButton4State(BaseButton left, BaseButton right, BaseButton up, BaseButton down, bool horizontalReverse, bool verticalReverse)
        {
            if (left is null || right is null || up is null || down is null) throw new ArgumentNullException();

            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
            p_hRev = horizontalReverse;
            p_vRev = verticalReverse;
        }

        #endregion

        #region 参数

        private BaseButton p_left;
        private BaseButton p_right;
        private BaseButton p_up;
        private BaseButton p_down;

        /// <summary>
        /// 水平反转
        /// </summary>
        private bool p_hRev;

        /// <summary>
        /// 垂直反转
        /// </summary>
        private bool p_vRev;

        #endregion

        #region 功能

        #region 通用

        /// <summary>
        /// 根据4个参数状态返回摇杆的方向
        /// </summary>
        /// <param name="left">左侧按钮按下状态</param>
        /// <param name="right">右侧按钮按下状态</param>
        /// <param name="up">上方按钮按下状态</param>
        /// <param name="down">下方按钮按下状态</param>
        /// <returns>根据4个按钮状态的摇杆综合朝向</returns>
        public static StateType GetState(bool left, bool right, bool up, bool down)
        {
            if (left)
            {
                if (right)
                {
                    if (up)
                    {
                        if (down)
                        {
                            //全按
                            return StateType.None;
                        }
                        else
                        {
                            //左右上
                            return StateType.Up;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            return StateType.Down;
                        }
                        else
                        {
                            //左右
                            return StateType.None;
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
                            return StateType.Left;
                        }
                        else
                        {

                            //左上
                            return StateType.LeftUp;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //左下
                            return StateType.LeftDown;
                        }
                        else
                        {
                            //左
                            return StateType.Left;
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
                            return StateType.Right;
                        }
                        else
                        {
                            //右上
                            return StateType.RigitUp;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //右下
                            return StateType.RightDown;
                        }
                        else
                        {
                            //右
                            return StateType.Right;
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
                            return StateType.None;
                        }
                        else
                        {
                            //上
                            return StateType.Up;
                        }
                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            return StateType.Down;
                        }
                        else
                        {
                            //无
                            return StateType.None;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据摇杆方向状态获取摇杆轴分量
        /// </summary>
        /// <param name="type">摇杆状态</param>
        /// <param name="horizontal">要获取的水平方向上的轴</param>
        /// <param name="vertical">要获取的垂直方向上的轴</param>
        public static void GetAxisHV(StateType type, out float horizontal, out float vertical)
        {
            switch (type)
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
                    horizontal = Maths.FSqrt0p5;
                    vertical = Maths.FSqrt0p5;
                    break;
                case StateType.LeftUp:
                    horizontal = -Maths.FSqrt0p5;
                    vertical = Maths.FSqrt0p5;
                    break;
                case StateType.LeftDown:
                    horizontal = -Maths.FSqrt0p5;
                    vertical = -Maths.FSqrt0p5;
                    break;
                case StateType.RightDown:
                    horizontal = Maths.FSqrt0p5;
                    vertical = -Maths.FSqrt0p5;
                    break;
                default:
                    horizontal = 0;
                    vertical = 0;
                    break;
            }
        }

        /// <summary>
        /// 根据摇杆方向状态获取摇杆轴分量
        /// </summary>
        /// <param name="type">摇杆状态</param>
        /// <param name="horizontal">要获取的水平方向上的轴</param>
        /// <param name="vertical">要获取的垂直方向上的轴</param>
        public static void GetAxisHV(StateType type, out double horizontal, out double vertical)
        {
            switch (type)
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
                    horizontal = Maths.Sqrt0p5;
                    vertical = Maths.Sqrt0p5;
                    break;
                case StateType.LeftUp:
                    horizontal = -Maths.Sqrt0p5;
                    vertical = Maths.Sqrt0p5;
                    break;
                case StateType.LeftDown:
                    horizontal = -Maths.Sqrt0p5;
                    vertical = -Maths.Sqrt0p5;
                    break;
                case StateType.RightDown:
                    horizontal = Maths.Sqrt0p5;
                    vertical = -Maths.Sqrt0p5;
                    break;
                default:
                    horizontal = 0;
                    vertical = 0;
                    break;
            }
        }

        #endregion

        #region 派生

        #region 权限

        public override bool CanGetHorizontalComponent
        {
            get => p_left.CanGetState && p_right.CanGetState;
        }

        public override bool CanGetVerticalComponent
        {
            get => p_down.CanGetState && p_up.CanGetState;
        }

        public override bool CanGetVector
        {
            get => p_left.CanGetState && p_right.CanGetState && p_down.CanGetState && p_up.CanGetState;
        }

        public override bool CanGetFourwayButtons => true;

        public override bool CanGetHorizontalReverse => true;

        public override bool CanSetHorizontalReverse => true;

        public override bool CanGetVerticalReverse => true;

        public override bool CanSetVerticalReverse => true;

        #endregion

        #region 获取

        /// <summary>
        /// 表示向左的按钮
        /// </summary>
        public override BaseButton LeftButton => p_left;

        /// <summary>
        /// 表示向右的按钮
        /// </summary>
        public override BaseButton RightButton => p_right;

        /// <summary>
        /// 表示向上的按钮
        /// </summary>
        public override BaseButton UpButton => p_up;

        /// <summary>
        /// 表示向下的按钮
        /// </summary>
        public override BaseButton DownButton => p_down;

        #endregion

        private void f_get4State(out bool left, out bool right, out bool up, out bool down)
        {
            if (p_hRev)
            {
                right = p_left.ButtonState;
                left = p_right.ButtonState;
            }
            else
            {
                left = p_left.ButtonState;
                right = p_right.ButtonState;
            }

            if (p_vRev)
            {
                down = p_up.ButtonState;
                up = p_down.ButtonState;
            }
            else
            {
                up = p_up.ButtonState;
                down = p_down.ButtonState;
            }
        }

        public override void GetAxis(out float horizontal, out float vertical)
        {
            bool left, right, up, down;
            f_get4State(out left, out right, out up, out down);
            var state = GetState(left, right, up, down);
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
                    horizontal = Maths.FSqrt0p5;
                    vertical = Maths.FSqrt0p5;
                    break;
                case StateType.LeftUp:
                    horizontal = -Maths.FSqrt0p5;
                    vertical = Maths.FSqrt0p5;
                    break;
                case StateType.LeftDown:
                    horizontal = -Maths.FSqrt0p5;
                    vertical = -Maths.FSqrt0p5;
                    break;
                case StateType.RightDown:
                    horizontal = Maths.FSqrt0p5;
                    vertical = -Maths.FSqrt0p5;
                    break;
                default:
                    horizontal = 0;
                    vertical = 0;
                    break;
            }
        }

        public override void GetVector(out float radian, out float length)
        {
            bool left, right, up, down;
            f_get4State(out left, out right, out up, out down);
            const double onceRadian = System.Math.PI / 180;

            var state = GetState(left, right, up, down);
            if(state == StateType.None)
            {
                radian = 0;
                length = 0;
                return;
            }
            length = 1;
            radian = (float)((((int)state - 1) * 45) * onceRadian);
        }

        public override void GetVectorAngle(out float angle, out float length)
        {
            bool left, right, up, down;
            f_get4State(out left, out right, out up, out down);
            var state = GetState(left, right, up, down);
            if (state == StateType.None)
            {
                angle = 0;
                length = 0;
                return;
            }
            length = 1;
            angle = (float)((((int)state - 1) * 45));
        }

        public override void GetAxisD(out double horizontal, out double vertical)
        {
            bool left, right, up, down;
            f_get4State(out left, out right, out up, out down);
            var state = GetState(left, right, up, down);
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
                    horizontal = Maths.Sqrt0p5;
                    vertical = Maths.Sqrt0p5;
                    break;
                case StateType.LeftUp:
                    horizontal = -Maths.Sqrt0p5;
                    vertical = Maths.Sqrt0p5;
                    break;
                case StateType.LeftDown:
                    horizontal = -Maths.Sqrt0p5;
                    vertical = -Maths.Sqrt0p5;
                    break;
                case StateType.RightDown:
                    horizontal = Maths.Sqrt0p5;
                    vertical = -Maths.Sqrt0p5;
                    break;
                default:
                    horizontal = 0;
                    vertical = 0;
                    break;
            }

        }

        public override void GetVectorD(out double radian, out double length)
        {
            GetVector(out float a, out float l);
            radian = a; length = l;
        }

        public override void GetVectorAngleD(out double angle, out double length)
        {
            GetVectorAngle(out float a, out float l);
            angle = a; length = l;
        }

        public override float Horizontal
        {
            get
            {
                GetAxis(out var h, out _);
                return h;
            }
            set { ThrowNotSupportedException(); }
        }

        public override float Vertical
        {
            get
            {
                GetAxis(out _, out var v);
                return v;
            }
            set { ThrowNotSupportedException(); }
        }

        public override double HorizontalD
        {
            get
            {
                GetAxisD(out var h, out _);
                return h;
            }
            set { ThrowNotSupportedException(); }
        }

        public override double VerticalD
        {
            get
            {
                GetAxisD(out _, out var v);
                return v;
            }
            set { ThrowNotSupportedException(); }
        }

        public override bool IsHorizontalReverse
        { 
            get => p_hRev;
            set => p_hRev = value;
        }

        public override bool IsVerticalReverse 
        {
            get => p_vRev; 
            set => p_vRev = value;
        }

        /// <summary>
        /// 无法设置
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <exception cref="NotSupportedException">没有设置权限</exception>
        public override void SetAxis(float horizontal, float vertical)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 无法设置
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="length"></param>
        /// <exception cref="NotSupportedException">没有设置权限</exception>
        public override void SetVector(float radian, float length)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 无法设置
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="length"></param>
        /// <exception cref="NotSupportedException">没有设置权限</exception>
        public override void SetVectorAngle(float angle, float length)
        {
            ThrowNotSupportedException();
        }        

        #endregion

        #endregion

    }

}
