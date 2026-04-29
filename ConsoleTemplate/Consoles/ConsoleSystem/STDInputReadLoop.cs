using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Security;
using System.IO;

using Cheng.Texts;
using Cheng.Memorys;

using Cheng.DataStructure;
using Cheng.DataStructure.Collections;

using Cheng.Algorithm;

using SCol = global::System.Console;
using CInR = Cheng.Consoles.ConsoleSystem.InputRecord;

namespace Cheng.Consoles
{

    /// <summary>
    /// 标准输入读取系统
    /// </summary>
    /// <remarks>
    /// <para>独立的标准输入流消息循环线程实现</para>
    /// <para>使用独立线程调用<see cref="LoopAction"/>或<see cref="LoopThreadStartFunc"/>委托开启消息循环，开启后可从缓冲区读取输入；用<see cref="ExitLoopFunc"/>关闭循环</para>
    /// </remarks>
    public unsafe class STDInputReadLoop : SafreleaseUnmanagedResources
    {

        #region 结构

        /// <summary>
        /// 消息添加类型
        /// </summary>
        public enum AddInputType : byte
        {

            /// <summary>
            /// 覆盖旧值
            /// </summary>
            /// <remarks>新消息按输入顺序覆盖旧消息</remarks>
            Cover = 0,

            /// <summary>
            /// 忽略溢出的输入
            /// </summary>
            /// <remarks>若缓冲区数量已满，则忽略新读取的输入</remarks>
            IgnoreOverflow = 1

        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化标准输入读取系统，从<see cref="ConsoleSystem.GetSTDInputHandle"/>获取标准输入句柄
        /// </summary>
        /// <exception cref="Win32Exception">win32错误</exception>
        public STDInputReadLoop() : this(ConsoleSystem.GetSTDInputHandle(), 32, null)
        {
        }

        /// <summary>
        /// 实例化标准输入读取系统
        /// </summary>
        /// <param name="inputHandle">控制台标准输入的句柄，可用<see cref="ConsoleSystem.GetSTDInputHandle"/>获取</param>
        public STDInputReadLoop(IntPtr inputHandle) : this(inputHandle, 32, null)
        {
        }

        /// <summary>
        /// 实例化标准输入读取系统
        /// </summary>
        /// <param name="inputHandle">控制台标准输入的句柄，可用<see cref="ConsoleSystem.GetSTDInputHandle"/>获取</param>
        /// <param name="bufferMaxCount">消息缓冲区的最大数量，默认是32</param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区最大数量小于或等于0</exception>
        public STDInputReadLoop(IntPtr inputHandle, int bufferMaxCount) : this(inputHandle, bufferMaxCount, null)
        {
        }

        /// <summary>
        /// 实例化标准输入读取系统
        /// </summary>
        /// <param name="inputHandle">控制台标准输入的句柄，可用<see cref="ConsoleSystem.GetSTDInputHandle"/>获取</param>
        /// <param name="bufferMaxCount">消息缓冲区的最大数量，默认是32</param>
        /// <param name="lockObj">在调用系统函数获取消息前要进行线程锁的对象；null则不使用线程锁</param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区最大数量小于或等于0</exception>
        public STDInputReadLoop(IntPtr inputHandle, int bufferMaxCount, object lockObj)
        {
            if (bufferMaxCount <= 0) throw new ArgumentOutOfRangeException();
            p_h = inputHandle;
            p_lock = lockObj;
            p_list = new ImmediatelyQueue<CInR>(Math.Min(bufferMaxCount, 32));
            p_maxBufCount = bufferMaxCount;
            p_addType = AddInputType.Cover;
            p_startLoop = false;
            p_filterLock = new object();
            p_addMegFilter = null;
            p_waitEvent = new ManualResetEvent(true);
            p_sb = new StringBuilder(16);
            p_pauseReadLoopEvent = new ManualResetEvent(true);
            p_isReading = true;
            p_isReadStoped = false;
            p_isPausedReadLoopCallback = new ManualResetEvent(false);
        }

        #endregion

        #region 参数

        private object p_lock;

        /// <summary>
        /// 消息队列
        /// </summary>
        protected ImmediatelyQueue<CInR> p_list;

        private object p_filterLock;
        private Predicate<CInR> p_addMegFilter;

        private EventWaitHandle p_waitEvent;
        private StringBuilder p_sb;

        private EventWaitHandle p_pauseReadLoopEvent;

        private EventWaitHandle p_isPausedReadLoopCallback;

        private IntPtr p_h;

        private int p_maxBufCount;
        private AddInputType p_addType;

        private bool p_startLoop;
        private bool p_isReading;
        private bool p_isReadStoped;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 消息循环函数委托
        /// </summary>
        public Action LoopAction
        {
            get => LoopReadFunction;
        }

        /// <summary>
        /// 消息循环函数委托
        /// </summary>
        public ThreadStart LoopThreadStartFunc
        {
            get => LoopReadFunction;
        }

        /// <summary>
        /// 标准输入流句柄
        /// </summary>
        public IntPtr InputHandle
        {
            get => p_h;
        }

        /// <summary>
        /// 添加消息到缓冲区的方式
        /// </summary>
        public AddInputType AddBufferType
        {
            get => p_addType;
            set => p_addType = value;
        }

        /// <summary>
        /// 是否已开启循环
        /// </summary>
        public bool StartLoop
        {
            get => p_startLoop;
        }

        /// <summary>
        /// 添加到缓冲区前的输入过滤器
        /// </summary>
        /// <value>将输入添加到缓冲区前使用过滤器进行筛选，只有返回true的参数才会添加；null表示没有过滤器，等同于过滤器返回true</value>
        public Predicate<CInR> AddFilterFunc
        {
            get => p_addMegFilter;
            set
            {
                lock (p_filterLock)
                {
                    p_addMegFilter = value;
                }
            }
        }

        /// <summary>
        /// 对象是否暂停消息循环的输入读取
        /// </summary>
        public bool IsPauseReading
        {
            get => p_isReading;
        }

        /// <summary>
        /// 是否已真正暂停消息循环的输入读取
        /// </summary>
        public bool IsPauseReadLoop
        {
            get => p_isReadStoped;
        }

        #endregion

        #region Exit

        /// <summary>
        /// 退出开启的消息循环
        /// </summary>
        /// <param name="IOCancel">
        /// <para>退出时是否取消标准输入的线程等待阻塞</para>
        /// <para>标准输入消息循环在进行I/O读取时，如果没有输入会在I/O阶段进行阻塞式等待；若线程处于阻塞时强行终止，可能会导致不可预估的后果；使用true参数会在循环标识符设为false后，调用<see cref="ConsoleSystem.CancelConsoleIO(IntPtr)"/>取消标准输入阻塞，从而顺利退出线程</para>
        /// </param>
        /// <returns>返回true成功退出；返回false表示循环没有开启</returns>
        public bool ExitLoopFunc(bool IOCancel)
        {
            if (p_startLoop)
            {
                p_startLoop = false;
                p_isReading = true;
                if (IOCancel)
                {
                    ConsoleSystem.CancelConsoleIO(p_h);
                }
                p_pauseReadLoopEvent?.Set();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 退出开启的消息循环并取消标准输入的线程等待阻塞
        /// </summary>
        /// <returns>返回true成功退出；返回false表示循环没有开启</returns>
        public bool ExitLoopFunc()
        {
            return ExitLoopFunc(true);
        }

        #endregion

        #region Read

        /// <summary>
        /// 从缓冲区获取一个输入
        /// </summary>
        /// <param name="input">获取的输入</param>
        /// <returns>true表示成功获取；false表示缓冲区没有输入</returns>
        public bool TryReadInput(out CInR input)
        {
            lock (p_list)
            {
                return p_list.TryDequeue(out input);
            }
        }

        /// <summary>
        /// 查看缓冲区队列第一个输入
        /// </summary>
        /// <remarks>
        /// <para>该函数仅获取缓冲区队列最后的元素，但不清除缓冲区参数</para>
        /// </remarks>
        /// <param name="input">查看的输入</param>
        /// <returns>true表示成功获取参数；false表示缓冲区没有输入</returns>
        public bool TryPeekInput(out CInR input)
        {
            lock (p_list)
            {
                return p_list.TryPeek(out input);
            }
        }

        /// <summary>
        /// 从缓冲区获取一个输入
        /// </summary>
        /// <returns>获取的输入，如果缓冲区没有输入返回null</returns>
        public CInR? ReadInput()
        {
            if (TryReadInput(out var i)) return i;
            return null;
        }

        /// <summary>
        /// 查看缓冲区队列第一个输入
        /// </summary>
        /// <remarks>
        /// <para>该函数仅获取缓冲区队列最后的元素，但不清除缓冲区参数</para>
        /// </remarks>
        /// <returns>要查看的输入，如果缓冲区没有输入返回null</returns>
        public CInR? PeekInput()
        {
            if (TryPeekInput(out var i)) return i;
            return null;
        }

        /// <summary>
        /// 输入队列的缓冲区数量
        /// </summary>
        public int BufferCount
        {
            get => p_list.Count;
        }

        /// <summary>
        /// 使用索引获取缓冲区队列内任意参数
        /// </summary>
        /// <param name="index">队列索引，范围在[0,<see cref="BufferCount"/>)，索引0的参数等同于用<see cref="PeekInput"/>获取的参数</param>
        /// <returns></returns>
        public CInR GetInputByIndex(int index)
        {
            lock (p_list)
            {
                return p_list.LastGetElement(index);
            }
        }

        /// <summary>
        /// 清空当前缓冲区输入队列
        /// </summary>
        public void ClearBuffer()
        {
            lock (p_list)
            {
                p_list.Clear();
            }
        }

        /// <summary>
        /// 每次循环最后调用
        /// </summary>
        /// <param name="readInputCount">此次成功添加到缓冲区的输入消息数量</param>
        protected virtual void OnceGetInputLoop(int readInputCount)
        {
        }

        /// <summary>
        /// 从缓冲区获取一个输入
        /// </summary>
        /// <param name="millisecondsTimeout">等待的毫秒数，如果超时则直接返回null</param>
        /// <returns>获取的输入，如果等待超时后缓冲区也没有输入返回null</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">等待毫秒数是负数</exception>
        public CInR? ReadInput(int millisecondsTimeout)
        {
            if (millisecondsTimeout < 0) throw new ArgumentOutOfRangeException();
            CInR re;
            
            if (TryReadInput(out re))
            {
                return re;
            }
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            p_waitEvent.WaitOne(millisecondsTimeout);
            if (TryReadInput(out re))
            {
                return re;
            }
            return null;
        }

        /// <summary>
        /// 从缓冲区获取一个输入，如果缓冲区没有输入则等待直到存在输入
        /// </summary>
        /// <remarks>
        /// <para>调用此函数的线程会从缓存队列获取输入，如果没有则持续阻塞等待直到消息循环读取输入并添加到缓存队列，否则将持续阻塞线程；请在调用函数前保证已开启或即将开启循环；用<see cref="StartLoop"/>判断是否开启消息循环</para>
        /// </remarks>
        /// <returns>获取的输入</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public CInR Read()
        {
            Loop:
            if (TryReadInput(out var re)) 
            {
                return re;
            }
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            p_waitEvent.WaitOne(300);
            goto Loop;
        }

        /// <summary>
        /// 暂停消息循环的输入读取
        /// </summary>
        /// <remarks>
        /// <para>暂停消息循环的用户输入读取，但不会退出消息循环</para>
        /// <para>在控制台读取用户输入时，如果有多个线程从标准输入流读取会出现资源随机抢占导致数据不稳定，调用该函数暂停循环读取后，可以在其它线程使用<see cref="SCol.ReadLine"/>等操作读取用户输入，不必担心被消息循环抢占资源</para>
        /// <para>调用函数后，只有当<see cref="IsPauseReadLoop"/>是true时，循环才真正进入停止循环读取的状态；<see cref="IsPauseReading"/>只是判断调用方是否开启或关闭的标识符</para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void PauseReadLoop()
        {
            PauseReadLoop(true);
        }

        /// <summary>
        /// 暂停消息循环的输入读取
        /// </summary>
        /// <remarks>
        /// <para>暂停消息循环的用户输入读取，但不会退出消息循环</para>
        /// <para>在控制台读取用户输入时，如果有多个线程从标准输入流读取会出现资源随机抢占导致数据不稳定，调用该函数暂停循环读取后，可以在其它线程使用<see cref="SCol.ReadLine"/>等操作读取用户输入，不必担心被消息循环抢占资源</para>
        /// <para>调用函数后，只有当<see cref="IsPauseReadLoop"/>是true时，循环才真正进入停止循环读取的状态；<see cref="IsPauseReading"/>只是判断调用方是否开启或关闭的标识符</para>
        /// </remarks>
        /// <param name="IOCancel">是否取消标准输入的线程等待阻塞</param>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void PauseReadLoop(bool IOCancel)
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            p_pauseReadLoopEvent.Reset();
            p_isReading = false;
            ConsoleSystem.FlushConsoleInputBuffer(p_h);
            if (IOCancel) ConsoleSystem.CancelConsoleIO(p_h);
        }

        /// <summary>
        /// 重新开启消息循环的输入读取
        /// </summary>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void StartReadLoop()
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            p_isReading = true;
            p_pauseReadLoopEvent.Set();
        }

        /// <summary>
        /// 暂停消息循环的输入读取并等待实际循环暂停
        /// </summary>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void PauseWaitIsReadLoop()
        {
            PauseReadLoop();
            while (!IsPauseReadLoop)
            {
                p_isPausedReadLoopCallback.WaitOne(200);
            }
        }

        /// <summary>
        /// 挂起当前线程，等待实际消息循环暂停读取
        /// </summary>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void WaitIsReadLoop()
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            while (!IsPauseReadLoop)
            {
                p_isPausedReadLoopCallback.WaitOne(200);
            }
        }

        /// <summary>
        /// 挂起当前线程，等待实际循环消息暂停读取
        /// </summary>
        /// <param name="millisecondsTimeout">要等待的最大毫秒数</param>
        /// <returns>如果等待期间循环成功暂停返回true，超时后仍没有成功暂停返回false</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">等待毫秒数不是非负数</exception>
        public bool WaitIsReadLoop(int millisecondsTimeout)
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            if (millisecondsTimeout < 0) throw new ArgumentOutOfRangeException();
            if (IsPauseReadLoop) return true;
            p_isPausedReadLoopCallback.WaitOne(millisecondsTimeout);
            return IsPauseReadLoop;
        }

        /// <summary>
        /// 无限期挂起当前线程，等待缓冲区存在数据
        /// </summary>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void WaitToHaveBuffer()
        {
            Loop:
            if (p_list.Count > 0) return;
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            if (p_waitEvent.WaitOne(200)) return;
            goto Loop;
        }

        /// <summary>
        /// 无限期挂起当前线程，等待消息循环读取输入并添加倒缓冲区
        /// </summary>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public void WaitUntilAddBuffer()
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            if (p_list.Count > 0) return;
            p_waitEvent.WaitOne();
        }

        /// <summary>
        /// 挂起当前线程，等待消息循环读取输入并添加倒缓冲区
        /// </summary>
        /// <param name="millisecondsTimeout">要等待的最大毫秒数，-1 表示无限期等待</param>
        /// <returns>如果在等待期间成功收到数据返回true，等待超时返回false</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">等待毫秒数是-1之外的负数</exception>
        public bool WaitUntilAddBuffer(int millisecondsTimeout)
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            if (p_list.Count > 0) return true;
            return p_waitEvent.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// 挂起当前线程，等待消息循环读取输入并添加倒缓冲区
        /// </summary>
        /// <param name="timeout">要等待的最大时间</param>
        /// <returns>如果在等待期间成功收到数据返回true，等待超时返回false</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">等待时间是-1之外的负数</exception>
        public bool WaitUntilAddBuffer(TimeSpan timeout)
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));
            if (p_list.Count > 0) return true;
            return p_waitEvent.WaitOne(timeout);
        }

        #endregion

        #region 预设

        /// <summary>
        /// 过滤器预设 - 判断键盘按下事件
        /// </summary>
        /// <param name="input">要判断的输入</param>
        /// <returns>当<paramref name="input"/>是按键按下事件时返回true</returns>
        public static bool KeyboardDownFilter(CInR input)
        {
            return input.RecordEventType == ConsoleSystem.EventType.Key && input.Key_KeyDown;
        }

        #endregion

        #endregion

        #region 实现

        private void f_addInput(CInR meg)
        {
            lock (p_list)
            {
                if (p_addType == AddInputType.Cover)
                {
                    if (p_list.Count == p_maxBufCount)
                    {
                        p_list.Dequeue();
                    }
                    p_list.Enqueue(meg);
                }
                else if(p_addType == AddInputType.IgnoreOverflow)
                {
                    if (p_list.Count < p_maxBufCount)
                    {
                        p_list.Enqueue(meg);
                    }
                }
            }
        }

        /// <summary>
        /// 消息循环函数
        /// </summary>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        protected void LoopReadFunction()
        {
            ThrowObjectDisposeException(nameof(STDInputReadLoop));

            if (p_startLoop)
            {
                return;
            }
            const int bufMaxCount = 16;
            p_startLoop = true;
            //var buf = new CInR[16];
            CPtr<CInR> bufP = stackalloc CInR[bufMaxCount];
            int readC;
            int addC;
            while (p_startLoop)
            {
                bool readOver = false;
                var isRead = p_isReading;
                readC = 0;
                p_waitEvent?.Reset();
                if (isRead)
                {
                    p_isReadStoped = false;
                    if (p_lock is null)
                    {
                        readOver = ConsoleSystem.ReadConsoleInput(p_h, bufP, bufMaxCount, out readC);
                    }
                    else
                    {
                        lock (p_lock)
                        {
                            readOver = ConsoleSystem.ReadConsoleInput(p_h, bufP, bufMaxCount, out readC);
                        }
                    }
                }
                else
                {
                    p_isReadStoped = true;
                    p_isPausedReadLoopCallback?.Set();
                    p_pauseReadLoopEvent?.WaitOne(500);
                    p_isPausedReadLoopCallback?.Reset();
                }

                addC = 0;
                if (readOver)
                {
                    //成功读取
                    for (int i = 0; i < readC; i++)
                    {
                        bool isAdd;
                        lock (p_filterLock)
                        {
                            if (p_addMegFilter is null)
                            {
                                isAdd = true;
                            }
                            else
                            {
                                isAdd = p_addMegFilter.Invoke(bufP[i]);
                            }
                        }
                        if (isAdd)
                        {
                            f_addInput(bufP[i]);
                            addC++;
                        }
                    }
                    if(addC > 0) p_waitEvent?.Set();
                }

                if (p_startLoop) OnceGetInputLoop(addC);
            }
            p_waitEvent?.Set();
        }

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            ExitLoopFunc(disposeing);

            if (disposeing)
            {
                p_waitEvent.Close();
                p_pauseReadLoopEvent.Close();
                p_isPausedReadLoopCallback.Close();
            }

            p_waitEvent = null;
            p_pauseReadLoopEvent = null;
            p_isPausedReadLoopCallback = null;

            return true;
        }

        #endregion

    }

}
