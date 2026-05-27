using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi.Wave
{

    [Flags]
    internal enum WaveOutSupport
    {
        Pitch = 0x1,
        PlaybackRate = 0x2,
        Volume = 0x4,
        LRVolume = 0x8,
        Sync = 0x10,
        SampleAccurate = 0x20
    }

    [Flags]
    public enum SupportedWaveFormat
    {
        WAVE_FORMAT_1M08 = 0x1,
        WAVE_FORMAT_1S08 = 0x2,
        WAVE_FORMAT_1M16 = 0x4,
        WAVE_FORMAT_1S16 = 0x8,
        WAVE_FORMAT_2M08 = 0x10,
        WAVE_FORMAT_2S08 = 0x20,
        WAVE_FORMAT_2M16 = 0x40,
        WAVE_FORMAT_2S16 = 0x80,
        WAVE_FORMAT_4M08 = 0x100,
        WAVE_FORMAT_4S08 = 0x200,
        WAVE_FORMAT_4M16 = 0x400,
        WAVE_FORMAT_4S16 = 0x800,
        WAVE_FORMAT_44M08 = 0x100,
        WAVE_FORMAT_44S08 = 0x200,
        WAVE_FORMAT_44M16 = 0x400,
        WAVE_FORMAT_44S16 = 0x800,
        WAVE_FORMAT_48M08 = 0x1000,
        WAVE_FORMAT_48S08 = 0x2000,
        WAVE_FORMAT_48M16 = 0x4000,
        WAVE_FORMAT_48S16 = 0x8000,
        WAVE_FORMAT_96M08 = 0x10000,
        WAVE_FORMAT_96S08 = 0x20000,
        WAVE_FORMAT_96M16 = 0x40000,
        WAVE_FORMAT_96S16 = 0x80000
    }

    [Flags]
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// 由设备驱动程序设置，以指示它已使用完缓冲区并将其返回给应用程序
        /// </summary>
        Done = 0x1,
        /// <summary>
        /// 由Windows设置，以指示缓冲区已准备
        /// </summary>
        /// <remarks>由Windows设置，以指示缓冲区已使用waveInPrepareHeader或waveOutPrepareHead函数进行了准备</remarks>
        Prepared = 0x2,
        /// <summary>
        /// 这个缓冲区是循环中的第一个缓冲区
        /// </summary>
        /// <remarks>此标志仅用于输出缓冲区</remarks>
        BeginLoop = 0x4,
        /// <summary>
        /// 这个缓冲区是循环中的最后一个缓冲区
        /// </summary>
        /// <remarks>此标志仅用于输出缓冲区</remarks>
        EndLoop = 0x8,
        /// <summary>
        /// 由Windows设置以指示缓冲区已排队等待播放
        /// </summary>
        InQueue = 0x10      
    }

    [StructLayout(LayoutKind.Sequential)]
    public class WaveHeader
    {
        #region 参数
        /// <summary>
        /// 指向波形缓冲区的指针。
        /// </summary>
        public IntPtr dataBuffer;
        /// <summary>
        /// 缓冲区的长度（以字节为单位）
        /// </summary>
        public int bufferLength;
        /// <summary>
        /// 在输入中使用标头时，指定缓冲区中的数据量
        /// </summary>
        public int bytesRecorded;
        /// <summary>
        /// 用户数据
        /// </summary>
        public IntPtr userData;
        /// <summary>
        /// 波形标志的位域集合
        /// </summary>
        public WaveHeaderFlags flags;
        /// <summary>
        /// 循环播放的次数
        /// </summary>
        /// <remarks>此成员仅与输出缓冲区一起使用</remarks>
        public int loops;
        /// <summary>
        /// 保留
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// 保留
        /// </summary>
        public IntPtr reserved;
        #endregion
    }

    /// <summary>
    /// 多媒体时间格式
    /// </summary>
    public enum TimeType : uint
    {
        /// <summary>
        /// 时间以毫秒为单位
        /// </summary>
        TIME_MS = 0x1,
        /// <summary>
        /// 波形音频采样数
        /// </summary>
        TIME_SAMPLES = 0x2,
        /// <summary>
        /// 从文件开始的当前字节偏移量
        /// </summary>
        TIME_BYTES = 0x4,
        /// <summary>
        /// SMPTE时间
        /// </summary>
        TIME_SMPTE = 0x8,
        /// <summary>
        /// midi时间
        /// </summary>
        TIME_MIDI = 0x10,
        /// <summary>
        /// midi的tick
        /// </summary>
        TIME_TICKS = 0x20
    }

    /// <summary>
    /// 包含用于不同类型的多媒体数据的定时信息
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MmTime
    {
        /// <summary>
        /// 时间以毫秒为单位
        /// </summary>
        const uint TIME_MS = 1;
        /// <summary>
        /// 波形音频采样数
        /// </summary>
        const uint TIME_SAMPLES = 2;
        /// <summary>
        /// 从文件开始的当前字节偏移量
        /// </summary>
        const uint TIME_BYTES = 4;

        /// <summary>
        /// 时间格式
        /// </summary>
        [FieldOffset(0)]
        public TimeType wType;
        /// <summary>
        /// 毫秒数
        /// </summary>
        /// <remarks>当wType是<see cref="TimeType.TIME_MS"/>时使用</remarks>
        [FieldOffset(4)]
        public uint ms;
        /// <summary>
        /// 样本数量。
        /// </summary>
        /// <remarks>当wType为<see cref="TimeType.TIME_SAMPLES"/>时使用。</remarks>
        [FieldOffset(4)]
        public uint sample;
        /// <summary>
        /// 字节数量
        /// </summary>
        /// <remarks>当wType是<see cref="TimeType.TIME_BYTES"/>时使用</remarks>
        [FieldOffset(4)]
        public uint cb;
        /// <summary>
        /// midi
        /// </summary>
        [FieldOffset(4)]
        public uint ticks;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(4)]
        public byte smpteHour;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(5)]
        public byte smpteMin;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(6)]
        public byte smpteSec;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(7)]
        public byte smpteFrame;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(8)]
        public byte smpteFps;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(9)]
        public byte smpteDummy;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(10)]
        public byte smptePad0;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(11)]
        public byte smptePad1;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(4)]
        public uint midiSongPtrPos;
    }

    /// <summary>
    /// 描述了波形音频输出设备的能力
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WaveOutCapabilities
    {
        private short manufacturerId;

        private short productId;

        private int driverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string productName;

        private SupportedWaveFormat supportedFormats;

        private short channels;

        private short reserved;

        private WaveOutSupport support;

        private Guid manufacturerGuid;

        private Guid productGuid;

        private Guid nameGuid;

        private const int MaxProductNameLength = 32;

        public int Channels => channels;

        public bool SupportsPlaybackRateControl => (support & WaveOutSupport.PlaybackRate) == WaveOutSupport.PlaybackRate;

        public string ProductName => productName;

        public Guid NameGuid => nameGuid;

        public Guid ProductGuid => productGuid;

        public Guid ManufacturerGuid => manufacturerGuid;

        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }
    }

    /// <summary>
    /// 描述了波形音频输入设备的能力
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WaveInCapabilities
    {
        /// <summary>
        /// 输入设备的设备驱动程序的制造商标识
        /// </summary>
        /// <remarks>
        /// 波形音频输入设备的设备驱动程序的制造商标识符；<br/>
        /// 制造商标识符在制造商和产品标识符中定义
        /// </remarks>
        private short manufacturerId;
        /// <summary>
        /// 输入设备的产品标识符
        /// </summary>
        /// <remarks>
        /// 波形音频输入设备的产品标识符；产品标识符在制造商和产品标识符中定义。
        /// </remarks>
        private short productId;
        /// <summary>
        /// 输入设备的设备驱动程序的版本号
        /// </summary>
        /// <remarks>
        /// 波形音频输入设备的设备驱动程序的版本号；高阶字节是主要版本号，低阶字节是次要版本号。
        /// </remarks>
        private int driverVersion;
        /// <summary>
        /// 以null结尾的字符串中的产品名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string productName;
        /// <summary>
        /// 支持的标准格式
        /// </summary>
        private SupportedWaveFormat supportedFormats;
        /// <summary>
        /// 指定设备支持单声道还是立体声
        /// </summary>
        /// <remarks>1表示单声道，2表示立体声</remarks>
        private short channels;
        /// <summary>
        /// 填充区
        /// </summary>
        private short reserved;

        private Guid manufacturerGuid;

        private Guid productGuid;

        private Guid nameGuid;

        private const int MaxProductNameLength = 32;

        public int Channels => channels;

        public string ProductName => productName;

        public Guid NameGuid => nameGuid;

        public Guid ProductGuid => productGuid;

        public Guid ManufacturerGuid => manufacturerGuid;

        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }
    }


}
