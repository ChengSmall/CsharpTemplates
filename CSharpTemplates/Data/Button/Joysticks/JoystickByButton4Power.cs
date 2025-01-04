using System;

namespace Cheng.ButtonTemplates.Joysticks
{

    /// <summary>
    /// 使用4个按钮的力度值表示的摇杆
    /// </summary>
    /// <remarks>
    /// <para>该摇杆使用4个不同方向的按钮的力度映射摇杆数据；</para>
    /// <para>
    /// 两个同向方向的按钮，一方力度值取负，一方力度值为原数据；将正值减负值得到的结果，表示为这一方向上的分量；<br/>
    /// 上下两个按钮表示y轴分量，下负上正；左右两个按钮表示x轴分量，左负右正
    /// </para>
    /// </remarks>
    public class JoystickByButton4Power : BaseJoystick
    {

        #region 构造

        /// <summary>
        /// 实例化4个按钮
        /// </summary>
        /// <param name="left">表示向左的按钮</param>
        /// <param name="right">表示向右的按钮</param>
        /// <param name="up">表示向上的按钮</param>
        /// <param name="down">表示向下的按钮</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JoystickByButton4Power(BaseButton left, BaseButton right, BaseButton up, BaseButton down)
        {
            if (left is null || right is null || up is null || down is null) throw new ArgumentNullException();

            f_init(left, right, up, down);
        }

        void f_init(BaseButton left, BaseButton right, BaseButton up, BaseButton down)
        {
            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
        }
        #endregion

        #region 参数
        private BaseButton p_left;
        private BaseButton p_right;
        private BaseButton p_up;
        private BaseButton p_down;
        private bool p_isReverse;
        #endregion

        #region 功能

        #region 访问

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

        #region 派生

        #region 权限判断

        public override bool CanGetFourwayButtons
        {
            get => true;
        }

        public override bool CanGetHorizontalComponent
        {
            get => p_left.CanGetPower && p_right.CanGetPower;
        }

        public override bool CanGetVerticalComponent
        {
            get => p_down.CanGetPower && p_up.CanGetPower;
        }

        public override bool CanGetVector
        {
            get => p_left.CanGetPower && p_right.CanGetPower && p_down.CanGetPower && p_up.CanGetPower;
        }

        public override bool CanGetHorizontalReverse => true;
        #endregion

        #region 数据

        public override bool IsHorizontalReverse
        {
            get => p_isReverse;
            set => p_isReverse = value;
        }

        public override void GetVector(out float radian, out float length)
        {

            GetVectorRadionAndLength(Horizontal, Vertical, out radian, out length);
        }

        public override void GetAxis(out float horizontal, out float vertical)
        {
            horizontal = Horizontal;
            vertical = Vertical;
        }

        public override float Horizontal
        {
            get
            {
                float left, right;
                if (p_isReverse)
                {
                    right = p_left.Power;
                    left = p_right.Power;
                }
                else
                {
                    left = p_left.Power;
                    right = p_right.Power;
                }

                if (left == right) return 0;

                //bool lz, rz;
                //lz = left == 0;
                //rz = right == 0;
                
                //if (lz && rz) return 0;

                //if(lz) return right;

                //if (rz) return -left;

                return right - left;
                //if (left < right) return right - left;
                //else return -(left - right);

            }
            set { ThrowNotSupportedException(); }
        }

        public override float Vertical
        {
            get
            {
                float down, up;
                if (p_isReverse)
                {
                    up = p_left.Power;
                    down = p_right.Power;
                }
                else
                {
                    down = p_left.Power;
                    up = p_right.Power;
                }

                if (down == up) return 0;

                //bool dz, uz;
                //dz = down == 0;
                //uz = up == 0;

                //if (dz && uz) return 0;
                //if (dz) return up;
                //if (uz) return -down;

              
                return up - down;

                //if (down < up) return up - down;
                //else return -(down - up);
            }
            set { ThrowNotSupportedException(); }
        }

        public override void SetAxis(float horizontal, float vertical)
        {
            ThrowNotSupportedException();
        }
        public override void SetVectorAngle(float angle, float length)
        {
            ThrowNotSupportedException();
        }
        public override void SetVector(float radian, float length)
        {
            ThrowNotSupportedException();
        }

        #endregion

        #endregion

        #endregion

    }

}
