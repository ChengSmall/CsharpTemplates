using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace Cheng.Streams.Parsers.Serialization
{

    /// <summary>
    /// 流数据解析类型
    /// </summary>
    public enum SerializeStreamParserType : byte
    {
        /// <summary>
        /// 不是基本类型
        /// </summary>
        None,
        /// <summary>
        /// 单字节<see cref="sbyte"/>
        /// </summary>
        Int8,
        /// <summary>
        /// 无符号单字节<see cref="byte"/>
        /// </summary>
        UInt8,
        /// <summary>
        /// 短整型
        /// </summary>
        Int16,
        /// <summary>
        /// 无符号短整型
        /// </summary>
        UInt16,
        /// <summary>
        /// 32位整形
        /// </summary>
        Int32,
        /// <summary>
        /// 无符号32位整形
        /// </summary>
        UInt32,
        /// <summary>
        /// 64位整形
        /// </summary>
        Int64,
        /// <summary>
        /// 无符号64位整形
        /// </summary>
        UInt64,
        /// <summary>
        /// 单精度浮点类型(4 byte)
        /// </summary>
        Float,
        /// <summary>
        /// 双精度浮点类型(8 byte)
        /// </summary>
        Double,
        /// <summary>
        /// 布尔类型(1 byte)
        /// </summary>
        Boolean,
        /// <summary>
        /// 字符类型 (2 byte)
        /// </summary>
        Char,
        /// <summary>
        /// c#对象，十进制数<see cref="decimal"/>
        /// </summary>
        Decimal,
        /// <summary>
        /// C#对象，时间刻对象
        /// </summary>
        DateTime,
        /// <summary>
        /// C#对象，时间间隔对象
        /// </summary>
        TimeSpan,
        /// <summary>
        /// C#对象，<see cref="System.Guid"/>对象
        /// </summary>
        Guid,
        /// <summary>
        /// c#对象，表示一个null的值
        /// </summary>
        Null,
        /// <summary>
        /// c#对象，字符串类型<see cref="string"/>
        /// </summary>
        String,
        /// <summary>
        /// C#对象，<see cref="System.Array"/>派生数组，数组元素必须是可解析对象
        /// </summary>
        Array
    }

    /// <summary>
    /// 一个流数据解析器，能够直接解析自定义类型
    /// </summary>
    /// <remarks>可解析自定义类型的解析器，一般可解析类型查看<see cref="SerializeStreamParserType"/>枚举，自定义类型必须是有公开的无参构造函数的对象</remarks>
    public unsafe class SerializeStreamParser : StreamParser
    {

        #region 构造
        /// <summary>
        /// 实例化流数据解析器
        /// </summary>
        public SerializeStreamParser()
        {
            p_buffer = new byte[16];
        }

        #endregion

        #region 参数

        #region 常量

        const byte have = 255;
        const byte over = 0;

        #endregion

        #region buffer

        private byte[] p_buffer;

        #endregion

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
            if (newsize > length) Array.Resize(ref p_buffer, newsize > length ? newsize : length * 2);
        }

        /// <summary>
        /// 判断基础类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private SerializeStreamParserType f_IsBaseType(object obj)
        {
            if (obj is null) return SerializeStreamParserType.Null;

            if (obj is int)
            {
                return SerializeStreamParserType.Int32;
            }
            if (obj is uint)
            {
                return SerializeStreamParserType.UInt32;
            }


            if (obj is float) return SerializeStreamParserType.Float;
            if (obj is double) return SerializeStreamParserType.Double;

            if (obj is string) return SerializeStreamParserType.String;

            if (obj is long)
            {
                return SerializeStreamParserType.Int64;
            }
            if (obj is ulong)
            {
                return SerializeStreamParserType.UInt64;
            }

            if (obj is sbyte)
            {
                return SerializeStreamParserType.Int8;
            }
            if (obj is byte)
            {
                return SerializeStreamParserType.UInt8;
            }
            if (obj is short)
            {
                return SerializeStreamParserType.Int16;
            }
            if (obj is ushort)
            {
                return SerializeStreamParserType.UInt16;
            }

            if (obj is decimal) return SerializeStreamParserType.Decimal;

            if (obj is bool) return SerializeStreamParserType.Boolean;
            if (obj is char) return SerializeStreamParserType.Char;

            if(obj is Array)
            {
                return SerializeStreamParserType.Array;
            }

            //Type type = obj.GetType();
            //if (typeof(Array).IsAssignableFrom(type)) return SerializeStreamParserType.Array;


            return SerializeStreamParserType.None;
        }

        #endregion

        const string cp_nullTypeName = "Null";

        #region 写入

        #region 写入
        /// <summary>
        /// 写入非托管
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        private void f_writeUnm<T>(T value, Stream stream) where T : unmanaged
        {
            f_bufferCapacity(sizeof(T));

            value.ToByteArray(p_buffer, 0);

            stream.Write(p_buffer, 0, sizeof(T));
        }

        /// <summary>
        /// 写入固定长度对象
        /// </summary>
        /// <param name="obj">写入的对象</param>
        /// <param name="type">对象类型</param>
        /// <param name="stream">被写入的流</param>
        private void f_writefixedObj(object obj, SerializeStreamParserType type, Stream stream)
        {
            //写入固定长度字节序列
            int size;
            switch (type)
            {
                case SerializeStreamParserType.Int8:
                    size = 1;
                    ((sbyte)obj).ToByteArray(p_buffer, 0);

                    break;
                case SerializeStreamParserType.UInt8:
                    size = 1;
                    ((byte)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Int16:
                    size = sizeof(short);
                    ((short)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.UInt16:
                    size = sizeof(ushort);
                    ((ushort)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Int32:
                    size = sizeof(int);
                    ((int)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.UInt32:
                    size = sizeof(uint);
                    ((uint)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Int64:
                    size = sizeof(long);
                    ((long)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.UInt64:
                    size = sizeof(ulong);
                    ((ulong)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Float:
                    size = sizeof(float);
                    ((float)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Double:
                    size = sizeof(double);
                    ((double)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Boolean:
                    size = sizeof(bool);
                    ((bool)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Decimal:
                    size = sizeof(decimal);
                    ((decimal)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Char:
                    size = sizeof(char);
                    ((char)obj).ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.DateTime:
                    size = 8;
                    ((DateTime)obj).Ticks.ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.TimeSpan:
                    size = 8;
                    ((TimeSpan)obj).Ticks.ToByteArray(p_buffer, 0);
                    break;
                case SerializeStreamParserType.Guid:
                    size = 16;
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
            (length).ToByteArray(p_buffer, 0);
            stream.Write(p_buffer, 0, 4);

            if (length == 0) return;
            int wsize = length * 2;
            f_bufferCapacity(wsize);
            //写入字符串
            obj.ToByteArray(0, p_buffer, 0);
            stream.Write(p_buffer, 0, wsize);
        }
        /// <summary>
        /// 写入数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="stream"></param>
        private void f_writeArray(Array array, Stream stream)
        {

            if (array.Rank != 1) throw new StreamParserException();

            //写入元素类型
            object tobj = array.GetValue(0);
            var stype = f_IsBaseType(tobj);

            f_writeUnm((byte)stype, stream);

            if (stype == SerializeStreamParserType.None)
            {
                //复合类型写入标识符
                Type objType = array.GetValue(0)?.GetType();
                string typeName = (objType?.AssemblyQualifiedName) ?? cp_nullTypeName;
                f_writeString(typeName, typeName.Length, stream);
            }


            //长度
            int length = array.Length;

            //写入长度
            length.ToByteArray(p_buffer, 0);
            stream.Write(p_buffer, 0, 4);

            if(length == 0)
            {
                //空数组
                return;
            }

            object obj;
            int i;

            //写入

            for (i = 0; i < length; i++)
            {
                obj = array.GetValue(i);

                f_writeObj(obj, stream);
            }

        }

        /// <summary>
        /// 写入复合对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        private void f_writeFObj(object obj, Stream stream)
        {

            FieldInfo info;
            object to;
            string name;
            

            //获取对象运行时类型
            Type type = obj.GetType();

            //获取字段
            FieldInfo[] fs = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //写入对象类型id
            //Guid tid = type.GUID;
            //f_writeUnm<Guid>(tid, stream);

            name = type.AssemblyQualifiedName;
            f_writeString(name, name.Length, stream);


            int length = fs.Length;
            //没有字段写入终止符
            if(length == 0)
            {
                stream.WriteByte(over);
                return;
            }
            else
            {

                stream.WriteByte(have);
            }


            int end = length - 1;
            //开始写入字段
            for (int i = 0; i < length; i++)
            {


                info = fs[i];

                name = info.Name;

                to = info.GetValue(obj);
                //字段名称
                f_writeString(name, name.Length, stream);

                f_writeObj(to, stream);

                if(i == end)
                {
                    stream.WriteByte(over);
                }
                else
                {
                    stream.WriteByte(have);
                }

            }


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


            SerializeStreamParserType sptype;
            //bool flag;
            string str;


            sptype = f_IsBaseType(obj);
            if (sptype != SerializeStreamParserType.None)
            {
                try
                {
                    //内部类型
                    if (sptype >= SerializeStreamParserType.Int8 && sptype <= SerializeStreamParserType.Null)
                    {
                        //固定类型

                        //写入类型数据
                        stream.WriteByte((byte)sptype);
                        //写入固定数据
                        f_writefixedObj(obj, sptype, stream);
                        return;
                    }

                    if (sptype == SerializeStreamParserType.String)
                    {
                        //字符串
                        //写入类型数据
                        stream.WriteByte((byte)sptype);
                        str = (string)obj;
                        f_writeString(str, str.Length, stream);
                        return;
                    }

                    if (sptype == SerializeStreamParserType.Array)
                    {
                        //数组
                        stream.WriteByte((byte)sptype);
                        f_writeArray((Array)obj, stream);
                        return;
                    }

                    throw new StreamParserException(obj?.GetType(), "无法解析数据");
                }
                catch (Exception ex)
                {

                    throw new StreamParserException(obj?.GetType(), "无法解析数据", ex);
                }

            }
            else
            {

                //嵌套类型

                //写入类型数据
                stream.WriteByte((byte)sptype);

                f_writeFObj(obj, stream);
                return;
            }

            throw new StreamParserException(obj?.GetType(), "无法解析数据");

        }
        #endregion

        #region 读取

        /// <summary>
        /// 读取一个字节表类型
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private SerializeStreamParserType f_readType(Stream stream)
        {
            return (SerializeStreamParserType)stream.ReadByte();
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
            if (r != size) throw new StreamParserException();

            return p_buffer.ToStructure<T>(0);
        }
        /// <summary>
        /// 读取固定值
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object f_readPrmObj(Stream stream, SerializeStreamParserType type)
        {
            switch (type)
            {
                case SerializeStreamParserType.Int8:
                    return (sbyte)stream.ReadByte();
                case SerializeStreamParserType.UInt8:
                    return (byte)stream.ReadByte();
                case SerializeStreamParserType.Int16:
                    return f_readUnm<short>(stream);
                case SerializeStreamParserType.UInt16:
                    return f_readUnm<ushort>(stream);
                case SerializeStreamParserType.Int32:
                    return f_readUnm<int>(stream);
                case SerializeStreamParserType.UInt32:
                    return f_readUnm<uint>(stream);
                case SerializeStreamParserType.Int64:
                    return f_readUnm<long>(stream);
                case SerializeStreamParserType.UInt64:
                    return f_readUnm<ulong>(stream);
                case SerializeStreamParserType.Float:
                    return f_readUnm<float>(stream);
                case SerializeStreamParserType.Double:
                    return f_readUnm<double>(stream);
                case SerializeStreamParserType.Boolean:
                    return f_readUnm<bool>(stream);
                case SerializeStreamParserType.Char:
                    return f_readUnm<char>(stream);
                case SerializeStreamParserType.Decimal:
                    return f_readUnm<decimal>(stream);
                case SerializeStreamParserType.Null:
                    return null;
                default:
                    return null;
            }


        }
        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string f_readStr(Stream stream)
        {
            //字符数
            int length = f_readUnm<int>(stream);
            int bsize = length * 2;
            f_bufferCapacity(bsize);

            //字符串内存读取
            int r = stream.ReadBlock(p_buffer, 0, bsize);
            if (r != bsize) throw new StreamParserException();

            return p_buffer.ToStringBuffer(0, length);
        }


        static Type f_stypeGetType(SerializeStreamParserType type)
        {
            switch (type)
            {
                case SerializeStreamParserType.Int8:
                    return typeof(sbyte);
                case SerializeStreamParserType.UInt8:
                    return typeof(byte);
                case SerializeStreamParserType.Int16:
                    return typeof(short);
                case SerializeStreamParserType.UInt16:
                    return typeof(ushort);
                case SerializeStreamParserType.Int32:
                    return typeof(int);
                case SerializeStreamParserType.UInt32:
                    return typeof(uint);
                case SerializeStreamParserType.Int64:
                    return typeof(long);
                case SerializeStreamParserType.UInt64:
                    return typeof(ulong);
                case SerializeStreamParserType.Float:
                    return typeof(float);
                case SerializeStreamParserType.Double:
                    return typeof(double);
                case SerializeStreamParserType.Boolean:
                    return typeof(bool);
                case SerializeStreamParserType.Char:
                    return typeof(char);
                case SerializeStreamParserType.Decimal:
                    return typeof(decimal);
                case SerializeStreamParserType.DateTime:
                    return typeof(DateTime);
                case SerializeStreamParserType.TimeSpan:
                    return typeof(TimeSpan);
                case SerializeStreamParserType.Guid:
                    return typeof(Guid);
                case SerializeStreamParserType.Null:
                    return null;
                case SerializeStreamParserType.String:
                    return typeof(string);
                case SerializeStreamParserType.Array:
                    return typeof(Array);
                default:
                    return null;
            }
        }

        private Array f_readArray(Stream stream)
        {
            //读取类型
            SerializeStreamParserType stype = (SerializeStreamParserType)f_readUnm<byte>(stream);

            Type type;
            if(stype == SerializeStreamParserType.None)
            {
                string typeName = f_readStr(stream);
                type = Type.GetType(typeName);
            }
            else
            {
                type = f_stypeGetType(stype);
            }

            //获取长度
            int length = f_readUnm<int>(stream);
            //实例化数组
            Array array = Array.CreateInstance(type, length);
            object ov;

            int i;
            for (i = 0; i < length; i++)
            {
                //读取元素
                ov = f_readObject(stream);
                //写入对象
                array.SetValue(ov, i);
            }

            return array;

        }

        /// <summary>
        /// 读取复合对象
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private object f_readFObj(Stream stream)
        {

            /* 1：类型标识符
             * 2：按键值对写入（字段名，对象），后接终止符或继续符
             */

          
            Type type;

            //读取类型id
            //Guid guid;
            //guid = f_readUnm<Guid>(stream);
            //type = Type.GetTypeFromCLSID(guid, null, true);

            string typeName = f_readStr(stream);
            type = Type.GetType(typeName, true, false);

            //实例化对象
            var obj = Activator.CreateInstance(type);

            //var fs = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            FieldInfo info;
            byte rb;
            string name;

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            while (true)
            {
                rb = f_readUnm<byte>(stream);

                if(rb == over)
                {
                    //结束
                    break;
                }
                //读取字段名
                name = f_readStr(stream);
                //获取字段反射
                info = type.GetField(name, flags);
                //写入
                info.SetValue(obj, f_readObject(stream));

            }

            return obj;
        }

        /// <summary>
        /// 读取对象
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private object f_readObject(Stream stream)
        {
            var type = f_readType(stream);

            if (type > SerializeStreamParserType.None && type <= SerializeStreamParserType.Null)
            {
                //固定长度类型
                return f_readPrmObj(stream, type);
            }

            if (type == SerializeStreamParserType.String)
            {
                return f_readStr(stream);
            }

            if(type == SerializeStreamParserType.Array)
            {
                return f_readArray(stream);
            }

            if (type == SerializeStreamParserType.None)
            {
                return f_readFObj(stream);
            }

            throw new StreamParserException("无法解析类型");

        }
        #endregion

        #endregion

        public override object ConverToObject(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException("stream");
            
            return f_readObject(stream);
        }

        /// <summary>
        /// 将对象写入流
        /// </summary>
        /// <param name="obj">写入的对象，对象必须是指定的<see cref="SerializeStreamParserType"/>内部类型或者拥有无参构造的自定义类型</param>
        /// <param name="stream">要写入的流</param>
        /// <exception cref="ArgumentNullException">stream参数为null</exception>
        /// <exception cref="StreamParserException">无法解析数据</exception>
        public override void ConverToStream(object obj, Stream stream)
        {
            if (stream is null) throw new ArgumentNullException("stream");

            f_writeObj(obj, stream);
        }

        /// <summary>
        /// 将流转化为指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">要读取的流</param>
        /// <returns>转化的对象</returns>
        /// <exception cref="StreamParserException">无法解析数据</exception>
        /// <exception cref="InvalidCastException">无效的类型转换</exception>
        /// <exception cref="ArgumentNullException">stream参数为null</exception>
        public override T ConverToObject<T>(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException("stream");
            return (T)f_readObject(stream);
        }

    }

}
