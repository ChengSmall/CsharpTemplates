using Cheng.Texts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cheng.Consoles
{

    /// <summary>
    /// 控制台逐步打印器
    /// </summary>
    public sealed class ConsolePrint
    {

        #region 结构

        /// <summary>
        /// 线程等待决定函数
        /// </summary>
        /// <param name="c">要等待前输出的上一个字符</param>
        /// <param name="waitSpan">要等待的基础时间</param>
        /// <returns>最终要等待的时间</returns>
        public delegate TimeSpan WaitByChar(char c, TimeSpan waitSpan);

        #endregion

        #region 构造

        /// <summary>
        /// 初始化打印器
        /// </summary>
        public ConsolePrint()
        {
            p_buffer = new StringBuilder();
        }

        #endregion

        #region 参数

        private StringBuilder p_buffer;

        private TimeSpan p_span;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 待打印文本字符串缓冲区
        /// </summary>
        public StringBuilder StringBuffer
        {
            get => p_buffer;
        }

        /// <summary>
        /// 在延迟打印时每个文字打印后的等待时间
        /// </summary>
        public TimeSpan WFWTimeSpan
        {
            get => p_span;
            set
            {
                var ticks = value.Ticks;
                if ((ticks < 0 || ticks > int.MaxValue))
                {
                    throw new ArgumentOutOfRangeException();
                }
                p_span = value;
            }
        }

        #endregion

        #region 打印

        /// <summary>
        /// 逐步打印缓冲区的字符串到控制台，含线程等待
        /// </summary>
        public void PrintWFW()
        {
            var sb = p_buffer;
            var span = p_span;
            bool wait = span.Ticks > 0;

            lock (sb)
            {

                var length = sb.Length;
                if (wait)
                {
                    for (int i = 0; i < length; i++)
                    {
                        Console.Write(sb[i]);
                        Thread.Sleep(span);
                    }
                }
                else
                {
                    Console.Write(sb.ToString());
                }


            }

        }

        /// <summary>
        /// 逐步打印缓冲区的字符串到控制台并忽略等待样式转义字符，含线程等待
        /// </summary>
        public void StylePrintWFW()
        {
            var sb = p_buffer;
            var span = p_span;
            bool wait = span.Ticks > 0;

            lock (sb)
            {
                int i;
                int style_index;
                var length = sb.Length;
                if (wait)
                {
                    for (i = 0; i < length; i++)
                    {
                        char cs = sb[i];

                        if (cs == ConsoleTextStyle.ASNIStyle_Begin)
                        {
                            //起始索引
                            int beginS = i;

                            BeginStyle:
                            //样式转义
                            style_index = i;

                            if (style_index >= length) break;
                            
                            for ( ; style_index < length; style_index++)
                            {
                                cs = sb[style_index];
                                if(cs == ConsoleTextStyle.ASNIStyle_End)
                                {
                                    //到达样式转义终止符
                                    //后一位索引
                                    i = style_index + 1;

                                    if (i < length)
                                    {
                                        if (sb[i] == ConsoleTextStyle.ASNIStyle_Begin)
                                        {
                                            goto BeginStyle; //属于相邻转义，回去重新数
                                        }
                                        else
                                        {
                                            cs = sb[i];
                                            break; //没转义跳出
                                        }
                                    }
                                    else
                                    {
                                        //最后一位
                                        Console.Write(sb.ToString(beginS, i - beginS));
                                        return;
                                    }
                                    
                                }
                            }

                            //sb.ToString(beginS, style_index - beginS);
                            Console.Write(sb.ToString(beginS, i - beginS));
                        }

                        Console.Write(cs);
                        Thread.Sleep(span);
                    }
                }
                else
                {
                    Console.Write(sb.ToString());
                }


            }

        }

        /// <summary>
        /// （无线程安全）将指定缓冲区字符写入到文本输出对象
        /// </summary>
        /// <param name="buffer">要将其输出的缓冲区</param>
        /// <param name="print">要输出到的目标对象</param>
        /// <param name="waitSpan">每次打印一个文本后进行线程等待的基本等待时间，等于或小于0则不做等待</param>
        /// <param name="ignoreStyleText">是否忽略等待样式转移字符；true将判断并忽略等待样式转义字符，false将不进行判断等待任意字符（当该参数为true时，需要配合虚拟控制台功能）</param>
        /// <param name="SleepFunc">调用该函数进行线程等待，如果为null则使用<see cref="Thread.Sleep(TimeSpan)"/></param>
        /// <param name="waitByCharFunc">
        /// <para>当该参数不为null时，将在每次打印字符后进入线程等待前，调用该函数获取返回值，用返回的值进行线程等待，返回值小于或等于0时不进行等待</para>
        /// <para>该参数为null时直接使用<paramref name="waitSpan"/>做每次线程等待的时间</para>
        /// </param>
        /// <exception cref="ArgumentNullException">缓冲区或输出对象为null</exception>
        public static void PrintForWait(StringBuilder buffer, TextWriter print, TimeSpan waitSpan, bool ignoreStyleText, Action<TimeSpan> SleepFunc, WaitByChar waitByCharFunc)
        {
            if (print is null || buffer is null) throw new ArgumentNullException();
            if (SleepFunc is null) SleepFunc = Thread.Sleep;
            bool waitFunc = waitByCharFunc is object;
            var sb = buffer;
            //var span = waitSpan;
            bool wait = waitSpan > TimeSpan.Zero;

            int i;
            int style_index;
            var length = sb.Length;
            if (wait)
            {
                for (i = 0; i < length; i++)
                {
                    char cs = sb[i];

                    if (ignoreStyleText && cs == ConsoleTextStyle.ASNIStyle_Begin)
                    {
                        //起始索引
                        int beginS = i;

                        BeginStyle:
                        //样式转义
                        style_index = i;

                        if (style_index >= length) break;

                        for (; style_index < length; style_index++)
                        {
                            cs = sb[style_index];
                            if (cs == ConsoleTextStyle.ASNIStyle_End)
                            {
                                //到达样式转义终止符
                                //后一位索引
                                i = style_index + 1;

                                if (i < length)
                                {
                                    if (sb[i] == ConsoleTextStyle.ASNIStyle_Begin)
                                    {
                                        goto BeginStyle; //属于相邻转义，回去重新数
                                    }
                                    else
                                    {
                                        cs = sb[i];
                                        break; //没转义跳出
                                    }
                                }
                                else
                                {
                                    //最后一位
                                    Console.Write(sb.ToString(beginS, i - beginS));
                                    return;
                                }

                            }
                        }

                        //sb.ToString(beginS, style_index - beginS);
                        Console.Write(sb.ToString(beginS, i - beginS));
                    }

                    print.Write(cs);
                    if (waitFunc)
                    {
                        var fwt = waitByCharFunc.Invoke(cs, waitSpan);
                        if(fwt > TimeSpan.Zero) SleepFunc.Invoke(waitSpan);
                    }
                    else
                    {
                        SleepFunc.Invoke(waitSpan);
                    }
                        
                }
            }
            else
            {
                print.Write(sb.ToString());
            }

            

        }

        /// <summary>
        /// 如果判断字符为半角字符，则返回一半时间
        /// </summary>
        /// <param name="c">字符</param>
        /// <param name="wait">基本等待时间</param>
        /// <returns>如果字符<paramref name="c"/>为半角字符，则返回<paramref name="wait"/>的一半，否则返回<paramref name="wait"/></returns>
        public static TimeSpan NotFullWidthHalveTime(char c, TimeSpan wait)
        {
            return TextManipulation.IsFullWidth(c) ? wait : new TimeSpan(wait.Ticks / 2);
        }

        #endregion

        #endregion

    }


}
