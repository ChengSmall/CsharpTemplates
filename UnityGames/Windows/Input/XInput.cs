#if UNITY_STANDALONE_WIN

using System;

using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;

using Cheng.DataStructure.Cherrsdinates;
using Cheng.DataStructure;
using Cheng.Algorithm;

namespace Cheng.Unitys.Windows.XInput
{

    /// <summary>
    /// XInput控制器枚举
    /// </summary>
    [Flags]
    public enum GamePadIndex : byte
    {

        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 表示索引0的控制器
        /// </summary>
        Index_0 = 0b1,

        /// <summary>
        /// 表示索引1的控制器
        /// </summary>
        Index_1 = 0b10,

        /// <summary>
        /// 表示索引2的控制器
        /// </summary>
        Index_2 = 0b100,

        /// <summary>
        /// 表示索引3的控制器
        /// </summary>
        Index_3 = 0b1000
    }

    /// <summary>
    /// 按钮状态
    /// </summary>
    [Flags]
    public enum GamePadButtons : ushort
    {

        /// <summary>
        /// 无按键
        /// </summary>
        None = 0,

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
        /// 开始键（Start）
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
        /// 左肩键
        /// </summary>
        LeftShoulder = 0x0100,

        /// <summary>
        /// 右肩键
        /// </summary>
        RightShoulder = 0x0200,

        /// <summary>
        /// 按键 A
        /// </summary>
        A = 0x1000,

        /// <summary>
        /// 按键 B
        /// </summary>
        B = 0x2000,

        /// <summary>
        /// 按键 X
        /// </summary>
        X = 0x4000,

        /// <summary>
        /// 按键 Y
        /// </summary>
        Y = 0x8000
    }

    /// <summary>
    /// XInput控制器状态参数
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct XInputState
    {

        #region 参数

        /// <summary>
        /// 检查是否更新的值
        /// </summary>
        /// <remarks>状态包编号；如果手柄状态有更新，则该值与上次的值不同</remarks>
        public readonly uint UpdateNumber;

        /// <summary>
        /// 控制器参数
        /// </summary>
        public readonly XInputGamePad Gamepad;

        #endregion

    }

    /// <summary>
    /// 用于控制振动的电机马达
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct XInputVibration
    {

        #region

        /// <summary>
        /// 初始化震动马达值
        /// </summary>
        /// <param name="leftMotorSpeed">左电机（低频）的速度，范围在[0,65535]</param>
        /// <param name="rightMotorSpeed">右电机（高频）速度，范围在[0,65535]</param>
        public XInputVibration(ushort leftMotorSpeed, ushort rightMotorSpeed)
        {
            this.leftMotorSpeed = leftMotorSpeed;
            this.rightMotorSpeed = rightMotorSpeed;
        }

        /// <summary>
        /// 初始化震动马达值，使用归一化设置
        /// </summary>
        /// <param name="leftMotorSpeed">左电机（低频）的速度，范围在[0,1]</param>
        /// <param name="rightMotorSpeed">右电机（低频）的速度，范围在[0,1]</param>
        public XInputVibration(float leftMotorSpeed, float rightMotorSpeed)
        {
            this.leftMotorSpeed = (ushort)(leftMotorSpeed * 65535).Clamp(0, 65535);
            this.rightMotorSpeed = (ushort)(rightMotorSpeed * 65535).Clamp(0, 65535);
        }

        /// <summary>
        /// 左电机（低频）的速度
        /// </summary>
        /// <remarks>有效值在 0 到 65535 的范围内；0表示不使用电机；65535 表示 100% 使用电机</remarks>
        public readonly ushort leftMotorSpeed;

        /// <summary>
        /// 右电机（高频）速度
        /// </summary>
        /// <remarks>有效值在 0 到 65535 的范围内；0表示不使用电机；65535 表示 100% 使用电机</remarks>
        public readonly ushort rightMotorSpeed;

        /// <summary>
        /// 左电机（低频）的速度
        /// </summary>
        /// <value>范围在[0,1]</value>
        public float LeftMotorSpeedF
        {
            get => leftMotorSpeed / 65535f;
        }

        /// <summary>
        /// 右电机（高频）的速度
        /// </summary>
        /// <value>范围在[0,1]</value>
        public float RightMotorSpeedF
        {
            get => rightMotorSpeed / 65535f;
        }

        #endregion

    }

    /// <summary>
    /// XInput控制器参数
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct XInputGamePad
    {

        #region

        private readonly WindowsXInput.Win32api.XINPUT_GamePad p;

        #endregion

        #region 参数访问

        /// <summary>
        /// 手柄按钮状态
        /// </summary>
        public GamePadButtons Buttons
        {
            get => (GamePadButtons)p.wButtons;
        }

        /// <summary>
        /// 手柄按钮状态（16位整数值）
        /// </summary>
        public ushort ButtonsInt
        {
            get => p.wButtons;
        }

        /// <summary>
        /// 判断指定按钮状态
        /// </summary>
        /// <param name="button">要判断的状态</param>
        /// <returns>状态是1返回true，0返回false</returns>
        public bool IsButton(GamePadButtons button)
        {
            return ((GamePadButtons)p.wButtons & button) == (button);
        }

        static float GetJoystickF(short value)
        {
            if (value == 0) return 0f;
            if (value < 0)
            {
                return value / 32768f;
            }
            return value / 32767f;
        }

        static double GetJoystickD(short value)
        {
            if (value == 0) return 0D;
            if (value < 0)
            {
                return value / 32768D;
            }
            return value / 32767D;
        }

        /// <summary>
        /// 左扳机（LT）力度，范围[0,1]
        /// </summary>
        public float LeftTrigger => p.bLeftTrigger / 255f;

        /// <summary>
        /// 右扳机（RT）力度，范围[0,1]
        /// </summary>
        public float RightTrigger => p.bRightTrigger / 255f;

        /// <summary>
        /// 左摇杆的x和y轴归一化值
        /// </summary>
        public Point2F LeftJoystick
        {
            get => new Point2F(GetJoystickF(p.sThumbLX), GetJoystickF(p.sThumbLY));
        }

        /// <summary>
        /// 左摇杆的x和y轴归一化值
        /// </summary>
        /// <value>范围区间在[0,1]的左摇杆值</value>
        public Point2 LeftJoystickD
        {
            get => new Point2(GetJoystickD(p.sThumbLX), GetJoystickD(p.sThumbLY));
        }

        /// <summary>
        /// 右摇杆的x和y轴归一化值
        /// </summary>
        /// <value>范围区间在[0,1]的右摇杆值</value>
        public Point2F RightJoystick
        {
            get => new Point2F(GetJoystickF(p.sThumbRX), GetJoystickF(p.sThumbRY));
        }

        /// <summary>
        /// 右摇杆的x和y轴归一化值
        /// </summary>
        public Point2 RightJoystickD
        {
            get => new Point2(GetJoystickD(p.sThumbRX), GetJoystickD(p.sThumbRY));
        }

        /// <summary>
        /// 左扳机（LT）力度值原始数据，范围[0,255]
        /// </summary>
        public byte LeftTriggerInt => p.bLeftTrigger;

        /// <summary>
        /// 右扳机（RT）力度值原始数据，范围[0,255]
        /// </summary>
        public byte RightTriggerInt => p.bRightTrigger;

        /// <summary>
        /// 左摇杆原始数据，每个轴的范围在[-32768,32767]
        /// </summary>
        public PointInt2 LeftJoystickInt
        {
            get => new PointInt2(p.sThumbLX, p.sThumbLY);
        }

        /// <summary>
        /// 右摇杆原始数据，每个轴的范围在[-32768,32767]
        /// </summary>
        public PointInt2 RightJoystickInt
        {
            get => new PointInt2(p.sThumbRX, p.sThumbRY);
        }

        /// <summary>
        /// 左摇杆X轴原始数据，范围在[-32768,32767]
        /// </summary>
        public short ThumbLXInt => p.sThumbLX;

        /// <summary>
        /// 左摇杆Y轴原始数据，范围在[-32768,32767]
        /// </summary>
        public short ThumbLYInt => p.sThumbLY;

        /// <summary>
        /// 右摇杆X轴原始数据，范围在[-32768,32767]
        /// </summary>
        public short ThumbRXInt => p.sThumbRX;

        /// <summary>
        /// 右摇杆Y轴原始数据，范围在[-32768,32767]
        /// </summary>
        public short ThumbRYInt => p.sThumbRY;

        #endregion

    }

    /// <summary>
    /// 描述连接的控制器的功能
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct XInputCapabilities
    {

        #region 参数

        private readonly byte Type;

        private readonly byte SubType;

        private readonly ushort Flags;

        /// <summary>
        /// 控制器按键参数
        /// </summary>
        public readonly XInputGamePad GamePad;

        /// <summary>
        /// 控制器的震动电机马达
        /// </summary>
        public readonly XInputVibration Vibration;

        #endregion

    }

    /// <summary>
    /// 电池类型
    /// </summary>
    public enum BatteryType : byte
    {
        /// <summary>
        /// 设备未连接
        /// </summary>
        DisconNected = 0,

        /// <summary>
        /// 该设备是有线设备，没有电池
        /// </summary>
        Wired = 1,

        /// <summary>
        /// ALKALINE 该设备有一个碱电池
        /// </summary>
        AlkalineBattery = 2,

        /// <summary>
        /// 该设备具有镍氢电池
        /// </summary>
        NickelMetalHydrideBattery = 3,

        /// <summary>
        /// 设备的电池类型未知
        /// </summary>
        Unknown = 0xFF
    }

    /// <summary>
    /// 电量
    /// </summary>
    public enum BatteryLevel : byte
    {

        /// <summary>
        /// 空
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 低电量
        /// </summary>
        Low = 1,

        /// <summary>
        /// 中等电量 MEDIUM
        /// </summary>
        Medium = 2,

        /// <summary>
        /// 满电量
        /// </summary>
        Full = 3,
    }

    /// <summary>
    /// 设备电池信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct XInputBattery
    {

        #region 参数

        /// <summary>
        /// 电池类型
        /// </summary>
        public readonly BatteryType batteryType;

        /// <summary>
        /// 电池电量
        /// </summary>
        public readonly BatteryLevel batteryLevel;

        #endregion

    }

    /// <summary>
    /// 访问 windows XInput 控制器
    /// </summary>
    public static unsafe class WindowsXInput
    {

        internal static class Win32api
        {

            [StructLayout(LayoutKind.Sequential)]
            public struct XINPUT_GamePad
            {
                public ushort wButtons;

                public byte bLeftTrigger;

                public byte bRightTrigger;

                public short sThumbLX;

                public short sThumbLY;

                public short sThumbRX;

                public short sThumbRY;
            }

            [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
            public static extern uint winapi_XInputGetState(int index, void* pState);

            [DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")]
            public static extern uint winapi_XInputSetState(int index, void* pVibration);

            [DllImport("xinput1_4.dll", EntryPoint = "XInputGetCapabilities")]
            public static extern uint winapi_XInputGetCapabilities(int index, uint dwFlags, void* pCapabilities);

            [DllImport("xinput1_4.dll", EntryPoint = "XInputGetBatteryInformation")]
            public static extern uint winapi_XInputGetBatteryInformation(int index, byte devType, void* pBatteryInformation);

        }

        /// <summary>
        /// 获取控制器状态
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <param name="state">用于接收手柄状态的引用</param>
        /// <returns>返回0表示成功，其他值代表错误代码</returns>
        public static int TryGetState(int index, out XInputState state)
        {
            uint re;
            state = default;
            fixed (XInputState* ptr = &state)
            {
                re = Win32api.winapi_XInputGetState(index, ptr);
            }
            return (int)re;
        }

        /// <summary>
        /// 获取控制器状态
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <returns>控制器状态</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="Win32Exception">控制器未连接或出现其它无法获取控制器状态的错误</exception>
        public static XInputState GetState(int index)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException();
            XInputState state;
            var re = Win32api.winapi_XInputGetState(index, &state);
            if (re == 0) return state;
            throw new Win32Exception((int)re);
        }

        /// <summary>
        /// 设置控制器状态
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <param name="vibration">要设置的电机参数</param>
        /// <returns>返回0表示成功，其他值代表错误代码</returns>
        public static int TrySetState(int index, XInputVibration vibration)
        {
            return (int)Win32api.winapi_XInputSetState(index, &vibration);
        }

        /// <summary>
        /// 设置控制器状态
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <param name="vibration">要设置的电机参数</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="Win32Exception">控制器未连接或出现其它无法获取控制器状态的错误</exception>
        public static void SetState(int index, XInputVibration vibration)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException();
            var re = Win32api.winapi_XInputSetState(index, &vibration);
            if (re != 0) throw new Win32Exception((int)re);
        }

        /// <summary>
        /// 查看指定索引是否已连接
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <param name="error">访问数据后返回的错误码</param>
        /// <returns>true表示指定索引的控制器能够正常访问，false表示无法访问</returns>
        public static bool Connected(int index, out uint error)
        {
            error = 0;
            if (index < 0 || index > 3) return false;
            XInputState state;
            error = Win32api.winapi_XInputGetState(index, &state);
            return error == 0;
        }

        /// <summary>
        /// 查看已连接的控制器
        /// </summary>
        /// <param name="count">已连接的控制器数量</param>
        /// <returns>当前已连接的控制器枚举</returns>
        public static GamePadIndex ConnectedAll(out int count)
        {
            XInputState state;
            uint re;
            count = 0;
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                re = Win32api.winapi_XInputGetState(i, &state);
                if (re == 0)
                {
                    index |= (1 << (i));
                    count++;
                }
            }
            return (GamePadIndex)index;
        }

        /// <summary>
        /// 查看已连接的控制器
        /// </summary>
        /// <returns>当前已连接的控制器枚举</returns>
        public static GamePadIndex ConnectedAll()
        {
            return ConnectedAll(out _);
        }

        /// <summary>
        /// 检索连接的控制器的功能和特性
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <param name="capabilities">指向接收控制器功能的引用</param>
        /// <returns>如果成功，返回0；失败返回错误代码</returns>
        public static int TryGetCapabilities(int index, out XInputCapabilities capabilities)
        {
            capabilities = default;
            fixed (void* ptr = &capabilities)
            {
                return (int)Win32api.winapi_XInputGetCapabilities(index, 0, ptr);
            }
        }

        /// <summary>
        /// 检索连接的控制器的功能和特性
        /// </summary>
        /// <param name="index">用户控制器的索引</param>
        /// <returns>控制器的功能和特性</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="Win32Exception">控制器未连接或出现其它无法获取控制器状态的错误</exception>
        public static XInputCapabilities GetCapabilities(int index)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException();
            XInputCapabilities ca;
            var re = Win32api.winapi_XInputGetCapabilities(index, 0, &ca);
            if (re != 0) throw new Win32Exception((int)re);
            return ca;
        }

        /// <summary>
        /// 检索无线控制器的电池类型和充电状态
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <param name="batteryInformation">用于接收电池信息结构的引用</param>
        /// <returns>如果成功，返回0；失败返回错误代码</returns>
        public static int TryGetBatteryInformation(int index, out XInputBattery batteryInformation)
        {
            batteryInformation = default;
            fixed (void* ptr = &batteryInformation)
            {
                return (int)Win32api.winapi_XInputGetBatteryInformation(index, 0, ptr);
            }
        }

        /// <summary>
        /// 检索无线控制器的电池类型和充电状态
        /// </summary>
        /// <param name="index">控制器索引[0,3]，对应第1到第4个手柄</param>
        /// <returns>电池信息结构</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="Win32Exception">控制器未连接或出现其它无法获取控制器状态的错误</exception>
        public static XInputBattery GetBatteryInformation(int index)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException();
            XInputBattery b = default;
            var re = Win32api.winapi_XInputGetBatteryInformation(index, 0, &b);
            if (re != 0) throw new Win32Exception((int)re);
            return b;
        }

    }
}

#endif