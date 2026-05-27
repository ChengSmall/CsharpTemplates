using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Windows.MIDI
{

    /// <summary>
    /// 函数<see cref="WinAPI.midiOutOpen(IntPtr*, uint, void*, void*, uint)"/>的 fdwOpen 参数
    /// </summary>
    public enum OpenCallbackType : uint
    {

        /// <summary>
        /// 没有回调机制；此值是默认设置
        /// </summary>
        Null = 0x00000000,

        /// <summary>
        /// dwCallback 参数是事件句柄；此回调机制仅用于输出
        /// </summary>
        Event = 0x00050000,

        /// <summary>
        /// dwCallback 参数是回调函数地址
        /// </summary>
        Function = 0x00030000,

        /// <summary>
        /// dwCallback 参数是线程标识符
        /// </summary>
        Thread = 0x00020000,

        /// <summary>
        /// dwCallback 参数是窗口句柄
        /// </summary>
        Window = 0x00010000,
    }

    /// <summary>
    /// 用于处理传出 MIDI 信息的回调函数委托
    /// </summary>
    /// <remarks>
    /// <para>应用程序不应从回调函数内部调用任何多媒体函数，因为这样做可能会导致死锁。可以安全地从回调调用其他系统函数</para>
    /// </remarks>
    /// <param name="hmo">与回调函数关联的 MIDI 设备的句柄</param>
    /// <param name="wMsg">MIDI 输出消息</param>
    /// <param name="dwInstance">实例数据由<see cref="WinAPI.midiOutOpen(IntPtr*, uint, void*, void*, uint)"/>函数提供</param>
    /// <param name="dwParam1">Message 参数1</param>
    /// <param name="dwParam2">Message 参数2</param>
    public unsafe delegate void MidiOutProc(IntPtr hmo, uint wMsg, void* dwInstance, void* dwParam1, void* dwParam2);

    /// <summary>
    /// MIDI相关函数返回的错误码
    /// </summary>
    public enum MidiError : uint
    {

        /// <summary>
        /// 无错误
        /// </summary>
        None = 0,

        /// <summary>
        /// 找不到 MIDI 端口
        /// </summary>
        /// <remarks>仅当映射器打开时，才会发生此错误</remarks>
        NoDevice = 64 + 4,

        /// <summary>
        /// 已分配指定的资源
        /// </summary>
        AllOcated = 4,

        /// <summary>
        /// 指定的设备标识符在范围外
        /// </summary>
        BadDeviceID = 0 + 2,

        /// <summary>
        /// 指定的指针或结构无效
        /// </summary>
        InvalParam = 0 + 11,

        /// <summary>
        /// 系统无法分配或锁定内存
        /// </summary>
        NoMem = 0 + 7,

        /// <summary>
        /// 应用程序将不带状态字节的消息发送到流句柄
        /// </summary>
        BadopenMode = 64 + 6,

        /// <summary>
        /// 硬件正忙于处理其他数据
        /// </summary>
        NotReady = 64 + 3,

        /// <summary>
        /// 指定的设备句柄无效
        /// </summary>
        InvalHandle = 0 + 5,

        /// <summary>
        /// 缓冲区仍在队列中
        /// </summary>
        StillPlaying = 64 + 1,
    }


    /// <summary>
    /// windows MIDI 接口
    /// </summary>
    public static unsafe class WinAPI
    {

        /// <summary>
        /// 操作系统内的音频库文件名
        /// </summary>
        public const string dllName = "winmm.dll";

        /// <summary>
        /// 打开 MIDI 输出设备进行播放
        /// </summary>
        /// <param name="phmo">
        /// <para>指向 HMIDIOUT 句柄的指针</para>
        /// <para>此位置由标识打开的 MIDI 输出设备的句柄填充。 句柄用于在调用其他 MIDI 输出函数时标识设备</para>
        /// </param>
        /// <param name="uDeviceID">
        /// <para>要打开的 MIDI 输出设备的标识符</para>
        /// <para>若要确定系统中存在的 MIDI 输出设备数，请使用 <see cref="midiOutGetNumDevs"/> 函数。 <paramref name="uDeviceID"/> 指定的设备标识符从 0 开始计数，最大值不超过<see cref="midiOutGetNumDevs"/>函数的返回值；MIDI_MAPPER还可以用作设备标识符</para>
        /// </param>
        /// <param name="dwCallback">
        /// <para>指向回调函数、事件句柄、线程标识符或在 MIDI 播放期间调用的窗口或线程句柄的指针，以处理与播放进度相关的消息</para>
        /// <para>如果不需要任何回调，请为此参数指定 null</para>
        /// </param>
        /// <param name="dwInstance">传递给回调的用户实例数据；此参数不与窗口回调或线程一起使用</param>
        /// <param name="fdwOpen">用于打开设备的回调标志</param>
        /// <returns>如果成功，则返回0；否则返回错误代码，可转换为<see cref="MidiError"/>枚举</returns>
        [DllImport(dllName)]
        public static extern uint midiOutOpen(IntPtr* phmo, uint uDeviceID, void* dwCallback, void* dwInstance, uint fdwOpen);

        /// <summary>
        /// 打开 MIDI 输出设备进行播放
        /// </summary>
        /// <param name="phmo">
        /// <para>指向 HMIDIOUT 句柄的指针</para>
        /// <para>此位置由标识打开的 MIDI 输出设备的句柄填充。 句柄用于在调用其他 MIDI 输出函数时标识设备</para>
        /// </param>
        /// <param name="uDeviceID">要打开的 MIDI 输出设备的标识符</param>
        /// <param name="dwCallback">
        /// <para>回调函数委托</para>
        /// <para>如果不需要任何回调，请为此参数指定 null</para>
        /// </param>
        /// <param name="dwInstance">传递给回调的用户实例数据；此参数不与窗口回调或线程一起使用</param>
        /// <param name="fdwOpen">用于打开设备的回调标志</param>
        /// <returns>如果成功，则返回0；否则返回错误代码</returns>
        [DllImport(dllName, EntryPoint = "midiOutOpen")]
        public static extern uint CallBack_midiOutOpen(IntPtr* phmo, uint uDeviceID, [MarshalAs(UnmanagedType.FunctionPtr)] MidiOutProc dwCallback, void* dwInstance, uint fdwOpen);

        /// <summary>
        /// 将短 MIDI 消息发送到指定的 MIDI 输出设备
        /// </summary>
        /// <remarks></remarks>
        /// <param name="hMidiOut">
        /// <para>MIDI 输出设备的句柄</para>
        /// <para>此参数也可以是将 MIDI 流强制转换为 HMIDIOUT 的句柄</para>
        /// </param>
        /// <param name="dwMsg">
        /// <para>MIDI 消息</para>
        /// <para>消息被打包为uint值，其中消息的第一个字节位于低位字节</para>
        /// <para>
        /// 具体打包方式如下：<br/>
        /// 将4字节的uint参数分为两个块，高位块（2字节short）和低位块（2字节short）<br/>
        /// 其中每一个字节的含义如下：<br/>
        /// 低位块的低位字节: 状态参数<br/>
        /// 低位块的高位字节：第一个 MIDI 数据字节（可选参数）
        /// 高位块的低位字节：第二个 MIDI 数据字节（可选参数）
        /// 高位块高位字节：未使用
        /// </para>
        /// <para>两个 MIDI 数据字节是可选的，具体如何选择取决于 MIDI 状态参数</para>
        /// <para>
        /// 当一系列消息具有相同的状态参数时，除第一条消息外，后续消息可以省略状态参数（形成运行状态）。运行状态消息的打包方式如下:<br/>
        /// 低位块的低位字节: 第一个 MIDI 数据字节（可选参数）<br/>
        /// 低位块的高位字节：第二个 MIDI 数据字节（可选参数）<br/>
        /// 高位块：全部未使用
        /// </para>
        /// </param>
        /// <returns>成功时返回0，否则返回错误代码；可转换成<see cref="MidiError"/>查看</returns>
        [DllImport(dllName)]
        public static extern int midiOutShortMsg(IntPtr hMidiOut, uint dwMsg);

        /// <summary>
        /// 关闭指定的 MIDI 输出设备
        /// </summary>
        /// <param name="hMidiOut">
        /// <para>MIDI 输出设备的句柄</para>
        /// <para>如果函数成功，则句柄在调用此函数后不再有效</para>
        /// </param>
        /// <returns>如果成功，则返回0，否则返回错误码，可转换成<see cref="MidiError"/>查看</returns>
        [DllImport(dllName)]
        public static extern int midiOutClose(IntPtr hMidiOut);

        /// <summary>
        /// 检索系统中存在的 MIDI 输出设备的数量
        /// </summary>
        /// <returns>
        /// <para>返回 MIDI 输出设备的数量；返回值为零表示系统中没有设备（不代表没有错误）</para>
        /// </returns>
        [DllImport(dllName, SetLastError = true)]
        public static extern uint midiOutGetNumDevs();

    }

}
