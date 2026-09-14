using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cheng.Algorithm.HashCodes;
using Cheng.Algorithm.Randoms;
using Cheng.DataStructure.Hashs;
using Cheng.Memorys;
using Cheng.Texts;

namespace Cheng.DataStructure.Uuids
{

    /// <summary>
    /// 表示UUID的对象
    /// </summary>
    /// <remarks>
    /// <para>一个16字节大小的UUID值</para>
    /// </remarks>
    public unsafe readonly struct Uuid : IEquatable<Uuid>, IHashCode64
    {

        #region 初始化

        /// <summary>
        /// 初始化UUID对象
        /// </summary>
        /// <remarks>
        /// <para>此构造函数仅用于代码构造对象使用，非UUID标准参数格式</para>
        /// </remarks>
        /// <param name="i32_1"></param>
        /// <param name="i32_2"></param>
        /// <param name="i64"></param>
        public Uuid(uint i32_1, uint i32_2, ulong i64)
        {
            this.i32_1 = i32_1;
            this.i32_2 = i32_2;
            this.i64 = i64;
        }

        #endregion

        #region 参数

        // xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        // 4字节-2字节-2字节-2字节-6字节

        // xxxxxxxx xxxx-xxxx xxxx-xxxxxxxxxxxx
        // 4字节 4字节 8字节

        public readonly uint i32_1;

        public readonly uint i32_2;

        public readonly ulong i64;

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 空值
        /// </summary>
        public Uuid Nil
        {
            get => new Uuid(0, 0, 0);
        }

        /// <summary>
        /// 最大值 (MaxUUID)
        /// </summary>
        public Uuid MaxUUID
        {
            get => new Uuid(uint.MaxValue, uint.MaxValue, ulong.MaxValue);
        }

        /// <summary>
        /// 获取UUID的版本字段
        /// </summary>
        /// <value>前4bit组合的UUID版本字段值</value>
        public byte Version
        {
            get
            {
                // 第7个字节高4位
                // p_i32_2 - b4-b7 == 版本字段
                return (byte)(((i32_2 >> (16 + 4)) & 0xF));
            }
        }

        /// <summary>
        /// 获取UUID的变体字段
        /// </summary>
        /// <value>前4bit组合的UUID变体字段值</value>
        public byte Variant
        {
            get
            {
                // 第9个字节高2位
                // p_i64 - b6-b7 == 变体字段
                return (byte)((i64 & 0xFF) >> 4);
            }
        }

        /// <summary>
        /// 获取UUID的版本字段
        /// </summary>
        public UuidVersionField VersionField
        {
            get => (UuidVersionField)Version;
        }

        /// <summary>
        /// 设置当前UUID值的版本字段与变体字段并返回新设置的值
        /// </summary>
        /// <param name="ver">版本字段，仅前4bit位可用</param>
        /// <param name="variant">变体字段，仅前4bit位可用</param>
        /// <returns>新设置的值</returns>
        public Uuid SetVersionAndVariant(byte ver, byte variant)
        {
            var i32_2 = (this.i32_2 & (~(0xF0U << 16))) | (((uint)(ver & 0xF)) << (16 + 4));
            var i64 = (this.i64 & (~(0b1111UL << 4))) | (((ulong)(variant & 0b1111)) << 4);
            return new Uuid(i32_1, i32_2, i64);
        }

        /// <summary>
        /// 设置当前UUID值的版本字段与变体字段并返回新设置的值
        /// </summary>
        /// <param name="ver">版本字段，仅前4bit位可用</param>
        /// <param name="variant">变体字段单独前2bit值，仅前2bit位可用</param>
        /// <returns>新设置的值</returns>
        public Uuid SetVersionAndVariantB2(byte ver, byte variant)
        {
            var i32_2 = (this.i32_2 & (~(0xF0U << 16))) | (((uint)(ver & 0xF)) << (16 + 4));
            var i64 = (this.i64 & (~(0b11UL << 6))) | (((ulong)(variant & 0b11)) << 6);
            return new Uuid(i32_1, i32_2, i64);
        }

        /// <summary>
        /// 将UUID设为v4版本
        /// </summary>
        /// <returns>新设置的值</returns>
        public Uuid SetToUUIDv4()
        {
            return SetVersionAndVariantB2(0b0100, 0b10);
        }

        /// <summary>
        /// 将UUID设为v8版本
        /// </summary>
        /// <returns>新设置的值</returns>
        public Uuid SetToUUIDv8()
        {
            return SetVersionAndVariantB2(0b1000, 0b10);
        }

        /// <summary>
        /// 设置UUID的版本字段
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        public Uuid SetUuidVersion(UuidVersionField ver)
        {
            return SetVersionAndVariantB2((byte)ver, 0b10);
        }

        /// <summary>
        /// 设置当前UUID值的版本字段与变体字段并返回新设置的值
        /// </summary>
        /// <param name="ver">版本字段，仅前4bit位可用</param>
        /// <returns>新设置的值</returns>
        public Uuid SetVersion(byte ver)
        {
            var i32_2 = (this.i32_2 & (~(0xF0U << 16))) | (((uint)(ver & 0xF)) << (16 + 4));
            return new Uuid(i32_1, i32_2, i64);
        }

        /// <summary>
        /// 设置当前UUID值的版本字段与变体字段并返回新设置的值
        /// </summary>
        /// <param name="variant">变体字段，仅前4bit位可用</param>
        /// <returns>新设置的值</returns>
        public Uuid SetVersionVariant(byte variant)
        {
            var i64 = (this.i64 & (~(0b1111UL << 4))) | (((ulong)(variant & 0b1111)) << 4);
            return new Uuid(i32_1, i32_2, i64);
        }

        /// <summary>
        /// 设置当前UUID值的版本字段与变体字段并返回新设置的值
        /// </summary>
        /// <param name="variant">变体字段单独前2bit值，仅前2bit位可用</param>
        /// <returns>新设置的值</returns>
        public Uuid SetVersionVariantB2(byte variant)
        {
            var i64 = (this.i64 & (~(0b11UL << 6))) | (((ulong)(variant & 0b11)) << 6);
            return new Uuid(i32_1, i32_2, i64);
        }

        #endregion

        #region 转换到文本

        private void f_ToText(TextWriter writer, bool isUpper, char link)
        {
            byte* buf = stackalloc byte[16];
            f_toByteBuf(buf);
            int i;
            byte tb;
            char lc, rc;
            // 前4字节
            for (i = 0; i < 4; i++)
            {
                tb = buf[i];
                tb.ValueToX16Char(isUpper, out lc, out rc);
                writer.Write(lc); writer.Write(rc);
            }
            writer.Write(link);
            // 2字节 1
            tb = buf[4];
            tb.ValueToX16Char(isUpper, out lc, out rc);
            writer.Write(lc); writer.Write(rc);
            tb = buf[5];
            tb.ValueToX16Char(isUpper, out lc, out rc);
            writer.Write(lc); writer.Write(rc);

            writer.Write(link);
            // 2字节 2
            tb = buf[6];
            tb.ValueToX16Char(isUpper, out lc, out rc);
            writer.Write(lc); writer.Write(rc);
            tb = buf[7];
            tb.ValueToX16Char(isUpper, out lc, out rc);
            writer.Write(lc); writer.Write(rc);

            writer.Write(link);
            // 2字节 3

            tb = buf[8];
            tb.ValueToX16Char(isUpper, out lc, out rc);
            writer.Write(lc); writer.Write(rc);
            tb = buf[9];
            tb.ValueToX16Char(isUpper, out lc, out rc);
            writer.Write(lc); writer.Write(rc);

            writer.Write(link);
            // 6字节
            for (i = 0; i < 6; i++)
            {
                tb = buf[10 + i];
                tb.ValueToX16Char(isUpper, out lc, out rc);
                writer.Write(lc); writer.Write(rc);
            }
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="writer">要写入的文本写入器</param>
        /// <param name="isUpper">字母是否大写</param>
        /// <param name="link">连接符号</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(TextWriter writer, bool isUpper, char link)
        {
            if (writer is null) throw new ArgumentNullException();
            f_ToText(writer, isUpper, link);
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="writer">要写入的文本写入器</param>
        /// <param name="isUpper">字母是否大写</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(TextWriter writer, bool isUpper)
        {
            ToText(writer, isUpper, '-');
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="writer">要写入的文本写入器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(TextWriter writer)
        {
            ToText(writer, false, '-');
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <param name="isUpper">字母是否大写</param>
        /// <param name="link">连接符号</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(StringBuilder append, bool isUpper, char link)
        {
            using (StringWriter strwr = new StringWriter(append))
            {
                f_ToText(strwr, isUpper, link);
            }
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <param name="isUpper">字母是否大写</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(StringBuilder append, bool isUpper)
        {
            ToText(append, isUpper, '-');
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(StringBuilder append)
        {
            ToText(append, false, '-');
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <param name="isUpper">字母是否大写</param>
        /// <param name="link">连接符号</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(CMStringBuilder append, bool isUpper, char link)
        {
            using (var strwr = new CMStringBuilderWriter(append))
            {
                f_ToText(strwr, isUpper, link);
            }
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <param name="isUpper">字母是否大写</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(CMStringBuilder append, bool isUpper)
        {
            ToText(append, isUpper, '-');
        }

        /// <summary>
        /// 将UUID的值以文本格式写入
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(CMStringBuilder append)
        {
            ToText(append, false, '-');
        }

        /// <summary>
        /// 返回UUID格式的字符串
        /// </summary>
        /// <param name="isUpper">字母是否大写</param>
        /// <returns>UUID格式字符串</returns>
        public string ToString(bool isUpper)
        {
            StringBuilder sb = new StringBuilder(20);
            ToText(sb, isUpper);
            return sb.ToString();
        }

        #endregion

        #region 字节数组转换

        private void f_toByteBuf(byte* buffer)
        {
            // 前4字节
            buffer[0] = (byte)((i32_1) & 0xFF);
            buffer[1] = (byte)((i32_1 >> 8) & 0xFF);
            buffer[2] = (byte)((i32_1 >> 16) & 0xFF);
            buffer[3] = (byte)((i32_1 >> 24) & 0xFF);

            // 字段2 前2字节
            buffer[4] = (byte)((i32_2) & 0xFF);
            buffer[5] = (byte)((i32_2 >> 8) & 0xFF);
            // 后2字节
            buffer[6] = (byte)((i32_2 >> 16) & 0xFF);
            buffer[7] = (byte)((i32_2 >> 24) & 0xFF);

            // 字段3 前2字节
            buffer[8] = (byte)((i64) & 0xFF);
            buffer[9] = (byte)((i64 >> 8) & 0xFF);

            buffer[10] = (byte)((i64 >> (8 * 2)) & 0xFF);
            buffer[11] = (byte)((i64 >> (8 * 3)) & 0xFF);
            buffer[12] = (byte)((i64 >> (8 * 4)) & 0xFF);
            buffer[13] = (byte)((i64 >> (8 * 5)) & 0xFF);
            buffer[14] = (byte)((i64 >> (8 * 6)) & 0xFF);
            buffer[15] = (byte)((i64 >> (8 * 7)) & 0xFF);
        }

        /// <summary>
        /// 将uuid转换到以大端顺序存储的字节序列
        /// </summary>
        /// <param name="buffer">指向要写入的字节序列首地址的指针，需保证地址至少有16个可用字节</param>
        public void ToByteBuffer(CPtr<byte> buffer)
        {
            if (buffer.IsEmpty) throw new ArgumentNullException();
            f_toByteBuf(buffer);
        }

        /// <summary>
        /// 将uuid转换到以大端顺序存储的字节序列
        /// </summary>
        /// <param name="buffer">要写入的字节数组</param>
        /// <param name="offset">要写入的起始字节偏移</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        public void ToBytes(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset + 16 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* p = buffer)
            {
                f_toByteBuf(p + offset);
            }
        }

        /// <summary>
        /// 将uuid转换到以大端顺序存储的字节序列
        /// </summary>
        /// <param name="buffer">要写入的字节数组</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToBytes(byte[] buffer)
        {
            ToBytes(buffer, 0);
        }

        /// <summary>
        /// 将uuid转换到以大端顺序存储的字节序列
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] bs = new byte[16];
            ToBytes(bs, 0);
            return bs;
        }

        private static Uuid f_createToBytes(byte* buffer)
        {
            uint i32_1, i32_2;
            ulong i64;
            i32_1 = (((uint)buffer[0])) |
            (((uint)buffer[1]) << 8) |
            (((uint)buffer[2]) << 16) |
            (((uint)buffer[3]) << 24)
            ;

            i32_2 = (((uint)buffer[4])) |
            (((uint)buffer[5]) << 8) |
            (((uint)buffer[6]) << 16) |
            (((uint)buffer[7]) << 24)
            ;

            i64 = (((ulong)buffer[8])) |
            (((ulong)buffer[9])     << 8) |
            (((ulong)buffer[10])    << (8 * 2)) |
            (((ulong)buffer[11])    << (8 * 3)) |
            (((ulong)buffer[12])    << (8 * 4)) |
            (((ulong)buffer[13])    << (8 * 5)) |
            (((ulong)buffer[14])    << (8 * 6)) |
            (((ulong)buffer[15])    << (8 * 7))
            ;
            return new Uuid(i32_1, i32_2, i64);
        }

        /// <summary>
        /// 从字节序列内存创建uuid
        /// </summary>
        /// <param name="buffer">指向表示uuid字节序内存地址的指针，需保证地址至少有16个可用字节</param>
        /// <returns>一个uuid值</returns>
        public static Uuid CreateFromByteBuffer(CPtr<byte> buffer)
        {
            if (buffer.IsEmpty) throw new ArgumentNullException();
            return f_createToBytes(buffer);
        }

        /// <summary>
        /// 从字节数组创建uuid
        /// </summary>
        /// <param name="buffer">表示uuid的字节数组</param>
        /// <param name="offset">要读取的起始字节偏移</param>
        /// <returns>一个uuid值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">范围小于16个字节</exception>
        public static Uuid CreateFromBytes(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset + 16 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* p = buffer)
            {
                return f_createToBytes(p + offset);
            }
        }

        /// <summary>
        /// 从字节数组创建uuid
        /// </summary>
        /// <param name="buffer">表示uuid的字节数组</param>
        /// <returns>一个uuid值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字节数组容量小于16个字节</exception>
        public static Uuid CreateFromBytes(byte[] buffer)
        {
            return CreateFromBytes(buffer, 0);
        }

        #endregion

        #region 从文本读取

        public static bool f_createToStrBuf(char* str, out Uuid uuid)
        {
            char* cbuf = stackalloc char[32];
            byte* buf = stackalloc byte[16];
            int of = 0;
            int i;
            //xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
            // 去连接符
            for (i = 0; i < 36; i++)
            {
                if(i == 8 || i == 13 || i == 18 || i == 23)
                {
                    continue;
                }
                cbuf[of] = str[i];
                of++;
            }
            byte tb;
            of = 0;
            i = 0;
            for (; i < 32; )
            {
                if(TextManipulation.X16TextToByte(cbuf[i], cbuf[i + 1], out tb))
                {
                    buf[of] = tb;
                }
                else
                {
                    uuid = default;
                    return false;
                }
                of++;
                i += 2;
            }

            uuid = f_createToBytes(buf);
            return true;
        }

        /// <summary>
        /// 从字符串创建uuid
        /// </summary>
        /// <param name="str">指向表示uuid的字符串指针，该指针必须拥有至少36个可用字符</param>
        /// <param name="uuid">读取到的值；如果返回值是false，该值无效</param>
        /// <returns>是否成功读取</returns>
        public static bool TryCreateFromStringBuffer(CPtr<char> str, out Uuid uuid)
        {
            if (str.IsEmpty)
            {
                uuid = default;
                return false;
            }
            return f_createToStrBuf(str, out uuid);
        }

        /// <summary>
        /// 从字符串创建uuid
        /// </summary>
        /// <param name="value">表示uuid的字符串</param>
        /// <param name="startIndex">要读取的第一个字符索引</param>
        /// <param name="uuid">读取到的值；如果返回值是false，该值无效</param>
        /// <returns>是否成功读取</returns>
        public static bool TryCreateFromString(string value, int startIndex, out Uuid uuid)
        {
            uuid = default;
            if (value is null) return false;
            if (startIndex < 0 || startIndex + 36 > value.Length) return false;
            fixed (char* p = value)
            {
                if (!f_createToStrBuf(p + startIndex, out uuid))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 从字符串创建uuid
        /// </summary>
        /// <param name="value">表示uuid的字符串</param>
        /// <param name="startIndex">要读取的第一个字符索引</param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        /// <exception cref="FormatException">UUID格式错误</exception>
        public static Uuid CreateFromString(string value, int startIndex)
        {
            if (value is null) throw new ArgumentNullException();
            if (startIndex < 0 || startIndex + 36 > value.Length) throw new ArgumentOutOfRangeException();
            Uuid re;
            fixed (char* p = value)
            {
                if(f_createToStrBuf(p + startIndex, out re))
                {
                    return re;
                }
            }
            throw new FormatException();
        }

        /// <summary>
        /// 从字符串创建uuid
        /// </summary>
        /// <param name="value">表示uuid的字符串</param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="FormatException">UUID格式错误</exception>
        public static Uuid CreateFromString(string value)
        {
            return CreateFromString(value, 0);
        }

        /// <summary>
        /// 从字符数组读取UUID字符串创建uuid
        /// </summary>
        /// <param name="buffer">表示uuid文本的字符数组</param>
        /// <param name="index">要读取的第一个字符索引</param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        /// <exception cref="FormatException">UUID格式错误</exception>
        public static Uuid CreateFromCharAray(char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 36 > buffer.Length) throw new ArgumentOutOfRangeException();
            Uuid re;
            fixed (char* p = buffer)
            {
                if (f_createToStrBuf(p + index, out re))
                {
                    return re;
                }
            }
            throw new FormatException();
        }

        /// <summary>
        /// 从字符数组读取UUID字符串创建uuid
        /// </summary>
        /// <param name="buffer">表示uuid文本的字符数组</param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="FormatException">UUID格式错误</exception>
        public static Uuid CreateFromCharAray(char[] buffer)
        {
            return CreateFromCharAray(buffer, 0);
        }

        #endregion

        #region 其它创建方式

        /// <summary>
        /// 从指定随机字节序列生成器创建一个uuid
        /// </summary>
        /// <param name="random"></param>
        /// <returns>随机的uuid</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static Uuid CreateFromRandom(IRandomGenerateBytes random)
        {
            if (random is null) throw new ArgumentNullException();
            byte* buf = stackalloc byte[16];
            random.NextPtr(buf, 16);
            return f_createToBytes(buf).SetToUUIDv4();
        }

        /// <summary>
        /// 将<see cref="Hash128">128位哈希值</see>转换到uuid
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static Uuid CreateFromHash128(Hash128 hash)
        {
            byte* p = stackalloc byte[16];
            hash.ToBytes(p);
            return f_createToBytes(p);
        }

        /// <summary>
        /// 将uuid转换到<see cref="Hash128">128位哈希值</see>
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static Hash128 ToHash128(Uuid uuid)
        {
            byte* p = stackalloc byte[16];
            uuid.f_toByteBuf(p);
            return Hash128.BytesToHash(p);
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 返回UUID格式的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(20);
            ToText(sb);
            return sb.ToString();
        }

        /// <summary>
        /// 比较两值相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Uuid left, Uuid right)
        {
            return left.i32_1 == right.i32_1 && left.i32_2 == right.i32_2 && left.i64 == right.i64;
        }

        /// <summary>
        /// 比较两值不相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Uuid left, Uuid right)
        {
            return left.i32_1 != right.i32_1 || left.i32_2 != right.i32_2 || left.i64 != right.i64;
        }

        public bool Equals(Uuid other)
        {
            return i32_1 == other.i32_1 && i32_2 == other.i32_2 && i64 == other.i64;
        }

        public override bool Equals(object obj)
        {
            if (obj is Uuid other) return this == other; return false;
        }

        public override int GetHashCode()
        {
            return GetHashCode64().GetHashCode();
        }

        public long GetHashCode64()
        {
            return (long)((((ulong)i32_1) | (((ulong)i32_1) << 32)) ^ i64);
        }

        #endregion

    }

}
