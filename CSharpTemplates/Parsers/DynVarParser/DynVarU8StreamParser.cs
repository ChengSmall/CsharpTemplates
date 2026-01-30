using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cheng.Memorys;
using Cheng.Streams;
using Cheng.IO;
using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.DataStructure.DynamicVariables;
using Cheng.Texts;

namespace Cheng.Streams.Parsers.DynVariableParser
{

    /// <summary>
    /// 动态对象Utf8格式二进制序列化转换器
    /// </summary>
    public unsafe sealed class DynVarU8StreamParser : StreamParser
    {

        #region 构造

        public DynVarU8StreamParser() : this(1024 * 4)
        {}

        public DynVarU8StreamParser(int bufferSize)
        {
            p_utf8 = new UTF8Encoding(true);
            p_utf8Enr = p_utf8.GetEncoder();
            p_buffer = new byte[bufferSize];
            p_readOnLock = false;
        }

        #endregion

        #region 参数

        private Encoding p_utf8;

        private Encoder p_utf8Enr;

        private byte[] p_buffer;

        private bool p_readOnLock;

        #endregion

        #region 功能

        #region 封装

        #region 写入

        #region 简单值

        private static void f_setInt32(Stream stream, int value)
        {
            uint re = (uint)value;
            for (int i = 0; i < 4; i++)
            {
                byte wr = (byte)((re >> (8 * i)) & 0b1111_1111);
                stream.WriteByte(wr);
            }
        }

        private static void f_setInt64(Stream stream, long value)
        {
            ulong re = (ulong)value;

            for (int i = 0; i < 8; i++)
            {
                byte wr = (byte)((re >> (8 * i)) & 0b1111_1111);
                stream.WriteByte(wr);
            }
        }

        private static void f_setFloat(Stream stream, float value)
        {
            f_setInt32(stream, *(int*)&value);
        }

        private static void f_setDouble(Stream stream, double value)
        {
            f_setInt64(stream, *(long*)&value);
        }

#if DEBUG
        /// <summary>
        /// 组合布尔值 + 布尔类型 + 类型字节
        /// </summary>
        /// <param name="value">组合的布尔值</param>
        /// <param name="type">要合并的类型字节</param>
        /// <returns></returns>
#endif
        private static byte f_getBoolValue(bool value, byte type)
        {
            byte t = (byte)DynVariableType.Boolean;
            if(value) t |= 0b0001_0000;
            return (byte)(t | type);
        }

        private static void f_setStrValue(Stream stream, string value, Encoder enr, byte[] buffer)
        {
            enr.Reset();
            fixed (char* strp = value)
            {
                fixed (byte* bufptr = buffer)
                {
                    int strI = 0;
                    //剩余字符数
                    int lastCharCount = value.Length;

                    //此次转换的字符数
                    int onCharCount;

                    //此次转换成功的字节数
                    int onByteSize;

                    //成功转换所有字符
                    bool toAllChar;

                    while (lastCharCount > 0)
                    {
                        //转换
                        enr.Convert(strp + strI, lastCharCount, bufptr, buffer.Length, true, out onCharCount, out onByteSize, out toAllChar);

                        //写入转换的字节
                        stream.Write(buffer, 0, onByteSize);

                        lastCharCount -= onCharCount;
                        strI += onCharCount;

                    }
                }
                
            }
        }

#if DEBUG
        /// <summary>
        /// 写入u8字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <param name="enc"></param>
        /// <param name="enr"></param>
        /// <param name="buffer"></param>
#endif
        private static void f_setStr(Stream stream, string value, Encoding enc, Encoder enr, byte[] buffer)
        {
            //写入长度
            f_setInt32(stream, enc.GetByteCount(value));
            f_setStrValue(stream, value, enr, buffer);
        }

        static string fs_throwCharOutRangeStr()
        {
            return Cheng.Properties.Resources.Exception_Char16NotConvertBytes;
        }

        private static void f_setStrKey(Stream stream, string value, Encoding enc, Encoder enr, byte[] buffer)
        {
            const int u8strMaxLen = 1048575;

            var bsize = enc.GetByteCount(value);
            if(bsize > u8strMaxLen)
            {
                throw new NotImplementedException(fs_throwCharOutRangeStr());
            }
            //写入3字节长度
            uint bi = (uint)bsize;
            stream.WriteByte((byte)((bi) & 0xFF));
            stream.WriteByte((byte)((bi >> 8) & 0xFF));
            stream.WriteByte((byte)((bi >> 16) & 0b1111));

            f_setStrValue(stream, value, enr, buffer);
        }

        #endregion

        #region 集合

        private void f_writeList(Stream stream, DynList dlist)
        {
            foreach (var item in dlist)
            {
                //组合类型字节
                byte type = (byte)(((byte)(item.DynType)) | 0b1000_0000);
                //写入对象
                f_writerObj(stream, item, type);
            }
            stream.WriteByte(0);
        }

        #endregion

        #region 字典

        private void f_writeDict(Stream stream, DynDictionary dict)
        {
            foreach (var pair in dict)
            {
                //写入标识符
                stream.WriteByte(1);
                //写入key
                f_setStrKey(stream, pair.Key, p_utf8, p_utf8Enr, p_buffer);
                //写入对象
                f_writerToTypeAndObj(stream, pair.Value);
            }
            stream.WriteByte(0);
        }

        #endregion

        private void f_writerObj(Stream stream, DynVariable value, byte type)
        {
            byte btype = (byte)(type & 0b1111);

            if((type & 0b1000) == 0)
            {
                //基本类型
                if (btype < 4)
                {
                    stream.WriteByte(type);
                    if (btype < 1)
                    {
                        //空值
                        return;
                    }
                    else if(btype == 1)
                    {
                        //int32
                        f_setInt32(stream, value.Int32Value);
                    }
                    else
                    {
                        //int64
                        f_setInt64(stream, value.Int64Value);
                    }
                    return;
                }
                else if(btype == 4)
                {
                    //布尔值
                    byte booltype = type;
                    if (value.BooleanValue)
                    {
                        booltype |= 0b0001_0000;
                    }
                    else
                    {
                        booltype &= 0b1110_1111;
                    }
                    stream.WriteByte(booltype);
                    return;
                }
                else
                {
                    stream.WriteByte(type);
                    if (btype < 6)
                    {
                        //float
                        f_setFloat(stream, value.FloatValue);
                    }
                    else if(btype == 6)
                    {
                        //double
                        f_setDouble(stream, value.DoubleValue);
                    }
                    else
                    {
                        //string
                        f_setStr(stream, value.StringValue, p_utf8, p_utf8Enr, p_buffer);
                    }
                    return;
                }
            }
            else
            {
                stream.WriteByte(type);
                //复合类型
                if (type == 0b1001)
                {
                    //集合
                    f_writeList(stream, value.DynamicList);
                }
                else
                {
                    //字典
                    f_writeDict(stream, value.DynamicDictionary);
                }
                return;
            }
        }

#if DEBUG
        /// <summary>
        /// 写入一个对象头添类型
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
#endif
        private void f_writerToTypeAndObj(Stream stream, DynVariable value)
        {
            f_writerObj(stream, value, (byte)value.DynType);
        }

        #endregion

        #region 读取

        #region 简单值

        private static int f_getInt32(Stream stream)
        {
            uint value = 0;

            for (int i = 0; i < 4; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((uint)re) << ((8 * i));
            }

            return ((int)value);
        }

        private static long f_getInt64(Stream stream)
        {
            ulong value = 0;

            for (int i = 0; i < 8; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((ulong)re) << ((8 * i));
            }

            return ((long)value);
        }

        private static float f_getFloat(Stream stream)
        {
            uint value = 0;

            for (int i = 0; i < 4; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((uint)re) << ((8 * i));
            }

            return (*(float*)&value);
        }

        private static double f_getDouble(Stream stream)
        {
            ulong value = 0;

            for (int i = 0; i < 8; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((ulong)re) << ((8 * i));
            }

            return (*(long*)&value);
        }

        private static DynVariable f_readBool(byte type)
        {
            return DynVariable.GetBooleanValue((type & 0b1_0000) == 0b1_0000);
        }

        private string f_readOnlyStr(Stream stream, uint len)
        {
            var pos = stream.Position;
            var bfs = new NotBufferTruncateStream(stream, pos, len, false);
            string str;
            using (StreamReader sreader = new StreamReader(bfs, p_utf8, false, 1024))
            {
                str = sreader.ReadToEnd();
            }
            return str;
        }

        private string f_readStr(Stream stream)
        {
            //读取长度
            uint len = (uint)f_getInt32(stream);
            len = (len) & (~(1U << 31));
            return f_readOnlyStr(stream, len);
        }

        private string f_readStrKey(Stream stream)
        {
            uint len;
            var re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            len = (uint)re;

            re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            len |= ((uint)re << 8);

            re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            len |= ((uint)(re & 0b1111) << 16);

            return f_readOnlyStr(stream, len);
        }

        #endregion

        #region 集合

        private DynList f_readList(Stream stream)
        {
            int re;
            DynList list = new DynList();
            Loop:
            //读取类型字节
            re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            //终止符
            if ((re & 0b1000_0000) == 0)
            {
                if (p_readOnLock)
                {
                    list.OnLock();
                }
                return list; //末尾
            }

            //获取值并添加
            list.Add(f_getOnceVar(stream, (byte)(re & 0b0111_1111)));
            goto Loop;

        }

        #endregion

        #region 字典

        private DynDictionary f_readDict(Stream stream)
        {
            int re;
            DynDictionary dict = new DynDictionary();

            Loop:
            re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();

            if(re == 0)
            {
                if (p_readOnLock)
                {
                    dict.OnLock();
                }
                return dict;
            }

            var key = f_readStrKey(stream);
            var value = f_readObj(stream);

            dict[key] = value;
            goto Loop;
        }

        #endregion

        private DynVariable f_getOnceVar(Stream stream, byte type)
        {
            var tt = (byte)(type & 0b1111);

            if ((tt & 0b1000) == 0)
            {
                //基本类型
                if (tt < 4)
                {
                    if (tt < 1)
                    {
                        //空值
                        return DynVariable.EmptyValue;
                    }
                    else if (tt == 1)
                    {
                        //int32
                        return DynVariable.CreateInt32(f_getInt32(stream));
                    }
                    else
                    {
                        //int64
                        return DynVariable.CreateInt64(f_getInt64(stream));
                    }
                   
                }
                else if (tt == 4)
                {
                    //布尔值
                    return f_readBool(tt);
                }
                else
                {
                    if (tt < 6)
                    {
                        //float
                        return DynVariable.CreateFloat(f_getFloat(stream));
                    }
                    else if (tt == 6)
                    {
                        //double
                        return DynVariable.CreateDouble(f_getDouble(stream));
                    }
                    else
                    {
                        //string
                        return DynVariable.CreateString(f_readStr(stream));
                    }
                }
            }
            else
            {
                //复合类型
                if (tt == 0b1001)
                {
                    //集合
                    return f_readList(stream);
                }
                else
                {
                    //字典
                    return f_readDict(stream);
                }
            }

        }

        private DynVariable f_readObj(Stream stream)
        {
            var re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            return f_getOnceVar(stream, (byte)re);
        }

        #endregion

        #endregion

        /// <summary>
        /// 从流数据读取对象时，是否锁定读取完毕的动态对象
        /// </summary>
        /// <value>默认为false</value>
        public bool ConverToObjOnLock
        {
            get => p_readOnLock;
            set => p_readOnLock = value;
        }

        /// <summary>
        /// 从流对象中读取并反序列化到对象
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <returns>反序列化后的对象</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>是null</exception>
        /// <exception cref="NotSupportedException">流对象没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">流对象已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotImplementedException">无法读取数据，格式错误</exception>
        public DynVariable ConverToValue(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new NotSupportedException();
            return f_readObj(stream);
        }

        /// <summary>
        /// 将对象序列化到流数据
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="stream">流数据</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>是null</exception>
        /// <exception cref="NotSupportedException">流对象没有写入权限</exception>
        /// <exception cref="ObjectDisposedException">流对象已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">写入流时发生的其它错误</exception>
        public void ConverToStream(DynVariable value, Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!stream.CanWrite) throw new NotSupportedException();
            f_writerToTypeAndObj(stream, value);
        }

        #endregion

        #region 派生

        public override object ConverToObject(Stream stream)
        {
            return ConverToValue(stream);
        }

        public override void ConverToStream(object obj, Stream stream)
        {
            ConverToStream(obj as DynVariable, stream);
        }

        #endregion

    }

}
#if DEBUG
#endif