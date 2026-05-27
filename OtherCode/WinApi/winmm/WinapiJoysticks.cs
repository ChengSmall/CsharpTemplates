using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi.Winmm
{
    
    public unsafe static partial class WinapiJoysticks
    {

        /// <summary>
        /// 按钮状态枚举
        /// </summary>
        [Flags]
        public enum ButtonState : uint
        {
            /// <summary>
            /// 第1个按钮
            /// </summary>
            Button1 = 0x1,
            /// <summary>
            /// 第2个按钮
            /// </summary>
            Button2 = 0x2,
            /// <summary>
            /// 第3个按钮
            /// </summary>
            Button3 = 0x4,
            /// <summary>
            /// 第4个按钮
            /// </summary>
            Button4 = 0x8,

            Button1CHG = 0x0100,
            Button2CHG = 0x0200,
            Button3CHG = 0x0400,
            Button4CHG = 0x0800,

            Button5 = 0x00000010,
            Button6 = 0x00000020,
            Button7 = 0x00000040,
            Button8 = 0x00000080,
            Button9 = 0x00000100,
            Button10 = 0x00000200,
            Button11 = 0x00000400,
            Button12 = 0x00000800,
            Button13 = 0x00001000,
            Button14 = 0x00002000,
            Button15 = 0x00004000,
            Button16 = 0x00008000,
            Button17 = 0x00010000,
            Button18 = 0x00020000,
            Button19 = 0x00040000,
            Button20 = 0x00080000,
            Button21 = 0x00100000,
            Button22 = 0x00200000,
            Button23 = 0x00400000,
            Button24 = 0x00800000,
            Button25 = 0x01000000,
            Button26 = 0x02000000,
            Button27 = 0x04000000,
            Button28 = 0x08000000,
            Button29 = 0x10000000,
            Button30 = 0x20000000,
            Button31 = 0x40000000,
            Button32 = 0x80000000,
        }

        /// <summary>
        /// 摇杆状态
        /// </summary>
        public unsafe struct JOYINFO
        {

            /// <summary>
            /// 当前X坐标
            /// </summary>
            public uint wXpos;
            /// <summary>
            /// 当前Y坐标
            /// </summary>
            public uint wYpos;
            /// <summary>
            /// 当前Z坐标
            /// </summary>
            public uint wZpos;
            /// <summary>
            /// 由一个或多个位域值描述的操纵手柄按钮的当前状态，可通过<see cref="ButtonState"/>转化
            /// </summary>
            public uint wButtons;
        }

        [Flags]
        public enum JoyFlags : uint
        {

            /// <summary>
            /// 相当于设置除<see cref="RETURNRAWDATA"/>之外的所有RETURN位
            /// </summary>
            RETURNALL = RETURNX | RETURNY | RETURNZ | RETURNR | RETURNU | RETURNV | RETURNPOV | RETURNBUTTONS,
            /// <summary>
            /// dwButtons成员包含有关每个操纵杆按钮状态的有效信息
            /// </summary>
            RETURNBUTTONS = 0x80,
            /// <summary>
            /// 将操纵手柄空档位置居中到每个移动轴的中间值
            /// </summary>
            RETURNCENTERED = 0x400,
            /// <summary>
            /// dwPOV成员包含有关视点控件的有效信息，以离散单位表示
            /// </summary>
            RETURNPOV = 0x40,
            /// <summary>
            /// dwPOV成员包含有关以连续的百分之一度单位表示的视点控件的有效信息
            /// </summary>
            RETURNPOVCTS = 0x200,
            /// <summary>
            /// dwRpos成员包含有效的方向舵踏板数据。此信息表示另一个（第四个）轴
            /// </summary>
            RETURNR = 0x8,
            /// <summary>
            /// 此结构中存储的数据是未校准的操纵杆读数
            /// </summary>
            RETURNRAWDATA = 0x100,
            /// <summary>
            /// dwUpos成员包含操纵杆第五个轴的有效数据（如果该轴可用），否则返回零
            /// </summary>
            RETURNU = 0x10,
            /// <summary>
            /// dwVpos成员包含操纵杆第六个轴的有效数据（如果该轴可用），否则返回零
            /// </summary>
            RETURNV = 0x20,
            /// <summary>
            /// dwXpos成员包含操纵杆x坐标的有效数据
            /// </summary>
            RETURNX = 0x1,
            /// <summary>
            /// dwXpos成员包含操纵杆y坐标的有效数据
            /// </summary>
            RETURNY = 0x2,
            /// <summary>
            /// dwXpos成员包含操纵杆z坐标的有效数据
            /// </summary>
            RETURNZ = 0x4,


        }

        /// <summary>
        /// 更多按钮摇杆的状态
        /// </summary>
        public unsafe struct JOYINFOEX
        {
            #region
            /// <summary>
            /// 结构大小
            /// </summary>
            /// <remarks>值还用于在传递给joyGetPosEx函数时标识结构的版本号</remarks>
            public uint dwSize;
            /// <summary>
            /// 指示在此结构中返回的有效信息的标志
            /// </summary>
            /// <remarks>不包含有效信息的成员被设置为零</remarks>
            public uint dwFlags;
            /// <summary>
            /// 当前X坐标
            /// </summary>
            /// <remarks></remarks>
            public uint dwXpos;
            /// <summary>
            /// 当前Y坐标
            /// </summary>
            /// <remarks></remarks>
            public uint dwYpos;
            /// <summary>
            /// 当前Z坐标
            /// </summary>
            /// <remarks></remarks>
            public uint dwZpos;
            /// <summary>
            /// 方向舵或第四操纵杆轴的当前位置
            /// </summary>
            /// <remarks></remarks>
            public uint dwRpos;
            /// <summary>
            /// 当前第五轴位置
            /// </summary>
            /// <remarks></remarks>
            public uint dwUpos;
            /// <summary>
            /// 当前第六轴位置
            /// </summary>
            /// <remarks></remarks>
            public uint dwVpos;
            /// <summary>
            /// 32个操纵手柄按钮的当前状态
            /// </summary>
            /// <remarks>该成员的值可以设置为JOY_BUTTON n标志的任何组合，其中n是与按下的按钮相对应的1到32范围内的值。</remarks>
            public uint dwButtons;
            /// <summary>
            /// 当前按下的按钮编号
            /// </summary>
            /// <remarks></remarks>
            public uint dwButtonNumber;
            /// <summary>
            /// 视图控件的当前位置
            /// </summary>
            /// <remarks>此成员的值在0到35900之间，这些值表示每个视图的角度（以度为单位）乘以100。</remarks>
            public uint dwPOV;
            /// <summary>
            /// 保留
            /// </summary>
            /// <remarks></remarks>
            public uint dwReserved1;
            /// <summary>
            /// 保留
            /// </summary>
            /// <remarks></remarks>
            public uint dwReserved2;
            #endregion
        }

        /// <summary>
        /// 摇杆参数
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct JOYCAPS
        {

            #region 参数
            const int MAXPNAMELEN = 32;
            const int MAX_JOYSTICKOEMVXDNAME = 260;

            /// <summary>
            /// 制造商标识符
            /// </summary>
            public ushort wMid;
            /// <summary>
            /// 产品标识符
            /// </summary>
            public ushort wPid;
            /// <summary>
            /// 包含游戏杆产品名称的以Null结尾的字符串
            /// </summary>
            public fixed char szPname[MAXPNAMELEN];
            /// <summary>
            /// 最小 X 坐标
            /// </summary>
            public uint wXmin;
            /// <summary>
            /// 最大 X 坐标
            /// </summary>
            public uint wXmax;
            /// <summary>
            /// 最小 Y 坐标
            /// </summary>
            public uint wYmin;
            /// <summary>
            /// 最大 Y 坐标
            /// </summary>
            public uint wYmax;
            /// <summary>
            /// 最小 Z 坐标
            /// </summary>
            public uint wZmin;
            /// <summary>
            /// 最大 Z 坐标
            /// </summary>
            public uint wZmax;
            /// <summary>
            /// 游戏杆按钮数
            /// </summary>
            public uint wNumButtons;
            /// <summary>
            /// 由 joySetCapture 函数捕获时支持的最小轮询频率
            /// </summary>
            public uint wPeriodMin;
            /// <summary>
            /// 由 joySetCapture 函数捕获时支持的最大轮询频率
            /// </summary>
            public uint wPeriodMax;
            /// <summary>
            /// 最小 rudder 值 方向舵是第四个运动轴
            /// </summary>
            public uint wRmin;
            /// <summary>
            /// 最大 rudder 值 方向舵是第四个运动轴
            /// </summary>
            public uint wRmax;
            /// <summary>
            /// 最小 u 坐标 (第五个轴) 值
            /// </summary>
            public uint wUmin;
            /// <summary>
            /// 最大 u 坐标 (第五个轴) 值
            /// </summary>
            public uint wUmax;
            /// <summary>
            /// (第六个轴的最小 v 坐标) 值
            /// </summary>
            public uint wVmin;
            /// <summary>
            /// (第六个轴的最大 v 坐标) 值
            /// </summary>
            public uint wVmax;
            /// <summary>
            /// 游戏杆功能
            /// </summary>
            public uint wCaps;
            /// <summary>
            /// 游戏杆支持的最大轴数
            /// </summary>
            public uint wMaxAxes;
            /// <summary>
            /// 游戏杆当前使用的轴数
            /// </summary>
            public uint wNumAxes;
            /// <summary>
            /// 游戏杆支持的最大按钮数
            /// </summary>
            public uint wMaxButtons;
            /// <summary>
            /// 包含游戏杆注册表项的 Null 终止字符串
            /// </summary>
            public fixed char szRegKey[MAXPNAMELEN];
            /// <summary>
            /// 以 Null 结尾的字符串，标识游戏杆驱动程序 OEM
            /// </summary>
            public fixed char szOEMVxD[MAX_JOYSTICKOEMVXDNAME];
            #endregion

        }

        /// <summary>
        /// 获取操纵杆位置和按钮状态
        /// </summary>
        /// <param name="uJoyID">只有一个手柄时传入0</param>
        /// <param name="pji">JOYINFOEX 结构体</param>
        /// <returns>0则是获取到了摇杆</returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetPosEx(int uJoyID, JOYINFOEX* pji);

        /// <summary>
        /// 查询游戏杆以确定其功能
        /// </summary>
        /// <param name="joyID">要查询的游戏杆的标识符。 uJoyID 的有效值范围为 -1 到 15。 如果值为 -1，则无论设备是否存在，都允许检索 JOYCAPS 结构的 szRegKey 成员</param>
        /// <param name="lpjoycaps">返回摇杆参数</param>
        /// <param name="uSize">结构的大小</param>
        /// <returns>0表示成功</returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetDevCaps(UIntPtr joyID, ref JOYCAPS lpjoycaps, int uSize);

        /// <summary>
        /// 使用窗口消息捕获摇杆状态
        /// </summary>
        /// <remarks></remarks>
        /// <param name="hwnd">用于接收游戏杆消息的窗口的句柄</param>
        /// <param name="uJoyID">要捕获的游戏杆的标识符 有效值范围为0到15</param>
        /// <param name="uPeriod">轮询频率（以毫秒为单位）</param>
        /// <param name="fChanged">更改位置标志
        /// <para>指定为true，以便仅当位置更改的值大于游戏杆移动阈值时发送消息。 否则，将按 uPeriod 中指定的轮询频率发送消息</para>
        /// </param>
        /// <returns>成功返回0，否则返回错误代码</returns>
        [DllImport("winmm.dll")]
        public static extern int joySetCapture(IntPtr hwnd, uint uJoyID, uint uPeriod, [MarshalAs(UnmanagedType.Bool)] bool fChanged);

        /// <summary>
        /// 释放指定的捕获游戏杆
        /// </summary>
        /// <param name="uJoyID">要释放游戏杆的标识符</param>
        /// <returns>成功返回0，否则返回错误代码</returns>
        [DllImport("winmm.dll")]
        public static extern int joyReleaseCapture(uint uJoyID);

    }

}
