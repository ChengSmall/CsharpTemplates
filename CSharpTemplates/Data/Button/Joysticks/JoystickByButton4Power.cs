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
    public sealed class JoystickByButton4Power : BaseJoystick
    {

        #region 构造

        /// <summary>
        /// 实例化4个按钮
        /// </summary>
        /// <param name="left">表示向左的按钮</param>
        /// <param name="right">表示向右的按钮</param>
        /// <param name="up">表示向上的按钮</param>
        /// <param name="down">表示向下的按钮</param>
        /// <exception cref="ArgumentNullException">按钮参数是null</exception>
        public JoystickByButton4Power(BaseButton left, BaseButton right, BaseButton up, BaseButton down)
        {
            if (left is null || right is null || up is null || down is null) throw new ArgumentNullException();

            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
            p_isHorReverse = false;
            p_isVerReverse = false;
        }

        /// <summary>
        /// 实例化4个按钮
        /// </summary>
        /// <param name="left">表示向左的按钮</param>
        /// <param name="right">表示向右的按钮</param>
        /// <param name="up">表示向上的按钮</param>
        /// <param name="down">表示向下的按钮</param>
        /// <param name="horizontalReverse">是否开启水平翻转</param>
        /// <param name="verticalReverse">是否开启垂直翻转</param>
        /// <exception cref="ArgumentNullException">按钮参数是null</exception>
        public JoystickByButton4Power(BaseButton left, BaseButton right, BaseButton up, BaseButton down, bool horizontalReverse, bool verticalReverse)
        {
            if (left is null || right is null || up is null || down is null) throw new ArgumentNullException();

            p_left = left;
            p_right = right;
            p_up = up;
            p_down = down;
            p_isHorReverse = horizontalReverse;
            p_isVerReverse = verticalReverse;
        }

        #endregion

        #region 参数

        private BaseButton p_left;
        private BaseButton p_right;
        private BaseButton p_up;
        private BaseButton p_down;

        private bool p_isHorReverse;
        private bool p_isVerReverse;

        #endregion

        #region 功能

        #region 访问

        /// <summary>
        /// 表示向左的按钮
        /// </summary>
        public override BaseButton LeftButton
        {
            get => p_left;
            set => p_left = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 表示向右的按钮
        /// </summary>
        public override BaseButton RightButton
        {
            get => p_right;
            set => p_right = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 表示向上的按钮
        /// </summary>
        public override BaseButton UpButton 
        {
            get => p_up;
            set => p_up = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 表示向下的按钮
        /// </summary>
        public override BaseButton DownButton 
        {
            get => p_down;
            set => p_down = value ?? throw new ArgumentNullException();
        }

        #endregion

        #region 派生

        #region 权限判断

        public override JoystickAvailablePermissions AvailablePermissions
        {
            get
            {
                const JoystickAvailablePermissions or = JoystickAvailablePermissions.CanGetFourwayButtons |
                     JoystickAvailablePermissions.CanSetFourwayButtons |
                    JoystickAvailablePermissions.CanSetAndGetAllReverse;

                var leftA = p_left.AvailablePermissions;
                var rightA = p_right.AvailablePermissions;
                var upA = p_up.AvailablePermissions;
                var downA = p_down.AvailablePermissions;

                JoystickAvailablePermissions jp = or;

                bool LR_Power;
                bool UD_Power;

                LR_Power = (((leftA & (ButtonAvailablePermissions.CanGetPower)) == ButtonAvailablePermissions.CanGetPower) && ((rightA & (ButtonAvailablePermissions.CanGetPower)) == ButtonAvailablePermissions.CanGetPower));

                if (LR_Power)
                {
                    jp |= JoystickAvailablePermissions.CanGetHorizontalComponent;
                }

                UD_Power = (((upA & (ButtonAvailablePermissions.CanGetPower)) == ButtonAvailablePermissions.CanGetPower) && ((downA & (ButtonAvailablePermissions.CanGetPower)) == ButtonAvailablePermissions.CanGetPower));

                if (UD_Power)
                {
                    jp = JoystickAvailablePermissions.CanGetVerticalComponent;
                }

                if(LR_Power && UD_Power)
                {
                    jp |= JoystickAvailablePermissions.CanGetVector;
                }

                return jp;
            }
        }

        #endregion

        #region 数据

        public override bool IsHorizontalReverse
        {
            get => p_isHorReverse;
            set => p_isHorReverse = value;
        }

        public override bool IsVerticalReverse
        {
            get => p_isVerReverse; 
            set => p_isVerReverse = value; 
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
                if (p_isHorReverse)
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

                return right - left;

            }
            set { ThrowNotSupportedException(); }
        }

        public override float Vertical
        {
            get
            {
                float down, up;
                if (p_isVerReverse)
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
