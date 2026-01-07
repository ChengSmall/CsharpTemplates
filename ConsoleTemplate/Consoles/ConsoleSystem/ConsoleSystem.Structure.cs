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

            #region 鼠标

            /// <summary>
            /// 鼠标事件 - 光标位置的水平坐标，以控制台屏幕缓冲区的字符单元格坐标为单位
            /// </summary>
            public int Mouse_PositionX
            {
                get => mouseEvent.dwMousePosition.X;
            }

            /// <summary>
            /// 鼠标事件 - 光标位置的垂直坐标，以控制台屏幕缓冲区的字符单元格坐标为单位
            /// </summary>
            public int Mouse_PositionY
            {
                get => mouseEvent.dwMousePosition.Y;
            }

            /// <summary>
            /// 鼠标事件 - 光标位置坐标，以控制台屏幕缓冲区的字符单元格坐标为单位
            /// </summary>
            public Cheng.DataStructure.Cherrsdinates.PointInt2 Mouse_Position
            {
                get => new DataStructure.Cherrsdinates.PointInt2(mouseEvent.dwMousePosition.X, mouseEvent.dwMousePosition.Y);
            }

            /// <summary>
            /// 鼠标事件 - 鼠标按钮状态
            /// </summary>
            public MouseButtonState Mouse_ButtonState
            {
                get => (MouseButtonState)mouseEvent.dwButtonState;
            }

            /// <summary>
            /// 鼠标事件 - 鼠标事件下的控制键状态
            /// </summary>
            public ControlKeyState Mouse_ControlState
            {
                get => (ControlKeyState)mouseEvent.dwControlKeyState;
            }

            /// <summary>
            /// 鼠标事件 - 鼠标事件类型
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

    }
}