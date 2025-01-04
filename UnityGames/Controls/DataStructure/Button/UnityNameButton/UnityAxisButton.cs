using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 一个 Unity Axis 按钮
    /// </summary>
    /// <remarks>
    /// <para>
    /// 使用<see cref="Input.GetAxis(string)"/>或<see cref="Input.GetAxisRaw(string)"/>获取力度值<see cref="Power"/>的 Axis 按钮；按钮状态<see cref="ButtonState"/>使用力度值<see cref="Power"/>进行映射
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class UnityAxisButton : UnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        public UnityAxisButton()
        {
            p_buttonName = string.Empty;
            this.p_mid = 0.25f;
            p_axisSmooth = true;
        }

        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        /// <param name="name">指定的虚拟按钮标识</param>
        public UnityAxisButton(string name)
        {
            ButtonName = name;
            this.p_mid = 0.25f;
            p_axisSmooth = true;
        }

        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        /// <param name="name">指定的虚拟按钮标识</param>
        /// <param name="mid">指定按钮状态映射时的中间值</param>
        public UnityAxisButton(string name, float mid)
        {
            ButtonName = name;
            this.p_mid = mid;
            p_axisSmooth = true;
        }

        /// <summary>
        /// 实例化一个Unity虚拟按钮
        /// </summary>
        /// <param name="name">指定的虚拟按钮标识</param>
        /// <param name="mid">指定按钮状态映射时的中间值</param>
        /// <param name="axismooth">是否使用平滑处理方法</param>
        public UnityAxisButton(string name, float mid, bool axismooth)
        {
            ButtonName = name;
            this.p_mid = mid;
            p_axisSmooth = axismooth;
        }

        #endregion

        #region 参数

        [SerializeField] private string p_buttonName;

        [SerializeField] private float p_mid;

        [SerializeField] private bool p_axisSmooth;

#if UNITY_EDITOR

        /// <summary>
        /// 名称字符串变量名
        /// </summary>
        public const string cp_EditorProperityFieldButtonName = nameof(p_buttonName);

        /// <summary>
        /// 是否开启平滑的布尔值变量名
        /// </summary>
        public const string cp_EditorProperityFieldAxisToolName = nameof(p_axisSmooth);

        /// <summary>
        /// Power映射State中间值浮点数变量名称
        /// </summary>
        public const string cp_EditorMid_Name = nameof(p_mid);

#endif

        #endregion

        #region 派生

        public override bool CanGetState => true;
        public override bool CanGetPower => true;
        public override bool CanGetChangeFrameButtonDown => false;

        /// <summary>
        /// 表示按钮状态
        /// </summary>
        /// <returns>
        /// 通过<see cref="Power"/>映射的按钮状态；当<see cref="Power"/>大于<see cref="StateMid"/>时，返回true，否则返回false
        /// </returns>
        /// <exception cref="NotSupportedException">无法设置</exception>
        public override bool ButtonState
        {
            get
            {
                return Power > p_mid;
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

        /// <summary>
        /// 访问或设置从<see cref="Power"/>映射到<see cref="ButtonState"/>的中间值
        /// </summary>
        public float StateMid
        {
            get => p_mid;
            set
            {
                p_mid = value;
            }
        }

        #endregion


    }
}
