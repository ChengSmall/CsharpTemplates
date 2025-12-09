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
    /// 动态对象二进制序列化转换器
    /// </summary>
    public unsafe sealed class DynVarStreamParser : StreamParser
    {

        #region 初始化

        /// <summary>
        /// 实例化动态对象转换器
        /// </summary>
        public DynVarStreamParser()
        {
            p_strBuf = new StringBuilder(32);
            p_dynObjOnLock = false;
        }

        #endregion

        #region 参数

        private StringBuilder p_strBuf;

#if DEBUG
        /// <summary>
        /// 创建可变对象后是否锁定
        /// </summary>
#endif
        private bool p_dynObjOnLock;

        #endregion

        #region 实现

        #region 封装

        #region 读取

        #region 静态类型

        private static DynVariable f_getInt32(Stream stream)
        {
            uint value = 0;
           
            for (int i = 0; i < 4; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((uint)re) << ((8 * i));
            }

            return DynVariable.CreateInt32((int)value);
        }

        private static DynVariable f_getInt64(Stream stream)
        {
            ulong value = 0;

            for (int i = 0; i < 8; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((ulong)re) << ((8 * i));
            }

            return DynVariable.CreateInt64((long)value);
        }

        private static DynVariable f_getFloat(Stream stream)
        {
            uint value = 0;

            for (int i = 0; i < 4; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((uint)re) << ((8 * i));
            }

            return DynVariable.CreateFloat(*(float*)&value);
        }

        private static DynVariable f_getDouble(Stream stream)
        {
            ulong value = 0;

            for (int i = 0; i < 8; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                value |= ((ulong)re) << ((8 * i));
            }

            return DynVariable.CreateDouble(*(long*)&value);
        }

#if DEBUG
        /// <summary>
        /// 读取对象字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
#endif
        private DynVariable f_getStr(Stream stream)
        {
            uint len = 0;
            int re;
            int i;
            for (i = 0; i < 4; i++)
            {
                re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                len |= ((uint)re) << ((8 * i));
            }

            p_strBuf.Clear();

            for (i = 0; i < (int)len; i++)
            {
                ushort uc;
                re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                uc = (byte)re;

                re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                uc |= (ushort)((re) << 8);

                p_strBuf.Append((char)uc);
            }

            return DynVariable.CreateString(p_strBuf.ToString());

        }

#if DEBUG
        /// <summary>
        /// 读取key字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
#endif
        private string f_getStrKey(Stream stream)
        {
            ushort len = 0;
            int re;
            int i;
            for (i = 0; i < 2; i++)
            {
                re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                len |= (ushort)(((ushort)re) << (8 * i));
            }

            p_strBuf.Clear();

            for (i = 0; i < len; i++)
            {
                ushort uc;
                re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                uc = (byte)re;

                re = stream.ReadByte();
                if (re == -1) throw new NotImplementedException();
                uc |= (ushort)((re) << 8);

                p_strBuf.Append((char)uc);
            }

            return p_strBuf.ToString();

        }

        #endregion

        private DynList f_getList(Stream stream)
        {
            int re;

            DynList list = new DynList();

            //读取头字节

            Loop:
            re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();

            if ((re & 0b1000_0000) == 0)
            {
                if (p_dynObjOnLock)
                {
                    list.OnLock();
                }
                return list; //末尾
            }

            //类型
            byte first = (byte)(re & 0b0111_1111);
            //获取并添加
            list.Add(f_getOnceVar(stream, first));
            goto Loop;

        }

        private DynDictionary f_getDict(Stream stream)
        {
            int re;

            DynDictionary dict = new DynDictionary();

            Loop:
            //读取头字节
            re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();

            if ((re & 0b1000_0000) == 0)
            {
                //末尾
                if (p_dynObjOnLock)
                {
                    dict.OnLock();
                }
                return dict;
            }

            //读取key
            var key = f_getStrKey(stream);
            //读取值
            var value = f_getOnceVar(stream, (byte)re);

            dict[key] = value;
            goto Loop;

        }

        private DynVariable f_getOnceVar(Stream stream, byte firstType)
        {

            DynVariableType dt = (DynVariableType)(firstType & 0b0000_1111);

            if((firstType & 0b1000) == 0b1000)
            {
                //动态对象
                if(dt == DynVariableType.List)
                {
                    return f_getList(stream);
                }
                else if(dt == DynVariableType.Dictionary)
                {
                    return f_getDict(stream);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                //静态类型

                switch (dt)
                {
                    case DynVariableType.Empty:
                        return DynVariable.EmptyValue;
                    case DynVariableType.Boolean:
                        if((firstType & 0b10000) == 0b10000)
                        {
                            return DynVariable.BooleanTrue;
                        }
                        else
                        {
                            return DynVariable.BooleanFalse;
                        }
                    case DynVariableType.Int32:
                        return f_getInt32(stream);
                    case DynVariableType.Int64:
                        return f_getInt64(stream);
                    case DynVariableType.Float:
                        return f_getFloat(stream);
                    case DynVariableType.Double:
                        return f_getDouble(stream);
                    case DynVariableType.String:
                        return f_getStr(stream);
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        private DynVariable f_getVar(Stream stream)
        {
            //读取类型
            var re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            return f_getOnceVar(stream, (byte)re);
        }

        #endregion

        #region 写入

        #region 简单类型

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

        private static void f_setStr(Stream stream, string value)
        {
            int len = value.Length;
            //写入长度
            f_setInt32(stream, len);

            for (int i = 0; i < len; i++)
            {
                var c = value[i];
                stream.WriteByte((byte)(c & 0b11111111));
                stream.WriteByte((byte)((c >> 8) & 0b11111111));
            }
        }

        #endregion

        #region list

        private void f_setList(Stream stream, DynList list)
        {

            var length = list.Count;

            for (int i = 0; i < length; i++)
            {
                byte first;
                var dn = list[i];
                first = (byte)((byte)dn.DynType | 0b1000_0000);

                f_setVar(dn, stream, first);
            }
            //终止符
            stream.WriteByte(0);

        }

        #endregion

        #region dict

        private static void f_setStrKey(Stream stream, string value)
        {
            ushort len = (ushort)value.Length;
            //写入长度
            stream.WriteByte((byte)((len) & 0b11111111));
            stream.WriteByte((byte)((len >> 8) & 0b11111111));

            for (int i = 0; i < len; i++)
            {
                var c = value[i];
                stream.WriteByte((byte)(c & 0b11111111));
                stream.WriteByte((byte)((c >> 8) & 0b11111111));
            }
        }

        private void f_setDict(Stream stream, DynDictionary dict)
        {
            foreach (var pair in dict)
            {
                byte first = 0b1000_0000;
                //类型
                var dt = pair.Value.DynType;
                //与表头数结合
                first |= (byte)dt;
                //写入头
                stream.WriteByte(first);
                //写入键
                f_setStrKey(stream, pair.Key);
                //仅写入值
                f_setVerOnly(stream, pair.Value, (byte)dt);
            }
            //终止符
            stream.WriteByte(0);
        }

        #endregion

        private void f_setVerOnly(Stream stream, DynVariable value, byte first)
        {
            DynVariableType type = (DynVariableType)(first & 0b0111_1111);

            if (((byte)type & 0b1000) == 0b1000)
            {
                //可变类型
                //stream.WriteByte((byte)first);
                if (type == DynVariableType.List)
                {
                    f_setList(stream, value.DynamicList);
                }
                else if (type == DynVariableType.Dictionary)
                {
                    f_setDict(stream, value.DynamicDictionary);
                }
            }
            else
            {
                //静态类型
                switch (type)
                {
                    case DynVariableType.Int32:
                        f_setInt32(stream, value.Int32Value);
                        return;
                    case DynVariableType.Int64:
                        f_setInt64(stream, value.Int64Value);
                        return;
                    case DynVariableType.Float:
                        f_setFloat(stream, value.FloatValue);
                        return;
                    case DynVariableType.Double:
                        f_setDouble(stream, value.DoubleValue);
                        return;
                    case DynVariableType.String:
                        f_setStr(stream, value.StringValue);
                        return;
                }


            }
        }

        private void f_setVar(DynVariable value, Stream stream, byte first)
        {
            DynVariableType type = (DynVariableType)(first & 0b0111_1111);

            if (((byte)type & 0b1000) == 0b1000)
            {
                //可变类型
                stream.WriteByte((byte)first);
                if (type == DynVariableType.List)
                {
                    f_setList(stream, value.DynamicList);
                }
                else if (type == DynVariableType.Dictionary)
                {
                    f_setDict(stream, value.DynamicDictionary);
                }
            }
            else
            {
                //静态类型
                
                switch (type)
                {
                    case DynVariableType.Empty:
                        stream.WriteByte((byte)first);
                        return;
                    case DynVariableType.Boolean:
                        byte ff = (byte)first;
                        if (value.BooleanValue)
                        {
                            ff |= 0b0001_0000;
                        }
                        stream.WriteByte(ff);
                        return;
                    case DynVariableType.Int32:
                        stream.WriteByte((byte)first);
                        f_setInt32(stream, value.Int32Value);
                        return;
                    case DynVariableType.Int64:
                        stream.WriteByte((byte)first);
                        f_setInt64(stream, value.Int64Value);
                        return;
                    case DynVariableType.Float:
                        stream.WriteByte((byte)first);
                        f_setFloat(stream, value.FloatValue);
                        return;
                    case DynVariableType.Double:
                        stream.WriteByte((byte)first);
                        f_setDouble(stream, value.DoubleValue);
                        return;
                    case DynVariableType.String:
                        stream.WriteByte((byte)first);
                        f_setStr(stream, value.StringValue);
                        return;
                }


            }

        }

        #endregion

        #endregion

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

            var re = stream.ReadByte();
            if (re == -1) throw new NotImplementedException();
            return f_getOnceVar(stream, (byte)re);
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

            f_setVar(value ?? DynVariable.EmptyValue, stream, (byte)value.DynType);
        }

        #endregion

        #region 派生

        public override T ConverToObject<T>(Stream stream)
        {
            return (T)(object)ConverToValue(stream);
        }

        public override object ConverToObject(Stream stream)
        {
            return ConverToValue(stream);
        }

        public override void ConverToStream(object obj, Stream stream)
        {
            ConverToStream((DynVariable)obj, stream);
        }

        #endregion

        #region 参数

        /// <summary>
        /// 一个值是true的布尔类型对象
        /// </summary>
        public DynVariable BooleanTrue => DynVariable.BooleanTrue;

        /// <summary>
        /// 一个值是false的布尔类型对象
        /// </summary>
        public DynVariable BooleanFalse => DynVariable.BooleanFalse;

        /// <summary>
        /// 表示空对象
        /// </summary>
        public DynVariable EmptyValue => DynVariable.EmptyValue;

        /// <summary>
        /// 反序列化可变类型时将其锁定
        /// </summary>
        /// <value>
        /// <para>参数为true时，在反序列化创建集合或键值对后，调用<see cref="DynamicObject.OnLock"/>进行锁定；false则不进行锁定</para>
        /// <para>该参数默认为false</para>
        /// </value>
        public bool ConverDynObjectOnLock
        {
            get => p_dynObjOnLock;
            set
            {
                p_dynObjOnLock = value;
            }
        }

        #endregion

    }

}
#if DEBUG
#endif