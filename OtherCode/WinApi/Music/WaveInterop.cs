using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi.Wave
{

    public static class WaveInterop
    {
        public const string WinapiName = "winmm.dll";

        [Flags]
        public enum WaveInOutOpenFlags
        {
            /// <summary>
            /// 无回调机制（默认）
            /// </summary>
            CallbackNull = 0x0,
            /// <summary>
            /// dwCallback 参数是回调过程地址
            /// </summary>
            CallbackFunction = 0x30000,
            /// <summary>
            /// dwCallback 参数是事件句柄
            /// </summary>
            CallbackEvent = 0x50000,
            /// <summary>
            /// dwCallback 参数是窗口句柄
            /// </summary>
            CallbackWindow = 0x10000,
            /// <summary>
            /// dwCallback 参数是线程标识符
            /// </summary>
            CallbackThread = 0x20000
        }

        public enum WaveMessage
        {
            /// <summary>
            /// 使用waveInOpen功能打开设备时发送
            /// </summary>
            WaveInOpen = 958,
            /// <summary>
            /// 使用waveInClose功能关闭设备时发送
            /// </summary>
            WaveInClose = 959,

            WaveInData = 960,
            /// <summary>
            /// 使用waveOutClose功能关闭设备时发送
            /// </summary>
            WaveOutClose = 956,
            /// <summary>
            /// 
            /// </summary>
            WaveOutDone = 957,
            /// <summary>
            /// 使用waveOutOpen功能打开设备时发送
            /// </summary>
            WaveOutOpen = 955
        }

        /// <summary>
        /// 与波形音频输出设备一起使用的回调函数
        /// </summary>
        /// <remarks>
        /// waveOutProc函数是与波形音频输出设备一起使用的回调函数；waveOutProc函数是应用程序定义的函数名称的占位符。此函数的地址可以在waveOutOpen函数的回调地址参数中指定。
        /// </remarks>
        /// <param name="hWaveOut">与回调相关联的波形音频设备的句柄</param>
        /// <param name="message">波形音频输出消息</param>
        /// <param name="dwInstance">使用waveOutOpen指定的用户实例数据</param>
        /// <param name="wavhdr"></param>
        /// <param name="dwReserved"></param>
        public delegate void WaveCallback(IntPtr hWaveOut, WaveMessage message, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved);

        [DllImport(WinapiName)]
        public static extern int mmioStringToFOURCC([MarshalAs(UnmanagedType.LPStr)] string s, int flags);

        #region 输出

        /// <summary>
        /// 检索系统中存在的波形音频输出设备数
        /// </summary>
        /// <returns>返回设备数；返回值为零表示不存在任何设备或发生了错误</returns>
        [DllImport(WinapiName)]
        public static extern int waveOutGetNumDevs();

        /// <summary>
        /// 开始准备用于播放的波形音频数据块
        /// </summary>
        /// <param name="hWaveOut">波形音频输出设备的句柄</param>
        /// <param name="lpWaveOutHdr">该结构标识要准备的数据块</param>
        /// <param name="uSize">结构的大小（以字节为单位）</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveOutPrepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutUnprepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutWrite(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

        /// <summary>
        /// 打开给定的波形音频输出设备进行播放
        /// </summary>
        /// <param name="hWaveOut">
        /// 指向缓冲区的指针，该缓冲区接收标识打开的波形音频输出设备的句柄；<br/>
        /// 调用其他波形音频输出函数时，使用句柄标识设备
        /// </param>
        /// <param name="uDeviceID">要打开的波形音频输出设备的标识符</param>
        /// <param name="lpFormat">该结构标识要发送到设备的波形音频数据的格式。可以将此结构传递到 waveOutOpen 后立即释放它。</param>
        /// <param name="dwCallback">指定回调机制</param>
        /// <param name="dwInstance">传递给回调机制的用户实例数据；此参数不与窗口回调机制一起使用</param>
        /// <param name="dwFlags">用于打开设备的标志</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveOutOpen(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        [DllImport(WinapiName, EntryPoint = "waveOutOpen")]
        public static extern MmResult waveOutOpenWindow(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutReset(IntPtr hWaveOut);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutClose(IntPtr hWaveOut);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutPause(IntPtr hWaveOut);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutRestart(IntPtr hWaveOut);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutGetPosition(IntPtr hWaveOut, ref MmTime mmTime, int uSize);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

        [DllImport(WinapiName)]
        public static extern MmResult waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

        [DllImport(WinapiName, CharSet = CharSet.Auto)]
        public static extern MmResult waveOutGetDevCaps(IntPtr deviceID, out WaveOutCapabilities waveOutCaps, int waveOutCapsSize);
        #endregion

        #region 输入

        /// <summary>
        /// 返回系统中存在的波形音频输入设备的数量
        /// </summary>
        /// <returns>返回设备数。 返回值为零表示不存在任何设备或发生了错误</returns>
        [DllImport(WinapiName)]
        public static extern int waveInGetNumDevs();
        /// <summary>
        /// 检索给定波形音频输入设备的功能
        /// </summary>
        /// <param name="deviceID">波形音频输出设备的标识符<br/>
        /// 它可以是设备标识符，也可以是开放波形音频输入设备的句柄
        /// </param>
        /// <param name="waveInCaps">该结构要填充有关设备功能的信息</param>
        /// <param name="waveInCapsSize">结构的大小（以字节为单位）</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName, CharSet = CharSet.Auto)]
        public static extern MmResult waveInGetDevCaps(IntPtr deviceID, out WaveInCapabilities waveInCaps, int waveInCapsSize);
        /// <summary>
        /// 将输入缓冲区发送到给定的波形音频输入设备。
        /// </summary>
        /// <remarks>填充缓冲区时，应用程序会收到通知</remarks>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <param name="pwh">指向用于标识缓冲区的对象</param>
        /// <param name="cbwh"><paramref name="pwh"/>结构的大小（以字节为单位）</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInAddBuffer(IntPtr hWaveIn, WaveHeader pwh, int cbwh);
        /// <summary>
        /// 关闭给定的波形音频输入设备
        /// </summary>
        /// <param name="hWaveIn">波形音频输入设备的句柄。 如果函数成功，则此调用后句柄不再有效</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInClose(IntPtr hWaveIn);
        /// <summary>
        /// 打开给定波形音频输入设备进行录制
        /// </summary>
        /// <param name="hWaveIn">
        /// 指向缓冲区的指针，该缓冲区接收标识打开的波形音频输入设备的句柄<br/>
        /// 调用其他波形音频输入函数时，使用此句柄标识设备。 如果为 <paramref name="dwFlags"/> 指定了WAVE_FORMAT_QUERY，则此参数可以为 NULL。
        /// </param>
        /// <param name="uDeviceID">要打开的波形音频输入设备的标识符</param>
        /// <param name="lpFormat">指向 WaveFormat 结构的指针，该结构标识录制波形音频数据所需的格式</param>
        /// <param name="dwCallback">指向固定回调函数、事件句柄、窗口句柄的指针</param>
        /// <param name="dwInstance">传递给回调机制的用户实例数据</param>
        /// <param name="dwFlags">用于打开设备的标志</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInOpen(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);
        
        [DllImport(WinapiName, EntryPoint = "waveInOpen")]
        public static extern MmResult waveInOpenWindow(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);
        /// <summary>
        /// 波形音频输入准备缓冲区
        /// </summary>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <param name="lpWaveInHdr">该结构标识要准备的缓冲区</param>
        /// <param name="uSize">WAVEHDR 结构的大小（以字节为单位）</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInPrepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, uint uSize);
        /// <summary>
        /// 清理<see cref="waveInPrepareHeader(IntPtr, WaveHeader, uint)"/>函数执行的准备工作
        /// </summary>
        /// <remarks>
        /// 在设备驱动程序填充缓冲区并将其返回到应用程序后，必须调用此函数;<br/>
        /// 在释放缓冲区之前，必须调用此函数
        /// </remarks>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <param name="lpWaveInHdr">标识要清理的缓冲区</param>
        /// <param name="uSize">结构的大小（以字节为单位）</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInUnprepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, uint uSize);
        /// <summary>
        /// 停止给定波形音频输入设备上的输入，并将当前位置重置为零
        /// </summary>
        /// <remarks>
        /// 所有挂起的缓冲区都标记为已完成并返回到应用程序。
        /// </remarks>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInReset(IntPtr hWaveIn);
        /// <summary>
        /// 在给定的波形音频输入设备上启动输入
        /// </summary>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInStart(IntPtr hWaveIn);
        /// <summary>
        /// 停止波形音频输入
        /// </summary>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInStop(IntPtr hWaveIn);
        /// <summary>
        /// 检索给定波形音频输入设备的当前输入位置
        /// </summary>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <param name="mmTime">指向 MMTIME 结构的指针</param>
        /// <param name="uSize">MMTIME 结构的大小（以字节为单位）</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInGetPosition(IntPtr hWaveIn, out MmTime mmTime, int uSize);
        /// <summary>
        /// 获取给定波形音频输入设备的设备标识符       
        /// </summary>
        /// <remarks>
        /// 支持此函数以实现向后兼容性；新应用程序可以强制转换设备的句柄，而不是检索设备标识符。
        /// </remarks>
        /// <param name="hWaveIn">波形音频输入设备的句柄</param>
        /// <param name="puDeviceID">指向要用设备标识符填充的变量的指针</param>
        /// <returns>如果成功，则返回<see cref="MmResult.NoError"/>，否则返回错误</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInGetID(IntPtr hWaveIn, IntPtr puDeviceID);
        /// <summary>
        /// 将消息发送到波形音频输入设备驱动程序
        /// </summary>
        /// <param name="hWaveIn">接收消息的波形设备的标识符。 必须将设备 ID 强制转换为 HWAVEIN 句柄类型</param>
        /// <param name="uMsg">要发送的消息</param>
        /// <param name="dw1">Message参数</param>
        /// <param name="dw2">Message参数</param>
        /// <returns>返回从驱动程序返回的值</returns>
        [DllImport(WinapiName)]
        public static extern MmResult waveInMessage(IntPtr hWaveIn, uint uMsg, IntPtr dw1, IntPtr dw2);

        #endregion

    }

}
