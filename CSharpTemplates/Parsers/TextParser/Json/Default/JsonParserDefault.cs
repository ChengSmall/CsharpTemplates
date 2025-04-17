using Cheng.Texts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Cheng.Json
{

    /// <summary>
    /// Json默认实现的解析器，能够动态化解析json文本
    /// </summary>
    /// <remarks>
    /// <para>该解析器允许json文本使用注释：//单行注释 和 /*多行注释*/；</para>
    /// <para>可以定义键值对对象转化方式，数值超出调剂，超出范围的数值自动转化为字符串实例，转化为文本时适当换行缩进，格式化\u文本，区域性文本转化</para>
    /// </remarks>
    public unsafe sealed class JsonParserDefault : JsonParser
    {

        #region 结构

        /// <summary>
        /// 键值对转化方式
        /// </summary>
        public enum ConverDictionaryType : byte
        {
            /// <summary>
            /// 没有转化方式
            /// </summary>
            /// <remarks>
            /// 每次转化一对键值对时使用<see cref="JsonDictionary.Add(string, JsonVariable)"/>添加
            /// </remarks>
            None,
            /// <summary>
            /// 覆盖旧值
            /// </summary>
            /// <remarks>
            /// 当后添加的键与已添加的键相同时，将此键所对应的值替换为新添加的值
            /// </remarks>
            Cover,
            /// <summary>
            /// 仅添加新值
            /// </summary>
            /// <remarks>
            /// 当后添加的键与已添加的键相同时，保留旧键值
            /// </remarks>
            Add
        }

        #endregion

        #region 参数

        private CultureInfo p_cultureInfo;

        private char[] p_charArrayBuffer;
        private string p_newLine;
        private ConverDictionaryType p_converDictType;

        private NumberStyles p_numStyles;

        private bool p_toJsonTextNewLine;
        private bool p_JsonNotConverNumToString;

        /// <summary>
        /// 文本转化到对象的转义字符开关
        /// </summary>
        private bool p_toObjEscapeChar;

        /// <summary>
        /// 对象转化文本的转义字符开关
        /// </summary>
        private bool p_toTextEscapeChar;

        /// <summary>
        /// 对象转文本时全转\u格式
        /// </summary>
        private bool p_jsonToTextIsUnicode;

        /// <summary>
        /// 对象转文本的key时全转\u格式
        /// </summary>
        private bool p_jsonToKeyIsUnicode;

        /// <summary>
        /// 对象转义字符单引号从文本到对象的转义开关
        /// </summary>
        private bool p_jsonEscapeCharacterSingleQuotation;

        /// <summary>
        /// 从对象到文本的单引号转义字符开关
        /// </summary>
        private bool p_jsonWriterCharacterSingleQuotation;

        #endregion

        #region 封装

        #region 读取
        /// <summary>
        /// 读取字符到缓冲区charArrayBuffer
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="count">读取的字符数</param>
        /// <param name="charArrayBuffer">读取到的缓冲区</param>
        /// <returns>是否读取到了指定的字符数</returns>
        private bool read(TextReader reader, int count, ref char[] charArrayBuffer)
        {
            if (count > charArrayBuffer.Length)
            {
                int newc = charArrayBuffer.Length * 2;
                Array.Resize(ref charArrayBuffer, count > newc ? count : newc);
            }
            int rsize;

            rsize = reader.ReadBlock(charArrayBuffer, 0, count);

            return rsize == count;
        }

        /// <summary>
        /// 读取一个字符到charBuffer
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charBuffer">读取到的引用</param>
        /// <returns>是否能够读取</returns>
        private bool read(TextReader reader, out char charBuffer)
        {
            charBuffer = default;
            int r = reader.Read();
            if (r == -1) return false;
            charBuffer = (char)r;
            return true;
        }

        /// <summary>
        /// Peek一个字符到charBuffer
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="charBuffer">读取到的引用</param>
        /// <returns></returns>
        private bool peek(TextReader reader, out char charBuffer)
        {
            charBuffer = default;
            int r = reader.Peek();
            if (r == -1) return false;
            charBuffer = (char)r;
            return true;
        }

        /// <summary>
        /// 推进一个字符位置
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>是否到达结尾</returns>
        private bool next(TextReader reader)
        {
            return reader.Read() != -1;
        }
        #endregion

        #region 判断
        /// <summary>
        /// 判断<paramref name="ch"/>是否属于<paramref name="cs"/>集合内的元素
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        private static bool IsContainChar(char ch, char[] cs)
        {
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i] == ch) return true;
            }
            return false;
        }
        /// <summary>
        /// 判断类型
        /// </summary>
        /// <param name="c">判断</param>
        /// <param name="type">类型</param>
        /// <param name="isNum">是数值</param>
        /// <returns>是否合法</returns>
        private static bool IsJsonType(char c, out JsonType type, out bool isNum)
        {
            isNum = false;

            if (c == '{')
            {
                type = JsonType.Dictionary;
            }
            else if (c == '[')
            {
                type = JsonType.List;
            }
            else if (c == '\"')
            {
                type = JsonType.String;
            }
            else if (c == 't' || c == 'f')
            {
                type = JsonType.Boolean;
            }
            else if ((c >= '0' && c <= '9') || (c == '-'))
            {
                type = JsonType.RealNum;
                isNum = true;
            }
            else if (c == 'n')
            {
                type = JsonType.Null;
            }
            else
            {
                type = default;
                return false;
            }

            return true;
        }
        /// <summary>
        /// 将int字符转化为字符
        /// </summary>
        /// <param name="i"></param>
        /// <param name="c"></param>
        /// <returns>是否有效</returns>
        private static bool f_intToChar(int i, out char c)
        {
            c = (char)i;
            return i != -1;
        }
        /// <summary>
        /// peek读取器字符并判断是否到头
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool f_peekToChar(TextReader reader, out char c)
        {
            int i = reader.Peek();
            c = (char)i;
            return i != -1;
        }
        #endregion

        #region 排异

        /// <summary>
        /// 检查忽略文本
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsIngoringChar(char c)
        {
            //CharUnicodeInfo.GetUnicodeCategory('s');
            return char.IsWhiteSpace(c);

            //return c == ' ' || c == '\r' || c == '\n' || c == '\t' || c == '\b' || c == '\v';
        }

        /// <summary>
        /// 判断字符串内是否包含指定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        static bool isCharinStr(string str, char c)
        {
            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                if (str[i] == c) return true;
            }
            return false;
        }

        /// <summary>
        /// 跳过注释
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>是否合法</returns>
        private bool f_jumpExplanatory(TextReader reader)
        {
            if (!read(reader, 2, ref p_charArrayBuffer)) return false;

            char c = p_charArrayBuffer[1];
            int ic;

            if (c == '/')
            {
                //单行注释
                while (true)
                {
                    ic = reader.Read();
                    if (ic == -1) return false;
                    c = (char)ic;

                    if (isCharinStr(p_newLine, c))
                    {
                        //换行
                        LoopNewLine:
                        ic = reader.Peek();
                        if (ic == -1) return false;
                        c = (char)ic;

                        if (isCharinStr(p_newLine, c))
                        {
                            //包含换行符
                            reader.Read();
                            goto LoopNewLine;
                        }

                        return true;
                        
                    }
                }

                //return true;
            }

            if (c == '*')
            {
                //扩充注释

                while (true)
                {
                    ic = reader.Read();
                    if (ic == -1) return false;
                    c = (char)ic;

                    if (c == '*')
                    {
                        ic = reader.Peek();
                        if (ic == -1) return false;
                        c = (char)ic;
                        if (c == '/')
                        {
                            //截止到后止符
                            reader.Read();
                            break;
                        }
                    }
                }

                return true;
            }
            //非法语句
            return false;
        }

        /// <summary>
        /// 跳过可忽略文本
        /// </summary>
        /// <param name="read"></param>
        /// <returns>是否成功跳过</returns>
        private bool f_jumpIngoringText(TextReader read)
        {
            int ic;
            char c;

            Start:
            ic = read.Peek();

            if (ic == -1) return false;

            c = (char)ic;
            if (IsIngoringChar(c))
            {
                //可忽略字符
                read.Read();
                goto Start;
            }

            if (c == '/')
            {
                //注释

                if (!f_jumpExplanatory(read)) return false;

                goto Start;
            }

            //不是忽略文本

            return true;
        }

        #endregion

        #region 转化到对象

        #region null

        /// <summary>
        /// 完全判断null
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private bool ConverNull(TextReader reader)
        {
            if (!read(reader, 4, ref p_charArrayBuffer)) return false;

            char[] buf = p_charArrayBuffer;

            return buf[0] == 'n' && buf[1] == 'u' && buf[2] == 'l' && buf[3] == 'l';
        }

        #endregion

        #region boolean

        /// <summary>
        /// 判断并转化为布尔值
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="b">首字符</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否合规</returns>
        private bool ConverBoolean(TextReader reader, char b, out bool value)
        {
            value = true;

            //if (!read(reader, 4)) throw new NotImplementedException();
            ref char[] buf = ref p_charArrayBuffer;

            if (b == 't')
            {
                //true
                if (!read(reader, 4, ref p_charArrayBuffer)) return false;
                return buf[0] == 't' && buf[1] == 'r' && buf[2] == 'u' && buf[3] == 'e';
            }
            else if (b == 'f')
            {
                if (!read(reader, 5, ref p_charArrayBuffer)) return false;

                //false;
                value = false;
                return buf[0] == 'f' && buf[1] == 'a' && buf[2] == 'l' && buf[3] == 's' && buf[4] == 'e';
            }

            return false;
        }
        #endregion

        #region string

        /// <summary>
        /// 将4个代表16位的16进制字符转化为一个16位值
        /// </summary>
        /// <param name="c4">第4位16进制数</param>
        /// <param name="c3">第3位16进制数</param>
        /// <param name="c2">第2位16进制数</param>
        /// <param name="c1">第1位16进制数</param>
        /// <param name="reValue">转化后的值</param>
        /// <returns>是否符合格式</returns>
        static bool f_charToX16(char c4, char c3, char c2, char c1, out ushort reValue)
        {
            //const ushort ToLopper = 0b00000000_00000000;

            const ushort ToUpper = 0b11111111_11011111;
            //ushort uv;

            char ct;

            int de;


            ct = c1;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                reValue = 0;
                return false;
            }

            reValue = (ushort)(de);

            ct = c2;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                return false;
            }


            reValue |= (ushort)(de << 4);


            ct = c3;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                return false;
            }

            reValue |= (ushort)(de << 8);

            ct = c4;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                return false;
            }

            reValue |= (ushort)(de << 12);

            return true;
        }

        /// <summary>
        /// 转义字符推进
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        private bool f_EscapeCharacter(TextReader reader, StringBuilder sb)
        {

            int ic = reader.Read();
            if (ic == -1) return false;

            if (ic == '"')
            {
                sb.Append('"');
                return true;
            }
            if (ic == '\'' && p_jsonEscapeCharacterSingleQuotation)
            {
                sb.Append('\'');
                return true;
            }
            if (ic == 'r')
            {
                sb.Append('\r');
                return true;
            }
            if (ic == 'n')
            {
                sb.Append('\n');
                return true;
            }
            if (ic == '\\')
            {
                sb.Append('\\');
                return true;
            }
            if (ic == 'b')
            {
                sb.Append('\b');
                return true;
            }
            if (ic == 't')
            {
                sb.Append('\t');
                return true;
            }
            if (ic == 'a')
            {
                sb.Append('\a');
                return true;
            }
            if (ic == '0')
            {
                sb.Append('\0');
                return true;
            }
            if (ic == 'v')
            {
                sb.Append('\v');
                return true;
            }

            if(ic == 'u')
            {
                //16位通用字符码表达式

                if(!read(reader, 4, ref p_charArrayBuffer))
                {
                    //没有读取完
                    return false;
                }

                //读取完毕4位字符
                ushort rec;
                if(!f_charToX16(p_charArrayBuffer[3], p_charArrayBuffer[2], p_charArrayBuffer[1], p_charArrayBuffer[0], out rec))
                {
                    //错误格式
                    return false;
                }

                sb.Append(((char)rec));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断并转化为字符串到strBuffer缓冲区
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>是否成功</returns>
        private bool ConverString(TextReader reader, StringBuilder sb)
        {
            int next;

            next = reader.Read();
            if (next == -1)
            {
                return false;
            }

            //ref char clate = ref *((char*)&late);
            ref char cnext = ref *((char*)&next);

            sb.Clear();

            while (true)
            {
                //向后推进
                //late = next;
                next = reader.Read();
                if (next == -1) return false;


                if (p_toObjEscapeChar)
                {
                    if (cnext == '\\')
                    {
                        //转义字符
                        if (!f_EscapeCharacter(reader, sb)) return false;

                        continue;
                    }
                }


                if (cnext == '\"')
                {
                    //结尾
                    return true;
                }

                //其它字符
                sb.Append(cnext);
            }

        }

        #endregion

        #region number

        /// <summary>
        /// 检查所有可能是数值的字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns>-1表示不是数字符号，0表示数字，1表示十进制数的10-15位数，2表示小数点，3表示负数前缀，4表示其它数字符号</returns>
        private int f_checkNumberChar(char c)
        {
            NumberFormatInfo nf = p_cultureInfo?.NumberFormat;

            if (nf is null)
            {
                if (c >= 48 && c <= 57)
                {
                    return 0;
                }
                if ((c >= 65 && c <= 70) || (c >= 97 && c <= 102))
                {
                    return 1;
                }

                if (c == '.') return 2;
                if (c == '-') return 3;

                if (c == '+' || c == 'e') return 4;

                return -1;
            }
            //if (c == ',') return false;

            if (char.IsDigit(c))
            {
                if (c < 48 || c > 57) return 1;
                return 0;
            }

            if (nf.NumberDecimalSeparator.IndexOf(c) != -1)
            {
                //是小数分隔符
                return 2;
            }
            if (nf.NegativeSign.IndexOf(c) != -1)
            {
                return 3;
            }

            //if (nf.NumberGroupSeparator.IndexOf(c) != -1) return true;

            if (nf.NegativeInfinitySymbol.IndexOf(c) != -1)
            {
                return 4;
            }
            if (nf.NegativeInfinitySymbol.IndexOf(c) != -1)
            {
                return 4;
            }

            return -1;

        }

        /// <summary>
        /// 读取可能的表值字符串
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="nextChar">如果读取后向后超出一位，则该值表示后一位字符，推进器表示第二位</param>
        /// <returns></returns>
        private string f_readNumberText(TextReader reader, out int nextChar)
        {
            nextChar = -1;
            char charBuffer;
            StringBuilder sb = new StringBuilder();

            //上一循环有读取到小数点
            //bool lateReadSep = false;

            while (true)
            {
                if (!peek(reader, out charBuffer))
                {
                    break;
                }

                int ir = f_checkNumberChar(charBuffer);
                if (ir == -1)
                {
                    //不是数值
                    break;
                }

                if (ir == 2)
                {
                    //是小数点
                    //lateReadSep = true;

                    if (!read(reader, out char crr))
                    {
                        nextChar = crr;
                        break;
                    }

                    nextChar = crr;

                    if (!peek(reader, out crr))
                    {
                        //nextChar = crr;
                        //小数点后没有了
                        break;
                    }

                    ir = f_checkNumberChar(crr);
                    if (ir == -1)
                    {
                        //小数点后不是数字
                        break;
                    }
                    //小数点后是数字符号

                    if (ir == 0 || ir == 1 || ir == 4)
                    {                       
                        sb.Append(charBuffer);
                    }                    
                    continue;
                }
                else
                {
                    //lateReadSep = false;
                }

                //是数
                sb.Append(charBuffer);
                next(reader);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 判断整数或浮点数并转化
        /// </summary>
        /// <param name="reader">读取json文本</param>
        /// <param name="json">转化到的引用</param>
        /// <returns>是否成功</returns>
        private bool f_converNum(TextReader reader, out JsonVariable json, out int nextChar)
        {
            long i64;
            double d;
            bool flag;

            string str = f_readNumberText(reader, out nextChar);

            var nf = p_cultureInfo?.NumberFormat;

            if (nf is null)
            {
                flag = long.TryParse(str, out i64);
                if (flag)
                {
                    json = new JsonInteger(i64);
                    return true;
                }

                flag = double.TryParse(str, out d);
                if (flag)
                {
                    json = new JsonRealNumber(d);
                    return true;
                }
            }
            else
            {

                flag = long.TryParse(str, p_numStyles, nf, out i64);
                if (flag)
                {
                    json = new JsonInteger(i64);
                    return true;
                }

                flag = double.TryParse(str, p_numStyles, nf, out d);
                if (flag)
                {
                    json = new JsonRealNumber(d);
                    return true;
                }
            }

            if (p_JsonNotConverNumToString)
            {
                json = new JsonString(str);
                return true;
            }

            json = null;
            return false;
        }

        #endregion

        #region 复合对象

        #region 集合

        /// <summary>
        /// 判断并转化集合
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="json">实例化的空集合</param>
        /// <returns></returns>
        private bool ConverList(TextReader reader, JsonList json)
        {
            // [, , ,]
            
            bool flag;
            const char end = ']';
            const char fen = ',';

            char c;
            flag = read(reader, out c);
            if (!flag)
            {
                return false;
            }
            if (c != '[')
            {
                return false;
            }

            //跳过忽略文本
            flag = f_jumpIngoringText(reader);
            if (!flag) return false;

            flag = peek(reader, out c);
            if (!flag) return false;

            //空集合
            if (c == end)
            {
                next(reader);
                return true;
            }

            JsonVariable item;

            int nextChar;

            while (true)
            {
                //读取值
                flag = ConverJsonText(reader, out item, out nextChar);
                if (!flag) return false;

                //存值
                json.Add(item);

                if (nextChar == fen)
                {
                    //等于分隔符

                    //跳过忽略文本
                    flag = f_jumpIngoringText(reader);
                    if (!flag) return false;

                    //peek字符
                    flag = peek(reader, out c);
                    if (!flag) return false;

                    if (c == end)
                    {
                        return true; //空分隔符
                    }

                    //不是终止符
                    //跳过忽略文本
                    flag = f_jumpIngoringText(reader);
                    if (!flag) return false;
                    continue;
                }

                if(nextChar == end)
                {
                    return true;
                }

                //跳过忽略文本
                flag = f_jumpIngoringText(reader);
                if (!flag) return false;

                flag = read(reader, out c);
                if (!flag) return false;

                //终结符
                if (c == end)
                {
                    return true;
                }
                //不是分隔符
                if(c != fen) return false;

                //跳过忽略文本
                flag = f_jumpIngoringText(reader);
                if (!flag) return false;

                flag = peek(reader, out c);
                if(flag && c == end)
                {
                    //是空分隔符
                    return true;
                }

            }

        }

        #endregion

        #region 键值对

        private bool addKeyValue(string key, JsonVariable value, JsonDictionary json)
        {

            if (p_converDictType == ConverDictionaryType.Cover)
            {

                json.p_dict[key] = value;
                return true;
            }

            if (p_converDictType == ConverDictionaryType.Add)
            {
                if (!json.p_dict.ContainsKey(key))
                {
                    json.p_dict.Add(key, value);
                }
                return true;
            }

            if (p_converDictType == ConverDictionaryType.None)
            {
                try
                {
                    json.Add(key, value);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            return false;

        }

        private bool checkIsKey(TextReader reader, out string key)
        {
            char c;
            bool flag;
            key = null;
            
            flag = peek(reader, out c);
            if (!flag) return false;

            if (c != '"') return false;

            StringBuilder sb = new StringBuilder();

            flag = ConverString(reader, sb);
            if (!flag) return false;

            key = sb.ToString();
            return true;
        }

        /// <summary>
        /// 判断并转化键值对
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="json">实例化的空键值对</param>
        /// <returns></returns>
        private bool ConverDict(TextReader reader, JsonDictionary json)
        {
            const char fen = ',';
            const char end = '}';
            const char kf = ':';

            bool flag;
            char c;
            JsonVariable item;
            ref char[] buf = ref p_charArrayBuffer;
            next(reader);

            //跳过忽略文本
            flag = f_jumpIngoringText(reader);
            if (!flag) return false;

            flag = peek(reader, out c);
            if (!flag) return false;

            //空键值对
            if (c == end)
            {
                next(reader);
                return true;
            }

            int nextChar;

            string key;
            while (true)
            {
                //检查并读取key
                flag = checkIsKey(reader, out key);
                if (!flag) return false;

                //跳过忽略文本
                flag = f_jumpIngoringText(reader);
                if (!flag) return false;

                //检查键分隔符
                flag = read(reader, out c);
                if (!flag) return false;
                if (c != kf) return false;
                //if ((!flag) && (c != kf)) return false;

                //跳过忽略文本
                flag = f_jumpIngoringText(reader);
                if (!flag) return false;

                //读取对象
                flag = ConverJsonText(reader, out item, out nextChar);
                if (!flag) return false;

                //添加
                flag = addKeyValue(key, item, json);
                if (!flag) return false;

                if (nextChar == -1)
                {

                    //跳过忽略文本
                    flag = f_jumpIngoringText(reader);
                    if (!flag) return false;

                    //读取分隔符
                    flag = read(reader, out c);
                    if (!flag) return false;

                    //结束
                    if (c == end) return true;
                    if (c != fen) return false;

                    //跳过忽略文本
                    flag = f_jumpIngoringText(reader);
                    if (!flag) return false;

                    flag = peek(reader, out c);
                    //是空分隔符
                    if (flag && c == end) return true;
                }
                else
                {

                    if (nextChar == fen)
                    {
                        //下一位是分隔符

                        //跳过忽略文本
                        flag = f_jumpIngoringText(reader);
                        if (!flag) return false;

                        flag = peek(reader, out c);
                        //是空分隔符
                        if (flag && c == end) return true;

                    }
                    else
                    {
                        //跳过忽略文本
                        flag = f_jumpIngoringText(reader);
                        if (!flag) return false;

                        //读取分隔符
                        flag = read(reader, out c);
                        if (!flag) return false;

                        //结束
                        if (c == end) return true;
                        if (c != fen) return false;

                        //跳过忽略文本
                        flag = f_jumpIngoringText(reader);
                        if (!flag) return false;

                        flag = peek(reader, out c);
                        //是空分隔符
                        if (flag && c == end) return true;
                    }

                }

            }

        }

        #endregion

        #endregion

        /// <summary>
        /// 读取一个json对象文本写入并转化为实例
        /// </summary>
        /// <param name="reader">读取的对象文本，首字符紧挨</param>
        /// <param name="json">实例引用，若没有实际的对象则为一个null引用</param>
        /// <param name="nextChar">区域性兼容参数</param>
        /// <returns>是否成功读取</returns>
        private bool ConverJsonText(TextReader reader, out JsonVariable json, out int nextChar)
        {
            nextChar = -1;
            int ic;
            char c;
            bool flag;
            bool flag2;

            JsonType type;

            json = null;
            flag = f_jumpIngoringText(reader);
            if (!flag)
            {
                return false;
            }

            ic = reader.Peek();
            if (ic == -1)
            {
                json = null;
                return true;
            }
            c = (char)ic;

            flag = IsJsonType(c, out type, out flag2);
            if (!flag)
            {
                return false;
            }

            if (flag2)
            {
                //数值
                flag = f_converNum(reader, out json, out nextChar);
                if (!flag) return false;

                return true;
            }

            if (type == JsonType.String)
            {
                //字符串
                StringBuilder sb = new StringBuilder();
                flag = ConverString(reader, sb);
                if (flag)
                {
                    json = new JsonString(sb.ToString());
                    return true;
                }
                return false;
            }

            if (type == JsonType.Boolean)
            {
                //布尔值

                flag = ConverBoolean(reader, c, out flag2);

                if (flag)
                {
                    json = new JsonBoolean(flag2);
                    return true;
                }
                return false;
            }

            if (type == JsonType.Null)
            {
                //空
                flag = ConverNull(reader);
                if (flag)
                {
                    json = JsonNull.Nullable;
                    return true;
                }
                return false;
            }

            if (type == JsonType.Dictionary)
            {
                //键值对
                json = new JsonDictionary();
                flag = ConverDict(reader, (JsonDictionary)json);
                if (flag)
                {
                    return true;
                }
                return false;
            }

            if (type == JsonType.List)
            {
                //集合
                json = new JsonList();

                flag = ConverList(reader, (JsonList)json);
                if (flag)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        #endregion

        #region 转化到文本

        private void ParsToJson(JsonVariable json, TextWriter writer, int lay)
        {
            JsonType type = json.DataType;

            if (type == JsonType.List)
            {
                ArrayToText((JsonList)json, writer, lay);
            }
            else if (type == JsonType.Dictionary)
            {
                DictToText((JsonDictionary)json, writer, lay);
            }
            else
            {
                EasyToText(json, writer);
            }
        }

        /// <summary>
        /// c#转义判断和转化
        /// </summary>
        /// <param name="c">判断的字符</param>
        /// <param name="esc">转化到json不带\的字符</param>
        /// <returns>是否为正确转义</returns>
        private bool f_EscapeCharToJson(char c, out char esc)
        {
            esc = c;
            // '\a','\b','\f','\n','\r', '\t', '\v', '\\','\'', '\"'

            if (c == '\\')
            {
                esc = '\\';
                return true;
            }

            if (c == '\r')
            {
                esc = 'r';
                return true;
            }
            if (c == '\n')
            {
                esc = 'n';
                return true;
            }

            if (c == '\'' && p_jsonWriterCharacterSingleQuotation)
            {
                esc = '\'';
                return true;
            }
            if (c == '"')
            {
                esc = '"';
                return true;
            }
            if (c == '\t')
            {
                esc = 't';
                return true;
            }
            if (c == '\v')
            {
                esc = 'v';
                return true;
            }

            if (c == '\a')
            {
                esc = 'a';
                return true;
            }

            if (c == '\b')
            {
                esc = 'b';
                return true;
            }
            if (c == '\f')
            {
                esc = 'f';
                return true;
            }

            return false;

        }

        /// <summary>
        /// 根据字符值返回指定16进制位值中的字符
        /// </summary>
        /// <param name="value">字符值</param>
        /// <param name="x10Index">访问位数 范围[0,3]</param>
        /// <returns>指定位数的字符表达值</returns>
        private char f_Bit4CharToByte(char value, int x10Index)
        {
            //const ushort ToLopper = 0b00000000_00000000;
            //const ushort ToUpper = 0b11111111_11011111;

            byte b = (byte)(((value) >> (x10Index * 4)) & 0xF);

            if(b < 10)
            {
                return (char)(b + '0');
            }
            
            return (char)((b - 10) + 'A');

        }

        private void ToStrText(string jsonStr, TextWriter wr, bool textIsUnicode)
        {
            
            char c, oc;

            wr.Write('"');

            //StartV1:
            if (textIsUnicode)
            {
                StringReader r = new StringReader(jsonStr);

                while (read(r, out c))
                {
                    p_charArrayBuffer[3] = f_Bit4CharToByte(c, 0);
                    p_charArrayBuffer[2] = f_Bit4CharToByte(c, 1);
                    p_charArrayBuffer[1] = f_Bit4CharToByte(c, 2);
                    p_charArrayBuffer[0] = f_Bit4CharToByte(c, 3);
                    wr.Write('\\');
                    wr.Write('u');
                    wr.Write(p_charArrayBuffer, 0, 4);
                }
            }
            else
            {
                if (p_toTextEscapeChar)
                {
                    StringReader r = new StringReader(jsonStr);
                    while (read(r, out c))
                    {
                        if (f_EscapeCharToJson(c, out oc))
                        {
                            //是转义
                            wr.Write('\\');
                            wr.Write(oc);
                            //continue;
                        }
                        else
                        {
                            //直接写入字符
                            wr.Write(c);
                        }

                    }

                }
                else
                {
                    wr.Write(jsonStr);
                }

            }

            //End:

            wr.Write('"');

        }

        #region 简单值写入

        private void f_writeInt(TextWriter wr, long value)
        {
            var nf = p_cultureInfo?.NumberFormat;
            if (nf is null)
            {
                wr.Write(value);
            }
            else
            {
                wr.Write(value.ToString(nf));
            }
        }

        private void f_writeDouble(TextWriter wr, double value)
        {
            var nf = p_cultureInfo?.NumberFormat;
            if (nf is null)
            {
                wr.Write(value);
            }
            else
            {
                wr.Write(value.ToString(nf));
            }
        }

        private void f_writeBool(TextWriter wr, bool value)
        {
            var nf = p_cultureInfo?.NumberFormat;
            if (nf is null)
            {
                wr.Write(value);
            }
            else
            {
                wr.Write(value.ToString(nf));
            }
        }

        private void f_writeNull(TextWriter wr)
        {
            wr.Write("null");
        }

        #endregion

        private void EasyToText(JsonVariable json, TextWriter wr)
        {
            JsonType type = json.DataType;
            switch (type)
            {
                case JsonType.Integer:
                    f_writeInt(wr, json.Integer);
                    break;
                case JsonType.RealNum:
                    f_writeDouble(wr, json.RealNum);
                    break;
                case JsonType.Boolean:
                    f_writeBool(wr, json.Boolean);
                    break;
                case JsonType.String:
                    ToStrText(json.String, wr, p_jsonToTextIsUnicode);
                    break;
                case JsonType.Null:
                    f_writeNull(wr);
                    break;                
            }
        }

        private void ArrayToText(JsonList json, TextWriter wr, int lay)
        {
            const char startArray = '[';
            const char endArray = ']';
            const char separator = ',';
            const char tab = '\t';

            int length = json.Count;
            JsonVariable temp;
            int end = length - 1;
            JsonType ty;
            bool newl = default;
            int l;

            if (json.Count == 0)
            {
                wr.Write("[]");
                return;
            }

            //写入集合头
            wr.Write(startArray);

            for (int i = 0; i < length; i++)
            {
                temp = json[i];

                ty = temp.DataType;

                if (p_toJsonTextNewLine)
                {
                    //判断是否换行对象
                    newl = ty == JsonType.List;

                    if (newl)
                    {
                        wr.WriteLine();

                        for (l = 0; l < lay; l++)
                        {
                            wr.Write(tab);
                        }
                    }
                }             

                //写入一个
                ParsToJson(temp, wr, lay + 1);

                //写入分隔符
                if (i != end)
                {
                    wr.Write(separator);
                }


                if (p_toJsonTextNewLine)
                {
                    if (newl)
                    {
                        wr.WriteLine();

                        for (l = 0; l < lay; l++)
                        {
                            wr.Write(tab);
                        }
                    }
                }

                   


            }

            //写入集合尾
            wr.Write(endArray);
        }

        private void DictToText(JsonDictionary json, TextWriter wr, int lay)
        {
            #region 变量
            const char start = '{';
            const char end = '}';
            const char separator = ',';
            const char tab = '\t';
            const char keySepar = ':';

            int i;
            int count = 0;
            int length = json.Count - 1;
            int laymove = lay + 1;
            JsonType ty;
            KeyValuePair<string, JsonVariable> item;
            #endregion

            bool move;

            if (json.Count == 0)
            {
                wr.Write("{}");
                return;
            }

            var enr = json.GetEnumerator();

            wr.Write(start);

            #region 循环

            Start:
            move = enr.MoveNext();

            if (!move) goto End;

            item = enr.Current;

            ty = item.Value.DataType;

            if (p_toJsonTextNewLine)
            {
                wr.WriteLine();
                for (i = 0; i < laymove; i++)
                {
                    wr.Write(tab);
                }
            }

            #region 键值对写入一次
            //键

            ToStrText(item.Key, wr, p_jsonToKeyIsUnicode);
            //wr.Write('"');
            //wr.Write(item.Key);
            //wr.Write('"');

            //键值分割
            wr.Write(keySepar);
            //值
            ParsToJson(item.Value, wr, ty == JsonType.Dictionary ? lay + 1 : lay);
            #endregion

            //分隔符
            if (count != length)
            {
                wr.Write(separator);
            }


            count++;
            goto Start;

            #endregion

            End:

            if (p_toJsonTextNewLine)
            {
                wr.WriteLine();
                for (i = 0; i < lay; i++)
                {
                    wr.Write(tab);
                }
            }           

            wr.Write(end);


        }

        #endregion

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个默认实现的json解析器
        /// </summary>
        public JsonParserDefault()
        {
            f_init();
        }

        private void f_init()
        {
            p_converDictType = ConverDictionaryType.Cover;
            p_JsonNotConverNumToString = false;
            p_charArrayBuffer = new char[4];
            p_newLine = Environment.NewLine;
            p_toJsonTextNewLine = false;
            p_toObjEscapeChar = true;
            p_toTextEscapeChar = true;
            p_jsonToTextIsUnicode = false;
            p_jsonToKeyIsUnicode = false;
            p_numStyles = TextParserNumStylesDefault;
            p_jsonEscapeCharacterSingleQuotation = true;
            p_jsonWriterCharacterSingleQuotation = false;
            try
            {
                p_cultureInfo = CultureInfo.InvariantCulture;
            }
            catch (Exception)
            {
                p_cultureInfo = null;
            }

        }
        #endregion

        #region 参数访问

        /// <summary>
        /// 参数<see cref="ToJsonIdentifyingEscapeCharacters"/>的默认值
        /// </summary>
        public const NumberStyles TextParserNumStylesDefault = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent;

        /// <summary>
        /// 将文本转化为整形或浮点型时需要能够转化的数值样式
        /// </summary>
        /// <value>
        /// <para>若参数<see cref="JsonParserDefault.ParserCultureInfo"/>是null，则该参数无效</para>
        /// <para>默认为<see cref="TextParserNumStylesDefault"/></para>
        /// </value>
        public NumberStyles TextParserNumStyles
        {
            get => p_numStyles;
            set
            {
                p_numStyles = value;
            }
        }

        /// <summary>
        /// 解析文本到对象时是否识别转义字符
        /// </summary>
        /// <remarks>
        /// 支持的转义字符识别：<![CDATA[\r \n \t \a \b \\ \' \" \0 \v \uFFFF]]>
        /// </remarks>
        /// <value>
        /// 设为false则在解析文本时忽略使用'\'字符前缀的转义字符；true则在解析文本时将拥有'\'前缀的字符组合转化为相应的转义字符。参数默认为true
        /// </value>
        public bool ToJsonIdentifyingEscapeCharacters
        {
            get => p_toObjEscapeChar;
            set
            {
                p_toObjEscapeChar = value;
            }
        }

        /// <summary>
        /// 对象转化到文本时是否转化转义字符
        /// </summary>
        /// <value>
        /// 设为true在将字符串对象转化到文本时，将字符串中的转义字符以json文本的方式转录；false则忽略转义字符将所有字符串直接写入json文本。默认为true
        /// </value>
        public bool ToTextIdentifyingEscapeCharacters
        {
            get => p_toTextEscapeChar;
            set
            {
                p_toTextEscapeChar = value;
            }
        }

        /// <summary>
        /// 从对象转化到文本时是否将字符串转化为\u通用编码
        /// </summary>
        /// <remarks>
        /// 从对象转化到文本时，所有的字符串对象都将转化为 \uXXXX 格式的文本；默认为false
        /// <para>该属性优先级高于<see cref="ToTextIdentifyingEscapeCharacters"/></para>
        /// </remarks>
        public bool ToTextCharIsUnicodeEscape
        {
            get => p_jsonToTextIsUnicode;
            set => p_jsonToTextIsUnicode = value;
        }

        /// <summary>
        /// 从对象转化到Key时是否将字符串转化为\u通用编码
        /// </summary>
        /// <remarks>
        /// 从对象转化到<see cref="JsonType.Dictionary"/>类型的key参数时，key字符串都将转化为 \uXXXX 格式的文本；默认为false
        /// <para>该属性优先级高于<see cref="ToTextIdentifyingEscapeCharacters"/></para>
        /// </remarks>
        public bool ToTextKeyCharIsUnicodeEscape
        {
            get => p_jsonToKeyIsUnicode;
            set => p_jsonToKeyIsUnicode = value;
        }

        /// <summary>
        /// 是否将无法转化为数值的文本转化为字符串
        /// </summary>
        /// <value>
        /// 当此值设为true时，若出现因为超出数值范围而无法正确转化为整数或小数的json文本时，会将数值文本转化为文本对象；
        /// 值为false时若无法转化为数值，则正常引发错误；默认值为false
        /// </value>
        public bool NotConverNumToString
        {
            get => p_JsonNotConverNumToString;
            set
            {
                p_JsonNotConverNumToString = value;
            }
        }

        /// <summary>
        /// 是否将单引号 ( ' ) 字符归类为转义字符检测
        /// </summary>
        /// <value>
        /// <para>
        /// 当参数为true时，在进行转义字符检测时会将单引号加入检测列表；当遇到字符串 "it \'s" 时，会将 \' 识别为 '
        /// </para>
        /// <para>若参数为false，不会将单引号加入转义字符检测列表</para>
        /// <para>该参数默认为true</para>
        /// </value>
        public bool EscapeCharacterSingleQuotation
        {
            get => p_jsonEscapeCharacterSingleQuotation;
            set
            {
                p_jsonEscapeCharacterSingleQuotation = value;
            }
        }

        /// <summary>
        /// 是否将单引号 ( ' ) 字符归类为转义字符转化
        /// </summary>
        /// <value>
        /// <para>当参数为true时，在将json对象实例写入到文本时，会将字符串内的单引号用 \' 的形式写入字符串；false则不会加转义字符 \ </para>
        /// <para>该参数默认为false</para>
        /// </value>
        public bool WriterCharacterSingleQuotation
        {
            get => p_jsonWriterCharacterSingleQuotation;
            set
            {
                p_jsonWriterCharacterSingleQuotation = value;
            }
        }

        /// <summary>
        /// 键值对转化方式
        /// </summary>
        /// <remarks>
        /// 在将json文本中的键值对文本转化为对象时，该参数决定了转化对象的方法；默认值为<see cref="ConverDictionaryType.Cover"/>
        /// </remarks>
        /// <exception cref="ArgumentException">不是预定的枚举值</exception>
        public ConverDictionaryType ConverDictType
        {
            get => p_converDictType;
            set
            {
                if (value < ConverDictionaryType.None || value > ConverDictionaryType.Add) throw new ArgumentException();
                p_converDictType = value;
            }
        }

        /// <summary>
        /// 将json对象转化到文本时是否使用换行格式
        /// </summary>
        /// <value>值设为true则转化的文本使用可读性较高的换行模式，false则转化的文本集中在一行不会有多余的空隙；默认为false</value>
        public bool ToJsonTextNewLine
        {
            get => p_toJsonTextNewLine;
            set
            {
                p_toJsonTextNewLine = value;
            }
        }

        /// <summary>
        /// 访问或设置该解析器在进行文本转化时的区域信息
        /// </summary>
        /// <value>
        /// <para>该对象用于本地化处理值和文本的相互转化</para>
        /// <para>
        /// 若该值设置为null，则不使用区域信息；将对象转化为文本时，跟随文本写入器的<see cref="TextWriter.FormatProvider"/>
        /// </para>
        /// <para>初始化时默认为<see cref="CultureInfo.InvariantCulture"/></para>
        /// </value>
        public CultureInfo ParserCultureInfo
        {
            get => p_cultureInfo;
            set
            {                
                p_cultureInfo = value;
            }
        }

        #endregion

        #region 功能

        /// <summary>
        /// 判断将json文本读取器是否拥有有效json格式，且将读取器推进到第一位有效字符的位置
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="reader">要读取的Json文本读取器</param>
        /// <param name="notError">推进读取器时是否没有json语法错误，true表示没有错误，false表示有语法错误</param>
        /// <returns>
        /// 当推进完成后若出现有效的json文本字符，返回true；若没有出现有效的json字符，返回false
        /// <para>当返回值是false时，若参数<paramref name="notError"/>为true，则表示该读取器内没有可用的json文本并且读取器已经到达结尾；参数<paramref name="notError"/>为false时，则遇到了错误的json文本格式</para>
        /// </returns>
        public bool JumpIngoringText(TextReader reader, out bool notError)
        {
            notError = f_jumpIngoringText(reader);
            return reader.Peek() != -1;
        }

        public override void ParsingJson(JsonVariable json, TextWriter writer)
        {
            if (json is null || writer is null) throw new ArgumentNullException();

            ParsToJson(json, writer, 0);
        }
    
        public override JsonVariable ToJsonData(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            JsonVariable json;
            if (ConverJsonText(reader, out json, out _))
            {
                return json;
            }
            throw new NotImplementedException(Cheng.Properties.Resources.Exception_NotParserJsonText);
        }

        public override bool ToJsonData(TextReader reader, out JsonVariable json)
        {
            if (reader is null) throw new ArgumentNullException();
            return ConverJsonText(reader, out json, out _);
        }

        #endregion

    }

}
