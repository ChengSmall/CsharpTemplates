using Cheng.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.Memorys;
using Cheng.Texts;
using Cheng.DataStructure;
using System.ComponentModel;
using System;


namespace Cheng.Windows.MIDI
{

    #region

    /// <summary>
    /// MIDI 消息的状态字节后4位值
    /// </summary>
    public enum MidiMessageType : byte
    {

        /// <summary>
        /// 停止播放某个音符
        /// </summary>
        NoteOff =           0b1000,

        /// <summary>
        /// 开始播放某个音符
        /// </summary>
        NoteOn =            0b1001,

        /// <summary>
        /// 复音压力（单音符触后）
        /// </summary>
        PolyPressure =      0b1010,

        /// <summary>
        /// 控制器变更（如音量、踏板等）
        /// </summary>
        ControlChange =     0b1011,

        /// <summary>
        /// 音色切换（选择乐器）
        /// </summary>
        ProgramChange =     0b1100,

        /// <summary>
        /// 通道压力（整体触后）
        /// </summary>
        ChannelPressure =   0b1101,

        /// <summary>
        /// 弯音轮事件
        /// </summary>
        PitchBend =         0b1110,

        /// <summary>
        /// 系统消息
        /// </summary>
        System =            0b1111
    }

    /// <summary>
    /// 标准12半音音色编号
    /// </summary>
    public enum MidiNodeTone : byte
    {
        //C, C#, D, D#, E, F, F#, G, G#, A, A#, B

        /// <summary>
        /// C
        /// </summary>
        C = 0,

        /// <summary>
        /// C#
        /// </summary>
        Cs,

        /// <summary>
        /// D
        /// </summary>
        D,

        /// <summary>
        /// D#
        /// </summary>
        Ds,

        /// <summary>
        /// E
        /// </summary>
        E,

        /// <summary>
        /// F
        /// </summary>
        F,

        /// <summary>
        /// F#
        /// </summary>
        Fs,

        /// <summary>
        /// G
        /// </summary>
        G,

        /// <summary>
        /// G#
        /// </summary>
        Gs,

        /// <summary>
        /// A
        /// </summary>
        A,

        /// <summary>
        /// A#
        /// </summary>
        As,

        /// <summary>
        /// B
        /// </summary>
        B
    }


    /// <summary>
    /// 提供 MIDI 扩展方法
    /// </summary>
    public static class MidiExtend
    {

        #region 音色

        /// <summary>
        /// MIDI音高编号 - 低音区分界线
        /// </summary>
        public const byte NodeToneLowfrequency = 36;

        /// <summary>
        /// MIDI音高编号 - 高音区分界线
        /// </summary>
        public const byte NodeToneTreble = 72;

        /// <summary>
        /// MIDI音高编号 - 中央 C（钢琴基准）
        /// </summary>
        public const byte NodeToneCentralC = 60;

        /// <summary>
        /// MIDI音高编号 - 标准音高 A4（440Hz）
        /// </summary>
        public const byte StandardPitchA4 = 69;

        /// <summary>
        /// 音高默认八度值
        /// </summary>
        public const byte OctaveDefault = 4;

        /// <summary>
        /// 使用标注12半音和八度值返回Node音高值
        /// </summary>
        /// <remarks>
        /// <para><![CDATA[音高值 = (八度数 + 1) * 12 + 12半音索引]]></para>
        /// </remarks>
        /// <param name="tone">标准12半音表</param>
        /// <param name="octave">八度数，范围区间在[-1,9]</param>
        /// <returns>MIDI音高编号</returns>
        public static byte GetNodeTone(this MidiNodeTone tone, int octave)
        {
            //(八度数 + 1) * 12 + 音名索引
            return (byte)(((octave + 1) * 12) + (byte)tone);
        }

        #endregion

        #region 

        #endregion

    }

    #endregion

}
