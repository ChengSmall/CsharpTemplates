using Cheng.Memorys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Cheng.Streams.Parsers.Default
{

    /// <summary>
    /// 默认实现的数据解析器
    /// </summary>
    /// <remarks>
    /// 一个<see cref="StreamParser"/>的派生类实现，能够解析所有的基元类型和常用类型转化成流数据，也可以自定义类型解析
    /// </remarks>
    public unsafe class StreamParserDefault : StreamParser
    {

        #region 构造

        /// <summary>
        /// 实例化一个默认实现的数据解析器
        /// </summary>
        public StreamParserDefault()
        {
            p_diyPar = new Dictionary<Guid, ICustomParser>();
            p_buffer = new byte[16];
        }

        #endregion

        #region 参数
        const byte have = 255;
        const byte over = 0;

        private Dictionary<Guid, ICustomParser> p_diyPar;
        private byte[] p_buffer;

        #endregion

        #region 派生

        public override object ConverToObject(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException("stream");

            return f_readObject(stream);
        }

        public override void ConverToStream(object obj, Stream stream)
        {
            if (stream is null) throw new ArgumentNullException("stream");

            f_writeObj(obj, stream);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 为指定类型设置一个自定义解析器
        /// </summary>
        /// <param name="type">指定要解析的对象类型</param>
        /// <param name="parser">指定解析器实现</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void SetParser(ICustomParser parser)
        {
            if (parser is null) throw new ArgumentNullException();
            p_diyPar[parser.GetObjectType().GUID] = parser;
        }

        /// <summary>
        /// 删除指定类型的自定义解析器
        /// </summary>
        /// <param name="parser">要删除的解析器</param>
        /// <returns>是否成功删除</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveParser(ICustomParser parser)
        {
            if (parser is null) throw new ArgumentNullException();
            return p_diyPar.Remove(parser.GetObjectType().GUID);
        }

        /// <summary>
        /// 删除指定类型的自定义解析器
        /// </summary>
        /// <param name="type">要删除的解析类型</param>
        /// <returns>是否成功删除该类型的解析器</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveParser(Type type)
        {
            if (type is null) throw new ArgumentNullException();
            return p_diyPar.Remove(type.GUID);
        }

        /// <summary>
        /// 访问其中指定类型的自定义解析器
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>自定义解析器</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="KeyNotFoundException">指定类型所在的键不存在</exception>
        public ICustomParser GetCustomParser(Type type)
        {
            if (type is null) throw new ArgumentNullException();
            return p_diyPar[type.GUID];
        }

        /// <summary>
        /// 清空所有自定义解析器
        /// </summary>
        public void ClearParser()
        {
            p_diyPar.Clear();
        }

        /// <summary>
        /// 查询自定义解析器是否包含指定类型的解析器
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>拥有包含该类型的解析器返回true，没有解析器可解析此类型返回false；该查询仅查询自定义解析器，不会查询基础可解析类型</returns>
        public bool ContainsParser(Type type)
        {
            return p_diyPar.ContainsKey(type.GUID);
        }

        /// <summary>
        /// 访问当前解析器内所有的自定义类型解析器
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid,ICustomParser>.ValueCollection.Enumerator GetParserEnumerator()
        {
            return p_diyPar.Values.GetEnumerator();
        }

        /// <summary>
        /// 判断指定对象是否属于可解析类型
        /// </summary>
        /// <param name="obj">判断的对象</param>
        /// <returns>表示该对象类型</returns>
        public StreamParserType GetObjectType(object obj)
        {
            StreamParserType sp = f_IsPrmType(obj);
            if (sp != StreamParserType.None) return sp;
            return f_IsDiyType(obj, out _) ? StreamParserType.Custom : StreamParserType.None;
        }

        /// <summary>
        /// 返回给定类型的字节大小
        /// </summary>
        /// <param name="type">类型枚举</param>
        /// <returns>类型对象大小，若不是固定长度的类型返回-1；null类型返回0</returns>
        public static int SizeOf(StreamParserType type)
        {
            switch (type)
            {
                case StreamParserType.Int8:
                case StreamParserType.UInt8:
                case StreamParserType.Boolean:
                    return sizeof(byte);
                case StreamParserType.Int16:
                case StreamParserType.UInt16:
                case StreamParserType.Char:
                    return sizeof(short);
                case StreamParserType.Int32:
                case StreamParserType.UInt32:
                case StreamParserType.Float:
                    return sizeof(int);
                case StreamParserType.Int64:
                case StreamParserType.UInt64:
                case StreamParserType.Double:
                    return sizeof(long);
                case StreamParserType.Decimal:
                    return sizeof(decimal);
                case StreamParserType.Null:
                    return 0;
                default:
                    return -1;
            }
        }
        #endregion

        #region 封装

        #region 判断
        /// <summary>
        /// 尝试扩容
        /// </summary>
        /// <param name="newsize"></param>
        private void f_bufferCapacity(int newsize)
        {
            int length = p_buffer.Length;
            if(newsize > length) Array.Resize(ref p_buffer, newsize > length ? newsize : length * 2);
        }

        /// <summary>
        /// 判断基础类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private StreamParserType f_IsPrmType(object obj)
        {
            if (obj is null) return StreamParserType.Null;

            if (obj is int)
            {
                return StreamParserType.Int32;
            }
            if (obj is uint)
            {
                return StreamParserType.UInt32;
            }


            if (obj is float) return StreamParserType.Float;
            if (obj is double) return StreamParserType.Double;

            if (obj is string) return StreamParserType.String;

            if (obj is byte[]) return StreamParserType.ByteArray;

            if (obj is long)
            {
                return StreamParserType.Int64;
            }
            if (obj is ulong)
            {
                return StreamParserType.UInt64;
            }

            if (obj is byte)
            {
                return StreamParserType.Int8;
            }
            if (obj is sbyte)
            {
                return StreamParserType.UInt8;
            }
            if (obj is short)
            {
                return StreamParserType.Int16;
            }
            if (obj is ushort)
            {
                return StreamParserType.UInt16;
            }

            if (obj is decimal) return StreamParserType.Decimal;

            if (obj is bool) return StreamParserType.Boolean;
            if (obj is char) return StreamParserType.Char;

            if (obj is DateTime) return StreamParserType.DateTime;
            if (obj is TimeSpan) return StreamParserType.TimeSpan;
            if (obj is Guid) return StreamParserType.Guid;

            if (obj is IDictionary<string, object>) return StreamParserType.Dictionary;

            if (obj is IEnumerable) return StreamParserType.List;

            return StreamParserType.None;
        }

        /// <summary>
        /// 判断是否包含自定义
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool f_IsDiyType(object obj, out KeyValuePair<Type, ICustomParser> pars)
        {
            Type type = obj.GetType();
            ICustomParser csp;
            bool flag;

            flag = p_diyPar.TryGetValue(type.GUID, out csp);
            if (flag)
            {
                pars = new KeyValuePair<Type, ICustomParser>(type, csp);
                return true;
            }

            var vs = p_diyPar.Values;
            Type pt;

            foreach (var item in vs)
            {
                pt = item.GetObjectType();
                flag = pt.IsAssignableFrom(type);
                if (flag)
                {
                    pars = new KeyValuePair<Type, ICustomParser>(type, item);
                    return true;
                }
            }

            pars = default;
            return false;
        }
        #endregion

        #region 写入

        #region 写入

        /// <summary>
        /// 写入固定长度对象
        /// </summary>
        /// <param name="obj">写入的对象</param>
        /// <param name="type">对象类型</param>
        /// <param name="stream">被写入的流</param>
        private void f_writefixedObj(object obj, StreamParserType type, Stream stream)
        {
            //写入固定长度字节序列
            int size;
            switch (type)
            {
                case StreamParserType.Int8:
                    size = 1;
                    ((byte)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.UInt8:
                    size = 1;
                    ((sbyte)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Int16:
                    size = sizeof(short);
                    ((short)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.UInt16:
                    size = sizeof(ushort);
                    ((ushort)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Int32:
                    size = sizeof(int);
                    ((int)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.UInt32:
                    size = sizeof(uint);
                    ((uint)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Int64:
                    size = sizeof(long);
                    ((long)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.UInt64:
                    size = sizeof(ulong);
                    ((ulong)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Float:
                    size = sizeof(float);
                    ((float)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Double:
                    size = sizeof(double);
                    ((double)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Boolean:
                    size = sizeof(bool);
                    ((bool)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Decimal:
                    size = sizeof(decimal);
                    ((decimal)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Char:
                    size = sizeof(char);
                    ((char)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.DateTime:
                    size = sizeof(DateTime);
                    ((DateTime)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.TimeSpan:
                    size = sizeof(TimeSpan);
                    ((TimeSpan)obj).ToByteArray(p_buffer, 0);
                    break;
                case StreamParserType.Guid:
                    size = sizeof(Guid);
                    ((Guid)obj).ToByteArray(p_buffer, 0);
                    break;
                default:
                    return;
            }

            stream.Write(p_buffer, 0, size);
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        private void f_writeString(string obj, int length, Stream stream)
        {
            //写入字符数
            ((ushort)length).ToByteArray(p_buffer, 0);
            stream.Write(p_buffer, 0, 2);

            if (length == 0) return;
            int wsize = length * 2;
            f_bufferCapacity(wsize);
            //写入字符串
            obj.ToByteArray(0, p_buffer, 0);
            stream.Write(p_buffer, 0, wsize);
        }

        /// <summary>
        /// 写入集合
        /// </summary>
        /// <param name="enumator">集合</param>
        /// <param name="stream">写入的流</param>
        private void f_writeList(object enumator, Stream stream)
        {
            var enr = ((IEnumerable)enumator).GetEnumerator();
            bool flag;
            object obj;

            Loop:
            flag = enr.MoveNext();

            if (flag)
            {
                //有数据
                stream.WriteByte(have);
            }
            else
            {
                //无数据可写
                stream.WriteByte(over);
                return;
            }
            //获取数据
            obj = enr.Current;

            //写入数据

            f_writeObj(obj, stream);
            goto Loop;
        }

        /// <summary>
        /// 写入键值对
        /// </summary>
        /// <param name="dict">键值对</param>
        /// <param name="stream">写入的流</param>
        private void f_writeDict(IDictionary<string, object> dict, Stream stream)
        {
            var enr = dict.GetEnumerator();
            bool flag;
            KeyValuePair<string, object> pair;

            Loop:
            //推进
            flag = enr.MoveNext();

            if (flag)
            {
                //有数据
                stream.WriteByte(have);
            }
            else
            {
                //没有数据
                stream.WriteByte(over);
                return;
            }
            //获取值
            pair = enr.Current;

            //直接写入键
            f_writeString(pair.Key, pair.Key.Length, stream);

            //写入值
            f_writeObj(pair.Value, stream);

            goto Loop;

        }

        /// <summary>
        /// 写入自定义数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parser"></param>
        /// <param name="stream"></param>
        private void f_writeCumtonObj(object obj, ICustomParser parser, Stream stream)
        {
            //对象类型
            Type type = parser.GetObjectType();

            //类型名称
            //string name = f_getTypeName(type);
            //f_writeString(name, name.Length, stream);

            //写入类型标识符
            Guid guid = type.GUID;
            guid.ToByteArray(p_buffer, 0);
            stream.Write(p_buffer, 0, 16);

            //执行自定义写入
            try
            {
                parser.WriteObject(obj, stream);
            }
            catch (Exception ex)
            {
                f_throwCunstomParserException(ex, obj?.GetType());
            }        
        }

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="stream"></param>
        private void f_writeByteArray(byte[] buffer, Stream stream)
        {
            buffer.Length.ToByteArray(p_buffer, 0);
            //写入字节数
            stream.Write(p_buffer, 0, 4);
            //写入字节数组
            stream.Write(buffer, 0, buffer.Length);

        }

        #endregion

        /// <summary>
        /// 写入一个对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="stream">写入的流</param>
        private void f_writeObj(object obj, Stream stream)
        {
            //写入方式：类型值->对象字节序列
            

            StreamParserType sptype;
            bool flag;
            KeyValuePair<Type, ICustomParser> pair;
            string str;
            

            sptype = f_IsPrmType(obj);
            if(sptype != StreamParserType.None)
            {
                try
                {
                    //内部类型
                    if (sptype >= StreamParserType.Int8 && sptype <= StreamParserType.Null)
                    {
                        //固定类型

                        //写入类型数据
                        stream.WriteByte((byte)sptype);
                        //写入固定数据
                        f_writefixedObj(obj, sptype, stream);
                        return;
                    }
                    if (sptype == StreamParserType.String)
                    {
                        //字符串
                        //写入类型数据
                        stream.WriteByte((byte)sptype);
                        str = (string)obj;
                        f_writeString(str, str.Length, stream);
                        return;
                    }

                    if(sptype == StreamParserType.ByteArray)
                    {
                        stream.WriteByte((byte)sptype);
                        f_writeByteArray((byte[])obj, stream);
                        return;
                    }

                    if (sptype == StreamParserType.List)
                    {
                        stream.WriteByte((byte)sptype);
                        f_writeList(obj, stream);
                        return;
                    }
                    if (sptype == StreamParserType.Dictionary)
                    {
                        stream.WriteByte((byte)sptype);
                        f_writeDict((IDictionary<string, object>)obj, stream);
                        return;
                    }

                    throw new StreamParserDefaultException(obj?.GetType(), Cheng.Properties.Resources.StreamParserDef_NotConver);
                }
                catch (Exception ex)
                {

                    throw new StreamParserDefaultException(obj?.GetType(), Cheng.Properties.Resources.StreamParserDef_NotConver, ex);
                }

            }
            else
            {
                try
                {
                    flag = f_IsDiyType(obj, out pair);
                    if (flag)
                    {
                        //是自定义

                        //写入类型数据
                        stream.WriteByte((byte)StreamParserType.Custom);
                        //写入自定义数据
                        f_writeCumtonObj(obj, pair.Value, stream);

                        return;
                    }
                }
                catch (Exception ex)
                {

                    throw new StreamParserDefaultException(obj?.GetType(), Cheng.Properties.Resources.StreamParserDef_NotConver, ex);
                }
             
            }

            f_throwNotParserException(obj.GetType());

        }

        #endregion

        #region 读取

        /// <summary>
        /// 读取一个字节表类型
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private StreamParserType f_readType(Stream stream)
        {
            return (StreamParserType)stream.ReadByte();
        }

        /// <summary>
        /// 读取非托管内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private T f_readUnm<T>(Stream stream) where T : unmanaged
        {
            int size = sizeof(T);
            int r;

            f_bufferCapacity(size);
            r = stream.ReadBlock(p_buffer, 0, size);
            if(r != size) throw new StreamParserDefaultException();

            return p_buffer.ToStructure<T>(0);
        }

        /// <summary>
        /// 读取固定值
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object f_readPrmObj(Stream stream, StreamParserType type)
        {
            switch (type)
            {
                case StreamParserType.Int8:
                    return (byte)stream.ReadByte();
                case StreamParserType.UInt8:
                    return (sbyte)stream.ReadByte();
                case StreamParserType.Int16:
                    return f_readUnm<short>(stream);
                case StreamParserType.UInt16:
                    return f_readUnm<ushort>(stream);
                case StreamParserType.Int32:
                    return f_readUnm<int>(stream);
                case StreamParserType.UInt32:
                    return f_readUnm<uint>(stream);
                case StreamParserType.Int64:
                    return f_readUnm<long>(stream);
                case StreamParserType.UInt64:
                    return f_readUnm<ulong>(stream);
                case StreamParserType.Float:
                    return f_readUnm<float>(stream);
                case StreamParserType.Double:
                    return f_readUnm<double>(stream);
                case StreamParserType.Boolean:
                    return f_readUnm<bool>(stream);
                case StreamParserType.Char:
                    return f_readUnm<char>(stream);
                case StreamParserType.Decimal:
                    return f_readUnm<decimal>(stream);
                case StreamParserType.DateTime:
                    return f_readUnm<DateTime>(stream);
                case StreamParserType.TimeSpan:
                    return f_readUnm<TimeSpan>(stream);
                case StreamParserType.Guid:
                    return f_readUnm<Guid>(stream);
                case StreamParserType.Null:
                    return null;
            }

            return null;
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string f_readStr(Stream stream)
        {
            //字符数
            ushort length = f_readUnm<ushort>(stream);
            int bsize = length * 2;
            f_bufferCapacity(bsize);

            //字符串内存读取
            int r = stream.ReadBlock(p_buffer, 0, bsize);
            if (r != bsize) throw new StreamParserDefaultException();

            return p_buffer.ToStringBuffer(0, length);
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte[] f_readBytes(Stream stream)
        {
            int r = stream.ReadBlock(p_buffer, 0, 4);
            if(r != 4) throw new StreamParserDefaultException();
            int count = p_buffer.ToStructure<int>(0);

            byte[] rt = new byte[count];
            r = stream.ReadBlock(rt, 0, count);
            if (r != count) throw new StreamParserDefaultException();

            return rt;
        }

        /// <summary>
        /// 读取集合
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private List<object> f_readList(Stream stream)
        {
            List<object> list = new List<object>();
            int ri;

            Loop:
            //读取分隔符
            ri = stream.ReadByte();
            if (ri == -1) throw new StreamParserDefaultException();

            
            if(ri == over)
            {
                //结束
                return list;
            }
            if (ri != have) throw new StreamParserDefaultException();

            //继续读取值添加
            list.Add(f_readObject(stream));

            goto Loop;

        }

        /// <summary>
        /// 读取键值对
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private Dictionary<string, object> f_readDict(Stream stream)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            int ri;
            string key;
            object obj;

            Loop:
            //读取分隔符
            ri = stream.ReadByte();
            if (ri == over)
            {
                return dict;
            }
            if (ri != have) throw new StreamParserDefaultException();

            //读取字符串键
            key = f_readStr(stream);

            //读取值
            obj = f_readObject(stream);

            //添加
            addKeyValuePair(dict, key, obj);

            goto Loop;
        }

        private void addKeyValuePair(IDictionary<string, object> dict, string key, object value)
        {
            dict[key] = value;
            //dict.Add(key, value);
        }

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private object f_readCustom(Stream stream)
        {
            //读取类型id
            ICustomParser cp;
            //string name = f_readStr(stream);
            Guid guid;

            int ri = stream.ReadBlock(p_buffer, 0, 16);
            if(ri != 16) throw new StreamParserDefaultException();

            fixed (byte* bufp = p_buffer)
            {
                guid = *((Guid*)bufp);
            }

            if (!p_diyPar.TryGetValue(guid, out cp))
            {
                //不存在
                throw new StreamParserDefaultException();
            }

            //读取值
            return cp.ReadObject(stream);
        }

        /// <summary>
        /// 读取对象
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private object f_readObject(Stream stream)
        {
            var type = f_readType(stream);

            if(type > StreamParserType.None && type <= StreamParserType.Null)
            {
                //固定长度类型
                return f_readPrmObj(stream, type);
            }

            if(type == StreamParserType.String)
            {
                return f_readStr(stream);
            }
            if(type == StreamParserType.ByteArray)
            {
                return f_readBytes(stream);
            }
            if(type == StreamParserType.List)
            {
                return f_readList(stream);
            }
            if (type == StreamParserType.Dictionary)
            {
                return f_readDict(stream);
            }
            if (type == StreamParserType.Custom)
            {
                return f_readCustom(stream);
            }

            return f_throwNotParserException(null);
        }

        #endregion

        #region 异常抛出

        /// <summary>
        /// 抛出"无法解析"异常
        /// </summary>
        /// <param name="type"></param>
        private object f_throwNotParserException(Type type)
        {
            throw new StreamParserDefaultException(type, Cheng.Properties.Resources.StreamParserDef_NotConver);
        }

        /// <summary>
        /// 抛自定义写入异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object f_throwCunstomParserException(Exception ex, Type type)
        {
            throw new StreamParserDefaultException(type, Cheng.Properties.Resources.StreamParserDef_CunstomWriterExc, ex);
        }

        #endregion

        #endregion

    }

}
