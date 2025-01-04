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
            cp_s2 = (float)System.Math.Sqrt(0.5);
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
            cp_s2 = (float)System.Math.Sqrt(0.5);
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

        private readonly float cp_s2;
        #endregion

        #region 功能

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

        public override bool CanGetFourwayButtons => true;        

        #endregion

        #region 派生

        public override bool CanGetHorizontalComponent
        {
            get
            {
                return p_left.CanGetState && p_right.CanGetState;
            }
        }

        public override bool CanGetVerticalComponent
        {
            get
            {
                return p_up.CanGetState && p_down.CanGetState;
            }
        }

        public override bool CanGetVector
        {
            get
            {
                return p_left.CanGetState && p_right.CanGetState && p_up.CanGetState && p_down.CanGetState;
            }
        }

        public override void GetAxis(out float horizontal, out float vertical)
        {
            bool left, right, up, down;

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

            //const double onceRadian = System.Math.PI / 180;

            horizontal = 0;
            vertical = 0;

            if ((left && right && up && down) || ((!left) && (!right) && (!up) && (!down))) return;
            

            if (left)
            {

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //全按
                        }
                        else
                        {

                            //左右上
                            vertical = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            vertical = -1;
                        }
                        else
                        {

                            //左右
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
                        }
                        else
                        {

                            //左上
                            horizontal = -cp_s2;
                            vertical = cp_s2;
                        }


                    }
                    else
                    {
                        if (down)
                        {

                            //左下
                            horizontal = -cp_s2;
                            vertical = horizontal;
                        }
                        else
                        {

                            //左
                            horizontal = -1;
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
                        }
                        else
                        {

                            //右上
                            horizontal = cp_s2;
                            vertical = cp_s2;
                        }

                    }
                    else
                    {
                        if (down)
                        {

                            //右下
                            horizontal = cp_s2;
                            vertical = -cp_s2;
                        }
                        else
                        {

                            //右
                            horizontal = 1;
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
                        }
                        else
                        {

                            //上
                            vertical = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            vertical = -1;
                        }
                        else
                        {
                            //无
                        }
                    }
                }
            }

            //GetVector(out float r, out float l);
            //BaseJoystick.GetVectorComponent(r, l, out horizontal, out vertical);
        }

        public override void GetVector(out float radian, out float length)
        {
            bool left, right, up, down;

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

            const double onceRadian = System.Math.PI / 180;

            radian = 0;
            length = 0;

            if ((left && right && up && down) || ((!left) && (!right) && (!up) && (!down))) return;

            if (left)
            {

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //全按
                        }
                        else
                        {

                            //左右上
                            radian = (float)(onceRadian * (90));
                            length = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            radian = (float)(onceRadian * (90 * 3));
                            length = 1;
                        }
                        else
                        {
                            //左右
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
                            radian = (float)(onceRadian * (90 * 2));
                            length = 1;
                        }
                        else
                        {

                            //左上
                            radian = (float)(onceRadian * (135));
                            length = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {

                            //左下
                            radian = (float)(onceRadian * (225));
                            length = 1;
                        }
                        else
                        {

                            //左
                            radian = (float)(onceRadian * (90 * 2));
                            length = 1;
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
                            radian = 0;
                            length = 1;
                        }
                        else
                        {

                            //右上
                            radian = (float)(onceRadian * (45));
                            length = 1;
                        }

                    }
                    else
                    {
                        if (down)
                        {

                            //右下
                            radian = (float)(onceRadian * (315));
                            length = 1;
                        }
                        else
                        {

                            //右
                            radian = 0;
                            length = 1;
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
                        }
                        else
                        {

                            //上
                            radian = (float)(onceRadian * (90));
                            length = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            radian = (float)(onceRadian * (270));
                            length = 1;
                        }
                        else
                        {
                            //无
                        }
                    }
                }
            }

        }

        public override void GetVectorAngle(out float angle, out float length)
        {
            bool left, right, up, down;

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

            angle = 0;
            length = 0;

            if ((left && right && up && down) || ((!left) && (!right) && (!up) && (!down))) return;

            if (left)
            {

                if (right)
                {

                    if (up)
                    {

                        if (down)
                        {
                            //全按
                        }
                        else
                        {

                            //左右上
                            angle = (float)(90);
                            length = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {
                            //左右下
                            angle = (float)((90 * 3));
                            length = 1;
                        }
                        else
                        {
                            //左右
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
                            angle = (float)((90 * 2));
                            length = 1;
                        }
                        else
                        {

                            //左上
                            angle = (float)((135));
                            length = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {

                            //左下
                            angle = (float)((225));
                            length = 1;
                        }
                        else
                        {

                            //左
                            angle = (float)((90 * 2));
                            length = 1;
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
                            angle = 0;
                            length = 1;
                        }
                        else
                        {

                            //右上
                            angle = (float)((45));
                            length = 1;
                        }

                    }
                    else
                    {
                        if (down)
                        {

                            //右下
                            angle = (float)((315));
                            length = 1;
                        }
                        else
                        {

                            //右
                            angle = 0;
                            length = 1;
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
                        }
                        else
                        {

                            //上
                            angle = (float)((90));
                            length = 1;
                        }


                    }
                    else
                    {
                        if (down)
                        {
                            //下
                            angle = (float)((270));
                            length = 1;
                        }
                        else
                        {
                            //无
                        }
                    }
                }
            }

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

        public override bool CanGetHorizontalReverse => true;

        public override bool CanSetHorizontalReverse => true;

        public override bool CanGetVerticalReverse => true;

        public override bool CanSetVerticalReverse => true;

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
        /// 该实例无法设置
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <exception cref="NotSupportedException">没有设置权限</exception>
        public override void SetAxis(float horizontal, float vertical)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 该实例无法设置
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <exception cref="NotSupportedException">没有设置权限</exception>
        public override void SetVector(float radian, float length)
        {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// 该实例无法设置
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <exception cref="NotSupportedException">没有设置权限</exception>
        public override void SetVectorAngle(float angle, float length)
        {
            ThrowNotSupportedException();
        }        

        #endregion

        #endregion

    }

}
