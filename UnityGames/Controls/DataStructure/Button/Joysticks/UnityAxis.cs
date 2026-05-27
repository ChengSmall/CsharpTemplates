using System;
using UnityEngine;

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

    /// <summary>
    /// Unity虚拟轴
    /// </summary>
    /// <remarks>
    /// <para>
    /// 使用两个Unity虚拟标识名称表示2个虚拟轴，并使用 <see cref="Input"/> 实现摇杆参数
    /// </para>
    /// <para>
    /// 可从 Unity Inspector 设置参数，从左至右分别表示：虚拟轴名称、平滑开关、反转开关
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class UnityAxis : BaseJoystick
    {

        #region 构造

        /// <summary>
        /// 实例化一个Unity虚拟轴
        /// </summary>
        public UnityAxis()
        {
            HorizontalName = string.Empty;
            VerticalName = string.Empty;
            IsHorizontalSmoothAxis = true;
            IsVerticalSmoothAxis = true;
            p_isXReversed = false;
            p_isYReversed = false;
        }

        /// <summary>
        /// 实例化一个Unity虚拟轴
        /// </summary>
        /// <param name="horizontalName">横轴分量虚拟轴名称</param>
        /// <param name="verticalName">竖轴分量虚拟轴名称</param>
        public UnityAxis(string horizontalName, string verticalName)
        {
            HorizontalName = horizontalName;
            VerticalName = verticalName;
            IsHorizontalSmoothAxis = true;
            IsVerticalSmoothAxis = true;
            p_isXReversed = false;
            p_isYReversed = false;
        }

        /// <summary>
        /// 实例化一个Unity虚拟轴
        /// </summary>
        /// <param name="horizontalName">横轴分量虚拟轴名称</param>
        /// <param name="verticalName">竖轴分量虚拟轴名称</param>
        /// <param name="horizontalSmoothAxis">是否让横轴应用平滑处理</param>
        /// <param name="verticalSmoothAxis">是否让纵轴应用平滑处理</param>
        public UnityAxis(string horizontalName, string verticalName, bool horizontalSmoothAxis, bool verticalSmoothAxis)
        {
            HorizontalName = horizontalName;
            VerticalName = verticalName;
            IsHorizontalSmoothAxis = horizontalSmoothAxis;
            IsVerticalSmoothAxis = verticalSmoothAxis;
            p_isXReversed = false;
            p_isYReversed = false;
        }

        /// <summary>
        /// 实例化一个Unity虚拟轴
        /// </summary>
        /// <param name="horizontalName">横轴分量虚拟轴名称</param>
        /// <param name="verticalName">竖轴分量虚拟轴名称</param>
        /// <param name="horizontalSmoothAxis">是否让横轴应用平滑处理</param>
        /// <param name="verticalSmoothAxis">是否让纵轴应用平滑处理</param>
        /// <param name="isHorizontalReversed">是否设置为反转x轴</param>
        /// <param name="isVerticalReversed">是否设置为反转y轴</param>
        public UnityAxis(string horizontalName, string verticalName, bool horizontalSmoothAxis, bool verticalSmoothAxis, bool isHorizontalReversed, bool isVerticalReversed)
        {
            HorizontalName = horizontalName;
            VerticalName = verticalName;
            IsHorizontalSmoothAxis = horizontalSmoothAxis;
            IsVerticalSmoothAxis = verticalSmoothAxis;
            p_isXReversed = isHorizontalReversed;
            p_isYReversed = isVerticalReversed;
        }

        #endregion

        #region 参数

        [SerializeField] private string p_horizontalName;

        [SerializeField] private string p_verticalName;

        [SerializeField] private bool p_isXSmooth;

        [SerializeField] private bool p_isYSmooth;

        [SerializeField] private bool p_isXReversed;

        [SerializeField] private bool p_isYReversed;

        #region Editor
#if UNITY_EDITOR

        /// <summary>
        /// 横轴字符串变量名称
        /// </summary>
        public const string FieldName_Horizontal = nameof(p_horizontalName);

        /// <summary>
        /// 纵轴字符串变量名称
        /// </summary>
        public const string FieldName_Vertical = nameof(p_verticalName);

        /// <summary>
        /// x轴平滑应用布尔值字段名称
        /// </summary>
        public const string FieldName_XSmooth = nameof(p_isXSmooth);

        /// <summary>
        /// y轴平滑应用布尔值字段名称
        /// </summary>
        public const string FieldName_YSmooth = nameof(p_isYSmooth);

        /// <summary>
        /// 反转x轴布尔值字段名称
        /// </summary>
        public const string FieldName_IsXReversed = nameof(p_isXReversed);

        /// <summary>
        /// 反转y轴布尔值字段名称
        /// </summary>
        public const string FieldName_IsYReversed = nameof(p_isYReversed);
#endif
        #endregion

        #endregion

        #region 参数访问

        /// <summary>
        /// 访问或设置水平摇杆的虚拟标识
        /// </summary>
        public string HorizontalName
        {
            get => p_horizontalName;
            set
            {
                p_horizontalName = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 访问或设置垂直摇杆的虚拟标识
        /// </summary>
        public string VerticalName
        {
            get => p_verticalName;
            set
            {
                p_verticalName = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 是否让横轴应用平滑处理
        /// </summary>
        /// <value>
        /// 设置为true会使用 <see cref="Input.GetAxis(string)"/> 做摇杆参数；设置为false会使用 <see cref="Input.GetAxisRaw(string)"/> 做摇杆参数；此参数默认为true
        /// </value>
        public bool IsHorizontalSmoothAxis
        {
            get => p_isXSmooth;
            set => p_isXSmooth = value;
        }

        /// <summary>
        /// 是否让纵轴应用平滑处理
        /// </summary>
        /// <value>
        /// 设置为true会使用 <see cref="Input.GetAxis(string)"/> 做摇杆参数；设置为false会使用 <see cref="Input.GetAxisRaw(string)"/> 做摇杆参数；此参数默认为true
        /// </value>
        public bool IsVerticalSmoothAxis
        {
            get => p_isYSmooth;
            set => p_isYSmooth = value;
        }        

        /// <summary>
        /// 设置两个分量是否应用平滑处理
        /// </summary>
        /// <param name="smooth">
        /// 设置为true会使用 <see cref="Input.GetAxis(string)"/> 做摇杆参数；设置为false会使用 <see cref="Input.GetAxisRaw(string)"/> 做摇杆参数；此参数默认为true
        /// </param>
        public void SetAllSmoothAxis(bool smooth)
        {
            p_isXSmooth = smooth;
            p_isXSmooth = smooth;
        }

        #endregion

        #region 派生

        public override bool CanGetVector => true;

        public override bool CanGetHorizontalComponent => true;

        public override bool CanGetVerticalComponent => true;

        public override bool CanGetHorizontalReverse => true;

        public override bool CanGetVerticalReverse => true;

        public override bool CanSetHorizontalReverse => true;

        public override bool CanSetVerticalReverse => true;

        /// <summary>
        /// 访问垂直摇杆的值
        /// </summary>
        /// <returns> 使用<see cref="VerticalName"/> 作为参数，调用 <see cref="Input.GetAxis(string)"/> 或 <see cref="Input.GetAxisRaw(string)"/> 获取值</returns>
        /// <exception cref="NotSupportedException">无法设置</exception>
        public override float Vertical
        {
            get
            {
                var re = p_isYSmooth ? Input.GetAxis(p_verticalName) : Input.GetAxisRaw(p_verticalName);
                return p_isYReversed ? -re : re;
            }
        }

        /// <summary>
        /// 访问水平摇杆的值
        /// </summary>
        /// <returns>使用 <see cref="HorizontalName"/> 作为参数，调用 <see cref="Input.GetAxis(string)"/> 或 <see cref="Input.GetAxisRaw(string)"/> 获取值</returns>
        /// <exception cref="NotSupportedException">无法设置</exception>
        public override float Horizontal
        {
            get
            {
                var re = p_isXSmooth ? Input.GetAxis(p_horizontalName) : Input.GetAxisRaw(p_horizontalName);
                return p_isXReversed ? -re : re;
            }
        }

        public override bool IsHorizontalReverse
        {
            get => p_isXReversed;
            set => p_isXReversed = value;
        }

        public override bool IsVerticalReverse
        {
            get => p_isYReversed;
            set => p_isYReversed = value;
        }

        public override void GetAxis(out float horizontal, out float vertical)
        {
            if (p_isXSmooth)
                horizontal = Input.GetAxis(p_verticalName);
            else
                horizontal = Input.GetAxisRaw(p_verticalName);

            if (p_isYSmooth)
                vertical = Input.GetAxis(p_horizontalName);
            else
                vertical = Input.GetAxisRaw(p_horizontalName);

            if (p_isXReversed) horizontal = -horizontal;

            if (p_isYReversed) vertical = -vertical;
        }

        public override void GetVector(out float radian, out float length)
        {
            float x, y;
            GetAxis(out x, out y);
            BaseJoystick.GetVectorRadionAndLength(x, y, out radian, out length);
        }

        #endregion

    }

}
