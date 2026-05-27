#if UNITY_STANDALONE_WIN

using System;
using UnityEngine;

using Cheng.Unitys.Windows.XInput;
using Cheng.Unitys.Windows.XInput.Win32API;


namespace Cheng.ButtonTemplates.UnityButtons.Windows
{

    /// <summary>
    /// XInput控制器按钮类型
    /// </summary>
    public enum XInputButtonType
    {

        /// <summary>
        /// 控制器按钮
        /// </summary>
        PadButton = 1,

        /// <summary>
        /// 控制器扳机
        /// </summary>
        TriggerButton = 2
    }

    /// <summary>
    /// XInput输入按钮基类
    /// </summary>
    public abstract class XInputUnityButton : UnityButton
    {

        #region 参数访问

        /// <summary>
        /// 控制器按钮类型
        /// </summary>
        public abstract XInputButtonType ButtonType { get; }

        /// <summary>
        /// 按键绑定的控制器索引
        /// </summary>
        public abstract int PadIndex { get; set; }

        /// <summary>
        /// 获取控制器按钮
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型</exception>
        public virtual XInputPadUnityButton PadButton
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// 获取控制器扳机
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型</exception>
        public virtual XInputTriggerUnityButton TriggerButton
        {
            get => throw new NotImplementedException();
        }

        #endregion

    }

}

#endif