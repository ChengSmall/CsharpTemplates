using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Consoles
{

    /// <summary>
    /// 控制台更改文本样式的ASNI转移序列
    /// </summary>
    /// <remarks>使用前需要开启控制台虚拟终端；使用<see cref="Consoles.VirtualTerminalConsole"/>开启虚拟终端</remarks>
    public unsafe static class ConsoleTextStyle
    {

        /// <summary>
        /// 样式转义字符
        /// </summary>
        public const char ASNIStyle_Begin = '\u001B';

        /// <summary>
        /// 样式转义终止符
        /// </summary>
        public const char ASNIStyle_End = 'm';

        static void f_appendByte(this StringBuilder append, byte value)
        {
            char c;
            int r;
            //获取左侧

            if (value > 199)
            {
                append.Append('2');
            }
            else if (value > 99)
            {
                append.Append('1');
            }

            if (value > 9)
            {
                r = (value / 10) % 10;
                c = (char)('0' + r);
                append.Append(c);
            }

            r = value % 10;
            c = (char)('0' + r);
            append.Append(c);

        }

        static void f_appendByte(this TextWriter append, byte value)
        {
            char c;
            int r;
            //获取左侧

            if (value > 199)
            {
                append.Write('2');
            }
            else if (value > 99)
            {
                append.Write('1');
            }

            if (value > 9)
            {
                r = (value / 10) % 10;
                c = (char)('0' + r);
                append.Write(c);
            }

            r = value % 10;
            c = (char)('0' + r);
            append.Write(c);
        }

        /// <summary>
        /// 转化修改文本颜色的ASNI转义序列
        /// </summary>
        /// <param name="r">颜色的R值</param>
        /// <param name="g">颜色的G值</param>
        /// <param name="b">颜色的B值</param>
        /// <param name="isBackground">是否为背景色，背景色则为true，前景色（文本颜色）则为false</param>
        /// <param name="append">要转化后写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void ColorToText(byte r, byte g, byte b, bool isBackground, StringBuilder append)
        {
            if (append is null) throw new ArgumentNullException();

            append.Append(ASNIStyle_Begin);
            append.Append('[');
            append.Append((isBackground ? "48;" : "38;"));
            append.Append("2;");
            append.f_appendByte(r);
            append.Append(';');
            append.f_appendByte(g);
            append.Append(';');
            append.f_appendByte(b);
            append.Append(ASNIStyle_End);
        }

        /// <summary>
        /// 转化修改文本颜色的ASNI转义序列
        /// </summary>
        /// <param name="r">颜色的R值</param>
        /// <param name="g">颜色的G值</param>
        /// <param name="b">颜色的B值</param>
        /// <param name="isBackground">是否为背景色，背景色则为true，前景色（文本颜色）则为false</param>
        /// <param name="append">要转化后写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void ColorToText(byte r, byte g, byte b, bool isBackground, TextWriter append)
        {
            if (append is null) throw new ArgumentNullException();

            append.Write(ASNIStyle_Begin);
            append.Write('[');
            append.Write((isBackground ? "48;" : "38;"));
            append.Write("2;");
            append.f_appendByte(r);
            append.Write(';');
            append.f_appendByte(g);
            append.Write(';');
            append.f_appendByte(b);
            append.Write(ASNIStyle_End);
        }

        /// <summary>
        /// 计算指定颜色转化到ASNI文本转义序列字符的字符数量
        /// </summary>
        /// <param name="r">颜色的R值</param>
        /// <param name="g">颜色的G值</param>
        /// <param name="b">颜色的B值</param>
        /// <returns>指定颜色的ASNI文本转义序列字符串的字符数量</returns>
        public static int ColorToTextCount(byte r, byte g, byte b)
        {
            int count;
            if (r > 99)
            {
                count = 3;
            }
            else if (r > 9)
            {
                count = 2;
            }
            else
            {
                count = 1;
            }

            if (g > 99)
            {
                count += 3;
            }
            else if (r > 9)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }

            if (b > 99)
            {
                count += 3;
            }
            else if (r > 9)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }

            return 10 + count;
        }

        /// <summary>
        /// 转化修改文本颜色的ASNI转义序列
        /// </summary>
        /// <param name="r">颜色的R值</param>
        /// <param name="g">颜色的G值</param>
        /// <param name="b">颜色的B值</param>
        /// <param name="isBackground">是否为背景色，背景色则为true，前景色（文本颜色）则为false</param>
        /// <returns>转化后的ASNI转义序列文本</returns>
        public static string ColorToText(byte r, byte g, byte b, bool isBackground)
        {
            int count;
            if (r > 99)
            {
                count = 3;
            }
            else if(r > 9)
            {
                count = 2;
            }
            else
            {
                count = 1;
            }

            if (g > 99)
            {
                count += 3;
            }
            else if (r > 9)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }

            if (b > 99)
            {
                count += 3;
            }
            else if (r > 9)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }

            StringBuilder sb = new StringBuilder(10 + count);
            ColorToText(r, g, b, isBackground, sb);
            return sb.ToString();
        }

        /// <summary>
        /// 将文本样式重置为默认的ASNI转义序列
        /// </summary>
        public const string ResetColorStyleText = "\u001B[0m";

        /// <summary>
        /// 按照颜色参数转化为修改字符颜色的ANSI转义序列并写入
        /// </summary>
        /// <param name="append"></param>
        /// <param name="r">红色</param>
        /// <param name="g">绿色</param>
        /// <param name="b">蓝色</param>
        /// <param name="isBackground">是否是背景色，true表示背景色，false表示前景色（文本颜色）</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void WriteANSIColorText(this TextWriter append, byte r, byte g, byte b, bool isBackground)
        {
            ColorToText(r, g, b, isBackground, append);
        }

        /// <summary>
        /// 按照颜色参数转化为修改字符颜色的ANSI转义序列并写入
        /// </summary>
        /// <param name="append"></param>
        /// <param name="r">红色</param>
        /// <param name="g">绿色</param>
        /// <param name="b">蓝色</param>
        /// <param name="isBackground">是否是背景色，true表示背景色，false表示前景色（文本颜色）</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void AppendANSIColorText(this StringBuilder append, byte r, byte g, byte b, bool isBackground)
        {
            ColorToText(r, g, b, isBackground, append);
        }

        /// <summary>
        /// 添加重置文字样式的ANSI转义字符序列
        /// </summary>
        /// <param name="append"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteANSIResetColorText(this TextWriter append)
        {
            if (append is null) throw new ArgumentNullException();
            append.Write(ResetColorStyleText);
        }

        /// <summary>
        /// 添加重置文字样式的ANSI转义字符序列
        /// </summary>
        /// <param name="append"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AppendANSIResetColorText(this StringBuilder append)
        {
            if (append is null) throw new ArgumentNullException();
            append.Append(ResetColorStyleText);
        }

    }

}
