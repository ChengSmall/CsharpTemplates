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

    /// <summary>
    /// MIDI 异常基类
    /// </summary>
    public class MidiException : Exception
    {

        #region

        public MidiException(MidiError error) : base(Cheng.NetFrameworkTemplate.Properties.Resources.ExceptionMeg_MIDI_Error)
        {
            p_error = error;
        }

        public MidiException(MidiError error, string message) : base(message)
        {
            p_error = error;
        }

        public MidiException(MidiError error, string message, Exception innerException) : base(message, innerException)
        {
            p_error = error;
        }

        public MidiException() : base(Cheng.NetFrameworkTemplate.Properties.Resources.ExceptionMeg_MIDI_Error)
        {
        }

        public MidiException(string message) : base(message)
        {
        }

        public MidiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion

        #region 参数

        protected MidiError p_error;

        #endregion

        #region 

        /// <summary>
        /// MIDI 错误码
        /// </summary>
        public MidiError MidiError
        {
            get => p_error;
        }

        #endregion

    }

    /// <summary>
    /// 系统找不到MIDI设备
    /// </summary>
    public sealed class NotMidiDeviceException : MidiException
    {

        /// <summary>
        /// 系统找不到MIDI设备
        /// </summary>
        public NotMidiDeviceException() : base(Cheng.NetFrameworkTemplate.Properties.Resources.ExceptionMeg_MIDI_SysNotMidiDev)
        {
            p_error = MidiError.NoDevice;
        }

        /// <summary>
        /// 系统找不到MIDI设备
        /// </summary>
        /// <param name="message">错误消息</param>
        public NotMidiDeviceException(string message) : base(message)
        {
            p_error = MidiError.NoDevice;
        }

    }

}
