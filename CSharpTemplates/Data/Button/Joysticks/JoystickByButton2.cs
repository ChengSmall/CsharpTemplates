using System;

namespace Cheng.ButtonTemplates.Joysticks
{

    /// <summary>
    /// 可连接2个按钮的摇杆封装
    /// </summary>
    /// <remarks>
    /// 将2个按钮表示为摇杆的上下、左右四个方向，以此模拟一个摇杆
    /// <para>使用按钮<see cref="BaseButton.Power"/>属性做摇杆的偏移量</para>
    /// </remarks>
    public class JoystickByButton2 : BaseJoystick
    {

        #region 构造

        /// <summary>
        /// 实例化一个摇杆按钮封装器
        /// </summary>
        /// <param name="lr">表示左右方向摇杆的按钮</param>
        /// <param name="ud">表示上下方向摇杆的按钮</param>
        /// <exception cref="ArgumentNullException">有参数是null</exception>
        public JoystickByButton2(BaseButton lr, BaseButton ud)
        {
            if (lr is null || ud is null) throw new ArgumentNullException();

            p_lr = lr;
            p_ud = ud;
        }

        #endregion

        #region 参数

        private BaseButton p_lr;

        private BaseButton p_ud;

        #endregion

        #region 功能

        #region 派生

        public override bool CanGetHorizontalComponent => p_lr.CanGetState;

        public override bool CanGetVerticalComponent => p_ud.CanGetState;

        public override bool CanSetHorizontalComponent => p_lr.CanSetState;

        public override bool CanSetVerticalComponent => p_lr.CanSetState;

        public override bool CanGetVector
        {
            get
            {
                return p_lr.CanGetState && p_ud.CanGetState;
            }
        }

        public override bool CanSetVector
        {
            get => p_lr.CanSetState && p_ud.CanSetState;
        }

        public override float Horizontal
        {
            get
            {
                return p_lr.Power;
            }
            set
            {
                p_lr.Power = value;
            }
        }

        public override float Vertical
        {
            get
            {
                return p_ud.Power;
            }
            set
            {
                p_ud.Power = value;
            }
        }

        #endregion

        #region 参数访问

        /// <summary>
        /// 访问表示左右方向摇杆的按钮
        /// </summary>
        public BaseButton LeftRightButton
        {
            get => p_lr;
        }

        /// <summary>
        /// 访问表示上下方向摇杆的按钮
        /// </summary>
        public BaseButton UpDownButton
        {
            get => p_ud;
        }

        #endregion

        #region 功能

        public override void GetAxis(out float horizontal, out float vertical)
        {
            horizontal = p_lr.Power;
            vertical = p_ud.Power;
        }


        public override void SetAxis(float horizontal, float vertical)
        {
            p_lr.Power = horizontal;
            p_ud.Power = vertical;
        }


        public override void GetVector(out float radian, out float length)
        {
            var horizontal = p_lr.Power;
            var vertical = p_ud.Power;

            GetVectorRadionAndLength(horizontal, vertical, out radian, out length);
        }


        public override void SetVector(float radian, float length)
        {
            BaseJoystick.GetVectorComponent(radian, length, out float x, out float y);
            p_lr.Power = x;
            p_ud.Power = y;
        }   

        #endregion

        #endregion

    }

}
