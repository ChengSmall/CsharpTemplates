using System;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 检测 Unity 任意触控的按钮
    /// </summary>
    public class AnyTouchUnityButton : UnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个检测 Unity 触控按钮
        /// </summary>
        public AnyTouchUnityButton()
        {
        }

        #endregion

        #region 参数

        #endregion

        #region 派生

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {

                if(Input.touchSupported) return ButtonAvailablePermissions.CanGetState |
                 ButtonAvailablePermissions.CanGetPower |
                 ButtonAvailablePermissions.CanGetChangeFrameButtonUp |
                 ButtonAvailablePermissions.CanGetChangeFrameButtonDown | 
                    UnityButtonAvailablePromissions;

                return UnityButtonAvailablePromissions;
            }
        }

        /// <summary>
        /// 如果有手指触摸在了设备上，返回true，否则返回false
        /// </summary>
        /// <exception cref="NotSupportedException">设备不支持触摸</exception>
        public override bool ButtonState 
        { 
            get
            {
                if (!Input.touchSupported) ThrowSupportedException();

                return Input.touchCount > 0;
            }
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 在当前帧，有手指开始触摸了设备屏幕则返回true，否则返回false
        /// </summary>
        /// <exception cref="NotSupportedException">设备不支持触摸</exception>
        public override bool ButtonDown
        {
            set => ThrowSupportedException();
            get
            {
                if (!Input.touchSupported) ThrowSupportedException();
                var count = Input.touchCount;

                if (count == 0) return false;

                for (int i = 0; i < count; i++)
                {
                    var touch = Input.GetTouch(i);

                    if(touch.phase == TouchPhase.Began)
                    {
                        return true;
                    }

                }

                return false;
            }
        }

        /// <summary>
        /// 在当前帧，最后一个已经触摸在设备上的手指离开屏幕则返回true，否则返回false
        /// </summary>
        /// <exception cref="NotSupportedException">设备不支持触摸</exception>
        public override bool ButtonUp
        {
            set => ThrowSupportedException();
            get
            {
                if (!Input.touchSupported) ThrowSupportedException();
                var count = Input.touchCount;
                if (count == 0) return false;

                for (int i = 0; i < count; i++)
                {
                    var touch = Input.GetTouch(i);

                    if (touch.phase != TouchPhase.Ended)
                    {
                        return false;
                    }

                }

                return true;
            }
        }

        /// <summary>
        /// 当前设备的最大触摸压力，没有则为0，若不支持压力参数则在有触摸时返回1
        /// </summary>
        /// <exception cref="NotSupportedException">设备不支持触摸</exception>
        public override float Power 
        {
            get
            {
                if (!Input.touchSupported) ThrowSupportedException();
                var count = Input.touchCount;

                if (count == 0) return 0f;

                if (!Input.touchPressureSupported) return 1f;

                float ps = 0;

                for (int i = 0; i < count; i++)
                {
                    var touch = Input.GetTouch(i);
                    var tp = touch.pressure;
                    if (tp > ps) ps = tp;
                }

                return ps;
            }
            set => ThrowSupportedException();
        }

        #endregion

    }

}
