using System;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 使用Unity虚拟按钮映射的按钮
    /// </summary>
    /// <remarks>
    /// 此按钮封装一个 Input Manager 窗口中的虚拟按钮名称，
    /// 通过 <see cref="ButtonName"/> 属性设置虚拟按钮标识，使用 <see cref="Input"/> 类实现按钮状态；<br/>
    /// 使用<see cref="UnityEngine.Input.GetButton(string)"/>获取按钮状态；使用<see cref="UnityEngine.Input.GetAxis(string)"/>或<see cref="UnityEngine.Input.GetAxisRaw(string)"/>获取按钮力度值；
    /// <para>
    /// 允许作为脚本参数在 Inspector 中设置；<br/>
    /// 通过 Inspector 设置虚拟按钮名称和是否应用平滑开关
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class UnityNameButton : UnityButton
    {

        #region 构造
        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        /// <param name="name">指定的虚拟按钮标识</param>
        public UnityNameButton(string name)
        {
            ButtonName = name;
            p_axisSmooth = true;
        }
        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        public UnityNameButton()
        {
            p_buttonName = string.Empty;
            p_axisSmooth = true;
        }
        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        /// <param name="name">指定的虚拟按钮标识</param>
        /// <param name="axismooth">力度获取是否使用平滑处理方法</param>
        public UnityNameButton(string name, bool axismooth)
        {
            ButtonName = name;
            p_axisSmooth = axismooth;
        }
        #endregion

        #region 参数

        [SerializeField] private string p_buttonName;

        [SerializeField] private bool p_axisSmooth;

#if UNITY_EDITOR
        public const string EditorProperityFieldButtonName = nameof(p_buttonName);
        public const string EditorProperityFieldAxisToolName = nameof(p_axisSmooth);
#endif

        #endregion

        #region 派生

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                return UnityButtonAvailablePromissions |
                 ButtonAvailablePermissions.AllGetStateAndPower |
                 ButtonAvailablePermissions.CanGetChangeFrameButtonDown |
                 ButtonAvailablePermissions.CanGetChangeFrameButtonUp;
            }
        }

        /// <summary>
        /// 使用<see cref="Input.GetButtonDown(string)"/>获取参数
        /// </summary>
        public override bool ButtonDown => Input.GetButtonDown(p_buttonName);

        /// <summary>
        /// 使用<see cref="Input.GetButtonUp(string)"/>获取参数
        /// </summary>
        public override bool ButtonUp => Input.GetButtonUp(p_buttonName);

        /// <summary>
        /// 访问虚拟按钮状态
        /// </summary>
        /// <returns>使用参数<see cref="ButtonName"/>提供的虚拟标识，调用<see cref="Input.GetButton(string)"/>返回按钮状态</returns>
        /// <value>没有属性设置功能</value>
        /// <exception cref="NotSupportedException">没有属性设置功能</exception>
        public override bool ButtonState
        {
            get
            {
                return Input.GetButton(p_buttonName);
            }
            set { ThrowSupportedException(); }
        }

        /// <summary>
        /// 访问虚拟按钮轴
        /// </summary>
        /// <returns>使用参数<see cref="ButtonName"/>提供的虚拟标识，调用<see cref="Input.GetAxis(string)"/>或<see cref="Input.GetAxisRaw(string)"/>返回按钮力度；
        /// </returns>
        public override float Power
        {
            get
            {
                return p_axisSmooth ? Input.GetAxis(p_buttonName) : Input.GetAxisRaw(p_buttonName);
            }
            set { ThrowSupportedException(); }
        }   

        /// <summary>
        /// 返回虚拟按钮标识
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_buttonName;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置Unity虚拟按钮标识
        /// </summary>
        public string ButtonName
        {
            get => p_buttonName;
            set => p_buttonName = value ?? string.Empty;
        }

        /// <summary>
        /// 获取或访问轴力度时是否使用平滑处理方法
        /// </summary>
        /// <value>
        /// 当值设为true时，<see cref="Power"/>属性使用<see cref="Input.GetAxis(string)"/>获取值；设为false时，<see cref="Power"/>属性使用<see cref="Input.GetAxisRaw(string)"/>获取值<br/>
        /// 此值默认为true
        /// </value>
        public bool AxisSmooth
        {
            get => p_axisSmooth;
            set => p_axisSmooth = value;
        }

        #endregion

    }


}
