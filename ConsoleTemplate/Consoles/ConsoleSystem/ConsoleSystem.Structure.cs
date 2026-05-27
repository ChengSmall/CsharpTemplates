using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Security;
using System.IO;

using SCol = global::System.Console;
using Cheng.Texts;


namespace Cheng.Consoles
{

    static unsafe partial class ConsoleSystem
    {

        #region 输入参数

        #region

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            public short X;
            public short Y;
        }

        /// <summary>
        /// 控制键状态位
        /// </summary>
        [Flags]
        public enum ControlKeyState : uint
        {

            /// <summary>
            /// 按下左Alt键
            /// </summary>
            LeftAlt = 0x0002,

            /// <summary>
            /// 按下右Alt键
            /// </summary>
            RightAlt = 0x0001,

            /// <summary>
            /// 按下左Ctrl键
            /// </summary>
            LeftCtrl = 0x0008,

            /// <summary>
            /// 按下右Ctrl键
            /// </summary>
            RightCtrl = 0x0004,

            /// <summary>
            /// 按下Shift
            /// </summary>
            Shift = 0x0010,

            /// <summary>
            /// Num Lock 已打开
            /// </summary>
            NumLockON = 0x0020,

            /// <summary>
            /// Scroll Lock 已打开
            /// </summary>
            ScrollLockON = 0x0040,

            /// <summary>
            /// 大写锁定 CapsLock 已开启
            /// </summary>
            CapsLockON = 0x0080,

            /// <summary>
            /// 属于增强型键盘
            /// </summary>
            /// <remarks>
            /// <para>IBM® 101 和 102 键键盘的增强键是键盘左侧的 INS、DEL、HOME、END、PAGE UP、PAGE DOWN，四个方向键，键盘上的除号 '/' 和 ENTER 键</para>
            /// </remarks>
            EnhancedKey = 0x0100,

        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEY_EVENT_RECORD
        {
            public uint bKeyDown;

            public ushort wRepeatCount;

            public ushort wVirtualKeyCode;

            public ushort wVirtualScanCode;

            public char UnicodeChar;

            public uint dwControlKeyState;
        }

        /// <summary>
        /// 鼠标事件的类型
        /// </summary>
        [Flags]
        public enum MouseEventFlags : uint
        {
            /// <summary>
            /// 按下或释放鼠标按钮
            /// </summary>
            MouseKey = 0,

            /// <summary>
            /// 修改鼠标位置
            /// </summary>
            MouseMoved = 0x0001,

            /// <summary>
            /// 双击的第二次单击
            /// </summary>
            DoubleClick = 0x0002,

            /// <summary>
            /// 垂直鼠标滚轮已移动
            /// </summary>
            MouseWheeled = 0x0004,

            /// <summary>
            /// 水平鼠标滚轮已移动
            /// </summary>
            MouseHWheeled = 0x0008,
        }

        /// <summary>
        /// 鼠标按钮的状态
        /// </summary>
        [Flags]
        public enum MouseButtonState
        {

            /// <summary>
            /// 最左侧的鼠标按钮
            /// </summary>
            FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,

            /// <summary>
            /// 左侧的第二个按钮
            /// </summary>
            FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,

            /// <summary>
            /// 左侧的第三个按钮
            /// </summary>
            FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,

            /// <summary>
            /// 左侧的第四个按钮
            /// </summary>
            FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,

            /// <summary>
            /// 最右侧的鼠标按钮
            /// </summary>
            RIGHTMOST_BUTTON_PRESSED = 0x0002
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MENU_EVENT_RECORD
        {
            public uint dwCommandId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FOCUS_EVENT_RECORD
        {
            public uint bSetFocus;
        }

        #endregion

        /// <summary>
        /// 事件类型
        /// </summary>
        [Flags]
        public enum EventType : ushort
        {
            /// <summary>
            /// 键盘按键消息
            /// </summary>
            Key = 0x0001,

            /// <summary>
            /// 鼠标，按钮，光标消息
            /// </summary>
            Mouse = 0x0002,

            /// <summary>
            /// 包含有关控制台屏幕缓冲区的新大小的信息
            /// </summary>
            WinBufferSize = 0x0004

            //MENU_EVENT = 0x0008,

            //FOCUS_EVENT = 0x0010
        }

        /// <summary>
        /// 控制台输入消息结构
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct InputRecord : IEquatable<InputRecord>
        {

            #region

            [FieldOffset(0)]
            internal EventType eventType;

            [FieldOffset(4)]
            internal KEY_EVENT_RECORD keyEvent;

            [FieldOffset(4)]
            internal MOUSE_EVENT_RECORD mouseEvent;

            [FieldOffset(4)]
            internal WINDOW_BUFFER_SIZE_RECORD windowBufferSizeEvent;

            [FieldOffset(4)]
            internal MENU_EVENT_RECORD menuEvent;

            [FieldOffset(4)]
            internal FOCUS_EVENT_RECORD focusEvent;

            #endregion

            #region 参数

            /// <summary>
            /// 消息事件类型，该参数决定了哪些成员有实际作用
            /// </summary>
            public EventType RecordEventType
            {
                get => eventType;
            }

            /// <summary>
            /// 确认当前消息是否存在指定类型
            /// </summary>
            /// <param name="type">要确认的类型参数</param>
            /// <returns>存在返回true，不存在返回false</returns>
            public bool IsEventType(EventType type)
            {
                return (eventType & type) == type;
            }

            #region 键盘

            /// <summary>
            /// 按键消息 - 按键按下还是释放
            /// </summary>
            /// <returns>true表示按下按键，false为释放按键</returns>
            public bool Key_KeyDown
            {
                get => keyEvent.bKeyDown != 0;
            }

            /// <summary>
            /// 按键消息 - 重复计数，表示按键被持续按住的状态
            /// </summary>
            /// <remarks>
            /// <para>例如：当按键被按住时，可能触发五次该成员值为1的事件，或一次该成员值为5的事件，也可能触发多次该成员值大于等于1的事件</para>
            /// </remarks>
            public int Key_RepeatCount
            {
                get
                {
                    return keyEvent.wRepeatCount;
                }
            }

            /// <summary>
            /// 按键消息 - 虚拟键码
            /// </summary>
            public int Key_VirtualCode
            {
                get => keyEvent.wVirtualKeyCode;
            }

            /// <summary>
            /// 按键消息 - 虚拟扫描码，代表键盘硬件生成的设备相关按键值
            /// </summary>
            public int Key_VirtualScanCode
            {
                get => keyEvent.wVirtualScanCode;
            }

            /// <summary>
            /// 按键消息 - 翻译后的Unicode字符
            /// </summary>
            public char Key_Char
            {
                get => keyEvent.UnicodeChar;
            }

            /// <summary>
            /// 按键消息 - 将虚拟键码其转换为<see cref="System.ConsoleKey"/>
            /// </summary>
            public System.ConsoleKey Key_ConsoleKey
            {
                get => (System.ConsoleKey)keyEvent.wVirtualKeyCode;
            }

            /// <summary>
            /// 控制键状态
            /// </summary>
            public ControlKeyState Key_ControlState
            {
                get => (ControlKeyState)keyEvent.dwControlKeyState;
            }

            #endregion

            #region 光标

            /// <summary>
            /// 光标事件 - 光标位置的水平坐标，以控制台屏幕缓冲区的字符单元格坐标为单位
            /// </summary>
            public int Mouse_PositionX
            {
                get => mouseEvent.dwMousePosition.X;
            }

            /// <summary>
            /// 光标事件 - 光标位置的垂直坐标，以控制台屏幕缓冲区的字符单元格坐标为单位
            /// </summary>
            public int Mouse_PositionY
            {
                get => mouseEvent.dwMousePosition.Y;
            }

            /// <summary>
            /// 光标事件 - 光标位置坐标，以控制台屏幕缓冲区的字符单元格坐标为单位
            /// </summary>
            public Cheng.DataStructure.Cherrsdinates.PointInt2 Mouse_Position
            {
                get => new DataStructure.Cherrsdinates.PointInt2(mouseEvent.dwMousePosition.X, mouseEvent.dwMousePosition.Y);
            }

            /// <summary>
            /// 光标事件 - 鼠标按钮状态
            /// </summary>
            public MouseButtonState Mouse_ButtonState
            {
                get => (MouseButtonState)mouseEvent.dwButtonState;
            }

            /// <summary>
            /// 光标事件 - 鼠标事件下的控制键状态
            /// </summary>
            public ControlKeyState Mouse_ControlState
            {
                get => (ControlKeyState)mouseEvent.dwControlKeyState;
            }

            /// <summary>
            /// 光标事件 - 鼠标事件类型
            /// </summary>
            public MouseEventFlags Mouse_EventFlags
            {
                get => (MouseEventFlags)mouseEvent.dwEventFlags;
            }

            #endregion

            #region 控制台缓冲区

            /// <summary>
            /// 控制台缓冲区消息 - 控制台屏幕缓冲区的长度，以字符单元格列和行为单位
            /// </summary>
            public int Console_SizeLength
            {
                get => this.windowBufferSizeEvent.dwSize.X;
            }

            /// <summary>
            /// 控制台缓冲区消息 - 控制台屏幕缓冲区的高度，以字符单元格列和行为单位
            /// </summary>
            public int Console_SizeHeight
            {
                get => this.windowBufferSizeEvent.dwSize.Y;
            }

            /// <summary>
            /// 控制台缓冲区消息 - 包含控制台屏幕缓冲区的大小，以字符单元格列和行为单位
            /// </summary>
            public Cheng.DataStructure.Cherrsdinates.PointInt2 Console_Size
            {
                get => new DataStructure.Cherrsdinates.PointInt2(windowBufferSizeEvent.dwSize.X, windowBufferSizeEvent.dwSize.Y);
            }

            #endregion

            #endregion

            #region 派生

            public static bool operator ==(InputRecord x, InputRecord y)
            {
                return Cheng.Memorys.MemoryOperation.EqualsMemory(&x, &y, sizeof(InputRecord));
            }

            public static bool operator !=(InputRecord x, InputRecord y)
            {
                return !Cheng.Memorys.MemoryOperation.EqualsMemory(&x, &y, sizeof(InputRecord));
            }

            public override bool Equals(object obj)
            {
                if(obj is InputRecord input) return this == input;
                return false;
            }

            public override int GetHashCode()
            {
                return (int)((this.keyEvent.bKeyDown | ((uint)this.keyEvent.wVirtualKeyCode | (uint)(this.keyEvent.wVirtualScanCode << 16))) ^ ((uint)this.keyEvent.UnicodeChar | ((uint)this.keyEvent.wRepeatCount << 16)));
            }

            public bool Equals(InputRecord other)
            {
                return this == other;
            }

            #endregion

        }

        #endregion

        #region SetConsoleMode

        /// <summary>
        /// 控制台输出缓冲区的输出模式参数
        /// </summary>
        [Flags]
        public enum OutputConsoleMode : uint
        {

            /// <summary>
            /// 控制台显示进行分析 ASCII 控制序列，并执行正确的操作
            /// </summary>
            EnableProcessedOutput = 0x0001,

            /// <summary>
            /// 当光标到达当前行的末尾时，它将移到下一行的开头
            /// </summary>
            /// <remarks>
            /// <para>这会导致控制台窗口中显示的行在光标前进到窗口的最后一行时自动向上滚动；还会导致控制台屏幕缓冲区的内容在光标前进到控制台屏幕缓冲区的最后一行时向上滚动（<![CDATA[../]]>丢弃控制台屏幕缓冲区的顶行）</para>
            /// <para>如果禁用此模式，则将覆盖该行中的最后一个字符以及后面的任何字符</para>
            /// </remarks>
            EnableWrapAtEOLOutput = 0x0002,

            /// <summary>
            /// 开启虚拟终端
            /// </summary>
            /// <remarks>
            /// <para>让控制台能够识别并处理<![CDATA[VT100/xterm]]>兼容的ANSI转义序列。</para>
            /// <para>在Windows 10周年更新（版本1511）之前，控制台默认禁用此功能</para>
            /// </remarks>
            EnableVirtualTerminalProcessing = 0x0004,

            /// <summary>
            /// 禁用到达行末时的自动回车行为
            /// </summary>
            /// <remarks>
            /// <para>设置此标志后，控制台禁用到达行末时的自动回车行为。光标会停留在最后一列，直到遇到明确的换行或回车指令</para>
            /// <para>在未设置此标志时，向控制台输出文本时，如果文本到达控制台缓冲区的最后一列，光标会自动移到下一行的开头。若之后紧跟换行符'\n'，就会多出一个空行</para>
            /// </remarks>
            DisableNewLineAutoReturn = 0x0008,

            /// <summary>
            /// 用于写入字符属性的 API 允许使用来自字符属性的标志调整文本的前景色和背景色
            /// </summary>
            /// <remarks>
            /// <para>
            /// 用于写入字符属性（包括 WriteConsoleOutput 和 WriteConsoleOutputAttribute 函数）的 API 允许使用来自字符属性的标志调整文本的前景色和背景色<br/>
            /// 此外，使用 COMMON_LVB 前缀指定了 DBCS 标志范围。 过去，这些标志仅在中文、日语和韩语的 DBCS 代码页中起作用
            /// </para>
            /// <para>除前导字节和尾随字节标志以外，其余描述线条绘制和反向显示（<![CDATA[../]]>前景色和背景色转换）的标志对于其他语言很有用，可用于强调输出的各个部分</para>
            /// <para>如果设置此控制台模式标志，则将允许在每种语言的每个代码页中使用这些属性</para>
            /// <para>默认情况下，此标志处于禁用状态，以保持与过去利用控制台的已知应用程序的兼容性，控制台会忽略非 CJK 计算机上的这些标志，以存储这些位字段，供自己使用或发生意外时使用</para>
            /// <para>请注意，使用 <see cref="EnableVirtualTerminalProcessing"/> 模式可能会导致设置 LVB 的网格和反向显示标志，而如果附加应用程序通过控制台虚拟终端序列请求下划线或反向显示，则此标志仍会处于禁用状态</para>
            /// </remarks>
            EnableLVBGridWorldWide = 0x0010,

        }

        #endregion

    }
}