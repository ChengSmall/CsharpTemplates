using Cheng.Memorys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Cheng.Streams.Parsers.Default
{

    #region 枚举

    /// <summary>
    /// <see cref="StreamParserDefault"/>对象当中的可解析类型
    /// </summary>
    public enum StreamParserType : byte
    {
        /// <summary>
        /// 不是可解析的类型
        /// </summary>
        None,

        /// <summary>
        /// 单字节<see cref="byte"/>
        /// </summary>
        Int8,

        /// <summary>
        /// 无符号单字节
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
        /// C#对象，<see cref="System.Guid"/>
        /// </summary>
        Guid,

        /// <summary>
        /// C#对象，时间刻<see cref="System.DateTime"/>
        /// </summary>
        DateTime,

        /// <summary>
        /// C#对象，时间间隔<see cref="System.TimeSpan"/>
        /// </summary>
        TimeSpan,

        /// <summary>
        /// c#对象，表示一个null的值
        /// </summary>
        Null,

        /// <summary>
        /// c#对象，字符串类型<see cref="string"/>
        /// </summary>
        String,

        /// <summary>
        /// c#对象，字节数组<see cref="byte"/>[]
        /// </summary>
        ByteArray,

        /// <summary>
        /// 集合类型
        /// </summary>
        /// <remarks>
        /// 任何继承了<see cref="IEnumerable"/>接口的类型都可以当作一个集合类型；但是集合中的值必须是可解析的类型；允许类型嵌套
        /// </remarks>
        List,

        /// <summary>
        /// 键值对类型
        /// </summary>
        /// <remarks>
        /// 当一个对象继承了<see cref="IDictionary{TKey, TValue}"/>接口，且TKey是字符串类型，TValue是可解析的类型，则表示这个对象是可解析的键值对类型；允许类型嵌套
        /// </remarks>
        Dictionary,

        /// <summary>
        /// 自定义类型
        /// </summary>
        /// <remarks>
        /// 在<see cref="StreamParserDefault"/>默认实现的对象中，使用<see cref="StreamParserDefault.SetParser(ICustomParser)"/>添加自定义类型解析器，可做到对自定义类型的解析
        /// </remarks>
        Custom

    }

    /// <summary>
    /// 自定义类型解析的标识符类型
    /// </summary>
    public enum CustomTypeNameAccess : byte
    {

        /// <summary>
        /// 类型的完全限定全称；命名空间.类名
        /// </summary>
        FullName,

        /// <summary>
        /// 类型的名称
        /// </summary>
        Name,

        /// <summary>
        /// 类型的程序集限定名，其中包括从中加载的<see cref="Type"/>程序集的名称
        /// </summary>
        AssemblyQualifiedName,

        /// <summary>
        /// 完全自定义返回名称
        /// </summary>
        CustomRetValue
    }

    #endregion

    #region 异常

    /// <summary>
    /// 默认流数据解析器出现错误时引发的异常
    /// </summary>
    public class StreamParserDefaultException : StreamParserException
    {

        #region

        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        public StreamParserDefaultException() : base("流数据解析器异常")
        {
        }

        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="message">指定错误消息</param>
        public StreamParserDefaultException(string message) : base(message)
        {
        }

        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="message">指定错误消息</param>
        /// <param name="exception">引发该异常的异常</param>
        public StreamParserDefaultException(string message, Exception exception) : base(message, exception)
        {
        }

        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        public StreamParserDefaultException(Type type) : base(type)
        {
        }

        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="message">指定错误消息</param>
        public StreamParserDefaultException(Type type, string message) : base(type, message)
        {
        }

        /// <summary>
        /// 实例化一个解析器错误异常
        /// </summary>
        /// <param name="message">指定错误消息</param>
        /// <param name="exception">引发该异常的异常</param>
        public StreamParserDefaultException(Type type, string message, Exception exception) : base(type, message, exception)
        {
        }

        #endregion

    }

    #endregion

    #region 自定义解析器派生

    /// <summary>
    /// <see cref="StreamParserDefault"/>的自定义类型解析器
    /// </summary>
    /// <remarks>
    /// 用于<see cref="StreamParserDefault"/>类型对象的自定义实现；自定义解析器在<see cref="StreamParserDefault"/>当中是最低优先级，如果实现的类型已经在<see cref="StreamParserType"/>类型枚举中出现，则会优先使用默认实现
    /// </remarks>
    public interface ICustomParser
    {
        /// <summary>
        /// 获取自定义解析的对象类型
        /// </summary>
        /// <returns>需要解析的对象类型</returns>
        Type GetObjectType();

        /// <summary>
        /// 从当前位置写入自定义类型的对象
        /// </summary>
        /// <remarks>
        /// 在派生类中实现，你需要定义一种方法能够将<paramref name="obj"/>转化并写入到流当中
        /// </remarks>
        /// <param name="obj">写入的对象</param>
        /// <param name="stream">要写入的流，写入时不得改变流的位置</param>
        void WriteObject(object obj, Stream stream);

        /// <summary>
        /// 从当前位置的流数据中读取自定义对象
        /// </summary>
        /// <remarks>
        /// <para>默认实现器会将流的位置推进到数据段的首位，在派生类实现时你需要使用<paramref name="stream"/>读取字节序列转化为对象并返回；<br/>
        /// 在读取数据时请保证读取的字节大小和使用<see cref="WriteObject(object, Stream)"/>方法写入的字节大小一致，否则流的位置将不会处于下一个对象数据段的起始位
        /// </para>
        /// </remarks>
        /// <param name="stream">要读取的流数据</param>
        /// <returns>读取到的对象</returns>
        object ReadObject(Stream stream);

    }

    /// <summary>
    /// <see cref="StreamParserDefault"/>的自定义类型解析器公共基类
    /// </summary>
    public abstract class CustomParser : ICustomParser
    {
        public abstract Type GetObjectType();
        public abstract object ReadObject(Stream stream);
        public abstract void WriteObject(object obj, Stream stream);
    }

    /// <summary>
    /// 非托管类型数据解析器
    /// </summary>
    /// <remarks>
    /// 此派生类实现了所有非托管类型数据的读写
    /// </remarks>
    /// <typeparam name="T">非托管类型</typeparam>
    public sealed unsafe class UnmanagedParser<T> : CustomParser where T : unmanaged
    {
        private readonly Type type = typeof(T);

        public override Type GetObjectType()
        {
            return type;
        }

        private byte[] buffer = new byte[sizeof(T)];

        public override object ReadObject(Stream stream)
        {
            int r = stream.ReadBlock(buffer, 0, sizeof(T));

            if (r != buffer.Length) return null;

            return buffer.ToStructure<T>(0);
        }

        public override void WriteObject(object obj, Stream stream)
        {
            T t = (T)obj;
            t.ToByteArray(buffer, 0);
            stream.Write(buffer, 0, sizeof(T));
        }

    }

    #endregion

}
