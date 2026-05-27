using Cheng.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.Memorys;
using Cheng.Texts;
using Cheng.DataStructure;
using System.ComponentModel;
using System;
using System.Runtime.InteropServices;

namespace Cheng.Windows.MIDI
{

    /// <summary>
    /// 一个 MIDI 句柄
    /// </summary>
    public unsafe class MidiHandle : System.Runtime.InteropServices.SafeHandle
    {

        #region

        #endregion

        #region 构造

        /// <summary>
        /// 空构造
        /// </summary>
        protected MidiHandle() : base(IntPtr.Zero, true)
        {
            SetHandle(IntPtr.Zero);
        }

        /// <summary>
        /// 实例化一个无回调的midi句柄
        /// </summary>
        /// <param name="deviceID">midi设备标识符</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        /// <exception cref="NotMidiDeviceException">系统找不到MIDI输出设备</exception>
        /// <exception cref="MidiException">MIDI错误</exception>
        public MidiHandle(int deviceID) : base(IntPtr.Zero, true)
        {
            if (deviceID < 0) throw new ArgumentOutOfRangeException();
            var devc = WinAPI.midiOutGetNumDevs();
            if (devc == 0)
            {
                throw new NotMidiDeviceException();
            }

            IntPtr handle = default;
            var err = (MidiError)WinAPI.midiOutOpen(&handle, (uint)deviceID, null, null, (uint)OpenCallbackType.Null);

            if(err != MidiError.None)
            {
                throw new MidiException(err);
            }
            SetHandle(handle);
        }

        #endregion

        #region 参数

        #endregion

        #region 释放

        public override bool IsInvalid => DangerousGetHandle() == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            var ptr = DangerousGetHandle();
            if (ptr == IntPtr.Zero) return true;
            var b = WinAPI.midiOutClose(ptr) == 0;
            SetHandle(IntPtr.Zero);
            return b;
        }

        #endregion

        #region 设备

        /// <summary>
        /// 返回系统中的midi设备数量
        /// </summary>
        /// <returns>系统中的midi设备数量，0表示系统中不存在midi设备驱动</returns>
        public static int GetDeviceCount()
        {
            return (int)WinAPI.midiOutGetNumDevs();
        }

        #endregion

        #region 播放

        #region 短消息

        /// <summary>
        /// 向系统发送短消息
        /// </summary>
        /// <param name="state">状态参数所在的低位字节</param>
        /// <param name="message">16位短消息值</param>
        /// <returns>引发的错误代码，没有错误返回<see cref="MidiError.None"/></returns>
        /// <exception cref="ObjectDisposedException">句柄已释放</exception>
        public MidiError MIDIOutShortMsg(byte state, ushort message)
        {
            var hp = this.handle;

            //uint meg = ((uint)state) | ((uint)message << 8);
            return (MidiError)WinAPI.midiOutShortMsg(hp, ((uint)state) | ((uint)message << 8));
        }

        /// <summary>
        /// 向系统发送短消息
        /// </summary>
        /// <param name="state">状态参数所在的低位字节</param>
        /// <param name="lowMeg">消息值低位字节值</param>
        /// <param name="hiMeg">消息值高位字节值</param>
        /// <returns>引发的错误代码，没有错误返回<see cref="MidiError.None"/></returns>
        /// <exception cref="ObjectDisposedException">句柄已释放</exception>
        public MidiError MIDIOutShortMsg(byte state, byte lowMeg, byte hiMeg)
        {
            //if (handle is null) throw new ArgumentNullException();
            var hp = this.handle;
            return (MidiError)WinAPI.midiOutShortMsg(hp, (((uint)state) | ((uint)lowMeg << 8) | ((uint)hiMeg << 16)));
        }

        /// <summary>
        /// 向系统发送短消息
        /// </summary>
        /// <param name="channel">状态字节的前4位值，表示通道索引，范围区间在[0,15]</param>
        /// <param name="messageType">状态字节的后4位值，表示消息类型</param>
        /// <param name="message">midi消息数据</param>
        /// <returns></returns>
        public MidiError MIDIOutShortMeg(byte channel, MidiMessageType messageType, ushort message)
        {

            return (MidiError)WinAPI.midiOutShortMsg(this.handle, ((uint)((((byte)channel & 0xF) | (((byte)messageType & 0xF) << 4)))) | ((uint)message << 8));
        }

        /// <summary>
        /// 向系统发送短消息
        /// </summary>
        /// <param name="channel">midi通道，范围区间在[0,15]</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="lowMeg">midi消息数据的低位字节</param>
        /// <param name="hiMeg">midi消息数据的高位字节</param>
        /// <returns>引发的错误代码，没有错误返回<see cref="MidiError.None"/></returns>
        /// <exception cref="ObjectDisposedException">句柄已释放</exception>
        public MidiError MIDIOutShortMeg(byte channel, MidiMessageType messageType, byte lowMeg, byte hiMeg)
        {
            var hp = this.handle;

            uint meg = (((uint)(channel & 0xF)) | ((uint)((byte)messageType & 0xF) << 4)) | ((uint)lowMeg << 8) | ((uint)hiMeg << 16);

            return (MidiError)WinAPI.midiOutShortMsg(hp, meg);
        }

        #region 类型包装

        /// <summary>
        /// 向系统发送播放或关闭音符消息
        /// </summary>
        /// <param name="channel">指定midi通道，范围区间在[0,15]</param>
        /// <param name="onPlay">true表示播放这个音符，false表示停止播放这个音符</param>
        /// <param name="value">要播放的音高值，范围区间在[0,127]</param>
        /// <param name="volume">范围区间在[0,127]的音量控制，0表示停止播放</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByOnNote(byte channel, bool onPlay, byte value, byte volume)
        {
            return MIDIOutShortMeg(channel, onPlay ? MidiMessageType.NoteOn : MidiMessageType.NoteOff, value, volume);
        }

        /// <summary>
        /// 向系统发送播放或关闭音符消息
        /// </summary>
        /// <param name="channel">指定midi通道，范围区间在[0,15]</param>
        /// <param name="onPlay">true表示播放这个音符，false表示停止播放这个音符</param>
        /// <param name="tone">播放音符的标准12音值</param>
        /// <param name="octave">12音所在的八度位，范围区间在[-1,9]</param>
        /// <param name="volume">范围区间在[0,127]的音量控制，0表示停止播放</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByOnNote(byte channel, bool onPlay, MidiNodeTone tone, int octave, byte volume)
        {
            return MIDIOutShortMeg(channel, onPlay ? MidiMessageType.NoteOn : MidiMessageType.NoteOff, ((byte)(((octave + 1) * 12) + (byte)tone)), volume);
        }

        /// <summary>
        /// 向系统发送控制器变更消息
        /// </summary>
        /// <param name="channel">指定midi通道，范围区间在[0,15]</param>
        /// <param name="controlNumber">范围区间在[0,127]的控制器编号</param>
        /// <param name="value">要变更的控制器值</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByControlChange(byte channel, byte controlNumber, byte value)
        {
            return MIDIOutShortMeg(channel, MidiMessageType.ControlChange, controlNumber, value);
        }


        /// <summary>
        /// 向系统发送音色（乐器）变更消息
        /// </summary>
        /// <param name="channel">指定midi通道，范围区间在[0,15]</param>
        /// <param name="number">范围区间在[0,127]的音色编号</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByProgramChange(byte channel, byte number)
        {
            return MIDIOutShortMeg(channel, MidiMessageType.ProgramChange, number, 0);
        }

        /// <summary>
        /// 向系统发送音色（乐器）变更消息
        /// </summary>
        /// <param name="channel">指定midi通道，范围区间在[0,15]</param>
        /// <param name="number">范围区间在[0,127]的音色编号</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByProgramChange(byte channel, ProgramChangeNumber number)
        {
            return MIDIOutShortMeg(channel, MidiMessageType.ProgramChange, (byte)number, 0);
        }

        /// <summary>
        /// 向系统发送弯音轮事件
        /// </summary>
        /// <param name="channel">midi通道，范围区间在[0,15]</param>
        /// <param name="value">14位有效值[0,16383]</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByPitchBend(byte channel, short value)
        {
            return MIDIOutShortMeg(channel, MidiMessageType.PitchBend, (ushort)(value & 0b00111111_11111111));
        }

        /// <summary>
        /// 向系统发送复音压力事件
        /// </summary>
        /// <param name="channel">midi通道，范围区间在[0,15]</param>
        /// <param name="tone">事件音符的标准12音值</param>
        /// <param name="octave">12音所在的八度位，范围区间在[-1,9]</param>
        /// <param name="power">单个音符的触后力度，范围区间在[0,127]</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByPolyPressure(byte channel, MidiNodeTone tone, int octave, byte power)
        {
            return MIDIOutShortMeg(channel, MidiMessageType.PolyPressure, ((byte)(((octave + 1) * 12) + (byte)tone)), power);
        }

        /// <summary>
        /// 向系统发送复音压力事件
        /// </summary>
        /// <param name="channel">midi通道，范围区间在[0,15]</param>
        /// <param name="value">指定音高值，范围区间在[0,127]</param>
        /// <param name="power">单个音符的触后力度，范围区间在[0,127]</param>
        /// <returns>错误代码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public MidiError MIDIShortMegByPolyPressure(byte channel, byte value, byte power)
        {
            return MIDIOutShortMeg(channel, MidiMessageType.PolyPressure, value, power);
        }


        #endregion

        #endregion

        #endregion

    }


}
