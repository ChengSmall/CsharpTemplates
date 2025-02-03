using System;
using System.Collections.Generic;
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

        #endregion

        #endregion

    }


}
