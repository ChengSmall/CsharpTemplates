using Cheng.Algorithm;
using Cheng.DataStructure.Cherrsdinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.ButtonTemplates.Joysticks
{

    /// <summary>
    /// 可进行操作反转的只读摇杆
    /// </summary>
    public sealed class ReversibleJoystick : BaseJoystick
    {

        #region

        /// <summary>
        /// 封装一个摇杆，并添加可反转数据的功能
        /// </summary>
        /// <param name="baseJoystick">要封装的摇杆</param>
        /// <exception cref="ArgumentNullException">摇杆参数是null</exception>
        public ReversibleJoystick(BaseJoystick baseJoystick) : this(baseJoystick, false, false)
        {
        }

        /// <summary>
        /// 封装一个摇杆，并添加可反转数据的功能
        /// </summary>
        /// <param name="baseJoystick">要封装的摇杆</param>
        /// <param name="isHorizontalReverse">是否开启水平反转；默认为false</param>
        /// <param name="isVerticalReverse">是否开启垂直反转；默认为false</param>
        /// <exception cref="ArgumentNullException">摇杆参数是null</exception>
        public ReversibleJoystick(BaseJoystick baseJoystick, bool isHorizontalReverse, bool isVerticalReverse)
        {
            if (baseJoystick is null) throw new ArgumentNullException();
            p_joy = baseJoystick;
            p_isRevHor = isHorizontalReverse;
            p_isRevVer = isVerticalReverse;
        }

        #endregion

        #region

        private BaseJoystick p_joy;

        private bool p_isRevHor;
        private bool p_isRevVer;

        #endregion

        #region 功能

        #region 派生

        public override bool CanGetHorizontalComponent => p_joy.CanGetHorizontalComponent;

        public override bool CanGetVerticalComponent => p_joy.CanGetVerticalComponent;

        public override bool CanSetHorizontalComponent => p_joy.CanSetHorizontalComponent;

        public override bool CanSetVerticalComponent => p_joy.CanSetVerticalComponent;

        public override bool CanGetVector => p_joy.CanGetVector;

        public override bool CanSetVector => p_joy.CanSetVector;

        public override bool CanChangeEvent => false;

        public override bool CanGetFourwayButtons => false;

        public override bool CanGetHorizontalReverse => true;

        public override bool CanSetHorizontalReverse => true;

        public override bool CanGetVerticalReverse => true;

        public override bool CanSetVerticalReverse => true;

        public override bool CanGetInternalJoystick => true;

        public override bool CanSetInternalJoystick => true;

        /// <summary>
        /// 是否应用水平轴反转
        /// </summary>
        public override bool IsHorizontalReverse 
        {
            get => p_isRevHor;
            set => p_isRevHor = value; 
        }

        /// <summary>
        /// 是否应用垂直轴反转
        /// </summary>
        public override bool IsVerticalReverse 
        {
            get => p_isRevVer;
            set => p_isRevVer = value;
        }

        /// <summary>
        /// 访问或设置内部封装的摇杆对象
        /// </summary>
        /// <exception cref="ArgumentNullException">设置的对象是null</exception>
        public override BaseJoystick InternalJoystick 
        {
            get => p_joy;
            set
            {
                p_joy = value ?? throw new ArgumentNullException();
            }
        }

        public override float Horizontal 
        {
            get => p_isRevHor ? -p_joy.Horizontal: p_joy.Horizontal;
        }

        public override float Vertical 
        {
            get => p_isRevVer ? -p_joy.Vertical : p_joy.Vertical;
        }

        public override double HorizontalD { get => p_isRevHor ? -p_joy.HorizontalD : p_joy.HorizontalD; }

        public override double VerticalD { get => p_isRevVer ? -p_joy.VerticalD : p_joy.VerticalD; }

        public override void GetVector(out float radian, out float length)
        {
            p_joy.GetVector(out radian, out length);

            if (p_isRevHor)
            {
                if (p_isRevVer)
                {
                    //双反转
                    radian = (float)BaseJoystick.ToReverseVector(radian);
                }
                else
                {
                    //水平反转
                    radian = (float)BaseJoystick.ToReverseHorizontal(radian);
                }
            }
            else
            {
                if (p_isRevVer)
                {
                    //垂直反转
                    radian = (float)BaseJoystick.ToReverseVertical(radian);
                }
            }
        }

        public override void GetVectorAngle(out float angle, out float length)
        {
            GetVector(out angle, out length);
            angle = (float)(angle / Maths.OneRadian);
        }

        public override void GetAxis(out float horizontal, out float vertical)
        {
            p_joy.GetAxis(out horizontal, out vertical);
            if (p_isRevHor)
            {
                horizontal = -horizontal;
            }
            if (p_isRevVer)
            {
                vertical = -vertical;
            }
        }

        public override void GetVectorD(out double radian, out double length)
        {
            p_joy.GetVectorD(out radian, out length);

            if (p_isRevHor)
            {
                if (p_isRevVer)
                {
                    //双反转
                    radian = BaseJoystick.ToReverseVector(radian);
                }
                else
                {
                    //水平反转
                    radian = BaseJoystick.ToReverseHorizontal(radian);
                }
            }
            else
            {
                if (p_isRevVer)
                {
                    //垂直反转
                    radian = BaseJoystick.ToReverseVertical(radian);
                }
            }
        }

        public override void GetVectorAngleD(out double angle, out double length)
        {
            GetVectorD(out angle, out length);
            angle = (angle / Maths.OneRadian);
        }

        public override void GetAxisD(out double horizontal, out double vertical)
        {
            p_joy.GetAxisD(out horizontal, out vertical);
            if (p_isRevHor)
            {
                horizontal = -horizontal;
            }
            if (p_isRevVer)
            {
                vertical = -vertical;
            }
        }

        #endregion

        #endregion

    }

}
