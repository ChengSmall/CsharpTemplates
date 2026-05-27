using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi.Wave
{

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class WaveFormat
    {
        #region 参数
        protected WaveFormatEncoding waveFormatTag;

        protected short channels;

        protected int sampleRate;

        protected int averageBytesPerSecond;

        protected short blockAlign;

        protected short bitsPerSample;

        protected short extraSize;
        #endregion

        #region 构造
        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
            }

            waveFormatTag = WaveFormatEncoding.Pcm;
            this.channels = (short)channels;
            sampleRate = rate;
            bitsPerSample = (short)bits;
            extraSize = 0;
            blockAlign = (short)(channels * (bits / 8));
            averageBytesPerSecond = sampleRate * blockAlign;
        }
        public WaveFormat()
            : this(44100, 16, 2)
        {
        }
        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        public WaveFormat(BinaryReader br)
        {
            int formatChunkLength = br.ReadInt32();
            ReadWaveFormat(br, formatChunkLength);
        }
        #endregion

        #region 参数访问
        public WaveFormatEncoding Encoding => waveFormatTag;

        public int Channels => channels;

        public int SampleRate => sampleRate;

        public int AverageBytesPerSecond => averageBytesPerSecond;

        public virtual int BlockAlign => blockAlign;

        public int BitsPerSample => bitsPerSample;

        public int ExtraSize => extraSize;

        #endregion

        #region 功能


        public int ConvertLatencyToByteSize(int milliseconds)
        {
            int num = (int)((double)AverageBytesPerSecond / 1000.0 * (double)milliseconds);
            if (num % BlockAlign != 0)
            {
                num = num + BlockAlign - num % BlockAlign;
            }

            return num;
        }

        public static WaveFormat CreateCustomFormat(WaveFormatEncoding tag, int sampleRate, int channels, int averageBytesPerSecond, int blockAlign, int bitsPerSample)
        {
            return new WaveFormat
            {
                waveFormatTag = tag,
                channels = (short)channels,
                sampleRate = sampleRate,
                averageBytesPerSecond = averageBytesPerSecond,
                blockAlign = (short)blockAlign,
                bitsPerSample = (short)bitsPerSample,
                extraSize = 0
            };
        }

        public static WaveFormat CreateALawFormat(int sampleRate, int channels)
        {
            return CreateCustomFormat(WaveFormatEncoding.ALaw, sampleRate, channels, sampleRate * channels, channels, 8);
        }

        public static WaveFormat CreateMuLawFormat(int sampleRate, int channels)
        {
            return CreateCustomFormat(WaveFormatEncoding.MuLaw, sampleRate, channels, sampleRate * channels, channels, 8);
        }

        public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
        {
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.waveFormatTag = WaveFormatEncoding.IeeeFloat;
            waveFormat.channels = (short)channels;
            waveFormat.bitsPerSample = 32;
            waveFormat.sampleRate = sampleRate;
            waveFormat.blockAlign = (short)(4 * channels);
            waveFormat.averageBytesPerSecond = sampleRate * waveFormat.blockAlign;
            waveFormat.extraSize = 0;
            return waveFormat;
        }

        public static WaveFormat MarshalFromPtr(IntPtr pointer)
        {
            WaveFormat waveFormat = Marshal.PtrToStructure<WaveFormat>(pointer);
            switch (waveFormat.Encoding)
            {
                case WaveFormatEncoding.Pcm:
                    waveFormat.extraSize = 0;
                    break;
                case WaveFormatEncoding.Extensible:
                    waveFormat = Marshal.PtrToStructure<WaveFormatExtensible>(pointer);
                    break;
                case WaveFormatEncoding.Adpcm:
                    waveFormat = Marshal.PtrToStructure<AdpcmWaveFormat>(pointer);
                    break;
                case WaveFormatEncoding.Gsm610:
                    waveFormat = Marshal.PtrToStructure<Gsm610WaveFormat>(pointer);
                    break;
                default:
                    if (waveFormat.ExtraSize > 0)
                    {
                        waveFormat = Marshal.PtrToStructure<WaveFormatExtraData>(pointer);
                    }

                    break;
            }

            return waveFormat;
        }

        public static IntPtr MarshalToPtr(WaveFormat format)
        {
            IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, intPtr, fDeleteOld: false);
            return intPtr;
        }

        public static WaveFormat FromFormatChunk(BinaryReader br, int formatChunkLength)
        {
            WaveFormatExtraData waveFormatExtraData = new WaveFormatExtraData();
            waveFormatExtraData.ReadWaveFormat(br, formatChunkLength);
            waveFormatExtraData.ReadExtraData(br);
            return waveFormatExtraData;
        }

        private void ReadWaveFormat(BinaryReader br, int formatChunkLength)
        {
            if (formatChunkLength < 16)
            {
                throw new InvalidDataException("Invalid WaveFormat Structure");
            }

            waveFormatTag = (WaveFormatEncoding)br.ReadUInt16();
            channels = br.ReadInt16();
            sampleRate = br.ReadInt32();
            averageBytesPerSecond = br.ReadInt32();
            blockAlign = br.ReadInt16();
            bitsPerSample = br.ReadInt16();
            if (formatChunkLength > 16)
            {
                extraSize = br.ReadInt16();
                if (extraSize != formatChunkLength - 18)
                {
                    extraSize = (short)(formatChunkLength - 18);
                }
            }
        }

        public override string ToString()
        {
            switch (waveFormatTag)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    return $"{bitsPerSample} bit PCM: {sampleRate}Hz {channels} channels";
                case WaveFormatEncoding.IeeeFloat:
                    return $"{bitsPerSample} bit IEEFloat: {sampleRate}Hz {channels} channels";
                default:
                    return waveFormatTag.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            WaveFormat waveFormat = obj as WaveFormat;
            if (waveFormat != null)
            {
                if (waveFormatTag == waveFormat.waveFormatTag && channels == waveFormat.channels && sampleRate == waveFormat.sampleRate && averageBytesPerSecond == waveFormat.averageBytesPerSecond && blockAlign == waveFormat.blockAlign)
                {
                    return bitsPerSample == waveFormat.bitsPerSample;
                }

                return false;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)waveFormatTag ^ (int)channels ^ sampleRate ^ averageBytesPerSecond ^ blockAlign ^ bitsPerSample;
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(18 + extraSize);
            writer.Write((short)Encoding);
            writer.Write((short)Channels);
            writer.Write(SampleRate);
            writer.Write(AverageBytesPerSecond);
            writer.Write((short)BlockAlign);
            writer.Write((short)BitsPerSample);
            writer.Write(extraSize);
        }

        #endregion

    }

    #region 非托管封送
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class WaveFormatExtensible : WaveFormat
    {
        private short wValidBitsPerSample;
        private int dwChannelMask;
        private Guid subFormat;
        public Guid SubFormat => subFormat;

        private WaveFormatExtensible()
        {
        }

        public WaveFormatExtensible(int rate, int bits, int channels)
            : base(rate, bits, channels)
        {
            waveFormatTag = WaveFormatEncoding.Extensible;
            extraSize = 22;
            wValidBitsPerSample = (short)bits;
            for (int i = 0; i < channels; i++)
            {
                dwChannelMask |= 1 << i;
            }

            if (bits == 32)
            {
                subFormat = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
            }
            else
            {
                subFormat = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
            }
        }


        public WaveFormat ToStandardWaveFormat()
        {
            if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT && bitsPerSample == 32)
            {
                return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            }

            if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_PCM)
            {
                return new WaveFormat(sampleRate, bitsPerSample, channels);
            }

            return this;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(wValidBitsPerSample);
            writer.Write(dwChannelMask);
            byte[] array = subFormat.ToByteArray();
            writer.Write(array, 0, array.Length);
        }


        public override string ToString()
        {
            return "WAVE_FORMAT_EXTENSIBLE " + AudioMediaSubtypes.GetAudioSubtypeName(subFormat) + " " + $"{base.SampleRate}Hz {base.Channels} channels {base.BitsPerSample} bit";
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class AdpcmWaveFormat : WaveFormat
    {
        private short samplesPerBlock;

        private short numCoeff;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        private short[] coefficients;

        public int SamplesPerBlock => samplesPerBlock;


        public int NumCoefficients => numCoeff;


        public short[] Coefficients => coefficients;

        private AdpcmWaveFormat()
            : this(8000, 1)
        {
        }

        public AdpcmWaveFormat(int sampleRate, int channels)
            : base(sampleRate, 0, channels)
        {
            waveFormatTag = WaveFormatEncoding.Adpcm;
            extraSize = 32;
            switch (base.sampleRate)
            {
                case 8000:
                case 11025:
                    blockAlign = 256;
                    break;
                case 22050:
                    blockAlign = 512;
                    break;
                default:
                    blockAlign = 1024;
                    break;
            }

            bitsPerSample = 4;
            samplesPerBlock = (short)((blockAlign - 7 * channels) * 8 / (bitsPerSample * channels) + 2);
            averageBytesPerSecond = base.SampleRate * blockAlign / samplesPerBlock;
            numCoeff = 7;
            coefficients = new short[14]
            {
                256,
                0,
                512,
                -256,
                0,
                0,
                192,
                64,
                240,
                0,
                460,
                -208,
                392,
                -232
            };
        }


        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
            writer.Write(numCoeff);
            short[] array = coefficients;
            foreach (short value in array)
            {
                writer.Write(value);
            }
        }

        public override string ToString()
        {
            return $"Microsoft ADPCM {base.SampleRate} Hz {channels} channels {bitsPerSample} bits per sample {samplesPerBlock} samples per block";
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class Gsm610WaveFormat : WaveFormat
    {
        private readonly short samplesPerBlock;


        public short SamplesPerBlock => samplesPerBlock;

        public Gsm610WaveFormat()
        {
            waveFormatTag = WaveFormatEncoding.Gsm610;
            channels = 1;
            averageBytesPerSecond = 1625;
            bitsPerSample = 0;
            blockAlign = 65;
            sampleRate = 8000;
            extraSize = 2;
            samplesPerBlock = 320;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
        }
    }
    #endregion

}