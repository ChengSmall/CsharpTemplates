using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Resources;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Cheng.OtherCode.Winapi
{

    /// <summary>
    /// 鼠标事件
    /// </summary>
    public unsafe static class Mouse
    {

        public const string apiName = "user32.dll";

        /// <summary>
        /// 合成鼠标运动和按钮单击
        /// </summary>
        /// <remarks>
        /// <para></para>
        /// </remarks>
        /// <param name="dwFlags">控制鼠标运动和按钮单击的各个方面的枚举值</param>
        /// <param name="dx">
        /// <para>鼠标沿x轴的绝对位置或其自上次生成鼠标事件以来的运动量，具体取决于Absolute的设置</para>
        /// </param>
        /// <param name="dy">鼠标沿 y 轴的绝对位置或其自上次生成鼠标事件以来的运动量，具体取决于 Absolute的设置</param>
        /// <param name="dwData">
        /// <para>
        /// 如果<paramref name="dwFlags"/>包含<see cref="MouseEventFlags.Mouse_Wheel"/>，则 dwData 指定滚轮移动量<br/>
        /// 正值表示滚轮向前旋转，负值表示滚轮向后旋转； 一滚轮定义为120
        /// </para>
        /// <para>
        /// 如果<paramref name="dwFlags"/>包含<see cref="MouseEventFlags.Mouse_HWheel"/>，则 dwData 指定滚轮移动量<br/>
        /// 正值表示方向盘向右倾斜;负值表示方向盘向左倾斜
        /// </para>
        /// <para>
        /// 如果<paramref name="dwFlags"/>包含<see cref="MouseEventFlags.Mouse_XDown"/>或<see cref="MouseEventFlags.Mouse_XUp"/>，则该参数指定按下或释放的 X 按钮<br/>
        /// 值可以是 0x1（第一个x按钮） 和 0x2（第二个x按钮） 两种标志的位域组合
        /// </para>
        /// <para>如果<paramref name="dwFlags"/>不包含以上提到的几个参数，则该参数应设为0</para>
        /// </param>
        /// <param name="dwExtraInfo">
        /// 与鼠标事件关联的附加值，默认设为null
        /// </param>
        [DllImport(apiName)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, void* dwExtraInfo);

        /// <summary>
        /// 合成鼠标运动和按钮单击
        /// </summary>
        /// <remarks>
        /// <para></para>
        /// </remarks>
        /// <param name="dwFlags">控制鼠标运动和按钮单击的各个方面的枚举值</param>
        /// <param name="dx">
        /// <para>鼠标沿x轴的绝对位置或其自上次生成鼠标事件以来的运动量，具体取决于Absolute的设置</para>
        /// </param>
        /// <param name="dy">鼠标沿 y 轴的绝对位置或其自上次生成鼠标事件以来的运动量，具体取决于 Absolute的设置</param>
        /// <param name="dwData">
        /// <para>
        /// 如果<paramref name="dwFlags"/>包含<see cref="MouseEventFlags.Mouse_Wheel"/>，则 dwData 指定滚轮移动量<br/>
        /// 正值表示滚轮向前旋转，负值表示滚轮向后旋转； 一滚轮定义为120
        /// </para>
        /// <para>
        /// 如果<paramref name="dwFlags"/>包含<see cref="MouseEventFlags.Mouse_HWheel"/>，则 dwData 指定滚轮移动量<br/>
        /// 正值表示方向盘向右倾斜;负值表示方向盘向左倾斜
        /// </para>
        /// <para>
        /// 如果<paramref name="dwFlags"/>包含<see cref="MouseEventFlags.Mouse_XDown"/>或<see cref="MouseEventFlags.Mouse_XUp"/>，则该参数指定按下或释放的 X 按钮<br/>
        /// 值可以是 0x1（第一个x按钮） 和 0x2（第二个x按钮） 两种标志的位域组合
        /// </para>
        /// <para>如果<paramref name="dwFlags"/>不包含以上提到的几个参数，则该参数应设为0</para>
        /// </param>
        /// <param name="dwExtraInfo">
        /// 与鼠标事件关联的附加值，默认设为null
        /// </param>
        [DllImport(apiName, EntryPoint = "mouse_event")]
        public static extern void MouseEvent_Winapi(MouseEventFlags dwFlags, int dx, int dy, uint dwData, void* dwExtraInfo);

        /// <summary>
        /// 鼠标事件类型
        /// </summary>
        [Flags]
        public enum MouseEventFlags : uint
        {

            /// <summary>
            /// dx 和 dy 参数包含规范化的绝对坐标
            /// </summary>
            /// <remarks>
            /// <para>如果未设置，则这些参数包含相对数据；自上次报告位置以来的位置更改</para>
            /// <para>无论哪种类型的鼠标或类似鼠标的设备连接到系统，都可以设置或不设置此标志</para>
            /// </remarks>
            Absolute = 0x8000,

            /// <summary>
            /// 左按钮按下
            /// </summary>
            Mouse_LeftDown = 0x0002,

            /// <summary>
            /// 做按钮松开
            /// </summary>
            Mouse_LeftUp = 0x0004,

            /// <summary>
            /// 中间按钮按下
            /// </summary>
            Mouse_MiddleDown = 0x0020,

            /// <summary>
            /// 中间按钮松开
            /// </summary>
            Mouse_MiddleUp = 0x0040,

            /// <summary>
            /// 移动
            /// </summary>
            MouseMove = 0x0001,

            /// <summary>
            /// 右侧按钮已关闭
            /// </summary>
            Mouse_RightDown = 0x0008,

            /// <summary>
            /// 右侧按钮已打开
            /// </summary>
            Mouse_RightUp =
            0x0010,

            /// <summary>
            /// 鼠标滚轮移动
            /// </summary>
            /// <remarks>移动量在 dwData 参数中指定</remarks>
            Mouse_Wheel = 0x0800,

            /// <summary>
            /// 按下了 X 按钮。
            /// </summary>
            Mouse_XDown = 0x0080,

            /// <summary>
            /// 释放 X 按钮。
            /// </summary>
            Mouse_XUp = 0x0100,

            /// <summary>
            /// 滚轮按钮倾斜
            /// </summary>
            Mouse_HWheel = 0x01000

        }

    }

}
