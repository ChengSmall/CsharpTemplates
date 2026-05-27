using System;
using System.IO;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Reflection;

namespace Cheng.OtherCode.Winapi
{


    internal static unsafe class Win32XInput
    {

        /// <summary>
        /// 手柄按钮的枚举，对应 XInput 的宏定义
        /// </summary>
        [Flags]
        public enum GamePadButtons : ushort
        {

            /// <summary>
            /// 方向键 - 上
            /// </summary>
            DPadUp = 0x0001,

            /// <summary>
            /// 方向键 - 下
            /// </summary>
            DPadDown = 0x0002,

            /// <summary>
            /// 方向键 -  左
            /// </summary>
            DPadLeft = 0x0004,

            /// <summary>
            /// 方向键 - 右
            /// </summary>
            DPadRight = 0x0008,

            /// <summary>
            /// 开始建（Start）
            /// </summary>
            Start = 0x0010,

            /// <summary>
            /// 返回键（Back）
            /// </summary>
            Back = 0x0020,

            /// <summary>
            /// 左摇杆按压
            /// </summary>
            LeftThumb = 0x0040,

            /// <summary>
            /// 右摇杆按压
            /// </summary>
            RightThumb = 0x0080,

            /// <summary>
            /// 左肩扳机
            /// </summary>
            LeftShoulder = 0x0100,

            /// <summary>
            /// 右肩扳机
            /// </summary>
            RightShoulder = 0x0200,

            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        }

        /// <summary>
        /// 手柄游戏板的核心数据，包含所有按键和摇杆的瞬时状态
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_GAMEPAD
        {
            /// <summary>
            /// 按键状态，<see cref="GamePadButtons"/>枚举判断
            /// </summary>
            public ushort wButtons;

            /// <summary>
            /// 左扳机键 (LT)，值域 0-255
            /// </summary>
            public byte bLeftTrigger;

            /// <summary>
            /// 右扳机键 (RT)，值域 0-255
            /// </summary>
            public byte bRightTrigger;

            /// <summary>
            /// 左摇杆 X轴，值域 -32768 到 32767
            /// </summary>
            public short sThumbLX;

            /// <summary>
            /// 左摇杆 Y轴，值域 -32768 到 32767
            /// </summary>
            public short sThumbLY;

            /// <summary>
            /// 右摇杆 X轴，值域 -32768 到 32767
            /// </summary>
            public short sThumbRX;

            /// <summary>
            /// 右摇杆 Y轴，值域 -32768 到 32767
            /// </summary>
            public short sThumbRY;
        }

        /// <summary>
        /// 手柄的完整状态，包含一个包序号和游戏板数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_STATE
        {
            /// <summary>
            /// 状态包编号，可用于判断状态是否更新；如果手柄状态有更新，则该值与上次的值不同
            /// </summary>
            public uint dwPacketNumber;


            public XINPUT_GAMEPAD Gamepad;
        }

        /// <summary>
        /// （可选）用于控制振动的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_VIBRATION
        {

            /// <summary>
            /// 左电机（低频）的速度；有效值在 0 到 65535 的范围内。0表示不使用电机；65535 表示 100% 使用电机
            /// </summary>
            public ushort wLeftMotorSpeed;

            /// <summary>
            /// 右电机（高频）速度；有效值在 0 到 65535 的范围内。0表示不使用电机；65535 表示 100% 使用电机
            /// </summary>
            public ushort wRightMotorSpeed;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_CAPABILITIES
        {
            public byte Type;

            public byte SubType;

            public ushort Flags;

            public XINPUT_GAMEPAD Gamepad;

            public XINPUT_VIBRATION Vibration;

        }

        /// <summary>
        /// 导入获取状态的函数
        /// </summary>
        /// <param name="dwUserIndex">玩家索引 (0-3)，对应第1到第4个手柄</param>
        /// <param name="pState">用于接收手柄状态<see cref="XINPUT_STATE"/>的引用</param>
        /// <returns>返回0表示成功，其他值表示失败（如手柄未连接）</returns>
        [DllImport("xinput1_4.dll")]
        public static extern uint XInputGetState(int dwUserIndex, void* pState);

        /// <summary>
        /// 设置马达振动
        /// </summary>
        /// <param name="dwUserIndex"></param>
        /// <param name="pVibration">包含马达速度的结构体<see cref="XINPUT_VIBRATION"/></param>
        /// <returns></returns>
        [DllImport("xinput1_4.dll")]
        public static extern uint XInputSetState(int dwUserIndex, void* pVibration);

        /// <summary>
        /// 检索连接的控制器的功能和特性
        /// </summary>
        /// <param name="dwUserIndex">用户控制器的索引</param>
        /// <param name="dwFlags">识控制器类型的输入标志。如果此值为 0，则返回连接到系统的所有控制器的功能</param>
        /// <param name="pCapabilities">指向接收控制器功能的指针</param>
        /// <returns>返回0表示成功，其他值表示失败（如手柄未连接）</returns>
        [DllImport("xinput1_4.dll")]
        public static extern uint XInputGetCapabilities(int dwUserIndex, uint dwFlags, void* pCapabilities);

    }

}