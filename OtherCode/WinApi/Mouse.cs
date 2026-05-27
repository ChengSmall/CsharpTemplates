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

        [DllImport(apiName, SetLastError = true)]
        public extern static ushort GetAsyncKeyState(int key);

        /// <summary>
        /// 合成键击、鼠标动作和按钮单击
        /// </summary>
        /// <remarks>
        /// <para>此函数受 UIPI 约束，仅允许应用程序将输入注入到完整性级别相等或更低级别的应用程序</para>
        /// <para>函数将 <see cref="INPUT"/> 结构中的事件串行插入键盘或鼠标输入流<br/>
        /// 这些事件不会与用户 (键盘或鼠标) 插入的其他键盘或鼠标输入事件，或者通过调用 keybd_event、 <see cref="mouse_event(uint, int, int, uint, void*)"/> 或 对 <see cref="SendInput(uint, INPUT*, int)"/>的其他调用插入</para>
        /// <para>
        /// 此函数不会重置键盘的当前状态<br/>
        /// 调用函数时已按下的任何键都可能会干扰此函数生成的事件<br/>
        /// 若要避免此问题，请使用<see cref="GetAsyncKeyState(int)"/>函数检查键盘的状态，并根据需要进行更正
        /// </para>
        /// </remarks>
        /// <param name="nInputs">数组元素数量</param>
        /// <param name="pInputs">数组，每个元素都表示要插入键盘或鼠标输入流的事件</param>
        /// <param name="cbSize">数组每个元素的大小</param>
        /// <returns>
        /// <para>返回成功插入键盘或鼠标输入流的事件数</para>
        /// <para>如果函数返回零，则表示输入已被另一个线程阻止，使用<see cref="Marshal.GetLastWin32Error"/>获取错误代码</para>
        /// </returns>
        [DllImport(apiName, SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT* pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            /// <summary>
            /// 输入事件的类型
            /// </summary>
            public uint type;

            /// <summary>
            /// 时间参数
            /// </summary>
            public INPUTUNION u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {

            /// <summary>
            /// 虚拟键码
            /// </summary>
            public ushort wVk;

            /// <summary>
            /// 扫描码
            /// </summary>
            public ushort wScan;

            /// <summary>
            /// 按键击发的状态
            /// </summary>
            /// <remarks>
            /// <para>
            /// 可有以下参数：
            /// <para>KEYEVENTF_EXTENDEDKEY: 0x0001 = 指定后<see cref="wScan"/>扫描代码由两个字节组成的序列组成，其中第一个字节的值为0xE0</para>
            /// <para>KEYEVENTF_KEYUP: 0x0002 = 如果指定则释放；未指定则按下</para>
            /// <para>KEYEVENTF_UNICODE: 0x0004 = 如果指定，系统会合成 VK_PACKET 击键；<see cref="wVk"/>参数必须为零，此标志只能与 KEYEVENTF_KEYUP 标志组合使用</para>
            /// </para>
            /// <para>KEYEVENTF_SCANCODE: 0x0008 = 如果指定，将识别<see cref="wScan"/>而不是<see cref="wVk"/></para>
            /// </remarks>
            public uint dwFlags;

            /// <summary>
            /// 事件的时间戳（以毫秒为单位）
            /// </summary>
            /// <remarks>如果此参数为零，则系统将提供自己的时间戳</remarks>
            public uint time;

            /// <summary>
            /// 与击键关联的附加值
            /// </summary>
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 鼠标事件
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {

            public int dx;

            public int dy;

            public uint mouseData;

            public uint dwFlags;

            public uint time;

            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 由键盘或鼠标以外的输入设备生成的模拟消息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;

            public ushort wParamL;

            public ushort wParamH;
        }

    }

}
