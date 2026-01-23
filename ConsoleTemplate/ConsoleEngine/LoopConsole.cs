using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.Memorys;
using Cheng.LoopThreads;
using Cheng.Algorithm;
using Cheng.DataStructure;

using Cheng.DataStructure.Collections;

using InMeg = Cheng.Consoles.ConsoleSystem.InputRecord;
using IEType = Cheng.Consoles.ConsoleSystem.EventType;

namespace Cheng.Consoles
{

    /// <summary>
    /// 控制台消息循环
    /// </summary>
    public unsafe class LoopConsole : Cheng.LoopThreads.LoopFunction
    {

        #region 结构

        #endregion

        #region 构造

        /// <summary>
        /// 实例化控制台消息循环
        /// </summary>
        public LoopConsole() : base()
        {
            p_handle = IntPtr.Zero;
            p_megList = new ImmediatelyQueue<InMeg>();
            p_inputBuf = new InMeg[16];
            p_nowCharValue = -1;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 标准输入设备句柄
        /// </summary>
        protected IntPtr p_handle;

#if DEBUG
        /// <summary>
        /// 读取消息缓冲区
        /// </summary>
#endif
        private InMeg[] p_inputBuf;

        /// <summary>
        /// 暂存的消息队列
        /// </summary>
        protected ImmediatelyQueue<InMeg> p_megList;

        /// <summary>
        /// 当前帧要处理的消息
        /// </summary>
        protected InMeg? p_nowFrameMeg;

        /// <summary>
        /// 当前帧字符输入消息的输入字符
        /// </summary>
        protected int p_nowCharValue;

        #endregion

        #region 功能

        #region 封装

        #region 消息处理

        private void f_readMegBuf()
        {
            var mei = ConsoleSystem.TryGetConsoleInputEventCount(p_handle);
            if (mei > 0)
            {
                mei = ConsoleSystem.ReadConsoleInput(p_handle, p_inputBuf, 0, p_inputBuf.Length);
                for (int i = 0; i < mei; i++)
                {
                    p_megList.Enqueue(p_inputBuf[i]);
                }
            }
        }

        private void f_megListAction()
        {
            p_nowCharValue = -1;
            if (p_megList.Count == 0)
            {
                p_nowFrameMeg = null;
                return;
            }
            var meg = p_megList.Dequeue();
            p_nowFrameMeg = meg;
            var etype = meg.RecordEventType;

            if((etype & IEType.Key) == IEType.Key)
            {
                eventAction_Key(meg);
            }

            if ((etype & IEType.Mouse) == IEType.Mouse)
            {
                eventAction_Mouse(meg);
            }

            if ((etype & IEType.WinBufferSize) == IEType.WinBufferSize)
            {
                eventAction_WinBufferSize(meg);
            }
            EventMessageAction?.Invoke(this, meg);
        }

        /// <summary>
        /// 处理控制台屏幕缓冲区消息
        /// </summary>
        /// <remarks>派生类重写需调用基实现</remarks>
        /// <param name="meg"></param>
        protected virtual void eventAction_WinBufferSize(InMeg meg)
        {
            WinBufferMessageAction?.Invoke(this, meg);
        }

        /// <summary>
        /// 处理键盘消息
        /// </summary>
        /// <remarks>派生类重写需调用基实现</remarks>
        /// <param name="meg"></param>
        protected virtual void eventAction_Key(InMeg meg)
        {
            KeyMessageAction?.Invoke(this, meg);

            bool isPr(char t_c)
            {
                if (t_c == '\0') return false;

                switch (t_c)
                {
                    case '\t':
                        return true;
                }

                if (char.IsControl(t_c)) return false;
                if (t_c == '\r') return false;
                return true;
            }

            if (meg.Key_KeyDown)
            {
                var c = meg.Key_Char;

                //属于可打印字符
                if (isPr(c))
                {
                    CharMessageAction?.Invoke(this, c);
                    p_nowCharValue = c;
                }
            }

        }

        /// <summary>
        /// 处理鼠标消息
        /// </summary>
        /// <remarks>派生类重写需调用基实现</remarks>
        /// <param name="meg"></param>
        protected virtual void eventAction_Mouse(InMeg meg)
        {
            MouseMessageAction?.Invoke(this, meg);
        }

        #endregion

        #endregion

        #region 消息

        /// <summary>
        /// 当前帧要处理的消息，null表示当前帧不存在消息
        /// </summary>
        public InMeg? NowFrameMessage
        {
            get => p_nowFrameMeg;
        }

        /// <summary>
        /// 当前帧存在可处理的消息时触发的事件
        /// </summary>
        public event LoopThreadAction<InMeg, LoopConsole> EventMessageAction;

        /// <summary>
        /// 当前帧存在可处理的按键消息时触发的事件
        /// </summary>
        public event LoopThreadAction<InMeg, LoopConsole> KeyMessageAction;

        /// <summary>
        /// 当前帧存在可处理的字符输入消息时触发的事件
        /// </summary>
        public event LoopThreadAction<char, LoopConsole> CharMessageAction;

        /// <summary>
        /// 当前帧存在可处理的光标消息时触发的事件
        /// </summary>
        public event LoopThreadAction<InMeg, LoopConsole> MouseMessageAction;

        /// <summary>
        /// 当前帧存在可处理的控制台屏幕缓冲区消息时触发事件
        /// </summary>
        public event LoopThreadAction<InMeg, LoopConsole> WinBufferMessageAction;

        /// <summary>
        /// 当前帧字符输入消息的输入字符，不存在为-1
        /// </summary>
        public int NowInputChar
        {
            get => p_nowCharValue;
        }

        /// <summary>
        /// 清空当前循环内所有未处理的消息队列
        /// </summary>
        public void ClearMessageList()
        {
            lock (p_megList)
            {
                p_megList.Clear();
            }
        }

        /// <summary>
        /// 当前循环内未处理的消息数量
        /// </summary>
        public int MessageListCount
        {
            get => p_megList.Count;
        }

        #endregion

        #region 派生

        /// <summary>
        /// 首次循环前调用
        /// </summary>
        /// <remarks>重写需调用基实现</remarks>
        protected override void LoopStartInvoke()
        {
            //初始化
            try
            {
                p_handle = ConsoleSystem.GetSTDInputHandle();
            }
            catch (Exception)
            {
                p_handle = IntPtr.Zero;
                throw;
            }
            if(p_handle == IntPtr.Zero)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 每次循环时调用一次，此函数总是在所有循环事件发生前调用
        /// </summary>
        /// <remarks>重写需调用基实现</remarks>
        protected override void LoopFirst()
        {
            f_readMegBuf();
            f_megListAction();
        }

        /// <summary>
        /// 每次循环时调用一次，此函数总是在所有循环事件发生后调用
        /// </summary>
        /// <remarks>重写需调用基实现</remarks>
        protected override void LoopEnd()
        {
            p_nowFrameMeg = null;
        }

        #endregion

        #region 运行

        /// <summary>
        /// 运行循环线程函数
        /// </summary>
        public void Run()
        {
            this.LoopFunctionMethod();
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif