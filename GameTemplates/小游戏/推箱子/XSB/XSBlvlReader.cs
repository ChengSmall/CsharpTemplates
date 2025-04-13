using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.GameTemplates.PushingBoxes.XSB
{

    /// <summary>
    /// xsb关卡转化器
    /// </summary>
    public sealed class XSBlvlReader
    {

        #region 构造

        public XSBlvlReader()
        {
            p_lineStrBuf = new List<string>();
        }

        #endregion

        #region 参数

        private List<string> p_lineStrBuf;

        #endregion

        #region 功能

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            p_lineStrBuf.Clear();
        }

        /// <summary>
        /// 判断字符属于xsb内的标准替代符
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static bool IsXSBChar(char value)
        {
            switch (value)
            {
                case XSBReader.XSB_box:
                case XSBReader.XSB_boxOnTarget:
                case XSBReader.XSB_ground_1:
                case XSBReader.XSB_ground_2:
                case XSBReader.XSB_ground_3:
                case XSBReader.XSB_player:
                case XSBReader.XSB_playerOnTarget:
                case XSBReader.XSB_target:
                case XSBReader.XSB_wall:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断整个字符串的所有字符是否有不属于xsb场景格式的字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns>是否有不属于xsb场景格式的字符；字符串为null或空字符串也返回false</returns>
        public static bool IsNotXSBString(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                if (!IsXSBChar(value[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 读取xsb关卡并返回关卡合集
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public PushBoxLevel[] ReaderXSBToPushBoxLevel(TextReader reader)
        {
            var enumator = f_readerXSBToPushBoxLevelEnumator(reader, null);
            enumator = System.Linq.Enumerable.Where(enumator, f_whereFunc);

            return System.Linq.Enumerable.ToArray(enumator);
        }

        private static bool f_whereFunc(PushBoxLevel t_lvl)
        {
            if (t_lvl.exception != null) return false;
            return (object)t_lvl.scene != null;
        }

        /// <summary>
        /// 读取xsb关卡合集返回关卡的枚举器
        /// </summary>
        /// <param name="reader">xsb关卡读取器</param>
        /// <returns>
        /// <para>每次枚举会返回一个关卡实例</para>
        /// <para>如果一次返回的关卡实例参数为null，则此次不属于完整关卡，只有<see cref="PushBoxLevel.scene"/>不为null时才算一个完整的关卡</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">读取器已释放</exception>
        /// <exception cref="Exception">其它读取器错误</exception>
        public IEnumerable<PushBoxLevel> ReaderXSBToPushBoxLevelEnumator(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();

            return f_readerXSBToPushBoxLevelEnumator(reader, null);
        }

        /// <summary>
        /// 读取xsb关卡合集返回关卡的枚举器
        /// </summary>
        /// <param name="reader">xsb关卡读取器</param>
        /// <returns>
        /// <para>每次枚举会返回一个完整的关卡实例</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">读取器已释放</exception>
        /// <exception cref="Exception">其它读取器错误</exception>
        public IEnumerable<PushBoxLevel> ReaderXSBToPushBoxCompleteLevelEnumator(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            var enumator = f_readerXSBToPushBoxLevelEnumator(reader, null);
            enumator = System.Linq.Enumerable.Where(enumator, f_whereFunc);
            return enumator;
        }

        /// <summary>
        /// 读取xsb关卡合集返回关卡的枚举器
        /// </summary>
        /// <param name="reader">xsb关卡读取器</param>
        /// <param name="commentWriter">遇到单行注释或多行注释时，用于写入此实例；null表示不需要接受注释</param>
        /// <returns>
        /// <para>每次枚举会返回一个关卡实例</para>
        /// <para>如果一次返回的关卡实例参数为null，则此次不属于完整关卡，只有<see cref="PushBoxLevel.scene"/>不为null时才算一个完整的关卡</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">读取器已释放</exception>
        /// <exception cref="Exception">其它读取器错误</exception>
        public IEnumerable<PushBoxLevel> ReaderXSBToPushBoxLevelEnumator(TextReader reader, TextWriter commentWriter)
        {
            if (reader is null) throw new ArgumentNullException();
            return f_readerXSBToPushBoxLevelEnumator(reader, commentWriter);
        }

        /// <summary>
        /// 读取xsb关卡合集返回关卡的枚举器
        /// </summary>
        /// <param name="reader">xsb关卡读取器</param>
        /// <param name="commentWriter">遇到单行注释或多行注释时，用于写入此实例；null表示不需要接受注释</param>
        /// <returns>
        /// <para>每次枚举会返回一个完整的关卡实例</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">读取器已释放</exception>
        /// <exception cref="Exception">其它读取器错误</exception>
        public IEnumerable<PushBoxLevel> ReaderXSBToPushBoxCompleteLevelEnumator(TextReader reader, TextWriter commentWriter)
        {
            if (reader is null) throw new ArgumentNullException();
            var enumator = f_readerXSBToPushBoxLevelEnumator(reader, commentWriter);
            enumator = System.Linq.Enumerable.Where(enumator, f_whereFunc);
            return enumator;
        }

        #region

        private enum xsbType
        {
            /// <summary>
            /// 错误格式
            /// </summary>
            Error = -1,
            /// <summary>
            /// 空行
            /// </summary>
            Empty = 0,
            /// <summary>
            /// 空引用
            /// </summary>
            Null = -2,
            /// <summary>
            /// xsb关卡
            /// </summary>
            XSBlvl = 1,
            /// <summary>
            /// 标题
            /// </summary>
            Title = 2,
            /// <summary>
            /// 作者
            /// </summary>
            Author = 3,
            /// <summary>
            /// 多行注释 <![CDATA[Comment -> Comment-End]]>
            /// </summary>
            Comment = 4,
            /// <summary>
            /// 单行注释 ;message
            /// </summary>
            SingleLineComment
        }

        [Flags]
        private enum overType : byte
        {
            None = 0,
            Title = 0b1,
            Author = 0b10,
            XSBlvl = 0b100,
            AllOK = 0b111
        }

        private xsbType f_checkType(string line)
        {
            if (line is null) return xsbType.Null;

            int length = line.Length;
            if (length == 0) return xsbType.Empty;
            bool b;
            
            if(line[0] == ';')
            {
                return xsbType.SingleLineComment;
            }

            if (length > 4)
            {
                b = (char.ToLower(line[0]).Equals('t') && char.ToLower(line[1]).Equals('i') && char.ToLower(line[2]).Equals('t') && char.ToLower(line[3]).Equals('l') && char.ToLower(line[4]).Equals('e'));

                if (b)
                {
                    return xsbType.Title;
                }

                if (length > 5)
                {
                    b = (char.ToLower(line[0]).Equals('a') && char.ToLower(line[1]).Equals('u') && char.ToLower(line[2]).Equals('t') && char.ToLower(line[3]).Equals('h') && char.ToLower(line[4]).Equals('o') && char.ToLower(line[5]).Equals('r') && line[6].Equals(':'));

                    if (b)
                    {
                        return xsbType.Author;
                    }

                    b = (char.ToLower(line[0]).Equals('c') && char.ToLower(line[1]).Equals('o') && char.ToLower(line[2]).Equals('m') && char.ToLower(line[3]).Equals('m') && char.ToLower(line[4]).Equals('e') && char.ToLower(line[5]).Equals('n') && line[6].Equals('t'));
                    if (b)
                    {
                        return xsbType.Comment;
                    }

                }
            }

            for (int i = 0; i < length; i++)
            {
                if (!IsXSBChar(line[i]))
                {
                    //不是xsb
                    goto Over;
                }
            }
            return xsbType.XSBlvl;

            Over:

            if (string.IsNullOrWhiteSpace(line)) return xsbType.Empty;
            return xsbType.Error;

        }

        /// <summary>
        /// 读取Comment注释并推进
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line">作为第一行"Comment:"传入，并确保是"Comment:"；结束后在语句的后一行开始返回引用</param>
        /// <param name="append">接受注释的缓冲区，null表示不接受注释仅推进文本</param>
        /// <returns>成功读取完毕，并结尾是"Comment-End:"则返回true，没有碰到结尾标识但读取器结束返回false</returns>
        private bool f_reader_comment(TextReader reader, ref string line, TextWriter append)
        {
            /*
            开头 Comment:
            结尾 Comment-End:
            */
            bool isAppend = (object)append != null;

            while (true)
            {

                line = reader.ReadLine();

                var lineToLor = line?.ToLower()?.Trim();

                switch (lineToLor)
                {
                    case "comment-end:":
                    case "comment-end":
                    case "comment end:":
                    case "comment end":
                        line = reader.ReadLine();
                        return true;
                    case null:
                        return false;
                }

                if (isAppend)
                {
                    append.WriteLine(line);
                }

            }
            
            //return true;
        }

        /// <summary>
        /// 读取分号开头的单行注释
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line">第一个字符是';'的单行注释，推进接受引用</param>
        /// <param name="append">添加单行注释内容的缓冲区，null表示不添加</param>
        private void f_reader_singleLineComment(TextReader reader, ref string line, TextWriter append)
        {
            append?.WriteLine(line.Substring(1, line.Length - 1));
            line = reader.ReadLine();
        }

        /// <summary>
        /// 尝试读取xsb
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line">作为起始的第一行，并确保属于xsb格式的一行；有数据则使用此数据作为第一行，null则先行读取一行，结束后line作为最后一行的末尾一行返回</param>
        /// <param name="scene"></param>
        /// <returns>是否成功读取关卡</returns>
        private bool f_reader_xsb(TextReader reader, ref string line, out PushBoxScene scene)
        {
            scene = null;

            if(line is null)
            {
                line = reader.ReadLine();
            }

            if (IsNotXSBString(line))
            {
                return false;
            }

            var lineStrBuf = p_lineStrBuf;

            //非空行
            int width, height;
            lineStrBuf.Clear();

            lock (lineStrBuf)
            {

                //将有效数据读取完毕
                while (true)
                {
                    lineStrBuf.Add(line);
                    line = reader.ReadLine();
                    if (IsNotXSBString(line))
                    {
                        //非xsb行
                        break;
                    }
                }
                //纵列
                height = lineStrBuf.Count;
                //长度
                width = System.Linq.Enumerable.Max(lineStrBuf, strLineLengthToInt);

                int strLineLengthToInt(string tf_value)
                {
                    return tf_value.Length;
                }

                //实例化场景
                scene = new PushBoxScene(width, height);
                scene.ResetEmpty();
                SceneGrid sceneGrid;
                string sceneLine;
                for (int t_y = 0; t_y < height; t_y++)
                {
                    //读取一行
                    sceneLine = lineStrBuf[t_y];

                    for (int t_x = 0; t_x < width && t_x < sceneLine.Length; t_x++)
                    {
                        if (XSBReader.XSBToGrid(sceneLine[t_x], out sceneGrid))
                        {
                            scene[t_x, (height - 1) - t_y] = sceneGrid;
                        }
                        else
                        {
                            //读取到不属于xsb的格式
                            //跳过
                            lineStrBuf.Clear();
                            return false;
                        }

                    }
                }

                lineStrBuf.Clear();
            }

            return true;
        }

        /// <summary>
        /// 返回作者行的作者信息并推进
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line">作为作者行传入，并如果有信息则保证是信息正确的作者行，执行后推进读取由此引用接收</param>
        /// <param name="author"></param>
        private void f_reader_author(TextReader reader, ref string line, out string author)
        {
            author = line.Substring(7).Trim();
            line = reader.ReadLine();
        }

        /// <summary>
        /// 返回标题行并推进
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line">标题行信息，执行后推进读取由此引用接收</param>
        /// <param name="title">返回的标题</param>
        private void f_reader_title(TextReader reader, ref string line, out string title)
        {
            title = line.Substring(6).Trim();
            line = reader.ReadLine();
        }


        private IEnumerable<PushBoxLevel> f_readerXSBToPushBoxLevelEnumator(TextReader reader, TextWriter commentAppend)
        {

            //List<PushBoxLevel> p_readList = p_listBuf;

            string title = null, author = null;
            PushBoxScene scene = null;
            string line;

            overType onceIsReader;
            xsbType xtype;

            //读取第一行
            line = reader.ReadLine();
            bool isOver = false;
            while (true)
            {

                //重置真值集
                onceIsReader = overType.None;

                WhileHead:

                if(onceIsReader == overType.AllOK)
                {
                    //结束一轮
                    goto AllOver;
                }

                if (isOver)
                {
                    yield return new PushBoxLevel(title, author, scene);
                    break;
                }

                //读取行类型
                xtype = f_checkType(line);

                switch (xtype)
                {
                    case xsbType.Empty:
                        //空行，跳过
                        line = reader.ReadLine();
                        break;
                    case xsbType.Null:
                        //读取到结尾
                        isOver = true;
                        break;
                    case xsbType.Error:
                        //错误，直接结束
                        yield return new PushBoxLevel(new FormatException("XSB文本有错误格式"));
                        line = reader.ReadLine();
                        continue;
                    case xsbType.Title:
                        if((onceIsReader & overType.Title) == overType.Title)
                        {
                            //一次循环已过
                            goto AllOver;
                        }
                        //设置此次轮循标题完毕
                        onceIsReader |= overType.Title;

                        f_reader_title(reader, ref line, out title);
                        break;
                    case xsbType.Author:
                        if ((onceIsReader & overType.Author) == overType.Author)
                        {
                            //一次循环已过
                            goto AllOver;
                        }
                        onceIsReader |= overType.Author;

                        f_reader_author(reader, ref line, out author);
                        break;
                    case xsbType.Comment:
                        f_reader_comment(reader, ref line, commentAppend);
                        //注释
                        break;
                    case xsbType.SingleLineComment:
                        //单行注释
                        //推进
                        f_reader_singleLineComment(reader, ref line, commentAppend);
                        break;
                    case xsbType.XSBlvl:
                    default:
                        if ((onceIsReader & overType.XSBlvl) == overType.XSBlvl)
                        {
                            //一次循环已过
                            goto AllOver;
                        }
                        onceIsReader |= overType.XSBlvl;
                        //关卡
                        if(!f_reader_xsb(reader, ref line, out scene))
                        {
                            //关卡中间解析错误
                            //跳过
                            scene = null;
                        }
                        break;
                }

                yield return default;
                goto WhileHead;

                AllOver:
                yield return new PushBoxLevel(title, author, scene);
                title = null;
                author = null;
                scene = null;

            }

            p_lineStrBuf.Clear();
        }

        #endregion

        #endregion

    }

}
