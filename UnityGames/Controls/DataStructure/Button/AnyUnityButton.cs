using Cheng.Unitys;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 任意按键检查按钮
    /// </summary>
    /// <remarks>
    /// <para>将<see cref="Input.anyKey"/>封装到按钮状态的Unity按钮</para>
    /// </remarks>
    [Serializable]
    public class AnyUnityButton : UnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个任意检查按钮
        /// </summary>
        public AnyUnityButton()
        {
        }

        #endregion

        #region 参数

        #endregion

        #region 功能

        #region 参数访问

        #endregion

        #region 权限

        public override bool CanGetState => true;

        public override bool CanGetPower => true;

        public override bool CanGetChangeFrameButtonDown => true;

        public override bool CanGetMaxPower => true;

        public override bool CanGetMinPower => true;

        #endregion

        #region 派生

        /// <summary>
        /// 任意按键按下则返回true
        /// </summary>
        public override bool ButtonDown
        {
            get
            {
                return Input.anyKeyDown;
            }
        }

        /// <summary>
        /// 任意按键处于按下时返回true
        /// </summary>
        public override bool ButtonState
        {
            get
            {
                return Input.anyKey;
            }
            set => ThrowSupportedException();
        }

        public override float Power 
        {
            get => ButtonState ? 1f : 0f;
            set => ThrowSupportedException();
        }

        public override float MaxPower
        {
            get => 1f;
        }

        public override float MinPower
        {
            get => 0f;
        }

        #endregion

        #endregion

    }

}
