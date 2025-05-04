using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UObj = UnityEngine.Object;
using Cheng.Streams;
using NAudio.Wave;
using Cheng.Memorys;
using Cheng.IO;

namespace Cheng.Unitys.NAudios
{

    /// <summary>
    /// 将NAudio封装为Unity音频容器
    /// </summary>
    public sealed class NAudioWarpper : SafreleaseUnmanagedResources
    {

        #region

        private delegate void WaveReaderAction(short channels, short bitsPerSample, int blockAlign, WaveStream byteStream, float[] buffer, byte[] sampleBuffer);

        #endregion

        #region 构造

        /// <summary>
        /// 使用NAudio库的音频流解码器创建一个Unity音频容器
        /// </summary>
        /// <param name="waveStream">音频流解码器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">解码器不支持</exception>
        /// <exception cref="EndOfStreamException">在播放时出现意外数据</exception>
        public NAudioWarpper(WaveStream waveStream) : this(waveStream, null, true, true)
        {
        }

        /// <summary>
        /// 使用NAudio库的音频流解码器创建一个Unity音频容器
        /// </summary>
        /// <param name="waveStream">音频流解码器</param>
        /// <param name="clipName">Unity音频容器名称，空表示默认类名</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">解码器不支持</exception>
        /// <exception cref="EndOfStreamException">在播放时出现意外数据</exception>
        public NAudioWarpper(WaveStream waveStream, string clipName) : this(waveStream, clipName, true, true)
        {
        }

        /// <summary>
        /// 使用NAudio库的音频流解码器创建一个Unity音频容器
        /// </summary>
        /// <param name="waveStream">音频流解码器</param>
        /// <param name="clipName">Unity音频容器名称，空表示默认类名</param>
        /// <param name="onDispose">是否在销毁对象时连待销毁传入的<paramref name="waveStream"/>；默认为true销毁对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">解码器不支持</exception>
        /// <exception cref="EndOfStreamException">在播放时出现意外数据</exception>
        public NAudioWarpper(WaveStream waveStream, string clipName, bool onDispose) : this(waveStream, clipName, onDispose, true)
        {
        }

        /// <summary>
        /// 使用NAudio库的音频流解码器创建一个Unity音频容器
        /// </summary>
        /// <param name="waveStream">音频流解码器</param>
        /// <param name="clipName">Unity音频容器名称，空表示默认类名</param>
        /// <param name="onDispose">是否在销毁对象时连待销毁传入的<paramref name="waveStream"/>；默认为true销毁对象</param>
        /// <param name="stream">是否采用流式读取，true表示在播放时逐步读取，false则在初始化时将数据全部加载到内存；默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">解码器不支持</exception>
        /// <exception cref="EndOfStreamException">在播放时出现意外数据</exception>
        public NAudioWarpper(WaveStream waveStream, string clipName, bool onDispose, bool stream)
        {
            if (waveStream is null) throw new ArgumentNullException(nameof(waveStream), "参数是null");
            if (string.IsNullOrEmpty(clipName)) clipName = nameof(NAudioWarpper);
            p_waveStream = waveStream;
            p_onDispose = onDispose;

            p_readerAction = f_createReaderAction(waveStream);
            p_audioClip = f_createAudioClipFromWaveStream(waveStream, clipName, stream);
        }


        #endregion

        #region 参数

        /// <summary>
        /// u3d音频源
        /// </summary>
        private AudioClip p_audioClip;

        /// <summary>
        /// 音频流解码器
        /// </summary>
        private WaveStream p_waveStream;

        /// <summary>
        /// 数据缓冲区
        /// </summary>
        private byte[] p_blockAlignBuffer;

        private WaveReaderAction p_readerAction;

        private bool p_onDispose;

        #endregion

        #region 功能

        #region 封装

        static void sfe_readerPCM(short channels, short bitsPerSample, int blockAlign, WaveStream byteStream, float[] buffer, byte[] sampleBuffer)
        {

            int bufferSize = buffer.Length;
            int sampleCount = bufferSize / channels;

            // 最大振幅值（根据位深度）
            double maxAmplitude;

            switch (bitsPerSample)
            {
                case 8:
                    maxAmplitude = 128;
                    break;
                case 16:
                    maxAmplitude = 32768;
                    break;
                case 24:
                    maxAmplitude = (double)(1 << 23);
                    break;
                case 32:
                    maxAmplitude = (double)(2147483648D);
                    break;
                default:
                    throw new NotSupportedException($"不支持{bitsPerSample}位音频格式");
            }

            try
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    // 读取一个完整的样本块
                    int bytesRead = byteStream.ReadBlock(sampleBuffer, 0, blockAlign);

                    // 验证读取完整性
                    if (bytesRead != blockAlign)
                    {
                        if (bytesRead == 0)
                        {
                            Array.Clear(buffer, 0, buffer.Length);
                            break;
                        }
                        throw new EndOfStreamException($"音频流意外结束 读取块字节:{bytesRead} 应读取字节:{blockAlign}");
                    }

                    // 处理每个通道
                    for (int ch = 0; ch < channels; ch++)
                    {
                        int channelOffset = ch * (bitsPerSample / 8);
                        double sampleValue;
                        byte b0, b1, b2, b3;
                        // 根据位深度转换
                        if (bitsPerSample == 16)
                        {
                            // 手动处理小端字节序
                            short pcm16 = (short)(
                                (sampleBuffer[channelOffset + 1] << 8) |
                                sampleBuffer[channelOffset]
                            );
                            sampleValue = pcm16 / maxAmplitude;
                        }
                        else if (bitsPerSample == 8) // 8位
                        {
                            sbyte pcm8 = (sbyte)sampleBuffer[channelOffset];
                            sampleValue = pcm8 / maxAmplitude;
                        }
                        else if (bitsPerSample == 24)
                        {
                            //24位
                            b0 = sampleBuffer[channelOffset];
                            b1 = sampleBuffer[channelOffset + 1];
                            b2 = sampleBuffer[channelOffset + 2];

                            int pcm24 = (((b2 << 24) | (b1 << 16) | (b0 << 8)) >> 8);
                            sampleValue = (double)pcm24 / maxAmplitude;
                        }
                        else
                        {
                            int pcm32;

                            //b0 = sampleBuffer[channelOffset];
                            //b1 = sampleBuffer[channelOffset + 1];
                            //b2 = sampleBuffer[channelOffset + 2];
                            //b3 = sampleBuffer[channelOffset + 3];
                            //pcm32 = (int)((((uint)b3 << 24) | ((uint)b2 << 16) | ((uint)b1 << 8) | (uint)b0));

                            b0 = sampleBuffer[channelOffset];
                            b1 = sampleBuffer[channelOffset + 1];
                            b2 = sampleBuffer[channelOffset + 2];
                            b3 = sampleBuffer[channelOffset + 3];
                            pcm32 = (int)(b0 | (b1 << 8) | (b2 << 16) | (b3 << 24));

                            //pcm32 = BitConverter.ToInt32(sampleBuffer, channelOffset);

                            //pcm32 = sampleBuffer.OrderToInt32(channelOffset);

                            sampleValue = (double)pcm32 / maxAmplitude;

                        }

                        // 写入缓冲区（注意索引计算）
                        buffer[i * channels + ch] = (float)sampleValue;
                        //Debug.Log(sampleValue);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("音频转换失败", ex);
            }
        }

        static void sfe_readerIeeeFloat(short channels, short bitsPerSample, int blockAlign, WaveStream byteStream, float[] buffer, byte[] sampleBuffer)
        {

            int bufferSize = buffer.Length;
            int sampleCount = bufferSize / channels;

            // 最大振幅值（根据位深度）
            //double maxAmplitude;

            try
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    // 读取一个完整的样本块
                    int bytesRead = byteStream.ReadBlock(sampleBuffer, 0, blockAlign);

                    // 验证读取完整性
                    if (bytesRead != blockAlign)
                    {
                        if (bytesRead == 0)
                        {
                            Array.Clear(buffer, 0, buffer.Length);
                            break;
                        }
                        throw new EndOfStreamException($"音频流意外结束 读取块字节:{bytesRead} 应读取字节:{blockAlign}");
                    }

                    // 处理每个通道
                    for (int ch = 0; ch < channels; ch++)
                    {
                        int channelOffset = ch * (bitsPerSample / 8);
                        // 写入缓冲区
                        if (bitsPerSample == 64)
                        {
                            buffer[i * channels + ch] = (float)sampleBuffer.OrderToDouble(channelOffset);
                        }
                        else
                        {
                            buffer[i * channels + ch] = sampleBuffer.OrderToFloat(channelOffset);
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("音频转换失败", ex);
            }
        }

        private void fe_AudioReader(float[] data)
        {
            ThrowObjectDisposeException("已经销毁U3D音频容器");

            var waveFormat = p_waveStream.WaveFormat;
            p_readerAction.Invoke((short)waveFormat.Channels, (short)waveFormat.BitsPerSample, waveFormat.BlockAlign, p_waveStream, data, p_blockAlignBuffer);
            //sfe_audioReader((short)waveFormat.Channels, (short)waveFormat.BitsPerSample, waveFormat.BlockAlign, p_waveStream, data, p_blockAlignBuffer, waveFormat.Encoding);
        }

        private void fe_AudioSetPosition(int position)
        {
            ThrowObjectDisposeException("已经销毁U3D音频容器");
            p_waveStream.Position = position;
        }

        private WaveReaderAction f_createReaderAction(WaveStream waveStream)
        {
            var wf = waveStream.WaveFormat;
            var en = wf.Encoding;
            switch (en)
            {
                case WaveFormatEncoding.Pcm:
                    return sfe_readerPCM;
                case WaveFormatEncoding.IeeeFloat:
                    return sfe_readerIeeeFloat;
                default:
                    return sfe_readerPCM;
            }

        }

        private AudioClip f_createAudioClipFromWaveStream(WaveStream waveStream, string clipName, bool stream)
        {
            // 获取音频格式信息
            WaveFormat waveFormat = waveStream.WaveFormat;
            var bps = waveFormat.BitsPerSample;
            
            if(bps == 0)
            {
                throw new ArgumentException("无法判断解码器的bps");
            }

            // 计算音频参数
            int sampleCount = (int)((waveStream.Length / (bps / 8)) / waveFormat.Channels);
            int channels = waveFormat.Channels;
            int frequency = waveFormat.SampleRate;

            p_blockAlignBuffer = new byte[waveFormat.BlockAlign];

            // 创建AudioClip
            AudioClip audioClip = AudioClip.Create(name:clipName,
                lengthSamples:sampleCount, channels:channels, frequency:frequency, stream: stream,
                fe_AudioReader, fe_AudioSetPosition
            );

            return audioClip;
        }

        #endregion

        #region 参数访问

        /// <summary>
        /// 获取封装好的Unity音频数据容器
        /// </summary>
        /// <exception cref="ObjectDisposedException">已经销毁对象</exception>
        public AudioClip U3DAudioClip
        {
            get
            {
                ThrowObjectDisposeException();
                return p_audioClip;
            }
        }

        #endregion

        #region 销毁

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                if (p_audioClip != null) UObj.Destroy(p_audioClip);

                if (p_onDispose)
                {
                    p_waveStream?.Close();
                }
            }

            p_audioClip = null;
            p_waveStream = null;
            p_blockAlignBuffer = null;
            return true;
        }

        /// <summary>
        /// 调用该函数销毁创建的Unity资源和托管资源
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        #endregion

        #endregion

    }

}