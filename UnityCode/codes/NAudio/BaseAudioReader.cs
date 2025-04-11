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
using URandom = UnityEngine.Random;
using NAudio.Flac;
using Cheng.Streams;
using NAudio.Wave;
using Cheng.Memorys;

namespace Cheng.Unitys.NAudios
{

    public abstract class BaseAudioReader : MonoBehaviour
    {

        #region

        public BaseAudioReader()
        {
            audioName = string.Empty;
        }

        #endregion

        #region 参数

        #region

#if UNITY_EDITOR
        [Tooltip("创建音频流时的名称")]
#endif
        [SerializeField] protected string audioName;

        #endregion

        #region 内部参数

        /// <summary>
        /// u3d音频源
        /// </summary>
        protected AudioClip audioClip;

        /// <summary>
        /// 音频流解码器
        /// </summary>
        protected WaveStream audioReader;

        private byte[] blockAlignBuffer;

        #endregion

        #endregion

        #region 封装

        private static void sfe_audioReader(short channels, int sampleRate, short bitsPerSample, int blockAlign, WaveStream byteStream, float[] buffer, byte[] sampleBuffer)
        {

            int bufferSize = buffer.Length;
            int sampleCount = bufferSize / channels;

            // 最大振幅值（根据位深度）
            double maxAmplitude;

            switch (bitsPerSample)
            {
                case 8:
                    maxAmplitude = 255;
                    break;
                case 16:
                    maxAmplitude = 32768;
                    break;
                case 24:
                    maxAmplitude = (double)(1 << 23);
                    break;
                default:
                    throw new NotSupportedException($"不支持{bitsPerSample}位音频格式");
            }

            //maxAmplitude = (bitsPerSample == 16) ? 32768.0 : 256.0;

            try
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    // 读取一个完整的样本块
                    int bytesRead = byteStream.ReadBlock(sampleBuffer, 0, blockAlign);

                    // 验证读取完整性
                    if (bytesRead != blockAlign)
                        throw new EndOfStreamException("音频流意外结束");

                    // 处理每个通道
                    for (int ch = 0; ch < channels; ch++)
                    {
                        int channelOffset = ch * (bitsPerSample / 8);
                        double sampleValue;

                        // 根据位深度转换
                        if (bitsPerSample == 16)
                        {
                            // 手动处理小端字节序（避免BitConverter的潜在问题）
                            short pcm16 = (short)(
                                (sampleBuffer[channelOffset + 1] << 8) |
                                sampleBuffer[channelOffset]
                            );
                            sampleValue = pcm16 / maxAmplitude;
                        }
                        else if (bitsPerSample == 8) // 8位
                        {
                            sbyte pcm8 = (sbyte)sampleBuffer[channelOffset];
                            sampleValue = pcm8 / 255.0; // 8位有符号最大值127
                        }
                        else
                        {
                            //24位
                            byte b0 = sampleBuffer[channelOffset], b1 = sampleBuffer[channelOffset + 1], b2 = sampleBuffer[channelOffset + 2];

                            int pcm24 = (((b2 << 24) | (b1 << 16) | (b0 << 8)) >> 8);
                            sampleValue = (double)pcm24 / maxAmplitude;
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

        private void fe_AudioReader(float[] data)
        {
            if (audioReader is null)
            {
                Debug.LogError("解码器没有初始化");
                return;
            }

            int length = data.Length;
            var waveFormat = audioReader.WaveFormat;
            try
            {
                sfe_audioReader((short)waveFormat.Channels, waveFormat.SampleRate, (short)waveFormat.BitsPerSample, waveFormat.BlockAlign, audioReader, data, blockAlignBuffer);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        private void fe_AudioSetPosition(int position)
        {
            if(audioReader is null)
            {
                Debug.LogError("解码器没有初始化");
            }
            else
            {
                audioReader.Position = position;
            }
            
        }

        private AudioClip f_createAudioClipFromWaveStream(WaveStream waveStream, string clipName)
        {
            // 获取音频格式信息
            WaveFormat waveFormat = waveStream.WaveFormat;

            // 计算音频参数
            int sampleCount = (int)(waveStream.Length / (waveFormat.BitsPerSample / 8) / waveFormat.Channels);
            int channels = waveFormat.Channels;
            int frequency = waveFormat.SampleRate;

            blockAlignBuffer = new byte[waveFormat.BlockAlign];

            // 创建AudioClip
            AudioClip audioClip = AudioClip.Create(
                clipName,                     // 剪辑名称
                sampleCount,                  // 样本数量（长度）
                channels,                     // 声道数
                frequency,                    // 采样率
                true,                         // 是否3D音频（通常设为false）
                fe_AudioReader, fe_AudioSetPosition
            );

            return audioClip;
        }

        /// <summary>
        /// 从已经初始化好的音频流解码器创建<see cref="AudioClip"/>到<see cref="audioClip"/>
        /// </summary>
        protected virtual void initAudio()
        {
            try
            {
                audioClip = f_createAudioClipFromWaveStream(audioReader, audioName);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(64);
                sb.Append("音频流初始化错误:");
                sb.AppendLine(ex.Message);
                sb.Append("错误类型:");
                sb.AppendLine(ex.GetType().FullName);
                Debug.LogError(sb.ToString());

                destoryAudio();
            }

        }

        /// <summary>
        /// 销毁和释放脚本内的资源
        /// </summary>
        protected virtual void destoryAudio()
        {
            if (audioClip != null) Destroy(audioClip);

            if (audioReader != null)
            {
                audioReader.Close();
            }

            audioClip = null;
            audioReader = null;
        }

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置初始化时创建的音频流名称
        /// </summary>
        public string AudioName
        {
            get => audioName;
            set => audioName = value ?? string.Empty;
        }

        /// <summary>
        /// 获取创建的音频流，若没有创建或以销毁则为null
        /// </summary>
        public AudioClip AudioStreamClip
        {
            get => audioClip;
        }

        #endregion

        #endregion

        #region 运行


        protected virtual void Awake()
        {

        }


        protected virtual void OnDestroy()
        {
            destoryAudio();
        }

        #endregion

    }

}